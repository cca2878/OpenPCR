using global::OpenPCR.AnimKit;
using System;
using Elements.Data;

namespace OpenPCR.Host;

/// <summary>
/// Prepared read-only graphs shared by reference across checkpoints. Runtime skeletons, bones, slots,
/// track entries and units retain their own captured state.
/// </summary>
internal static class CheckpointSharing
{
	internal static bool IsPreparedReadOnly(Type type)
	{
		// Animation timelines, including the host's property-only placeholders, hold authored curves and frames.
		if (typeof(Timeline).IsAssignableFrom(type)) return true;
		// Frozen external clock inputs have no cursor; session.EngineFrame chooses the next update after restore.
		if (type == typeof(BattleUpdateClock)) return true;
		if (type.Assembly != typeof(MasterDataManager).Assembly) return type.FullName == "OpenPCR.BattleInputs.PreparedGrowthCurve";
		if (type.Namespace == "Elements.Data")
		{
			// Master rows complete SetUp in their constructors. Lazy primary-key caches memoize deterministic lookups
			// and are never enumerated, so retained entries from abandoned branches do not change row values.
			var outer = type;
			while (outer.DeclaringType is not null) outer = outer.DeclaringType;
			return outer.Name.StartsWith("Master", StringComparison.Ordinal);
		}
		if (type.Namespace != typeof(global::OpenPCR.AnimKit.SkeletonData).Namespace) return false;
		// Setup data and animations. Attachments, skins and all run instances keep their field images.
		return type == typeof(SkeletonData) || type == typeof(BoneData) || type == typeof(SlotData) || type == typeof(EventData) ||
			type == typeof(IkConstraintData) || type == typeof(TransformConstraintData) || type == typeof(PathConstraintData) ||
			type == typeof(Animation);
	}
}
