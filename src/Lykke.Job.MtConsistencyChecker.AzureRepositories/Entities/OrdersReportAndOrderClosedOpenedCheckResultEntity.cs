using Common;
using Lykke.Job.MtConsistencyChecker.Contract;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Job.MtConsistencyChecker.AzureRepositories.Entities
{
    public class OrdersReportAndOrderClosedOpenedCheckResultEntity :TableEntity, IOrdersReportAndOrderClosedOpenedCheckResult
    {
        public IEnumerable<ITradingOrder> OrderReport { get; set; }
        public ITradingPosition Position { get; set; }
        public string Error { get; set; }
        public string PositionJson { get; set; }
        public string OrderReportJson { get; set; }

        public static IEnumerable<OrdersReportAndOrderClosedOpenedCheckResultEntity> CreateBatch(IEnumerable<IOrdersReportAndOrderClosedOpenedCheckResult> src, DateTime checkDate)
        {
            var counter = 0;
            var res = src.Select(x => Create(x, checkDate, counter++));
            return res;
        }

        private static OrdersReportAndOrderClosedOpenedCheckResultEntity Create(IOrdersReportAndOrderClosedOpenedCheckResult src, DateTime checkDate, int index)
        {
            var partitionKey = checkDate.ToString("yyyyMMdd_HHmm");
            var rowKey = $"{partitionKey}_{index:0000}";
            return new OrdersReportAndOrderClosedOpenedCheckResultEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                OrderReport = src.OrderReport,
                Position = src.Position,
                OrderReportJson = src.OrderReport.ToJson(),
                PositionJson = src.Position.ToJson(),
                Error = src.Error,
            };
        }

    }
}
