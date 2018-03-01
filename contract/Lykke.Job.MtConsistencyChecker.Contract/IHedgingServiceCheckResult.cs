namespace Lykke.Job.MtConsistencyChecker.Contract
{
    public interface IHedgingServiceCheckResult
    {
        ITradingPosition OpenPosition { get; }        
        string Error { get; }
    }
}
