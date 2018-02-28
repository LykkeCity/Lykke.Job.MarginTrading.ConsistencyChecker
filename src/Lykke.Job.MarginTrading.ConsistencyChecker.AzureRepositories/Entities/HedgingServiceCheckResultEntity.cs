using Common;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.AzureRepositories.Entities
{
    public class HedgingServiceCheckResultEntity : TableEntity, IHedgingServiceCheckResult
    {
        public ITradingPosition OpenPosition { get; set; }
        public string Error { get; set; }
        public string OpenPositionJson { get; set; }

        public static IEnumerable<HedgingServiceCheckResultEntity> CreateBatch(IEnumerable<IHedgingServiceCheckResult> src, DateTime checkDate)
        {
            var counter = 0;
            var res = src.Select(x => Create(x, checkDate, counter++));
            return res;
        }
        private static HedgingServiceCheckResultEntity Create(IHedgingServiceCheckResult src, DateTime checkDate, int index)
        {
            var partitionKey = checkDate.ToString("yyyyMMdd_HHmm");
            var rowKey = $"{partitionKey}_{index.ToString("0000")}";
            return new HedgingServiceCheckResultEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                OpenPosition = src.OpenPosition,
                OpenPositionJson = src.OpenPosition.ToJson(),
                Error = src.Error
            };
        }
    }
}
