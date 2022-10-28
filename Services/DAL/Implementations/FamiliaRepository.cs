using Services.BLL.Contracts;
using Services.DAL.Contracts;
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

    internal sealed class FamiliaRepository : IGenericRepository<Familia>
    {
        IExceptionBLL _exceptionBLL;
        IAdapter<Familia> _familiaAdapter;
        public FamiliaRepository(IExceptionBLL exceptionBLL, IAdapter<Familia> familiaAdapter)
        {
            _exceptionBLL = exceptionBLL;
            _familiaAdapter = familiaAdapter;
        }

        public void Add(Familia obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Familia> SelectAll()
        {
            throw new NotImplementedException();
        }

        public Familia SelectOne(Guid id)
        {
            Familia familiaGet = null;

            try
            {
                var paramsSQL = new SqlParameter[] { new SqlParameter("@IdFamilia", id) };
                using (var table = SqlServerHelper.ExecuteReader("SELECT [IdFamilia],[Nombre] FROM Familia WHERE IdFamilia = @IdFamilia", default,paramsSQL))
                {                    

                    if (table != null && table.Rows.Count > 0)
                    {
                        var row = table.Rows[0];
                        familiaGet = _familiaAdapter.Adapt(row);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Handle(this,_exceptionBLL);
            }

            return familiaGet;
        }

        public void Update(Familia obj)
        {
            throw new NotImplementedException();
        }

        public Familia Fetch(Familia obj)
        {
            throw new NotImplementedException();
        }
    }

}
