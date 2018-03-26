using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Job.MtConsistencyChecker.AzureRepositories.Entities;
using Lykke.Job.MtConsistencyChecker.Contract;
using Lykke.Job.MtConsistencyChecker.Core.Repositories;
using Lykke.SettingsReader;
using Microsoft.WindowsAzure.Storage.Table;
using MoreLinq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Job.MtConsistencyChecker.AzureRepositories
{
    public class TradingOrderRepository : ITradingOrderRepository
    {
        private readonly INoSQLTableStorage<TradingOrderEntity> _tableStorage;

        public TradingOrderRepository(IReloadingManager<string> connectionString, ILog log)
        {
            _tableStorage = AzureTableStorage<TradingOrderEntity>.Create(connectionString,
                "TradingOrderReport", log);
        }
        
        public async Task<IEnumerable<ITradingOrder>> GetAsync(DateTime? dtFrom, DateTime? dtTo)
        {
            var keys = await GetPartitionKeys();
            return (await _tableStorage.WhereAsync(keys, dtFrom ?? DateTime.MinValue, dtTo ?? DateTime.MaxValue, ToIntervalOption.IncludeTo))
                .OrderByDescending(item => item.Timestamp);
        }

        private async Task<IEnumerable<string>> GetPartitionKeys()
        {
            var partitionKeys = new ConcurrentBag<string>();
            await _tableStorage.ExecuteAsync(new TableQuery<TradingOrderEntity>(), entity =>
                entity.Select(m => m.PartitionKey).ForEach(pk => partitionKeys.Add(pk)));
            return partitionKeys;
        }

    }
}
