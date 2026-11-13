using System.Globalization;

namespace OpenPCR.Foundation
{
	public static class XXHash64Utility
	{
		public static ulong XXHash64StringToU64(string _hexString)
		{
			return ulong.TryParse(_hexString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result) ? result : 0UL;
		}
	}
}
