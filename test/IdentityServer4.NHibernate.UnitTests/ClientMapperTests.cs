using AutoMapper;
using IdentityServer4.NHibernate.Automapper;
using Xunit;
using Entites = IdentityServer4.NHibernate.Entities;

namespace IdentityServer4.NHibernate.UnitTests
{
    public class ClientMapperTests
    {
        [Fact]
        public void AutomapperConfigurationIsValid()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<ClientProfile>());
            config.AssertConfigurationIsValid();
        }

        [Fact]
        public void CanMap()
        {
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientProfile>()).CreateMapper();
            var model = new Models.Client();
            var mappedEntity = mapper.Map<Entites.Client>(model);
            var mappedModel = mapper.Map<Models.Client>(mappedEntity);

            Assert.NotNull(mappedEntity);
            Assert.NotNull(mappedModel);
        }
    }
}