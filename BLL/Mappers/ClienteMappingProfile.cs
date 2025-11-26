using AutoMapper;
using BLL.DTOs;
using DomainModel.Domain;

namespace BLL.Mappers
{
    /// <summary>
    /// Perfil de Mapeo AutoMapper para Cliente.
    /// 
    /// Define las transformaciones bidireccionales entre:
    /// - ClienteDM (Model de Dominio): Entidad que contiene la lógica de negocio y validaciones
    /// - ClienteDTO (Data Transfer Object): Objeto utilizado para transferir datos entre UI y BLL
    /// 
    /// AutoMapper utiliza este perfil para generar automáticamente código de conversión,
    /// evitando la escritura manual de mapeos repetitivos y propensos a errores.
    /// 
    /// Configuración de mapeo:
    /// - ClienteDM ? ClienteDTO: Conversión del modelo de dominio al objeto de transferencia
    ///   Se ejecuta cuando se retornan datos desde BLL hacia la UI.
    /// 
    /// - ClienteDTO ? ClienteDM: Conversión del objeto de transferencia al modelo de dominio
    ///   Generalmente NO se utiliza porque ClienteDM tiene constructores específicos
    ///   que ejecutan validaciones de negocio. El servicio crea instancias de ClienteDM
    ///   utilizando los constructores directamente en lugar de este mapeo.
    /// </summary>
    public class ClienteMappingProfile : Profile
    {
        /// <summary>
        /// Constructor del perfil de mapeo.
        /// Define todas las reglas de transformación entre ClienteDM y ClienteDTO.
        /// </summary>
        public ClienteMappingProfile()
        {
            /// <summary>
            /// Mapeo de ClienteDM (entidad de dominio) a ClienteDTO (objeto de transferencia).
            /// 
            /// Cada propiedad se mapea explícitamente aunque AutoMapper podría hacerlo automáticamente
            /// porque en este caso la convención de nombres coincide. La mapeo explícito proporciona:
            /// - Claridad sobre qué propiedades se transforman
            /// - Control total sobre el comportamiento
            /// - Facilidad para agregar transformaciones personalizadas en el futuro
            /// 
            /// Propiedades mapeadas:
            /// - Identificadores (Id)
            /// - Datos básicos (Código, RazonSocial)
            /// - Documento (Tipo, Número)
            /// - Referencias (IdVendedor, IdProvincia)
            /// - Información fiscal (TipoIva, AlicuotaArba)
            /// - Contacto (Email, Teléfono, Dirección, Localidad)
            /// - Control (Activo, FechaAlta, FechaModificacion)
            /// </summary>
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

            /// <summary>
            /// Mapeo inverso: ClienteDTO a ClienteDM.
            /// 
            /// IMPORTANTE - Este mapeo generalmente NO se utiliza en la práctica porque:
            /// 
            /// 1. ClienteDM contiene lógica de negocio y validaciones en sus constructores.
            ///    Un mapeo automático podría omitir estas validaciones críticas.
            /// 
            /// 2. ClienteDM podría tener constructores especializados que requieren
            ///    lógica adicional (ej: validación de CUIT, cálculos, etc.)
            /// 
            /// 3. El servicio ClienteService crea instancias de ClienteDM utilizando
            ///    los constructores directamente con las validaciones correspondientes.
            /// 
            /// Se deja este mapeo por completitud arquitectónica y en caso de necesidad
            /// futura, pero se recomienda su reemplazo por constructores explícitos cuando sea posible.
            /// </summary>
            CreateMap<ClienteDTO, ClienteDM>();
        }
    }
}
