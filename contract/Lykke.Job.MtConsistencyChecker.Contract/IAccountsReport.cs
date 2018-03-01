using System;

namespace Lykke.Job.MtConsistencyChecker.Contract
{
    public interface IAccountsReport
    {
        string Id { get; }
        DateTime Date { get; }
        string TakerCounterpartyId { get; }
        string TakerAccountId { get; }
        string BaseAssetId { get; }
        bool IsLive { get; }
    }
}
