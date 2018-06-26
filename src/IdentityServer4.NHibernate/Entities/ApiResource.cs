using System;
using System.Collections.Generic;

namespace IdentityServer4.NHibernate.Entities
{
    public class ApiResource
    {
        public virtual Guid Id { get; set; }
        public virtual bool Enabled { get; set; } = true;
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
        public virtual ICollection<Secret> ApiSecrets { get; set; } = new List<Secret>();
        public virtual ICollection<ApiResourceScope> Scopes { get; set; } = new List<ApiResourceScope>();
        public virtual ICollection<string> UserClaims { get; set; } = new List<string>();
    }
}