using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Results;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Services
{
    internal static class BalanceAndTransactionAmountExtensions
    {
        /// <summary>
        /// Balance should not be negative
        /// </summary>
        /// <param name="accountStats"></param>
        /// <returns></returns>
        internal static IEnumerable<BalanceAndTransactionAmountCheckResult> CheckNegativeBalance(this IEnumerable<IAccountsStatReport> accountStats)
        {
            return accountStats.Where(x => x.Balance < 0)
                .Select(a => new BalanceAndTransactionAmountCheckResult
                {
                    AccountsStatReport = a,
                    Error = $"Negative balance: {a.Balance}"
                });
        }

        /// <summary>
        /// AccountStatusReport.Balance should be equal to total of MarginAccountTransactionReport.Amount
        /// </summary>
        /// <param name="accountStats"></param>
        /// <param name="accountTransaction"></param>
        /// <returns></returns>
        internal static IEnumerable<BalanceAndTransactionAmountCheckResult> CheckBalanceTransactions(this IEnumerable<IAccountsStatReport> accountStats, IEnumerable<IAccountTransactionsReport> accountTransaction)
        {
            var result = new List<BalanceAndTransactionAmountCheckResult>();
            foreach (var stat in accountStats)
            {
                var total = accountTransaction.Where(t => t.AccountId == stat.AccountId)
                    .Sum(x => x.Amount);
                if (stat.Balance != total)
                {
                    result.Add(new BalanceAndTransactionAmountCheckResult
                    {
                        AccountsStatReport = stat,
                        Error = $"[delta]={stat.Balance - total}, [accountStats]={stat.Balance}, [accountTransaction.Sum]={total}"
                    });
                }
            }
            return result;
        }
    }
}
