using Lykke.Job.MtConsistencyChecker.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.MtConsistencyChecker.Core.Repositories
{
    public interface IBalanceAndTransactionAmountRepository
    {
        Task AddAsync(IEnumerable<IBalanceAndTransactionAmountCheckResult> entities, DateTime checkDate);
    }
}
