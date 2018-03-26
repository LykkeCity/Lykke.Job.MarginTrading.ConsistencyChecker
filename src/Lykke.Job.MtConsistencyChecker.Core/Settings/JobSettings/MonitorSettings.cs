namespace Lykke.Job.MtConsistencyChecker.Core.Settings.JobSettings
{
    public class MonitorSettings
    {
        /// <summary>
        /// Use SQL Server connection for consistency check
        /// </summary>
        public bool CheckSql { get; set; }
        /// <summary>
        /// Consistency Check Interval in milliseconds 
        /// </summary>
        public int ConsistencyCheckInterval { get; set; }
    }
}
