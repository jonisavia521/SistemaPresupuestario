using AutoMapper;
using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller.ViewModels.Mapper
{
    //como se usa inyeccion de dependencias, esta clase el framwork lo llama de manera implicita a todos los que heren de profile
    public  class MapperHelper:Profile
    {
        
        public MapperHelper()
        {
          CreateMap<UsuarioView, Usuario>()
                .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.Id))
                   .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                   .ForMember(dest => dest.Usuario_, opt => opt.MapFrom(src => src.Usuario))
                   .ForMember(dest => dest.Clave, opt => opt.MapFrom(src => src.Clave))
                   .ReverseMap();

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
