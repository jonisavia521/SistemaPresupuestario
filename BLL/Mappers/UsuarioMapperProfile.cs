using AutoMapper;
using BLL.DTOs;
using DAL.Implementation.EntityFramework.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Mappers
{
    //como se usa inyeccion de dependencias, esta clase el framwork lo llama de manera implicita a todos los que heren de profile
    public  class UsuarioMapperProfile : Profile
    {
        
        public UsuarioMapperProfile()
        {
            CreateMap<DomainModel.Domain.Usuario, UsuarioDTO>()
                  .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.Usuario_))
                  .ReverseMap()
                  .ForMember(dest => dest.Usuario_, opt => opt.MapFrom(src => src.Usuario))
                  .ConstructUsing(src =>
                      new DomainModel.Domain.Usuario(
                          idUsuario: src.Id,
                          nombre: src.Nombre,
                          usuarioNombre: src.Usuario,
                          claveHash: src.Clave
                      ));

            CreateMap<Usuario, DomainModel.Domain.Usuario>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdUsuario))
                 .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                 .ForMember(dest => dest.Usuario_, opt => opt.MapFrom(src => src.Usuario1))
                 .ForMember(dest => dest.Clave, opt => opt.MapFrom(src => src.Clave))
                 .ReverseMap()
                 .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.Id))
                 .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                 .ForMember(dest => dest.Usuario1, opt => opt.MapFrom(src => src.Usuario_))
                 .ForMember(dest => dest.Clave, opt => opt.MapFrom(src => src.Clave))
                 .ForMember(dest => dest.UsuarioFamilia, opt => opt.Ignore())
                 .ForMember(dest => dest.UsuarioPatente, opt => opt.Ignore())
                 .ConstructUsing(src =>
                     new Usuario
                     {
                         IdUsuario = src.Id,
                         Nombre = src.Nombre,
                         Usuario1 = src.Usuario_,
                         Clave = src.Clave
                     })
                 .AfterMap((src, dest) =>
                 {
                     dest.UsuarioFamilia = new HashSet<UsuarioFamilia>();
                     dest.UsuarioPatente = new HashSet<UsuarioPatente>();
                 });
            // Resolver la conversión de listas
            CreateMap<List<object>, List<object>>().ConvertUsing<CustomResolver>();
            
        }

     

        private class CustomResolver : ITypeConverter<List<object>, List<object>>
        {
            public List<object> Convert(List<object> source, List<object> destination, ResolutionContext context)
            {
                var objects = new List<object>();
                foreach (var obj in source)
                {
                    var destinationType = context.ConfigurationProvider.GetAllTypeMaps()
                        .First(x => x.SourceType == obj.GetType())
                        .DestinationType;
                    var target = context.Mapper.Map(obj, obj.GetType(), destinationType);
                    objects.Add(target);
                }
                return objects;
            }
        }
    }

}