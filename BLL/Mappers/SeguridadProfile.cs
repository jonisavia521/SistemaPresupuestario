using AutoMapper;
using BLL.DTOs.Seguridad;
using DAL.Implementation.EntityFramework;
using DomainModel.Domain;
using DomainModel.Domain.Seguridad;
using System;
using System.Linq;

namespace BLL.Mappers
{
    /// <summary>
    /// Perfil de AutoMapper para entidades de seguridad
    /// DECISIÓN: Se incluyen mapeos Domain ↔ DTO y EF6 ↔ Domain
    /// </summary>
    public class SeguridadProfile : Profile
    {
        public SeguridadProfile()
        {
            // ============================================
            // MAPEOS DOMAIN MODEL ↔ DTOs
            // ============================================

            // Usuario Domain -> UserDto
            CreateMap<UsuarioDomain, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.Usuario_))
                .ForMember(dest => dest.VersionConcurrencia, opt => opt.Ignore()) // Se calcula en servicio
                .ForMember(dest => dest.CantidadFamiliasDirectas, opt => opt.Ignore()) // Se calcula en servicio
                .ForMember(dest => dest.CantidadPatentesDirectas, opt => opt.Ignore()) // Se calcula en servicio
                .ForMember(dest => dest.CantidadPermisosEfectivos, opt => opt.Ignore()) // Se calcula en servicio
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true)); // Por defecto activo

            // Usuario Domain -> UserEditDto
            CreateMap<UsuarioDomain, UserEditDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.Usuario_))
                .ForMember(dest => dest.Clave, opt => opt.Ignore()) // No se mapea por seguridad
                .ForMember(dest => dest.ConfirmarClave, opt => opt.Ignore())
                .ForMember(dest => dest.VersionConcurrencia, opt => opt.Ignore()) // Se calcula en servicio
                .ForMember(dest => dest.FamiliasAsignadas, opt => opt.Ignore()) // Se calcula en servicio
                .ForMember(dest => dest.PatentesAsignadas, opt => opt.Ignore()) // Se calcula en servicio
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CambiarClave, opt => opt.MapFrom(src => false));

            // UserEditDto -> Usuario Domain
            CreateMap<UserEditDto, UsuarioDomain>()
                .ConstructUsing(src => src.Id == Guid.Empty 
                    ? new UsuarioDomain(src.Nombre, src.Usuario, src.Clave) // Crear nuevo
                    : new UsuarioDomain(src.Id, src.Nombre, src.Usuario, src.Clave)) // Cargar existente
                .ForAllOtherMembers(opt => opt.Ignore());

            // Familia Domain -> FamiliaDto
            CreateMap<FamiliaDomain, FamiliaDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.FamiliasPadre, opt => opt.Ignore()) // Se calcula en servicio
                .ForMember(dest => dest.FamiliasHijas, opt => opt.MapFrom(src => src.FamiliasHijas))
                .ForMember(dest => dest.PatentesDirectas, opt => opt.MapFrom(src => src.PatentesDirectas))
                .ForMember(dest => dest.CantidadPatentesEfectivas, opt => opt.MapFrom(src => src.ContarPatentes()))
                .ForMember(dest => dest.VersionConcurrencia, opt => opt.Ignore()) // Se calcula en servicio
                .ForMember(dest => dest.AsignadaDirectamente, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.NivelJerarquia, opt => opt.MapFrom(src => 0));

            // Patente Domain -> PatenteDto
            CreateMap<PatenteDomain, PatenteDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Vista, opt => opt.MapFrom(src => src.Vista))
                .ForMember(dest => dest.VersionConcurrencia, opt => opt.Ignore()) // Se calcula en servicio
                .ForMember(dest => dest.AsignadaDirectamente, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.EsHeredada, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.FamiliaOrigen, opt => opt.Ignore());

            // ============================================
            // MAPEOS ENTITY FRAMEWORK ↔ DOMAIN MODEL
            // ============================================

            // EF Usuario -> Domain UsuarioDomain
            CreateMap<Usuario, UsuarioDomain>()
                .ConstructUsing(src => new UsuarioDomain(src.IdUsuario, src.Nombre, src.Usuario1, src.Clave))
                .ForAllOtherMembers(opt => opt.Ignore());

            // Domain UsuarioDomain -> EF Usuario
            CreateMap<UsuarioDomain, Usuario>()
                .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Usuario1, opt => opt.MapFrom(src => src.Usuario_))
                .ForMember(dest => dest.Clave, opt => opt.MapFrom(src => src.Clave))
                .ForMember(dest => dest.timestamp, opt => opt.Ignore()) // Manejado por EF
                .ForMember(dest => dest.Usuario_Familia, opt => opt.Ignore()) // Manejado por repositorio
                .ForMember(dest => dest.Usuario_Patente, opt => opt.Ignore()); // Manejado por repositorio

            // EF Familia -> Domain FamiliaDomain
            CreateMap<Familia, FamiliaDomain>()
                .ConstructUsing(src => new FamiliaDomain(src.IdFamilia, src.Nombre, src.timestamp))
                .ForAllOtherMembers(opt => opt.Ignore());

            // Domain FamiliaDomain -> EF Familia
            CreateMap<FamiliaDomain, Familia>()
                .ForMember(dest => dest.IdFamilia, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.timestamp, opt => opt.MapFrom(src => src.VersionConcurrencia))
                .ForMember(dest => dest.RelacionesComoPadre, opt => opt.Ignore()) // Manejado por repositorio
                .ForMember(dest => dest.RelacionesComoHijo, opt => opt.Ignore()) // Manejado por repositorio
                .ForMember(dest => dest.Familia_Patente, opt => opt.Ignore()) // Manejado por repositorio
                .ForMember(dest => dest.Usuario_Familia, opt => opt.Ignore()); // Manejado por repositorio

            // EF Patente -> Domain PatenteDomain
            CreateMap<Patente, PatenteDomain>()
                .ConstructUsing(src => new PatenteDomain(src.IdPatente, src.Nombre, src.Vista, src.timestamp))
                .ForAllOtherMembers(opt => opt.Ignore());

            // Domain PatenteDomain -> EF Patente
            CreateMap<PatenteDomain, Patente>()
                .ForMember(dest => dest.IdPatente, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Vista, opt => opt.MapFrom(src => src.Vista))
                .ForMember(dest => dest.timestamp, opt => opt.MapFrom(src => src.VersionConcurrencia))
                .ForMember(dest => dest.Familia_Patente, opt => opt.Ignore()) // Manejado por repositorio
                .ForMember(dest => dest.Usuario_Patente, opt => opt.Ignore()); // Manejado por repositorio

            // ============================================
            // MAPEOS PARA VERSIONADO (byte[] <-> base64)
            // ============================================

            // Convertir byte[] a string base64 para DTOs
            CreateMap<byte[], string>().ConvertUsing(src => src != null ? Convert.ToBase64String(src) : null);
            // Convertir string base64 a byte[] para entidades
            CreateMap<string, byte[]>().ConvertUsing(src => !string.IsNullOrEmpty(src) ? Convert.FromBase64String(src) : null);


            // ============================================
            // MAPEOS DIRECTOS ENTITY FRAMEWORK -> DTOs (NUEVOS)
            // ============================================

            // Usuario (EF) -> UserDto (para evitar doble salto EF -> Domain -> DTO)
            CreateMap<Usuario, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdUsuario))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.Usuario1))
                .ForMember(dest => dest.VersionConcurrencia, opt => opt.Ignore()) // Se completa en servicio si aplica
                .ForMember(dest => dest.CantidadFamiliasDirectas, opt => opt.Ignore())
                .ForMember(dest => dest.CantidadPatentesDirectas, opt => opt.Ignore())
                .ForMember(dest => dest.CantidadPermisosEfectivos, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true));

            // Usuario (EF) -> UserEditDto
            CreateMap<Usuario, UserEditDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdUsuario))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.Usuario1))
                .ForMember(dest => dest.Clave, opt => opt.Ignore())          // No exponer
                .ForMember(dest => dest.ConfirmarClave, opt => opt.Ignore())
                .ForMember(dest => dest.VersionConcurrencia, opt => opt.Ignore())
                .ForMember(dest => dest.FamiliasAsignadas, opt => opt.Ignore())
                .ForMember(dest => dest.PatentesAsignadas, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CambiarClave, opt => opt.MapFrom(src => false));
        }
    }
}