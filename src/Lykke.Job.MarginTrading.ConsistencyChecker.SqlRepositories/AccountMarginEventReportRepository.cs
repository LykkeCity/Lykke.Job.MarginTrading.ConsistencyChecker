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
    public class AccountMarginEventReportRepository : IAccountMarginEventReportRepository
    {
        private const string TableName = "AccountMarginEventsReports";
        
        private readonly string _connectionString;
        private readonly ILog _log;

        public AccountMarginEventReportRepository(string connectionString, ILog log)
        {
            _log = log;
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<IAccountMarginEventReport>> GetAsync(string[] accountIds, DateTime? dtFrom, DateTime? dtTo)
        {
            var from = dtFrom.HasValue ? dtFrom?.ToString("u") : DateTime.MinValue.ToString("u");
            var to = dtTo.HasValue ? dtTo?.ToString("u") : DateTime.MaxValue.ToString("u");
            var query = $"SELECT" +
                    " Id, EventId, ClientId, AccountId, TradingConditionId, Balance, BaseAssetId, EventTime, FreeMargin, IsEventStopout, MarginAvailable, " +
                    " MarginCall, MarginInit, MarginUsageLevel, OpenPositionsCount, PnL, StopOut, TotalCapital, UsedMargin, WithdrawTransferLimit" +
                    $" FROM {TableName}" +
                    $" WHERE Id in({string.Join(",", accountIds)}) " +
                    $" AND (EventTime >= '{from}' AND EventTime<='{to}')";

            using (var conn = new SqlConnection(_connectionString))
            {
                return await conn.QueryAsync<AccountMarginEventReport>(query);
            }
        }
    }
}
