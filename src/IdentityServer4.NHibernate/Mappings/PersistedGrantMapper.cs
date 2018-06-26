using IdentityServer4.NHibernate.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace IdentityServer4.NHibernate.Mappings
{
    public class PersistedGrantMapper : ClassMapping<PersistedGrant>
    {
        public PersistedGrantMapper()
        {
            Table(DbNames.Tables.PersistedGrants);
            if (!string.IsNullOrWhiteSpace(DbNames.ConfigurationSchema))
            {
                Schema(DbNames.ConfigurationSchema);
            }
            Id(x => x.Key, map => map.Generator(Generators.Assigned));

            Property(x => x.Type);
            Property(x => x.SubjectId);
            Property(x => x.ClientId);
            Property(x => x.CreationTime, pm => pm.Type<UtcDateTimeType>());
            Property(x => x.Expiration, pm => pm.Type<UtcDateTimeType>());
            Property(x => x.Data);
        }
    }
}