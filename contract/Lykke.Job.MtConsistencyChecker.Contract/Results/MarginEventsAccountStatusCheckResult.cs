namespace Lykke.Job.MtConsistencyChecker.Contract.Results
{
    public class MarginEventsAccountStatusCheckResult : IMarginEventsAccountStatusCheckResult
    {
        public IAccountMarginEventReport MarginEvent { get; set; }
        public string Error { get; set; }
    }
}
