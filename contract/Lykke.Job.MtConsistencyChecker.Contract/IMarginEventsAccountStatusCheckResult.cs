namespace Lykke.Job.MtConsistencyChecker.Contract
{
    public interface IMarginEventsAccountStatusCheckResult
    {
        IAccountMarginEventReport MarginEvent { get; }
        string Error { get; }
    }
}
