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
    public class TradingOrderRepository : ITradingOrderRepository
    {
        private const string TableName = "TradingOrderReport";

        private readonly string _connectionString;
        private readonly ILog _log;

        private string GetColumns =>
            string.Join(",", typeof(ITradingOrder).GetProperties().Select(x => x.Name));

        public TradingOrderRepository(string connectionString, ILog log)
        {
            _log = log;
            _connectionString = connectionString;
        }


        public async Task<IEnumerable<ITradingOrder>> GetAsync(DateTime? dtFrom, DateTime? dtTo)
        {
            var query = $"SELECT " + GetColumns +                 
                 $" FROM {TableName}" +
                 $" WHERE (Date >= @from AND Date <= @to)";

            using (var conn = new SqlConnection(_connectionString))
            {
                return (await conn.QueryAsync<TradingOrder>(query, new { from = dtFrom ?? new DateTime(2000, 01, 01), to = dtTo ?? DateTime.MaxValue }))
                    .OrderByDescending(x => x.Date);
            }
        }
    }
}
