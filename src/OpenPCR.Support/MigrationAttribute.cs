using System;
using System.Diagnostics;

namespace OpenPCR
{
	/// <summary>Source-only UUID registration; never emitted in the mirror.</summary>
	[Conditional("OPENPCR_EMIT_AUDIT_METADATA")]
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
	public sealed class MigrationAttribute : Attribute
	{
		public string Id { get; }
		public MigrationAttribute(string id) { Id = id; }
	}

	/// <summary>Register a compiler-supplied member by UUID at its owning declaration without changing its body.</summary>
	[Conditional("OPENPCR_EMIT_AUDIT_METADATA")]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Event | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
	public sealed class ImplicitMigrationAttribute : Attribute
	{
		public string Id { get; }
		public ImplicitMigrationAttribute(string id) { Id = id; }
	}
}
