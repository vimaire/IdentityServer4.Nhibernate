using IdentityServer4.NHibernate.Mappings;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;

namespace NHibernate.Cfg
{
    public static class NhibernateConfigurationExtensions
    {
        public static Configuration AddConfigurationMappings(this Configuration configuration)
        {
            configuration.AddMapping(GetMappingFor(typeof(ClientMapper),
                typeof(ApiResourceMapper),
                typeof(ApiResourceScopeMapper),
                typeof(IdentityResourceMapper)
            ));

            return configuration;
        }

        public static Configuration AddOperationalMappings(this Configuration configuration)
        {
            configuration.AddMapping(GetMappingFor(typeof(PersistedGrantMapper)));
            return configuration;
        }

        private static HbmMapping GetMappingFor(params System.Type[] types)
        {
            var mapper = new ModelMapper();
            mapper.AddMappings(types);
            return mapper.CompileMappingForAllExplicitlyAddedEntities();
        }
    }
}