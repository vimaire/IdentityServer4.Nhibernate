using AutoMapper;

namespace IdentityServer4.NHibernate.Automapper
{
    public class PersistedGrantProfile : Profile
    {
        public PersistedGrantProfile()
        {
            CreateMap<Entities.PersistedGrant, Models.PersistedGrant>()
                .ReverseMap();
        }
    }
}