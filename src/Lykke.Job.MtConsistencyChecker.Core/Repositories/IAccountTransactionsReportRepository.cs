using Lykke.Job.MtConsistencyChecker.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.MtConsistencyChecker.Core.Repositories
{
    public interface  IAccountTransactionsReportRepository
    {
        Task<IEnumerable<IAccountTransactionsReport>> GetAsync(DateTime? dtFrom, DateTime? dtTo);
    }
}
