using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public static class AbnormalStateDefine
	{
		public const float SLOW_ANIM_SPEED = 0.5f;
		public const float HASTE_ANIM_SPEED = 2f;
		public const float NORMAL_ANIM_SPEED = 1f;
		public const float SLIP_DAMAGE_INTERVAL_TIME = 1f;
		public const float SLIP_DAMAGE_INTERVAL_EXTEND_DEFAULT = 1f;
		public static readonly Dictionary<eAbnormalState, AbnormalConstData> ABNORMAL_CONST_DATA;
		public static readonly Dictionary<BuffParamKind, bool> IS_EFFECT_BOSS_BODY_ONLY;
		[global::OpenPCR.Migration("c5a3c84f-fa25-4301-bf90-f64c8e417636")]
		static AbnormalStateDefine()
		{
			ABNORMAL_CONST_DATA = new Dictionary<eAbnormalState, AbnormalConstData>
			{
				{
					(eAbnormalState)1,
					new AbnormalConstData
					{
						IconType = (eStateIconType)24,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)2,
					new AbnormalConstData
					{
						IconType = (eStateIconType)25,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)3,
					new AbnormalConstData
					{
						IconType = (eStateIconType)26,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)4,
					new AbnormalConstData
					{
						IconType = (eStateIconType)27,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)5,
					new AbnormalConstData
					{
						IconType = (eStateIconType)28,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)6,
					new AbnormalConstData
					{
						IconType = (eStateIconType)29,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)7,
					new AbnormalConstData
					{
						IconType = (eStateIconType)9,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)8,
					new AbnormalConstData
					{
						IconType = (eStateIconType)33,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)59,
					new AbnormalConstData
					{
						IconType = (eStateIconType)33,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)9,
					new AbnormalConstData
					{
						IconType = (eStateIconType)38,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)10,
					new AbnormalConstData
					{
						IconType = (eStateIconType)39,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)60,
					new AbnormalConstData
					{
						IconType = (eStateIconType)39,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)11,
					new AbnormalConstData
					{
						IconType = (eStateIconType)22,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)12,
					new AbnormalConstData
					{
						IconType = (eStateIconType)32,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)13,
					new AbnormalConstData
					{
						IconType = (eStateIconType)40,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)14,
					new AbnormalConstData
					{
						IconType = (eStateIconType)36,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)15,
					new AbnormalConstData
					{
						IconType = (eStateIconType)34,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)49,
					new AbnormalConstData
					{
						IconType = (eStateIconType)89,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)16,
					new AbnormalConstData
					{
						IconType = (eStateIconType)35,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)17,
					new AbnormalConstData
					{
						IconType = (eStateIconType)41,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)77,
					new AbnormalConstData
					{
						IconType = (eStateIconType)166,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)18,
					new AbnormalConstData
					{
						IconType = (eStateIconType)42,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)19,
					new AbnormalConstData
					{
						IconType = (eStateIconType)43,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)58,
					new AbnormalConstData
					{
						IconType = (eStateIconType)43,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)20,
					new AbnormalConstData
					{
						IconType = (eStateIconType)45,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)21,
					new AbnormalConstData
					{
						IconType = (eStateIconType)33,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)22,
					new AbnormalConstData
					{
						IconType = (eStateIconType)10,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)62,
					new AbnormalConstData
					{
						IconType = (eStateIconType)10,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)23,
					new AbnormalConstData
					{
						IconType = (eStateIconType)30,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)24,
					new AbnormalConstData
					{
						IconType = (eStateIconType)30,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)39,
					new AbnormalConstData
					{
						IconType = (eStateIconType)0,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)25,
					new AbnormalConstData
					{
						IconType = (eStateIconType)0,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)72,
					new AbnormalConstData
					{
						IconType = (eStateIconType)0,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)26,
					new AbnormalConstData
					{
						IconType = (eStateIconType)37,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)27,
					new AbnormalConstData
					{
						IconType = (eStateIconType)0,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)28,
					new AbnormalConstData
					{
						IconType = (eStateIconType)44,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)29,
					new AbnormalConstData
					{
						IconType = (eStateIconType)46,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)63,
					new AbnormalConstData
					{
						IconType = (eStateIconType)46,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)30,
					new AbnormalConstData
					{
						IconType = (eStateIconType)48,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)31,
					new AbnormalConstData
					{
						IconType = (eStateIconType)49,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)61,
					new AbnormalConstData
					{
						IconType = (eStateIconType)49,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)32,
					new AbnormalConstData
					{
						IconType = (eStateIconType)51,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)33,
					new AbnormalConstData
					{
						IconType = (eStateIconType)52,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)34,
					new AbnormalConstData
					{
						IconType = (eStateIconType)53,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)35,
					new AbnormalConstData
					{
						IconType = (eStateIconType)54,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)36,
					new AbnormalConstData
					{
						IconType = (eStateIconType)62,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)64,
					new AbnormalConstData
					{
						IconType = (eStateIconType)62,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)37,
					new AbnormalConstData
					{
						IconType = (eStateIconType)65,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)38,
					new AbnormalConstData
					{
						IconType = (eStateIconType)66,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)40,
					new AbnormalConstData
					{
						IconType = (eStateIconType)71,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)41,
					new AbnormalConstData
					{
						IconType = (eStateIconType)73,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)42,
					new AbnormalConstData
					{
						IconType = (eStateIconType)74,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)43,
					new AbnormalConstData
					{
						IconType = (eStateIconType)75,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)44,
					new AbnormalConstData
					{
						IconType = (eStateIconType)78,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)45,
					new AbnormalConstData
					{
						IconType = (eStateIconType)79,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)46,
					new AbnormalConstData
					{
						IconType = (eStateIconType)80,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)47,
					new AbnormalConstData
					{
						IconType = (eStateIconType)81,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)48,
					new AbnormalConstData
					{
						IconType = (eStateIconType)86,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)50,
					new AbnormalConstData
					{
						IconType = (eStateIconType)91,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)51,
					new AbnormalConstData
					{
						IconType = (eStateIconType)93,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)52,
					new AbnormalConstData
					{
						IconType = (eStateIconType)96,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)53,
					new AbnormalConstData
					{
						IconType = (eStateIconType)100,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)54,
					new AbnormalConstData
					{
						IconType = (eStateIconType)102,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)55,
					new AbnormalConstData
					{
						IconType = (eStateIconType)103,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)56,
					new AbnormalConstData
					{
						IconType = (eStateIconType)103,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)57,
					new AbnormalConstData
					{
						IconType = (eStateIconType)103,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)65,
					new AbnormalConstData
					{
						IconType = (eStateIconType)0,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)66,
					new AbnormalConstData
					{
						IconType = (eStateIconType)0,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)67,
					new AbnormalConstData
					{
						IconType = (eStateIconType)114,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)68,
					new AbnormalConstData
					{
						IconType = (eStateIconType)124,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)69,
					new AbnormalConstData
					{
						IconType = (eStateIconType)128,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)70,
					new AbnormalConstData
					{
						IconType = (eStateIconType)128,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)71,
					new AbnormalConstData
					{
						IconType = (eStateIconType)132,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)73,
					new AbnormalConstData
					{
						IconType = (eStateIconType)0,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)74,
					new AbnormalConstData
					{
						IconType = (eStateIconType)184,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)75,
					new AbnormalConstData
					{
						IconType = (eStateIconType)172,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)76,
					new AbnormalConstData
					{
						IconType = (eStateIconType)0,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)78,
					new AbnormalConstData
					{
						IconType = (eStateIconType)167,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)79,
					new AbnormalConstData
					{
						IconType = (eStateIconType)185,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)80,
					new AbnormalConstData
					{
						IconType = (eStateIconType)202,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)82,
					new AbnormalConstData
					{
						IconType = (eStateIconType)227,
						IsBuff = false
					}
				},
				{
					(eAbnormalState)83,
					new AbnormalConstData
					{
						IconType = (eStateIconType)215,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)84,
					new AbnormalConstData
					{
						IconType = (eStateIconType)216,
						IsBuff = true
					}
				},
				{
					(eAbnormalState)85,
					new AbnormalConstData
					{
						IconType = (eStateIconType)217,
						IsBuff = true
					}
				},
			};
			IS_EFFECT_BOSS_BODY_ONLY = new Dictionary<BuffParamKind, bool>
			{
				{
					(BuffParamKind)1,
					false
				},
				{
					(BuffParamKind)2,
					false
				},
				{
					(BuffParamKind)3,
					false
				},
				{
					(BuffParamKind)4,
					false
				},
				{
					(BuffParamKind)5,
					false
				},
				{
					(BuffParamKind)6,
					false
				},
				{
					(BuffParamKind)7,
					false
				},
				{
					(BuffParamKind)8,
					true
				},
				{
					(BuffParamKind)9,
					false
				},
				{
					(BuffParamKind)10,
					true
				},
				{
					(BuffParamKind)11,
					true
				},
				{
					(BuffParamKind)12,
					true
				},
				{
					(BuffParamKind)13,
					false
				},
				{
					(BuffParamKind)14,
					true
				},
				{
					(BuffParamKind)15,
					true
				},
				{
					(BuffParamKind)16,
					true
				},
				{
					(BuffParamKind)17,
					true
				},
				{
					(BuffParamKind)18,
					true
				},
				{
					(BuffParamKind)19,
					true
				},
				{
					(BuffParamKind)100,
					true
				},
				{
					(BuffParamKind)200,
					true
				},
			};
		}
	}
}
