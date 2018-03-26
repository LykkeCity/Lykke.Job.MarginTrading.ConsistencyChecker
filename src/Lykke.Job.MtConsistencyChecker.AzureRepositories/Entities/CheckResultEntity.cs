using Lykke.Job.MtConsistencyChecker.Contract;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Lykke.Job.MtConsistencyChecker.AzureRepositories.Entities
{
    public class CheckResultEntity : TableEntity, ICheckResult
    {
        public DateTime Date { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Comments { get; set; }

        public static CheckResultEntity Create(ICheckResult src)
        {
            var partitionKey = src.Date.ToString("yyyyMMdd_HHmm");
            var rowKey = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss.fff");
            return new CheckResultEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                Date = src.Date,
                DateFrom = src.DateFrom,
                DateTo = src.DateTo,
                Comments = src.Comments,
                Timestamp = DateTime.UtcNow
            };
        }
    }
    
}
