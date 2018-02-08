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
    public class AccountsStatReportRepository : IAccountsStatReportRepository
    {
        private const string TableName = "ClientAccountsStatusReports";

        private readonly string _connectionString;
        private readonly ILog _log;

        public AccountsStatReportRepository(string connectionString, ILog log)
        {
            _log = log;
            _connectionString = connectionString;
        }
                
        public async Task<IEnumerable<IAccountsStatReport>> GetAsync(DateTime? dtFrom, DateTime? dtTo)
        {            
            var query = $"SELECT" +
                    " Id, Date, BaseAssetId, AccountId, ClientId, TradingConditionId, Balance, WithdrawTransferLimit, MarginCall, StopOut, TotalCapital, FreeMargin, MarginAvailable, UsedMargin, MarginInit, PnL, OpenPositionsCount, MarginUsageLevel, IsLive" +
                    $" FROM {TableName}" +
                    $" WHERE (Date >= @from AND Date <= @to)";

            using (var conn = new SqlConnection(_connectionString))
            {
                try { return await conn.QueryAsync<AccountsStatReport>(query,new { from = dtFrom ?? new DateTime(2000,01,01), to = dtTo ?? DateTime.MaxValue }); }
                catch (Exception ex)
                {                    
                    throw ex;
                }
            }
        }
    }
}
