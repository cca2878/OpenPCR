using global::OpenPCR.AnimKit;
using global::OpenPCR.Runtime.Api;
using System;
using Elements.Battle;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public partial class SummonAction : ActionParameter
	{
		public enum eSummonSide
		{
			OURS = 1,
			OTHER
		}

		public enum eSummonType
		{
			NONE = 0,
			SUMMON = 1,
			PHANTOM = 2,
			DIVISION = 1001
		}

		public enum eMoveType
		{
			NORMAL = 1,
			LINEAR
		}

		private Attachment targetAttachment;
		private Attachment appliedAttachment;
		private bool attachmentChanged;
		[global::OpenPCR.Migration("7dffbca6-c67d-46b9-bc3c-d6b62c2ecf79")]
		private IEnumerator updateAttachmentChange(UnitCtrl _source, int _targetIndex)
		{
			while (true)
			{
				yield return null;
				bool flag = (_source.IsOther ? base.battleManager.EnemyList : base.battleManager.UnitList).Contains(_source.SummonUnitDictionary[base.ActionId]);
				if (!attachmentChanged & flag)
				{
					attachmentChanged = true;
					((_source.UnitSpineCtrl.skeleton.skin == null) ? _source.UnitSpineCtrl.skeleton.data.defaultSkin : _source.UnitSpineCtrl.skeleton.skin).AddAttachment(_targetIndex, _source.SummonTargetAttachmentName, appliedAttachment);
					_source.UnitSpineCtrl.skeleton.slots.Items[_targetIndex].attachment = appliedAttachment;
				}

				if (attachmentChanged && !flag)
				{
					attachmentChanged = false;
					((_source.UnitSpineCtrl.skeleton.skin == null) ? _source.UnitSpineCtrl.skeleton.data.defaultSkin : _source.UnitSpineCtrl.skeleton.skin).AddAttachment(_targetIndex, _source.SummonTargetAttachmentName, targetAttachment);
					_source.UnitSpineCtrl.skeleton.slots.Items[_targetIndex].attachment = targetAttachment;
				}
			}
		}

		[global::OpenPCR.Migration("9311932f-9bcf-4782-ac99-fbb983b248fe")]
		public SummonAction() : base()
		{
		}

		[global::OpenPCR.Migration("fab705f2-b138-481e-8eff-c957a2cd3188")]
		public override void Initialize(UnitCtrl _ownerUnitCtrl)
		{
			bool isWaveEnemy = _ownerUnitCtrl != null && _ownerUnitCtrl.IsOther;
			base.Initialize();
			int summonResourceId = UnitUtility.GetSummonResourceId(UnitUtility.GetUnitResourceId(base.ActionDetail2));
			Singleton<BattleUnitLoader>.Instance.AddLoadResource(summonResourceId, delegate (GameObject _loadedObject)
			{
				base.SummonPrefab = _loadedObject;
			}, isWaveEnemy);
			ManagerSingleton<ResourceManager>.Instance.StartLoad();
		}

		[global::OpenPCR.Migration("99134299-2a2d-4773-bc0a-f4278b6abd7d")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			eUnitRespawnPos basePos = (eUnitRespawnPos)Math.Min(9, Math.Max(0, (int)_source.RespawnPos + BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_2])));
			UnitCtrl unitCtrl = base.battleManager.Summon(new SummonData { SummonId = base.ActionDetail2, TargetPosition = _target.GetLocalPosition() + new Vector3(((_source.IsLeftDir || _source.IsForceLeftDir) ? (-1f) : 1f) * (float)_valueDictionary[eValueNumber.VALUE_7], 0f), Owner = _source, Skill = _skill, Prefab = base.SummonPrefab, SummonSide = (eSummonSide)base.ActionDetail3, SummonType = (eSummonType)BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_5]), MoveSpeed = (float)_valueDictionary[eValueNumber.VALUE_1], FromPosition = _source.transform.localPosition + new Vector3((float)((!_source.IsLeftDir && !_source.IsForceLeftDir) ? 1 : (-1)) * (float)_valueDictionary[eValueNumber.VALUE_6], 0f, 0f), UseRespawnPos = (!BattleUtil.Approximately(_valueDictionary[eValueNumber.VALUE_2], 99.0)), RespawnPos = BattleUtil.SearchRespawnPos(basePos, _source.IsOther ? base.battleManager.EnemyList : base.battleManager.UnitList), ConsiderEquipmentAndBonus = (base.ActionDetail1 == 3) });
			int actionId = base.ActionId;
			if (!_source.SummonUnitDictionary.ContainsKey(actionId))
			{
				_source.SummonUnitDictionary.Add(actionId, unitCtrl);
			}
			else
			{
				UnitCtrl unitCtrl2 = _source.SummonUnitDictionary[actionId];
				if (!_source.ComponentState.IdleOnly && !_source.ComponentState.IsDead)
				{
					unitCtrl2.ComponentState.IdleOnly = true;
					unitCtrl2.ComponentAbnormal.CureAllAbnormalState();
				}

				_source.SummonUnitDictionary[actionId] = unitCtrl;
			}

			unitCtrl.SummonSource = _source;
			UnitCtrl unitCtrl3 = _source.SummonUnitDictionary[actionId];
			if (_source.ComponentState.IdleOnly || base.battleManager.GameState != eBattleGameState.PLAY)
			{
				unitCtrl3.ComponentState.IdleOnly = true;
				if (base.battleManager.UnitList.Contains(unitCtrl3))
				{
					base.battleManager.UnitList.Remove(unitCtrl3);
				}
			}

			if (!_source.SummonTargetAttachmentName.IsNullOrEmpty() && appliedAttachment == null)
			{
				int num = _source.UnitSpineCtrl.skeleton.slots.FindIndex((Slot e) => e.data.Name == _source.SummonTargetAttachmentName);
				targetAttachment = _source.UnitSpineCtrl.skeleton.GetAttachment(num, _source.SummonTargetAttachmentName);
				int slotIndex = _source.UnitSpineCtrl.skeleton.slots.FindIndex((Slot e) => e.data.Name == _source.SummonAppliedAttachmentName);
				appliedAttachment = _source.UnitSpineCtrl.skeleton.data.defaultSkin.GetAttachment(slotIndex, _source.SummonAppliedAttachmentName);
				_source.StartCoroutine(updateAttachmentChange(_source, num));
			}
		}
	}
}
