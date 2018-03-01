using Lykke.Job.MtConsistencyChecker.Contract;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Lykke.Job.MtConsistencyChecker.AzureRepositories.Entities
{
    public class TradingOrderEntity: TableEntity, ITradingOrder
    {
        public DateTime Date => Timestamp.UtcDateTime;

        public string Id => GetPartitionKey();

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



        private string GetPartitionKey()
        {
            return $"{TakerCounterpartyId}_{CoreSymbol}";
        }

        private string GetRowKey()
        {
            return TakerOrderId;
        }

        public static TradingOrderEntity Create(ITradingOrder src)
        {
            var entity = new TradingOrderEntity
            {
                TakerPositionId = src.TakerPositionId,
                TakerOrderId = src.TakerOrderId,
                CoreSide = src.CoreSide,
                CoreSymbol = src.CoreSymbol,
                ExecutionDuration = src.ExecutionDuration,
                ExecutionTimestamp = src.ExecutionTimestamp,
                TakerAction = src.TakerAction,
                TakerCounterpartyId = src.TakerCounterpartyId,
                TakerExternalSymbol = src.TakerExternalSymbol,
                TakerRequestedPrice = src.TakerRequestedPrice,
                TimeForceCondition = src.TimeForceCondition,
                Volume = src.Volume
            };

            entity.PartitionKey = entity.GetPartitionKey();
            entity.RowKey = entity.GetRowKey();

            return entity;
        }
    }
}
