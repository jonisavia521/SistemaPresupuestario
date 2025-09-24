using AutoMapper;
using BLL.DTOs;
using DAL.Implementation.EntityFramework;
using System.Linq;

namespace BLL.Mappers
{
    /// <summary>
    /// Profile de AutoMapper para entidades de seguridad
    /// </summary>
    public class SeguridadProfile : Profile
    {
        public SeguridadProfile()
        {
            // Mapeo de Familia
            CreateMap<Familia, FamiliaDto>()
                .ForMember(dest => dest.IdFamilia, opt => opt.MapFrom(src => src.IdFamilia))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.timestamp))
                .ForMember(dest => dest.FamiliasHijas, opt => opt.MapFrom(src => 
                    src.RelacionesComoPadre.Select(r => r.IdFamiliaHijo)))
                .ForMember(dest => dest.PatentesAsignadas, opt => opt.MapFrom(src => 
                    src.Familia_Patente.Select(fp => fp.IdPatente)));

            CreateMap<FamiliaDto, Familia>()
                .ForMember(dest => dest.IdFamilia, opt => opt.MapFrom(src => src.IdFamilia))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.timestamp, opt => opt.MapFrom(src => src.Timestamp))
                .ForMember(dest => dest.RelacionesComoPadre, opt => opt.Ignore())
                .ForMember(dest => dest.RelacionesComoHijo, opt => opt.Ignore())
                .ForMember(dest => dest.Familia_Patente, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario_Familia, opt => opt.Ignore());

            // Mapeo de Patente
            CreateMap<Patente, PatenteDto>()
                .ForMember(dest => dest.IdPatente, opt => opt.MapFrom(src => src.IdPatente))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Vista, opt => opt.MapFrom(src => src.Vista))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.timestamp));

            CreateMap<PatenteDto, Patente>()
                .ForMember(dest => dest.IdPatente, opt => opt.MapFrom(src => src.IdPatente))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Vista, opt => opt.MapFrom(src => src.Vista))
                .ForMember(dest => dest.timestamp, opt => opt.MapFrom(src => src.Timestamp))
                .ForMember(dest => dest.Familia_Patente, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario_Patente, opt => opt.Ignore());

            // Mapeo extendido de Usuario para edición
            CreateMap<Usuario, UserEditDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdUsuario))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.Usuario1))
                .ForMember(dest => dest.Clave, opt => opt.Ignore()) // No mapear clave por seguridad
                .ForMember(dest => dest.ConfirmarClave, opt => opt.Ignore())
                .ForMember(dest => dest.CambiarClave, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.timestamp))
                .ForMember(dest => dest.FamiliasAsignadas, opt => opt.MapFrom(src => 
                    src.Usuario_Familia.Select(uf => uf.IdFamilia)))
                .ForMember(dest => dest.PatentesAsignadas, opt => opt.MapFrom(src => 
                    src.Usuario_Patente.Select(up => up.IdPatente)));

            CreateMap<UserEditDto, Usuario>()
                .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Usuario1, opt => opt.MapFrom(src => src.Usuario))
                .ForMember(dest => dest.Clave, opt => opt.Ignore()) // Se maneja por separado
                .ForMember(dest => dest.timestamp, opt => opt.MapFrom(src => src.Timestamp))
                .ForMember(dest => dest.Usuario_Familia, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario_Patente, opt => opt.Ignore());
        }
    }
}