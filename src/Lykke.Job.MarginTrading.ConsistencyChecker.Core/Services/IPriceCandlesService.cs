using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services
{
    public interface IPriceCandlesService
    {
        Task<IEnumerable<ICandle>> GetMinuteCandle(string assetPair, bool isAsk, DateTime from, DateTime to);
    }
}
