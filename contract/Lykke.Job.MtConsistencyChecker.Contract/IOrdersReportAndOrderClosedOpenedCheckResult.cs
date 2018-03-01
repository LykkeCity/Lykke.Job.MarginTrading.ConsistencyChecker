using System.Collections.Generic;

namespace Lykke.Job.MtConsistencyChecker.Contract
{
    public interface IOrdersReportAndOrderClosedOpenedCheckResult
    {
        IEnumerable<ITradingOrder> OrderReport { get; }
        ITradingPosition Position { get; }
        string Error { get; }
    }
}
