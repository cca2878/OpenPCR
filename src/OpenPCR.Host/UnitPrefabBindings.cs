using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Elements;
using Elements.Battle.Core;
using Pcr.Prefab;
using Skill = Elements.Skill;
using SkillEffect = Pcr.Prefab.SkillEffect;
using ShakeEffect = Elements.ShakeEffect;
using ScaleChanger = Elements.ScaleChanger;

namespace OpenPCR.Host;

/// <summary>Hydrates existing serialized fields. Gameplay expansion remains in UnitActionController.Initialize.</summary>
internal sealed class UnitPrefabBindings
{
	private readonly Func<int, Pptr, GameObject> prefab;
	private readonly PrefabInputs inputs;
	public UnitPrefabBindings(Func<int, Pptr, GameObject> prefab, PrefabInputs inputs)
	{
		this.prefab = prefab ?? throw new ArgumentNullException(nameof(prefab)); this.inputs = inputs ?? throw new ArgumentNullException(nameof(inputs));
	}
	private GameObject? Reference(int id, Pptr? pointer) => pointer is null || pointer.PathId == 0 ? null : prefab(id, pointer) ?? throw new InvalidDataException("Missing authored effect resource.");
	public void Bind(UnitPrefab tree, UnitCtrl unit, UnitActionController controller)
	{
		var geometry = tree.Geometry ?? throw new InvalidDataException("Missing unit geometry.");
		Copy(geometry, unit, "UnitAppearDelay", "BossAppearDelay", "BossDeltaX", "BossDeltaY", "BossBodyWidthOffset", "Scale", "BattleCameraSize", "AllUnitCenter", "UseTargetCursorOver", "OverCursorSize", "DisableFlash", "IsForceLeftDir", "BossSortIsBack", "IsGameStartDepthBack", "OneRemainingDisableEffect");
		foreach (string name in new[] { "gameStartEffects", "dieEffects", "damageEffects", "SummonEffects", "idleEffects", "auraEffects" }) RuntimeBinding.Set(unit, name, new List<PrefabWithTime>());
		foreach (string name in new[] { "gameStartShakes", "dieShakes", "damageShakes" }) RuntimeBinding.Set(unit, name, new List<ShakeEffect>());
		foreach (var group in tree.PresetEffects)
		{
			string name = group.Slot switch { PresetEffectSlot.GameStart => "gameStartEffects", PresetEffectSlot.Die => "dieEffects", PresetEffectSlot.Damage => "damageEffects", PresetEffectSlot.Summon => "SummonEffects", PresetEffectSlot.Idle => "idleEffects", PresetEffectSlot.Aura => "auraEffects", _ => throw new InvalidDataException("Unknown preset effect slot.") };
			var values = new List<PrefabWithTime>();
			foreach (var effect in group.Effects)
				values.Add(new PrefabWithTime { Prefab = Reference(tree.PrefabId, effect.Prefab), PrefabLeft = Reference(tree.PrefabId, effect.PrefabLeft), Time = effect.Time, SummonSkillIndex = effect.SummonSkillIndex, TargetBoneName = inputs.RequireLogicBone(effect.TargetBoneName), EffectDifficlty = (PrefabWithTime.eEffectDifficulty)effect.EffectDifficulty, IsTrackRotation = effect.IsTrackRotation });
			RuntimeBinding.Set(unit, name, values);
		}
		foreach (var group in tree.UnitShakes)
		{
			string name = group.Slot switch { UnitShakeSlot.GameStartShakes => "gameStartShakes", UnitShakeSlot.DieShakes => "dieShakes", UnitShakeSlot.DamageShakes => "damageShakes", _ => throw new InvalidDataException("Unknown unit shake slot.") };
			RuntimeBinding.Set(unit, name, group.Shakes.Select(Shake).ToList());
		}
		// Prepared serialized values only. UnitCtrl.Initialize creates the actual
		// PartsData runtime copies through its existing constructor and master rows.
		unit.BossPartsList = tree.BossParts.Select(source =>
		{
			if (source.PartIndex <= 0) throw new InvalidDataException("Serialized boss part requires its original positive index.");
			var part = (PartsData)RuntimeHelpers.GetUninitializedObject(typeof(PartsData));
			Copy(source, part, "PositionX", "PositionY", "BodyWidthValue", "EnemyId", "DisableStartBreakEffect", "BreakEffectTime", "BreakEffectOffsetY", "BreakEffectSize", "ChangeAttachmentStartTime", "ChangeAttachmentEndTime");
			part.Index = source.PartIndex;
			part.AttachmentNamePairList = source.Attachments.Select(pair => new BasePartsData.AttachmentNamePair
			{
				TargetSlotName = pair.TargetSlotName,
				TargetAttachmentName = pair.TargetAttachmentName,
				AppliedSlotName = pair.AppliedSlotName,
				AppliedAttachmentName = pair.AppliedAttachmentName
			}).ToList();
			return part;
		}).ToList();
		foreach (string name in new[] { "UnionBurstList", "MainSkillList", "SpecialSkillList", "SpecialSkillEvolutionList", "UnionBurstEvolutionList", "MainSkillEvolutionList", "SubUnionBurstList" }) RuntimeBinding.Set(controller, name, new List<Skill>());
		foreach (var source in tree.Slots.Where(slot => slot.SlotName != SlotName.AttackDetail).OrderBy(slot => slot.Ordinal))
		{
			var skill = ReadSkill(tree.PrefabId, source);
			string name = source.SlotName.ToString();
			if (source.SlotName is SlotName.Attack or SlotName.Annihilation) RuntimeBinding.Set(controller, name, skill);
			else
			{
				var list = RuntimeBinding.Get(controller, name) as List<Skill> ?? throw new InvalidDataException("Unknown skill slot " + name);
				if (list.Count != source.Ordinal) throw new InvalidDataException("Skill ordinals must be contiguous: " + name);
				list.Add(skill);
			}
		}
		var detail = tree.Slots.Single(slot => slot.SlotName == SlotName.AttackDetail && slot.Ordinal == -1);
		if (!detail.HasUseDefaultDelay || detail.DirectDetails.Count != 1) throw new InvalidDataException("Incomplete attack slot.");
		controller.UseDefaultDelay = detail.UseDefaultDelay; controller.AttackDetail = ReadDetail(tree.PrefabId, detail.DirectDetails[0]);
	}
	private Skill ReadSkill(int id, SkillSlot source)
	{
		if (!source.BlurEffectsExtracted || !source.ChangeSortDatasExtracted || source.SlowEffect is null || source.ZoomEffect is null || !source.ScaleChangersExtracted) throw new InvalidDataException("Skill extraction markers are required.");
		if (!source.SlowEffect.HasTo) throw new InvalidDataException("Missing authored SlowEffect.To; re-extract unit_prefab.");
		var skill = new Skill();
		Copy(source, skill, "IsPrincessForm", "ParameterTarget", "SkillMotionType", "BlackOutTime", "CutInSkipTime", "ForcePlayNoTarget", "ForceComboDamage", "TrackDamageNum", "PauseStopState");
		skill.ShakeEffects = source.ShakeEffects.Select(Shake).ToList();
		foreach (var sourceParameter in source.Parameters)
		{
			var parameter = new ActionParameterOnPrefab(); Copy(sourceParameter, parameter, "Visible", "ActionType", "EffectType");
			parameter.IsExcludedFromBlackOutTarget = sourceParameter.IsExcludedFromBlackoutTarget;
			parameter.KnockAnimationCurve = PrefabInputs.LoadCurve(sourceParameter.KnockCurve ?? new Curve());
			parameter.KnockDownAnimationCurve = PrefabInputs.LoadCurve(sourceParameter.KnockDownCurve ?? new Curve());
			parameter.Details = sourceParameter.Details.Select(detail => ReadDetail(id, detail)).ToList(); skill.ActionParametersOnPrefab.Add(parameter);
		}
		skill.SkillEffects = source.Effects.Select(effect => ReadEffect(id, effect)).ToList();
		foreach (var child in source.OverrideSkills) skill.OverrideSkillList.Add(ReadSkill(id, child));
		skill.PrincessSkillMovieDataList = source.PrincessSkillMovieData.Select(movie => new Elements.PrincessSkillMovieData { UseFade = movie.UseFade, FadeStartTime = movie.FadeStartTime, FadeDuration = movie.FadeDuration }).ToList();
		skill.BlurEffects = source.BlurEffects.Select(blur => new BlurEffect.BlurEffectData { StartTime = blur.StartTime, Value = blur.Value, StartDuration = blur.StartDuration, Duration = blur.Duration, EndDuration = blur.EndDuration }).ToList();
		skill.ChangeSortDatas = source.ChangeSortDatas.Select(sort => new Elements.ChangeSortOrderData { SortType = (Elements.ChangeSortOrderData.eSortType)sort.SortType, Time = sort.Time }).ToList();
		Copy(source.SlowEffect, skill.SlowEffect, "Enable", "StartDelay", "StartDuration", "Duration", "EndDuration", "To");
		Copy(source.ZoomEffect, skill.ZoomEffect, "Enable", "ConsiderScreenSize", "StartDelay", "StartDuration", "Duration", "EndDuration", "To", "ZoomTarget", "OffsetY", "OffsetX", "UseYBottom", "StartEasingType", "EndEasingType", "BoneName");
		inputs.RequireLogicBone(skill.ZoomEffect.BoneName);
		foreach (var plual in source.ZoomEffect.ZoomEffectList)
		{
			var target = new BattleDefine.ZoomEffectPlual(); skill.ZoomEffect.ZoomEffectList.Add(target);
			Copy(plual, target, "StartDuration", "Duration", "To", "OffsetX", "OffsetY", "ZoomTarget", "StartEasingType", "UseYBottom", "BoneName");
			inputs.RequireLogicBone(target.BoneName);
		}
		skill.ScaleChangers = source.ScaleChangers.Select(changer =>
		{
			var target = new ScaleChanger(); Copy(changer, target, "Time", "Scale", "Duration", "Easing"); return target;
		}).ToList();
		return skill;
	}
	private ActionParameterOnPrefabDetail ReadDetail(int id, ActionDetail source)
	{
		if (!source.EffectsExtracted) throw new InvalidDataException("Action effect extraction marker is required.");
		var detail = new ActionParameterOnPrefabDetail { Visible = source.Visible, ActionId = source.ActionId };
		foreach (var item in source.ExecTimes) detail.ExecTimeForPrefab.Add(new ActionExecTime { Time = item.Time, DamageNumType = (eDamageEffectType)item.DamageNumType, Weight = item.Weight, DamageNumScale = item.DamageNumScale });
		foreach (var item in source.ExecTimeCombos) detail.ExecTimeCombo.Add(new ActionExecTimeCombo { StartTime = item.StartTime, OffsetTime = item.OffsetTime, Weight = item.Weight, Count = item.Count, InterporationType = (Elements.Battle.Core.eComboInterporationType)item.InterpolationType, Curve = PrefabInputs.LoadCurve(item.Curve ?? new Curve()) });
		detail.ActionEffectList = source.ActionEffects.Select(effect => ReadEffect(id, effect)).ToList();
		detail.ActionSubEffectList = source.ActionSubEffects.Select(effect => ReadEffect(id, effect)).ToList();
		return detail;
	}
	private NormalSkillEffect ReadEffect(int id, SkillEffect source)
	{
		var effect = new NormalSkillEffect();
		Copy(source, effect, "IsReaction", "TargetActionIsReflexive", "TargetActionIndex", "TargetActionId", "FireActionId", "TargetMotionIndex", "EffectBehavior", "EffectTarget", "TargetBone", "TrackType", "TrackDimension", "TargetBoneName", "TrackRotation", "DestroyOnEnemyDead", "CenterY", "DeltaY", "TrackTarget", "Height", "AbsoluteFireDistance", "TargetBranchId");
		inputs.RequireLogicBone(effect.TargetBoneName);
		effect.FireArmEndTarget = (eEffectTarget)source.FirearmEndTarget; effect.FireArmEndTargetBone = (eTargetBone)source.FirearmEndTargetBone;
		effect.IsAbsoluteFireArm = source.IsAbsoluteFirearm; effect.PlayWithCutIn = source.PlayWithCutin;
		effect.Prefab = Reference(id, source.Prefab); effect.PrefabLeft = Reference(id, source.PrefabLeft);
		effect.ExecTime = source.ExecTimes.ToArray(); effect.ShakeEffects = source.ShakeEffects.Select(Shake).ToList();
		effect.FireArmEndEffects = source.FireArmEndEffects.Select(child => (SkillEffectBase)ReadEffect(id, child)).ToList();
		return effect;
	}
	private static ShakeEffect Shake(Pcr.Prefab.ShakeEffect source)
	{
		// Serialized input object: constructor is not certified; runtime reset and simulation remain the real mirror bodies.
		var shake = (ShakeEffect)RuntimeHelpers.GetUninitializedObject(typeof(ShakeEffect));
		Copy(source, shake, "ShakeType", "StartTime", "Duration", "TargetMotion", "StartAmplitude", "EndAmplitude", "StartFrequency", "EndFrequency", "Rotation"); return shake;
	}
	private static void Copy(object source, object target, params string[] names)
	{
		foreach (string name in names)
		{
			var field = target.GetType().GetField(name, RuntimeBinding.Members) ?? throw new MissingFieldException(target.GetType().FullName, name);
			var input = source.GetType().GetProperty(name == "CutInSkipTime" ? "CutinSkipTime" : name)?.GetValue(source) ?? throw new InvalidDataException("Missing serialized field " + name);
			field.SetValue(target, field.FieldType.IsEnum ? Enum.ToObject(field.FieldType, input) : input);
		}
	}
}
