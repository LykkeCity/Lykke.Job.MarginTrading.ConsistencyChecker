using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Job.MarginTrading.ConsistencyChecker.AzureRepositories.Entities;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Repositories;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Settings.JobSettings;
using Lykke.SettingsReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.AzureRepositories
{
    public class AccountMarginEventReportRepository : IAccountMarginEventReportRepository
    {
        private readonly INoSQLTableStorage<AccountMarginEventReportEntity> _tableStorage;

        public AccountMarginEventReportRepository(IReloadingManager<DbSettings> settings, ILog log)
        {
            _tableStorage = AzureTableStorage<AccountMarginEventReportEntity>.Create(settings.Nested(s => s.ReportsConnString),
                "AccountMarginEventsReports", log);
        }

        public async Task<IEnumerable<IAccountMarginEventReport>> GetAsync(string[] accountIds, DateTime? dtFrom, DateTime? dtTo)
        {
            return (await _tableStorage.WhereAsync(accountIds, dtFrom ?? DateTime.MinValue, dtTo ?? DateTime.MaxValue, ToIntervalOption.IncludeTo))
                .OrderByDescending(item => item.EventTime);
        }        
    }
}
