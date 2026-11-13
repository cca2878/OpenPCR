using global::OpenPCR.AnimKit;
using System.Runtime.CompilerServices;

namespace OpenPCR.AnimKit
{
	public class RegionAttachment : Attachment
	{
		internal float x;
		internal float y;
		internal float rotation;
		internal float scaleX = 1f;
		internal float scaleY = 1f;
		internal float width;
		internal float height;
		internal float regionOffsetX;
		internal float regionOffsetY;
		internal float regionWidth;
		internal float regionHeight;
		internal float regionOriginalWidth;
		internal float regionOriginalHeight;
		internal float[] offset = new float[8];
		internal float[] uvs = new float[8];
		internal float r = 1f;
		internal float g = 1f;
		internal float b = 1f;
		internal float a = 1f;
		public object RendererObject;

		public RegionAttachment(string name) : base(name)
		{
		}

		public void SetUVs(float u, float v, float u2, float v2, bool rotate)
		{
			float[] array = uvs;
			if (rotate)
			{
				array[4] = u;
				array[5] = v2;
				array[6] = u;
				array[7] = v;
				array[0] = u2;
				array[1] = v;
				array[2] = u2;
				array[3] = v2;
			}
			else
			{
				array[2] = u;
				array[3] = v2;
				array[4] = u;
				array[5] = v;
				array[6] = u2;
				array[7] = v;
				array[0] = u2;
				array[1] = v2;
			}
		}

		public string Path { get; set; }

		[global::OpenPCR.Migration("cde91403-bfd5-46f0-9c96-309372ac22a5")]
		public void UpdateOffset()
		{
			float num = width;
			float num2 = height;
			float num3 = scaleX;
			float num4 = scaleY;
			float num5 = num / regionOriginalWidth * num3;
			float num6 = num2 / regionOriginalHeight * num4;
			float num7 = regionOffsetX * num5 - (num * 0.5f) * num3;
			float num8 = regionOffsetY * num6 - (num2 * 0.5f) * num4;
			float num9 = num7 + regionWidth * num5;
			float num10 = num8 + regionHeight * num6;
			float degrees = rotation;
			float num11 = MathUtils.CosDeg(degrees);
			float num12 = MathUtils.SinDeg(degrees);
			float num13 = x;
			float num14 = y;
			float num15 = num7 * num11 + num13;
			float num16 = num7 * num12;
			float num17 = num8 * num11 + num14;
			float num18 = num8 * num12;
			float num19 = num9 * num11 + num13;
			float num20 = num9 * num12;
			float num21 = num10 * num11 + num14;
			float num22 = num10 * num12;
			float[] array = offset;
			array[0] = num15 - num18;
			array[1] = num17 + num16;
			array[2] = num15 - num22;
			array[3] = num21 + num16;
			array[4] = num19 - num22;
			array[5] = num21 + num20;
			array[6] = num19 - num18;
			array[7] = num17 + num20;
		}
	}
}
