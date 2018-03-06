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
    public class BalanceAndOrderClosedRepository : IBalanceAndOrderClosedRepository
    {
        private readonly INoSQLTableStorage<BalanceAndOrderClosedCheckResultEntity> _tableStorage;

        public BalanceAndOrderClosedRepository(IReloadingManager<string> connectionString, ILog log)
        {
            _tableStorage = AzureTableStorage<BalanceAndOrderClosedCheckResultEntity>.Create(connectionString,
                "CheckBalanceAndOrderClosed", log);
        }

        public async Task AddAsync(IEnumerable<IBalanceAndOrderClosedCheckResult> entities, DateTime checkDate)
        {
            await _tableStorage.InsertOrMergeBatchAsync(BalanceAndOrderClosedCheckResultEntity.CreateBatch(entities, checkDate));
        }
    }
}
