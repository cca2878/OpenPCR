using global::Elements;
using global::OpenPCR.Runtime.Api;
using System.Runtime.CompilerServices;
using global::OpenPCR.AnimKit;

namespace Elements.Battle.Core
{
	public class ToadData
	{
		public Vector3 LeftDirScale { get; set; } = default(Vector3);
		public Vector3 RightDirScale { get; set; } = default(Vector3);
		public bool ReleaseByHeal { get; set; }
		public bool Enable { get; set; }
		public BattleAnimKitController BattleSpineController { get; set; }

		[global::OpenPCR.Migration("b965ec3c-3b9c-44a0-8c46-16228c861450")]
		public ToadData()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.ToadData::.ctor (reachable only through Excluded ToadAction: no first-phase loader)"); }

		public UnitActionController UnitActionController { get; set; }
	}
}
