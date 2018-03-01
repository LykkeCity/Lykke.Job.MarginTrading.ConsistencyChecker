using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Results;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Services
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

            var accountIds = hedgingServicePositionsOpened.Select(x => x.TakerAccountId)
                .Distinct();

            foreach (var accountId in accountIds)
            {
                var coreSymbols = hedgingServicePositionsOpened
                    .Where(a => a.TakerAccountId == accountId)
                    .Select(x => x.CoreSymbol)
                    .Distinct();
                foreach (var coreSymbol in coreSymbols)
                {
                    var accountId_coreSymbolRecords = hedgingServicePositionsOpened.Where(a => a.TakerAccountId == accountId && a.CoreSymbol == coreSymbol)
                        .ToList();
                    if (accountId_coreSymbolRecords.Count != 1)
                    {
                        result.Add(new HedgingServiceCheckResult
                        {   
                            Error = $"Hedging Service Opened Positions has more than one record for accountId_coreSymbol {accountId}_{coreSymbol}"
                        });
                    }
                    else
                    {
                        var accountId_coreSymbol_volume = accountId_coreSymbolRecords.First().Volume;
                        var lastClosed = hedgingServicePositionsClosed
                            .Where(a => a.TakerAccountId == accountId && a.CoreSymbol == coreSymbol)
                            .OrderByDescending(p => p.CloseDate)
                            .Take(2)
                            .ToArray();
                        var accountId_coreSymbol_closedVolume = lastClosed[0].Volume - lastClosed[1].Volume;
                        if (accountId_coreSymbol_volume != accountId_coreSymbol_closedVolume)
                        {
                            result.Add(new HedgingServiceCheckResult
                            {
                                OpenPosition = accountId_coreSymbolRecords.First(),
                                Error = $"[delta]={accountId_coreSymbol_volume - accountId_coreSymbol_closedVolume}, [OpenPosition.Volume]={accountId_coreSymbol_volume}, [ClosePositions.VolumeDiff]={accountId_coreSymbol_closedVolume}"
                            });
                        }
                    }

                }
            }
            return result;
        }
    }
}
