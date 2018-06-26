using AutoMapper;
using IdentityServer4.Models;
using IdentityServer4.NHibernate.Automapper;
using IdentityServer4.Stores;
using NHibernate.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.NHibernate.Stores
{
    public class ClientStore : IClientStore
    {
        private static readonly IMapper _mapper;

        static ClientStore()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientProfile>()).CreateMapper();
        }

        private readonly ConfigurationSessionProvider _sessionProvider;

        public ClientStore(ConfigurationSessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = await _sessionProvider.Session
                .Query<Entities.Client>().FetchMany(x => x.AllowedCorsOrigins)
                .Where(x => x.ClientId == clientId)
                .OrderBy(x => x.ClientId)
                .Distinct()
                .FirstOrDefaultAsync();

            return client == null ? null : _mapper.Map<Client>(client);
        }
    }
}