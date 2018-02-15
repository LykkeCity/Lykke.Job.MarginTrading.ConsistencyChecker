using Common.Log;
using Dapper;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Models;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.SqlRepositories
{
    public class AccountTransactionsReportRepository : IAccountTransactionsReportRepository
    {
        private const string TableName = "MarginTradingAccountTransactionsReports";

        private readonly string _connectionString;
        private readonly ILog _log;

        public AccountTransactionsReportRepository(string connectionString, ILog log)
        {
            _log = log;
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<IAccountTransactionsReport>> GetAsync(DateTime? dtFrom, DateTime? dtTo)
        {            
            var query = $"SELECT" +
                    " Id, Date, AccountId, ClientId, Amount, Balance, WithdrawTransferLimit, Comment, Type, PositionId" +
                    $" FROM {TableName}" +
                    $" WHERE (Date >= @from AND Date <= @to)";

            using (var conn = new SqlConnection(_connectionString))
            {
                return (await conn.QueryAsync<AccountTransactionsReport>(query, new { from = dtFrom ?? new DateTime(2000, 01, 01), to = dtTo ?? DateTime.MaxValue }))
                    .OrderByDescending(x => x.Date);
            }
        }
    }
}
