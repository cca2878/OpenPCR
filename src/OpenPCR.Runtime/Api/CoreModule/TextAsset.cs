using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	public class TextAsset : global::OpenPCR.Runtime.Api.Object
	{
		[global::OpenPCR.Migration("2eed5690-0fd0-4ff2-a3a1-5f301645891a")]
		public TextAsset()
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.TextAsset::.ctor()", "Unity infrastructure not implemented");
		}
		public byte[] bytes
		{
			[global::OpenPCR.Migration("32a8184d-9e92-46d5-806a-d7a99ddeea74")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.TextAsset::get_bytes():System.Byte[]", "Unity infrastructure not implemented");
			}
		}
		public string text
		{
			[global::OpenPCR.Migration("9c40ccc9-d766-49c1-b7d3-bf37f74dc66e")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.TextAsset::get_text():System.String", "Unity infrastructure not implemented");
			}
		}
	}
}
