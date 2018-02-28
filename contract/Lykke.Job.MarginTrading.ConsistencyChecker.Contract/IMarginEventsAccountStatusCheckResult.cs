namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract
{
    public interface IMarginEventsAccountStatusCheckResult
    {
        IAccountMarginEventReport MarginEvent { get; }
        string Error { get; }
    }
}
