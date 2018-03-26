using Lykke.Job.MtConsistencyChecker.Core.Repositories;
using Lykke.Job.MtConsistencyChecker.Core.Services;

namespace Lykke.Job.MtConsistencyChecker.Services
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

        private readonly ITradingPositionRepository _tradingPositionSql;
        private readonly ITradingPositionRepository _tradingPositionAzure;

        private readonly ITradingOrderRepository _tradingOrderAzure;
        private readonly ITradingOrderRepository _tradingOrderSql;

        public RepositoryManager(
            IAccountMarginEventReportRepository accountMarginEventReportAzure,
            IAccountMarginEventReportRepository accountMarginEventReportSql,
            IAccountsReportRepository accountsReportAzure,
            IAccountsReportRepository accountsReportSql,
            IAccountsStatReportRepository accountsStatReportAzure,
            IAccountsStatReportRepository accountsStatReportSql,
            IAccountTransactionsReportRepository accountTransactionsReportAzure,
            IAccountTransactionsReportRepository accountTransactionsReportSql,
            ITradingPositionRepository tradingPositionAzure,
            ITradingPositionRepository tradingPositionSql,
            ITradingOrderRepository tradingOrderAzure,
            ITradingOrderRepository tradingOrderSql
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
            _tradingPositionAzure = tradingPositionAzure;
            _tradingPositionSql = tradingPositionSql;
            _tradingOrderAzure = tradingOrderAzure;
            _tradingOrderSql = tradingOrderSql;
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

        public ITradingOrderRepository GetTradingOrder(bool isSql)
        {
            return isSql ? _tradingOrderSql : _tradingOrderAzure;
        }

        public ITradingPositionRepository GetTradingPosition(bool isSql)
        {
            return isSql ? _tradingPositionSql : _tradingPositionAzure;
        }
    }
}
