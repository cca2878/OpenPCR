using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	public sealed class Camera : global::OpenPCR.Runtime.Api.Behaviour
	{
		[global::OpenPCR.Migration("9e5295dd-dbea-4ecb-856a-fc7130ac09f0")]
		public Camera()
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Camera::.ctor()", "Unity infrastructure not implemented");
		}
		public float orthographicSize
		{
			[global::OpenPCR.Migration("f6e01f32-d5cd-4f91-87e6-25dc7390bdb9")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Camera::get_orthographicSize():System.Single", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("8f4d1d4d-4d57-4d66-ba35-5475ddac65d3")]
			set
			{
				global::OpenPCR.Presentation.Noop();
			}
		}
	}
}
