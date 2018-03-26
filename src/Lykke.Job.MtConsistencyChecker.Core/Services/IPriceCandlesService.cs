using Lykke.Job.MtConsistencyChecker.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.MtConsistencyChecker.Core.Services
{
    public interface IPriceCandlesService
    {
        Task<IEnumerable<ICandle>> GetMinuteCandle(string assetPair, bool isAsk, DateTime from, DateTime to);
    }
}
