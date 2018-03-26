using System.Collections.Generic;
using System.Linq;
using Lykke.Job.MtConsistencyChecker.Contract;
using Lykke.Job.MtConsistencyChecker.Contract.Results;

namespace Lykke.Job.MtConsistencyChecker.Services.Extensions
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
            var accountTransactionsReports = accountTransaction.ToList();
            foreach (var marginEvent in marginEvents.OrderBy(e => e.EventTime))
            {
                var transactionsUntilEvent = accountTransactionsReports
                    .Where(t => t.AccountId == marginEvent.AccountId && t.Date <= marginEvent.EventTime)
                    .OrderBy(t => t.Date);
                var balance = transactionsUntilEvent.Sum(x => x.Amount);
                if (marginEvent.Balance != balance)
                    result.Add(new MarginEventsAccountStatusCheckResult
                    {
                        MarginEvent = marginEvent,
                        Error = $"[balance delta]={marginEvent.Balance - balance}, [marginEvent.Balance]={marginEvent.Balance}, [accountTransaction.Sum]={balance}"
                    });
            }
            return result;
        }

        public static IEnumerable<MarginEventsAccountStatusCheckResult> CheckOpenPositions(this IEnumerable<IAccountMarginEventReport> marginEvents, IEnumerable<ITradingPosition> tradingPositions)
        {
            var result = new List<MarginEventsAccountStatusCheckResult>();
            var positions = tradingPositions.ToList();
            foreach (var marginEvent in marginEvents.OrderBy(e => e.EventTime))
            {
                var accountPositions = positions.Where(t => t.TakerAccountId == marginEvent.AccountId)
                    .OrderBy(t => t.Date)
                    .ToList();
                if (accountPositions.Any())
                {
                    result.Add(new MarginEventsAccountStatusCheckResult
                    {
                        MarginEvent = marginEvent,
                        Error = $"No TradingPositions for Margin Event Account Id=[{marginEvent.AccountId}]"
                    });
                }
                else
                {
                    var accountOpenPositionsUntilEvent = accountPositions.Where(t => t.CloseDate == null || t.CloseDate > marginEvent.EventTime);
                    var openPositions = accountOpenPositionsUntilEvent.Count();

                    if (marginEvent.OpenPositionsCount != openPositions)
                        result.Add(new MarginEventsAccountStatusCheckResult
                        {
                            MarginEvent = marginEvent,
                            Error = $"[open positions delta]={marginEvent.OpenPositionsCount - openPositions}, [marginEvent.OpenPositionsCount]={marginEvent.OpenPositionsCount}, [accountTransaction.openPositions]={openPositions}"
                        });
                }
            }
            return result;
        }

    }
}
