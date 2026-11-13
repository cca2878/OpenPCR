using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	public sealed class Shader : global::OpenPCR.Runtime.Api.Object
	{
		[global::OpenPCR.Migration("24c39e70-d558-4129-872a-0a106ab9c9c6")]
		private Shader()
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Shader::.ctor()", "Unity infrastructure not implemented");
		}
		[global::OpenPCR.Migration("bf0c5a36-f5db-4171-a639-6cc228b3ecd3")]
		public static int PropertyToID(string name)
		{
			return global::OpenPCR.Presentation.Noop<int>();
		}
	}
}
