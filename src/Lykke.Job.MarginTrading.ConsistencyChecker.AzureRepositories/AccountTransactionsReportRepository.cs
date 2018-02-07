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
    public class AccountTransactionsReportRepository : IAccountTransactionsReportRepository
    {
        private readonly INoSQLTableStorage<AccountTransactionsReportEntity> _tableStorage;

        public AccountTransactionsReportRepository(IReloadingManager<DbSettings> settings, ILog log)
        {
            _tableStorage = AzureTableStorage<AccountTransactionsReportEntity>.Create(settings.Nested(s => s.ReportsConnString),
                "MarginTradingAccountTransactionsReports", log);
        }

        public async Task<IEnumerable<IAccountTransactionsReport>> GetAsync(string[] accountIds, DateTime? dtFrom, DateTime? dtTo)
        {
            return (await _tableStorage.WhereAsync(accountIds, dtFrom ?? DateTime.MinValue, dtTo ?? DateTime.MaxValue, ToIntervalOption.IncludeTo))
                .OrderByDescending(item => item.Timestamp);
        }
    }
}
