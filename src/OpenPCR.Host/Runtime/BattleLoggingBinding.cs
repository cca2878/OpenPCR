using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elements;
using Elements.Battle;

namespace OpenPCR.Host;

/// <summary>The captured effective log switches, separate from the account's server defaults.</summary>
internal sealed class BattleLoggingBinding : IDisposable
{
	private readonly Dictionary<eBattleLogType, bool> settings;
	private readonly Dictionary<eBattleLogType, bool> previous;
	public bool Enabled => settings.Values.Any(value => value);
	internal BattleLoggingBinding(BattleLogging input)
	{
		settings = (Dictionary<eBattleLogType, bool>)RuntimeBinding.Get(typeof(BattleLog), "BATTLE_LOG_SETTING")!;
		previous = new(settings);
		var prepared = new Dictionary<eBattleLogType, bool>();
		foreach (var property in input.EffectiveSwitches)
		{
			if (!settings.ContainsKey((eBattleLogType)property.Key))
				throw new InvalidDataException("Unknown captured log switch: " + property.Key);
			prepared.Add((eBattleLogType)property.Key, property.Value);
		}
		// Native initializes INVALID_VALUE to false; it has no server/capture switch.
		prepared.TryAdd(eBattleLogType.INVALID_VALUE, false);
		if (prepared.Count != settings.Count) throw new InvalidDataException("Incomplete captured effective log switches.");
		foreach (var pair in prepared) settings[pair.Key] = pair.Value;
	}
	public void Dispose() { foreach (var pair in previous) settings[pair.Key] = pair.Value; }
}
