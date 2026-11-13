using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System.Collections.Generic;

namespace OpenPCR.UICompat
{

	public class UIProgressBar : UIWidgetContainer
	{
		[SerializeField]
		protected UIWidget mFG;
		public float value
		{
			get
			{
				throw new global::OpenPCR.OutOfBoundaryException("UIProgressBar::value");
			}

			set
			{
				throw new global::OpenPCR.OutOfBoundaryException("UIProgressBar::value");
			}
		}

		public UIProgressBar()
		{
			throw new global::OpenPCR.MigrationPendingException("UIProgressBar::.ctor()", "body migration pending");
		}

		public UIWidget foregroundWidget
		{
			get
			{
				return mFG;
			}

			set
			{
				throw new global::OpenPCR.OutOfBoundaryException("UIProgressBar::set_foregroundWidget(UIWidget): scene widget output producer is not implemented");
			}
		}
	}

}
