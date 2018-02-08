using Common.Log;
using Dapper;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Models;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.SqlRepositories
{
    public class AccountsReportRepository : IAccountsReportRepository
    {
        private const string TableName = "ClientAccountsReports";

        private readonly string _connectionString;
        private readonly ILog _log;

        public AccountsReportRepository(string connectionString, ILog log)
        {
            _log = log;
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<IAccountsReport>> GetAsync(string[] accountIds, DateTime? dtFrom, DateTime? dtTo)
        {
            var from = dtFrom.HasValue ? dtFrom?.ToString("u") : DateTime.MinValue.ToString("u");
            var to = dtTo.HasValue ? dtTo?.ToString("u") : DateTime.MaxValue.ToString("u");
            var query = $"SELECT" +
                    " Id, Date, TakerCounterpartyId, TakerAccountId, BaseAssetId, IsLive" +
                    $" FROM {TableName}" +
                    $" WHERE Id in({string.Join(",", accountIds)}) " +
                    $" AND (Date >= '{from}' AND Date <='{to}')";

            using (var conn = new SqlConnection(_connectionString))
            {
                try { return await conn.QueryAsync<AccountsReport>(query); }
                catch (Exception ex)
                {
                    await _log?.WriteErrorAsync("AccountsReportRepository", "GetAsync", ex);
                    throw;
                }
            }
        }
    }
}
