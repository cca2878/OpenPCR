using global::OpenPCR.AnimKit;
using global::OpenPCR.AnimKit.Scene;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenPCR.AnimKit.Scene
{
	[Serializable]
	public class MeshGenerator
	{
		[Serializable]
		[StructLayout((LayoutKind)0, Pack = 1, Size = 28)]
		public struct Settings
		{
		}

		public Settings settings = global::OpenPCR.MigrationPending.Throw<global::OpenPCR.AnimKit.Scene.MeshGenerator.Settings>("OpenPCR.AnimKit.Scene.MeshGenerator::field settings:OpenPCR.AnimKit.Scene.MeshGenerator+Settings", "This field initializer is not implemented.");
		[NonSerialized]
		private readonly ExposedList<Vector3> vertexBuffer = new ExposedList<Vector3>(4);
		[NonSerialized]
		private readonly ExposedList<Vector2> uvBuffer = new ExposedList<Vector2>(4);
		[NonSerialized]
		private readonly ExposedList<Color32> colorBuffer = new ExposedList<Color32>(4);
		[NonSerialized]
		private readonly ExposedList<ExposedList<int>> submeshes = global::OpenPCR.MigrationPending.Throw<global::OpenPCR.AnimKit.ExposedList<global::OpenPCR.AnimKit.ExposedList<int>>>("OpenPCR.AnimKit.Scene.MeshGenerator::field submeshes:OpenPCR.AnimKit.ExposedList`1<OpenPCR.AnimKit.ExposedList`1<System.Int32>>", "This field initializer is not implemented.");
		[NonSerialized]
		private SkeletonClipping clipper = new SkeletonClipping();
		[NonSerialized]
		private float[] tempVerts = new float[8];

		[global::OpenPCR.Migration("1f00d7c4-1e40-45eb-8908-78bd762df718")]
		public MeshGenerator()
		{
			throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.AnimKit.Scene.MeshGenerator::.ctor(): runtime capability is unavailable.");
		}
	}
}
