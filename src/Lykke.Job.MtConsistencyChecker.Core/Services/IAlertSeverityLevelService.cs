using Lykke.Job.MtConsistencyChecker.Core.Enums;

namespace Lykke.Job.MtConsistencyChecker.Core.Services
{
    public interface IAlertSeverityLevelService
    {
        string GetSlackChannelType(EventTypeEnum eventType);
    }
}
