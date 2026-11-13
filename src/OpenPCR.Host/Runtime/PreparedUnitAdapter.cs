using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elements;
using Elements.Data;

namespace OpenPCR.Host;

/// <summary>Formal unit input: account/configuration data and independent prepared growth results.</summary>
internal static class PreparedUnitAdapter
{
	public static UnitParameter CreateParameter(this PreparedUnit product, RuntimeMasterData master)
		=> CreateParameter(master, product.Unique, product.Limited, product.UniqueInputs, product.LimitedInputs, product.IsClanSupportOrReplay);
	internal static UnitParameter CreateParameter(RuntimeMasterData master, UnitData Unique, UnitData Limited, PreparedUnitInputs UniqueInputs, PreparedUnitInputs LimitedInputs, bool isClanSupportOrReplay = false)
	{
		if ((int)Unique.Id != (int)Limited.Id) throw new InvalidDataException("Unique/Limited unit identity differs.");
		PreparedUnitInputBinding.Bind(Unique, UniqueInputs); PreparedUnitInputBinding.Bind(Limited, LimitedInputs);
		int resourceId = UnitUtility.GetUnitResourceId(Unique.Id);
		var metadata = master.Manager.masterUnitData.Get(resourceId);
		if (metadata is null)
		{
			var enemy = master.Manager.masterUnitEnemyData.Get(resourceId) ?? throw new InvalidDataException("Missing unit metadata: " + resourceId);
			metadata = new MasterUnitData.UnitData(enemy);
		}
		return new UnitParameter
		{
			IsClanSupportOrReplay = isClanSupportOrReplay,
			UniqueData = Unique.CreateCopyData(),
			LimitedUnitData = Limited.CreateCopyData(),
			MasterData = metadata,
			SkillData = master.Manager.masterUnitSkillData[resourceId] ?? throw new InvalidDataException("Missing unit skill metadata: " + resourceId)
		};
	}
}
