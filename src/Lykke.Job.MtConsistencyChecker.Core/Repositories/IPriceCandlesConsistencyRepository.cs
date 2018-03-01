using Lykke.Job.MtConsistencyChecker.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.MtConsistencyChecker.Core.Repositories
{
    public interface IPriceCandlesConsistencyRepository
    {
        Task AddAsync(IEnumerable<IPriceCandlesConsistencyResult> entities, DateTime checkDate);
    }
}
