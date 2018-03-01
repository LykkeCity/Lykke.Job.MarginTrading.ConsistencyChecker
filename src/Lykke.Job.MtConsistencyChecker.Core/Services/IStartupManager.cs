using System.Threading.Tasks;

namespace Lykke.Job.MtConsistencyChecker.Core.Services
{
    public interface IStartupManager
    {
        Task StartAsync();
    }
}