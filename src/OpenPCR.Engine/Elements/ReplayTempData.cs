using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class ReplayTempData
	{

		public int FailedResultTipsImageId { get; set; }
		public int TeamLevel;
		public string EncriptKey = "";
		public int FrameRate = 30;
		public string AdditionalDetailTitleString = "";

		public bool IsLocalReplay { get; set; }
		public bool IsPostedToClanChat { get; set; }

		public ReplayTempData()
		{
			throw new OpenPCR.MigrationPendingException("Elements.ReplayTempData::.ctor()", "structure diagnostic remains after declaration migration");
		}

		[global::OpenPCR.Migration("1e105340-9c69-4ac1-bca2-eade4158ec74")]
		public void ResetData()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.ReplayTempData::ResetData():System.Void: periphery output member; behavior is supplied by the wiring phase");
		}
	}
}
