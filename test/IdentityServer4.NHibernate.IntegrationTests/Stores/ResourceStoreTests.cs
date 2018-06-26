using AutoMapper;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.NHibernate.Automapper;
using IdentityServer4.NHibernate.Stores;
using IdentityServer4.Stores;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer4.NHibernate.IntegrationTests.Stores
{
    public class ResourceStoreTests
    {
        private readonly IMapper _mapper;

        public ResourceStoreTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ResourceProfile>()).CreateMapper();
        }

        [Theory, ClassData(typeof(ConfigurationSessionFactoriesGenerator))]
        public async Task FindResourcesAsync_WhenResourcesExist_ExpectResourcesReturned(ISessionFactory sessionFactory)
        {
            var testIdentityResource = CreateIdentityTestResource();
            var testApiResource = CreateApiTestResource();

            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                await provider.Session.SaveAsync(_mapper.Map<Entities.IdentityResource>(testIdentityResource));
                await provider.Session.SaveAsync(_mapper.Map<Entities.ApiResource>(testApiResource));
            }

            Resources resources;
            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                var store = new ResourceStore(provider);
                resources = await store.FindResourcesByScopeAsync(new List<string>
                {
                    testIdentityResource.Name,
                    testApiResource.Scopes.First().Name
                });
            }

            Assert.NotNull(resources);
            Assert.NotNull(resources.IdentityResources);
            Assert.NotEmpty(resources.IdentityResources);
            Assert.NotNull(resources.ApiResources);
            Assert.NotEmpty(resources.ApiResources);
            Assert.NotNull(resources.IdentityResources.FirstOrDefault(x => x.Name == testIdentityResource.Name));
            Assert.NotNull(resources.ApiResources.FirstOrDefault(x => x.Name == testApiResource.Name));
        }

        [Theory, ClassData(typeof(ConfigurationSessionFactoriesGenerator))]
        public async Task FindResourcesAsync_WhenResourcesExist_ExpectOnlyResourcesRequestedReturned(ISessionFactory sessionFactory)
        {
            var testIdentityResource = CreateIdentityTestResource();
            var testApiResource = CreateApiTestResource();

            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                await provider.Session.SaveAsync(_mapper.Map<Entities.IdentityResource>(testIdentityResource));
                await provider.Session.SaveAsync(_mapper.Map<Entities.ApiResource>(testApiResource));
                await provider.Session.SaveAsync(_mapper.Map<Entities.IdentityResource>(CreateIdentityTestResource()));
                await provider.Session.SaveAsync(_mapper.Map<Entities.ApiResource>(CreateApiTestResource()));
            }

            Resources resources;
            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                var store = new ResourceStore(provider);
                resources = await store.FindResourcesByScopeAsync(new List<string>
                {
                    testIdentityResource.Name,
                    testApiResource.Scopes.First().Name
                });
            }

            Assert.NotNull(resources);
            Assert.NotNull(resources.IdentityResources);
            Assert.NotEmpty(resources.IdentityResources);
            Assert.NotNull(resources.ApiResources);
            Assert.NotEmpty(resources.ApiResources);
            Assert.Equal(1, resources.IdentityResources.Count);
            Assert.Equal(1, resources.ApiResources.Count);
        }

        [Theory, ClassData(typeof(ConfigurationSessionFactoriesGenerator))]
        public async Task GetAllResources_WhenAllResourcesRequested_ExpectAllResourcesIncludingHidden(ISessionFactory sessionFactory)
        {
            var visibleIdentityResource = CreateIdentityTestResource();
            var visibleApiResource = CreateApiTestResource();
            var hiddenIdentityResource = new IdentityResource { Name = Guid.NewGuid().ToString(), ShowInDiscoveryDocument = false };
            var hiddenApiResource = new ApiResource
            {
                Name = Guid.NewGuid().ToString(),
                Scopes = new List<Scope> { new Scope { Name = Guid.NewGuid().ToString(), ShowInDiscoveryDocument = false } }
            };

            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                await provider.Session.SaveAsync(_mapper.Map<Entities.IdentityResource>(visibleIdentityResource));
                await provider.Session.SaveAsync(_mapper.Map<Entities.ApiResource>(visibleApiResource));
                await provider.Session.SaveAsync(_mapper.Map<Entities.IdentityResource>(hiddenIdentityResource));
                await provider.Session.SaveAsync(_mapper.Map<Entities.ApiResource>(hiddenApiResource));
            }

            Resources resources;
            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                var store = new ResourceStore(provider);
                resources = await store.GetAllResourcesAsync();
            }

            Assert.NotNull(resources);
            Assert.NotEmpty(resources.IdentityResources);
            Assert.NotEmpty(resources.ApiResources);

            Assert.Contains(resources.IdentityResources, x => !x.ShowInDiscoveryDocument);
            Assert.Contains(resources.ApiResources, x => !x.Scopes.Any(y => y.ShowInDiscoveryDocument));
        }

        [Theory, ClassData(typeof(ConfigurationSessionFactoriesGenerator))]
        public async Task FindIdentityResourcesByScopeAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned(ISessionFactory sessionFactory)
        {
            var resource = CreateIdentityTestResource();

            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                await provider.Session.SaveAsync(_mapper.Map<Entities.IdentityResource>(resource));
            }

            IEnumerable<IdentityResource> resources;
            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                var store = new ResourceStore(provider);
                resources = await store.FindIdentityResourcesByScopeAsync(new List<string>
                {
                    resource.Name
                });
            }

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            var foundScope = resources.Single();

            Assert.Equal(resource.Name, foundScope.Name);
            Assert.NotNull(foundScope.UserClaims);
            Assert.NotEmpty(foundScope.UserClaims);
        }

        [Theory, ClassData(typeof(ConfigurationSessionFactoriesGenerator))]
        public async Task FindIdentityResourcesByScopeAsync_WhenResourcesExist_ExpectOnlyRequestedReturned(ISessionFactory sessionFactory)
        {
            var resource = CreateIdentityTestResource();

            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                await provider.Session.SaveAsync(_mapper.Map<Entities.IdentityResource>(resource));
                await provider.Session.SaveAsync(_mapper.Map<Entities.IdentityResource>(CreateIdentityTestResource()));
            }

            IEnumerable<IdentityResource> resources;
            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                var store = new ResourceStore(provider);
                resources = await store.FindIdentityResourcesByScopeAsync(new List<string>
                {
                    resource.Name
                });
            }

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.Single(resources);
        }

        [Theory, ClassData(typeof(ConfigurationSessionFactoriesGenerator))]
        public async Task FindApiResourceAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned(ISessionFactory sessionFactory)
        {
            var resource = CreateApiTestResource();

            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                await provider.Session.SaveAsync(_mapper.Map<Entities.ApiResource>(resource));
            }

            ApiResource foundResource;
            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                var store = new ResourceStore(provider);
                foundResource = await store.FindApiResourceAsync(resource.Name);
            }

            Assert.NotNull(foundResource);

            Assert.NotNull(foundResource.UserClaims);
            Assert.NotEmpty(foundResource.UserClaims);
            Assert.NotNull(foundResource.ApiSecrets);
            Assert.NotEmpty(foundResource.ApiSecrets);
            Assert.NotNull(foundResource.Scopes);
            Assert.NotEmpty(foundResource.Scopes);
            Assert.Contains(foundResource.Scopes, x => x.UserClaims.Any());
        }

        [Theory, ClassData(typeof(ConfigurationSessionFactoriesGenerator))]
        public async Task FindApiResourcesByScopeAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned(ISessionFactory sessionFactory)
        {
            var resource = CreateApiTestResource();

            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                await provider.Session.SaveAsync(_mapper.Map<Entities.ApiResource>(resource));
            }

            IEnumerable<ApiResource> resources;
            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                var store = new ResourceStore(provider);
                resources = await store.FindApiResourcesByScopeAsync(new List<string> { resource.Scopes.First().Name });
            }

            Assert.NotEmpty(resources);
            Assert.NotNull(resources);

            Assert.NotNull(resources.First().UserClaims);
            Assert.NotEmpty(resources.First().UserClaims);
            Assert.NotNull(resources.First().ApiSecrets);
            Assert.NotEmpty(resources.First().ApiSecrets);
            Assert.NotNull(resources.First().Scopes);
            Assert.NotEmpty(resources.First().Scopes);
            Assert.Contains(resources.First().Scopes, x => x.UserClaims.Any());
        }

        [Theory, ClassData(typeof(ConfigurationSessionFactoriesGenerator))]
        public async Task FindApiResourcesByScopeAsync_WhenMultipleResourcesExist_ExpectOnlyRequestedResourcesReturned(ISessionFactory sessionFactory)
        {
            var resource = CreateApiTestResource();
            resource.Scopes.Add(new Scope
            {
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                DisplayName = Guid.NewGuid().ToString()
            });

            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                await provider.Session.SaveAsync(_mapper.Map<Entities.ApiResource>(resource));
                await provider.Session.SaveAsync(_mapper.Map<Entities.ApiResource>(CreateApiTestResource()));
                await provider.Session.SaveAsync(_mapper.Map<Entities.ApiResource>(CreateApiTestResource()));
            }

            IEnumerable<ApiResource> resources;
            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                var store = new ResourceStore(provider);
                resources = await store.FindApiResourcesByScopeAsync(new List<string> { resource.Scopes.First().Name });
            }

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.Single(resources);
        }

        private static IdentityResource CreateIdentityTestResource()
        {
            return new IdentityResource()
            {
                Name = Guid.NewGuid().ToString(),
                DisplayName = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                ShowInDiscoveryDocument = true,
                UserClaims =
                {
                    JwtClaimTypes.Subject,
                    JwtClaimTypes.Name,
                }
            };
        }

        private static ApiResource CreateApiTestResource()
        {
            return new ApiResource()
            {
                Name = Guid.NewGuid().ToString(),
                ApiSecrets = new List<Secret> { new Secret("secret".Sha256()) },
                Scopes =
                    new List<Scope>
                    {
                        new Scope
                        {
                            Name = Guid.NewGuid().ToString(),
                            UserClaims = {Guid.NewGuid().ToString()}
                        }
                    },
                UserClaims =
                {
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                }
            };
        }
    }
}