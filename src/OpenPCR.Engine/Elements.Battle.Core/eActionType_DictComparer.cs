using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class eActionType_DictComparer : IEqualityComparer<eActionType>
	{
		[global::OpenPCR.Migration("f9ddd3cd-5d5a-42fe-8502-349458aa94bf")]
		public eActionType_DictComparer()
		{
		}

		[global::OpenPCR.Migration("44c80106-140a-44a6-a37e-2193754a4ade")]
		public bool Equals(eActionType _x, eActionType _y)
		{
			return _x == _y;
		}

		[global::OpenPCR.Migration("bab979e8-149e-4a1f-a90c-6ba33482626f")]
		public int GetHashCode(eActionType _obj)
		{
			return (int)_obj;
		}
	}
}
