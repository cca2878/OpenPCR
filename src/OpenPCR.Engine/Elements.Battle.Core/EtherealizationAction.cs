using System;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class EtherealizationAction : ActionParameter
	{
		public enum eTargetAttackType
		{
			PHYSICAL = 1,
			MAGIC,
			BOTH
		}

		[global::OpenPCR.Migration("558b91ee-a2fe-4bda-9fbb-1b5c4ce9fe2a")]
		public EtherealizationAction() : base()
		{
		}

		[global::OpenPCR.Migration("f8e6779b-0f0e-45db-baf7-002fbcf81f4e")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("dce0cd70-ba89-4afb-b3af-b19d36226e5f")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			eTargetAttackType kind = (eTargetAttackType)ActionDetail1;
			eStateIconType icon = (eStateIconType)ActionDetail2;
			UnitComponentAbnormal abnormal = _target.Owner.ComponentAbnormal;
			var seals = abnormal.SealDictionary;
			if (seals.ContainsKey(icon))
			{
				SealData seal = seals[icon];
				if (!seal.IsEtherealization)
				{
					seal.IsEtherealization = true;
					if (seal.GetCurrentCount() >= 1)
					{
						abnormal.AddEtherealizationCount(kind);
						battleManager.StartAllUnitEtherealizationCheck(_target.Owner);
					}

					seal.OnAddAction = createOnAddSeal(kind, abnormal, _target.Owner);
					seal.OnRemoveAction = createOnRemoveSeal(kind, abnormal);
				}
			}
			else
			{
				var seal = new SealData();
				seal.OnAddAction = createOnAddSeal(kind, abnormal, _target.Owner);
				seal.OnRemoveAction = createOnRemoveSeal(kind, abnormal);
				seal.IsEtherealization = true;
				seal.DisplayCount = true;
				seals.Add(icon, seal);
			}
		}

		[global::OpenPCR.Migration("c76681d4-7983-4b60-957c-076a2d1313cc")]
		private Action<int, bool> createOnRemoveSeal(eTargetAttackType _targetAttackType, UnitComponentAbnormal _abnormal)
		{
			return (_id, _oneToZero) =>
			{
				if (!_oneToZero)
					return;
				switch (_targetAttackType)
				{
					case eTargetAttackType.PHYSICAL:
						_abnormal.EtherealizationPhysicalCount = unchecked(_abnormal.EtherealizationPhysicalCount - 1);
						break;
					case eTargetAttackType.MAGIC:
						_abnormal.EtherealizationMagicCount = unchecked(_abnormal.EtherealizationMagicCount - 1);
						break;
					case eTargetAttackType.BOTH:
						_abnormal.EtherealizationBothCount = unchecked(_abnormal.EtherealizationBothCount - 1);
						break;
				}
			};
		}

		[global::OpenPCR.Migration("2917fb65-0a02-4742-b320-df0563c841b9")]
		private Action<int, bool> createOnAddSeal(eTargetAttackType _targetAttackType, UnitComponentAbnormal _abnormal, UnitCtrl _target)
		{
			return (_id, _zeroToOne) =>
			{
				if (_zeroToOne)
				{
					_abnormal.AddEtherealizationCount(_targetAttackType);
					battleManager.StartAllUnitEtherealizationCheck(_target);
				}
			};
		}

		[global::OpenPCR.Migration("16a4bbae-f8e0-4ada-8436-996cdcf74088")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
		}
	}
}
