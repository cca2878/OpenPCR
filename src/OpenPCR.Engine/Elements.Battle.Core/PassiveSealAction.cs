using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public partial class PassiveSealAction : ActionParameter
	{
		public enum eSealTarget
		{
			SOURCE,
			TARGET
		}

		public enum ePassiveTiming
		{
			BUFF = 1,
			DAMAGED,
			DEBUFF,
			TEAM_UB
		}
		[global::OpenPCR.Migration("3d6732b6-dbfb-4a53-a20a-7cf61465fd6b")]
		public PassiveSealAction() : base()
		{
		}

		[global::OpenPCR.Migration("c484ebbd-781a-4b49-9c23-dcb1da5b45ef")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("ef0e76db-388b-4602-9783-3f2e68a0f991")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			var data = new PassiveSealData
			{
				DisplayCount = ActionDetail2 == 1,
				SealTarget = (eSealTarget)ActionDetail3,
				Source = _source,
				Target = _target.Owner,
				TargetStateIcon = (eStateIconType)global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_2]),
				SealDuration = (float)_valueDictionary[eValueNumber.VALUE_3],
				LifeTime = (float)_valueDictionary[eValueNumber.VALUE_5],
				SealNumLimit = BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]),
				SealNum = BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_7])
			};
			AppendIsAlreadyExeced(_target.Owner, _num);
			_source.AppendCoroutine(data.Update(), ePauseType.SYSTEM, null);
			_target.Owner.ComponentAbnormal.AddPassiveSeal((ePassiveTiming)ActionDetail1, data);
		}

		[global::OpenPCR.Migration("0b5c64a6-38d9-40b6-b541-7d63987871d8")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
			Value[eValueNumber.VALUE_5] = MasterData.action_value_5 + MasterData.action_value_6 * _level;
		}
	}
}
