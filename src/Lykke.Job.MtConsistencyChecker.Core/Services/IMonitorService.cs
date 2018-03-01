using System;
using System.Threading.Tasks;

namespace Lykke.Job.MtConsistencyChecker.Core.Services
{
    public interface IMonitorService
    {
        int MonitorInterval { get; }
        DateTime? LastCheck { get; }
        Task CheckConsistency();
    }
}
