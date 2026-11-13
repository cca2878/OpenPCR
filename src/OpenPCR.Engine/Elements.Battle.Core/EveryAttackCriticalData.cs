using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class EveryAttackCriticalData
	{
		public float Timer;
		public eStateIconType IconType { get; set; }
		public bool Enable { get; set; }

		[global::OpenPCR.Migration("2c9d1b63-b962-43cf-aa0e-3ecbf83fb8c0")]
		public EveryAttackCriticalData()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.EveryAttackCriticalData::.ctor (reachable only through Excluded EveryAttackCriticalAction: no first-phase loader)"); }
	}
}
