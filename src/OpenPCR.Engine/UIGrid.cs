using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System;
using System.Collections.Generic;

namespace OpenPCR.UICompat
{

	public class UIGrid : UIWidgetContainer
	{

		public UIGrid()
		{
			throw new global::OpenPCR.MigrationPendingException("UIGrid::.ctor()", "body migration pending");
		}

		[global::OpenPCR.Migration("a7bc2f40-2591-47b4-875c-9cc5aba899c2")]
		public virtual void Reposition()
		{
			global::OpenPCR.Presentation.Noop();
		}
	}

}
