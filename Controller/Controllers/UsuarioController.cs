using AutoMapper;
using BLL.Contracts;
using Controller.Contracts;
using Controller.ViewModels;
using Controller.ViewModels.Mapper;
using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller.Controllers
{
    public class UsuarioController : ICRUDController<UsuarioView>
    {
        private readonly IGenericBusinessService<Usuario> usuarioBLL;
        private readonly IMapper mapper;

        public UsuarioController(IGenericBusinessService<Usuario> usuarioBLL, IMapper mapper)
        {
            this.usuarioBLL = usuarioBLL;
            this.mapper = mapper;
        }
        public void Add(UsuarioView obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UsuarioView> GetAll()
        {
            var UsuariosDTO = usuarioBLL.SelectAll().ToList();
            var UsuarioView = mapper.Map<List<Usuario>, List<UsuarioView>>(UsuariosDTO);

            return UsuarioView;
        }

        public UsuarioView GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public UsuarioView GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Remove(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(UsuarioView obj)
        {
            throw new NotImplementedException();
        }
    }
}
