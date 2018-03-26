using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Job.MtConsistencyChecker.Contract;
using Lykke.Job.MtConsistencyChecker.Contract.Results;

namespace Lykke.Job.MtConsistencyChecker.Services.Extensions
{
    internal static class HedgingServiceExtensions
    {

        /// <summary>
        /// TradePositionReportClosed records with Counterparty ID = LykkeHedgingService* should be checked separately: 
        /// sum of volume on each TakerAccountId-CoreSymbol must be equal to current value taken from TradingPositionOpen.
        /// need to take each value of TakerAccountId-CoreSymbol from TradingPositionOpen and compare it to sum of TakerAccountId-CoreSymbol's from TradingPositionClosed
        /// </summary>
        /// <param name="hedgingServicePositionsOpened"></param>
        /// <param name="hedgingServicePositionsClosed"></param>
        /// <returns></returns>
        internal static IEnumerable<HedgingServiceCheckResult> CheckHedgingServicePositionsVolume(this IEnumerable<ITradingPosition> hedgingServicePositionsOpened, IEnumerable<ITradingPosition> hedgingServicePositionsClosed)
        {
            var result = new List<HedgingServiceCheckResult>();
            var openedList = hedgingServicePositionsOpened.ToList();
            var accountIds = openedList.Select(x => x.TakerAccountId)
                .Distinct();

            var closedList = hedgingServicePositionsClosed.ToList();
            foreach (var accountId in accountIds)
            {
                var coreSymbols = openedList
                    .Where(a => a.TakerAccountId == accountId)
                    .Select(x => x.CoreSymbol)
                    .Distinct();
                foreach (var coreSymbol in coreSymbols)
                {
                    var accountIdCoreSymbolRecords = openedList.Where(a => a.TakerAccountId == accountId && a.CoreSymbol == coreSymbol)
                        .ToList();
                    if (accountIdCoreSymbolRecords.Count != 1)
                    {
                        result.Add(new HedgingServiceCheckResult
                        {   
                            Error = $"Hedging Service Opened Positions has more than one record for accountId_coreSymbol {accountId}_{coreSymbol}"
                        });
                    }
                    else
                    {
                        var accountIdCoreSymbolVolume = accountIdCoreSymbolRecords.First().Volume;
                        var lastClosed = closedList
                            .Where(a => a.TakerAccountId == accountId && a.CoreSymbol == coreSymbol)
                            .OrderByDescending(p => p.CloseDate)
                            .Take(2)
                            .ToArray();
                        if (lastClosed.Length < 2)
                        {
                            result.Add(new HedgingServiceCheckResult
                            {
                                OpenPosition = accountIdCoreSymbolRecords.First(),
                                Error = $"Hedging Service Closed Positions list must have at least 2 entries for comparison. AccountId_coreSymbol {accountId}_{coreSymbol}"
                            });
                            continue;
                        }
                        var accountIdCoreSymbolClosedVolume = lastClosed[0].Volume - lastClosed[1].Volume;
                        if (Math.Abs(accountIdCoreSymbolVolume - accountIdCoreSymbolClosedVolume) > Double.Epsilon)
                        {
                            result.Add(new HedgingServiceCheckResult
                            {
                                OpenPosition = accountIdCoreSymbolRecords.First(),
                                Error = $"[delta]={accountIdCoreSymbolVolume - accountIdCoreSymbolClosedVolume}, [OpenPosition.Volume]={accountIdCoreSymbolVolume}, [ClosePositions.VolumeDiff]={accountIdCoreSymbolClosedVolume}"
                            });
                        }
                    }

                }
            }
            return result;
        }
    }
}
