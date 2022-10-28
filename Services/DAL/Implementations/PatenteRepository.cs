using Services.DAL.Contracts;
using Services.DomainModel.Security.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.DAL.Tools;
using System.Data.SqlClient;
using Services.Services.Extensions;
using Services.DAL.Implementations.Adapter;
using System.Data;
using Services.BLL.Contracts;

namespace Services.DAL.Implementations
{

    internal sealed class PatenteRepository : IGenericRepository<Patente>
    {
        IExceptionBLL _iExceptionBLL;
        IAdapter<Patente> _patenteAdapter;
        public PatenteRepository(IExceptionBLL iExceptionBLL, IAdapter<Patente> patenteAdapter)
        {
            _iExceptionBLL = iExceptionBLL;
            _patenteAdapter = patenteAdapter;
        }




        public void Add(Patente obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Patente> SelectAll()
        {   
            List<Patente> patentes = null;

            try
            {
                using (var table = SqlServerHelper.ExecuteReader("SELECT [IdPatente],[Nombre],[Vista] FROM Patente", default))
                {
                    if (table != null && table.Rows.Count > 0)
                    {
                        patentes = (from row in table.AsEnumerable()
                                        select _patenteAdapter.Adapt(row)).ToList();

                      
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Handle(this, _iExceptionBLL);
            }

            return patentes;
        }

        public Patente SelectOne(Guid id)
        {
            Patente patenteGet = null;

            try
            {
                var paramsSQL = new SqlParameter[] { new SqlParameter("@IdPatente", id) };
                using (var table = SqlServerHelper.ExecuteReader("SELECT [IdPatente],[Nombre],[Vista] FROM Patente WHERE IdPatente = @IdPatente", default,paramsSQL))
                {
                    if (table != null && table.Rows.Count > 0)
                    {
                        var row = table.Rows[0];
                        patenteGet = _patenteAdapter.Adapt(row);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Handle(this, _iExceptionBLL);
            }

            return patenteGet;
        }

        public void Update(Patente obj)
        {
            throw new NotImplementedException();
        }

        public Patente Fetch(Patente obj)
        {
            throw new NotImplementedException();
        }
    }
}
