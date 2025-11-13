using iTextSharp.text;
using iTextSharp.text.pdf;
using Services.DAL.Implementations;
using Services.DAL.Tools;
using Services.DomainModel;
using Services.Services.Contracts;
using System;
using System.Diagnostics;
using System.IO;

namespace Services.Services
{
    /// <summary>
    /// Servicio para generar PDFs de presupuestos
    /// Implementación interna que usa ADO.NET para acceder a datos
    /// </summary>
    internal class PresupuestoPdfService : IPresupuestoPdfService
    {
        private readonly PresupuestoPdfRepository _repository;
        private readonly Font _fontTitle;
        private readonly Font _fontSubtitle;
        private readonly Font _fontNormal;
        private readonly Font _fontBold;
        private readonly Font _fontSmall;

        public PresupuestoPdfService(SqlServerHelper sqlHelper)
        {
            _repository = new PresupuestoPdfRepository(sqlHelper);

            // Configurar fuentes
            _fontTitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLACK);
            _fontSubtitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLACK);
            _fontNormal = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
            _fontBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.BLACK);
            _fontSmall = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY);
        }

        /// <summary>
        /// Genera un PDF del presupuesto y lo guarda en el sistema
        /// </summary>
        public string GenerarPdf(Guid idPresupuesto)
        {
            // Obtener datos del presupuesto desde la base de datos
            var presupuesto = _repository.ObtenerPresupuestoParaPdf(idPresupuesto);

            if (presupuesto == null)
                throw new Exception($"No se encontró el presupuesto con ID {idPresupuesto}");

            // Obtener ruta del sistema
            string rutaSistema = AppDomain.CurrentDomain.BaseDirectory;
            string carpetaReportes = Path.Combine(rutaSistema, "Reportes");

            // Crear carpeta si no existe
            if (!Directory.Exists(carpetaReportes))
            {
                Directory.CreateDirectory(carpetaReportes);
            }

            // Generar nombre de archivo único
            string nombreArchivo = $"Presupuesto_{presupuesto.Numero.Replace("/", "-")}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string rutaCompleta = Path.Combine(carpetaReportes, nombreArchivo);

            // Crear documento PDF
            Document document = new Document(PageSize.A4, 50, 50, 25, 25);

            try
            {
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(rutaCompleta, FileMode.Create));
                document.Open();

                // Agregar contenido
                AgregarEncabezado(document, presupuesto);
                AgregarEspacio(document, 20);
                AgregarDatosCliente(document, presupuesto);
                AgregarEspacio(document, 20);
                AgregarDatosVendedor(document, presupuesto);
                AgregarEspacio(document, 20);
                AgregarTablaDetalles(document, presupuesto);
                AgregarEspacio(document, 20);
                AgregarTotales(document, presupuesto);
                AgregarEspacio(document, 30);
                AgregarPiePagina(document);

                document.Close();

                return rutaCompleta;
            }
            catch (Exception ex)
            {
                document.Close();
                throw new Exception($"Error al generar el PDF: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Genera el PDF y lo abre automáticamente
        /// </summary>
        public void GenerarYAbrirPdf(Guid idPresupuesto)
        {
            string rutaPdf = GenerarPdf(idPresupuesto);
            Process.Start(rutaPdf);
        }

        private void AgregarEncabezado(Document document, PresupuestoPdfModel presupuesto)
        {
            // Título principal
            Paragraph titulo = new Paragraph("PRESUPUESTO", _fontTitle);
            titulo.Alignment = Element.ALIGN_CENTER;
            document.Add(titulo);

            AgregarEspacio(document, 10);

            // Información del presupuesto
            PdfPTable tableInfo = new PdfPTable(2);
            tableInfo.WidthPercentage = 100;
            tableInfo.SetWidths(new float[] { 1, 1 });

            // Columna izquierda
            PdfPCell cellLeft = new PdfPCell();
            cellLeft.Border = Rectangle.NO_BORDER;
            cellLeft.AddElement(new Paragraph($"Número: {presupuesto.Numero}", _fontBold));
            cellLeft.AddElement(new Paragraph($"Fecha Emisión: {presupuesto.FechaEmision:dd/MM/yyyy}", _fontNormal));
            if (presupuesto.FechaVencimiento.HasValue)
            {
                cellLeft.AddElement(new Paragraph($"Fecha Vencimiento: {presupuesto.FechaVencimiento.Value:dd/MM/yyyy}", _fontNormal));
            }

            // Columna derecha
            PdfPCell cellRight = new PdfPCell();
            cellRight.Border = Rectangle.NO_BORDER;
            cellRight.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellRight.AddElement(new Paragraph($"Estado: {presupuesto.Estado}", _fontBold));
            cellRight.AddElement(new Paragraph($"Fecha Generación: {DateTime.Now:dd/MM/yyyy HH:mm}", _fontSmall));

            tableInfo.AddCell(cellLeft);
            tableInfo.AddCell(cellRight);

            document.Add(tableInfo);
        }

        private void AgregarDatosCliente(Document document, PresupuestoPdfModel presupuesto)
        {
            Paragraph subtitulo = new Paragraph("DATOS DEL CLIENTE", _fontSubtitle);
            document.Add(subtitulo);

            AgregarEspacio(document, 5);

            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 1, 3 });

            AgregarFila(table, "Código:", presupuesto.ClienteCodigo ?? "N/A");
            AgregarFila(table, "Razón Social:", presupuesto.ClienteRazonSocial ?? "N/A");

            document.Add(table);
        }

        private void AgregarDatosVendedor(Document document, PresupuestoPdfModel presupuesto)
        {
            if (!string.IsNullOrWhiteSpace(presupuesto.VendedorNombre))
            {
                Paragraph subtitulo = new Paragraph("DATOS DEL VENDEDOR", _fontSubtitle);
                document.Add(subtitulo);

                AgregarEspacio(document, 5);

                PdfPTable table = new PdfPTable(2);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1, 3 });

                AgregarFila(table, "Nombre:", presupuesto.VendedorNombre);

                document.Add(table);
            }
        }

        private void AgregarTablaDetalles(Document document, PresupuestoPdfModel presupuesto)
        {
            Paragraph subtitulo = new Paragraph("DETALLE DE ARTÍCULOS", _fontSubtitle);
            document.Add(subtitulo);

            AgregarEspacio(document, 5);

            // Crear tabla con 6 columnas
            PdfPTable table = new PdfPTable(6);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 1.5f, 3f, 1f, 1.5f, 1f, 1.5f });

            // Encabezados
            AgregarCeldaEncabezado(table, "Código");
            AgregarCeldaEncabezado(table, "Descripción");
            AgregarCeldaEncabezado(table, "Cantidad");
            AgregarCeldaEncabezado(table, "Precio Unit.");
            AgregarCeldaEncabezado(table, "Desc %");
            AgregarCeldaEncabezado(table, "Total");

            // Detalles
            if (presupuesto.Detalles != null)
            {
                foreach (var detalle in presupuesto.Detalles)
                {
                    AgregarCelda(table, detalle.Codigo ?? "N/A", Element.ALIGN_LEFT);
                    AgregarCelda(table, detalle.Descripcion ?? "N/A", Element.ALIGN_LEFT);
                    AgregarCelda(table, detalle.Cantidad.ToString("N2"), Element.ALIGN_RIGHT);
                    AgregarCelda(table, detalle.Precio.ToString("C2"), Element.ALIGN_RIGHT);
                    AgregarCelda(table, detalle.Descuento.ToString("N2"), Element.ALIGN_RIGHT);
                    AgregarCelda(table, detalle.Total.ToString("C2"), Element.ALIGN_RIGHT);
                }
            }

            document.Add(table);
        }

        private void AgregarTotales(Document document, PresupuestoPdfModel presupuesto)
        {
            // Crear tabla de totales alineada a la derecha
            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 50;
            table.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.SetWidths(new float[] { 2, 1 });

            // Subtotal
            AgregarFilaTotal(table, "Subtotal:", presupuesto.Subtotal.ToString("C2"));

            // IVA
            AgregarFilaTotal(table, "IVA:", presupuesto.TotalIva.ToString("C2"));

            // Total
            PdfPCell cellLabel = new PdfPCell(new Phrase("TOTAL:", _fontBold));
            cellLabel.Border = Rectangle.TOP_BORDER;
            cellLabel.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellLabel.PaddingTop = 5;
            cellLabel.PaddingBottom = 5;

            PdfPCell cellValue = new PdfPCell(new Phrase(presupuesto.Total.ToString("C2"), _fontBold));
            cellValue.Border = Rectangle.TOP_BORDER;
            cellValue.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellValue.PaddingTop = 5;
            cellValue.PaddingBottom = 5;

            table.AddCell(cellLabel);
            table.AddCell(cellValue);

            document.Add(table);
        }

        private void AgregarPiePagina(Document document)
        {
            Paragraph pie = new Paragraph("Documento generado automáticamente por Sistema Presupuestario", _fontSmall);
            pie.Alignment = Element.ALIGN_CENTER;
            document.Add(pie);
        }

        // Métodos auxiliares

        private void AgregarEspacio(Document document, float espacioMm)
        {
            document.Add(new Paragraph(" ")
            {
                SpacingAfter = espacioMm
            });
        }

        private void AgregarFila(PdfPTable table, string label, string value)
        {
            PdfPCell cellLabel = new PdfPCell(new Phrase(label, _fontBold));
            cellLabel.Border = Rectangle.NO_BORDER;
            cellLabel.PaddingBottom = 5;

            PdfPCell cellValue = new PdfPCell(new Phrase(value, _fontNormal));
            cellValue.Border = Rectangle.NO_BORDER;
            cellValue.PaddingBottom = 5;

            table.AddCell(cellLabel);
            table.AddCell(cellValue);
        }

        private void AgregarCeldaEncabezado(PdfPTable table, string texto)
        {
            PdfPCell cell = new PdfPCell(new Phrase(texto, _fontBold));
            cell.BackgroundColor = new BaseColor(200, 200, 200);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 5;
            table.AddCell(cell);
        }

        private void AgregarCelda(PdfPTable table, string texto, int alineacion)
        {
            PdfPCell cell = new PdfPCell(new Phrase(texto, _fontNormal));
            cell.HorizontalAlignment = alineacion;
            cell.Padding = 5;
            table.AddCell(cell);
        }

        private void AgregarFilaTotal(PdfPTable table, string label, string value)
        {
            PdfPCell cellLabel = new PdfPCell(new Phrase(label, _fontNormal));
            cellLabel.Border = Rectangle.NO_BORDER;
            cellLabel.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellLabel.PaddingTop = 2;
            cellLabel.PaddingBottom = 2;

            PdfPCell cellValue = new PdfPCell(new Phrase(value, _fontNormal));
            cellValue.Border = Rectangle.NO_BORDER;
            cellValue.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellValue.PaddingTop = 2;
            cellValue.PaddingBottom = 2;

            table.AddCell(cellLabel);
            table.AddCell(cellValue);
        }
    }
}
