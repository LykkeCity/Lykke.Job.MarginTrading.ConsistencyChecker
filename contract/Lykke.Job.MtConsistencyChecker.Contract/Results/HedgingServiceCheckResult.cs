namespace Lykke.Job.MtConsistencyChecker.Contract.Results
{
    public class HedgingServiceCheckResult : IHedgingServiceCheckResult
    {
        public ITradingPosition OpenPosition { get; set; }
        public string Error { get; set; }
    }
}
