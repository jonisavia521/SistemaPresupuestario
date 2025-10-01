namespace SistemaPresupuestario.Maestros.Seguridad
{
    partial class FrmUsuarioEdit
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
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblConfirmPassword = new System.Windows.Forms.Label();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.chkActivo = new System.Windows.Forms.CheckBox();
            this.chkCambiarPassword = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabDatos = new System.Windows.Forms.TabPage();
            this.tabFamilias = new System.Windows.Forms.TabPage();
            this.tvFamilias = new System.Windows.Forms.TreeView();
            this.tabPatentes = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDesasignarPatente = new System.Windows.Forms.Button();
            this.btnAsignarPatente = new System.Windows.Forms.Button();
            this.lstPatentesAsignadas = new System.Windows.Forms.ListBox();
            this.lstPatentesDisponibles = new System.Windows.Forms.ListBox();
            this.lblPatentesAsignadas = new System.Windows.Forms.Label();
            this.lblPatentesDisponibles = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.tabControl1.SuspendLayout();
            this.tabDatos.SuspendLayout();
            this.tabFamilias.SuspendLayout();
            this.tabPatentes.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(27, 25);
            this.lblNombre.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(59, 16);
            this.lblNombre.TabIndex = 0;
            this.lblNombre.Text = "Nombre:";
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(107, 21);
            this.txtNombre.Margin = new System.Windows.Forms.Padding(4);
            this.txtNombre.MaxLength = 1000;
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(399, 22);
            this.txtNombre.TabIndex = 1;
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Location = new System.Drawing.Point(27, 62);
            this.lblUsuario.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(57, 16);
            this.lblUsuario.TabIndex = 2;
            this.lblUsuario.Text = "Usuario:";
            // 
            // txtUsuario
            // 
            this.txtUsuario.Location = new System.Drawing.Point(107, 58);
            this.txtUsuario.Margin = new System.Windows.Forms.Padding(4);
            this.txtUsuario.MaxLength = 20;
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(265, 22);
            this.txtUsuario.TabIndex = 3;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(27, 98);
            this.lblPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(79, 16);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "Contraseña:";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(160, 95);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4);
            this.txtPassword.MaxLength = 50;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(265, 22);
            this.txtPassword.TabIndex = 5;
            // 
            // lblConfirmPassword
            // 
            this.lblConfirmPassword.AutoSize = true;
            this.lblConfirmPassword.Location = new System.Drawing.Point(27, 135);
            this.lblConfirmPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblConfirmPassword.Name = "lblConfirmPassword";
            this.lblConfirmPassword.Size = new System.Drawing.Size(139, 16);
            this.lblConfirmPassword.TabIndex = 6;
            this.lblConfirmPassword.Text = "Confirmar Contraseña:";
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.Location = new System.Drawing.Point(160, 132);
            this.txtConfirmPassword.Margin = new System.Windows.Forms.Padding(4);
            this.txtConfirmPassword.MaxLength = 50;
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.PasswordChar = '*';
            this.txtConfirmPassword.Size = new System.Drawing.Size(265, 22);
            this.txtConfirmPassword.TabIndex = 7;
            // 
            // chkActivo
            // 
            this.chkActivo.AutoSize = true;
            this.chkActivo.Checked = true;
            this.chkActivo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActivo.Location = new System.Drawing.Point(467, 60);
            this.chkActivo.Margin = new System.Windows.Forms.Padding(4);
            this.chkActivo.Name = "chkActivo";
            this.chkActivo.Size = new System.Drawing.Size(66, 20);
            this.chkActivo.TabIndex = 8;
            this.chkActivo.Text = "Activo";
            this.chkActivo.UseVisualStyleBackColor = true;
            // 
            // chkCambiarPassword
            // 
            this.chkCambiarPassword.AutoSize = true;
            this.chkCambiarPassword.Location = new System.Drawing.Point(467, 97);
            this.chkCambiarPassword.Margin = new System.Windows.Forms.Padding(4);
            this.chkCambiarPassword.Name = "chkCambiarPassword";
            this.chkCambiarPassword.Size = new System.Drawing.Size(150, 20);
            this.chkCambiarPassword.TabIndex = 9;
            this.chkCambiarPassword.Text = "Cambiar contraseña";
            this.chkCambiarPassword.UseVisualStyleBackColor = true;
            this.chkCambiarPassword.Visible = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabDatos);
            this.tabControl1.Controls.Add(this.tabFamilias);
            this.tabControl1.Controls.Add(this.tabPatentes);
            this.tabControl1.Location = new System.Drawing.Point(16, 15);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1013, 492);
            this.tabControl1.TabIndex = 10;
            // 
            // tabDatos
            // 
            this.tabDatos.Controls.Add(this.chkCambiarPassword);
            this.tabDatos.Controls.Add(this.chkActivo);
            this.tabDatos.Controls.Add(this.txtConfirmPassword);
            this.tabDatos.Controls.Add(this.lblConfirmPassword);
            this.tabDatos.Controls.Add(this.txtPassword);
            this.tabDatos.Controls.Add(this.lblPassword);
            this.tabDatos.Controls.Add(this.txtUsuario);
            this.tabDatos.Controls.Add(this.lblUsuario);
            this.tabDatos.Controls.Add(this.txtNombre);
            this.tabDatos.Controls.Add(this.lblNombre);
            this.tabDatos.Location = new System.Drawing.Point(4, 25);
            this.tabDatos.Margin = new System.Windows.Forms.Padding(4);
            this.tabDatos.Name = "tabDatos";
            this.tabDatos.Padding = new System.Windows.Forms.Padding(4);
            this.tabDatos.Size = new System.Drawing.Size(1005, 463);
            this.tabDatos.TabIndex = 0;
            this.tabDatos.Text = "Datos Básicos";
            this.tabDatos.UseVisualStyleBackColor = true;
            // 
            // tabFamilias
            // 
            this.tabFamilias.Controls.Add(this.tvFamilias);
            this.tabFamilias.Location = new System.Drawing.Point(4, 25);
            this.tabFamilias.Margin = new System.Windows.Forms.Padding(4);
            this.tabFamilias.Name = "tabFamilias";
            this.tabFamilias.Padding = new System.Windows.Forms.Padding(4);
            this.tabFamilias.Size = new System.Drawing.Size(1005, 463);
            this.tabFamilias.TabIndex = 1;
            this.tabFamilias.Text = "Familias";
            this.tabFamilias.UseVisualStyleBackColor = true;
            // 
            // tvFamilias
            // 
            this.tvFamilias.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvFamilias.CheckBoxes = true;
            this.tvFamilias.Location = new System.Drawing.Point(8, 7);
            this.tvFamilias.Margin = new System.Windows.Forms.Padding(4);
            this.tvFamilias.Name = "tvFamilias";
            this.tvFamilias.Size = new System.Drawing.Size(985, 445);
            this.tvFamilias.TabIndex = 0;
            // 
            // tabPatentes
            // 
            this.tabPatentes.Controls.Add(this.panel1);
            this.tabPatentes.Location = new System.Drawing.Point(4, 25);
            this.tabPatentes.Margin = new System.Windows.Forms.Padding(4);
            this.tabPatentes.Name = "tabPatentes";
            this.tabPatentes.Size = new System.Drawing.Size(1005, 463);
            this.tabPatentes.TabIndex = 2;
            this.tabPatentes.Text = "Patentes";
            this.tabPatentes.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnDesasignarPatente);
            this.panel1.Controls.Add(this.btnAsignarPatente);
            this.panel1.Controls.Add(this.lstPatentesAsignadas);
            this.panel1.Controls.Add(this.lstPatentesDisponibles);
            this.panel1.Controls.Add(this.lblPatentesAsignadas);
            this.panel1.Controls.Add(this.lblPatentesDisponibles);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1005, 463);
            this.panel1.TabIndex = 0;
            // 
            // btnDesasignarPatente
            // 
            this.btnDesasignarPatente.Location = new System.Drawing.Point(480, 222);
            this.btnDesasignarPatente.Margin = new System.Windows.Forms.Padding(4);
            this.btnDesasignarPatente.Name = "btnDesasignarPatente";
            this.btnDesasignarPatente.Size = new System.Drawing.Size(43, 28);
            this.btnDesasignarPatente.TabIndex = 5;
            this.btnDesasignarPatente.Text = "<";
            this.btnDesasignarPatente.UseVisualStyleBackColor = true;
            // 
            // btnAsignarPatente
            // 
            this.btnAsignarPatente.Location = new System.Drawing.Point(480, 185);
            this.btnAsignarPatente.Margin = new System.Windows.Forms.Padding(4);
            this.btnAsignarPatente.Name = "btnAsignarPatente";
            this.btnAsignarPatente.Size = new System.Drawing.Size(43, 28);
            this.btnAsignarPatente.TabIndex = 4;
            this.btnAsignarPatente.Text = ">";
            this.btnAsignarPatente.UseVisualStyleBackColor = true;
            // 
            // lstPatentesAsignadas
            // 
            this.lstPatentesAsignadas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstPatentesAsignadas.FormattingEnabled = true;
            this.lstPatentesAsignadas.ItemHeight = 16;
            this.lstPatentesAsignadas.Location = new System.Drawing.Point(535, 37);
            this.lstPatentesAsignadas.Margin = new System.Windows.Forms.Padding(4);
            this.lstPatentesAsignadas.Name = "lstPatentesAsignadas";
            this.lstPatentesAsignadas.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstPatentesAsignadas.Size = new System.Drawing.Size(452, 404);
            this.lstPatentesAsignadas.TabIndex = 3;
            // 
            // lstPatentesDisponibles
            // 
            this.lstPatentesDisponibles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstPatentesDisponibles.FormattingEnabled = true;
            this.lstPatentesDisponibles.ItemHeight = 16;
            this.lstPatentesDisponibles.Location = new System.Drawing.Point(16, 37);
            this.lstPatentesDisponibles.Margin = new System.Windows.Forms.Padding(4);
            this.lstPatentesDisponibles.Name = "lstPatentesDisponibles";
            this.lstPatentesDisponibles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstPatentesDisponibles.Size = new System.Drawing.Size(454, 404);
            this.lstPatentesDisponibles.TabIndex = 2;
            // 
            // lblPatentesAsignadas
            // 
            this.lblPatentesAsignadas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPatentesAsignadas.AutoSize = true;
            this.lblPatentesAsignadas.Location = new System.Drawing.Point(535, 12);
            this.lblPatentesAsignadas.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPatentesAsignadas.Name = "lblPatentesAsignadas";
            this.lblPatentesAsignadas.Size = new System.Drawing.Size(131, 16);
            this.lblPatentesAsignadas.TabIndex = 1;
            this.lblPatentesAsignadas.Text = "Patentes Asignadas:";
            // 
            // lblPatentesDisponibles
            // 
            this.lblPatentesDisponibles.AutoSize = true;
            this.lblPatentesDisponibles.Location = new System.Drawing.Point(16, 12);
            this.lblPatentesDisponibles.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPatentesDisponibles.Name = "lblPatentesDisponibles";
            this.lblPatentesDisponibles.Size = new System.Drawing.Size(138, 16);
            this.lblPatentesDisponibles.TabIndex = 0;
            this.lblPatentesDisponibles.Text = "Patentes Disponibles:";
            // 
            // btnGuardar
            // 
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.Enabled = false;
            this.btnGuardar.Location = new System.Drawing.Point(821, 514);
            this.btnGuardar.Margin = new System.Windows.Forms.Padding(4);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(100, 28);
            this.btnGuardar.TabIndex = 11;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.BtnGuardar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.Location = new System.Drawing.Point(929, 514);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(100, 28);
            this.btnCancelar.TabIndex = 12;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.BtnCancelar_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // FrmUsuarioEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1045, 558);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmUsuarioEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Usuario";
            this.Load += new System.EventHandler(this.FrmUsuarioEdit_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabDatos.ResumeLayout(false);
            this.tabDatos.PerformLayout();
            this.tabFamilias.ResumeLayout(false);
            this.tabPatentes.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblConfirmPassword;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private System.Windows.Forms.CheckBox chkActivo;
        private System.Windows.Forms.CheckBox chkCambiarPassword;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabDatos;
        private System.Windows.Forms.TabPage tabFamilias;
        private System.Windows.Forms.TreeView tvFamilias;
        private System.Windows.Forms.TabPage tabPatentes;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnDesasignarPatente;
        private System.Windows.Forms.Button btnAsignarPatente;
        private System.Windows.Forms.ListBox lstPatentesAsignadas;
        private System.Windows.Forms.ListBox lstPatentesDisponibles;
        private System.Windows.Forms.Label lblPatentesAsignadas;
        private System.Windows.Forms.Label lblPatentesDisponibles;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}