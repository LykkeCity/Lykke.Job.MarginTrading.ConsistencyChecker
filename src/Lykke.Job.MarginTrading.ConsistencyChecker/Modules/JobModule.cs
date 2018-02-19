using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Settings.JobSettings;
using Lykke.Job.MarginTrading.ConsistencyChecker.Services;
using Lykke.SettingsReader;
using Lykke.Job.MarginTrading.ConsistencyChecker.PeriodicalHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Modules
{
    public class JobModule : Module
    {
        private readonly ConsistencyCheckerSettings _settings;
        //private readonly IReloadingManager<DbSettings> _dbSettingsManager;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public JobModule(ConsistencyCheckerSettings settings, IReloadingManager<DbSettings> dbSettingsManager, ILog log)
        {
            _settings = settings;
            _log = log;
            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // NOTE: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            // builder.RegisterType<QuotesPublisher>()
            //  .As<IQuotesPublisher>()
            //  .WithParameter(TypedParameter.From(_settings.Rabbit.ConnectionString))


            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();
            RegisterPeriodicalHandlers(builder);

            builder.RegisterType<ConsistencyService>()
                .As<IConsistencyService>();

            builder.RegisterType<MonitorService>()
                .As<IMonitorService>()
                .WithParameter(TypedParameter.From(_settings.Monitor));

            builder.Populate(_services);
        }

        private void RegisterPeriodicalHandlers(ContainerBuilder builder)
        {
            // TODO: You should register each periodical handler in DI container as IStartable singleton and autoactivate it

            builder.RegisterType<ConsistencyCheckHandler>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance();
        }

    }
}
