using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenPCR.PrefabExtractor;

/// <summary>Source scopes to re-extract or remove for the supplied manifest.</summary>
internal static class Update
{
	internal sealed record Plan(List<string> Changed, List<string> Removed);

	/// <summary>
	/// A current key is re-extracted when it has no record, a recorded bundle is absent from the manifest or differs in MD5
	/// or size, or a bundle the source names for it is not among the recorded reads. Recorded keys the manifest no longer
	/// produces are removed.
	/// </summary>
	internal static Plan For(string source, IReadOnlyList<string> keys, IReadOnlyDictionary<string, List<BundleEntry>> recorded, Resources resources)
	{
		bool Stale(string key)
		{
			if (!recorded.TryGetValue(key, out var used) || used.Count == 0) return true;
			foreach (var bundle in used)
				if (resources.Entry(bundle.Name) is not { } entry || entry.Size != bundle.Size || !entry.Md5.Equals(bundle.Md5, StringComparison.OrdinalIgnoreCase)) return true;
			return Sources.BundleNames(source, key, resources).Any(name => used.All(bundle => bundle.Name != name));
		}
		var current = keys.ToHashSet(StringComparer.Ordinal);
		return new(keys.Where(Stale).ToList(), recorded.Keys.Where(key => !current.Contains(key)).Order(StringComparer.Ordinal).ToList());
	}
}
