using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenPCR.Host;

/// <summary>Source-independent scene definition. Contains neither raw growth nor an operation timeline.</summary>
public sealed record BattleDefinition(string MasterVersion, int[] Party, BattleSupportIdentity? Support, BattleScene Scene,
	BattleAccountEnvironment Account, BattleSettings Settings, BattleLogging Logging)
{
	internal void Validate(bool preparedLabyrinth = false)
	{
		if (string.IsNullOrEmpty(MasterVersion) || MasterVersion.Length != 12 || MasterVersion.Any(c => c is < '0' or > '9')) throw new InvalidDataException("MasterVersion must be twelve digits.");
		if (Scene is null || Account is null || Account.Values is null || Settings is null || Logging is null) throw new InvalidDataException("Battle input groups cannot be null.");
		if (Party is null || Party.Length != 5 || Party.All(id => id == 0) || Party.Any(id => id < 0) || Party.Where(id => id != 0).Distinct().Count() != Party.Count(id => id != 0)) throw new InvalidDataException("Party requires five slots, one to five distinct units; 0 is empty and 1 selects support.");
		if (Party.Contains(1) != (Support is not null)) throw new InvalidDataException("Support and party marker 1 must be supplied together.");
		if (Support is { } support && (support.OwnerViewerId <= 0 || support.UnitId <= 1 || Party.Contains(support.UnitId))) throw new InvalidDataException("Invalid support identity or duplicate owned/support unit.");
		if (Scene.Category is not (9 or 32) && !(preparedLabyrinth && Scene.Category is 59 or 60) || Scene.EnemyIds is null || Scene.EnemyIds.Length == 0 || Scene.EnemyIds.Any(id => id <= 0)) throw new NotSupportedException("Unsupported category or empty enemy wave; Labyrinth requires explicit prepared mode inputs.");
		if (!float.IsFinite(Scene.TimeLimit) || Scene.TimeLimit <= 0) throw new InvalidDataException("TimeLimit must be a finite positive number of seconds.");
		if (Scene.Backgrounds is null || Scene.Backgrounds.Length != 1 || Scene.Backgrounds[0] <= 0) throw new InvalidDataException("A single wave requires one explicit positive background ID.");
		if (Scene.Category == 9)
		{
			if (Scene.Clan is not { } clan || Scene.Trial is not null || Support is not null) throw new NotSupportedException("Category 9 requires Clan, no Trial and no support (clan support producer is unavailable).");
			if (!Scene.EnemyIds.Contains(clan.EnemyId) || clan.MaxHp <= 0 || clan.CurrentHp < 0 || clan.CurrentHp > clan.MaxHp) throw new InvalidDataException("Invalid clan boss identity, HP or mode flags.");
		}
		else if (Scene.Category == 32 && (Scene.Trial is null || Scene.Clan is not null)) throw new InvalidDataException("Category 32 requires Trial and no Clan.");
		else if (Scene.Category is 59 or 60 && (Scene.Trial is not null || Scene.Clan is not null || Support is not null)) throw new InvalidDataException("Labyrinth requires its own context and no clan/trial/support fields.");
		if (Settings.FrameRate is not (30 or 60) || Settings.ClanRehearsalMaxSpeed is not (null or 2 or 4)) throw new NotSupportedException("Supported FPS is 30/60 and clan rehearsal max speed is 2/4.");
		if (Scene.Category == 9 && Settings.ClanRehearsalMaxSpeed is null) throw new InvalidDataException("Category 9 requires ClanRehearsalMaxSpeed.");
		if (Settings.AutoSpeed is null || Settings.AutoSpeed.ButtonState is not (Elements.SystemSettingDefine.eTypeButtonState.OFF or Elements.SystemSettingDefine.eTypeButtonState.ON)) throw new InvalidDataException("AutoSpeed requires a valid explicit button state.");
		if (Account.Values.Quests is null || Account.Values.Quests.Any(q => q is null) || Account.Values.Quests.Select(q => q.QuestId).Distinct().Count() != Account.Values.Quests.Length) throw new InvalidDataException("Quests must be explicit and unique.");
		if (Logging.AccountSwitches is null || Logging.EffectiveSwitches is null) throw new InvalidDataException("Both account and effective logging switches must be explicit.");
		var logNames = typeof(Elements.BattleLogType).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Select(p => p.Name).ToArray();
		if (Logging.AccountSwitches.Count != logNames.Length || Logging.AccountSwitches.Keys.Except(logNames).Any()) throw new InvalidDataException("Account log switches must be complete and known.");
		var logKeys = Enumerable.Range(1, 14).Concat(new[] { 17, 18 }).ToArray();
		if (logKeys.Except(Logging.EffectiveSwitches.Keys).Any() || Logging.EffectiveSwitches.Keys.Except(logKeys.Append(-1)).Any() || Logging.EffectiveSwitches.GetValueOrDefault(-1)) throw new InvalidDataException("Effective log switches must be complete and known; INVALID_VALUE is false.");
		if (Logging.AccountSwitches.Values.Any(v => v == 1) != Logging.EffectiveSwitches.Values.Any(v => v)) throw new InvalidDataException("Effective log switches change the Native Awake log-enable result.");
		if (Account.TeamLevel <= 0) throw new InvalidDataException("TeamLevel must be positive.");
	}
	internal BattleDefinition Copy() => this with
	{
		Party = Party.ToArray(),
		Scene = Scene with { EnemyIds = Scene.EnemyIds.ToArray(), Backgrounds = Scene.Backgrounds.ToArray() },
		Account = Account with { Values = Account.Values with { Quests = Account.Values.Quests.ToArray() } },
		Logging = Logging with { AccountSwitches = new(Logging.AccountSwitches), EffectiveSwitches = new(Logging.EffectiveSwitches) }
	};
}

public sealed record BattleSupportIdentity(int UnitId, long OwnerViewerId);
/// <summary>Detached account values read during battle; no stories, equipment inventory or calculator.</summary>
public sealed record BattleAccountEnvironment(int TeamLevel, long NowUnixTime, BattleEnvironment Values);

public sealed record BattleScene([property: JsonRequired] int Category, [property: JsonRequired] int[] EnemyIds,
	[property: JsonRequired] float TimeLimit, [property: JsonRequired] int BattleLogId, [property: JsonRequired] int Seed,
	[property: JsonRequired] int[] Backgrounds, [property: JsonRequired] BattleClan? Clan, [property: JsonRequired] BattleTrial? Trial);
public sealed record BattleTrial([property: JsonRequired] int ClanBattleId, [property: JsonRequired] int OrderNum,
	[property: JsonRequired] int Phase, [property: JsonRequired] int InfinityHpModeKey, [property: JsonRequired] int BossResultPosY);
public sealed record BattleClan([property: JsonRequired] int ClanId, [property: JsonRequired] int EnemyId,
	[property: JsonRequired] long MaxHp, [property: JsonRequired] long CurrentHp,
	[property: JsonRequired] bool IsRehearsal, [property: JsonRequired] bool IsTrainingBattle,
	[property: JsonRequired] int BattleId, [property: JsonRequired] int LapNum, [property: JsonRequired] int Phase,
	[property: JsonRequired] int OrderNum, [property: JsonRequired] int Mode, [property: JsonRequired] int TrainingId,
	[property: JsonRequired] bool ActualBossStatus, [property: JsonRequired] int StockIndex, [property: JsonRequired] int TopMode);
public sealed record BattleEnvironment([property: JsonRequired] long ViewerId, [property: JsonRequired] int EnergyRecoveryRateMax,
	[property: JsonRequired] int CurrentClanBattleMode, [property: JsonRequired] long SlipDamageLogBarrierReleaseUnixTime,
	[property: JsonRequired] int MainStoryArrivalNormalQuestId, [property: JsonRequired] BattleQuest[] Quests);
public sealed record BattleQuest([property: JsonRequired] int QuestId, [property: JsonRequired] int ClearFlg,
	[property: JsonRequired] int ResultType, [property: JsonRequired] int DailyClearCount, [property: JsonRequired] int DailyRecoveryCount);
public sealed record BattleSettings([property: JsonRequired] int FrameRate, [property: JsonRequired] bool ApplyOtherUserSkins,
	[property: JsonRequired] bool BossInfinityHp, [property: JsonRequired] bool UseUbSetMode, [property: JsonRequired] bool ResetSkillReserveEveryTime,
	[property: JsonRequired] bool AdvantageAndBreakdownBonusShows, [property: JsonRequired] bool MultiTargetFirst,
	[property: JsonRequired] bool TalentFormationBonusFirst, [property: JsonRequired] int? ClanRehearsalMaxSpeed,
	[property: JsonRequired] BattleAutoSpeed AutoSpeed, bool PlayMultiTargetAppearance = false,
	[property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] bool SpeedQuadruple = false);
public sealed record BattleAutoSpeed([property: JsonRequired] Elements.SystemSettingDefine.eTypeButtonState ButtonState,
	[property: JsonRequired] bool IsAuto, [property: JsonRequired] bool IsAllSet, [property: JsonRequired] bool IsSpeedUp)
{
	internal Elements.AutoSpeedSetting Create() => new() { ButtonState = ButtonState, IsAuto = IsAuto, IsAllSet = IsAllSet, IsSpeedUp = IsSpeedUp };
}
public sealed record BattleLogging([property: JsonRequired] Dictionary<string, long> AccountSwitches,
	[property: JsonRequired] Dictionary<int, bool> EffectiveSwitches, [property: JsonRequired] int BarrierCoefficient, [property: JsonRequired] int BarrierThreshold);
