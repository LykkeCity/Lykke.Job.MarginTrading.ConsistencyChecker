using System.Collections.Generic;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Results
{
    public class OrdersReportAndOrderClosedOpenedCheckResult : IOrdersReportAndOrderClosedOpenedCheckResult
    {
        public IEnumerable<ITradingOrder> OrderReport { get; set; }
        public ITradingPosition Position { get; set; }
        public string Error { get; set; }
    }
}
