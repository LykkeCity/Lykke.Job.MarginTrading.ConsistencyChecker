namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract
{
    public interface IHedgingServiceCheckResult
    {
        ITradingPosition OpenPosition { get; }        
        string Error { get; }
    }
}
