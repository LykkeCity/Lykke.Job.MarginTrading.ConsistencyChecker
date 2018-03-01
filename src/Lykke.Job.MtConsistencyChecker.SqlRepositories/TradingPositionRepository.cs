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
    public class TradingPositionRepository : ITradingPositionRepository
    {
        private const string OpenedTableName = "TradingPositionOpened";
        private const string ClosedTableName = "TradingPositionClosed";

        private readonly string _connectionString;
        private readonly ILog _log;

        private string GetColumns =>
            string.Join(",", typeof(ITradingPosition).GetProperties().Select(x => x.Name));

        public TradingPositionRepository(string connectionString, ILog log)
        {
            _log = log;
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<ITradingPosition>> GetClosedAsync(DateTime? dtFrom, DateTime? dtTo)
        {
            return await GetDataAsync(false, dtFrom, dtTo);
        }

        public async Task<IEnumerable<ITradingPosition>> GetOpenedAsync(DateTime? dtFrom, DateTime? dtTo)
        {
            return await GetDataAsync(true, dtFrom, dtTo);
        }

        private async Task<IEnumerable<ITradingPosition>> GetDataAsync(bool openedTable, DateTime? dtFrom, DateTime? dtTo)
        {
            string table = openedTable ? OpenedTableName : ClosedTableName;
            var query = $"SELECT " + GetColumns +
                       $" FROM {table}" +
                       $" WHERE (Date >= @from AND Date <= @to)";

            using (var conn = new SqlConnection(_connectionString))
            {
                return (await conn.QueryAsync<TradingPosition>(query, new { from = dtFrom ?? new DateTime(2000, 01, 01), to = dtTo ?? DateTime.MaxValue }))
                    .OrderByDescending(x => x.Date);
            }
        }
    }
}
