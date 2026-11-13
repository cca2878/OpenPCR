using Elements.Battle;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class ChangeSpeedOverwriteFieldData : FieldDataBase
	{
		public global::Elements.Battle.Core.ActionParameter ActionParameter;
		public int ActionCounter;
		public global::Elements.Battle.Core.eOverwriteAbnormalState AbnormalState;
		public double EffectValue;
		public bool IsShowUniqueEffect;
		private Dictionary<UnitCtrl, int> alreadyExecedTargetCount;
		[global::OpenPCR.Migration("ca41843e-c6c4-48c1-8bd3-543acbdcd7db")]
		public ChangeSpeedOverwriteFieldData() : base()
		{
		}

		[global::OpenPCR.Migration("8c8dd613-42d3-49a4-8c4f-72290a3b9cc6")]
		public override void StartField()
		{
			alreadyExecedTargetCount = new Dictionary<UnitCtrl, int>();
			if (base.UniqueEffectPrefab != null && IsShowUniqueEffect)
			{
				skillEffect = base.battleEffectPool.GetEffect(base.Source.IsLeftDir ? base.UniqueEffectPrefabLeft : base.UniqueEffectPrefab);
				initializeSkillEffect();
			}

			base.StartField();
		}

		[global::OpenPCR.Migration("641d1874-4a24-486b-a06d-71c31ae2b0b9")]
		public override void OnEnter(BasePartsData _parts)
		{
			base.OnEnter(_parts);
			UnitCtrl owner = _parts.Owner;
			alreadyExecedTargetCount.TryGetValue(owner, out int count);
			count++;
			alreadyExecedTargetCount[owner] = count;
			if (count == 1)
			{
				ActionParameter.SourceFieldIndex = fieldIndex;
				owner.ComponentAbnormal.AddOverwriteAbnormalState(ActionCounter, new OverwriteAbnormalStateData(_parts, Source, Skill, ActionParameter, AbnormalState, EffectValue));
			}
		}

		[global::OpenPCR.Migration("df1cb9d6-e9b6-430e-ad30-5d716327447d")]
		public override void OnExit(BasePartsData _parts)
		{
			base.OnExit(_parts);
			UnitCtrl owner = _parts.Owner;
			alreadyExecedTargetCount[owner]--;
			if (alreadyExecedTargetCount[owner] <= 0)
				owner.ComponentAbnormal.RemoveOverwriteAbnormalState(ActionCounter, fieldIndex);
		}

		[global::OpenPCR.Migration("0946c1b8-904b-46dd-9628-eac678aebf5d")]
		public override void ResetTarget(UnitCtrl _unit, eAbnormalState _abnormalState, bool _strict = false)
		{
			base.ResetTarget(_unit, _abnormalState, false);
			eAbnormalState state = AbnormalStateUtility.GetAbnormalState(AbnormalState);
			if (_strict)
			{
				if (state != _abnormalState)
					return;
			}
			else if (AbnormalStateUtility.GetAbnormalStateCategory(state) !=
					 AbnormalStateUtility.GetAbnormalStateCategory(_abnormalState))
				return;

			List<BasePartsData> parts = new List<BasePartsData>();
			if (_unit.IsPartsBoss)
			{
				List<PartsData> bossParts = _unit.ComponentMultiPartsBoss.BossPartsListForBattle;
				for (int i = 0; i < bossParts.Count; i++)
					parts.Add(bossParts[i]);
			}
			else
				parts.Add(_unit.ComponentMultiPartsBoss.GetFirstParts(false, 0f));

			for (int i = 0; i < parts.Count; i++)
			{
				BasePartsData part = parts[i];
				if (TargetList.Contains(part))
				{
					TargetList.Remove(part);
					OnExit(part);
				}
			}
		}
	}
}
