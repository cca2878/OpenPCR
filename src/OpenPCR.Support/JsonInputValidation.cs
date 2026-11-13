using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OpenPCR;

/// <summary>Reject ambiguous object fields before deserializing independently supplied input.</summary>
public static class JsonInputValidation
{
	public static void RejectDuplicateProperties(JsonElement value)
	{
		if (value.ValueKind == JsonValueKind.Object)
		{
			var names = new HashSet<string>(StringComparer.Ordinal);
			foreach (var property in value.EnumerateObject())
			{
				if (!names.Add(property.Name)) throw new InvalidDataException("Duplicate input property: " + property.Name);
				RejectDuplicateProperties(property.Value);
			}
		}
		else if (value.ValueKind == JsonValueKind.Array) foreach (var item in value.EnumerateArray()) RejectDuplicateProperties(item);
	}
}
