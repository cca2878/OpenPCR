using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using global::OpenPCR.Foundation;
using Elements.Battle;
using Elements.Battle.Core;
using Elements.Data;

namespace Elements
{
	public static class BattleVoiceUtility
	{
		[global::OpenPCR.ImplicitMigration("c90911a1-6291-4ee1-9a50-038708f3e36d")]
		private static IBattleManagerForBattleVoiceUtility battleManager => staticBattleManager;

		private static readonly Dictionary<SoundManager.eVoiceType, bool> isUsedFlagDic = new Dictionary<SoundManager.eVoiceType, bool>
		{
			{
				SoundManager.eVoiceType.SPECIAL_CUT_IN,
				false
			},
			{
				SoundManager.eVoiceType.SPECIAL_APPLAUD,
				false
			},
			{
				SoundManager.eVoiceType.SPECIAL_THANKS,
				false
			}
		};
		private static readonly Dictionary<SoundManager.eVoiceType, int> activationRateDic = new Dictionary<SoundManager.eVoiceType, int>
		{
			{
				SoundManager.eVoiceType.SPECIAL_BATTLE_START,
				50
			},
			{
				SoundManager.eVoiceType.SPECIAL_CUT_IN,
				50
			},
			{
				SoundManager.eVoiceType.SPECIAL_WAVE_START,
				50
			},
			{
				SoundManager.eVoiceType.SPECIAL_NEXT_WAVE,
				50
			},
			{
				SoundManager.eVoiceType.SPECIAL_CLEAR_WIN,
				50
			},
			{
				SoundManager.eVoiceType.SPECIAL_WIN,
				50
			},
			{
				SoundManager.eVoiceType.SPECIAL_CLOSE_GAME,
				50
			},
			{
				SoundManager.eVoiceType.SPECIAL_LOSE,
				50
			},
			{
				SoundManager.eVoiceType.SPECIAL_APPLAUD,
				50
			},
			{
				SoundManager.eVoiceType.SPECIAL_THANKS,
				50
			}
		};
		private static IBattleManagerForBattleVoiceUtility staticBattleManager = null;

		private static readonly Dictionary<eBattleCategory, List<int>> BORDER_UNIT_NUM = new Dictionary<eBattleCategory, List<int>>
{
	{ (eBattleCategory)0, new List<int> { 1, 2 } },
	{ (eBattleCategory)10, new List<int> { 1, 2 } },
	{ (eBattleCategory)56, new List<int> { 1, 2 } },
	{ (eBattleCategory)3, new List<int> { 3, -1 } },
	{ (eBattleCategory)24, new List<int> { 3, -1 } },
	{ (eBattleCategory)7, new List<int> { 3, -1 } },
	{ (eBattleCategory)1, new List<int> { 1, 2 } },
	{ (eBattleCategory)9, new List<int> { 3, -1 } },
	{ (eBattleCategory)2, new List<int> { 1, 2 } },
	{ (eBattleCategory)30, new List<int> { 1, 2 } },
	{ (eBattleCategory)11, new List<int> { 1, 2 } },
	{ (eBattleCategory)14, new List<int> { 1, 2 } },
	{ (eBattleCategory)15, new List<int> { 3, -1 } },
	{ (eBattleCategory)21, new List<int> { 1, 2 } },
	{ (eBattleCategory)36, new List<int> { 1, 2 } },
	{ (eBattleCategory)37, new List<int> { 1, 2 } },
	{ (eBattleCategory)38, new List<int> { 1, 2 } },
	{ (eBattleCategory)31, new List<int> { 1, 2 } },
	{ (eBattleCategory)32, new List<int> { 1, 2 } },
	{ (eBattleCategory)59, new List<int> { 1, 2 } },
	{ (eBattleCategory)60, new List<int> { 3, -1 } },
};

		public enum eConditionType { VICTORY = 0, NORMAL = 1, NARROW = 2, MAX = 3 }

		private static eConditionType getWinCondition(List<UnitCtrl> _unitList, eBattleCategory _battleCategory)
		{
			List<int> borderUnitNum;
			if (!BORDER_UNIT_NUM.TryGetValue(_battleCategory, out borderUnitNum))
				borderUnitNum = BORDER_UNIT_NUM[(eBattleCategory)0];
			int deadCount = _unitList.FindAll(e =>
				(e.ComponentParameter.Hp == 0L || e.IsRecreated) && !e.IsSummonOrPhantom).Count;
			if (deadCount < borderUnitNum[0]) return eConditionType.VICTORY;
			if (deadCount < borderUnitNum[1]) return eConditionType.NORMAL;
			return eConditionType.NARROW;
		}

		private static void groupPlayVoice(int _groupId, int _groupUnitId, SoundManager.eVoiceType _voiceType, int _index)
		{ ManagerSingleton<SoundManager>.Instance.PlaySpecialVoice(_groupId, _groupUnitId, _voiceType, _index); }

		[global::OpenPCR.Migration("ba181643-705c-4221-92f0-9e51c4f89bb6")]
		public static VoiceTimingData GetVoiceTimingData(int _unitId, bool _isDirectLoad = false)
		{
			return global::OpenPCR.Presentation.Noop<VoiceTimingData>();
		}

		[global::OpenPCR.Migration("4a20b363-7c33-4db0-9a84-ad0ae51f066c")]
		public static void StaticInit(IBattleManagerForBattleVoiceUtility _battleManager)
		{
			staticBattleManager = _battleManager;
		}

		[global::OpenPCR.Migration("ae2d9c94-d596-406a-b100-c15f624bb0bd")]
		public static void StaticRelease()
		{
			staticBattleManager = null;
		}

		private static int getRandomNumber(int _min, int _max)
		{
			return unchecked(_min + (int)(Singleton<TempData>.Instance.XorShiftRandom.Next() % (long)unchecked(_max - _min)));
		}

		[global::OpenPCR.Migration("7a35f1d4-f3d6-4870-87d7-e44e1f1b4f82")]
		private static bool isSelectSpecialVoice(SoundManager.eVoiceType _voiceType)
		{
			if (_voiceType == SoundManager.eVoiceType.SPECIAL_BATTLE_START)
			{
				return global::OpenPCR.Runtime.Api.Random.Range(0, 100) < activationRateDic[_voiceType];
			}

			return getRandomNumber(0, 100) < activationRateDic[_voiceType];
		}

		[global::OpenPCR.Migration("c941d955-f47a-4278-a135-f2eb4402de73")]
		private static int selectRandomId(List<int> _idList)
		{
			return _idList[getRandomNumber(0, _idList.Count)];
		}

		[global::OpenPCR.Migration("d4b73a69-23c0-4b9b-944d-da159a33d082")]
		private static UnitCtrl selectRandomUnit(List<UnitCtrl> _unitList)
		{
			return _unitList[getRandomNumber(0, _unitList.Count)];
		}

		[global::OpenPCR.Migration("4af0c22f-5bf0-424f-bdba-5a99aa739ad8")]
		public static List<UnitCtrl> GetAliveUnitList(List<UnitCtrl> _unitList)
		{
			return _unitList.FindAll(unit => !unit.ComponentState.IsDead && !unit.IsSummonOrPhantom && !unit.IsRecreated);
		}

		[global::OpenPCR.Migration("866b8f7b-b0c3-4b9f-b761-ee2e8f538418")]
		public static List<UnitCtrl> GetNoSummonUnitList(List<UnitCtrl> _unitList)
		{
			return _unitList.FindAll(unit => !unit.IsSummonOrPhantom);
		}

		[global::OpenPCR.Migration("f2d28388-7bb8-4d2a-8666-ac0cc87c4657")]
		public static List<VoiceGroupAndGroupUnit> GetEnableGroupList(List<int> _unitIdList)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.BattleVoiceUtility.GetEnableGroupList: special voice Master group production");
		}

		public static void PlayWinVoice(List<UnitCtrl> _unitList, eBattleCategory _battleCategory, bool _isNormalOnly = false)
		{
			SoundManager sound = ManagerSingleton<SoundManager>.Instance;
			List<UnitCtrl> alive = GetAliveUnitList(_unitList);
			List<VoiceGroupAndGroupUnit> groups = GetEnableGroupList(GetOriginalUnitIdListFromUnitCtrl(alive));
			if (groups.Count != 0 && isSelectSpecialVoice(SoundManager.eVoiceType.SPECIAL_WIN) && !_isNormalOnly && !TutorialManager.IsStartTutorial)
			{
				int groupId = groups[getRandomNumber(0, groups.Count)].GroupId;
				eConditionType condition = getWinCondition(_unitList, _battleCategory);
				SoundManager.eVoiceType type = condition == eConditionType.VICTORY ? SoundManager.eVoiceType.SPECIAL_CLEAR_WIN : condition == eConditionType.NARROW ? SoundManager.eVoiceType.SPECIAL_CLOSE_GAME : SoundManager.eVoiceType.SPECIAL_WIN;
				if (getGroupVoiceExists(groupId, 0, type, 1))
				{
					groupPlayVoice(groupId, 0, type, 1);
					return;
				}
			}
			List<UnitCtrl> candidates = alive.Where(unit => !unit.IsGuest).ToList();
			if (candidates.Count == 0) return;
			UnitComponentSound component = selectRandomUnit(candidates).ComponentSound;
			int upper = component.TryGetVoiceDictionary(SoundManager.eVoiceType.WIN, out Dictionary<int, int> dictionary) ? unchecked(dictionary[0] + 1) : 4;
			int index = getRandomNumber(1, upper);
			sound.PlayVoice(component.SoundUnitId, SoundManager.eVoiceType.WIN, index);
		}

		[global::OpenPCR.Migration("212969f7-1e20-4088-bb9d-014e505559a2")]
		public static void SelectLoseVoiceUnit(List<UnitCtrl> _unitList)
		{
			List<UnitCtrl> unitList = GetNoSummonUnitList(_unitList);
			List<UnitCtrl> nonGuestUnits = unitList.Where(unit => !unit.IsGuest).ToList();
			if (nonGuestUnits.Count != 0)
				unitList = nonGuestUnits;
			UnitCtrl selectedUnit = selectRandomUnit(unitList);
			battleManager.PlayVoiceUnitId = selectedUnit.UnitId;
			battleManager.PlayVoiceUnitIsRarity6 = selectedUnit.Rarity == 6;
		}

		[global::OpenPCR.Migration("e69e89aa-0af1-419c-8feb-96b52e537eaa")]
		public static List<int> GetOriginalUnitIdListFromUnitCtrl(List<UnitCtrl> _unitList)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < _unitList.Count; i++)
			{
				list.Add(_unitList[i].OriginalUnitId);
			}

			return list;
		}

		[global::OpenPCR.Migration("520594b1-c3ba-4ac3-9740-04c4b2a8bc77")]
		public static CutinVoiceStatus SelectCutinVoice(UnitCtrl _targetUnit, List<UnitCtrl> _unitList, int _voiceId, bool _isSpeedUpFlag)
		{
			if (_voiceId == 1 && !TutorialManager.IsStartTutorial)
			{
				int originalUnitId = _targetUnit.OriginalUnitId;
				List<int> enebleGroupIdList = getEnebleGroupIdList(originalUnitId, _unitList);
				if (enebleGroupIdList.Count != 0)
				{
					bool isEnemy = !_targetUnit.IsPlayerUnit;
					List<int> firstTimeVoice = getFirstTimeVoice(originalUnitId, enebleGroupIdList, SoundManager.eVoiceType.SPECIAL_CUT_IN, isEnemy);
					if (firstTimeVoice.Count != 0)
					{
						int num = selectRandomId(firstTimeVoice);
						int groupUnitIdFromUnitId = getGroupUnitIdFromUnitId(num, originalUnitId);
						if (getGroupVoiceExists(num, groupUnitIdFromUnitId, SoundManager.eVoiceType.SPECIAL_CUT_IN, 1))
						{
							battleManager.BattleVoiceFlagMgr.SetPlayedFlag(originalUnitId, num, SoundManager.eVoiceType.SPECIAL_CUT_IN, _isPlayed: true, isEnemy);
							VoiceGroupIdSet groupIdSet = new VoiceGroupIdSet(num, groupUnitIdFromUnitId, originalUnitId);
							VoiceTimingSet voiceTimingGroup = getVoiceTimingGroup(_targetUnit, num, _isSpeedUpFlag);
							return new CutinVoiceStatus(_IsUseSpecialVoice: true, groupIdSet, voiceTimingGroup);
						}
						return new CutinVoiceStatus(_IsUseSpecialVoice: false, default(VoiceGroupIdSet), null);
					}
					if (isSelectSpecialVoice(SoundManager.eVoiceType.SPECIAL_CUT_IN))
					{
						int num2 = selectRandomId(enebleGroupIdList);
						int groupUnitIdFromUnitId2 = getGroupUnitIdFromUnitId(num2, originalUnitId);
						if (getGroupVoiceExists(num2, groupUnitIdFromUnitId2, SoundManager.eVoiceType.SPECIAL_CUT_IN, 1))
						{
							VoiceGroupIdSet groupIdSet2 = new VoiceGroupIdSet(num2, groupUnitIdFromUnitId2, originalUnitId);
							VoiceTimingSet voiceTimingGroup2 = getVoiceTimingGroup(_targetUnit, num2, _isSpeedUpFlag);
							return new CutinVoiceStatus(_IsUseSpecialVoice: true, groupIdSet2, voiceTimingGroup2);
						}
						return new CutinVoiceStatus(_IsUseSpecialVoice: false, default(VoiceGroupIdSet), null);
					}
				}
			}
			return new CutinVoiceStatus(_IsUseSpecialVoice: false, default(VoiceGroupIdSet), null);
		}

		[global::OpenPCR.Migration("ef798942-b290-4343-b222-b5419b4f3434")]
		public static void PlayWaveStartVoice(UnitCtrl _targetUnit)
		{ if (!_targetUnit.GetIsMuteVoice()) throw new global::OpenPCR.OutOfBoundaryException("Prepared runtime requires an explicit muted wave-start voice output"); }

		[global::OpenPCR.Migration("ac7fa826-0594-4b52-973d-08dd28d00b44")]
		public static void PlayDialogueVoice(SoundManager.eVoiceType _voiceType, UnitCtrl _targetUnit, List<UnitCtrl> _otherUnitList, int _cutInGroupId = 0, int _groupUnitId = 0)
		{
			if (!_targetUnit.GetIsMuteVoice())
				throw new global::OpenPCR.OutOfBoundaryException("Offline dialogue output requires explicitly muted voice; no group/cue producer is supplied.");
		}

		[global::OpenPCR.Migration("0b3d55a4-a816-4f60-b5ac-ffa941eaf5a5")]
		private static List<int> getFirstTimeVoice(int _unitId, List<int> _groupIdList, SoundManager.eVoiceType _voiceType, bool _isEnemy)
		{
			BattleVoiceFlagManager manager = battleManager.BattleVoiceFlagMgr;
			List<int> result = new List<int>();
			for (int i = 0; i < _groupIdList.Count; i++)
				if (!manager.IsPlayed(_unitId, _groupIdList[i], _voiceType, _isEnemy)) result.Add(_groupIdList[i]);
			return result;
		}

		[global::OpenPCR.Migration("f5c1602f-4a69-420f-8918-fdd6781993e5")]
		private static VoiceTimingSet getVoiceTimingGroup(UnitCtrl _unit, int _goupId, bool _isSpeedUpFlag)
		{
			VoiceTimingGroup group = _unit.SpecialVoiceTimingData.VoiceTimingGroupList.Find(data => data.VoiceGroupId == _goupId);
			return _isSpeedUpFlag ? group.DoubleSpeed : group.ConstantVelocity;
		}

		[global::OpenPCR.Migration("8e2ad389-ae23-471c-b269-9226f0c42cc2")]
		public static string ConvertToSkillCueSheetName(int _unitId)
		{
			return string.Format("se_btl_{0}", _unitId);
		}

		[global::OpenPCR.Migration("8e006d0b-3107-438b-95d5-3c52c9cc84be")]
		private static List<int> getEnebleGroupIdList(int _unitId, List<UnitCtrl> _unitList = null)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.BattleVoiceUtility::getEnebleGroupIdList special voice Master group production");
		}

		[global::OpenPCR.Migration("2a9f143a-ac26-4cd5-a3a8-c720bdf5c3f4")]
		private static int getGroupUnitIdFromUnitId(int _groupId, int _unitId)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.BattleVoiceUtility::getGroupUnitIdFromUnitId special voice Master group production");
		}

		[global::OpenPCR.Migration("3844a8ea-ae3d-4dfe-afd9-bca7a4d7f91e")]
		private static bool getGroupVoiceExists(int _groupId, int _groupUnitId, SoundManager.eVoiceType _voiceType, int _index)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.BattleVoiceUtility::getGroupVoiceExists audio cue resource existence");
		}
	}
}
