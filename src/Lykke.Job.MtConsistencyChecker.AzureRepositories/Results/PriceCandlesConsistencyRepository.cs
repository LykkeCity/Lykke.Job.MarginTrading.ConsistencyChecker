using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Job.MtConsistencyChecker.AzureRepositories.Entities;
using Lykke.Job.MtConsistencyChecker.Contract;
using Lykke.Job.MtConsistencyChecker.Core.Repositories;
using Lykke.SettingsReader;

namespace Lykke.Job.MtConsistencyChecker.AzureRepositories.Results
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
