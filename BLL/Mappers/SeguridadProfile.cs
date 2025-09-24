using AutoMapper;
using BLL.DTOs;
using DAL.Implementation.EntityFramework;
using DomainModel.Domain;
using System;
using System.Linq;

namespace BLL.Mappers
{
    /// <summary>
    /// Perfil de AutoMapper para entidades de seguridad
    /// DECISIÓN: Perfil separado para módulo de seguridad para mejor organización
    /// </summary>
    public class SeguridadProfile : Profile
    {
        public SeguridadProfile()
        {
            // Mapeos Usuario Entity <-> DTOs
            CreateMap<Usuario, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdUsuario))
                .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.Usuario1))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.CantPermisosDirectos, opt => opt.Ignore()) // Se calcula en servicio
                .ForMember(dest => dest.CantPermisosEfectivos, opt => opt.Ignore()) // Se calcula en servicio
                .ForMember(dest => dest.Estado, opt => opt.Ignore()) // Para futura implementación
                .ForMember(dest => dest.TimestampBase64, opt => opt.Ignore()); // Se asigna manualmente

            CreateMap<Usuario, UserEditDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdUsuario))
                .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.Usuario1))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.NuevaClaveOpcional, opt => opt.Ignore()) // Nunca mapear contraseñas
                .ForMember(dest => dest.ConfirmarClave, opt => opt.Ignore())
                .ForMember(dest => dest.FamiliasAsignadasIds, opt => opt.Ignore()) // Se carga manualmente
                .ForMember(dest => dest.PatentesAsignadasIds, opt => opt.Ignore()) // Se carga manualmente
                .ForMember(dest => dest.TimestampBase64, opt => opt.Ignore()) // Se asigna manualmente
                .ForMember(dest => dest.ForzarCambioClave, opt => opt.Ignore()) // Para futura implementación
                .ForMember(dest => dest.Estado, opt => opt.Ignore()); // Para futura implementación

            CreateMap<UserEditDto, Usuario>()
                .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Usuario1, opt => opt.MapFrom(src => src.Usuario))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Clave, opt => opt.Ignore()) // Se maneja en servicio
                .ForMember(dest => dest.timestamp, opt => opt.Ignore()) // Sistema
                .ForMember(dest => dest.Usuario_Familia, opt => opt.Ignore()) // Relaciones
                .ForMember(dest => dest.Usuario_Patente, opt => opt.Ignore()); // Relaciones

            // Mapeos Familia Entity <-> DTOs
            CreateMap<Familia, FamiliaDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdFamilia))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Hijos, opt => opt.Ignore()) // Se construye manualmente la jerarquía
                .ForMember(dest => dest.Patentes, opt => opt.MapFrom(src => src.Familia_Patente.Select(fp => fp.Patente)))
                .ForMember(dest => dest.EsSeleccionada, opt => opt.Ignore()) // Se asigna en servicio
                .ForMember(dest => dest.Nivel, opt => opt.Ignore()) // Se calcula en servicio
                .ForMember(dest => dest.IdPadre, opt => opt.Ignore()) // Se calcula en servicio
                .ForMember(dest => dest.TimestampBase64, opt => opt.MapFrom(src => Convert.ToBase64String(src.timestamp)));

            // Mapeos Patente Entity <-> DTOs
            CreateMap<Patente, PatenteDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdPatente))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Vista, opt => opt.MapFrom(src => src.Vista))
                .ForMember(dest => dest.EsDirecta, opt => opt.Ignore()) // Se asigna en servicio
                .ForMember(dest => dest.EsHeredada, opt => opt.Ignore()) // Se asigna en servicio
                .ForMember(dest => dest.Origen, opt => opt.Ignore()) // Se asigna en servicio
                .ForMember(dest => dest.TimestampBase64, opt => opt.MapFrom(src => Convert.ToBase64String(src.timestamp)));

            // Mapeos Domain Model <-> DTOs (mantener compatibilidad con perfil existente)
            CreateMap<UsuarioDM, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.Usuario_))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.CantPermisosDirectos, opt => opt.Ignore())
                .ForMember(dest => dest.CantPermisosEfectivos, opt => opt.Ignore())
                .ForMember(dest => dest.Estado, opt => opt.Ignore())
                .ForMember(dest => dest.TimestampBase64, opt => opt.Ignore());

            CreateMap<UserDto, UsuarioDM>()
                .ConstructUsing(src => new UsuarioDM(
                    src.Id,
                    src.Nombre,
                    src.Usuario,
                    string.Empty // La clave no se mapea desde DTO
                ));

            // Mapeo para compatibilidad con el perfil existente
            CreateMap<UsuarioDM, Usuario>()
                .ConstructUsing(src => new Usuario
                {
                    IdUsuario = src.Id,
                    Nombre = src.Nombre,
                    Usuario1 = src.Usuario_,
                    Clave = src.Clave
                })
                .ForMember(dest => dest.Usuario_Familia, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario_Patente, opt => opt.Ignore())
                .ForMember(dest => dest.timestamp, opt => opt.Ignore());

            CreateMap<Usuario, UsuarioDM>()
                .ConstructUsing(src => new UsuarioDM(
                    src.IdUsuario,
                    src.Nombre,
                    src.Usuario1,
                    src.Clave
                ));
        }
    }
}