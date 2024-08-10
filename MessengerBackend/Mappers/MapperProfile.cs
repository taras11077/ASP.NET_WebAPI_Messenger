using AutoMapper;
using MessengerBackend.Core.Models;
using MessengerBackend.DTOs;
using MessengerBackend.Requests;

namespace MessengerBackend.Mappers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<User, UserDTO>()
            /*.ForMember(dest => dest.Nickname,
                opt => opt.MapFrom(
                    src => src.UserName))*/
            .ReverseMap();

        CreateMap<User, CreateUserRequest>().ReverseMap();
    }
}