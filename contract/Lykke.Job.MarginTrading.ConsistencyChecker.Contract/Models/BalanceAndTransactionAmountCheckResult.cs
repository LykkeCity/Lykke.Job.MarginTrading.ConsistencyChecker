namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Models
{
    public class BalanceAndTransactionAmountCheckResult : IBalanceAndTransactionAmountCheckResult
    {
        public IAccountsStatReport AccountsStatReport { get; set; }
        public string Error { get; set; }
    }
}
