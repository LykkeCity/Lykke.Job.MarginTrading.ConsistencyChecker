using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services
{
    public interface IPriceCandlesService
    {
        Task<List<double>> GetCandle(string assetPair, DateTime interval)
    }
}
