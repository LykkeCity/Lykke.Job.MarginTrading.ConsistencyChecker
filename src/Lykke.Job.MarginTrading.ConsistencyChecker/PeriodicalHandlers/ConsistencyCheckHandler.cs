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

        public ConsistencyCheckHandler(ILog log, IMonitorService monitorService) :
            // TODO: Sometimes, it is enough to hardcode the period right here, but sometimes it's better to move it to the settings.
            // Choose the simplest and sufficient solution
            base(nameof(ConsistencyCheckHandler), (int)TimeSpan.FromSeconds(40).TotalMilliseconds, log)
            //base(nameof(ConsistencyCheckHandler), monitorService.MonitorInterval, log)
        {
            _log = log;
            _monitorService = monitorService;
        }

        public override async Task Execute()
        {
            await _monitorService.CheckConsistency();
        }
    }
}
