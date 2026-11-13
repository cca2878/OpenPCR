using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	public sealed class Resources
	{
		[global::OpenPCR.Migration("5cf4691e-7b0c-485e-934a-fd51174ceb67")]
		public static T Load<T>(string path) where T : global::OpenPCR.Runtime.Api.Object
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Resources::Load`1(System.String):!!0", "Unity infrastructure not implemented");
		}
		[global::OpenPCR.Migration("2da1f1ab-5fd8-45dc-a54a-41492c90a1c0")]
		public static global::OpenPCR.Runtime.Api.Object Load(string path, global::System.Type systemTypeInstance)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Resources::Load(System.String,System.Type):OpenPCR.Runtime.Api.Object", "Unity infrastructure not implemented");
		}
		[global::OpenPCR.Migration("10bcc6b5-d4f6-43fe-945a-56009fea246a")]
		public static global::OpenPCR.Runtime.Api.AsyncOperation UnloadUnusedAssets()
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Resources::UnloadUnusedAssets():OpenPCR.Runtime.Api.AsyncOperation", "Unity infrastructure not implemented");
		}
	}
}
