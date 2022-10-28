using Services.DAL.Contracts;
using Services.DAL.Implementations.Joins;
using Services.DomainModel.Security.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DAL.Factory
{
    internal static class LoginFactory
    {
        public static IGenericRepository<Familia> familiaRepository;
        public static IGenericRepository<Usuario> usuarioRepository;
        public static IGenericRepository<Patente> patenteRepository;

        public static IJoinRepository<Usuario> usuarioFamiliaRepository;
        public static IJoinRepository<Usuario> usuarioPatenteRepository;
        public static IJoinRepository<Familia> familiaFamiliaRepository;
        public static IJoinRepository<Familia> familiaPatenteRepository;
        
    }
}