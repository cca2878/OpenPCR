using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public partial class LogBarrierAction : ActionParameter
	{
		private enum eDamageCutType { ATK = 1, MGC, ALL }
		private static readonly Dictionary<eDamageCutType, eAbnormalState> abnormalStateDic;
		[global::OpenPCR.Migration("075ba481-6b70-4a16-9544-33bf1cadfe25")]
		static LogBarrierAction()
		{
			abnormalStateDic = new Dictionary<eDamageCutType, eAbnormalState>
			{
				{ eDamageCutType.ATK, (eAbnormalState)44 }, { eDamageCutType.MGC, (eAbnormalState)45 }, { eDamageCutType.ALL, (eAbnormalState)46 }
			};
		}

		[global::OpenPCR.Migration("0cd69087-80ed-4024-bd82-4c73447eb579")]
		public LogBarrierAction() : base()
		{
		}

		[global::OpenPCR.Migration("f7dfdf56-dbf7-4bd2-88e0-8312a96593c4")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("209a19cf-f75d-4057-a669-708b8fc631de")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			_target.Owner.ComponentAbnormal.SetAbnormalState(_source, abnormalStateDic[(eDamageCutType)ActionDetail1],
				(float)_valueDictionary[eValueNumber.VALUE_3], this, _skill, _valueDictionary[eValueNumber.VALUE_1],
				_valueDictionary[eValueNumber.VALUE_5], false, false, 1.0,
				BattleUtil.Approximately(_valueDictionary[eValueNumber.VALUE_6], 0.0), false, false);
		}

		[global::OpenPCR.Migration("93af70ad-eeee-464a-b7dc-169f6ef2a7aa")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}
	}
}
