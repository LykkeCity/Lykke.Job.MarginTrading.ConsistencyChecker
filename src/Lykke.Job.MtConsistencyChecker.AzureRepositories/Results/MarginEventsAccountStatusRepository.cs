using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Job.MtConsistencyChecker.AzureRepositories.Entities;
using Lykke.Job.MtConsistencyChecker.Contract;
using Lykke.Job.MtConsistencyChecker.Core.Repositories;
using Lykke.SettingsReader;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.MtConsistencyChecker.AzureRepositories
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
