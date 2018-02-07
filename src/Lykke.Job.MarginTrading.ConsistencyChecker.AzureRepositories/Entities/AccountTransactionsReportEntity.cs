using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.AzureRepositories.Entities
{
    internal class AccountTransactionsReportEntity: TableEntity, IAccountTransactionsReport
    {
        public string Id
        {
            get => RowKey;
            set => RowKey = value;
        }

        public string AccountId
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }

        public double Amount { get; set; }
        public double Balance { get; set; }
        public string ClientId { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public double WithdrawTransferLimit { get; set; }
        public string PositionId { get; set; }

        public static AccountTransactionsReportEntity Create(IAccountTransactionsReport src)
        {
            return new AccountTransactionsReportEntity
            {
                Id = src.Id,
                Date = src.Date,
                AccountId = src.AccountId,
                ClientId = src.ClientId,
                Amount = src.Amount,
                Balance = src.Balance,
                WithdrawTransferLimit = src.WithdrawTransferLimit,
                Comment = src.Comment,
                Type = src.Type,
                PositionId = src.PositionId
            };
        }
    }
}
