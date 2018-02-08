using Autofac;
using Common.Log;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Settings.JobSettings;
using Lykke.Job.MarginTrading.ConsistencyChecker.Services;
using Lykke.SettingsReader;
using System;
using System.Collections.Generic;
using System.Text;

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
                new SqlRepositories.AccountMarginEventReportRepository(_dbSettings.Nested(x => x.ReportsConnString).CurrentValue, _log)
                ))
                .As<IRepositoryManager>()
                .SingleInstance();
        }
    }
}
