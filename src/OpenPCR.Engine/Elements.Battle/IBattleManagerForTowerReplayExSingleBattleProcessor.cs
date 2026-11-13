namespace Elements.Battle
{
	public interface IBattleManagerForTowerReplayExSingleBattleProcessor : IBattleManagerForGeneralReplayBattleProcessor, IBattleManagerForBaseBattleProcessor
	{
		void OpenTowerExResultDialog(bool _isVictory, int _finalWaveIndex, bool _isFirst);

		void ShowResultDialog();

		void SetBattleResult(eBattleResult result);
	}
}
