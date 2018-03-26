namespace Lykke.Job.MtConsistencyChecker.Contract.Results
{
    public class PriceCandlesConsistencyResult : IPriceCandlesConsistencyResult
    {
        public ITradingPosition Position { get; set; }
        public ICandle Candle { get; set; }
        public string Error { get; set; }
    }
}
