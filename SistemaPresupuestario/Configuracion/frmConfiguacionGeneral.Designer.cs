namespace SistemaPresupuestario.Configuracion
{
    partial class frmConfiguacionGeneral
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabConfiguracionGeneral = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cboProvincia = new System.Windows.Forms.ComboBox();
            this.lblProvincia = new System.Windows.Forms.Label();
            this.cboTipoIva = new System.Windows.Forms.ComboBox();
            this.lblTipoIva = new System.Windows.Forms.Label();
            this.btnGuardarConfiguracion = new System.Windows.Forms.Button();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.lblTelefono = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtDireccion = new System.Windows.Forms.TextBox();
            this.lblDireccion = new System.Windows.Forms.Label();
            this.txtLocalidad = new System.Windows.Forms.TextBox();
            this.lblLocalidad = new System.Windows.Forms.Label();
            this.txtCUIT = new System.Windows.Forms.TextBox();
            this.lblCUIT = new System.Windows.Forms.Label();
            this.txtRazonSocial = new System.Windows.Forms.TextBox();
            this.lblRazonSocial = new System.Windows.Forms.Label();
            this.tabIdioma = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnCambiarIdioma = new System.Windows.Forms.Button();
            this.rbIngles = new System.Windows.Forms.RadioButton();
            this.rbEspanol = new System.Windows.Forms.RadioButton();
            this.lblIdiomaActual = new System.Windows.Forms.Label();
            this.lblIdiomaActualLabel = new System.Windows.Forms.Label();
            this.tabBackup = new System.Windows.Forms.TabPage();
            this.lblEstado = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvHistorial = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRestaurar = new System.Windows.Forms.Button();
            this.btnCrearBackup = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabConfiguracionGeneral.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabIdioma.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabBackup.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabConfiguracionGeneral);
            this.tabControl1.Controls.Add(this.tabIdioma);
            this.tabControl1.Controls.Add(this.tabBackup);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1067, 615);
            this.tabControl1.TabIndex = 0;
            // 
            // tabConfiguracionGeneral
            // 
            this.tabConfiguracionGeneral.Controls.Add(this.groupBox3);
            this.tabConfiguracionGeneral.Location = new System.Drawing.Point(4, 25);
            this.tabConfiguracionGeneral.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabConfiguracionGeneral.Name = "tabConfiguracionGeneral";
            this.tabConfiguracionGeneral.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabConfiguracionGeneral.Size = new System.Drawing.Size(1059, 586);
            this.tabConfiguracionGeneral.TabIndex = 0;
            this.tabConfiguracionGeneral.Text = "Configuración General";
            this.tabConfiguracionGeneral.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cboProvincia);
            this.groupBox3.Controls.Add(this.lblProvincia);
            this.groupBox3.Controls.Add(this.cboTipoIva);
            this.groupBox3.Controls.Add(this.lblTipoIva);
            this.groupBox3.Controls.Add(this.btnGuardarConfiguracion);
            this.groupBox3.Controls.Add(this.txtTelefono);
            this.groupBox3.Controls.Add(this.lblTelefono);
            this.groupBox3.Controls.Add(this.txtEmail);
            this.groupBox3.Controls.Add(this.lblEmail);
            this.groupBox3.Controls.Add(this.txtDireccion);
            this.groupBox3.Controls.Add(this.lblDireccion);
            this.groupBox3.Controls.Add(this.txtLocalidad);
            this.groupBox3.Controls.Add(this.lblLocalidad);
            this.groupBox3.Controls.Add(this.txtCUIT);
            this.groupBox3.Controls.Add(this.lblCUIT);
            this.groupBox3.Controls.Add(this.txtRazonSocial);
            this.groupBox3.Controls.Add(this.lblRazonSocial);
            this.groupBox3.Location = new System.Drawing.Point(27, 25);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Size = new System.Drawing.Size(1000, 529);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Datos de la Empresa";
            // 
            // cboProvincia
            // 
            this.cboProvincia.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProvincia.FormattingEnabled = true;
            this.cboProvincia.Location = new System.Drawing.Point(200, 247);
            this.cboProvincia.Name = "cboProvincia";
            this.cboProvincia.Size = new System.Drawing.Size(320, 24);
            this.cboProvincia.TabIndex = 16;
            // 
            // lblProvincia
            // 
            this.lblProvincia.AutoSize = true;
            this.lblProvincia.Location = new System.Drawing.Point(40, 250);
            this.lblProvincia.Name = "lblProvincia";
            this.lblProvincia.Size = new System.Drawing.Size(66, 16);
            this.lblProvincia.TabIndex = 15;
            this.lblProvincia.Text = "Provincia:";
            // 
            // cboTipoIva
            // 
            this.cboTipoIva.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoIva.FormattingEnabled = true;
            this.cboTipoIva.Location = new System.Drawing.Point(200, 153);
            this.cboTipoIva.Name = "cboTipoIva";
            this.cboTipoIva.Size = new System.Drawing.Size(280, 24);
            this.cboTipoIva.TabIndex = 14;
            // 
            // lblTipoIva
            // 
            this.lblTipoIva.AutoSize = true;
            this.lblTipoIva.Location = new System.Drawing.Point(40, 156);
            this.lblTipoIva.Name = "lblTipoIva";
            this.lblTipoIva.Size = new System.Drawing.Size(62, 16);
            this.lblTipoIva.TabIndex = 13;
            this.lblTipoIva.Text = "Tipo IVA:";
            // 
            // btnGuardarConfiguracion
            // 
            this.btnGuardarConfiguracion.Location = new System.Drawing.Point(400, 455);
            this.btnGuardarConfiguracion.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnGuardarConfiguracion.Name = "btnGuardarConfiguracion";
            this.btnGuardarConfiguracion.Size = new System.Drawing.Size(200, 43);
            this.btnGuardarConfiguracion.TabIndex = 12;
            this.btnGuardarConfiguracion.Text = "Guardar Configuración";
            this.btnGuardarConfiguracion.UseVisualStyleBackColor = true;
            this.btnGuardarConfiguracion.Click += new System.EventHandler(this.btnGuardarConfiguracion_Click);
            // 
            // txtTelefono
            // 
            this.txtTelefono.Location = new System.Drawing.Point(200, 394);
            this.txtTelefono.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(732, 22);
            this.txtTelefono.TabIndex = 11;
            // 
            // lblTelefono
            // 
            this.lblTelefono.AutoSize = true;
            this.lblTelefono.Location = new System.Drawing.Point(40, 398);
            this.lblTelefono.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTelefono.Name = "lblTelefono";
            this.lblTelefono.Size = new System.Drawing.Size(64, 16);
            this.lblTelefono.TabIndex = 10;
            this.lblTelefono.Text = "Teléfono:";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(200, 345);
            this.txtEmail.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(732, 22);
            this.txtEmail.TabIndex = 9;
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(40, 348);
            this.lblEmail.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(44, 16);
            this.lblEmail.TabIndex = 8;
            this.lblEmail.Text = "Email:";
            // 
            // txtDireccion
            // 
            this.txtDireccion.Location = new System.Drawing.Point(200, 197);
            this.txtDireccion.Margin = new System.Windows.Forms.Padding(4);
            this.txtDireccion.Multiline = true;
            this.txtDireccion.Name = "txtDireccion";
            this.txtDireccion.Size = new System.Drawing.Size(732, 25);
            this.txtDireccion.TabIndex = 7;
            // 
            // lblDireccion
            // 
            this.lblDireccion.AutoSize = true;
            this.lblDireccion.Location = new System.Drawing.Point(40, 201);
            this.lblDireccion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDireccion.Name = "lblDireccion";
            this.lblDireccion.Size = new System.Drawing.Size(67, 16);
            this.lblDireccion.TabIndex = 6;
            this.lblDireccion.Text = "Dirección:";
            // 
            // txtLocalidad
            // 
            this.txtLocalidad.Location = new System.Drawing.Point(200, 295);
            this.txtLocalidad.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtLocalidad.Name = "txtLocalidad";
            this.txtLocalidad.Size = new System.Drawing.Size(732, 22);
            this.txtLocalidad.TabIndex = 5;
            // 
            // lblLocalidad
            // 
            this.lblLocalidad.AutoSize = true;
            this.lblLocalidad.Location = new System.Drawing.Point(40, 299);
            this.lblLocalidad.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLocalidad.Name = "lblLocalidad";
            this.lblLocalidad.Size = new System.Drawing.Size(70, 16);
            this.lblLocalidad.TabIndex = 4;
            this.lblLocalidad.Text = "Localidad:";
            // 
            // txtCUIT
            // 
            this.txtCUIT.Location = new System.Drawing.Point(200, 98);
            this.txtCUIT.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCUIT.MaxLength = 13;
            this.txtCUIT.Name = "txtCUIT";
            this.txtCUIT.Size = new System.Drawing.Size(332, 22);
            this.txtCUIT.TabIndex = 3;
            // 
            // lblCUIT
            // 
            this.lblCUIT.AutoSize = true;
            this.lblCUIT.Location = new System.Drawing.Point(40, 102);
            this.lblCUIT.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCUIT.Name = "lblCUIT";
            this.lblCUIT.Size = new System.Drawing.Size(41, 16);
            this.lblCUIT.TabIndex = 2;
            this.lblCUIT.Text = "CUIT:";
            // 
            // txtRazonSocial
            // 
            this.txtRazonSocial.Location = new System.Drawing.Point(200, 49);
            this.txtRazonSocial.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtRazonSocial.Name = "txtRazonSocial";
            this.txtRazonSocial.Size = new System.Drawing.Size(732, 22);
            this.txtRazonSocial.TabIndex = 1;
            // 
            // lblRazonSocial
            // 
            this.lblRazonSocial.AutoSize = true;
            this.lblRazonSocial.Location = new System.Drawing.Point(40, 53);
            this.lblRazonSocial.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRazonSocial.Name = "lblRazonSocial";
            this.lblRazonSocial.Size = new System.Drawing.Size(90, 16);
            this.lblRazonSocial.TabIndex = 0;
            this.lblRazonSocial.Text = "Razón Social:";
            // 
            // tabIdioma
            // 
            this.tabIdioma.Controls.Add(this.groupBox4);
            this.tabIdioma.Location = new System.Drawing.Point(4, 25);
            this.tabIdioma.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabIdioma.Name = "tabIdioma";
            this.tabIdioma.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabIdioma.Size = new System.Drawing.Size(1059, 586);
            this.tabIdioma.TabIndex = 1;
            this.tabIdioma.Text = "Idioma";
            this.tabIdioma.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnCambiarIdioma);
            this.groupBox4.Controls.Add(this.rbIngles);
            this.groupBox4.Controls.Add(this.rbEspanol);
            this.groupBox4.Controls.Add(this.lblIdiomaActual);
            this.groupBox4.Controls.Add(this.lblIdiomaActualLabel);
            this.groupBox4.Location = new System.Drawing.Point(27, 25);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Size = new System.Drawing.Size(1000, 529);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Selección de Idioma";
            // 
            // btnCambiarIdioma
            // 
            this.btnCambiarIdioma.Location = new System.Drawing.Point(200, 246);
            this.btnCambiarIdioma.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCambiarIdioma.Name = "btnCambiarIdioma";
            this.btnCambiarIdioma.Size = new System.Drawing.Size(200, 43);
            this.btnCambiarIdioma.TabIndex = 4;
            this.btnCambiarIdioma.Text = "Cambiar Idioma";
            this.btnCambiarIdioma.UseVisualStyleBackColor = true;
            this.btnCambiarIdioma.Click += new System.EventHandler(this.btnCambiarIdioma_Click);
            // 
            // rbIngles
            // 
            this.rbIngles.AutoSize = true;
            this.rbIngles.Location = new System.Drawing.Point(200, 185);
            this.rbIngles.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbIngles.Name = "rbIngles";
            this.rbIngles.Size = new System.Drawing.Size(64, 20);
            this.rbIngles.TabIndex = 3;
            this.rbIngles.Text = "Inglés";
            this.rbIngles.UseVisualStyleBackColor = true;
            // 
            // rbEspanol
            // 
            this.rbEspanol.AutoSize = true;
            this.rbEspanol.Checked = true;
            this.rbEspanol.Location = new System.Drawing.Point(200, 135);
            this.rbEspanol.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbEspanol.Name = "rbEspanol";
            this.rbEspanol.Size = new System.Drawing.Size(78, 20);
            this.rbEspanol.TabIndex = 2;
            this.rbEspanol.TabStop = true;
            this.rbEspanol.Text = "Español";
            this.rbEspanol.UseVisualStyleBackColor = true;
            // 
            // lblIdiomaActual
            // 
            this.lblIdiomaActual.AutoSize = true;
            this.lblIdiomaActual.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIdiomaActual.Location = new System.Drawing.Point(196, 74);
            this.lblIdiomaActual.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblIdiomaActual.Name = "lblIdiomaActual";
            this.lblIdiomaActual.Size = new System.Drawing.Size(69, 18);
            this.lblIdiomaActual.TabIndex = 1;
            this.lblIdiomaActual.Text = "Español";
            // 
            // lblIdiomaActualLabel
            // 
            this.lblIdiomaActualLabel.AutoSize = true;
            this.lblIdiomaActualLabel.Location = new System.Drawing.Point(40, 75);
            this.lblIdiomaActualLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblIdiomaActualLabel.Name = "lblIdiomaActualLabel";
            this.lblIdiomaActualLabel.Size = new System.Drawing.Size(91, 16);
            this.lblIdiomaActualLabel.TabIndex = 0;
            this.lblIdiomaActualLabel.Text = "Idioma Actual:";
            // 
            // tabBackup
            // 
            this.tabBackup.Controls.Add(this.lblEstado);
            this.tabBackup.Controls.Add(this.progressBar);
            this.tabBackup.Controls.Add(this.groupBox2);
            this.tabBackup.Controls.Add(this.groupBox1);
            this.tabBackup.Location = new System.Drawing.Point(4, 25);
            this.tabBackup.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabBackup.Name = "tabBackup";
            this.tabBackup.Size = new System.Drawing.Size(1059, 586);
            this.tabBackup.TabIndex = 2;
            this.tabBackup.Text = "BackUp";
            this.tabBackup.UseVisualStyleBackColor = true;
            // 
            // lblEstado
            // 
            this.lblEstado.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEstado.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEstado.ForeColor = System.Drawing.Color.Blue;
            this.lblEstado.Location = new System.Drawing.Point(16, 517);
            this.lblEstado.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(1013, 25);
            this.lblEstado.TabIndex = 3;
            this.lblEstado.Text = "Listo";
            this.lblEstado.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblEstado.Visible = false;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(16, 485);
            this.progressBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1013, 28);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 2;
            this.progressBar.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dgvHistorial);
            this.groupBox2.Location = new System.Drawing.Point(16, 121);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(1013, 357);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Historial de Backups";
            // 
            // dgvHistorial
            // 
            this.dgvHistorial.AllowUserToAddRows = false;
            this.dgvHistorial.AllowUserToDeleteRows = false;
            this.dgvHistorial.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvHistorial.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistorial.Location = new System.Drawing.Point(19, 31);
            this.dgvHistorial.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvHistorial.Name = "dgvHistorial";
            this.dgvHistorial.ReadOnly = true;
            this.dgvHistorial.RowHeadersWidth = 51;
            this.dgvHistorial.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistorial.Size = new System.Drawing.Size(973, 308);
            this.dgvHistorial.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRestaurar);
            this.groupBox1.Controls.Add(this.btnCrearBackup);
            this.groupBox1.Location = new System.Drawing.Point(16, 15);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(1013, 98);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Operaciones";
            // 
            // btnRestaurar
            // 
            this.btnRestaurar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRestaurar.Location = new System.Drawing.Point(267, 31);
            this.btnRestaurar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRestaurar.Name = "btnRestaurar";
            this.btnRestaurar.Size = new System.Drawing.Size(240, 49);
            this.btnRestaurar.TabIndex = 1;
            this.btnRestaurar.Text = "Restaurar Backup";
            this.btnRestaurar.UseVisualStyleBackColor = true;
            this.btnRestaurar.Click += new System.EventHandler(this.btnRestaurar_Click);
            // 
            // btnCrearBackup
            // 
            this.btnCrearBackup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCrearBackup.Location = new System.Drawing.Point(19, 31);
            this.btnCrearBackup.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCrearBackup.Name = "btnCrearBackup";
            this.btnCrearBackup.Size = new System.Drawing.Size(240, 49);
            this.btnCrearBackup.TabIndex = 0;
            this.btnCrearBackup.Text = "Crear Backup";
            this.btnCrearBackup.UseVisualStyleBackColor = true;
            this.btnCrearBackup.Click += new System.EventHandler(this.btnCrearBackup_Click);
            // 
            // frmConfiguacionGeneral
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 615);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(1082, 653);
            this.Name = "frmConfiguacionGeneral";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración del Sistema";
            this.Load += new System.EventHandler(this.frmConfiguacionGeneral_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabConfiguracionGeneral.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabIdioma.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabBackup.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        // Tab Control principal
        private System.Windows.Forms.TabControl tabControl1;
        
        // Pestaña Configuración General
        private System.Windows.Forms.TabPage tabConfiguracionGeneral;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblRazonSocial;
        private System.Windows.Forms.TextBox txtRazonSocial;
        private System.Windows.Forms.Label lblCUIT;
        private System.Windows.Forms.TextBox txtCUIT;
        private System.Windows.Forms.Label lblTipoIva;
        private System.Windows.Forms.ComboBox cboTipoIva;
        private System.Windows.Forms.Label lblDireccion;
        private System.Windows.Forms.TextBox txtDireccion;
        private System.Windows.Forms.Label lblProvincia;
        private System.Windows.Forms.ComboBox cboProvincia;
        private System.Windows.Forms.Label lblLocalidad;
        private System.Windows.Forms.TextBox txtLocalidad;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblTelefono;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.Button btnGuardarConfiguracion;
        
        // Pestaña Idioma
        private System.Windows.Forms.TabPage tabIdioma;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lblIdiomaActualLabel;
        private System.Windows.Forms.Label lblIdiomaActual;
        private System.Windows.Forms.RadioButton rbEspanol;
        private System.Windows.Forms.RadioButton rbIngles;
        private System.Windows.Forms.Button btnCambiarIdioma;
        
        // Pestaña BackUp
        private System.Windows.Forms.TabPage tabBackup;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnRestaurar;
        private System.Windows.Forms.Button btnCrearBackup;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvHistorial;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblEstado;
    }
}
