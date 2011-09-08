using System;

namespace Utilities.General
{
	public static class DateTimeExtensions
	{
		public static DateTime EndOfDay(this DateTime dateTime)
		{
			return dateTime.AddDays(1).AddSeconds(-1);
		}
	}
}