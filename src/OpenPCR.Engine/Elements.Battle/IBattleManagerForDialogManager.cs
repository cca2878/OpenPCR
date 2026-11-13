namespace Elements.Battle
{
	public interface IBattleManagerForDialogManager
	{
		eBattleCategory BattleCategory { get; }

		UnitUiCtrl UnitUiCtrl { get; }

		bool CanReserveSkill { get; }

		bool CanQuadruple { get; }

		bool IsPrincessSkillRunning { get; }

		BattleDefineData BattleDefineData { get; }

		bool HasWarnedSuspendAbuse { get; set; }

		void DisableAutoClear();

		bool IsShowEffectiveIcon(int _unitId);

		bool NeedsSuspendAbuseWarning();
	}
}
