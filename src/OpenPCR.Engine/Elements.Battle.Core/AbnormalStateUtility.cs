namespace Elements.Battle.Core
{
	public static class AbnormalStateUtility
	{
		[global::OpenPCR.Migration("657ec8c1-4ad4-4aa0-9d24-6eaa88afb365")]
		public static eAbnormalStateCategory GetAbnormalStateCategory(eAbnormalState abnormalState)
		{
			switch (abnormalState)
			{
				case (eAbnormalState)1:
				case (eAbnormalState)3:
					return (eAbnormalStateCategory)2;
				case (eAbnormalState)2:
				case (eAbnormalState)4:
					return (eAbnormalStateCategory)1;
				case (eAbnormalState)5:
				case (eAbnormalState)6:
					return (eAbnormalStateCategory)3;
				case (eAbnormalState)7:
				case (eAbnormalState)11:
					return (eAbnormalStateCategory)7;
				case (eAbnormalState)8:
					return (eAbnormalStateCategory)4;
				case (eAbnormalState)9:
					return (eAbnormalStateCategory)5;
				case (eAbnormalState)10:
					return (eAbnormalStateCategory)6;
				case (eAbnormalState)12:
					return (eAbnormalStateCategory)9;
				case (eAbnormalState)13:
					return (eAbnormalStateCategory)8;
				case (eAbnormalState)14:
					return (eAbnormalStateCategory)12;
				case (eAbnormalState)15:
					return (eAbnormalStateCategory)14;
				case (eAbnormalState)16:
					return (eAbnormalStateCategory)13;
				case (eAbnormalState)17:
					return (eAbnormalStateCategory)22;
				case (eAbnormalState)18:
					return (eAbnormalStateCategory)21;
				case (eAbnormalState)19:
					return (eAbnormalStateCategory)10;
				case (eAbnormalState)20:
					return (eAbnormalStateCategory)11;
				case (eAbnormalState)21:
					return (eAbnormalStateCategory)24;
				case (eAbnormalState)22:
					return (eAbnormalStateCategory)18;
				case (eAbnormalState)23:
					return (eAbnormalStateCategory)19;
				case (eAbnormalState)24:
					return (eAbnormalStateCategory)20;
				case (eAbnormalState)25:
					return (eAbnormalStateCategory)23;
				case (eAbnormalState)26:
					return (eAbnormalStateCategory)17;
				case (eAbnormalState)27:
					return (eAbnormalStateCategory)16;
				case (eAbnormalState)28:
					return (eAbnormalStateCategory)25;
				case (eAbnormalState)29:
					return (eAbnormalStateCategory)26;
				case (eAbnormalState)30:
					return (eAbnormalStateCategory)27;
				case (eAbnormalState)31:
					return (eAbnormalStateCategory)28;
				case (eAbnormalState)32:
					return (eAbnormalStateCategory)29;
				case (eAbnormalState)33:
					return (eAbnormalStateCategory)30;
				case (eAbnormalState)34:
					return (eAbnormalStateCategory)31;
				case (eAbnormalState)35:
					return (eAbnormalStateCategory)32;
				case (eAbnormalState)36:
					return (eAbnormalStateCategory)33;
				case (eAbnormalState)37:
					return (eAbnormalStateCategory)34;
				case (eAbnormalState)38:
					return (eAbnormalStateCategory)35;
				case (eAbnormalState)39:
					return (eAbnormalStateCategory)36;
				case (eAbnormalState)40:
					return (eAbnormalStateCategory)37;
				case (eAbnormalState)41:
					return (eAbnormalStateCategory)38;
				case (eAbnormalState)42:
					return (eAbnormalStateCategory)39;
				case (eAbnormalState)43:
					return (eAbnormalStateCategory)40;
				case (eAbnormalState)44:
					return (eAbnormalStateCategory)41;
				case (eAbnormalState)45:
					return (eAbnormalStateCategory)42;
				case (eAbnormalState)46:
					return (eAbnormalStateCategory)43;
				case (eAbnormalState)47:
					return (eAbnormalStateCategory)44;
				case (eAbnormalState)48:
					return (eAbnormalStateCategory)45;
				case (eAbnormalState)49:
					return (eAbnormalStateCategory)15;
				case (eAbnormalState)50:
					return (eAbnormalStateCategory)46;
				case (eAbnormalState)51:
					return (eAbnormalStateCategory)47;
				case (eAbnormalState)52:
					return (eAbnormalStateCategory)48;
				case (eAbnormalState)53:
					return (eAbnormalStateCategory)49;
				case (eAbnormalState)54:
					return (eAbnormalStateCategory)50;
				case (eAbnormalState)55:
					return (eAbnormalStateCategory)53;
				case (eAbnormalState)56:
					return (eAbnormalStateCategory)52;
				case (eAbnormalState)57:
					return (eAbnormalStateCategory)51;
				case (eAbnormalState)58:
					return (eAbnormalStateCategory)54;
				case (eAbnormalState)59:
					return (eAbnormalStateCategory)55;
				case (eAbnormalState)60:
					return (eAbnormalStateCategory)56;
				case (eAbnormalState)61:
					return (eAbnormalStateCategory)57;
				case (eAbnormalState)62:
					return (eAbnormalStateCategory)58;
				case (eAbnormalState)63:
					return (eAbnormalStateCategory)59;
				case (eAbnormalState)64:
					return (eAbnormalStateCategory)60;
				case (eAbnormalState)65:
				case (eAbnormalState)66:
					return (eAbnormalStateCategory)61;
				case (eAbnormalState)67:
					return (eAbnormalStateCategory)62;
				case (eAbnormalState)68:
					return (eAbnormalStateCategory)63;
				case (eAbnormalState)69:
					return (eAbnormalStateCategory)64;
				case (eAbnormalState)70:
				case (eAbnormalState)81:
					return (eAbnormalStateCategory)0;
				case (eAbnormalState)71:
					return (eAbnormalStateCategory)65;
				case (eAbnormalState)72:
					return (eAbnormalStateCategory)66;
				case (eAbnormalState)73:
					return (eAbnormalStateCategory)68;
				case (eAbnormalState)74:
					return (eAbnormalStateCategory)69;
				case (eAbnormalState)75:
					return (eAbnormalStateCategory)67;
				case (eAbnormalState)76:
					return (eAbnormalStateCategory)70;
				case (eAbnormalState)77:
					return (eAbnormalStateCategory)71;
				case (eAbnormalState)78:
					return (eAbnormalStateCategory)72;
				case (eAbnormalState)79:
					return (eAbnormalStateCategory)73;
				case (eAbnormalState)80:
					return (eAbnormalStateCategory)74;
				case (eAbnormalState)82:
					return (eAbnormalStateCategory)75;
				case (eAbnormalState)83:
					return (eAbnormalStateCategory)76;
				case (eAbnormalState)84:
					return (eAbnormalStateCategory)77;
				case (eAbnormalState)85:
					return (eAbnormalStateCategory)78;
				default:
					return eAbnormalStateCategory.NONE;
			}
		}

		[global::OpenPCR.Migration("e7f2afa7-c039-4385-82d2-33a79cae155d")]
		public static eAbnormalState GetAbnormalState(eOverwriteAbnormalState _abnormalState)
		{
			return unchecked((global::Elements.Battle.Core.eAbnormalState)(((unchecked((uint)(@_abnormalState)) == 1u) ? 11u : ((unchecked((uint)(@_abnormalState)) == 2u) ? 7u : 0u))));
		}

		[global::OpenPCR.Migration("e3a9f32a-7c9e-484b-a0db-335cded9a568")]
		public static bool IsUnableAbnormalState(eAbnormalState _state)
		{
			switch (_state)
			{
				case eAbnormalState.PARALYSIS:
				case eAbnormalState.FREEZE:
				case eAbnormalState.CHAINED:
				case eAbnormalState.SLEEP:
				case eAbnormalState.STUN:
				case eAbnormalState.DETAIN:
				case eAbnormalState.STONE:
				case eAbnormalState.FAINT:
				case eAbnormalState.PAUSE_ACTION:
				case eAbnormalState.NPC_STUN:
				case eAbnormalState.CRYSTALIZE:
				case eAbnormalState.STUN2:
				case eAbnormalState.CHAINED_ACN:
					return true;
				default:
					return false;
			}
		}

		[global::OpenPCR.Migration("fb15db65-b374-48e0-8553-40205a8febd0")]
		public static bool CanOverlapAbnormalState(eAbnormalState _abnormalState)
		{
			if ((unchecked((uint)(@_abnormalState)) == 7u))
			{
				return true;
			}
			else
			{
				if ((unchecked((uint)(@_abnormalState)) == 11u))
				{
					return true;
				}
				else
				{
					return (((unchecked(unchecked((uint)(@_abnormalState)) - 65u) < 2u) ? 1u : 0u) != 0);
				}
			}
		}
	}
}
