using System;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Core
{
    public static class DateTimeExtensions
    {
        public static string ToDateTimeCustomString(this DateTime src)
        {
            return src.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
    }
}
