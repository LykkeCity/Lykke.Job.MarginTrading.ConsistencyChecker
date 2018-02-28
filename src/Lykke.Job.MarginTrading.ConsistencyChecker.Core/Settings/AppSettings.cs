using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Settings.JobSettings;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Settings.SlackNotifications;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Core.Settings
{
    public class AppSettings
    {
        public ConsistencyCheckerSettings MtConsistencyCheckerJob { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public RiskInformingSettings RiskInformingSettings { get; set; }
    }
}
