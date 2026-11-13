using System;
using System.Collections.Generic;
using OpenPCR;

internal static class PreparedResourceLoadTests
{
	internal static void Run(Action<bool, string> check)
	{
		var loader = new object(); var manager = new object(); var parent = new object();
		var events = new List<string>();
		var inputs = new List<(int Id, long Index, object? Parent)>();
		var instances = new List<object>(); object? received = null;
		try
		{
			PreparedResourceInputBinding.Bind(manager, (id, index, owner) =>
			{
				events.Add($"instance:{id}:{index}"); inputs.Add((id, index, owner));
				var instance = new object(); instances.Add(instance); return instance;
			});
			PreparedBattleResourceLoads.Bind(loader, manager, (_, _, _) => throw new InvalidOperationException("Unexpected unit resource request."),
				(id, index, _) => { events.Add($"read:{id}:{index}"); return new object(); });
			PreparedBattleResourceLoads.AddInstantiate(manager, 41, 11, parent, null);
			PreparedBattleResourceLoads.AddResource(manager, 42, 12, _ => events.Add("resource-callback"), true);
			PreparedBattleResourceLoads.AddInstantiate(manager, 43, 13, null, instance => { received = instance; events.Add("instance-callback"); });
			check(instances.Count == 0, "Prepared instances must wait for StartLoad's ordered flush.");
			PreparedBattleResourceLoads.Flush(manager, () => events.Add("complete"));
			check(instances.Count == 2, "An instantiate request with no callback must still produce one fresh instance.");
			check(inputs[0].Id == 41 && inputs[0].Index == 11 && ReferenceEquals(inputs[0].Parent, parent), "A callback-free instantiate request lost its resource identity or parent.");
			check(inputs[1].Id == 43 && inputs[1].Index == 13 && inputs[1].Parent is null && ReferenceEquals(received, instances[1]), "A regular instantiate request lost its null parent or produced callback value.");
			check(string.Join(",", events) == "instance:41:11,read:42:12,resource-callback,instance:43:13,instance-callback,complete", "Callback-free instantiation must retain shared request and completion ordering.");
		}
		finally { PreparedBattleResourceLoads.Release(loader, manager); PreparedResourceInputBinding.Release(manager); }

		// Reuse both keys after releasing the first session, so these inputs cannot leak into later tests.
		var failure = new InvalidOperationException("Synthetic instance producer failure.");
		int attempts = 0; bool completed = false; Exception? observed = null;
		try
		{
			PreparedResourceInputBinding.Bind(manager, (_, _, _) => { attempts++; throw failure; });
			PreparedBattleResourceLoads.Bind(loader, manager, (_, _, _) => throw new InvalidOperationException("Unexpected unit resource request."));
			PreparedBattleResourceLoads.AddInstantiate(manager, 44, 14, parent, null);
			try { PreparedBattleResourceLoads.Flush(manager, () => completed = true); }
			catch (InvalidOperationException error) { observed = error; }
			check(attempts == 1 && ReferenceEquals(observed, failure), "A null callback must not hide an instance producer failure.");
			check(!completed, "A failed instance producer must not report successful load completion.");
		}
		finally { PreparedBattleResourceLoads.Release(loader, manager); PreparedResourceInputBinding.Release(manager); }
	}
}
