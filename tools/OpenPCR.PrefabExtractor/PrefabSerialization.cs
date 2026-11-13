using System;
using System.Collections;
using System.IO;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace OpenPCR.PrefabExtractor;

internal static class PrefabSerialization
{
	// Generated C# writers test value != 0f and omit negative zero. Python's producer preserves
	// its sign bit. Keep the schema and standard protobuf parser; write through the official
	// coded stream using descriptor order and bitwise float default checks.
	public static byte[] Encode(IMessage message)
	{
		using var bytes = new MemoryStream();
		using (var output = new CodedOutputStream(bytes, true))
		{
			foreach (var field in message.Descriptor.Fields.InFieldNumberOrder())
			{
				object value = field.Accessor.GetValue(message);
				if (field.IsRepeated)
				{
					var values = ((IEnumerable)value).Cast<object>().ToArray(); if (values.Length == 0) continue;
					if (field.IsPacked && field.FieldType is not (FieldType.Message or FieldType.String or FieldType.Bytes))
					{
						using var packed = new MemoryStream();
						using (var payload = new CodedOutputStream(packed, true)) foreach (object item in values) Scalar(payload, field, item);
						output.WriteTag(field.FieldNumber, WireFormat.WireType.LengthDelimited); output.WriteBytes(ByteString.CopyFrom(packed.ToArray()));
					}
					else foreach (object item in values) Field(output, field, item);
					continue;
				}
				if (field.HasPresence ? !field.Accessor.HasValue(message) : IsDefault(field, value)) continue;
				Field(output, field, value);
			}
		}
		return bytes.ToArray();
	}
	private static bool IsDefault(FieldDescriptor field, object value) => field.FieldType switch
	{
		FieldType.Float => BitConverter.SingleToInt32Bits((float)value) == 0,
		FieldType.Double => BitConverter.DoubleToInt64Bits((double)value) == 0,
		FieldType.Bool => !(bool)value,
		FieldType.String => ((string)value).Length == 0,
		FieldType.Int32 or FieldType.Enum => Convert.ToInt32(value) == 0,
		FieldType.SInt64 => (long)value == 0,
		_ => throw new InvalidDataException("Unsupported protobuf default: " + field.FullName)
	};
	private static void Field(CodedOutputStream output, FieldDescriptor field, object value)
	{
		var wire = field.FieldType switch
		{
			FieldType.Float => WireFormat.WireType.Fixed32,
			FieldType.Double => WireFormat.WireType.Fixed64,
			FieldType.String or FieldType.Message => WireFormat.WireType.LengthDelimited,
			_ => WireFormat.WireType.Varint
		};
		output.WriteTag(field.FieldNumber, wire); Scalar(output, field, value);
	}
	private static void Scalar(CodedOutputStream output, FieldDescriptor field, object value)
	{
		switch (field.FieldType)
		{
			case FieldType.Float: output.WriteFloat((float)value); break;
			case FieldType.Double: output.WriteDouble((double)value); break;
			case FieldType.Bool: output.WriteBool((bool)value); break;
			case FieldType.Int32: output.WriteInt32((int)value); break;
			case FieldType.SInt64: output.WriteSInt64((long)value); break;
			case FieldType.Enum: output.WriteEnum(Convert.ToInt32(value)); break;
			case FieldType.String: output.WriteString((string)value); break;
			case FieldType.Message: output.WriteBytes(ByteString.CopyFrom(Encode((IMessage)value))); break;
			default: throw new InvalidDataException("Unsupported protobuf field: " + field.FullName);
		}
	}
}
