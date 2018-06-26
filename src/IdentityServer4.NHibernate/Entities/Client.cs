using System;
using IdentityServer4.Models;
using System.Collections.Generic;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityServer4.NHibernate.Entities
{
    public class Client
    {
        public virtual Guid Id { get; set; }
        public virtual bool Enabled { get; set; } = true;
        public virtual string ClientId { get; set; }
        public virtual string ProtocolType { get; set; } = ProtocolTypes.OpenIdConnect;
        public virtual IEnumerable<Secret> ClientSecrets { get; set; } = new List<Secret>();
        public virtual bool RequireClientSecret { get; set; } = true;
        public virtual string ClientName { get; set; }
        public virtual string ClientUri { get; set; }
        public virtual string LogoUri { get; set; }
        public virtual string Description { get; set; }
        public virtual bool RequireConsent { get; set; } = true;
        public virtual bool AllowRememberConsent { get; set; } = true;
        public virtual bool AlwaysIncludeUserClaimsInIdToken { get; set; }
        public virtual ICollection<string> AllowedGrantTypes { get; set; } = new List<string>();
        public virtual bool RequirePkce { get; set; }
        public virtual bool AllowPlainTextPkce { get; set; }
        public virtual bool AllowAccessTokensViaBrowser { get; set; }
        public virtual ICollection<string> RedirectUris { get; set; } = new List<string>();
        public virtual ICollection<string> PostLogoutRedirectUris { get; set; } = new List<string>();
        public virtual bool FrontChannelLogoutSessionRequired { get; set; } = true;
        public virtual string FrontChannelLogoutUri { get; set; }
        public virtual bool BackChannelLogoutSessionRequired { get; set; } = true;
        public virtual string BackChannelLogoutUri { get; set; }
        public virtual bool AllowOfflineAccess { get; set; }
        public virtual ICollection<string> AllowedScopes { get; set; } = new List<string>();
        public virtual int IdentityTokenLifetime { get; set; } = 300;
        public virtual int AccessTokenLifetime { get; set; } = 3600;
        public virtual int AuthorizationCodeLifetime { get; set; } = 300;
        public virtual int? ConsentLifetime { get; set; } = null;
        public virtual int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;
        public virtual int SlidingRefreshTokenLifetime { get; set; } = 1296000;
        public virtual int RefreshTokenUsage { get; set; } = (int)TokenUsage.OneTimeOnly;
        public virtual bool UpdateAccessTokenClaimsOnRefresh { get; set; }
        public virtual int RefreshTokenExpiration { get; set; } = (int)TokenExpiration.Absolute;
        public virtual int AccessTokenType { get; set; } = (int)0; // AccessTokenType.Jwt;
        public virtual bool EnableLocalLogin { get; set; } = true;
        public virtual ICollection<string> IdentityProviderRestrictions { get; set; } 
        public virtual bool IncludeJwtId { get; set; }
        public virtual IDictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
        public virtual bool AlwaysSendClientClaims { get; set; }
        public virtual string ClientClaimsPrefix { get; set; } = "client_";
        public virtual string PairWiseSubjectSalt { get; set; }
        public virtual ICollection<string> AllowedCorsOrigins { get; set; } = new List<string>();
        public virtual IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}