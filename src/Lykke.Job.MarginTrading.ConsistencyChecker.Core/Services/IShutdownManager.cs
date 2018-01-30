using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}