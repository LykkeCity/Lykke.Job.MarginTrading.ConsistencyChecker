using Lykke.Job.MtConsistencyChecker.Core.Settings.JobSettings;
using Lykke.Job.MtConsistencyChecker.Core.Settings.SlackNotifications;

namespace Lykke.Job.MtConsistencyChecker.Core.Settings
{
    public class AppSettings
    {
        public ConsistencyCheckerSettings MtConsistencyCheckerJob { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public RiskInformingSettings RiskInformingSettings { get; set; }
    }
}
