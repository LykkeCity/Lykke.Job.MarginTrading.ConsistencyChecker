using System;
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

        public ConsistencyCheckHandler(ILog log, IMonitorService monitorService) :
            base(nameof(ConsistencyCheckHandler), monitorService.MonitorInterval, log)
        {
            _log = log;
            _monitorService = monitorService;
        }

        public override async Task Execute()
        {
            await _monitorService.CheckConsistency();
            await _log.WriteInfoAsync(nameof(ConsistencyCheckHandler), null, $"Next Consistency Check:[{DateTime.UtcNow.AddMilliseconds(_monitorService.MonitorInterval):u}]");
        }
    }
}
