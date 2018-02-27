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
