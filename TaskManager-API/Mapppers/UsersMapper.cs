using AutoMapper;
using TaskManager_API.Models.Domain;
using TaskManager_API.DTOs;

namespace TaskManager_API.Mappings
{
    public class UsersMapper : Profile
    {
        public UsersMapper()
        {
            CreateMap<User, UsersDto>().ReverseMap();
            CreateMap<CreateUser, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());

            CreateMap<UpdateUser, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UserImageURL, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }

    }
}