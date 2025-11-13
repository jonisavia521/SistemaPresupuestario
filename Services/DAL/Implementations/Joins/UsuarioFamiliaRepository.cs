using Services.BLL.Contracts;
using Services.DAL.Contracts;
using Services.DAL.Factory;
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

namespace Services.DAL.Implementations.Joins
{
    internal sealed class UsuarioFamiliaRepository : IJoinRepository<Usuario>
    {
       
        IExceptionBLL _iExceptionBLL;
        private SqlServerHelper _sqlHelper;

        //IGenericRepository<Familia> _familiaRepository;

        public UsuarioFamiliaRepository(IExceptionBLL iExceptionBLL , SqlServerHelper sqlHelper/*IGenericRepository<Familia> familiaRepository*/)
        {
            _iExceptionBLL = iExceptionBLL;
            //_familiaRepository = familiaRepository;
            _sqlHelper = sqlHelper;
            _sqlHelper.setDataBase(enumDataBase.Huamani_Seguridad);
        }

        public void Add(Usuario obj)
        {
            try
            {
                // 1️⃣ Primero eliminar todas las familias actuales del usuario
                var deleteParams = new SqlParameter[]
                {
                    new SqlParameter("@IdUsuario", obj.Id)
                };

                _sqlHelper.ExecuteNonQuery(
                    "DELETE FROM Usuario_Familia WHERE IdUsuario = @IdUsuario",
                    CommandType.Text,
                    deleteParams
                );

                // 2️⃣ Insertar las nuevas familias seleccionadas
                foreach (var permiso in obj.Permisos.Where(p => p is Familia))
                {
                    var familia = permiso as Familia;
                    var insertParams = new SqlParameter[]
                    {
                        new SqlParameter("@IdUsuario", obj.Id),
                        new SqlParameter("@IdFamilia", familia.IdComponent)
                    };

                    _sqlHelper.ExecuteNonQuery(
                        "INSERT INTO Usuario_Familia (IdUsuario, IdFamilia) VALUES (@IdUsuario, @IdFamilia)",
                        CommandType.Text,
                        insertParams
                    );
                }
            }
            catch (Exception ex)
            {
                ex.Handle(this, _iExceptionBLL);
            }
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
                
                using (var table = _sqlHelper.ExecuteReader("SELECT IdUsuario,IdFamilia FROM Usuario_familia WHERE IdUsuario = @IdUsuario", default, paramsSQL))
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
