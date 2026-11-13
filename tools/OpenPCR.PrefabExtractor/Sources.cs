using global::OpenPCR.PrefabExtractor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AssetsTools.NET.Extra;
using Google.Protobuf;

namespace OpenPCR.PrefabExtractor;

internal static class Sources
{
	public static string[] Names => Table.Sources.Keys.ToArray();
	public static IEnumerable<string> Keys(string source, Resources resources, Database database)
	{
		IEnumerable<string> Match(string pattern, string prefix, string suffix) => resources.Names.Where(n => Regex.IsMatch(n, pattern, RegexOptions.CultureInvariant)).Select(n => n[prefix.Length..^suffix.Length]);
		return source switch
		{
			"unit_prefab" => Match(@"^all_battleunitprefab_\d+\.unity3d$", "all_battleunitprefab_", ".unity3d").OrderBy(int.Parse),
			"spine" => Match(@"^spine_.+_chara_base\.cysp\.unity3d$", "spine_", "_chara_base.cysp.unity3d"),
			"unit_spine" => Match(@"^spine_sdnormal_.+\.unity3d$", "spine_sdnormal_", ".unity3d")
				.Concat(Match(@"^spine_sdshadow_.+\.unity3d$", "spine_sdshadow_", ".unity3d").Select(key => "shadow_" + key)),
			"mode_spine" => Match(@"^spine_sdmodechange_.+_1\.unity3d$", "spine_sdmodechange_", "_1.unity3d"),
			"skill_spine" => Match(@"^spine_skilleffect_.+\.unity3d$", "spine_skilleffect_", ".unity3d"),
			"firearm" => database.Keys("prefab_tree", "prefab_id"),
			"battle_manager" => ["0"],
			"circle_effect" => ["a/all_effectprefab_3.unity3d"],
			"skill_effect" => resources.Names.Where(n => Regex.IsMatch(n, @"^all_(?:battleunit(?:prefab)?_\d+|fxsk_\d+|effectprefab_\w+)\.unity3d$", RegexOptions.CultureInvariant)).Select(n => "a/" + n),
			"cysp" => resources.Names.Where(n => Regex.IsMatch(n, @"^spine_.+_battle\.cysp\.unity3d$", RegexOptions.CultureInvariant)).Select(n => n[6..].Replace("_common_battle.cysp.unity3d", "", StringComparison.Ordinal).Replace("_battle.cysp.unity3d", "", StringComparison.Ordinal)).Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal),
			_ => throw new ArgumentException("Unknown source: " + source)
		};
	}
	public static string[] BundleNames(string source, string key, Resources? resources) => source switch
	{
		"unit_prefab" or "firearm" => [$"all_battleunitprefab_{key}.unity3d"],
		"spine" => [$"spine_{key}_chara_base.cysp.unity3d"],
		"unit_spine" => [$"spine_{UnitSpineIdentity(key).Kind}_{UnitSpineIdentity(key).Skin}.unity3d"],
		"mode_spine" => [$"spine_sdmodechange_{key}_1.unity3d"],
		"skill_spine" => [$"spine_skilleffect_{key}.unity3d"],
		"cysp" => new[] { $"spine_{key}_battle.cysp.unity3d", $"spine_{key}_common_battle.cysp.unity3d" }.Where(n => resources!.Contains(n)).ToArray(),
		"battle_manager" => ["bdl_battlemanager.unity3d", "bdl_battlespinecontroller.unity3d"],
		"skill_effect" or "circle_effect" => [key.StartsWith("a/", StringComparison.Ordinal) ? key[2..] : throw new ArgumentException("Expected a/ bundle key")],
		_ => throw new ArgumentException("Unknown source: " + source)
	};
	public static Rows Parse(string source, string key, Resources resources)
	{
		string[] names = BundleNames(source, key, resources);
		if (names.Length == 0) throw new InvalidDataException("No bundles for " + key);
		var rows = new Rows(source);
		if (source == "cysp") { AnimKitSources.Cysp(key, names, resources, rows); return rows; }
		byte[] bytes = resources.Fetch(names[0]);
		using var bundle = new UnityBundle(bytes, names[0]);
		switch (source)
		{
			case "unit_prefab":
				var message = UnitPrefabBuilder.Build(int.Parse(key, CultureInfo.InvariantCulture), bytes, bundle);
				rows.AddRow("prefab_tree", message.PrefabId, message.SourceSha256, message.UnityVersion, "ok", PrefabSerialization.Encode(message)); break;
			case "firearm":
				var found = Firearms(bundle);
				foreach (string dependency in bundle.Dependencies)
				{
					string name = dependency.Split('/')[^1];
					using var dep = new UnityBundle(resources.Fetch(name), name);
					foreach (var pair in Firearms(dep)) found[pair.Key] = pair.Value;
				}
				foreach (var (id, values) in found.OrderBy(p => p.Key)) rows.AddRow("firearm_prefab", new object?[] { id, int.Parse(key, CultureInfo.InvariantCulture) }.Concat(values).ToArray());
				break;
			case "skill_effect": SkillEffects(key, bundle, rows); break;
			case "circle_effect": Circles(key, bundle, rows); break;
			case "battle_manager":
				using (var spine = new UnityBundle(resources.Fetch(names[1]), names[1])) BattleManager(bundle, spine, rows);
				break;
			case "unit_spine":
				var identity = UnitSpineIdentity(key);
				var asset = bundle.Container[$"assets/_elementsresources/resources/spine/{identity.Kind}/spine_{identity.Skin}/{identity.Skin}_skeletondata.asset"].Tree;
				foreach (string field in new[] { "skeletonJSON", "fromAnimation", "toAnimation", "duration", "defaultMix" }) asset.Required(field);
				rows.AddRow("unit_spine_data", key, names[0], Tree.Json(asset)); break;
			case "spine": AnimKitSources.Setup(key, bundle, rows); break;
			case "mode_spine": case "skill_spine": AnimKitSources.Standalone(source, key, names[0], bundle, rows); break;
			default: throw new ArgumentException(source);
		}
		return rows;
	}
	private static (string Kind, string Skin) UnitSpineIdentity(string key)
		=> key.StartsWith("shadow_", StringComparison.Ordinal) ? ("sdshadow", key[7..]) : ("sdnormal", key);
	private static Dictionary<long, Tree> GameObjects(UnityBundle b) => b.OfType(AssetClassID.GameObject).ToDictionary(o => o.Id, o => o.Tree);
	private static Dictionary<long, Tree> Positions(UnityBundle b) => b.OfType(AssetClassID.Transform).ToDictionary(o => o.Tree.Path("m_GameObject"), o => o.Tree.Node("m_LocalPosition"));
	private static Dictionary<long, object?[]> Firearms(UnityBundle bundle)
	{
		var found = new Dictionary<long, object?[]>(); var objects = GameObjects(bundle); var positions = Positions(bundle);
		foreach (var obj in bundle.OfType(AssetClassID.MonoBehaviour))
		{
			if (obj.ClassName is not ("FirearmCtrl" or "FirearmCtrlEx" or "SpineFireArm" or "SpineCowFireArm")) continue;
			var t = obj.Tree; long go = t.Path("m_GameObject"); var p = positions[go]; var box = t.Node("ColliderBox");
			var center = box.Node(box.ContainsKey("m_Center") ? "m_Center" : "center"); var extent = box.Node(box.ContainsKey("m_Extent") ? "m_Extent" : "extents");
			found.Add(go, [t.Int("MoveType"), t.Number("MoveRate"), t.Number("duration"), t.Number("HitDelay"), p.Number("x"), p.Number("y"), p.Number("z"), center.Number("x"), extent.Number("x"), extent.Number("y"), objects[go].Text("m_Name")]);
		}
		return found;
	}
	private static void SkillEffects(string key, UnityBundle bundle, Rows rows)
	{
		var objects = GameObjects(bundle); var positions = Positions(bundle);
		var behaviours = bundle.OfType(AssetClassID.MonoBehaviour).ToDictionary(o => o.Id);
		var particles = bundle.OfType(AssetClassID.ParticleSystem).ToDictionary(o => o.Id);
		var referencedParticles = new SortedSet<long>(); var tweeners = new SortedSet<long>();
		foreach (var obj in behaviours.Values)
		{
			var t = obj.Tree; bool boneTimeLine = obj.ClassName == "EffectBoneTimeLineData";
			if (!boneTimeLine && (!t.ContainsKey("isRepeat") || !t.ContainsKey("particle"))) continue;
			long go = t.Path("m_GameObject"); var gameObject = objects[go]; var position = positions[go];
			int index = gameObject.Nodes("m_Component").Select((c, i) => (c, i)).Single(c => c.c.Path("component") == obj.Id).i;
			rows.AddRow("skill_effect_component_data", key, obj.Id, go, index, Tree.Json(t));
			if (boneTimeLine)
			{
				// SkillEffectCtrl reads it through GetComponent; exported so the host fails on effects that carry it.
				rows.AddRow("skill_effect_component", key, go, index, obj.ClassName!, gameObject.Text("m_Name"), false, 0, 0L, position.Number("x"), position.Number("y"), position.Number("z"));
				continue;
			}
			foreach (string field in new[] { "TweenerList", "TweenRotList" })
				foreach (var pointer in t.Nodes(field))
				{
					if (pointer.Int("m_FileID") != 0) throw new InvalidDataException("External tweener PPtr requires serialized-file resolution");
					if (pointer.Long("m_PathID") != 0) tweeners.Add(pointer.Long("m_PathID"));
				}
			var particle = t.Node("particle");
			if (particle.Long("m_PathID") != 0)
			{
				if (particle.Int("m_FileID") != 0) throw new InvalidDataException("External particle PPtr requires serialized-file resolution");
				referencedParticles.Add(particle.Long("m_PathID"));
			}
			string className = obj.ClassName ?? throw new InvalidDataException("Effect component has a null script");
			rows.AddRow("skill_effect_component", key, go, index, className, gameObject.Text("m_Name"), t.Bool("isRepeat"), particle.Int("m_FileID"), particle.Long("m_PathID"), position.Number("x"), position.Number("y"), position.Number("z"));
			if (className != "CountDownEffectCtrl") continue;
			rows.AddRow("count_down_effect", key, go, t.Int("warningLimitTime"));
			foreach (var (effect, i) in t.Nodes("CountEndEffectData").Select((e, i) => (e, i)))
				rows.AddRow("count_down_end_effect", key, go, i, effect.Node("Prefab").Int("m_FileID"), effect.Path("Prefab"), effect.Node("PrefabLeft").Int("m_FileID"), effect.Path("PrefabLeft"), effect.Number("Time"));
		}
		foreach (long id in referencedParticles) rows.AddRow("skill_effect_particle", key, id, particles[id].Tree.Number("lengthInSec"));
		foreach (long id in tweeners) rows.AddRow("skill_effect_tweener", key, id, behaviours[id].ClassName, Tree.Json(behaviours[id].Tree));
		foreach (var (path, obj) in bundle.Container) rows.AddRow("skill_effect_asset", key, path, obj.Id);
		rows.AddRow("skill_effect_bundle", key);
	}
	private static void Circles(string key, UnityBundle bundle, Rows rows)
	{
		var objects = GameObjects(bundle); var positions = Positions(bundle); var behaviours = bundle.OfType(AssetClassID.MonoBehaviour).ToDictionary(o => o.Id);
		foreach (var obj in behaviours.Values.Where(o => o.ClassName == "CircleEffectController"))
		{
			var t = obj.Tree; string name = objects[t.Path("m_GameObject")].Text("m_Name");
			rows.AddRow("circle_effect_prefab", key, name, t.Items("Children").Count, t.Bool("FollowXY"), t.Number("duration"), t.Number("Radius"), t.Number("Roll"), t.Number("Yaw"), t.Number("angularVelocity"));
			foreach (var (pointer, index) in t.Nodes("Children").Select((p, i) => (p, i)))
			{
				var child = behaviours[pointer.Long("m_PathID")].Tree;
				rows.AddRow("circle_effect_child", key, name, index, positions[child.Path("m_GameObject")].Number("x"), child.Number("StartPitch"));
			}
		}
	}
	private static void BattleManager(UnityBundle bundle, UnityBundle spine, Rows rows)
	{
		const string path = "assets/_elementsbundles/resources/bdl/parts/battlespinecontroller.prefab";
		var root = spine.Container[path];
		var transform = spine.OfType(AssetClassID.Transform).Single(o => o.Tree.Path("m_GameObject") == root.Id);
		rows.AddRow("battle_spine_transform", path, Tree.Json(transform.Tree));
		var manager = bundle.OfType(AssetClassID.MonoBehaviour).Single(o => o.Tree.ContainsKey("energyStackRatioDamagedFront"));
		foreach (var (name, value) in manager.Tree.OrderBy(p => p.Key, StringComparer.Ordinal))
			if (!name.StartsWith("m_", StringComparison.Ordinal) && value is not (Tree or List<object?> or string or null)) rows.AddRow("battle_manager_field", name, Convert.ToDouble(value));
		long id = manager.Tree.Path("unitParentTransform"); var visited = new HashSet<long>();
		for (int depth = 0; id != 0; depth++)
		{
			if (!visited.Add(id) || depth >= 12) throw new InvalidDataException("Invalid BattleManager transform chain");
			var t = bundle.Objects.Single(o => o.File == manager.File && o.Id == id).Tree;
			string name = bundle.Objects.Single(o => o.File == manager.File && o.Id == t.Path("m_GameObject")).Tree.Text("m_Name");
			long father = t.Path("m_Father");
			rows.AddRow("battle_manager_transform", id, name, depth, t.Node("m_LocalScale").Number("x"), father == 0 ? null : father); id = father;
		}
	}
}
