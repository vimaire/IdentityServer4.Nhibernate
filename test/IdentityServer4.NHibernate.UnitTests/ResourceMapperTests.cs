using AutoMapper;
using IdentityServer4.NHibernate.Automapper;
using Xunit;

namespace IdentityServer4.NHibernate.UnitTests
{
    public class ResourceMapperTests
    {   
        [Fact]
        public void AutomapperConfigurationIsValid()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<ResourceProfile>());
            config.AssertConfigurationIsValid();
        }

        [Fact]
        public void CanMapApiResource()
        {
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<ResourceProfile>()).CreateMapper();
            var model = new Models.ApiResource();
            var mappedEntity = mapper.Map<Entities.ApiResource>(model);
            var mappedModel = mapper.Map<Models.ApiResource>(mappedEntity);

            Assert.NotNull(mappedEntity);
            Assert.NotNull(mappedModel);
        }
        
        [Fact]
        public void CanMapIdentityResource()
        {
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<ResourceProfile>()).CreateMapper();
            var model = new Models.IdentityResource();
            var mappedEntity = mapper.Map<Entities.IdentityResource>(model);
            var mappedModel = mapper.Map<Models.IdentityResource>(mappedEntity);

            Assert.NotNull(mappedEntity);
            Assert.NotNull(mappedModel);
        }
    }
}