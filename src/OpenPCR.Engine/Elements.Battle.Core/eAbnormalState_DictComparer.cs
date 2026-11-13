using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class eAbnormalState_DictComparer : IEqualityComparer<eAbnormalState>
	{
		[global::OpenPCR.Migration("e3ba52f0-0870-4933-9b98-f754aa4bff1d")]
		public eAbnormalState_DictComparer()
		{
		}

		[global::OpenPCR.Migration("bedf7a4e-8e9e-4e40-a91e-807dc4561a14")]
		public bool Equals(eAbnormalState _x, eAbnormalState _y)
		{
			return _x == _y;
		}

		[global::OpenPCR.Migration("2a4c60dd-3b8a-4903-b53d-d41a2c9e884f")]
		public int GetHashCode(eAbnormalState _obj)
		{
			return (int)_obj;
		}
	}
}
