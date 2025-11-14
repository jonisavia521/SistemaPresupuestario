using Services.BLL.Contracts;
using Services.DAL.Contracts;
using Services.DAL.Implementations.Adapter;
using Services.DAL.Tools;
using Services.DAL.Tools.Enums;
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
        private SqlServerHelper _sqlHelper;

        public FamiliaRepository(IExceptionBLL exceptionBLL, IAdapter<Familia> familiaAdapter, SqlServerHelper sqlHelper)
        {
            _exceptionBLL = exceptionBLL;
            _familiaAdapter = familiaAdapter;
            _sqlHelper = sqlHelper;
            
        }

        public void Add(Familia obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Obtiene todas las familias del sistema para mostrar en controles UI
        /// </summary>
        public IEnumerable<Familia> SelectAll()
        {
            List<Familia> familias = new List<Familia>();

            try
            {
                // ✅ Obtener todas las familias de la base de datos
                _sqlHelper.setDataBase(enumDataBase.Huamani_Seguridad);
                using (var table = _sqlHelper.ExecuteReader(
                    "SELECT [IdFamilia],[Nombre],[Vista] FROM Familia",
                    default))
                {
                    if (table != null && table.Rows.Count > 0)
                    {
                        // ✅ Adaptar cada fila a un objeto Familia
                        // NOTA: El adaptador carga automáticamente los hijos (familias y patentes)
                        familias = (from row in table.AsEnumerable()
                                    select _familiaAdapter.Adapt(row)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Handle(this, _exceptionBLL);
            }

            return familias;
        }

        public Familia SelectOne(Guid id)
        {
            Familia familiaGet = null;

            try
            {
                var paramsSQL = new SqlParameter[] { new SqlParameter("@IdFamilia", id) };
                _sqlHelper.setDataBase(enumDataBase.Huamani_Seguridad);
                using (var table = _sqlHelper.ExecuteReader("SELECT [IdFamilia],[Nombre],[Vista] FROM Familia WHERE IdFamilia = @IdFamilia", default,paramsSQL))
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
