using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace OpenPCR.PrefabExtractor;

// Floats widen to double here, just as UnityPy exposes a serialized Single as a Python float.
internal sealed class Tree : Dictionary<string, object?>
{
	private static readonly JsonSerializerOptions JsonOptions = new() { Converters = { new ExactDoubleConverter() } };
	public object Required(string name) => TryGetValue(name, out var value) && value != null ? value : throw new InvalidDataException("Missing serialized field: " + name);
	public Tree Node(string name) => (Tree)Required(name);
	public List<object?> Items(string name) => (List<object?>)Required(name);
	public IEnumerable<Tree> Nodes(string name) => Items(name).Cast<Tree>();
	public long Long(string name) => Convert.ToInt64(Required(name), CultureInfo.InvariantCulture);
	public int Int(string name) => checked((int)Long(name));
	public double Number(string name) => Convert.ToDouble(Required(name), CultureInfo.InvariantCulture);
	public bool Bool(string name) => Convert.ToBoolean(Required(name), CultureInfo.InvariantCulture);
	public string Text(string name) => (string)Required(name);
	public long Path(string name) => Node(name).Long("m_PathID");
	public static string Json(object? value) => JsonSerializer.Serialize(value, JsonOptions);
	public static Tree Of(params (string Key, object? Value)[] pairs)
	{
		var result = new Tree();
		foreach (var (key, value) in pairs) result.Add(key, value);
		return result;
	}
	private sealed class ExactDoubleConverter : JsonConverter<double>
	{
		public override double Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) => reader.GetDouble();
		public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
		{
			if (!double.IsFinite(value)) throw new InvalidDataException("Nonfinite serialized JSON input");
			// -0 parses as an integer in Python and loses its sign. G17 also avoids ambiguous
			// shortened decimals that .NET and Python can round to different doubles.
			writer.WriteRawValue(value == 0 && BitConverter.DoubleToInt64Bits(value) < 0 ? "-0.0" : value.ToString("G17", CultureInfo.InvariantCulture));
		}
	}
}

internal sealed class UnityBundle : IDisposable
{
	private readonly AssetsManager manager = new();
	public List<UnityObject> Objects { get; } = new();
	public Dictionary<string, UnityObject> Container { get; } = new(StringComparer.Ordinal);
	public List<string> Dependencies { get; } = new();
	public UnityBundle(byte[] bytes, string name)
	{
		try
		{
			var bundle = manager.LoadBundleFile(new MemoryStream(bytes, false), name, true);
			for (int i = 0; i < bundle.file.GetAllFileNames().Count; i++)
			{
				var file = manager.LoadAssetsFileFromBundle(bundle, i, false);
				if (file == null) continue; // .resS and other non-serialized bundle entries
				string version = file.file.Metadata.UnityVersion;
				if (version != "0.0.0" && version != UnitPrefabBuilder.UnityVersion) throw new InvalidDataException(name + ": unsupported Unity version " + version);
				if (!file.file.Metadata.TypeTreeEnabled) throw new InvalidDataException(name + ": embedded type tree required");
				foreach (var info in file.file.AssetInfos) Objects.Add(new UnityObject(this, file, info));
			}
			foreach (var asset in OfType(AssetClassID.AssetBundle))
			{
				foreach (var entry in asset.Tree.Nodes("m_Container"))
				{
					var pointer = entry.Node("second").Node("asset");
					// Unity's path map (and UnityPy's container) keeps the last serialized entry.
					Container[entry.Text("first")] = Resolve(asset, pointer);
				}
				Dependencies.AddRange(asset.Tree.Items("m_Dependencies").Cast<string>());
			}
		}
		catch { Dispose(); throw; }
	}
	public IEnumerable<UnityObject> OfType(AssetClassID type) => Objects.Where(o => o.Type == type);
	public UnityObject Resolve(UnityObject owner, Tree pointer)
	{
		int fileId = pointer.Int("m_FileID");
		long pathId = pointer.Long("m_PathID");
		if (fileId == 0) return Objects.Single(o => o.File == owner.File && o.Id == pathId);
		var external = manager.GetExtAsset(owner.File, fileId, pathId, true);
		if (external.info == null) throw new InvalidDataException("Unresolved PPtr " + fileId + ":" + pathId);
		return Objects.Single(o => o.File == external.file && o.Id == pathId);
	}
	public void Dispose() => manager.UnloadAll();
	private static object? ConvertField(AssetTypeValueField field)
	{
		if (field.TemplateField.IsArray) return field.Children.Select(ConvertField).ToList();
		if (field.TemplateField.ValueType == AssetValueType.Float) return (double)field.AsFloat;
		if (field.TemplateField.ValueType == AssetValueType.String) return field.AsString;
		if (field.TemplateField.ValueType == AssetValueType.ByteArray) return field.AsByteArray.Select(b => (object?)(int)b).ToList();
		if (field.TemplateField.ValueType != AssetValueType.None && field.Value != null) return field.AsObject;
		if (field.Children.Count == 1 && field.Children[0].TemplateField.IsArray) return ConvertField(field.Children[0]);
		var result = new Tree();
		foreach (var child in field.Children) result.Add(child.TemplateField.Name, ConvertField(child));
		return result;
	}
	internal sealed class UnityObject(UnityBundle bundle, AssetsFileInstance file, AssetFileInfo info)
	{
		private Tree? tree;
		public AssetsFileInstance File { get; } = file;
		public long Id => info.PathId;
		public AssetClassID Type => (AssetClassID)info.TypeId;
		public Tree Tree => tree ??= (Tree)ConvertField(bundle.manager.GetBaseField(File, info))!;
		public string? ClassName => Tree.Path("m_Script") == 0 ? null : bundle.Resolve(this, Tree.Node("m_Script")).Tree.Text("m_ClassName");
		public byte[] TextBytes()
		{
			if (Type != AssetClassID.TextAsset) throw new InvalidDataException("TextAsset required");
			var reader = File.file.Reader;
			reader.Position = info.GetAbsoluteByteOffset(File.file);
			byte[] raw = reader.ReadBytes(checked((int)info.ByteSize));
			int nameSize = BinaryPrimitives.ReadInt32LittleEndian(raw);
			int offset = checked((4 + nameSize + 3) & ~3);
			if (nameSize < 0 || offset < 4 || offset > raw.Length - 4) throw new InvalidDataException("Invalid TextAsset name extent");
			int size = BinaryPrimitives.ReadInt32LittleEndian(raw.AsSpan(offset));
			return raw.AsSpan(offset + 4, size).ToArray();
		}
	}
}
