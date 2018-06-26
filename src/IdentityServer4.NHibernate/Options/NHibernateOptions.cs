using NHibernate.Cfg.Loquacious;
using System;
using NHibernate;

namespace IdentityServer4.NHibernate.Options
{
    public class NHibernateOptions
    {
        /// <summary>
        /// Add user configuration to the <see cref="ConfigurationExtensions.DataBaseIntegration"/>.
        /// The minimum required are ConnectionString and Dialect.
        /// The <see cref="SetupConfiguration"/> action can be used to set theese too.
        /// Some default values are provided:
        /// KeywordsAutoImport=<value>Hbm2DDLKeyWords.Keywords</value>
        /// IsolationLevel=<value>IsolationLevel.ReadCommitted</value>)
        /// </summary>
        public Action<IDbIntegrationConfigurationProperties> SetupDbIntegration { get; set; }

        /// <summary>
        /// Extention to set optionnal configuration.
        /// For instance, to automatically generate the schema, the following properties must be set:
        /// SetProperty("ddl2hbm.auto", "update")
        /// and depending of the database used, SetProperty("ddl2hbm.delmiter", ";")
        /// </summary>
        public Action<global::NHibernate.Cfg.Configuration> SetupConfiguration { get; set; }

        /// <summary>
        /// Do not set the default mappings.
        /// If <value>false</value>, the mappings must be supplied in <see cref="SetupConfiguration"/>  method.
        /// </summary>
        public bool DontSetMappings { get; set; }

        /// <summary>
        /// Use the <see cref="ISessionFactory"/> provided instead of building one.
        /// Setting up a value in this property will disable SessionFactory building.
        /// </summary>
        public ISessionFactory SessionFactory { get; set; }
    }
}