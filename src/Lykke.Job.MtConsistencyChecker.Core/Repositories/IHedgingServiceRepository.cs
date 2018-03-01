using Lykke.Job.MtConsistencyChecker.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.MtConsistencyChecker.Core.Repositories
{
    public interface IHedgingServiceRepository
    {
        Task AddAsync(IEnumerable<IHedgingServiceCheckResult> entities, DateTime checkDate);
    }
}
