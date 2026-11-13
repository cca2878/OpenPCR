using System;
using System.Collections.Generic;
using global::OpenPCR.Runtime.Api;

namespace Elements.Battle.Core
{
	public class UnableStateGuardForExtraEffectAction : global::Elements.Battle.Core.UnableStateGuardActionBase
	{
		[global::OpenPCR.Migration("cae1027b-bb78-4fc1-8926-ce6077f34095")]
		public UnableStateGuardForExtraEffectAction() : base()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.UnableStateGuardForExtraEffectAction::.ctor() (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("a43cfbd9-2ccd-4b89-908c-7e14573fde8d")]
		protected override void createSeal(global::Elements.Battle.Core.UnitComponentAbnormal _componentAbnormal, int _maxCount, bool _isReleaseTimeLimitOnly)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.UnableStateGuardForExtraEffectAction::createSeal(Elements.Battle.Core.UnitComponentAbnormal,System.Int32,System.Boolean):System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("a89667e6-ceb0-415e-8c3a-a85544364a1e")]
		protected override global::Elements.eStateIconType getIconType()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.UnableStateGuardForExtraEffectAction::getIconType():Elements.eStateIconType (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("16820677-10e8-4b2f-b998-9a8403c75b05")]
		protected override global::Elements.Battle.Core.SealData getSealData(global::Elements.Battle.Core.UnitComponentAbnormal _componentAbnormal)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.UnableStateGuardForExtraEffectAction::getSealData(Elements.Battle.Core.UnitComponentAbnormal):Elements.Battle.Core.SealData (outside supported first-phase lifecycle/input scope)"); }
	}
}
