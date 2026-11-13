using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class TauntAction : ActionParameter
	{
		private BasePartsData parts;
		private enum eTauntType
		{
			NORMAL = 1
		}

		private static readonly Dictionary<eTauntType, eAbnormalState> abnormalStateDic = new Dictionary<eTauntType, eAbnormalState>
		{
			{
				eTauntType.NORMAL,
				(eAbnormalState)75
			}
		};
		[global::OpenPCR.Migration("8bdd3be3-e394-4f82-bfea-92718b6f94fa")]
		public TauntAction() : base()
		{
		}

		[global::OpenPCR.Migration("c4b7c28e-fdd9-461e-87ba-3a969b2e0049")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			parts = _source.ComponentMultiPartsBoss.BossPartsListForBattle.Find(e => e.Index == _skill.ParameterTarget);
		}

		[global::OpenPCR.Migration("a7ee0830-3005-4cb7-81cd-fee4e883db3a")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _startTime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _startTime, _enabledChildAction, _valueDictionary);
			float random = Elements.Battle.BattleManager.Random(0f, 1f);
			eAbnormalState state = abnormalStateDic[(eTauntType)ActionDetail1];
			if (_source.ComponentAbnormal.IsAbnormalState(eAbnormalState.SPY) || _source.ComponentAbnormal.IsEtherealization())
				return;
			if (random >= BattleUtil.GetDodgeByLevelDiff(_skill.Level, _target))
			{
				ActionExecedData data = AlreadyExecedData[_target.Owner][_num];
				if (data.ExecedPartsNumber == data.TargetPartsNumber)
					_target.Owner.ComponentDamageControl.SetMissAtk(_source, eMissLogType.DODGE_TAUNT, eDamageEffectType.NORMAL, data.TargetPartsNumber == 1 ? _target : null, 1f);
				return;
			}

			AppendIsAlreadyExeced(_target.Owner, _num);
			UnitComponentAbnormal abnormal = _target.Owner.ComponentAbnormal;
			if (abnormal.IsAbnormalState(eAbnormalStateCategory.TAUNT))
				abnormal.TauntSource.Owner.ComponentAbnormal.TauntTargetList.Remove(_target.Owner);
			abnormal.SetAbnormalState(_source, state, (float)_valueDictionary[eValueNumber.VALUE_1], this, _skill);
			abnormal.TauntSource = _source.IsPartsBoss ? parts : _source.ComponentMultiPartsBoss.DummyPartsData;
			_source.ComponentAbnormal.TauntTargetList.Add(_target.Owner);
			_target.Owner.SetDirectionAuto();
			UnitComponentSearchArea search = _target.Owner.ComponentSearchArea;
			BasePartsData source = abnormal.GetTauntSource();
			search.UpdateAttackTargetImpl(source);
		}

		[global::OpenPCR.Migration("a2960684-aaf3-4e2f-b2cc-09dc7ac1cd58")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
		}
	}
}
