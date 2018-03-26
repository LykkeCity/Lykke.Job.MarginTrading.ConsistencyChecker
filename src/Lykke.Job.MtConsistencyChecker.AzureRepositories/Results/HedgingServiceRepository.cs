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
    public class HedgingServiceRepository : IHedgingServiceRepository
    {
        private readonly INoSQLTableStorage<HedgingServiceCheckResultEntity> _tableStorage;

        public HedgingServiceRepository(IReloadingManager<string> connectionString, ILog log)
        {
            _tableStorage = AzureTableStorage<HedgingServiceCheckResultEntity>.Create(connectionString,
                "CheckHedgingService", log);
        }

        public async Task AddAsync(IEnumerable<IHedgingServiceCheckResult> entities, DateTime checkDate)
        {
            await _tableStorage.InsertOrMergeBatchAsync(HedgingServiceCheckResultEntity.CreateBatch(entities, checkDate));
        }
    }
}
