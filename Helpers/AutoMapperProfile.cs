using AutoMapper;
using DotsApi.Models;

namespace DotsApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterDto, User>();
            CreateMap<UpdateDto, User>();

            CreateMap<AddNoticeDto, Notice>();
            CreateMap<UpdateNoticeDto, Notice>();
        }
    }
}
