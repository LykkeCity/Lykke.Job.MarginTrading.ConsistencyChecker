using System.Collections.Generic;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract
{
    public interface IOrdersReportAndOrderClosedOpenedCheckResult
    {
        IEnumerable<ITradingOrder> OrderReport { get; }
        ITradingPosition Position { get; }
        string Error { get; }
    }
}
