using AutoMapper;
using IdentityServer4.Models;
using IdentityServer4.NHibernate.Automapper;
using IdentityServer4.NHibernate.Stores;
using NHibernate;
using System;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer4.NHibernate.IntegrationTests.Stores
{
    public class ClientStoreTests
    {
        private readonly IMapper _mapper;

        public ClientStoreTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientProfile>()).CreateMapper();
        }

        [Theory, ClassData(typeof(ConfigurationSessionFactoriesGenerator))]
        public async Task FindClientByIdAsync_WhenClientExists_ExpectClientRetured(ISessionFactory sessionFactory)
        {
            var testClient = CreateTestClient();

            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                await provider.Session.SaveAsync(_mapper.Map<Entities.Client>(testClient));
            }

            Client client;
            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                var store = new ClientStore(provider);
                client = await store.FindClientByIdAsync(testClient.ClientId);
            }

            Assert.NotNull(client);
        }

        private static Client CreateTestClient()
        {
            return new Client
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = Guid.NewGuid().ToString()
            };
        }
    }
}