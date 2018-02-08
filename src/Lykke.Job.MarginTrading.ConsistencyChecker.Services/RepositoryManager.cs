using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Repositories;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Services
{
    public class RepositoryManager: IRepositoryManager
    {
        private readonly IAccountMarginEventReportRepository _accountMarginEventReportAzure;
        private readonly IAccountMarginEventReportRepository _accountMarginEventReportSql;

        public RepositoryManager(
            IAccountMarginEventReportRepository accountMarginEventReportAzure,
            IAccountMarginEventReportRepository accountMarginEventReportSql)
        {
            _accountMarginEventReportAzure = accountMarginEventReportAzure;
            _accountMarginEventReportSql = accountMarginEventReportSql;
        }

        public IAccountMarginEventReportRepository GetAccountMarginEventReport(bool isSql)
        {
            return isSql ? _accountMarginEventReportSql : _accountMarginEventReportAzure;
        }

    }
}
