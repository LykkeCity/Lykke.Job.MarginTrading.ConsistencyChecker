using System;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract
{
    public interface ITradingOrder
    {
        string CoreSide { get; }
        string CoreSymbol { get; }
        DateTime Date { get; }
        double? ExecutionDuration { get; }
        DateTime? ExecutionTimestamp { get; }
        string Id { get; }
        string TakerAction { get; }
        string TakerCounterpartyId { get; }
        string TakerExternalSymbol { get; }
        string TakerOrderId { get; }
        string TakerPositionId { get; }
        double? TakerRequestedPrice { get; }
        string TimeForceCondition { get; }
        double Volume { get; }
    }
}
