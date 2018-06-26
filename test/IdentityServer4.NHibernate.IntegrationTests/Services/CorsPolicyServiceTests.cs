using AutoMapper;
using IdentityServer4.Models;
using IdentityServer4.NHibernate.Automapper;
using IdentityServer4.NHibernate.Services;
using IdentityServer4.NHibernate.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer4.NHibernate.IntegrationTests.Services
{
    public class CorsPolicyServiceTests
    {
        private readonly IMapper _mapper;

        public CorsPolicyServiceTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientProfile>()).CreateMapper();
        }

        [Theory, ClassData(typeof(ConfigurationSessionFactoriesGenerator))]
        public async Task IsOriginAllowedAsync_WhenOriginIsAllowed_ExpectTrue(ISessionFactory sessionFactory)
        {
            const string testCorsOrigin = "https://identityserver.io/";

            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                await provider.Session.SaveAsync(_mapper.Map<Entities.Client>(new Client
                {
                    ClientId = Guid.NewGuid().ToString(),
                    ClientName = Guid.NewGuid().ToString(),
                    AllowedCorsOrigins = new List<string> { "https://www.identityserver.com" }
                }));
                await provider.Session.SaveAsync(_mapper.Map<Entities.Client>(new Client
                {
                    ClientId = Guid.NewGuid().ToString(),
                    ClientName = Guid.NewGuid().ToString(),
                    AllowedCorsOrigins = new List<string> { "https://www.identityserver.com", testCorsOrigin }
                }));
            }

            bool result;
            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                var ctx = new DefaultHttpContext();
                var svcs = new ServiceCollection();
                svcs.AddSingleton(provider);
                ctx.RequestServices = svcs.BuildServiceProvider();
                var ctxAccessor = new HttpContextAccessor
                {
                    HttpContext = ctx
                };

                var service = new CorsPolicyService(ctxAccessor);
                result = service.IsOriginAllowedAsync(testCorsOrigin).Result;
            }

            Assert.True(result);
        }

        [Theory, ClassData(typeof(ConfigurationSessionFactoriesGenerator))]
        public async Task IsOriginAllowedAsync_WhenOriginIsNotAllowed_ExpectFalse(ISessionFactory sessionFactory)
        {
            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                await provider.Session.SaveAsync(_mapper.Map<Entities.Client>(new Client
                {
                    ClientId = Guid.NewGuid().ToString(),
                    ClientName = Guid.NewGuid().ToString(),
                    AllowedCorsOrigins = new List<string> { "https://www.identityserver.com" }
                }));
            }

            bool result;
            using (var provider = new ConfigurationSessionProvider(sessionFactory.OpenSession))
            {
                var ctx = new DefaultHttpContext();
                var svcs = new ServiceCollection();
                svcs.AddSingleton(provider);
                ctx.RequestServices = svcs.BuildServiceProvider();
                var ctxAccessor = new HttpContextAccessor
                {
                    HttpContext = ctx
                };

                var service = new CorsPolicyService(ctxAccessor);
                result = service.IsOriginAllowedAsync("InvalidOrigin").Result;
            }

            Assert.False(result);
        }
    }
}