using System;

namespace Template.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToShortDateTimeString(this DateTime value)
        {
            return value.ToString("dd/MM/yyyy hh:mm tt");
        }

        public static string ToLongDateTimeString(this DateTime value)
        {
            return value.ToString("dddd, dd MMMM yyyy hh:mm tt");
        }
    }
}
