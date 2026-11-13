using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Elements;
using OpenPCR;

internal static class SpeedSettingsTests
{
	internal static void Run(Action<bool, string> check)
	{
		var owner = (SaveDataManager)RuntimeHelpers.GetUninitializedObject(typeof(SaveDataManager));
		void Reject(string message)
		{
			bool rejected = false;
			try { owner.IsBattleSpeedQuadruple = true; }
			catch (InvalidOperationException) { rejected = true; }
			check(rejected, message);
		}
		Reject("Unbound speed preference write was accepted.");
		PreparedSettingsInputs.Bind(owner, new Dictionary<string, object>());
		Reject("Missing speed preference gained a default on write.");
		PreparedSettingsInputs.Release(owner);
		PreparedSettingsInputs.Bind(owner, new Dictionary<string, object> { [nameof(SaveDataManager.IsBattleSpeedQuadruple)] = 1 });
		Reject("Wrong speed preference type was accepted.");
		PreparedSettingsInputs.Release(owner);
		PreparedSettingsInputs.Bind(owner, new Dictionary<string, object> { [nameof(SaveDataManager.IsBattleSpeedQuadruple)] = false });
		owner.IsBattleSpeedQuadruple = true;
		check(owner.IsBattleSpeedQuadruple, "Native speed preference write was not visible to its next read.");
		owner.IsBattleSpeedQuadruple = false;
		check(!owner.IsBattleSpeedQuadruple, "Native speed preference could not be cleared.");
		check(PreparedSettingsInputs.SaveCount(owner) == 0, "Speed preference setter added an explicit save absent from Native.");
		PreparedSettingsInputs.Release(owner);
		Reject("Released speed preference owner remained writable.");
	}
}
