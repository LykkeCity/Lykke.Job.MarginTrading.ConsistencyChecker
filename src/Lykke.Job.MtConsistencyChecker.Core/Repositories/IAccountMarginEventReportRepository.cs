using Lykke.Job.MtConsistencyChecker.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.MtConsistencyChecker.Core.Repositories
{
    public interface IAccountMarginEventReportRepository
    {   
        Task<IEnumerable<IAccountMarginEventReport>> GetAsync(DateTime? dtFrom, DateTime? dtTo);
    }
}
