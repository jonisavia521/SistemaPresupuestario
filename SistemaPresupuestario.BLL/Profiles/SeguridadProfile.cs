using AutoMapper;
using SistemaPresupuestario.BLL.DTOs;
using SistemaPresupuestario.DAL.Repositories.Interfaces;
using SistemaPresupuestario.DomainModel.Seguridad;
using System.Linq;

namespace SistemaPresupuestario.BLL.Profiles
{
    /// <summary>
    /// AutoMapper profile for mapping between domain entities and DTOs
    /// Handles complex mappings for security entities
    /// </summary>
    public class SeguridadProfile : Profile
    {
        public SeguridadProfile()
        {
            ConfigureUsuarioMappings();
            ConfigureFamiliaMappings();
            ConfigurePatenteMappings();
        }

        private void ConfigureUsuarioMappings()
        {
            // Usuario -> UsuarioDto
            CreateMap<Usuario, UsuarioDto>()
                .ForMember(dest => dest.Familias, opt => opt.MapFrom(src => src.Familias))
                .ForMember(dest => dest.Patentes, opt => opt.MapFrom(src => src.Patentes))
                .ForMember(dest => dest.PermisosEfectivos, opt => opt.Ignore()); // Will be set separately

            // UsuarioDto -> Usuario (for updates)
            CreateMap<UsuarioDto, Usuario>()
                .ForMember(dest => dest.Familias, opt => opt.Ignore()) // Handled separately
                .ForMember(dest => dest.Patentes, opt => opt.Ignore()) // Handled separately
                .ForMember(dest => dest.ClaveHash, opt => opt.Ignore()) // Never map password hash from DTO
                .ForMember(dest => dest.Salt, opt => opt.Ignore());

            // UsuarioEditDto -> Usuario
            CreateMap<UsuarioEditDto, Usuario>()
                .ForMember(dest => dest.Familias, opt => opt.Ignore()) // Handled separately through IDs
                .ForMember(dest => dest.Patentes, opt => opt.Ignore()) // Handled separately through IDs
                .ForMember(dest => dest.ClaveHash, opt => opt.Ignore()) // Will be hashed separately
                .ForMember(dest => dest.Salt, opt => opt.Ignore()) // Will be generated separately
                .ForMember(dest => dest.UltimoLogin, opt => opt.Ignore())
                .ForMember(dest => dest.IntentosLoginFallidos, opt => opt.Ignore())
                .ForMember(dest => dest.CuentaBloqueadaHasta, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore());

            // Usuario -> UsuarioEditDto
            CreateMap<Usuario, UsuarioEditDto>()
                .ForMember(dest => dest.Clave, opt => opt.Ignore()) // Never expose password
                .ForMember(dest => dest.ConfirmarClave, opt => opt.Ignore())
                .ForMember(dest => dest.CambiarClave, opt => opt.UseValue(false))
                .ForMember(dest => dest.FamiliasAsignadasIds, opt => opt.MapFrom(src => src.Familias.Select(f => f.Id)))
                .ForMember(dest => dest.PatentesAsignadasIds, opt => opt.MapFrom(src => src.Patentes.Select(p => p.Id)));
        }

        private void ConfigureFamiliaMappings()
        {
            // Familia -> FamiliaDto
            CreateMap<Familia, FamiliaDto>()
                .ForMember(dest => dest.FamiliaPadreNombre, opt => opt.MapFrom(src => src.FamiliaPadre != null ? src.FamiliaPadre.Nombre : null))
                .ForMember(dest => dest.Nivel, opt => opt.MapFrom(src => src.ObtenerNivel()))
                .ForMember(dest => dest.TieneHijos, opt => opt.MapFrom(src => src.FamiliasHijas.Any()))
                .ForMember(dest => dest.FamiliasHijas, opt => opt.MapFrom(src => src.FamiliasHijas))
                .ForMember(dest => dest.Patentes, opt => opt.MapFrom(src => src.Patentes))
                .ForMember(dest => dest.PatentesDirectasCount, opt => opt.MapFrom(src => src.Patentes.Count))
                .ForMember(dest => dest.PatentesEfectivasCount, opt => opt.MapFrom(src => src.ObtenerPatentesEfectivas().Count()))
                .ForMember(dest => dest.UsuariosCount, opt => opt.MapFrom(src => src.Usuarios.Count))
                .ForMember(dest => dest.RutaCompleta, opt => opt.Ignore()); // Will be calculated separately

            // FamiliaDto -> Familia
            CreateMap<FamiliaDto, Familia>()
                .ForMember(dest => dest.FamiliaPadre, opt => opt.Ignore()) // Handled through FamiliaPadreId
                .ForMember(dest => dest.FamiliasHijas, opt => opt.Ignore()) // Handled separately
                .ForMember(dest => dest.Patentes, opt => opt.Ignore()) // Handled separately
                .ForMember(dest => dest.Usuarios, opt => opt.Ignore()) // Handled separately
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore());

            // FamiliaHierarchyDto -> FamiliaDto (for repository DTOs)
            CreateMap<FamiliaHierarchyDto, FamiliaDto>()
                .ForMember(dest => dest.FamiliaPadreId, opt => opt.MapFrom(src => src.ParentId))
                .ForMember(dest => dest.Nivel, opt => opt.MapFrom(src => src.Level))
                .ForMember(dest => dest.TieneHijos, opt => opt.MapFrom(src => src.HasChildren))
                .ForMember(dest => dest.PatentesDirectasCount, opt => opt.MapFrom(src => src.DirectPatentsCount))
                .ForMember(dest => dest.PatentesEfectivasCount, opt => opt.MapFrom(src => src.EffectivePatentsCount))
                .ForMember(dest => dest.UsuariosCount, opt => opt.MapFrom(src => src.UsersCount))
                .ForMember(dest => dest.Patentes, opt => opt.MapFrom(src => src.DirectPatents))
                .ForMember(dest => dest.FamiliasHijas, opt => opt.MapFrom(src => src.Children));
        }

        private void ConfigurePatenteMappings()
        {
            // Patente -> PatenteDto
            CreateMap<Patente, PatenteDto>()
                .ForMember(dest => dest.UsuariosDirectosCount, opt => opt.MapFrom(src => src.Usuarios.Count))
                .ForMember(dest => dest.FamiliasCount, opt => opt.MapFrom(src => src.Familias.Count))
                .ForMember(dest => dest.UsuariosEfectivosCount, opt => opt.Ignore()) // Calculated separately
                .ForMember(dest => dest.EsHeredada, opt => opt.UseValue(false)) // Default value
                .ForMember(dest => dest.FuenteHerencia, opt => opt.Ignore()); // Set contextually

            // PatenteDto -> Patente
            CreateMap<PatenteDto, Patente>()
                .ForMember(dest => dest.Usuarios, opt => opt.Ignore()) // Handled separately
                .ForMember(dest => dest.Familias, opt => opt.Ignore()) // Handled separately
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore());

            // PatenteUsageDto -> PatenteDto (for repository DTOs)
            CreateMap<PatenteUsageDto, PatenteDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PatenteId))
                .ForMember(dest => dest.UsuariosDirectosCount, opt => opt.MapFrom(src => src.DirectUsersCount))
                .ForMember(dest => dest.FamiliasCount, opt => opt.MapFrom(src => src.FamiliesCount))
                .ForMember(dest => dest.UsuariosEfectivosCount, opt => opt.MapFrom(src => src.TotalEffectiveUsersCount));
        }

        /// <summary>
        /// Creates a mapping configuration that can be used with custom resolvers
        /// </summary>
        public static MapperConfiguration CreateConfiguration()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SeguridadProfile>();
                
                // Add custom value resolvers if needed
                cfg.AllowNullCollections = true;
                cfg.AllowNullDestinationValues = true;
            });
        }

        /// <summary>
        /// Creates a mapper instance with the security profile
        /// </summary>
        public static IMapper CreateMapper()
        {
            var config = CreateConfiguration();
            return config.CreateMapper();
        }
    }
}