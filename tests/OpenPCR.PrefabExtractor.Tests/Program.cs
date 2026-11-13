using global::OpenPCR.PrefabExtractor;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Pcr.Prefab;

int assertions = 0;
void Check(bool condition, string message) { assertions++; if (!condition) throw new Exception(message); }
void Reject(Action action, string message)
{
	try { action(); } catch (Exception ex) when (ex is InvalidDataException or ArgumentException or OverflowException or SqliteException) { assertions++; return; }
	throw new Exception("Accepted invalid input: " + message);
}
void JsonCheck(string a, string b, bool equal)
{
	using var left = JsonDocument.Parse(a); using var right = JsonDocument.Parse(b);
	Check(DatabaseComparison.JsonEqual(left.RootElement, right.RootElement) == equal, "JSON comparison: " + a + " / " + b);
}
void JsonReject(string a, string b)
{
	using var left = JsonDocument.Parse(a); using var right = JsonDocument.Parse(b);
	Reject(() => DatabaseComparison.JsonEqual(left.RootElement, right.RootElement), "JSON comparison: " + a + " / " + b);
}

var legacy = ManifestBuilder.ParseLine("a/test.unity3d,0123456789abcdef0123456789abcdef,asset,123,0")!;
Check(legacy.PoolKey == legacy.Md5 && legacy.Size == 123, "legacy manifest address");
var extended = ManifestBuilder.ParseLine("a/test.unity3d,0123456789abcdef0123456789abcdef,abcd0123456789ef,asset,123,0")!;
Check(extended.PoolKey == "abcd0123456789ef", "short pool key must not use MD5");
Check(ManifestBuilder.ParseLine("  ") == null, "blank manifest line");
Reject(() => ManifestBuilder.ParseLine("bad,row"), "short manifest row");
Reject(() => ManifestBuilder.ParseLine("../escape,0123456789abcdef0123456789abcdef,asset,1,0"), "manifest traversal");
Reject(() => ManifestBuilder.ParseLine("a/test,0123456789abcdef0123456789abcdef,asset,-1,0"), "negative size");
Reject(() => Options.Parse(["--manifest", "m", "--bundles", "b", "--output", "o", "--all", "--keys", "100101"]), "ambiguous keys");
Reject(() => Options.Parse(["--manifest", "m", "--bundles", "b", "--output", "o", "--only", "unknown"]), "unknown source");
Reject(() => Options.Parse(["--manifest", "m", "--bundles", "b", "--output", "o", "--all", "--jobs", "0"]), "invalid concurrency");
string[] offlineArgs = ["--manifest", "m", "--bundles", "b", "--output", "o", "--all"];
Check(Options.Parse(offlineArgs).ResourceRoot == null, "offline extraction has no implicit server");
Reject(() => Options.Parse([.. offlineArgs, "--download"]), "download without explicit server");
Reject(() => Options.Parse(["--build-manifest", "123", "--output", "o"]), "manifest without explicit server");
Reject(() => Options.Parse([.. offlineArgs, "--download", "--resource-root", "http://fixture.invalid"]), "non-HTTPS download root");
Check(Options.Parse([.. offlineArgs, "--download", "--resource-root", "https://fixture.invalid"]).Download, "explicit download root");
Check(Options.Parse(["--build-manifest", "123", "--output", "o", "--resource-root", "https://fixture.invalid"]).BuildManifest == "123", "explicit manifest root");
JsonCheck("{\"a\":1.0,\"b\":[null,true]}", "{\"b\":[null,true],\"a\":1}", true);
Check(Sources.BundleNames("unit_spine", "106361", null).Single() == "spine_sdnormal_106361.unity3d", "existing normal unit-spine keys");
Check(Sources.BundleNames("unit_spine", "shadow_106361", null).Single() == "spine_sdshadow_106361.unity3d", "distinct shadow unit-spine keys");
JsonCheck("9007199254740992", "9007199254740993", false);
JsonCheck("9007199254740993", "9007199254740992.0", false);
JsonCheck("9223372036854775807", "9.223372036854775808e18", false);
JsonCheck("18446744073709551615", "18446744073709551615.0", true);
JsonCheck("18446744073709551615", "18446744073709551616.0", false);
JsonCheck("184467440737095516150", "1.8446744073709551615e20", true);
JsonCheck("-18446744073709551615", "-1.8446744073709551615e19", true);
JsonCheck("1e400", "2e400", false);
JsonCheck("1e400", "10e399", true);
JsonCheck("1e999999999999999999999", "10e999999999999999999998", true);
JsonCheck("1", "1.00000000000000001", false);
JsonCheck("0.1", "0.10000000000000001", true);
JsonCheck("1.25", "1.2500000000000000", true);
JsonCheck("1e-300", "1.0000000000000000e-300", true);
JsonCheck("-0.0", "0", false);
JsonCheck("-0", "0.0", false);
JsonCheck("-0", "-0.0e999999999999999999999", true);
JsonCheck("0", "0.0", true);
JsonReject("{\"a\":1,\"a\":1}", "{\"a\":9,\"a\":1}");
JsonReject("{\"a\":1}", "{\"a\":1,\"a\":1}");
JsonReject("{\"nested\":[{\"a\":1,\"a\":1}]}", "{}");
JsonReject("{\"a\":1,\"\\u0061\":1}", "{}");
JsonReject("1e-9999", "2e-9999");
JsonReject(new string('9', 400) + ".1", new string('9', 400) + ".2");
JsonCheck("[1,2]", "[2,1]", false);
JsonCheck("{\"a\":0}", "{}", false);
Check(Tree.Json(-0.0) == "-0.0" && Tree.Json(-2.9802322387695312e-08) == "-2.9802322387695312E-08", "portable exact JSON doubles and negative zero");
Reject(() => Tree.Json(double.NaN), "nonfinite JSON resource input");
var signedKey = new CurveKey { InSlope = -0.0f, OutSlope = -0.0f };
Check(Convert.ToHexStringLower(PrefabSerialization.Encode(signedKey)) == "1d000000802500000080", "protobuf negative-zero presence and bits");
var transport = new UnitPrefab { PrefabId = 100101, Geometry = new Geometry { Scale = 1 }, Slots = { new SkillSlot { SlotName = SlotName.AttackDetail, Ordinal = -1, UseDefaultDelay = false, Effects = { new SkillEffect { ExecTimes = { -0.0f, 1.25f }, Prefab = new Pptr { PathId = -9007199254740993L } } } } } };
byte[] encoded = PrefabSerialization.Encode(transport);
var decoded = UnitPrefab.Parser.ParseFrom(encoded);
Check(encoded.SequenceEqual(PrefabSerialization.Encode(decoded)) && decoded.Slots[0].HasUseDefaultDelay && !decoded.Slots[0].UseDefaultDelay && BitConverter.SingleToInt32Bits(decoded.Slots[0].Effects[0].ExecTimes[0]) == int.MinValue, "protobuf nested, optional, zigzag and packed fields");
Tree SkillTree(Tree? zoom)
{
	var plual = Tree.Of(("StartDuration", 2.9), ("Duration", 0.3), ("To", 1.0), ("OffsetX", 0.0), ("OffsetY", 0.0), ("ZoomTarget", 0), ("StartEasingType", 1), ("UseYBottom", 0), ("BoneName", ""));
	zoom ??= Tree.Of(("Enable", 1), ("ConsiderScreenSize", 1), ("StartDelay", -0.0), ("StartDuration", 0.15), ("Duration", 1.55), ("EndDuration", 0.1), ("To", 0.8), ("ZoomTarget", 3), ("OffsetY", -1.5), ("OffsetX", 0.25), ("UseYBottom", 1), ("ZoomEffectList", new System.Collections.Generic.List<object?> { plual }), ("StartEasingType", 2), ("EndEasingType", 26), ("BoneName", "Head"));
	var tree = Tree.Of(("IsPrincessForm", 0), ("ParameterTarget", 0), ("SkillMotionType", 0), ("BlackOutTime", 4.6), ("CutInSkipTime", 0.0), ("ForcePlayNoTarget", 0), ("ForceComboDamage", 0), ("TrackDamageNum", 0), ("PauseStopState", 0),
		("ChangeSortDatas", new System.Collections.Generic.List<object?>()), ("BlurEffects", new System.Collections.Generic.List<object?>()),
		("SlowEffect", Tree.Of(("Enable", 0), ("StartDelay", 0.0), ("StartDuration", 0.0), ("Duration", 1.55), ("EndDuration", 0.1), ("To", 0.14))),
		("ScaleChangers", new System.Collections.Generic.List<object?> { Tree.Of(("Time", 0.0), ("Scale", 0.3), ("Duration", 0.0), ("Easing", 2)), Tree.Of(("Time", 1.1), ("Scale", 1.0), ("Duration", 0.5), ("Easing", 17)) }));
	if (zoom.Count != 0) tree.Add("ZoomEffect", zoom);
	return tree;
}
var authoredSkill = UnitPrefabBuilder.Skill(SkillTree(null), SlotName.UnionBurstList, 0);
var authoredZoom = authoredSkill.ZoomEffect;
Check(authoredZoom.Enable && authoredZoom.ConsiderScreenSize && authoredZoom.UseYBottom && authoredZoom.ZoomTarget == 3 && authoredZoom.StartEasingType == 2 && authoredZoom.EndEasingType == 26 && authoredZoom.BoneName == "Head"
	&& BitConverter.SingleToInt32Bits(authoredZoom.StartDelay) == int.MinValue && authoredZoom.OffsetY == -1.5f && authoredZoom.ZoomEffectList.Single().StartEasingType == 1 && authoredZoom.ZoomEffectList.Single().StartDuration == 2.9f,
	"zoom effect fields, enum values, nested list and negative zero");
Check(authoredSkill.ScaleChangersExtracted && authoredSkill.ScaleChangers.Select(s => (s.Time, s.Scale, s.Duration, s.Easing)).SequenceEqual([(0f, 0.3f, 0f, 2), (1.1f, 1f, 0.5f, 17)]), "ordered scale changers");
var reencoded = UnitPrefab.Parser.ParseFrom(PrefabSerialization.Encode(new UnitPrefab { Slots = { authoredSkill } })).Slots.Single();
Check(reencoded.SlowEffect.HasTo && reencoded.SlowEffect.To == 0.14f, "authored slow scale protobuf transport");
foreach (float target in new[] { 0f, -0.0f, 0.14f, 1f })
{
	var value = new SlowEffect { To = target };
	var slowDecoded = SlowEffect.Parser.ParseFrom(PrefabSerialization.Encode(value));
	Check(slowDecoded.HasTo && BitConverter.SingleToInt32Bits(slowDecoded.To) == BitConverter.SingleToInt32Bits(target), "slow target presence and Single bits");
}
var missingSlow = SkillTree(null); missingSlow.Node("SlowEffect").Remove("To");
Reject(() => UnitPrefabBuilder.Skill(missingSlow, SlotName.UnionBurstList, 0), "missing serialized slow target");
Check(reencoded.ZoomEffect.Equals(authoredZoom) && reencoded.ScaleChangers.SequenceEqual(authoredSkill.ScaleChangers), "zoom and scale changer protobuf transport");
Reject(() => UnitPrefabBuilder.Skill(SkillTree(new Tree()), SlotName.UnionBurstList, 0), "skill without serialized zoom effect");
AnimKitReader reader = new AnimKitReader([0x3f, 0x80, 0, 0, 1]);
Check(reader.Float() == 1 && reader.Bool(), "Spine big endian float and boolean");
Reject(() => reader.Byte(), "truncated Spine input");
Reject(() => new AnimKitReader([128, 128, 128, 128, 128]).Var(), "unterminated varint");
Reject(() => AnimKitBinary.Segments(new byte[32]), "bad CYSP magic");
var cysp = new byte[65]; "cysp"u8.CopyTo(cysp); BinaryPrimitives.WriteInt32LittleEndian(cysp.AsSpan(12), 1); BinaryPrimitives.WriteUInt64LittleEndian(cysp.AsSpan(40), 1);
Check(AnimKitBinary.Segments(cysp).Single() == (64, 1), "CYSP directory extent");
BinaryPrimitives.WriteUInt64LittleEndian(cysp.AsSpan(40), 2); Reject(() => AnimKitBinary.Segments(cysp), "CYSP size overflow");
AnimKitReader animationReader = new AnimKitReader([0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 230, 255, 255, 255, 15, 0]);
AnimKitAnimation drawAnimation = AnimKitAnimation.Read(animationReader);
Check(animationReader.Position == animationReader.Length && drawAnimation.Timelines.Timelines.Single().DrawOffsets.SequenceEqual(new uint[] { 0, 4294967270 }), "uint32 draw offset transport");
{
	// One slot with an Attachment timeline (a null name, then "a"); one bone with a Rotate timeline (-0 with a bezier, then 90).
	byte[] F(float value) { var bytes = new byte[4]; BinaryPrimitives.WriteSingleBigEndian(bytes, value); return bytes; }
	byte[] authored = new byte[] { 1, 2, 1, 0, 2 }.Concat(F(0)).Append((byte)0).Concat(F(0.5f)).Concat(new byte[] { 2, (byte)'a' })
		.Concat(new byte[] { 1, 3, 1, 0, 2 }).Concat(F(0)).Concat(F(-0f)).Append((byte)2).Concat(F(0.25f)).Concat(F(0)).Concat(F(0.75f)).Concat(F(1))
		.Concat(F(1)).Concat(F(90)).Concat(new byte[6]).ToArray();
	var authoredReader = new AnimKitReader(authored); var animation = AnimKitAnimation.Read(authoredReader);
	Check(authoredReader.Position == authoredReader.Length && animation.Duration == 1 && Tree.Json(animation.Properties) == "[[\"Attachment\",2],[\"Rotate\",3]]", "animation extent, duration and property identities");
	var stored = SpineAnimation.Parser.ParseFrom(ZProto.Unpack(ZProto.Pack(Google.Protobuf.MessageExtensions.ToByteArray(animation.Timelines))));
	Check(stored.FormatVersion == AnimKitAnimation.FormatVersion && stored.Timelines.Count == 2, "animation format version and timeline order");
	var names = stored.Timelines[0]; var rotate = stored.Timelines[1];
	Check(names.Kind == SpineTimelineKind.Attachment && names.Index == 2 && names.Names.SequenceEqual(new[] { "", "a" }) && names.NamesAbsent.SequenceEqual(new[] { true, false }) && names.CurveTypes.IsEmpty, "null attachment name stays distinct from an empty one");
	Check(rotate.Kind == SpineTimelineKind.Rotate && rotate.Index == 3 && rotate.Times.SequenceEqual(new[] { 0f, 1f }) && rotate.Values.Count == 2 && rotate.Values[1] == 90, "bone timeline frames");
	Check(BitConverter.SingleToInt32Bits(rotate.Values[0]) == BitConverter.SingleToInt32Bits(-0f), "negative zero survives the packed float column");
	Check(rotate.CurveTypes.ToByteArray().SequenceEqual(new byte[] { 2 }) && rotate.CurveParams.SequenceEqual(new[] { 0.25f, 0f, 0.75f, 1f }), "last frame has no curve; bezier parameters follow their frame");
	authored[18] = 9; Reject(() => AnimKitAnimation.Read(new AnimKitReader(authored)), "unknown curve type");
}

string scratch = Path.Combine(Path.GetTempPath(), "openpcr-prefab-tests-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(scratch);
try
{
	string comparisonPath = Path.Combine(scratch, "comparison.db");
	using (var comparison = new Database(comparisonPath)) comparison.Commit();
	using (var connection = new SqliteConnection("Data Source=" + comparisonPath + ";Pooling=False"))
	{
		connection.Open(); using var command = connection.CreateCommand();
		command.CommandText = "INSERT INTO spine_pose_setup VALUES('fixture', $data)";
		command.Parameters.AddWithValue("$data", ZJson.Pack("{\"a\":1,\"a\":1}"));
		command.ExecuteNonQuery();
	}
	Reject(() => DatabaseComparison.Compare(comparisonPath, comparisonPath), "identical database JSON with duplicate keys");
	Check(System.Text.Encoding.UTF8.GetString(ZJson.Unpack(ZJson.Pack("{\"名\":-0.0}"))) == "{\"名\":-0.0}", "zjson UTF-8 round trip");
	string packedPath = Path.Combine(scratch, "packed.db"), textPath = Path.Combine(scratch, "text.db"), otherPath = Path.Combine(scratch, "other.db");
	using (var packed = new Database(packedPath))
	{
		var setup = new Rows("spine"); setup.AddRow("spine_pose_setup", "fixture", "{\"bones\":[1.5]}"); packed.Write("spine", "fixture", setup);
		Reject(() => { var wrong = new Rows("spine"); wrong.AddRow("spine_pose_setup", "fixture", 1); packed.Write("spine", "fixture", wrong); }, "zjson column requires JSON text");
		Check(packed.Keys("spine_pose_setup", "skeleton_key").Count == 1, "rejected zjson value leaves the scope intact");
		byte[] message = Google.Protobuf.MessageExtensions.ToByteArray(new SpineAnimation { FormatVersion = 1 });
		var poses = new Rows("cysp"); poses.AddRow("cysp_pose", "fixture", "bundle", "idle", message); poses.Pack("cysp");
		Check(poses["cysp_pose"][0][3] is PackedProto, "zproto message packed on the parsing side");
		packed.Write("cysp", "fixture", poses);
		Reject(() => { var wrong = new Rows("cysp"); wrong.AddRow("cysp_pose", "fixture", "bundle", "idle", "[]"); packed.Write("cysp", "fixture", wrong); }, "zproto column requires a serialized message");
		using (var command = packed.Connection.CreateCommand())
		{
			command.CommandText = "SELECT typeof(timelines), timelines FROM cysp_pose";
			using var values = command.ExecuteReader(); values.Read();
			Check(values.GetString(0) == "blob" && ZProto.Unpack((byte[])values.GetValue(1)).AsSpan().SequenceEqual(message), "zproto column stored as zlib blob");
		}
		using (var command = packed.Connection.CreateCommand())
		{
			command.CommandText = "SELECT typeof(data), data FROM spine_pose_setup";
			using var values = command.ExecuteReader(); values.Read();
			Check(values.GetString(0) == "blob" && System.Text.Encoding.UTF8.GetString(ZJson.Unpack((byte[])values.GetValue(1))) == "{\"bones\":[1.5]}", "zjson column stored as zlib blob");
		}
		packed.Commit();
	}
	File.Copy(packedPath, textPath); File.Copy(packedPath, otherPath);
	using (var connection = new SqliteConnection("Data Source=" + textPath + ";Pooling=False"))
	{
		connection.Open(); using var command = connection.CreateCommand(); command.CommandText = "UPDATE spine_pose_setup SET data='{\"bones\":[1.5]}'"; command.ExecuteNonQuery();
	}
	using (var connection = new SqliteConnection("Data Source=" + otherPath + ";Pooling=False"))
	{
		connection.Open(); using var command = connection.CreateCommand(); command.CommandText = "UPDATE spine_pose_setup SET data=$data"; command.Parameters.AddWithValue("$data", ZJson.Pack("{\"bones\":[2.5]}")); command.ExecuteNonQuery();
	}
	Check(DatabaseComparison.Compare(packedPath, packedPath) == 0, "zjson comparison reads compressed JSON");
	Check(DatabaseComparison.Compare(textPath, packedPath) == 1, "text in a zjson column differs");
	Check(DatabaseComparison.Compare(otherPath, packedPath) == 1, "different compressed JSON differs");
	string protoPath = Path.Combine(scratch, "proto.db"); File.Copy(packedPath, protoPath);
	using (var connection = new SqliteConnection("Data Source=" + protoPath + ";Pooling=False"))
	{
		connection.Open(); using var command = connection.CreateCommand(); command.CommandText = "UPDATE cysp_pose SET timelines=$data";
		command.Parameters.AddWithValue("$data", ZProto.Pack(Google.Protobuf.MessageExtensions.ToByteArray(new SpineAnimation { FormatVersion = 2 }))); command.ExecuteNonQuery();
	}
	Check(DatabaseComparison.Compare(protoPath, packedPath) == 1, "different compressed message differs");
	string generatedManifest = Path.Combine(scratch, "generated.db");
	string Row(string name, int size) => name + ",0123456789abcdef0123456789abcdef,abcd0123456789ef,asset," + size + ",0";
	await ManifestBuilder.Build("123", generatedManifest, "https://fixture.invalid", url => Task.FromResult(url.EndsWith("manifest_assetmanifest", StringComparison.Ordinal) ? Row("manifest/child", 0) + "\n" + Row("a/repeated.unity3d", 1) : Row("a/repeated.unity3d", 2)));
	using (var connection = new SqliteConnection("Data Source=" + generatedManifest + ";Mode=ReadOnly;Pooling=False"))
	{
		connection.Open(); using var command = connection.CreateCommand(); command.CommandText = "SELECT size FROM manifest_entries WHERE url='a/repeated.unity3d'";
		Check((long)command.ExecuteScalar()! == 2, "manifest depth first overwrite");
		command.CommandText = "SELECT ver FROM manifest_meta"; Check((string)command.ExecuteScalar()! == "123", "explicit manifest version metadata");
	}
	byte[] manifestDigest = SHA256.HashData(File.ReadAllBytes(generatedManifest));
	try { await ManifestBuilder.Build("123", generatedManifest, "https://fixture.invalid", _ => Task.FromResult(Row("manifest/manifest_assetmanifest", 0))); throw new Exception("Accepted manifest cycle"); }
	catch (InvalidDataException) { assertions++; }
	Check(SHA256.HashData(File.ReadAllBytes(generatedManifest)).SequenceEqual(manifestDigest), "failed manifest refresh preserves existing file");
	Reject(() => { using var unknown = new Database(generatedManifest); }, "existing database without prefab deployment metadata");
	string databasePath = Path.Combine(scratch, "prefab.sqlite3");
	using (var database = new Database(databasePath))
	{
		Check(database.Keys("prefab_meta", "value").SequenceEqual(new[] { "10", "6000.0.58f2" }), "Host schema/unity deployment metadata");
		Reject(database.RequireNonempty, "empty full-build tables");
		var rows = new Rows("circle_effect"); rows.AddRow("circle_effect_prefab", "bundle", "name", 0, 0, -0.0, 1.25, 2.5, 0.0, 3.0);
		database.Write("circle_effect", "bundle", rows);
		Check(database.Keys("circle_effect_prefab", "bundle").Count == 1, "insert scope");
		using (var command = database.Connection.CreateCommand())
		{
			command.CommandText = "SELECT hex(duration),hex(radius) FROM circle_effect_prefab";
			using var values = command.ExecuteReader(); values.Read(); Check(values.GetString(0) == "00000080" && values.GetString(1) == "0000A03F", "f32 width, byte order and negative zero");
		}
		rows["circle_effect_prefab"].Add(rows["circle_effect_prefab"][0]); Reject(() => database.Write("circle_effect", "bundle", rows), "duplicate output identities");
		Check(database.Keys("circle_effect_prefab", "bundle").Count == 1, "duplicate rejected before deletion");
		database.Write("circle_effect", "bundle", new Rows("circle_effect"));
		Check(database.Keys("circle_effect_prefab", "bundle").Count == 0, "successful empty scope removes stale rows");
		rows = new Rows("firearm"); rows.AddRow("firearm_prefab", -9007199254740993L, 100101, 1, 1.25, -0.0, 0, 0, 0, 0, 0, 0, 0, "name"); database.Write("firearm", "100101", rows);
		using (var command = database.Connection.CreateCommand())
		{
			command.CommandText = "SELECT prefab_path_id,hex(duration),length(move_rate) FROM firearm_prefab";
			using var values = command.ExecuteReader(); values.Read(); Check(values.GetInt64(0) == -9007199254740993L && values.GetString(1) == "0000000000000080" && values.GetInt64(2) == 8, "full PPtr identity and f64 bits");
		}
		database.Commit();
	}
	string manifest = Path.Combine(scratch, "manifest_1.db"), bundles = Path.Combine(scratch, "bundles"); Directory.CreateDirectory(bundles);
	byte[] corrupt = [0, 1, 2, 3];
	using (var connection = new SqliteConnection("Data Source=" + manifest + ";Pooling=False"))
	{
		connection.Open(); using var command = connection.CreateCommand();
		command.CommandText = "CREATE TABLE manifest_entries(url TEXT,category TEXT,pool_key TEXT,md5 TEXT,size INTEGER,kind TEXT); INSERT INTO manifest_entries VALUES('a/all_battleunitprefab_100101.unity3d','AssetBundles/Android','abcd',$md5,4,'asset');";
		command.Parameters.AddWithValue("$md5", Convert.ToHexStringLower(MD5.HashData(corrupt))); command.ExecuteNonQuery();
		command.CommandText = "INSERT INTO manifest_entries VALUES('a/spine_sdnormal_106361.unity3d','AssetBundles/Android','abcd',$md5,4,'asset'),('a/spine_sdshadow_106361.unity3d','AssetBundles/Android','abcd',$md5,4,'asset');";
		command.ExecuteNonQuery();
	}
	byte[] original = SHA256.HashData(File.ReadAllBytes(databasePath));
	Check(Sources.Keys("unit_spine", new Resources(manifest, bundles, false, null), null!).SequenceEqual(["106361", "shadow_106361"]), "manifest discovery retains distinct normal/shadow keys");
	string[] shadowCli = ["--manifest", manifest, "--bundles", bundles, "--output", databasePath, "--only", "unit_spine", "--keys", "shadow_106361"];
	Check(await OpenPCR.PrefabExtractor.Program.Main(shadowCli) == 1, "missing offline shadow bundle fails");
	File.WriteAllBytes(Path.Combine(bundles, "spine_sdshadow_106361.unity3d"), corrupt);
	Check(await OpenPCR.PrefabExtractor.Program.Main(shadowCli) == 1, "malformed shadow bundle fails");
	Check(SHA256.HashData(File.ReadAllBytes(databasePath)).SequenceEqual(original), "failed shadow extraction preserves the published output");
	string[] cli = ["--manifest", manifest, "--bundles", bundles, "--output", databasePath, "--only", "unit_prefab", "--keys", "100101"];
	Check(await OpenPCR.PrefabExtractor.Program.Main(cli) == 1, "missing offline bundle fails");
	Check(SHA256.HashData(File.ReadAllBytes(databasePath)).SequenceEqual(original), "failed extraction preserves published output");
	File.WriteAllBytes(Path.Combine(bundles, "all_battleunitprefab_100101.unity3d"), [1, 2, 3, 4]);
	Check(await OpenPCR.PrefabExtractor.Program.Main(cli) == 1, "bad MD5 fails");
	File.WriteAllBytes(Path.Combine(bundles, "all_battleunitprefab_100101.unity3d"), corrupt);
	Check(await OpenPCR.PrefabExtractor.Program.Main(cli) == 1, "malformed Unity bundle fails");
	Check(SHA256.HashData(File.ReadAllBytes(databasePath)).SequenceEqual(original), "parse failure preserves published output");
	Check(!Directory.EnumerateFiles(scratch, "*.part").Any(), "temporary outputs removed on failure");
	// --update: recorded scope sources against a manifest.
	string md5 = Convert.ToHexStringLower(MD5.HashData(corrupt));
	BundleEntry Recorded(string name) => new(name, "AssetBundles/Android", "abcd", md5, 4);
	string updatePath = Path.Combine(scratch, "update.sqlite3");
	using (var database = new Database(updatePath))
	{
		foreach (var (key, bundle) in new[] { ("106361", "spine_sdnormal_106361.unity3d"), ("shadow_106361", "spine_sdshadow_106361.unity3d"), ("999999", "spine_sdnormal_999999.unity3d") })
		{
			var rows = new Rows("unit_spine"); rows.AddRow("unit_spine_data", key, bundle, "{}"); rows.Bundles.Add(Recorded(bundle)); database.Write("unit_spine", key, rows);
		}
		var replaced = new Rows("unit_spine"); replaced.AddRow("unit_spine_data", "999999", "spine_sdnormal_999999.unity3d", "{}"); replaced.Bundles.Add(Recorded("spine_sdnormal_999999.unity3d")); database.Write("unit_spine", "999999", replaced);
		Check(database.SourceBundles("unit_spine").Select(p => (p.Key, p.Value.Count)).SequenceEqual([("106361", 1), ("999999", 1), ("shadow_106361", 1)]), "source_bundle replaces each scope's records");
		var resources = new Resources(manifest, bundles, false, null);
		var plan = Update.For("unit_spine", ["106361", "shadow_106361", "777777"], database.SourceBundles("unit_spine"), resources);
		Check(plan.Changed.SequenceEqual(["777777"]) && plan.Removed.SequenceEqual(["999999"]), "update plan: unrecorded key changes, vanished key removed, matching records unchanged");
		var drifted = database.SourceBundles("unit_spine"); drifted["106361"] = [Recorded("spine_sdnormal_106361.unity3d") with { Md5 = new string('0', 32) }];
		drifted["shadow_106361"] = [Recorded("spine_sdshadow_106361.unity3d") with { Size = 5 }];
		Check(Update.For("unit_spine", ["106361", "shadow_106361"], drifted, resources).Changed.Count == 2, "update plan: MD5 or size drift changes the scope");
		Check(Update.For("unit_spine", ["106361"], new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<BundleEntry>> { ["106361"] = [Recorded("spine_sdshadow_106361.unity3d")] }, resources).Changed.Count == 1, "update plan: a named input missing from the records changes the scope");
		database.Commit();
	}
	string[] updateCli = ["--manifest", manifest, "--bundles", bundles, "--output", updatePath, "--update", "--only", "unit_spine"];
	Check(await OpenPCR.PrefabExtractor.Program.Main(updateCli) == 0, "update with unchanged records reads no bundle and succeeds");
	using (var check = new Database(updatePath))
	{
		Check(check.Keys("unit_spine_data", "skin_key").SequenceEqual(["106361", "shadow_106361"]) && !check.SourceBundles("unit_spine").ContainsKey("999999"), "update removes the scope of a vanished key");
	}
	string changedManifest = Path.Combine(scratch, "manifest_2.db"); File.Copy(manifest, changedManifest);
	using (var connection = new SqliteConnection("Data Source=" + changedManifest + ";Pooling=False"))
	{
		connection.Open(); using var command = connection.CreateCommand(); command.CommandText = "UPDATE manifest_entries SET md5='" + new string('0', 32) + "' WHERE url='a/spine_sdnormal_106361.unity3d'"; command.ExecuteNonQuery();
	}
	byte[] updated = SHA256.HashData(File.ReadAllBytes(updatePath));
	Check(await OpenPCR.PrefabExtractor.Program.Main(["--manifest", changedManifest, "--bundles", bundles, "--output", updatePath, "--update", "--only", "unit_spine"]) == 1, "update re-extracts a scope whose bundle changed");
	Check(SHA256.HashData(File.ReadAllBytes(updatePath)).SequenceEqual(updated), "failed update preserves the published output");
	Check(await OpenPCR.PrefabExtractor.Program.Main(["--manifest", manifest, "--bundles", bundles, "--output", Path.Combine(scratch, "absent.sqlite3"), "--update"]) == 1, "update requires an existing database");
	Check(await OpenPCR.PrefabExtractor.Program.Main(["--manifest", manifest, "--bundles", bundles, "--output", updatePath, "--update", "--all"]) == 1, "update rejects --all");
	Check(await OpenPCR.PrefabExtractor.Program.Main(["--manifest", manifest, "--bundles", bundles, "--output", updatePath, "--update", "--only", "unit_spine", "--keys", "106361"]) == 1, "update rejects --keys");
	using (var connection = new SqliteConnection("Data Source=" + databasePath + ";Pooling=False"))
	{
		connection.Open(); using var command = connection.CreateCommand(); command.CommandText = "UPDATE prefab_meta SET value='6' WHERE key='schema_version'"; command.ExecuteNonQuery();
	}
	Reject(() => { using var incompatible = new Database(databasePath); }, "incompatible existing deployment schema");
}
finally { Directory.Delete(scratch, true); }
Console.WriteLine("Prefab extractor tests: " + assertions + " assertions passed");
