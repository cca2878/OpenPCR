using System.Collections.Generic;

namespace Elements
{
	public class eResourceId_DictComparer : IEqualityComparer<eResourceId>
	{


		public bool Equals(eResourceId _x, eResourceId _y)
		{
			return _x == _y;
		}

		public int GetHashCode(eResourceId _obj)
		{
			return (int)_obj;
		}
	}
}
