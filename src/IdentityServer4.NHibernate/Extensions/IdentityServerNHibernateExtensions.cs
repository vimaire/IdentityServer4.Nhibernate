using IdentityServer4.NHibernate;
using IdentityServer4.NHibernate.Options;
using IdentityServer4.NHibernate.Services;
using IdentityServer4.NHibernate.Stores;
using IdentityServer4.Stores;
using Microsoft.Extensions.Hosting;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using System;
using System.Data;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerNHibernateExtensions
    {
        private static readonly Action<IDbIntegrationConfigurationProperties> _NHibernateDbConfiguration = db =>
        {
            db.KeywordsAutoImport = Hbm2DDLKeyWords.Keywords;
            db.IsolationLevel = IsolationLevel.ReadCommitted;
        };

        public static IIdentityServerBuilder AddConfigurationStore(this IIdentityServerBuilder builder,
            Action<ConfigurationStoreOptions> configureDbAction)
        {
            var options = new ConfigurationStoreOptions();

            configureDbAction?.Invoke(options);

            var sessionFactory = options.SessionFactory ?? BuildSessionFactory(options, NhibernateConfigurationExtensions.AddConfigurationMappings);

            builder.Services.AddSingleton(options);

            builder.Services.AddScoped(_ => new ConfigurationSessionProvider(sessionFactory.OpenSession));

            builder.AddClientStore<ClientStore>();
            builder.AddResourceStore<ResourceStore>();
            builder.AddCorsPolicyService<CorsPolicyService>();

            return builder;
        }

        public static IIdentityServerBuilder AddOperationalStore(this IIdentityServerBuilder builder,
            Action<OperationalStoreOptions> configureAction)
        {
            var options = new OperationalStoreOptions();

            configureAction?.Invoke(options);

            var sessionFactory = options.SessionFactory ?? BuildSessionFactory(options, NhibernateConfigurationExtensions.AddConfigurationMappings);

            builder.Services.AddSingleton(options);
            builder.Services.AddSingleton<TokenCleanup>();
            builder.Services.AddSingleton<IHostedService, TokenCleanupHost>();

            builder.Services.AddScoped(_ => new OperationalSessionProvider(sessionFactory.OpenSession));
            builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            return builder;
        }

        private static ISessionFactory BuildSessionFactory(NHibernateOptions options,
            Func<NHibernate.Cfg.Configuration, NHibernate.Cfg.Configuration> setMappingsMethod)
        {
            var cfg = new NHibernate.Cfg.Configuration();

            cfg.DataBaseIntegration(db =>
            {
                _NHibernateDbConfiguration(db);
                options.SetupDbIntegration?.Invoke(db);
            });

            if (!options.DontSetMappings)
            {
                setMappingsMethod(cfg);
            }

            options.SetupConfiguration?.Invoke(cfg);

            return cfg.BuildSessionFactory();
        }
    }
}