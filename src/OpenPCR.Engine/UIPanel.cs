using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;

namespace OpenPCR.UICompat
{

	public class UIPanel : UIRect
	{
		public override float alpha { get => global::OpenPCR.MigrationPending.Throw<float>("UIPanel.get_alpha()", "behavior migration pending"); set => global::OpenPCR.MigrationPending.Throw("UIPanel.set_alpha(System.Single)", "behavior migration pending"); }

		public int sortingOrder
		{
			get
			{
				throw new global::OpenPCR.OutOfBoundaryException("UIPanel::sortingOrder");
			}

			[global::OpenPCR.Migration("5a7099d2-174f-45de-b360-69283de66f54")]
			set
			{
				throw new global::OpenPCR.OutOfBoundaryException("UIPanel::sortingOrder");
			}
		}

		public UIPanel()
		{
			throw new global::OpenPCR.MigrationPendingException("UIPanel::.ctor()", "body migration pending");
		}
	}

}
