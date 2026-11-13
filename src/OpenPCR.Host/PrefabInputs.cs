using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elements;
using Microsoft.Data.Sqlite;
using Pcr.Prefab;

namespace OpenPCR.Host;

/// <summary>Serialized attack inputs, before game code expands/sorts combo execution times.</summary>
internal sealed record AttackPrefabInput(int PrefabId, bool UseDefaultDelay, ActionParameterOnPrefabDetail Detail)
{
	public void Bind(UnitActionController target)
	{
		ArgumentNullException.ThrowIfNull(target);
		target.AttackDetail = Detail; target.UseDefaultDelay = UseDefaultDelay;
	}
}

/// <summary>One protobuf-tree query per prefab. Cache serialized bytes; each LoadAttack gets fresh mutable objects.</summary>
internal sealed class PrefabInputs
{
	private readonly SqliteConnection database;
	private readonly Dictionary<int, byte[]> trees = new();
	// Immutable per deployed database file; battle sessions of one process share it.
	private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, System.Collections.Frozen.FrozenSet<string>> logicBoneSets = new();
	internal PrefabInputs(SqliteConnection database) { this.database = database; }
	/// <summary>
	/// Every authored bone name a battle reader can receive: the prefab trees' <c>*bone_name</c> fields and the effect
	/// components' <c>BoneName</c> values. The supported battle readers determine this set.
	/// </summary>
	public IReadOnlySet<string> LogicBoneNames
	{
		get
		{
			var file = new FileInfo(database.DataSource);
			if (!file.Exists) throw new InvalidDataException("Prefab database file is required for its logic bone names: " + database.DataSource);
			return logicBoneSets.GetOrAdd($"{file.FullName}|{file.Length}|{file.LastWriteTimeUtc.Ticks}", _ => ReadLogicBoneNames());
		}
	}
	private System.Collections.Frozen.FrozenSet<string> ReadLogicBoneNames()
	{
		var names = new HashSet<string>(StringComparer.Ordinal);
		using (var command = database.CreateCommand())
		{
			command.CommandText = "SELECT data FROM prefab_tree";
			using var reader = command.ExecuteReader(); var carriers = BoneNameCarriers(UnitPrefab.Descriptor);
			while (reader.Read()) AddBoneNames(UnitPrefab.Parser.ParseFrom((byte[])reader.GetValue(0)), names, carriers);
		}
		using (var command = database.CreateCommand())
		{
			command.CommandText = "SELECT DISTINCT j.value FROM skill_effect_component_data, json_tree(data) j WHERE j.type='text' AND j.key LIKE '%BoneName%'";
			using var reader = command.ExecuteReader();
			while (reader.Read()) names.Add(reader.GetString(0));
		}
		names.Remove(""); // Unset proto3 strings; readers treat an empty name as no bone.
		if (names.Count == 0) throw new InvalidDataException("Authored logic bone names have a zero denominator.");
		return System.Collections.Frozen.FrozenSet.ToFrozenSet(names, StringComparer.Ordinal);
	}
	/// <summary>Binding guard: a bone name outside <see cref="LogicBoneNames"/> would reach a reader whose bone timelines were stripped.</summary>
	public string RequireLogicBone(string name) => name.Length == 0 || LogicBoneNames.Contains(name) ? name : throw new InvalidDataException("Bone name outside the host's logic bone set: " + name);
	private static void AddBoneNames(Google.Protobuf.IMessage message, HashSet<string> names, HashSet<Google.Protobuf.Reflection.MessageDescriptor> carriers)
	{
		foreach (var field in message.Descriptor.Fields.InFieldNumberOrder())
		{
			bool isMessage = field.FieldType == Google.Protobuf.Reflection.FieldType.Message;
			if (isMessage ? !carriers.Contains(field.MessageType) : !IsBoneName(field)) continue;
			if (field.IsMap) throw new NotSupportedException("Map fields are outside the authored bone name scan: " + field.FullName);
			object value = field.Accessor.GetValue(message);
			var items = field.IsRepeated ? ((System.Collections.IEnumerable)value).Cast<object>() : new[] { value };
			foreach (var item in items)
				if (item is Google.Protobuf.IMessage child) AddBoneNames(child, names, carriers);
				else if (item is string name) names.Add(name);
		}
	}
	private static bool IsBoneName(Google.Protobuf.Reflection.FieldDescriptor field) => field.FieldType == Google.Protobuf.Reflection.FieldType.String && field.Name.EndsWith("bone_name", StringComparison.Ordinal);
	/// <summary>Message types that can contain a bone name field, as a fixed point over the (possibly recursive) schema.</summary>
	private static HashSet<Google.Protobuf.Reflection.MessageDescriptor> BoneNameCarriers(Google.Protobuf.Reflection.MessageDescriptor root)
	{
		var types = new List<Google.Protobuf.Reflection.MessageDescriptor>(); var seen = new HashSet<Google.Protobuf.Reflection.MessageDescriptor>();
		void Visit(Google.Protobuf.Reflection.MessageDescriptor type)
		{
			if (!seen.Add(type)) return; types.Add(type);
			foreach (var field in type.Fields.InFieldNumberOrder()) if (field.FieldType == Google.Protobuf.Reflection.FieldType.Message) Visit(field.MessageType);
		}
		Visit(root);
		var carriers = types.Where(t => t.Fields.InFieldNumberOrder().Any(IsBoneName)).ToHashSet();
		for (bool changed = true; changed;)
		{
			changed = false;
			foreach (var type in types)
				if (!carriers.Contains(type) && type.Fields.InFieldNumberOrder().Any(f => f.FieldType == Google.Protobuf.Reflection.FieldType.Message && carriers.Contains(f.MessageType)))
					changed = carriers.Add(type);
		}
		return carriers;
	}
	public UnitPrefab ReadTree(int prefabId)
	{
		if (!trees.TryGetValue(prefabId, out var bytes))
		{
			using var command = database.CreateCommand();
			command.CommandText = "SELECT data,source_sha256,unity_version,extract_status FROM prefab_tree WHERE prefab_id=$id";
			command.Parameters.AddWithValue("$id", prefabId);
			using var reader = command.ExecuteReader();
			if (!reader.Read()) throw new InvalidDataException("Missing prefab_tree for prefab_id=" + prefabId);
			bytes = (byte[])reader.GetValue(0);
			var parsed = UnitPrefab.Parser.ParseFrom(bytes);
			if (parsed.FormatVersion != 1) throw new InvalidDataException("Unsupported UnitPrefab payload format_version for " + prefabId + "; re-extract unit_prefab with the matching manifest (expected 1).");
			if (parsed.PrefabId != prefabId || parsed.SourceSha256 != reader.GetString(1) ||
				parsed.UnityVersion != reader.GetString(2) || parsed.UnityVersion != "6000.0.58f2" || reader.GetString(3) != "ok")
				throw new InvalidDataException("Prefab identity, source or extraction status disagrees: " + prefabId);
			trees.Add(prefabId, bytes);
			return parsed;
		}
		return UnitPrefab.Parser.ParseFrom(bytes);
	}
	public AttackPrefabInput LoadAttack(int prefabId)
	{
		var slots = ReadTree(prefabId).Slots.Where(s => s.SlotName == SlotName.AttackDetail && s.Ordinal == -1).ToArray();
		if (slots.Length != 1 || !slots[0].HasUseDefaultDelay || slots[0].DirectDetails.Count != 1)
			throw new InvalidDataException("Prefab requires exactly one authored AttackDetail and UseDefaultDelay: " + prefabId);
		var source = slots[0].DirectDetails[0];
		if (!source.EffectsExtracted || source.ActionEffects.Count != 0 || source.ActionSubEffects.Count != 0)
			throw new NotSupportedException("AttackDetail effects require their own resource binding; they cannot become empty lists.");
		var detail = new ActionParameterOnPrefabDetail { Visible = source.Visible, ActionId = source.ActionId };
		foreach (var item in source.ExecTimes)
			detail.ExecTimeForPrefab.Add(new ActionExecTime
			{
				Time = item.Time,
				DamageNumType = (eDamageEffectType)item.DamageNumType,
				Weight = item.Weight,
				DamageNumScale = item.DamageNumScale
			});
		foreach (var item in source.ExecTimeCombos)
		{
			detail.ExecTimeCombo.Add(new ActionExecTimeCombo
			{
				StartTime = item.StartTime,
				OffsetTime = item.OffsetTime,
				Weight = item.Weight,
				Count = item.Count,
				InterporationType = (Elements.Battle.Core.eComboInterporationType)item.InterpolationType,
				// The deployed extractor intentionally omits empty curves. Python exposes an empty
				// message for an absent field; generated C# exposes null. Preserve that transport rule.
				Curve = LoadCurve(item.Curve ?? new Curve())
			});
		}
		return new AttackPrefabInput(prefabId, slots[0].UseDefaultDelay, detail);
	}
	public static AnimationCurve LoadCurve(Curve source)
	{
		ArgumentNullException.ThrowIfNull(source);
		// Extractor omits empty curves; their missing envelope does not affect the native constant-zero evaluator.
		if (source.Keys.Count != 0 && (source.PreInfinity != 2 || source.PostInfinity != 2))
			throw new NotSupportedException("Nonempty resource curve is outside verified ClampForever modes.");
		var keys = new Keyframe[source.Keys.Count];
		for (int i = 0; i < keys.Length; i++)
		{
			var key = source.Keys[i];
			if (key.WeightedMode != 0) throw new NotSupportedException("Weighted resource curve requires a separately verified evaluator.");
			// Target's referenced Keyframe API exposes the four-argument unweighted constructor.
			// Authored weights remain in ReadTree; native unweighted evaluation does not consume them.
			keys[i] = new Keyframe(key.Time, key.Value, key.InSlope, key.OutSlope);
		}
		return new AnimationCurve(keys);
	}
}
