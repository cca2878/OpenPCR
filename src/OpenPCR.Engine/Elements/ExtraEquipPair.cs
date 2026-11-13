using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Elements.Data;

namespace Elements
{
	public class ExtraEquipPair
	{


		public ExtraEquipInfo Info { get; set; }
		[global::OpenPCR.ImplicitMigration("0c272490-7cac-4783-92dd-dda9d5b4a850")]
		public int ExEquipmentId => Info.ExEquipmentId;

		public ExtraEquipPair(ExtraEquipInfo _info)
		{
			Info = _info;
		}
	}
}
