using Common.Log;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Models;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Repositories;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Settings.JobSettings;
using Lykke.Job.MarginTrading.ConsistencyChecker.Services.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Services
{
    public class ConsistencyMonitor : IMonitorService
    {
        private readonly ILog _log;
        private readonly IConsistencyService _consistencyService;
        private readonly IMtSlackNotificationsSender _slackNotificationsSender;
        private readonly MonitorSettings _monitorSettings;
        private readonly int _monitorInterval;
        private readonly ICheckResultRepository _checkResultRepository;
        private readonly IBalanceAndTransactionAmountRepository _balanceAndTransactionAmountRepository;
        private readonly IBalanceAndOrderClosedRepository _balanceAndOrderClosedRepository;
        private readonly IOrdersReportAndOrderClosedOpenedRepository _ordersReportAndOrderClosedOpenedRepository;
        private readonly IPriceCandlesConsistencyRepository _priceCandlesConsistencyRepository;
        private readonly IMarginEventsAccountStatusRepository _marginEventsAccountStatusRepository;
        private readonly IHedgingServiceRepository _hedgingServiceRepository;
        private readonly IAlertSeverityLevelService _alertSeverityLevelService;

        DateTime? _lastCheck;

        public ConsistencyMonitor (MonitorSettings monitorSettings, 
            IConsistencyService consistencyService,
            IMtSlackNotificationsSender slackNotificationsSender,
            ICheckResultRepository checkResultRepository,
            IBalanceAndTransactionAmountRepository balanceAndTransactionAmountRepository,
            IBalanceAndOrderClosedRepository balanceAndOrderClosedRepository,
            IOrdersReportAndOrderClosedOpenedRepository ordersReportAndOrderClosedOpenedRepository,
            IPriceCandlesConsistencyRepository priceCandlesConsistencyRepository,
            IMarginEventsAccountStatusRepository marginEventsAccountStatusRepository,
            IHedgingServiceRepository hedgingServiceRepository,
            IAlertSeverityLevelService alertSeverityLevelService,
            ILog log)
        {
            _log = log;
            _monitorSettings = monitorSettings;
            _consistencyService = consistencyService;
            _monitorInterval = _monitorSettings.ConsistencyCheckInterval;
            _checkResultRepository = checkResultRepository;
            _balanceAndTransactionAmountRepository = balanceAndTransactionAmountRepository;
            _balanceAndOrderClosedRepository = balanceAndOrderClosedRepository;
            _ordersReportAndOrderClosedOpenedRepository = ordersReportAndOrderClosedOpenedRepository;
            _priceCandlesConsistencyRepository = priceCandlesConsistencyRepository;
            _marginEventsAccountStatusRepository = marginEventsAccountStatusRepository;
            _hedgingServiceRepository = hedgingServiceRepository;
            _alertSeverityLevelService = alertSeverityLevelService;
            _slackNotificationsSender = slackNotificationsSender;

            var lastCheckResult = Task.Run(async () => await _checkResultRepository.GetLastAsync()).Result;
            if (lastCheckResult == null)
                _lastCheck = null;
            else
                _lastCheck = lastCheckResult.DateTo;

            _log.WriteInfo(nameof(ConsistencyMonitor), null, $"Consistency Monitor Started. LastCheck:[{_lastCheck}]");
        }

        public int MonitorInterval => _monitorInterval;
        public DateTime? LastCheck => _lastCheck;

        public async Task CheckConsistency()
        {
            var currentCheck = DateTime.UtcNow;
            await _log.WriteInfoAsync("CheckConsistency", null, $"New Consistency Check. Interval:[{_lastCheck}]->[{currentCheck}]");
            try
            {
                var balanceAndTransactionAmount = await _consistencyService.CheckBalanceAndTransactionAmount(_monitorSettings.CheckSql, _lastCheck, currentCheck);
                await _balanceAndTransactionAmountRepository.AddAsync(balanceAndTransactionAmount, currentCheck);

                var balanceAndOrderClosed = await _consistencyService.CheckBalanceAndOrderClosed(_monitorSettings.CheckSql, _lastCheck, currentCheck);
                await _balanceAndOrderClosedRepository.AddAsync(balanceAndOrderClosed, currentCheck);

                var ordersReportAndOrderClosedOpened = await _consistencyService.CheckOrdersReportAndOrderClosedOpened(_monitorSettings.CheckSql, _lastCheck, currentCheck);
                await _ordersReportAndOrderClosedOpenedRepository.AddAsync(ordersReportAndOrderClosedOpened, currentCheck);

                var candlesPriceConsistency = await _consistencyService.CheckCandlesPriceConsistency(_monitorSettings.CheckSql, _lastCheck, currentCheck);
                await _priceCandlesConsistencyRepository.AddAsync(candlesPriceConsistency, currentCheck);

                var marginEventsAccountStatus = await _consistencyService.CheckMarginEventsAccountStatus(_monitorSettings.CheckSql, _lastCheck, currentCheck);
                await _marginEventsAccountStatusRepository.AddAsync(marginEventsAccountStatus, currentCheck);

                var hedgingServiceVolume = await _consistencyService.CheckHedgingService(_monitorSettings.CheckSql, _lastCheck, currentCheck);
                await _hedgingServiceRepository.AddAsync(hedgingServiceVolume, currentCheck);

                var totalErrors = balanceAndTransactionAmount.Count() + balanceAndOrderClosed.Count() + ordersReportAndOrderClosedOpened.Count() +
                    candlesPriceConsistency.Count() + marginEventsAccountStatus.Count() + hedgingServiceVolume.Count();
                await _checkResultRepository.AddAsync(new CheckResult
                {
                    Date = currentCheck,
                    DateFrom = _lastCheck ?? new DateTime(2000,01,01),
                    DateTo = currentCheck,
                    Comments = $"Check finished with {totalErrors} errors"
                });
                _lastCheck = currentCheck;


                if (totalErrors == 0)
                    await _log.WriteInfoAsync("CheckConsistency", null, "Consistency check finished without errors");
                else
                    WriteMessage($"Consistency check finished with {totalErrors} errors. Check Date: {currentCheck.ToString("u")}", EventTypeEnum.ConsistencyError);
            }
            catch (Exception ex01)
            {
                await _log.WriteErrorAsync(nameof(CheckConsistency), null, ex01);
            }
        }

        private void WriteMessage(string message, EventTypeEnum eventType)
        {
            _log.WriteInfoAsync("CheckConsistency", null, message);
            var slackChannelType = _alertSeverityLevelService.GetSlackChannelType(eventType);
            if (!string.IsNullOrWhiteSpace(slackChannelType))
                _slackNotificationsSender.SendRawAsync(slackChannelType, nameof(ConsistencyMonitor), message);
        }
    }
}
