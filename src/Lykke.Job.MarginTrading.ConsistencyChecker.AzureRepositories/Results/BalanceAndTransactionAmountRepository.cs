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
