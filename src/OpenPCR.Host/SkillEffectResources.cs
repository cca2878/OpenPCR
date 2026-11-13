using global::Elements;
using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Runtime.CompilerServices;
using Elements.Battle;

namespace OpenPCR.Host;

/// <summary>Authored prefab identities and particle lifetimes; never inferred from an action.</summary>
internal sealed class SkillEffectResources
{
	public readonly record struct AuthoredPosition(float X, float Y, float Z);
	public sealed record Resource(string Bundle, long PathId, string Name, string ComponentClass, bool Repeat,
		float? ParticleDuration, AuthoredPosition Position, string SerializedData);
	private readonly ResourceDatabases databases;
	private readonly Dictionary<(string Bundle, long PathId), Resource> resources = new();
	public SkillEffectResources(ResourceDatabases databases) => this.databases = databases;
	public Resource Read(string name) => ReadKey("prefab_name", name);
	public Resource Read(long pathId) => ReadKey("prefab_path_id", pathId);
	public Resource ReadChild(Resource parent, long id) => ReadKey("prefab_path_id", id, parent.Bundle);
	private Resource ReadKey(string column, object key, string? bundle = null)
	{
		using var command = databases.Prefab.CreateCommand();
		command.CommandText = "SELECT c.bundle,c.prefab_path_id,c.prefab_name,c.component_class,c.is_repeat,c.particle_path_id,c.root_position_x,c.root_position_y,c.root_position_z,d.data FROM skill_effect_component c JOIN skill_effect_component_data d USING(bundle,prefab_path_id,component_index) WHERE c." + column + "=$key" + (bundle is null ? "" : " AND c.bundle=$bundle") + " ORDER BY c.component_index";
		command.Parameters.AddWithValue("$key", key);
		if (bundle is not null) command.Parameters.AddWithValue("$bundle", bundle);
		using var reader = command.ExecuteReader();
		Resource? result = null;
		while (reader.Read())
		{
			var next = new Resource(reader.GetString(0), reader.GetInt64(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4) != 0,
				Duration(reader.GetInt64(5)), new AuthoredPosition(Scalar(reader[6]), Scalar(reader[7]), Scalar(reader[8])), reader.GetString(9));
			if (next.ComponentClass == nameof(EffectBoneTimeLineData))
				throw new NotSupportedException("EffectBoneTimeLineData requires its bone timeline producer and logic bone names: " + next.Name);
			if (result is null) result = next;
			else if (result.Bundle != next.Bundle && result with { Bundle = next.Bundle } != next) throw new InvalidDataException("Ambiguous authored effect identity: " + key);
		}
		if (result is null) throw new InvalidDataException("Missing authored effect: " + key);
		// A name or parent-scoped lookup cannot establish global path-ID uniqueness.
		// Resolve all candidates before consulting the bundle-local immutable resource cache.
		var identity = (result.Bundle, result.PathId);
		if (!resources.TryGetValue(identity, out var cached)) resources.Add(identity, cached = result);
		return cached;
	}
	private float? Duration(long particleId)
	{
		if (particleId == 0) return null;
		using var command = databases.Prefab.CreateCommand(); command.CommandText = "SELECT duration FROM skill_effect_particle WHERE particle_path_id=$id";
		command.Parameters.AddWithValue("$id", particleId); using var reader = command.ExecuteReader();
		byte[]? bytes = null;
		while (reader.Read())
		{
			var next = (byte[])reader[0];
			if (bytes is not null && !bytes.SequenceEqual(next)) throw new InvalidDataException("Ambiguous particle duration: " + particleId);
			bytes = next;
		}
		return bytes is null ? throw new InvalidDataException("Missing particle duration: " + particleId) : Scalar(bytes);
	}
	private static float Scalar(object input) => input is byte[] bytes && bytes.Length == 8 ? (float)BitConverter.ToDouble(bytes) : throw new InvalidDataException("Authored float must retain its float64 bytes.");
	public float CircleChildX(Resource resource, int index)
	{
		using var command = databases.Prefab.CreateCommand();
		command.CommandText = "SELECT local_position_x FROM circle_effect_child WHERE bundle=$bundle AND prefab_name=$name AND child_index=$index";
		command.Parameters.AddWithValue("$bundle", resource.Bundle); command.Parameters.AddWithValue("$name", resource.Name); command.Parameters.AddWithValue("$index", index);
		return command.ExecuteScalar() is byte[] bytes && bytes.Length == 4 ? BitConverter.ToSingle(bytes) : throw new InvalidDataException("Missing authored circle child position.");
	}
	public bool Enabled(Resource resource)
	{
		using var document = JsonDocument.Parse(resource.SerializedData);
		return document.RootElement.GetProperty("m_Enabled").GetInt32() switch
		{
			0 => false,
			1 => true,
			_ => throw new InvalidDataException("Invalid authored component enabled flag: " + resource.Name)
		};
	}
	private TweenPosition[] ReadTweeners(Resource resource, JsonElement pointers)
	{
		var result = new TweenPosition[pointers.GetArrayLength()];
		var instances = new Dictionary<long, TweenPosition>();
		int index = 0;
		foreach (var pointer in pointers.EnumerateArray())
		{
			long id = pointer.GetProperty("m_PathID").GetInt64();
			if (id == 0) { index++; continue; } // Native restart/pause still fail on this authored null slot.
			if (pointer.GetProperty("m_FileID").GetInt32() != 0) throw new NotSupportedException("Effect tweener requires its external serialized file.");
			if (!instances.TryGetValue(id, out var tween))
			{
				using var command = databases.Prefab.CreateCommand();
				command.CommandText = "SELECT component_class,data FROM skill_effect_tweener WHERE bundle=$bundle AND component_path_id=$id";
				command.Parameters.AddWithValue("$bundle", resource.Bundle); command.Parameters.AddWithValue("$id", id);
				using var reader = command.ExecuteReader();
				if (!reader.Read()) throw new InvalidDataException("Missing authored effect tweener: " + id);
				if (reader.GetString(0) != nameof(TweenPosition)) throw new NotSupportedException("Effect renderer projection requires TweenPosition.");
				using var document = JsonDocument.Parse(reader.GetString(1)); var data = document.RootElement;
				if (data.GetProperty("onFinished").GetArrayLength() != 0 || data.GetProperty("eventReceiver").GetProperty("m_PathID").GetInt64() != 0 || !string.IsNullOrEmpty(data.GetProperty("callWhenFinished").GetString()))
					throw new NotSupportedException("Authored effect tweener has completion callbacks.");
				// The callback-free renderer object has no engine registration, interpolator or clock.
				tween = (TweenPosition)RuntimeHelpers.GetUninitializedObject(typeof(TweenPosition));
				tween.onFinished = new List<EventDelegate>(); tween.callWhenFinished = data.GetProperty("callWhenFinished").GetString()!;
				PreparedEffectInputs.BindRendererOnlyTweener(tween, () => (tween.onFinished is null || tween.onFinished.Count == 0) && ReferenceEquals(tween.eventReceiver, null) && string.IsNullOrEmpty(tween.callWhenFinished));
				instances.Add(id, tween);
			}
			result[index++] = tween;
		}
		return result;
	}
	public SkillEffectCtrl Create(Resource resource, Func<Resource, long, GameObject>? childPrefab = null)
	{
		using var document = JsonDocument.Parse(resource.SerializedData); var input = document.RootElement;
		// Only the existing callback-free effect TweenPosition inputs have a renderer projection.
		if (input.GetProperty("TweenRotList").GetArrayLength() != 0) throw new NotSupportedException(resource.Name + " requires authored TweenRotList");
		// Resource component IDs retain their original spelling; only exact approved CLR type identities change.
		var type = resource.ComponentClass switch
		{
			"SpineController" => typeof(global::Elements.AnimKitController),
			"SpineManager" => typeof(global::Elements.AnimKitManager),
			"SpineResourceInfo" => typeof(global::Elements.AnimKitResourceInfo),
			"eSpineBinaryAnimationId" => typeof(global::Elements.eAnimKitBinaryAnimationId),
			"eSpineCharacterAnimeId" => typeof(global::Elements.eAnimKitCharacterAnimeId),
			"eSpineType" => typeof(global::Elements.eAnimKitType),
			"ChangeSpineSpeedEffect" => typeof(global::Elements.ChangeAnimKitSpeedEffect),
			"BattleSpineController" => typeof(global::Elements.BattleAnimKitController),
			"SpineCowFireArm" => typeof(global::Elements.AnimKitCowFireArm),
			"SpineFireArm" => typeof(global::Elements.AnimKitFireArm),
			"SpineNormalEffect" => typeof(global::Elements.AnimKitNormalEffect),
			"ExceptNGUIRoot" => typeof(global::Elements.ExceptUIRoot),
			"SpineDefine" => typeof(global::Elements.AnimKitDefine),
			"SpineConverterDefine" => typeof(global::Elements.AnimKitConverterDefine),
			"eSpineConvertCategoryType" => typeof(global::Elements.eAnimKitConvertCategoryType),
			"ResourceDefineSpine" => typeof(global::Elements.ResourceDefineAnimKit),
			"SpineTypeExtensions" => typeof(global::Elements.AnimKitTypeExtensions),
			"SpineNormalEffectStar3Shadow" => typeof(global::Elements.AnimKitNormalEffectStar3Shadow),
			"SpineGroundScale" => typeof(global::Elements.AnimKitGroundScale),
			"BattleUnitBaseSpineController" => typeof(global::Elements.BattleUnitBaseAnimKitController),
			"AnimKitController" or "AnimKitManager" or "AnimKitResourceInfo" or "eAnimKitBinaryAnimationId" or "eAnimKitCharacterAnimeId" or "eAnimKitType" or "ChangeAnimKitSpeedEffect" or "BattleAnimKitController" or "AnimKitCowFireArm" or "AnimKitFireArm" or "AnimKitNormalEffect" or "ExceptUiRoot" or "ExceptUIRoot" or "AnimKitDefine" or "AnimKitConverterDefine" or "eAnimKitConvertCategoryType" or "ResourceDefineAnimKit" or "AnimKitTypeExtensions" or "AnimKitNormalEffectStar3Shadow" or "AnimKitGroundScale" or "BattleUnitBaseAnimKitController" => throw new InvalidDataException("Unknown effect controller: " + resource.ComponentClass),
			_ => typeof(SkillEffectCtrl).Assembly.GetType("Elements." + resource.ComponentClass) ?? throw new InvalidDataException("Unknown effect controller: " + resource.ComponentClass)
		};
		if (!typeof(SkillEffectCtrl).IsAssignableFrom(type)) throw new InvalidDataException("Authored component is not a SkillEffectCtrl.");
		if (typeof(AnimKitNormalEffect).IsAssignableFrom(type) && type != typeof(AnimKitNormalEffect) && type != typeof(AnimKitNormalEffectStar3Shadow))
			throw new NotSupportedException("Derived effect requires its numerical geometry producer: " + resource.Name);
		if (type != typeof(SkillEffectCtrl) && type != typeof(FirearmCtrl) && type != typeof(FirearmCtrlEx)
			&& type != typeof(AnimKitNormalEffect) && type != typeof(AnimKitNormalEffectStar3Shadow)
			&& type != typeof(FieldEffectController) && type != typeof(CircleEffectController) && type != typeof(HitEffectController)
			&& type != typeof(ChangeColorEffect) && type != typeof(ChangeColorEffectEx) && type != typeof(ChangeAnimKitSpeedEffect) && type != typeof(ShadowSkinChangeEffectCtrl) && type != typeof(ShadowChangeEffectCtrl)
			&& type != typeof(AwakeUbNoneStopEffect) && type != typeof(BlurEffect) && type != typeof(CountDownEffectCtrl)
			&& type != typeof(MimiSwordEffectCtrl) && type != typeof(TargetNumEnableDisableEffect)
			&& type != typeof(AnimKitFireArm) && type != typeof(AnimKitCowFireArm) && type != typeof(TweenInitEffectCtrl) && type != typeof(ChangeRotateEffect))
			throw new NotSupportedException("Effect requires its authored field/lifecycle producer: " + resource.ComponentClass);
		Enabled(resource); // Validate independently of the explicit BattleManager effect driver.
		var effect = (SkillEffectCtrl)Activator.CreateInstance(type)!;
		effect.TweenerList = ReadTweeners(resource, input.GetProperty("TweenerList"));
		RuntimeBinding.Set(effect, "isRepeat", resource.Repeat);
		RuntimeBinding.Set(effect, "isCommon", input.GetProperty("isCommon").GetInt32() != 0);
		RuntimeBinding.Set(effect, "seType", (eBattleSkillSeType)input.GetProperty("seType").GetInt32());
		if (effect is MimiSwordEffectCtrl)
			RuntimeBinding.Set(effect, "stopAnimationName", input.GetProperty("stopAnimationName").GetString()!);
		if (effect is AwakeUbNoneStopEffect)
		{
			RuntimeBinding.Set(effect, "displayType", (AwakeUbNoneStopEffect.eDisplayType)input.GetProperty("displayType").GetInt32());
			RuntimeBinding.Set(effect, "startHpPercentage", input.GetProperty("startHpPercentage").GetSingle());
		}
		if (effect is BlurEffect)
		{
			var data = input.GetProperty("blueEffectData");
			RuntimeBinding.Set(effect, "blueEffectData", new BlurEffect.BlurEffectData
			{
				StartTime = data.GetProperty("StartTime").GetSingle(),
				Value = data.GetProperty("Value").GetSingle(),
				StartDuration = data.GetProperty("StartDuration").GetSingle(),
				Duration = data.GetProperty("Duration").GetSingle(),
				EndDuration = data.GetProperty("EndDuration").GetSingle()
			});
		}
		if (effect is CountDownEffectCtrl countdown)
		{
			countdown.CountEndEffectData = new List<CountEndEffectData>();
			foreach (var record in input.GetProperty("CountEndEffectData").EnumerateArray())
			{
				GameObject Resolve(string name)
				{
					var pointer = record.GetProperty(name); long id = pointer.GetProperty("m_PathID").GetInt64();
					if (id == 0) return null!;
					if (childPrefab is null) throw new InvalidOperationException("Countdown end effect requires a prepared resource producer.");
					// Python's verified end-effect adapter uses the unique deployed PathID
					// for external pointers; Read rejects conflicting bundle identities.
					var child = pointer.GetProperty("m_FileID").GetInt32() == 0 ? ReadChild(resource, id) : Read(id);
					return childPrefab(child, id);
				}
				countdown.CountEndEffectData.Add(new CountEndEffectData { Prefab = Resolve("Prefab"), PrefabLeft = Resolve("PrefabLeft"), Time = record.GetProperty("Time").GetSingle() });
			}
		}
		if (effect is ChangeAnimKitSpeedEffect)
		{
			foreach (string name in new[] { "startTimeScale", "startDuration", "duration", "endDuration", "timeScale" }) RuntimeBinding.Set(effect, name, input.GetProperty(name).GetSingle());
			foreach (string name in new[] { "startEasingType", "endEasingType" }) RuntimeBinding.Set(effect, name, (CustomEasing.eType)input.GetProperty(name).GetInt32());
		}
		if (effect is ChangeRotateEffect)
		{
			foreach (string name in new[] { "startAngle", "startDuration", "duration", "endDuration", "angle" }) RuntimeBinding.Set(effect, name, input.GetProperty(name).GetSingle());
			foreach (string name in new[] { "startEasingType", "endEasingType" }) RuntimeBinding.Set(effect, name, (CustomEasing.eType)input.GetProperty(name).GetInt32());
			var center = input.GetProperty("centerPos"); RuntimeBinding.Set(effect, "centerPos", new Vector2(center.GetProperty("x").GetSingle(), center.GetProperty("y").GetSingle()));
			RuntimeBinding.Set(effect, "reverseMode", input.GetProperty("reverseMode").GetInt32() != 0);
		}
		if (effect is ShadowSkinChangeEffectCtrl)
		{
			var settingType = type.GetNestedType("ShadowSkinTextureChange", RuntimeBinding.Members) ?? throw new InvalidDataException("Missing shadow skin setting declaration.");
			var settings = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(settingType))!;
			foreach (var source in input.GetProperty("shadowSkinTextureSetting").EnumerateArray())
			{
				var setting = RuntimeHelpers.GetUninitializedObject(settingType);
				RuntimeBinding.Set(setting, "SkinId", source.GetProperty("SkinId").GetInt32());
				// Child texture objects have no simulation consumer; retain authored setting count.
				settings.Add(setting);
			}
			RuntimeBinding.Set(effect, "shadowSkinTextureSetting", settings);
		}
		if (effect is ChangeColorEffect)
		{
			// Native/Python retain all three phase durations, alpha/aura gates and sort state.
			foreach (string name in new[] { "startDuration", "duration", "endDuration" }) RuntimeBinding.Set(effect, name, input.GetProperty(name).GetSingle());
			foreach (string name in new[] { "setSortBack", "exceptOwner" }) RuntimeBinding.Set(effect, name, input.GetProperty(name).GetInt32() != 0);
			foreach (string name in new[] { "additiveColor", "multiplyColor" })
			{
				var color = input.GetProperty(name);
				RuntimeBinding.Set(effect, name, new Color(color.GetProperty("r").GetSingle(), color.GetProperty("g").GetSingle(), color.GetProperty("b").GetSingle(), color.GetProperty("a").GetSingle()));
			}
		}
		if (effect is FirearmCtrl)
		{
			foreach (string name in new[] { "HitDelay", "MoveRate", "duration", "startRotate", "endRotate" }) RuntimeBinding.Set(effect, name, input.GetProperty(name).GetSingle());
			RuntimeBinding.Set(effect, "MoveType", (eMoveTypes)input.GetProperty("MoveType").GetInt32());
			static Vector3 Vector(JsonElement value) => new(value.GetProperty("x").GetSingle(), value.GetProperty("y").GetSingle(), value.GetProperty("z").GetSingle());
			var box = input.GetProperty("ColliderBox");
			var bounds = new Bounds(); bounds.center = Vector(box.GetProperty("m_Center")); bounds.extents = Vector(box.GetProperty("m_Extent"));
			RuntimeBinding.Set(effect, "ColliderBox", bounds);
		}
		GameObject ResolveProjectileChild(JsonElement pointer)
		{
			long id = pointer.GetProperty("m_PathID").GetInt64();
			if (id == 0) return null!;
			if (pointer.GetProperty("m_FileID").GetInt32() != 0 || childPrefab is null)
				throw new NotSupportedException("Projectile requires a same-bundle authored child effect producer.");
			var child = ReadChild(resource, id); return childPrefab(child, id);
		}
		if (effect is AnimKitFireArm)
		{
			foreach (string name in new[] { "WalkEffectPrefab", "WalkEffectPrefabLeft" })
				RuntimeBinding.Set(effect, name, input.GetProperty(name).EnumerateArray().Select(ResolveProjectileChild).ToList());
			foreach (string name in new[] { "skillId", "index", "sortOrder", "sortOrderType" })
			{
				var field = typeof(AnimKitFireArm).GetField(name, RuntimeBinding.Members)!;
				field.SetValue(effect, field.FieldType.IsEnum ? Enum.ToObject(field.FieldType, input.GetProperty(name).GetInt32()) : input.GetProperty(name).GetInt32());
			}
			foreach (string name in new[] { "moveDuration", "sortChangeTime" }) RuntimeBinding.Set(effect, name, input.GetProperty(name).GetSingle());
			RuntimeBinding.Set(effect, "isLeftDir", input.GetProperty("isLeftDir").GetInt32() != 0);
			var target = input.GetProperty("targetPosition");
			RuntimeBinding.Set(effect, "targetPosition", new Vector3(target.GetProperty("x").GetSingle(), target.GetProperty("y").GetSingle(), target.GetProperty("z").GetSingle()));
		}
		if (effect is AnimKitCowFireArm)
		{
			foreach (string name in new[] { "WalkEffectPrefab", "ReturnWalkEffectPrefab", "AttackEffectPrefab" }) RuntimeBinding.Set(effect, name, ResolveProjectileChild(input.GetProperty(name)));
			foreach (string name in new[] { "skillId", "index" }) RuntimeBinding.Set(effect, name, input.GetProperty(name).GetInt32());
			RuntimeBinding.Set(effect, "isLeftDir", input.GetProperty("isLeftDir").GetInt32() != 0);
			// A serialized input object, like UnitPrefabBindings.Shake; real reset/easing/RNG are mirror consumers.
			var shake = (ShakeEffect)RuntimeHelpers.GetUninitializedObject(typeof(ShakeEffect));
			var data = input.GetProperty("shake");
			shake.ShakeType = (Elements.Battle.Core.ShakeType)data.GetProperty("ShakeType").GetInt32();
			shake.TargetMotion = data.GetProperty("TargetMotion").GetInt32();
			foreach (string name in new[] { "StartTime", "Duration", "StartAmplitude", "EndAmplitude", "StartFrequency", "EndFrequency", "Rotation" }) RuntimeBinding.Set(shake, name, data.GetProperty(name).GetSingle());
			RuntimeBinding.Set(effect, "shake", shake);
		}
		if (effect is CircleEffectController circle)
		{
			foreach (string name in new[] { "duration", "Radius", "Roll", "Yaw", "angularVelocity" }) RuntimeBinding.Set(effect, name, input.GetProperty(name).GetSingle());
			circle.FollowXY = input.GetProperty("FollowXY").GetInt32() != 0;
			using var command = databases.Prefab.CreateCommand();
			command.CommandText = "SELECT child_index,local_position_x,start_pitch FROM circle_effect_child WHERE bundle=$bundle AND prefab_name=$name ORDER BY child_index";
			command.Parameters.AddWithValue("$bundle", resource.Bundle); command.Parameters.AddWithValue("$name", resource.Name);
			using var reader = command.ExecuteReader();
			circle.Children = new List<CircleEffectChild>();
			while (reader.Read())
			{
				if (reader.GetInt32(0) != circle.Children.Count) throw new InvalidDataException("Non-contiguous authored circle children.");
				circle.Children.Add(new CircleEffectChild { StartPitch = BitConverter.ToSingle((byte[])reader[2]) });
			}
			if (circle.Children.Count != input.GetProperty("Children").GetArrayLength()) throw new InvalidDataException("Circle child component identity/count mismatch.");
		}
		if (effect is FieldEffectController)
		{
			if (input.GetProperty("EndEffectPrefab").GetArrayLength() != 0)
				throw new NotSupportedException(resource.Name + " requires authored end-effect callbacks.");
			RuntimeBinding.Set(effect, "EndEffectPrefab", new List<PrefabWithTime>());
			RuntimeBinding.Set(effect, "IsBehindSortBackBoss", input.GetProperty("IsBehindSortBackBoss").GetInt32() != 0);
			RuntimeBinding.Set(effect, "DisplayCenter", input.GetProperty("DisplayCenter").GetInt32() != 0);
			RuntimeBinding.Set(effect, "EndDuration", input.GetProperty("EndDuration").GetSingle());
		}
		if (effect is AnimKitNormalEffect)
		{
			if (input.GetProperty("TweenPosition").GetProperty("m_PathID").GetInt64() != 0)
				throw new NotSupportedException(resource.Name + " requires tween callbacks.");
			foreach (string name in new[] { "skillId", "index", "sortOrder", "sortOrderType" })
			{
				var field = typeof(AnimKitNormalEffect).GetField(name, RuntimeBinding.Members)!;
				field.SetValue(effect, field.FieldType.IsEnum ? Enum.ToObject(field.FieldType, input.GetProperty(name).GetInt32()) : input.GetProperty(name).GetInt32());
			}
			foreach (string name in new[] { "Roop", "ConsiderCutInSkip", "useFrontToBack", "isFrontSort" }) RuntimeBinding.Set(effect, name, input.GetProperty(name).GetInt32() != 0);
			RuntimeBinding.Set(effect, "frontToBackTime", input.GetProperty("frontToBackTime").GetSingle());
			var children = new List<PrefabWithTime>();
			foreach (var record in input.GetProperty("BoneFollowEffectPrefab").EnumerateArray())
			{
				GameObject Resolve(string name)
				{
					var pointer = record.GetProperty(name);
					long id = pointer.GetProperty("m_PathID").GetInt64();
					if (id == 0) return null!;
					if (pointer.GetProperty("m_FileID").GetInt32() != 0 || childPrefab is null) throw new NotSupportedException("External/missing child effect resource producer.");
					var child = ReadChild(resource, id);
					if (child.ComponentClass != nameof(SkillEffectCtrl)) throw new NotSupportedException("Effect child requires numerical geometry/lifecycle producer: " + child.Name);
					return childPrefab(resource, id);
				}
				children.Add(new PrefabWithTime
				{
					Prefab = Resolve("Prefab"),
					PrefabLeft = Resolve("PrefabLeft"),
					Time = record.GetProperty("Time").GetSingle(),
					SummonSkillIndex = record.GetProperty("SummonSkillIndex").GetInt32(),
					TargetBoneName = databases.Prefabs.RequireLogicBone(record.GetProperty("TargetBoneName").GetString()!),
					IsTrackRotation = record.GetProperty("IsTrackRotation").GetInt32() != 0,
					EffectDifficlty = (PrefabWithTime.eEffectDifficulty)record.GetProperty("EffectDifficlty").GetInt32()
				});
			}
			RuntimeBinding.Set(effect, "BoneFollowEffectPrefab", children);
			if (effect is AnimKitNormalEffectStar3Shadow)
				foreach (string name in new[] { "Star1NormalIndex", "Star3NormalIndex", "Star6NormalIndex", "Star1ShadowIndex", "Star3ShadowIndex", "Star6ShadowIndex", "Star1SkinId", "Star3SkinId", "Star6SkinId" })
					RuntimeBinding.Set(effect, name, input.GetProperty(name).GetInt32());
		}
		// This collection is used exclusively for renderer-order outputs; no dummy renderers are created.
		RuntimeBinding.Set(effect, "particleRendererDictionary", new Dictionary<Renderer, SkillEffectCtrl.SortOrderAndOffset>());
		PreparedEffectInputs.Bind(effect, resource.ParticleDuration); return effect;
	}
}
