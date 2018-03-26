using Dapper;
using Lykke.Job.MtConsistencyChecker.Contract;
using Lykke.Job.MtConsistencyChecker.Core.Repositories;
using Lykke.Job.MtConsistencyChecker.SqlRepositories.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Job.MtConsistencyChecker.SqlRepositories
{
    public class AccountTransactionsReportRepository : IAccountTransactionsReportRepository
    {
        private const string TableName = "MarginTradingAccountTransactionsReports";

        private readonly string _connectionString;

        private static string GetColumns =>
            string.Join(",", typeof(IAccountTransactionsReport).GetProperties().Select(x => x.Name));

        public AccountTransactionsReportRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<IAccountTransactionsReport>> GetAsync(DateTime? dtFrom, DateTime? dtTo)
        {
            var query = "SELECT " + GetColumns +
                        $" FROM {TableName}" +
                        " WHERE (Date >= @from AND Date <= @to)";

            using (var conn = new SqlConnection(_connectionString))
            {
                return (await conn.QueryAsync<AccountTransactionsReportEntity>(query, new
                    {
                        from = dtFrom ?? new DateTime(2000, 01, 01),
                        to = dtTo ?? DateTime.MaxValue
                    }))
                    .OrderByDescending(x => x.Date);
            }
        }
    }
}
