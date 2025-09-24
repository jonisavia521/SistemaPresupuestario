using AutoMapper;
using BLL.DTOs;
using DAL.Implementation.EntityFramework;
using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Mappers
{
    //como se usa inyeccion de dependencias, esta clase el framwork lo llama de manera implicita a todos los que heren de profile
    public class UsuarioMapperProfile : Profile
    {

        public UsuarioMapperProfile()
        {
            CreateMap<DomainModel.Domain.UsuarioDM, UsuarioDTO>()
                  .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.Usuario_))
                  .ReverseMap()
                  .ForMember(dest => dest.Usuario_, opt => opt.MapFrom(src => src.Usuario))
                  .ConstructUsing(src =>
                      new DomainModel.Domain.UsuarioDM(
                          src.Id,
                          src.Nombre,
                          src.Usuario,
                          src.Clave
                      ));

            CreateMap<Usuario, UsuarioDM>()
     .ConstructUsing(src => new UsuarioDM(
         src.IdUsuario,
         src.Nombre,
         src.Usuario1,
         src.Clave
     ))
     .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdUsuario))
     .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
     .ForMember(dest => dest.Usuario_, opt => opt.MapFrom(src => src.Usuario1))
     .ForMember(dest => dest.Clave, opt => opt.MapFrom(src => src.Clave));

            CreateMap<UsuarioDM, Usuario>()
                .ConstructUsing(src => new Usuario
                {
                    IdUsuario = src.Id,
                    Nombre = src.Nombre,
                    Usuario1 = src.Usuario_,
                    Clave = src.Clave
                })
                .ForMember(dest => dest.Usuario_Familia, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario_Patente, opt => opt.Ignore());

        }



    }

}