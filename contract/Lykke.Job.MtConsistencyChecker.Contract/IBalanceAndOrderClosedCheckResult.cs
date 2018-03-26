namespace Lykke.Job.MtConsistencyChecker.Contract
{
    public interface IBalanceAndOrderClosedCheckResult
    {
        ITradingPosition TradingPosition { get; }
        IAccountTransactionsReport AccountTransaction { get; }
        string Error { get; }
    }
}
