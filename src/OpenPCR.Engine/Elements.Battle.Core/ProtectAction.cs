using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class ProtectAction : ActionParameter
	{
		[global::OpenPCR.Migration("09c6a425-0957-46f4-86a4-6208f798deaf")]
		public ProtectAction() : base()
		{
		}

		[global::OpenPCR.Migration("5bfa3ee3-7908-45e4-a32d-d7bcc0122626")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("a56e65ca-3e26-4edb-8d42-c473151efa51")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			if (_source == _target.Owner)
				return;
			var abnormal = _source.ComponentAbnormal;
			var previous = abnormal.ProtectionSourceData;
			if (previous != null && previous.Enable)
				previous.Disable();
			GameObject effectPrefab = null;
			GameObject effectPrefabLeft = null;
			if (ActionEffectList.Count > 0)
			{
				effectPrefab = ActionEffectList[0].Prefab;
				effectPrefabLeft = ActionEffectList[0].PrefabLeft;
			}

			var data = new ProtectionData
			{
				Target = _target.Owner,
				Timer = (float)_valueDictionary[eValueNumber.VALUE_3],
				Counter = BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]) != 0 ? BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]) : -1,
				IconType = (eStateIconType)ActionDetail1,
				Enable = true,
				ProtectionParts = _source.ComponentMultiPartsBoss.GetFirstParts(false, 0f),
				EffectPrefab = effectPrefab,
				EffectPrefabLeft = effectPrefabLeft,
				EffectMaxCount = BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_5]),
				EffectDuration = (float)_valueDictionary[eValueNumber.VALUE_6],
				Source = _source
			};
			abnormal.ProtectionSourceData = data;
			_target.Owner.ComponentAbnormal.EnchantProtection(data);
		}

		[global::OpenPCR.Migration("0da05353-1d0b-4ae9-9bb9-a93bc8ba6ed0")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}
	}
}
