using Lykke.Job.MtConsistencyChecker.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.MtConsistencyChecker.Core.Repositories
{
    public interface IOrdersReportAndOrderClosedOpenedRepository
    {
        Task AddAsync(IEnumerable<IOrdersReportAndOrderClosedOpenedCheckResult> entities, DateTime checkDate);
    }
}
