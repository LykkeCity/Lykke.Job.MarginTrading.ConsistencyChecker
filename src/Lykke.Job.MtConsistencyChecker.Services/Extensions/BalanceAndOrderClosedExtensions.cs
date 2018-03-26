using System.Collections.Generic;
using System.Linq;
using Lykke.Job.MtConsistencyChecker.Contract;
using Lykke.Job.MtConsistencyChecker.Contract.Results;

namespace Lykke.Job.MtConsistencyChecker.Services.Extensions
{
    internal static class BalanceAndOrderClosedExtensions
    {
        /// <summary>
        /// There should be a single record in MarginAccountTransactionReport with type "OrderClosed".
        /// No double records should exist on either side (MarginAccountTransactionReport)
        /// </summary>
        /// <param name="tradingPositionsClosed"></param>
        /// <param name="accountTransactions"></param>
        /// <returns></returns>
        internal static IEnumerable<BalanceAndOrderClosedCheckResult> CheckTransactionCount(this IEnumerable<ITradingPosition> tradingPositionsClosed, IEnumerable<IAccountTransactionsReport> accountTransactions)
        {
            var result = new List<BalanceAndOrderClosedCheckResult>();
            var accountTransactionsReports = accountTransactions.ToList();
            foreach (var position in tradingPositionsClosed)
            {
                var closedAccountTransactions = accountTransactionsReports
                   .Where(t => t.PositionId == position.TakerPositionId && t.Type == "OrderClosed")
                    .ToList();

                var numberOfTransactions = closedAccountTransactions.Count;
                if (numberOfTransactions < 1)
                {
                    result.Add(new BalanceAndOrderClosedCheckResult
                    {
                        TradingPosition = position,
                        Error = "Orphaned TradingPositionClosed"
                    });
                }
                else if (numberOfTransactions > 1)
                {
                    result.Add(new BalanceAndOrderClosedCheckResult
                    {
                        TradingPosition = position,
                        Error = $"MarginAccountTransactionReport has {numberOfTransactions} records with TYPE=[OrderClosed] for trading position."
                    });
                }
            }
            return result;
        }

        /// <summary>
        /// Amount of transaction should match PnL of closed position.
        /// </summary>
        /// <param name="tradingPositionsClosed"></param>
        /// <param name="accountTransactions"></param>
        /// <returns></returns>
        internal static IEnumerable<BalanceAndOrderClosedCheckResult> CheckTransactionPnL(this IEnumerable<ITradingPosition> tradingPositionsClosed, IEnumerable<IAccountTransactionsReport> accountTransactions)
        {
            var result = new List<BalanceAndOrderClosedCheckResult>();
            var accountTransactionsReports = accountTransactions.ToList();
            foreach (var position in tradingPositionsClosed)
            {
                var transaction = accountTransactionsReports
                    .FirstOrDefault(t => t.PositionId == position.TakerPositionId && t.Type == "OrderClosed");
                if (transaction?.Amount != position.PnL)
                    result.Add(new BalanceAndOrderClosedCheckResult
                    {
                        TradingPosition = position,
                        AccountTransaction = transaction,
                        Error = $"delta=[{transaction?.Amount - position.PnL}] transaction.Amount=[{transaction?.Amount}], position.PnL=[{position.PnL}]"
                    });
            }
            return result;
        }

        /// <summary>
        /// ClientID should match
        /// </summary>
        /// <param name="tradingPositionsClosed"></param>
        /// <param name="accountTransactions"></param>
        /// <returns></returns>
        internal static IEnumerable<BalanceAndOrderClosedCheckResult> CheckClientId(this IEnumerable<ITradingPosition> tradingPositionsClosed, IEnumerable<IAccountTransactionsReport> accountTransactions)
        {
            var result = new List<BalanceAndOrderClosedCheckResult>();
            var accountTransactionsReports = accountTransactions.ToList();
            foreach (var position in tradingPositionsClosed)
            {   
                var transaction = accountTransactionsReports
                    .FirstOrDefault(t => t.PositionId == position.TakerPositionId && t.Type == "OrderClosed");

                if (transaction?.ClientId != position.TakerCounterpartyId)
                    result.Add(new BalanceAndOrderClosedCheckResult
                    {
                        TradingPosition = position,
                        AccountTransaction = transaction,
                        Error = $"ClientID doesn't match  transaction.ClientId=[{transaction?.ClientId}], position.TakerCounterpartyId=[{position.TakerCounterpartyId}]"
                    });
            }
            return result;
        }

        /// <summary>
        /// AccountID should match
        /// </summary>
        /// <param name="tradingPositionsClosed"></param>
        /// <param name="accountTransactions"></param>
        /// <returns></returns>
        internal static IEnumerable<BalanceAndOrderClosedCheckResult> CheckAccountId(this IEnumerable<ITradingPosition> tradingPositionsClosed, IEnumerable<IAccountTransactionsReport> accountTransactions)
        {
            var result = new List<BalanceAndOrderClosedCheckResult>();
            var accountTransactionsReports = accountTransactions.ToList();
            foreach (var position in tradingPositionsClosed)
            {
                var transaction = accountTransactionsReports
                    .FirstOrDefault(t => t.PositionId == position.TakerPositionId && t.Type == "OrderClosed");

                if (transaction == null)
                {
                    result.Add(new BalanceAndOrderClosedCheckResult
                    {
                        TradingPosition = position,
                        AccountTransaction = transaction,
                        Error = "Position has no close transaction."
                    });
                    continue;
                }

                if (transaction.AccountId != position.TakerAccountId)
                    result.Add(new BalanceAndOrderClosedCheckResult
                    {
                        TradingPosition = position,
                        AccountTransaction = transaction,
                        Error =
                            $"AccountID doesn't match  transaction.AccountId=[{transaction.AccountId}], position.TakerAccountId=[{position.TakerAccountId}]"
                    });
            }
            return result;
        }

        /// <summary>
        /// DateTime should be similar (with difference no more than 1 minute
        /// </summary>
        /// <param name="tradingPositionsClosed"></param>
        /// <param name="accountTransactions"></param>
        /// <returns></returns>
        internal static IEnumerable<BalanceAndOrderClosedCheckResult> CheckDate(this IEnumerable<ITradingPosition> tradingPositionsClosed, IEnumerable<IAccountTransactionsReport> accountTransactions)
        {
            var result = new List<BalanceAndOrderClosedCheckResult>();
            var accountTransactionsReports = accountTransactions.ToList();
            foreach (var position in tradingPositionsClosed)
            {
                var transaction = accountTransactionsReports
                    .FirstOrDefault(t => t.PositionId == position.TakerPositionId && t.Type == "OrderClosed");
                if (transaction == null)
                {
                    result.Add(new BalanceAndOrderClosedCheckResult
                    {
                        TradingPosition = position,
                        AccountTransaction = transaction,
                        Error = "Position has no close transaction."
                    });
                    continue;
                }

                var dateDelta = transaction.Date - position.Date;
                if (dateDelta.TotalMinutes < -1 || dateDelta.TotalMinutes > 1)
                    result.Add(new BalanceAndOrderClosedCheckResult
                    {
                        TradingPosition = position,
                        AccountTransaction = transaction,
                        Error = $"Date Delta = [{dateDelta}] transaction.Date =[{transaction.Date}], position.Date=[{position.Date}]"
                    });
            }
            return result;
        }


        /// <summary>
        /// No orphaned MarginAccountTransactionReport records should exist
        /// No double records should exist on either side (TradingPositionClosed)
        /// </summary>
        /// <param name="tradingPositionsClosed"></param>
        /// <param name="accountTransactions"></param>
        /// <returns></returns>
        internal static IEnumerable<BalanceAndOrderClosedCheckResult> CheckPositionCount(this IEnumerable<ITradingPosition> tradingPositionsClosed, IEnumerable<IAccountTransactionsReport> accountTransactions)
        {
            var result = new List<BalanceAndOrderClosedCheckResult>();
            var closedTransactions = accountTransactions.Where(t => t.Type == "OrderClosed");
            var tradingPositions = tradingPositionsClosed.ToList();
            foreach (var transaction in closedTransactions)
            {
                var positions = tradingPositions.Where(p => p.TakerPositionId == transaction.PositionId);
                var numberOfpositions = positions.Count();
                if (numberOfpositions < 1)
                    result.Add(new BalanceAndOrderClosedCheckResult
                    {
                        AccountTransaction = transaction,
                        Error = "Orphaned MarginAccountTransactionReport"
                    });
                else if (numberOfpositions > 1)
                    result.Add(new BalanceAndOrderClosedCheckResult
                    {
                        AccountTransaction = transaction,
                        Error = $"TradingPositionClosed has {numberOfpositions} records for account transaction report."
                    });
            }
            return result;
        }
                
    }
}
