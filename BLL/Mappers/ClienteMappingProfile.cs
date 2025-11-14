using AutoMapper;
using BLL.DTOs;
using DomainModel.Domain;

namespace BLL.Mappers
{
    /// <summary>
    /// Perfil de mapeo AutoMapper para Cliente
    /// Define las transformaciones entre entidades de dominio y DTOs
    /// </summary>
    public class ClienteMappingProfile : Profile
    {
        public ClienteMappingProfile()
        {
            // Mapeo de ClienteDM (Dominio) a ClienteDTO (Vista)
            CreateMap<ClienteDM, ClienteDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CodigoCliente, opt => opt.MapFrom(src => src.CodigoCliente))
                .ForMember(dest => dest.RazonSocial, opt => opt.MapFrom(src => src.RazonSocial))
                .ForMember(dest => dest.TipoDocumento, opt => opt.MapFrom(src => src.TipoDocumento))
                .ForMember(dest => dest.NumeroDocumento, opt => opt.MapFrom(src => src.NumeroDocumento))
                .ForMember(dest => dest.IdVendedor, opt => opt.MapFrom(src => src.IdVendedor))
                .ForMember(dest => dest.IdProvincia, opt => opt.MapFrom(src => src.IdProvincia))
                .ForMember(dest => dest.TipoIva, opt => opt.MapFrom(src => src.TipoIva))
                .ForMember(dest => dest.CondicionPago, opt => opt.MapFrom(src => src.CondicionPago))
                .ForMember(dest => dest.AlicuotaArba, opt => opt.MapFrom(src => src.AlicuotaArba))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src => src.Telefono))
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.Direccion))
                .ForMember(dest => dest.Localidad, opt => opt.MapFrom(src => src.Localidad))
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => src.Activo))
                .ForMember(dest => dest.FechaAlta, opt => opt.MapFrom(src => src.FechaAlta))
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => src.FechaModificacion));

            // Mapeo inverso: ClienteDTO a ClienteDM
            // NOTA: Este mapeo generalmente NO se usa porque ClienteDM tiene constructores específicos
            // que ejecutan validaciones. Lo dejamos por completitud pero el servicio usa
            // los constructores directamente.
            CreateMap<ClienteDTO, ClienteDM>();
        }
    }
}
