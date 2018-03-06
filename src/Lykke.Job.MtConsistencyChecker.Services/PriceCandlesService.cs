using Common.Log;
using Lykke.Job.MtConsistencyChecker.Contract;
using Lykke.Job.MtConsistencyChecker.Core.Extensions;
using Lykke.Job.MtConsistencyChecker.Core.Services;
using Lykke.Job.MtConsistencyChecker.Core.Settings.JobSettings;
using Lykke.Job.MtConsistencyChecker.Services.Extensions;
using Lykke.Service.CandlesHistory.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Job.MtConsistencyChecker.Services
{
    public class PriceCandlesService: IPriceCandlesService
    {
        private readonly Candleshistoryservice _candleshistoryservice;
        private readonly ILog _log;
        public PriceCandlesService(PriceCandlesSettings priceCandlesSettings, ILog log)
        {
            _log = log;
            _candleshistoryservice = new Candleshistoryservice(new Uri(priceCandlesSettings.HistoryUrl));
        }

        public async Task<IEnumerable<ICandle>> GetMinuteCandle(string assetPait, bool isAsk, DateTime from, DateTime to)
        {
            await _log.WriteInfoAsync("GetMinuteCandle", null, $"Getting Candles for [{assetPait}] Ask={isAsk} From {from.ToDateTimeCustomString()} To {to.ToDateTimeCustomString()}");
            var candleHistory = await _candleshistoryservice.GetCandlesHistoryAsync(assetPait,
                Service.CandlesHistory.Client.Models.CandlePriceType.Ask,
                Service.CandlesHistory.Client.Models.CandleTimeInterval.Minute,
                from, to);
            return candleHistory.History.Select(c => c.ToDto());
        }
    }
}
