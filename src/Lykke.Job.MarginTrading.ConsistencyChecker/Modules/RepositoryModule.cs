using Autofac;
using Common.Log;
using Lykke.Job.MarginTrading.ConsistencyChecker.AzureRepositories;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Repositories;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Settings.JobSettings;
using Lykke.Job.MarginTrading.ConsistencyChecker.Services;
using Lykke.SettingsReader;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Modules
{
    public class RepositoryModule: Module
    {
        private readonly IReloadingManager<DbSettings> _dbSettings;
        private readonly ILog _log;

        public RepositoryModule(IReloadingManager<DbSettings> dbSettings, ILog log)
        {
            _dbSettings = dbSettings;
            _log = log;
        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterInstance(new RepositoryManager(
                new AzureRepositories.AccountMarginEventReportRepository(_dbSettings.Nested(x => x.ReportsConnString), _log),
                new SqlRepositories.AccountMarginEventReportRepository(_dbSettings.Nested(x => x.ReportsSqlConnString).CurrentValue, _log),
                new AzureRepositories.AccountsReportRepository(_dbSettings.Nested(x => x.ReportsConnString), _log),
                new SqlRepositories.AccountsReportRepository(_dbSettings.Nested(x => x.ReportsSqlConnString).CurrentValue, _log),
                new AzureRepositories.AccountsStatReportRepository(_dbSettings.Nested(x => x.ReportsConnString), _log),
                new SqlRepositories.AccountsStatReportRepository(_dbSettings.Nested(x => x.ReportsSqlConnString).CurrentValue, _log),
                new AzureRepositories.AccountTransactionsReportRepository(_dbSettings.Nested(x => x.ReportsConnString), _log),
                new SqlRepositories.AccountTransactionsReportRepository(_dbSettings.Nested(x => x.ReportsSqlConnString).CurrentValue, _log),
                null,
                new SqlRepositories.TradingPositionRepository(_dbSettings.Nested(x => x.ReportsSqlConnString).CurrentValue, _log),
                new AzureRepositories.TradingOrderRepository(_dbSettings.Nested(x => x.ReportsSqlConnString), _log),
                new SqlRepositories.TradingOrderRepository(_dbSettings.Nested(x => x.ReportsSqlConnString).CurrentValue, _log)

                ))
                .As<IRepositoryManager>()
                .SingleInstance();

            builder.Register<ICheckResultRepository>(ctx => new CheckResultRepository(_dbSettings.Nested(x => x.CheckResultsConnString), _log))
                .SingleInstance();
            builder.Register<IBalanceAndTransactionAmountRepository>(ctx => new BalanceAndTransactionAmountRepository(_dbSettings.Nested(x => x.CheckResultsConnString), _log))
                .SingleInstance();
            builder.Register<IBalanceAndOrderClosedRepository>(ctx => new BalanceAndOrderClosedRepository(_dbSettings.Nested(x => x.CheckResultsConnString), _log))
                .SingleInstance();
            builder.Register<IOrdersReportAndOrderClosedOpenedRepository>(ctx => new OrdersReportAndOrderClosedOpenedRepository(_dbSettings.Nested(x => x.CheckResultsConnString), _log))
                .SingleInstance();
            builder.Register<IPriceCandlesConsistencyRepository>(ctx => new PriceCandlesConsistencyRepository(_dbSettings.Nested(x => x.CheckResultsConnString), _log))
                .SingleInstance();
            builder.Register<IMarginEventsAccountStatusRepository>(ctx => new MarginEventsAccountStatusRepository(_dbSettings.Nested(x => x.CheckResultsConnString), _log))
                .SingleInstance();
            builder.Register<IHedgingServiceRepository>(ctx => new HedgingServiceRepository(_dbSettings.Nested(x => x.CheckResultsConnString), _log))
                .SingleInstance();

            


        }
    }
}
