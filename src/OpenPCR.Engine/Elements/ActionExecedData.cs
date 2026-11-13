namespace Elements
{
	public class ActionExecedData
	{
		public bool AlreadyExeced { get; set; }
		public int TargetPartsNumber { get; set; }
		public int ExecedPartsNumber { get; set; }

		[global::OpenPCR.Migration("9a9d01ec-c345-46f9-8376-f2b46d9ffe05")]
		public ActionExecedData()
		{
		}
	}
}
