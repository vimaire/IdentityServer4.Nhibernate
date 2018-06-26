using IdentityServer4.NHibernate.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System.Collections.Generic;

namespace IdentityServer4.NHibernate.Mappings
{
    public class IdentityResourceMapper : ClassMapping<IdentityResource>
    {
        public IdentityResourceMapper()
        {
            Table(DbNames.Tables.IdentityResources);
            if (!string.IsNullOrWhiteSpace(DbNames.ConfigurationSchema))
            {
                Schema(DbNames.ConfigurationSchema);
            }
            DynamicUpdate(true);
            Id(x => x.Id, map => map.Generator(Generators.GuidComb));

            Property(x => x.Enabled);
            Property(x => x.Name, pm =>
            {
                pm.Index($"IX_{DbNames.Tables.IdentityResources}_Name");
                pm.Unique(true);
                pm.UniqueKey($"UQ{DbNames.Tables.IdentityResources}_Name");
            });
            Property(x => x.DisplayName);
            Property(x => x.Description);
            Property(x => x.Required);
            Property(x => x.Emphasize);
            Property(x => x.ShowInDiscoveryDocument);
            Property(x => x.UserClaims, pm => pm.Type<JsonColumnType<List<string>>>());
        }
    }
}