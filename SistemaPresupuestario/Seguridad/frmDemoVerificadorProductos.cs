using SistemaPresupuestario.Helpers; // NUEVO
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SistemaPresupuestario.Seguridad
{
    /// <summary>
    /// Formulario de demostración académica de dígitos verificadores y sumas de control
    /// Este formulario es autocontenido y no interactúa con la base de datos
    /// </summary>
    public partial class frmDemoVerificadorProductos : FormBase
    {
        // Lista de datos en memoria para la demostración
        private List<ProductoDemo> _productos;
        
        // Suma de control original (almacenada después de calcular dígitos)
        private decimal _sumaControlOriginal = 0;
        
        // Dígito Verificador Vertical (DVV)
        private int _digitoVerificadorVertical = 0;
        
        // NUEVO: DVVs originales de cada columna
        private int _dvvCodigoOriginal = 0;
        private int _dvvDescripcionOriginal = 0;
        private int _dvvIvaOriginal = 0;

        public frmDemoVerificadorProductos()
        {
            InitializeComponent();
            _productos = new List<ProductoDemo>();
            
            // Configuración inicial del DataGridView
            ConfigurarGrilla();
            
            base.InitializeTranslation();
        }

        #region Configuración Inicial

        private void ConfigurarGrilla()
        {
            // La grilla ya está configurada en el Designer
            // Solo aseguramos que no se permita agregar filas
            dgvProductos.AllowUserToAddRows = false;
            dgvProductos.AllowUserToDeleteRows = false;
            dgvProductos.ReadOnly = false; // Permitir edición para simular corrupción manual
            
            // Configurar formato de columnas
            if (dgvProductos.Columns["colPorcentajeIVA"] != null)
            {
                dgvProductos.Columns["colPorcentajeIVA"].DefaultCellStyle.Format = "N2";
                dgvProductos.Columns["colPorcentajeIVA"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        #endregion

        #region Paso 1: Cargar Datos

        /// <summary>
        /// Carga datos hardcodeados en la grilla (simula un lote de productos)
        /// </summary>
        private void btnCargarDatos_Click(object sender, EventArgs e)
        {
            try
            {
                // Limpiar datos anteriores
                _productos.Clear();
                dgvProductos.Rows.Clear();
                _sumaControlOriginal = 0;
                _digitoVerificadorVertical = 0;
                _dvvCodigoOriginal = 0;
                _dvvDescripcionOriginal = 0;
                _dvvIvaOriginal = 0;

                // Crear productos de demostración hardcodeados
                _productos = new List<ProductoDemo>
                {
                    new ProductoDemo { Codigo = "A1001", Descripcion = "Producto A", PorcentajeIVA = 21.00M },
                    new ProductoDemo { Codigo = "B2005", Descripcion = "Producto B", PorcentajeIVA = 10.50M },
                    new ProductoDemo { Codigo = "A1007", Descripcion = "Producto C", PorcentajeIVA = 21.00M },
                    new ProductoDemo { Codigo = "C3010", Descripcion = "Producto D", PorcentajeIVA = 5.00M },
                    new ProductoDemo { Codigo = "D4020", Descripcion = "Producto E", PorcentajeIVA = 27.00M }
                };

                // Poblar la grilla
                foreach (var producto in _productos)
                {
                    int rowIndex = dgvProductos.Rows.Add();
                    var row = dgvProductos.Rows[rowIndex];
                    
                    row.Cells["colCodigo"].Value = producto.Codigo;
                    row.Cells["colDescripcion"].Value = producto.Descripcion;
                    row.Cells["colPorcentajeIVA"].Value = producto.PorcentajeIVA;
                    row.Cells["colDigitoH"].Value = string.Empty;
                    
                    // Limpiar estilos
                    row.DefaultCellStyle.BackColor = Color.White;
                }

                // Actualizar estado
                ActualizarEstado("Datos cargados. Presione 'Calcular Dígitos' para continuar.", Color.Black);
                lblSumaControlVertical.Text = "Suma de Control (Lote): N/A | DVV: N/A";
                lblSumaControlVertical.BackColor = Color.Transparent;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Paso 2: Calcular Dígitos Verificadores

        /// <summary>
        /// Calcula los dígitos verificadores horizontales y la suma de control vertical
        /// Simula el "cierre" de un lote de datos
        /// </summary>
        private void btnCalcularDigitos_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvProductos.Rows.Count == 0)
                {
                    MessageBox.Show("No hay datos cargados. Presione 'Cargar Datos' primero.",
                        "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal sumaTotal = 0;

                // Calcular dígito horizontal para cada fila
                foreach (DataGridViewRow row in dgvProductos.Rows)
                {
                    if (row.IsNewRow) continue;

                    string codigo = row.Cells["colCodigo"].Value?.ToString() ?? "";
                    decimal porcentajeIVA = Convert.ToDecimal(row.Cells["colPorcentajeIVA"].Value ?? 0);

                    // Calcular dígito verificador para el código
                    int digitoH = CalcularDigitoHorizontal(codigo);
                    
                    // Establecer el dígito en la celda
                    row.Cells["colDigitoH"].Value = digitoH;
                    row.Cells["colDigitoH"].Style.BackColor = Color.LightGreen;

                    // Acumular para la suma vertical
                    sumaTotal += porcentajeIVA;
                }

                // Guardar suma de control vertical
                _sumaControlOriginal = sumaTotal;
                
                // Calcular Dígito Verificador Vertical (DVV) general
                _digitoVerificadorVertical = CalcularDigitoVertical(sumaTotal);
                
                // NUEVO: Calcular DVV de cada columna
                _dvvCodigoOriginal = CalcularDVVColumna("colCodigo");
                _dvvDescripcionOriginal = CalcularDVVColumna("colDescripcion");
                _dvvIvaOriginal = CalcularDVVColumna("colPorcentajeIVA");
                
                // Agregar fila de checksums al final
                AgregarFilaChecksums();
                
                // Almacenar valores en el Tag (formato: "suma|dvv")
                lblSumaControlVertical.Tag = $"{_sumaControlOriginal}|{_digitoVerificadorVertical}";
                
                // Mostrar suma y DVV
                lblSumaControlVertical.Text = $"Suma de Control (Lote): {_sumaControlOriginal:N2} | DVV General: {_digitoVerificadorVertical}";
                lblSumaControlVertical.BackColor = Color.LightGreen;

                // Actualizar estado
                ActualizarEstado("Lote Calculado y Sellado ? (DVH + DVV calculados)", Color.Green);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al calcular dígitos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// NUEVO: Calcula el DVV de una columna específica
        /// </summary>
        private int CalcularDVVColumna(string nombreColumna)
        {
            string concatenacion = "";
            
            foreach (DataGridViewRow row in dgvProductos.Rows)
            {
                // Excluir la fila de checksums del cálculo
                if (row.IsNewRow) 
                    continue;
                    
                if (row.Tag != null && row.Tag.ToString() == "CHECKSUM_ROW")
                    continue;
                
                var valor = row.Cells[nombreColumna].Value?.ToString() ?? "";
                concatenacion += valor;
            }
            
            return CalcularDigitoHorizontal(concatenacion);
        }

        /// <summary>
        /// NUEVO: Agrega una fila especial al final con los checksums de cada columna
        /// </summary>
        private void AgregarFilaChecksums()
        {
            // Eliminar fila de checksums anterior si existe
            EliminarFilaChecksums();
            
            // Agregar nueva fila
            int rowIndex = dgvProductos.Rows.Add();
            var row = dgvProductos.Rows[rowIndex];
            
            // Marcar como fila de checksum (usar Tag)
            row.Tag = "CHECKSUM_ROW";
            
            // Establecer valores de DVV para cada columna
            row.Cells["colCodigo"].Value = $"DVV: {_dvvCodigoOriginal}";
            row.Cells["colDescripcion"].Value = $"DVV: {_dvvDescripcionOriginal}";
            row.Cells["colPorcentajeIVA"].Value = $"DVV: {_dvvIvaOriginal}";
            
            // Estilo especial para la fila de checksums
            row.DefaultCellStyle.BackColor = Color.LightBlue;
            row.DefaultCellStyle.Font = new Font(dgvProductos.Font, FontStyle.Bold);
            row.DefaultCellStyle.ForeColor = Color.DarkBlue;
            row.ReadOnly = true;
            
            // Centrar texto
            foreach (DataGridViewCell cell in row.Cells)
            {
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        /// <summary>
        /// NUEVO: Elimina la fila de checksums si existe
        /// </summary>
        private void EliminarFilaChecksums()
        {
            for (int i = dgvProductos.Rows.Count - 1; i >= 0; i--)
            {
                if (dgvProductos.Rows[i].Tag?.ToString() == "CHECKSUM_ROW")
                {
                    dgvProductos.Rows.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Algoritmo de dígito verificador horizontal usando Módulo 11
        /// Este es un algoritmo simplificado para propósitos académicos
        /// </summary>
        private int CalcularDigitoHorizontal(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
                return 0;

            // Calcular suma ponderada de los caracteres
            int suma = 0;
            int peso = 2; // Factor de ponderación inicial

            // Recorrer el código de derecha a izquierda
            for (int i = codigo.Length - 1; i >= 0; i--)
            {
                // Convertir el carácter a su valor ASCII y multiplicar por el peso
                suma += (int)codigo[i] * peso;
                
                // Incrementar peso (ciclo 2-7)
                peso++;
                if (peso > 7)
                    peso = 2;
            }

            // Calcular el módulo 11
            int resto = suma % 11;
            int digitoVerificador = 11 - resto;

            // Casos especiales del Módulo 11
            if (digitoVerificador == 11)
                digitoVerificador = 0;
            else if (digitoVerificador == 10)
                digitoVerificador = 1; // Simplificación: usar 1 en lugar de 'X'

            return digitoVerificador;
        }

        /// <summary>
        /// Calcula el Dígito Verificador Vertical (DVV) usando Módulo 11
        /// Protege la integridad de toda la columna (suma de IVAs)
        /// </summary>
        private int CalcularDigitoVertical(decimal sumaTotal)
        {
            // Convertir la suma a una cadena sin el punto decimal
            // Ejemplo: 84.50 -> "8450"
            string sumaSinPunto = ((int)(sumaTotal * 100)).ToString();
            
            // Calcular suma ponderada de los dígitos
            int suma = 0;
            int peso = 2;
            
            // Recorrer de derecha a izquierda
            for (int i = sumaSinPunto.Length - 1; i >= 0; i--)
            {
                int digito = int.Parse(sumaSinPunto[i].ToString());
                suma += digito * peso;
                
                // Incrementar peso (ciclo 2-7)
                peso++;
                if (peso > 7)
                    peso = 2;
            }
            
            // Calcular el módulo 11
            int resto = suma % 11;
            int digitoVerificador = 11 - resto;
            
            // Casos especiales
            if (digitoVerificador == 11)
                digitoVerificador = 0;
            else if (digitoVerificador == 10)
                digitoVerificador = 1;
            
            return digitoVerificador;
        }

        #endregion

        #region Paso 3: Simular Corrupción de Datos

        /// <summary>
        /// Simula errores de transmisión o modificación indebida de datos
        /// </summary>
        private void btnSimularCorrupcion_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvProductos.Rows.Count == 0)
                {
                    MessageBox.Show("No hay datos para corromper. Cargue y calcule dígitos primero.",
                        "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_sumaControlOriginal == 0)
                {
                    MessageBox.Show("Debe calcular los dígitos primero antes de simular corrupción.",
                        "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Contar filas de datos (excluir fila de checksums)
                int cantidadFilasDatos = 0;
                foreach (DataGridViewRow row in dgvProductos.Rows)
                {
                    if (row.Tag?.ToString() != "CHECKSUM_ROW")
                        cantidadFilasDatos++;
                }

                // Alteración Horizontal: Modificar el código de la segunda fila
                if (cantidadFilasDatos > 1)
                {
                    var row2 = dgvProductos.Rows[1];
                    string codigoOriginal = row2.Cells["colCodigo"].Value?.ToString() ?? "";
                    
                    // Cambiar el último carácter del código
                    if (codigoOriginal.Length > 0)
                    {
                        char ultimoChar = codigoOriginal[codigoOriginal.Length - 1];
                        char nuevoChar = (char)(ultimoChar + 1); // Incrementar el último carácter
                        string codigoCorrupto = codigoOriginal.Substring(0, codigoOriginal.Length - 1) + nuevoChar;
                        
                        row2.Cells["colCodigo"].Value = codigoCorrupto;
                        row2.Cells["colCodigo"].Style.BackColor = Color.Yellow;
                    }
                }

                // Alteración Vertical: Modificar el IVA de la cuarta fila
                if (cantidadFilasDatos > 3)
                {
                    var row4 = dgvProductos.Rows[3];
                    decimal ivaOriginal = Convert.ToDecimal(row4.Cells["colPorcentajeIVA"].Value ?? 0);
                    decimal ivaCorrupto = ivaOriginal + 1.00M; // Incrementar 1%
                    
                    row4.Cells["colPorcentajeIVA"].Value = ivaCorrupto;
                    row4.Cells["colPorcentajeIVA"].Style.BackColor = Color.Yellow;
                }

                // Actualizar estado
                ActualizarEstado("¡Datos Alterados! Presione 'Verificar Integridad' para detectar errores.", Color.Orange);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al simular corrupción: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Paso 4: Verificar Integridad

        /// <summary>
        /// Verifica la integridad de los datos comparando dígitos y suma de control
        /// </summary>
        private void btnVerificarIntegridad_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvProductos.Rows.Count == 0)
                {
                    MessageBox.Show("No hay datos para verificar.",
                        "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_sumaControlOriginal == 0)
                {
                    MessageBox.Show("No hay suma de control calculada. Calcule los dígitos primero.",
                        "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool errorHorizontal = false;
                bool errorVertical = false;
                bool errorDVV = false;
                decimal sumaActual = 0;

                // Verificar dígitos horizontales (DVH)
                foreach (DataGridViewRow row in dgvProductos.Rows)
                {
                    // Excluir filas vacías y fila de checksums
                    if (row.IsNewRow) 
                        continue;
                        
                    if (row.Tag != null && row.Tag.ToString() == "CHECKSUM_ROW")
                        continue;

                    string codigoActual = row.Cells["colCodigo"].Value?.ToString() ?? "";
                    int digitoAlmacenado = Convert.ToInt32(row.Cells["colDigitoH"].Value ?? 0);
                    decimal porcentajeIVA = Convert.ToDecimal(row.Cells["colPorcentajeIVA"].Value ?? 0);

                    // Recalcular dígito con el código actual
                    int digitoRecalculado = CalcularDigitoHorizontal(codigoActual);

                    // Comparar
                    if (digitoAlmacenado != digitoRecalculado)
                    {
                        row.Cells["colDigitoH"].Style.BackColor = Color.Red;
                        errorHorizontal = true;
                    }
                    else
                    {
                        // Si no hay error, volver a verde
                        if (row.Cells["colDigitoH"].Style.BackColor != Color.LightGreen)
                        {
                            row.Cells["colDigitoH"].Style.BackColor = Color.LightGreen;
                        }
                    }

                    // Acumular para verificación vertical
                    sumaActual += porcentajeIVA;
                }

                // Verificar suma de control vertical
                if (Math.Abs(sumaActual - _sumaControlOriginal) > 0.01M) // Tolerancia de 0.01 por redondeo
                {
                    errorVertical = true;
                }

                // Verificar Dígito Verificador Vertical (DVV)
                int dvvRecalculado = CalcularDigitoVertical(sumaActual);
                if (dvvRecalculado != _digitoVerificadorVertical)
                {
                    errorDVV = true;
                }

                // NUEVO: Recalcular y verificar DVVs de cada columna
                int dvvCodigoActual = CalcularDVVColumna("colCodigo");
                int dvvDescripcionActual = CalcularDVVColumna("colDescripcion");
                int dvvIvaActual = CalcularDVVColumna("colPorcentajeIVA");

                bool errorDVVCodigo = (dvvCodigoActual != _dvvCodigoOriginal);
                bool errorDVVDescripcion = (dvvDescripcionActual != _dvvDescripcionOriginal);
                bool errorDVVIva = (dvvIvaActual != _dvvIvaOriginal);

                // Actualizar fila de checksums con colores
                ActualizarFilaChecksums(errorDVVCodigo, errorDVVDescripcion, errorDVVIva,
                    dvvCodigoActual, dvvDescripcionActual, dvvIvaActual);

                // Actualizar color del label según errores verticales
                if (errorVertical || errorDVV)
                {
                    lblSumaControlVertical.BackColor = Color.Red;
                }
                else
                {
                    lblSumaControlVertical.BackColor = Color.LightGreen;
                }

                // Actualizar texto con suma y DVV actuales
                lblSumaControlVertical.Text = $"Suma Original: {_sumaControlOriginal:N2} | Actual: {sumaActual:N2} | DVV Orig: {_digitoVerificadorVertical} | DVV Actual: {dvvRecalculado}";

                // Mostrar resultado final
                if (errorHorizontal || errorVertical || errorDVV || errorDVVCodigo || errorDVVDescripcion || errorDVVIva)
                {
                    string mensaje = "¡ERROR DE INTEGRIDAD DETECTADO!\n\n";
                    
                    if (errorHorizontal)
                        mensaje += "• DVH: Se detectaron cambios en códigos de productos (dígitos horizontales)\n";
                    
                    if (errorVertical)
                        mensaje += "• Suma: Se detectaron cambios en los porcentajes de IVA (suma vertical)\n";
                    
                    if (errorDVV)
                        mensaje += "• DVV General: El dígito verificador vertical no coincide\n";

                    if (errorDVVCodigo || errorDVVDescripcion || errorDVVIva)
                    {
                        mensaje += "• DVV Columnas: Se detectaron cambios en las columnas:\n";
                        if (errorDVVCodigo)
                            mensaje += $"  - Código: DVV {_dvvCodigoOriginal} ? {dvvCodigoActual}\n";
                        if (errorDVVDescripcion)
                            mensaje += $"  - Descripción: DVV {_dvvDescripcionOriginal} ? {dvvDescripcionActual}\n";
                        if (errorDVVIva)
                            mensaje += $"  - IVA: DVV {_dvvIvaOriginal} ? {dvvIvaActual}\n";
                    }

                    mensaje += $"\n?? Detalles:\n";
                    mensaje += $"   Suma Original: {_sumaControlOriginal:N2}\n";
                    mensaje += $"   Suma Actual: {sumaActual:N2}\n";
                    mensaje += $"   DVV General Original: {_digitoVerificadorVertical}\n";
                    mensaje += $"   DVV General Recalculado: {dvvRecalculado}";

                    ActualizarEstado("¡ERROR DE INTEGRIDAD DETECTADO! ?", Color.Red);
                    
                    MessageBox.Show(mensaje, "Verificación Fallida",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    ActualizarEstado("VERIFICACIÓN OK ? - Todos los DVH y DVV coinciden, datos íntegros", Color.Green);
                    
                    MessageBox.Show(
                        "? Verificación exitosa.\n\n" +
                        "• Todos los dígitos verificadores horizontales (DVH) coinciden\n" +
                        "• La suma de control vertical coincide\n" +
                        "• El dígito verificador vertical (DVV) general coincide\n" +
                        "• Todos los DVV de columnas coinciden\n\n" +
                        "Los datos no han sido alterados.",
                        "Verificación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al verificar integridad: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// NUEVO: Actualiza la fila de checksums con los valores actuales y colores según errores
        /// </summary>
        private void ActualizarFilaChecksums(bool errorCodigo, bool errorDescripcion, bool errorIva,
            int dvvCodigoActual, int dvvDescripcionActual, int dvvIvaActual)
        {
            // Buscar fila de checksums
            DataGridViewRow filaChecksum = null;
            foreach (DataGridViewRow row in dgvProductos.Rows)
            {
                if (row.Tag?.ToString() == "CHECKSUM_ROW")
                {
                    filaChecksum = row;
                    break;
                }
            }

            if (filaChecksum == null) return;

            // Actualizar valores y colores
            filaChecksum.Cells["colCodigo"].Value = $"DVV: {_dvvCodigoOriginal} ? {dvvCodigoActual}";
            filaChecksum.Cells["colCodigo"].Style.BackColor = errorCodigo ? Color.Red : Color.LightGreen;

            filaChecksum.Cells["colDescripcion"].Value = $"DVV: {_dvvDescripcionOriginal} ? {dvvDescripcionActual}";
            filaChecksum.Cells["colDescripcion"].Style.BackColor = errorDescripcion ? Color.Red : Color.LightGreen;

            filaChecksum.Cells["colPorcentajeIVA"].Value = $"DVV: {_dvvIvaOriginal} ? {dvvIvaActual}";
            filaChecksum.Cells["colPorcentajeIVA"].Style.BackColor = errorIva ? Color.Red : Color.LightGreen;

            // Si no hay errores, mantener el color distintivo
            if (!errorCodigo && !errorDescripcion && !errorIva)
            {
                foreach (DataGridViewCell cell in filaChecksum.Cells)
                {
                    cell.Style.BackColor = Color.LightGreen;
                }
            }
        }

        #endregion

        #region Métodos Auxiliares

        /// <summary>
        /// Actualiza el label de estado con mensaje y color
        /// </summary>
        private void ActualizarEstado(string mensaje, Color color)
        {
            lblEstadoGeneral.Text = mensaje;
            lblEstadoGeneral.ForeColor = color;
        }

        #endregion

        #region Clase Auxiliar para Demostración

        /// <summary>
        /// Clase simple para representar un producto en la demostración
        /// No tiene validaciones ni lógica de negocio (solo para demo)
        /// </summary>
        private class ProductoDemo
        {
            public string Codigo { get; set; }
            public string Descripcion { get; set; }
            public decimal PorcentajeIVA { get; set; }
        }

        #endregion
    }
}
