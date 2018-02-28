namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Results
{
    public class BalanceAndTransactionAmountCheckResult : IBalanceAndTransactionAmountCheckResult
    {
        public IAccountsStatReport AccountsStatReport { get; set; }
        public string Error { get; set; }
    }
}
