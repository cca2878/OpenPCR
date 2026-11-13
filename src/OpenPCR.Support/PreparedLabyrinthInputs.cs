using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OpenPCR;

/// <summary>Detached mode products, fixed before one battle. No account, database or calculator callbacks.</summary>
public sealed class PreparedLabyrinthInputs
{
	public IReadOnlyList<int> ActiveEffectGroups { get; }
	public IReadOnlyList<int> MarkEffectGroups { get; }
	public IReadOnlyList<int> DifficultyEnemies { get; }
	public IReadOnlyList<int> DifficultyTimings { get; }
	private readonly IReadOnlyDictionary<int, int> targetValues;
	public int Difficulty { get; }
	public PreparedLabyrinthInputs(int difficulty, IEnumerable<int> difficultyEnemies, IEnumerable<int> difficultyTimings,
		IEnumerable<int> activeEffectGroups, IEnumerable<int> markEffectGroups, IReadOnlyDictionary<int, int> targetValues)
	{
		ArgumentNullException.ThrowIfNull(difficultyEnemies); ArgumentNullException.ThrowIfNull(difficultyTimings);
		ArgumentNullException.ThrowIfNull(activeEffectGroups); ArgumentNullException.ThrowIfNull(markEffectGroups); ArgumentNullException.ThrowIfNull(targetValues);
		Difficulty = difficulty;
		DifficultyEnemies = Array.AsReadOnly(difficultyEnemies.ToArray()); DifficultyTimings = Array.AsReadOnly(difficultyTimings.ToArray());
		ActiveEffectGroups = Array.AsReadOnly(activeEffectGroups.ToArray()); MarkEffectGroups = Array.AsReadOnly(markEffectGroups.ToArray());
		this.targetValues = new ReadOnlyDictionary<int, int>(new Dictionary<int, int>(targetValues));
		if (difficulty <= 0 || DifficultyEnemies.Any(id => id <= 0) || DifficultyEnemies.Count != DifficultyTimings.Count || ActiveEffectGroups.Concat(MarkEffectGroups).Any(id => id <= 0))
			throw new ArgumentException("Invalid Labyrinth product identity or aligned timing list.");
	}
	public int TargetValue(int type) => targetValues.TryGetValue(type, out int value) ? value : throw new InvalidOperationException("No prepared Labyrinth target value: " + type);
	private static readonly ConditionalWeakTable<object, PreparedLabyrinthInputs> bindings = new();
	public static void Bind(object receiver, PreparedLabyrinthInputs products) { ArgumentNullException.ThrowIfNull(receiver); ArgumentNullException.ThrowIfNull(products); bindings.Add(receiver, products); }
	public static PreparedLabyrinthInputs Require(object receiver) => bindings.TryGetValue(receiver, out var value) ? value : throw new InvalidOperationException("Labyrinth mode products are unbound.");
	public static void Release(object receiver) => bindings.Remove(receiver);
}
