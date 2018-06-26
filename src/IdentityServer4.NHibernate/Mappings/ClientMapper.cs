using System.Collections.Generic;
using IdentityServer4.NHibernate.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace IdentityServer4.NHibernate.Mappings
{
    public class ClientMapper : ClassMapping<Client>
    {
        public ClientMapper()
        {
            Table(DbNames.Tables.Clients);
            if (!string.IsNullOrWhiteSpace(DbNames.ConfigurationSchema))
            {
                Schema(DbNames.ConfigurationSchema);
            }
            DynamicUpdate(true);
            Id(x => x.Id, map => map.Generator(Generators.GuidComb));

            Property(x => x.Enabled);
            Property(x => x.ClientId, pm =>
            {
                pm.Index($"IX_{DbNames.Tables.Clients}_ClientId");
                pm.Unique(true);
                pm.UniqueKey($"UQ{DbNames.Tables.Clients}_ClientId");
            });
            Property(x => x.ProtocolType);
            Property(x => x.RequireClientSecret);
            Property(x => x.ClientName);
            Property(x => x.ClientUri);
            Property(x => x.LogoUri);
            Property(x => x.Description);
            Property(x => x.RequireConsent);
            Property(x => x.AllowRememberConsent);
            Property(x => x.AlwaysIncludeUserClaimsInIdToken);
            Property(x => x.RequirePkce);
            Property(x => x.AllowPlainTextPkce);
            Property(x => x.AllowAccessTokensViaBrowser);
            Property(x => x.FrontChannelLogoutSessionRequired);
            Property(x => x.FrontChannelLogoutUri);
            Property(x => x.BackChannelLogoutSessionRequired);
            Property(x => x.BackChannelLogoutUri);
            Property(x => x.AllowOfflineAccess);
            Property(x => x.IdentityTokenLifetime);
            Property(x => x.AccessTokenLifetime);
            Property(x => x.AuthorizationCodeLifetime);
            Property(x => x.ConsentLifetime);
            Property(x => x.AbsoluteRefreshTokenLifetime);
            Property(x => x.SlidingRefreshTokenLifetime);
            Property(x => x.RefreshTokenUsage);
            Property(x => x.UpdateAccessTokenClaimsOnRefresh);
            Property(x => x.RefreshTokenExpiration);
            Property(x => x.AccessTokenType);
            Property(x => x.EnableLocalLogin);
            Property(x => x.IncludeJwtId);
            Property(x => x.AlwaysSendClientClaims);
            Property(x => x.ClientClaimsPrefix);
            Property(x => x.PairWiseSubjectSalt);

            Property(x => x.ClientSecrets, pm => pm.Type<JsonColumnType<List<Secret>>>());
            Property(x => x.AllowedGrantTypes, pm => pm.Type<JsonColumnType<List<string>>>());
            Property(x => x.RedirectUris, pm => pm.Type<JsonColumnType<List<string>>>());
            Property(x => x.PostLogoutRedirectUris, pm => pm.Type<JsonColumnType<List<string>>>());
            Property(x => x.AllowedScopes, pm => pm.Type<JsonColumnType<List<string>>>());
            Property(x => x.IdentityProviderRestrictions, pm => pm.Type<JsonColumnType<List<string>>>());
            Property(x => x.Claims, pm => pm.Type<JsonColumnType<Dictionary<string, string>>>());
            Property(x => x.Properties, pm => pm.Type<JsonColumnType<Dictionary<string, string>>>());

            
            Bag(x => x.AllowedCorsOrigins, 
                cm =>
            {
                cm.Key(km =>
                {
                    km.Column(col =>
                    {
                        col.Name($"{DbNames.Columns.ClientFk}");
                        col.Index($"IX_{DbNames.Tables.ClientAllowedCorsOrigins}_{DbNames.Columns.ClientFk}");
                    });
                    km.ForeignKey($"FK_{DbNames.Tables.ClientAllowedCorsOrigins}_{DbNames.Tables.Clients}");
                    km.NotNullable(true);
                    km.Update(false);
                });
            }, 
                rel => rel.Element(em => em.Column(DbNames.Columns.Value)));
        }
    }
}