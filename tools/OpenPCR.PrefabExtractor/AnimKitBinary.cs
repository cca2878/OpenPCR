using global::OpenPCR.PrefabExtractor;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenPCR.PrefabExtractor;

// Resource decoding only. No scaling, pose evaluation, frame conversion or interpolation.
internal sealed class AnimKitReader(byte[] bytes, int position = 0)
{
	public int Position { get; private set; } = position;
	public int Length => bytes.Length;
	private ReadOnlySpan<byte> Take(int count)
	{
		if (count < 0 || count > bytes.Length - Position) throw new InvalidDataException("Spine extent exceeds input at " + Position);
		var result = bytes.AsSpan(Position, count); Position += count; return result;
	}
	public int Byte() => Take(1)[0];
	public int SignedByte() => unchecked((sbyte)Byte());
	public bool Bool() => Byte() != 0;
	public int Int() => BinaryPrimitives.ReadInt32BigEndian(Take(4));
	public double Float() => BinaryPrimitives.ReadSingleBigEndian(Take(4));
	public int Var(bool positive = true)
	{
		uint value = 0;
		for (int shift = 0; shift <= 28; shift += 7)
		{
			int b = Byte(); value |= (uint)(b & 127) << shift;
			if ((b & 128) == 0)
			{
				if (shift == 28 && (b & 240) != 0) throw new InvalidDataException("Spine varint overflow");
				return positive ? unchecked((int)value) : (int)(value >> 1) ^ -((int)value & 1);
			}
		}
		throw new InvalidDataException("Invalid Spine varint");
	}
	public string? String() { int size = Var(); return size == 0 ? null : Encoding.UTF8.GetString(Take(checked(size - 1))); }
	public long UnsignedVar() => unchecked((uint)Var());
	public double[] Floats(int count) { var result = new double[count]; for (int i = 0; i < count; i++) result[i] = Float(); return result; }
	public int[] Ints() { int count = Var(); var result = new int[count]; for (int i = 0; i < count; i++) result[i] = Var(); return result; }
	public void Shorts() { Take(checked(Var() * 2)); }
}

internal static class AnimKitBinary
{
	public static (int Offset, int Size)[] Segments(byte[] raw)
	{
		if (raw.Length < 32 || !raw.AsSpan(0, 4).SequenceEqual("cysp"u8)) throw new InvalidDataException("Invalid CYSP header");
		int count = BinaryPrimitives.ReadInt32LittleEndian(raw.AsSpan(12));
		if (count <= 0 || count > (raw.Length - 32) / 32) throw new InvalidDataException("Invalid CYSP body count");
		int offset = checked(32 + 32 * count); var result = new (int, int)[count];
		for (int i = 0; i < count; i++)
		{
			int size = checked((int)BinaryPrimitives.ReadUInt64LittleEndian(raw.AsSpan(32 + i * 32 + 8)));
			if (size > raw.Length - offset) throw new InvalidDataException("CYSP directory exceeds input");
			result[i] = (offset, size); offset = checked(offset + size);
		}
		if (offset != raw.Length) throw new InvalidDataException("CYSP directory size mismatch");
		return result;
	}
	private static void Vertices(AnimKitReader r, int count, Tree result)
	{
		bool weighted = r.Bool(); result["weighted"] = weighted;
		if (!weighted) { result["bones"] = null; result["vertices"] = r.Floats(checked(count * 2)); return; }
		var bones = new List<int>(); var vertices = new List<double>();
		for (int i = 0; i < count; i++)
		{
			int boneCount = r.Var(); bones.Add(boneCount);
			for (int j = 0; j < boneCount; j++) { bones.Add(r.Var()); vertices.AddRange(r.Floats(3)); }
		}
		result["bones"] = bones; result["vertices"] = vertices;
	}
	private static Tree Attachment(AnimKitReader r, string? defaultName, bool nonessential)
	{
		string? name = r.String() ?? defaultName; int type = r.Byte();
		string[] types = ["Region", "BoundingBox", "Mesh", "LinkedMesh", "Path", "Point", "Clipping"];
		if ((uint)type >= types.Length) throw new InvalidDataException("Unknown Spine attachment: " + type);
		var result = Tree.Of(("name", name), ("type", types[type]));
		switch (type)
		{
			case 0:
				r.String(); foreach (string field in new[] { "rotation", "x", "y", "scaleX", "scaleY", "width", "height" }) result[field] = r.Float(); r.Int(); break;
			case 1:
				int count = r.Var(); result["vertex_count"] = count; Vertices(r, count, result); if (nonessential) r.Int(); break;
			case 2:
				r.String(); r.Int(); count = r.Var(); r.Floats(checked(2 * count)); r.Shorts(); Vertices(r, count, new Tree()); r.Var();
				if (nonessential) { r.Shorts(); r.Float(); r.Float(); }
				break;
			case 3:
				r.String(); r.Int(); r.String(); r.String(); r.Bool(); if (nonessential) { r.Float(); r.Float(); }
				break;
			case 4:
				result["closed"] = r.Bool(); result["constantSpeed"] = r.Bool(); count = r.Var(); result["vertex_count"] = count; Vertices(r, count, result); result["lengths"] = r.Floats(count / 3); if (nonessential) r.Int(); break;
			case 5: r.Floats(3); if (nonessential) r.Int(); break;
			case 6: r.Var(); count = r.Var(); Vertices(r, count, new Tree()); if (nonessential) r.Int(); break;
		}
		return result;
	}
	private static Tree? Skin(AnimKitReader r, string? name, bool nonessential)
	{
		int count = r.Var(); if (count == 0) return null;
		var slots = new List<object?>(); var result = Tree.Of(("name", name), ("slots", slots));
		for (int i = 0; i < count; i++)
		{
			int index = r.Var(); var attachments = new List<object?>();
			for (int j = 0, n = r.Var(); j < n; j++)
			{
				string? key = r.String(); var attachment = Attachment(r, key, nonessential); attachment["key"] = key; attachments.Add(attachment);
			}
			slots.Add(Tree.Of(("slot_index", index), ("attachments", attachments)));
		}
		return result;
	}
	public static Tree Skeleton(byte[] raw)
	{
		AnimKitReader r = new AnimKitReader(raw);
		var result = Tree.Of(("hash", r.String()), ("version", r.String()), ("width", r.Float()), ("height", r.Float()));
		bool nonessential = r.Bool(); if (nonessential) { result["fps"] = r.Float(); result["imagesPath"] = r.String(); }
		var bones = new List<object?>(); int count = r.Var(); result["bone_count"] = count; result["bones"] = bones;
		for (int i = 0; i < count; i++)
		{
			var bone = Tree.Of(("name", r.String()), ("parent", i == 0 ? -1 : r.Var()));
			foreach (string field in new[] { "rotation", "x", "y", "scaleX", "scaleY", "shearX", "shearY", "length" }) bone[field] = r.Float();
			bone["transform_mode"] = r.Var(); if (nonessential) r.Int(); bones.Add(bone);
		}
		var slots = new List<object?>(); var slotNames = new List<object?>(); result["slots"] = slots; result["slot_names"] = slotNames;
		for (int i = 0, n = r.Var(); i < n; i++)
		{
			string? name = r.String(); slotNames.Add(name);
			slots.Add(Tree.Of(("name", name), ("bone", r.Var()), ("color", r.Int()), ("darkColor", r.Int()), ("attachmentName", r.String()), ("blendMode", r.Var())));
		}
		var constraints = new List<object?>(); result["constraints"] = constraints;
		for (int i = 0, n = r.Var(); i < n; i++) constraints.Add(Tree.Of(("kind", "Ik"), ("name", r.String()), ("order", r.Var()), ("bones", r.Ints()), ("target", r.Var()), ("mix", r.Float()), ("bendDirection", r.SignedByte())));
		for (int i = 0, n = r.Var(); i < n; i++)
		{
			var c = Tree.Of(("kind", "Transform"), ("name", r.String()), ("order", r.Var()), ("bones", r.Ints()), ("target", r.Var()), ("local", r.Bool()), ("relative", r.Bool()));
			foreach (string field in new[] { "offsetRotation", "offsetX", "offsetY", "offsetScaleX", "offsetScaleY", "offsetShearY", "rotateMix", "translateMix", "scaleMix", "shearMix" }) c[field] = r.Float(); constraints.Add(c);
		}
		for (int i = 0, n = r.Var(); i < n; i++)
		{
			var c = Tree.Of(("kind", "Path"), ("name", r.String()), ("order", r.Var()), ("bones", r.Ints()), ("target", r.Var()), ("positionMode", r.Var()), ("spacingMode", r.Var()), ("rotateMode", r.Var()));
			foreach (string field in new[] { "offsetRotation", "position", "spacing", "rotateMix", "translateMix" }) c[field] = r.Float(); constraints.Add(c);
		}
		result["default_skin"] = Skin(r, "default", nonessential); var skins = new List<object?>(); result["named_skins"] = skins;
		for (int i = 0, n = r.Var(); i < n; i++) skins.Add(Skin(r, r.String(), nonessential));
		result["_bytes_consumed"] = r.Position; result["_bytes_total"] = raw.Length; return result;
	}
}
