namespace Elements.Utility
{
	public static class BattleEffectUtility
	{
		private class BattleEffectSettingCache
		{
			public BattleDefine.eDamageNumSizeType BattleSettingDamageNumSizeType;
			public BattleDefine.eDamageNumSizeType BattleSettingHealNumSizeType;
			public BattleDefine.eDamageNumEnableType BattleSettingDamageNumEnableTypeUb;
			public BattleDefine.eDamageNumEnableType BattleSettingDamageNumEnableTypeNormal;
			public BattleDefine.eRecoverEffectEnableType BattleSettingRecoverEffectEnableType;
			public BattleDefine.eBuffArrowEffectEnableType BattleSettingBuffArrowEffectEnableType;

		}

		private static BattleEffectSettingCache effectSettingCache;
		static BattleEffectUtility()
		{
			effectSettingCache = new BattleEffectSettingCache();
		}

		public static bool IsEffectSettingEnable(eBattleCategory _battleCategory)
		{
			return _battleCategory != eBattleCategory.STORY;
		}

		[global::OpenPCR.Migration("21c8215e-fae3-4c29-bbb3-19735b391bc7")]
		public static int GetDamageNumEffectSize(bool _isUb, eBattleCategory _battleCategory)
		{
			if (!IsEffectSettingEnable(_battleCategory))
				return 100;
			BattleDefine.eDamageNumEnableType enable = _isUb ? effectSettingCache.BattleSettingDamageNumEnableTypeUb : effectSettingCache.BattleSettingDamageNumEnableTypeNormal;
			if ((uint)enable > 1u)
				return 0;
			return BattleDefine.DAMAGE_NUM_SIZE_DICTIONARY.TryGetValue(effectSettingCache.BattleSettingDamageNumSizeType, out int value) ? value : 100;
		}

		[global::OpenPCR.Migration("68f3700b-cb56-4f27-a21f-99dc030c4109")]
		public static int GetHealNumEffectSize(bool _isUb, eBattleCategory _battleCategory)
		{
			if (!IsEffectSettingEnable(_battleCategory))
				return 100;
			BattleDefine.eDamageNumEnableType enable = _isUb ? effectSettingCache.BattleSettingDamageNumEnableTypeUb : effectSettingCache.BattleSettingDamageNumEnableTypeNormal;
			if (((int)enable | 2) != 2)
				return 0;
			return BattleDefine.DAMAGE_NUM_SIZE_DICTIONARY.TryGetValue(effectSettingCache.BattleSettingHealNumSizeType, out int value) ? value : 100;
		}

		[global::OpenPCR.Migration("bf8df1b0-671f-40e6-8018-345ab6b05171")]
		public static int GetBuffArrowEffectSize(eBattleCategory _battleCategory)
		{
			if (!IsEffectSettingEnable(_battleCategory))
				return 100;
			return BattleDefine.BUFF_ARROW_EFFECT_SIZE_DICTIONARY.TryGetValue(effectSettingCache.BattleSettingBuffArrowEffectEnableType, out int value) ? value : 100;
		}

		[global::OpenPCR.Migration("13396a7f-2045-4b44-afdb-bf80dc4d505b")]
		public static bool IsRecoverEffectEnable(eBattleCategory _battleCategory)
		{
			return !IsEffectSettingEnable(_battleCategory) || effectSettingCache.BattleSettingRecoverEffectEnableType == BattleDefine.eRecoverEffectEnableType.ON;
		}
	}
}
