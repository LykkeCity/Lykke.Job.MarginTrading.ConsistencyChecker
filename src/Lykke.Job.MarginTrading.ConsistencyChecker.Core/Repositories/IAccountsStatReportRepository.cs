using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Core.Repositories
{
    public interface IAccountsStatReportRepository
    {   
        Task<IEnumerable<IAccountsStatReport>> GetAsync(DateTime? dtFrom, DateTime? dtTo);
    }
}
