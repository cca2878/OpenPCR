namespace OpenPCR.Runtime.Api
{
	public abstract class CustomYieldInstruction : global::System.Collections.IEnumerator
	{
		[global::OpenPCR.Migration("c62783e0-2ac6-43fa-a540-50d2de9d8c96")]
		protected CustomYieldInstruction()
		{

		}
		[global::OpenPCR.Migration("6ba1aa82-08fc-41dc-bea2-ed4166c9652a")]
		public bool MoveNext()
		{
			return keepWaiting;
		}
		[global::OpenPCR.Migration("50823ce3-8dfe-4d2d-9663-78125e7b2a17")]
		public virtual void Reset()
		{

		}
		public abstract bool keepWaiting
		{
			get;
		}
		public object Current
		{
			[global::OpenPCR.Migration("f35a3b3a-27b0-4711-8ba9-ac0cbc35faba")]
			get
			{
				return null;
			}
		}
	}
}
