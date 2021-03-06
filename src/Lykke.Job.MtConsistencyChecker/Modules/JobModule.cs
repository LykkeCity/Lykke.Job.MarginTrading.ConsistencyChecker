﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lykke.Job.MtConsistencyChecker.Core.Services;
using Lykke.Job.MtConsistencyChecker.Core.Settings;
using Lykke.Job.MtConsistencyChecker.Core.Settings.JobSettings;
using Lykke.Job.MtConsistencyChecker.PeriodicalHandlers;
using Lykke.Job.MtConsistencyChecker.Services;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Job.MtConsistencyChecker.Modules
{
    public class JobModule : Module
    {
        private readonly ConsistencyCheckerSettings _settings;
        private readonly IReloadingManager<RiskInformingSettings> _riskInformingSettingsManager;
        private readonly IServiceCollection _services;

        public JobModule(ConsistencyCheckerSettings settings,  IReloadingManager<RiskInformingSettings> riskInformingSettingsManager)
        {
            _settings = settings;
            _riskInformingSettingsManager = riskInformingSettingsManager;
            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // NOTE: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            // builder.RegisterType<QuotesPublisher>()
            //  .As<IQuotesPublisher>()
            //  .WithParameter(TypedParameter.From(_settings.Rabbit.ConnectionString))

            builder.RegisterType<AlertSeverityLevelService>()
               .As<IAlertSeverityLevelService>()
               .WithParameter(TypedParameter.From(_riskInformingSettingsManager.CurrentValue))
               .SingleInstance();
            
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();
            RegisterPeriodicalHandlers(builder);
                       
            builder.RegisterType<PriceCandlesService>()
                .As<IPriceCandlesService>()
                .WithParameter(TypedParameter.From(_settings.PriceCandles));

            builder.RegisterType<ConsistencyService>()
                .As<IConsistencyService>();

            builder.RegisterType<ConsistencyMonitor >()
                .As<IMonitorService>()
                .WithParameter(TypedParameter.From(_settings.Monitor));

            builder.Populate(_services);
        }

        private void RegisterPeriodicalHandlers(ContainerBuilder builder)
        {
            builder.RegisterType<ConsistencyCheckHandler>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance();
        }

    }
}
