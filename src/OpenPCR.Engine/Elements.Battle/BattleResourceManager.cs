using global::OpenPCR.Runtime.Api;
using Elements.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Elements.Battle.Core;

namespace Elements.Battle
{
	public class BattleResourceManager : Singleton<BattleResourceManager>
	{
		[global::OpenPCR.Migration("5e750e02-b906-41ee-b850-08780f1b05f3")]
		public void LoadAcnEffectPrefab()
		{
			abnormalEffectPrefabDic[1].Add((eAbnormalState)78, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_PUPPET, eResourceId.FX_STATE_PUPPET_L, false, 0));
			abnormalEffectPrefabDic[1].Add((eAbnormalState)77, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_CHAINED_ACN, eResourceId.FX_STATE_CHAINED_ACN_L, false, 0));
		}

		public void Init()
		{
			resourceMgr = ManagerSingleton<ResourceManager>.Instance;
			abnormalEffectPrefabDic = new Dictionary<int, Dictionary<eAbnormalState, AbnormalStateEffectPrefabData>>();
			AttackHitCenterEffectPrefabDic = new Dictionary<eResourceId, GameObject>(new eResourceId_DictComparer());
		}
		public GameObject SealActionEffectPrefab { get; private set; }
		public global::OpenPCR.Runtime.Api.GameObject EndebuffEffectPrefab { get; private set; }
		public global::OpenPCR.Runtime.Api.GameObject EnbuffEffectPrefab { get; private set; }
		public global::System.Collections.Generic.Dictionary<System.Int32, global::OpenPCR.Runtime.Api.GameObject> StrikeBackEffectDic { get; set; } = new Dictionary<int, GameObject>();
		public global::System.Collections.Generic.Dictionary<System.Int32, global::OpenPCR.Runtime.Api.GameObject> StrikeBackEffectLeftDic { get; set; } = new Dictionary<int, GameObject>();
		public global::System.Collections.Generic.Dictionary<Elements.Battle.Core.DamageByBehaviourAction.eDamageByBehaviourEffectType, global::OpenPCR.Runtime.Api.GameObject> DamageByBehaviourEffectPrefab { get; private set; } = new Dictionary<DamageByBehaviourAction.eDamageByBehaviourEffectType, GameObject>();
		public global::System.Collections.Generic.Dictionary<System.Int32, global::OpenPCR.Runtime.Api.GameObject> AttackFieldEffectPrefabDic { get; private set; }

		[global::OpenPCR.Migration("7c9275fb-9a52-4380-8734-1135418e3c6f")]
		public GameObject GetAttaclFieldEffect(int _effectId)
		{
			AttackFieldEffectPrefabDic.TryGetValue(_effectId, out var value);
			return value;
		}

		public global::OpenPCR.Runtime.Api.GameObject UnitDamageNumEffectPrefab { get; private set; }
		public global::OpenPCR.Runtime.Api.GameObject RecoveryNumEffectPrefab { get; private set; }
		public global::OpenPCR.Runtime.Api.GameObject HealFieldEffectPrefab { get; private set; }
		public global::OpenPCR.Runtime.Api.GameObject DebuffFieldEffectPrefab { get; private set; }
		public global::OpenPCR.Runtime.Api.GameObject ChargeEnergyFieldHealEffectPrefab { get; private set; }
		public global::OpenPCR.Runtime.Api.GameObject ChargeEnergyFieldDamageEffectPrefab { get; private set; }

		[global::OpenPCR.Migration("22288be7-7fff-4f99-9816-7452b84ed7d3")]
		public void LoadLegacyStrikeBackEffect(int _effectType)
		{
			if (StrikeBackEffectDic.ContainsKey(_effectType))
				return;
			switch (_effectType)
			{
				case 1:
					StrikeBackEffectDic[1] = resourceMgr.LoadResourceImmediately(eResourceId.FX_TOY_STRIKE_BACK, 0L) as GameObject;
					StrikeBackEffectLeftDic[1] = resourceMgr.LoadResourceImmediately(eResourceId.FX_TOY_STRIKE_BACK, 0L) as GameObject;
					break;
				case 2:
					StrikeBackEffectDic[2] = resourceMgr.LoadResourceImmediately(eResourceId.FX_OMEME_STRIKE_BACK, 0L) as GameObject;
					StrikeBackEffectLeftDic[2] = resourceMgr.LoadResourceImmediately(eResourceId.FX_OMEME_STRIKE_BACK_LEFT, 0L) as GameObject;
					break;
				case 3:
					StrikeBackEffectDic[3] = resourceMgr.LoadResourceImmediately(eResourceId.FX_OMEME_PLUS_STRIKE_BACK, 0L) as GameObject;
					StrikeBackEffectLeftDic[3] = resourceMgr.LoadResourceImmediately(eResourceId.FX_OMEME_PLUS_STRIKE_BACK_LEFT, 0L) as GameObject;
					break;
				case 4:
					StrikeBackEffectDic[4] = resourceMgr.LoadResourceImmediately(eResourceId.FX_LIGHT_SHIELD_STRIKE_BACK, 0L) as GameObject;
					StrikeBackEffectLeftDic[4] = resourceMgr.LoadResourceImmediately(eResourceId.FX_LIGHT_SHIELD_STRIKE_BACK_LEFT, 0L) as GameObject;
					break;
			}
		}

		[global::OpenPCR.Migration("61030576-4b32-4388-b515-e25039b1cac1")]
		public void LoadStrikeBackEffect(int _index)
		{
			if (!StrikeBackEffectDic.ContainsKey(_index))
			{
				StrikeBackEffectDic[_index] = resourceMgr.LoadResourceImmediately(eResourceId.FX_STRIKE_BACK, _index) as GameObject;
				StrikeBackEffectLeftDic[_index] = resourceMgr.LoadResourceImmediately(eResourceId.FX_STRIKE_BACK_LEFT, _index) as GameObject;
			}
		}

		public global::OpenPCR.Runtime.Api.GameObject RunSmokeLPrefab { get; private set; }
		public global::OpenPCR.Runtime.Api.GameObject RunSmokeRPrefab { get; private set; }

		private Dictionary<int, Dictionary<eAbnormalState, AbnormalStateEffectPrefabData>> abnormalEffectPrefabDic;
		private ResourceManager resourceMgr;
		public Dictionary<eResourceId, GameObject> AttackHitCenterEffectPrefabDic { get; private set; }
		public GameObject LevelUpPrefab { get; private set; }
		public GameObject MaxHpDebuffPrefabRight { get; private set; }
		public GameObject MaxHpDebuffPrefabLeft { get; private set; }
		public GameObject RegenerateEnergyHealPrefab { get; private set; }
		public GameObject EnergyHealPrefab { get; private set; }
		public GameObject RegenerateEffectPrefab { get; private set; }
		public GameObject RevivalEffectFrontPrefab { get; private set; }

		[global::OpenPCR.Migration("6053607c-3214-4e67-a03d-9352f2fc8c4d")]
		public void LoadAttackFieldEffect(int _effectId)
		{
			if (AttackFieldEffectPrefabDic == null)
			{
				AttackFieldEffectPrefabDic = new Dictionary<int, GameObject>();
			}

			resourceMgr.AddLoadResourceId(eResourceId.ATTACK_FIELD_EFFECT, _effectId, delegate (object GLDFPJIMHNE)
			{
				if (!AttackFieldEffectPrefabDic.ContainsKey(_effectId))
				{
					AttackFieldEffectPrefabDic.Add(_effectId, GLDFPJIMHNE as GameObject);
				}
				else if (AttackFieldEffectPrefabDic[_effectId] == null)
				{
					AttackFieldEffectPrefabDic[_effectId] = GLDFPJIMHNE as GameObject;
				}
			});
		}

		[global::OpenPCR.Migration("f2b5679a-5dfb-4c41-bcfd-83749fa94690")]
		public void LoadDebuffFieldEffect()
		{
			if (DebuffFieldEffectPrefab == null)
			{
				Singleton<BattleUnitLoader>.Instance.AddLoadEffect(eResourceId.DEBUFF_FIELD_EFFECT_170, delegate (object EDGLKHFFDDL)
				{
					DebuffFieldEffectPrefab = EDGLKHFFDDL as GameObject;
				});
			}
		}

		[global::OpenPCR.Migration("347c28a8-0f9e-4d71-a8e3-a9b275553270")]
		public void LoadChargeEnergyFieldEffect()
		{
			if (ChargeEnergyFieldHealEffectPrefab == null)
			{
				Singleton<BattleUnitLoader>.Instance.AddLoadEffect(eResourceId.FX_ENERGY_FIELD_MAGICCIRCLE, delegate (object EDGLKHFFDDL)
				{
					ChargeEnergyFieldHealEffectPrefab = EDGLKHFFDDL as GameObject;
				});
			}

			if (ChargeEnergyFieldDamageEffectPrefab == null)
			{
				Singleton<BattleUnitLoader>.Instance.AddLoadEffect(eResourceId.FX_ENERGY_DAMAGEFIELD_MAGICCIRCLE, delegate (object EDGLKHFFDDL)
				{
					ChargeEnergyFieldDamageEffectPrefab = EDGLKHFFDDL as GameObject;
				});
			}
		}

		[global::OpenPCR.Migration("03271991-e886-4735-9500-fc2efe816e36")]
		public void LoadHealFieldEffect()
		{
			if (HealFieldEffectPrefab == null)
			{
				Singleton<BattleUnitLoader>.Instance.AddLoadEffect(eResourceId.HEAL_FIELD_EFFECT_170, delegate (object EDGLKHFFDDL)
				{
					HealFieldEffectPrefab = EDGLKHFFDDL as GameObject;
				});
			}
		}

		[global::OpenPCR.Migration("106ea37b-22cb-4cc1-8e6d-031b5fa9650b")]
		public void LoadSealActionEffect()
		{
			if (SealActionEffectPrefab == null)
			{
				Singleton<BattleUnitLoader>.Instance.AddLoadEffect(eResourceId.FX_STATE_SEAL_170, delegate (object JPBMABCKONG)
				{
					SealActionEffectPrefab = JPBMABCKONG as GameObject;
				});
			}
		}

		[global::OpenPCR.Migration("17089cc1-1a60-4e5f-bec7-04765a544dac")]
		public BattleResourceManager()
		{
		}

		[global::OpenPCR.Migration("b9287943-e30b-48b9-b8eb-0a48eea44824")]
		public void LoadDamageByBehaviourEffect(DamageByBehaviourAction.eDamageByBehaviourEffectType _effectType)
		{
			if (!DamageByBehaviourEffectPrefab.ContainsKey(_effectType))
				DamageByBehaviourEffectPrefab[_effectType] = resourceMgr.LoadResourceImmediately(eResourceId.FX_DAMAGE_BY_BEHAVIOUR, (long)_effectType) as GameObject;
		}

		[global::OpenPCR.Migration("5102c6ba-692f-4f6e-a8cb-c8416846d3a0")]
		private void loadAbnormalEffectPrefab()
		{
			abnormalEffectPrefabDic[1] = new Dictionary<eAbnormalState, AbnormalStateEffectPrefabData>();
			Dictionary<eAbnormalState, AbnormalStateEffectPrefabData> dictionary = abnormalEffectPrefabDic[1];
			dictionary.Add((eAbnormalState)14, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_HEART, eResourceId.FX_STATE_HEART_L, true, 0));
			dictionary.Add((eAbnormalState)31, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_CONFUSION, eResourceId.FX_STATE_CONFUSION_L, true, 0));
			dictionary.Add((eAbnormalState)11, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_SLOW, eResourceId.FX_STATE_SLOW_L, false, 0));
			dictionary.Add((eAbnormalState)65, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_SLOW, eResourceId.FX_STATE_SLOW_L, false, 0));
			dictionary.Add((eAbnormalState)8, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_POISON, eResourceId.FX_STATE_POISON, true, 0));
			dictionary.Add((eAbnormalState)26, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_DECOY, eResourceId.FX_STATE_DECOY_L, false, 0));
			dictionary.Add((eAbnormalState)10, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_CURSE, eResourceId.FX_STATE_CURSE_L, false, 0));
			dictionary.Add((eAbnormalState)12, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_PARALYSIS, eResourceId.FX_STATE_PARALYSIS_L, false, 0));
			dictionary.Add((eAbnormalState)19, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_STUN, eResourceId.FX_STATE_STUN_L, false, 0));
			dictionary.Add((eAbnormalState)58, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_STUN2, eResourceId.FX_STATE_STUN2_L, false, 0));
			dictionary.Add((eAbnormalState)51, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_NPC_STUN, eResourceId.FX_STATE_NPC_STUN_L, false, 0));
			dictionary.Add((eAbnormalState)54, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_CRYSTALIZE, eResourceId.FX_STATE_CRYSTALIZE_L, false, 0));
			dictionary.Add((eAbnormalState)20, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_DETAIN, eResourceId.FX_STATE_DETAIN_L, false, 0));
			dictionary.Add((eAbnormalState)17, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_BONDAGE, eResourceId.FX_STATE_BONDAGE_L, false, 0));
			dictionary.Add((eAbnormalState)18, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_SLEEP, eResourceId.FX_STATE_SLEEP_L, false, 0));
			dictionary.Add((eAbnormalState)13, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_FREEZE, eResourceId.FX_STATE_FREEZE_L, false, 0));
			dictionary.Add((eAbnormalState)16, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_SILENCE, eResourceId.FX_STATE_SILENCE_L, false, 0));
			dictionary.Add((eAbnormalState)15, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_DARK, eResourceId.FX_STATE_DARK_L, false, 0));
			dictionary.Add((eAbnormalState)49, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_MAGIC_DARK, eResourceId.FX_STATE_MAGIC_DARK_L, false, 0));
			dictionary.Add((eAbnormalState)1, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_GUARD, eResourceId.FX_STATE_GUARD_L, false, 0));
			dictionary.Add((eAbnormalState)32, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_VENOM, eResourceId.FX_STATE_VENOM_L, true, 0));
			dictionary.Add((eAbnormalState)37, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_HEX, eResourceId.FX_STATE_HEX_L, true, 0));
			dictionary.Add((eAbnormalState)38, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_FAINT, eResourceId.FX_STATE_FAINT_L, false, 0));
			dictionary.Add((eAbnormalState)40, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_COMPENSATION, eResourceId.FX_STATE_COMPENSATION_L, false, 0));
			dictionary.Add((eAbnormalState)41, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_CUT_ATK_DAMAGE, eResourceId.FX_STATE_CUT_ATK_DAMAGE_L, false, 0));
			dictionary.Add((eAbnormalState)42, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_CUT_MGC_DAMAGE, eResourceId.FX_STATE_CUT_MGC_DAMAGE_L, false, 0));
			dictionary.Add((eAbnormalState)43, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_CUT_ALL_DAMAGE, eResourceId.FX_STATE_CUT_ALL_DAMAGE_L, false, 0));
			dictionary.Add((eAbnormalState)9, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_BURN, eResourceId.FX_STATE_BURN_L, true, 0));
			dictionary.Add((eAbnormalState)48, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_UB_SILENCE, eResourceId.FX_STATE_UB_SILENCE_L, true, 0));
			dictionary.Add((eAbnormalState)53, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_POISON, eResourceId.FX_STATE_POISON, true, 0));
			dictionary.Add((eAbnormalState)59, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_POISON2, eResourceId.FX_STATE_POISON2_L, true, 0));
			dictionary.Add((eAbnormalState)60, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_CURSE2, eResourceId.FX_STATE_CURSE2_L, false, 0));
			dictionary.Add((eAbnormalState)61, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_CONFUSION2, eResourceId.FX_STATE_CONFUSION2_L, true, 0));
			dictionary.Add((eAbnormalState)67, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_SPY, eResourceId.FX_STATE_SPY_L, true, 0));
			dictionary.Add((eAbnormalState)69, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_BLACK_FRAME, eResourceId.FX_STATE_BLACK_FRAME_L, true, 0));
			dictionary.Add((eAbnormalState)71, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_FLIGHT, eResourceId.FX_STATE_FLIGHT_L, false, 0));
			dictionary.Add((eAbnormalState)75, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_TAUNT, eResourceId.FX_STATE_TAUNT_L, true, 0));
			dictionary.Add((eAbnormalState)80, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_RATIO_SLIP, eResourceId.FX_STATE_RATIO_SLIP_L, false, 0));
		}

		[global::OpenPCR.Migration("677e2e97-19cb-4c77-afee-8ad3d7e169ef")]
		public void LoadMultiUnitAbnormalEffectPrefab(int _unitId)
		{
			int unitSetCount = UnitUtility.GetUnitSetCount(_unitId);
			if (unitSetCount != 1 && !abnormalEffectPrefabDic.ContainsKey(unitSetCount))
			{
				abnormalEffectPrefabDic[unitSetCount] = new Dictionary<global::Elements.Battle.Core.eAbnormalState, AbnormalStateEffectPrefabData>();
				Dictionary<global::Elements.Battle.Core.eAbnormalState, AbnormalStateEffectPrefabData> dictionary = abnormalEffectPrefabDic[unitSetCount];
				dictionary.Add(global::Elements.Battle.Core.eAbnormalState.FREEZE, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_FREEZE_MULTI, eResourceId.FX_STATE_FREEZE_MULTI_L, _isHead: false, unitSetCount));
				dictionary.Add(global::Elements.Battle.Core.eAbnormalState.CRYSTALIZE, createAbnormalStateEffectPrefab(eResourceId.FX_STATE_CRYSTALIZE_MULTI, eResourceId.FX_STATE_CRYSTALIZE_MULTI_L, _isHead: false, unitSetCount));
			}
		}

		[global::OpenPCR.Migration("b391dfe2-4332-410e-81c6-21cc9bd2954b")]
		public AbnormalStateEffectPrefabData GetAbnormalStateEffectPrefabData(eAbnormalState _abnormalState, int _unitId)
		{
			int unitSetCount = UnitUtility.GetUnitSetCount(_unitId);
			AbnormalStateEffectPrefabData value = null;
			if (abnormalEffectPrefabDic[unitSetCount].TryGetValue(_abnormalState, out value))
			{
				return value;
			}

			if (abnormalEffectPrefabDic[1].TryGetValue(_abnormalState, out value))
			{
				return value;
			}

			return null;
		}

		[global::OpenPCR.Migration("d94f0c34-d48a-4802-aa34-03ff060e307d")]
		private void loadAttackHitCenterEffectPrefab()
		{
			Action<eResourceId, bool> action = delegate (eResourceId JKANINJBEEK, bool OPKHDAOKDPE)
			{
				resourceMgr.AddLoadResourceId(JKANINJBEEK, delegate (object GLDFPJIMHNE)
				{
					AttackHitCenterEffectPrefabDic.Add(JKANINJBEEK, GLDFPJIMHNE as GameObject);
				}, _ableAbandon: false, OPKHDAOKDPE);
			};
			action(eResourceId.FX_HIT1_CENTER, arg2: true);
			action(eResourceId.FX_HIT1_CENTER_L, arg2: false);
			action(eResourceId.FX_HIT2, arg2: true);
			action(eResourceId.FX_HIT2_L, arg2: false);
			action(eResourceId.FX_HIT3, arg2: true);
			action(eResourceId.FX_HIT3_L, arg2: false);
			action(eResourceId.FX_HIT4, arg2: true);
			action(eResourceId.FX_HIT4_L, arg2: false);
			action(eResourceId.FX_HIT6_CENTER_MULTI, arg2: true);
			action(eResourceId.FX_HIT6_CENTER_MULTI_L, arg2: false);
			action(eResourceId.FX_HIT7_CENTER_MULTI, arg2: true);
			action(eResourceId.FX_HIT7_CENTER_MULTI_L, arg2: false);
		}

		[global::OpenPCR.Migration("2439c8a3-c624-47c8-8dc0-253776c447b2")]
		private AbnormalStateEffectPrefabData createAbnormalStateEffectPrefab(eResourceId _rightPrefabId, eResourceId _leftPrefabId, bool _isHead, int _resourceIndex = 0)
		{
			AbnormalStateEffectPrefabData result = new AbnormalStateEffectPrefabData();
			resourceMgr.AddLoadResourceId(_rightPrefabId, _resourceIndex, delegate (object EDIOAGKLHFJ)
			{
				result.RightEffectPrefab = EDIOAGKLHFJ as GameObject;
			});
			resourceMgr.AddLoadResourceId(_leftPrefabId, _resourceIndex, delegate (object EDIOAGKLHFJ)
			{
				result.LeftEffectPrefab = EDIOAGKLHFJ as GameObject;
			}, _ableAbandon: false, _isNothingToError: false);
			result.IsHead = _isHead;
			return result;
		}
	}
}
