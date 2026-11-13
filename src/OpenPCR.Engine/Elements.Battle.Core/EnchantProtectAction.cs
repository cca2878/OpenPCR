using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class EnchantProtectAction : ActionParameter
	{
		[global::OpenPCR.Migration("8ad84c69-d924-40ae-bc56-a6b207dacc3e")]
		public EnchantProtectAction() : base()
		{
		}

		[global::OpenPCR.Migration("88ad61f2-1b13-4508-afc0-3cc848d4e352")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("ca412070-5c40-4c96-b739-acb75ba4c5a1")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			if (_source == _target.Owner)
				return;
			var abnormal = _target.Owner.ComponentAbnormal;
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

			var subEffects = new List<AbnormalStateEffectPrefabData>();
			foreach (var effect in ActionSubEffectList)
				subEffects.Add(new AbnormalStateEffectPrefabData { LeftEffectPrefab = effect.PrefabLeft, RightEffectPrefab = effect.Prefab, TargetBone = effect.TargetBone, TrackType = effect.TrackType });
			var data = new ProtectionData
			{
				Target = _source,
				Timer = (float)_valueDictionary[eValueNumber.VALUE_3],
				Counter = BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]) != 0 ? BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]) : -1,
				IconType = (eStateIconType)ActionDetail1,
				Enable = true,
				ProtectionParts = _target.Owner.ComponentMultiPartsBoss.GetFirstParts(false, 0f),
				EffectPrefab = effectPrefab,
				EffectPrefabLeft = effectPrefabLeft,
				EffectPrefabForProtectionPartsList = subEffects,
				EffectMaxCount = BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_5]),
				EffectDuration = (float)_valueDictionary[eValueNumber.VALUE_6],
				IsEnchantProtect = true,
				Source = _source
			};
			abnormal.ProtectionSourceData = data;
			_source.ComponentAbnormal.EnchantProtection(data);
		}

		[global::OpenPCR.Migration("22e293ab-8233-4993-bd95-f9771efb5699")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}
	}
}
