namespace Lykke.Job.MtConsistencyChecker.Contract.Results
{
    public class BalanceAndOrderClosedCheckResult : IBalanceAndOrderClosedCheckResult
    {
        public ITradingPosition TradingPosition { get; set; }

        public IAccountTransactionsReport AccountTransaction { get; set; }

        public string Error { get; set; }
    }
}
