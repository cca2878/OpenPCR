using global::OpenPCR.PrefabExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AssetsTools.NET.Extra;

namespace OpenPCR.PrefabExtractor;

internal static class AnimKitSources
{
	public static void Setup(string key, UnityBundle bundle, Rows rows)
	{
		byte[] raw = bundle.OfType(AssetClassID.TextAsset).First().TextBytes(); var segments = AnimKitBinary.Segments(raw);
		if (segments.Length != 1) throw new InvalidDataException("Embedded CYSP animations require extraction: " + segments.Length);
		var segment = segments[0]; var skeleton = AnimKitBinary.Skeleton(raw.AsSpan(segment.Offset, segment.Size).ToArray()); skeleton["cysp_body_count"] = 1;
		foreach (var (bone, index) in skeleton.Nodes("bones").Select((b, i) => (b, i)))
			rows.AddRow("spine_bone", key, index, bone.Text("name"), bone.Int("parent"), bone.Number("rotation"), bone.Number("x"), bone.Number("y"), bone.Number("scaleX"), bone.Number("scaleY"), bone.Number("shearX"), bone.Number("shearY"), bone.Number("length"), bone.Int("transform_mode"));
		var skins = new List<Tree>(); if (skeleton["default_skin"] is Tree skin) skins.Add(skin); skins.AddRange(skeleton.Items("named_skins").OfType<Tree>());
		foreach (var s in skins)
			foreach (var slot in s.Nodes("slots"))
				foreach (var attachment in slot.Nodes("attachments").Where(a => a.Text("type") == "BoundingBox"))
				{
					int slotIndex = slot.Int("slot_index"); string slotName = (string)skeleton.Items("slot_names")[slotIndex]!; string name = attachment.Text("name");
					bool weighted = attachment.Bool("weighted"); rows.AddRow("spine_bounding_box", key, s.Text("name"), slotName, name, slotIndex, attachment.Int("vertex_count"), weighted);
					if (weighted) continue;
					var vertices = (double[])attachment.Required("vertices");
					for (int i = 0; i < vertices.Length / 2; i++) rows.AddRow("spine_bounding_box_vertex", key, s.Text("name"), slotName, name, i, vertices[2 * i], vertices[2 * i + 1]);
				}
		rows.AddRow("spine_pose_setup", key, Tree.Json(skeleton));
	}
	// Floats only occur in packed repeated fields, which the generated writer stores bit for bit.
	private static byte[] Timelines(AnimKitAnimation animation) => Google.Protobuf.MessageExtensions.ToByteArray(animation.Timelines);
	public static void Cysp(string key, string[] names, Resources resources, Rows rows)
	{
		var seenEvents = new HashSet<int>();
		foreach (string name in names)
		{
			using var bundle = new UnityBundle(resources.Fetch(name), name);
			byte[] raw = bundle.OfType(AssetClassID.TextAsset).First().TextBytes();
			foreach (var (segment, index) in AnimKitBinary.Segments(raw).Select((s, i) => (s, i)))
			{
				byte[] body = raw.AsSpan(segment.Offset, segment.Size).ToArray(); AnimKitReader r = new AnimKitReader(body); string anim = r.String() ?? throw new InvalidDataException("Null animation name");
				AnimKitAnimation animation = AnimKitAnimation.Read(r);
				if (r.Position != r.Length) throw new InvalidDataException(name + "/" + anim + ": animation extent mismatch");
				string baseName = anim.StartsWith(key + "_", StringComparison.Ordinal) ? anim[(key.Length + 1)..] : anim;
				rows.AddRow("cysp_animation", key, anim, baseName, name, index, segment.Offset, segment.Size, animation.Duration);
				rows.AddRow("cysp_pose", key, name, anim, Timelines(animation));
				if (baseName == "damage") for (int i = 0; i < animation.Events.Count; i++) if (seenEvents.Add(i)) rows.AddRow("cysp_event", key, baseName, i, animation.Events[i]);
			}
		}
	}
	public static void Standalone(string source, string key, string name, UnityBundle bundle, Rows rows)
	{
		bool mode = source == "mode_spine";
		Tree asset = mode ? bundle.Container[$"assets/_elementsresources/resources/spine/sdmodechange/spine_{key}_1/{key}_1_skeletondata.asset"].Tree : bundle.OfType(AssetClassID.MonoBehaviour).Single(o => o.Tree.ContainsKey("skeletonJSON")).Tree;
		var reference = asset.Node("skeletonJSON");
		if (reference.Int("m_FileID") != 0 || reference.Long("m_PathID") == 0) throw new InvalidDataException("Unresolved skeletonJSON PPtr");
		var obj = mode ? bundle.OfType(AssetClassID.TextAsset).Single(o => o.Id == reference.Long("m_PathID")) : bundle.OfType(AssetClassID.TextAsset).Single(o => o.Tree.Text("m_Name").EndsWith(".skel", StringComparison.Ordinal));
		byte[] raw = obj.TextBytes(); var skeleton = AnimKitBinary.Skeleton(raw); AnimKitReader r = new AnimKitReader(raw, skeleton.Int("_bytes_consumed"));
		if (mode) rows.AddRow("mode_spine_setup", name, Tree.Json(skeleton));
		for (int i = 0, n = r.Var(); i < n; i++) { r.String(); r.Var(false); r.Float(); r.String(); }
		for (int i = 0, n = r.Var(); i < n; i++)
		{
			string anim = r.String() ?? throw new InvalidDataException("Null animation name"); AnimKitAnimation animation = AnimKitAnimation.Read(r);
			rows.AddRow(source + "_animation", key, anim, name, i, animation.Duration);
			rows.AddRow(mode ? "mode_spine_pose" : "skill_spine_timeline", name, anim, mode ? Timelines(animation) : Tree.Json(animation.Properties));
		}
		if (r.Position != r.Length) throw new InvalidDataException(name + ": skeleton animation extent mismatch");
		rows.AddRow(source + "_data", key, name, Tree.Json(asset));
	}
}
