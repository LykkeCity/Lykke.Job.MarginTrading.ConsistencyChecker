using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.PeriodicalHandlers
{
    public class ConsistencyCheckHandler : TimerPeriod
    {
        private readonly ILog _log;
        private readonly IMonitorService _monitorService;

        private bool initialCall;

        public ConsistencyCheckHandler(ILog log, IMonitorService monitorService) :
            base(nameof(ConsistencyCheckHandler), monitorService.MonitorInterval, log)
        {
            _log = log;
            _monitorService = monitorService;
            initialCall = true;
        }

        public override async Task Execute()
        {
            if (initialCall)
                initialCall = false;
            else
                await _monitorService.CheckConsistency();
            await _log.WriteInfoAsync(nameof(ConsistencyCheckHandler), null, $"Next Consistency Check:[{DateTime.UtcNow.AddMilliseconds(_monitorService.MonitorInterval).ToString("u")}]");
        }
    }
}
