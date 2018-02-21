using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Results;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Services.Extensions
{
    public static class MarginEventsAccountStatusExtensions
    {
        /// <summary>
        /// For each record in MarginEvents check that balance equals the account balance calculated for the corresponding date base on transactions 
        /// </summary>
        /// <param name="marginEvents"></param>
        /// <param name="accountTransaction"></param>
        /// <returns></returns>
        public static IEnumerable<MarginEventsAccountStatusCheckResult> CheckBalanceTransactions(this IEnumerable<IAccountMarginEventReport> marginEvents, IEnumerable<IAccountTransactionsReport> accountTransaction)
        {
            var result = new List<MarginEventsAccountStatusCheckResult>();

            // There can be several events for the same AccountId:
            var accountIds = marginEvents.Select(e => e.AccountId)
                .Distinct();

            a
            foreach (var accountId in accountIds)
            {
                var accountMarginEvents = marginEvents.Where(e => e.AccountId == accountId)
                    .OrderBy(x => x.EventTime);

                var total = accountTransaction
                    .Where(t => t.AccountId == accountId)
                    .Sum(x => x.Amount);

                var lastEntry = accountMarginEvents.Last();
                if (lastEntry.Balance != total)
                    result.Add(new MarginEventsAccountStatusCheckResult
                    {
                        MarginEvent = lastEntry,
                        Error = $"[delta]={lastEntry.Balance - total}, [marginEvent.Balance]={lastEntry.Balance}, [accountTransaction.Sum]={total}"
                    });

            }

            return result;
        }

        public static IEnumerable<MarginEventsAccountStatusCheckResult> CheckOpenPositions(this IEnumerable<IAccountMarginEventReport> marginEvents, IEnumerable<ITradingPosition> tradingPositions)
        {
            return new List<MarginEventsAccountStatusCheckResult>();
        }

    }
}
