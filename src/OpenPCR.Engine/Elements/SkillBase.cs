using global::Elements;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Elements.Battle.Core;

namespace Elements
{
	[Serializable]
	public class SkillBase
	{
		private List<ActionParameter> actionParameters;
		public bool IsPrincessForm;
		public List<PrincessSkillMovieData> PrincessSkillMovieDataList;
		public List<ActionParameterOnPrefab> ActionParametersOnPrefab;
		public List<NormalSkillEffect> SkillEffects;
		public List<ShakeEffect> ShakeEffects;
		public List<BlurEffect.BlurEffectData> BlurEffects;
		public BattleDefine.ZoomEffect ZoomEffect;
		public BattleDefine.SlowEffect SlowEffect;
		public bool ForcePlayNoTarget;
		public int ParameterTarget;
		public List<ChangeSortOrderData> ChangeSortDatas;
		public List<ScaleChanger> ScaleChangers;
		[NonSerialized]
		public int SkillId;
		[NonSerialized]
		public float skillAreaWidth;
		[NonSerialized]
		public eAnimKitCharacterAnimeId AnimId;
		[NonSerialized]
		public bool Cancel;
		public bool DisableOtherCharaOnStart;
		protected List<SkillEffectCtrl> effectObjs;
		protected List<SkillEffectCtrl> loopEffectObjs;
		public float BlackOutTime;
		public bool BlackoutEndWithMotion;
		public bool ForceComboDamage;
		public Color BlackoutColor;
		public float CutInMovieFadeStartTime;
		public float CutInMovieFadeDurationTime;
		public float CutInSkipTime;
		public bool ForceCutinOff;
		[NonSerialized]
		public int Level;
		[NonSerialized]
		public string SkillName;
		public SkillMotionType SkillMotionType;
		public bool TrackDamageNum;
		[NonSerialized]
		public SystemIdDefine.eWeaponSeType WeaponType;
		public bool PauseStopState;
		public List<int> BranchIds;
		public SubUbVoiceTimingSet SpeedUpWithCutinVoice;
		public SubUbVoiceTimingSet SpeedUpNoCutinVoice;
		public SubUbVoiceTimingSet NormalWithCutinVoice;
		public SubUbVoiceTimingSet NormalNoCutinVoice;
		public List<ActionParameter> ActionParameters
		{
			[global::OpenPCR.Migration("980d5b69-917c-490e-ab5f-4f643c2c5335")]
			get
			{
				return actionParameters;
			}

			[global::OpenPCR.Migration("515b8543-0f22-4096-9e4e-8dbbdeea5996")]
			set
			{
				actionParameters = value;
			}
		}

		public int SkillNum { get; set; }

		public List<SkillEffectCtrl> EffectObjs
		{
			[global::OpenPCR.Migration("d1d0b057-5fc7-4b41-b087-bc5138b5b65b")]
			get
			{
				return effectObjs;
			}
		}

		public bool IsModeChange { get; set; }
		public bool UseAccumulatedDamage { get; set; }
		public bool DefeatByThisSkill { get; set; }
		public int EffectBranchId { get; set; }
		public List<BasePartsData> CriticalPartsList { get; set; }
		public int BonusId { get; set; }

		[global::OpenPCR.Migration("ce12c164-0d32-4ae7-8fbf-4f798953beaa")]
		public SkillBase()
		{
			actionParameters = new List<ActionParameter>();
			ActionParametersOnPrefab = new List<ActionParameterOnPrefab>();
			SkillEffects = new List<NormalSkillEffect>();
			ShakeEffects = new List<ShakeEffect>();
			BlurEffects = new List<BlurEffect.BlurEffectData>();
			ZoomEffect = new BattleDefine.ZoomEffect();
			SlowEffect = new BattleDefine.SlowEffect();
			ChangeSortDatas = new List<ChangeSortOrderData>();
			ScaleChangers = new List<ScaleChanger>();
			ProtectParts = new Dictionary<BasePartsData, BasePartsData>();
			effectObjs = new List<SkillEffectCtrl>();
			loopEffectObjs = new List<SkillEffectCtrl>();
			BlackoutColor = new Color32(0, 0, 0, 245);
			CutInMovieFadeStartTime = 1.9f;
			CutInMovieFadeDurationTime = 0.1f;
			TrackDamageNum = true;
			AccumulatedTpRecovery = new Dictionary<UnitCtrl, double>();
			UseAccumulatedDamage = true;
			BranchIds = new List<int>();
			IsLifeStealEnabled = true;
			SpeedUpWithCutinVoice = new SubUbVoiceTimingSet();
			SpeedUpNoCutinVoice = new SubUbVoiceTimingSet();
			NormalWithCutinVoice = new SubUbVoiceTimingSet();
			NormalNoCutinVoice = new SubUbVoiceTimingSet();
		}

		public float CastTime { get; set; }
		public float UnionBurstCoolDownTime { get; set; }
		public Dictionary<BasePartsData, BasePartsData> ProtectParts { get; set; }
		public List<int> HasParentIndexes { get; set; }
		public Vector3 OwnerReturnPosition { get; set; }
		public Dictionary<UnitCtrl, double> AccumulatedTpRecovery { get; set; }
		public long AccumulatedLifeSteal { get; set; }
		public int DefeatEnemyCount { get; set; }
		public bool AlreadyAddAttackSelfSeal { get; set; }
		public bool AlreadyExexActionByHit { get; set; }
		public bool AlreadyAddAttackSealForAllEnemy { get; set; }
		public int LifeSteal { get; set; }
		public bool CountBlind { get; set; }
		public AttackActionBase.eAttackType CountBlindType { get; set; }
		public bool HasAttack { get; set; }
		public bool LoopEffectAlreadyDone { get; set; }
		public List<BasePartsData> DamagedPartsList { get; set; }
		public long TotalDamage { get; set; }
		public double AweValue { get; set; }
		public bool IsLifeStealEnabled { get; set; }
		public long AbsorberValue { get; set; }

		public List<SkillEffectCtrl> LoopEffectObjs
		{
			[global::OpenPCR.Migration("8428bb58-5c96-45c0-8a7d-ed8905f77cc2")]
			get
			{
				return loopEffectObjs;
			}
		}

		[global::OpenPCR.Migration("21fa7b31-4682-4258-ad95-910c633f2256")]
		public Skill CloneAsSkill()
		{
			return new Skill(this);
		}

		[global::OpenPCR.Migration("d6ea6a89-d28c-48d6-bc90-d1c37cf970cc")]
		public void SetLevel(int _level)
		{
			Level = _level;
			for (int i = 0; i < actionParameters.Count; i++)
			{
				actionParameters[i].SetLevel(_level);
			}
		}

		[global::OpenPCR.Migration("fa3a6734-5fd6-48e0-8f80-229eda815a78")]
		public void ReadySkill()
		{
			TotalDamage = 0L;
			ProtectParts.Clear();
			AccumulatedTpRecovery.Clear();
			AccumulatedLifeSteal = 0L;
			DamagedPartsList.Clear();
			for (int i = 0; i < SkillEffects.Count; i++)
			{
				NormalSkillEffect effect = SkillEffects[i];
				List<UnitCtrl> keys = effect.AlreadyFireArmExecedKeys;
				for (int j = 0; j < keys.Count; j++)
				{
					effect.AlreadyFireArmExecedData[keys[j]] = false;
				}
			}
		}
	}
}
