using Common.Log;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Models;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Services
{
    public class ConsistencyService: IConsistencyService
    {
        IRepositoryManager _repositoryManager;
        ILog _log;
        public ConsistencyService(IRepositoryManager repositoryManager, ILog log)
        {
            _repositoryManager = repositoryManager;
            _log = log;
        }

        public async Task<IEnumerable<IBalanceAndTransactionAmountCheckResult>> CheckBalanceAndTransactionAmount(bool isSql, DateTime? from, DateTime? to)
        {
            _log.WriteInfo("CheckBalanceAndTransactionAmount", null, "Started Check");
            var accountStatsRepo = _repositoryManager.GetAccountsStatReport(isSql);
            var accountTransactionRepo = _repositoryManager.GetAccountTransactionsReport(isSql);

            var accountStats = await accountStatsRepo.GetAsync(from, to);
            var accountTransaction = await accountTransactionRepo.GetAsync(from, to);
            var result = new List<BalanceAndTransactionAmountCheckResult>();
            foreach (var item in accountStats)
            {
                if (item.Balance < 0)
                {
                    result.Add(new BalanceAndTransactionAmountCheckResult
                    {
                        AccountsStatReport = item,
                        Error = $"Negative balance: {item.Balance}"
                    });
                }
                var total = accountTransaction.Where(t => t.AccountId == item.AccountId)
                    .Sum(x => x.Amount);
                if (item.Balance != total)
                {
                    result.Add(new BalanceAndTransactionAmountCheckResult
                    {
                        AccountsStatReport = item,
                        Error = $"[delta]={item.Balance - total}, [accountStats]={item.Balance}, [accountTransaction.Sum]={total}"
                    });
                }
            }
            _log.WriteInfo("CheckBalanceAndTransactionAmount", null, $"Check finished with {result.Count} errors");
            return result;
        }
    }
}
