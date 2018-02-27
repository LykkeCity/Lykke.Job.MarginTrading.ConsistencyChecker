using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Core.Repositories
{
    public interface ICheckResultRepository
    {
        Task AddAsync(ICheckResult entity);

        Task<ICheckResult> GetLastAsync();
    }
}
