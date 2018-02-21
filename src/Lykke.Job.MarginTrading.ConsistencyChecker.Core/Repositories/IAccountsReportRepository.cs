using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Core.Repositories
{
    public interface IAccountsReportRepository
    {   
        Task<IEnumerable<IAccountsReport>> GetAsync(DateTime? dtFrom, DateTime? dtTo);
    }
}
