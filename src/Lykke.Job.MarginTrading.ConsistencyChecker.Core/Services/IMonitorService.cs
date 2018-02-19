using System;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services
{
    public interface IMonitorService
    {
        int MonitorInterval { get; }
        DateTime? LastCheck { get; }
        Task CheckConsistency();
    }
}
