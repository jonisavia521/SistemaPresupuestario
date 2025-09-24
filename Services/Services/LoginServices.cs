using Services.DAL.Contracts;
using Services.DAL.Factory;
using Services.DAL.Implementations;
using Services.DAL.Implementations.Joins;
using Services.DomainModel.Security.Composite;
using Services.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    internal class LoginServices : ILogin
    {
        
        public Usuario user { get; set; }

        public LoginServices(IGenericRepository<Familia> familiaRepository, IGenericRepository<Usuario> usuarioRepository, IGenericRepository<Patente> patenteRepository,
           IEnumerable<IJoinRepository<Usuario>> iJoinUsuarioRepository,

           IEnumerable<IJoinRepository<Familia>> iJoinFamiliaRepository)
        {
            LoginFactory.familiaRepository = familiaRepository;
            LoginFactory.usuarioRepository = usuarioRepository;
            LoginFactory.patenteRepository = patenteRepository;

            LoginFactory.usuarioPatenteRepository = iJoinUsuarioRepository.SingleOrDefault(s => s.GetType() == typeof(UsuarioPatenteRepository));
            LoginFactory.usuarioFamiliaRepository = iJoinUsuarioRepository.SingleOrDefault(s => s.GetType() == typeof(UsuarioFamiliaRepository));

            LoginFactory.familiaPatenteRepository = iJoinFamiliaRepository.SingleOrDefault(s => s.GetType() == typeof(FamiliaPatenteRepository));
            LoginFactory.familiaFamiliaRepository = iJoinFamiliaRepository.SingleOrDefault(s => s.GetType() == typeof(FamiliaFamiliaRepository));

        }

        

        public bool Login(string usuario , string clave )
        {
            Usuario login = new Usuario { User =usuario,Password = clave};
            this.user = LoginFactory.usuarioRepository.Fetch(login);
            return user != null;
        }

       public IEnumerable<Usuario> GetAll()
        {
            return LoginFactory.usuarioRepository.SelectAll();
        }
    }
}
