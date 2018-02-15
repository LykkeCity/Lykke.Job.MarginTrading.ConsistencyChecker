using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Models
{
    public class TradingOrder : ITradingOrder
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }

        public string TakerPositionId { get; set; }
        public string TakerOrderId { get; set; }
        public string TakerCounterpartyId { get; set; }
        public string CoreSide { get; set; }
        public string CoreSymbol { get; set; }
        public double Volume { get; set; }
        public string TakerAction { get; set; }
        public string TakerExternalSymbol { get; set; }
        public double? TakerRequestedPrice { get; set; }
        public string TimeForceCondition { get; set; }
        public DateTime? ExecutionTimestamp { get; set; }
        public double? ExecutionDuration { get; set; }
    }
}
