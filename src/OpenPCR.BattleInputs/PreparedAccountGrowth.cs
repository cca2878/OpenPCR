using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Elements;

namespace OpenPCR.BattleInputs;

public sealed record PreparedExtraEquipmentIdentity(int SerialId, int EquipmentId);
/// <summary>Resolved account dependencies consumed by Engine; no original inventory or story history.</summary>
public sealed class PreparedAccountGrowth
{
	public bool ChangeRarityReleased { get; }
	public IReadOnlyList<PreparedExtraEquipmentIdentity> ExtraEquipment { get; }
	public PreparedAccountGrowth(bool changeRarityReleased, IEnumerable<PreparedExtraEquipmentIdentity> equipment)
	{
		ChangeRarityReleased = changeRarityReleased;
		var values = equipment.ToArray();
		if (values.Any(value => value.SerialId == 0) || values.Select(value => value.SerialId).Distinct().Count() != values.Length) throw new ArgumentException("Invalid/duplicate prepared EX serial.");
		ExtraEquipment = Array.AsReadOnly(values);
	}
	public void Bind(UserData account)
	{
		RuntimeBinding.Set(account, "IsReleaseChangeRarityFlag", ChangeRarityReleased);
		var inventory = new InventoryController();
		var cache = (Dictionary<int, ExtraEquipPair>)RuntimeBinding.Get(inventory, "extraEquipDictionary")!;
		foreach (var entry in ExtraEquipment)
		{
			var info = new ExtraEquipInfo();
			RuntimeBinding.Set(info, "SerialId", (global::System.Int32)entry.SerialId);
			RuntimeBinding.Set(info, "ExEquipmentId", (global::System.Int32)entry.EquipmentId);
			cache.Add(entry.SerialId, new ExtraEquipPair(info));
		}
		RuntimeBinding.Set(account, "inventoryController", inventory);
	}
}
