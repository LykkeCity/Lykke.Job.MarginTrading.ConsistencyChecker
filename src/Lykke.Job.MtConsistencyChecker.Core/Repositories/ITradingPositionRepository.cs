using Lykke.Job.MtConsistencyChecker.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.MtConsistencyChecker.Core.Repositories
{
    public interface ITradingPositionRepository
    {
        Task<IEnumerable<ITradingPosition>> GetOpenedAsync(DateTime? dtFrom, DateTime? dtTo);
        Task<IEnumerable<ITradingPosition>> GetClosedAsync(DateTime? dtFrom, DateTime? dtTo);
    }
}
