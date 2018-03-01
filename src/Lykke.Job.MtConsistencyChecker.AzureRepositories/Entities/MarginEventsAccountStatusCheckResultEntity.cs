using Common;
using Lykke.Job.MtConsistencyChecker.Contract;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Job.MtConsistencyChecker.AzureRepositories.Entities
{
    public class MarginEventsAccountStatusCheckResultEntity : TableEntity, IMarginEventsAccountStatusCheckResult
    {
        public IAccountMarginEventReport MarginEvent { get; set; }
        public string Error { get; set; }
        public string MarginEventJson { get; set; }

        public static IEnumerable<MarginEventsAccountStatusCheckResultEntity> CreateBatch(IEnumerable<IMarginEventsAccountStatusCheckResult> src, DateTime checkDate)
        {
            var counter = 0;
            var res = src.Select(x => Create(x, checkDate, counter++));
            return res;
        }

        private static MarginEventsAccountStatusCheckResultEntity Create(IMarginEventsAccountStatusCheckResult src, DateTime checkDate, int index)
        {
            var partitionKey = checkDate.ToString("yyyyMMdd_HHmm");
            var rowKey = $"{partitionKey}_{index.ToString("0000")}";
            return new MarginEventsAccountStatusCheckResultEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                MarginEvent = src.MarginEvent,
                MarginEventJson = src.MarginEvent.ToJson(),
                Error = src.Error,
            };
        }
    }
}
