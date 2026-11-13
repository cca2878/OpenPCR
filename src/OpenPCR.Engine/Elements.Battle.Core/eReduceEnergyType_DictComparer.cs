using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class eReduceEnergyType_DictComparer : IEqualityComparer<eReduceEnergyType>
	{
		[global::OpenPCR.Migration("54b790bf-4d7d-4f27-8865-2323671fdc77")]
		public eReduceEnergyType_DictComparer()
		{
		}

		[global::OpenPCR.Migration("923a533b-9ba3-4c59-a0bc-6c929eba74e3")]
		public bool Equals(eReduceEnergyType _x, eReduceEnergyType _y)
		{
			return _x == _y;
		}

		[global::OpenPCR.Migration("bc42b242-0f3a-4991-8cb9-895b41b65d2a")]
		public int GetHashCode(eReduceEnergyType _obj)
		{
			return (int)_obj;
		}
	}
}
