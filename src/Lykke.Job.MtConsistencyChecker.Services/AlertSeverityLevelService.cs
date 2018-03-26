using Lykke.Job.MtConsistencyChecker.Core.Domain;
using Lykke.Job.MtConsistencyChecker.Core.Enums;
using Lykke.Job.MtConsistencyChecker.Core.Services;
using Lykke.Job.MtConsistencyChecker.Core.Settings;
using MoreLinq;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Lykke.Job.MtConsistencyChecker.Services
{
    public class AlertSeverityLevelService : IAlertSeverityLevelService
    {
        private readonly ReadOnlyCollection<(EventTypeEnum Event, string SlackChannelType)> _levels;
        
        public AlertSeverityLevelService(RiskInformingSettings settings)
        {
            _levels = settings.Data.Where(d => d.System == "ConsistencyMonitor")
                .Select(d => (ConvertEventTypeCode(d.EventTypeCode), ConvertLevel(d.Level)))
                .ToList()
                .AsReadOnly();
            
        }

        private static EventTypeEnum ConvertEventTypeCode(string eventTypeCode)
        {
            switch (eventTypeCode)
            {
                case "CM01": return EventTypeEnum.ConsistencyError;
                default:
                    throw new ArgumentOutOfRangeException(nameof(RiskInformingParams.EventTypeCode), eventTypeCode, null);
            }
        }

        private static string ConvertLevel(string alertSeverityLevel)
        {
            switch (alertSeverityLevel)
            {
                case "None":
                    return null;
                case "Information":
                    return Constants.SlackNotificationChannelInfo;
                case "Warning":
                    return Constants.SlackNotificationChannelWarning;
                default:
                    return Constants.SlackNotificationChannelCritical;
            }
        }

        public string GetSlackChannelType(EventTypeEnum eventType)
        {
            return _levels.Where(l => l.Event == eventType).Select(l => l.SlackChannelType)
                .FallbackIfEmpty(Constants.SlackNotificationChannelCritical).Single();
        }
    }
}
