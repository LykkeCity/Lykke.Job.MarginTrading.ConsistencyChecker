using Lykke.Service.CandlesHistory.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Services
{
    public class PriceCandlesService: IPriceCandlesService
    {
        private readonly Candleshistoryservice _candlesClient;

        public PriceCandlesService()
        {
            _candlesClient = new Candleshistoryservice();
        }

        public async Task<IEnumerable<double>> GetCandleMinute(string assetPait, DateTime interval)
        {
            var dateFrom = new DateTime(interval.Year, interval.Month, interval.Day, interval.Hour, interval.Minute, 0);
            var dateTo = new DateTime(interval.Year, interval.Month, interval.Day, interval.Hour, interval.Minute, 59);
            var candleHistory = await _candlesClient.GetCandlesHistoryAsync(assetPait,
                Service.CandlesHistory.Client.Models.CandlePriceType.Ask,
                Service.CandlesHistory.Client.Models.CandleTimeInterval.Minute,
                dateFrom, dateTo);
            var candle = candleHistory.History.First();
            return new List<double>()
            {
                candle.Open,
                candle.High,
                candle.Low,
                candle.Close,
            };
        }
    }
}
