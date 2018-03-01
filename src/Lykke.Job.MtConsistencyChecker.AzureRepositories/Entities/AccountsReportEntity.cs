using Lykke.Job.MtConsistencyChecker.Contract;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Lykke.Job.MtConsistencyChecker.AzureRepositories.Entities
{
    internal class AccountsReportEntity: TableEntity,IAccountsReport
    {
        public string TakerCounterpartyId
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }

        public string TakerAccountId
        {
            get => RowKey;
            set => RowKey = value;
        }

        public string BaseAssetId { get; set; }
        public bool IsLive { get; set; }

        public string Id => TakerAccountId;
        public DateTime Date => Timestamp.DateTime;

        public static AccountsReportEntity Create(IAccountsReport src)
        {
            return new AccountsReportEntity
            {
                TakerCounterpartyId = src.TakerCounterpartyId,
                TakerAccountId = src.TakerAccountId,
                BaseAssetId = src.BaseAssetId,
                IsLive = src.IsLive
            };
        }
    }
}
