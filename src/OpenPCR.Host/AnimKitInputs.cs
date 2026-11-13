using global::Elements;
using global::OpenPCR.AnimKit;
using global::OpenPCR.Host;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Pcr.Prefab;
using Animation = global::OpenPCR.AnimKit.Animation;

namespace OpenPCR.Host;

/// <summary>Named extracted pose and binary inputs. Runtime pose evaluation stays in AnimKit.</summary>
internal sealed class AnimKitInputs
{
	private readonly SqliteConnection database;
	private static readonly ConditionalWeakTable<BattleAnimKitController, object> zeroMixRenderEffects = new();
	private static readonly ConditionalWeakTable<BattleAnimKitController, HashSet<(int Type, int Index)>> unitAnimations = new();
	private readonly Dictionary<string, (SkeletonData Data, JsonElement Asset)> cache = new();
	private readonly IReadOnlySet<string> logicBones;
	public AnimKitInputs(ResourceDatabases resources) { database = resources.Prefab; logicBones = resources.Prefabs.LogicBoneNames; }
	private JsonElement One(string table, string column, string key)
	{
		using var command = database.CreateCommand(); command.CommandText = $"SELECT data FROM {table} WHERE {column}=$key";
		command.Parameters.AddWithValue("$key", key);
		var value = command.ExecuteScalar() ?? throw new InvalidDataException($"Missing {table}: {key}");
		using var parsed = ParseJson(value, table is "spine_pose_setup" or "mode_spine_setup"); return parsed.RootElement.Clone();
	}
	/// <summary>
	/// Skeleton setups and skill-effect timeline properties are zlib-compressed UTF-8 JSON;
	/// the other JSON columns are text. Either form in the wrong column is rejected.
	/// </summary>
	private static JsonDocument ParseJson(object value, bool compressed)
	{
		if (!compressed) return JsonDocument.Parse(value as string ?? throw new InvalidDataException("Prefab JSON column must be text."));
		using var zlib = new System.IO.Compression.ZLibStream(new MemoryStream(value as byte[] ?? throw new InvalidDataException("Prefab compressed JSON column must be a zlib blob.")), System.IO.Compression.CompressionMode.Decompress);
		using var json = new MemoryStream(); zlib.CopyTo(json);
		return JsonDocument.Parse(json.GetBuffer().AsMemory(0, (int)json.Length));
	}
	public BattleAnimKitController CreateUnitSkin(int skinId, RuntimeMasterData master, eAnimKitType type)
		=> CreateUnitSkin(skinId, master, type, 0);
	public BattleAnimKitController CreateUnitSkin(int skinId, RuntimeMasterData master, eAnimKitType type, int prefabSkinId)
	{
		if (type is not (eAnimKitType.SD_NORMAL or eAnimKitType.SD_NORMAL_BOUND or eAnimKitType.SD_SHADOW_BOUND)) throw new NotSupportedException("Unprepared unit animation type: " + type);
		// The resource set uses the prefab override for SkinId, but derives UnitId from
		// the original skin. Base/common/battle animations and MotionType keep that unit identity.
		int resourceSkinId = prefabSkinId != 0 ? prefabSkinId : skinId;
		int unitId = UnitUtility.SkinIdToUnitId(skinId);
		int motion = master.Manager.masterUnitData.Get(unitId)?.MotionType ?? 0;
		string skeletonKey = UnitUtility.GetSpineCharaBaseAnimationNumber(unitId).ToString("D6");
		var bundles = new List<string>();
		if (!UnitUtility.JudgeIsGuest(unitId))
		{
			int common = motion != 0 ? motion : !UnitUtility.IsEnemyUnit(unitId) ? unitId : -1;
			if (common != -1) bundles.Add($"spine_{common:D2}_common_battle.cysp.unity3d");
		}
		bundles.Add($"spine_{unitId:D6}_battle.cysp.unity3d");
		string resourceKind = type == eAnimKitType.SD_SHADOW_BOUND ? "sdshadow" : "sdnormal";
		var input = Unit(skeletonKey, $"spine_{resourceKind}_{resourceSkinId:D6}.unity3d", bundles);
		BattleAnimKitController controller = CreateController(input, resourceSkinId, unitId, motion, type);
		var loaded = new HashSet<(int, int)> { ((int)eAnimKitBinaryAnimationId.BATTLE, unitId) };
		if (bundles.Count == 2) loaded.Add(((int)eAnimKitBinaryAnimationId.COMMON_BATTLE, motion != 0 ? motion : unitId));
		unitAnimations.Add(controller, loaded);
		return controller;
	}
	public static void RequireUnitAnimation(object owner, int type, int index)
	{
		if (owner is not BattleAnimKitController controller || !unitAnimations.TryGetValue(controller, out var loaded))
			throw new InvalidDataException("Animation bundle was not prepared for this actual controller: " + type + ":" + index);
		// The original animation loader returns before resource
		// lookup for index -1, including COMMON_BATTLE of motionless enemy units.
		if (index == -1) return;
		if (!loaded.Contains((type, index)))
			throw new InvalidDataException("Animation bundle was not prepared for this actual controller: " + type + ":" + index);
		// The extracted CYSP bundle was read in original common/battle order at creation,
		// with first-loaded names and Native partial-read failure retained. Re-loading
		// that same named bundle has no new animation names to append.
	}
	private static BattleAnimKitController CreateController((SkeletonData Data, JsonElement Asset) input, int skinId, int unitId, int motion, eAnimKitType spineType)
	{
		// Supplies skeleton component data and state; mesh instructions and buffers are outside this producer.
		BattleAnimKitController controller = (BattleAnimKitController)RuntimeHelpers.GetUninitializedObject(typeof(BattleAnimKitController));
		RuntimeBinding.Set(controller, "separatorSlots", new List<Slot>());
		RuntimeBinding.Set(controller, "separatorSlotNames", Array.Empty<string>());
		RuntimeBinding.Set(controller, "defaultAttachmentDic", input.Data.DefaultSkin?.CopyAttachment() ?? new Dictionary<Skin.AttachmentKeyTuple, Attachment>());
		RuntimeBinding.Set(controller, "DefaultSkin", input.Data.DefaultSkin);
		RuntimeBinding.Set(controller, "currentChildObjectShader", Array.Empty<Shader>());
		RuntimeBinding.Set(controller, "colorShader", new ColorShader());
		RuntimeBinding.Set(controller, "AnimeName", "");
		RuntimeBinding.Set(controller, "UnitId", unitId); RuntimeBinding.Set(controller, "MotionType", motion);
		RuntimeBinding.Set(controller, "skinId", skinId); RuntimeBinding.Set(controller, "spineType", spineType);
		RuntimeBinding.Set(controller, "timeScaleMain", 1f); RuntimeBinding.Set(controller, "timeScaleForEffect", 1f);
		controller.timeScale = 1f;
		controller.skeleton = new Skeleton(input.Data);
		var stateData = new AnimationStateData(input.Data);
		RuntimeBinding.Set(stateData, "defaultMix", input.Asset.GetProperty("defaultMix").GetSingle());
		var from = input.Asset.GetProperty("fromAnimation"); var to = input.Asset.GetProperty("toAnimation"); var durations = input.Asset.GetProperty("duration");
		if (from.GetArrayLength() != to.GetArrayLength() || from.GetArrayLength() != durations.GetArrayLength()) throw new InvalidDataException("animation mix arrays differ.");
		for (int i = 0; i < from.GetArrayLength(); i++) stateData.SetMix(from[i].GetString()!, to[i].GetString()!, durations[i].GetSingle());
		controller.state = new global::OpenPCR.AnimKit.AnimationState(stateData);
		controller.valid = true;
		controller.skeleton.UpdateWorldTransform();
		RuntimeBinding.Call(controller, "EntryCallBack");
		return controller;
	}

	/// <summary>Lifetime-only effect projection: ordered properties, mixing and completion, without geometry production.</summary>
	public BattleAnimKitController CreateEffect(int skillId, int index)
	{
		string bundle = $"spine_skilleffect_{skillId:D6}_{index:D2}.unity3d";
		var asset = One("skill_spine_data", "bundle", bundle);
		var data = new SkeletonData();
		bool projectedDeform = false;
		var animations = (Dictionary<string, Animation>)RuntimeBinding.Get(data, "animations")!;
		using var command = database.CreateCommand();
		command.CommandText = "SELECT a.anim_name,a.duration_sec,p.properties FROM skill_spine_animation a JOIN skill_spine_timeline p ON a.bundle=p.bundle AND a.anim_name=p.anim_name WHERE a.bundle=$bundle ORDER BY a.anim_index";
		command.Parameters.AddWithValue("$bundle", bundle);
		using var reader = command.ExecuteReader();
		while (reader.Read())
		{
			using var properties = ParseJson(reader.GetValue(2), true);
			var timelines = new ExposedList<Timeline>();
			foreach (var property in properties.RootElement.EnumerateArray())
			{
				if (property[0].GetString() == "Deform")
				{
					if (asset.GetProperty("defaultMix").GetSingle() != 0 || asset.GetProperty("duration").EnumerateArray().Any(value => value.GetSingle() != 0))
						throw new NotSupportedException("Effect deform with mixing requires actual attachment/property identity input: " + bundle);
					if (property.GetArrayLength() != 4 || property[1].GetInt32() < 0 || property[2].GetInt32() < 0 || string.IsNullOrEmpty(property[3].GetString()))
						throw new InvalidDataException("Invalid authored effect Deform identity.");
					// With no mixing or events, render-only effect timelines cannot affect duration or completion.
					projectedDeform = true;
					continue;
				}
				int type = property[0].GetString() switch
				{
					"Rotate" => 0,
					"Translate" => 1,
					"Scale" => 2,
					"Shear" => 3,
					"Attachment" => 4,
					"Color" => 5,
					"Event" => 7,
					"DrawOrder" => 8,
					"IkConstraint" => 9,
					"TransformConstraint" => 10,
					"PathConstraintPosition" => 11,
					"PathConstraintSpacing" => 12,
					"PathConstraintMix" => 13,
					"TwoColor" => 14,
					_ => throw new InvalidDataException("Unknown effect timeline identity.")
				};
				timelines.Add(new PropertyOnly((type << 24) + property[1].GetInt32()));
			}
			var bytes = (byte[])reader[1];
			if (bytes.Length != 8) throw new InvalidDataException("Effect animation duration must retain float64 bytes.");
			animations.Add(reader.GetString(0), new Animation(reader.GetString(0), timelines, (float)BitConverter.ToDouble(bytes)));
		}
		if (animations.Count == 0) throw new InvalidDataException("Missing independent effect animation input: " + bundle);
		BattleAnimKitController controller = CreateController((data, asset), skillId, skillId, 0, eAnimKitType.SKILL_EFFECT);
		if (projectedDeform) zeroMixRenderEffects.Add(controller, new object());
		return controller;
	}

	public BattleAnimKitController CreateMode(int skinId, RuntimeMasterData master)
	{
		string bundle = $"spine_sdmodechange_{skinId:D6}_1.unity3d";
		string key = "mode|" + bundle;
		if (!cache.TryGetValue(key, out var input))
		{
			var raw = One("mode_spine_setup", "bundle", bundle);
			var asset = One("mode_spine_data", "bundle", bundle);
			var data = ReadSetup(raw); var keep = LogicPose(raw, logicBones);
			var animations = (Dictionary<string, Animation>)RuntimeBinding.Get(data, "animations")!;
			using var command = database.CreateCommand();
			command.CommandText = "SELECT a.anim_name,a.duration_sec,p.timelines FROM mode_spine_animation a JOIN mode_spine_pose p ON a.bundle=p.bundle AND a.anim_name=p.anim_name WHERE a.bundle=$bundle ORDER BY a.anim_index";
			command.Parameters.AddWithValue("$bundle", bundle); using var reader = command.ExecuteReader();
			bool found = false;
			while (reader.Read())
			{
				found = true; var bytes = (byte[])reader[1];
				if (bytes.Length != 8) throw new InvalidDataException("Mode animation duration must be float64.");
				string name = reader.GetString(0);
				animations.Add(name, new Animation(name, ReadAnimation(ParseTimelines(reader.GetValue(2)), data, keep), (float)BitConverter.ToDouble(bytes)));
			}
			if (!found) throw new InvalidDataException("Missing mode animation timelines: " + bundle);
			cache.Add(key, (data, asset)); input = (data, asset);
		}
		int unitId = UnitUtility.SkinIdToUnitId(skinId);
		return CreateController(input, skinId, unitId, master.Manager.masterUnitData.Get(unitId)?.MotionType ?? 0, eAnimKitType.SD_MODE_CHANGE);
	}
	private (SkeletonData Data, JsonElement Asset) Unit(string skeletonKey, string assetBundle, IReadOnlyList<string> bundles)
	{
		string key = skeletonKey + "|" + assetBundle + "|" + string.Join('|', bundles);
		if (cache.TryGetValue(key, out var previous)) return previous;
		var raw = One("spine_pose_setup", "skeleton_key", skeletonKey);
		if (raw.GetProperty("cysp_body_count").GetInt32() != 1) throw new NotSupportedException("Embedded skeleton animations are outside the extracted input contract.");
		var asset = One("unit_spine_data", "bundle", assetBundle);
		var data = ReadSetup(raw); var keep = LogicPose(raw, logicBones);
		var animations = (Dictionary<string, Animation>)RuntimeBinding.Get(data, "animations")!;
		foreach (string bundle in bundles)
		{
			using var command = database.CreateCommand(); command.CommandText = "SELECT a.anim_name,a.duration_sec,p.timelines FROM cysp_animation a JOIN cysp_pose p ON a.bundle=p.bundle AND a.anim_name=p.anim_name WHERE a.bundle=$bundle ORDER BY a.anim_index";
			command.Parameters.AddWithValue("$bundle", bundle); using var reader = command.ExecuteReader(); bool found = false;
			while (reader.Read())
			{
				found = true; string name = reader.GetString(0); var duration = (byte[])reader[1];
				if (duration.Length != 8) throw new InvalidDataException("animation duration blob must be float64: " + name);
				ExposedList<Timeline> timelines;
				try { timelines = ReadAnimation(ParseTimelines(reader.GetValue(2)), data, keep); }
				catch (AnimationInputReadException) { break; } // Native AddAnimationCysp owns this failure, retains earlier entries and aborts this bundle.
				if (!animations.ContainsKey(name)) animations.Add(name, new Animation(name, timelines, (float)BitConverter.ToDouble(duration)));
			}
			if (!found) throw new InvalidDataException("Missing cysp_pose bundle: " + bundle);
		}
		cache.Add(key, (data, asset)); return (data, asset);
	}
	private static SkeletonData ReadSetup(JsonElement raw)
	{
		var data = new SkeletonData(); var bones = (ExposedList<BoneData>)RuntimeBinding.Get(data, "bones")!;
		foreach (var input in raw.GetProperty("bones").EnumerateArray())
		{
			int parent = input.GetProperty("parent").GetInt32();
			var bone = new BoneData(bones.Count, input.GetProperty("name").GetString()!, bones.Count == 0 ? null : bones.Items[parent]);
			foreach (string field in new[] { "rotation", "x", "y", "scaleX", "scaleY", "shearX", "shearY", "length" })
				RuntimeBinding.Set(bone, field, input.GetProperty(field).GetSingle() * (field is "x" or "y" or "length" ? 0.5f : 1f));
			// Authored binary stores an index into the original parser table, not enum bits.
			int transformIndex = input.GetProperty("transform_mode").GetInt32();
			if ((uint)transformIndex >= (uint)SkeletonBinary.TransformModeValues.Length) throw new InvalidDataException("Invalid serialized animation transform mode: " + transformIndex);
			RuntimeBinding.Set(bone, "transformMode", SkeletonBinary.TransformModeValues[transformIndex]); bones.Add(bone);
		}
		var slots = (ExposedList<SlotData>)RuntimeBinding.Get(data, "slots")!;
		foreach (var input in raw.GetProperty("slots").EnumerateArray())
		{
			var slot = new SlotData(slots.Count, input.GetProperty("name").GetString()!, bones.Items[input.GetProperty("bone").GetInt32()]);
			RuntimeBinding.Set(slot, "attachmentName", input.GetProperty("attachmentName").GetString()); slots.Add(slot);
		}
		foreach (var input in raw.GetProperty("constraints").EnumerateArray())
		{
			string kind = input.GetProperty("kind").GetString()!;
			object constraint = kind switch { "Transform" => new TransformConstraintData(input.GetProperty("name").GetString()!), "Path" => new PathConstraintData(input.GetProperty("name").GetString()!), _ => throw new NotSupportedException("Unrepresented animation constraint " + kind) };
			RuntimeBinding.Set(constraint, "order", input.GetProperty("order").GetInt32());
			foreach (var index in input.GetProperty("bones").EnumerateArray()) ((ExposedList<BoneData>)RuntimeBinding.Get(constraint, "bones")!).Add(bones.Items[index.GetInt32()]);
			RuntimeBinding.Set(constraint, "target", kind == "Transform" ? (object)bones.Items[input.GetProperty("target").GetInt32()] : slots.Items[input.GetProperty("target").GetInt32()]);
			foreach (var field in input.EnumerateObject().Where(p => p.Name is not ("kind" or "name" or "order" or "bones" or "target")))
			{
				var target = constraint.GetType().GetField(field.Name, RuntimeBinding.Members) ?? throw new InvalidDataException("Unrepresented constraint field " + field.Name);
				object value = target.FieldType == typeof(bool) ? field.Value.GetBoolean() : target.FieldType.IsEnum ? Enum.ToObject(target.FieldType, field.Value.GetInt32()) : field.Value.GetSingle() * (field.Name is "offsetX" or "offsetY" || field.Name == "position" && input.GetProperty("positionMode").GetInt32() == 0 || field.Name == "spacing" && input.GetProperty("spacingMode").GetInt32() <= 1 ? 0.5f : 1f);
				target.SetValue(constraint, value);
			}
			if (constraint is TransformConstraintData transform) ((ExposedList<TransformConstraintData>)RuntimeBinding.Get(data, "transformConstraints")!).Add(transform);
			else ((ExposedList<PathConstraintData>)RuntimeBinding.Get(data, "pathConstraints")!).Add((PathConstraintData)constraint);
		}
		var skins = (ExposedList<Skin>)RuntimeBinding.Get(data, "skins")!;
		foreach (var input in new[] { raw.GetProperty("default_skin") }.Concat(raw.GetProperty("named_skins").EnumerateArray()))
		{
			if (input.ValueKind == JsonValueKind.Null) continue;
			var skin = new Skin(input.GetProperty("name").GetString()!);
			foreach (var slot in input.GetProperty("slots").EnumerateArray())
				foreach (var item in slot.GetProperty("attachments").EnumerateArray())
				{
					string kind = item.GetProperty("type").GetString()!, name = item.GetProperty("name").GetString()!;
					Attachment attachment = kind switch { "Path" => new PathAttachment(name), "BoundingBox" => new BoundingBoxAttachment(name), "Mesh" or "LinkedMesh" or "Clipping" => new VertexAttachment(name), "Region" or "Point" => new RenderAttachment(name), _ => throw new InvalidDataException("Unrepresented attachment " + kind) };
					if (attachment is BoundingBoxAttachment box)
					{
						var rawBones = item.GetProperty("bones");
						int[]? boxBones = rawBones.ValueKind == JsonValueKind.Null ? null : rawBones.EnumerateArray().Select(x => x.GetInt32()).ToArray();
						RuntimeBinding.Set(box, "bones", boxBones);
						RuntimeBinding.Set(box, "worldVerticesLength", item.GetProperty("vertex_count").GetInt32() << 1);
						RuntimeBinding.Set(box, "vertices", item.GetProperty("vertices").EnumerateArray().Select((x, i) => x.GetSingle() * (boxBones is not null && i % 3 == 2 ? 1f : 0.5f)).ToArray());
					}
					if (attachment is PathAttachment path)
					{
						RuntimeBinding.Set(path, "closed", item.GetProperty("closed").GetBoolean()); RuntimeBinding.Set(path, "constantSpeed", item.GetProperty("constantSpeed").GetBoolean());
						RuntimeBinding.Set(path, "worldVerticesLength", item.GetProperty("vertex_count").GetInt32() << 1);
						var inputBones = item.GetProperty("bones"); int[]? weights = inputBones.ValueKind == JsonValueKind.Null ? null : inputBones.EnumerateArray().Select(x => x.GetInt32()).ToArray();
						RuntimeBinding.Set(path, "bones", weights);
						RuntimeBinding.Set(path, "vertices", item.GetProperty("vertices").EnumerateArray().Select((x, i) => x.GetSingle() * (weights is not null && i % 3 == 2 ? 1f : 0.5f)).ToArray());
						RuntimeBinding.Set(path, "lengths", item.GetProperty("lengths").EnumerateArray().Select(x => x.GetSingle() * 0.5f).ToArray());
					}
					skin.AddAttachment(slot.GetProperty("slot_index").GetInt32(), item.GetProperty("key").GetString()!, attachment);
				}
			skins.Add(skin); if (skins.Count == 1 && raw.GetProperty("default_skin").ValueKind != JsonValueKind.Null) data.DefaultSkin = skin;
		}
		return data;
	}
	/// <summary>
	/// Pose data consumed by supported battle logic. Bones: names its bone readers can receive,
	/// bones of constraints, collision and path attachments, and their ancestors. Slots: path constraint targets, whose
	/// current attachment only the path constraint reads. Other bones and slot attachments only drive rendering.
	/// </summary>
	private static (bool[] Bones, bool[] Slots) LogicPose(JsonElement raw, IReadOnlySet<string> names)
	{
		var bones = raw.GetProperty("bones"); var keep = new bool[bones.GetArrayLength()]; var parents = new int[keep.Length];
		for (int i = 0; i < keep.Length; i++)
		{
			string name = bones[i].GetProperty("name").GetString()!; parents[i] = bones[i].GetProperty("parent").GetInt32();
			keep[i] = name is "Center" or "State" || names.Contains(name) || IsPartsBone(name);
		}
		var slots = raw.GetProperty("slots"); var keepSlots = new bool[slots.GetArrayLength()];
		int SlotBone(int slot) => slots[slot].GetProperty("bone").GetInt32();
		void Weighted(JsonElement weights)
		{
			if (weights.ValueKind == JsonValueKind.Null) return;
			for (int i = 0, length = weights.GetArrayLength(); i < length;)
				for (int n = weights[i++].GetInt32(); n > 0; n--) keep[weights[i++].GetInt32()] = true;
		}
		var pathSlots = new HashSet<int>();
		foreach (var constraint in raw.GetProperty("constraints").EnumerateArray())
		{
			foreach (var index in constraint.GetProperty("bones").EnumerateArray()) keep[index.GetInt32()] = true;
			int target = constraint.GetProperty("target").GetInt32();
			if (constraint.GetProperty("kind").GetString() == "Transform") keep[target] = true;
			else { keep[SlotBone(target)] = true; pathSlots.Add(target); keepSlots[target] = true; }
		}
		foreach (var skin in new[] { raw.GetProperty("default_skin") }.Concat(raw.GetProperty("named_skins").EnumerateArray()))
		{
			if (skin.ValueKind == JsonValueKind.Null) continue;
			foreach (var slot in skin.GetProperty("slots").EnumerateArray())
				foreach (var item in slot.GetProperty("attachments").EnumerateArray())
				{
					string kind = item.GetProperty("type").GetString()!; int index = slot.GetProperty("slot_index").GetInt32();
					if (kind == "BoundingBox" || kind == "Path" && pathSlots.Contains(index)) { keep[SlotBone(index)] = true; Weighted(item.GetProperty("bones")); }
				}
		}
		for (int i = keep.Length - 1; i > 0; i--)
			if (keep[i])
			{
				if ((uint)parents[i] >= (uint)i) throw new InvalidDataException("animation bones must follow their parents.");
				keep[parents[i]] = true;
			}
		return (keep, keepSlots);
	}
	/// <summary>PartsData.Initialize reads <c>Center_{Index:D2}</c> and <c>State_{Index:D2}</c>.</summary>
	private static bool IsPartsBone(string name)
	{
		int start = name.StartsWith("Center_", StringComparison.Ordinal) ? 7 : name.StartsWith("State_", StringComparison.Ordinal) ? 6 : -1;
		if (start < 0 || name.Length - start < 2) return false;
		for (int i = start; i < name.Length; i++) if (name[i] is < '0' or > '9') return false;
		return true;
	}
	/// <summary>
	/// Timeline columns hold a zlib-compressed <c>SpineAnimation</c> (Schema/spine_animation.proto) with every authored frame.
	/// Frames are built only for timelines a battle reader reaches; the rest keep their kind, index and frame count.
	/// </summary>
	private static SpineAnimation ParseTimelines(object value)
	{
		using var zlib = new System.IO.Compression.ZLibStream(new MemoryStream(value as byte[] ?? throw new InvalidDataException("Prefab timeline column must be a zlib blob.")), System.IO.Compression.CompressionMode.Decompress);
		var animation = SpineAnimation.Parser.ParseFrom(zlib);
		if (animation.FormatVersion != 1) throw new InvalidDataException("Unsupported spine animation format version: " + animation.FormatVersion);
		return animation;
	}
	private static ExposedList<Timeline> ReadAnimation(SpineAnimation raw, SkeletonData data, (bool[] Bones, bool[] Slots) keep)
	{
		var result = new ExposedList<Timeline>();
		foreach (var input in raw.Timelines)
		{
			var kind = input.Kind; int count = input.Times.Count, index = input.Index;
			// Timelines of bones and slot attachments no battle reader reaches keep only their identity, like the other render-only timelines.
			bool render = kind is SpineTimelineKind.Rotate or SpineTimelineKind.Translate or SpineTimelineKind.Scale or SpineTimelineKind.Shear ? !keep.Bones[index] : kind == SpineTimelineKind.Attachment && !keep.Slots[index];
			Timeline timeline = kind switch
			{
				SpineTimelineKind.Rotate when render => new PropertyOnly(new RotateTimeline(count, index).PropertyId),
				SpineTimelineKind.Translate when render => new PropertyOnly(new TranslateTimeline(count, index).PropertyId),
				SpineTimelineKind.Scale when render => new PropertyOnly(new ScaleTimeline(count, index).PropertyId),
				SpineTimelineKind.Shear when render => new PropertyOnly(new ShearTimeline(count, index).PropertyId),
				SpineTimelineKind.Attachment when render => new RenderAttachments(count, index),
				SpineTimelineKind.Rotate => new RotateTimeline(count, index),
				SpineTimelineKind.Translate => new TranslateTimeline(count, index),
				SpineTimelineKind.Scale => new ScaleTimeline(count, index),
				SpineTimelineKind.Shear => new ShearTimeline(count, index),
				SpineTimelineKind.Attachment => new AttachmentTimeline(count, index),
				SpineTimelineKind.TransformConstraint => new TransformConstraintTimeline(count, index),
				SpineTimelineKind.PathConstraintPosition => new PathConstraintPositionTimeline(count, index),
				SpineTimelineKind.PathConstraintSpacing => new PathConstraintSpacingTimeline(count, index),
				SpineTimelineKind.PathConstraintMix => new PathConstraintMixTimeline(count, index),
				SpineTimelineKind.Color => new PropertyOnly(new ColorTimeline(count, index).PropertyId),
				SpineTimelineKind.TwoColor => new PropertyOnly(new TwoColorTimeline(count, index).PropertyId),
				SpineTimelineKind.DrawOrder => new RenderDrawOrder(count),
				SpineTimelineKind.Event => new UnsubscribedEvents(input.Times),
				SpineTimelineKind.Deform => Deform(index, input.DeformSlot, input.DeformAttachment, count, data),
				_ => throw new NotSupportedException("Unrepresented animation timeline " + kind)
			};
			if (timeline is not PropertyOnly && timeline is not UnsubscribedEvents && timeline is not RenderAttachments && timeline is not RenderDrawOrder)
			{
				float scale = kind == SpineTimelineKind.Translate ? 0.5f : 1f;
				if (kind is SpineTimelineKind.PathConstraintPosition or SpineTimelineKind.PathConstraintSpacing)
				{
					var constraint = ((ExposedList<PathConstraintData>)RuntimeBinding.Get(data, "pathConstraints")!).Items[index];
					if (kind == SpineTimelineKind.PathConstraintPosition ? (int)(PositionMode)RuntimeBinding.Get(constraint, "positionMode")! == 0 : (int)(SpacingMode)RuntimeBinding.Get(constraint, "spacingMode")! <= 1) scale = 0.5f;
				}
				var times = input.Times; var values = input.Values; var curveTypes = input.CurveTypes.Span; var curveParams = input.CurveParams;
				int width = timeline switch { RotateTimeline or PathConstraintPositionTimeline => 1, TranslateTimeline or PathConstraintMixTimeline => 2, TransformConstraintTimeline => 4, _ => 0 };
				if (values.Count != count * width) throw new InvalidDataException($"Animation timeline {kind} carries {values.Count} values for {count} frames.");
				if (timeline is CurveTimeline && curveTypes.Length != Math.Max(count - 1, 0)) throw new InvalidDataException($"Animation timeline {kind} carries {curveTypes.Length} curves for {count} frames.");
				if (timeline is AttachmentTimeline && (input.Names.Count != count || input.NamesAbsent.Count != count)) throw new InvalidDataException("Attachment timeline names do not match its frames.");
				if (timeline is DeformTimeline && (input.Counts.Count != count || input.Integers.Count != count)) throw new InvalidDataException("Deform timeline offsets do not match its frames.");
				float[]? baseVertices = null; int[]? deformBones = null; int parameter = 0, vertex = 0;
				for (int i = 0; i < count; i++)
				{
					float time = times[i];
					if (timeline is AttachmentTimeline attachment) attachment.SetFrame(i, time, input.NamesAbsent[i] ? null! : input.Names[i]);
					else if (timeline is DeformTimeline deform)
					{
						if (baseVertices is null)
						{
							var path = (PathAttachment)data.Skins.Items[index].GetAttachment(input.DeformSlot, input.DeformAttachment);
							baseVertices = (float[])RuntimeBinding.Get(path, "vertices")!; deformBones = (int[]?)RuntimeBinding.Get(path, "bones");
						}
						int supplied = input.Counts[i];
						var vertices = supplied == 0 ? deformBones is null ? baseVertices : new float[baseVertices.Length / 3 * 2] : new float[deformBones is null ? baseVertices.Length : baseVertices.Length / 3 * 2];
						if (supplied != 0)
						{
							int start = input.Integers[i]; for (int k = 0; k < supplied; k++) vertices[start++] = input.Vertices[vertex + k] * 0.5f;
							if (deformBones is null) for (int j = 0; j < vertices.Length; j++) vertices[j] += baseVertices[j];
						}
						vertex += supplied;
						deform.SetFrame(i, time, vertices);
					}
					else
					{
						int o = i * width;
						switch (timeline)
						{
							case RotateTimeline rotate: rotate.SetFrame(i, time, values[o] * scale); break;
							case TranslateTimeline translate: translate.SetFrame(i, time, values[o] * scale, values[o + 1] * scale); break;
							case TransformConstraintTimeline transform: transform.SetFrame(i, time, values[o] * scale, values[o + 1] * scale, values[o + 2] * scale, values[o + 3] * scale); break;
							case PathConstraintPositionTimeline position: position.SetFrame(i, time, values[o] * scale); break;
							case PathConstraintMixTimeline mix: mix.SetFrame(i, time, values[o] * scale, values[o + 1] * scale); break;
							default: throw new InvalidDataException("Animation timeline " + kind + " has no frame reader.");
						}
					}
					if (timeline is CurveTimeline curve && i < count - 1)
					{
						int curveType = curveTypes[i];
						if (curveType == 1) curve.SetStepped(i);
						else if (curveType == 2) { curve.SetCurve(i, curveParams[parameter], curveParams[parameter + 1], curveParams[parameter + 2], curveParams[parameter + 3]); parameter += 4; }
						else if (curveType != 0) throw new InvalidDataException("Unknown animation curve " + curveType);
					}
				}
			}
			result.Add(timeline);
		}
		return result;
	}
	public static void RequireUnsubscribedEvents(BattleAnimKitController controller)
	{
		if (RuntimeBinding.Get(controller.state, "Event") is not null) throw new NotSupportedException("animation event subscriptions require full event data; this resource projection provides ordered event identity only.");
		var tracks = (ExposedList<TrackEntry>)RuntimeBinding.Get(controller.state, "tracks")!;
		foreach (var entry in tracks)
			for (var current = entry; current is not null; current = (TrackEntry?)RuntimeBinding.Get(current, "mixingFrom"))
			{
				if (zeroMixRenderEffects.TryGetValue(controller, out _) && (float)RuntimeBinding.Get(current, "mixDuration")! != 0)
					throw new NotSupportedException("Projected effect deform cannot acquire a nonzero runtime mix.");
				if (RuntimeBinding.Get(current, "Event") is not null) throw new NotSupportedException("Track event consumer is outside the unsubscribed animation projection.");
			}
	}
	private static Timeline Deform(int skin, int slot, string name, int count, SkeletonData data)
	{
		var attachment = data.Skins.Items[skin].GetAttachment(slot, name);
		if (attachment is not VertexAttachment vertex) throw new AnimationInputReadException("Missing/wrong Deform attachment.");
		var timeline = new DeformTimeline(count, slot, vertex);
		return attachment is PathAttachment ? timeline : new PropertyOnly(timeline.PropertyId);
	}
	private sealed class AnimationInputReadException(string message) : Exception(message);
	// The original DAMAGE callback finds EventTimeline and reads its first authored time.
	// Event payload delivery remains outside the explicitly unsubscribed host mode.
	// Only timeline identity and timing are supplied here; event names and payloads must come from explicit inputs.
	private sealed class UnsubscribedEvents : EventTimeline, Timeline
	{
		public UnsubscribedEvents(IReadOnlyList<float> times) : base(times.Count)
		{
			RuntimeBinding.Set(this, "frames", times.ToArray());
		}
		public new void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<global::OpenPCR.AnimKit.Event> firedEvents, float alpha, MixPose pose, MixDirection direction) { }
	}
	private sealed class RenderAttachment(string name) : Attachment(name);
	// AnimationState.ApplyMixingFrom skips attachment and draw order timelines by type before adding to totalAlpha, so
	// render-only ones keep their type and property identity and only their Apply is empty.
	private sealed class RenderAttachments(int frameCount, int slotIndex) : AttachmentTimeline(frameCount, slotIndex), Timeline
	{
		public new void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<global::OpenPCR.AnimKit.Event> firedEvents, float alpha, MixPose pose, MixDirection direction) { }
	}
	private sealed class RenderDrawOrder(int frameCount) : DrawOrderTimeline(frameCount), Timeline
	{
		public new void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<global::OpenPCR.AnimKit.Event> firedEvents, float alpha, MixPose pose, MixDirection direction) { }
	}
	private sealed class PropertyOnly(int propertyId) : Timeline
	{
		public int PropertyId => propertyId;
		public void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<global::OpenPCR.AnimKit.Event> firedEvents, float alpha, MixPose pose, MixDirection direction) { }
	}
}
