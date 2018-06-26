using IdentityServer4.NHibernate.Mappings;

namespace IdentityServer4.NHibernate.Options
{
    public class ConfigurationStoreOptions : NHibernateOptions
    {
        public string Schema
        {
            get => DbNames.ConfigurationSchema;
            set => DbNames.ConfigurationSchema = value;
        }
        
    }
}