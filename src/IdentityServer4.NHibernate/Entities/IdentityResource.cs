using System;
using System.Collections.Generic;

namespace IdentityServer4.NHibernate.Entities
{
    public class IdentityResource
    {
        public virtual Guid Id { get; set; }
        public virtual bool Enabled { get; set; } = true;
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
        public virtual bool Required { get; set; }
        public virtual bool Emphasize { get; set; }
        public virtual bool ShowInDiscoveryDocument { get; set; } = true;
        public virtual ICollection<string> UserClaims { get; set; } = new List<string>();
    }
}