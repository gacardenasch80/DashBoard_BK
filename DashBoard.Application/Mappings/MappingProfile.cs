using AutoMapper;
using DashBoard.Application.DTOs;
using DashBoard.Core.Entities;

namespace DashBoard.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Usuario, UsuarioDto>();
        CreateMap<CreateUsuarioDto, Usuario>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Analisis, opt => opt.Ignore());
        
        CreateMap<Analisis, AnalisisDto>()
            .ForMember(dest => dest.NombreUsuario, 
                opt => opt.MapFrom(src => $"{src.Usuario.Nombres} {src.Usuario.Apellidos}"));
        
        CreateMap<Analisis, AnalisisResponseDto>()
            .ForMember(dest => dest.NombreUsuario,
                opt => opt.MapFrom(src => $"{src.Usuario.Nombres} {src.Usuario.Apellidos}"));
    }
}
