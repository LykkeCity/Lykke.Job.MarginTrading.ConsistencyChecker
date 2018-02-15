namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Models
{
    public class BalanceAndOrderClosedCheckResult : IBalanceAndOrderClosedCheckResult
    {
        public ITradingPosition TradingPosition { get; set; }

        public IAccountTransactionsReport AccountTransaction { get; set; }

        public string Error { get; set; }

        
    }
}
