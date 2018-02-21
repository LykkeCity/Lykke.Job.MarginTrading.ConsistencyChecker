using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Job.MarginTrading.ConsistencyChecker.AzureRepositories.Entities;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Repositories;
using Lykke.SettingsReader;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.AzureRepositories
{
    public class AccountsStatReportRepository : IAccountsStatReportRepository
    {
        private readonly INoSQLTableStorage<AccountsStatReportEntity> _tableStorage;

        public AccountsStatReportRepository(IReloadingManager<string> connectionString, ILog log)
        {
            _tableStorage = AzureTableStorage<AccountsStatReportEntity>.Create(connectionString,
                "ClientAccountsStatusReports", log);
        }

        public async Task<IEnumerable<IAccountsStatReport>> GetAsync(DateTime? dtFrom, DateTime? dtTo)
        {
            var partitionKeys = await GetPartitionKeys();
            return (await _tableStorage.WhereAsync(partitionKeys, dtFrom ?? DateTime.MinValue, dtTo ?? DateTime.MaxValue, ToIntervalOption.IncludeTo))
                .OrderByDescending(item => item.Timestamp);
        }

        private async Task<IEnumerable<string>> GetPartitionKeys()
        {
            System.Collections.Concurrent.ConcurrentDictionary<string, byte> partitionKeys = new System.Collections.Concurrent.ConcurrentDictionary<string, byte>();
            await _tableStorage.ExecuteAsync(new TableQuery<AccountsStatReportEntity>(), entity =>
            {
                foreach (var et in entity.Select(m => m.PartitionKey))
                    partitionKeys.TryAdd(et, 0);
            });
            return partitionKeys.Select(m => m.Key);
        }
    }
}
