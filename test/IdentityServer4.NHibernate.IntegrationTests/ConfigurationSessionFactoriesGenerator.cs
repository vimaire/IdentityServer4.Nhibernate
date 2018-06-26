using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Dialect;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Xunit;

namespace IdentityServer4.NHibernate.IntegrationTests
{
    public class ConfigurationSessionFactoriesGenerator : TheoryData<ISessionFactory>
    {
        private static readonly IEnumerable<ISessionFactory> _sessionFactories;

        static ConfigurationSessionFactoriesGenerator()
        {
            _sessionFactories = new List<ISessionFactory>
            {
                BuildPostgresSessionFactory(),
                BuildMySqlSessionFactory(),
                BuildMsSqlSessionFactory(),
                BuildSqliteSessionFactory()
            };
        }

        public ConfigurationSessionFactoriesGenerator()
        {
            foreach (var sessionFactory in _sessionFactories)
            {
                Add(sessionFactory);
            }
        }

        private static ISessionFactory BuildPostgresSessionFactory()
        {
            var cfg = BuildConfiguration(db =>
            {
                db.ConnectionString = "Server=192.168.99.100;Database=test;User Id=test;Password=test;";
                db.Dialect<PostgreSQL83Dialect>();
            });

            return cfg.BuildSessionFactory();
        }

        private static ISessionFactory BuildSqliteSessionFactory()
        {
            if (File.Exists("./configuration.db"))
            {
                File.Delete("./configuration.db");
            }
            var cfg = BuildConfiguration(db =>
            {
                db.ConnectionString = $"Data Source=./configuration.db;Version=3;New=True;BinaryGUID=False;";
                db.Dialect<SQLiteDialect>();
                db.KeywordsAutoImport = Hbm2DDLKeyWords.None;
            });

            var sessionFactory = cfg.BuildSessionFactory();
            
            return sessionFactory;
        }

        private static ISessionFactory BuildMySqlSessionFactory()
        {
            var cfg = BuildConfiguration(db =>
            {
                db.ConnectionString = $"Server=192.168.99.100;Database=test;Uid=test;Pwd=test;";
                db.Dialect<MySQL57Dialect>();
            });

            cfg.SetProperty("hbm2ddl.delimiter", ";");

            return cfg.BuildSessionFactory();
        }

        private static ISessionFactory BuildMsSqlSessionFactory()
        {
            var cfg = BuildConfiguration(db =>
            {
                db.ConnectionString = $"Server=192.168.99.100;Database=test;User Id=test;Password=test;";
                db.Dialect<MsSql2012Dialect>();
            });

            return cfg.BuildSessionFactory();
        }

        private static global::NHibernate.Cfg.Configuration BuildConfiguration(Action<IDbIntegrationConfigurationProperties> configureAction)
        {
            var cfg = new global::NHibernate.Cfg.Configuration();
            cfg.DataBaseIntegration(db =>
            {
                configureAction(db);
                db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                db.IsolationLevel = IsolationLevel.ReadCommitted;
            });

            cfg.SetProperty("hbm2ddl.auto", "create");

            return cfg.AddConfigurationMappings();
        }
    }
}