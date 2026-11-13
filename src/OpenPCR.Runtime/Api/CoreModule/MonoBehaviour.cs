using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Impl;
namespace OpenPCR.Runtime.Api
{
	public class MonoBehaviour : global::OpenPCR.Runtime.Api.Behaviour
	{
		[global::OpenPCR.Migration("7dcf47d9-b2a3-4493-8e1f-96659c647220")]
		public MonoBehaviour()
		{

		}
		[global::OpenPCR.Migration("442cc9d5-2272-41e9-b890-3bc2d9e94f4d")]
		public global::OpenPCR.Runtime.Api.Coroutine StartCoroutine(global::System.Collections.IEnumerator routine)
		{
			return (global::OpenPCR.Runtime.Api.Coroutine)global::OpenPCR.Runtime.Impl.PreparedCoroutineOperations.Start(this, routine);
		}
		[global::OpenPCR.Migration("8c91db88-dd83-4eb7-98cf-4588320abb92")]
		public void StopCoroutine(global::System.Collections.IEnumerator routine)
		{
			global::OpenPCR.Runtime.Impl.PreparedCoroutineOperations.Stop(this, routine);
		}
		[global::OpenPCR.Migration("1984ba0d-993c-4da2-9734-7c7c5dac5b71")]
		public void StopCoroutine(global::OpenPCR.Runtime.Api.Coroutine routine)
		{
			global::OpenPCR.Runtime.Impl.PreparedCoroutineOperations.Stop(this, routine);
		}
		[global::OpenPCR.Migration("3d165e1f-fe34-416e-a805-d07ce63e63c2")]
		public void StopAllCoroutines()
		{
			global::OpenPCR.Runtime.Impl.PreparedCoroutineOperations.StopAll(this);
		}
	}
}
