using global::Elements;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Elements
{
	public class AnimKitConverterDefine
	{
		public delegate void CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion);

		public class ConverterData
		{
			public string AnimeName { get; private set; }

			public eAnimKitConvertCategoryType SpineConvertCategoryType { get; private set; }

			private CONVERT_FUNC animationCheck { get; set; }

			public ConverterData(string _setAnimeName, eAnimKitConvertCategoryType _setSpineConvertCategoryType, CONVERT_FUNC _setAnimationCheck)
			{
				AnimeName = _setAnimeName;
				SpineConvertCategoryType = _setSpineConvertCategoryType;
				animationCheck = _setAnimationCheck;
			}

		}

		public const int NYXMK2_UNITID = 192101;

		public const string ANIMATION_NEBBIA = "nebbia";

		public const string ANIMATION_PIPPLE = "pipple";

		public static Dictionary<eAnimKitCharacterAnimeId, ConverterData> DataDic { get; set; } = new Dictionary<eAnimKitCharacterAnimeId, ConverterData>
		{
			{
				eAnimKitCharacterAnimeId.RARITYUP_POSING,
				new ConverterData("rarityup_posing", eAnimKitConvertCategoryType.NORMAL, POSING_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.MANA_IDLE,
				new ConverterData("mana_idle", eAnimKitConvertCategoryType.NORMAL, POSING_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.MANA_JUMP,
				new ConverterData("mana_jump", eAnimKitConvertCategoryType.NORMAL, POSING_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.STAMINA_BOW,
				new ConverterData("stamina_karin_ojigi", eAnimKitConvertCategoryType.NORMAL, POSING_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.FRIEND_HIGHTOUCH,
				new ConverterData("friend_hightouch", eAnimKitConvertCategoryType.NORMAL, POSING_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.DEAR_IDOL,
				new ConverterData("dear_idol", eAnimKitConvertCategoryType.NORMAL, DEAR_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.DEAR_JUMP,
				new ConverterData("dear_jump", eAnimKitConvertCategoryType.NORMAL, DEAR_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.DEAR_SMILE,
				new ConverterData("dear_smile", eAnimKitConvertCategoryType.NORMAL, DEAR_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.RACE_EAT_JUN,
				new ConverterData("eat_jun", eAnimKitConvertCategoryType.NORMAL, RACE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.RACE_EAT_NORMAL,
				new ConverterData("eat_normal", eAnimKitConvertCategoryType.NORMAL, RACE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.RACE_EAT_SHAVEICE,
				new ConverterData("unit_race_eat_shaveice", eAnimKitConvertCategoryType.NORMAL, RACE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.RACE_RUN_SPRING_JUMP,
				new ConverterData("run_springJump", eAnimKitConvertCategoryType.NORMAL, RACE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.RACE_BALLOON_FLYING_DOWN,
				new ConverterData("balloon_flying_down", eAnimKitConvertCategoryType.NORMAL, RACE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.RACE_BALLOON_FLYING_LOOP,
				new ConverterData("balloon_flying_loop", eAnimKitConvertCategoryType.NORMAL, RACE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.RACE_BALLOON_FLYING_UP,
				new ConverterData("balloon_flying_up", eAnimKitConvertCategoryType.NORMAL, RACE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.RACE_BALLOON_IN,
				new ConverterData("balloon_in", eAnimKitConvertCategoryType.NORMAL, RACE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.RACE_BALLOON_OUT,
				new ConverterData("balloon_out", eAnimKitConvertCategoryType.NORMAL, RACE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.RACE_RUN,
				new ConverterData("run_race", eAnimKitConvertCategoryType.NORMAL, RACE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.UNIT_RACE,
				new ConverterData("unit_race", eAnimKitConvertCategoryType.NORMAL, RACE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.RUN_JUMP,
				new ConverterData("run_jump", eAnimKitConvertCategoryType.NORMAL, RUN_JUMP_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.RUN_HIGH_JUMP,
				new ConverterData("run_highJump", eAnimKitConvertCategoryType.NORMAL, RUN_JUMP_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.NO_WEAPON_IDLE,
				new ConverterData("noWeapon_idle", eAnimKitConvertCategoryType.NORMAL, NO_WEAPON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.NO_WEAPON_JOY_SHORT,
				new ConverterData("noWeapon_joy_short", eAnimKitConvertCategoryType.NORMAL, NO_WEAPON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.NO_WEAPON_RUN,
				new ConverterData("noWeapon_run", eAnimKitConvertCategoryType.NORMAL, NO_WEAPON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.NO_WEAPON_RUN_SUPER,
				new ConverterData("noWeapon_run_super", eAnimKitConvertCategoryType.NORMAL, NO_WEAPON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.PCT,
				new ConverterData("pct_", eAnimKitConvertCategoryType.MINIGAME, MINIGAME_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.FKE_IDLE,
				new ConverterData("fke_kuka_idle", eAnimKitConvertCategoryType.MINIGAME, MINIGAME_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.FKE,
				new ConverterData("fke_", eAnimKitConvertCategoryType.MINIGAME, MINIGAME_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.PKB,
				new ConverterData("pkb_", eAnimKitConvertCategoryType.MINIGAME, MINIGAME_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.CGG,
				new ConverterData("cgg_", eAnimKitConvertCategoryType.MINIGAME, MINIGAME_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.PSY,
				new ConverterData("psy_", eAnimKitConvertCategoryType.MINIGAME, MINIGAME_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.TAQ,
				new ConverterData("taq_", eAnimKitConvertCategoryType.MINIGAME, MINIGAME_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.SJR,
				new ConverterData("sjr_", eAnimKitConvertCategoryType.MINIGAME, MINIGAME_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_GLD_01,
				new ConverterData("ekd_gld_01", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_GLD_02,
				new ConverterData("ekd_gld_02", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_GLD_03,
				new ConverterData("ekd_gld_03", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_GLD_04,
				new ConverterData("ekd_gld_04", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_GLD_05,
				new ConverterData("ekd_gld_05", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_GLD_06,
				new ConverterData("ekd_gld_06", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_IDLE_ANGER,
				new ConverterData("ekd_idle_anger", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_IDLE_JOY,
				new ConverterData("ekd_idle_joy", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_IDLE_JOY2,
				new ConverterData("ekd_idle_joy2", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_IDLE_NORMAL,
				new ConverterData("ekd_idle_normal", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_IDLE_SAD,
				new ConverterData("ekd_idle_sad", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_IDLE_SURPRISE1,
				new ConverterData("ekd_idle_surprise1", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_IDLE_SURPRISE2,
				new ConverterData("ekd_idle_surprise2", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_OTHER_01,
				new ConverterData("ekd_other_01", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_OTHER_02,
				new ConverterData("ekd_other_02", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_OTHER_03,
				new ConverterData("ekd_other_03", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_OTHER_04,
				new ConverterData("ekd_other_04", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_OTHER_05,
				new ConverterData("ekd_other_05", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_OTHER_06,
				new ConverterData("ekd_other_06", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_OTHER_07,
				new ConverterData("ekd_other_07", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_OTHER_08,
				new ConverterData("ekd_other_08", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_OTHER_09,
				new ConverterData("ekd_other_09", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_OTHER_10,
				new ConverterData("ekd_other_10", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PARTY_01,
				new ConverterData("ekd_party_01", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PARTY_02,
				new ConverterData("ekd_party_02", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PARTY_03,
				new ConverterData("ekd_party_03", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PARTY_04,
				new ConverterData("ekd_party_04", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PARTY_05,
				new ConverterData("ekd_party_05", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PARTY_06,
				new ConverterData("ekd_party_06", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PARTY_07,
				new ConverterData("ekd_party_07", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PARTY_08,
				new ConverterData("ekd_party_08", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PARTY_09,
				new ConverterData("ekd_party_09", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PARTY_10,
				new ConverterData("ekd_party_10", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PSN_01,
				new ConverterData("ekd_psn_01", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PSN_02,
				new ConverterData("ekd_psn_02", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PSN_03,
				new ConverterData("ekd_psn_03", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PSN_04,
				new ConverterData("ekd_psn_04", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PSN_05,
				new ConverterData("ekd_psn_05", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PSN_06,
				new ConverterData("ekd_psn_06", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PSN_07,
				new ConverterData("ekd_psn_07", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PSN_08,
				new ConverterData("ekd_psn_08", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PSN_09,
				new ConverterData("ekd_psn_09", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_PSN_10,
				new ConverterData("ekd_psn_10", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_RUN,
				new ConverterData("ekd_run", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_SMILE,
				new ConverterData("ekd_smile", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_SURPRISE,
				new ConverterData("ekd_surprise", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_TALK_ANGER,
				new ConverterData("ekd_talk_anger", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_TALK_JOY,
				new ConverterData("ekd_talk_joy", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_TALK_JOY2,
				new ConverterData("ekd_talk_joy2", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_TALK_NORMAL,
				new ConverterData("ekd_talk_normal", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_TALK_SAD,
				new ConverterData("ekd_talk_sad", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_TALK_SURPRISE1,
				new ConverterData("ekd_talk_surprise1", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EKD_TALK_SURPRISE2,
				new ConverterData("ekd_talk_surprise2", eAnimKitConvertCategoryType.EKD, EKD_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.OMP,
				new ConverterData("omp_", eAnimKitConvertCategoryType.OMP, OMP_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.UEK,
				new ConverterData("uek_", eAnimKitConvertCategoryType.UEK, UEK_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.NYX,
				new ConverterData("nyx_", eAnimKitConvertCategoryType.NYX, NYX_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.MHP,
				new ConverterData("mhp_", eAnimKitConvertCategoryType.MHP, MHP_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.NOP,
				new ConverterData("nop_", eAnimKitConvertCategoryType.NOP, NOP_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.DSB,
				new ConverterData("dsb_", eAnimKitConvertCategoryType.DSB, DSB_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.BLB_DRAMA,
				new ConverterData("blb_", eAnimKitConvertCategoryType.BLB_DRAMA, BLB_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.TRAVEL_TOP_ROOM_UNIT,
				new ConverterData("travel_r_", eAnimKitConvertCategoryType.TRAVEL_TOP, TRAVEL_DRAMA_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.TRAVEL_TOP_BATTLE_UNIT,
				new ConverterData("travel_b_", eAnimKitConvertCategoryType.TRAVEL_TOP, TRAVEL_DRAMA_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.TRAVEL_TOPDRAMA_ROOM_UNIT,
				new ConverterData("travel_topdrama_r_", eAnimKitConvertCategoryType.TRAVEL_TOPDRAMA, TRAVEL_TOP_DRAMA_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.TRAVEL_TOPDRAMA_BATTLE_UNIT,
				new ConverterData("travel_topdrama_b_", eAnimKitConvertCategoryType.TRAVEL_TOPDRAMA, TRAVEL_TOP_DRAMA_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.TRAVEL_EXDRAMA_ROOM_UNIT,
				new ConverterData("travel_exdrama_r_", eAnimKitConvertCategoryType.TRAVEL_EXDRAMA, TRAVEL_DRAMA_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.TRAVEL_EXDRAMA_BATTLE_UNIT,
				new ConverterData("travel_exdrama_b_", eAnimKitConvertCategoryType.TRAVEL_EXDRAMA, TRAVEL_DRAMA_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.WAC,
				new ConverterData("wac_", eAnimKitConvertCategoryType.WAC, WAC_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.LSV,
				new ConverterData("lsv_", eAnimKitConvertCategoryType.LSV, LSV_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.IDLE_MAP_S,
				new ConverterData("idle_Map_s", eAnimKitConvertCategoryType.NORMAL, MAP_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.IDLE_MAP,
				new ConverterData("idle_Map", eAnimKitConvertCategoryType.NORMAL, MAP_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.AWAKE_MAP_RELEASE,
				new ConverterData("awake_Map_release", eAnimKitConvertCategoryType.NORMAL, MAP_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.AWAKE_MAP,
				new ConverterData("awake_Map", eAnimKitConvertCategoryType.NORMAL, MAP_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.DIE_MAP,
				new ConverterData("die_Map", eAnimKitConvertCategoryType.NORMAL, MAP_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.IDLE,
				new ConverterData("idle", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.IDLE_MULTI_TARGET,
				new ConverterData("idle_multi_target_", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.DAMAGE_MULTI_TARGET,
				new ConverterData("damage_multi_target_", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.WALK,
				new ConverterData("walk", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.JOY_SHORT,
				new ConverterData("joy_short", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.JOY_LONG,
				new ConverterData("joy_long", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.IDLE_STAND_BY,
				new ConverterData("multi_idle_standBy", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.IDLE_NO_WEAPON,
				new ConverterData("multi_idle_noWeapon", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.RUN_GAME_START,
				new ConverterData("run_gamestart", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.COOP_STAND_BY,
				new ConverterData("multi_standBy", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.RUN,
				new ConverterData("run", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.STAND_BY,
				new ConverterData("standBy", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.LANDING,
				new ConverterData("landing", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.IDLE_SKIP,
				new ConverterData("idle_Skip", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.DAMAGE_SKIP,
				new ConverterData("damage_Skip", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.LOADING,
				new ConverterData("loading", eAnimKitConvertCategoryType.NORMAL, LOADING_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ANNIHILATION,
				new ConverterData("annihilation", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.SLEEP,
				new ConverterData("sleep", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.DUNGEON_IDLE_SKIP,
				new ConverterData("idle", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EVENT_BANNER_MOTION,
				new ConverterData("event_banner_motion", eAnimKitConvertCategoryType.NORMAL, EVENT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EVENT_BANNER_MOTION_CIRCLE,
				new ConverterData("event_banner_motion_circle", eAnimKitConvertCategoryType.NORMAL, EVENT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.SMILE,
				new ConverterData("smile", eAnimKitConvertCategoryType.NORMAL, SMILE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.OWN_DIE,
				new ConverterData("die", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ATTACK,
				new ConverterData("attack", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.DAMEGE,
				new ConverterData("damage", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.DIE,
				new ConverterData("die", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.LOSE,
				new ConverterData("lose", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.DEFEAT,
				new ConverterData("defeat", eAnimKitConvertCategoryType.NORMAL, DEFEAT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.DIE_LOOP,
				new ConverterData("die_loop", eAnimKitConvertCategoryType.NORMAL, COMMON_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.RACE_ENEMY,
				new ConverterData("anniversary_race", eAnimKitConvertCategoryType.RACE_ENEMY, RACE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.COMBINE_JOY_RESULT,
				new ConverterData("_joy_combine_", eAnimKitConvertCategoryType.NORMAL, COMBINE_JOY_RESULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.SKILL,
				new ConverterData("skill", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.PRINCESS_SKILL,
				new ConverterData("p", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.PRINCESS_SKILL_EVOLUTION,
				new ConverterData("p", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.SKILL_EVOLUTION,
				new ConverterData("skill_evolution", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.SUB_UNION_BURST,
				new ConverterData("skillUb", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.SPECIAL_SKILL_EVOLUTION,
				new ConverterData("skillSp_evolution", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.JOY_RESULT,
				new ConverterData("joyResult", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.SPECIAL_SKILL,
				new ConverterData("skillSp", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.OWN_IDLE,
				new ConverterData("idle", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.OWN_WALK,
				new ConverterData("walk", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.OWN_JOY,
				new ConverterData("joy", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.OWN_ATTACK,
				new ConverterData("attack", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.OWN_DAMEGE,
				new ConverterData("damage", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.SUMMON,
				new ConverterData("summon", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.GAME_START,
				new ConverterData("gamestart", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.AWAKE,
				new ConverterData("awake", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ATTACK_SKIPQUEST,
				new ConverterData("attack_skipQuest", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.IDLE_RESULT,
				new ConverterData("idle_Result", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.SPECIAL_IDLE,
				new ConverterData("special_idle", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.IDLE_RESULT_LATER,
				new ConverterData("idle_Result_Later", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.AWAKE_RESULT,
				new ConverterData("awake_Result", eAnimKitConvertCategoryType.NORMAL, BATTLE_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.EVENT_STORY,
				new ConverterData("event_", eAnimKitConvertCategoryType.NORMAL, EVENT_STORY_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_IDOL,
				new ConverterData("cmn_idol", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_JUMP,
				new ConverterData("cmn_jump", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_WALK,
				new ConverterData("cmn_walk", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_RUN,
				new ConverterData("cmn_run", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_APPEAR,
				new ConverterData("cmn_appear", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_DISAPPEAR,
				new ConverterData("cmn_disappear", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_DISAPPEAR2,
				new ConverterData("cmn_disappear2", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE01,
				new ConverterData("cmn_pose01", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE02,
				new ConverterData("cmn_pose02", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE03,
				new ConverterData("cmn_pose03", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE04,
				new ConverterData("cmn_pose04", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE05,
				new ConverterData("cmn_pose05", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE06,
				new ConverterData("cmn_pose06", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE07,
				new ConverterData("cmn_pose07", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE08,
				new ConverterData("cmn_pose08", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE09,
				new ConverterData("cmn_pose09", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE10,
				new ConverterData("cmn_pose10", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE11,
				new ConverterData("cmn_pose11", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE12,
				new ConverterData("cmn_pose12", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE13,
				new ConverterData("cmn_pose13", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE14,
				new ConverterData("cmn_pose14", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE15,
				new ConverterData("cmn_pose15", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE16,
				new ConverterData("cmn_pose16", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE17,
				new ConverterData("cmn_pose17", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE18,
				new ConverterData("cmn_pose18", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE19,
				new ConverterData("cmn_pose19", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE20,
				new ConverterData("cmn_pose20", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE21,
				new ConverterData("cmn_pose21", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE22,
				new ConverterData("cmn_pose22", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE23,
				new ConverterData("cmn_pose23", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE24,
				new ConverterData("cmn_pose24", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE25,
				new ConverterData("cmn_pose25", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE26,
				new ConverterData("cmn_pose26", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE27,
				new ConverterData("cmn_pose27", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE28,
				new ConverterData("cmn_pose28", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE29,
				new ConverterData("cmn_pose29", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_POSE30,
				new ConverterData("cmn_pose30", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_CALL,
				new ConverterData("cmn_call", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_NOTICE,
				new ConverterData("cmn_notice", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_SHOCK,
				new ConverterData("cmn_shock", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_NOD,
				new ConverterData("cmn_nod", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_HANDCLAP,
				new ConverterData("cmn_handclap", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_GRAB,
				new ConverterData("cmn_grab01", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_GRAB_PANIC,
				new ConverterData("cmn_grab02", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_SMILE01,
				new ConverterData("cmn_smile01", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_SMILE02,
				new ConverterData("cmn_smile02", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_SMILE02_KAISER,
				new ConverterData("cmn_smile02_Kaiser", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_SMILE03,
				new ConverterData("cmn_smile03", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_SMILE04,
				new ConverterData("cmn_smile04", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_SMILE05,
				new ConverterData("cmn_smile05", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_NOREACTION01,
				new ConverterData("cmn_noReaction01", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_HIGHTOUCH01,
				new ConverterData("cmn_highTouch01", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_SHAKEHAND01,
				new ConverterData("cmn_shakeHand01", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_SWING01,
				new ConverterData("cmn_swing01", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK01,
				new ConverterData("cmn_prank01", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK02,
				new ConverterData("cmn_prank02", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK03,
				new ConverterData("cmn_prank03", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK04,
				new ConverterData("cmn_prank04", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK05,
				new ConverterData("cmn_prank05", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK06,
				new ConverterData("cmn_prank06", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK07,
				new ConverterData("cmn_prank07", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK08,
				new ConverterData("cmn_prank08", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK09,
				new ConverterData("cmn_prank09", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK10,
				new ConverterData("cmn_prank10", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK11,
				new ConverterData("cmn_prank11", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK12,
				new ConverterData("cmn_prank12", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK13,
				new ConverterData("cmn_prank13", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK14,
				new ConverterData("cmn_prank14", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK15,
				new ConverterData("cmn_prank15", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK16,
				new ConverterData("cmn_prank16", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK17,
				new ConverterData("cmn_prank17", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK18,
				new ConverterData("cmn_prank18", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK19,
				new ConverterData("cmn_prank19", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK20,
				new ConverterData("cmn_prank20", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK21,
				new ConverterData("cmn_prank21", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK22,
				new ConverterData("cmn_prank22", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK23,
				new ConverterData("cmn_prank23", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK24,
				new ConverterData("cmn_prank24", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK25,
				new ConverterData("cmn_prank25", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK26,
				new ConverterData("cmn_prank26", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK27,
				new ConverterData("cmn_prank27", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK28,
				new ConverterData("cmn_prank28", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK29,
				new ConverterData("cmn_prank29", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_PRANK30,
				new ConverterData("cmn_prank30", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_ITEM,
				new ConverterData("itm_", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_SYNC,
				new ConverterData("sync_", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.ROOM_TRACK,
				new ConverterData("track_", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.NAVI,
				new ConverterData("navi_", eAnimKitConvertCategoryType.ROOM, COMMON_ROOM_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.STORY_EYE_IDLE,
				new ConverterData("eye_idle", eAnimKitConvertCategoryType.STILL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.STORY_EYE_BLINK,
				new ConverterData("eye_blink", eAnimKitConvertCategoryType.STILL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.STORY_EYE_OPEN,
				new ConverterData("eye_open", eAnimKitConvertCategoryType.STILL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.STORY_EYE_CLOSE,
				new ConverterData("eye_close", eAnimKitConvertCategoryType.STILL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.STORY_MOUTH_IDLE,
				new ConverterData("mouth_idle", eAnimKitConvertCategoryType.STILL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.STORY_MOUTH_TALK,
				new ConverterData("mouth_talk", eAnimKitConvertCategoryType.STILL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.STORY_MOUTH_OPEN,
				new ConverterData("mouth_open", eAnimKitConvertCategoryType.STILL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.STORY_MOUTH_CLOSE,
				new ConverterData("mouth_close", eAnimKitConvertCategoryType.STILL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT,
				new ConverterData("effect", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT_ATTACK,
				new ConverterData("effect_attack", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT_WALK,
				new ConverterData("effect_walk", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT_IDLE,
				new ConverterData("effect_idle", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT_SUMMON,
				new ConverterData("effect_summon", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT_DIE,
				new ConverterData("effect_die", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT_REVERSE,
				new ConverterData("effect_reverse", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.TREASURE_EFFECT01,
				new ConverterData("effect", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.TREASURE_EFFECT02,
				new ConverterData("effect_2", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.TREASURE_EFFECT03,
				new ConverterData("effect_3", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.TREASURE_EFFECT04,
				new ConverterData("effect_4", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.TREASURE_EFFECT05,
				new ConverterData("effect_5", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.TREASURE_EFFECT06,
				new ConverterData("effect_6", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.TREASURE_OUT_EFFECT01,
				new ConverterData("effect_out", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.TREASURE_OUT_EFFECT02,
				new ConverterData("effect_2_out", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.TREASURE_OUT_EFFECT03,
				new ConverterData("effect_3_out", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.TREASURE_OUT_EFFECT04,
				new ConverterData("effect_4_out", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.TREASURE_OUT_EFFECT05,
				new ConverterData("effect_4_out", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			},
			{
				eAnimKitCharacterAnimeId.TREASURE_OUT_EFFECT06,
				new ConverterData("effect_4_out", eAnimKitConvertCategoryType.NORMAL, DEFAULT_CONVERT_FUNC)
			}
		};

		private static int extractionPrefixId6(string _extractionString)
		{
			int result = -1;
			if (Regex.IsMatch(_extractionString, "^[0-9]{6,6}_"))
			{
				int.TryParse(_extractionString.Substring(0, 6), out result);
			}
			return result;
		}

		private static int extractionPrefixId2(string _extractionString)
		{
			int result = -1;
			if (Regex.IsMatch(_extractionString, "^[0-9]{2,2}_"))
			{
				int.TryParse(_extractionString.Substring(0, 2), out result);
			}
			return result;
		}

		private static void DEFAULT_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId = 0, string _checkStr = "", string _animeName = "", Func<int, bool> _isUniqueMotion = null)
		{
			_binaryId = eAnimKitBinaryAnimationId.NONE;
			_id = -1;
		}

		private static void COMMON_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (!_checkStr.Contains(_animeName))
			{
				return;
			}
			int num = extractionPrefixId6(_checkStr);
			int num2 = extractionPrefixId2(_checkStr);
			if (num != -1 || num2 != -1)
			{
				if (UnitUtility.IsEnemyUnit(_atlasFileUnitId))
				{
					_id = _atlasFileUnitId;
					_binaryId = eAnimKitBinaryAnimationId.BATTLE;
				}
				else if (num2 != -1)
				{
					_id = num2;
					_binaryId = eAnimKitBinaryAnimationId.COMMON_BATTLE;
				}
				else if (_isUniqueMotion(num))
				{
					_id = num;
					_binaryId = eAnimKitBinaryAnimationId.COMMON_BATTLE;
				}
				else
				{
					_id = _atlasFileUnitId;
					_binaryId = eAnimKitBinaryAnimationId.BATTLE;
				}
			}
		}

		private static void LOADING_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (!_checkStr.Contains(_animeName))
			{
				return;
			}
			int num = extractionPrefixId6(_checkStr);
			int num2 = extractionPrefixId2(_checkStr);
			if ((num != -1 || num2 != -1) && !UnitUtility.IsEnemyUnit(_atlasFileUnitId))
			{
				if (num2 != -1)
				{
					_id = num2;
					_binaryId = eAnimKitBinaryAnimationId.LOADING;
				}
				else if (num != -1)
				{
					_id = num;
					_binaryId = eAnimKitBinaryAnimationId.LOADING;
				}
			}
		}

		private static void COMBINE_JOY_RESULT_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (Regex.IsMatch(_checkStr, "[0-9]{6,6}_joy_combine_[0-9]{6,6}") && int.TryParse(_checkStr.Substring(19, 6), out _id))
			{
				_binaryId = eAnimKitBinaryAnimationId.COMBINE_JOY_RESULT;
			}
		}

		private static void BATTLE_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (!_checkStr.Contains(_animeName))
			{
				return;
			}
			int num = extractionPrefixId6(_checkStr);
			int num2 = extractionPrefixId2(_checkStr);
			if (num != -1 || num2 != -1)
			{
				if (UnitUtility.IsEnemyUnit(_atlasFileUnitId))
				{
					_id = _atlasFileUnitId;
					_binaryId = eAnimKitBinaryAnimationId.BATTLE;
				}
				else if (num2 != -1)
				{
					_id = num2;
					_binaryId = eAnimKitBinaryAnimationId.COMMON_BATTLE;
				}
				else
				{
					_id = num;
					_binaryId = eAnimKitBinaryAnimationId.BATTLE;
				}
			}
		}

		private static void COMMON_ROOM_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			_id = 0;
			if (Regex.IsMatch(_checkStr, "sync_[0-9]{6,6}_[A-J]"))
			{
				if (int.TryParse(_checkStr.Substring(5, 6), out _id))
				{
					_binaryId = eAnimKitBinaryAnimationId.ROOM_SYNC;
				}
			}
			else if (Regex.IsMatch(_checkStr, "track_[0-9]{6,6}"))
			{
				if (int.TryParse(_checkStr.Substring(6, 6), out _id))
				{
					_binaryId = eAnimKitBinaryAnimationId.ROOM_TRACK;
				}
			}
			else if (Regex.IsMatch(_checkStr, "itm_[0-9]{6,6}_"))
			{
				if (int.TryParse(_checkStr.Substring(4, 6), out _id))
				{
					_binaryId = eAnimKitBinaryAnimationId.ROOM_ITEM;
				}
			}
			else if (_checkStr.Contains("nebbia") || _checkStr.Contains("pipple"))
			{
				_binaryId = eAnimKitBinaryAnimationId.UNIQUE_ROOM;
				_id = _atlasFileUnitId;
			}
			else if (_checkStr.Contains(192101.ToString()))
			{
				_binaryId = eAnimKitBinaryAnimationId.UNIQUE_ROOM;
				_id = 192101;
			}
			else if (_checkStr.Contains("cmn_"))
			{
				_binaryId = eAnimKitBinaryAnimationId.COMMON_ROOM;
			}
			else if (Regex.IsMatch(_checkStr, "navi_[0-9]{3,3}_") && int.TryParse(_checkStr.Substring(5, 3), out _id))
			{
				_binaryId = eAnimKitBinaryAnimationId.NAVI;
			}
		}

		private static void POSING_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.POSING;
				int num = extractionPrefixId6(_checkStr);
				_id = ((num != -1) ? num : 0);
			}
		}

		private static void DEFEAT_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.DEFEAT;
				int num = extractionPrefixId6(_checkStr);
				_id = ((num != -1) ? num : 0);
			}
		}

		private static void EVENT_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.EVENT;
				_id = 0;
			}
		}

		private static void EVENT_STORY_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			_id = 0;
			if (Regex.IsMatch(_checkStr, "event_[0-9]{2}_[0-9]{5}_") && int.TryParse(_checkStr.Substring(9, 5), out _id))
			{
				_binaryId = eAnimKitBinaryAnimationId.EVENT_STORY;
			}
		}

		private static void SMILE_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.SMILE;
				int num = extractionPrefixId6(_checkStr);
				_id = ((num != -1) ? num : 0);
			}
		}

		private static void MAP_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.MAP;
				_id = _atlasFileUnitId;
			}
		}

		private static void MINIGAME_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.MINIGAME;
				if (_checkStr.Contains("pct_"))
				{
					_id = 1001;
				}
				else if (_checkStr.Contains("fke_"))
				{
					_id = 1002;
				}
				else if (_checkStr.Contains("pkb_"))
				{
					_id = 1006;
				}
				else if (_checkStr.Contains("cgg_"))
				{
					_id = 1007;
				}
				else if (_checkStr.Contains("psy_"))
				{
					_id = 1008;
				}
				else if (_checkStr.Contains("taq_"))
				{
					_id = 1009;
				}
				else if (_checkStr.Contains("sjr_"))
				{
					_id = 1010;
				}
			}
		}

		private static void EKD_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.EKD;
				_id = 0;
			}
		}

		private static void OMP_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.OMP;
				_id = 0;
			}
		}

		private static void UEK_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.UEK;
				_id = extractionPrefixId2(_checkStr);
			}
		}

		private static void NYX_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.NYX;
				_id = 0;
			}
		}

		private static void MHP_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.MHP;
				_id = 0;
			}
		}

		private static void NOP_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.NOP;
				_id = 0;
			}
		}

		private static void DSB_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.DSB;
				_id = 0;
			}
		}

		private static void BLB_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.BLB_DRAMA;
				_id = 0;
			}
		}

		private static void WAC_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.WAC;
				_id = 0;
			}
		}

		private static void LSV_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.LSV;
				_id = 0;
			}
		}

		private static void RUN_JUMP_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.RUN_JUMP;
				int num = extractionPrefixId6(_checkStr);
				_id = ((num != -1) ? num : 0);
			}
		}

		private static void NO_WEAPON_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.NO_WEAPON;
				int num = extractionPrefixId6(_checkStr);
				_id = ((num != -1) ? num : 0);
			}
		}

		private static void RACE_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.RACE;
				int num = extractionPrefixId6(_checkStr);
				_id = ((num != -1) ? num : 0);
			}
		}

		private static void DEAR_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.DEAR;
				int num = extractionPrefixId6(_checkStr);
				_id = ((num != -1) ? num : 0);
			}
		}

		private static void TRAVEL_DRAMA_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.NONE;
				if (_checkStr.Contains("travel_r_"))
				{
					_binaryId = eAnimKitBinaryAnimationId.TRAVEL_TOP_ROOM_UNIT;
				}
				else if (_checkStr.Contains("travel_b_"))
				{
					_binaryId = eAnimKitBinaryAnimationId.TRAVEL_TOP_BATTLE_UNIT;
				}
				else if (_checkStr.Contains("travel_exdrama_r_"))
				{
					_binaryId = eAnimKitBinaryAnimationId.TRAVEL_EX_DRAMA_ROOM_UNIT;
				}
				else if (_checkStr.Contains("travel_exdrama_b_"))
				{
					_binaryId = eAnimKitBinaryAnimationId.TRAVEL_EX_DRAMA_BATTLE_UNIT;
				}
				int num = extractionPrefixId6(_checkStr);
				_id = ((num != -1) ? num : 0);
			}
		}

		private static void TRAVEL_TOP_DRAMA_CONVERT_FUNC(out eAnimKitBinaryAnimationId _binaryId, out int _id, int _atlasFileUnitId, string _checkStr, string _animeName, Func<int, bool> _isUniqueMotion)
		{
			DEFAULT_CONVERT_FUNC(out _binaryId, out _id);
			if (_checkStr.Contains(_animeName))
			{
				_binaryId = eAnimKitBinaryAnimationId.NONE;
				if (_checkStr.Contains("travel_topdrama_r_"))
				{
					_binaryId = eAnimKitBinaryAnimationId.TRAVEL_TOP_DRAMA_ROOM_UNIT;
				}
				else if (_checkStr.Contains("travel_topdrama_b_"))
				{
					_binaryId = eAnimKitBinaryAnimationId.TRAVEL_TOP_DRAMA_BATTLE_UNIT;
				}
				if (Regex.IsMatch(_checkStr, "travel_topdrama_[br]{1}_[0-9]{4}"))
				{
					int.TryParse(Regex.Match(_checkStr, "[0-9]").Value, out _id);
				}
			}
		}
	}
}
