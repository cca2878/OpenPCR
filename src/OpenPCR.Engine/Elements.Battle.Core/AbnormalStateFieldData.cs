using System.Collections.Generic;
using Elements.Battle;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class AbnormalStateFieldData : FieldDataBase
	{
		public ActionParameter TargetAction { get; set; }
		public UnitActionController SourceActionController { get; set; }

		[global::OpenPCR.Migration("701b515a-b32f-4e4b-8082-10b02cef1d0e")]
		public AbnormalStateFieldData() : base()
		{
		}

		[global::OpenPCR.Migration("1d87e2f9-971e-4253-bd96-1b44f7774b59")]
		public override void StartField()
		{
			if (base.UniqueEffectPrefab != null)
			{
				skillEffect = base.battleEffectPool.GetEffect(base.Source.IsLeftDir ? base.UniqueEffectPrefabLeft : base.UniqueEffectPrefab);
				initializeSkillEffect();
			}

			base.StartField();
		}

		[global::OpenPCR.Migration("eac9c50f-327e-47c5-911d-5cd2ad4f485f")]
		public override void OnRepeat()
		{
		}

		[global::OpenPCR.Migration("a44bef98-4691-48d2-82e2-fa5641a1b253")]
		public override void OnExit(BasePartsData _parts)
		{
			base.OnExit((global::Elements.BasePartsData)(@_parts));
			global::Elements.Battle.Core.ActionParameter nativeValue1 = (this).TargetAction;
			if (global::System.Object.ReferenceEquals(nativeValue1, null))
				throw new global::System.NullReferenceException();
			(nativeValue1).ResetHitData();
			if (global::System.Object.ReferenceEquals(@_parts, null))
				throw new global::System.NullReferenceException();
			global::Elements.UnitCtrl nativeGetter2 = (@_parts).Owner;
			if (global::System.Object.ReferenceEquals(nativeGetter2, null))
				throw new global::System.NullReferenceException();
			global::Elements.Battle.Core.ActionParameter nativeValue3 = (this).TargetAction;
			if (global::System.Object.ReferenceEquals(nativeValue3, null))
				throw new global::System.NullReferenceException();
			global::Elements.Battle.Core.AbnormalStateFieldAction nativeValue4 = (nativeValue3).AbnormalStateFieldAction;
			if (global::System.Object.ReferenceEquals(nativeValue4, null))
				throw new global::System.NullReferenceException();
			global::Elements.Battle.Core.UnitComponentAbnormal nativeValue5 = (nativeGetter2).ComponentAbnormal;
			if (global::System.Object.ReferenceEquals(nativeValue5, null))
				throw new global::System.NullReferenceException();
			int nativeValue6 = (this).fieldIndex;
			int nativeValue7 = (nativeValue3).ActionId;
			global::Elements.Battle.Core.eAbnormalState nativeValue8 = (nativeValue4).TargetAbnormalState;
			(nativeValue5).DisableAbnormalStateById(unchecked((global::Elements.Battle.Core.eAbnormalState)(unchecked((uint)(nativeValue8)))), unchecked((int)(unchecked((uint)(nativeValue7)))), false, unchecked((int)(unchecked((uint)(nativeValue6)))));
		}

		[global::OpenPCR.Migration("eff94085-7a9c-453d-a018-0c51a48faa7f")]
		public override void OnEnter(BasePartsData _parts)
		{
			UnitCtrl owner = _parts.Owner;
			if (!TargetAction.IsChangeSpeedAction() || owner.ComponentAbnormal.OverwriteSpeedDataCount <= 0)
			{
				base.OnEnter(_parts);
				if (_parts.Owner.IsSummonOrPhantom)
				{
					TargetAction.AppendTargetNum(_parts.Owner, 0);
				}

				if (!TargetAction.IsAppendTargetNum(_parts.Owner))
				{
					TargetAction.AppendTargetNum(_parts.Owner, 0);
				}

				TargetAction.SourceFieldIndex = fieldIndex;
				SourceActionController.ExecAction(TargetAction, base.Skill, _parts, 0, 0f, false, null, true);
			}
		}

		[global::OpenPCR.Migration("e40d518c-dd6e-4ca0-8591-7fd26a9952d9")]
		public override void ResetTarget(UnitCtrl _unit, eAbnormalState _abnormalState, bool _strict = false)
		{
			base.ResetTarget(_unit, _abnormalState, false);
			eAbnormalState targetState = TargetAction.AbnormalStateFieldAction.TargetAbnormalState;
			if (_strict ? targetState != _abnormalState : AbnormalStateUtility.GetAbnormalStateCategory(targetState) != AbnormalStateUtility.GetAbnormalStateCategory(_abnormalState))
				return;
			TargetAction.ResetHitData();
			UnitComponentMultiPartsBoss parts = _unit.ComponentMultiPartsBoss;
			if (_unit.IsPartsBoss)
			{
				List<PartsData> list = parts.BossPartsListForBattle;
				for (int i = 0; i < list.Count; i++)
					TargetList.Remove(list[i]);
			}
			else
				TargetList.Remove(parts.GetFirstParts());
			if (Enable)
				updateTarget();
		}

		[global::OpenPCR.Migration("7120c22a-16b8-4f00-bbbc-be4c45fc780d")]
		protected override bool judgeWorking(UnitCtrl _unit)
		{
			bool nativeValue1 = (this).IsTargetAbnormalStateBuff;
			if (((nativeValue1 ? 1u : 0u) == 0))
			{
				if (global::System.Object.ReferenceEquals(@_unit, null))
					throw new global::System.NullReferenceException();
				global::Elements.Battle.Core.UnitComponentState nativeValue2 = (@_unit).ComponentState;
				if (global::System.Object.ReferenceEquals(nativeValue2, null))
					throw new global::System.NullReferenceException();
				bool nativeValue3 = (nativeValue2).ModeChangeUnableStateBarrier;
				if (((nativeValue3 ? 1u : 0u) == 0))
				{
					global::Elements.Battle.Core.UnitComponentAbnormal nativeValue4 = (@_unit).ComponentAbnormal;
					if (global::System.Object.ReferenceEquals(nativeValue4, null))
						throw new global::System.NullReferenceException();
					bool nativeResult1 = (nativeValue4).IsNoDamageMotion();
					return ((((nativeResult1 ? 1u : 0u) ^ 1u) & 1u) != 0);
				}
				else
				{
					return ((0u & 1u) != 0);
				}
			}
			else
			{
				return ((1u & 1u) != 0);
			}
		}

		[global::OpenPCR.Migration("6a7d453f-624c-4ddb-b011-b6fb8c82f48e")]
		public void CallUpdateTarget()
		{
			if (Enable)
				updateTarget();
		}
	}
}
