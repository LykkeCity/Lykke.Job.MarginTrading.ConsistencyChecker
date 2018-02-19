﻿using Common.Log;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Models;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Settings.JobSettings;
using Lykke.Job.MarginTrading.ConsistencyChecker.Services.Extensions;
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
        private readonly PriceCandlesSettings _priceCandlesSettings;
        private readonly ILog _log;
        public PriceCandlesService(PriceCandlesSettings priceCandlesSettings, ILog log)
        {            
            _log = log;
            _priceCandlesSettings = priceCandlesSettings;
            _candlesClient = new Candleshistoryservice();
        }

        public async Task<IEnumerable<ICandle>> GetMinuteCandle(string assetPait, bool isAsk, DateTime from, DateTime to)
        {            
            var candleHistory = await _candlesClient.GetCandlesHistoryAsync(assetPait,
                Service.CandlesHistory.Client.Models.CandlePriceType.Ask,
                Service.CandlesHistory.Client.Models.CandleTimeInterval.Minute,
                from, to);
            return candleHistory.History.Select(c => c.ToDto());
        }
        public async Task<IEnumerable<ICandle>> GetHourCandle(string assetPait, bool isAsk, DateTime from, DateTime to)
        {
            var candleHistory = await _candlesClient.GetCandlesHistoryAsync(assetPait,
                isAsk ? Service.CandlesHistory.Client.Models.CandlePriceType.Ask : Service.CandlesHistory.Client.Models.CandlePriceType.Bid,
                Service.CandlesHistory.Client.Models.CandleTimeInterval.Hour,
                from, to);
            return candleHistory.History.Select(c => c.ToDto());
        }
    }
}
