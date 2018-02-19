namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract
{
    public interface IPriceCandlesConsistencyResult
    {
        ITradingPosition Position { get; }
        string Error { get; }
    }
}
