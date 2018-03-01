namespace Lykke.Job.MtConsistencyChecker.Contract
{
    public interface IBalanceAndTransactionAmountCheckResult
    {
        IAccountsStatReport AccountsStatReport { get; }
        string Error { get; }
    }
}
