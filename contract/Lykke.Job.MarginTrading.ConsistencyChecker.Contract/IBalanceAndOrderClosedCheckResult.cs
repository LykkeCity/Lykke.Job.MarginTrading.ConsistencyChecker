namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract
{
    public interface IBalanceAndOrderClosedCheckResult
    {
        ITradingPosition TradingPosition { get; }
        IAccountTransactionsReport AccountTransaction { get; }
        string Error { get; }
    }
}
