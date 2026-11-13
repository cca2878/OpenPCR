using global::OpenPCR.Foundation.Audio;
using global::OpenPCR.Runtime.Api;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Common;
using global::OpenPCR.Foundation;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class SoundManager : ManagerSingleton<SoundManager>
	{

		private static readonly string SOUND_PLAY_SPEED_CATEGORY = "SpeedControl";

		public void PlaySpecialVoice(int _groupId, int _groupUnitId, eVoiceType _type, int _index)
		{
			throw new global::OpenPCR.OutOfBoundaryException("SoundManager.PlaySpecialVoice: Native audio/resource playback production; caller group selection, condition and RNG remain real.");
		}


		public class eVoiceType_DictComparer : global::System.Collections.Generic.IEqualityComparer<global::Elements.SoundManager.eVoiceType>
		{
			[global::OpenPCR.Migration("02922aff-123a-4df7-a9e3-0d37e6dcac6f")]
			public eVoiceType_DictComparer() { }
			[global::OpenPCR.Migration("467071d7-aea9-4052-b878-ded497a1a9c5")]
			public bool Equals(global::Elements.SoundManager.eVoiceType _x, global::Elements.SoundManager.eVoiceType _y) { return _x == _y; }
			[global::OpenPCR.Migration("90c5cb75-76e4-4ce9-9dd8-8310fea39b91")]
			public int GetHashCode(global::Elements.SoundManager.eVoiceType _obj) { return (int)_obj; }
		}


		public enum eVoiceType
		{
			TITLE_PREIN,
			TITLE_CUSTOMIZE,
			MYPAGE,
			GACHA,
			LOVE,
			DETAIL,
			RARITY,
			ATTACK,
			ATTACK_MODE_CHANGE,
			DAMAGE,
			DAMAGE_MODE_CHANGE,
			RETIRE,
			UNION_BURST,
			SKILL,
			SP_SKILL,
			SUB_UNION_BURST,
			WAVE_START,
			WAVE_START_FOR_MULTI,
			NEXT_WAVE,
			NEXT_WAVE_FOR_MULTI,
			LAST_WAVE,
			LAST_WAVE_FOR_MULTI,
			WIN,
			LOSE,
			APPLAUD,
			APPLAUD_MODE_CHANGE,
			APPLAUD_FOR_MULTI,
			THANKS,
			THANKS_MODE_CHANGE,
			THANKS_FOR_MULTI,
			BATTLE_START,
			BATTLE_RESULT_WIN,
			SPECIAL_BATTLE_START,
			SPECIAL_CUT_IN,
			SPECIAL_WAVE_START,
			SPECIAL_NEXT_WAVE,
			SPECIAL_LAST_WAVE,
			SPECIAL_CLEAR_WIN,
			SPECIAL_WIN,
			SPECIAL_CLOSE_GAME,
			SPECIAL_LOSE,
			SPECIAL_APPLAUD,
			SPECIAL_THANKS,
			NAVI,
			LOGIN_BONUS,
			STORY,
			CUT_IN,
			CUT_IN_MODE_CHANGE,
			OTHER,
			EVENT_NAVI,
			ROOM_AUTO,
			ROOM_TAP,
			ROOM_TIME,
			ROOM_ITEM,
			ROOM_APPEAR,
			SPECIAL_LOGIN,
			BIRTH_DAY,
			VOTE_RESULT,
			MINIGAME,
			CLAN_STAMP,
			EVENT_NAVI_SUB,
			CARAVAN_COIN_SHOP,
			CARAVAN_JOY,
			CARAVAN_SNEEZE,
			LABYRINTH_COIN_SHOP,
			LABYRINTH_TOP,
			SEASONPASS
		}

		public class SoundParam
		{
			public string CueSheetName { get; private set; }

			public string CueName { get; private set; }

			public SoundParam(string _cueSheetName, string _cueName)
			{ throw new global::OpenPCR.MigrationPendingException("Elements.SoundManager\u002BSoundParam::.ctor(System.String,System.String)", "Behavior implementation is not available."); }
		}

		public static readonly Dictionary<eBGM, SoundParam> BGM_PARAM_DICT = new Dictionary<eBGM, SoundParam>
		{
			{
				eBGM.TITLE,
				new SoundParam("preinstall", "bgm_title_main_1")
			},
			{
				eBGM.HOME,
				new SoundParam("preinstall", "bgm_M36_02_division")
			},
			{
				eBGM.UNIT,
				new SoundParam("bgm_M53", "bgm_M53")
			},
			{
				eBGM.STORY,
				new SoundParam("preinstall", "bgm_M36_02_division")
			},
			{
				eBGM.QUEST,
				new SoundParam("preinstall", "bgm_M36_02_division")
			},
			{
				eBGM.QUEST_MAIN,
				new SoundParam("bgm_M61", "bgm_M61")
			},
			{
				eBGM.QUEST_BATTLE,
				new SoundParam("bgm_M06", "bgm_M06")
			},
			{
				eBGM.QUEST_BATTLE_BOSS,
				new SoundParam("bgm_M07", "bgm_M07")
			},
			{
				eBGM.QUEST_RESULT_WIN,
				new SoundParam("bgm_win_01_loop", "bgm_win_01_loop")
			},
			{
				eBGM.DUNGEON,
				new SoundParam("bgm_M20", "bgm_M20_00_Menu")
			},
			{
				eBGM.DUNGEON_BATTLE,
				new SoundParam("bgm_M38", "bgm_M38")
			},
			{
				eBGM.SECRET_DUNGEON,
				new SoundParam("bgm_M541", "bgm_M541")
			},
			{
				eBGM.CLAN_BATTLE,
				new SoundParam("bgm_M17", "bgm_M17")
			},
			{
				eBGM.HATSUNE_GACHA,
				new SoundParam("bgm_M61", "bgm_M61_04")
			},
			{
				eBGM.HATSUNE_XEH_TOP,
				new SoundParam("bgm_M638", "bgm_M638")
			},
			{
				eBGM.TOWER,
				new SoundParam("bgm_M125", "bgm_M125")
			},
			{
				eBGM.DIMENSION_FAULT,
				new SoundParam("bgm_MC007", "bgm_MC007")
			},
			{
				eBGM.ARENA,
				new SoundParam("bgm_M06", "bgm_M06")
			},
			{
				eBGM.ARENA_BATTLE,
				new SoundParam("bgm_M07", "bgm_M07")
			},
			{
				eBGM.GRAND_ARENA_BATTLE_1,
				new SoundParam("bgm_M17", "bgm_M17")
			},
			{
				eBGM.GRAND_ARENA_BATTLE_2,
				new SoundParam("bgm_M17", "bgm_M17_02")
			},
			{
				eBGM.GRAND_ARENA_BATTLE_3,
				new SoundParam("bgm_M17", "bgm_M17_03")
			},
			{
				eBGM.GACHA,
				new SoundParam("bgm_M45", "bgm_M45")
			},
			{
				eBGM.GACHA_PLAY,
				new SoundParam("bgm_M44", "bgm_M44_Gacha_a_01")
			},
			{
				eBGM.GACHA_POSTER_PLAY,
				new SoundParam("bgm_M44", "bgm_M44_Gacha_a_02")
			},
			{
				eBGM.GACHA_MOVE_PLAY,
				new SoundParam("bgm_M44", "bgm_M44_Gacha_a_03")
			},
			{
				eBGM.ROOM_DAYTIME,
				new SoundParam("bgm_M35", "bgm_M35")
			},
			{
				eBGM.ROOM_EVENING,
				new SoundParam("bgm_M35", "bgm_M35")
			},
			{
				eBGM.ROOM_NIGHT,
				new SoundParam("bgm_M35", "bgm_M35")
			},
			{
				eBGM.SHOP,
				new SoundParam("bgm_M04", "bgm_M04_02NoIntro01")
			},
			{
				eBGM.ALBUM,
				new SoundParam("bgm_M24", "bgm_M24")
			},
			{
				eBGM.NEW_CHARACTER_INTRODUCTION,
				new SoundParam("bgm_M36", "bgm_M36")
			},
			{
				eBGM.CARTOON,
				new SoundParam("bgm_M33", "bgm_M33")
			},
			{
				eBGM.JUKEBOX_TOP,
				new SoundParam("bgm_M61", "bgm_M61_04")
			},
			{
				eBGM.JUKEBOX_CHANGE_LIST,
				new SoundParam("bgm_M05", "bgm_M05")
			},
			{
				eBGM.VOTE_TOP,
				new SoundParam("bgm_M65", "bgm_M65")
			},
			{
				eBGM.VOTE_RESULT,
				new SoundParam("bgm_M36", "bgm_M36")
			},
			{
				eBGM.EVENT_ROBO_LLAMA_TOP,
				new SoundParam("bgm_M219", "bgm_M219")
			},
			{
				eBGM.EVENT_SPACE_BATTLE_TOP,
				new SoundParam("bgm_M348", "bgm_M348")
			},
			{
				eBGM.FRIEND,
				new SoundParam("bgm_M101", "bgm_M101_Friend")
			},
			{
				eBGM.KAISER_BATTLE_END,
				new SoundParam("bgm_M313", "bgm_M313")
			},
			{
				eBGM.UEK_TOP,
				new SoundParam("bgm_M342", "bgm_M342")
			},
			{
				eBGM.SHIORI_EVENT_SELECT,
				new SoundParam("bgm_M33", "bgm_M33_02")
			},
			{
				eBGM.TRAVEL_TOP,
				new SoundParam("bgm_M615", "bgm_M615")
			},
			{
				eBGM.TME_TOP,
				new SoundParam("bgm_M375", "bgm_M375")
			},
			{
				eBGM.MINIGAME_PCT_TOP,
				new SoundParam("bgm_M189", "bgm_M189")
			},
			{
				eBGM.MINIGAME_PCT_SHIZURU,
				new SoundParam("bgm_M199", "bgm_M199")
			},
			{
				eBGM.MINIGANE_PCT_ERIKO,
				new SoundParam("bgm_M198", "bgm_M198")
			},
			{
				eBGM.SRT_IN_GAME,
				new SoundParam("bgm_M277", "bgm_M277_game")
			},
			{
				eBGM.MINIGAME_PKB,
				new SoundParam("bgm_M352", "bgm_M352_2AVer")
			},
			{
				eBGM.MINIGAME_PKB_GAME_START,
				new SoundParam("bgm_M398", "bgm_M398")
			},
			{
				eBGM.MINIGAME_SRT_TOP,
				new SoundParam("bgm_M273", "bgm_M273")
			},
			{
				eBGM.NONE,
				new SoundParam(string.Empty, string.Empty)
			}
		};
		private static readonly Dictionary<eSE, SoundParam> SE_SOUND_PARAM_DICT = new Dictionary<eSE, SoundParam>
		{
			{
				eSE.RACE_LOGO,
				new SoundParam("se_10006800", "se_10006800_race_logo")
			},
			{
				eSE.RACE_START,
				new SoundParam("se_10006800", "se_10006800_race_start")
			},
			{
				eSE.RACE_GOAL,
				new SoundParam("se_10006800", "se_10006800_race_goal")
			},
			{
				eSE.RACE_AWARD,
				new SoundParam("se_10006800", "se_10006800_race_award")
			},
			{
				eSE.RACE_STONE_CRUSH,
				new SoundParam("se_10006800", "se_10006800_race_stone_crush")
			},
			{
				eSE.RACE_JUMP,
				new SoundParam("se_10006800", "se_10006800_race_jump")
			},
			{
				eSE.RACE_WATER_SPLASH,
				new SoundParam("se_10006800", "se_10006800_race_water_splash")
			},
			{
				eSE.RACE_FAST_RUN,
				new SoundParam("se_10006800", "se_10006800_race_fast_run")
			},
			{
				eSE.RACE_WARP,
				new SoundParam("se_10006800", "se_10006800_race_warp")
			},
			{
				eSE.RACE_NOTE,
				new SoundParam("se_10006800", "se_10006800_race_note")
			},
			{
				eSE.RACE_SWEAT,
				new SoundParam("se_10006800", "se_10006800_race_sweat")
			},
			{
				eSE.RACE_CHARA_SELECT,
				new SoundParam("se_10006800", "se_10006800_race_chara_select")
			},
			{
				eSE.RACE_FANFARE,
				new SoundParam("se_10006800", "se_10006800_race_fanfare")
			},
			{
				eSE.RACE_CROWD,
				new SoundParam("se_10006800", "se_10006800_race_crowd")
			},
			{
				eSE.RACE_GOAL_POSE,
				new SoundParam("se_10006800", "se_10006800_race_goal_pose")
			},
			{
				eSE.RACE_SURF_UP,
				new SoundParam("se_10030800", "se_10030800_surf_up")
			},
			{
				eSE.RACE_SURF_LOOP,
				new SoundParam("se_10030800", "se_10030800_surf_loop")
			},
			{
				eSE.RACE_SURF_DOWN,
				new SoundParam("se_10030800", "se_10030800_surf_down")
			},
			{
				eSE.RACE_CARPET_SPOTLIGHT_ON,
				new SoundParam("se_10035700", "se_10035700_spot_light_on")
			},
			{
				eSE.RACE_CARPET_SPOTLIGHT_OFF,
				new SoundParam("se_10035700", "se_10035700_spot_light_off")
			},
			{
				eSE.RACE_CARPET_STAR,
				new SoundParam("se_10035700", "se_10035700_stars")
			},
			{
				eSE.SEKAI_FADE_IN,
				new SoundParam("se_10008000", "se_robolima_ui_01")
			},
			{
				eSE.FKE_DRAG,
				new SoundParam("se_minigame_1002", "se_minigame_1002_drag_001")
			},
			{
				eSE.FKE_RELEASE,
				new SoundParam("se_minigame_1002", "se_minigame_1002_release_001")
			},
			{
				eSE.FKE_WIPE,
				new SoundParam("se_minigame_1002", "se_minigame_1002_wipe_001")
			},
			{
				eSE.FKE_GURUGURU,
				new SoundParam("se_minigame_1002", "se_minigame_1002_gimmick_001")
			},
			{
				eSE.FKE_DRILL,
				new SoundParam("se_minigame_1002", "se_minigame_1002_gimmick_002")
			},
			{
				eSE.FKE_GO_SPACE,
				new SoundParam("se_minigame_1002", "se_minigame_1002_gimmick_003")
			},
			{
				eSE.FKE_ROCKET,
				new SoundParam("se_minigame_1002", "se_minigame_1002_gimmick_004")
			},
			{
				eSE.FKE_LOW_FLYING,
				new SoundParam("se_minigame_1002", "se_minigame_1002_gimmick_005")
			},
			{
				eSE.FKE_SUPER_BALL,
				new SoundParam("se_minigame_1002", "se_minigame_1002_gimmick_006")
			},
			{
				eSE.FKE_NINON_EXPLOSION,
				new SoundParam("se_minigame_1002", "se_minigame_1002_gimmick_007")
			},
			{
				eSE.FKE_FUNSUI,
				new SoundParam("se_minigame_1002", "se_minigame_1002_gimmick_008")
			},
			{
				eSE.FKE_AYUMI_FAN,
				new SoundParam("se_minigame_1002", "se_minigame_1002_gimmick_009")
			},
			{
				eSE.FKE_MESARTHIM,
				new SoundParam("se_minigame_1002", "se_minigame_1002_gimmick_010")
			},
			{
				eSE.FKE_JUMP_BANANA,
				new SoundParam("se_minigame_1002", "se_minigame_1002_gimmick_011")
			},
			{
				eSE.FKE_SLIDING_BANANA,
				new SoundParam("se_minigame_1002", "se_minigame_1002_gimmick_012")
			},
			{
				eSE.FKE_ZIZ,
				new SoundParam("se_minigame_1002", "se_minigame_1002_gimmick_013")
			},
			{
				eSE.FKE_TORNADO,
				new SoundParam("se_minigame_1002", "se_minigame_1002_gimmick_014")
			},
			{
				eSE.FKE_RAIN,
				new SoundParam("se_minigame_1002", "se_minigame_1002_gimmick_015")
			},
			{
				eSE.FKE_HEAD_WIND,
				new SoundParam("se_minigame_1002", "se_minigame_1002_gimmick_016")
			},
			{
				eSE.FKE_AKUDAIKAN,
				new SoundParam("se_minigame_1002", "se_minigame_1002_gimmick_017")
			},
			{
				eSE.FKE_THUNDER,
				new SoundParam("se_minigame_1002", "se_minigame_1002_gimmick_018")
			},
			{
				eSE.FKE_BOUND,
				new SoundParam("se_minigame_1002", "se_minigame_1002_bound_001")
			},
			{
				eSE.FKE_SLIDING,
				new SoundParam("se_minigame_1002", "se_minigame_1002_gimmick_019")
			},
			{
				eSE.KMK_PUNCH,
				new SoundParam("se_minigame_1003", "se_minigame_1003_punch")
			},
			{
				eSE.KMK_KICK,
				new SoundParam("se_minigame_1003", "se_minigame_1003_kick")
			},
			{
				eSE.KMK_HIT,
				new SoundParam("se_minigame_1003", "se_minigame_1003_hit")
			},
			{
				eSE.KMK_DAMAGE,
				new SoundParam("se_minigame_1003", "se_minigame_1003_damage")
			},
			{
				eSE.KMK_SNAKE_ATTACK,
				new SoundParam("se_minigame_1003", "se_minigame_1003_snake_attack")
			},
			{
				eSE.KMK_SNAKE_DAMAGE_LEFT,
				new SoundParam("se_minigame_1003", "se_minigame_1003_snake_damage_left")
			},
			{
				eSE.KMK_SNAKE_DAMAGE_RIGHT,
				new SoundParam("se_minigame_1003", "se_minigame_1003_snake_damage_right")
			},
			{
				eSE.KMK_SNAKE_DEAD_LEFT,
				new SoundParam("se_minigame_1003", "se_minigame_1003_snake_die_left")
			},
			{
				eSE.KMK_SNAKE_DEAD_RIGHT,
				new SoundParam("se_minigame_1003", "se_minigame_1003_snake_die_right")
			},
			{
				eSE.KMK_HIT_1,
				new SoundParam("se_minigame_1003", "se_minigame_1003_hit_01")
			},
			{
				eSE.KMK_HIT_2,
				new SoundParam("se_minigame_1003", "se_minigame_1003_hit_02")
			},
			{
				eSE.KMK_HIT_3,
				new SoundParam("se_minigame_1003", "se_minigame_1003_hit_03")
			},
			{
				eSE.KMK_HEART_BREAK,
				new SoundParam("se_minigame_1003", "se_minigame_1003_heart_eff")
			},
			{
				eSE.KMK_FEVER_START,
				new SoundParam("se_minigame_1003", "se_minigame_1003_feverstart")
			},
			{
				eSE.KMK_FEVER_BURST,
				new SoundParam("se_minigame_1003", "se_minigame_1003_skill")
			},
			{
				eSE.KMK_FEVER_GAUGE_UP,
				new SoundParam("se_minigame_1003", "se_minigame_1003_fevergauge")
			},
			{
				eSE.KMK_EXTRA_RELEASE,
				new SoundParam("se_minigame_1003", "se_minigame_1003_extra")
			},
			{
				eSE.BTL_HIT_DUALSWORD_1,
				new SoundParam("se_battle", "se_btle_hit_dualsword_1")
			},
			{
				eSE.BTL_HIT_SLINGSHOT_1,
				new SoundParam("se_battle", "se_btl_hit_slingshot_1")
			},
			{
				eSE.BTL_HIT_GLORY_1,
				new SoundParam("se_battle", "se_btl_hit_glory_1")
			},
			{
				eSE.BTL_HIT_CROSSBOW_1,
				new SoundParam("se_battle", "se_btl_hit_crossbow_1")
			},
			{
				eSE.BTL_HIT_HORN_1,
				new SoundParam("se_battle", "se_btl_hit_horn_1")
			},
			{
				eSE.BTL_HIT_PEN_1,
				new SoundParam("se_battle", "se_btl_hit_pen_1")
			},
			{
				eSE.BTL_HIT_AX_2,
				new SoundParam("se_battle", "se_btl_hit_ax_2")
			},
			{
				eSE.BTL_HIT_WING,
				new SoundParam("se_battle", "se_btl_hit_wing")
			},
			{
				eSE.BTL_HIT_AMES,
				new SoundParam("se_battle", "se_btl_hit_ames")
			},
			{
				eSE.BTL_HIT_KAISER,
				new SoundParam("se_battle", "se_btl_hit_kaiser")
			},
			{
				eSE.BTL_HIT_MITSUKI_NY,
				new SoundParam("se_battle", "se_btl_hit_mitsuki_ny")
			},
			{
				eSE.BTL_HIT_MISORA,
				new SoundParam("se_battle", "se_btl_hit_misora")
			}
		};
		private Dictionary<eSE, eSEPartial> PARAM_SEPARTIAL_TYPE_DICT = new Dictionary<eSE, eSEPartial>
		{
			{
				eSE.SYS_BUTTON,
				eSEPartial.SYS_BUTTON
			},
			{
				eSE.SYS_BUTTON_LARGE,
				eSEPartial.SYS_BUTTON_LARGE
			},
			{
				eSE.SYS_CANCEL,
				eSEPartial.SYS_CANCEL
			},
			{
				eSE.SYS_SWIPE,
				eSEPartial.SYS_SWIPE
			},
			{
				eSE.SYS_TITLE_START,
				eSEPartial.SYS_TITLE_START
			},
			{
				eSE.SYS_BUTTON_SMALL,
				eSEPartial.SYS_BUTTON_SMALL
			},
			{
				eSE.SYS_CANCEL_SMALL,
				eSEPartial.SYS_CANCEL_SMALL
			},
			{
				eSE.SYS_WINDOW_APPEAR,
				eSEPartial.SYS_WINDOW_APPEAR
			},
			{
				eSE.SYS_ICON_APPEAR_01,
				eSEPartial.SYS_ICON_APPEAR_01
			},
			{
				eSE.SYS_ICON_CHANGE_01,
				eSEPartial.SYS_ICON_CHANGE_01
			},
			{
				eSE.BTL_DAMAGE,
				eSEPartial.BTL_DAMAGE
			},
			{
				eSE.BTL_BUTTON_SKILL,
				eSEPartial.BTL_BUTTON_SKILL
			},
			{
				eSE.BTL_NEXT_WAVE,
				eSEPartial.BTL_NEXTWAVE
			},
			{
				eSE.BTL_HIT_ARROW_1,
				eSEPartial.BTL_HIT_ARROW_1
			},
			{
				eSE.BTL_HIT_AX_1,
				eSEPartial.BTL_HIT_AX_1
			},
			{
				eSE.BTL_HIT_KNUCKLE_1,
				eSEPartial.BTL_HIT_KNUCKLE_1
			},
			{
				eSE.BTL_FIREARM_ARROW_1,
				eSEPartial.BTL_FIREARM_ARROW_1
			},
			{
				eSE.BTL_HIT_LONGSWORD_1,
				eSEPartial.BTL_HIT_LONGSWORD_1
			},
			{
				eSE.BTL_HIT_SHORTSWORD_1,
				eSEPartial.BTL_HIT_SHORTSWORD_1
			},
			{
				eSE.BTL_HIT_SPEAR_1,
				eSEPartial.BTL_HIT_SPEAR_1
			},
			{
				eSE.BTL_HIT_SWORD_1,
				eSEPartial.BTL_HIT_SWORD_1
			},
			{
				eSE.BTL_HIT_WAND_1,
				eSEPartial.BTL_HIT_WAND_1
			},
			{
				eSE.BTL_HIT_BITE_1,
				eSEPartial.BTL_HIT_BITE_1
			},
			{
				eSE.BTL_HIT_COMMON_1,
				eSEPartial.BTL_HIT_COMMON_1
			},
			{
				eSE.BTL_HIT_COMMON_2,
				eSEPartial.BTL_HIT_COMMON_2
			},
			{
				eSE.BTL_HIT_EXPLO_1,
				eSEPartial.BTL_HIT_EXPLO_1
			},
			{
				eSE.BTL_HIT_HAMMER_1,
				eSEPartial.BTL_HIT_HAMMER_1
			},
			{
				eSE.BTL_HIT_SCRATCH_2,
				eSEPartial.BTL_HIT_SCRATCH_2
			},
			{
				eSE.BTL_HIT_SCRATCH_1,
				eSEPartial.BTL_HIT_SCRATCH_1
			},
			{
				eSE.BTL_HIT_DUALKNUCKLE_1,
				eSEPartial.BTL_HIT_DUALKNUCKLE_1
			},
			{
				eSE.BTL_DIE_FADEOUT,
				eSEPartial.BTL_DIE_FADEOUT
			},
			{
				eSE.BTL_BATTLE_START,
				eSEPartial.BTL_BATTLE_START
			},
			{
				eSE.MULTI_TARGETS_BREAK,
				eSEPartial.MULTI_TARGETS_BREAK
			},
			{
				eSE.MULTI_TARGETS_START,
				eSEPartial.MULTI_TARGETS_START
			},
			{
				eSE.MULTI_TARGETS_REVIVAL,
				eSEPartial.MULTI_TARGETS_REVIVAL
			},
			{
				eSE.BTL_RESULT_WIN,
				eSEPartial.BTL_RESULT_WIN
			},
			{
				eSE.BTL_DROP_MANA,
				eSEPartial.BTL_DROP_MANA
			},
			{
				eSE.BTL_GET_BOX_MANA,
				eSEPartial.BTL_GET_BOX_MANA
			},
			{
				eSE.BTL_RESULT_TRANSITION,
				eSEPartial.BTL_RESULT_TRANSITION
			},
			{
				eSE.BTL_RESULT_STAR,
				eSEPartial.BTL_RESULT_STAR
			},
			{
				eSE.BTL_SUPPORT_GAUGE,
				eSEPartial.BTL_SUPPORT_GAUGE
			},
			{
				eSE.BTL_CLAN_RESULT,
				eSEPartial.BTL_CLAN_RESULT
			},
			{
				eSE.BTL_NORMAL_BOX_01,
				eSEPartial.BTL_NORMAL_BOX_01
			},
			{
				eSE.BTL_BRONZE_BOX_01,
				eSEPartial.BTL_BRONZE_BOX_01
			},
			{
				eSE.BTL_SILVER_BOX_01,
				eSEPartial.BTL_SILVER_BOX_01
			},
			{
				eSE.BTL_GOLD_BOX_01,
				eSEPartial.BTL_GOLD_BOX_01
			},
			{
				eSE.BTL_SILVER_BOX_02,
				eSEPartial.BTL_SILVER_BOX_02
			},
			{
				eSE.BTL_GOLD_BOX_02,
				eSEPartial.BTL_GOLD_BOX_02
			},
			{
				eSE.BTL_CLAN_CP_UP,
				eSEPartial.BTL_CLAN_CP_UP
			},
			{
				eSE.BTL_CLAN_GAUGE_UP,
				eSEPartial.BTL_CLAN_GAGE_UP
			},
			{
				eSE.BTL_TOWER_TIMEUP,
				eSEPartial.BTL_TOWER_TIMEUP
			},
			{
				eSE.BTL_TOWER_TIMECOUNT,
				eSEPartial.BTL_TOWER_TIMEUP_COUNT
			},
			{
				eSE.BTL_ARENA_SKIP,
				eSEPartial.BTL_ARENA_SKIP
			},
			{
				eSE.SYS_ARENA_RANK_UP,
				eSEPartial.NONE
			},
			{
				eSE.SYS_CHAR_AWAKE,
				eSEPartial.NONE
			},
			{
				eSE.SYS_CLOSE_MENU,
				eSEPartial.SYS_CLOSE_MENU
			},
			{
				eSE.BTL_RESULT_GAGE_UP,
				eSEPartial.BTL_RESULT_GAGE_UP
			},
			{
				eSE.SYS_WORLDMAP_CLOSE,
				eSEPartial.SYS_WORLDMAP_CLOSE
			},
			{
				eSE.SYS_WORLDMAP_OPEN,
				eSEPartial.SYS_WORLDMAP_OPEN
			},
			{
				eSE.SYS_WORLDMAP_STAGE_ICON,
				eSEPartial.SYS_WORLDMAP_STAGE_ICON
			},
			{
				eSE.SYS_WORLDMAP_WHOOSH,
				eSEPartial.SYS_WORLDMAP_WHOOSH
			},
			{
				eSE.SYS_WORLDMAP_WHOOSH_02,
				eSEPartial.SYS_WORLDMAP_WHOOSH_02
			},
			{
				eSE.SYS_OPEN_MENU,
				eSEPartial.SYS_OPEN_MENU
			},
			{
				eSE.SYS_RECIEVE_GIFT,
				eSEPartial.SYS_RECIEVE_GIFT
			},
			{
				eSE.SYS_RECIEVE_GIFT_02,
				eSEPartial.SYS_RECIEVE_GIFT_02
			},
			{
				eSE.SYS_TEAM_LEVEL_UP,
				eSEPartial.NONE
			},
			{
				eSE.SYS_CHAR_ICON_LANDING,
				eSEPartial.SYS_CHAR_ICON_LANDING
			},
			{
				eSE.SYS_WINDOW_SLIDE_1,
				eSEPartial.SYS_WINDOW_SLIDE_1
			},
			{
				eSE.SYS_MANABOX_OPEN_01,
				eSEPartial.SYS_MANABOX_OPEN_01
			},
			{
				eSE.SYS_UNIT_LEVEL_UP_01,
				eSEPartial.SYS_UNIT_LEVEL_UP_01
			},
			{
				eSE.SYS_UNIT_LEVEL_UP_02,
				eSEPartial.SYS_UNIT_LEVEL_UP_02
			},
			{
				eSE.SYS_UNIT_LEVEL_UP_03,
				eSEPartial.SYS_UNIT_LEVEL_UP_03
			},
			{
				eSE.SYS_CHAR_RANK_UP,
				eSEPartial.SYS_CHAR_RANK_UP
			},
			{
				eSE.SYS_CHAR_RANK_UP_SHORT,
				eSEPartial.SYS_CHAR_RANK_UP_02
			},
			{
				eSE.SYS_POWER_UP_SKILL,
				eSEPartial.SYS_POWER_UP_SKILL
			},
			{
				eSE.SYS_MAKE_EQUIPMENT,
				eSEPartial.SYS_MAKE_EQUIPMENT
			},
			{
				eSE.SYS_EQUIP,
				eSEPartial.SYS_EQUIP
			},
			{
				eSE.SYS_EQUIPMENT_POWER_UP,
				eSEPartial.SYS_EQUIPMENT_POWER_UP
			},
			{
				eSE.SYS_MEMORYPIECE_RARITY_05,
				eSEPartial.SYS_MEMORYPIECE_RARITY_05
			},
			{
				eSE.SYS_ICON_6STAR,
				eSEPartial.SYS_ICON_6STAR
			},
			{
				eSE.SYS_WINDOW_CLOSE,
				eSEPartial.SYS_WINDOW_CLOSE
			},
			{
				eSE.SYS_COMMON_COMPLETED_2,
				eSEPartial.SYS_COMMON_COMPLETED_2
			},
			{
				eSE.SYS_ITEMBOX_GET,
				eSEPartial.SYS_ITEMBOX_GET
			},
			{
				eSE.SYS_TRANSITION_WHIRLPOOL,
				eSEPartial.SYS_TRANSITION_WHIRLPOOL
			},
			{
				eSE.SYS_CHAT_01,
				eSEPartial.SYS_CHAT_01
			},
			{
				eSE.SYS_BATTLE_READY,
				eSEPartial.SYS_BATTLE_READY
			},
			{
				eSE.SYS_JOIN,
				eSEPartial.SYS_JOIN_01
			},
			{
				eSE.SYS_LEAVE,
				eSEPartial.SYS_LEAVE_01
			},
			{
				eSE.SYS_BATTLE_START,
				eSEPartial.SYS_BATTLE_START
			},
			{
				eSE.SYS_DIE,
				eSEPartial.SYS_DIE
			},
			{
				eSE.SYS_ADV_TITLE_START,
				eSEPartial.SYS_TITLE_01
			},
			{
				eSE.SYS_RECOVER_STAMINA,
				eSEPartial.SYS_RECOVER_STAMINA
			},
			{
				eSE.SYS_BUY_01,
				eSEPartial.SYS_BUY_01
			},
			{
				eSE.SYS_SHOP_OPEN,
				eSEPartial.SYS_SHOP_OPEN
			},
			{
				eSE.SYS_NAME_PLATE_APPEAR,
				eSEPartial.SYS_NAME_PLATE_APPEAR
			},
			{
				eSE.SYS_AUTO_UNIT_LV_UP,
				eSEPartial.SYS_AUTO_UNIT_LEVEL_UP
			},
			{
				eSE.SYS_AUTO_SKILL_LV_UP,
				eSEPartial.SYS_AUTO_SKILL_LEVEL_UP
			},
			{
				eSE.SYS_AUTO_EQUIP,
				eSEPartial.SYS_AUTO_EQUIP
			},
			{
				eSE.SYS_CLEAR_1,
				eSEPartial.SYS_CLEAR_1
			},
			{
				eSE.SYS_EQUIP_02,
				eSEPartial.SYS_EQUIP_02
			},
			{
				eSE.SYS_EQUIP_03,
				eSEPartial.SYS_EQUIP_03
			},
			{
				eSE.SYS_EQUIP_LEVEL_LIMIT,
				eSEPartial.SYS_EQUIP_LEVEL_LIMIT
			},
			{
				eSE.SYS_EQUIPMENT_POWER_UP_02,
				eSEPartial.SYS_EQUIPMENT_POWER_UP_02
			},
			{
				eSE.SYS_MAKE_EQUIPMENT_02,
				eSEPartial.SYS_MAKE_EQUIPMENT_02
			},
			{
				eSE.SYS_EXHIBITION_LOGO,
				eSEPartial.SYS_EXHIBITION_LOGO
			},
			{
				eSE.SYS_EQUIPMENT_POWER_UP_03,
				eSEPartial.SYS_EQUIPMENT_POWER_UP_03
			},
			{
				eSE.SYS_STAR_APPEAR02,
				eSEPartial.SYS_STAR_APPEAR02
			},
			{
				eSE.SYS_SWIPE_02,
				eSEPartial.SYS_SWIPE_02
			},
			{
				eSE.SYS_COMMON_COMPLETED_3,
				eSEPartial.SYS_COMMON_COMPLETED_3
			},
			{
				eSE.SYS_ICON_CHANGE_02,
				eSEPartial.SYS_ICON_CHANGE_02
			},
			{
				eSE.SYS_TIES_RANK_UP,
				eSEPartial.SYS_TIES_RANK_UP
			},
			{
				eSE.GACHA_TRANSITION,
				eSEPartial.GACHA_TRANSITION
			},
			{
				eSE.ROOM_OBJECT_LEVEL_UP,
				eSEPartial.ROOM_OBJECT_LEVEL_UP
			},
			{
				eSE.ROOM_PUT_OBJECT,
				eSEPartial.ROOM_PUT_OBJECT
			},
			{
				eSE.ROOM_SELECT_OBJECT,
				eSEPartial.ROOM_SELECT_OBJECT
			},
			{
				eSE.ROOM_SEND_GIFT,
				eSEPartial.ROOM_SEND_GIFT
			},
			{
				eSE.ROOM_MOVE_OBJECT,
				eSEPartial.ROOM_MOVE_OBJECT
			},
			{
				eSE.ROOM_LIKE,
				eSEPartial.ROOM_LIKE
			},
			{
				eSE.TOWER_REACH_NORMAL,
				eSEPartial.TOWER_REACH_01
			},
			{
				eSE.TOWER_REACH_BOSS,
				eSEPartial.TOWER_REACH_02
			},
			{
				eSE.TOWER_FLOOR_CHANGE_HORIZON,
				eSEPartial.TOWER_EXWINDOW
			},
			{
				eSE.TOWER_FLOOR_CHANGE_VERTICAL,
				eSEPartial.TOWER_FLOOR_01
			},
			{
				eSE.TOWER_FLOOR_CHANGE,
				eSEPartial.TOWER_FLOOR_02
			},
			{
				eSE.TOWER_CLEAR_EX,
				eSEPartial.TOWER_EXCLEAR
			},
			{
				eSE.TOWER_REACH_TOP,
				eSEPartial.TOWER_REACHTOP
			},
			{
				eSE.TOWER_CLOISTER_RELEASE,
				eSEPartial.TOWER_RELEASE_01
			},
			{
				eSE.NONE,
				eSEPartial.NONE
			}
		};
		public const float CUTE_RATE = 10f;
		public const int DEFAULT_RATE = 7;
		public const float BGM_COMMON_FADE_TIME = 0.7f;
		public const float BGM_JINGLE_WAIT_TIME = 0.5f;
		public const int VIEW_BGM_DEFAULT_CHANNEL = 0;
		public static readonly string PREINSTALL_CUESHEET = "preinstall";
		public static readonly string SE_BATTLE_CUESHEET = "se_battle";
		public const string VOICE_BTL_CUESHEET_FORMAT = "vo_btl_{0}";
		public const string SE_CUT_IN_CUESHEET_FORMAT = "se_cutin_{0}";
		public const string SE_BTL_CUESHEET_FORMAT = "se_btl_{0}";
		public const string SE_MINIGAME_CUESHEET_FORMAT = "se_minigame_{0}";
		public const int SE_BTL_CUESHEET_VARIATION_MAX = 3;
		public const string VOICE_BTG_CUESHEET_FORMAT = "vo_btg_{0}";
		public const string STORY_VOICE_CUE_SHEET_NAME_PREFIX = "v/t/vo_adv_";
		public const string EVENT_TOP_NAVI_VOICE_NAME_PREFIX = "v/vo_enavi_";
		public const int EVENT_TOP_NAVI_VOICE_NAME_NUMBER_LENGTH = 6;
		public const string SEVEN_SUB_CONTENT_VOICE_NAME_PREFIX = "v/vo_subc_";
		public const string SEVEN_SUB_CONTENT_SE_NAME_PREFIX = "s/se_subc_";
		public static readonly string VOICE_ROOM_CUESHEET_FORMAT = "vo_room_{0}";
		public const string VO_CNM_PREFIX = "v/vo_cmn_";
		public const int VO_CNM_CHAR_ID_LENGTH = 4;
		private AudioManager cuteAudioManager;
		private AudioPlayback lastPlayedBgm = AudioPlayback.Error(SoundGroup.Bgm);
		private AudioPlayback lastPlayedJingle = AudioPlayback.Error(SoundGroup.Bgm);
		private AudioPlayback lastPlayedVoice = AudioPlayback.Error(SoundGroup.Voice);
		private static readonly Dictionary<eJINGLE, SoundParam> JINGLE_PARAM_DICT = new Dictionary<eJINGLE, SoundParam>
		{
			{
				eJINGLE.JINGLE_CHAR_LEVEL_UP,
				new SoundParam("bgm_jingle_char_level_up", "bgm_jingle_char_level_up")
			},
			{
				eJINGLE.JINGLE_LEVEL_UP_01,
				new SoundParam("bgm_jingle_level_up_01", "bgm_jingle_level_up_01")
			},
			{
				eJINGLE.JINGLE_WIN_01,
				new SoundParam("bgm_jingle_win_01", "bgm_jingle_win_01")
			},
			{
				eJINGLE.JINGLE_WIN_01_LOOP,
				new SoundParam("bgm_jingle_win_01_loop", "bgm_jingle_win_01_loop")
			}
		};
		private static readonly Dictionary<eSEPartial, SoundParam> SE_PARAM_DICT = new Dictionary<eSEPartial, SoundParam>(new SEDictComparer())
		{
			{
				eSEPartial.SYS_BUTTON_LARGE,
				new SoundParam("preinstall", "se_sys_button_large")
			},
			{
				eSEPartial.SYS_CANCEL,
				new SoundParam("preinstall", "se_sys_cancel")
			},
			{
				eSEPartial.SYS_SWIPE,
				new SoundParam("preinstall", "se_sys_swipe")
			},
			{
				eSEPartial.SYS_TITLE_START,
				new SoundParam("preinstall", "se_sys_title_start")
			},
			{
				eSEPartial.SYS_WINDOW_APPEAR,
				new SoundParam("preinstall", "se_sys_window_appear")
			},
			{
				eSEPartial.SYS_BUTTON_SMALL,
				new SoundParam("preinstall", "se_sys_button_small")
			},
			{
				eSEPartial.SYS_CANCEL_SMALL,
				new SoundParam("preinstall", "se_sys_cancel_small")
			},
			{
				eSEPartial.VO_TITLE_1058_001,
				new SoundParam("preinstall", "vo_title_1058_001")
			},
			{
				eSEPartial.SYS_BUTTON,
				new SoundParam("preinstall", "se_sys_button")
			},
			{
				eSEPartial.FX_SKILL_HEAL_DRAIN,
				new SoundParam("se_battle", "fx_skill_heal_drain")
			},
			{
				eSEPartial.FX_SKILL_HEAL_ENERGY,
				new SoundParam("se_battle", "fx_skill_heal_energy")
			},
			{
				eSEPartial.BTL_DAMAGE,
				new SoundParam("se_battle", "se_btl_damage")
			},
			{
				eSEPartial.FX_SKILL_HEAL2,
				new SoundParam("se_battle", "fx_skill_heal2")
			},
			{
				eSEPartial.HIT_DARK,
				new SoundParam("se_battle", "hit_dark")
			},
			{
				eSEPartial.HIT_FIRE,
				new SoundParam("se_battle", "hit_fire")
			},
			{
				eSEPartial.HIT_LIGHT,
				new SoundParam("se_battle", "hit_light")
			},
			{
				eSEPartial.BTL_BUTTON_SKILL,
				new SoundParam("se_battle", "se_btl_button_skill")
			},
			{
				eSEPartial.HIT_NON,
				new SoundParam("se_battle", "hit_non")
			},
			{
				eSEPartial.HIT_WATER,
				new SoundParam("se_battle", "hit_water")
			},
			{
				eSEPartial.HIT_WIND,
				new SoundParam("se_battle", "hit_wind")
			},
			{
				eSEPartial.FX_STATE_SILENCE,
				new SoundParam("se_battle", "fx_state_silence")
			},
			{
				eSEPartial.FX_HIT_SLEEP,
				new SoundParam("se_battle", "fx_hit_sleep")
			},
			{
				eSEPartial.FX_STATE_BARRIER,
				new SoundParam("se_battle", "fx_state_barrier")
			},
			{
				eSEPartial.FX_STATE_BARRIER_PHYSICS,
				new SoundParam("se_battle", "fx_state_barrier_physics")
			},
			{
				eSEPartial.FX_STATE_BONDAGE,
				new SoundParam("se_battle", "fx_state_bondage")
			},
			{
				eSEPartial.BTL_BOSS_APPEAR,
				new SoundParam("se_battle", "se_btl_boss_appear")
			},
			{
				eSEPartial.BTL_HIT_ARROW_1,
				new SoundParam("se_battle", "se_btl_hit_arrow_1")
			},
			{
				eSEPartial.DIE,
				new SoundParam("se_battle", "die")
			},
			{
				eSEPartial.BTL_HIT_AX_1,
				new SoundParam("se_battle", "se_btl_hit_ax_1")
			},
			{
				eSEPartial.BTL_HIT_KNUCKLE_1,
				new SoundParam("se_battle", "se_btl_hit_knuckle_1")
			},
			{
				eSEPartial.BTL_FIREARM_ARROW_1,
				new SoundParam("se_battle", "se_btl_firearm_arrow_1")
			},
			{
				eSEPartial.BTL_HIT_LONGSWORD_1,
				new SoundParam("se_battle", "se_btl_hit_longsword_1")
			},
			{
				eSEPartial.BTL_HIT_SHORTSWORD_1,
				new SoundParam("se_battle", "se_btl_hit_shortsword_1")
			},
			{
				eSEPartial.BTL_HIT_SPEAR_1,
				new SoundParam("se_battle", "se_btl_hit_spear_1")
			},
			{
				eSEPartial.BTL_HIT_SWORD_1,
				new SoundParam("se_battle", "se_btl_hit_sword_1")
			},
			{
				eSEPartial.BTL_HIT_WAND_1,
				new SoundParam("se_battle", "se_btl_hit_wand_1")
			},
			{
				eSEPartial.BTL_HIT_BITE_1,
				new SoundParam("se_battle", "se_btl_hit_bite_1")
			},
			{
				eSEPartial.BTL_HIT_COMMON_1,
				new SoundParam("se_battle", "se_btl_hit_common_1")
			},
			{
				eSEPartial.BTL_HIT_COMMON_2,
				new SoundParam("se_battle", "se_btl_hit_common_2")
			},
			{
				eSEPartial.BTL_HIT_EXPLO_1,
				new SoundParam("se_battle", "se_btl_hit_explo_1")
			},
			{
				eSEPartial.BTL_HIT_HAMMER_1,
				new SoundParam("se_battle", "se_btl_hit_hammer_1")
			},
			{
				eSEPartial.BTL_HIT_SCRATCH_2,
				new SoundParam("se_battle", "se_btl_hit_scratch_2")
			},
			{
				eSEPartial.BTL_HIT_SCRATCH_1,
				new SoundParam("se_battle", "se_btl_hit_scratch_1")
			},
			{
				eSEPartial.FX_STATE_CURSE,
				new SoundParam("se_battle", "fx_state_curse")
			},
			{
				eSEPartial.FX_STATE_DARKNESS,
				new SoundParam("se_battle", "fx_state_darkness")
			},
			{
				eSEPartial.FX_STATE_DECOY,
				new SoundParam("se_battle", "fx_state_decoy")
			},
			{
				eSEPartial.FX_STATE_FASCINATION,
				new SoundParam("se_battle", "fx_state_fascination")
			},
			{
				eSEPartial.FX_STATE_FROZEN,
				new SoundParam("se_battle", "fx_state_frozen")
			},
			{
				eSEPartial.FX_STATE_STUN,
				new SoundParam("se_battle", "fx_state_stun")
			},
			{
				eSEPartial.FX_STATE_PARALYSIS,
				new SoundParam("se_battle", "fx_state_paralysis")
			},
			{
				eSEPartial.FX_STATE_POISON,
				new SoundParam("se_battle", "fx_state_poison")
			},
			{
				eSEPartial.FX_STATE_SLOW,
				new SoundParam("se_battle", "fx_state_slow")
			},
			{
				eSEPartial.FIREBALL_BLUE_BODY,
				new SoundParam("se_battle", "fireball_blue_body")
			},
			{
				eSEPartial.BUFF1_BLUE,
				new SoundParam("se_battle", "buff1_blue")
			},
			{
				eSEPartial.FX_BUFF,
				new SoundParam("se_battle", "fx_buff")
			},
			{
				eSEPartial.FX_DEBUFF,
				new SoundParam("se_battle", "fx_debuff")
			},
			{
				eSEPartial.BTL_DIE_FADEOUT,
				new SoundParam("se_battle", "se_btl_die_fadeout")
			},
			{
				eSEPartial.BUFF1_GREEN,
				new SoundParam("se_battle", "buff1_green")
			},
			{
				eSEPartial.BUFF1_PURPLE,
				new SoundParam("se_battle", "buff1_purple")
			},
			{
				eSEPartial.BUFF1_RED,
				new SoundParam("se_battle", "buff1_red")
			},
			{
				eSEPartial.BUFF1_YELLOW,
				new SoundParam("se_battle", "buff1_yellow")
			},
			{
				eSEPartial.BTL_BATTLE_START,
				new SoundParam("se_battle", "se_btl_battle_start")
			},
			{
				eSEPartial.FIREBALL_PINK_BODY,
				new SoundParam("se_battle", "fireball_pink_body")
			},
			{
				eSEPartial.BTL_DROP_BOX,
				new SoundParam("se_battle", "se_btl_drop_box")
			},
			{
				eSEPartial.BTL_DROP_MANA,
				new SoundParam("se_battle", "se_btl_drop_mana")
			},
			{
				eSEPartial.BTL_GET_BOX_MANA,
				new SoundParam("se_battle", "se_btl_get_box_mana")
			},
			{
				eSEPartial.FX_SKILLEXE1_DARK,
				new SoundParam("se_battle", "fx_skillexe1_dark")
			},
			{
				eSEPartial.FX_SKILLEXE1_FIRE,
				new SoundParam("se_battle", "fx_skillexe1_fire")
			},
			{
				eSEPartial.FX_SKILLEXE1_LIGHT,
				new SoundParam("se_battle", "fx_skillexe1_light")
			},
			{
				eSEPartial.FX_SKILLEXE1_PINK,
				new SoundParam("se_battle", "fx_skillexe1_pink")
			},
			{
				eSEPartial.FX_SKILLEXE1_WATER,
				new SoundParam("se_battle", "fx_skillexe1_water")
			},
			{
				eSEPartial.FX_SKILLEXE1_WIND,
				new SoundParam("se_battle", "fx_skillexe1_wind")
			},
			{
				eSEPartial.BUFF1_PINK,
				new SoundParam("se_battle", "buff1_pink")
			},
			{
				eSEPartial.FIREBALL_GREEN_BODY,
				new SoundParam("se_battle", "fireball_green_body")
			},
			{
				eSEPartial.FIREBALL_PURPLE_BODY,
				new SoundParam("se_battle", "fireball_purple_body")
			},
			{
				eSEPartial.FIREBALL_RED_BODY,
				new SoundParam("se_battle", "fireball_red_body")
			},
			{
				eSEPartial.FIREBALL_YELLOW_BODY,
				new SoundParam("se_battle", "fireball_yellow_body")
			},
			{
				eSEPartial.THRUST_RED,
				new SoundParam("se_battle", "thrust_red")
			},
			{
				eSEPartial.THRUST__PINK,
				new SoundParam("se_battle", "thrust__pink")
			},
			{
				eSEPartial.THRUST__PURPLE,
				new SoundParam("se_battle", "thrust__purple")
			},
			{
				eSEPartial.THRUST__YELLOW,
				new SoundParam("se_battle", "thrust__yellow")
			},
			{
				eSEPartial.THRUST__BLUE,
				new SoundParam("se_battle", "thrust__blue")
			},
			{
				eSEPartial.THRUST__GREEN,
				new SoundParam("se_battle", "thrust__green")
			},
			{
				eSEPartial.FX_ATTACK_FIELD_MAGICCIRCLE_0001,
				new SoundParam("se_battle", "fx_attack_field_magiccircle_0001")
			},
			{
				eSEPartial.FX_CONTINUOUS_ATTACK_NEARBY,
				new SoundParam("se_battle", "fx_continuous_attack_nearby")
			},
			{
				eSEPartial.FX_SKILLEXE1_DARK_SHORT,
				new SoundParam("se_battle", "fx_skillexe1_dark_short")
			},
			{
				eSEPartial.FX_SKILLEXE1_FIRE_SHORT,
				new SoundParam("se_battle", "fx_skillexe1_fire_short")
			},
			{
				eSEPartial.FX_SKILLEXE1_LIGHT_SHORT,
				new SoundParam("se_battle", "fx_skillexe1_light_short")
			},
			{
				eSEPartial.FX_SKILLEXE1_PINK_SHORT,
				new SoundParam("se_battle", "fx_skillexe1_pink_short")
			},
			{
				eSEPartial.FX_SKILLEXE1_WATER_SHORT,
				new SoundParam("se_battle", "fx_skillexe1_water_short")
			},
			{
				eSEPartial.FX_SKILLEXE1_WIND_SHORT,
				new SoundParam("se_battle", "fx_skillexe1_wind_short")
			},
			{
				eSEPartial.FX_SKILLEXE1_DARK_SHORT_NONCIRCLE,
				new SoundParam("se_battle", "fx_skillexe1_dark_short_noncircle")
			},
			{
				eSEPartial.FX_SKILLEXE1_FIRE_SHORT_NONCIRCLE,
				new SoundParam("se_battle", "fx_skillexe1_fire_short_noncircle")
			},
			{
				eSEPartial.FX_SKILLEXE1_LIGHT_SHORT_NONCIRCLE,
				new SoundParam("se_battle", "fx_skillexe1_light_short_noncircle")
			},
			{
				eSEPartial.FX_SKILLEXE1_PINK_SHORT_NONCIRCLE,
				new SoundParam("se_battle", "fx_skillexe1_pink_short_noncircle")
			},
			{
				eSEPartial.FX_SKILLEXE1_WATER_SHORT_NONCIRCLE,
				new SoundParam("se_battle", "fx_skillexe1_water_short_noncircle")
			},
			{
				eSEPartial.FX_SKILLEXE1_WIND_SHORT_NONCIRCLE,
				new SoundParam("se_battle", "fx_skillexe1_wind_short_noncircle")
			},
			{
				eSEPartial.FX_SKILLEXE1_DARK_SHORT_0,
				new SoundParam("se_battle", "fx_skillexe1_dark_short_0")
			},
			{
				eSEPartial.BTL_RESULT_GAGE_UP,
				new SoundParam("se_battle", "se_btl_result_gage_up")
			},
			{
				eSEPartial.BTL_RESULT_LOSE,
				new SoundParam("se_battle", "se_btl_result_lose")
			},
			{
				eSEPartial.BTL_RESULT_LOSE_SMALL,
				new SoundParam("se_battle", "se_btl_result_lose_small")
			},
			{
				eSEPartial.BTL_RESULT_STAR,
				new SoundParam("se_battle", "se_btl_result_star")
			},
			{
				eSEPartial.BTL_RESULT_TIMEUP,
				new SoundParam("se_battle", "se_btl_result_timeup")
			},
			{
				eSEPartial.BTL_RESULT_TRANSITION,
				new SoundParam("se_battle", "se_btl_result_transition")
			},
			{
				eSEPartial.BTL_RESULT_VICTORY,
				new SoundParam("se_battle", "se_btl_result_victory")
			},
			{
				eSEPartial.BTL_RESULT_WIN,
				new SoundParam("se_battle", "se_btl_result_win")
			},
			{
				eSEPartial.BTL_RESULT_WIN_SMALL,
				new SoundParam("se_battle", "se_btl_result_win_small")
			},
			{
				eSEPartial.BTL_SUPPORT_GAUGE,
				new SoundParam("se_battle", "se_btl_support_gauge")
			},
			{
				eSEPartial.FX_STATE_SEAL,
				new SoundParam("se_battle", "fx_state_seal")
			},
			{
				eSEPartial.BTL_NEXTWAVE,
				new SoundParam("se_battle", "se_btl_nextwave")
			},
			{
				eSEPartial.BTL_RESULT_FAILED,
				new SoundParam("se_battle", "se_btl_result_failed")
			},
			{
				eSEPartial.BTL_RESULT_DEFEAT,
				new SoundParam("se_battle", "se_btl_result_defeat")
			},
			{
				eSEPartial.BTL_CLAN_RESULT,
				new SoundParam("se_battle", "se_btl_clan_result")
			},
			{
				eSEPartial.BTL_CLAN_CP_UP,
				new SoundParam("se_battle", "se_btl_clan_cp_up")
			},
			{
				eSEPartial.BTL_CLAN_GAGE_UP,
				new SoundParam("se_battle", "se_btl_clan_gage_up")
			},
			{
				eSEPartial.BTL_TOWER_TIMEUP,
				new SoundParam("se_battle", "se_btl_tower_timeup")
			},
			{
				eSEPartial.BTL_BRONZE_BOX_01,
				new SoundParam("se_battle", "se_btl_bronze_box_01")
			},
			{
				eSEPartial.BTL_GOLD_BOX_01,
				new SoundParam("se_battle", "se_btl_gold_box_01")
			},
			{
				eSEPartial.BTL_GOLD_BOX_02,
				new SoundParam("se_battle", "se_btl_gold_box_02")
			},
			{
				eSEPartial.BTL_TOWER_TIMEUP_COUNT,
				new SoundParam("se_battle", "se_btl_tower_timeup_count")
			},
			{
				eSEPartial.BTL_SILVER_BOX_01,
				new SoundParam("se_battle", "se_btl_silver_box_01")
			},
			{
				eSEPartial.BTL_SILVER_BOX_02,
				new SoundParam("se_battle", "se_btl_silver_box_02")
			},
			{
				eSEPartial.BTL_NORMAL_BOX_01,
				new SoundParam("se_battle", "se_btl_normal_box_01")
			},
			{
				eSEPartial.BTL_HALLOWEEN_BATTLE_START,
				new SoundParam("se_battle", "se_btl_halloween_battle_start")
			},
			{
				eSEPartial.BTL_HALLOWEEN_RESULT_WIN,
				new SoundParam("se_battle", "se_btl_halloween_result_win")
			},
			{
				eSEPartial.BTL_HALLOWEEN_RESULT_LOSE,
				new SoundParam("se_battle", "se_btl_halloween_result_lose")
			},
			{
				eSEPartial.BTL_NEXTMODE_01,
				new SoundParam("se_battle", "se_btl_nextmode_01")
			},
			{
				eSEPartial.SP_SHADOW_SPAWN_0600,
				new SoundParam("se_battle", "sp_shadow_spawn_0600")
			},
			{
				eSEPartial.FX_STATE_CONFUSION,
				new SoundParam("se_battle", "fx_state_confusion")
			},
			{
				eSEPartial.FX_STATE_VENOM,
				new SoundParam("se_battle", "fx_state_venom")
			},
			{
				eSEPartial.MULTI_TARGETS_BREAK,
				new SoundParam("se_battle", "multi_targets_break")
			},
			{
				eSEPartial.MULTI_TARGETS_REVIVAL,
				new SoundParam("se_battle", "multi_targets_revival")
			},
			{
				eSEPartial.MULTI_TARGETS_START,
				new SoundParam("se_battle", "multi_targets_start")
			},
			{
				eSEPartial.BTL_CLAN_RESULT_REWARD,
				new SoundParam("se_battle", "se_btl_clan_result_reward")
			},
			{
				eSEPartial.BTL_CLAN_EXBATTLE_COMPLETE,
				new SoundParam("se_battle", "se_btl_clan_exbattle_complete")
			},
			{
				eSEPartial.BTL_CLAN_EXBATTLE_START,
				new SoundParam("se_battle", "se_btl_clan_exbattle_start")
			},
			{
				eSEPartial.BTL_CLAN_EXBATTLE_AWAKE_FINISH,
				new SoundParam("se_battle", "se_btl_clan_exbattle_awake_finish")
			},
			{
				eSEPartial.BTL_CLAN_EXBATTLE_AWAKE_LOOP,
				new SoundParam("se_battle", "se_btl_clan_exbattle_awake_loop")
			},
			{
				eSEPartial.BTL_MMAP_RESULT,
				new SoundParam("se_battle", "se_btl_mmap_result")
			},
			{
				eSEPartial.BTL_MMAP_START,
				new SoundParam("se_battle", "se_btl_mmap_start")
			},
			{
				eSEPartial.BTL_MMAP_WIN,
				new SoundParam("se_battle", "se_btl_mmap_win")
			},
			{
				eSEPartial.BTL_ARENA_SKIP,
				new SoundParam("se_battle", "se_btl_arena_skip")
			},
			{
				eSEPartial.BTLE_HIT_DUALSWORD_1,
				new SoundParam("se_battle", "se_btle_hit_dualsword_1")
			},
			{
				eSEPartial.BTL_HIT_DUALKNUCKLE_1,
				new SoundParam("se_battle", "se_btl_hit_dualknuckle_1")
			},
			{
				eSEPartial.FX_SKILL_HEAL_ENERGY_DRAIN,
				new SoundParam("se_battle", "fx_skill_heal_energy_drain")
			},
			{
				eSEPartial.SKILL_HEAL3,
				new SoundParam("se_battle", "skill_heal3")
			},
			{
				eSEPartial.SYS_MEMORYPIECE_RARITY_01,
				new SoundParam("se_common", "se_sys_memorypiece_rarity_01")
			},
			{
				eSEPartial.SYS_MEMORYPIECE_RARITY_02,
				new SoundParam("se_common", "se_sys_memorypiece_rarity_02")
			},
			{
				eSEPartial.SYS_CLOSE_MENU,
				new SoundParam("se_common", "se_sys_close_menu")
			},
			{
				eSEPartial.SYS_MEMORYPIECE_CHARAGET_01,
				new SoundParam("se_common", "se_sys_memorypiece_charaget_01")
			},
			{
				eSEPartial.SYS_MEMORYPIECE_RARITY_03_01,
				new SoundParam("se_common", "se_sys_memorypiece_rarity_03_01")
			},
			{
				eSEPartial.SYS_MEMORYPIECE_CHARAGET_02,
				new SoundParam("se_common", "se_sys_memorypiece_charaget_02")
			},
			{
				eSEPartial.SYS_WORLDMAP_CLOSE,
				new SoundParam("se_common", "se_sys_worldmap_close")
			},
			{
				eSEPartial.SYS_WORLDMAP_OPEN,
				new SoundParam("se_common", "se_sys_worldmap_open")
			},
			{
				eSEPartial.SYS_WORLDMAP_STAGE_ICON,
				new SoundParam("se_common", "se_sys_worldmap_stage_icon")
			},
			{
				eSEPartial.SYS_WORLDMAP_WHOOSH,
				new SoundParam("se_common", "se_sys_worldmap_whoosh")
			},
			{
				eSEPartial.SYS_MEMORYPIECE_CHARAGET_03,
				new SoundParam("se_common", "se_sys_memorypiece_charaget_03")
			},
			{
				eSEPartial.SYS_OPEN_MENU,
				new SoundParam("se_common", "se_sys_open_menu")
			},
			{
				eSEPartial.SYS_MEMORYPIECE_CHARAGET_04,
				new SoundParam("se_common", "se_sys_memorypiece_charaget_04")
			},
			{
				eSEPartial.SYS_MESSAGE_APPEAR_01,
				new SoundParam("se_common", "se_sys_message_appear_01")
			},
			{
				eSEPartial.SYS_RECIEVE_GIFT,
				new SoundParam("se_common", "se_sys_recieve_gift")
			},
			{
				eSEPartial.SYS_DEAR_HEART_01,
				new SoundParam("se_common", "se_sys_dear_heart_01")
			},
			{
				eSEPartial.SYS_MEMORYPIECE_RARITY_04,
				new SoundParam("se_common", "se_sys_memorypiece_rarity_04")
			},
			{
				eSEPartial.SYS_TITLE_01,
				new SoundParam("se_common", "se_sys_title_01")
			},
			{
				eSEPartial.SYS_DIE,
				new SoundParam("se_common", "se_sys_die")
			},
			{
				eSEPartial.SYS_JOIN_01,
				new SoundParam("se_common", "se_sys_join_01")
			},
			{
				eSEPartial.SYS_CHAR_ICON_LANDING,
				new SoundParam("se_common", "se_sys_char_icon_landing")
			},
			{
				eSEPartial.SYS_WINDOW_SLIDE_1,
				new SoundParam("se_common", "se_sys_window_slide_1")
			},
			{
				eSEPartial.SYS_LOGIN_ANNOUNCE_ACHIVEMENT,
				new SoundParam("se_common", "se_sys_login_announce_achivement")
			},
			{
				eSEPartial.SYS_LOGIN_COMPLETE,
				new SoundParam("se_common", "se_sys_login_complete")
			},
			{
				eSEPartial.SYS_LOGIN_GET_PRIZE,
				new SoundParam("se_common", "se_sys_login_get_prize")
			},
			{
				eSEPartial.SYS_LOGIN_NEXT_PRIZE,
				new SoundParam("se_common", "se_sys_login_next_prize")
			},
			{
				eSEPartial.SYS_LOGIN_PAPER_MOVE_01,
				new SoundParam("se_common", "se_sys_login_paper_move_01")
			},
			{
				eSEPartial.SYS_LOGIN_PAPER_MOVE_02,
				new SoundParam("se_common", "se_sys_login_paper_move_02")
			},
			{
				eSEPartial.SYS_LOGIN_PAPER_MOVE_03,
				new SoundParam("se_common", "se_sys_login_paper_move_03")
			},
			{
				eSEPartial.SYS_LOGIN_STAMP,
				new SoundParam("se_common", "se_sys_login_stamp")
			},
			{
				eSEPartial.SYS_LOGIN_WRITING,
				new SoundParam("se_common", "se_sys_login_writing")
			},
			{
				eSEPartial.SYS_DEAR_HEART_02,
				new SoundParam("se_common", "se_sys_dear_heart_02")
			},
			{
				eSEPartial.SYS_DEAR_LEVEL_UP_01,
				new SoundParam("se_common", "se_sys_dear_level_up_01")
			},
			{
				eSEPartial.SYS_DEAR_LEVEL_UP_02,
				new SoundParam("se_common", "se_sys_dear_level_up_02")
			},
			{
				eSEPartial.SYS_DEAR_WHOOSH,
				new SoundParam("se_common", "se_sys_dear_whoosh")
			},
			{
				eSEPartial.SYS_MESSAGE_WHOOSH_01,
				new SoundParam("se_common", "se_sys_message_whoosh_01")
			},
			{
				eSEPartial.SYS_AUTO_UNIT_LEVEL_UP,
				new SoundParam("se_common", "se_sys_auto_unit_level_up")
			},
			{
				eSEPartial.SYS_BATTLE_READY,
				new SoundParam("se_common", "se_sys_battle_ready")
			},
			{
				eSEPartial.SYS_BATTLE_START,
				new SoundParam("se_common", "se_sys_battle_start")
			},
			{
				eSEPartial.SYS_AUTO_SKILL_LEVEL_UP,
				new SoundParam("se_common", "se_sys_auto_skill_level_up")
			},
			{
				eSEPartial.SYS_BUY_01,
				new SoundParam("se_common", "se_sys_buy_01")
			},
			{
				eSEPartial.SYS_CHAT_01,
				new SoundParam("se_common", "se_sys_chat_01")
			},
			{
				eSEPartial.SYS_RECIEVE_GIFT_02,
				new SoundParam("se_common", "se_sys_recieve_gift_02")
			},
			{
				eSEPartial.SYS_RECOVER_STAMINA,
				new SoundParam("se_common", "se_sys_recover_stamina")
			},
			{
				eSEPartial.SYS_LEAVE_01,
				new SoundParam("se_common", "se_sys_leave_01")
			},
			{
				eSEPartial.SYS_SHOP_OPEN,
				new SoundParam("se_common", "se_sys_shop_open")
			},
			{
				eSEPartial.SYS_DEAR_BIRD_01,
				new SoundParam("se_common", "se_sys_dear_bird_01")
			},
			{
				eSEPartial.SYS_NAME_PLATE_APPEAR,
				new SoundParam("se_common", "se_sys_name_plate_appear")
			},
			{
				eSEPartial.SYS_STAR_APPEAR,
				new SoundParam("se_common", "se_sys_star_appear")
			},
			{
				eSEPartial.SYS_MANABOX_OPEN_01,
				new SoundParam("se_common", "se_sys_manabox_open_01")
			},
			{
				eSEPartial.SYS_MESSAGE_WHOOSH_02,
				new SoundParam("se_common", "se_sys_message_whoosh_02")
			},
			{
				eSEPartial.SYS_WHOOSH_01,
				new SoundParam("se_common", "se_sys_whoosh_01")
			},
			{
				eSEPartial.SYS_CHAR_RANK_UP,
				new SoundParam("se_common", "se_sys_char_rank_up")
			},
			{
				eSEPartial.SYS_EQUIP,
				new SoundParam("se_common", "se_sys_equip")
			},
			{
				eSEPartial.SYS_EQUIPMENT_POWER_UP,
				new SoundParam("se_common", "se_sys_equipment_power_up")
			},
			{
				eSEPartial.SYS_MAKE_EQUIPMENT,
				new SoundParam("se_common", "se_sys_make_equipment")
			},
			{
				eSEPartial.SYS_POWER_UP_SKILL,
				new SoundParam("se_common", "se_sys_power_up_skill")
			},
			{
				eSEPartial.SYS_UNIT_LEVEL_UP_03,
				new SoundParam("se_common", "se_sys_unit_level_up_03")
			},
			{
				eSEPartial.SYS_UNIT_LEVEL_UP_01,
				new SoundParam("se_common", "se_sys_unit_level_up_01")
			},
			{
				eSEPartial.SYS_UNIT_LEVEL_UP_02,
				new SoundParam("se_common", "se_sys_unit_level_up_02")
			},
			{
				eSEPartial.SYS_DEAR_LEVEL_UP_02_02,
				new SoundParam("se_common", "se_sys_dear_level_up_02_02")
			},
			{
				eSEPartial.SYS_DEAR_LEVEL_UP_01_02,
				new SoundParam("se_common", "se_sys_dear_level_up_01_02")
			},
			{
				eSEPartial.SYS_AUTO_EQUIP,
				new SoundParam("se_common", "se_sys_auto_equip")
			},
			{
				eSEPartial.SYS_CLEAR_1,
				new SoundParam("se_common", "se_sys_clear_1")
			},
			{
				eSEPartial.SYS_HALLOWEEN_FIRECRACKER_01,
				new SoundParam("se_common", "se_sys_halloween_firecracker_01")
			},
			{
				eSEPartial.SYS_UNLOCK,
				new SoundParam("se_common", "se_sys_unlock")
			},
			{
				eSEPartial.SYS_ICON_APPEAR_01,
				new SoundParam("se_common", "se_sys_icon_appear_01")
			},
			{
				eSEPartial.SYS_SPECIAL_LOGO_EFFECT,
				new SoundParam("se_common", "se_sys_special_logo_effect")
			},
			{
				eSEPartial.SYS_HALLOWEEN_FIRECRACKER_02,
				new SoundParam("se_common", "se_sys_halloween_firecracker_02")
			},
			{
				eSEPartial.SYS_MAP_WARP_1,
				new SoundParam("se_common", "se_sys_map_warp_1")
			},
			{
				eSEPartial.SYS_MAP_WARP_2,
				new SoundParam("se_common", "se_sys_map_warp_2")
			},
			{
				eSEPartial.SYS_WORLDMAP_WHOOSH_02,
				new SoundParam("se_common", "se_sys_worldmap_whoosh_02")
			},
			{
				eSEPartial.SYS_SPECIAL_LOGO_EFFECT_2,
				new SoundParam("se_common", "se_sys_special_logo_effect_2")
			},
			{
				eSEPartial.SYS_MEMORYPIECE_RARITY_05,
				new SoundParam("se_common", "se_sys_memorypiece_rarity_05")
			},
			{
				eSEPartial.SYS_ICON_6STAR,
				new SoundParam("se_common", "se_sys_icon_6star")
			},
			{
				eSEPartial.SYS_WINDOW_CLOSE,
				new SoundParam("se_common", "se_sys_window_close")
			},
			{
				eSEPartial.SYS_ITEMBOX_GET,
				new SoundParam("se_common", "se_sys_itembox_get")
			},
			{
				eSEPartial.SYS_COMMON_COMPLETED_2,
				new SoundParam("se_common", "se_sys_common_completed_2")
			},
			{
				eSEPartial.SYS_NEW_RECORD_01,
				new SoundParam("se_common", "se_sys_new_record_01")
			},
			{
				eSEPartial.SYS_TRANSITION_WHIRLPOOL,
				new SoundParam("se_common", "se_sys_transition_whirlpool")
			},
			{
				eSEPartial.SYS_RARITY_3STAR_01,
				new SoundParam("se_common", "se_sys_rarity_3star_01")
			},
			{
				eSEPartial.SYS_RARITY_3STAR_02,
				new SoundParam("se_common", "se_sys_rarity_3star_02")
			},
			{
				eSEPartial.SYS_RARITY_3STAR_03,
				new SoundParam("se_common", "se_sys_rarity_3star_03")
			},
			{
				eSEPartial.SYS_ICON_CHANGE_01,
				new SoundParam("se_common", "se_sys_icon_change_01")
			},
			{
				eSEPartial.SYS_ICON_APPEAR_02,
				new SoundParam("se_common", "se_sys_icon_appear_02")
			},
			{
				eSEPartial.SYS_EQUIP_02,
				new SoundParam("se_common", "se_sys_equip_02")
			},
			{
				eSEPartial.SYS_EQUIP_LEVEL_LIMIT,
				new SoundParam("se_common", "se_sys_equip_level_limit")
			},
			{
				eSEPartial.SYS_EQUIPMENT_POWER_UP_02,
				new SoundParam("se_common", "se_sys_equipment_power_up_02")
			},
			{
				eSEPartial.SYS_MAKE_EQUIPMENT_02,
				new SoundParam("se_common", "se_sys_make_equipment_02")
			},
			{
				eSEPartial.SYS_EXHIBITION_LOGO,
				new SoundParam("se_common", "se_sys_exhibition_logo")
			},
			{
				eSEPartial.SYS_EQUIP_03,
				new SoundParam("se_common", "se_sys_equip_03")
			},
			{
				eSEPartial.SYS_EQUIPMENT_POWER_UP_03,
				new SoundParam("se_common", "se_sys_equipment_power_up_03")
			},
			{
				eSEPartial.SYS_STAR_APPEAR02,
				new SoundParam("se_common", "se_sys_star_appear02")
			},
			{
				eSEPartial.SYS_SWIPE_02,
				new SoundParam("se_common", "se_sys_swipe_02")
			},
			{
				eSEPartial.SYS_COMMON_COMPLETED_3,
				new SoundParam("se_common", "se_sys_common_completed_3")
			},
			{
				eSEPartial.SYS_ICON_CHANGE_02,
				new SoundParam("se_common", "se_sys_icon_change_02")
			},
			{
				eSEPartial.SYS_BUTTON_6STAR,
				new SoundParam("se_common", "se_sys_button_6star")
			},
			{
				eSEPartial.SYS_STAR_CHANGE,
				new SoundParam("se_common", "se_sys_star_change")
			},
			{
				eSEPartial.SYS_CHAR_RANK_UP_02,
				new SoundParam("se_common", "se_sys_char_rank_up_02")
			},
			{
				eSEPartial.SYS_TIES_RANK_UP,
				new SoundParam("se_common", "se_sys_ties_rank_up")
			},
			{
				eSEPartial.SYS_LOGIN_BIRTHDAY_01,
				new SoundParam("se_common", "se_sys_login_birthday_01")
			},
			{
				eSEPartial.SYS_LOGIN_BIRTHDAY_02,
				new SoundParam("se_common", "se_sys_login_birthday_02")
			},
			{
				eSEPartial.SYS_LOGIN_BIRTHDAY_CRACKER,
				new SoundParam("se_common", "se_sys_login_birthday_cracker")
			},
			{
				eSEPartial.SYS_LOGIN_BIRTHDAY_NEW_01,
				new SoundParam("se_common", "se_sys_login_birthday_new_01")
			},
			{
				eSEPartial.SYS_LOGIN_BIRTHDAY_BLOW_CANDLE,
				new SoundParam("se_common", "se_sys_login_birthday_blow_candle")
			},
			{
				eSEPartial.PRIZEGACHA_1ST_GRADE,
				new SoundParam("se_gacha", "se_prizegacha_1st_grade")
			},
			{
				eSEPartial.PRIZEGACHA_2ND_GRADE,
				new SoundParam("se_gacha", "se_prizegacha_2nd_grade")
			},
			{
				eSEPartial.PRIZEGACHA_CARDOPEN,
				new SoundParam("se_gacha", "se_prizegacha_cardopen")
			},
			{
				eSEPartial.PRIZEGACHA_GOLDCARD,
				new SoundParam("se_gacha", "se_prizegacha_goldcard")
			},
			{
				eSEPartial.PRIZEGACHA_3RD_GRADE,
				new SoundParam("se_gacha", "se_prizegacha_3rd_grade")
			},
			{
				eSEPartial.GACHA_RAINBOW_OPEN_01,
				new SoundParam("se_gacha", "se_gacha_rainbow_open_01")
			},
			{
				eSEPartial.PRIZEGACHA_CIRCLE,
				new SoundParam("se_gacha", "se_prizegacha_circle")
			},
			{
				eSEPartial.GACHA_BONUS_01,
				new SoundParam("se_gacha", "se_gacha_bonus_01")
			},
			{
				eSEPartial.GACHA_PAPER_MOVE_01,
				new SoundParam("se_gacha", "se_gacha_paper_move_01")
			},
			{
				eSEPartial.GACHA_PAPER_MOVE_02,
				new SoundParam("se_gacha", "se_gacha_paper_move_02")
			},
			{
				eSEPartial.GACHA_TRANSITION,
				new SoundParam("se_gacha", "se_gacha_transition")
			},
			{
				eSEPartial.GAHCA_NORMAL_APPEAR,
				new SoundParam("se_gacha", "se_gahca_normal_appear")
			},
			{
				eSEPartial.GAHCA_NORMAL_OPEN,
				new SoundParam("se_gacha", "se_gahca_normal_open")
			},
			{
				eSEPartial.GACHA_WHOOSH,
				new SoundParam("se_gacha", "se_gacha_whoosh")
			},
			{
				eSEPartial.GACHA_PAPER_PASTING_SILVER,
				new SoundParam("se_gacha", "se_gacha_paper_pasting_silver")
			},
			{
				eSEPartial.GAHCA_NORMAL_OPEN_02,
				new SoundParam("se_gacha", "se_gahca_normal_open_02")
			},
			{
				eSEPartial.GAHCA_NORMAL_SHAKE,
				new SoundParam("se_gacha", "se_gahca_normal_shake")
			},
			{
				eSEPartial.GACHA_PAPER_PASTING_RAINBOW,
				new SoundParam("se_gacha", "se_gacha_paper_pasting_rainbow")
			},
			{
				eSEPartial.GACHA_PAPER_PASTING_GOLD,
				new SoundParam("se_gacha", "se_gacha_paper_pasting_gold")
			},
			{
				eSEPartial.GACHA_SKIP,
				new SoundParam("se_gacha", "se_gacha_skip")
			},
			{
				eSEPartial.ROOM_OBJECT_LEVEL_UP,
				new SoundParam("se_room", "se_room_object_level_up")
			},
			{
				eSEPartial.ROOM_PUT_OBJECT,
				new SoundParam("se_room", "se_room_put_object")
			},
			{
				eSEPartial.ROOM_SELECT_OBJECT,
				new SoundParam("se_room", "se_room_select_object")
			},
			{
				eSEPartial.ROOM_MOVE_OBJECT,
				new SoundParam("se_room", "se_room_move_object")
			},
			{
				eSEPartial.ROOM_SEND_GIFT,
				new SoundParam("se_room", "se_room_send_gift")
			},
			{
				eSEPartial.ROOM_LIKE,
				new SoundParam("se_room", "se_room_like")
			},
			{
				eSEPartial.TOWER_EXCLEAR,
				new SoundParam("se_tower", "se_tower_exclear")
			},
			{
				eSEPartial.TOWER_EXWINDOW,
				new SoundParam("se_tower", "se_tower_exwindow")
			},
			{
				eSEPartial.TOWER_FLOOR_02,
				new SoundParam("se_tower", "se_tower_floor_02")
			},
			{
				eSEPartial.TOWER_REACH_01,
				new SoundParam("se_tower", "se_tower_reach_01")
			},
			{
				eSEPartial.TOWER_REACH_02,
				new SoundParam("se_tower", "se_tower_reach_02")
			},
			{
				eSEPartial.TOWER_FLOOR_01,
				new SoundParam("se_tower", "se_tower_floor_01")
			},
			{
				eSEPartial.TOWER_REACHTOP,
				new SoundParam("se_tower", "se_tower_reachtop")
			},
			{
				eSEPartial.SYS_TOWER_EXWAVE,
				new SoundParam("se_tower", "se_sys_tower_exwave")
			},
			{
				eSEPartial.TOWER_EXRELEASE,
				new SoundParam("se_tower", "se_tower_exrelease")
			},
			{
				eSEPartial.TOWER_RELEASE_01,
				new SoundParam("se_tower", "se_tower_release_01")
			}
		};

		public SoundManager()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.SoundManager::.ctor()", "body migration pending");
		}

		[global::OpenPCR.Migration("9049c00d-7c95-430f-98d0-d032eb7b0576")]
		public AudioPlayback PlayViewBGM(eBGM _type, float _fadeTime = 0f, int _channel = 0, bool _loop = true, float _offsetTime = 0f, bool _isSameBgmPlay = false, float _startTime = 0f, int _additionalVoicePriority = 0)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.SoundManager::PlayViewBGM(Elements.eBGM,System.Single,System.Int32,System.Boolean,System.Single,System.Boolean,System.Single,System.Int32):OpenPCR.Foundation.Audio.AudioPlayback: runtime capability is unavailable.");
		}

		[global::OpenPCR.Migration("b96a974e-1750-4dc0-87c9-7af60501cc2d")]
		public void StopFadeBgm(int _channel = 0, float _fadeTime = 0.5f, Action _callback = null)
		{
			cuteAudioManager.Stop(SoundGroup.Bgm, _channel, _fadeTime, _callback);
		}

		[global::OpenPCR.Migration("12d8be0b-2375-4b05-aae9-47c84f98216e")]
		public void PlayJingleCrossFade(eJINGLE _type, bool _loop = false, float _fadeTime = 0f)
		{
			SoundParam soundParam = searchSoundParam(_type);
			lastPlayedJingle = playJingle(soundParam.CueSheetName, soundParam.CueName, _loop, _fadeTime);
		}

		[global::OpenPCR.Migration("3261dd1d-ee45-4d47-8d2c-870b6efe3264")]
		public AudioPlayback PlaySe(eSE _type, bool _loop = false)
		{
			return global::OpenPCR.Presentation.Noop<AudioPlayback>();
		}

		[global::OpenPCR.Migration("ae5a6ce5-8871-4b21-8544-438269a28d3d")]
		public void DisableSoundListener()
		{
			cuteAudioManager.Set3dListenerPos(Vector3.zero);
			cuteAudioManager.Enable3dListener(isEnable: false);
		}

		private AudioPlayback playSe(string _cueSheet, string _cueName, bool _loop)
		{
			if (string.IsNullOrEmpty(_cueName))
			{
				return AudioPlayback.Error(SoundGroup.Se);
			}

			return cuteAudioManager.Play(SoundGroup.Se, _cueSheet, _cueName, _loop);
		}

		private AudioPlayback playJingle(string _cueSheet, string _cueName, bool _loop, float _fadeTime)
		{
			if (string.IsNullOrEmpty(_cueName))
			{
				return AudioPlayback.Error(SoundGroup.Bgm);
			}

			return cuteAudioManager.PlayCrossFade(SoundGroup.Bgm, _cueSheet, _cueName, _loop, _fadeTime);
		}

		private SoundParam getSoundParam(eSE _type)
		{
			eSEPartial value = eSEPartial.NONE;
			SoundParam value2 = null;
			if (PARAM_SEPARTIAL_TYPE_DICT.TryGetValue(_type, out value))
			{
				value2 = searchSoundParam(value);
			}
			else
			{
				SE_SOUND_PARAM_DICT.TryGetValue(_type, out value2);
			}

			return value2;
		}

		private static SoundParam searchSoundParam(eJINGLE _jingle)
		{
			return JINGLE_PARAM_DICT[_jingle];
		}

		private static SoundParam searchSoundParam(eSEPartial _se)
		{
			if ((uint)(_se - 287) <= 1u)
			{
				return null;
			}

			return SE_PARAM_DICT[_se];
		}
		protected override void onInit() => global::OpenPCR.MigrationPending.Throw("Elements.SoundManager.onInit()", "behavior migration pending");

		private class SEDictComparer : global::System.Collections.Generic.IEqualityComparer<eSEPartial>
		{
			public SEDictComparer()
			{
				throw new global::OpenPCR.MigrationPendingException("Elements.SoundManager\u002BSEDictComparer::.ctor()", "Behavior implementation is not available.");
			}

			public bool Equals(eSEPartial x, eSEPartial y) => x == y;
			public int GetHashCode(eSEPartial obj) => (int)obj;
		}

		[global::OpenPCR.Migration("35b63ee8-cb89-4d6a-a7c3-1eef2adadb81")]
		public AudioPlayback PlaySeFromName(string _cueSheet, string _cueName, bool _loop = false)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.SoundManager::PlaySeFromName(System.String,System.String,System.Boolean):OpenPCR.Foundation.Audio.AudioPlayback: runtime capability is unavailable.");
		}

		public AudioPlayback PlayVoice(int _id, eVoiceType _type, int _index = 0)
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.SoundManager::PlayVoice(System.Int32,Elements.SoundManager+eVoiceType,System.Int32):OpenPCR.Foundation.Audio.AudioPlayback", "voice selection RNG requires Native migration before hardware boundary");
		}

		public void StopSe(AudioPlayback _audioPlayback, float _fadeTime = 0f)
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.SoundManager::StopSe(OpenPCR.Foundation.Audio.AudioPlayback,System.Single):System.Void", "audio state and timing require Native migration before hardware boundary");
		}

	}
}
