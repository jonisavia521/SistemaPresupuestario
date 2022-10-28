using Services.DAL.Contracts;
using Services.DAL.Factory;
using Services.DAL.Implementations.Joins;
using Services.DomainModel.Security.Composite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DAL.Implementations.Adapter
{


    internal sealed class UsuarioAdapter : IAdapter<Usuario>
    {
        //IJoinRepository<Usuario> _usuarioPatenteRepository;
        //IJoinRepository<Usuario> _usuarioFamiliaRepository;
        
        public UsuarioAdapter(/*IEnumerable<IJoinRepository<Usuario>> iJoinRepository*/)
        {
            //    _usuarioPatenteRepository = iJoinRepository.SingleOrDefault(s => s.GetType() == typeof(UsuarioPatenteRepository));            
            //    _usuarioFamiliaRepository = iJoinRepository.SingleOrDefault(s => s.GetType() == typeof(UsuarioFamiliaRepository));
        
        }
        public Usuario Adapt(DataRow row)
        {
            //Hidratar el objeto familia -> Nivel 1
            Usuario usuario = new Usuario()
            {
                Id = new Guid(row.Field<string>("IdUsuario")),
                Nombre = row.Field<string>("Nombre"),
                User = row.Field<string>("Usuario"),
                Password = row.Field<string>("Clave")
            };


            //Nivel 2 de hidratación...
            LoginFactory.usuarioFamiliaRepository.GetChildren(usuario);
            LoginFactory.usuarioPatenteRepository.GetChildren(usuario);

            return usuario;
        }
    }
}
