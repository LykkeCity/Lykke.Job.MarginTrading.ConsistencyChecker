using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Models;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Services.Extensions
{
    internal static class PriceCandlesExtensions
    {
        public static ICandle ToDto(this Service.CandlesHistory.Client.Models.Candle src)
        {
            return new Candle
            {
                Close = src.Close,
                DateTime = src.DateTime,
                High = src.High,
                LastTradePrice = src.LastTradePrice,
                Low = src.Low,
                Open = src.Open,
                TradingOppositeVolume = src.TradingOppositeVolume,
                TradingVolume = src.TradingVolume
            };
        }
        public static IEnumerable<PriceCandlesConsistencyResult> CheckOpenPriceCandlesConsistency(this IEnumerable<ITradingPosition> tradingPositions, Dictionary<string, IEnumerable<ICandle>> askCandles, Dictionary<string, IEnumerable<ICandle>> bidCandles)
        {
            var result = new List<PriceCandlesConsistencyResult>();
            foreach (var tradingPosition in tradingPositions.OrderBy(t => t.OpenDate))
            {
                var candlesList = tradingPosition.CoreSide == "Buy" ? askCandles : bidCandles;
                var minute = new DateTime(tradingPosition.OpenDate.Value.Year, tradingPosition.OpenDate.Value.Month, tradingPosition.OpenDate.Value.Day, tradingPosition.OpenDate.Value.Hour, tradingPosition.OpenDate.Value.Minute, 0);
                var openCandle = candlesList[tradingPosition.CoreSymbol]
                    .Single(c => c.DateTime == minute);

                if (tradingPosition.OpenPrice > openCandle.High)
                    result.Add(new PriceCandlesConsistencyResult
                    {
                        Position = tradingPosition,
                        Candle = openCandle,
                        Error = $"TradingPosition Open Price over Candle High Limit {openCandle.High}"
                    });
                if (tradingPosition.OpenPrice < openCandle.Low)
                    result.Add(new PriceCandlesConsistencyResult
                    {
                        Position = tradingPosition,
                        Candle = openCandle,
                        Error = $"TradingPosition Open Price below Candle Low Limit {openCandle.Low}"
                    });              
            }

            return result;
        }

        public static IEnumerable<PriceCandlesConsistencyResult> CheckClosePriceCandlesConsistency(this IEnumerable<ITradingPosition> tradingPositions, Dictionary<string, IEnumerable<ICandle>> askCandles, Dictionary<string, IEnumerable<ICandle>> bidCandles)
        {
            var result = new List<PriceCandlesConsistencyResult>();
            foreach (var tradingPosition in tradingPositions.OrderBy(t => t.CloseDate))
            {
                var candlesList = tradingPosition.CoreSide == "Buy" ? askCandles : bidCandles;
                var minute = new DateTime(tradingPosition.CloseDate.Value.Year, tradingPosition.CloseDate.Value.Month, tradingPosition.CloseDate.Value.Day, tradingPosition.CloseDate.Value.Hour, tradingPosition.CloseDate.Value.Minute, 0);
                var closeCandle = candlesList[tradingPosition.CoreSymbol]
                    .Single(c => c.DateTime == minute);

                if (tradingPosition.ClosePrice > closeCandle.High)
                    result.Add(new PriceCandlesConsistencyResult
                    {
                        Position = tradingPosition,
                        Error = $"TradingPosition Close Price over Candle High Limit {closeCandle.High}"
                    });
                if (tradingPosition.ClosePrice < closeCandle.Low)
                    result.Add(new PriceCandlesConsistencyResult
                    {
                        Position = tradingPosition,
                        Error = $"TradingPosition Close Price below Candle Low Limit {closeCandle.Low}"
                    });
            }

            return result;
        }
    }
    
}
