using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elements;
using Elements.Data;

namespace OpenPCR.Host;

/// <summary>Explicit previous results and enemy continuation for one EX party. Every bind creates independent Native state.</summary>
public sealed class PreparedLabyrinthExContext
{
	public int PartyIndex { get; }
	public IReadOnlyList<IReadOnlyList<int>> Parties { get; }
	private readonly TowerUnit[] enemies;
	private readonly DungeonQueryUnit[] dead;
	private readonly TowerWaveResultInfo[] waves;
	private readonly Dictionary<int, LabyrinthUnitResultInfo>[] results;
	private readonly eBattleClearType[] clearTypes;
	private readonly ContinuousUnit[][]? continuous;
	internal bool MatchesEnemyWave(IEnumerable<int> ids) => enemies.Select(row => row.UnitId).SequenceEqual(ids);
	public PreparedLabyrinthExContext(int partyIndex, IEnumerable<IEnumerable<int>> parties, IEnumerable<TowerUnit> enemies,
		IEnumerable<DungeonQueryUnit> dead, IEnumerable<TowerWaveResultInfo> waves, IEnumerable<IReadOnlyDictionary<int, LabyrinthUnitResultInfo>> results,
		IEnumerable<eBattleClearType> clearTypes, IEnumerable<IEnumerable<ContinuousUnit>>? continuous)
	{
		ArgumentNullException.ThrowIfNull(parties); ArgumentNullException.ThrowIfNull(enemies); ArgumentNullException.ThrowIfNull(dead);
		ArgumentNullException.ThrowIfNull(waves); ArgumentNullException.ThrowIfNull(results); ArgumentNullException.ThrowIfNull(clearTypes);
		if (partyIndex is < 0 or > 2) throw new ArgumentOutOfRangeException(nameof(partyIndex));
		PartyIndex = partyIndex;
		Parties = Array.AsReadOnly(parties.Select(row => (IReadOnlyList<int>)Array.AsReadOnly(row.ToArray())).ToArray());
		if (Parties.Count != 3 || Parties.Any(row => row.Count != 5 || row.Any(id => id < 0 || id == 1)) || Parties[partyIndex].All(id => id == 0))
			throw new InvalidDataException("EX requires three explicit five-slot parties and a nonempty selected party without support.");
		this.enemies = enemies.Select(Clone).ToArray(); this.dead = dead.Select(Clone).ToArray(); this.waves = waves.Select(Clone).ToArray();
		this.results = results.Select(row => row.ToDictionary(pair => pair.Key, pair => new LabyrinthUnitResultInfo(pair.Value.Hp, pair.Value.MaxHp, pair.Value.Energy))).ToArray();
		this.clearTypes = clearTypes.ToArray();
		this.continuous = continuous?.Select(row => row.Select(Clone).ToArray()).ToArray();
		if (this.enemies.Length == 0 || this.enemies.Any(row => row.UnitId <= 0) || this.enemies.Select(row => row.UnitId).Distinct().Count() != this.enemies.Length)
			throw new InvalidDataException("Explicit unique EX enemy records are required.");
		if (this.waves.Length != partyIndex || this.results.Length != partyIndex || this.clearTypes.Length != partyIndex ||
			this.waves.Where((row, index) => row.wave_num != index + 1).Any() || this.clearTypes.Any(value => value is not (eBattleClearType.MANUAL or eBattleClearType.AUTO_ONLY or eBattleClearType.ALL_SET_ONLY)))
			throw new InvalidDataException("EX prior wave, own-result and clear-mode lists must cover every previous party.");
		if (partyIndex > 0 && this.continuous is null || this.continuous is not null && (this.continuous.Length != 1 || !this.continuous[0].Select(row => row.UnitId).SequenceEqual(this.enemies.Select(row => row.UnitId))))
			throw new InvalidDataException("EX enemy continuation must explicitly preserve the single enemy-wave order.");
	}
	internal void Bind(LabyrinthTempData data, TempData temp, UserData account)
	{
		data.CurrentExPartyIndex = PartyIndex;
		data.ExEnemyUnitList = enemies.Select(Clone).ToList(); data.AlreadyDeadExEnemyList = dead.Select(Clone).ToList();
		data.ExWaveResultInfoList = waves.Select(Clone).ToList();
		data.UnitResultInfoDictionaryList = results.Select(row => row.ToDictionary(pair => pair.Key, pair => new LabyrinthUnitResultInfo(pair.Value.Hp, pair.Value.MaxHp, pair.Value.Energy))).ToList();
		RuntimeBinding.Set(data, "ExWaveClearTypeList", clearTypes.ToList());
		temp.BattleStageData.CurrentContinuousUnit = continuous?.Select(row => row.Select(Clone).ToList()).ToList();
		RuntimeBinding.Set(account, "partyDictionary", Parties.Select((party, index) => (party, index)).ToDictionary(row => (ePartyType)(132 + row.index), row => row.party.ToArray()));
		// These UI history containers are fresh per invocation and never feed battle or finish-request inputs.
		temp.UnitDamageHistoryList = new(); temp.EnemyDamageHistoryList = new(); temp.EnemyColorHistoryList = new();
	}
	internal static List<SkillLimitCounter> Clone(IEnumerable<SkillLimitCounter> source) => source.Select(row => new SkillLimitCounter(row.skill_id, row.counter)).ToList();
	internal static ContinuousUnit Clone(ContinuousUnit row) => new(row.UnitId, row.IdentifyNum, row.Hp, row.Energy, Clone(row.SkillLimitCounter));
	internal static TowerUnit Clone(TowerUnit row)
	{
		ArgumentNullException.ThrowIfNull(row);
		var copy = new TowerUnit();
		copy.SetUnitId(row.UnitId); copy.SetIdentifyNum(row.IdentifyNum); copy.SetHp(row.Hp); copy.SetEnergy(row.Energy);
		copy.SetSkillLimitCounter(Clone(row.SkillLimitCounter)); copy.SetMaxHp(row.MaxHp); copy.SetSavedHp(row.SavedHp);
		copy.SetLevel(row.Level); copy.SetRarity(row.Rarity); copy.SetPromotionLevel(row.PromotionLevel); copy.SetEnemyColor(row.EnemyColor);
		if (row.SkinData is { } skin)
		{
			var target = new SkinData();
			RuntimeBinding.Set(target, "IconSkinId", skin.IconSkinId); RuntimeBinding.Set(target, "SdSkinId", skin.SdSkinId);
			RuntimeBinding.Set(target, "StillSkinId", skin.StillSkinId); RuntimeBinding.Set(target, "MotionId", skin.MotionId);
			RuntimeBinding.Set(copy, "SkinData", target);
		}
		return copy;
	}
	internal static DungeonQueryUnit Clone(DungeonQueryUnit row)
	{
		ArgumentNullException.ThrowIfNull(row);
		var parts = row.parts_list.Select(part => { var copy = new PartsInfo(); RuntimeBinding.Set(copy, "parts_id", part.parts_id); RuntimeBinding.Set(copy, "hp", part.hp); return copy; }).ToArray();
		return new(row.owner_viewer_id, row.unit_id, row.retired, row.hp, row.energy, Clone(row.skill_limit_counter).ToArray(), row.damage, parts);
	}
	internal static TowerWaveResultInfo Clone(TowerWaveResultInfo row)
	{
		ArgumentNullException.ThrowIfNull(row);
		return new(row.wave_num, row.unit_info_list.Select(unit => new TowerWaveResultUnitInfo(unit.unit_id, unit.owner_viewer_id, unit.damage, unit.is_alive)).ToArray(), row.remain_time);
	}
}

public sealed record LabyrinthExFinishRequest(int QuestId, TowerWaveResultInfo[] Waves, DungeonQueryUnit[] Enemies, int AutoClear);
public sealed record LabyrinthExUnitResult(long Hp, long MaxHp, double Energy);
public sealed record LabyrinthExContinuousUnit(int UnitId, int IdentifyNum, long Hp, int Energy, SkillLimitCounter[] SkillLimitCounter);
public sealed record LabyrinthExBattleSettlement(string Schema, int PartyIndex, bool IsFinalTeam, LabyrinthExFinishRequest? FinishRequest,
	TowerUnit[] Enemies, DungeonQueryUnit[] AlreadyDeadEnemies, TowerWaveResultInfo[] Waves, IReadOnlyDictionary<int, LabyrinthExUnitResult>[] UnitResults,
	eBattleClearType[] ClearTypes, LabyrinthExContinuousUnit[][]? EnemyContinuation);
