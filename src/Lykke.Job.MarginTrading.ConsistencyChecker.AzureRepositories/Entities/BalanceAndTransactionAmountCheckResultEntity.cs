using Common;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.AzureRepositories.Entities
{
    class BalanceAndTransactionAmountCheckResultEntity : TableEntity, IBalanceAndTransactionAmountCheckResult
    {
        public IAccountsStatReport AccountsStatReport { get; set; }
        public string Error { get; set; }
        public string AccountsStatReportJson { get; set; }

        public static IEnumerable<BalanceAndTransactionAmountCheckResultEntity> CreateBatch(IEnumerable<IBalanceAndTransactionAmountCheckResult> src, DateTime checkDate)
        {
            var counter = 0;
            var res = src.Select(x => Create(x, checkDate, counter++));
            return res;
        }

        private static BalanceAndTransactionAmountCheckResultEntity Create(IBalanceAndTransactionAmountCheckResult src, DateTime checkDate, int index)
        {
            var partitionKey = checkDate.ToString("yyyyMMdd_HHmm");
            var rowKey = $"{partitionKey}_{index.ToString("0000")}";
            return new BalanceAndTransactionAmountCheckResultEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                AccountsStatReport = src.AccountsStatReport,
                AccountsStatReportJson = src.AccountsStatReport.ToJson(),
                Error = src.Error,
            };
        }
    }
}
