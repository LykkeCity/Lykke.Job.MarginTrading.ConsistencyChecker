using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Job.MtConsistencyChecker.AzureRepositories.Entities;
using Lykke.Job.MtConsistencyChecker.Contract;
using Lykke.Job.MtConsistencyChecker.Core.Repositories;
using Lykke.SettingsReader;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;

namespace Lykke.Job.MtConsistencyChecker.AzureRepositories
{
    public class AccountMarginEventReportRepository : IAccountMarginEventReportRepository
    {
        private readonly INoSQLTableStorage<AccountMarginEventReportEntity> _tableStorage;

        public AccountMarginEventReportRepository(IReloadingManager<string> connectionString, ILog log)
        {
            _tableStorage = AzureTableStorage<AccountMarginEventReportEntity>.Create(connectionString,
                "AccountMarginEventsReports", log);
        }

        public async Task<IEnumerable<IAccountMarginEventReport>> GetAsync(DateTime? dtFrom, DateTime? dtTo)
        {
            var partitionKeys = await GetPartitionKeys();
            return (await _tableStorage.WhereAsync(partitionKeys, dtFrom ?? DateTime.MinValue, dtTo ?? DateTime.MaxValue, ToIntervalOption.IncludeTo))
                .OrderByDescending(item => item.EventTime);
        }

        private async Task<IEnumerable<string>> GetPartitionKeys()
        {
            var partitionKeys = new ConcurrentBag<string>();
            await _tableStorage.ExecuteAsync(new TableQuery<AccountMarginEventReportEntity>(), entity =>
                entity.Select(m => m.PartitionKey).ForEach(pk => partitionKeys.Add(pk)));
            return partitionKeys.Distinct();
        }
    }
}
