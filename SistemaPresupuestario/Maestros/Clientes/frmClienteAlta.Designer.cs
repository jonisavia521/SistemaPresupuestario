namespace SistemaPresupuestario.Maestros.Clientes
{
    partial class frmClienteAlta
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
            this.txtRazonSocial = new System.Windows.Forms.TextBox();
            this.lblRazonSocial = new System.Windows.Forms.Label();
            this.txtCodigoCliente = new System.Windows.Forms.TextBox();
            this.lblCodigoCliente = new System.Windows.Forms.Label();
            this.grpDocumento = new System.Windows.Forms.GroupBox();
            this.txtNumeroDocumento = new System.Windows.Forms.TextBox();
            this.lblNumeroDocumento = new System.Windows.Forms.Label();
            this.cboTipoDocumento = new System.Windows.Forms.ComboBox();
            this.lblTipoDocumento = new System.Windows.Forms.Label();
            this.grpDatosComerciales = new System.Windows.Forms.GroupBox();
            this.cboCondicionPago = new System.Windows.Forms.ComboBox();
            this.lblCondicionPago = new System.Windows.Forms.Label();
            this.cboTipoIva = new System.Windows.Forms.ComboBox();
            this.lblTipoIva = new System.Windows.Forms.Label();
            this.txtCodigoVendedor = new System.Windows.Forms.TextBox();
            this.lblCodigoVendedor = new System.Windows.Forms.Label();
            this.grpContacto = new System.Windows.Forms.GroupBox();
            this.txtDireccion = new System.Windows.Forms.TextBox();
            this.lblDireccion = new System.Windows.Forms.Label();
            this.cboProvincia = new System.Windows.Forms.ComboBox();
            this.lblProvincia = new System.Windows.Forms.Label();
            this.txtLocalidad = new System.Windows.Forms.TextBox();
            this.lblLocalidad = new System.Windows.Forms.Label();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.lblTelefono = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.grpDatosBasicos.SuspendLayout();
            this.grpDocumento.SuspendLayout();
            this.grpDatosComerciales.SuspendLayout();
            this.grpContacto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // grpDatosBasicos
            // 
            this.grpDatosBasicos.Controls.Add(this.txtRazonSocial);
            this.grpDatosBasicos.Controls.Add(this.lblRazonSocial);
            this.grpDatosBasicos.Controls.Add(this.txtCodigoCliente);
            this.grpDatosBasicos.Controls.Add(this.lblCodigoCliente);
            this.grpDatosBasicos.Location = new System.Drawing.Point(12, 12);
            this.grpDatosBasicos.Name = "grpDatosBasicos";
            this.grpDatosBasicos.Size = new System.Drawing.Size(460, 100);
            this.grpDatosBasicos.TabIndex = 0;
            this.grpDatosBasicos.TabStop = false;
            this.grpDatosBasicos.Text = "Datos Básicos";
            // 
            // txtRazonSocial
            // 
            this.txtRazonSocial.Location = new System.Drawing.Point(130, 60);
            this.txtRazonSocial.MaxLength = 200;
            this.txtRazonSocial.Name = "txtRazonSocial";
            this.txtRazonSocial.Size = new System.Drawing.Size(305, 22);
            this.txtRazonSocial.TabIndex = 3;
            // 
            // lblRazonSocial
            // 
            this.lblRazonSocial.AutoSize = true;
            this.lblRazonSocial.Location = new System.Drawing.Point(15, 63);
            this.lblRazonSocial.Name = "lblRazonSocial";
            this.lblRazonSocial.Size = new System.Drawing.Size(90, 16);
            this.lblRazonSocial.TabIndex = 2;
            this.lblRazonSocial.Text = "Razón Social:";
            // 
            // txtCodigoCliente
            // 
            this.txtCodigoCliente.Location = new System.Drawing.Point(130, 25);
            this.txtCodigoCliente.MaxLength = 20;
            this.txtCodigoCliente.Name = "txtCodigoCliente";
            this.txtCodigoCliente.Size = new System.Drawing.Size(200, 22);
            this.txtCodigoCliente.TabIndex = 1;
            // 
            // lblCodigoCliente
            // 
            this.lblCodigoCliente.AutoSize = true;
            this.lblCodigoCliente.Location = new System.Drawing.Point(15, 28);
            this.lblCodigoCliente.Name = "lblCodigoCliente";
            this.lblCodigoCliente.Size = new System.Drawing.Size(98, 16);
            this.lblCodigoCliente.TabIndex = 0;
            this.lblCodigoCliente.Text = "Código Cliente:";
            // 
            // grpDocumento
            // 
            this.grpDocumento.Controls.Add(this.txtNumeroDocumento);
            this.grpDocumento.Controls.Add(this.lblNumeroDocumento);
            this.grpDocumento.Controls.Add(this.cboTipoDocumento);
            this.grpDocumento.Controls.Add(this.lblTipoDocumento);
            this.grpDocumento.Location = new System.Drawing.Point(12, 118);
            this.grpDocumento.Name = "grpDocumento";
            this.grpDocumento.Size = new System.Drawing.Size(460, 100);
            this.grpDocumento.TabIndex = 1;
            this.grpDocumento.TabStop = false;
            this.grpDocumento.Text = "Documento";
            // 
            // txtNumeroDocumento
            // 
            this.txtNumeroDocumento.Location = new System.Drawing.Point(130, 60);
            this.txtNumeroDocumento.MaxLength = 11;
            this.txtNumeroDocumento.Name = "txtNumeroDocumento";
            this.txtNumeroDocumento.Size = new System.Drawing.Size(200, 22);
            this.txtNumeroDocumento.TabIndex = 3;
            this.txtNumeroDocumento.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNumeroDocumento_KeyPress);
            // 
            // lblNumeroDocumento
            // 
            this.lblNumeroDocumento.AutoSize = true;
            this.lblNumeroDocumento.Location = new System.Drawing.Point(15, 63);
            this.lblNumeroDocumento.Name = "lblNumeroDocumento";
            this.lblNumeroDocumento.Size = new System.Drawing.Size(58, 16);
            this.lblNumeroDocumento.TabIndex = 2;
            this.lblNumeroDocumento.Text = "Número:";
            // 
            // cboTipoDocumento
            // 
            this.cboTipoDocumento.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoDocumento.FormattingEnabled = true;
            this.cboTipoDocumento.Location = new System.Drawing.Point(130, 25);
            this.cboTipoDocumento.Name = "cboTipoDocumento";
            this.cboTipoDocumento.Size = new System.Drawing.Size(150, 24);
            this.cboTipoDocumento.TabIndex = 1;
            // 
            // lblTipoDocumento
            // 
            this.lblTipoDocumento.AutoSize = true;
            this.lblTipoDocumento.Location = new System.Drawing.Point(15, 28);
            this.lblTipoDocumento.Name = "lblTipoDocumento";
            this.lblTipoDocumento.Size = new System.Drawing.Size(38, 16);
            this.lblTipoDocumento.TabIndex = 0;
            this.lblTipoDocumento.Text = "Tipo:";
            // 
            // grpDatosComerciales
            // 
            this.grpDatosComerciales.Controls.Add(this.cboCondicionPago);
            this.grpDatosComerciales.Controls.Add(this.lblCondicionPago);
            this.grpDatosComerciales.Controls.Add(this.cboTipoIva);
            this.grpDatosComerciales.Controls.Add(this.lblTipoIva);
            this.grpDatosComerciales.Controls.Add(this.txtCodigoVendedor);
            this.grpDatosComerciales.Controls.Add(this.lblCodigoVendedor);
            this.grpDatosComerciales.Location = new System.Drawing.Point(12, 224);
            this.grpDatosComerciales.Name = "grpDatosComerciales";
            this.grpDatosComerciales.Size = new System.Drawing.Size(460, 140);
            this.grpDatosComerciales.TabIndex = 2;
            this.grpDatosComerciales.TabStop = false;
            this.grpDatosComerciales.Text = "Datos Comerciales";
            // 
            // cboCondicionPago
            // 
            this.cboCondicionPago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCondicionPago.FormattingEnabled = true;
            this.cboCondicionPago.Location = new System.Drawing.Point(130, 100);
            this.cboCondicionPago.Name = "cboCondicionPago";
            this.cboCondicionPago.Size = new System.Drawing.Size(200, 24);
            this.cboCondicionPago.TabIndex = 5;
            // 
            // lblCondicionPago
            // 
            this.lblCondicionPago.AutoSize = true;
            this.lblCondicionPago.Location = new System.Drawing.Point(15, 103);
            this.lblCondicionPago.Name = "lblCondicionPago";
            this.lblCondicionPago.Size = new System.Drawing.Size(106, 16);
            this.lblCondicionPago.TabIndex = 4;
            this.lblCondicionPago.Text = "Condición Pago:";
            // 
            // cboTipoIva
            // 
            this.cboTipoIva.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoIva.FormattingEnabled = true;
            this.cboTipoIva.Location = new System.Drawing.Point(130, 60);
            this.cboTipoIva.Name = "cboTipoIva";
            this.cboTipoIva.Size = new System.Drawing.Size(280, 24);
            this.cboTipoIva.TabIndex = 3;
            // 
            // lblTipoIva
            // 
            this.lblTipoIva.AutoSize = true;
            this.lblTipoIva.Location = new System.Drawing.Point(15, 63);
            this.lblTipoIva.Name = "lblTipoIva";
            this.lblTipoIva.Size = new System.Drawing.Size(62, 16);
            this.lblTipoIva.TabIndex = 2;
            this.lblTipoIva.Text = "Tipo IVA:";
            // 
            // txtCodigoVendedor
            // 
            this.txtCodigoVendedor.Location = new System.Drawing.Point(130, 25);
            this.txtCodigoVendedor.MaxLength = 50;
            this.txtCodigoVendedor.Name = "txtCodigoVendedor";
            this.txtCodigoVendedor.ReadOnly = true;
            this.txtCodigoVendedor.Size = new System.Drawing.Size(305, 22);
            this.txtCodigoVendedor.TabIndex = 1;
            this.txtCodigoVendedor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCodigoVendedor_KeyDown);
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
            // grpContacto
            // 
            this.grpContacto.Controls.Add(this.txtDireccion);
            this.grpContacto.Controls.Add(this.lblDireccion);
            this.grpContacto.Controls.Add(this.cboProvincia);
            this.grpContacto.Controls.Add(this.lblProvincia);
            this.grpContacto.Controls.Add(this.txtLocalidad);
            this.grpContacto.Controls.Add(this.lblLocalidad);
            this.grpContacto.Controls.Add(this.txtTelefono);
            this.grpContacto.Controls.Add(this.lblTelefono);
            this.grpContacto.Controls.Add(this.txtEmail);
            this.grpContacto.Controls.Add(this.lblEmail);
            this.grpContacto.Location = new System.Drawing.Point(12, 370);
            this.grpContacto.Name = "grpContacto";
            this.grpContacto.Size = new System.Drawing.Size(460, 220);
            this.grpContacto.TabIndex = 3;
            this.grpContacto.TabStop = false;
            this.grpContacto.Text = "Datos de Contacto (Opcional)";
            // 
            // txtDireccion
            // 
            this.txtDireccion.Location = new System.Drawing.Point(130, 180);
            this.txtDireccion.MaxLength = 200;
            this.txtDireccion.Name = "txtDireccion";
            this.txtDireccion.Size = new System.Drawing.Size(320, 22);
            this.txtDireccion.TabIndex = 9;
            // 
            // lblDireccion
            // 
            this.lblDireccion.AutoSize = true;
            this.lblDireccion.Location = new System.Drawing.Point(15, 183);
            this.lblDireccion.Name = "lblDireccion";
            this.lblDireccion.Size = new System.Drawing.Size(67, 16);
            this.lblDireccion.TabIndex = 8;
            this.lblDireccion.Text = "Dirección:";
            // 
            // cboProvincia
            // 
            this.cboProvincia.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProvincia.FormattingEnabled = true;
            this.cboProvincia.Location = new System.Drawing.Point(130, 140);
            this.cboProvincia.Name = "cboProvincia";
            this.cboProvincia.Size = new System.Drawing.Size(320, 24);
            this.cboProvincia.TabIndex = 7;
            // 
            // lblProvincia
            // 
            this.lblProvincia.AutoSize = true;
            this.lblProvincia.Location = new System.Drawing.Point(15, 143);
            this.lblProvincia.Name = "lblProvincia";
            this.lblProvincia.Size = new System.Drawing.Size(66, 16);
            this.lblProvincia.TabIndex = 6;
            this.lblProvincia.Text = "Provincia:";
            // 
            // txtLocalidad
            // 
            this.txtLocalidad.Location = new System.Drawing.Point(130, 100);
            this.txtLocalidad.MaxLength = 100;
            this.txtLocalidad.Name = "txtLocalidad";
            this.txtLocalidad.Size = new System.Drawing.Size(320, 22);
            this.txtLocalidad.TabIndex = 5;
            // 
            // lblLocalidad
            // 
            this.lblLocalidad.AutoSize = true;
            this.lblLocalidad.Location = new System.Drawing.Point(15, 103);
            this.lblLocalidad.Name = "lblLocalidad";
            this.lblLocalidad.Size = new System.Drawing.Size(69, 16);
            this.lblLocalidad.TabIndex = 4;
            this.lblLocalidad.Text = "Localidad:";
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
            this.txtEmail.Size = new System.Drawing.Size(305, 22);
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
            this.btnAceptar.Location = new System.Drawing.Point(285, 605);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(90, 30);
            this.btnAceptar.TabIndex = 4;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = true;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(382, 605);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(90, 30);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // frmClienteAlta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 647);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.grpContacto);
            this.Controls.Add(this.grpDatosComerciales);
            this.Controls.Add(this.grpDocumento);
            this.Controls.Add(this.grpDatosBasicos);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmClienteAlta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cliente";
            this.Load += new System.EventHandler(this.frmClienteAlta_Load);
            this.grpDatosBasicos.ResumeLayout(false);
            this.grpDatosBasicos.PerformLayout();
            this.grpDocumento.ResumeLayout(false);
            this.grpDocumento.PerformLayout();
            this.grpDatosComerciales.ResumeLayout(false);
            this.grpDatosComerciales.PerformLayout();
            this.grpContacto.ResumeLayout(false);
            this.grpContacto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpDatosBasicos;
        private System.Windows.Forms.TextBox txtRazonSocial;
        private System.Windows.Forms.Label lblRazonSocial;
        private System.Windows.Forms.TextBox txtCodigoCliente;
        private System.Windows.Forms.Label lblCodigoCliente;
        private System.Windows.Forms.GroupBox grpDocumento;
        private System.Windows.Forms.TextBox txtNumeroDocumento;
        private System.Windows.Forms.Label lblNumeroDocumento;
        private System.Windows.Forms.ComboBox cboTipoDocumento;
        private System.Windows.Forms.Label lblTipoDocumento;
        private System.Windows.Forms.GroupBox grpDatosComerciales;
        private System.Windows.Forms.ComboBox cboCondicionPago;
        private System.Windows.Forms.Label lblCondicionPago;
        private System.Windows.Forms.ComboBox cboTipoIva;
        private System.Windows.Forms.Label lblTipoIva;
        private System.Windows.Forms.TextBox txtCodigoVendedor;
        private System.Windows.Forms.Label lblCodigoVendedor;
        private System.Windows.Forms.GroupBox grpContacto;
        private System.Windows.Forms.TextBox txtDireccion;
        private System.Windows.Forms.Label lblDireccion;
        private System.Windows.Forms.ComboBox cboProvincia;
        private System.Windows.Forms.Label lblProvincia;
        private System.Windows.Forms.TextBox txtLocalidad;
        private System.Windows.Forms.Label lblLocalidad;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.Label lblTelefono;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}
