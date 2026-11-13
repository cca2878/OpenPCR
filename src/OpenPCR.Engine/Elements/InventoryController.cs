using System.Linq;
using System;
using System.Collections.Generic;
using Elements.Data;

namespace Elements
{
	public class InventoryController
	{
		private Dictionary<int, InventoryData> itemDictionary = new Dictionary<int, InventoryData>();
		private Dictionary<int, InventoryData> equipDictionary = new Dictionary<int, InventoryData>();
		private Dictionary<eInventoryType, Dictionary<int, InventoryData>> inventoryTypeToDictionary;
		private Dictionary<int, ExtraEquipPair> extraEquipDictionary = new Dictionary<int, ExtraEquipPair>();
		public InventoryController()
		{
			inventoryTypeToDictionary = new Dictionary<eInventoryType, Dictionary<int, InventoryData>>
			{
				{
					eInventoryType.Item,
					itemDictionary
				},
				{
					eInventoryType.Equip,
					equipDictionary
				}
			};
		}

		public ExtraEquipPair SearchExtraEquip(int _serialId)
		{
			extraEquipDictionary.TryGetValue(_serialId, out var value);
			return value;
		}
	}
}
