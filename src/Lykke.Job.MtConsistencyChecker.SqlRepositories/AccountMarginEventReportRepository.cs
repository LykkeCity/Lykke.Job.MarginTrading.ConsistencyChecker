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
    public class AccountMarginEventReportRepository : IAccountMarginEventReportRepository
    {
        private const string TableName = "AccountMarginEventsReports";

        private readonly string _connectionString;

        private static string GetColumns =>
            string.Join(",", typeof(IAccountMarginEventReport).GetProperties().Select(x => x.Name));

        public AccountMarginEventReportRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<IAccountMarginEventReport>> GetAsync(DateTime? dtFrom, DateTime? dtTo)
        {
            var query = "SELECT " + GetColumns +
                        $" FROM {TableName}" +
                        " WHERE (EventTime >= @from AND EventTime <= @to)";

            using (var conn = new SqlConnection(_connectionString))
            {
                return await conn.QueryAsync<AccountMarginEventReportEntity>(query, new
                {
                    from = dtFrom ?? new DateTime(2000, 01, 01),
                    to = dtTo ?? DateTime.MaxValue
                });
            }
        }
    }
}
