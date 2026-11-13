using System.Collections.Generic;

namespace Elements
{
	public class ePauseType_DictComparer : IEqualityComparer<ePauseType>
	{
		[global::OpenPCR.Migration("c2542544-75c9-44da-8209-210317425e2f")]
		public ePauseType_DictComparer()
		{
		}

		[global::OpenPCR.Migration("d64db1ff-087b-48a9-82ee-304021c11112")]
		public bool Equals(ePauseType _x, ePauseType _y)
		{
			return _x == _y;
		}

		[global::OpenPCR.Migration("7b32fc45-48e8-41d9-b68f-eef9f902dac5")]
		public int GetHashCode(ePauseType _obj)
		{
			return (int)_obj;
		}
	}
}
