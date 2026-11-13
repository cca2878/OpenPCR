using Elements.Battle;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public partial class ChangeParameterFieldData : FieldDataBase
	{
		public enum eFieldEffectType
		{
			NONE = 0,
			BUFF = 1,
			DEBUFF = 2,
			UNIQUE = 3
		}
		private global::System.Collections.Generic.Dictionary<Elements.BasePartsData, System.Int64> buffDebuffParamDecreaseResistRate = new Dictionary<BasePartsData, long>();
		public global::Elements.Battle.Core.ChangeParameterFieldData.eFieldEffectType EffectType { get; set; }
		public double Value { get; set; }
		public global::Elements.Battle.Core.BuffDebuffAction.eChangeParameterType ValueType { get; set; }
		public global::System.Collections.Generic.Dictionary<Elements.UnitCtrl, System.Int32> AlreadyExecedTargetCount { get; set; }

		public bool ShowsIcon = true;
		public bool TriggerCall = true;
		public BuffParamKind BuffParamKind { get; set; }
		public bool IsBuff { get; set; }

		[global::OpenPCR.Migration("60ec7154-46c5-49c6-ac67-2545e8ce76a4")]
		public ChangeParameterFieldData()
		{
		}

		[global::OpenPCR.Migration("720de229-837f-4a42-b17e-4098cbb409de")]
		protected override int getClearedIndex(UnitCtrl _unit)
		{
			if (!IsBuff)
			{
				return _unit.ComponentAbnormal.ClearedDebuffFieldIndex;
			}

			return _unit.ComponentAbnormal.ClearedBuffFieldIndex;
		}

		[global::OpenPCR.Migration("15668fe5-18e2-4439-a330-aa12f8ca9928")]
		public override void StartField()
		{
			if (EffectType == eFieldEffectType.DEBUFF)
			{
				skillEffect = battleEffectPool.GetEffect(Singleton<BattleResourceManager>.Instance.DebuffFieldEffectPrefab);
				initializeSkillEffect();
			}
			else if (EffectType == eFieldEffectType.UNIQUE)
			{
				skillEffect = battleEffectPool.GetEffect(Source.IsLeftDir ? UniqueEffectPrefabLeft : UniqueEffectPrefab);
				initializeSkillEffect();
			}

			base.StartField();
		}

		[global::OpenPCR.Migration("ea2f4157-ca3e-47db-8ba4-c4ebff257182")]
		public override void OnRepeat()
		{
		}

		[global::OpenPCR.Migration("b8f43bc3-e02d-4e9c-9249-affe669c274b")]
		public override void OnExit(BasePartsData _parts)
		{
			base.OnExit(_parts);
			AlreadyExecedTargetCount[_parts.Owner]--;
			if (AlreadyExecedTargetCount[_parts.Owner] != 0)
				return;
			var values = new Dictionary<BasePartsData, long>();
			UnitComponentMultiPartsBoss parts = _parts.Owner.ComponentMultiPartsBoss;
			if (_parts.Owner.IsPartsBoss && !AbnormalStateDefine.IS_EFFECT_BOSS_BODY_ONLY[BuffParamKind])
			{
				List<PartsData> list = parts.BossPartsListForBattle;
				for (int i = 0; i < list.Count; i++)
					values.Add(list[i], buffDebuffParamDecreaseResistRate[list[i]]);
			}
			else
				values.Add(parts.DummyPartsData, buffDebuffParamDecreaseResistRate[parts.DummyPartsData]);
			UnitComponentAbnormal abnormal = _parts.Owner.ComponentAbnormal;
			abnormal.EnableBuffParam(BuffParamKind, values, false, null, IsBuff, false, ShowsIcon, TriggerCall);
			abnormal.EnableBuffDebuffField(false, this);
		}

		[global::OpenPCR.Migration("5717efe4-d5c5-4c31-aa77-72f74284245a")]
		public override void OnEnter(BasePartsData _parts)
		{
			UnitCtrl owner = _parts.Owner;
			base.OnEnter(_parts);
			if (AlreadyExecedTargetCount.ContainsKey(_parts.Owner))
			{
				AlreadyExecedTargetCount[_parts.Owner]++;
				if (AlreadyExecedTargetCount[_parts.Owner] != 1)
					return;
			}
			else
				AlreadyExecedTargetCount.Add(_parts.Owner, 1);
			var values = new Dictionary<BasePartsData, long>();
			UnitComponentMultiPartsBoss parts = _parts.Owner.ComponentMultiPartsBoss;
			if (_parts.Owner.IsPartsBoss && !AbnormalStateDefine.IS_EFFECT_BOSS_BODY_ONLY[BuffParamKind])
			{
				List<PartsData> list = parts.BossPartsListForBattle;
				for (int i = 0; i < list.Count; i++)
				{
					BasePartsData key = list[i];
					long value = BuffDebuffAction.CalculateBuffDebuffParam(list[i], Value, ValueType, BuffParamKind, !IsBuff);
					buffDebuffParamDecreaseResistRate[key] = value;
					values.Add(key, value);
				}
			}
			else
			{
				BasePartsData dummy = parts.DummyPartsData;
				long value = BuffDebuffAction.CalculateBuffDebuffParam(_parts, Value, ValueType, BuffParamKind, !IsBuff);
				buffDebuffParamDecreaseResistRate[dummy] = value;
				values.Add(dummy, value);
			}

			UnitComponentAbnormal abnormal = owner.ComponentAbnormal;
			abnormal.SetBuffParam(global::Elements.Battle.Core.BuffParamKind.NUM, BuffParamKind, values, 0f, 0, null, true, eEffectType.COMMON, IsBuff, false, false, 0, -1, true);
			abnormal.EnableBuffParam(BuffParamKind, values, true, null, IsBuff, false, ShowsIcon, TriggerCall);
			abnormal.EnableBuffDebuffField(true, this);
		}

		[global::OpenPCR.Migration("97013069-c1cb-443e-befd-5f9c6058e5b3")]
		public void DispelImmediately(UnitCtrl _unit)
		{
			if (getClearedIndex(_unit) < fieldIndex)
				return;
			var parts = new List<BasePartsData>();
			UnitComponentMultiPartsBoss component = _unit.ComponentMultiPartsBoss;
			if (_unit.IsPartsBoss)
			{
				List<PartsData> list = component.BossPartsListForBattle;
				for (int i = 0; i < list.Count; i++)
					parts.Add(list[i]);
			}
			else
				parts.Add(component.GetFirstParts());
			for (int i = 0; i < parts.Count; i++)
				if (TargetList.Contains(parts[i]))
					OnExit(parts[i]);
		}
	}
}
