using Lykke.SlackNotifications;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Services.Infrastructure
{
    public interface IMtSlackNotificationsSender : ISlackNotificationsSender
    {
        Task SendRawAsync(string type, string sender, string message);
    }
}
