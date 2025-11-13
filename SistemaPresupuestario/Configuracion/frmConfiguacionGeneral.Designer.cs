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
            this.btnGuardarConfiguracion = new System.Windows.Forms.Button();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDireccion = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtLocalidad = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtProvincia = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCUIT = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtRazonSocial = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabIdioma = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnCambiarIdioma = new System.Windows.Forms.Button();
            this.rbIngles = new System.Windows.Forms.RadioButton();
            this.rbEspanol = new System.Windows.Forms.RadioButton();
            this.lblIdiomaActual = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
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
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabConfiguracionGeneral);
            this.tabControl1.Controls.Add(this.tabIdioma);
            this.tabControl1.Controls.Add(this.tabBackup);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 500);
            this.tabControl1.TabIndex = 0;
            // 
            // tabConfiguracionGeneral
            // 
            this.tabConfiguracionGeneral.Controls.Add(this.groupBox3);
            this.tabConfiguracionGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabConfiguracionGeneral.Name = "tabConfiguracionGeneral";
            this.tabConfiguracionGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfiguracionGeneral.Size = new System.Drawing.Size(792, 474);
            this.tabConfiguracionGeneral.TabIndex = 0;
            this.tabConfiguracionGeneral.Text = "Configuración General";
            this.tabConfiguracionGeneral.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnGuardarConfiguracion);
            this.groupBox3.Controls.Add(this.txtTelefono);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.txtEmail);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.txtDireccion);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.txtLocalidad);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.txtProvincia);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.txtCUIT);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.txtRazonSocial);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(20, 20);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(750, 430);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Datos de la Empresa";
            // 
            // btnGuardarConfiguracion
            // 
            this.btnGuardarConfiguracion.Location = new System.Drawing.Point(300, 370);
            this.btnGuardarConfiguracion.Name = "btnGuardarConfiguracion";
            this.btnGuardarConfiguracion.Size = new System.Drawing.Size(150, 35);
            this.btnGuardarConfiguracion.TabIndex = 14;
            this.btnGuardarConfiguracion.Text = "Guardar Configuración";
            this.btnGuardarConfiguracion.UseVisualStyleBackColor = true;
            this.btnGuardarConfiguracion.Click += new System.EventHandler(this.btnGuardarConfiguracion_Click);
            // 
            // txtTelefono
            // 
            this.txtTelefono.Location = new System.Drawing.Point(150, 320);
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(550, 20);
            this.txtTelefono.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(30, 323);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Teléfono:";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(150, 280);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(550, 20);
            this.txtEmail.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(30, 283);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Email:";
            // 
            // txtDireccion
            // 
            this.txtDireccion.Location = new System.Drawing.Point(150, 160);
            this.txtDireccion.Multiline = true;
            this.txtDireccion.Name = "txtDireccion";
            this.txtDireccion.Size = new System.Drawing.Size(550, 60);
            this.txtDireccion.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(30, 163);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Dirección:";
            // 
            // txtLocalidad
            // 
            this.txtLocalidad.Location = new System.Drawing.Point(150, 240);
            this.txtLocalidad.Name = "txtLocalidad";
            this.txtLocalidad.Size = new System.Drawing.Size(550, 20);
            this.txtLocalidad.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 243);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Localidad:";
            // 
            // txtProvincia
            // 
            this.txtProvincia.Location = new System.Drawing.Point(150, 200);
            this.txtProvincia.Name = "txtProvincia";
            this.txtProvincia.Size = new System.Drawing.Size(550, 20);
            this.txtProvincia.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 203);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Provincia:";
            // 
            // txtCUIT
            // 
            this.txtCUIT.Location = new System.Drawing.Point(150, 80);
            this.txtCUIT.MaxLength = 13;
            this.txtCUIT.Name = "txtCUIT";
            this.txtCUIT.Size = new System.Drawing.Size(250, 20);
            this.txtCUIT.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "CUIT:";
            // 
            // txtRazonSocial
            // 
            this.txtRazonSocial.Location = new System.Drawing.Point(150, 40);
            this.txtRazonSocial.Name = "txtRazonSocial";
            this.txtRazonSocial.Size = new System.Drawing.Size(550, 20);
            this.txtRazonSocial.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Razón Social:";
            // 
            // tabIdioma
            // 
            this.tabIdioma.Controls.Add(this.groupBox4);
            this.tabIdioma.Location = new System.Drawing.Point(4, 22);
            this.tabIdioma.Name = "tabIdioma";
            this.tabIdioma.Padding = new System.Windows.Forms.Padding(3);
            this.tabIdioma.Size = new System.Drawing.Size(792, 474);
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
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Location = new System.Drawing.Point(20, 20);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(750, 430);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Selección de Idioma";
            // 
            // btnCambiarIdioma
            // 
            this.btnCambiarIdioma.Location = new System.Drawing.Point(150, 200);
            this.btnCambiarIdioma.Name = "btnCambiarIdioma";
            this.btnCambiarIdioma.Size = new System.Drawing.Size(150, 35);
            this.btnCambiarIdioma.TabIndex = 4;
            this.btnCambiarIdioma.Text = "Cambiar Idioma";
            this.btnCambiarIdioma.UseVisualStyleBackColor = true;
            this.btnCambiarIdioma.Click += new System.EventHandler(this.btnCambiarIdioma_Click);
            // 
            // rbIngles
            // 
            this.rbIngles.AutoSize = true;
            this.rbIngles.Location = new System.Drawing.Point(150, 150);
            this.rbIngles.Name = "rbIngles";
            this.rbIngles.Size = new System.Drawing.Size(57, 17);
            this.rbIngles.TabIndex = 3;
            this.rbIngles.Text = "Inglés";
            this.rbIngles.UseVisualStyleBackColor = true;
            // 
            // rbEspanol
            // 
            this.rbEspanol.AutoSize = true;
            this.rbEspanol.Checked = true;
            this.rbEspanol.Location = new System.Drawing.Point(150, 110);
            this.rbEspanol.Name = "rbEspanol";
            this.rbEspanol.Size = new System.Drawing.Size(64, 17);
            this.rbEspanol.TabIndex = 2;
            this.rbEspanol.TabStop = true;
            this.rbEspanol.Text = "Español";
            this.rbEspanol.UseVisualStyleBackColor = true;
            // 
            // lblIdiomaActual
            // 
            this.lblIdiomaActual.AutoSize = true;
            this.lblIdiomaActual.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIdiomaActual.Location = new System.Drawing.Point(147, 60);
            this.lblIdiomaActual.Name = "lblIdiomaActual";
            this.lblIdiomaActual.Size = new System.Drawing.Size(56, 15);
            this.lblIdiomaActual.TabIndex = 1;
            this.lblIdiomaActual.Text = "Español";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(30, 61);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(74, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Idioma Actual:";
            // 
            // tabBackup
            // 
            this.tabBackup.Controls.Add(this.lblEstado);
            this.tabBackup.Controls.Add(this.progressBar);
            this.tabBackup.Controls.Add(this.groupBox2);
            this.tabBackup.Controls.Add(this.groupBox1);
            this.tabBackup.Location = new System.Drawing.Point(4, 22);
            this.tabBackup.Name = "tabBackup";
            this.tabBackup.Size = new System.Drawing.Size(792, 474);
            this.tabBackup.TabIndex = 2;
            this.tabBackup.Text = "BackUp";
            this.tabBackup.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRestaurar);
            this.groupBox1.Controls.Add(this.btnCrearBackup);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(760, 80);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Operaciones";
            // 
            // btnRestaurar
            // 
            this.btnRestaurar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRestaurar.Location = new System.Drawing.Point(200, 25);
            this.btnRestaurar.Name = "btnRestaurar";
            this.btnRestaurar.Size = new System.Drawing.Size(180, 40);
            this.btnRestaurar.TabIndex = 1;
            this.btnRestaurar.Text = "Restaurar Backup";
            this.btnRestaurar.UseVisualStyleBackColor = true;
            this.btnRestaurar.Click += new System.EventHandler(this.btnRestaurar_Click);
            // 
            // btnCrearBackup
            // 
            this.btnCrearBackup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCrearBackup.Location = new System.Drawing.Point(14, 25);
            this.btnCrearBackup.Name = "btnCrearBackup";
            this.btnCrearBackup.Size = new System.Drawing.Size(180, 40);
            this.btnCrearBackup.TabIndex = 0;
            this.btnCrearBackup.Text = "Crear Backup";
            this.btnCrearBackup.UseVisualStyleBackColor = true;
            this.btnCrearBackup.Click += new System.EventHandler(this.btnCrearBackup_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dgvHistorial);
            this.groupBox2.Location = new System.Drawing.Point(12, 98);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(760, 290);
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
            this.dgvHistorial.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHistorial.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistorial.Location = new System.Drawing.Point(14, 25);
            this.dgvHistorial.Name = "dgvHistorial";
            this.dgvHistorial.ReadOnly = true;
            this.dgvHistorial.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistorial.Size = new System.Drawing.Size(730, 250);
            this.dgvHistorial.TabIndex = 0;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 394);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(760, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 2;
            this.progressBar.Visible = false;
            // 
            // lblEstado
            // 
            this.lblEstado.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEstado.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEstado.ForeColor = System.Drawing.Color.Blue;
            this.lblEstado.Location = new System.Drawing.Point(12, 420);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(760, 20);
            this.lblEstado.TabIndex = 3;
            this.lblEstado.Text = "Listo";
            this.lblEstado.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblEstado.Visible = false;
            // 
            // frmConfiguacionGeneral
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(816, 539);
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        // Tab Control principal
        private System.Windows.Forms.TabControl tabControl1;
        
        // Pestaña Configuración General
        private System.Windows.Forms.TabPage tabConfiguracionGeneral;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtRazonSocial;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCUIT;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtProvincia;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtLocalidad;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDireccion;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.Button btnGuardarConfiguracion;
        
        // Pestaña Idioma
        private System.Windows.Forms.TabPage tabIdioma;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label9;
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
