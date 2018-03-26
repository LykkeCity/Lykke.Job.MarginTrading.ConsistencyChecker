using Lykke.Job.MtConsistencyChecker.Contract;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Lykke.Job.MtConsistencyChecker.AzureRepositories.Entities
{
    internal class AccountsStatReportEntity: TableEntity, IAccountsStatReport
    {
        public string BaseAssetId
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }

        public string AccountId
        {
            get => RowKey;
            set => RowKey = value;
        }

        public string ClientId { get; set; }
        public string TradingConditionId { get; set; }
        public double Balance { get; set; }
        public double WithdrawTransferLimit { get; set; }
        public double MarginCall { get; set; }
        public double StopOut { get; set; }
        public double TotalCapital { get; set; }
        public double FreeMargin { get; set; }
        public double MarginAvailable { get; set; }
        public double UsedMargin { get; set; }
        public double MarginInit { get; set; }
        public double PnL { get; set; }
        public double OpenPositionsCount { get; set; }
        public double MarginUsageLevel { get; set; }
        public bool IsLive { get; set; }

        public string Id => AccountId;

        public DateTime Date => Timestamp.DateTime;

        public static AccountsStatReportEntity Create(IAccountsStatReport src)
        {
            return new AccountsStatReportEntity
            {
                BaseAssetId = src.BaseAssetId,
                AccountId = src.AccountId,
                ClientId = src.ClientId,
                TradingConditionId = src.TradingConditionId,
                Balance = src.Balance,
                WithdrawTransferLimit = src.WithdrawTransferLimit,
                MarginCall = src.MarginCall,
                StopOut = src.StopOut,
                TotalCapital = src.TotalCapital,
                FreeMargin = src.FreeMargin,
                MarginAvailable = src.MarginAvailable,
                UsedMargin = src.UsedMargin,
                MarginInit = src.MarginInit,
                PnL = src.PnL,
                OpenPositionsCount = src.OpenPositionsCount,
                MarginUsageLevel = src.MarginUsageLevel,
                IsLive = src.IsLive
            };
        }
    }
}
