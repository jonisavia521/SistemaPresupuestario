using Services.BLL.Contracts;
using Services.DAL.Contracts;
using Services.DAL.Factory;
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
            _sqlHelper.setDataBase(enumDataBase.Huamani_Seguridad);
        }

        public void Add(Usuario obj)
        {
            try
            {
                // El password ya viene hasheado desde la capa de servicio
                var parametrosSQL = new SqlParameter[]
                {
                    new SqlParameter("@IdUsuario", obj.Id == Guid.Empty ? Guid.NewGuid() : obj.Id),
                    new SqlParameter("@Nombre", obj.Nombre),
                    new SqlParameter("@Usuario", obj.User),
                    new SqlParameter("@Clave", obj.HashPassword) // Ya viene hasheado
                };

                // ExecuteScalar para INSERT
                _sqlHelper.ExecuteScalar(
                    "INSERT INTO Usuario (IdUsuario, Nombre, Usuario, Clave) VALUES (@IdUsuario, @Nombre, @Usuario, @Clave)",
                    CommandType.Text,
                    parametrosSQL
                );
            }
            catch (Exception ex)
            {
                ex.Handle(this, _iExceptionBLL);
            }
        }



        public IEnumerable<Usuario> SelectAll()
        {
            try
            {
                using (var table = _sqlHelper.ExecuteReader(
                    "SELECT IdUsuario, Nombre, Usuario FROM Usuario"))
                {
                    if (table != null && table.Rows.Count > 0)
                    {
                        // ✅ Crear usuarios SIN cargar permisos (más rápido para listados)
                        return table.AsEnumerable().Select(x => new Usuario
                        {
                            Id = x.Field<Guid>("IdUsuario"),
                            Nombre = x.Field<string>("Nombre"),
                            User = x.Field<string>("Usuario")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Handle(this, _iExceptionBLL);
            }

            return Enumerable.Empty<Usuario>();
        }



        public Usuario Fetch(Usuario obj)
        {
            Usuario usuario = null;
            try
            {
                var parametrosSQL = new SqlParameter[]
                {
                    new SqlParameter("@Usuario", obj.User),
                    new SqlParameter("@Clave", obj.HashPassword)
                };
                using (var table = _sqlHelper.ExecuteReader(
                    "SELECT IdUsuario, Nombre, Usuario, Clave FROM Usuario WHERE Usuario=@Usuario AND Clave=@Clave",
                    default,
                    parametrosSQL))
                {
                    if (table != null && table.Rows.Count > 0)
                    {
                        var row = table.Rows[0];
                        // ✅ El adaptador carga automáticamente los permisos
                        usuario = _usuarioAdapter.Adapt(row);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Handle(this, _iExceptionBLL);
            }

            return usuario;
        }

        public void Delete(Guid id)
        {
            try
            {
                var parametrosSQL = new SqlParameter[]
                {
                    new SqlParameter("@IdUsuario", id)
                };

                // ⚠️ IMPORTANTE: Primero eliminar relaciones en tablas intermedias
                // Si tienes FK con DELETE CASCADE, puedes omitir estos deletes

                // Eliminar relaciones Usuario_Familia
                _sqlHelper.ExecuteNonQuery(
                    "DELETE FROM Usuario_Familia WHERE IdUsuario = @IdUsuario",
                    CommandType.Text,
                    parametrosSQL
                );

                // Eliminar relaciones Usuario_Patente
                _sqlHelper.ExecuteNonQuery(
                    "DELETE FROM Usuario_Patente WHERE IdUsuario = @IdUsuario",
                    CommandType.Text,
                    parametrosSQL
                );

                // Finalmente eliminar el usuario
                _sqlHelper.ExecuteNonQuery(
                    "DELETE FROM Usuario WHERE IdUsuario = @IdUsuario",
                    CommandType.Text,
                    parametrosSQL
                );
            }
            catch (Exception ex)
            {
                ex.Handle(this, _iExceptionBLL);
            }
        }
        
        /// <summary>
        /// ✅ CORRECCIÓN DE SEGURIDAD CRÍTICA:
        /// Actualiza SOLO Nombre y Usuario, NUNCA la Clave
        /// 
        /// Para cambiar la contraseña, crear un método separado UpdatePassword()
        /// que requiera validación adicional (ej: contraseña actual)
        /// </summary>
        public void Update(Usuario obj)
        {
            try
            {
                // ✅ IMPORTANTE: NO incluir @Clave en el UPDATE
                // La contraseña solo se actualiza mediante un método específico de cambio de contraseña
                var parametrosSQL = new SqlParameter[]
                {
                    new SqlParameter("@IdUsuario", obj.Id),
                    new SqlParameter("@Nombre", obj.Nombre),
                    new SqlParameter("@Usuario", obj.User)
                };

                _sqlHelper.ExecuteNonQuery(
                    @"UPDATE Usuario 
                      SET Nombre = @Nombre, 
                          Usuario = @Usuario
                      WHERE IdUsuario = @IdUsuario",
                    CommandType.Text,
                    parametrosSQL
                );
            }
            catch (Exception ex)
            {
                ex.Handle(this, _iExceptionBLL);
            }
        }



        public Usuario SelectOne(Guid id)
        {
            Usuario usuario = null;
            try
            {
                var parametrosSQL = new SqlParameter[]
                {
                    new SqlParameter("@IdUsuario", id)
                };
                
                using (var table = _sqlHelper.ExecuteReader(
                    "SELECT IdUsuario, Nombre, Usuario, Clave FROM Usuario WHERE IdUsuario = @IdUsuario",
                    default,
                    parametrosSQL))
                {
                    if (table != null && table.Rows.Count > 0)
                    {
                        var row = table.Rows[0];
                        // ✅ El adaptador carga automáticamente los permisos (familias y patentes)
                        usuario = _usuarioAdapter.Adapt(row);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Handle(this, _iExceptionBLL);
            }

            return usuario;
        }
    }
}
