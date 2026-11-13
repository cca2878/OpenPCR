using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Elements;

namespace OpenPCR.Growth;

public sealed partial class GrowthCalculator
{
	/// <summary>Declared NPC: retain its explicit server bonus and rebuild the unclamped Labyrinth product.</summary>
	public PreparedUnit CalculateLabyrinthNpc(RawUnit npc, RawStatusShort? bonus)
	{
		_ = ActiveInput;
		ArgumentNullException.ThrowIfNull(npc);
		npc.Validate();
		var data = npc.ToNative();
		if (bonus is not null) data.SetBonusParam(bonus.ToNative());
		// Native addUnit and UnitParameter(UnitData) publish UniqueData without
		// UpdateLimitedUnitData. In particular, a Native NPC can have null CbExEquipSlot.
		CalcParamAndSkill(data);
		var sibling = CopyForGrowth(data);
		CalcParamAndSkill(sibling);
		var metadata = master.Require("unit_data", ("unit_id", Conversion(data.Id, data.UnitRarity)));
		// Both slots of the common DTO carry detached, unclamped NPC products.
		return new(data, sibling, Prepare(data, true), Prepare(sibling, true), metadata.Int("prefab_id"));
	}
	/// <summary>
	/// The rarity-6 conversion copy of a Labyrinth NPC. Native MightConvertRarity6Unit publishes the copy through
	/// UnitParameter(UnitData) without UpdateLimitedUnitData, so the copy is the same unclamped product as
	/// <see cref="CalculateLabyrinthNpc"/> under the converted identity. Null when the NPC has no conversion.
	/// </summary>
	public (UnitData Limited, OpenPCR.PreparedUnitInputs Inputs)? CalculateConvertedLabyrinthNpc(UnitData unique)
	{
		ArgumentNullException.ThrowIfNull(unique);
		int converted = Conversion(unique.Id, unique.UnitRarity);
		if (converted == (int)unique.Id) return null;
		var copy = CopyForGrowth(unique); copy.SetId(converted);
		CalcParamAndSkill(copy);
		return (copy, Prepare(copy, true));
	}
}
