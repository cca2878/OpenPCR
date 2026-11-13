using global::OpenPCR.Foundation;
using System;

namespace OpenPCR.Foundation
{
	public class TimeData
	{
		private static readonly DateTime UNIX_EPOCH;
		public const int NEXT_DAY_HOUR = 5;

		private DateTime nextDayTime;
		private long serverTime;
		private long connectClientTime;
		private DateTime connectClientDateTime;

		static TimeData()
		{
			UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0);
		}

		public TimeData()
		{
			nextDayTime = UNIX_EPOCH;
			connectClientDateTime = DateTime.Now;
		}

		public DateTime GetNowTime()
		{
			return TimeUtil.GetNowTime(serverTime, connectClientTime);
		}
	}
}
