using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class eAbnormalStateCategory_DictComparer : IEqualityComparer<eAbnormalStateCategory>
	{
		[global::OpenPCR.Migration("5b22c4d0-5f42-48c6-ba08-8827d40bda35")]
		public eAbnormalStateCategory_DictComparer()
		{
		}

		[global::OpenPCR.Migration("7e093eab-fc2b-47e0-b29a-ab12991ab26e")]
		public bool Equals(eAbnormalStateCategory _x, eAbnormalStateCategory _y)
		{
			return _x == _y;
		}

		[global::OpenPCR.Migration("c136ff61-3dc1-4818-883b-e9e78a0f8512")]
		public int GetHashCode(eAbnormalStateCategory _obj)
		{
			return (int)_obj;
		}
	}
}
