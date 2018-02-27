using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Job.MarginTrading.ConsistencyChecker.AzureRepositories.Entities;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Repositories;
using Lykke.SettingsReader;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.AzureRepositories
{
    public class MarginEventsAccountStatusRepository : IMarginEventsAccountStatusRepository
    {
        private readonly INoSQLTableStorage<MarginEventsAccountStatusCheckResultEntity> _tableStorage;

        public MarginEventsAccountStatusRepository(IReloadingManager<string> connectionString, ILog log)
        {
            _tableStorage = AzureTableStorage<MarginEventsAccountStatusCheckResultEntity>.Create(connectionString,
                "CheckMarginEventsAccountStatus", log);
        }
                
        public async Task AddAsync(IEnumerable<IMarginEventsAccountStatusCheckResult> entities, DateTime checkDate)
        {
            await _tableStorage.InsertOrMergeBatchAsync(MarginEventsAccountStatusCheckResultEntity.CreateBatch(entities, checkDate));
        }
    }
}
