using Services.BLL.Contracts;
using Services.DAL.Contracts;
using Services.DAL.Factory;
using Services.DAL.Implementations.Adapter;
using Services.DAL.Tools;
using Services.DomainModel.Security.Composite;
using Services.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DAL.Implementations
{
    internal class UsuarioRepository:IGenericRepository<Usuario>
    {
        IExceptionBLL _iExceptionBLL;
        IAdapter<Usuario> _usuarioAdapter;
        private SqlServerHelper _sqlHelper;

        public UsuarioRepository(IExceptionBLL iExceptionBLL, IAdapter<Usuario> usuarioAdapter, SqlServerHelper sqlHelper)
        {
            _iExceptionBLL = iExceptionBLL;
            _usuarioAdapter=usuarioAdapter;
            _sqlHelper = sqlHelper;
        }

        public void Add(Usuario obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Usuario> SelectAll()
        {
            try
            {
                
                using (var table = _sqlHelper.ExecuteReader("select IdUsuario,Nombre,Usuario from Usuario "))
                {
                    if (table != null && table.Rows.Count > 0)
                    {
                        return table.AsEnumerable().Select(x => new Usuario {Id = x.Field<Guid>("IdUsuario"),Nombre= x.Field<string>("Nombre"),User= x.Field<string>("Usuario") });
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Handle(this, _iExceptionBLL);
            }

            return null;
        }

     
        public void Update(Usuario obj)
        {
            throw new NotImplementedException();
        }

        public Usuario SelectOne(Guid id)
        {
            throw new NotImplementedException();
        }

        public Usuario Fetch(Usuario obj)
        {
            Usuario usuario = null;
            try
            {
                var parametrosSQL = new SqlParameter[] { new SqlParameter("@Usuario", obj.User), new SqlParameter("@Clave", obj.HashPassword) };
                using (var table = _sqlHelper.ExecuteReader("select IdUsuario,Nombre,Usuario,Clave from Usuario where Usuario=@Usuario and Clave=@Clave", default, parametrosSQL))
                {
                    if (table != null && table.Rows.Count > 0)
                    {
                        var row = table.Rows[0];                    
                        usuario = _usuarioAdapter.Adapt(row);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Handle(this,_iExceptionBLL);
            }

            return usuario;
        }
    }
}
