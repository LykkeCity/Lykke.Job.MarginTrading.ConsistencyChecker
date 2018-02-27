using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Job.MarginTrading.ConsistencyChecker.AzureRepositories.Entities;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Repositories;
using Lykke.SettingsReader;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.AzureRepositories
{
    public class PriceCandlesConsistencyRepository : IPriceCandlesConsistencyRepository
    {
        private readonly INoSQLTableStorage<PriceCandlesConsistencyResultEntity> _tableStorage;

        public PriceCandlesConsistencyRepository(IReloadingManager<string> connectionString, ILog log)
        {
            _tableStorage = AzureTableStorage<PriceCandlesConsistencyResultEntity>.Create(connectionString,
                "CheckPriceCandlesConsistency", log);
        }
                
        public async Task AddAsync(IEnumerable<IPriceCandlesConsistencyResult> entities, DateTime checkDate)
        {
            await _tableStorage.InsertOrMergeBatchAsync(PriceCandlesConsistencyResultEntity.CreateBatch(entities, checkDate));
        }
    }
}
