namespace OpenPCR.Foundation
{
	public class XorShift128Random
	{
		private uint x;
		private uint y;
		private uint z;
		private uint w;

		[global::OpenPCR.Migration("56bd205f-55c7-4eba-bbed-7b2b50d1ad17")]
		public XorShift128Random(uint x, uint y = 362436069u, uint z = 521288629u, uint w = 88675123u)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public uint Next()
		{
			uint oldW = w;
			uint t = x ^ (x << 11);
			x = y;
			y = z;
			uint result = (t >> 8) ^ (oldW >> 19) ^ t ^ oldW;
			z = oldW;
			w = result;
			return result;
		}
	}
}
