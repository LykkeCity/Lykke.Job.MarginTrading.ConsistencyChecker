﻿using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Job.MtConsistencyChecker.Core.Services;

namespace Lykke.Job.MtConsistencyChecker.PeriodicalHandlers
{
    public class ConsistencyCheckHandler : TimerPeriod
    {
        private readonly ILog _log;
        private readonly IMonitorService _monitorService;

        private bool _initialCall;

        public ConsistencyCheckHandler(ILog log, IMonitorService monitorService) :
            base(nameof(ConsistencyCheckHandler), monitorService.MonitorInterval, log)
        {
            _log = log;
            _monitorService = monitorService;
            _initialCall = true;
        }

        public override async Task Execute()
        {
            if (_initialCall)
                _initialCall = false;
            else
                await _monitorService.CheckConsistency();
            await _log.WriteInfoAsync(nameof(ConsistencyCheckHandler), null, $"Next Consistency Check:[{DateTime.UtcNow.AddMilliseconds(_monitorService.MonitorInterval):u}]");
        }
    }
}
