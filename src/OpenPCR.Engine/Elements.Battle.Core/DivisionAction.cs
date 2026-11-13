using global::OpenPCR.Runtime.Api;
using Elements.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class DivisionAction : ActionParameter
	{
		private class DivisionData
		{
			public GameObject Prefab { get; set; }
			public float PositionX { get; set; }

			[global::OpenPCR.Migration("c5ae4974-1b0f-4bfc-8ae4-e23ddebbbd72")]
			public DivisionData() : base()
			{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.DivisionAction\u002BDivisionData::.ctor (reachable only through Excluded DivisionAction: no first-phase loader)"); }
		}

		private List<DivisionData> divisionDataList = new List<DivisionData>();
		private List<UnitCtrl> divisionList = new List<UnitCtrl>();
		[global::OpenPCR.Migration("65748f09-d22d-457f-94cb-80cc57982ba1")]
		public DivisionAction() : base()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.DivisionAction::.ctor (action_type 51: no first-phase loader)"); }
	}
}
