using Lykke.Job.MtConsistencyChecker.Contract;
using System;

namespace Lykke.Job.MtConsistencyChecker.SqlRepositories.Entities
{
    internal class AccountsReportEntity : IAccountsReport
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string TakerCounterpartyId { get; set; }
        public string TakerAccountId { get; set; }
        public string BaseAssetId { get; set; }
        public bool IsLive { get; set; }
    }
}
