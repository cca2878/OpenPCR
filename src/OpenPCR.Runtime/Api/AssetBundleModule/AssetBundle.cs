using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{











	public unsafe class @AssetBundle : global::OpenPCR.Runtime.Api.Object
	{


		[global::OpenPCR.Migration("12055772-ca94-4169-9915-0c1a7b9f3e46")]
		private @AssetBundle() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		[global::OpenPCR.Migration("494a6d1b-fc72-4591-bf30-1401a0a61678")]
		public static global::OpenPCR.Runtime.Api.AssetBundle @LoadFromFile(string @path) { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		[global::OpenPCR.Migration("7232a14c-1c28-4fe0-b486-3b3b3ad893c8")]
		public global::OpenPCR.Runtime.Api.Object[] @LoadAllAssets() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }




		[global::OpenPCR.Migration("0b4ae6e1-bafc-403b-87bc-3be5237f3e7f")]
		public void @Unload(bool @unloadAllLoadedObjects) { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }



		[global::OpenPCR.Migration("af2f98cf-0e70-4ce2-8827-656a22f22280")]
		public string[] @GetAllAssetNames() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
	}
}
