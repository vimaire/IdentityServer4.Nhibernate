using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.NHibernate.Entities;
using IdentityServer4.NHibernate.Stores;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using Xunit;

namespace IdentityServer4.NHibernate.IntegrationTests
{
    public class TokenCleanupTests
    {
        [Theory(Skip = "Fail other tests")]
        [ClassData(typeof(OperationalSessionFactoriesGenerator))]
        public async Task ClearTokens_OnExpiredTokens_ExpectNone(ISessionFactory sessionFactory)
        {
            var svcs = new ServiceCollection();
            svcs.AddScoped(_ => new OperationalSessionProvider(sessionFactory.OpenSession));

            var tokenCleanup = new TokenCleanup(svcs.BuildServiceProvider());

            var baseDate = DateTime.UtcNow;

            // create test data
            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                var grant = new PersistedGrant
                {
                    ClientId = Guid.NewGuid().ToString(),
                    Key = Guid.NewGuid().ToString(),
                    Type = Guid.NewGuid().ToString(),
                    Data = Guid.NewGuid().ToString(),
                    SubjectId = Guid.NewGuid().ToString(),
                    CreationTime = baseDate.AddDays(-2),
                    Expiration = baseDate.AddMinutes(-1)
                };
                await provider.Session.SaveAsync(grant);
            }

            // Clear tokens
            await tokenCleanup.ClearTokens(baseDate);

            // assert no data exists
            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                var exists = provider.Session
                    .Query<PersistedGrant>()
                    .Any(x => x.Expiration >= baseDate);

                Assert.False(exists);
            }
        }

        [Theory(Skip = "Fail other tests")]
        [ClassData(typeof(OperationalSessionFactoriesGenerator))]
        public async Task ClearTokens_OnNonExpiredTokens_ExpectSome(ISessionFactory sessionFactory)
        {
            var svcs = new ServiceCollection();
            svcs.AddScoped(_ => new OperationalSessionProvider(sessionFactory.OpenSession));

            var tokenCleanup = new TokenCleanup(svcs.BuildServiceProvider());

            var baseDate = DateTime.UtcNow;

            // create test data
            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                var grant = new PersistedGrant
                {
                    ClientId = Guid.NewGuid().ToString(),
                    Key = Guid.NewGuid().ToString(),
                    Type = Guid.NewGuid().ToString(),
                    Data = Guid.NewGuid().ToString(),
                    SubjectId = Guid.NewGuid().ToString(),
                    CreationTime = baseDate.AddDays(2),
                    Expiration = baseDate.AddMinutes(1)
                };
                await provider.Session.SaveAsync(grant);
            }

            // Clear tokens
            await tokenCleanup.ClearTokens(baseDate);

            // assert no data exists
            using (var provider = new OperationalSessionProvider(sessionFactory.OpenSession))
            {
                var exists = provider.Session
                    .Query<PersistedGrant>()
                    .Any(x => x.Expiration >= baseDate);

                Assert.True(exists);
            }
        }
    }
}