namespace Lykke.Job.MtConsistencyChecker.Contract
{
    public interface IPriceCandlesConsistencyResult
    {
        ITradingPosition Position { get; }
        ICandle Candle { get; }
        string Error { get; }
    }
}
