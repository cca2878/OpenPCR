using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Elements.Data;
using System.Linq;

namespace Elements.Battle.Core
{
	public class IfContainsCharaTypeGroupAction : ActionParameter
	{
		private global::System.Collections.Generic.List<int> targetActionIds { get; set; } = new List<int>();
		private global::System.Collections.Generic.List<int> targetCharaIds { get; set; } = new List<int>();

		[global::OpenPCR.Migration("78d0d91b-7fef-4d34-a052-76448f019b03")]
		public IfContainsCharaTypeGroupAction() : base()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.IfContainsCharaTypeGroupAction::.ctor (action_type 131: no first-phase loader)"); }
	}
}
