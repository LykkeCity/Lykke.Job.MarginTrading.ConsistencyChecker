using Common.Log;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Models;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core;
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

            // Balance should not be negative
            result.AddRange(CheckNegativeBalance(accountStats));

            // AccountStatusReport.Balance should be equal to total of MarginAccountTransactionReport.Amount
            result.AddRange(CheckBalanceTransactions(accountStats, accountTransaction));
            
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

            var accountTransactions = await accountTransactionRepo.GetAsync(from, to);
            // TradePositionReportClosed records with Counterparty ID = LykkeHedgingService should be excluded from the check
            var tradingPositionsClosed = (await tradingPositionRepo.GetClosedAsync(from, to))
                .Where(m => m.TakerCounterpartyId != "LykkeHedgingService"); ;
            
            var result = new List<BalanceAndOrderClosedCheckResult>();
            
            // There should be a single record in MarginAccountTransactionReport with type "OrderClosed".
            // No double records should exist on either side (MarginAccountTransactionReport)
            result.AddRange(CheckTransactionCount(accountTransactions, tradingPositionsClosed));

            // Amount of transaction should match PnL of closed position.
            result.AddRange(CheckTransactionPnL(accountTransactions, tradingPositionsClosed));

            foreach (var position in clientClosedPositions)
            {
              

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

        private IEnumerable<BalanceAndOrderClosedCheckResult> CheckTransactionPnL(IEnumerable<IAccountTransactionsReport> accountTransactions, IEnumerable<ITradingPosition> tradingPositionsClosed)
        {
            foreach (var item in tradingPositionsClosed)
            {
                // Amount of transaction should match PnL of closed position.
                var transaction = positionClosedtransactions.FirstOrDefault();
                if (transaction.Amount != position.PnL)
                    result.Add(new BalanceAndOrderClosedCheckResult
                    {
                        TradingPosition = position,
                        AccountTransaction = transaction,
                        Error = $"delta=[{transaction.Amount - position.PnL}] transaction.Amount=[{transaction.Amount}], position.PnL=[{position.PnL}]"
                    });
            }
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
            var tradingPositions = new List<ITradingPosition>();
            tradingPositions.AddRange((await tradingPositionRepo.GetClosedAsync(from, to))
                .Where(x => x.TakerCounterpartyId != "LykkeHedgingService"));
            tradingPositions.AddRange((await tradingPositionRepo.GetOpenedAsync(from, to))
                .Where(x => x.TakerCounterpartyId != "LykkeHedgingService"));
            
            var result = new List<OrdersReportAndOrderClosedOpenedCheckResult>();
            // for each TradePosition based on TakerPositionID (Closed Positions Table)
            foreach (var tradingPosition in tradingPositions.OrderBy(x => x.Date))
            {
                // there should exist one and only one order for OpenDate and another for CloseDate (if closed)
                var positionOrders = tradingOrders
                    .Where(t => t.TakerPositionId == tradingPosition.TakerPositionId);
                if (tradingPosition.CloseDate != null && positionOrders.Count() != 2)
                {
                    result.Add(new OrdersReportAndOrderClosedOpenedCheckResult
                    {
                        OrderReport = positionOrders,
                        Position = tradingPosition,
                        Error = "Position should have 2 Order Reports (Open and Close)"
                    });
                }
                if (tradingPosition.CloseDate == null && positionOrders.Count() != 1)
                {
                    result.Add(new OrdersReportAndOrderClosedOpenedCheckResult
                    {
                        OrderReport = positionOrders,
                        Position = tradingPosition,
                        Error = "Position should have 1 Order Report (Open only)"
                    });
                }
                var openOrderReport = positionOrders.FirstOrDefault(x => x.TakerAction == "Open");
                var closeOrderReport = tradingPosition.CloseDate != null ? positionOrders.FirstOrDefault(x => x.TakerAction == "Close") : null;

                if (openOrderReport == null)
                {
                    result.Add(new OrdersReportAndOrderClosedOpenedCheckResult
                    {
                        OrderReport = positionOrders,
                        Position = tradingPosition,
                        Error = "Open Order Report doesn't exist" 
                    });
                }
                if (tradingPosition.CloseDate != null && closeOrderReport == null)
                {
                    result.Add(new OrdersReportAndOrderClosedOpenedCheckResult
                    {
                        OrderReport = positionOrders,
                        Position = tradingPosition,
                        Error = "Close Order Report doesn't exist"
                    });
                }

                // Date should exactly match
                var msg = new List<string>();

                if (openOrderReport.Date.CompareTo(tradingPosition.OpenDate) != 0)
                    msg.Add($"Open date doesn't match: openOrderReport.Date=[{openOrderReport.Date.ToDateTimeCustomString()}] tradingPosition.OpenDate=[{tradingPosition.OpenDate?.ToDateTimeCustomString()}]");
                if (closeOrderReport.Date.CompareTo(tradingPosition.CloseDate) != 0)
                    msg.Add($"Close date doesn't match: closeOrderReport.Date=[{closeOrderReport.Date.ToDateTimeCustomString()}] tradingPosition.CloseDate=[{tradingPosition.CloseDate?.ToDateTimeCustomString()}]");
                if (msg.Count > 0)
                    result.Add(new OrdersReportAndOrderClosedOpenedCheckResult
                    {
                        OrderReport = positionOrders,
                        Position = tradingPosition,
                        Error = string.Join(";", msg)
                    });

                // OrderType should match the CloseReason for close orders
                // - order type?????

                // AccountID should match
                // - TradingOrderReport doesn't have Account Id

                // ClientID should match
                if (openOrderReport.TakerCounterpartyId != tradingPosition.TakerCounterpartyId)
                    result.Add(new OrdersReportAndOrderClosedOpenedCheckResult
                    {
                        OrderReport = positionOrders,
                        Position = tradingPosition,
                        Error = $"openOrderReport.TakerCounterpartyId=[{openOrderReport.TakerCounterpartyId}] tradingPosition.TakerCounterpartyId=[{tradingPosition.TakerCounterpartyId}]"
                    });
                if (closeOrderReport != null && closeOrderReport.TakerCounterpartyId != tradingPosition.TakerCounterpartyId)
                    result.Add(new OrdersReportAndOrderClosedOpenedCheckResult
                    {
                        OrderReport = positionOrders,
                        Position = tradingPosition,
                        Error = $"closeOrderReport.TakerCounterpartyId=[{closeOrderReport.TakerCounterpartyId}] tradingPosition.TakerCounterpartyId=[{tradingPosition.TakerCounterpartyId}]"
                    });
            }

            _log.WriteInfo("CheckBalanceAndOrderClosed", null, $"Check finished with {result.Count} errors");
            return result;
        }



        private IEnumerable<BalanceAndTransactionAmountCheckResult> CheckNegativeBalance(IEnumerable<IAccountsStatReport> accountStats)
        {
            return accountStats.Where(x => x.Balance < 0)
                .Select(a => new BalanceAndTransactionAmountCheckResult
                {
                    AccountsStatReport = a,
                    Error = $"Negative balance: {a.Balance}"
                });
        }
        private IEnumerable<BalanceAndTransactionAmountCheckResult> CheckBalanceTransactions(IEnumerable<IAccountsStatReport> accountStats, IEnumerable<IAccountTransactionsReport> accountTransaction)
        {
            var result = new List<BalanceAndTransactionAmountCheckResult>();
            foreach (var item in accountStats)
            {
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
            return result;
        }

        private IEnumerable<BalanceAndOrderClosedCheckResult> CheckTransactionCount(IEnumerable<IAccountTransactionsReport> accountTransactions, IEnumerable<ITradingPosition> tradingPositionsClosed)
        {
            var result = new List<BalanceAndOrderClosedCheckResult>();
            foreach (var position in tradingPositionsClosed)
            {
                var positionClosedtransactions = accountTransactions
                   .Where(t => t.PositionId == position.TakerPositionId && t.Type == "OrderClosed");

                var numberOfTransactions = positionClosedtransactions.Count();
                if (numberOfTransactions != 1)
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
    }
}
