using System;
using System.IO;
using System.Text.Json;
using Elements;
using OpenPCR.Host;

internal static class MultiTargetAppearanceTests
{
	internal static void Run(Action<bool, string> check)
	{
		var settings = new BattleSettings(30, false, false, true, false, false, false, false, 4,
			new(SystemSettingDefine.eTypeButtonState.OFF, false, false, false));
		check(!settings.PlayMultiTargetAppearance, "Existing scene constructors must retain skip as their default.");
		foreach (bool play in new[] { false, true })
			foreach (bool first in new[] { false, true })
			{
				var input = settings with { PlayMultiTargetAppearance = play, MultiTargetFirst = first };
				check(JsonSerializer.Deserialize<BattleSettings>(JsonSerializer.Serialize(input)) == input, "Effective appearance choice did not round-trip.");
				using var binding = new BattleSettingsBinding(input, 0);
				check(binding.Manager.MultiTargetConfirm == (play ? 0 : 1), "Host resolved a daily preference instead of using the explicit choice.");
				binding.Manager.MultiTargetFirst = !first;
				check(binding.Manager.MultiTargetConfirm == (play ? 0 : 1), "Client played-marker output changed the effective choice.");
			}
		check(ManagerSingleton<SaveDataManager>.Instance is null, "Appearance settings leaked across sessions.");
	}
}
