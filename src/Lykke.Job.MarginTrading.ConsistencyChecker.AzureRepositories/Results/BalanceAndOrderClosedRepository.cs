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
