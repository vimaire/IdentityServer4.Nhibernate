using IdentityServer4.NHibernate.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System.Collections.Generic;

namespace IdentityServer4.NHibernate.Mappings
{
    public class ApiResourceMapper : ClassMapping<ApiResource>
    {
        public ApiResourceMapper()
        {
            Table(DbNames.Tables.ApiResources);
            if (!string.IsNullOrWhiteSpace(DbNames.ConfigurationSchema))
            {
                Schema(DbNames.ConfigurationSchema);
            }
            Id(x => x.Id, map => map.Generator(Generators.GuidComb));

            Property(x => x.Enabled);
            Property(x => x.Name, pm =>
            {
                pm.Index($"IX_{DbNames.Tables.ApiResources}_Name");
                pm.Unique(true);
                pm.UniqueKey($"UQ_{DbNames.Tables.ApiResources}_Name");

            });
            Property(x => x.DisplayName);
            Property(x => x.Description);
            Property(x => x.ApiSecrets, pm => pm.Type<JsonColumnType<List<Secret>>>());
            Property(x => x.UserClaims, pm => pm.Type<JsonColumnType<List<string>>>());

            Bag(x => x.Scopes,
                cm =>
                {
                    cm.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                    cm.Key(km =>
                    {
                        km.Column(col =>
                        {
                            col.Name($"{DbNames.Columns.ApiResourceFk}");
                            col.Index($"IX_{DbNames.Tables.ApiResourceScopes}_{DbNames.Columns.ApiResourceFk}");
                        });
                        km.ForeignKey($"FK_{DbNames.Tables.ApiResourceScopes}_{DbNames.Tables.ApiResources}");
                        km.NotNullable(true);
                        km.Update(false);
                    });
                },
                x => x.OneToMany());
        }
    }
}