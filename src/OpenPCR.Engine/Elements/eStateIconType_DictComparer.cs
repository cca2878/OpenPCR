using System.Collections.Generic;
namespace Elements
{
	public class eStateIconType_DictComparer : IEqualityComparer<eStateIconType>
	{

		public bool Equals(eStateIconType _x, eStateIconType _y) => _x == _y;
		public int GetHashCode(eStateIconType _obj) => (int)_obj;
	}
}
