namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Results
{
    public class HedgingServiceCheckResult : IHedgingServiceCheckResult
    {
        public ITradingPosition OpenPosition { get; set; }
        public string Error { get; set; }
    }
}
