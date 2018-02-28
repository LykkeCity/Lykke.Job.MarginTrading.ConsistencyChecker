using System;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract
{
    public interface ITradingPosition
    {
        string Id { get; }
        DateTime Date { get; set; }

        double? CloseCommission { get; }
        DateTime? CloseDate { get; }
        double? ClosePrice { get; }
        string CloseReason { get; }
        string CoreSide { get; }
        string CoreSymbol { get; }
        string FillType { get; }
        double? Fpl { get; }
        double? MatchedCloseVolume { get; }
        double? MatchedVolume { get; }
        double? OpenCommission { get; }
        DateTime? OpenDate { get; }
        double? OpenPrice { get; }
        double? PnL { get; }
        double? PnlInUsd { get; }
        string PositionStatus { get; }
        double SwapCommission { get; }
        string TakerAccountAssetId { get; }
        string TakerAccountId { get; }
        string TakerCounterpartyId { get; }
        string TakerPositionId { get; }
        double Volume { get; }
        double? VolumeUsdAtClose { get; }
        double? VolumeUsdAtOpen { get; }
    }
}
