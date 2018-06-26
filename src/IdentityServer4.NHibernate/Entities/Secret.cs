using System;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityServer4.NHibernate.Entities
{
    public class Secret
    {
        public virtual string Description { get; set; }
        public virtual string Value { get; set; }
        public virtual DateTime? Expiration { get; set; }
        public virtual string Type { get; set; } = SecretTypes.SharedSecret;
    }
}