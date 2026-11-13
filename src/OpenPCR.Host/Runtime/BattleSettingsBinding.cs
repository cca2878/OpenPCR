using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Elements;

namespace OpenPCR.Host;

internal sealed class BattleSettingsBinding : IDisposable
{
	internal SaveDataManager Manager { get; }
	internal BattleSettingsBinding(BattleSettings input, int infinityHpModeKey)
	{
		if (ManagerSingleton<SaveDataManager>.Instance is not null) throw new InvalidOperationException("Local settings are already active.");
		var values = new Dictionary<string, object>(StringComparer.Ordinal)
		{
			["ApplySkinSettingsFromOtherUsers"] = input.ApplyOtherUserSkins,
			["UseUbSetMode"] = input.UseUbSetMode,
			["ResetSkillReserveEveryTime"] = input.ResetSkillReserveEveryTime,
			["TalentFormationBonusConfirm"] = 1,
			["AdvantageAndBreakdownBonusShows"] = input.AdvantageAndBreakdownBonusShows,
			["MultiTargetFirst"] = input.MultiTargetFirst,
			// The caller supplies the effective choice; no daily preference or clock is evaluated here.
			["MultiTargetConfirm"] = input.PlayMultiTargetAppearance ? 0 : 1,
			["TalentFormationBonusFirst"] = input.TalentFormationBonusFirst,
			// Device battle frame rate preference. BattleManager.Awake derives each battle's FrameRate from it with
			// GetFrameRate: 30 for quadruple speed when allowed, otherwise this preference.
			["IsBattleSpeedQuadruple"] = input.SpeedQuadruple,
			["BattleFrameRate"] = input.FrameRate,
			["TrialBattleInfinityHpActiveQuestIds"] = input.BossInfinityHp ? new List<int> { infinityHpModeKey } : new List<int>()
		};
		// Uncaptured device setting; BattleManager.Awake reads it for clan battles. Absent means not supplied.
		if (input.ClanRehearsalMaxSpeed is { } maxSpeed) values["ClanReharsalBattleMaxSpeed"] = maxSpeed;
		// Uncaptured device setting read by UnitUiCtrl.Initialize (restore auto/all-set/speed). Absent means not supplied.
		values["AutoSpeedSetting"] = input.AutoSpeed.Create();
		Manager = Bind(values, ePlaybackType.NONE);
	}
	internal BattleSettingsBinding(Dictionary<string, object> values, ePlaybackType cutIn) => Manager = Bind(values, cutIn);
	private static SaveDataManager Bind(Dictionary<string, object> values, ePlaybackType cutIn)
	{
		if (ManagerSingleton<SaveDataManager>.Instance is not null) throw new InvalidOperationException("Local settings are already active.");
		var manager = (SaveDataManager)RuntimeHelpers.GetUninitializedObject(typeof(SaveDataManager));
		RuntimeBinding.Set(manager, "cutInData", new CutInData { PlaybackType = cutIn });
		RuntimeBinding.Set(manager, "battleFrameRate", (int)values["BattleFrameRate"]);
		PreparedSettingsInputs.Bind(manager, values);
		RuntimeBinding.Instance(typeof(ManagerSingleton<>), typeof(SaveDataManager), manager);
		return manager;
	}
	public void Dispose()
	{
		PreparedSettingsInputs.Release(Manager);
		if (ReferenceEquals(ManagerSingleton<SaveDataManager>.Instance, Manager)) Manager.DestroyInstance();
	}
}
