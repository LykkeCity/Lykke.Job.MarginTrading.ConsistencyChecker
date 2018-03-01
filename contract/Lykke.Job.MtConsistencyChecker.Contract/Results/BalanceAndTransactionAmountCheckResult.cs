namespace Lykke.Job.MtConsistencyChecker.Contract.Results
{
    public class BalanceAndTransactionAmountCheckResult : IBalanceAndTransactionAmountCheckResult
    {
        public IAccountsStatReport AccountsStatReport { get; set; }
        public string Error { get; set; }
    }
}
