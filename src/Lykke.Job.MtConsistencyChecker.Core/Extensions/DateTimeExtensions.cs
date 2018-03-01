using System;

namespace Lykke.Job.MtConsistencyChecker.Core
{
    public static class DateTimeExtensions
    {
        public static string ToDateTimeCustomString(this DateTime src)
        {
            return src.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
    }
}
