using System;
using System.Collections.Generic;

namespace OpenPCR.Host;

public sealed partial class BattleSession
{
	/// <summary>Read the pending timeline and earliest anchors accepted at this frame boundary.</summary>
	public PlayerInputState ReadPlayerInputs()
	{
		RequireAvailable();
		return state.PlayerInputs.Read();
	}
	/// <summary>Atomically replace all pending inputs. Capture/Restore also saves this timeline and its cursors.</summary>
	public void ReplaceFutureOperations(IReadOnlyList<PlayerOperation> operations)
	{
		RequireAvailable();
		ArgumentNullException.ThrowIfNull(operations);
		if (state.UbExploration) throw new InvalidOperationException("Use ChooseUb, or end UB exploration before editing player controls.");
		if (Completed || Exhausted) throw new InvalidOperationException("Restore an earlier checkpoint before editing a stopped battle.");
		busy = true;
		try { state.PlayerInputs.ReplaceFuture(operations); }
		finally { busy = false; }
	}
}
