using System;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class InventoryData
	{
		public eInventoryType Type { get; private set; }
		public int Id { get; private set; }
		public int Stock { get; private set; }
		public DateTime CreateTime { get; private set; }

		public InventoryData(eInventoryType _type, int _id, int _stock, DateTime _createTime = default(DateTime))
		{
			Type = _type;
			Id = _id;
			Stock = _stock;
			CreateTime = _createTime;
		}
	}
}
