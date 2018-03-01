using System;

namespace Lykke.Job.MtConsistencyChecker.Contract.Results
{
    public class BalanceAndOrderClosedCheckResult : IBalanceAndOrderClosedCheckResult
    {
        public DateTime CheckDate { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public ITradingPosition TradingPosition { get; set; }

        public IAccountTransactionsReport AccountTransaction { get; set; }

        public string Error { get; set; }
    }
}
