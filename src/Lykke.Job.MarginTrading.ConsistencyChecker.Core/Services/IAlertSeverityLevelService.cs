using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Enums;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services
{
    public interface IAlertSeverityLevelService
    {
        string GetSlackChannelType(EventTypeEnum eventType);
    }
}
