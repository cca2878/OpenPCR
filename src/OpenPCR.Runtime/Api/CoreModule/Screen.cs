using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	public sealed class Screen
	{
		public static int width
		{
			[global::OpenPCR.Migration("5b049aed-876f-4631-bb6d-bedd5778960a")]
			get
			{
				return global::OpenPCR.Presentation.Noop<int>();
			}
		}
		public static int height
		{
			[global::OpenPCR.Migration("9ba801f2-ef3c-4cd0-af59-facae06b0e81")]
			get
			{
				return global::OpenPCR.Presentation.Noop<int>();
			}
		}
		public static int sleepTimeout
		{
			[global::OpenPCR.Migration("c83d5c12-4e5b-4177-9e2b-2826601ece45")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Screen::get_sleepTimeout():System.Int32", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("ff775026-291b-45d7-8d8d-9bab0fe5ed6a")]
			set
			{
				global::OpenPCR.Presentation.Noop();
			}
		}
		public static global::OpenPCR.Runtime.Api.Rect safeArea
		{
			[global::OpenPCR.Migration("44575fe6-939b-410b-8fba-873e504ec990")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Screen::get_safeArea():OpenPCR.Runtime.Api.Rect", "Unity infrastructure not implemented");
			}
		}
	}
}
