using IdentityServer4.NHibernate.Entities;
using IdentityServer4.Services;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.NHibernate.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.NHibernate.Services
{
    public class CorsPolicyService : ICorsPolicyService
    {
        private readonly IHttpContextAccessor _context;

        public CorsPolicyService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            // doing this here and not in the ctor because: https://github.com/aspnet/CORS/issues/105
            var sessionProvider = _context.HttpContext.RequestServices.GetRequiredService<ConfigurationSessionProvider>();
            return sessionProvider.Session.Query<Client>()
                .SelectMany(x => x.AllowedCorsOrigins)
                .AnyAsync(x => x.ToUpperInvariant() == origin.ToUpperInvariant());
        }
    }
}