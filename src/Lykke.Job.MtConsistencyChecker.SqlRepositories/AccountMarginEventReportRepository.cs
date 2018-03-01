using Common.Log;
using Dapper;
using Lykke.Job.MtConsistencyChecker.Contract;
using Lykke.Job.MtConsistencyChecker.Contract.Models;
using Lykke.Job.MtConsistencyChecker.Core.Repositories;
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
        private readonly ILog _log;

        private string GetColumns =>
            string.Join(",", typeof(IAccountMarginEventReport).GetProperties().Select(x => x.Name));

        public AccountMarginEventReportRepository(string connectionString, ILog log)
        {
            _log = log;
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<IAccountMarginEventReport>> GetAsync(DateTime? dtFrom, DateTime? dtTo)
        {   
            var query = $"SELECT " + GetColumns +
                    $" FROM {TableName}" +
                    $" WHERE (EventTime >= @from AND EventTime <= @to)";

            using (var conn = new SqlConnection(_connectionString))
            {
                return await conn.QueryAsync<AccountMarginEventReport>(query, new { from = dtFrom ?? new DateTime(2000, 01, 01), to = dtTo ?? DateTime.MaxValue });
            }
        }
    }
}
