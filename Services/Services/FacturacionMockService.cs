using Services.DAL.Implementations;
using Services.DAL.Tools;
using Services.DAL.Tools.Enums;
using Services.DomainModel;
using Services.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Services.Services
{
    /// <summary>
    /// Servicio MOCK para simular facturación electrónica con AFIP
    /// Genera facturas con CAE simulado para presupuestos aprobados
    /// </summary>
    internal class FacturacionMockService : IFacturacionService
    {
        private readonly SqlServerHelper _sqlHelper;
        private readonly IPresupuestoPdfService _pdfService;

        public FacturacionMockService(SqlServerHelper sqlHelper, IPresupuestoPdfService pdfService)
        {
            _sqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof(sqlHelper));
            _pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
            _sqlHelper.setDataBase(enumDataBase.Huamani_SistemaPresupuestario);
        }

        /// <summary>
        /// Genera una factura MOCK con CAE simulado para uno o varios presupuestos
        /// </summary>
        public FacturaModel GenerarFacturaConCAE(Guid[] idsPresupuestos)
        {
            if (idsPresupuestos == null || idsPresupuestos.Length == 0)
                throw new ArgumentException("Debe especificar al menos un presupuesto para facturar", nameof(idsPresupuestos));

            try
            {
                // Cargar datos de presupuestos desde la base de datos
                var presupuestos = CargarPresupuestos(idsPresupuestos);

                if (presupuestos.Count == 0)
                    throw new Exception("No se encontraron presupuestos válidos");

                // Validar que todos los presupuestos sean del mismo cliente
                var idCliente = presupuestos[0].IdCliente;
                if (presupuestos.Any(p => p.IdCliente != idCliente))
                    throw new Exception("Todos los presupuestos deben ser del mismo cliente");

                // Validar que todos los presupuestos estén en estado Aprobado (3)
                if (presupuestos.Any(p => p.Estado != 3))
                    throw new Exception("Solo se pueden facturar presupuestos en estado Aprobado");

                // Cargar datos del cliente
                var cliente = CargarCliente(idCliente);

                // Cargar datos de la empresa (configuración del sistema)
                var empresa = CargarDatosEmpresa();

                // Crear modelo de factura
                var factura = new FacturaModel
                {
                    Exitosa = true,
                    FechaEmision = DateTime.Now,
                    TipoFactura = DeterminarTipoFactura(cliente.TipoIva),
                    PuntoVenta = 1, // MOCK: Punto de venta fijo
                    NumeroDesde = GenerarNumeroFactura(), // MOCK: Número secuencial
                    ErrorMessage = null
                };

                factura.NumeroHasta = factura.NumeroDesde; // Una sola factura
                factura.NumeroFactura = $"A{factura.PuntoVenta:D5}-{factura.NumeroDesde:D8}";

                // MOCK: Generar CAE simulado (14 dígitos)
                factura.CAE = GenerarCAESimulado();
                factura.VencimientoCae = DateTime.Now.AddDays(10); // MOCK: Vencimiento 10 días

                // Datos de la empresa
                factura.EmpresaRazonSocial = empresa.RazonSocial;
                factura.EmpresaCUIT = empresa.CUIT;
                factura.EmpresaDireccion = empresa.Direccion ?? "";
                factura.EmpresaLocalidad = empresa.Localidad ?? "";
                factura.EmpresaProvincia = empresa.Provincia ?? "";
                factura.EmpresaTelefono = empresa.Telefono ?? "";
                factura.EmpresaEmail = empresa.Email ?? "";
                factura.EmpresaCondicionIva = empresa.TipoIva;
                factura.EmpresaInicioActividades = DateTime.Now.AddYears(-5); // MOCK

                // Datos del cliente
                factura.ClienteCodigo = cliente.CodigoCliente ?? "";
                factura.ClienteRazonSocial = cliente.RazonSocial;
                factura.ClienteCUIT = cliente.NumeroDocumento;
                factura.ClienteDireccion = cliente.Direccion ?? "";
                factura.ClienteLocalidad = cliente.Localidad ?? "";
                factura.ClienteProvincia = cliente.Provincia ?? "";
                factura.ClienteCondicionIva = cliente.TipoIva;
                factura.ClienteCondicionPago = ObtenerDescripcionCondicionPago(cliente.CondicionPago);

                // Datos del vendedor (tomar del primer presupuesto)
                factura.VendedorNombre = presupuestos[0].VendedorNombre ?? "";

                // Acumular presupuestos
                factura.NumerosPresupuestos = presupuestos.Select(p => p.Numero).ToList();

                // Acumular detalles (agrupar por código de producto)
                var detallesAgrupados = AcumularDetalles(presupuestos);
                factura.Detalles = detallesAgrupados;

                // Calcular totales acumulados
                factura.Subtotal = presupuestos.Sum(p => p.Subtotal);
                factura.TotalIva = presupuestos.Sum(p => p.TotalIva);
                factura.ImporteArba = presupuestos.Sum(p => p.ImporteArba);
                factura.Total = presupuestos.Sum(p => p.Total);

                // MOCK: Generar código de barras y QR (simplificado)
                factura.CodigoBarras = GenerarCodigoBarras(factura);
                factura.QRCodeData = GenerarDatosQR(factura);

                return factura;
            }
            catch (Exception ex)
            {
                // En caso de error, retornar factura fallida
                return new FacturaModel
                {
                    Exitosa = false,
                    ErrorMessage = $"Error al generar factura: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Genera el PDF de la factura y lo guarda en el sistema
        /// </summary>
        public string GenerarPdfFactura(FacturaModel factura)
        {
            if (factura == null)
                throw new ArgumentNullException(nameof(factura));

            if (!factura.Exitosa)
                throw new Exception($"No se puede generar PDF de una factura fallida: {factura.ErrorMessage}");

            // Delegar al servicio de PDF de facturas
            var pdfService = new FacturaPdfService(_sqlHelper);
            return pdfService.GenerarPdf(factura);
        }

        /// <summary>
        /// Genera el PDF de la factura y lo abre automáticamente
        /// </summary>
        public void GenerarYAbrirPdfFactura(FacturaModel factura)
        {
            string rutaPdf = GenerarPdfFactura(factura);
            System.Diagnostics.Process.Start(rutaPdf);
        }

        // ==================== MÉTODOS PRIVADOS ====================

        private List<PresupuestoParaFacturar> CargarPresupuestos(Guid[] ids)
        {
            var presupuestos = new List<PresupuestoParaFacturar>();

            foreach (var id in ids)
            {
                string query = @"
                    SELECT 
                        p.Id,
                        p.Numero,
                        p.IdCliente,
                        p.Estado,
                        p.Subtotal,
                        p.TotalIva,
                        p.ImporteArba,
                        p.Total,
                        v.Nombre as VendedorNombre
                    FROM Presupuesto p
                    LEFT JOIN Vendedor v ON p.IdVendedor = v.Id
                    WHERE p.Id = @Id";

                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@Id", id)
                };

                using (var table = _sqlHelper.ExecuteReader(query, CommandType.Text, parametros))
                {
                    if (table != null && table.Rows.Count > 0)
                    {
                        var row = table.Rows[0];

                        var presupuesto = new PresupuestoParaFacturar
                        {
                            Id = (Guid)row["Id"],
                            Numero = (string)row["Numero"],
                            IdCliente = (Guid)row["IdCliente"],
                            Estado = (int)row["Estado"],
                            Subtotal = (decimal)row["Subtotal"],
                            TotalIva = (decimal)row["TotalIva"],
                            ImporteArba = (decimal)row["ImporteArba"],
                            Total = (decimal)row["Total"],
                            VendedorNombre = row["VendedorNombre"] == DBNull.Value 
                                ? null 
                                : (string)row["VendedorNombre"],
                            Detalles = new List<DetalleParaFacturar>()
                        };

                        presupuestos.Add(presupuesto);

                        // Cargar detalles del presupuesto
                        presupuesto.Detalles = CargarDetallesPresupuesto(id);
                    }
                }
            }

            return presupuestos;
        }

        private List<DetalleParaFacturar> CargarDetallesPresupuesto(Guid idPresupuesto)
        {
            var detalles = new List<DetalleParaFacturar>();

            string query = @"
                SELECT 
                    pd.Cantidad,
                    pd.Precio,
                    pd.Descuento,
                    p.PorcentajeIVA,
                    pd.Total,
                    p.Codigo,
                    p.Descripcion
                FROM Presupuesto_Detalle pd
                INNER JOIN Producto p ON pd.IdProducto = p.Id
                WHERE pd.IdPresupuesto = @IdPresupuesto
                ORDER BY pd.Renglon";

            var parametros = new SqlParameter[]
            {
                new SqlParameter("@IdPresupuesto", idPresupuesto)
            };

            using (var table = _sqlHelper.ExecuteReader(query, CommandType.Text, parametros))
            {
                if (table != null && table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        detalles.Add(new DetalleParaFacturar
                        {
                            Codigo = (string)row["Codigo"],
                            Descripcion = (string)row["Descripcion"],
                            Cantidad = (decimal)row["Cantidad"],
                            PrecioUnitario = (decimal)row["Precio"],
                            Descuento = (decimal)row["Descuento"],
                            PorcentajeIVA = (decimal)row["PorcentajeIVA"],
                            Total = (decimal)row["Total"]
                        });
                    }
                }
            }

            return detalles;
        }

        private ClienteParaFacturar CargarCliente(Guid idCliente)
        {
            string query = @"
                SELECT 
                    c.CodigoCliente,
                    c.RazonSocial,
                    c.CUIT as NumeroDocumento,
                    c.TipoIva,
                    c.DireccionLegal as Direccion,
                    c.Localidad,
                    c.CondicionPago,
                    p.Nombre as Provincia
                FROM Cliente c
                LEFT JOIN Provincia p ON c.IdProvincia = p.Id
                WHERE c.Id = @Id";

            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Id", idCliente)
            };

            using (var table = _sqlHelper.ExecuteReader(query, CommandType.Text, parametros))
            {
                if (table != null && table.Rows.Count > 0)
                {
                    var row = table.Rows[0];

                    return new ClienteParaFacturar
                    {
                        CodigoCliente = row["CodigoCliente"] == DBNull.Value 
                            ? null 
                            : (string)row["CodigoCliente"],
                        RazonSocial = (string)row["RazonSocial"],
                        NumeroDocumento = (string)row["NumeroDocumento"],
                        TipoIva = (string)row["TipoIva"],
                        Direccion = row["Direccion"] == DBNull.Value 
                            ? null 
                            : (string)row["Direccion"],
                        Localidad = row["Localidad"] == DBNull.Value 
                            ? null 
                            : (string)row["Localidad"],
                        CondicionPago = row["CondicionPago"] == DBNull.Value 
                            ? null 
                            : (string)row["CondicionPago"],
                        Provincia = row["Provincia"] == DBNull.Value 
                            ? null 
                            : (string)row["Provincia"]
                    };
                }
            }

            throw new Exception($"No se encontró el cliente con ID {idCliente}");
        }

        private EmpresaParaFacturar CargarDatosEmpresa()
        {
            string query = @"
                SELECT 
                    c.RazonSocial,
                    c.CUIT,
                    c.TipoIva,
                    c.Direccion,
                    c.Localidad,
                    c.Email,
                    c.Telefono,
                    p.Nombre as Provincia
                FROM Configuracion c
                LEFT JOIN Provincia p ON c.IdProvincia = p.Id";

            using (var table = _sqlHelper.ExecuteReader(query, CommandType.Text))
            {
                if (table != null && table.Rows.Count > 0)
                {
                    var row = table.Rows[0];

                    return new EmpresaParaFacturar
                    {
                        RazonSocial = (string)row["RazonSocial"],
                        CUIT = (string)row["CUIT"],
                        TipoIva = (string)row["TipoIva"],
                        Direccion = row["Direccion"] == DBNull.Value 
                            ? null 
                            : (string)row["Direccion"],
                        Localidad = row["Localidad"] == DBNull.Value 
                            ? null 
                            : (string)row["Localidad"],
                        Email = row["Email"] == DBNull.Value 
                            ? null 
                            : (string)row["Email"],
                        Telefono = row["Telefono"] == DBNull.Value 
                            ? null 
                            : (string)row["Telefono"],
                        Provincia = row["Provincia"] == DBNull.Value 
                            ? null 
                            : (string)row["Provincia"]
                    };
                }
            }

            throw new Exception("No se encontró la configuración del sistema. Configure los datos de la empresa primero.");
        }

        private List<FacturaDetalleModel> AcumularDetalles(List<PresupuestoParaFacturar> presupuestos)
        {
            // Agrupar detalles por código de producto
            var detallesAgrupados = new Dictionary<string, FacturaDetalleModel>();

            foreach (var presupuesto in presupuestos)
            {
                foreach (var detalle in presupuesto.Detalles)
                {
                    if (detallesAgrupados.ContainsKey(detalle.Codigo))
                    {
                        // Acumular cantidad y total
                        var detalleExistente = detallesAgrupados[detalle.Codigo];
                        detalleExistente.Cantidad += detalle.Cantidad;
                        detalleExistente.Total += detalle.Total;
                    }
                    else
                    {
                        // Agregar nuevo detalle
                        detallesAgrupados[detalle.Codigo] = new FacturaDetalleModel
                        {
                            Codigo = detalle.Codigo,
                            Descripcion = detalle.Descripcion,
                            Cantidad = detalle.Cantidad,
                            PrecioUnitario = detalle.PrecioUnitario,
                            Descuento = detalle.Descuento,
                            PorcentajeIVA = detalle.PorcentajeIVA,
                            Total = detalle.Total
                        };
                    }
                }
            }

            return detallesAgrupados.Values.ToList();
        }

        private string DeterminarTipoFactura(string condicionIvaCliente)
        {
            // Lógica para determinar tipo de factura según condición de IVA
            // Regla: Responsable Inscripto ? Factura A, Resto ? Factura B

            if (string.IsNullOrWhiteSpace(condicionIvaCliente))
                return "B"; // Por defecto si no hay condición

            var condicion = condicionIvaCliente.ToUpper().Trim();

            // Si el cliente es Responsable Inscripto ? Factura A
            if (condicion.Contains("RESPONSABLE INSCRIPTO") || 
                condicion.Contains("INSCRIPTO") ||
                condicion == "RESPONSABLE INSCRIPTO")
            {
                return "A";
            }

            // Cualquier otra condición (Monotributo, Exento, Consumidor Final, etc.) ? Factura B
            return "B";
        }

        private int GenerarNumeroFactura()
        {
            // MOCK: Generar número secuencial simple
            // En producción, esto debe obtenerse del último número de factura + 1
            return new Random().Next(1, 999999);
        }

        private string GenerarCAESimulado()
        {
            // MOCK: Generar CAE de 14 dígitos
            var random = new Random();
            var cae = "";
            for (int i = 0; i < 14; i++)
            {
                cae += random.Next(0, 10).ToString();
            }
            return cae;
        }

        private string GenerarCodigoBarras(FacturaModel factura)
        {
            // MOCK: Formato simplificado del código de barras AFIP
            // En producción, esto debe generarse según especificaciones de AFIP
            return $"{factura.EmpresaCUIT}{factura.TipoFactura}{factura.PuntoVenta:D4}{factura.CAE}";
        }

        private string GenerarDatosQR(FacturaModel factura)
        {
            // MOCK: Datos para código QR
            // En producción, esto debe incluir URL de validación de AFIP
            return $"https://www.afip.gob.ar/fe/qr/?p={Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(factura.CAE))}";
        }

        private string ObtenerDescripcionCondicionPago(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return "Contado";

            var descripciones = new Dictionary<string, string>
            {
                { "01", "Contado" },
                { "02", "30 días" },
                { "03", "60 días" },
                { "04", "90 días" },
                { "05", "120 días" }
            };

            return descripciones.ContainsKey(codigo) ? descripciones[codigo] : codigo;
        }

        // ==================== CLASES AUXILIARES ====================

        private class PresupuestoParaFacturar
        {
            public Guid Id { get; set; }
            public string Numero { get; set; }
            public Guid IdCliente { get; set; }
            public int Estado { get; set; }
            public decimal Subtotal { get; set; }
            public decimal TotalIva { get; set; }
            public decimal ImporteArba { get; set; }
            public decimal Total { get; set; }
            public string VendedorNombre { get; set; }
            public List<DetalleParaFacturar> Detalles { get; set; }
        }

        private class DetalleParaFacturar
        {
            public string Codigo { get; set; }
            public string Descripcion { get; set; }
            public decimal Cantidad { get; set; }
            public decimal PrecioUnitario { get; set; }
            public decimal Descuento { get; set; }
            public decimal PorcentajeIVA { get; set; }
            public decimal Total { get; set; }
        }

        private class ClienteParaFacturar
        {
            public string CodigoCliente { get; set; }
            public string RazonSocial { get; set; }
            public string NumeroDocumento { get; set; }
            public string TipoIva { get; set; }
            public string Direccion { get; set; }
            public string Localidad { get; set; }
            public string Provincia { get; set; }
            public string CondicionPago { get; set; }
        }

        private class EmpresaParaFacturar
        {
            public string RazonSocial { get; set; }
            public string CUIT { get; set; }
            public string TipoIva { get; set; }
            public string Direccion { get; set; }
            public string Localidad { get; set; }
            public string Provincia { get; set; }
            public string Telefono { get; set; }
            public string Email { get; set; }
        }
    }
}
