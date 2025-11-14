using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextFont = iTextSharp.text.Font;
using iTextRectangle = iTextSharp.text.Rectangle;
using QRCoder;
using Services.DAL.Tools;
using Services.DomainModel;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Services.Services
{
    /// <summary>
    /// Servicio para generar PDFs de facturas con formato legal
    /// Simula una factura tipo A, B o C con todos los datos obligatorios
    /// </summary>
    internal class FacturaPdfService
    {
        private readonly SqlServerHelper _sqlHelper;
        private readonly iTextFont _fontTitle;
        private readonly iTextFont _fontSubtitle;
        private readonly iTextFont _fontNormal;
        private readonly iTextFont _fontBold;
        private readonly iTextFont _fontSmall;
        private readonly iTextFont _fontTiny;
        private readonly iTextFont _fontCAE;

        public FacturaPdfService(SqlServerHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;

            // Configurar fuentes
            _fontTitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 22, BaseColor.BLACK);
            _fontSubtitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLACK);
            _fontNormal = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.BLACK);
            _fontBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.BLACK);
            _fontSmall = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.BLACK);
            _fontTiny = FontFactory.GetFont(FontFactory.HELVETICA, 7, BaseColor.GRAY);
            _fontCAE = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.BLACK);
        }

        /// <summary>
        /// Genera un PDF de la factura con formato legal
        /// </summary>
        public string GenerarPdf(FacturaModel factura)
        {
            if (factura == null)
                throw new ArgumentNullException(nameof(factura));

            // Obtener ruta del sistema
            string rutaSistema = AppDomain.CurrentDomain.BaseDirectory;
            string carpetaFacturas = Path.Combine(rutaSistema, "Facturas");

            // Crear carpeta si no existe
            if (!Directory.Exists(carpetaFacturas))
            {
                Directory.CreateDirectory(carpetaFacturas);
            }

            // Generar nombre de archivo único
            string nombreArchivo = $"Factura_{factura.NumeroFactura.Replace("/", "-")}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string rutaCompleta = Path.Combine(carpetaFacturas, nombreArchivo);

            // Crear documento PDF con márgenes más amplios
            Document document = new Document(PageSize.A4, 40, 40, 30, 30);

            try
            {
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(rutaCompleta, FileMode.Create));
                document.Open();

                // Agregar contenido
                AgregarEncabezadoFactura(document, factura);
                AgregarEspacio(document, 15);
                AgregarDatosEmisorReceptor(document, factura);
                AgregarEspacio(document, 15);
                AgregarTablaDetalles(document, factura);
                AgregarEspacio(document, 15);
                AgregarTotales(document, factura);
                AgregarEspacio(document, 20);
                AgregarCAE(document, factura);
                AgregarEspacio(document, 15);
                AgregarPiePagina(document, factura);

                document.Close();

                return rutaCompleta;
            }
            catch (Exception ex)
            {
                document.Close();
                throw new Exception($"Error al generar el PDF de la factura: {ex.Message}", ex);
            }
        }

        private void AgregarEncabezadoFactura(Document document, FacturaModel factura)
        {
            // Crear tabla con 3 columnas para el encabezado
            PdfPTable tableHeader = new PdfPTable(3);
            tableHeader.WidthPercentage = 100;
            tableHeader.SetWidths(new float[] { 2f, 1f, 2f });

            // Columna IZQUIERDA: Datos del emisor
            PdfPCell cellEmisor = new PdfPCell();
            cellEmisor.Border = iTextRectangle.BOX;
            cellEmisor.Padding = 8;
            cellEmisor.AddElement(new Paragraph(factura.EmpresaRazonSocial, _fontBold));
            cellEmisor.AddElement(new Paragraph($"CUIT: {FormatearCUIT(factura.EmpresaCUIT)}", _fontSmall));
            cellEmisor.AddElement(new Paragraph($"Condición IVA: {factura.EmpresaCondicionIva}", _fontSmall));
            cellEmisor.AddElement(new Paragraph($"{factura.EmpresaDireccion}", _fontSmall));
            cellEmisor.AddElement(new Paragraph($"{factura.EmpresaLocalidad} - {factura.EmpresaProvincia}", _fontSmall));
            if (!string.IsNullOrWhiteSpace(factura.EmpresaTelefono))
                cellEmisor.AddElement(new Paragraph($"Tel: {factura.EmpresaTelefono}", _fontSmall));
            if (!string.IsNullOrWhiteSpace(factura.EmpresaEmail))
                cellEmisor.AddElement(new Paragraph($"Email: {factura.EmpresaEmail}", _fontSmall));

            // Columna CENTRAL: Tipo de factura (A, B, C) en grande
            PdfPCell cellTipo = new PdfPCell();
            cellTipo.Border = iTextRectangle.BOX;
            cellTipo.HorizontalAlignment = Element.ALIGN_CENTER;
            cellTipo.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellTipo.Padding = 10;
            cellTipo.BackgroundColor = new BaseColor(220, 220, 220);

            Paragraph tipoFactura = new Paragraph(factura.TipoFactura, _fontTitle);
            tipoFactura.Alignment = Element.ALIGN_CENTER;
            cellTipo.AddElement(tipoFactura);

            Paragraph codigoFactura = new Paragraph("COD. 01", _fontSmall);
            codigoFactura.Alignment = Element.ALIGN_CENTER;
            cellTipo.AddElement(codigoFactura);

            // Columna DERECHA: Datos de la factura
            PdfPCell cellFactura = new PdfPCell();
            cellFactura.Border = iTextRectangle.BOX;
            cellFactura.Padding = 8;

            Paragraph tituloFactura = new Paragraph("FACTURA", _fontSubtitle);
            tituloFactura.Alignment = Element.ALIGN_CENTER;
            cellFactura.AddElement(tituloFactura);

            cellFactura.AddElement(new Paragraph($"Número: {factura.NumeroFactura}", _fontBold));
            cellFactura.AddElement(new Paragraph($"Fecha: {factura.FechaEmision:dd/MM/yyyy}", _fontNormal));
            cellFactura.AddElement(new Paragraph($"Punto de Venta: {factura.PuntoVenta:D5}", _fontSmall));

            if (factura.EmpresaInicioActividades.HasValue)
                cellFactura.AddElement(new Paragraph($"Inicio Act.: {factura.EmpresaInicioActividades.Value:dd/MM/yyyy}", _fontTiny));

            tableHeader.AddCell(cellEmisor);
            tableHeader.AddCell(cellTipo);
            tableHeader.AddCell(cellFactura);

            document.Add(tableHeader);
        }

        private void AgregarDatosEmisorReceptor(Document document, FacturaModel factura)
        {
            // Datos del cliente (receptor)
            PdfPTable tableCliente = new PdfPTable(1);
            tableCliente.WidthPercentage = 100;

            PdfPCell cellCliente = new PdfPCell();
            cellCliente.Border = iTextRectangle.BOX;
            cellCliente.Padding = 8;
            cellCliente.BackgroundColor = new BaseColor(245, 245, 245);

            cellCliente.AddElement(new Paragraph("DATOS DEL CLIENTE", _fontBold));
            cellCliente.AddElement(new Paragraph($"Razón Social: {factura.ClienteRazonSocial}", _fontNormal));
            cellCliente.AddElement(new Paragraph($"CUIT/DNI: {FormatearCUIT(factura.ClienteCUIT)}", _fontNormal));
            cellCliente.AddElement(new Paragraph($"Condición IVA: {factura.ClienteCondicionIva}", _fontNormal));
            cellCliente.AddElement(new Paragraph($"Domicilio: {factura.ClienteDireccion}", _fontNormal));
            cellCliente.AddElement(new Paragraph($"Localidad: {factura.ClienteLocalidad} - {factura.ClienteProvincia}", _fontNormal));
            cellCliente.AddElement(new Paragraph($"Condición de Pago: {factura.ClienteCondicionPago}", _fontNormal));

            if (!string.IsNullOrWhiteSpace(factura.VendedorNombre))
                cellCliente.AddElement(new Paragraph($"Vendedor: {factura.VendedorNombre}", _fontSmall));

            // Presupuestos origen
            if (factura.NumerosPresupuestos != null && factura.NumerosPresupuestos.Count > 0)
            {
                string presupuestosStr = string.Join(", ", factura.NumerosPresupuestos);
                cellCliente.AddElement(new Paragraph($"Presupuestos origen: {presupuestosStr}", _fontTiny));
            }

            tableCliente.AddCell(cellCliente);
            document.Add(tableCliente);
        }

        private void AgregarTablaDetalles(Document document, FacturaModel factura)
        {
            // Crear tabla con 7 columnas
            PdfPTable table = new PdfPTable(7);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 1.2f, 3f, 0.8f, 1.2f, 0.8f, 1.2f, 1.2f });

            // Encabezados
            AgregarCeldaEncabezado(table, "Código");
            AgregarCeldaEncabezado(table, "Descripción");
            AgregarCeldaEncabezado(table, "Cant.");
            AgregarCeldaEncabezado(table, "Precio Unit.");
            AgregarCeldaEncabezado(table, "Desc %");
            AgregarCeldaEncabezado(table, "Subtotal");
            AgregarCeldaEncabezado(table, "Total");

            // Detalles
            if (factura.Detalles != null)
            {
                foreach (var detalle in factura.Detalles)
                {
                    AgregarCelda(table, detalle.Codigo ?? "N/A", Element.ALIGN_LEFT);
                    AgregarCelda(table, detalle.Descripcion ?? "N/A", Element.ALIGN_LEFT);
                    AgregarCelda(table, detalle.Cantidad.ToString("N2"), Element.ALIGN_RIGHT);
                    AgregarCelda(table, detalle.PrecioUnitario.ToString("C2"), Element.ALIGN_RIGHT);
                    AgregarCelda(table, detalle.Descuento.ToString("N2"), Element.ALIGN_RIGHT);
                    AgregarCelda(table, detalle.ImporteNeto.ToString("C2"), Element.ALIGN_RIGHT);
                    AgregarCelda(table, detalle.Total.ToString("C2"), Element.ALIGN_RIGHT);
                }
            }

            document.Add(table);
        }

        private void AgregarTotales(Document document, FacturaModel factura)
        {
            // Crear tabla de totales alineada a la derecha
            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 50;
            table.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.SetWidths(new float[] { 2.5f, 1f });

            // Subtotal (Base Imponible)
            AgregarFilaTotal(table, "Subtotal (Base Imponible):", factura.Subtotal.ToString("C2"));

            // IVA
            AgregarFilaTotal(table, "IVA (21%):", factura.TotalIva.ToString("C2"));

            // Percepción ARBA (solo si es mayor a 0)
            if (Math.Round(factura.ImporteArba, 2) > 0)
            {
                AgregarFilaTotal(table, "Percepción IIBB ARBA:", factura.ImporteArba.ToString("C2"));
            }

            // Total
            PdfPCell cellLabel = new PdfPCell(new Phrase("TOTAL:", _fontBold));
            cellLabel.Border = iTextRectangle.TOP_BORDER | iTextRectangle.BOTTOM_BORDER;
            cellLabel.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellLabel.Padding = 8;
            cellLabel.BackgroundColor = new BaseColor(230, 230, 230);

            PdfPCell cellValue = new PdfPCell(new Phrase(factura.Total.ToString("C2"), _fontBold));
            cellValue.Border = iTextRectangle.TOP_BORDER | iTextRectangle.BOTTOM_BORDER;
            cellValue.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellValue.Padding = 8;
            cellValue.BackgroundColor = new BaseColor(230, 230, 230);

            table.AddCell(cellLabel);
            table.AddCell(cellValue);

            document.Add(table);
        }

        private void AgregarCAE(Document document, FacturaModel factura)
        {
            // Crear tabla para CAE con 2 columnas: una para texto y otra para QR
            PdfPTable tableCAE = new PdfPTable(2);
            tableCAE.WidthPercentage = 100;
            tableCAE.SetWidths(new float[] { 3f, 1f }); // 75% texto, 25% QR

            // Celda IZQUIERDA: Información del CAE
            PdfPCell cellInfo = new PdfPCell();
            cellInfo.Border = iTextRectangle.BOX;
            cellInfo.Padding = 10;
            cellInfo.BackgroundColor = new BaseColor(255, 255, 200);

            Paragraph tituloCAE = new Paragraph("COMPROBANTE AUTORIZADO", _fontBold);
            tituloCAE.Alignment = Element.ALIGN_CENTER;
            cellInfo.AddElement(tituloCAE);

            AgregarEspacio(cellInfo, 5);

            Paragraph caeTexto = new Paragraph($"CAE N°: {FormatearCAE(factura.CAE)}", _fontCAE);
            caeTexto.Alignment = Element.ALIGN_CENTER;
            cellInfo.AddElement(caeTexto);

            Paragraph vencimientoCAE = new Paragraph(
                $"Fecha de Vencimiento: {factura.VencimientoCae?.ToString("dd/MM/yyyy") ?? "N/A"}",
                _fontNormal);
            vencimientoCAE.Alignment = Element.ALIGN_CENTER;
            cellInfo.AddElement(vencimientoCAE);

            AgregarEspacio(cellInfo, 5);

            // Código de barras (simplificado, solo texto)
            Paragraph codigoBarras = new Paragraph($"Código de Barras: {factura.CodigoBarras}", _fontTiny);
            codigoBarras.Alignment = Element.ALIGN_CENTER;
            cellInfo.AddElement(codigoBarras);

            // Celda DERECHA: Código QR
            PdfPCell cellQR = new PdfPCell();
            cellQR.Border = iTextRectangle.BOX;
            cellQR.Padding = 10;
            cellQR.BackgroundColor = new BaseColor(255, 255, 200);
            cellQR.HorizontalAlignment = Element.ALIGN_CENTER;
            cellQR.VerticalAlignment = Element.ALIGN_MIDDLE;

            try
            {
                // Generar código QR con los datos de la factura
                string datosQR = GenerarDatosQRCompletos(factura);
                
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                {
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(datosQR, QRCodeGenerator.ECCLevel.Q);
                    
                    using (QRCode qrCode = new QRCode(qrCodeData))
                    {
                        // Generar imagen del QR (150x150 px)
                        using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                        {
                            // Convertir Bitmap a byte array
                            using (MemoryStream ms = new MemoryStream())
                            {
                                qrCodeImage.Save(ms, ImageFormat.Png);
                                byte[] imageBytes = ms.ToArray();

                                // Crear imagen de iTextSharp
                                iTextSharp.text.Image qrImage = iTextSharp.text.Image.GetInstance(imageBytes);
                                
                                // Ajustar tamaño del QR
                                qrImage.ScaleToFit(100f, 100f);
                                qrImage.Alignment = Element.ALIGN_CENTER;
                                
                                cellQR.AddElement(qrImage);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Si falla la generación del QR, mostrar mensaje
                Paragraph errorQR = new Paragraph("QR no disponible", _fontTiny);
                errorQR.Alignment = Element.ALIGN_CENTER;
                cellQR.AddElement(errorQR);
            }

            tableCAE.AddCell(cellInfo);
            tableCAE.AddCell(cellQR);
            
            document.Add(tableCAE);
        }

        /// <summary>
        /// Genera los datos completos para el código QR según especificaciones de AFIP
        /// </summary>
        private string GenerarDatosQRCompletos(FacturaModel factura)
        {
            // Formato del QR según especificaciones de AFIP:
            // {"ver":1,"fecha":"2024-01-15","cuit":20123456789,"ptoVta":1,"tipoCmp":1,"nroCmp":12345678,"importe":1500.50,"moneda":"PES","ctz":1,"tipoDocRec":80,"nroDocRec":20987654321,"tipoCodAut":"E","codAut":12345678901234}
            
            string datosQR = "{" +
                "\"ver\":1," +
                $"\"fecha\":\"{factura.FechaEmision:yyyy-MM-dd}\"," +
                $"\"cuit\":{factura.EmpresaCUIT.Replace("-", "")}," +
                $"\"ptoVta\":{factura.PuntoVenta}," +
                $"\"tipoCmp\":{ObtenerCodigoTipoComprobante(factura.TipoFactura)}," +
                $"\"nroCmp\":{factura.NumeroDesde}," +
                $"\"importe\":{factura.Total:F2}," +
                "\"moneda\":\"PES\"," +
                "\"ctz\":1," +
                "\"tipoDocRec\":80," + // 80 = CUIT
                $"\"nroDocRec\":{factura.ClienteCUIT.Replace("-", "")}," +
                "\"tipoCodAut\":\"E\"," + // E = CAE
                $"\"codAut\":{factura.CAE}" +
                "}" ;
            
            return datosQR;
        }

        /// <summary>
        /// Obtiene el código de tipo de comprobante según AFIP
        /// </summary>
        private int ObtenerCodigoTipoComprobante(string tipoFactura)
        {
            switch (tipoFactura?.ToUpper())
            {
                case "A": return 1;  // Factura A
                case "B": return 6;  // Factura B
                case "C": return 11; // Factura C
                default: return 1;
            }
        }

        private void AgregarPiePagina(Document document, FacturaModel factura)
        {
            AgregarEspacio(document, 10);

            Paragraph pie1 = new Paragraph(
                "Este comprobante es una simulación MOCK para fines de demostración.",
                _fontTiny);
            pie1.Alignment = Element.ALIGN_CENTER;
            document.Add(pie1);

            Paragraph pie2 = new Paragraph(
                "NO es válido para operaciones reales con AFIP.",
                _fontTiny);
            pie2.Alignment = Element.ALIGN_CENTER;
            document.Add(pie2);

            AgregarEspacio(document, 5);

            Paragraph pie3 = new Paragraph(
                $"Documento generado el {DateTime.Now:dd/MM/yyyy HH:mm} - Sistema Presupuestario v1.0",
                _fontTiny);
            pie3.Alignment = Element.ALIGN_CENTER;
            document.Add(pie3);
        }

        // ==================== MÉTODOS AUXILIARES ====================

        private void AgregarEspacio(Document document, float espacioMm)
        {
            document.Add(new Paragraph(" ")
            {
                SpacingAfter = espacioMm
            });
        }

        private void AgregarEspacio(PdfPCell cell, float espacioMm)
        {
            cell.AddElement(new Paragraph(" ")
            {
                SpacingAfter = espacioMm
            });
        }

        private void AgregarCeldaEncabezado(PdfPTable table, string texto)
        {
            PdfPCell cell = new PdfPCell(new Phrase(texto, _fontBold));
            cell.BackgroundColor = new BaseColor(180, 180, 180);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 5;
            table.AddCell(cell);
        }

        private void AgregarCelda(PdfPTable table, string texto, int alineacion)
        {
            PdfPCell cell = new PdfPCell(new Phrase(texto, _fontSmall));
            cell.HorizontalAlignment = alineacion;
            cell.Padding = 4;
            table.AddCell(cell);
        }

        private void AgregarFilaTotal(PdfPTable table, string label, string value)
        {
            PdfPCell cellLabel = new PdfPCell(new Phrase(label, _fontNormal));
            cellLabel.Border = iTextRectangle.NO_BORDER;
            cellLabel.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellLabel.PaddingTop = 3;
            cellLabel.PaddingBottom = 3;
            cellLabel.PaddingRight = 5;

            PdfPCell cellValue = new PdfPCell(new Phrase(value, _fontNormal));
            cellValue.Border = iTextRectangle.NO_BORDER;
            cellValue.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellValue.PaddingTop = 3;
            cellValue.PaddingBottom = 3;

            table.AddCell(cellLabel);
            table.AddCell(cellValue);
        }

        private string FormatearCUIT(string cuit)
        {
            if (string.IsNullOrWhiteSpace(cuit))
                return "N/A";

            // Remover caracteres no numéricos
            string cuitLimpio = new string(cuit.Where(char.IsDigit).ToArray());

            if (cuitLimpio.Length == 11)
                return $"{cuitLimpio.Substring(0, 2)}-{cuitLimpio.Substring(2, 8)}-{cuitLimpio.Substring(10, 1)}";

            return cuit;
        }

        private string FormatearCAE(string cae)
        {
            if (string.IsNullOrWhiteSpace(cae))
                return "N/A";

            // Formatear CAE en grupos de 4 dígitos para mejor legibilidad
            if (cae.Length == 14)
                return $"{cae.Substring(0, 4)} {cae.Substring(4, 4)} {cae.Substring(8, 4)} {cae.Substring(12, 2)}";

            return cae;
        }
    }
}
