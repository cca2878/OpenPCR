using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Elements
{
	public class BattleDefine
	{
		public const string BATTLE_LOGIC_VERSION = "4";
		public const int ARENA_BG_ID = 100001;
		public const int GRANDARENA_BG_ID = 100002;
		public const int COLOSSEUM_BG_ID = 100003;
		public const float MAIN_QUEST_ZOOM_POS = 4935f;
		public const float OTHER_QUEST_ZOOM_POS = 4780f;
		public const int LAST_SEAL_COUNT = 1;
		public const int LOGIC_FRAME_RATE = 60;
		public static readonly Dictionary<eBattleCategory, eBGM[]> BattleBgmDic;
		public const int INVALID_BLOCK_ID = -1;
		public static readonly List<eUnitRespawnPos> UnitRespawnPosTopToBottom;
		private static readonly List<eUnitRespawnPos> enemyRespawnPosList;
		public const int BACKGROUND_ORDER = -1100;
		public const int FOREGROUND_ORDER = 10900;
		public const int MIDDLEGROUND_ORDER = 680;
		public const int BACK_CHARA_ORDER = 350;
		public const int FRONT_CHARA_ORDER = 10850;
		public const int CHANGE_WAVE_FRAME_NUM = 100000;
		public const string BATTLE_PATH = "Assets/_ElementsResources/Resources/All/Battle";
		private static readonly Dictionary<eUnitRespawnPos, int> unitSortOrderDictionary;
		private const int RESULT_SORT_ORDER_OFFSET = 100;
		public const int STATUS_EFFECT_SORT_ORDER_OFFSET = 350;
		public const int EFFECT_UNIT_FRONT_SORT_ORDER_OFFSET = 300;
		public const int EFFECT_UNIT_BACK_SORT_ORDER_OFFSET = -300;
		public const int EFFECT_FRONT_SORT_ORDER = 11000;
		public const int EFFECT_BACK_SORT_ORDER = 690;
		public const int UI_SORT_ORDER = 11250;
		public const int BLACK_OUT_SORT_ORDER = 11500;
		public const int RESULT_UI_BACK_SORT_ORDER = 9000;
		public const int EFFECT_UNIT_FRONT_SORT_ORDER_OFFSET_FLATOUT = 200;
		public const int CRITICAL_RATE_MAX = 1;
		public const double CRITICAL_ATK_RATE = 2.0;
		public const int DEFAULT_CRITICAL_DAMAGE_RATE = 100;
		public const long MIN_CRITICAL_DAMAGE_RATE = 50L;
		public const int DEFAULT_RECEIVE_DAMAGE_ADDITIONAL_PERCENT = 0;
		public const int DEFAULT_DAMAGE_UP_ADDITIONAL_PERCENT = 0;
		public const int DEFAULT_TALENT_FORMATION_BONUS = 0;
		public const int DEFAULT_TALENT_ENHANCE = 0;
		public const double MIN_DAMAGE = 1.0;
		public const double DEFAULT_DAMAGE_RATE = 1.0;
		public const double SKILL_ENERGY_MAX = 1000.0;
		public const int TRANCATION_DIGIT = 1000;
		public const int ALONE_COUNT = 1;
		public const double UNIT_BASE_SPEED = 1.0;
		public const double UNIT_MIN_SPEED = 0.10000000149011612;
		private const float RESPAWN_POS_TOP_Y_1 = 40f;
		private const float RESPAWN_POS_BOTTOM_Y_1 = -186f;
		private const int LANE_COUNT_MINUS_ONE = 9;
		public const float BATTLE_POS_Y = 5000f;
		public const float BATTLE_BG_SIZE = 1.15f;
		public const float BATTLE_POS_Y_OTHER_THAN_QUEST = 5140f;
		public const float BATTLE_UNIT_SPACE_X = 200f;
		public const string SKILL_FX_FILENAME_PREFIX = "fxsk_";
		public const string SKILL_FX_FILENAME_PREFIX_SAMPLE = "fxsk_0000_CUT_";
		public const float QUEST_WIN_OFFSET = 280f;
		public const float GRAND_ARENA_WIN_OFFSET = 302f;
		public const float QUEST_WIN_POS_Y = -100f;
		public const int MODE_CHANGE_PREFIX = 1;
		public const double DEFAULT_SLIP_DAMAGE_RATE = 1.0;
		public const int MOVE_SPEED_MIN = 1;
		public const float BATTLE_START_POS = 560f;
		public const float BOSS_DEFAULT_DELTA_X = -34f;
		public const float ARENA_TIME_LIMIT = 90f;
		public const float MAX_BATTLE_TIME = 9999f;
		public const float LOSE_WAIT_TIME = 1.3f;
		public const float TIME_UP_WAIT_TIME = 0.5f;
		public const float BATTLE_START_WAIT_TIME = 0.5f;
		public const float BATTLE_START_ADV_WAIT_TIME = 2f;
		public const float MY_TURN_DISP_WAIT_TIME = 3f;
		public const float MY_TURN_DISP_FADE_OUT_TIME = 0.2f;
		public const float WIN_FLATOUT_WAIT_TIME = 0.5f;
		public const float LOSE_FLATOUT_WAIT_TIME = 0.5f;
		public const float MOVE_STAGE_WIDTH = -480f;
		public const float E_TO_E_COEFFICIENT = 2.0444443f;
		public const float JOY_MOTION_PAUSE_DURATION = 0.3f;
		public const int JOY_MOTION_PAUSE_START_FRAME = 3;
		public const float INVISIBLE_X = -1400f;
		public const float UNIT_RUNOUT_THRESHOLD_X = 1800f;
		public const float RUN_MOVE_RATE = 1.6f;
		public const int BATTLE_UB_TIMELINE_MAX = 500;
		public static readonly string[] HP_GAUGE_MODE_LABEL_OUTLINE_COLOR;
		public const float BATTLE_SCALE = 540f;
		public const float CAMERA_MIN_X = 1100f;
		public const int BG_WIDTH = 1440;
		public const int BG_HEIGHT = 540;
		public const float RESULT_CAMERA_DEFAULT_SCALE = 1f;
		public static readonly float HEAD_OFFSET;
		public const float UNIT_MOVE_SPEED = 1600f;
		public const int ALONE_UNIT_COUNT = 1;
		public const string FOOTER_SHAKE_NAME = "battle_footer_icon_shake";
		public const string FOOTER_SKILL_RESERVE_ANIMATION_NAME = "SkillReserveEffect_idle";
		public const string HEADER_SKILL_RESERVE_LABEL_ANIMATION_NAME = "ReserveSkillIcon_disappear";
		public const string ENVIRONMENT_APPEAR_ANIMATTION_NAME_L = "EnvironmentIcon_appear_l";
		public const string ENVIRONMENT_DISAPPEAR_ANIMATTION_NAME_L = "EnvironmentIcon_disappear_l";
		public const string ENVIRONMENT_APPEAR_ANIMATTION_NAME_R = "EnvironmentIcon_appear_r";
		public const string ENVIRONMENT_DISAPPEAR_ANIMATTION_NAME_R = "EnvironmentIcon_disappear_r";
		public const string TALENT_BONUS_BACKGROUND_SPRITE_NAME = "battle_talent_bonus_bg_{0}";
		public const string TALENT_BONUS_BACKGROUND_RIGHT_SPRITE_NAME = "battle_talent_bonus_bg_r_{0}";
		public const string TALENT_BONUS_SYMBOL_SPRITE_NAME = "battle_talent_bonus_{0}";
		public const float GOLD_EFFECT_DELAY = 0.2f;
		public const float TREASURE_DROP_INTERVAL = 0.02f;
		public const float TREASURE_DROP_INTERVAL_RARE = 0.06f;
		public const float TREASURE_MOVE_START_INTERVAL = 0.05f;
		public const float TREASURE_POS_OFFSET_Y = 1f / 12f;
		public static readonly Vector3 TREASURE_BOX_POS;
		public const int WAVE_MAX = 3;
		public const int FIRST_MODE_NUM = 1;
		public const int FINAL_MODE_3_NUM = 3;
		public const int MODE_CLEAR_WAIT_TIME = 2;
		public const int BGM_ADDITIONAL_VOICE_PRIORITY = 10;
		public const double MAX_DAMAGE = 9999999.0;
		public const long MAX_TOTAL_DAMAGE = 99999999L;
		public const long MAX_RECOVERY_VALUE = 99999999L;
		public const long MAX_PARAMETER_VALUE = 2000000000L;
		public const int MOTION_PREFIX_MODE_CHANGE = 1;
		public const double PERCENT_DIGIT = 100.0;
		public const int PERCENT_DIGIT_INT = 100;
		public const double PERMYRIAD_DIGIT = 10000.0;
		public const double FROM_PERCENT_TO_PERMYRIAD_DIGIT = 100.0;
		public const int ENEMY_COLOR_DEFAULT = 0;
		public const int ENEMY_COLOR_START = 1;
		public const int ENEMY_COLOR_LAST = 5;
		public const float HEAL_INTERVAL = 0.45f;
		public const int COLUMN_VALUE_TO_SHOW_ABNORMAL_STATE_ICON = 0;
		public const int IN_BATTLE_DIALOG_SORTING_ORDER = 9000;
		public static readonly Dictionary<eDamageNumSizeType, int> DAMAGE_NUM_SIZE_DICTIONARY;
		public const int EFFECT_SIZE_DEFAULT = 100;
		public const float DEFAULT_DAMAGE_NUM_EFFECT_SCALE = 1f;
		public const int DAMAGE_NUM_EFFECT_SIZE_HIDDEN = 0;
		public const int DAMAGE_NUM_EFFECT_SIZE_DEFAULT = 100;
		public enum eBuffArrowEffectEnableType
		{
			PERCENT_100 = 0,
			PERCENT_75 = 1,
			OFF = 2
		}

		public enum eRecoverEffectEnableType
		{
			ON = 0,
			OFF = 1
		}

		public enum eDamageNumEnableType
		{
			ALL = 0,
			DAMAGE_ONLY = 1,
			HEAL_ONLY = 2,
			OFF = 3
		}

		public enum eDamageNumSizeType
		{
			PERCENT_100 = 0,
			PERCENT_75 = 1,
			PERCENT_50 = 2
		}

		public static readonly global::System.Collections.Generic.Dictionary<Elements.BattleDefine.eBuffArrowEffectEnableType, System.Int32> BUFF_ARROW_EFFECT_SIZE_DICTIONARY;
		public static readonly global::System.Collections.Generic.Dictionary<Elements.SystemIdDefine.eWeaponSeType, Elements.eSE> WEAPON_SE_DIC;
		public static readonly global::System.Collections.Generic.Dictionary<Elements.SystemIdDefine.eWeaponSeType, Elements.eResourceId> WEAPON_HIT_EFFECT_DIC;
		public static readonly global::System.Collections.Generic.Dictionary<Elements.eResourceId, Elements.eResourceId> WEAPON_HIT_EFFECT_DIC_L;
		[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 12)]
		public struct VoiceJudgeData
		{
			public bool PlayIfOther;
			public bool PlayIfShadow;
			public bool Star6Voice;
		}

		public static readonly global::System.Collections.Generic.Dictionary<Elements.SoundManager.eVoiceType, Elements.BattleDefine.VoiceJudgeData> VoiceJudgeDataDictionary;
		public static readonly global::System.Collections.Generic.Dictionary<Elements.SoundManager.eVoiceType, Elements.SoundManager.eVoiceType> ModeChangeVoiceDictionary;
		[global::OpenPCR.Migration("be4855d9-4ca2-43f4-9a6f-a8fd591ccfe2")]
		static BattleDefine()
		{
			BattleBgmDic = new Dictionary<eBattleCategory, eBGM[]>
			{
				{
					(eBattleCategory)0,
					null
				},
				{
					(eBattleCategory)1,
					null
				},
				{
					(eBattleCategory)2,
					null
				},
				{
					(eBattleCategory)30,
					null
				},
				{
					(eBattleCategory)3,
					new eBGM[]
					{
						(eBGM)14
					}
				},
				{
					(eBattleCategory)4,
					new eBGM[]
					{
						(eBGM)14
					}
				},
				{
					(eBattleCategory)24,
					new eBGM[]
					{
						(eBGM)14
					}
				},
				{
					(eBattleCategory)9,
					null
				},
				{
					(eBattleCategory)20,
					null
				},
				{
					(eBattleCategory)5,
					null
				},
				{
					(eBattleCategory)6,
					null
				},
				{
					(eBattleCategory)7,
					new eBGM[]
					{
						(eBGM)15,
						(eBGM)16,
						(eBGM)17
					}
				},
				{
					(eBattleCategory)8,
					new eBGM[]
					{
						(eBGM)15,
						(eBGM)16,
						(eBGM)17
					}
				},
				{
					(eBattleCategory)10,
					null
				},
				{
					(eBattleCategory)11,
					null
				},
				{
					(eBattleCategory)16,
					null
				},
				{
					(eBattleCategory)12,
					null
				},
				{
					(eBattleCategory)13,
					null
				},
				{
					(eBattleCategory)14,
					null
				},
				{
					(eBattleCategory)19,
					null
				},
				{
					(eBattleCategory)33,
					null
				},
				{
					(eBattleCategory)15,
					null
				},
				{
					(eBattleCategory)17,
					null
				},
				{
					(eBattleCategory)18,
					null
				},
				{
					(eBattleCategory)34,
					null
				},
				{
					(eBattleCategory)21,
					null
				},
				{
					(eBattleCategory)22,
					null
				},
				{
					(eBattleCategory)36,
					null
				},
				{
					(eBattleCategory)37,
					null
				},
				{
					(eBattleCategory)38,
					null
				},
				{
					(eBattleCategory)23,
					null
				},
				{
					(eBattleCategory)25,
					null
				},
				{
					(eBattleCategory)26,
					null
				},
				{
					(eBattleCategory)29,
					null
				},
				{
					(eBattleCategory)27,
					null
				},
				{
					(eBattleCategory)28,
					null
				},
				{
					(eBattleCategory)31,
					null
				},
				{
					(eBattleCategory)32,
					null
				},
				{
					(eBattleCategory)35,
					null
				},
				{
					(eBattleCategory)39,
					null
				},
				{
					(eBattleCategory)40,
					null
				},
				{
					(eBattleCategory)41,
					new eBGM[]
					{
						(eBGM)19
					}
				},
				{
					(eBattleCategory)42,
					new eBGM[]
					{
						(eBGM)19
					}
				},
				{
					(eBattleCategory)43,
					null
				},
				{
					(eBattleCategory)44,
					new eBGM[]
					{
						(eBGM)21
					}
				},
				{
					(eBattleCategory)45,
					new eBGM[]
					{
						(eBGM)21
					}
				},
				{
					(eBattleCategory)46,
					null
				},
				{
					(eBattleCategory)47,
					null
				},
				{
					(eBattleCategory)48,
					null
				},
				{
					(eBattleCategory)49,
					null
				},
				{
					(eBattleCategory)50,
					null
				},
				{
					(eBattleCategory)51,
					null
				},
				{
					(eBattleCategory)52,
					null
				},
				{
					(eBattleCategory)53,
					null
				},
				{
					(eBattleCategory)54,
					null
				},
				{
					(eBattleCategory)55,
					null
				},
				{
					(eBattleCategory)56,
					null
				},
				{
					(eBattleCategory)57,
					null
				},
				{
					(eBattleCategory)58,
					null
				},
				{
					(eBattleCategory)59,
					null
				},
				{
					(eBattleCategory)60,
					null
				},
				{
					(eBattleCategory)66,
					null
				},
			};
			UnitRespawnPosList = new List<eUnitRespawnPos>
			{
				(eUnitRespawnPos)4,
				(eUnitRespawnPos)8,
				(eUnitRespawnPos)0,
				(eUnitRespawnPos)6,
				(eUnitRespawnPos)2,
				(eUnitRespawnPos)9
			};
			UnitRespawnPosTopToBottom = new List<eUnitRespawnPos>
			{
				(eUnitRespawnPos)0,
				(eUnitRespawnPos)2,
				(eUnitRespawnPos)4,
				(eUnitRespawnPos)6,
				(eUnitRespawnPos)8
			};
			enemyRespawnPosList = new List<eUnitRespawnPos>
			{
				(eUnitRespawnPos)4,
				(eUnitRespawnPos)8,
				(eUnitRespawnPos)0,
				(eUnitRespawnPos)6,
				(eUnitRespawnPos)2
			};
			unitSortOrderDictionary = new Dictionary<eUnitRespawnPos, int>
			{
				{
					(eUnitRespawnPos)0,
					1500
				},
				{
					(eUnitRespawnPos)2,
					3500
				},
				{
					(eUnitRespawnPos)4,
					5500
				},
				{
					(eUnitRespawnPos)6,
					7500
				},
				{
					(eUnitRespawnPos)8,
					9500
				},
				{
					(eUnitRespawnPos)1,
					2500
				},
				{
					(eUnitRespawnPos)3,
					4500
				},
				{
					(eUnitRespawnPos)5,
					6500
				},
				{
					(eUnitRespawnPos)7,
					8500
				},
				{
					(eUnitRespawnPos)9,
					10500
				},
			};
			RESPAWN_POS = new Dictionary<eUnitRespawnPos, float>
			{
				{
					(eUnitRespawnPos)0,
					40.0f
				},
				{
					(eUnitRespawnPos)2,
					-10.222221f
				},
				{
					(eUnitRespawnPos)4,
					-60.444443f
				},
				{
					(eUnitRespawnPos)6,
					-110.666664f
				},
				{
					(eUnitRespawnPos)8,
					-160.88889f
				},
				{
					(eUnitRespawnPos)1,
					14.888891f
				},
				{
					(eUnitRespawnPos)3,
					-35.333336f
				},
				{
					(eUnitRespawnPos)5,
					-85.55556f
				},
				{
					(eUnitRespawnPos)7,
					-135.77779f
				},
				{
					(eUnitRespawnPos)9,
					-186.0f
				},
			};
			SUMMON_RESPAWN_PRIORITY = new Dictionary<eUnitRespawnPos, List<eUnitRespawnPos>>
			{
				{
					(eUnitRespawnPos)0,
					new List<eUnitRespawnPos>
					{
						(eUnitRespawnPos)9,
						(eUnitRespawnPos)8,
						(eUnitRespawnPos)5,
						(eUnitRespawnPos)4,
						(eUnitRespawnPos)7,
						(eUnitRespawnPos)6,
						(eUnitRespawnPos)3,
						(eUnitRespawnPos)2,
						(eUnitRespawnPos)1,
						(eUnitRespawnPos)0
					}
				},
				{
					(eUnitRespawnPos)2,
					new List<eUnitRespawnPos>
					{
						(eUnitRespawnPos)9,
						(eUnitRespawnPos)8,
						(eUnitRespawnPos)5,
						(eUnitRespawnPos)4,
						(eUnitRespawnPos)7,
						(eUnitRespawnPos)6,
						(eUnitRespawnPos)1,
						(eUnitRespawnPos)0,
						(eUnitRespawnPos)3,
						(eUnitRespawnPos)2
					}
				},
				{
					(eUnitRespawnPos)4,
					new List<eUnitRespawnPos>
					{
						(eUnitRespawnPos)9,
						(eUnitRespawnPos)8,
						(eUnitRespawnPos)1,
						(eUnitRespawnPos)0,
						(eUnitRespawnPos)7,
						(eUnitRespawnPos)6,
						(eUnitRespawnPos)3,
						(eUnitRespawnPos)2,
						(eUnitRespawnPos)5,
						(eUnitRespawnPos)4
					}
				},
				{
					(eUnitRespawnPos)6,
					new List<eUnitRespawnPos>
					{
						(eUnitRespawnPos)0,
						(eUnitRespawnPos)1,
						(eUnitRespawnPos)4,
						(eUnitRespawnPos)5,
						(eUnitRespawnPos)6,
						(eUnitRespawnPos)7,
						(eUnitRespawnPos)2,
						(eUnitRespawnPos)3,
						(eUnitRespawnPos)0,
						(eUnitRespawnPos)1
					}
				},
				{
					(eUnitRespawnPos)8,
					new List<eUnitRespawnPos>
					{
						(eUnitRespawnPos)0,
						(eUnitRespawnPos)1,
						(eUnitRespawnPos)4,
						(eUnitRespawnPos)5,
						(eUnitRespawnPos)2,
						(eUnitRespawnPos)3,
						(eUnitRespawnPos)8,
						(eUnitRespawnPos)9,
						(eUnitRespawnPos)6,
						(eUnitRespawnPos)7
					}
				},
				{
					(eUnitRespawnPos)9,
					new List<eUnitRespawnPos>
					{
						(eUnitRespawnPos)0,
						(eUnitRespawnPos)1,
						(eUnitRespawnPos)4,
						(eUnitRespawnPos)5,
						(eUnitRespawnPos)2,
						(eUnitRespawnPos)3,
						(eUnitRespawnPos)8,
						(eUnitRespawnPos)9,
						(eUnitRespawnPos)6,
						(eUnitRespawnPos)7
					}
				},
			};
			BATTLE_FIELD_SIZE = 1024f;
			WEAPON_SE_DIC = new Dictionary<SystemIdDefine.eWeaponSeType, eSE>
			{
				{
					(SystemIdDefine.eWeaponSeType)8,
					(eSE)12
				},
				{
					(SystemIdDefine.eWeaponSeType)3,
					(eSE)13
				},
				{
					(SystemIdDefine.eWeaponSeType)0,
					(eSE)9
				},
				{
					(SystemIdDefine.eWeaponSeType)1,
					(eSE)14
				},
				{
					(SystemIdDefine.eWeaponSeType)5,
					(eSE)16
				},
				{
					(SystemIdDefine.eWeaponSeType)2,
					(eSE)17
				},
				{
					(SystemIdDefine.eWeaponSeType)6,
					(eSE)18
				},
				{
					(SystemIdDefine.eWeaponSeType)4,
					(eSE)19
				},
				{
					(SystemIdDefine.eWeaponSeType)7,
					(eSE)20
				},
				{
					(SystemIdDefine.eWeaponSeType)11,
					(eSE)27
				},
				{
					(SystemIdDefine.eWeaponSeType)12,
					(eSE)26
				},
				{
					(SystemIdDefine.eWeaponSeType)14,
					(eSE)21
				},
				{
					(SystemIdDefine.eWeaponSeType)15,
					(eSE)22
				},
				{
					(SystemIdDefine.eWeaponSeType)16,
					(eSE)23
				},
				{
					(SystemIdDefine.eWeaponSeType)17,
					(eSE)24
				},
				{
					(SystemIdDefine.eWeaponSeType)13,
					(eSE)25
				},
				{
					(SystemIdDefine.eWeaponSeType)27,
					(eSE)188
				},
				{
					(SystemIdDefine.eWeaponSeType)28,
					(eSE)13
				},
				{
					(SystemIdDefine.eWeaponSeType)29,
					(eSE)20
				},
				{
					(SystemIdDefine.eWeaponSeType)30,
					(eSE)13
				},
				{
					(SystemIdDefine.eWeaponSeType)32,
					(eSE)28
				},
				{
					(SystemIdDefine.eWeaponSeType)33,
					(eSE)189
				},
				{
					(SystemIdDefine.eWeaponSeType)34,
					(eSE)190
				},
				{
					(SystemIdDefine.eWeaponSeType)35,
					(eSE)191
				},
				{
					(SystemIdDefine.eWeaponSeType)36,
					(eSE)192
				},
				{
					(SystemIdDefine.eWeaponSeType)37,
					(eSE)193
				},
				{
					(SystemIdDefine.eWeaponSeType)38,
					(eSE)194
				},
				{
					(SystemIdDefine.eWeaponSeType)39,
					(eSE)20
				},
				{
					(SystemIdDefine.eWeaponSeType)40,
					(eSE)19
				},
				{
					(SystemIdDefine.eWeaponSeType)41,
					(eSE)16
				},
				{
					(SystemIdDefine.eWeaponSeType)42,
					(eSE)20
				},
				{
					(SystemIdDefine.eWeaponSeType)43,
					(eSE)195
				},
				{
					(SystemIdDefine.eWeaponSeType)44,
					(eSE)196
				},
				{
					(SystemIdDefine.eWeaponSeType)45,
					(eSE)197
				},
				{
					(SystemIdDefine.eWeaponSeType)46,
					(eSE)198
				},
				{
					(SystemIdDefine.eWeaponSeType)48,
					(eSE)18
				},
				{
					(SystemIdDefine.eWeaponSeType)49,
					(eSE)199
				},
				{
					(SystemIdDefine.eWeaponSeType)51,
					(eSE)200
				},
				{
					(SystemIdDefine.eWeaponSeType)54,
					(eSE)201
				},
				{
					(SystemIdDefine.eWeaponSeType)55,
					(eSE)20
				},
			};
			WEAPON_HIT_EFFECT_DIC = new Dictionary<SystemIdDefine.eWeaponSeType, eResourceId>
			{
				{
					(SystemIdDefine.eWeaponSeType)8,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)3,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)0,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)1,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)5,
					(eResourceId)1520815359
				},
				{
					(SystemIdDefine.eWeaponSeType)2,
					(eResourceId)1792410446
				},
				{
					(SystemIdDefine.eWeaponSeType)6,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)4,
					(eResourceId)1520815359
				},
				{
					(SystemIdDefine.eWeaponSeType)7,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)11,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)12,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)14,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)15,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)16,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)17,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)13,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)27,
					(eResourceId)1312789630
				},
				{
					(SystemIdDefine.eWeaponSeType)28,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)29,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)30,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)32,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)33,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)34,
					(eResourceId)1520815359
				},
				{
					(SystemIdDefine.eWeaponSeType)35,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)36,
					(eResourceId)1792410446
				},
				{
					(SystemIdDefine.eWeaponSeType)37,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)38,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)39,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)40,
					(eResourceId)1520815359
				},
				{
					(SystemIdDefine.eWeaponSeType)41,
					(eResourceId)1520815359
				},
				{
					(SystemIdDefine.eWeaponSeType)42,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)43,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)44,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)45,
					(eResourceId)1520815359
				},
				{
					(SystemIdDefine.eWeaponSeType)46,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)48,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)49,
					(eResourceId)1855402071
				},
				{
					(SystemIdDefine.eWeaponSeType)51,
					(eResourceId)1974951892
				},
				{
					(SystemIdDefine.eWeaponSeType)54,
					(eResourceId)1848040296
				},
				{
					(SystemIdDefine.eWeaponSeType)55,
					(eResourceId)1848040296
				},
			};
			WEAPON_HIT_EFFECT_DIC_L = new Dictionary<eResourceId, eResourceId>
			{
				{
					(eResourceId)1792410446,
					(eResourceId)1861612899
				},
				{
					(eResourceId)1848040296,
					(eResourceId)1593129970
				},
				{
					(eResourceId)1520815359,
					(eResourceId)1903280913
				},
				{
					(eResourceId)1312789630,
					(eResourceId)2070190455
				},
				{
					(eResourceId)1855402071,
					(eResourceId)1914594109
				},
				{
					(eResourceId)1974951892,
					(eResourceId)1163357076
				},
			};
			WEAPON_HIT_DELAY_DIC = new Dictionary<SystemIdDefine.eWeaponMotionType, float>
			{
				{
					(SystemIdDefine.eWeaponMotionType)8,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)3,
					0.45f
				},
				{
					(SystemIdDefine.eWeaponMotionType)1,
					0.4f
				},
				{
					(SystemIdDefine.eWeaponMotionType)5,
					0.47f
				},
				{
					(SystemIdDefine.eWeaponMotionType)2,
					0.4f
				},
				{
					(SystemIdDefine.eWeaponMotionType)6,
					0.35f
				},
				{
					(SystemIdDefine.eWeaponMotionType)4,
					0.35f
				},
				{
					(SystemIdDefine.eWeaponMotionType)7,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)9,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)10,
					0.6f
				},
				{
					(SystemIdDefine.eWeaponMotionType)22,
					0.4f
				},
				{
					(SystemIdDefine.eWeaponMotionType)21,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)23,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)25,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)26,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)27,
					0.3f
				},
				{
					(SystemIdDefine.eWeaponMotionType)28,
					0.45f
				},
				{
					(SystemIdDefine.eWeaponMotionType)29,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)30,
					0.45f
				},
				{
					(SystemIdDefine.eWeaponMotionType)31,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)32,
					0.4f
				},
				{
					(SystemIdDefine.eWeaponMotionType)33,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)34,
					0.35f
				},
				{
					(SystemIdDefine.eWeaponMotionType)35,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)36,
					0.4f
				},
				{
					(SystemIdDefine.eWeaponMotionType)37,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)38,
					0.45f
				},
				{
					(SystemIdDefine.eWeaponMotionType)39,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)40,
					0.35f
				},
				{
					(SystemIdDefine.eWeaponMotionType)41,
					0.47f
				},
				{
					(SystemIdDefine.eWeaponMotionType)42,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)43,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)44,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)45,
					1.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)46,
					0.3f
				},
				{
					(SystemIdDefine.eWeaponMotionType)47,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)48,
					0.35f
				},
				{
					(SystemIdDefine.eWeaponMotionType)50,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)52,
					0.4f
				},
				{
					(SystemIdDefine.eWeaponMotionType)53,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)54,
					0.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)55,
					0.0f
				},
			};
			WEAPON_EFFECT_DELAY_DIC = new Dictionary<SystemIdDefine.eWeaponMotionType, float>
			{
				{
					(SystemIdDefine.eWeaponMotionType)8,
					0.9f
				},
				{
					(SystemIdDefine.eWeaponMotionType)7,
					0.3f
				},
				{
					(SystemIdDefine.eWeaponMotionType)9,
					0.3f
				},
				{
					(SystemIdDefine.eWeaponMotionType)29,
					0.3f
				},
				{
					(SystemIdDefine.eWeaponMotionType)31,
					0.3f
				},
				{
					(SystemIdDefine.eWeaponMotionType)33,
					0.9f
				},
				{
					(SystemIdDefine.eWeaponMotionType)35,
					0.9f
				},
				{
					(SystemIdDefine.eWeaponMotionType)37,
					0.3f
				},
				{
					(SystemIdDefine.eWeaponMotionType)39,
					0.3f
				},
				{
					(SystemIdDefine.eWeaponMotionType)42,
					0.3f
				},
				{
					(SystemIdDefine.eWeaponMotionType)44,
					0.3f
				},
				{
					(SystemIdDefine.eWeaponMotionType)45,
					1.0f
				},
				{
					(SystemIdDefine.eWeaponMotionType)46,
					0.3f
				},
				{
					(SystemIdDefine.eWeaponMotionType)50,
					0.3f
				},
				{
					(SystemIdDefine.eWeaponMotionType)53,
					0.3f
				},
				{
					(SystemIdDefine.eWeaponMotionType)54,
					0.9f
				},
				{
					(SystemIdDefine.eWeaponMotionType)55,
					0.3f
				},
			};
			HP_GAUGE_MODE_LABEL_OUTLINE_COLOR = new[]
			{
				"#591988FF",
				"#085499FF",
				"#8D3A1AFF"
			};
			HEAD_OFFSET = 100f;
			AUDIO_LISTENER_POS_Z = -9.25926018f;
			TREASURE_POS_OFFSET_X = new[]
			{
				0f,
				0.277777791f,
				-0.277777791f
			};
			TREASURE_BOX_POS = new Vector3(-1.16199994f, -0.177259997f, 0f);
			VoiceJudgeDataDictionary = new Dictionary<SoundManager.eVoiceType, VoiceJudgeData>
			{
				{
					(SoundManager.eVoiceType)30,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)22,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)20,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)16,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)18,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)11,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)46,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = true
					}
				},
				{
					(SoundManager.eVoiceType)12,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = true
					}
				},
				{
					(SoundManager.eVoiceType)13,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)14,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)15,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)24,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)27,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)9,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)7,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)32,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)34,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)35,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)36,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)33,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)41,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)42,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)37,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)38,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)39,
					new VoiceJudgeData
					{
						PlayIfOther = false,
						PlayIfShadow = false,
						Star6Voice = false
					}
				},
				{
					(SoundManager.eVoiceType)48,
					new VoiceJudgeData
					{
						PlayIfOther = true,
						PlayIfShadow = true,
						Star6Voice = false
					}
				},
			};
			ModeChangeVoiceDictionary = new Dictionary<SoundManager.eVoiceType, SoundManager.eVoiceType>
			{
				{
					(SoundManager.eVoiceType)7,
					(SoundManager.eVoiceType)8
				},
				{
					(SoundManager.eVoiceType)9,
					(SoundManager.eVoiceType)10
				},
				{
					(SoundManager.eVoiceType)24,
					(SoundManager.eVoiceType)25
				},
				{
					(SoundManager.eVoiceType)27,
					(SoundManager.eVoiceType)28
				},
				{
					(SoundManager.eVoiceType)46,
					(SoundManager.eVoiceType)47
				},
			};
			DAMAGE_NUM_SIZE_DICTIONARY = new Dictionary<eDamageNumSizeType, int>
			{
				{
					(eDamageNumSizeType)0,
					100
				},
				{
					(eDamageNumSizeType)1,
					75
				},
				{
					(eDamageNumSizeType)2,
					50
				},
			};
			BUFF_ARROW_EFFECT_SIZE_DICTIONARY = new Dictionary<eBuffArrowEffectEnableType, int>
			{
				{
					(eBuffArrowEffectEnableType)0,
					100
				},
				{
					(eBuffArrowEffectEnableType)1,
					75
				},
				{
					(eBuffArrowEffectEnableType)2,
					0
				},
			};
		}

		[Serializable]
		public class ZoomEffect
		{
			public float ToPosX { get; set; }
			public float ToPosY { get; set; }

			public bool Enable;
			public bool ConsiderScreenSize;
			public float StartDelay;
			public float StartDuration;
			public float Duration;
			public float EndDuration;
			public float To;
			public eZoomTarget ZoomTarget;
			public float OffsetY;
			public float OffsetX;
			public bool UseYBottom;
			public CustomEasing StartEasing;
			public CustomEasing EndEasing;
			public CustomEasing StartPosXEasing;
			public CustomEasing EndPosXEasing;
			public CustomEasing StartPosYEasing;
			public CustomEasing EndPosYEasing;
			public List<ZoomEffectPlual> ZoomEffectList;
			public CustomEasing.eType StartEasingType;
			public CustomEasing.eType EndEasingType;
			public string BoneName;
			[global::OpenPCR.Migration("a7385e39-f701-4899-a71b-91c443e1dea6")]
			public ZoomEffect()
			{
				ConsiderScreenSize = true;
				StartDuration = 0.150000006f;
				Duration = 1.54999995f;
				EndDuration = 0.100000001f;
				To = 0.899999976f;
				ZoomEffectList = new List<ZoomEffectPlual>();
				BoneName = string.Empty;
				StartEasingType = (CustomEasing.eType)2;
				EndEasingType = (CustomEasing.eType)2;
			}
		}

		[Serializable]
		public class ZoomEffectPlual
		{
			public float ToPosX { get; set; }
			public float ToPosY { get; set; }

			public float StartDuration;
			public float Duration;
			public float To;
			public float OffsetX;
			public float OffsetY;
			public eZoomTarget ZoomTarget;
			public CustomEasing StartEasing;
			public CustomEasing StartPosXEasing;
			public CustomEasing StartPosYEasing;
			public CustomEasing.eType StartEasingType;
			public bool UseYBottom;
			public string BoneName;
			public ZoomEffectPlual()
			{
				StartDuration = 0.150000006f;
				Duration = 1.54999995f;
				To = 0.5f;
				StartEasingType = (CustomEasing.eType)2;
				BoneName = string.Empty;
			}
		}

		[Serializable]
		public class SlowEffect
		{
			public bool Enable;
			public float StartDelay;
			public float StartDuration;
			public float Duration;
			public float EndDuration;
			public float To;
			public CustomEasing StartEasing;
			public CustomEasing EndEasing;
			[global::OpenPCR.Migration("d36d3fbe-1a13-483b-ad81-97aab11a9590")]
			public SlowEffect()
			{
				Duration = 1.54999995f;
				EndDuration = 0.100000001f;
				To = 0.899999976f;
			}
		}

		public enum eExtraEffectExecTiming
		{
			EX_SKILL = 1,
			ALL_UNIT_ACTION,
			MY_TEAM_ACTION,
			ENEMY_ACTION
		}

		public static readonly List<eUnitRespawnPos> UnitRespawnPosList;
		public static readonly Dictionary<eUnitRespawnPos, float> RESPAWN_POS;
		public static readonly Dictionary<eUnitRespawnPos, List<eUnitRespawnPos>> SUMMON_RESPAWN_PRIORITY;
		public static readonly float BATTLE_FIELD_SIZE;
		public static readonly float AUDIO_LISTENER_POS_Z;
		public static readonly float[] TREASURE_POS_OFFSET_X;


		[global::OpenPCR.Migration("5f189dcf-129a-4b21-aab9-ef5299dc6972")]
		public static eUnitRespawnPos GetUnitRespawnPos(int index, bool isEnemySide = false)
		{
			if (index < 0 || index >= UnitRespawnPosList.Count)
				return eUnitRespawnPos.MAIN_POS_3;
			return isEnemySide ? enemyRespawnPosList[index] : UnitRespawnPosList[index];
		}

		[global::OpenPCR.Migration("e02bf5b1-df77-4adf-be19-c618453d5ab3")]
		public static int GetUnitSortOrder(UnitCtrl _unit)
		{
			if (_unit.IsDepthBack)
				return BACK_CHARA_ORDER;
			if (!unitSortOrderDictionary.ContainsKey(_unit.RespawnPos))
				return 0;
			return unitSortOrderDictionary[_unit.RespawnPos] - (_unit.IsOther ? 500 : 0);
		}

		public static readonly Dictionary<SystemIdDefine.eWeaponMotionType, float> WEAPON_HIT_DELAY_DIC;
		public static readonly Dictionary<SystemIdDefine.eWeaponMotionType, float> WEAPON_EFFECT_DELAY_DIC;
	}
}
