using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using Elements;
using Elements.Battle;

namespace OpenPCR.Host;

internal sealed partial record BattleEnvironmentInputs(int TeamLevel, long ViewerId, int EnergyRecoveryRateMax, int? CurrentClanBattleMode)
{
	public UserData BindAccount()
	{
		if (Singleton<UserData>.Instance is not null) throw new InvalidOperationException("A user-data input is already active.");
		var account = (UserData)RuntimeHelpers.GetUninitializedObject(typeof(UserData)); var info = new UserInfo();
		RuntimeBinding.Set(info, "ViewerId", (global::System.Int64)ViewerId);
		RuntimeBinding.Set(info, "TeamLevel", (global::System.Int32)TeamLevel);
		RuntimeBinding.Set(account, "UserInfo", info);
		// Legacy prepared battle consumers do not read the raw account clan mode.
		// Absence stays absent from the producer; no captured mode is invented.
		if (CurrentClanBattleMode is { } mode) RuntimeBinding.Set(account, "CurrentClanBattleMode", mode);
		account.EnergyRecoveryRateMax = EnergyRecoveryRateMax;
		RuntimeBinding.Instance(typeof(Singleton<>), typeof(UserData), account); return account;
	}
	public void BindManager(BattleManager manager, ResourceDatabases resources)
	{
		using var command = resources.Prefab.CreateCommand(); command.CommandText = "SELECT field,value FROM battle_manager_field";
		using var reader = command.ExecuteReader();
		while (reader.Read())
		{
			string name = reader.GetString(0); var bytes = (byte[])reader[1];
			if (bytes.Length != 8) throw new InvalidDataException("Battle manager scalar must be float64: " + name);
			var field = manager.GetType().GetField(name, RuntimeBinding.Members);
			var property = manager.GetType().GetProperty(name, RuntimeBinding.Members);
			var type = field?.FieldType ?? property?.PropertyType ?? throw new MissingMemberException(manager.GetType().FullName, name);
			RuntimeBinding.Set(manager, name, RuntimeMasterData.Wrap(BitConverter.ToDouble(bytes), type));
		}
	}

	internal static void BindAccountBattleInputs(UserData account, BattleDefinition definition, int initialFrameCount)
	{
		var input = definition.Account.Values;
		RuntimeBinding.Set(account, "nowUnixTime", definition.Account.NowUnixTime);
		// Bind the supplied clock origin, including a nonzero frameCount.
		RuntimeBinding.Set(account, "currentFrame", initialFrameCount);
		RuntimeBinding.Set(account, "slipDamageLogBarrierReleaseUnixTime", input.SlipDamageLogBarrierReleaseUnixTime);
		RuntimeBinding.Set(account, "MainStoryArrivalNormalQuestId", input.MainStoryArrivalNormalQuestId);
		var quests = new System.Collections.Generic.Dictionary<int, UserQuestInfo>();
		foreach (var row in input.Quests) quests.Add(row.QuestId, new UserQuestInfo(row.QuestId, row.ClearFlg, row.ResultType, row.DailyClearCount, row.DailyRecoveryCount));
		RuntimeBinding.Set(account, "QuestMapParameterDictionary", quests);
		RuntimeBinding.Set(account, "QuestStageParameter", new QuestStageParameter());
		var settings = definition.Logging;
		var target = (InitSettingParameter)RuntimeHelpers.GetUninitializedObject(typeof(InitSettingParameter));
		var logType = (BattleLogType)RuntimeHelpers.GetUninitializedObject(typeof(BattleLogType));
		var names = typeof(BattleLogType).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
		if (settings.AccountSwitches.Count != names.Length) throw new InvalidDataException("Incomplete account log switches.");
		foreach (var value in settings.AccountSwitches)
		{
			var property = typeof(BattleLogType).GetProperty(value.Key) ?? throw new InvalidDataException("Unknown battle log setting: " + value.Key);
			RuntimeBinding.Set(logType, value.Key, RuntimeMasterData.Wrap(value.Value, property.PropertyType));
		}
		target.BattleLogType = logType;
		var barrier = (LogBarrierIniSetting)RuntimeHelpers.GetUninitializedObject(typeof(LogBarrierIniSetting));
		RuntimeBinding.Set(barrier, "Coefficient", (global::System.Int32)settings.BarrierCoefficient);
		RuntimeBinding.Set(barrier, "Threshold", (global::System.Int32)settings.BarrierThreshold);
		target.LogBarrierIniSetting = barrier;
		RuntimeBinding.Set(account, "InitSettingParameter", target);
	}
}
