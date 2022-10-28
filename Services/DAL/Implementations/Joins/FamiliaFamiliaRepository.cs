using Services.DAL.Contracts;
using Services.DAL.Tools;
using Services.DomainModel.Security.Composite;
using Services.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Services.Services.Extensions;
using Services.BLL.Contracts;
using Services.DAL.Factory;

namespace Services.DAL.Implementations.Joins
{

    internal sealed class FamiliaFamiliaRepository : IJoinRepository<Familia>
    {
        IExceptionBLL _exceptionBLL;

        //IGenericRepository<Familia> _familiaRepository;
        
        public FamiliaFamiliaRepository(IExceptionBLL exceptionBLL  /*IGenericRepository<Familia> familiaRepository*/)
        {
            _exceptionBLL = exceptionBLL;
            //_familiaRepository = familiaRepository;
            
        }
        public void Add(Familia obj)
        {
            foreach (var item in obj.GetChildrens())
            {
                //Verificar si los hijos son familia o patente...
                //Familia_Familia_Insert
                if(item.ChildrenCount() > 0)
                {

                }
            }
        }

        public void Delete(Familia obj)
        {
            //Ejecutar el sp Familia_Familia_DeleteParticular

            throw new NotImplementedException();
        }

        //public void GetChildren(Familia obj)
        //{
        //    //1) Buscar en SP Familia_Familia_SelectParticular y retornar id de familias relacionados
        //    //2) Iterar sobre esos ids -> LLamar al Adaptador y cargar las familias...

        //    Familia familiaGet = null;

        //    try
        //    {
        //        using (var reader = SqlServerHelper.ExecuteReader("Select * from Familia_Familia where IdFamilia=@IdFamilia", 
        //                                                System.Data.CommandType.StoredProcedure,
        //                                                new SqlParameter[] {new SqlParameter("@IdFamilia", obj.IdComponent) }))
        //        {
        //            object[] values = new object[reader.FieldCount];

        //            while (reader.Read())
        //            {
        //                reader.GetValues(values);
        //                //Obtengo el id de familia relacionado a la familia principal...(obj)
        //                Guid idFamiliaHija = Guid.Parse(values[1].ToString());

        //                familiaGet = FamiliaRepository.Current.SelectOne(idFamiliaHija);

        //                obj.Add(familiaGet);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.Handle(this);
        //    }
        //}
        public void GetChildren(Familia obj)
        {
            //1) Buscar en SP Familia_Familia_SelectParticular y retornar id de familias relacionados
            //2) Iterar sobre esos ids -> LLamar al Adaptador y cargar las familias...   

            try
            {
                var paramsSQL = new SqlParameter[] { new SqlParameter("@IdFamilia", obj.IdComponent.ToString()) };
                using (var table = SqlServerHelper.ExecuteReader("Select * from Familia_Familia where IdFamilia=@IdFamilia", default, paramsSQL))
                {
                    if (table != null && table.Rows.Count > 0)
                    {
                        var familias = (from row in table.AsEnumerable()
                                        select (Component)LoginFactory.familiaRepository.SelectOne(Guid.Parse(row.Field<string>("IdFamiliaHijo")))).ToList();

                        obj.Set(familias);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Handle(this, _exceptionBLL);
            }
        }
    }
}
