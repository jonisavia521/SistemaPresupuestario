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
    internal sealed class UsuarioPatenteRepository : IJoinRepository<Usuario>
    {
        IExceptionBLL _iExceptionBLL;
        private SqlServerHelper _sqlHelper;

        //IGenericRepository<Patente> _patenteRepository;

        public UsuarioPatenteRepository(IExceptionBLL iExceptionBLL, SqlServerHelper sqlHelper /* IGenericRepository<Patente> patenteRepository*/)
        {
            _iExceptionBLL = iExceptionBLL;
            //_patenteRepository = patenteRepository;
            _sqlHelper = sqlHelper;
        
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
                using (var table = _sqlHelper.ExecuteReader("SELECT IdUsuario,IdPatente FROM Usuario_Patente WHERE IdUsuario = @IdUsuario", default, paramsSQL))
                {
                    if (table != null && table.Rows.Count > 0)
                    {
                        var patentes = (from row in table.AsEnumerable()
                                        select (Component)LoginFactory.patenteRepository.SelectOne(row.Field<Guid>("IdPatente"))).ToList();

                        obj.Permisos.AddRange(patentes);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Handle(this,_iExceptionBLL);
            }
        }
    }
}
