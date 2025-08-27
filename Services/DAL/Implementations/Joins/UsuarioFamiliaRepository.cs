using Services.BLL.Contracts;
using Services.DAL.Contracts;
using Services.DAL.Factory;
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

namespace Services.DAL.Implementations.Joins
{
    internal sealed class UsuarioFamiliaRepository : IJoinRepository<Usuario>
    {
       
        IExceptionBLL _iExceptionBLL;

        //IGenericRepository<Familia> _familiaRepository;
        
        public UsuarioFamiliaRepository(IExceptionBLL iExceptionBLL /*IGenericRepository<Familia> familiaRepository*/)
        {
            _iExceptionBLL = iExceptionBLL;
            //_familiaRepository = familiaRepository;
        
        }
       
        public void Add(Usuario obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(Usuario obj)
        {
            throw new NotImplementedException();
        }

        public void GetChildren(Usuario obj)
        {
            try
            {
                var paramsSQL = new SqlParameter[] { new SqlParameter("@IdUsuario", obj.Id.ToString()) };
                using (var table = SqlServerHelper.ExecuteReader("SELECT IdUsuario,IdFamilia FROM Usuario_familia WHERE IdUsuario = @IdUsuario", default, paramsSQL))
                {
                    if (table != null && table.Rows.Count > 0)
                    {
                        var familias = (from row in table.AsEnumerable()
                                        select (Component)LoginFactory.familiaRepository.SelectOne(row.Field<Guid>("IdFamilia"))).ToList();

                        obj.Permisos.AddRange(familias);                        
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Handle(this, _iExceptionBLL);
            }
        }
    }
}
