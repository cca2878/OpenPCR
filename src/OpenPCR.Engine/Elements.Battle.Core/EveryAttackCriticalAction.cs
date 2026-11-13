using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class EveryAttackCriticalAction : ActionParameter
	{
		public enum eEveryAtkCriticalType
		{
			PHYSICAL = 1,
			MAGIC = 2,
			BOTH = 3
		}

		[global::OpenPCR.Migration("4bc54589-9aa6-4821-85a2-040b1421478f")]
		public EveryAttackCriticalAction() : base()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.EveryAttackCriticalAction::.ctor (action_type 104: no first-phase loader)"); }
	}
}
