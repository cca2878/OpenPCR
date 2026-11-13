using Elements.Battle;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class FearAction : ActionParameter
	{
		public enum eFearType { FEAR, ALL_FEAR }
		public static readonly Dictionary<eFearType, eAbnormalState> AbnormalStateDic = new Dictionary<eFearType, eAbnormalState> {
			{ eFearType.FEAR, eAbnormalState.FEAR }, { eFearType.ALL_FEAR, eAbnormalState.ALL_FEAR }
		};

		[global::OpenPCR.Migration("b82ff2b3-f4d0-4134-b57f-4f7004d6af62")]
		public FearAction() : base()
		{
		}

		[global::OpenPCR.Migration("d85b157d-370b-45e2-9c22-48b43a28d5d8")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			float random = Elements.Battle.BattleManager.Random(0f, 1f);
			bool hit = BattleUtil.LessThanOrApproximately(random, _valueDictionary[eValueNumber.VALUE_3]);
			random = Elements.Battle.BattleManager.Random(0f, 1f);
			if (hit && BattleUtil.LessThanOrApproximately(random, BattleUtil.CalcDodgeByLevelDiffUseParam(_skill.Level, _source, _target, IsExEquipSkillAction, 1f)))
			{
				eAbnormalState state = AbnormalStateDic[(eFearType)ActionDetail1];
				AppendIsAlreadyExeced(_target.Owner, _num);
				_target.Owner.ComponentAbnormal.SetAbnormalState(_source, state, (float)_valueDictionary[eValueNumber.VALUE_1], this, _skill);
			}
			else
			{
				ActionExecedData data = AlreadyExecedData[_target.Owner][_num];
				if (data.ExecedPartsNumber == data.TargetPartsNumber)
					_target.Owner.ComponentDamageControl.SetMissAtk(_source, eMissLogType.DODGE_CHARM, eDamageEffectType.NORMAL, data.TargetPartsNumber == 1 ? _target : null, 1f);
			}
		}

		[global::OpenPCR.Migration("c869965f-cdb7-4327-8dea-21242376804c")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}
	}
}
