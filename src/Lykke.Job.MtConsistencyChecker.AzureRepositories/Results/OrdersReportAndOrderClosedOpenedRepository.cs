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
    public class OrdersReportAndOrderClosedOpenedRepository : IOrdersReportAndOrderClosedOpenedRepository
    {
        private readonly INoSQLTableStorage<OrdersReportAndOrderClosedOpenedCheckResultEntity> _tableStorage;

        public OrdersReportAndOrderClosedOpenedRepository(IReloadingManager<string> connectionString, ILog log)
        {
            _tableStorage = AzureTableStorage<OrdersReportAndOrderClosedOpenedCheckResultEntity>.Create(connectionString,
                "CheckOrdersReportAndOrderClosedOpened", log);
        }

        public async Task AddAsync(IEnumerable<IOrdersReportAndOrderClosedOpenedCheckResult> entities, DateTime checkDate)
        {
            await _tableStorage.InsertOrReplaceAsync(OrdersReportAndOrderClosedOpenedCheckResultEntity.CreateBatch(entities, checkDate));
        }
        
    }
}
