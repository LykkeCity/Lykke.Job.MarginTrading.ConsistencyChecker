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
