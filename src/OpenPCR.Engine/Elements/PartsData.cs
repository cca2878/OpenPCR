using global::Elements;
using global::OpenPCR.AnimKit;
using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Elements.Battle;
using Elements.Battle.Core;
using Elements.Data;

namespace Elements
{
	[Serializable]
	public class PartsData : BasePartsData, ISingletonField
	{

		[global::OpenPCR.Migration("0d0ba4bc-a5b8-4619-9fc4-4825fbe225e4")]
		public override long GetDefZero()
		{
			return System.Math.Min(2000000000L, unchecked(System.Math.Max(0L, (long)Def) + getAdditionalBuff(Elements.Battle.Core.BuffParamKind.DEF)));
		}
		public global::System.Int64 HpRecoveryRate { get; private set; }
		private IBattleEffectPool battleEffectPool;
		private global::System.Int64 DefForDamagedEnergy = 0;
		private global::System.Int64 MagicDefForDamagedEnergy = 0;
		public Action OnBreak { get; set; }
		public Action OnBreakEnd { get; set; }
		public bool IsBreak { get; set; }
		public UnitCtrl BreakSource { get; set; }
		public MultiTargetCursor MultiTargetCursor { get; set; }
		private Bone centerBone { get; set; }
		private Bone stateBone { get; set; }
		private Vector3 fixedCenterPos { get; set; }
		public int Level { get; protected set; }
		public global::System.Int64 Atk { get; protected set; }
		public global::System.Int64 MagicStr { get; protected set; }
		public global::System.Int64 Def { get; protected set; }
		public global::System.Int64 MagicDef { get; protected set; }
		public global::System.Int64 Dodge { get; protected set; }
		public global::System.Int64 Accuracy { get; protected set; }
		public global::System.Int64 PhysicalCritical { get; protected set; }
		public global::System.Int64 MagicCritical { get; protected set; }
		public global::System.Int64 LifeSteal { get; private set; }
		public global::System.Int64 StartAtk { get; protected set; }
		public global::System.Int64 StartMagicStr { get; protected set; }
		public global::System.Int64 StartDef { get; protected set; }
		public global::System.Int64 StartMagicDef { get; protected set; }
		public global::System.Int64 StartDodge { get; protected set; }
		public global::System.Int64 StartPhysicalCritical { get; protected set; }
		public global::System.Int64 StartMagicCritical { get; protected set; }
		public global::System.Int64 StartLifeSteal { get; private set; }
		public long BreakPoint { get; set; }
		public float BreakTime { get; set; }
		public global::System.Int64 MaxBreakPoint { get; private set; }
		private Dictionary<BuffParamKind, long> additionalBuffDictionary { get; set; } = new Dictionary<BuffParamKind, long>();

		[global::OpenPCR.Migration("2a904b08-41ca-4e75-aa95-5263d7d403fc")]
		public PartsData()
		{
			Yggdrasil<PartsData> tree = SingletonTreeTreeFunc.CreateSingletonTree<PartsData>();
			battleEffectPool = tree.Get<BattleEffectPool>();
		}

		[global::OpenPCR.Migration("24771cbd-653b-4447-abf2-2d464e47c4e4")]
		private long getAdditionalBuff(BuffParamKind _kind)
		{
			if (!additionalBuffDictionary.TryGetValue(_kind, out var value))
			{
				return 0;
			}

			return value;
		}

		[global::OpenPCR.Migration("4c49cdf3-e588-4261-a0d3-af070ec7e359")]
		public override bool GetTargetable()
		{
			if (IsBreak)
			{
				return AttachmentNamePairList.Count <= 0;
			}

			return true;
		}

		[global::OpenPCR.Migration("4e2cd5df-ddba-4d9f-8ca9-ee0bc36d892a")]
		public void Initialize(MasterEnemyMParts.EnemyMParts _enemyMParts, BattleManager _battleManager)
		{
			SetBattleManager(_battleManager);
			BattleAnimKitController spine = Owner.GetCurrentSpineCtrl();
			foreach (AttachmentNamePair pair in AttachmentNamePairList)
			{
				pair.TargetIndex = spine.skeleton.slots.FindIndex(slot => slot.data.Name == pair.TargetSlotName);
				pair.TargetAttachment = spine.skeleton.GetAttachment(pair.TargetIndex, pair.TargetAttachmentName);
				int appliedIndex = spine.skeleton.slots.FindIndex(slot => slot.data.Name == pair.AppliedSlotName);
				pair.AppliedAttachment = spine.skeleton.GetAttachment(appliedIndex, pair.AppliedAttachmentName);
			}
			centerBone = spine.skeleton.FindBone($"Center_{Index:D2}");
			stateBone = spine.skeleton.FindBone($"State_{Index:D2}");
			fixedCenterPos = Vector3.Scale(new Vector3(centerBone.worldX, centerBone.worldY, 0f), Owner.UnitSpineCtrl.transform.lossyScale);
			int enemyId = _enemyMParts != null ? _enemyMParts.GetChildUnitId(Index) : EnemyId;
			MasterEnemyParameter.EnemyParameter parameter = ManagerSingleton<MasterDataManager>.Instance.masterEnemyParameter.GetFromAllKind(enemyId);
			Dodge = StartDodge = (long)(int)parameter.dodge;
			LifeSteal = StartLifeSteal = (long)(int)parameter.life_steal;
			HpRecoveryRate = (long)(int)parameter.hp_recovery_rate;
			SetAbnormalResistId((int)parameter.resist_status_id, true);
			SetDebuffResistId((int)parameter.resist_variation_id, true);
			battleManager.SetPartsTalentWeakness(enemyId, this);
			EnemyId = enemyId;
			Atk = StartAtk = (long)(int)parameter.atk;
			Def = StartDef = (long)(int)parameter.def;
			MagicStr = StartMagicStr = (long)(int)parameter.magic_str;
			MagicDef = StartMagicDef = (long)(int)parameter.magic_def;
			PhysicalCritical = StartPhysicalCritical = (long)(int)parameter.physical_critical;
			MagicCritical = StartMagicCritical = (long)(int)parameter.magic_critical;
			Accuracy = (long)(int)parameter.accuracy;
			Level = (int)parameter.level;
			MaxBreakPoint = (long)parameter.hp;
			BreakPoint = (long)MaxBreakPoint;
			DefForDamagedEnergy = (long)(int)parameter.def;
			MagicDefForDamagedEnergy = (long)(int)parameter.magic_def;
		}

		[global::OpenPCR.Migration("8ce8ac7b-5614-4039-a793-33522ac4eede")]
		public override Bone GetStateBone()
		{
			return stateBone;
		}

		[global::OpenPCR.Migration("fa67e78e-6887-4879-8555-42f5d2dbfd7f")]
		public override Bone GetCenterBone()
		{
			return centerBone;
		}

		[global::OpenPCR.Migration("1d49a03b-df9f-4748-802e-9f9893c6c467")]
		public override Vector3 GetBottomTransformPosition()
		{
			return base.Owner.BottomTransform.position + new Vector3(PositionX / 540f, 0f);
		}

		[global::OpenPCR.Migration("c29db51c-0b81-4ec5-8b1a-1fb3936c945e")]
		public override Vector3 GetFixedCenterPos()
		{
			bool flag = base.Owner.IsLeftDir || base.Owner.IsForceLeftDirOrPartsBoss;
			float num = base.InitialPositionX / 540f;
			return new Vector3(flag ? (-fixedCenterPos.x) : fixedCenterPos.x, fixedCenterPos.y, fixedCenterPos.z) + new Vector3(flag ? (-num) : num, 0f);
		}

		[global::OpenPCR.Migration("9b90f499-0b51-4aa5-85fc-dfdca220518a")]
		public override Vector3 GetPosition()
		{
			return base.Owner.transform.position + new Vector3(PositionX / 540f, 0f);
		}

		[global::OpenPCR.Migration("9fc7c9dd-5602-44a4-8320-3ee20f7311b2")]
		public override Vector3 GetLocalPosition()
		{
			return base.Owner.transform.localPosition + new Vector3(PositionX, 0f, 0f);
		}

		[global::OpenPCR.Migration("5d35d7eb-31f2-45ff-a410-abd1b2454617")]
		public override float GetBodyWidth()
		{
			return BodyWidthValue;
		}

		[global::OpenPCR.Migration("4fcdd4af-c140-4ea7-8bae-9077e90e4134")]
		public override Vector3 GetColliderCenter()
		{
			return new Vector3((base.Owner.IsLeftDir || base.Owner.IsForceLeftDirOrPartsBoss) ? (-fixedCenterPos.x) : fixedCenterPos.x, fixedCenterPos.y, fixedCenterPos.z);
		}

		[global::OpenPCR.Migration("00644228-05af-448b-ae56-691d146d1c85")]
		public override Vector3 GetColliderSize()
		{
			return new Vector3(BodyWidthValue / 540f, 0f, 0f);
		}

		[global::OpenPCR.Migration("2b1120c7-1bd3-41cb-82eb-3f629a4cd6be")]
		public override long GetMagicDefZero()
		{
			return System.Math.Min(2000000000L, unchecked(System.Math.Max(0L, (long)MagicDef) + getAdditionalBuff(Elements.Battle.Core.BuffParamKind.MAGIC_DEF)));
		}

		[global::OpenPCR.Migration("1a458fab-f46e-4a78-856c-e274b9084522")]
		private long getDodgeZero()
		{
			return System.Math.Min(2000000000L, unchecked(System.Math.Max(0L, (long)Dodge) + getAdditionalBuff(Elements.Battle.Core.BuffParamKind.DODGE)));
		}

		[global::OpenPCR.Migration("0cca5850-14ab-4163-89bd-ce459a716415")]
		public override long GetAtkZero()
		{
			return System.Math.Min(2000000000L, unchecked(System.Math.Max(0L, (long)Atk) + getAdditionalBuff(Elements.Battle.Core.BuffParamKind.ATK)));
		}

		[global::OpenPCR.Migration("85777b06-7c40-4636-bff5-14513e94be9c")]
		public override long GetMagicStrZero()
		{
			return System.Math.Min(2000000000L, unchecked(System.Math.Max(0L, (long)MagicStr) + getAdditionalBuff(Elements.Battle.Core.BuffParamKind.MAGIC_STR)));
		}

		[global::OpenPCR.Migration("107df1b0-cfe9-4967-98cd-bb9b3896aa4b")]
		public override long GetAccuracyZero()
		{
			return System.Math.Min(2000000000L, unchecked(System.Math.Max(0L, (long)Accuracy) + getAdditionalBuff(Elements.Battle.Core.BuffParamKind.ACCURACY)));
		}

		[global::OpenPCR.Migration("d7977ebc-0634-40e1-ae2d-677f2faf7afd")]
		public override long GetPhysicalCriticalZero()
		{
			return System.Math.Min(2000000000L, unchecked(System.Math.Max(0L, (long)PhysicalCritical) + getAdditionalBuff(Elements.Battle.Core.BuffParamKind.PHYSICAL_CRITICAL)));
		}

		[global::OpenPCR.Migration("188c3466-80a7-4ea1-8909-f7f6bd25f9d1")]
		public override long GetMagicCriticalZero()
		{
			return System.Math.Min(2000000000L, unchecked(System.Math.Max(0L, (long)MagicCritical) + getAdditionalBuff(Elements.Battle.Core.BuffParamKind.MAGIC_CRITICAL)));
		}

		[global::OpenPCR.Migration("a39e6097-182a-49e1-85aa-23e6e8110be2")]
		public override long GetLifeStealZero()
		{
			return System.Math.Min(2000000000L, unchecked(System.Math.Max(0L, (long)LifeSteal) + getAdditionalBuff(Elements.Battle.Core.BuffParamKind.LIFE_STEAL)));
		}

		[global::OpenPCR.Migration("6461e395-f356-43b4-a0fe-727c21c71eda")]
		public override long GetHpRecoverRateZero()
		{
			return System.Math.Min(2000000000L, System.Math.Max(0L, (long)HpRecoveryRate));
		}

		[global::OpenPCR.Migration("4baaa099-6578-4f48-8cf5-ae3017e41e82")]
		public override int GetLevel()
		{
			return Level;
		}

		[global::OpenPCR.Migration("34fcd8f9-76b3-4589-acb2-4fef512c4366")]
		public override float GetDodgeRate(long _accuracy)
		{
			long value = Math.Max(unchecked(getDodgeZero() - _accuracy), 0L);
			return (float)value / ((float)value + 100f);
		}

		[global::OpenPCR.Migration("7a8c4012-c914-4b9d-b516-18bad7870e1f")]
		public override long GetDefZeroForDamagedEnergy()
		{
			return System.Math.Min(2000000000L, unchecked(System.Math.Max(0L, (long)DefForDamagedEnergy) + getAdditionalBuff(Elements.Battle.Core.BuffParamKind.DEF)));
		}

		[global::OpenPCR.Migration("404f8181-36b8-421c-91f5-868b0b7d6d55")]
		public override long GetMagicDefZeroForDamagedEnergy()
		{
			return System.Math.Min(2000000000L, unchecked(System.Math.Max(0L, (long)MagicDefForDamagedEnergy) + getAdditionalBuff(Elements.Battle.Core.BuffParamKind.MAGIC_DEF)));
		}

		[global::OpenPCR.Migration("54f6d08e-4900-441b-bf97-64b080b6cb09")]
		public override long GetStartAtk()
		{
			return StartAtk;
		}

		[global::OpenPCR.Migration("49ae9c4e-1b11-4c38-b8a7-d0998f43ae76")]
		public override long GetStartDef()
		{
			return StartDef;
		}

		[global::OpenPCR.Migration("c8225f83-a21c-4bfc-a189-bceb30df5dee")]
		public override long GetStartMagicStr()
		{
			return StartMagicStr;
		}

		[global::OpenPCR.Migration("9d94d113-9f46-4f33-a30d-8d35dead61f4")]
		public override long GetStartMagicDef()
		{
			return StartMagicDef;
		}

		[global::OpenPCR.Migration("ae9c23f9-2d6f-45cb-8a88-05a7c3e2fc7e")]
		public override long GetStartDodge()
		{
			return StartDodge;
		}

		[global::OpenPCR.Migration("7dfcbd39-e6f6-4f7f-bdfe-3a7c14cc6c87")]
		public override long GetStartPhysicalCritical()
		{
			return StartPhysicalCritical;
		}

		[global::OpenPCR.Migration("9245ae01-9281-45b5-93a5-ca493da49ce5")]
		public override long GetStartMagicCritical()
		{
			return StartMagicCritical;
		}

		[global::OpenPCR.Migration("94a44f01-c468-4ed7-a99e-8d58e15e6292")]
		public override long GetStartLifeSteal()
		{
			return StartLifeSteal;
		}

		[global::OpenPCR.Migration("7e79b9cb-ee3b-452e-82b2-7455e84ad586")]
		public override void SetMissAtk(UnitCtrl _source, eMissLogType _missLogType, eDamageEffectType _damageEffectType, float _scale)
		{
			Owner.ComponentDamageControl.SetMissAtk(_source, _missLogType, _damageEffectType, this, _scale);
		}

		[global::OpenPCR.Migration("df939b1e-83ff-4eb1-8037-574bfe47d767")]
		public override void SetBuffDebuff(bool _enable, long _value, BuffParamKind _kind, UnitCtrl _source, IBattleLog _battleLog, bool _additional)
		{
			if (!_enable)
			{
				_value = unchecked(-_value);
			}

			_battleLog.AppendBattleLog(eBattleLogType.SET_BUFF_DEBUFF, _source: _source, _target: base.Owner, _type: (int)((_enable ? 1 : 2) * 10 + _kind), _value: _value, _currentValue: 0L, _duration: 0, _actionId: 0);
			if (_additional)
			{
				if (additionalBuffDictionary.ContainsKey(_kind))
				{
					additionalBuffDictionary[_kind] += _value;
				}
				else
				{
					additionalBuffDictionary.Add(_kind, _value);
				}

				return;
			}

			switch (_kind)
			{
				case global::Elements.Battle.Core.BuffParamKind.ATK:
					Atk = unchecked((long)Atk + _value);
					break;
				case global::Elements.Battle.Core.BuffParamKind.MAGIC_STR:
					MagicStr = unchecked((long)MagicStr + _value);
					break;
				case global::Elements.Battle.Core.BuffParamKind.DEF:
					Def = unchecked((long)Def + _value);
					DefForDamagedEnergy = unchecked((long)DefForDamagedEnergy + _value);
					break;
				case global::Elements.Battle.Core.BuffParamKind.MAGIC_DEF:
					MagicDef = unchecked((long)MagicDef + _value);
					MagicDefForDamagedEnergy = unchecked((long)MagicDefForDamagedEnergy + _value);
					break;
				case global::Elements.Battle.Core.BuffParamKind.DODGE:
					Dodge = unchecked((long)Dodge + _value);
					break;
				case global::Elements.Battle.Core.BuffParamKind.PHYSICAL_CRITICAL:
					PhysicalCritical = unchecked((long)PhysicalCritical + _value);
					break;
				case global::Elements.Battle.Core.BuffParamKind.MAGIC_CRITICAL:
					MagicCritical = unchecked((long)MagicCritical + _value);
					break;
				case global::Elements.Battle.Core.BuffParamKind.LIFE_STEAL:
					LifeSteal = unchecked((long)LifeSteal + _value);
					break;
				case global::Elements.Battle.Core.BuffParamKind.ACCURACY:
					Accuracy = unchecked((long)Accuracy + _value);
					break;
				case global::Elements.Battle.Core.BuffParamKind.ENERGY_RECOVER_RATE:
				case global::Elements.Battle.Core.BuffParamKind.MOVE_SPEED:
				case global::Elements.Battle.Core.BuffParamKind.PHYSICAL_CRITICAL_DAMAGE_RATE:
				case global::Elements.Battle.Core.BuffParamKind.MAGIC_CRITICAL_DAMAGE_RATE:
					break;
			}
		}

		[global::OpenPCR.Migration("acdc2655-ff62-45c1-9772-411996a6d3a9")]
		public IEnumerator TrackBottomCoroutine(bool _topBack)
		{
			while (true)
			{
				TrackBottom(_topBack);
				yield return null;
			}
		}

		[global::OpenPCR.Migration("3b168496-6eef-4917-a12f-daa7aea1015a")]
		public void TrackBottom(bool _isTopBack)
		{
			if (!(MultiTargetCursor == null) && !(MultiTargetCursor.Panel == null))
			{
				if (BreakPoint == 0L && IsBreak)
				{
					MultiTargetCursor.RedUi.alpha = 0f;
				}
				else
				{
					MultiTargetCursor.RedUi.alpha = Mathf.Lerp(1f, 0f, (float)BreakPoint / (float)(long)MaxBreakPoint);
				}

				if (!base.Owner.ComponentSortOrder.IsFront && !_isTopBack)
				{
					MultiTargetCursor.Panel.sortingOrder = base.Owner.GetCurrentSpineCtrl().Depth + (base.Owner.UseTargetCursorOver ? 100 : (-100));
				}

				MultiTargetCursor.Panel.transform.position = GetBottomTransformPosition() + new Vector3(0f, PositionY, 0f);
			}
		}

		[global::OpenPCR.Migration("42529d71-c0b1-42b2-8b7a-3b71d26ff375")]
		public void SetDamage(long _damage, UnitCtrl _source)
		{
			if (Owner.ComponentAbnormal.IsAbnormalState(eAbnormalState.PARTS_NO_DAMAGE))
				return;
			BreakPoint = unchecked(BreakPoint - _damage);
			if (BreakPoint < 0L)
			{
				BreakPoint = 0L;
				BreakSource = _source;
			}
			// Cursor RedUi alpha is rendering output; retain the damage, clamp and source above.

		}

		[global::OpenPCR.Migration("20fe482e-670b-43fa-a3d8-640400b6383b")]
		public void SetBreak(bool _enable, Transform _unitUiCtrl, bool _displayEffect)
		{
			if (!(MultiTargetCursor != null))
			{
				return;
			}
			if (_enable)
			{
				if (!base.Owner.ComponentMultiPartsBoss.BossPartsListForBattle.Exists((PartsData e) => !e.IsBreak))
				{
					if (base.Owner.IsBoss && !base.Owner.ComponentState.GameStartDone)
					{
						base.Owner.ComponentState.IsOnBreakAllCallWhenGameStartDone = true;
					}
					else
					{
						base.Owner.ComponentState.OnBreakAll.Call();
					}
				}
				if (_displayEffect)
				{
					if (BreakEffectTime != 0f)
					{
						base.Owner.AppendCoroutine(waitAndPlayBreak(BreakEffectTime, _unitUiCtrl), ePauseType.SYSTEM, base.Owner);
					}
					// The zero-delay branch only creates a cursor Animator and emits muted sound.
					createBreakEffect(0);
					base.Owner.AppendCoroutine(updateChangeAttachment(ChangeAttachmentStartTime, _enable: true), ePauseType.SYSTEM, base.Owner);
				}
				// Cursor alpha, animation and visibility are output only.
			}
			else
			{
				base.Owner.AppendCoroutine(waitAndBreakPointReset(), ePauseType.SYSTEM);
				createBreakEffect(1);
				// Cursor revival animation/sound/visibility have no gameplay consumer.

			}

		}

		[global::OpenPCR.Migration("e7fed436-3c70-4984-a2f7-6c3f79520539")]
		private void createBreakEffect(int _targetMotion)
		{
			for (int i = 0; i < base.BreakEffectList.Count; i++)
			{
				if (_targetMotion == base.BreakEffectList[i].TargetMotionIndex)
				{
					GameObject mDOJNMEMHLN = (base.Owner.IsLeftDir ? base.BreakEffectList[i].PrefabLeft : base.BreakEffectList[i].Prefab);
					SkillEffectCtrl effect = battleEffectPool.GetEffect(mDOJNMEMHLN);
					effect.transform.parent = ExceptUIRoot.Transform;
					effect.InitializeSort();
					effect.PlaySe(base.Owner.ComponentSound.SoundUnitId, base.Owner.IsLeftDir);
					effect.SetPossitionAppearanceType(base.BreakEffectList[i], base.Owner.ComponentMultiPartsBoss.GetFirstParts(true, 0f), base.Owner, null);
					effect.ExecAppendCoroutine(base.Owner);
				}
			}

		}

		[global::OpenPCR.Migration("e77d4d6e-09d3-4a59-9728-b1b2e89aa56e")]
		private IEnumerator waitAndPlayBreak(float _timer, Transform _unitUiCtrl)
		{
			while (true)
			{
				_timer -= base.Owner.DeltaTimeForPause;
				if (_timer < 0f)
				{
					break;
				}
				yield return null;
			}
			// After the original strict-negative pause-clock wait, only cursor animation,
			// render transforms and muted sound remain. Coroutine lifetime is unchanged.

		}

		[global::OpenPCR.Migration("43aa0468-8581-4fbf-b767-38ad20dd33f2")]
		private IEnumerator updateChangeAttachment(float _timer, bool _enable)
		{
			while (true)
			{
				_timer -= base.Owner.DeltaTimeForPause;
				if (_timer < 0f)
				{
					break;
				}

				if (IsBreak != _enable)
				{
					yield break;
				}

				yield return null;
			}

			BattleAnimKitController currentSpineCtrl = base.Owner.GetCurrentSpineCtrl();
			if (_enable)
			{
				foreach (AttachmentNamePair attachmentNamePair in AttachmentNamePairList)
				{
					((currentSpineCtrl.skeleton.skin == null) ? currentSpineCtrl.skeleton.data.defaultSkin : currentSpineCtrl.skeleton.skin).AddAttachment(attachmentNamePair.TargetIndex, attachmentNamePair.TargetAttachmentName, attachmentNamePair.AppliedAttachment);
					currentSpineCtrl.skeleton.slots.Items[attachmentNamePair.TargetIndex].attachment = attachmentNamePair.AppliedAttachment;
				}

				yield break;
			}

			foreach (AttachmentNamePair attachmentNamePair2 in AttachmentNamePairList)
			{
				((currentSpineCtrl.skeleton.skin == null) ? currentSpineCtrl.skeleton.data.defaultSkin : currentSpineCtrl.skeleton.skin).AddAttachment(attachmentNamePair2.TargetIndex, attachmentNamePair2.TargetAttachmentName, attachmentNamePair2.TargetAttachment);
				currentSpineCtrl.skeleton.slots.Items[attachmentNamePair2.TargetIndex].attachment = attachmentNamePair2.TargetAttachment;
			}
		}

		[global::OpenPCR.Migration("6623f579-776a-4dd2-9009-02e61cd89bfc")]
		public void FixAttachment(UnitCtrl _owner)
		{
			BattleAnimKitController currentSpineCtrl = _owner.GetCurrentSpineCtrl();
			foreach (AttachmentNamePair attachmentNamePair in AttachmentNamePairList)
			{
				((currentSpineCtrl.skeleton.skin == null) ? currentSpineCtrl.skeleton.data.defaultSkin : currentSpineCtrl.skeleton.skin).AddAttachment(attachmentNamePair.TargetIndex, attachmentNamePair.TargetAttachmentName, attachmentNamePair.TargetAttachment);
				currentSpineCtrl.skeleton.slots.Items[attachmentNamePair.TargetIndex].attachment = attachmentNamePair.TargetAttachment;
			}
		}

		[global::OpenPCR.Migration("2ca081be-0811-4b47-8747-dc7a05538379")]
		public void DisableCursor()
		{
			if ((object)MultiTargetCursor != null)
				MultiTargetCursor.SetActive(false);
		}

		[global::OpenPCR.Migration("4981e90a-6a7d-4ac2-807b-26075c7f8ab4")]
		private IEnumerator waitAndBreakPointReset()
		{
			float time = 1.12f;
			while (time > 0f)
			{
				time -= battleManager.DeltaTime;
				yield return null;
			}
			IsBreak = false;
			Owner.AppendCoroutine(updateChangeAttachment(ChangeAttachmentEndTime, false), ePauseType.SYSTEM, Owner);
			BreakPoint = (long)MaxBreakPoint;
			OnBreakEnd.Call();
		}
	}
}
