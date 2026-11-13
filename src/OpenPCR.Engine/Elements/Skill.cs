using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;

namespace Elements
{
	[Serializable]
	public class Skill : SkillBase
	{
		public List<SkillBase> OverrideSkillList = new List<SkillBase>();
		[global::OpenPCR.Migration("5ef9ea7f-a994-4100-a2b0-d2eb2ad335e4")]
		public Skill()
		{
		}

		[global::OpenPCR.Migration("59c21b6f-7763-4a06-ab47-ac8f91d226e5")]
		public Skill(SkillBase _skillBase)
		{
			IsPrincessForm = _skillBase.IsPrincessForm;
			PrincessSkillMovieDataList = _skillBase.PrincessSkillMovieDataList;
			ActionParameters = _skillBase.ActionParameters;
			ActionParametersOnPrefab = _skillBase.ActionParametersOnPrefab;
			SkillEffects = _skillBase.SkillEffects;
			ShakeEffects = _skillBase.ShakeEffects;
			BlurEffects = _skillBase.BlurEffects;
			ZoomEffect = _skillBase.ZoomEffect;
			SlowEffect = _skillBase.SlowEffect;
			ForcePlayNoTarget = _skillBase.ForcePlayNoTarget;
			ParameterTarget = _skillBase.ParameterTarget;
			ChangeSortDatas = _skillBase.ChangeSortDatas;
			ScaleChangers = _skillBase.ScaleChangers;
			CastTime = _skillBase.CastTime;
			UnionBurstCoolDownTime = _skillBase.UnionBurstCoolDownTime;
			SkillId = _skillBase.SkillId;
			skillAreaWidth = _skillBase.skillAreaWidth;
			AnimId = _skillBase.AnimId;
			Cancel = _skillBase.Cancel;
			ProtectParts = _skillBase.ProtectParts;
			DisableOtherCharaOnStart = _skillBase.DisableOtherCharaOnStart;
			SkillNum = _skillBase.SkillNum;
			effectObjs = _skillBase.EffectObjs;
			loopEffectObjs = _skillBase.LoopEffectObjs;
			HasParentIndexes = _skillBase.HasParentIndexes;
			BlackOutTime = _skillBase.BlackOutTime;
			BlackoutEndWithMotion = _skillBase.BlackoutEndWithMotion;
			ForceComboDamage = _skillBase.ForceComboDamage;
			BlackoutColor = _skillBase.BlackoutColor;
			CutInMovieFadeStartTime = _skillBase.CutInMovieFadeStartTime;
			CutInMovieFadeDurationTime = _skillBase.CutInMovieFadeDurationTime;
			CutInSkipTime = _skillBase.CutInSkipTime;
			ForceCutinOff = _skillBase.ForceCutinOff;
			Level = _skillBase.Level;
			SkillName = _skillBase.SkillName;
			OwnerReturnPosition = _skillBase.OwnerReturnPosition;
			IsModeChange = _skillBase.IsModeChange;
			SkillMotionType = _skillBase.SkillMotionType;
			TrackDamageNum = _skillBase.TrackDamageNum;
			WeaponType = _skillBase.WeaponType;
			AccumulatedTpRecovery = _skillBase.AccumulatedTpRecovery;
			AccumulatedLifeSteal = _skillBase.AccumulatedLifeSteal;
			UseAccumulatedDamage = _skillBase.UseAccumulatedDamage;
			PauseStopState = _skillBase.PauseStopState;
			DefeatEnemyCount = _skillBase.DefeatEnemyCount;
			DefeatByThisSkill = _skillBase.DefeatByThisSkill;
			AlreadyAddAttackSelfSeal = _skillBase.AlreadyAddAttackSelfSeal;
			AlreadyExexActionByHit = _skillBase.AlreadyExexActionByHit;
			AlreadyAddAttackSealForAllEnemy = _skillBase.AlreadyAddAttackSealForAllEnemy;
			LifeSteal = _skillBase.LifeSteal;
			CountBlind = _skillBase.CountBlind;
			CountBlindType = _skillBase.CountBlindType;
			EffectBranchId = _skillBase.EffectBranchId;
			BranchIds = _skillBase.BranchIds;
			HasAttack = _skillBase.HasAttack;
			LoopEffectAlreadyDone = _skillBase.LoopEffectAlreadyDone;
			DamagedPartsList = _skillBase.DamagedPartsList;
			CriticalPartsList = _skillBase.CriticalPartsList;
			TotalDamage = _skillBase.TotalDamage;
			AweValue = _skillBase.AweValue;
			IsLifeStealEnabled = _skillBase.IsLifeStealEnabled;
			AbsorberValue = _skillBase.AbsorberValue;
			SpeedUpWithCutinVoice = _skillBase.SpeedUpWithCutinVoice;
			SpeedUpNoCutinVoice = _skillBase.SpeedUpNoCutinVoice;
			NormalWithCutinVoice = _skillBase.NormalWithCutinVoice;
			NormalNoCutinVoice = _skillBase.NormalNoCutinVoice;
			BonusId = _skillBase.BonusId;
		}
	}
}
