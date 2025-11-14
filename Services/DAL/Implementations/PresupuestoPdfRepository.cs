using Services.DAL.Tools;
using Services.DAL.Tools.Enums;
using Services.DomainModel;
using Services.Services.Extensions;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Services.DAL.Implementations
{
    /// <summary>
    /// Repositorio para obtener datos de presupuestos usando ADO.NET.
    /// Utilizado exclusivamente para la generación de PDFs.
    /// </summary>
    internal class PresupuestoPdfRepository
    {
        private readonly SqlServerHelper _sqlHelper;

        public PresupuestoPdfRepository(SqlServerHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        /// <summary>
        /// Obtiene los datos completos de un presupuesto para generar el PDF
        /// </summary>
        public PresupuestoPdfModel ObtenerPresupuestoParaPdf(Guid idPresupuesto)
        {
            try
            {
                PresupuestoPdfModel presupuesto = null;

                string query = @"
                    SELECT 
                        p.ID,
                        p.Numero,
                        p.FechaEmision,
                        p.FechaVencimiento,
                        p.Estado,
                        p.Subtotal,
                        p.TotalIva,
                        p.ImporteArba,
                        p.Total,
                        c.CodigoCliente AS ClienteCodigo,
                        c.RazonSocial AS ClienteRazonSocial,
                        v.Nombre AS VendedorNombre
                    FROM Presupuesto p
                    INNER JOIN Cliente c ON p.IdCliente = c.ID
                    LEFT JOIN Vendedor v ON p.IdVendedor = v.ID
                    WHERE p.ID = @IdPresupuesto";

                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@IdPresupuesto", idPresupuesto)
                };
                
                _sqlHelper.setDataBase(enumDataBase.Huamani_SistemaPresupuestario);
                using (var table = _sqlHelper.ExecuteReader(query, CommandType.Text, parametros))
                {
                    if (table != null && table.Rows.Count > 0)
                    {
                        var row = table.Rows[0];

                        presupuesto = new PresupuestoPdfModel
                        {
                            Id = row.Field<Guid>("ID"),
                            Numero = row.Field<string>("Numero"),
                            FechaEmision = row.Field<DateTime>("FechaEmision"),
                            FechaVencimiento = row.Field<DateTime?>("FechaVencimiento"),
                            Estado = ObtenerTextoEstado(row.Field<int>("Estado")),
                            Subtotal = row.Field<decimal>("Subtotal"),
                            TotalIva = row.Field<decimal>("TotalIva"),
                            ImporteArba = row.Field<decimal>("ImporteArba"),
                            Total = row.Field<decimal>("Total"),
                            ClienteCodigo = row.Field<string>("ClienteCodigo"),
                            ClienteRazonSocial = row.Field<string>("ClienteRazonSocial"),
                            VendedorNombre = row.Field<string>("VendedorNombre")
                        };

                        presupuesto.Detalles = ObtenerDetalles(idPresupuesto).ToList();
                    }
                }

                return presupuesto;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener presupuesto para PDF: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene los detalles de un presupuesto
        /// </summary>
        private System.Collections.Generic.IEnumerable<PresupuestoDetallePdfModel> ObtenerDetalles(Guid idPresupuesto)
        {
            string query = @"
                SELECT 
                    pd.Renglon,
                    p.Codigo,
                    p.Descripcion,
                    pd.Cantidad,
                    pd.Precio,
                    pd.Descuento,
                    pd.Total
                FROM Presupuesto_Detalle pd
                INNER JOIN Producto p ON pd.IdProducto = p.ID
                WHERE pd.IdPresupuesto = @IdPresupuesto
                ORDER BY pd.Renglon";

            var parametros = new SqlParameter[]
            {
                new SqlParameter("@IdPresupuesto", idPresupuesto)
            };
            
            _sqlHelper.setDataBase(enumDataBase.Huamani_SistemaPresupuestario);
            using (var table = _sqlHelper.ExecuteReader(query, CommandType.Text, parametros))
            {
                if (table != null && table.Rows.Count > 0)
                {
                    return table.AsEnumerable().Select(row => new PresupuestoDetallePdfModel
                    {
                        Codigo = row.Field<string>("Codigo"),
                        Descripcion = row.Field<string>("Descripcion"),
                        Cantidad = row.Field<decimal?>("Cantidad") ?? 0,
                        Precio = row.Field<decimal?>("Precio") ?? 0,
                        Descuento = row.Field<decimal?>("Descuento") ?? 0,
                        Total = row.Field<decimal?>("Total") ?? 0
                    }).ToList();
                }
            }

            return Enumerable.Empty<PresupuestoDetallePdfModel>();
        }

        /// <summary>
        /// Convierte el código de estado a texto legible
        /// </summary>
        private string ObtenerTextoEstado(int estado)
        {
            switch (estado)
            {
                case 1: return "Borrado";
                case 2: return "Emitido";
                case 3: return "Aprobado";
                case 4: return "Rechazado";
                case 5: return "Vencido";
                case 6: return "Facturado";
                default: return "Desconocido";
            }
        }
    }
}
