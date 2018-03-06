using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Lykke.Job.MtConsistencyChecker.Contract;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Job.MtConsistencyChecker.AzureRepositories.Entities
{
    public class BalanceAndOrderClosedCheckResultEntity : TableEntity, IBalanceAndOrderClosedCheckResult
    {   
        public ITradingPosition TradingPosition { get; set; }
        public IAccountTransactionsReport AccountTransaction { get; set; }
        public string Error { get; set; }
        public string TradingPositionJson { get; set; }
        public string AccountTransactionJson { get; set; }

        public static IEnumerable<BalanceAndOrderClosedCheckResultEntity> CreateBatch(IEnumerable<IBalanceAndOrderClosedCheckResult> src, DateTime checkDate)
        {
            var counter = 0;            
            var res = src.Select(x => Create(x, checkDate, counter++));
            return res;            
        }
        private static BalanceAndOrderClosedCheckResultEntity Create(IBalanceAndOrderClosedCheckResult src, DateTime checkDate, int index)
        {
            var partitionKey = checkDate.ToString("yyyyMMdd_HHmm");
            var rowKey = $"{partitionKey}_{index:0000}";
            return new BalanceAndOrderClosedCheckResultEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                AccountTransaction = src.AccountTransaction,                
                Error = src.Error,
                TradingPosition = src.TradingPosition,
                AccountTransactionJson = src.AccountTransaction.ToJson(),
                TradingPositionJson = src.TradingPosition.ToJson()
            };
        }
    }
}
