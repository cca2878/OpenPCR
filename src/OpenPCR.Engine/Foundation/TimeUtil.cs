using System;

namespace OpenPCR.Foundation
{
	public class TimeUtil
	{
		private static DateTime UNIX_EPOCH { get; }

		static TimeUtil()
		{
			UNIX_EPOCH = new DateTime(1970, 1, 1, 8, 0, 0, 0);
		}

		public static DateTime GetNowTime(long serverTime, long connectClientTime)
		{
			DateTime epoch = UNIX_EPOCH;
			long deviceOperatingTime = TimeNativePlugin.GetDeviceOperatingTime();
			long seconds = unchecked(serverTime - connectClientTime + deviceOperatingTime);
			return epoch.AddSeconds(Convert.ToDouble(seconds));
		}

		public static long ToUnixTime(DateTime dateTime)
		{
			return global::OpenPCR.NativeNumeric.ToInt64((dateTime - UNIX_EPOCH).TotalSeconds);
		}
	}
}
