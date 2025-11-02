namespace SistemaPresupuestario
{
    partial class frmMain
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsPrincipalArchivo = new System.Windows.Forms.ToolStripMenuItem();
            this.salirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPrincipalMaestro = new System.Windows.Forms.ToolStripMenuItem();
            this.tsCliente = new System.Windows.Forms.ToolStripMenuItem();
            this.tsProducto = new System.Windows.Forms.ToolStripMenuItem();
            this.tsUsuario = new System.Windows.Forms.ToolStripMenuItem();
            this.tsVendedor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPrincipalPresupuesto = new System.Windows.Forms.ToolStripMenuItem();
            this.tsGenerarCotizacion = new System.Windows.Forms.ToolStripMenuItem();
            this.tsGestionarCotizacion = new System.Windows.Forms.ToolStripMenuItem();
            this.tsAprobarCotizacion = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPrincipalVenta = new System.Windows.Forms.ToolStripMenuItem();
            this.tsFactura = new System.Windows.Forms.ToolStripMenuItem();
            this.tsRecibo = new System.Windows.Forms.ToolStripMenuItem();
            this.tsArba = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPrincipalAyuda = new System.Windows.Forms.ToolStripMenuItem();
            this.txtUsuario = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsPrincipalArchivo,
            this.tsPrincipalMaestro,
            this.tsPrincipalPresupuesto,
            this.tsPrincipalVenta,
            this.tsPrincipalAyuda,
            this.txtUsuario});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1592, 28);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsPrincipalArchivo
            // 
            this.tsPrincipalArchivo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.salirToolStripMenuItem});
            this.tsPrincipalArchivo.Name = "tsPrincipalArchivo";
            this.tsPrincipalArchivo.Size = new System.Drawing.Size(73, 24);
            this.tsPrincipalArchivo.Text = "Archivo";
            // 
            // salirToolStripMenuItem
            // 
            this.salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            this.salirToolStripMenuItem.Size = new System.Drawing.Size(121, 26);
            this.salirToolStripMenuItem.Text = "Salir";
            this.salirToolStripMenuItem.Click += new System.EventHandler(this.salirToolStripMenuItem_Click);
            // 
            // tsPrincipalMaestro
            // 
            this.tsPrincipalMaestro.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsCliente,
            this.tsProducto,
            this.tsUsuario,
            this.tsVendedor});
            this.tsPrincipalMaestro.Name = "tsPrincipalMaestro";
            this.tsPrincipalMaestro.Size = new System.Drawing.Size(83, 24);
            this.tsPrincipalMaestro.Text = "Maestros";
            // 
            // tsCliente
            // 
            this.tsCliente.Name = "tsCliente";
            this.tsCliente.Size = new System.Drawing.Size(224, 26);
            this.tsCliente.Text = "Clientes";
            this.tsCliente.Click += new System.EventHandler(this.tsCliente_Click);
            // 
            // tsProducto
            // 
            this.tsProducto.Name = "tsProducto";
            this.tsProducto.Size = new System.Drawing.Size(224, 26);
            this.tsProducto.Text = "Productos";
            // 
            // tsUsuario
            // 
            this.tsUsuario.Name = "tsUsuario";
            this.tsUsuario.Size = new System.Drawing.Size(224, 26);
            this.tsUsuario.Text = "Usuarios";
            this.tsUsuario.Click += new System.EventHandler(this.usuariosToolStripMenuItem_Click);
            // 
            // tsVendedor
            // 
            this.tsVendedor.Name = "tsVendedor";
            this.tsVendedor.Size = new System.Drawing.Size(224, 26);
            this.tsVendedor.Text = "Vendedores";
            this.tsVendedor.Click += new System.EventHandler(this.tsVendedor_Click);
            // 
            // tsPrincipalPresupuesto
            // 
            this.tsPrincipalPresupuesto.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsGenerarCotizacion,
            this.tsGestionarCotizacion,
            this.tsAprobarCotizacion});
            this.tsPrincipalPresupuesto.Name = "tsPrincipalPresupuesto";
            this.tsPrincipalPresupuesto.Size = new System.Drawing.Size(103, 24);
            this.tsPrincipalPresupuesto.Text = "Presupuesto";
            // 
            // tsGenerarCotizacion
            // 
            this.tsGenerarCotizacion.Name = "tsGenerarCotizacion";
            this.tsGenerarCotizacion.Size = new System.Drawing.Size(251, 26);
            this.tsGenerarCotizacion.Text = "Generar Cotización";
            // 
            // tsGestionarCotizacion
            // 
            this.tsGestionarCotizacion.Name = "tsGestionarCotizacion";
            this.tsGestionarCotizacion.Size = new System.Drawing.Size(251, 26);
            this.tsGestionarCotizacion.Text = "Gestión de Cotizaciones";
            // 
            // tsAprobarCotizacion
            // 
            this.tsAprobarCotizacion.Name = "tsAprobarCotizacion";
            this.tsAprobarCotizacion.Size = new System.Drawing.Size(251, 26);
            this.tsAprobarCotizacion.Text = "Aprobar Cotizaciones";
            // 
            // tsPrincipalVenta
            // 
            this.tsPrincipalVenta.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsFactura,
            this.tsRecibo,
            this.tsArba});
            this.tsPrincipalVenta.Name = "tsPrincipalVenta";
            this.tsPrincipalVenta.Size = new System.Drawing.Size(66, 24);
            this.tsPrincipalVenta.Text = "Ventas";
            // 
            // tsFactura
            // 
            this.tsFactura.Name = "tsFactura";
            this.tsFactura.Size = new System.Drawing.Size(244, 26);
            this.tsFactura.Text = "Emisión de Factura";
            // 
            // tsRecibo
            // 
            this.tsRecibo.Name = "tsRecibo";
            this.tsRecibo.Size = new System.Drawing.Size(244, 26);
            this.tsRecibo.Text = "Emisión de Recibo";
            // 
            // tsArba
            // 
            this.tsArba.Name = "tsArba";
            this.tsArba.Size = new System.Drawing.Size(244, 26);
            this.tsArba.Text = "Actualizar Padrón Arba";
            // 
            // tsPrincipalAyuda
            // 
            this.tsPrincipalAyuda.Name = "tsPrincipalAyuda";
            this.tsPrincipalAyuda.Size = new System.Drawing.Size(65, 24);
            this.tsPrincipalAyuda.Text = "Ayuda";
            // 
            // txtUsuario
            // 
            this.txtUsuario.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.txtUsuario.Enabled = false;
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(156, 24);
            this.txtUsuario.Text = "toolStripMenuItem1";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1592, 679);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sistema de presupuestos";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsPrincipalArchivo;
        private System.Windows.Forms.ToolStripMenuItem salirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsPrincipalMaestro;
        private System.Windows.Forms.ToolStripMenuItem tsPrincipalPresupuesto;
        private System.Windows.Forms.ToolStripMenuItem tsPrincipalVenta;
        private System.Windows.Forms.ToolStripMenuItem tsPrincipalAyuda;
        private System.Windows.Forms.ToolStripMenuItem txtUsuario;
        private System.Windows.Forms.ToolStripMenuItem tsUsuario;
        private System.Windows.Forms.ToolStripMenuItem tsCliente;
        private System.Windows.Forms.ToolStripMenuItem tsProducto;
        private System.Windows.Forms.ToolStripMenuItem tsVendedor;
        private System.Windows.Forms.ToolStripMenuItem tsGenerarCotizacion;
        private System.Windows.Forms.ToolStripMenuItem tsGestionarCotizacion;
        private System.Windows.Forms.ToolStripMenuItem tsAprobarCotizacion;
        private System.Windows.Forms.ToolStripMenuItem tsFactura;
        private System.Windows.Forms.ToolStripMenuItem tsRecibo;
        private System.Windows.Forms.ToolStripMenuItem tsArba;
    }
}

