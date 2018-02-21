namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract
{
    public interface IPriceCandlesConsistencyResult
    {
        ITradingPosition Position { get; }
        ICandle Candle { get; }
        string Error { get; }
    }
}
