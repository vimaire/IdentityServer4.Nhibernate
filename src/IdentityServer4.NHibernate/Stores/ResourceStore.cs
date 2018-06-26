using AutoMapper;
using IdentityServer4.Models;
using IdentityServer4.NHibernate.Automapper;
using IdentityServer4.Stores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.NHibernate.Stores
{
    public class ResourceStore : IResourceStore
    {
        private static readonly IMapper _mapper;

        static ResourceStore()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ResourceProfile>()).CreateMapper();
        }

        private readonly ConfigurationSessionProvider _sessionProvider;

        public ResourceStore(ConfigurationSessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return await _sessionProvider.Session.Query<Entities.IdentityResource>()
                .Where(x => scopeNames.Contains(x.Name))
                .Select(x => _mapper.Map<IdentityResource>(x))
                .ToListAsync();
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var resources = await (from resource in _sessionProvider.Session.Query<Entities.ApiResource>().FetchMany(x => x.Scopes)
                from scope in resource.Scopes
                where scopeNames.Contains(scope.Name)
            select resource).Distinct().ToListAsync();

            return _mapper.Map<IEnumerable<ApiResource>>(resources);
        }

        public async Task<ApiResource> FindApiResourceAsync(string name)
        {
            var resource = await _sessionProvider.Session.Query<Entities.ApiResource>().FirstOrDefaultAsync(x => x.Name == name);
            return resource != null
                ? _mapper.Map<ApiResource>(resource)
                : null;
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            var apiResources = await _sessionProvider.Session.Query<Entities.ApiResource>().FetchMany(x => x.Scopes)
                .Distinct()
                .ToListAsync();

            var identityResources = await _sessionProvider.Session.Query<Entities.IdentityResource>()
                .Select(x => _mapper.Map<IdentityResource>(x))
                .ToListAsync();

            return new Resources(identityResources, _mapper.Map<IEnumerable<ApiResource>>(apiResources));
        }
    }
}