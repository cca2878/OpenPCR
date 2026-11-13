using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class eValueNumber_DictComparer : IEqualityComparer<eValueNumber>
	{
		[global::OpenPCR.Migration("080747e6-59d3-40da-a17c-c48bbe834d0b")]
		public eValueNumber_DictComparer()
		{
		}

		[global::OpenPCR.Migration("48592af2-d50e-4bb3-9a70-d2d7d6467968")]
		public bool Equals(eValueNumber _x, eValueNumber _y)
		{
			return _x == _y;
		}

		[global::OpenPCR.Migration("d722fe42-22bc-44b0-8fce-3301ca02853e")]
		public int GetHashCode(eValueNumber _obj)
		{
			return (int)_obj;
		}
	}
}
