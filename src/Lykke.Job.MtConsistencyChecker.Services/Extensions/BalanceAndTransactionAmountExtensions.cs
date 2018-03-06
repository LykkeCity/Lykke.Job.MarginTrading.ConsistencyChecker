using System.Collections.Generic;
using System.Linq;
using Lykke.Job.MtConsistencyChecker.Contract;
using Lykke.Job.MtConsistencyChecker.Contract.Results;

namespace Lykke.Job.MtConsistencyChecker.Services.Extensions
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
            var accountTransactionsReports = accountTransaction.ToList();
            foreach (var stat in accountStats)
            {
                var total = accountTransactionsReports.Where(t => t.AccountId == stat.AccountId)
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
