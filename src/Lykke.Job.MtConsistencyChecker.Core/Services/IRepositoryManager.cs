using Lykke.Job.MtConsistencyChecker.Core.Repositories;

namespace Lykke.Job.MtConsistencyChecker.Core.Services
{
    public interface IRepositoryManager
    {
        IAccountMarginEventReportRepository GetAccountMarginEventReport(bool isSql);
        IAccountsReportRepository GetAccountsReport(bool isSql);
        IAccountsStatReportRepository GetAccountsStatReport(bool isSql);
        IAccountTransactionsReportRepository GetAccountTransactionsReport(bool isSql);
        ITradingPositionRepository GetTradingPosition(bool isSql);
        ITradingOrderRepository GetTradingOrder(bool isSql);
    }
}
