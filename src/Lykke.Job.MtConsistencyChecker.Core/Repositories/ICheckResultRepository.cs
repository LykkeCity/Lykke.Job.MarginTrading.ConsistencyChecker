using Lykke.Job.MtConsistencyChecker.Contract;
using System.Threading.Tasks;

namespace Lykke.Job.MtConsistencyChecker.Core.Repositories
{
    public interface ICheckResultRepository
    {
        Task AddAsync(ICheckResult entity);

        Task<ICheckResult> GetLastAsync();
    }
}
