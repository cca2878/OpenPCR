using global::OpenPCR.Foundation;
namespace OpenPCR.Foundation
{
	public class Toolbox
	{
		private static AssetManager assetManager;
		private static QualityManager qualityManager;
		private static ResourcesManager resourcesManager;
		public static AssetManager AssetManager => assetManager;
		[global::OpenPCR.ImplicitMigration("cb1a99b0-3c99-49c8-9cf5-b694aa6ee74f")]
		public static QualityManager QualityManager => qualityManager;
		[global::OpenPCR.ImplicitMigration("a9872733-b2d1-4abc-bf18-96f1b0651af0")]
		public static ResourcesManager ResourcesManager => resourcesManager;


	}
}
