using Elements.Battle;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class BuffDebuffClearAction : ActionParameter
	{

		private List<int> BuffDebuffCancelType = new List<int>();
		[global::OpenPCR.Migration("978e3e82-972a-4793-b17a-cc96a9afa84a")]
		public BuffDebuffClearAction() : base()
		{
		}

		[global::OpenPCR.Migration("ad77d8a9-50ad-4c27-9df0-94311fc5b49e")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			if (base.ActionDetail1 == 20)
			{
				BuffDebuffCancelType.Add(global::OpenPCR.NativeNumeric.ToInt32(base.Value[eValueNumber.VALUE_4]));
				BuffDebuffCancelType.Add(global::OpenPCR.NativeNumeric.ToInt32(base.Value[eValueNumber.VALUE_5]));
				BuffDebuffCancelType.Add(global::OpenPCR.NativeNumeric.ToInt32(base.Value[eValueNumber.VALUE_6]));
				BuffDebuffCancelType.Add(global::OpenPCR.NativeNumeric.ToInt32(base.Value[eValueNumber.VALUE_7]));
				BuffDebuffCancelType.RemoveAll((int buffParam) => buffParam == 0);
			}
		}

		[global::OpenPCR.Migration("71979461-2c09-4bb5-9c33-54472f5d9ea2")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			float dodgeRoll = Elements.Battle.BattleManager.Random(0f, 1f);
			bool dodged = BattleUtil.LessThanOrApproximately(BattleUtil.GetDodgeByLevelDiff(_skill.Level, _target), dodgeRoll) && ActionDetail1 != 2;
			float chanceRoll = Elements.Battle.BattleManager.Random(0f, 1f);
			if (!((_valueDictionary[eValueNumber.VALUE_1] / 100.0 >= chanceRoll) & !dodged) && Value[eValueNumber.VALUE_3] == 0.0)
			{
				ActionExecedData executed = AlreadyExecedData[_target.Owner][_num];
				if (executed.ExecedPartsNumber == executed.TargetPartsNumber)
					_target.Owner.ComponentDamageControl.SetMissAtk(_source, eMissLogType.DODGE_ATTACK, eDamageEffectType.NORMAL, executed.ExecedPartsNumber == 1 ? _target : null, 1f);
				return;
			}
			AppendIsAlreadyExeced(_target.Owner, _num);
			UnitComponentAbnormal abnormal = _target.Owner.ComponentAbnormal;
			switch (ActionDetail1)
			{
				case 1: abnormal.DespeleBuffDebuff(true, CreateAbnormalEffectData()); break;
				case 2: abnormal.DespeleBuffDebuff(false, CreateAbnormalEffectData()); break;
				case 3: abnormal.CancelStatusUpBuff(); break;
				case 10:
					abnormal.DispelAbnormalState(eAbnormalState.GUARD_ATK);
					abnormal.DispelAbnormalState(eAbnormalState.GUARD_MGC);
					abnormal.DispelAbnormalState(eAbnormalState.GUARD_BOTH);
					abnormal.DispelAbnormalState(eAbnormalState.DRAIN_BOTH);
					abnormal.DispelAbnormalState(eAbnormalState.DRAIN_ATK);
					abnormal.DispelAbnormalState(eAbnormalState.DRAIN_MGC); break;
				case 11: abnormal.DispelAbnormalState(eAbnormalState.GUARD_ATK); break;
				case 12: abnormal.DispelAbnormalState(eAbnormalState.GUARD_MGC); break;
				case 13: abnormal.DispelAbnormalState(eAbnormalState.DRAIN_ATK); break;
				case 14: abnormal.DispelAbnormalState(eAbnormalState.DRAIN_MGC); break;
				case 15: abnormal.DispelAbnormalState(eAbnormalState.GUARD_BOTH); break;
				case 16: abnormal.DispelAbnormalState(eAbnormalState.DRAIN_BOTH); break;
				case 17: abnormal.DispelAbnormalState(eAbnormalState.GUARD_ATK); abnormal.DispelAbnormalState(eAbnormalState.DRAIN_ATK); break;
				case 18: abnormal.DispelAbnormalState(eAbnormalState.GUARD_MGC); abnormal.DispelAbnormalState(eAbnormalState.DRAIN_MGC); break;
				case 19: abnormal.DispelAbnormalState(eAbnormalState.GUARD_BOTH); abnormal.DispelAbnormalState(eAbnormalState.DRAIN_BOTH); break;
				case 20:
					for (int index = 0; index < BuffDebuffCancelType.Count; index++)
						abnormal.CancelBuffDebuffType(BuffDebuffCancelType[index]);
					break;
			}
		}

		[global::OpenPCR.Migration("186ad8cc-b006-4902-a130-02ec51e0214e")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
		}
	}
}
