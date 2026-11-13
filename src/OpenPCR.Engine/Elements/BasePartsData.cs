using global::Elements;
using global::OpenPCR.AnimKit;
using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System;
using System.Linq;
using Elements.Data;
using Elements.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Elements.Battle;
using Elements.Battle.Core;

namespace Elements
{
	[Serializable]
	public class BasePartsData
	{
		[global::OpenPCR.Migration("2a72b16c-1441-40f9-b790-761f55e18b33")]
		public virtual long GetDefZero()
		{
			return componentParameterClamped.DefZero;
		}
		[global::OpenPCR.Migration("35b8561b-d20f-4954-a040-ef0e35299cee")]
		public virtual void SetBuffDebuff(bool _enable, long _value, BuffParamKind _kind, UnitCtrl _source, IBattleLog _battleLog, bool _additional)
		{
			if (!_enable)
				_value = unchecked(-_value);
			_battleLog.AppendBattleLog(eBattleLogType.SET_BUFF_DEBUFF, (_enable ? 10 : 20) + (int)_kind, _value, 0L, 0, 0, 1, 1, _source, Owner);
			if (_additional)
			{
				Dictionary<BuffParamKind, long> dictionary = Owner.ComponentAbnormal.AdditionalBuffDictionary;
				if (dictionary.ContainsKey(_kind))
					dictionary[_kind] = unchecked(dictionary[_kind] + _value);
				else
					dictionary.Add(_kind, _value);
				return;
			}
			switch (_kind)
			{
				case BuffParamKind.ATK:
					componentParameter.Atk = unchecked((long)componentParameter.Atk + _value);
					break;
				case BuffParamKind.MAGIC_STR:
					componentParameter.MagicStr = unchecked((long)componentParameter.MagicStr + _value);
					break;
				case BuffParamKind.DODGE:
					componentParameter.Dodge = unchecked((long)componentParameter.Dodge + _value);
					break;
				case BuffParamKind.PHYSICAL_CRITICAL:
					componentParameter.PhysicalCritical = unchecked((long)componentParameter.PhysicalCritical + _value);
					break;
				case BuffParamKind.MAGIC_CRITICAL:
					componentParameter.MagicCritical = unchecked((long)componentParameter.MagicCritical + _value);
					break;
				case BuffParamKind.ENERGY_RECOVER_RATE:
					componentParameter.EnergyRecoveryRate = unchecked((long)componentParameter.EnergyRecoveryRate + _value);
					break;
				case BuffParamKind.LIFE_STEAL:
					componentParameter.LifeSteal = unchecked((long)componentParameter.LifeSteal + _value);
					break;
				case BuffParamKind.PHYSICAL_CRITICAL_DAMAGE_RATE:
					componentParameter.PhysicalCriticalDamageRate = unchecked((long)componentParameter.PhysicalCriticalDamageRate + _value);
					break;
				case BuffParamKind.MAGIC_CRITICAL_DAMAGE_RATE:
					componentParameter.MagicCriticalDamageRate = unchecked((long)componentParameter.MagicCriticalDamageRate + _value);
					break;
				case BuffParamKind.ACCURACY:
					componentParameter.Accuracy = unchecked((long)componentParameter.Accuracy + _value);
					break;
				case BuffParamKind.PHYSICAL_DAMAGE_UP_PERCENT:
					componentParameter.AdditionalPhysicalDamageUpPercent = unchecked((long)componentParameter.AdditionalPhysicalDamageUpPercent + _value);
					break;
				case BuffParamKind.MAGIC_DAMAGE_UP_PERCENT:
					componentParameter.AdditionalMagicDamageUpPercent = unchecked((long)componentParameter.AdditionalMagicDamageUpPercent + _value);
					break;
				case BuffParamKind.BUFF_ENHANCE:
					componentParameter.BuffEnhance = unchecked((long)componentParameter.BuffEnhance + _value);
					break;
				case BuffParamKind.DEF:
					componentParameter.Def = unchecked((long)componentParameter.Def + _value);
					componentParameter.DefForDamagedEnergy = unchecked((long)componentParameter.DefForDamagedEnergy + _value);
					break;
				case BuffParamKind.MAGIC_DEF:
					componentParameter.MagicDef = unchecked((long)componentParameter.MagicDef + _value);
					componentParameter.MagicDefForDamagedEnergy = unchecked((long)componentParameter.MagicDefForDamagedEnergy + _value);
					break;
				case BuffParamKind.RECEIVE_CRITICAL_DAMAGE_RATE:
					componentParameter.ReceiveCriticalDamageRate = unchecked((long)componentParameter.ReceiveCriticalDamageRate - _value);
					break;
				case BuffParamKind.RECEIVE_PHYSICAL_AND_MAGIC_DAMAGE_PERCENT:
					componentParameter.AdditionalPhysicalAndMagicReceiveDamagePercent = unchecked((long)componentParameter.AdditionalPhysicalAndMagicReceiveDamagePercent - _value);
					break;
				case BuffParamKind.RECEIVE_PHYSICAL_DAMAGE_PERCENT:
					componentParameter.AdditionalPhysicalReceiveDamagePercent = unchecked((long)componentParameter.AdditionalPhysicalReceiveDamagePercent - _value);
					break;
				case BuffParamKind.RECEIVE_MAGIC_DAMAGE_PERCENT:
					componentParameter.AdditionalMagicReceiveDamagePercent = unchecked((long)componentParameter.AdditionalMagicReceiveDamagePercent - _value);
					break;
				case BuffParamKind.MOVE_SPEED:
					componentParameter.MoveSpeed = (float)componentParameter.MoveSpeed + (float)_value;
					break;
				case BuffParamKind.MAX_HP:
					if (unchecked((long)componentParameter.MaxHp + _value) > 0L)
					{
						componentParameter.MaxHp = unchecked((long)componentParameter.MaxHp + _value);
						if ((long)componentParameter.MaxHp < (long)componentParameter.Hp)
							componentParameter.SetCurrentHp((long)componentParameter.MaxHp);
						if (_enable)
						{
							double rate = (double)(long)componentParameter.Hp / (double)(long)componentParameter.MaxHp;
							foreach (KeyValuePair<int, Action<double>> callback in Owner.ComponentDamageControl.OnRecoverListForChangeSpeedDisableByMaxHp)
								callback.Value.Call(rate);
						}
					}
					else if ((long)componentParameter.Hp > 0L)
					{
						Owner.ComponentParameter.Hp = 0L;
						Owner.ComponentAbnormal.ClearKnightGuard();
						Owner.ComponentAbnormal.ClearGuts();
						if (!Owner.ComponentState.IsDead && Owner.ComponentState.CurrentState < ActionState.DIE)
							Owner.ComponentState.SetState(ActionState.DIE, 0, 0, false);
					}
					break;
			}
		}
		[Serializable]
		public class AttachmentNamePair
		{
			public string TargetSlotName = string.Empty;
			public string TargetAttachmentName = string.Empty;
			public string AppliedSlotName = string.Empty;
			public string AppliedAttachmentName = string.Empty;
			public int TargetIndex { get; set; }
			public Attachment TargetAttachment { get; set; }
			public Attachment AppliedAttachment { get; set; }


		}

		private const int ALL_DETAIL_VALUE = -1;
		private int currentAbnormalResistId;
		private float lastHealEffectTime;
		private Dictionary<int, int> talentWeaknessPercent;
		public int StartAbnormalResistId { get; private set; }
		public bool PassiveUbIsMagic { get; set; }
		public int AbnormalResistIdSetCount { get; private set; }
		[global::OpenPCR.ImplicitMigration("dd7fcade-1fc5-4f97-a226-6c8568145d52")][global::OpenPCR.Migration("373b0ecd-eaae-49de-bf2a-123034a4239c")] private Dictionary<eActionType, Dictionary<int, int>> abnormalResistStatusDictionary => abnormalResistIdDictionary[currentAbnormalResistId];
		[global::OpenPCR.ImplicitMigration("54a98fe8-4304-4265-b80a-fc7659083dff")][global::OpenPCR.Migration("24167aef-6c42-4e66-af0c-0e4f6deeaf03")] public List<int> TalentWeaknessIds => global::OpenPCR.Presentation.Noop<global::System.Collections.Generic.List<int>>();

		private int currentDebuffResistId;
		public float PositionX;
		public float PositionY;
		public List<AttachmentNamePair> AttachmentNamePairList = new List<AttachmentNamePair>();
		public bool DisableStartBreakEffect;
		public float ChangeAttachmentStartTime;
		public float ChangeAttachmentEndTime;
		public float BodyWidthValue;
		public int Index;
		public int EnemyId = 100000001;
		public float BreakEffectTime;
		public float BreakEffectOffsetY;
		public float BreakEffectSize = 1f;
		private Dictionary<int, Dictionary<eActionType, Dictionary<int, int>>> abnormalResistIdDictionary = new Dictionary<int, Dictionary<eActionType, Dictionary<int, int>>>();
		private Dictionary<int, Dictionary<BuffParamKind, int>> debuffResistIdDictionary = new Dictionary<int, Dictionary<global::Elements.Battle.Core.BuffParamKind, int>>();
		public int StartDebuffResistId { get; private set; }
		public List<NormalSkillEffect> BreakEffectList { get; set; }
		public float InitialPositionX { get; set; }
		private UnitCtrl owner;
		protected UnitComponentParameter componentParameter { get; set; }
		protected UnitComponentParameterClamped componentParameterClamped { get; set; }

		[global::OpenPCR.Migration("95def0c3-5df3-4f78-9d08-b101a7e19d2d")]
		public UnitCtrl Owner
		{
			[global::OpenPCR.Migration("cdcc6e1b-ed2b-4ac7-9428-18159e9abe17")]
			get { return owner; }
			[global::OpenPCR.Migration("f080e9ac-ea5d-473b-9264-df9bf30e3394")]
			set
			{
				owner = value;
				componentParameter = owner.ComponentParameter;
				componentParameterClamped = owner.ComponentParameterClamped;
			}
		}
		public long TotalDamage { get; set; }
		protected BattleManager battleManager { get; set; }
		public int UbAttackHitCount { get; set; }
		public bool IsBlackoutTarget { get; set; }
		public int DebuffResistIdSetCount { get; private set; }
		[global::OpenPCR.ImplicitMigration("f7b01925-8153-4ae1-ac6d-a64a8443a62a")][global::OpenPCR.Migration("e1a8ba92-53ef-48b5-800c-43daf35a9213")] private Dictionary<BuffParamKind, int> debuffResistStatusDictionary => debuffResistIdDictionary[currentDebuffResistId];

		[global::OpenPCR.Migration("71d6d84a-a964-407d-a62e-50396f272a3f")]
		public BasePartsData()
		{

		}

		[global::OpenPCR.Migration("9e9540b1-d14b-43f3-a6c3-3199f4a043ad")]
		public void SetBattleManager(BattleManager _battleManager)
		{
			battleManager = _battleManager;
		}

		[global::OpenPCR.Migration("ab92d94d-352a-4c9a-a687-a49e1c5f7cea")]
		public virtual Vector3 GetPosition()
		{
			return Owner.transform.position;
		}

		[global::OpenPCR.Migration("0f80a11c-3c7c-4c00-b7f4-dfa907a2fe2e")]
		public virtual Vector3 GetLocalPosition()
		{
			return Owner.transform.localPosition;
		}

		[global::OpenPCR.Migration("52c23db3-caa5-4098-8a60-c6fd27f49fc3")]
		public virtual float GetBodyWidth()
		{
			return Owner.BodyWidth;
		}

		[global::OpenPCR.Migration("0e22ea88-b765-4e79-b7af-324ec1b2c75f")]
		public virtual Vector3 GetBottomTransformPosition()
		{
			if (Owner == null)
			{
				return Vector3.zero;
			}

			return Owner.BottomTransform.position;
		}

		[global::OpenPCR.Migration("ff777622-57ad-45b1-8f0a-0f69ea795f93")]
		public virtual Vector3 GetOwnerBottomTransformPosition()
		{
			return Owner.BottomTransform.position;
		}

		[global::OpenPCR.Migration("42884810-d54d-48ff-9243-59ddaff201cb")]
		public Vector3 GetBottomLossyScale()
		{
			return Owner.BottomTransform.lossyScale;
		}

		[global::OpenPCR.Migration("fb2acc2e-3fda-4503-ac66-f26c22b58feb")]
		public virtual Bone GetStateBone()
		{
			return Owner.StateBone;
		}

		[global::OpenPCR.Migration("d4abfbb3-ff3d-4445-924c-ebfb5021034e")]
		public virtual Bone GetCenterBone()
		{
			return Owner.CenterBone;
		}

		[global::OpenPCR.Migration("1f1bc112-c510-460d-9e2f-2d8d32ddf94c")]
		public virtual Vector3 GetFixedCenterPos()
		{
			return Owner.FixedCenterPos;
		}

		[global::OpenPCR.Migration("6cf8c468-7047-4588-9e60-79040c6774c6")]
		public virtual Vector3 GetColliderCenter()
		{
			return Owner.ColliderCenter;
		}

		[global::OpenPCR.Migration("861858a5-4054-4cba-b136-b6576a547dda")]
		public virtual Vector3 GetColliderSize()
		{
			return Owner.ColliderSize;
		}

		[global::OpenPCR.Migration("0f5a0d0b-41b5-4bef-8f14-d6bdb1b21eda")]
		public virtual int GetLevel()
		{
			return Owner.ComponentParameter.Level;
		}

		[global::OpenPCR.Migration("4926c0ff-e5bf-41c3-a60a-21848799fc81")]
		public void IncrementUbAttackHitCount()
		{
			UbAttackHitCount++;
		}

		[global::OpenPCR.Migration("df823448-0c63-4944-ae43-e180381c39e2")]
		public virtual long GetAtkZero()
		{
			return componentParameterClamped.AtkZero;
		}

		[global::OpenPCR.Migration("3d543ba9-3dd8-4b64-b46c-8a9d44e7aa81")]
		public virtual long GetMagicStrZero()
		{
			return componentParameterClamped.MagicStrZero;
		}

		[global::OpenPCR.Migration("179a390b-fd0c-45fb-a72d-97bd1a4bb89e")]
		public virtual long GetAccuracyZero()
		{
			return componentParameterClamped.AccuracyZero;
		}

		[global::OpenPCR.Migration("c9b37ee9-2550-4751-8eb3-1ce02a8bf76e")]
		public virtual long GetPhysicalCriticalZero()
		{
			return componentParameterClamped.PhysicalCriticalZero;
		}

		[global::OpenPCR.Migration("f78e572f-6770-4503-812c-d21994281df7")]
		public virtual long GetMagicCriticalZero()
		{
			return componentParameterClamped.MagicCriticalZero;
		}

		[global::OpenPCR.Migration("02671801-9c47-4747-b3ae-a427e6f5da24")]
		public virtual long GetLifeStealZero()
		{
			return componentParameterClamped.LifeStealZero;
		}

		[global::OpenPCR.Migration("f0930d5f-29c1-493d-8a78-2904eefe6b7d")]
		public virtual long GetHpRecoverRateZero()
		{
			return componentParameterClamped.HpRecoveryRateZero;
		}

		[global::OpenPCR.Migration("36dcabf5-e1c8-4dfd-abda-59c93848a33e")]
		public virtual float GetDodgeRate(long _accuracy)
		{
			return componentParameterClamped.GetDodgeRate(_accuracy);
		}

		[global::OpenPCR.Migration("127fb462-467b-4150-b96d-e35ec34f24fd")]
		public virtual long GetMagicDefZero()
		{
			return componentParameterClamped.MagicDefZero;
		}

		[global::OpenPCR.Migration("d3196430-2503-4238-bb66-60d309b2e562")]
		public virtual long GetDefZeroForDamagedEnergy()
		{
			return componentParameterClamped.DefZeroForDamagedEnergy;
		}

		[global::OpenPCR.Migration("63a5f803-dbda-45d9-b32b-56da52d61283")]
		public virtual long GetMagicDefZeroForDamagedEnergy()
		{
			return componentParameterClamped.MagicDefZeroForDamagedEnergy;
		}

		[global::OpenPCR.Migration("23f316d5-78cd-4c5a-9e4d-652bc70f8c2d")]
		public virtual void SetMissAtk(UnitCtrl _source, eMissLogType _missLogType, eDamageEffectType _damageEffectType, float _scale)
		{
			if (!Owner.ComponentAbnormal.IsNoDamageMotion())
				Owner.ComponentDamageControl.SetMissAtk(_source, _missLogType, _damageEffectType, null, _scale);
		}

		[global::OpenPCR.Migration("3ff1911d-201a-4eeb-85b5-1e98414c439f")]
		public void ShowHitEffect(SystemIdDefine.eWeaponSeType _weaponSeType, Skill _skill, bool _isLeft)
		{
			Owner.ComponentDamageControl.ShowHitEffect(_weaponSeType, _skill, _isLeft, this);
		}

		[global::OpenPCR.Migration("53edd8a4-1508-4a6d-9b12-85ef29a1a650")]
		public virtual long GetStartAtk()
		{
			return componentParameter.StartAtk;
		}

		[global::OpenPCR.Migration("22adc6a1-35b2-43a9-a4fc-6839bd2b77e5")]
		public virtual long GetStartDef()
		{
			return componentParameter.StartDef;
		}

		[global::OpenPCR.Migration("0b4614ff-801a-4c5a-ad34-b18ea1dd0342")]
		public virtual long GetStartMagicStr()
		{
			return componentParameter.StartMagicStr;
		}

		[global::OpenPCR.Migration("07e67b89-f9e1-4f42-bb1a-668b1c8353f3")]
		public virtual long GetStartMagicDef()
		{
			return componentParameter.StartMagicDef;
		}

		[global::OpenPCR.Migration("863e63bf-bb8e-4de6-aa07-e692fd6d87fe")]
		public virtual long GetStartDodge()
		{
			return componentParameter.StartDodge;
		}

		[global::OpenPCR.Migration("83367616-7054-4549-90f9-67895c12f0fc")]
		public virtual long GetStartPhysicalCritical()
		{
			return componentParameter.StartPhysicalCritical;
		}

		[global::OpenPCR.Migration("ffb42d6d-ad5e-4895-855a-4e7ea785a284")]
		public virtual long GetStartMagicCritical()
		{
			return componentParameter.StartMagicCritical;
		}

		[global::OpenPCR.Migration("53d7c761-f7e8-4bd1-a18a-e95a3b080cbc")]
		public virtual long GetStartLifeSteal()
		{
			return componentParameter.StartLifeSteal;
		}

		[global::OpenPCR.Migration("331b7944-496a-4894-90ac-d60fc34d9a8c")]
		public IEnumerator StartTotalDamage(bool _isLarge, float _delay, UnitCtrl _unit)
		{
			long totalDamage = TotalDamage;
			yield return new WaitForSeconds(_delay);
			DamageEffectCtrlBase effect = Owner.ComponentDamageControl.CreateDamageNumEffect(totalDamage, _isLarge ? battleManager.TotalDamageEffectPrefabLarge : battleManager.TotalDamageEffectPrefabSmall, null, PassiveUbIsMagic, _unit, DamageData.eDamageSoundType.HIT, this, 1f, false, false, true);
			if (effect != null)
				effect.SetSortOrder((int)Owner.RespawnPos + (Owner.ComponentSortOrder.IsFront ? 22750 : 11250));
		}

		[global::OpenPCR.Migration("e92da986-8366-49c6-b0ee-5cc418821f13")]
		public bool JudgeShowTotalDamage()
		{
			if (UbAttackHitCount < 2)
			{
				return false;
			}

			if (TotalDamage == 0L)
			{
				return false;
			}

			return true;
		}

		[global::OpenPCR.Migration("21950e9e-8fbd-4e26-ae77-75ed323561c9")]
		public virtual bool GetTargetable()
		{
			return true;
		}

		[global::OpenPCR.Migration("afefb07f-d1dc-4dbf-b349-1ad9bae32ea7")]
		public void SetDebuffResistId(int _resistId, bool _isInit)
		{
			if (!debuffResistIdDictionary.ContainsKey(_resistId))
			{
				createDebuffStatus(_resistId);
			}

			currentDebuffResistId = _resistId;
			if (_isInit)
			{
				StartDebuffResistId = _resistId;
			}

			DebuffResistIdSetCount++;
		}

		[global::OpenPCR.Migration("0a6b1f12-9cf3-4d07-ac56-a425efb74058")]
		private void createResistStatus(int _resistId)
		{
			abnormalResistIdDictionary.AddIfNoContains(_resistId, new Dictionary<eActionType, Dictionary<int, int>>());
			if (_resistId == 0)
				return;
			MasterDataManager manager = ManagerSingleton<MasterDataManager>.Instance;
			MasterResistData.ResistData resist = manager.masterResistData.Get(_resistId);
			for (int i = 0; i < resist.Ailments.Length; i++)
			{
				MasterAilmentData.AilmentData ailment = manager.masterAilmentData.Get(i + 1);
				if (ailment == null)
					break;
				eActionType kind = (eActionType)(int)ailment.ailment_action;
				if (!abnormalResistIdDictionary[_resistId].ContainsKey(kind))
					abnormalResistIdDictionary[_resistId].Add(kind, new Dictionary<int, int>());
				abnormalResistIdDictionary[_resistId][kind][(int)ailment.ailment_detail_1] = resist.Ailments[i];
			}
		}

		[global::OpenPCR.Migration("2aad91f9-5748-4829-a61e-e3206e885565")]
		private void createDebuffStatus(int _resistId)
		{
			debuffResistIdDictionary.AddIfNoContains(_resistId, new Dictionary<BuffParamKind, int>());
			if (_resistId != 0)
			{
				MasterResistVariationData.ResistVariationData resist = ManagerSingleton<MasterDataManager>.Instance.masterResistVariationData.Get(_resistId);
				for (int i = 0; i < 4; i++)
					debuffResistIdDictionary[_resistId][(BuffParamKind)(i + 1)] = resist.DebuffDecrement[i];
			}
		}

		[global::OpenPCR.Migration("6be128a6-8dc5-4b19-af67-786814e62dc7")]
		public int GetDebuffResistPercent(BuffParamKind _buffParamKind)
		{
			debuffResistStatusDictionary.TryGetValue(_buffParamKind, out var value);
			return value;
		}

		[global::OpenPCR.Migration("bd860278-66c7-4a26-be2e-d9bcda5ad2fd")]
		public Vector3 GetBottomTransformLocalPosition()
		{
			return Owner.BottomTransform.localPosition;
		}

		[global::OpenPCR.Migration("29e9330d-dd8e-4d84-b2f9-5d0eb1d00c50")]
		public void ResetTotalDamage()
		{
			TotalDamage = 0L;
			UbAttackHitCount = 0;
		}

		[global::OpenPCR.Migration("aedb1b13-214f-45aa-862e-ce2e73735274")]
		public void SetAbnormalResistId(int _resistId, bool _isInit)
		{
			if (!abnormalResistIdDictionary.ContainsKey(_resistId))
				createResistStatus(_resistId);
			currentAbnormalResistId = _resistId;
			if (_isInit)
				StartAbnormalResistId = _resistId;
			AbnormalResistIdSetCount++;
		}

		[global::OpenPCR.Migration("787824ea-0c83-4a10-a6c0-bdc272e1b550")]
		public bool ResistStatus(eActionType _actionType, int _detail1, UnitCtrl _source, bool _last, bool _targetOneParts, BasePartsData _target)
		{
			if (!abnormalResistStatusDictionary.ContainsKey(_actionType))
				return false;
			if (_target.Owner.ComponentAbnormal.IsForceHit(_actionType, _detail1))
				return false;
			Dictionary<int, int> dictionary = abnormalResistStatusDictionary[_actionType];
			if (dictionary.ContainsKey(ALL_DETAIL_VALUE))
				_detail1 = ALL_DETAIL_VALUE;
			if (!dictionary.ContainsKey(_detail1))
				return false;
			float random = BattleManager.Random(0f, 1f);
			bool resisted = (double)dictionary[_detail1] / 100d > (double)random;
			if (resisted && _last)
				Owner.ComponentDamageControl.SetMissAtk(_source, eMissLogType.DODGE_ATTACK, eDamageEffectType.NORMAL, _targetOneParts ? _target : null, 1f);
			return resisted;
		}

		[global::OpenPCR.Migration("1cfc6c67-8e27-473b-a0fd-e4aa0fc6887a")]
		public void SetTalentWeakness(Dictionary<int, int> _talentWeaknessPercent)
		{
			talentWeaknessPercent = _talentWeaknessPercent;
		}

		[global::OpenPCR.Migration("c4fc6c74-8a38-4dfa-97e5-60f7c9b4d1fe")]
		public double GetTalentWeaknessPercent(List<int> _talentIds)
		{
			if (_talentIds.Count == 0 || talentWeaknessPercent == null)
				return 100d;
			return (double)_talentIds.Select(id => talentWeaknessPercent.GetValueOrDefault(id, 100)).Max();
		}

		[global::OpenPCR.Migration("9053d170-e245-418e-90aa-0ba0b17f1f39")]
		public void RecoveryEffect(UnitCtrl _source, bool _isEnergy, IBattleEffectPool _battleEffectPool, bool _isRegenerate = false)
		{
			if (!battleManager.CanCreateBattleEffect())
				return;
			float time = Time.time;
			if (!(time >= lastHealEffectTime + 0.45f))
				return;
			BattleResourceManager resources = Singleton<BattleResourceManager>.Instance;
			GameObject prefab = _isEnergy
				? (_isRegenerate && resources.RegenerateEnergyHealPrefab != null ? resources.RegenerateEnergyHealPrefab : resources.EnergyHealPrefab)
				: (_isRegenerate && resources.RegenerateEffectPrefab != null ? resources.RegenerateEffectPrefab : resources.RevivalEffectFrontPrefab);
			if (prefab == null || !BattleEffectUtility.IsRecoverEffectEnable(battleManager.BattleCategory))
				return;
			GameObject effectObject = _battleEffectPool.GetEffect(prefab, null).gameObject;
			effectObject.transform.localScale = prefab.transform.localScale;
			effectObject.transform.position = prefab.transform.position + GetPosition();
			effectObject.transform.parent = ExceptUIRoot.Transform;
			SkillEffectCtrl effect = effectObject.GetComponent<SkillEffectCtrl>();
			effect.InitializeSort();
			effect.SortTarget = Owner;
			effect.SetSortOrderBack();
			effect.ExecAppendCoroutine(_source, false);
			if (Owner.gameObject.activeSelf)
				Owner.StartCoroutine(effect.TrackTarget(this, Vector3.zero, true, true, GetCenterBone(), false, SkillEffectCtrl.eToadTrackType.NONE));
			effect.PlaySe(Owner.ComponentSound.SoundUnitId, Owner.IsLeftDir);
			lastHealEffectTime = time;
		}
	}
}
