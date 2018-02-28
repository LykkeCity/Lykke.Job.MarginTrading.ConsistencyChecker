namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Results
{
    public class PriceCandlesConsistencyResult : IPriceCandlesConsistencyResult
    {
        public ITradingPosition Position { get; set; }
        public ICandle Candle { get; set; }
        public string Error { get; set; }
    }
}
