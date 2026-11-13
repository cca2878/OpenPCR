using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using AssetsTools.NET.Extra;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Pcr.Prefab;

namespace OpenPCR.PrefabExtractor;

internal static class UnitPrefabBuilder
{
	public const string UnityVersion = "6000.0.58f2";
	private static readonly string[] SlotFields = ["Attack", "UnionBurstList", "MainSkillList", "SpecialSkillList", "SpecialSkillEvolutionList", "UnionBurstEvolutionList", "MainSkillEvolutionList", "SubUnionBurstList", "Annihilation"];
	private static T Copy<T>(Tree source, string mapping) where T : IMessage<T>, new()
	{
		var target = new T();
		foreach (string pair in mapping.Split(' ', StringSplitOptions.RemoveEmptyEntries))
		{
			string[] names = pair.Split(':');
			var field = target.Descriptor.FindFieldByName(names[0]) ?? throw new InvalidDataException(pair);
			object value = source.Required(names[1]);
			field.Accessor.SetValue(target, field.FieldType switch
			{
				FieldType.Bool => Convert.ToBoolean(value),
				FieldType.Int32 => Convert.ToInt32(value),
				FieldType.SInt64 => Convert.ToInt64(value),
				FieldType.Float => Convert.ToSingle(value),
				FieldType.Double => Convert.ToDouble(value),
				FieldType.String => (string)value,
				_ => throw new InvalidDataException("Unsupported scalar: " + field.FullName)
			});
		}
		return target;
	}
	private static Tree Defaults(Tree source, params (string, object?)[] defaults)
	{
		var copy = new Tree(); foreach (var pair in source) copy.Add(pair.Key, pair.Value);
		foreach (var (key, value) in defaults) if (!copy.ContainsKey(key) || copy[key] == null) copy[key] = value;
		return copy;
	}
	private static IEnumerable<Tree> Nodes(Tree source, string name) => source.TryGetValue(name, out var value) && value != null ? ((List<object?>)value).Cast<Tree>() : [];
	private static Pptr Pointer(Tree source, string name) => source.TryGetValue(name, out var raw) && raw is Tree p ? new Pptr { FileId = p.Int("m_FileID"), PathId = p.Long("m_PathID") } : new();
	private static Curve? Curve(Tree source, string name)
	{
		if (!source.TryGetValue(name, out var raw) || raw is not Tree curve) return null;
		var keys = Nodes(curve, "m_Curve").ToArray(); if (keys.Length == 0) keys = Nodes(curve, "m_Keys").ToArray();
		if (keys.Length == 0) return null;
		var result = Copy<Curve>(Defaults(curve, ("m_PreInfinity", 0), ("m_PostInfinity", 0), ("m_RotationOrder", 0)), "pre_infinity:m_PreInfinity post_infinity:m_PostInfinity rotation_order:m_RotationOrder");
		foreach (var key in keys) result.Keys.Add(Copy<CurveKey>(key, "time:time value:value in_slope:inSlope out_slope:outSlope in_weight:inWeight out_weight:outWeight weighted_mode:weightedMode"));
		return result;
	}
	private static ShakeEffect Shake(Tree source) => Copy<ShakeEffect>(source, "shake_type:ShakeType start_time:StartTime duration:Duration target_motion:TargetMotion start_amplitude:StartAmplitude end_amplitude:EndAmplitude start_frequency:StartFrequency end_frequency:EndFrequency rotation:Rotation");
	private static SkillEffect Effect(Tree source)
	{
		var result = Copy<SkillEffect>(Defaults(source, ("FireActionId", -1), ("TargetBoneName", "")), "is_reaction:IsReaction target_action_is_reflexive:TargetActionIsReflexive target_action_index:TargetActionIndex target_action_id:TargetActionId fire_action_id:FireActionId target_motion_index:TargetMotionIndex effect_behavior:EffectBehavior effect_target:EffectTarget target_bone:TargetBone firearm_end_target:FireArmEndTarget firearm_end_target_bone:FireArmEndTargetBone track_type:TrackType track_dimension:TrackDimension target_bone_name:TargetBoneName track_rotation:TrackRotation destroy_on_enemy_dead:DestroyOnEnemyDead center_y:CenterY delta_y:DeltaY track_target:TrackTarget height:Height is_absolute_firearm:IsAbsoluteFireArm absolute_fire_distance:AbsoluteFireDistance target_branch_id:TargetBranchId play_with_cutin:PlayWithCutIn");
		result.Prefab = Pointer(source, "Prefab");
		result.PrefabLeft = Pointer(source, "PrefabLeft");
		if (source.TryGetValue("ExecTime", out var times) && times != null) result.ExecTimes.AddRange(((List<object?>)times).Select(Convert.ToSingle));
		result.ShakeEffects.AddRange(Nodes(source, "ShakeEffects").Select(Shake));
		result.FireArmEndEffects.AddRange(Nodes(source, "FireArmEndEffects").Select(Effect));
		return result;
	}
	private static ActionDetail Detail(Tree source)
	{
		var result = Copy<ActionDetail>(source, "visible:Visible action_id:ActionId"); result.EffectsExtracted = true;
		result.ActionEffects.AddRange(Nodes(source, "ActionEffectList").Select(Effect));
		result.ActionSubEffects.AddRange(Nodes(source, "ActionSubEffectList").Select(Effect));
		foreach (var item in Nodes(source, "ExecTimeForPrefab")) result.ExecTimes.Add(Copy<ExecTime>(item, "time:Time damage_num_type:DamageNumType weight:Weight damage_num_scale:DamageNumScale"));
		foreach (var item in Nodes(source, "ExecTimeCombo"))
		{
			var combo = Copy<ExecTimeCombo>(item, "start_time:StartTime offset_time:OffsetTime weight:Weight count:Count interpolation_type:InterporationType");
			combo.Curve = Curve(item, "Curve"); result.ExecTimeCombos.Add(combo);
		}
		return result;
	}
	internal static SkillSlot Skill(Tree source, SlotName slot, int ordinal)
	{
		var result = Copy<SkillSlot>(source, "is_princess_form:IsPrincessForm parameter_target:ParameterTarget skill_motion_type:SkillMotionType black_out_time:BlackOutTime cutin_skip_time:CutInSkipTime force_play_no_target:ForcePlayNoTarget force_combo_damage:ForceComboDamage track_damage_num:TrackDamageNum pause_stop_state:PauseStopState");
		result.SlotName = slot; result.Ordinal = ordinal; result.BlurEffectsExtracted = true; result.ChangeSortDatasExtracted = true;
		foreach (var item in Nodes(source, "ActionParametersOnPrefab"))
		{
			var parameter = Copy<ActionParameter>(item, "visible:Visible action_type:ActionType effect_type:EffectType is_excluded_from_blackout_target:IsExcludedFromBlackOutTarget");
			parameter.KnockCurve = Curve(item, "KnockAnimationCurve"); parameter.KnockDownCurve = Curve(item, "KnockDownAnimationCurve");
			parameter.Details.AddRange(Nodes(item, "Details").Select(Detail)); result.Parameters.Add(parameter);
		}
		result.ShakeEffects.AddRange(Nodes(source, "ShakeEffects").Select(Shake)); result.Effects.AddRange(Nodes(source, "SkillEffects").Select(Effect));
		foreach (var item in source.Nodes("ChangeSortDatas")) result.ChangeSortDatas.Add(Copy<ChangeSortOrderData>(item, "sort_type:SortType time:Time"));
		foreach (var item in source.Nodes("BlurEffects")) result.BlurEffects.Add(Copy<BlurEffectData>(item, "start_time:StartTime value:Value start_duration:StartDuration duration:Duration end_duration:EndDuration"));
		result.SlowEffect = Copy<SlowEffect>(source.Node("SlowEffect"), "enable:Enable start_delay:StartDelay start_duration:StartDuration duration:Duration end_duration:EndDuration to:To");
		var zoom = source.Node("ZoomEffect");
		result.ZoomEffect = Copy<ZoomEffect>(zoom, "enable:Enable consider_screen_size:ConsiderScreenSize start_delay:StartDelay start_duration:StartDuration duration:Duration end_duration:EndDuration to:To zoom_target:ZoomTarget offset_y:OffsetY offset_x:OffsetX use_y_bottom:UseYBottom start_easing_type:StartEasingType end_easing_type:EndEasingType bone_name:BoneName");
		foreach (var item in zoom.Nodes("ZoomEffectList")) result.ZoomEffect.ZoomEffectList.Add(Copy<ZoomEffectPlual>(item, "start_duration:StartDuration duration:Duration to:To offset_x:OffsetX offset_y:OffsetY zoom_target:ZoomTarget start_easing_type:StartEasingType use_y_bottom:UseYBottom bone_name:BoneName"));
		foreach (var item in source.Nodes("ScaleChangers")) result.ScaleChangers.Add(Copy<ScaleChanger>(item, "time:Time scale:Scale duration:Duration easing:Easing"));
		result.ScaleChangersExtracted = true;
		foreach (var item in Nodes(source, "PrincessSkillMovieDataList")) result.PrincessSkillMovieData.Add(Copy<PrincessSkillMovieData>(item, "use_fade:UseFade fade_start_time:FadeStartTime fade_duration:FadeDuration"));
		result.OverrideSkills.AddRange(Nodes(source, "OverrideSkillList").Select((item, i) => Skill(item, SlotName.OverrideSkillList, i)));
		return result;
	}
	public static UnitPrefab Build(int prefabId, byte[] bytes, UnityBundle bundle)
	{
		Tree Component(string name)
		{
			var matches = bundle.OfType(AssetClassID.MonoBehaviour).Where(o => o.ClassName == name).Select(o => o.Tree).ToArray();
			if (matches.Length == 0) throw new InvalidDataException("Missing " + name);
			string Comparable(Tree tree) => Tree.Json(tree.Where(p => p.Key != "m_GameObject").ToDictionary(p => p.Key, p => p.Value));
			if (matches.Any(t => Comparable(t) != Comparable(matches[0]))) throw new InvalidDataException("Conflicting " + name + " roots");
			return matches[0];
		}
		Tree ctrl = Component("UnitCtrl"), controller = Component("UnitActionController");
		var result = new UnitPrefab { PrefabId = prefabId, SourceSha256 = Convert.ToHexStringLower(SHA256.HashData(bytes)), UnityVersion = UnityVersion, FormatVersion = 1 };
		result.Geometry = Copy<Geometry>(ctrl, "unit_appear_delay:UnitAppearDelay boss_appear_delay:BossAppearDelay boss_delta_x:BossDeltaX boss_delta_y:BossDeltaY boss_body_width_offset:BossBodyWidthOffset scale:Scale battle_camera_size:BattleCameraSize all_unit_center:AllUnitCenter use_target_cursor_over:UseTargetCursorOver over_cursor_size:OverCursorSize disable_flash:DisableFlash is_force_left_dir:IsForceLeftDir boss_sort_is_back:BossSortIsBack is_game_start_depth_back:IsGameStartDepthBack one_remaining_disable_effect:OneRemainingDisableEffect");
		string[] presets = ["gameStartEffects", "dieEffects", "damageEffects", "summonEffects", "idleEffects", "auraEffects"];
		for (int i = 0; i < presets.Length; i++)
		{
			var effects = Nodes(ctrl, presets[i]).ToArray(); if (effects.Length == 0) continue;
			var list = new PresetEffectList { Slot = (PresetEffectSlot)(i + 1) };
			foreach (var effect in effects)
			{
				var e = Copy<PresetEffect>(Defaults(effect, ("SummonSkillIndex", -1), ("TargetBoneName", "")), "time:Time summon_skill_index:SummonSkillIndex target_bone_name:TargetBoneName effect_difficulty:EffectDifficlty is_track_rotation:IsTrackRotation");
				e.Prefab = Pointer(effect, "Prefab");
				e.PrefabLeft = Pointer(effect, "PrefabLeft");
				list.Effects.Add(e);
			}
			result.PresetEffects.Add(list);
		}
		string[] shakes = ["gameStartShakes", "dieShakes", "damageShakes"];
		for (int i = 0; i < shakes.Length; i++)
		{
			var values = Nodes(ctrl, shakes[i]).ToArray(); if (values.Length == 0) continue;
			var list = new UnitShakeList { Slot = (UnitShakeSlot)(i + 1) }; list.Shakes.AddRange(values.Select(Shake)); result.UnitShakes.Add(list);
		}
		foreach (var part in Nodes(ctrl, "BossPartsList"))
		{
			var p = Copy<BossPart>(part, "position_x:PositionX position_y:PositionY body_width_value:BodyWidthValue part_index:Index enemy_id:EnemyId disable_start_break_effect:DisableStartBreakEffect break_effect_time:BreakEffectTime break_effect_offset_y:BreakEffectOffsetY break_effect_size:BreakEffectSize change_attachment_start_time:ChangeAttachmentStartTime change_attachment_end_time:ChangeAttachmentEndTime");
			foreach (var a in Nodes(part, "AttachmentNamePairList")) p.Attachments.Add(Copy<BossPartAttachment>(a, "target_slot_name:TargetSlotName target_attachment_name:TargetAttachmentName applied_slot_name:AppliedSlotName applied_attachment_name:AppliedAttachmentName"));
			result.BossParts.Add(p);
		}
		// AttackDetail is a detail, not an authored Skill; its zero-valued placeholder fields are explicit.
		var attack = new SkillSlot { SlotName = SlotName.AttackDetail, Ordinal = -1, UseDefaultDelay = controller.Bool("UseDefaultDelay"), BlurEffectsExtracted = true, ChangeSortDatasExtracted = true };
		attack.DirectDetails.Add(Detail(controller.Node("AttackDetail"))); result.Slots.Add(attack);
		for (int i = 0; i < SlotFields.Length; i++)
		{
			if (!controller.TryGetValue(SlotFields[i], out var raw) || raw == null) continue;
			var values = raw is List<object?> list ? list.Cast<Tree>() : [(Tree)raw];
			result.Slots.AddRange(values.Select((s, ordinal) => Skill(s, (SlotName)(i + 2), ordinal)));
		}
		return result;
	}
}
