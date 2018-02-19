using Common.Log;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Settings.JobSettings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Services
{
    public class MonitorService: IMonitorService
    {
        private readonly ILog _log;
        private readonly IConsistencyService _consistencyService;
        private readonly MonitorSettings _monitorSettings;
        private readonly int _monitorInterval;
        DateTime? _lastCheck;

        public MonitorService(ILog log, MonitorSettings monitorSettings, IConsistencyService consistencyService )
        {
            _log = log;
            _monitorSettings = monitorSettings;
            _consistencyService = consistencyService;
            _monitorInterval = _monitorSettings.ConsistencyCheckInterval;
            _lastCheck = null;
        }

        public int MonitorInterval => _monitorInterval;
        public DateTime? LastCheck => _lastCheck;

        public async Task CheckConsistency()
        {
            var currentCheck = DateTime.UtcNow;
            try
            {
                var balanceAndTransactionAmount = await _consistencyService.CheckBalanceAndTransactionAmount(_monitorSettings.CheckSql, _lastCheck, currentCheck);

                var balanceAndOrderClosed = await _consistencyService.CheckBalanceAndOrderClosed(_monitorSettings.CheckSql, _lastCheck, currentCheck);

                var ordersReportAndOrderClosedOpened = await _consistencyService.CheckOrdersReportAndOrderClosedOpened(_monitorSettings.CheckSql, _lastCheck, currentCheck);

                var candlesPriceConsistency = await _consistencyService.CheckCandlesPriceConsistency(_monitorSettings.CheckSql, _lastCheck, currentCheck);

                var tradePnLConsistency = await _consistencyService.CheckTradePnLConsistency(_monitorSettings.CheckSql, _lastCheck, currentCheck);

                var marginEventsAccountStatus = await _consistencyService.CheckMarginEventsAccountStatus(_monitorSettings.CheckSql, _lastCheck, currentCheck);

                var hedgingServiceBalance = await _consistencyService.CheckHedgingServiceBalance(_monitorSettings.CheckSql, _lastCheck, currentCheck);

                _lastCheck = currentCheck;
            }
            catch (Exception ex01)
            {
                await _log.WriteErrorAsync(nameof(CheckConsistency), null, ex01);
            }
        }
    }
}
