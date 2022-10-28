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

    internal sealed class FamiliaPatenteRepository : IJoinRepository<Familia>
    {

        IExceptionBLL _iExceptionBLL;
        //IGenericRepository<Patente> _patenteRepository;
        
        public FamiliaPatenteRepository(IExceptionBLL iExceptionBLL/* IGenericRepository<Patente> patenteRepository*/)
        {
            _iExceptionBLL = iExceptionBLL;
            //_patenteRepository = patenteRepository;
        
        }
        public void Add(Familia obj)
        {
            foreach (var item in obj.GetChildrens())
            {
                //Verificar si los hijos son familia o patente...
                //Familia_Patente_Insert
                if (item.ChildrenCount() == 0)
                {

                }

            }
        }

        public void Delete(Familia obj)
        {
            //Familia_Patente_Delete
        }

        public void GetChildren(Familia obj)
        {
            //3) Buscar en SP Familia_Patente_Select y retornar id de patentes relacionados
            //4) Iterar sobre esos ids -> LLamar al Adaptador y cargar las patentes...          
            try
            {
                var paramsSQL = new SqlParameter[] { new SqlParameter("@IdFamilia", obj.IdComponent.ToString()) };
                using (var table = SqlServerHelper.ExecuteReader("SELECT [IdFamilia],[IdPatente] FROM Familia_Patente WHERE [IdFamilia] = @IdFamilia",default,paramsSQL))
                {
                    if (table != null && table.Rows.Count > 0)
                    {
                        var patentes = (from row in table.AsEnumerable()
                                        select (Component)LoginFactory.patenteRepository.SelectOne(Guid.Parse(row.Field<string>("IdPatente")))).ToList();

                        obj.Set(patentes);
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
