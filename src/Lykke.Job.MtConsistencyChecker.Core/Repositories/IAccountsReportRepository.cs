using Lykke.Job.MtConsistencyChecker.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.MtConsistencyChecker.Core.Repositories
{
    public interface IAccountsReportRepository
    {   
        Task<IEnumerable<IAccountsReport>> GetAsync(DateTime? dtFrom, DateTime? dtTo);
    }
}
