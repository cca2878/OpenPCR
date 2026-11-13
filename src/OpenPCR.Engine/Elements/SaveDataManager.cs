using global::OpenPCR.Foundation;
using global::OpenPCR.Runtime.Api;
using System;
using System.Linq;
using System.Collections.Generic;
using Common;

namespace Elements
{
	public partial class SaveDataManager : ManagerSingleton<SaveDataManager>
	{
		public bool AdvantageAndBreakdownBonusShows
		{
			[global::OpenPCR.Migration("97a5284e-adbd-46ff-9c48-788af629c222")]
			get => global::OpenPCR.PreparedSettingsInputs.Read<bool>(this, "AdvantageAndBreakdownBonusShows");
			set => setBool(eSaveKey.ADVANTAGE_AND_BREAKDOWN_BONUS_SHOWS, value);
		}

		public int MainQuestAutoProgressUbSettingIndex
		{
			get => getInt(eSaveKey.MAIN_QUEST_AUTO_PROGRESS_UB_SETTING_INDEX, 0, false);
			set => setInt(eSaveKey.MAIN_QUEST_AUTO_PROGRESS_UB_SETTING_INDEX, value, false);
		}

		public int EventQuestAutoProgressUbSettingIndex
		{
			get => getInt(eSaveKey.EVENT_QUEST_AUTO_PROGRESS_UB_SETTING_INDEX, 0, false);
			set => setInt(eSaveKey.EVENT_QUEST_AUTO_PROGRESS_UB_SETTING_INDEX, value, false);
		}

		public bool UseUbSetMode
		{
			[global::OpenPCR.Migration("70ef2a07-fc1b-4061-99d9-d1038d39224b")]
			get
			{ return global::OpenPCR.PreparedSettingsInputs.Read<bool>(this, nameof(UseUbSetMode)); }

			set
			{
				setBool(eSaveKey.USE_UB_SET_MODE, value);
			}
		}

		public bool IsPrincessUpMovieSpeedUp
		{
			[global::OpenPCR.Migration("26561385-e45b-4846-b040-5bfbfe4baad4")]
			get
			{
				return getBool(eSaveKey.BATTLE_PRINCESS_UB_MOVIE_SPEED_UP, false);
			}

			set
			{
				setBool(eSaveKey.BATTLE_PRINCESS_UB_MOVIE_SPEED_UP, value);
			}
		}

		public enum eBattleMaxSpeed
		{
			TWICE = 2,
			QUADRUPLE = 4
		}

		public enum eVoiceAssetDownload
		{
			OFF = 0,
			ON = 1
		}

		public enum eMoviQuality
		{
			Low = 0,
			High = 1
		}

		private eVoiceAssetDownload voiceAssetDownloadValue = eVoiceAssetDownload.ON;
		private eMoviQuality movieQualityDownloadValue = eMoviQuality.High;
		private SavedataManager cuteSaveDataManager;
		private CutInData cutInData;
		private int battleFrameRate = 30;
		public int TalentFormationBonusConfirm
		{
			[global::OpenPCR.Migration("b0de600d-b0cc-43c0-915b-845d3e26511d")]
			get
			{
				return global::OpenPCR.PreparedSettingsInputs.Read<int>(this, "TalentFormationBonusConfirm");
			}

			set
			{
				setInt(eSaveKey.TALENT_FORMATION_BONUS_CONFIRM, value, false);
			}
		}

		public bool TalentFormationBonusFirst
		{
			[global::OpenPCR.Migration("0a668678-fe58-47ae-bfee-92ae58581be9")]
			get
			{
				return global::OpenPCR.PreparedSettingsInputs.Read<bool>(this, "TalentFormationBonusFirst");
			}

			[global::OpenPCR.Migration("a2265f76-0f47-4540-a1fe-c009b193ba37")]
			set
			{
				setBool(eSaveKey.TALENT_FORMATION_BONUS_FIRST, value);
			}
		}

		private bool getBool(eSaveKey _searchKey, bool _defaultValue = false)
		{
			return cuteSaveDataManager.GetBool(_searchKey, _defaultValue, false);
		}

		private void setBool(eSaveKey _setKey, bool _setValue)
		{
			cuteSaveDataManager.SetBool(_setKey, _setValue, false);
		}

		private int getInt(eSaveKey _searchKey, int _defaultValue = 0, bool withOutViewID = false)
		{
			int result = cuteSaveDataManager.GetInt(_searchKey, _defaultValue, withOutViewID);
			if (_searchKey == eSaveKey.MOVIE_HIGH_QUALITY_ASSET_DOWNLOAD)
			{
				int newer = cuteSaveDataManager.GetIntForStringKey("NewMovieSetting", -1, true);
				if (newer < 0)
					cuteSaveDataManager.SetIntForStringKey("NewMovieSetting", result, true);
				else
				{
					cuteSaveDataManager.SetIntForStringKey("NewMovieQuality", newer != 0 ? 2 : 1, true);
					return newer;
				}
			}

			return result;
		}

		private void setInt(eSaveKey _setKey, int _setValue, bool withOutViewID = false)
		{
			if (_setKey == eSaveKey.MOVIE_HIGH_QUALITY_ASSET_DOWNLOAD)
				cuteSaveDataManager.SetIntForStringKey("NewMovieSetting", _setValue, true);
			else
				cuteSaveDataManager.SetInt(_setKey, _setValue, withOutViewID);
		}

		public bool IsBattleSpeedQuadruple
		{
			[global::OpenPCR.Migration("2bae21f6-59f6-422e-98aa-d5e8b86ec8b0")]
			get
			{
				return global::OpenPCR.PreparedSettingsInputs.ReadBool(this, "IsBattleSpeedQuadruple");
			}

			[global::OpenPCR.Migration("b4408cda-dc31-4836-a8a7-5c66b1f07ded")]
			set
			{
				global::OpenPCR.PreparedSettingsInputs.Write(this, nameof(IsBattleSpeedQuadruple), value);
			}
		}

		public bool ApplySkinSettingsFromOtherUsers
		{
			[global::OpenPCR.Migration("f93b6462-c9c8-4cc4-a5bb-da9d05f72c67")]
			get
			{
				return global::OpenPCR.PreparedSettingsInputs.ReadBool(this, "ApplySkinSettingsFromOtherUsers");
			}

			set
			{
				setBool(eSaveKey.APPLY_SKIN_SETTINGS_FROM_OTHER_USERS, value);
			}
		}

		public int MultiTargetConfirm
		{
			[global::OpenPCR.Migration("288afd1d-9a3d-405f-a8ae-e4eef8633564")]
			get
			{
				return global::OpenPCR.PreparedSettingsInputs.Read<int>(this, "MultiTargetConfirm");
			}

			set
			{
				setInt(eSaveKey.MULTI_TARGET_CONFIRM, value, false);
			}
		}

		public bool MultiTargetFirst
		{
			[global::OpenPCR.Migration("d071371b-823b-4132-afab-fdd0d5ac0780")]
			get
			{
				return global::OpenPCR.PreparedSettingsInputs.Read<bool>(this, "MultiTargetFirst");
			}

			[global::OpenPCR.Migration("8cf6f5d7-7ea9-4e27-9673-d3fb30d6c1eb")]
			set
			{
				global::OpenPCR.Presentation.Noop();
			}
		}

		public bool ResetSkillReserveEveryTime
		{
			[global::OpenPCR.Migration("cd2bd34b-0d2a-48be-a2db-982d942675bc")]
			get
			{
				return global::OpenPCR.PreparedSettingsInputs.Read<bool>(this, nameof(ResetSkillReserveEveryTime));
			}

			set
			{
				setBool(eSaveKey.RESET_SKILL_RESERVE_EVERY_TIME, value);
			}
		}

		public ePlaybackType CutInPlaybackType
		{
			[global::OpenPCR.Migration("9babffa1-f374-4e72-99da-9fc850e6f123")]
			get
			{
				return cutInData.PlaybackType;
			}

			set
			{
				cutInData.PlaybackType = value;
				setString(eSaveKey.CUT_IN_PLAY_MODE, JsonUtility.ToJson(cutInData), false);
			}
		}

		public AutoSpeedSetting AutoSpeedSetting
		{
			[global::OpenPCR.Migration("7117442a-8940-4aaf-a089-df7d40056dbd")]
			get
			{
				// Each read deserializes a fresh object from the supplied local setting.
				AutoSpeedSetting stored = global::OpenPCR.PreparedSettingsInputs.Read<AutoSpeedSetting>(this, "AutoSpeedSetting");
				return new AutoSpeedSetting { ButtonState = stored.ButtonState, IsSpeedUp = stored.IsSpeedUp, IsAuto = stored.IsAuto, IsAllSet = stored.IsAllSet };
			}

			[global::OpenPCR.Migration("c0470c91-1b13-45f2-9c87-bad506c2b1fa")]
			set
			{
				global::OpenPCR.PreparedSettingsInputs.Write(this, "AutoSpeedSetting", new AutoSpeedSetting
				{ ButtonState = value.ButtonState, IsSpeedUp = value.IsSpeedUp, IsAuto = value.IsAuto, IsAllSet = value.IsAllSet });
			}
		}

		public int BattleFrameRate
		{
			[global::OpenPCR.Migration("2b8a2af1-649f-4dcd-8894-bf297f9b81f3")]
			get
			{ return battleFrameRate; }

			set
			{
				battleFrameRate = value;
				setInt(eSaveKey.BATTLE_FRAME_RATE, value, false);
			}
		}

		public eBattleMaxSpeed ClanReharsalBattleMaxSpeed
		{
			[global::OpenPCR.Migration("99eeb741-1bad-456d-a6fe-5000f0480efb")]
			get
			{
				return (eBattleMaxSpeed)global::OpenPCR.PreparedSettingsInputs.Read<int>(this, "ClanReharsalBattleMaxSpeed");
			}

			set
			{
				setInt(eSaveKey.CLAN_REHARSAL_BATTLE_MAX_SPEED, (int)value, false);
			}
		}

		public eBattleMaxSpeed TowerBattleMaxSpeed
		{
			[global::OpenPCR.Migration("c34057a6-5810-4b48-a316-41643ff5e3ef")]
			get
			{
				return (eBattleMaxSpeed)getInt(eSaveKey.TOWER_BATTLE_MAX_SPEED, 4, false);
			}

			set
			{
				setInt(eSaveKey.TOWER_BATTLE_MAX_SPEED, (int)value, false);
			}
		}

		public eBattleMaxSpeed DimensionFaultBattleMaxSpeed
		{
			[global::OpenPCR.Migration("0c6dbea8-8027-46fb-9aec-b38c711d1cf0")]
			get
			{
				return (eBattleMaxSpeed)getInt(eSaveKey.DIMENSION_FAULT_BATTLE_MAX_SPEED, 4, false);
			}

			set
			{
				setInt(eSaveKey.DIMENSION_FAULT_BATTLE_MAX_SPEED, (int)value, false);
			}
		}



		[global::OpenPCR.Migration("523d2739-e3b3-459b-b3d8-b71611750c40")]
		public void AddCutInPlayedUnitId(int _unitId)
		{
			int charaId = UnitUtility.GetCharaId(_unitId);
			cutInData.AddCharaIdPlayedCutIn(charaId);
			setString(eSaveKey.CUT_IN_PLAY_MODE, JsonUtility.ToJson(cutInData), false);
		}

		private void setString(eSaveKey _setKey, string _setValue, bool withOutViewID = false)
		{
			cuteSaveDataManager.SetString(_setKey, _setValue, withOutViewID);
		}

		private string getString(eSaveKey _searchKey, string _defaultValue = "", bool withOutViewID = false)
		{
			return cuteSaveDataManager.GetString(_searchKey, _defaultValue, withOutViewID);
		}

		[global::OpenPCR.Migration("76e7cfba-3451-4b80-947b-a9588a055c17")]
		public void Save()
		{
			global::OpenPCR.PreparedSettingsInputs.Save(this);
		}

		[global::OpenPCR.Migration("e9307746-2729-4854-888f-6583a699cd93")]
		public bool IsCutInPlayedChara(int unitId)
		{
			return cutInData.IsCutInPlayedChara(unitId);
		}

		protected override void onInit()
		{
			throw new OpenPCR.OutOfBoundaryException("Elements.SaveDataManager::onInit persistence environment initialization and legacy tips migration");
		}
		public List<int> TrialBattleInfinityHpActiveQuestIds
		{
			[global::OpenPCR.Migration("0b3daf7c-be01-45a4-893a-990dce0fae06")]
			get => global::OpenPCR.PreparedSettingsInputs.Read<global::System.Collections.Generic.List<int>>(this, "TrialBattleInfinityHpActiveQuestIds");
			set => setIntList(eSaveKey.TRIAL_BATTLE_INFINITY_HP_ACTIVE_QUEST_IDS, value);
		}
		private void setIntList(eSaveKey _setKey, List<int> _setValue)
		{
			if (_setValue == null)
				throw new NullReferenceException();
			global::OpenPCR.Foundation.SavedataManager manager = cuteSaveDataManager;
			int[] values = _setValue.ToArray();
			manager.SetIntArray(_setKey, values);
		}
	}
}
