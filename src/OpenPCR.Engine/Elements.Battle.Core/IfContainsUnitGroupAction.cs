using Elements.Data;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class IfContainsUnitGroupAction : ActionParameter
	{
		private global::System.Collections.Generic.List<int> targetActionIds { get; set; } = new List<int>();
		private global::System.Collections.Generic.List<int> targetUnitIds { get; set; } = new List<int>();

		[global::OpenPCR.Migration("33b5e57b-daf4-4bee-ac74-a037d3272a48")]
		public IfContainsUnitGroupAction() : base()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.IfContainsUnitGroupAction::.ctor (action_type 108: no first-phase loader)"); }
	}
}
