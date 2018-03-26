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
    public class AccountsStatReportRepository : IAccountsStatReportRepository
    {
        private const string TableName = "ClientAccountsStatusReports";

        private readonly string _connectionString;

        private static string GetColumns =>
            string.Join(",", typeof(IAccountsStatReport).GetProperties().Select(x => x.Name));

        public AccountsStatReportRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<IAccountsStatReport>> GetAsync(DateTime? dtFrom, DateTime? dtTo)
        {
            var query = "SELECT " + GetColumns +
                        $" FROM {TableName}" +
                        " WHERE (Date >= @from AND Date <= @to)";

            using (var conn = new SqlConnection(_connectionString))
            {
                return await conn.QueryAsync<AccountsStatReportEntity>(query, new
                {
                    from = dtFrom ?? new DateTime(2000, 01, 01),
                    to = dtTo ?? DateTime.MaxValue
                });
            }
        }
    }
}
