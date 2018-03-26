using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Job.MtConsistencyChecker.AzureRepositories.Entities;
using Lykke.Job.MtConsistencyChecker.Contract;
using Lykke.Job.MtConsistencyChecker.Core.Repositories;
using Lykke.SettingsReader;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Job.MtConsistencyChecker.AzureRepositories.Results
{
    public class CheckResultRepository : ICheckResultRepository
    {
        private readonly INoSQLTableStorage<CheckResultEntity> _tableStorage;

        public CheckResultRepository(IReloadingManager<string> connectionString, ILog log)
        {
            _tableStorage = AzureTableStorage<CheckResultEntity>.Create(connectionString,
                "CheckResult", log);
        }

        public async Task AddAsync(ICheckResult entity)
        {
            await _tableStorage.InsertAsync(CheckResultEntity.Create(entity));
        }

        public async Task<ICheckResult> GetLastAsync()
        {
            var lastkey = (await GetPartitionKeys())
                .OrderBy(x => x)
                .LastOrDefault();
            return (await _tableStorage.GetDataAsync(lastkey)).FirstOrDefault();
        }

        private async Task<IEnumerable<string>> GetPartitionKeys()
        {
            System.Collections.Concurrent.ConcurrentDictionary<string, byte> partitionKeys = new System.Collections.Concurrent.ConcurrentDictionary<string, byte>();
            await _tableStorage.ExecuteAsync(new TableQuery<CheckResultEntity>(), entity =>
            {
                foreach (var et in entity.Select(m => m.PartitionKey))
                    partitionKeys.TryAdd(et, 0);
            });
            return partitionKeys.Select(m => m.Key);
        }
    }
}
