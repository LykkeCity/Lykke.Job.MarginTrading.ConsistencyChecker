using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Repositories;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services
{
    public interface IRepositoryManager
    {
        IAccountMarginEventReportRepository GetAccountMarginEventReport(bool isSql);
    }
}
