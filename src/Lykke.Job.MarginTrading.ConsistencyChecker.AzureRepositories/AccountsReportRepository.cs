using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Job.MarginTrading.ConsistencyChecker.AzureRepositories.Entities;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Repositories;
using Lykke.SettingsReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.AzureRepositories
{
    public class AccountsReportRepository: IAccountsReportRepository
    {
        private readonly INoSQLTableStorage<AccountsReportEntity> _tableStorage;

        public AccountsReportRepository(IReloadingManager<string> connectionString, ILog log)
        {
            _tableStorage = AzureTableStorage<AccountsReportEntity>.Create(connectionString,
                "ClientAccountsReports", log);
        }
        
        public async Task<IEnumerable<IAccountsReport>> GetAsync(string[] accountIds, DateTime? dtFrom, DateTime? dtTo)
        {
            return (await _tableStorage.WhereAsync(accountIds, dtFrom ?? DateTime.MinValue, dtTo ?? DateTime.MaxValue, ToIntervalOption.IncludeTo))
                .OrderByDescending(item => item.Timestamp);
        }

        
    }
}
