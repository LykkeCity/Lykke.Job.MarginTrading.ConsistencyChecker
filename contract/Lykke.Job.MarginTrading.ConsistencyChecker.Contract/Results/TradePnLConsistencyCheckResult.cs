namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Results
{
    public class TradePnLConsistencyCheckResult : ITradePnLConsistencyCheckResult
    {
        public ITradingPosition Position { get; set; }
        public string Error { get; set; }
    }
}
