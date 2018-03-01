namespace Lykke.Job.MtConsistencyChecker.Core.Settings.JobSettings
{
    public class ConsistencyCheckerSettings
    {
        public DbSettings Db { get; set; }
        public MonitorSettings Monitor { get; set; }
        public PriceCandlesSettings PriceCandles { get; set; }
    }
}
