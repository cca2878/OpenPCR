using global::OpenPCR.AnimKit.Scene;
using global::OpenPCR.Runtime.Api;
using System;
using System.IO;
using Elements;
using System.Collections.Generic;

namespace OpenPCR.AnimKit.Scene
{
	public class SkeletonDataAsset : ScriptableObject
	{
		public AtlasAsset[] atlasAssets = new AtlasAsset[0];
		public float scale = 0.01f;
		public string[] fromAnimation = new string[0];
		public string[] toAnimation = new string[0];
		public float[] duration = new float[0];
		public float defaultMix;
		private float skeletonDataScale = 1f;

		public SkeletonDataAsset()
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.AnimKit.Scene.SkeletonDataAsset::.ctor()", "body migration pending");
		}
	}
}
