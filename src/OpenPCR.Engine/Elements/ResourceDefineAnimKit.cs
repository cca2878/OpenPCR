using global::Elements;
namespace Elements
{
	public static class ResourceDefineAnimKit
	{
		private static readonly AnimKitResourceInfo[] resouceInfoArray;
		[global::OpenPCR.Migration("4fabf70e-a98b-4f60-b698-03c2b7c2639f")]
		static ResourceDefineAnimKit()
		{
			resouceInfoArray = new AnimKitResourceInfo[]
			{
				new AnimKitResourceInfo((eAnimKitType)0, (eBundleId)1335351041, (eResourceId)1289233317, (eResourceId)1595653543, (eResourceId)1126988621, 0.5f, true),
				new AnimKitResourceInfo((eAnimKitType)1, (eBundleId)1335351041, (eResourceId)1289233317, (eResourceId)1931325262, (eResourceId)1126988621, 0.5f, true),
				new AnimKitResourceInfo((eAnimKitType)2, (eBundleId)1335351041, (eResourceId)1289233317, (eResourceId)1543900266, (eResourceId)1126988621, 0.5f, true),
				new AnimKitResourceInfo((eAnimKitType)3, (eBundleId)1324782861, (eResourceId)1320432852, (eResourceId)1595653543, (eResourceId)1126988621, 0.5f, true),
				new AnimKitResourceInfo((eAnimKitType)4, (eBundleId)1324782861, (eResourceId)1320432852, (eResourceId)1931325262, (eResourceId)1126988621, 0.5f, true),
				new AnimKitResourceInfo((eAnimKitType)5, (eBundleId)1629494061, (eResourceId)1935999960, (eResourceId)1595653543, (eResourceId)1126988621, 0.5f, true),
				new AnimKitResourceInfo((eAnimKitType)6, (eBundleId)1629494061, (eResourceId)1935999960, (eResourceId)1931325262, (eResourceId)1126988621, 0.5f, true),
				new AnimKitResourceInfo((eAnimKitType)7, (eBundleId)2111079703, (eResourceId)1997813458, (eResourceId)1595653543, (eResourceId)0, 1f, true),
				new AnimKitResourceInfo((eAnimKitType)8, (eBundleId)2111079703, (eResourceId)1997813458, (eResourceId)1543900266, (eResourceId)0, 1f, true),
				new AnimKitResourceInfo((eAnimKitType)9, (eBundleId)1251386944, (eResourceId)1337684311, (eResourceId)1931325262, (eResourceId)0, 0.5f, true),
				new AnimKitResourceInfo((eAnimKitType)10, (eBundleId)2055137429, (eResourceId)1558037366, (eResourceId)1931325262, (eResourceId)0, 0.5f, true),
				new AnimKitResourceInfo((eAnimKitType)11, (eBundleId)1593207384, (eResourceId)1982827881, (eResourceId)1738952258, (eResourceId)1934096253, 1f, true),
				new AnimKitResourceInfo((eAnimKitType)12, (eBundleId)1593207384, (eResourceId)1982827881, (eResourceId)1738952258, (eResourceId)2100040657, 1f, true),
				new AnimKitResourceInfo((eAnimKitType)13, (eBundleId)2122918710, (eResourceId)1455522148, (eResourceId)1931325262, (eResourceId)0, 0.00999999978f, false),
				new AnimKitResourceInfo((eAnimKitType)14, (eBundleId)1423300154, (eResourceId)1611507009, (eResourceId)1595653543, (eResourceId)0, 1f, false),
				new AnimKitResourceInfo((eAnimKitType)15, (eBundleId)1423300154, (eResourceId)1611507009, (eResourceId)1543900266, (eResourceId)0, 1f, false),
				new AnimKitResourceInfo((eAnimKitType)16, (eBundleId)1709739176, (eResourceId)1559373856, (eResourceId)1543900266, (eResourceId)0, 1f, false),
				new AnimKitResourceInfo((eAnimKitType)17, (eBundleId)1877359206, (eResourceId)1413647328, (eResourceId)1595653543, (eResourceId)0, 0.600000024f, false),
				new AnimKitResourceInfo((eAnimKitType)18, (eBundleId)1959012844, (eResourceId)1959012844, (eResourceId)1543900266, (eResourceId)0, 1f, false),
				new AnimKitResourceInfo((eAnimKitType)19, (eBundleId)1363682687, (eResourceId)1306857843, (eResourceId)1543900266, (eResourceId)0, 1f, false),
				new AnimKitResourceInfo((eAnimKitType)20, (eBundleId)1768164837, (eResourceId)1290991002, (eResourceId)1931325262, (eResourceId)0, 0.005185185f, false),
				new AnimKitResourceInfo((eAnimKitType)21, (eBundleId)1949820418, (eResourceId)1937779574, (eResourceId)1931325262, (eResourceId)0, 0.005185185f, false),
				new AnimKitResourceInfo((eAnimKitType)22, (eBundleId)2019108612, (eResourceId)1918322631, (eResourceId)1940511881, (eResourceId)0, 1f, false),
				new AnimKitResourceInfo((eAnimKitType)23, (eBundleId)2019108612, (eResourceId)1440644660, (eResourceId)1940511881, (eResourceId)0, 1f, false),
				new AnimKitResourceInfo((eAnimKitType)24, (eBundleId)1593207384, (eResourceId)1982827881, (eResourceId)1543900266, (eResourceId)1934096253, 1f, true),
				new AnimKitResourceInfo((eAnimKitType)25, (eBundleId)1593207384, (eResourceId)1982827881, (eResourceId)1543900266, (eResourceId)2100040657, 1f, true),
				new AnimKitResourceInfo((eAnimKitType)26, (eBundleId)1513268025, (eResourceId)1154907002, (eResourceId)1543900266, (eResourceId)0, 1f, false),
			};
		}

		[global::OpenPCR.Migration("c3466216-30ac-40d2-85c7-a5a4ee723d64")]
		public static AnimKitResourceInfo GetResource(eAnimKitType _spineType)
		{
			return resouceInfoArray[(int)_spineType];
		}

		[global::OpenPCR.Migration("77646e01-8bcf-425e-bf7c-be08e33b1d73")]
		public static bool IsSkinIdSpineType(eAnimKitType _spineType)
		{
			return resouceInfoArray[(int)_spineType].IsSkinId;
		}
	}
}
