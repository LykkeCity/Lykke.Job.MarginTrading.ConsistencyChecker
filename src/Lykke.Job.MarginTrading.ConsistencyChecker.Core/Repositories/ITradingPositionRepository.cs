using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Core.Repositories
{
    public interface ITradingPositionRepository
    {
        Task<IEnumerable<ITradingPosition>> GetOpenedAsync(DateTime? dtFrom, DateTime? dtTo);
        Task<IEnumerable<ITradingPosition>> GetClosedAsync(DateTime? dtFrom, DateTime? dtTo);
    }
}
