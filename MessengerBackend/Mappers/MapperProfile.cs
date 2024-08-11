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
            // .ForMember(dest => dest.Nickname,
            //     opt => opt.MapFrom(
            //         src => src.UserName))  -  якщо назви полів моделі і ДТО не співпадають
            .ReverseMap();

        CreateMap<User, CreateUserRequest>().ReverseMap();
    }
}