namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Results
{
    public class BalanceAndOrderClosedCheckResult : IBalanceAndOrderClosedCheckResult
    {
        public ITradingPosition TradingPosition { get; set; }

        public IAccountTransactionsReport AccountTransaction { get; set; }

        public string Error { get; set; }

        
    }
}
