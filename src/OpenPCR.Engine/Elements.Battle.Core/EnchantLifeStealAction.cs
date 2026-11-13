using global::OpenPCR.Foundation;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class EnchantLifeStealAction : ActionParameter
	{
		[global::OpenPCR.Migration("3c808392-7ab1-4685-a994-824598a883c6")]
		public EnchantLifeStealAction() : base()
		{
		}

		[global::OpenPCR.Migration("8412f415-8d0e-4ef0-89fd-547266b18efc")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("2047cd2c-f384-4232-9cf5-949c6313bb6d")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			var queue = new Queue<int>();
			int count = BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_3]);
			for (int i = 0; i < count; i++)
				queue.Enqueue(global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_1]));
			_target.Owner.ComponentAbnormal.LifeStealQueueList.Add(queue);
			_target.Owner.ComponentAbnormal.OnChangeState.Call(eStateIconType.BUFF_ADD_LIFE_STEAL, true);
		}

		[global::OpenPCR.Migration("3f70f693-8e23-4bac-afd9-6a76b647f33d")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}
	}
}
