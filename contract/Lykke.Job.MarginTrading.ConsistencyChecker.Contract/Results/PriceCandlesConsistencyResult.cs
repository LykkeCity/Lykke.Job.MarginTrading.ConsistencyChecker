namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Models
{
    public class PriceCandlesConsistencyResult : IPriceCandlesConsistencyResult
    {
        public ITradingPosition Position { get; set; }
        public string Error { get; set; }
    }
}
