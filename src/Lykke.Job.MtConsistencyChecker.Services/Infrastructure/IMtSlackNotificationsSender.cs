using Lykke.SlackNotifications;
using System.Threading.Tasks;

namespace Lykke.Job.MtConsistencyChecker.Services.Infrastructure
{
    public interface IMtSlackNotificationsSender : ISlackNotificationsSender
    {
        Task SendRawAsync(string type, string sender, string message);
    }
}
