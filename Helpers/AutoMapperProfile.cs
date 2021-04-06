using AutoMapper;
using DotsApi.Models;

namespace DotsApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<RegisterDto, User>();
            CreateMap<UpdateDto, User>();
        }
    }
}
