using System;

namespace Elements.Battle.Core
{
	public class SealCancelData
	{
		protected Action CancelSealAction;
		[global::OpenPCR.Migration("7ac87176-588a-4c61-b769-6789ae6dae9a")]
		public SealCancelData(Action _cancelSealAction)
		{
			CancelSealAction = _cancelSealAction;
		}

		[global::OpenPCR.Migration("9b7332af-f672-4b00-bc5c-4cd145c8425f")]
		public virtual void OnActivate()
		{
		}
	}
}
