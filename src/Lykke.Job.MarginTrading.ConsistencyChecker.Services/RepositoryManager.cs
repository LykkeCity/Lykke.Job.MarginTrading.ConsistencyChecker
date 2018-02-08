using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Repositories;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Services
{
    public class RepositoryManager: IRepositoryManager
    {
        private readonly IAccountMarginEventReportRepository _accountMarginEventReportAzure;
        private readonly IAccountMarginEventReportRepository _accountMarginEventReportSql;
        private readonly IAccountsReportRepository _accountsReportAzure;
        private readonly IAccountsReportRepository _accountsReportSql;
        private readonly IAccountsStatReportRepository _accountsStatReportAzure;
        private readonly IAccountsStatReportRepository _accountsStatReportSql;
        private readonly IAccountTransactionsReportRepository _accountTransactionsReportAzure;
        private readonly IAccountTransactionsReportRepository _accountTransactionsReportSql;

        public RepositoryManager(
            IAccountMarginEventReportRepository accountMarginEventReportAzure,
            IAccountMarginEventReportRepository accountMarginEventReportSql,
            IAccountsReportRepository accountsReportAzure,
            IAccountsReportRepository accountsReportSql,
            IAccountsStatReportRepository accountsStatReportAzure,
            IAccountsStatReportRepository accountsStatReportSql,
            IAccountTransactionsReportRepository accountTransactionsReportAzure,
            IAccountTransactionsReportRepository accountTransactionsReportSql
            )
        {
            _accountMarginEventReportAzure = accountMarginEventReportAzure;
            _accountMarginEventReportSql = accountMarginEventReportSql;
            _accountsReportAzure = accountsReportAzure;
            _accountsReportSql = accountsReportSql;
            _accountsStatReportAzure = accountsStatReportAzure;
            _accountsStatReportSql = accountsStatReportSql;
            _accountTransactionsReportAzure = accountTransactionsReportAzure;
            _accountTransactionsReportSql = accountTransactionsReportSql;
        }

        public IAccountMarginEventReportRepository GetAccountMarginEventReport(bool isSql)
        {
            return isSql ? _accountMarginEventReportSql : _accountMarginEventReportAzure;
        }

        public IAccountsReportRepository GetAccountsReport(bool isSql)
        {
            return isSql ? _accountsReportSql : _accountsReportAzure;
        }

        public IAccountsStatReportRepository GetAccountsStatReport(bool isSql)
        {
            return isSql ? _accountsStatReportSql : _accountsStatReportAzure;
        }

        public IAccountTransactionsReportRepository GetAccountTransactionsReport(bool isSql)
        {
            return isSql ? _accountTransactionsReportSql : _accountTransactionsReportAzure;
        }
    }
}
