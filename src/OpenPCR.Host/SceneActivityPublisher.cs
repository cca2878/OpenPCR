using global::OpenPCR.Runtime.Impl;
using System;
using System.Collections.Generic;

namespace OpenPCR.Host;

/// <summary>Publishes the scene activity for one activeSelf change from the host's component enabled state.</summary>
internal sealed class SceneActivityPublisher
{
	private readonly Dictionary<object, bool> componentEnabled;
	private readonly Func<object, bool> enabled;
	private readonly bool owned;
	private long version;

	// An external table can change behind the publisher, so every publication is complete.
	internal SceneActivityPublisher(Dictionary<object, bool> componentEnabled)
	{
		this.componentEnabled = componentEnabled;
		enabled = component => this.componentEnabled[component];
	}

	// An owned table changes only through Add and Set, which move the version the Runtime compares between publications.
	internal SceneActivityPublisher() : this(new Dictionary<object, bool>(ReferenceEqualityComparer.Instance)) => owned = true;

	internal bool Enabled(object component) => componentEnabled[component];

	internal void Add(object component, bool value)
	{
		RequireOwned(); componentEnabled.Add(component, value); version++;
	}

	internal void Set(object component, bool value)
	{
		RequireOwned(); componentEnabled[component] = value; version++;
	}

	private void RequireOwned()
	{
		if (!owned) throw new InvalidOperationException("An external enabled table is written by its owner.");
	}

	// Every node is recomputed from the live topology, including reparenting, dynamically added components and newly
	// registered resource nodes; the Runtime validates before changing any field. With an owned table an unchanged
	// version and scene structure let the Runtime recompute only the published subtree.
	internal void Publish(object owner, bool active)
	{
		if (owned) SceneScripting.PublishActivity(owner, active, enabled, version);
		else SceneScripting.PublishActivity(owner, active, enabled);
	}
}
