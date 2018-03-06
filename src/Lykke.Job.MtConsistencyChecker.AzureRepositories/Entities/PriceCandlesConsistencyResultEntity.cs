using Common;
using Lykke.Job.MtConsistencyChecker.Contract;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Job.MtConsistencyChecker.AzureRepositories.Entities
{
    public class PriceCandlesConsistencyResultEntity : TableEntity, IPriceCandlesConsistencyResult
    {
        public ITradingPosition Position { get; set; }
        public ICandle Candle { get; set; }
        public string Error { get; set; }
        public string PositionJson { get; set; }
        public string CandleJson { get; set; }

        public static IEnumerable<PriceCandlesConsistencyResultEntity> CreateBatch(IEnumerable<IPriceCandlesConsistencyResult> src, DateTime checkDate)
        {
            var counter = 0;
            var res = src.Select(x => Create(x, checkDate, counter++));
            return res;
        }
        private static PriceCandlesConsistencyResultEntity Create(IPriceCandlesConsistencyResult src, DateTime checkDate, int index)
        {
            var partitionKey = checkDate.ToString("yyyyMMdd_HHmm");
            var rowKey = $"{partitionKey}_{index:0000}";
            return new PriceCandlesConsistencyResultEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                Candle = src.Candle,
                Position = src.Position,
                Error = src.Error,
                CandleJson = src.Candle.ToJson(),
                PositionJson = src.Position.ToJson()
            };
        }
    }
}
