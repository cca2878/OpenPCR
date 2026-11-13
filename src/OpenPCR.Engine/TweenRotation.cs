using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System;

namespace OpenPCR.UICompat
{

	public class TweenRotation : UITweener
	{

		protected override void OnUpdate(float factor, bool isFinished)
		{
			global::OpenPCR.MigrationPending.Throw("TweenRotation.OnUpdate(System.Single,System.Boolean)", "behavior migration pending");
		}

		public TweenRotation()
		{
			throw new global::OpenPCR.MigrationPendingException("TweenRotation::.ctor()", "body migration pending");
		}
	}

}
