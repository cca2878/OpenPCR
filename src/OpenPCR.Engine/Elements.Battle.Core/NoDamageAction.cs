using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public partial class NoDamageAction : ActionParameter
	{
		private enum eNoDamageType { NO_DAMAGE = 1, PHYSICS_DODGE, ALL_DODGE, ABNORMAL, DEBUFF, BREAK_POINT, NO_DAMAGE2, MAGIC_DODGE }
		private static readonly Dictionary<eNoDamageType, eAbnormalState> ABNORMAL_STATE_DIC;
		[global::OpenPCR.Migration("9028ef6b-12bb-43b6-b256-352bd8a8f0fe")]
		static NoDamageAction()
		{
			ABNORMAL_STATE_DIC = new Dictionary<eNoDamageType, eAbnormalState>
			{
				{ eNoDamageType.NO_DAMAGE, (eAbnormalState)22 }, { eNoDamageType.PHYSICS_DODGE, (eAbnormalState)30 },
				{ eNoDamageType.ABNORMAL, (eAbnormalState)23 }, { eNoDamageType.DEBUFF, (eAbnormalState)24 },
				{ eNoDamageType.BREAK_POINT, (eAbnormalState)39 }, { eNoDamageType.NO_DAMAGE2, (eAbnormalState)62 },
				{ eNoDamageType.MAGIC_DODGE, (eAbnormalState)74 }
			};
		}

		[global::OpenPCR.Migration("ec47182c-bee4-4609-ae2d-4b6efac7dcb3")]
		public NoDamageAction() : base()
		{
		}

		[global::OpenPCR.Migration("ea0ad64c-f74b-4f06-bfca-fc083a8d5026")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			switch ((eNoDamageType)ActionDetail1)
			{
				case eNoDamageType.NO_DAMAGE:
				case eNoDamageType.PHYSICS_DODGE:
				case eNoDamageType.ABNORMAL:
				case eNoDamageType.DEBUFF:
				case eNoDamageType.BREAK_POINT:
				case eNoDamageType.NO_DAMAGE2:
				case eNoDamageType.MAGIC_DODGE:
					_target.Owner.ComponentAbnormal.SetAbnormalState(_source, ABNORMAL_STATE_DIC[(eNoDamageType)ActionDetail1],
						(float)_valueDictionary[eValueNumber.VALUE_1], this, _skill, 0.0, 0.0, false, false, 1.0,
						_valueDictionary[eValueNumber.VALUE_3] == 0.0, false, false);
					break;
			}
		}

		[global::OpenPCR.Migration("8888c183-978e-475b-ad0c-e7d1e3dcb8d1")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
		}
	}
}
