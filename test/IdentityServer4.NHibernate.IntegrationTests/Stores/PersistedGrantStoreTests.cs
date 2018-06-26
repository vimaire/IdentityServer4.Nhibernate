using AutoMapper;
using IdentityServer4.Models;
using IdentityServer4.NHibernate.Automapper;
using IdentityServer4.NHibernate.Stores;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer4.NHibernate.IntegrationTests.Stores
{
    public class PersistedGrantStoreTests
    {
        private readonly IMapper _mapper;

        public PersistedGrantStoreTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<PersistedGrantProfile>()).CreateMapper();
        }

        [Theory, ClassData(typeof(OperationalSessionFactoriesGenerator))]
        public async Task StoreAsync_WhenPersistedGrantStored_ExpectSuccess(ISessionFactory sessionFactory)
        {
            var persistedGrant = CreateTestObject();
            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                var store = new PersistedGrantStore(provider);
                await store.StoreAsync(persistedGrant);
            }

            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                var foundGrant = await provider.Session.GetAsync<Entities.PersistedGrant>(persistedGrant.Key);
                Assert.NotNull(foundGrant);
            }
        }

        [Theory, ClassData(typeof(OperationalSessionFactoriesGenerator))]
        public async Task GetAsync_WithKeyAndPersistedGrantExists_ExpectPersistedGrantReturned(ISessionFactory sessionFactory)
        {
            var persistedGrant = CreateTestObject();

            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                await provider.Session.SaveAsync(_mapper.Map<Entities.PersistedGrant>(persistedGrant));
            }

            PersistedGrant foundPersistedGrant;
            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                var store = new PersistedGrantStore(provider);
                foundPersistedGrant = await store.GetAsync(persistedGrant.Key);
            }

            Assert.NotNull(foundPersistedGrant);
        }

        [Theory, ClassData(typeof(OperationalSessionFactoriesGenerator))]
        public async Task GetAsync_WithSubAndTypeAndPersistedGrantExists_ExpectPersistedGrantReturned(ISessionFactory sessionFactory)
        {
            var persistedGrant = CreateTestObject();

            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                await provider.Session.SaveAsync(_mapper.Map<Entities.PersistedGrant>(persistedGrant));
            }

            IEnumerable<PersistedGrant> foundPersistedGrants;

            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                var store = new PersistedGrantStore(provider);
                foundPersistedGrants = await store.GetAllAsync(persistedGrant.SubjectId);
            }

            Assert.NotNull(foundPersistedGrants);
            Assert.NotEmpty(foundPersistedGrants);
        }

        [Theory, ClassData(typeof(OperationalSessionFactoriesGenerator))]
        public async Task RemoveAsync_WhenKeyOfExistingReceived_ExpectGrantDeleted(ISessionFactory sessionFactory)
        {
            var persistedGrant = CreateTestObject();

            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                await provider.Session.SaveAsync(_mapper.Map<Entities.PersistedGrant>(persistedGrant));
            }

            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                var store = new PersistedGrantStore(provider);
                await store.RemoveAsync(persistedGrant.Key);
            }

            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                var foundGrant = await provider.Session.GetAsync<Entities.PersistedGrant>(persistedGrant.Key);
                Assert.Null(foundGrant);
            }
        }

        [Theory, ClassData(typeof(OperationalSessionFactoriesGenerator))]
        public async Task RemoveAsync_WhenSubIdAndClientIdOfExistingReceived_ExpectGrantDeleted(ISessionFactory sessionFactory)
        {
            var persistedGrant = CreateTestObject();

            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                await provider.Session.SaveAsync(_mapper.Map<Entities.PersistedGrant>(persistedGrant));
            }

            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                var store = new PersistedGrantStore(provider);
                await store.RemoveAllAsync(persistedGrant.SubjectId, persistedGrant.ClientId);
            }

            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                var foundGrant = await provider.Session.GetAsync<Entities.PersistedGrant>(persistedGrant.Key);
                Assert.Null(foundGrant);
            }
        }

        [Theory, ClassData(typeof(OperationalSessionFactoriesGenerator))]
        public async Task RemoveAsync_WhenSubIdClientIdAndTypeOfExistingReceived_ExpectGrantDeleted(ISessionFactory sessionFactory)
        {
            var persistedGrant = CreateTestObject();

            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                await provider.Session.SaveAsync(_mapper.Map<Entities.PersistedGrant>(persistedGrant));
            }

            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                var store = new PersistedGrantStore(provider);
                await store.RemoveAllAsync(persistedGrant.SubjectId, persistedGrant.ClientId, persistedGrant.Type);
            }

            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                var foundGrant = await provider.Session.GetAsync<Entities.PersistedGrant>(persistedGrant.Key);
                Assert.Null(foundGrant);
            }
        }

        [Theory, ClassData(typeof(OperationalSessionFactoriesGenerator))]
        public async Task Store_should_create_new_record_if_key_does_not_exist(ISessionFactory sessionFactory)
        {
            var persistedGrant = CreateTestObject();

            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                var foundGrant = await provider.Session.GetAsync<Entities.PersistedGrant>(persistedGrant.Key);
                Assert.Null(foundGrant);
            }

            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                var store = new PersistedGrantStore(provider);
                await store.StoreAsync(persistedGrant);
            }

            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                var foundGrant = await provider.Session.GetAsync<Entities.PersistedGrant>(persistedGrant.Key);
                Assert.NotNull(foundGrant);
            }
        }

        [Theory, ClassData(typeof(OperationalSessionFactoriesGenerator))]
        public async Task Store_should_update_record_if_key_already_exists(ISessionFactory sessionFactory)
        {
            var persistedGrant = CreateTestObject();

            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                await provider.Session.SaveAsync(_mapper.Map<Entities.PersistedGrant>(persistedGrant));
            }

            var newDate = persistedGrant.Expiration.Value.AddHours(1);
            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                var store = new PersistedGrantStore(provider);
                persistedGrant.Expiration = newDate;
                await store.StoreAsync(persistedGrant);
            }

            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                var foundGrant = await provider.Session.GetAsync<Entities.PersistedGrant>(persistedGrant.Key);
                Assert.NotNull(foundGrant);
                Assert.Equal(newDate, persistedGrant.Expiration);
            }
        }

        private static PersistedGrant CreateTestObject()
        {
            return new PersistedGrant
            {
                Key = Guid.NewGuid().ToString(),
                Type = "authorization_code",
                ClientId = Guid.NewGuid().ToString(),
                SubjectId = Guid.NewGuid().ToString(),
                CreationTime = new DateTime(2016, 08, 01).ToUniversalTime(),
                Expiration = new DateTime(2016, 08, 31).ToUniversalTime(),
                Data = Guid.NewGuid().ToString()
            };
        }
    }
}