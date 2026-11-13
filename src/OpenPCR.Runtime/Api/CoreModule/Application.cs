namespace OpenPCR.Runtime.Api
{
	public class Application
	{
		public static int targetFrameRate
		{
			[global::OpenPCR.Migration("bf04b214-81c2-4d34-a6c2-ce7819bba87c")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Application::get_targetFrameRate():System.Int32", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("d7fcd331-e34b-42ad-862d-e42f765f5b12")]
			set
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Application::set_targetFrameRate(System.Int32):System.Void", "Unity infrastructure not implemented");
			}
		}
	}
}
