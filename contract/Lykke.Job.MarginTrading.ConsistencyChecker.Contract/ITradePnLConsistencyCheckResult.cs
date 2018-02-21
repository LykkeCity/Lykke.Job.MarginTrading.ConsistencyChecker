namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract
{
    public interface ITradePnLConsistencyCheckResult
    {
        ITradingPosition Position { get; }        
        string Error { get; }
    }
}
