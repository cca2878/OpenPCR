using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenPCR.Host;

/// <summary>Explicit resource keys from the host's prepared scene. No battle state or frame0 values are required.</summary>
internal sealed record UnitResourceRequest(string Side, int Slot, int UnitId, int PrefabId, IReadOnlyList<int> SkillIds);
internal sealed record UnitResourceInput(UnitResourceRequest Request, AttackPrefabInput Attack,
	IReadOnlyList<MasterSkillInput> Skills, IReadOnlyList<MasterActionInput> Actions);

internal static class BattleResourceInputs
{
	public static IReadOnlyList<UnitResourceInput> Load(ResourceDatabases resources, IReadOnlyList<UnitResourceRequest> units)
	{
		ArgumentNullException.ThrowIfNull(resources); ArgumentNullException.ThrowIfNull(units);
		if (units.Count == 0) throw new ArgumentException("Supply at least one scene resource request.", nameof(units));
		var result = new List<UnitResourceInput>(); var slots = new HashSet<(string, int)>();
		foreach (var unit in units)
		{
			if (unit is null || unit.Side is not ("player" or "enemy") || unit.Slot < 0 || unit.UnitId <= 0 ||
				unit.PrefabId <= 0 || unit.SkillIds is null || unit.SkillIds.Any(id => id <= 0) || !slots.Add((unit.Side, unit.Slot)))
				throw new ArgumentException("Supply unique scene slots and explicit unit/prefab/skill resource keys.", nameof(units));
			var skills = unit.SkillIds.Select(resources.MasterData.Skill).ToArray();
			var actions = skills.SelectMany(skill => skill.ActionIds).Where(id => id > 0).Distinct().Select(resources.MasterData.Action).ToArray();
			result.Add(new UnitResourceInput(unit with { SkillIds = unit.SkillIds.ToArray() }, resources.Prefabs.LoadAttack(unit.PrefabId), skills, actions));
		}
		return result;
	}
}
