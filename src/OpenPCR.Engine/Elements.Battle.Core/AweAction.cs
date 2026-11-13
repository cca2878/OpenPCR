using Elements.Battle;
using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class AweAction : ActionParameter
	{
		public class AweData
		{
			public double Value;
			public SkillEffectCtrl Effect;
			public int CountLimit = -1;
			public eAweType AweType = eAweType.UB_AND_SKILL;
			[global::OpenPCR.Migration("571a5783-d0c8-40b8-b6d0-f3d822020f8b")]
			public AweData() : base()
			{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.AweAction\u002BAweData::.ctor (reachable only through Excluded AweAction: no first-phase loader)"); }
		}

		public enum eAweType
		{
			UB_ONLY,
			UB_AND_SKILL
		}

		[global::OpenPCR.Migration("cc0bddc7-a117-45af-aec2-bff0bbb373f2")]
		public AweAction() : base()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.AweAction::.ctor (action_type 62: no first-phase loader)"); }
	}
}
