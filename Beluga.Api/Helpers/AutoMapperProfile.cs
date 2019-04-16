using AutoMapper;
using Sendeazy.Api.Dtos;
using Sendeazy.Api.Entities;
using WebApi.Dtos;
using WebApi.Entities;

namespace SendeoApi.Helpers
{
  public class AutoMapperProfile : Profile
  {
    public AutoMapperProfile()
    {
      CreateMap<User, UserDto>();
      CreateMap<UserDto, User>();
      CreateMap<UserForUpdateDto, User>();
      CreateMap<User, UserForDetailDto>();
      CreateMap<Photo, PhotoForReturnDto>();
      CreateMap<PhotoForCreationDto, Photo>();
      CreateMap<UserForRegisterDto, User>();
    }
  }
}
