namespace SistemaPresupuestario.Maestros.Vendedores
{
    partial class frmVendedorAlta
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.grpDatosBasicos = new System.Windows.Forms.GroupBox();
            this.txtCUIT = new System.Windows.Forms.TextBox();
            this.lblCUIT = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtCodigoVendedor = new System.Windows.Forms.TextBox();
            this.lblCodigoVendedor = new System.Windows.Forms.Label();
            this.grpDatosComerciales = new System.Windows.Forms.GroupBox();
            this.numPorcentajeComision = new System.Windows.Forms.NumericUpDown();
            this.lblPorcentajeComision = new System.Windows.Forms.Label();
            this.grpContacto = new System.Windows.Forms.GroupBox();
            this.txtDireccion = new System.Windows.Forms.TextBox();
            this.lblDireccion = new System.Windows.Forms.Label();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.lblTelefono = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.grpDatosBasicos.SuspendLayout();
            this.grpDatosComerciales.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPorcentajeComision)).BeginInit();
            this.grpContacto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // grpDatosBasicos
            // 
            this.grpDatosBasicos.Controls.Add(this.txtCUIT);
            this.grpDatosBasicos.Controls.Add(this.lblCUIT);
            this.grpDatosBasicos.Controls.Add(this.txtNombre);
            this.grpDatosBasicos.Controls.Add(this.lblNombre);
            this.grpDatosBasicos.Controls.Add(this.txtCodigoVendedor);
            this.grpDatosBasicos.Controls.Add(this.lblCodigoVendedor);
            this.grpDatosBasicos.Location = new System.Drawing.Point(12, 12);
            this.grpDatosBasicos.Name = "grpDatosBasicos";
            this.grpDatosBasicos.Size = new System.Drawing.Size(460, 140);
            this.grpDatosBasicos.TabIndex = 0;
            this.grpDatosBasicos.TabStop = false;
            this.grpDatosBasicos.Text = "Datos Básicos";
            // 
            // txtCUIT
            // 
            this.txtCUIT.Location = new System.Drawing.Point(130, 100);
            this.txtCUIT.MaxLength = 13;
            this.txtCUIT.Name = "txtCUIT";
            this.txtCUIT.Size = new System.Drawing.Size(200, 22);
            this.txtCUIT.TabIndex = 5;
            this.txtCUIT.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCUIT_KeyPress);
            // 
            // lblCUIT
            // 
            this.lblCUIT.AutoSize = true;
            this.lblCUIT.Location = new System.Drawing.Point(15, 103);
            this.lblCUIT.Name = "lblCUIT";
            this.lblCUIT.Size = new System.Drawing.Size(40, 16);
            this.lblCUIT.TabIndex = 4;
            this.lblCUIT.Text = "CUIT:";
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(130, 60);
            this.txtNombre.MaxLength = 200;
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(320, 22);
            this.txtNombre.TabIndex = 3;
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(15, 63);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(59, 16);
            this.lblNombre.TabIndex = 2;
            this.lblNombre.Text = "Nombre:";
            // 
            // txtCodigoVendedor
            // 
            this.txtCodigoVendedor.Location = new System.Drawing.Point(130, 25);
            this.txtCodigoVendedor.MaxLength = 2;
            this.txtCodigoVendedor.Name = "txtCodigoVendedor";
            this.txtCodigoVendedor.Size = new System.Drawing.Size(60, 22);
            this.txtCodigoVendedor.TabIndex = 1;
            this.txtCodigoVendedor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCodigoVendedor_KeyPress);
            // 
            // lblCodigoVendedor
            // 
            this.lblCodigoVendedor.AutoSize = true;
            this.lblCodigoVendedor.Location = new System.Drawing.Point(15, 28);
            this.lblCodigoVendedor.Name = "lblCodigoVendedor";
            this.lblCodigoVendedor.Size = new System.Drawing.Size(117, 16);
            this.lblCodigoVendedor.TabIndex = 0;
            this.lblCodigoVendedor.Text = "Código Vendedor:";
            // 
            // grpDatosComerciales
            // 
            this.grpDatosComerciales.Controls.Add(this.numPorcentajeComision);
            this.grpDatosComerciales.Controls.Add(this.lblPorcentajeComision);
            this.grpDatosComerciales.Location = new System.Drawing.Point(12, 158);
            this.grpDatosComerciales.Name = "grpDatosComerciales";
            this.grpDatosComerciales.Size = new System.Drawing.Size(460, 70);
            this.grpDatosComerciales.TabIndex = 1;
            this.grpDatosComerciales.TabStop = false;
            this.grpDatosComerciales.Text = "Datos Comerciales";
            // 
            // numPorcentajeComision
            // 
            this.numPorcentajeComision.DecimalPlaces = 2;
            this.numPorcentajeComision.Location = new System.Drawing.Point(130, 28);
            this.numPorcentajeComision.Name = "numPorcentajeComision";
            this.numPorcentajeComision.Size = new System.Drawing.Size(100, 22);
            this.numPorcentajeComision.TabIndex = 1;
            // 
            // lblPorcentajeComision
            // 
            this.lblPorcentajeComision.AutoSize = true;
            this.lblPorcentajeComision.Location = new System.Drawing.Point(15, 30);
            this.lblPorcentajeComision.Name = "lblPorcentajeComision";
            this.lblPorcentajeComision.Size = new System.Drawing.Size(109, 16);
            this.lblPorcentajeComision.TabIndex = 0;
            this.lblPorcentajeComision.Text = "% Comisión (0-100):";
            // 
            // grpContacto
            // 
            this.grpContacto.Controls.Add(this.txtDireccion);
            this.grpContacto.Controls.Add(this.lblDireccion);
            this.grpContacto.Controls.Add(this.txtTelefono);
            this.grpContacto.Controls.Add(this.lblTelefono);
            this.grpContacto.Controls.Add(this.txtEmail);
            this.grpContacto.Controls.Add(this.lblEmail);
            this.grpContacto.Location = new System.Drawing.Point(12, 234);
            this.grpContacto.Name = "grpContacto";
            this.grpContacto.Size = new System.Drawing.Size(460, 140);
            this.grpContacto.TabIndex = 2;
            this.grpContacto.TabStop = false;
            this.grpContacto.Text = "Datos de Contacto (Opcional)";
            // 
            // txtDireccion
            // 
            this.txtDireccion.Location = new System.Drawing.Point(130, 100);
            this.txtDireccion.MaxLength = 200;
            this.txtDireccion.Name = "txtDireccion";
            this.txtDireccion.Size = new System.Drawing.Size(320, 22);
            this.txtDireccion.TabIndex = 5;
            // 
            // lblDireccion
            // 
            this.lblDireccion.AutoSize = true;
            this.lblDireccion.Location = new System.Drawing.Point(15, 103);
            this.lblDireccion.Name = "lblDireccion";
            this.lblDireccion.Size = new System.Drawing.Size(67, 16);
            this.lblDireccion.TabIndex = 4;
            this.lblDireccion.Text = "Dirección:";
            // 
            // txtTelefono
            // 
            this.txtTelefono.Location = new System.Drawing.Point(130, 60);
            this.txtTelefono.MaxLength = 20;
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(200, 22);
            this.txtTelefono.TabIndex = 3;
            // 
            // lblTelefono
            // 
            this.lblTelefono.AutoSize = true;
            this.lblTelefono.Location = new System.Drawing.Point(15, 63);
            this.lblTelefono.Name = "lblTelefono";
            this.lblTelefono.Size = new System.Drawing.Size(64, 16);
            this.lblTelefono.TabIndex = 2;
            this.lblTelefono.Text = "Teléfono:";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(130, 25);
            this.txtEmail.MaxLength = 100;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(320, 22);
            this.txtEmail.TabIndex = 1;
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(15, 28);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(44, 16);
            this.lblEmail.TabIndex = 0;
            this.lblEmail.Text = "Email:";
            // 
            // btnAceptar
            // 
            this.btnAceptar.Location = new System.Drawing.Point(285, 390);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(90, 30);
            this.btnAceptar.TabIndex = 3;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = true;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(382, 390);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(90, 30);
            this.btnCancelar.TabIndex = 4;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // frmVendedorAlta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 432);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.grpContacto);
            this.Controls.Add(this.grpDatosComerciales);
            this.Controls.Add(this.grpDatosBasicos);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmVendedorAlta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Vendedor";
            this.Load += new System.EventHandler(this.frmVendedorAlta_Load);
            this.grpDatosBasicos.ResumeLayout(false);
            this.grpDatosBasicos.PerformLayout();
            this.grpDatosComerciales.ResumeLayout(false);
            this.grpDatosComerciales.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPorcentajeComision)).EndInit();
            this.grpContacto.ResumeLayout(false);
            this.grpContacto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpDatosBasicos;
        private System.Windows.Forms.TextBox txtCUIT;
        private System.Windows.Forms.Label lblCUIT;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtCodigoVendedor;
        private System.Windows.Forms.Label lblCodigoVendedor;
        private System.Windows.Forms.GroupBox grpDatosComerciales;
        private System.Windows.Forms.NumericUpDown numPorcentajeComision;
        private System.Windows.Forms.Label lblPorcentajeComision;
        private System.Windows.Forms.GroupBox grpContacto;
        private System.Windows.Forms.TextBox txtDireccion;
        private System.Windows.Forms.Label lblDireccion;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.Label lblTelefono;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}
