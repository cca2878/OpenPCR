using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Elements;
using Elements.Battle.Core;
using OpenPCR.Growth;
using Pcr.Prefab;

namespace OpenPCR.Host;

/// <summary>Authored equipment skill scriptables from an explicitly deployed, versioned resource document.</summary>
internal sealed class EquipmentSkillResources
{
	private readonly ResourceDatabases databases;
	private Dictionary<int, ExSkillScriptable>? assets;
	internal EquipmentSkillResources(ResourceDatabases databases) { this.databases = databases; }
	internal ExSkillScriptable Read(long id)
	{
		if (assets is null)
		{
			string path = Path.Combine(databases.Root, "prefab", "equipment_skills.json");
			using var document = JsonDocument.Parse(File.ReadAllBytes(path)); var root = document.RootElement;
			JsonInputValidation.RejectDuplicateProperties(root);
			if (root.GetProperty("schema").GetString() != "openpcr-equipment-skills/v1" || root.GetProperty("unity_version").GetString() != "6000.0.58f2" || root.GetProperty("manifest_version").GetString() != databases.MasterVersion)
				throw new InvalidDataException("Equipment skill deployment schema, Unity or manifest differs.");
			RequireHash(root.GetProperty("manifest_sha256"));
			var prepared = new Dictionary<int, ExSkillScriptable>();
			foreach (var entry in root.GetProperty("assets").EnumerateArray())
			{
				int skillId = entry.GetProperty("skill_id").GetInt32();
				RequireHash(entry.GetProperty("source_sha256"));
				if (skillId <= 0 || entry.GetProperty("bundle").GetString() != $"a/all_equipment_skill_{skillId}.unity3d" ||
					entry.GetProperty("asset_path").GetString() != $"assets/_elementsresources/resources/all/battle/equipmentskill/equipment_skill_{skillId}.asset")
					throw new InvalidDataException("Equipment skill resource identity differs.");
				var source = entry.GetProperty("data");
				var asset = (ExSkillScriptable)RuntimeHelpers.GetUninitializedObject(typeof(ExSkillScriptable));
				asset.KnockAnimationCurve = Curve(source.GetProperty("KnockAnimationCurve"));
				asset.KnockDownAnimationCurve = Curve(source.GetProperty("KnockDownAnimationCurve"));
				asset.EffectType = (eEffectType)source.GetProperty("EffectType").GetInt32();
				if (!Enum.IsDefined(asset.EffectType)) throw new InvalidDataException("Unknown authored equipment effect type.");
				prepared.Add(skillId, asset);
			}
			if (prepared.Count == 0) throw new InvalidDataException("Equipment skill deployment is empty.");
			assets = prepared;
		}
		return id is > 0 and <= int.MaxValue && assets.TryGetValue((int)id, out var value) ? value : throw new InvalidDataException("Missing authored equipment skill: " + id);
	}
	private static void RequireHash(JsonElement value)
	{
		string? hash = value.GetString();
		if (hash is null || hash.Length != 64 || hash.Any(c => c is not (>= '0' and <= '9') and not (>= 'a' and <= 'f')))
			throw new InvalidDataException("Equipment skill SHA256 must contain 64 lowercase hexadecimal characters.");
	}
	private static global::OpenPCR.Runtime.Api.AnimationCurve Curve(JsonElement source)
	{
		var curve = new Curve { PreInfinity = source.GetProperty("m_PreInfinity").GetInt32(), PostInfinity = source.GetProperty("m_PostInfinity").GetInt32(), RotationOrder = source.GetProperty("m_RotationOrder").GetInt32() };
		foreach (var key in source.GetProperty("m_Curve").EnumerateArray()) curve.Keys.Add(new CurveKey
		{
			Time = key.GetProperty("time").GetSingle(),
			Value = key.GetProperty("value").GetSingle(),
			InSlope = key.GetProperty("inSlope").GetSingle(),
			OutSlope = key.GetProperty("outSlope").GetSingle(),
			InWeight = key.GetProperty("inWeight").GetSingle(),
			OutWeight = key.GetProperty("outWeight").GetSingle(),
			WeightedMode = key.GetProperty("weightedMode").GetInt32()
		});
		return PrefabInputs.LoadCurve(curve);
	}
}
