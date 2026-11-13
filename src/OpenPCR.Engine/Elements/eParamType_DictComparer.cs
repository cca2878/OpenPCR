using System.Collections.Generic;

namespace Elements
{
	public class eParamType_DictComparer : IEqualityComparer<eParamType>
	{
		[global::OpenPCR.Migration("58c20a15-0f19-4cc7-aff6-adf8e069ed31")]
		public eParamType_DictComparer()
		{
		}

		[global::OpenPCR.Migration("6266daa5-7360-4e3e-9a14-f4d36f64f72a")]
		public bool Equals(eParamType _x, eParamType _y)
		{
			return _x == _y;
		}

		[global::OpenPCR.Migration("e1ba69fa-1c36-4e44-9fd5-4f7ce06384da")]
		public int GetHashCode(eParamType _obj)
		{
			return (int)_obj;
		}
	}
}
