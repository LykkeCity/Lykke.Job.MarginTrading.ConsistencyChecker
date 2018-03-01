using Lykke.Job.MtConsistencyChecker.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.MtConsistencyChecker.Core.Repositories
{
    public interface IBalanceAndOrderClosedRepository
    {
        Task AddAsync(IEnumerable<IBalanceAndOrderClosedCheckResult> entities, DateTime checkDate);
    }
}
