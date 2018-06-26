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
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private static readonly IMapper _mapper;

        static PersistedGrantStore()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<PersistedGrantProfile>()).CreateMapper();
        }

        private readonly OperationalSessionProvider _sessionProvider;

        public PersistedGrantStore(OperationalSessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            var existing = await _sessionProvider.Session.GetAsync<Entities.PersistedGrant>(grant.Key);
            if (existing == null)
            {
                existing = _mapper.Map<Entities.PersistedGrant>(grant);
                await _sessionProvider.Session.SaveAsync(existing);
            }
            else
            {
                _mapper.Map(grant, existing);
            }
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            return await _sessionProvider.Session.Query<Entities.PersistedGrant>()
                .Where(x => x.Key == key)
                .Select(x => _mapper.Map<PersistedGrant>(x))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            return await _sessionProvider.Session.Query<Entities.PersistedGrant>()
                .Where(x => x.SubjectId == subjectId)
                .Select(x => _mapper.Map<PersistedGrant>(x))
                .ToListAsync();
        }

        public async Task RemoveAsync(string key)
        {
            await _sessionProvider.Session.CreateQuery("delete from PersistedGrant g where g.Key = :key")
                .SetParameter("key", key)
                .ExecuteUpdateAsync();
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            await _sessionProvider.Session
                .CreateQuery("delete from PersistedGrant g where g.SubjectId = :subjectId and g.ClientId = :clientId")
                .SetParameter("subjectId", subjectId)
                .SetParameter("clientId", clientId)
                .ExecuteUpdateAsync();
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            await _sessionProvider.Session.CreateQuery("delete from PersistedGrant").ExecuteUpdateAsync();
        }
    }
}