namespace Lykke.Job.MtConsistencyChecker.Core.Settings.JobSettings
{
    public class DbSettings
    {
        public string LogsConnString { get; set; }
        public string ReportsConnString { get; set; }
        public string ReportsSqlConnString { get; set; }
        public string CheckResultsConnString { get; set; }
    }
}
