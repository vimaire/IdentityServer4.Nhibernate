using AutoMapper;

namespace IdentityServer4.NHibernate.Automapper
{
    public class ResourceProfile : Profile
    {
        public ResourceProfile()
        {
            CreateMap<Entities.ApiResource, Models.ApiResource>(MemberList.Destination)
                .ConstructUsing(_ => new Models.ApiResource())
                .ReverseMap();

            CreateMap<Entities.ApiResourceScope, Models.Scope>(MemberList.Destination)
                .ConstructUsing(_ => new Models.Scope())
                .ReverseMap();

            CreateMap<Entities.Secret, Models.Secret>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null))
                .ReverseMap();

            CreateMap<Entities.IdentityResource, Models.IdentityResource>(MemberList.Destination)
                .ConstructUsing(_ => new Models.IdentityResource())
                .ReverseMap();
        }
    }
}