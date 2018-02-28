﻿using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Settings;
using Lykke.SettingsReader;
using MoreLinq;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Services
{
    public class AlertSeverityLevelService : IAlertSeverityLevelService
    {
        private readonly IReloadingManager<ReadOnlyCollection<(EventTypeEnum Event, string SlackChannelType)>> _levels;

        private static readonly string _defaultLevel = "mt-critical";

        public AlertSeverityLevelService(IReloadingManager<RiskInformingSettings> settings)
        {
            _levels = settings.Nested(s =>
            {
                return s.Data.Where(d => d.System == "ConsistencyMonitor")
                    .Select(d => (ConvertEventTypeCode(d.EventTypeCode), ConvertLevel(d.Level)))
                    .ToList().AsReadOnly();
            });
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
                    return "mt-information";
                case "Warning":
                    return "mt-warning";
                default:
                    return _defaultLevel;
            }
        }

        public string GetSlackChannelType(EventTypeEnum eventType)
        {
            return _levels.CurrentValue.Where(l => l.Event == eventType).Select(l => l.SlackChannelType)
                .FallbackIfEmpty(_defaultLevel).Single();
        }
    }
}