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
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILog _log;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="repositoryManager">IRepositoryManager object</param>
        /// <param name="log">ILog object</param>
        public ConsistencyService(IRepositoryManager repositoryManager, ILog log)
        {
            _repositoryManager = repositoryManager;
            _log = log;
        }

        /// <summary>
        /// Balance and transaction amount consistency
        /// </summary>
        /// <param name="isSql">Use SQL database or Azure database</param>
        /// <param name="from">Start Date</param>
        /// <param name="to">End Date</param>
        /// <returns></returns>
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
                // Balance should not be negative
                if (item.Balance < 0)
                {
                    result.Add(new BalanceAndTransactionAmountCheckResult
                    {
                        AccountsStatReport = item,
                        Error = $"Negative balance: {item.Balance}"
                    });
                }
                // AccountStatusReport.Balance should be equal to total of MarginAccountTransactionReport.Amount
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

        /// <summary>
        /// Balance transaction and OrderClosed consistency
        /// </summary>
        /// <param name="isSql">Use SQL database or Azure database</param>
        /// <param name="from">Start Date</param>
        /// <param name="to">End Date</param>
        /// <returns></returns>
        public async Task<IEnumerable<IBalanceAndOrderClosedCheckResult>> CheckBalanceAndOrderClosed(bool isSql, DateTime? from, DateTime? to)
        {
            _log.WriteInfo("CheckBalanceAndOrderClosed", null, "Started Check");
            
            var accountTransactionRepo = _repositoryManager.GetAccountTransactionsReport(isSql);
            var tradingPositionRepo = _repositoryManager.GetTradingPosition(isSql);

            var accountTransaction = await accountTransactionRepo.GetAsync(from, to);
            var tradingPositionsClosed = await tradingPositionRepo.GetClosedAsync(from, to);

            // TradePositionReportClosed records with Counterparty ID = LykkeHedgingService should be excluded from the check
            var clientClosedPositions = tradingPositionsClosed.Where(m => m.TakerCounterpartyId != "LykkeHedgingService");

            var result = new List<BalanceAndOrderClosedCheckResult>();
            foreach (var position in clientClosedPositions)
            {
                var positionClosedtransactions = accountTransaction
                    .Where(t => t.PositionId == position.TakerPositionId && t.Type == "OrderClosed");

                // There should be a single record in MarginAccountTransactionReport with type "OrderClosed".
                // No double records should exist on either side (MarginAccountTransactionReport)
                var numberOfTransactions = positionClosedtransactions.Count();                
                if (numberOfTransactions != 1)
                {
                    result.Add(new BalanceAndOrderClosedCheckResult
                    {
                        TradingPosition = position,
                        Error = $"MarginAccountTransactionReport has {numberOfTransactions} records with TYPE=[OrderClosed] for trading position."
                    });
                    continue;
                }

                // Amount of transaction should match PnL of closed position.
                var transaction = positionClosedtransactions.FirstOrDefault();
                if (transaction.Amount != position.PnL)
                    result.Add(new BalanceAndOrderClosedCheckResult
                    {
                        TradingPosition = position,
                        AccountTransaction = transaction,
                        Error = $"delta=[{transaction.Amount - position.PnL}] transaction.Amount=[{transaction.Amount}], position.PnL=[{position.PnL}]"
                    });

                // ClientID should match
                if (transaction.ClientId != position.TakerCounterpartyId)
                    result.Add(new BalanceAndOrderClosedCheckResult
                    {
                        TradingPosition = position,
                        AccountTransaction = transaction,
                        Error = $"ClientID doesn't match  transaction.ClientId=[{transaction.ClientId}], position.TakerCounterpartyId=[{position.TakerCounterpartyId}]"
                    });

                // AccountID should match
                if (transaction.AccountId != position.TakerAccountId)
                    result.Add(new BalanceAndOrderClosedCheckResult
                    {
                        TradingPosition = position,
                        AccountTransaction = transaction,
                        Error = $"AccountID doesn't match  transaction.AccountId=[{transaction.AccountId}], position.TakerAccountId=[{position.TakerAccountId}]"
                    });

                // DateTime should be similar (with difference no more than 1 minute
                var dateDelta = transaction.Date - position.Date;
                if (dateDelta.TotalMinutes < -1 || dateDelta.TotalMinutes > 1)
                    result.Add(new BalanceAndOrderClosedCheckResult
                    {
                        TradingPosition = position,
                        AccountTransaction = transaction,
                        Error = $"Date Delta = [{dateDelta}] transaction.Date =[{transaction.Date}], position.Date=[{position.Date}]"
                    });
            }

            // No orphaned MarginAccountTransactionReport records should exist
            // No double records should exist on either side (TradingPositionClosed)
            var closedTransactions = accountTransaction.Where(t => t.Type == "OrderClosed");
            foreach (var transaction in closedTransactions)
            {
                var positions = tradingPositionsClosed.Where(p => p.TakerPositionId == transaction.PositionId);
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
                        Error = $"TradingPositionClosed has {numberOfpositions} records for account transaction."
                    });
            }


            _log.WriteInfo("CheckBalanceAndOrderClosed", null, $"Check finished with {result.Count} errors");
            return result;
        }

        /// <summary>
        /// OrdersReport should be consistent with TradePositionReportClosed & TradePositionReportOpened tables
        /// </summary>
        /// <param name="isSql">Use SQL database or Azure database</param>
        /// <param name="from">Start Date</param>
        /// <param name="to">End Date</param>
        /// <returns></returns>
        public async Task<IEnumerable<IOrdersReportAndOrderClosedOpenedCheckResult>> CheckOrdersReportAndOrderClosedOpened(bool isSql, DateTime? from, DateTime? to)
        {
            _log.WriteInfo("CheckOrdersReportAndOrderClosedOpened", null, "Started Check");

            var tradingOrdersReportRepo = _repositoryManager.GetTradingOrder(isSql);
            var tradingPositionRepo = _repositoryManager.GetTradingPosition(isSql);

            // Ignore LykkeHedgingService
            var tradingOrders = (await tradingOrdersReportRepo.GetAsync(from, to))
                .Where(x => x.TakerCounterpartyId != "LykkeHedgingService");                
            var tradingPositionsClosed = (await tradingPositionRepo.GetClosedAsync(from, to))
                .Where(x => x.TakerCounterpartyId != "LykkeHedgingService");
            var tradingPositionsOpened = (await tradingPositionRepo.GetOpenedAsync(from, to))
                .Where(x => x.TakerCounterpartyId != "LykkeHedgingService");

            var result = new List<OrdersReportAndOrderClosedOpenedCheckResult>();
            // for each TradePosition based on TakerPositionID (Closed Positions Table)
            foreach (var tradingPosition in tradingPositionsClosed)
            {
                // there should exist one and only one order for OpenDate and another for CloseDate (if closed)
                var positionOrders = tradingOrders
                    .Where(t => t.TakerPositionId == tradingPosition.TakerPositionId);
                if (positionOrders.Count() != 2)
                {
                    result.Add(new OrdersReportAndOrderClosedOpenedCheckResult
                    {
                        OrderReport = positionOrders,
                        Position = tradingPosition,
                        Error = "Position should have 2 Order Reports (Open and Close)"
                    });
                }
                else
                {
                    // Date should exactly match
                    var open = 
                }

            }

            _log.WriteInfo("CheckBalanceAndOrderClosed", null, $"Check finished with {result.Count} errors");
            return result;
        }
    }
}
