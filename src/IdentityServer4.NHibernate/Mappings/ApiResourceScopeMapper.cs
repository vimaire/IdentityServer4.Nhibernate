using IdentityServer4.NHibernate.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System.Collections.Generic;

namespace IdentityServer4.NHibernate.Mappings
{
    public class ApiResourceScopeMapper : ClassMapping<ApiResourceScope>
    {
        public ApiResourceScopeMapper()
        {
            Table(DbNames.Tables.ApiResourceScopes);
            if (!string.IsNullOrWhiteSpace(DbNames.ConfigurationSchema))
            {
                Schema(DbNames.ConfigurationSchema);
            }
            Id(x => x.Id, map => map.Generator(Generators.GuidComb));
            Property(x => x.Name);
            Property(x => x.Description);
            Property(x => x.DisplayName);
            Property(x => x.Emphasize);
            Property(x => x.Required);
            Property(x => x.ShowInDiscoveryDocument);
            Property(x => x.UserClaims, pm => pm.Type<JsonColumnType<List<string>>>());
        }
    }
}