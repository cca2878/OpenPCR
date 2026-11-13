using global::OpenPCR.Host;
using global::OpenPCR.Runtime.Impl;
#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using OpenPCR.Checkpoint;
using Elements;
using Elements.Battle;

namespace OpenPCR.Host;

public sealed partial class BattleSession
{
	private CheckpointContext checkpoints;
	private readonly List<ICheckpointParticipant> checkpointParticipants = new();
	/// <summary>Register verification/output state before the first capture. External I/O is never rewound.</summary>
	public void AddCheckpointParticipant(ICheckpointParticipant participant)
	{
		RequireAvailable(); ArgumentNullException.ThrowIfNull(participant);
		if (checkpoints != null) throw new InvalidOperationException("Checkpoint participants are frozen after the first capture.");
		checkpointParticipants.Add(participant);
	}
	public OpenPCR.Checkpoint.Checkpoint Capture()
	{
		RequireAvailable(); busy = true;
		try { checkpoints ??= CreateCheckpointContext(); return checkpoints.Capture(); }
		finally { busy = false; }
	}
	public void Restore(OpenPCR.Checkpoint.Checkpoint checkpoint)
	{
		RequireAvailable();
		if (checkpoints == null) throw new ArgumentException("This session has no checkpoints.", nameof(checkpoint));
		checkpoints.Validate(checkpoint); // Invalid handles must not fault a healthy session.
		busy = true;
		try
		{
			// Detach wrappers from the abandoned future. The memento rebinds its original live wrappers.
			SceneScripting.EndSession();
			checkpoints.Restore(checkpoint);
			ClearUbAnchor();
			if (state.UbExploration) SetUbAnchor(checkpoints.Retain(checkpoint));
		}
		catch { faulted = true; throw; }
		finally { busy = false; }
	}
	private CheckpointContext CreateCheckpointContext()
	{
		var context = new CheckpointContext(
			shareType: t => typeof(JsonDocument).IsAssignableFrom(t) || typeof(Microsoft.Data.Sqlite.SqliteConnection).IsAssignableFrom(t) || CheckpointSharing.IsPreparedReadOnly(t),
			supportedType: t => t.Assembly.GetName().Name?.StartsWith("OpenPCR.", StringComparison.Ordinal) == true ||
				t.Assembly == typeof(BattleManager).Assembly || t.Namespace?.StartsWith("System.Collections", StringComparison.Ordinal) == true ||
				t.Namespace?.StartsWith("System.Linq", StringComparison.Ordinal) == true || t.Namespace == "System" || t.Namespace == "System.Text.Json", weakTableKeysAreRoots: false);
		context.KeepConcurrentMapMembership(RuntimeBinding.Get(typeof(CurveScripting), "curves"));
		context.Share(this);
		context.Share(ubBridge);
		// Session-owned, stateless output receivers. Their presentation-only methods
		// never mutate a battle value; reflecting audio fields would initialize unrelated device tables.
		if (soundOwner is not null) context.Share(soundOwner);
		if (blockLayerOwner is not null) context.Share(blockLayerOwner);
		if (progress != null) context.Share(progress);
		context.AddRoot("session.state", state);
		context.AddRoot("session.step", stepFrame);
		context.AddRoot("session.result", readResult);
		context.AddRoot("session.destruction", finishDestruction);
		foreach (var field in CheckpointStatics.Fields()) context.AddStatic(field);
		foreach (var participant in checkpointParticipants) context.AddParticipant(participant);
		return context;
	}
}

/// <summary>Explicit static storage boundary. Closed singleton bases are included, not just open definitions.</summary>
internal static class CheckpointStatics
{
	private const string ExpectedLayout = "0132C016100673BD1F998068CEDD4CD5CACEE039507849FF28ADCB3F1ED2ADE5";
	internal static string LayoutFingerprint => Fingerprint(Candidates());
	private static string Fingerprint(IEnumerable<FieldInfo> fields) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(string.Join("\n", fields.Select(f => $"{f.DeclaringType!.FullName}|{f.Name}|{f.FieldType}|{Policy(f)}")))));
	internal static void ValidateInventory(IEnumerable<FieldInfo> fields)
	{
		if (Fingerprint(fields) != ExpectedLayout) throw new InvalidOperationException("Checkpoint static layout changed. Classify the new storage and review the inventory before updating its fingerprint.");
	}
	internal static IEnumerable<FieldInfo> Fields()
	{
		ValidateInventory(Candidates());
		return Candidates().Where(f => Policy(f) == "snapshot");
	}
	internal static IEnumerable<FieldInfo> Candidates()
	{
		var engine = typeof(BattleManager).Assembly;
		var runtime = typeof(ScriptingTime).Assembly;
		var support = typeof(PreparedBattleResultOutput).Assembly;
		var types = new HashSet<Type>();
		foreach (var assembly in new[] { engine, runtime, support, typeof(BattleSession).Assembly, typeof(PreparedParameterProducts).Assembly })
			foreach (var type in assembly.GetTypes())
				for (Type current = type; current != null; current = current.BaseType)
					if (!current.ContainsGenericParameters && current.Assembly == assembly) types.Add(current);
		types.Add(typeof(AnimKitInputs));
		foreach (var type in types.OrderBy(t => t.FullName, StringComparer.Ordinal))
		{
			if (type.IsDefined(typeof(CompilerGeneratedAttribute), false)) continue; // Static, noncapturing delegate caches.
			foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).OrderBy(f => f.Name, StringComparer.Ordinal))
			{
				if (field.IsLiteral) continue;
				yield return field;
			}
		}
	}
	internal static string Policy(FieldInfo field)
	{
		if (field.DeclaringType.Namespace == "Pcr.Prefab") return "shared-protobuf-schema";
		if (Shared(field)) return "shared-immutable-or-observer";
		if (field.DeclaringType == typeof(PartsBattleSkipAnimation) || field.DeclaringType == typeof(SoundManager)) return "outside-muted-host";
		if (field.DeclaringType == typeof(BattleSession)) return "session-ownership";
		if (field.DeclaringType == typeof(RuntimeMasterData) || field.DeclaringType == typeof(OpenPCR.RuntimeBinding)) return "shared-input-or-reflection-cache";
		if (field.DeclaringType == typeof(SharedBoxes)) return "shared-input-or-reflection-cache"; // Immutable boxed masterdata cells.
		if (field.FieldType == typeof(JsonSerializerOptions)) return "shared-input-or-reflection-cache"; // Locked after first use; holds converter metadata only.
		if (field.DeclaringType == typeof(PrefabInputs) && field.Name == "logicBoneSets") return "shared-input-or-reflection-cache"; // Frozen per database file.
		return "snapshot";
	}
	private static bool Shared(FieldInfo field)
	{
		if (field.DeclaringType == typeof(EngineObjectIdentity)) return true; // Compiled wrapper accessors, no scene state.
		if (field.DeclaringType == typeof(SceneScripting) && field.Name == "baseTypes") return true;
		if (field.DeclaringType == typeof(CurveScripting) && field.Name == "nextHandle") return true; // Never reuse finalized handles.
		if (field.DeclaringType == typeof(ScriptingRandom) && field.Name == "Observer") return true;
		if (typeof(MemberInfo).IsAssignableFrom(field.FieldType) || field.FieldType == typeof(Type)) return true;
		return field.IsInitOnly && (field.FieldType.IsPrimitive || field.FieldType.IsEnum || field.FieldType == typeof(string));
	}
}
