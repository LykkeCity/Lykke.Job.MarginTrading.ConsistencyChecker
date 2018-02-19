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
    public class ConsistencyService : IConsistencyService
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
            result.AddRange(accountStats.CheckNegativeBalance());

            // AccountStatusReport.Balance should be equal to total of MarginAccountTransactionReport.Amount
            result.AddRange(accountStats.CheckBalanceTransactions(accountTransaction));

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
            result.AddRange(tradingPositionsClosed.CheckTransactionCount(accountTransactions));

            // Amount of transaction should match PnL of closed position.
            result.AddRange(tradingPositionsClosed.CheckTransactionPnL(accountTransactions));

            // ClientID should match
            result.AddRange(tradingPositionsClosed.CheckClientId(accountTransactions));

            // AccountID should match
            result.AddRange(tradingPositionsClosed.CheckAccountId(accountTransactions));

            // DateTime should be similar (with difference no more than 1 minute
            result.AddRange(tradingPositionsClosed.CheckDate(accountTransactions));

            /// No orphaned MarginAccountTransactionReport records should exist
            /// No double records should exist on either side (TradingPositionClosed)
            result.AddRange(tradingPositionsClosed.CheckPositionCount(accountTransactions));


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
            var tradingPositions = new List<ITradingPosition>();
            tradingPositions.AddRange((await tradingPositionRepo.GetClosedAsync(from, to))
                .Where(x => x.TakerCounterpartyId != "LykkeHedgingService"));
            tradingPositions.AddRange((await tradingPositionRepo.GetOpenedAsync(from, to))
                .Where(x => x.TakerCounterpartyId != "LykkeHedgingService"));

            var result = new List<OrdersReportAndOrderClosedOpenedCheckResult>();

            // for each TradePosition based on TakerPositionID (Closed Positions Table)
            // there should exist one and only one order for OpenDate and another for CloseDate (if closed)
            result.AddRange(tradingOrders.CheckNumberOfOrders(tradingPositions));
            
            // Date should exactly match
            result.AddRange(tradingOrders.CheckOrdersDate(tradingPositions));

            // OrderType should match the CloseReason for close orders
            result.AddRange(tradingOrders.CheckOrderTypes(tradingPositions));

            // AccountID should match
            result.AddRange(tradingOrders.CheckOrderAccountID(tradingPositions));

            // ClientID should match            
            result.AddRange(tradingOrders.CheckOrderClientID(tradingPositions));

            _log.WriteInfo("CheckBalanceAndOrderClosed", null, $"Check finished with {result.Count} errors");
            return result;
        }

        public async Task<IEnumerable<IOrdersReportAndOrderClosedOpenedCheckResult>> CheckMarginEventsAccountStatus(bool isSql, DateTime? from, DateTime? to)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IOrdersReportAndOrderClosedOpenedCheckResult>> CheckCandlesPriceConsistency(bool isSql, DateTime? from, DateTime? to)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IOrdersReportAndOrderClosedOpenedCheckResult>> CheckTradePnLConsistency(bool isSql, DateTime? from, DateTime? to)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IBalanceAndOrderClosedCheckResult>> CheckHedgingServiceBalance(bool isSql, DateTime? from, DateTime? to)
        {
            throw new NotImplementedException();
        }
    }
}
