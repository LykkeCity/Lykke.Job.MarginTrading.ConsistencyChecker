using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services
{
    public interface IConsistencyService
    {
        Task<IEnumerable<IBalanceAndTransactionAmountCheckResult>> CheckBalanceAndTransactionAmount(bool isSql, DateTime? from, DateTime? to);
    }
}
