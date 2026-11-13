using global::OpenPCR.Runtime.Api;
using Elements.Battle;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Elements.Data;

namespace Elements.Battle.Core
{
	public class ActionParameter : ISingletonField
	{
		public delegate void OnDamageHitDelegate(long damage);
		public delegate void OnActionEndDelegate();
		private static Yggdrasil<ActionParameter> staticSingletonTree;
		private static IBattleManagerForActionParameter staticBattleManager;
		private static IBattleEffectPool staticBattleEffectPool;
		private static IBattleLog staticBattleLog;
		public eActionType ActionType;
		public OnActionEndDelegate OnActionEnd;
		public bool IsSearchAndSorted;
		public bool IsAlwaysChargeEnegry;
		[global::OpenPCR.ImplicitMigration("f52d6e5c-b28b-44f5-b679-511d0b9ae2de")][global::OpenPCR.Migration("3d209a21-5081-476c-b3b2-f13246bb8394")] protected IBattleManagerForActionParameter battleManager => staticBattleManager;
		public eTargetAssignment TargetAssignment { get; set; }
		public int TargetNum { get; set; }
		public int ActionDetail1 { get; set; }
		public int ActionDetail2 { get; set; }
		public int ActionDetail3 { get; set; }
		public int ActionId { get; set; }
		public List<BasePartsData> TargetList { get; set; }
		public Dictionary<eValueNumber, double> Value { get; set; }
		public List<int> ActionChildrenIndexes { get; set; }
		public List<NormalSkillEffect> ActionEffectList { get; set; }
		public AbnormalStateFieldAction AbnormalStateFieldAction { get; set; }
		[global::OpenPCR.Migration("115b232e-bee6-4dcb-81d2-ed775e66c58e")] public bool IsEnvironmentAbnormalState { [global::OpenPCR.Migration("04260fa8-3c4c-4a24-8120-1a0d5f305eae")] get; [global::OpenPCR.Migration("9bc59f5f-70cf-4786-8b55-5620d768c07c")] set; }
		public bool CancelByIfForAll { get; set; }
		public int ContinuousTargetCount { get; set; }
		public AnimationCurve KnockAnimationCurve { get; set; }
		public Dictionary<BasePartsData, bool> HitOnceDic { get; set; }
		public Dictionary<UnitCtrl, Dictionary<int, ActionExecedData>> AlreadyExecedData { get; set; }
		public Dictionary<UnitCtrl, List<int>> AlreadyExecedKeys { get; set; }
		public List<BasePartsData> HitOnceKeyList { get; set; }
		public Dictionary<eValueNumber, double> AdditionalValue { get; set; }
		public Dictionary<eValueNumber, double> MultipleValue { get; set; }
		public Dictionary<eValueNumber, double> DivideValue { get; set; }
		public MasterSkillAction.SkillAction MasterData { get; set; }
		public bool IgnoreDecoy { get; set; }
		public bool TriggerCall { get; set; } = true;
		public int SourceFieldIndex { get; set; } = -1;
		public Dictionary<BasePartsData, List<CriticalData>> CriticalDataDictionary { get; set; } = new Dictionary<BasePartsData, List<CriticalData>>();
		public global::System.Collections.Generic.Dictionary<global::Elements.eStateIconType, global::System.Collections.Generic.List<global::Elements.UnitCtrl>> UsedChargeEnergyByReceiveDamage { get; set; } = new Dictionary<eStateIconType, List<UnitCtrl>>();
		public global::System.Collections.Generic.Dictionary<global::Elements.BasePartsData, long> LimitDamageDictionaryAtk { get; set; } = new Dictionary<BasePartsData, long>();
		public global::System.Collections.Generic.Dictionary<global::Elements.BasePartsData, long> LimitDamageDictionaryMgc { get; set; } = new Dictionary<BasePartsData, long>();
		public Dictionary<BasePartsData, long> TotalDamageDictionary { get; set; } = new Dictionary<BasePartsData, long>();
		public global::System.Collections.Generic.Dictionary<global::Elements.UnitCtrl, double> AllSealDamageCutExecValueDic { get; set; } = new Dictionary<UnitCtrl, double>();
		public double AllSealDamageCutValueForCountZero { get; set; } = -1.0;
		public List<UnitCtrl> ExecedMultiBossList { get; set; } = new List<UnitCtrl>();
		public global::System.Collections.Generic.HashSet<global::Elements.Battle.Core.TriggerAction> AlreadyTriggeredActions { get; set; } = new HashSet<TriggerAction>();
		public global::System.Collections.Generic.Dictionary<global::Elements.BasePartsData, double> ReductionDamageAtkDictionary { get; set; } = new Dictionary<BasePartsData, double>();
		public global::System.Collections.Generic.Dictionary<global::Elements.BasePartsData, double> ReductionDamageMgcDictionary { get; set; } = new Dictionary<BasePartsData, double>();
		public global::System.Collections.Generic.Dictionary<global::Elements.BasePartsData, double> ReductionDamageAllDictionary { get; set; } = new Dictionary<BasePartsData, double>();
		public global::System.Collections.Generic.Dictionary<global::Elements.BasePartsData, double> ReductionDamageAtkDictionaryForEnergy { get; set; } = new Dictionary<BasePartsData, double>();
		public global::System.Collections.Generic.Dictionary<global::Elements.BasePartsData, double> ReductionDamageMgcDictionaryForEnergy { get; set; } = new Dictionary<BasePartsData, double>();
		public global::System.Collections.Generic.Dictionary<global::Elements.BasePartsData, double> ReductionDamageAllDictionaryForEnergy { get; set; } = new Dictionary<BasePartsData, double>();

		public double EnergyChargeMultiple = 1.0;
		protected List<SkillEffectCtrl> changePatternCurrentSkillEffect = new List<SkillEffectCtrl>();
		[global::OpenPCR.Migration("57a614b5-1f35-45b7-8257-1284294a230c")]
		public ActionParameter() : base()
		{
			if (staticSingletonTree == null)
			{
				staticSingletonTree = this.CreateSingletonTree();
				staticBattleManager = staticSingletonTree.Get<BattleManager>();
				staticBattleEffectPool = staticSingletonTree.Get<BattleEffectPool>();
				staticBattleLog = staticSingletonTree.Get<BattleLog>();
			}
		}

		[global::OpenPCR.Migration("504a3257-1ea1-4cd1-a68f-10880285336b")]
		public static void StaticRelease()
		{
			staticSingletonTree = null;
			staticBattleManager = null;
			staticBattleEffectPool = null;
			staticBattleLog = null;
		}

		[global::OpenPCR.Migration("4c64c0c0-1c82-4a43-9a85-8a44b59aeb47")]
		public virtual void Initialize()
		{
			HitOnceDic = new Dictionary<BasePartsData, bool>();
			HitOnceKeyList = new List<BasePartsData>();
			AlreadyExecedData = new Dictionary<UnitCtrl, Dictionary<int, ActionExecedData>>();
			AlreadyExecedKeys = new Dictionary<UnitCtrl, List<int>>();
		}

		[global::OpenPCR.Migration("8754d11b-e8d3-4c05-889e-255ad729a6f1")]
		public virtual void Initialize(UnitCtrl _ownerUnitCtrl)
		{
			Initialize();
		}

		[global::OpenPCR.Migration("358ab6cf-a823-4f2c-b2c6-71e990293cee")]
		public virtual void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
		}

		[global::OpenPCR.Migration("2f343267-f6d7-454c-898e-a9a743ee64a1")]
		public virtual void ExecActionOnWaveStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
		}

		[global::OpenPCR.Migration("c9785001-7cee-45a1-8d90-51b120d7c3cb")]
		public virtual void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
		}

		[global::OpenPCR.Migration("08399756-59cc-460e-b862-dff46d1b7a78")]
		public virtual void SetLevel(int _level)
		{
		}

		[global::OpenPCR.Migration("d72b6eea-f2c2-45bd-b225-c6729a2ff645")]
		public virtual void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			AllSealDamageCutExecValueDic.Clear();
			AlreadyTriggeredActions.Clear();
			ReductionDamageAtkDictionary.Clear();
			ReductionDamageMgcDictionary.Clear();
			ReductionDamageAllDictionary.Clear();
			ReductionDamageAtkDictionaryForEnergy.Clear();
			ReductionDamageMgcDictionaryForEnergy.Clear();
			ReductionDamageAllDictionaryForEnergy.Clear();
			ExecedMultiBossList.Clear();
			AllSealDamageCutValueForCountZero = -1.0;
			ResetHitData();
		}

		[global::OpenPCR.Migration("db225dfc-e664-4d74-9302-a288f52b1f43")]
		public void ResetHitData()
		{
			for (int i = 0; i < HitOnceKeyList.Count; i++)
			{
				HitOnceDic[HitOnceKeyList[i]] = true;
			}

			Dictionary<UnitCtrl, Dictionary<int, ActionExecedData>>.Enumerator enumerator = AlreadyExecedData.GetEnumerator();
			while (enumerator.MoveNext())
			{
				for (int j = 0; j < AlreadyExecedKeys[enumerator.Current.Key].Count; j++)
				{
					ActionExecedData actionExecedData = AlreadyExecedData[enumerator.Current.Key][AlreadyExecedKeys[enumerator.Current.Key][j]];
					actionExecedData.AlreadyExeced = false;
					actionExecedData.ExecedPartsNumber = 0;
					actionExecedData.TargetPartsNumber = 0;
				}
			}
		}

		[global::OpenPCR.Migration("96f9e190-7256-4418-84d5-54b739427fa6")]
		public AbnormalStateEffectPrefabData CreateAbnormalEffectData()
		{
			AbnormalStateEffectPrefabData result = null;
			if (ActionEffectList.Count > 0)
			{
				result = new AbnormalStateEffectPrefabData
				{
					IsHead = (ActionEffectList[0].TargetBone == eTargetBone.HEAD),
					LeftEffectPrefab = ActionEffectList[0].PrefabLeft,
					RightEffectPrefab = ActionEffectList[0].Prefab
				};
			}

			return result;
		}

		[global::OpenPCR.Migration("66f126d0-899a-4c88-bdce-803c46d7b6dc")]
		public bool JudgeIsAlreadyExeced(UnitCtrl _target, int _num)
		{
			return !AlreadyExecedData[_target][_num].AlreadyExeced;
		}

		[global::OpenPCR.Migration("210ddf8b-f86f-400e-87b5-0b1192748d3b")]
		public bool IsAppendTargetNum(UnitCtrl _target)
		{
			return AlreadyExecedData.ContainsKey(_target);
		}

		[global::OpenPCR.Migration("7e8c9097-cfac-49ab-a6e0-8cf4f4a6a46e")]
		public void AppendTargetNum(UnitCtrl _target, int _num)
		{
			if (!AlreadyExecedData.ContainsKey(_target))
			{
				AlreadyExecedData.Add(_target, new Dictionary<int, ActionExecedData> { { _num, new ActionExecedData { TargetPartsNumber = 1 } } });
				AlreadyExecedKeys.Add(_target, new List<int> { _num });
			}
			else if (!AlreadyExecedData[_target].ContainsKey(_num))
			{
				AlreadyExecedData[_target].Add(_num, new ActionExecedData { TargetPartsNumber = 1 });
				AlreadyExecedKeys[_target].Add(_num);
			}
			else
			{
				AlreadyExecedData[_target][_num].TargetPartsNumber++;
			}
		}

		[global::OpenPCR.Migration("081deafa-5926-48fd-9038-749f33af5bef")]
		public void AppendIsAlreadyExeced(UnitCtrl _target, int _num)
		{
			if (!SkipAppendIsAlreadyExeced)
				AlreadyExecedData[_target][_num].AlreadyExeced = true;
		}

		[global::OpenPCR.Migration("6be8cc0d-f292-4416-9f70-10fd08838998")]
		public bool JudgeLastAndNotExeced(UnitCtrl _target, int _num)
		{
			ActionExecedData actionExecedData = AlreadyExecedData[_target][_num];
			actionExecedData.ExecedPartsNumber++;
			if (actionExecedData.AlreadyExeced)
			{
				return false;
			}

			return actionExecedData.ExecedPartsNumber == actionExecedData.TargetPartsNumber;
		}

		[global::OpenPCR.Migration("ab68f8be-1422-4408-b9c5-2edd170288fd")]
		public virtual bool IsIgnoreDebuffImmune()
		{
			return false;
		}

		[global::OpenPCR.Migration("342b6e1c-aab5-4c21-a765-d61437e3f600")]
		public bool IsChangeSpeedAction()
		{
			return (ActionType == eActionType.FORCE_CHANGE_SPEED || ActionType == eActionType.CHANGE_SPEED) && unchecked((uint)(ActionDetail1 - 1)) < 2u;
		}

		[global::OpenPCR.Migration("101231ca-dc49-438f-bc05-4bc0cb78b328")]
		public virtual double GetActionBaseValue(UnitCtrl _source, eValueNumber _type)
		{
			double value = 0.0;
			Value.TryGetValue(_type, out value);
			return value;
		}

		[global::OpenPCR.Migration("6c3063c7-c758-4065-ba48-1f3c085c7c4c")]
		protected global::Elements.Battle.IBattleEffectPool battleEffectPool
		{
			[global::OpenPCR.Migration("3a76ee5a-b66d-4611-8931-a3c016375b08")]
			get
			{
				return staticBattleEffectPool;
			}
		}

		[global::OpenPCR.Migration("ec2105eb-2242-428d-8932-63ae8bdbcf31")]
		protected global::Elements.Battle.IBattleLog battleLog
		{
			[global::OpenPCR.Migration("7994eaa3-223b-42a7-854a-e9f5816b8383")]
			get
			{
				return staticBattleLog;
			}
		}

		public global::Elements.Battle.Core.PriorityPattern TargetSort { get; set; }
		public int TargetNth { get; set; }
		public float TargetWidth { get; set; }
		public global::Elements.Battle.Core.DirectionType Direction { get; set; }
		public float Position { get; set; }
		public bool IsExEquipSkillAction { get; set; }
		public float[] ExecTime { get; set; }
		public global::System.Collections.Generic.List<global::Elements.ActionExecTime> ActionExecTimeList { get; set; }
		public double ActionWeightSum { get; set; }
		public int DepenedActionId { get; set; }
		public bool IsExcludedFromBlackOutTarget { get; set; }
		public bool ReferencedByEffect { get; set; }
		public bool ReferencedByReflection { get; set; }
		public global::Elements.Battle.Core.ActionParameter.OnDamageHitDelegate OnDamageHit { get; set; }
		public bool IsFlightStateTargetByReflection { get; set; }
		public global::System.Collections.Generic.List<global::Elements.NormalSkillEffect> ActionSubEffectList { get; set; }
		public global::Elements.Battle.Core.eAbnormalState EnvironmentTargetAbnormalState { get; set; }
		public global::System.Action OnDefeatEnemy { get; set; }
		public global::System.Action OnInitWhenNoTarget { get; set; }
		public bool NoDelayExecByTriggerOnly { get; set; }
		public global::System.Collections.Generic.Dictionary<global::Elements.BasePartsData, long> IdOffsetDictionary { get; set; }
		public global::OpenPCR.Runtime.Api.GameObject SummonPrefab { get; set; }
		public global::OpenPCR.Runtime.Api.AnimationCurve KnockDownAnimationCurve { get; set; }
		public bool SkipAppendIsAlreadyExeced { get; set; }
		public global::Elements.Battle.Core.eEffectType EffectType { get; set; }
		public global::Elements.BasePartsData ProtectedParts { get; set; }
		public bool IgnoreTriggerCausedByTrigger { get; set; }
	}
}
