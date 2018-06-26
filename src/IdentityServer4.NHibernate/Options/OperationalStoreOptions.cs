using System;
using IdentityServer4.NHibernate.Mappings;

namespace IdentityServer4.NHibernate.Options
{
    public class OperationalStoreOptions : NHibernateOptions
    {
        /// <summary>
        /// The database schema used by the mappings
        /// </summary>
        public string Schema
        {
            get => DbNames.OperationalSchema;
            set => DbNames.OperationalSchema = value;
        }

        public bool EnableTokenCleanup { get; set; }

        public TimeSpan TokenCleanupInterval { get; set; } = TimeSpan.FromSeconds(30);
    }
}