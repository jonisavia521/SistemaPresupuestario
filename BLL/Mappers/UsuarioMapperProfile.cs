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
          CreateMap<UsuarioDTO, Usuario>()
                .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.Id))
                   .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                   .ForMember(dest => dest.Usuario1, opt => opt.MapFrom(src => src.Usuario))
                   .ForMember(dest => dest.Clave, opt => opt.MapFrom(src => src.Clave))
                   .ForMember(dest => dest.Timestamp, opt => opt.Ignore())
                   .ForMember(dest => dest.UsuarioFamilia, opt => opt.Ignore())
                   .ForMember(dest => dest.UsuarioPatente, opt => opt.Ignore())
                   .ReverseMap()
                   .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdUsuario))
                   .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.Usuario1));

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