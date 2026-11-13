using System;
using System.Diagnostics;

namespace OpenPCR
{
	/// <summary>
	/// Source-only UUID registration of an <c>OPENPCR_OPTIMIZED</c> region; never emitted in the mirror.
	/// A replaced member keeps its reference implementation in the region's <c>#else</c> branch.
	/// </summary>
	[Conditional("OPENPCR_EMIT_AUDIT_METADATA")]
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public sealed class OptimizationAttribute : Attribute
	{
		public string Id { get; }
		public OptimizationAttribute(string id) { Id = id; }
	}
}
