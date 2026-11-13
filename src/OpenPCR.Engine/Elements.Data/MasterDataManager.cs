namespace Elements.Data
{
	public class MasterDataManager : ManagerSingleton<MasterDataManager>
	{
		public MasterEReduction masterEReduction { get; private set; }

		public MasterLabyrinthRelicEffectGroup masterLabyrinthRelicEffectGroup { get; private set; }
		public MasterLabyrinthRelicEffect masterLabyrinthRelicEffect { get; private set; }
		public MasterRelicSkillExecTime masterRelicSkillExecTime { get; private set; }
		public MasterRelicActionExecTime masterRelicActionExecTime { get; private set; }

		public global::Elements.Data.MasterLabyrinthSetting masterLabyrinthSetting { get; private set; }


		public global::Elements.Data.MasterTalentFormationBonus masterTalentFormationBonus { get; private set; }

		public global::Elements.Data.MasterTalentFormationOverwrite masterTalentFormationOverwrite { get; private set; }


		public global::Elements.Data.MasterClanBattle2MapData masterClanBattle2MapData { get; private set; }

		public global::Elements.Data.MasterClanBattleSMapData masterClanBattleSMapData { get; private set; }

		public global::Elements.Data.MasterBywayBattleQuestData masterBywayBattleQuestData { get; private set; }


		public global::Elements.Data.MasterEnemyEnableVoice masterEnemyEnableVoice { get; private set; }

		public global::Elements.Data.MasterUnitTalent masterUnitTalent { get; private set; }

		public global::Elements.Data.MasterUnitRoleData masterUnitRoleData { get; private set; }

		public global::Elements.Data.MasterSpskillLvInitializeData masterSpskillLvInitializeData { get; private set; }

		public global::Elements.Data.MasterExEquipmentData masterExEquipmentData { get; private set; }

		public global::Elements.Data.MasterPositionSetting masterPositionSetting { get; private set; }



		public global::Elements.Data.MasterContentReleaseData masterContentReleaseData { get; private set; }


		public global::Elements.Data.MasterItemData masterItemData { get; private set; }

		public global::Elements.Data.MasterRarity6QuestData masterRarity6QuestData { get; private set; }



		public MasterUnlockSkillData masterUnlockSkillData { get; private set; }

		public MasterUnitConversion masterUnitConversion { get; private set; }

		public global::Elements.Data.MasterSpBattleVoice masterSpBattleVoice { get; private set; }


		public global::Elements.Data.MasterEnemyMParts masterEnemyMParts { get; private set; }

		public global::Elements.Data.MasterUnitData masterUnitData { get; private set; }

		public global::Elements.Data.MasterUnitEnemyData masterUnitEnemyData { get; private set; }

		public global::Elements.Data.MasterUnitPosAdjustment masterUnitPosAdjustment { get; private set; }


		public MasterUnitAttackPattern masterUnitAttackPattern { get; private set; }

		public global::Elements.Data.MasterTalent masterTalent { get; private set; }

		public global::Elements.Data.MasterTalentWeakness masterTalentWeakness { get; private set; }


		public global::Elements.Data.MasterEnemyParameter masterEnemyParameter { get; private set; }

		public global::Elements.Data.MasterResistData masterResistData { get; private set; }

		public global::Elements.Data.MasterAilmentData masterAilmentData { get; private set; }

		public global::Elements.Data.MasterResistVariationData masterResistVariationData { get; private set; }

		public global::Elements.Data.MasterEnemyIgnoreSkillRf masterEnemyIgnoreSkillRf { get; private set; }

		public global::Elements.Data.MasterSkillAction masterSkillAction { get; private set; }

		public global::Elements.Data.MasterSkillData masterSkillData { get; private set; }

		public global::Elements.Data.MasterUnitSkillDataRf masterUnitSkillDataRf { get; private set; }

		public global::Elements.Data.MasterHpDrainAt masterHpDrainAt { get; private set; }

		public global::Elements.Data.MasterTpRecoveryAt masterTpRecoveryAt { get; private set; }

		public global::Elements.Data.MasterUnitSkillData masterUnitSkillData { get; private set; }

		public global::Elements.Data.MasterLabyrinthSkillActionReplace masterLabyrinthSkillActionReplace { get; private set; }

		public MasterDodgeTpRecovery masterDodgeTpRecovery { get; private set; }
		public MasterEnemyTalentWeakness masterEnemyTalentWeakness { get; private set; }
		public MasterExtraEffectData masterExtraEffectData { get; private set; }
		public MasterExceptEr masterExceptEr { get; private set; }

		public MasterDataManager()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.Data.MasterDataManager::.ctor()", "full resource/database initialization outside this declaration dependency pass");
		}

		protected override void onInit()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.Data.MasterDataManager::onInit():System.Void", "full resource/database initialization outside this declaration dependency pass");
		}
	}
}
