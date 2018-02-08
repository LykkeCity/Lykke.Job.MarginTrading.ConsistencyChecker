using System;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract
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
