using global::OpenPCR.AnimKit;
using System.Runtime.CompilerServices;

namespace OpenPCR.AnimKit
{
	public class MeshAttachment : VertexAttachment
	{
		internal float regionOffsetX;
		internal float regionOffsetY;
		internal float regionWidth;
		internal float regionHeight;
		internal float regionOriginalWidth;
		internal float regionOriginalHeight;
		internal float[] uvs;
		internal float[] regionUVs;
		internal int[] triangles;
		internal float r = 1f;
		internal float g = 1f;
		internal float b = 1f;
		internal float a = 1f;
		internal int hulllength;
		internal bool inheritDeform;
		public object RendererObject;
		public float RegionU { get; set; }
		public float RegionV { get; set; }
		public float RegionU2 { get; set; }
		public float RegionV2 { get; set; }
		public bool RegionRotate { get; set; }

		public MeshAttachment(string name) : base(name)
		{
		}

		public string Path { get; set; }

		public int HullLength
		{
			get
			{
				return hulllength;
			}
			set
			{
				hulllength = value;
			}
		}

		public int[] Edges { get; set; }

		public float Width { get; set; }

		public float Height { get; set; }

		[global::OpenPCR.Migration("ac82507b-99b4-4281-bf07-ff2645e7f4dc")]
		public void UpdateUVs()
		{
			float regionU = RegionU;
			float regionV = RegionV;
			float num = RegionU2 - RegionU;
			float num2 = RegionV2 - RegionV;
			float[] array = regionUVs;
			if (uvs == null || uvs.Length != array.Length)
			{
				uvs = new float[array.Length];
			}
			float[] array2 = uvs;
			if (RegionRotate)
			{
				int i = 0;
				for (int num3 = array2.Length; i < num3; i += 2)
				{
					array2[i] = regionU + array[i + 1] * num;
					array2[i + 1] = regionV + num2 - array[i] * num2;
				}
			}
			else
			{
				int j = 0;
				for (int num4 = array2.Length; j < num4; j += 2)
				{
					array2[j] = regionU + array[j] * num;
					array2[j + 1] = regionV + array[j + 1] * num2;
				}
			}
		}
	}
}
