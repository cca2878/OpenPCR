namespace Elements.Battle
{
	public interface IBattleManagerForTutorialBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		int CurrentWave { get; }

		int FrameRate { get; }

		float DeltaTime { get; }

		bool IsStartSkillExec { get; }

		void SetStartSkillExec(bool flag);

		UnitCtrl FindUnitForSoundUnitId(int soundUnitId);

		UnitCtrl FindEnemyForResourceId(int resourceId);

		void SetPauseTimeLimit(bool _flag);

		bool IsVoiceDownLoad();
	}
}
