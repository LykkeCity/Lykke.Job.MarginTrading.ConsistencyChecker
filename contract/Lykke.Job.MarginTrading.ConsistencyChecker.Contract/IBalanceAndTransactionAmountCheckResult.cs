namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract
{
    public interface IBalanceAndTransactionAmountCheckResult
    {
        IAccountsStatReport AccountsStatReport { get; }
        string Error { get; }
    }
}
