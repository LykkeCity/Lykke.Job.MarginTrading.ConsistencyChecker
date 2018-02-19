using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Services
{
    public interface IPriceCandlesService
    {
        Task<IEnumerable<double>> GetCandleMinute(string assetPait, DateTime interval);
        Task<IEnumerable<double>> GetCandleHour(string assetPait, DateTime interval);
    }
}
