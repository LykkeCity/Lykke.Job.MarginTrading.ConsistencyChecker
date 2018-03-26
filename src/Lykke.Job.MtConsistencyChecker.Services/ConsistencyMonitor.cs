using Common.Log;
using Lykke.Job.MtConsistencyChecker.Contract.Models;
using Lykke.Job.MtConsistencyChecker.Core.Enums;
using Lykke.Job.MtConsistencyChecker.Core.Repositories;
using Lykke.Job.MtConsistencyChecker.Core.Services;
using Lykke.Job.MtConsistencyChecker.Core.Settings.JobSettings;
using Lykke.Job.MtConsistencyChecker.Services.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Job.MtConsistencyChecker.Services
{
    public class ConsistencyMonitor : IMonitorService
    {
        private readonly ILog _log;
        private readonly IConsistencyService _consistencyService;
        private readonly IMtSlackNotificationsSender _slackNotificationsSender;
        private readonly MonitorSettings _monitorSettings;
        private readonly ICheckResultRepository _checkResultRepository;
        private readonly IBalanceAndTransactionAmountRepository _balanceAndTransactionAmountRepository;
        private readonly IBalanceAndOrderClosedRepository _balanceAndOrderClosedRepository;
        private readonly IOrdersReportAndOrderClosedOpenedRepository _ordersReportAndOrderClosedOpenedRepository;
        private readonly IPriceCandlesConsistencyRepository _priceCandlesConsistencyRepository;
        private readonly IMarginEventsAccountStatusRepository _marginEventsAccountStatusRepository;
        private readonly IHedgingServiceRepository _hedgingServiceRepository;
        private readonly IAlertSeverityLevelService _alertSeverityLevelService;

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
            MonitorInterval = _monitorSettings.ConsistencyCheckInterval;
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
                LastCheck = null;
            else
                LastCheck = lastCheckResult.DateTo;

            _log.WriteInfo(nameof(ConsistencyMonitor), null, $"Consistency Monitor Started. LastCheck:[{LastCheck?.ToString("u")}]");
        }

        public int MonitorInterval { get; }

        public DateTime? LastCheck { get; private set; }

        public async Task CheckConsistency()
        {
            var currentCheck = DateTime.UtcNow;
            await _log.WriteInfoAsync("CheckConsistency", null, $"New Consistency Check. Interval:[{LastCheck?.ToString("u")}]->[{currentCheck:u}]");
            var totalErrors = 0;
            try
            {
                var balanceAndTransactionAmount = (await _consistencyService.CheckBalanceAndTransactionAmount(_monitorSettings.CheckSql, LastCheck, currentCheck))
                    .ToList();
                totalErrors += balanceAndTransactionAmount.Count;
                await _balanceAndTransactionAmountRepository.AddAsync(balanceAndTransactionAmount, currentCheck);
            }
            catch (Exception ex01) { await _log.WriteErrorAsync(nameof(CheckConsistency), null, ex01); }

            try
            {
                var balanceAndOrderClosed = (await _consistencyService.CheckBalanceAndOrderClosed(_monitorSettings.CheckSql, LastCheck, currentCheck))
                    .ToList();
                totalErrors += balanceAndOrderClosed.Count;
                await _balanceAndOrderClosedRepository.AddAsync(balanceAndOrderClosed, currentCheck);
            }
            catch (Exception ex01) { await _log.WriteErrorAsync(nameof(CheckConsistency), null, ex01); }

            try
            {
                var ordersReportAndOrderClosedOpened = (await _consistencyService.CheckOrdersReportAndOrderClosedOpened(_monitorSettings.CheckSql, LastCheck, currentCheck))
                    .ToList();
                totalErrors += ordersReportAndOrderClosedOpened.Count;
                await _ordersReportAndOrderClosedOpenedRepository.AddAsync(ordersReportAndOrderClosedOpened, currentCheck);
            }
            catch (Exception ex01) { await _log.WriteErrorAsync(nameof(CheckConsistency), null, ex01); }

            try
            {
                var candlesPriceConsistency = (await _consistencyService.CheckCandlesPriceConsistency(_monitorSettings.CheckSql, LastCheck, currentCheck))
                    .ToList();
                totalErrors += candlesPriceConsistency.Count;
                await _priceCandlesConsistencyRepository.AddAsync(candlesPriceConsistency, currentCheck);
            }
            catch (Exception ex01) { await _log.WriteErrorAsync(nameof(CheckConsistency), null, ex01); }
            try
            {
                var marginEventsAccountStatus = (await _consistencyService.CheckMarginEventsAccountStatus(_monitorSettings.CheckSql, LastCheck, currentCheck))
                    .ToList();
                totalErrors += marginEventsAccountStatus.Count;
                await _marginEventsAccountStatusRepository.AddAsync(marginEventsAccountStatus, currentCheck);
            }
            catch (Exception ex01) { await _log.WriteErrorAsync(nameof(CheckConsistency), null, ex01); }

            try
            {
                var hedgingServiceVolume = (await _consistencyService.CheckHedgingService(_monitorSettings.CheckSql, LastCheck, currentCheck))
                    .ToList();
                totalErrors += hedgingServiceVolume.Count;
                await _hedgingServiceRepository.AddAsync(hedgingServiceVolume, currentCheck);
            }
            catch (Exception ex01) { await _log.WriteErrorAsync(nameof(CheckConsistency), null, ex01); }


            await _checkResultRepository.AddAsync(new CheckResult
            {
                Date = currentCheck,
                DateFrom = LastCheck ?? new DateTime(2000, 01, 01),
                DateTo = currentCheck,
                Comments = $"Check finished with {totalErrors} errors"
            });
            LastCheck = currentCheck;


            if (totalErrors == 0)
                await _log.WriteInfoAsync("CheckConsistency", null, "Consistency check finished without errors");
            else
                WriteMessage($"Consistency check finished with {totalErrors} errors. Check Date: {currentCheck:u}", EventTypeEnum.ConsistencyError);

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
