using global::Elements;
using global::OpenPCR.Runtime.Api;
namespace Elements
{
	public class ExceptUIRoot : global::SingletonMonoBehaviour<global::Elements.ExceptUIRoot>
	{
		public static global::OpenPCR.Runtime.Api.Transform Transform { [global::OpenPCR.Migration("ac157510-075a-486d-aea8-84faab7bcdce")] get { return Instance.transform; } }

		public ExceptUIRoot()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.ExceptUIRoot::.ctor()", "Native consumer declaration; behavior evidence pending");
		}
	}
}
