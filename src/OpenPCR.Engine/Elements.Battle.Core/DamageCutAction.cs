using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public partial class DamageCutAction : ActionParameter
	{
		public enum eDamageCutType { ATK = 1, MGC, ALL, ATK_REDUCTION, MGC_REDUCTION, ALL_REDUCTION }
		private static readonly Dictionary<eDamageCutType, eAbnormalState> abnormalStateDic;
		[global::OpenPCR.Migration("dab33c30-7658-4b5e-8209-af6ffba3bc41")]
		static DamageCutAction()
		{
			abnormalStateDic = new Dictionary<eDamageCutType, eAbnormalState>
			{
				{ eDamageCutType.ATK, (eAbnormalState)41 }, { eDamageCutType.MGC, (eAbnormalState)42 },
				{ eDamageCutType.ALL, (eAbnormalState)43 }, { eDamageCutType.ATK_REDUCTION, (eAbnormalState)83 },
				{ eDamageCutType.MGC_REDUCTION, (eAbnormalState)84 }, { eDamageCutType.ALL_REDUCTION, (eAbnormalState)85 }
			};
		}

		[global::OpenPCR.Migration("051edc5d-5678-4cd4-830c-c78582fe2f41")]
		public DamageCutAction() : base()
		{
		}

		[global::OpenPCR.Migration("a64048a4-7629-4d5a-aad8-45238e4b0bee")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("67d8d1f2-a2e4-4d90-91bd-f637b5985158")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			_target.Owner.ComponentAbnormal.SetAbnormalState(_source, abnormalStateDic[(eDamageCutType)ActionDetail1],
				(float)_valueDictionary[eValueNumber.VALUE_3], this, _skill,
				(double)BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]), 0.0, false, false, 1.0, true, false, false);
		}

		[global::OpenPCR.Migration("01bd1cff-4f0e-4dc8-83e7-f443c46683d3")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}
	}
}
