using System.Linq;
using global::OpenPCR.Foundation;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Elements.Data;
using global::OpenPCR.Runtime.Api;

namespace Elements
{
	public class LoadingManager : ManagerSingleton<LoadingManager>
	{
		private bool disableTips;

		public LoadingManager()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.LoadingManager::.ctor()", "body migration pending");
		}

		[global::OpenPCR.Migration("938c3c9b-8b56-4710-8fd8-86c9b6095d5f")]
		public void DisableTips()
		{
			disableTips = true;
		}
		protected override void onInit() => global::OpenPCR.MigrationPending.Throw("Elements.LoadingManager.onInit()", "behavior migration pending");

	}
}
