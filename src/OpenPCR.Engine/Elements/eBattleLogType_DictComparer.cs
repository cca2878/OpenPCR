using System.Collections.Generic;

namespace Elements
{
	public class eBattleLogType_DictComparer : IEqualityComparer<eBattleLogType>
	{


		public bool Equals(eBattleLogType _x, eBattleLogType _y)
		{
			return _x == _y;
		}

		public int GetHashCode(eBattleLogType _obj)
		{
			return (int)_obj;
		}
	}
}
