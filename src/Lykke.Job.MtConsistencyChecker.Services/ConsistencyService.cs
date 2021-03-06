﻿using Common.Log;
using Lykke.Job.MtConsistencyChecker.Contract;
using Lykke.Job.MtConsistencyChecker.Contract.Results;
using Lykke.Job.MtConsistencyChecker.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Job.MtConsistencyChecker.Services.Extensions;
using Lykke.Job.MtConsistencyChecker.Core.Domain;

namespace Lykke.Job.MtConsistencyChecker.Services
{
    public class ConsistencyService : IConsistencyService
    {
        private readonly IRepositoryManager _repositoryManager;        
        private readonly IPriceCandlesService _priceCandlesService;        
        private readonly ILog _log;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="repositoryManager">IRepositoryManager object</param>
        /// <param name="priceCandlesService">PriceCandles Service</param>
        /// <param name="log">ILog object</param>
        public ConsistencyService(IRepositoryManager repositoryManager,
            IPriceCandlesService priceCandlesService,
            ILog log)
        {            
            _repositoryManager = repositoryManager;
            _priceCandlesService = priceCandlesService;
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
            await _log.WriteInfoAsync("CheckBalanceAndTransactionAmount", null, $"Started Check IsSql={isSql} From [{from?.ToString("u")}] To [{to?.ToString("u")}]");
            var accountStatsRepo = _repositoryManager.GetAccountsStatReport(isSql);
            var accountTransactionRepo = _repositoryManager.GetAccountTransactionsReport(isSql);

            var accountStats = (await accountStatsRepo.GetAsync(from, to))
                .ToList();
            var accountTransaction = (await accountTransactionRepo.GetAsync(from, to))
                .ToList();

            var result = new List<BalanceAndTransactionAmountCheckResult>();

            // Balance should not be negative
            result.AddRange(accountStats.CheckNegativeBalance());

            // AccountStatusReport.Balance should be equal to total of MarginAccountTransactionReport.Amount
            result.AddRange(accountStats.CheckBalanceTransactions(accountTransaction));

            await _log.WriteInfoAsync("CheckBalanceAndTransactionAmount", null, $"Check finished with {result.Count} errors");
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
            await _log.WriteInfoAsync("CheckBalanceAndOrderClosed", null, $"Started Check IsSql={isSql} From [{from?.ToString("u")}] To [{to?.ToString("u")}]");

            var accountTransactionRepo = _repositoryManager.GetAccountTransactionsReport(isSql);
            var tradingPositionRepo = _repositoryManager.GetTradingPosition(isSql);

            var accountTransactions = (await accountTransactionRepo.GetAsync(from, to))
                .ToList();
            // TradePositionReportClosed records with Counterparty ID = LykkeHedgingService should be excluded from the check
            var tradingPositionsClosed = (await tradingPositionRepo.GetClosedAsync(from, to))
                .Where(m => m.TakerCounterpartyId != Constants.HedgingServiceName)
                .ToList();

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

            // No orphaned MarginAccountTransactionReport records should exist
            // No double records should exist on either side (TradingPositionClosed)
            result.AddRange(tradingPositionsClosed.CheckPositionCount(accountTransactions));

            await _log.WriteInfoAsync("CheckBalanceAndOrderClosed", null, $"Check finished with {result.Count} errors");
            return result;
        }

        /// <summary>
        /// TradePositionReportClosed records with Counterparty ID = LykkeHedgingService* should be checked separately: 
        /// sum of volume on each TakerAccountId-CoreSymbol must be equal to current value taken from TradingPositionOpen.
        /// need to take each value of TakerAccountId-CoreSymbol from TradingPositionOpen and compare it to sum of TakerAccountId-CoreSymbol's from TradingPositionClosed
        /// </summary>
        /// <param name="isSql"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IHedgingServiceCheckResult>> CheckHedgingService(bool isSql, DateTime? from, DateTime? to)
        {
            await _log.WriteInfoAsync("CheckHedgingService", null, $"Started Check IsSql={isSql} From [{from?.ToString("u")}] To [{to?.ToString("u")}]");
            var tradingPositionRepo = _repositoryManager.GetTradingPosition(isSql);

            var hedgingServicePositionsClosed = (await tradingPositionRepo.GetClosedAsync(from, to))
                .Where(m => m.TakerCounterpartyId == Constants.HedgingServiceName);

            var hedgingServicePositionsOpened = (await tradingPositionRepo.GetOpenedAsync(from, to))
                .Where(m => m.TakerCounterpartyId == Constants.HedgingServiceName);

            var result = new List<HedgingServiceCheckResult>();

            // TradePositionReportClosed records with Counterparty ID = LykkeHedgingService* should be checked separately: 
            // sum of volume on each TakerAccountId-CoreSymbol must be equal to current value taken from TradingPositionOpen.
            result.AddRange(hedgingServicePositionsOpened.CheckHedgingServicePositionsVolume(hedgingServicePositionsClosed));

            await _log.WriteInfoAsync("CheckHedgingService", null, $"Check finished with {result.Count} errors");
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
            await _log.WriteInfoAsync("CheckOrdersReportAndOrderClosedOpened", null, $"Started Check IsSql={isSql} From [{from?.ToString("u")}] To [{to?.ToString("u")}]");

            var tradingOrdersReportRepo = _repositoryManager.GetTradingOrder(isSql);
            var tradingPositionRepo = _repositoryManager.GetTradingPosition(isSql);

            // Ignore LykkeHedgingService
            var tradingOrders = (await tradingOrdersReportRepo.GetAsync(from, to))
                .Where(x => x.TakerCounterpartyId != Constants.HedgingServiceName)
                .ToList();
            var tradingPositions = new List<ITradingPosition>();
            tradingPositions.AddRange((await tradingPositionRepo.GetClosedAsync(from, to))
                .Where(x => x.TakerCounterpartyId != Constants.HedgingServiceName)
                .ToList());
            tradingPositions.AddRange((await tradingPositionRepo.GetOpenedAsync(from, to))
                .Where(x => x.TakerCounterpartyId != Constants.HedgingServiceName)
                .ToList());

            var result = new List<OrdersReportAndOrderClosedOpenedCheckResult>();

            // for each TradePosition based on TakerPositionID (Closed Positions Table)
            // there should exist one and only one order for OpenDate and another for CloseDate (if closed)
            result.AddRange(tradingOrders.CheckNumberOfOrders(tradingPositions));
            
            // Date should exactly match
            result.AddRange(tradingOrders.CheckOrdersDate(tradingPositions));

            // OrderType should match the CloseReason for close orders - 
            //TODO: result.AddRange(tradingOrders.CheckOrderTypes(tradingPositions));

            // AccountID should match - No AccountId in Orders.. So check may be removed
            //TODO: result.AddRange(tradingOrders.CheckOrderAccountID(tradingPositions));

            // ClientID should match            
            result.AddRange(tradingOrders.CheckOrderClientId(tradingPositions));

            await _log.WriteInfoAsync("CheckOrdersReportAndOrderClosedOpened", null, $"Check finished with {result.Count} errors");
            return result;
        }
               
        /// <summary>
        /// OpenPrice & ClosePrice consistency with Price Candles data
        /// </summary>
        /// <param name="isSql"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IPriceCandlesConsistencyResult>> CheckCandlesPriceConsistency(bool isSql, DateTime? from, DateTime? to)
        {
            await _log.WriteInfoAsync("CheckCandlesPriceConsistency", null, $"Started Check IsSql={isSql} From [{from?.ToString("u")}] To [{to?.ToString("u")}]");
            var tradingPositionRepo = _repositoryManager.GetTradingPosition(isSql);

            var allTradingPosition = (await tradingPositionRepo.GetOpenedAsync(from, to))
                .Where(m => m.TakerCounterpartyId != Constants.HedgingServiceName)
                .ToList();
            allTradingPosition.AddRange((await tradingPositionRepo.GetClosedAsync(from, to))
                .Where(m => m.TakerCounterpartyId != Constants.HedgingServiceName));
                       
            
            var result = new List<PriceCandlesConsistencyResult>();
            if (allTradingPosition.Count < 1)
            {
                await _log.WriteInfoAsync("CheckCandlesPriceConsistency", null, "Check finished with 0 errors");
                return result;
            }


            // Process Open Price candles 1 day per loop
            var minOpenDate = allTradingPosition.Min(x => x.OpenDate);
            var maxOpenDate = allTradingPosition.Max(x => x.OpenDate);
            if (minOpenDate != null && maxOpenDate != null)
            {
                var currentOpenDay = minOpenDate.Value.Date;
                do
                {
                    var dayEnd = currentOpenDay.AddDays(1).AddMilliseconds(-1);
                    var dayTradingPositions = allTradingPosition
                        .Where(t => t.OpenDate >= currentOpenDay && t.OpenDate <= dayEnd)
                        .ToList();
                    var assets = dayTradingPositions.Select(x => x.CoreSymbol)
                        .Distinct();
                    if (dayTradingPositions.Any())
                    {
                        var bidCandles = new Dictionary<string, IEnumerable<ICandle>>();
                        var askCandles = new Dictionary<string, IEnumerable<ICandle>>();
                        foreach (var asset in assets)
                        {
                            try
                            {
                                var bcandles =
                                    await _priceCandlesService.GetMinuteCandle(asset, false, currentOpenDay, dayEnd);
                                bidCandles.Add(asset, bcandles);
                            }
                            catch (Exception ex)
                            {
                                await _log.WriteWarningAsync("CheckCandlesPriceConsistency", null, ex.Message);
                                bidCandles.Add(asset, new ICandle[0]);
                            }
                            try
                            {
                                var acandles =
                                await _priceCandlesService.GetMinuteCandle(asset, true, currentOpenDay, dayEnd);
                                askCandles.Add(asset, acandles);
                            }
                            catch (Exception ex)
                            {
                                await _log.WriteWarningAsync("CheckCandlesPriceConsistency", null, ex.Message);
                                askCandles.Add(asset, new ICandle[0]);
                            }
                        }

                        result.AddRange(dayTradingPositions.CheckOpenPriceCandlesConsistency(askCandles, bidCandles));
                    }

                    currentOpenDay = currentOpenDay.AddDays(1);
                } while (currentOpenDay <= maxOpenDate.Value.Date);
            }

            // Process Close Price candles 1 day per loop
            var lowerCloseDate = allTradingPosition.Min(x => x.CloseDate);
            var upperCloseDate = allTradingPosition.Max(x => x.CloseDate);

            if (upperCloseDate.HasValue && lowerCloseDate.HasValue)
            {
                var minCloseDate = lowerCloseDate.Value;
                var maxCloseDate = lowerCloseDate.Value;
                var currentCloseDay = minCloseDate.Date;
                do
                {
                    var dayEnd = currentCloseDay.AddDays(1).AddMilliseconds(-1);
                    var dayTradingPositions = allTradingPosition
                        .Where(t => t.CloseDate >= currentCloseDay && t.CloseDate <= dayEnd)
                        .ToList();
                    var assets = dayTradingPositions.Select(x => x.CoreSymbol)
                        .Distinct();
                    if (dayTradingPositions.Any())
                    {
                        var bidCandles = new Dictionary<string, IEnumerable<ICandle>>();
                        var askCandles = new Dictionary<string, IEnumerable<ICandle>>();
                        foreach (var asset in assets)
                        {
                            var bcandles = await _priceCandlesService.GetMinuteCandle(asset, false, currentCloseDay, dayEnd);
                            bidCandles.Add(asset, bcandles);

                            var acandles = await _priceCandlesService.GetMinuteCandle(asset, true, currentCloseDay, dayEnd);
                            askCandles.Add(asset, acandles);
                        }
                        result.AddRange(dayTradingPositions.CheckClosePriceCandlesConsistency(askCandles, bidCandles));
                    }
                    currentCloseDay = currentCloseDay.AddDays(1);
                } while (currentCloseDay <= maxCloseDate.Date);
            }
            await _log.WriteInfoAsync("CheckCandlesPriceConsistency", null, $"Check finished with {result.Count} errors");
            return result;
        }
        
        /// <summary>
        /// MarginEvents account status consistency with balance transactions
        /// </summary>
        /// <param name="isSql"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IMarginEventsAccountStatusCheckResult>> CheckMarginEventsAccountStatus(bool isSql, DateTime? from, DateTime? to)
        {
            await _log.WriteInfoAsync("CheckMarginEventsAccountStatus", null, $"Started Check IsSql={isSql} From [{from?.ToString("u")}] To [{to?.ToString("u")}]");
            var marginEventsRepo = _repositoryManager.GetAccountMarginEventReport(isSql);
            var accountTransactionRepo = _repositoryManager.GetAccountTransactionsReport(isSql);
            var tradingPositionRepo = _repositoryManager.GetTradingPosition(isSql);

            var marginEvents = (await marginEventsRepo.GetAsync(from, to))
                .ToList();
            var result = new List<MarginEventsAccountStatusCheckResult>();

            // balance equals the account balance calculated for the corresponding date base on transactions like in Check 1)
            var accountTransaction = await accountTransactionRepo.GetAsync(from, to);
            result.AddRange(marginEvents.CheckBalanceTransactions(accountTransaction));

            // Number of position open should correspond to positions open at that time according to OpenDate and CloseDate fields of Closed or Open Trades
            var tradingPositions = (await tradingPositionRepo.GetOpenedAsync(from, to)).ToList();
            tradingPositions.AddRange((await tradingPositionRepo.GetClosedAsync(from, to)));            
            result.AddRange(marginEvents.CheckOpenPositions(tradingPositions));

            await _log.WriteInfoAsync("CheckMarginEventsAccountStatus", null, $"Check finished with {result.Count} errors");
            return result;
        }
        

    }
}
