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
    public class BalanceAndTransactionAmountRepository : IBalanceAndTransactionAmountRepository
    {
        private readonly INoSQLTableStorage<BalanceAndTransactionAmountCheckResultEntity> _tableStorage;

        public BalanceAndTransactionAmountRepository(IReloadingManager<string> connectionString, ILog log)
        {
            _tableStorage = AzureTableStorage<BalanceAndTransactionAmountCheckResultEntity>.Create(connectionString,
                "CheckBalanceAndTransactionAmount", log);
        }
                
        public async Task AddAsync(IEnumerable<IBalanceAndTransactionAmountCheckResult> entities, DateTime checkDate)
        {
            await _tableStorage.InsertOrMergeBatchAsync(BalanceAndTransactionAmountCheckResultEntity.CreateBatch(entities, checkDate));
        }
    }
}
