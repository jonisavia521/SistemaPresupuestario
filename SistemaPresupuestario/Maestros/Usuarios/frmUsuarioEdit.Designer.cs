namespace SistemaPresupuestario.Maestros.Usuarios
{
    partial class frmUsuarioEdit
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
            this.lblClave = new System.Windows.Forms.Label();
            this.txtClave = new System.Windows.Forms.TextBox();
            this.lblConfirmarClave = new System.Windows.Forms.Label();
            this.txtConfirmarClave = new System.Windows.Forms.TextBox();
            this.chkCambiarClave = new System.Windows.Forms.CheckBox();
            this.grpDatosBasicos = new System.Windows.Forms.GroupBox();
            this.grpPermisos = new System.Windows.Forms.GroupBox();
            this.tabPermisos = new System.Windows.Forms.TabControl();
            this.tabFamilias = new System.Windows.Forms.TabPage();
            this.tvFamilias = new System.Windows.Forms.TreeView();
            this.tabPatentes = new System.Windows.Forms.TabPage();
            this.clbPatentes = new System.Windows.Forms.CheckedListBox();
            this.tabPermisosEfectivos = new System.Windows.Forms.TabPage();
            this.lvPermisosEfectivos = new System.Windows.Forms.ListView();
            this.colTipo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colNombre = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colOrigen = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnActualizarPermisos = new System.Windows.Forms.Button();
            this.grpDatosBasicos.SuspendLayout();
            this.grpPermisos.SuspendLayout();
            this.tabPermisos.SuspendLayout();
            this.tabFamilias.SuspendLayout();
            this.tabPatentes.SuspendLayout();
            this.tabPermisosEfectivos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(15, 30);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(59, 16);
            this.lblNombre.TabIndex = 0;
            this.lblNombre.Text = "Nombre:";
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(100, 27);
            this.txtNombre.MaxLength = 1000;
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(300, 22);
            this.txtNombre.TabIndex = 1;
            this.txtNombre.Validating += new System.ComponentModel.CancelEventHandler(this.txtNombre_Validating);
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Location = new System.Drawing.Point(15, 65);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(57, 16);
            this.lblUsuario.TabIndex = 2;
            this.lblUsuario.Text = "Usuario:";
            // 
            // txtUsuario
            // 
            this.txtUsuario.Location = new System.Drawing.Point(100, 62);
            this.txtUsuario.MaxLength = 20;
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(200, 22);
            this.txtUsuario.TabIndex = 3;
            this.txtUsuario.Validating += new System.ComponentModel.CancelEventHandler(this.txtUsuario_Validating);
            // 
            // lblClave
            // 
            this.lblClave.AutoSize = true;
            this.lblClave.Location = new System.Drawing.Point(15, 135);
            this.lblClave.Name = "lblClave";
            this.lblClave.Size = new System.Drawing.Size(44, 16);
            this.lblClave.TabIndex = 6;
            this.lblClave.Text = "Clave:";
            // 
            // txtClave
            // 
            this.txtClave.Location = new System.Drawing.Point(100, 132);
            this.txtClave.MaxLength = 50;
            this.txtClave.Name = "txtClave";
            this.txtClave.PasswordChar = '*';
            this.txtClave.Size = new System.Drawing.Size(200, 22);
            this.txtClave.TabIndex = 7;
            this.txtClave.Validating += new System.ComponentModel.CancelEventHandler(this.txtClave_Validating);
            // 
            // lblConfirmarClave
            // 
            this.lblConfirmarClave.AutoSize = true;
            this.lblConfirmarClave.Location = new System.Drawing.Point(15, 170);
            this.lblConfirmarClave.Name = "lblConfirmarClave";
            this.lblConfirmarClave.Size = new System.Drawing.Size(114, 16);
            this.lblConfirmarClave.TabIndex = 8;
            this.lblConfirmarClave.Text = "Confirmar Clave:";
            // 
            // txtConfirmarClave
            // 
            this.txtConfirmarClave.Location = new System.Drawing.Point(135, 167);
            this.txtConfirmarClave.MaxLength = 50;
            this.txtConfirmarClave.Name = "txtConfirmarClave";
            this.txtConfirmarClave.PasswordChar = '*';
            this.txtConfirmarClave.Size = new System.Drawing.Size(200, 22);
            this.txtConfirmarClave.TabIndex = 9;
            this.txtConfirmarClave.Validating += new System.ComponentModel.CancelEventHandler(this.txtConfirmarClave_Validating);
            // 
            // chkCambiarClave
            // 
            this.chkCambiarClave.AutoSize = true;
            this.chkCambiarClave.Location = new System.Drawing.Point(100, 100);
            this.chkCambiarClave.Name = "chkCambiarClave";
            this.chkCambiarClave.Size = new System.Drawing.Size(125, 20);
            this.chkCambiarClave.TabIndex = 5;
            this.chkCambiarClave.Text = "Cambiar Clave";
            this.chkCambiarClave.UseVisualStyleBackColor = true;
            this.chkCambiarClave.CheckedChanged += new System.EventHandler(this.chkCambiarClave_CheckedChanged);
            // 
            // grpDatosBasicos
            // 
            this.grpDatosBasicos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDatosBasicos.Controls.Add(this.lblNombre);
            this.grpDatosBasicos.Controls.Add(this.txtNombre);
            this.grpDatosBasicos.Controls.Add(this.lblUsuario);
            this.grpDatosBasicos.Controls.Add(this.txtUsuario);
            this.grpDatosBasicos.Controls.Add(this.chkCambiarClave);
            this.grpDatosBasicos.Controls.Add(this.lblClave);
            this.grpDatosBasicos.Controls.Add(this.txtClave);
            this.grpDatosBasicos.Controls.Add(this.lblConfirmarClave);
            this.grpDatosBasicos.Controls.Add(this.txtConfirmarClave);
            this.grpDatosBasicos.Location = new System.Drawing.Point(12, 12);
            this.grpDatosBasicos.Name = "grpDatosBasicos";
            this.grpDatosBasicos.Size = new System.Drawing.Size(760, 210);
            this.grpDatosBasicos.TabIndex = 0;
            this.grpDatosBasicos.TabStop = false;
            this.grpDatosBasicos.Text = "Datos Básicos";
            // 
            // grpPermisos
            // 
            this.grpPermisos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPermisos.Controls.Add(this.btnActualizarPermisos);
            this.grpPermisos.Controls.Add(this.tabPermisos);
            this.grpPermisos.Location = new System.Drawing.Point(12, 228);
            this.grpPermisos.Name = "grpPermisos";
            this.grpPermisos.Size = new System.Drawing.Size(760, 350);
            this.grpPermisos.TabIndex = 1;
            this.grpPermisos.TabStop = false;
            this.grpPermisos.Text = "Permisos";
            // 
            // tabPermisos
            // 
            this.tabPermisos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabPermisos.Controls.Add(this.tabFamilias);
            this.tabPermisos.Controls.Add(this.tabPatentes);
            this.tabPermisos.Controls.Add(this.tabPermisosEfectivos);
            this.tabPermisos.Location = new System.Drawing.Point(6, 50);
            this.tabPermisos.Name = "tabPermisos";
            this.tabPermisos.SelectedIndex = 0;
            this.tabPermisos.Size = new System.Drawing.Size(748, 294);
            this.tabPermisos.TabIndex = 1;
            // 
            // tabFamilias
            // 
            this.tabFamilias.Controls.Add(this.tvFamilias);
            this.tabFamilias.Location = new System.Drawing.Point(4, 25);
            this.tabFamilias.Name = "tabFamilias";
            this.tabFamilias.Padding = new System.Windows.Forms.Padding(3);
            this.tabFamilias.Size = new System.Drawing.Size(740, 265);
            this.tabFamilias.TabIndex = 0;
            this.tabFamilias.Text = "Familias";
            this.tabFamilias.UseVisualStyleBackColor = true;
            // 
            // tvFamilias
            // 
            this.tvFamilias.CheckBoxes = true;
            this.tvFamilias.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFamilias.Location = new System.Drawing.Point(3, 3);
            this.tvFamilias.Name = "tvFamilias";
            this.tvFamilias.Size = new System.Drawing.Size(734, 259);
            this.tvFamilias.TabIndex = 0;
            this.tvFamilias.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvFamilias_AfterCheck);
            // 
            // tabPatentes
            // 
            this.tabPatentes.Controls.Add(this.clbPatentes);
            this.tabPatentes.Location = new System.Drawing.Point(4, 25);
            this.tabPatentes.Name = "tabPatentes";
            this.tabPatentes.Padding = new System.Windows.Forms.Padding(3);
            this.tabPatentes.Size = new System.Drawing.Size(740, 265);
            this.tabPatentes.TabIndex = 1;
            this.tabPatentes.Text = "Patentes Directas";
            this.tabPatentes.UseVisualStyleBackColor = true;
            // 
            // clbPatentes
            // 
            this.clbPatentes.CheckOnClick = true;
            this.clbPatentes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbPatentes.FormattingEnabled = true;
            this.clbPatentes.Location = new System.Drawing.Point(3, 3);
            this.clbPatentes.Name = "clbPatentes";
            this.clbPatentes.Size = new System.Drawing.Size(734, 259);
            this.clbPatentes.TabIndex = 0;
            // 
            // tabPermisosEfectivos
            // 
            this.tabPermisosEfectivos.Controls.Add(this.lvPermisosEfectivos);
            this.tabPermisosEfectivos.Location = new System.Drawing.Point(4, 25);
            this.tabPermisosEfectivos.Name = "tabPermisosEfectivos";
            this.tabPermisosEfectivos.Size = new System.Drawing.Size(740, 265);
            this.tabPermisosEfectivos.TabIndex = 2;
            this.tabPermisosEfectivos.Text = "Permisos Efectivos";
            this.tabPermisosEfectivos.UseVisualStyleBackColor = true;
            // 
            // lvPermisosEfectivos
            // 
            this.lvPermisosEfectivos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTipo,
            this.colNombre,
            this.colOrigen});
            this.lvPermisosEfectivos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvPermisosEfectivos.FullRowSelect = true;
            this.lvPermisosEfectivos.GridLines = true;
            this.lvPermisosEfectivos.HideSelection = false;
            this.lvPermisosEfectivos.Location = new System.Drawing.Point(0, 0);
            this.lvPermisosEfectivos.Name = "lvPermisosEfectivos";
            this.lvPermisosEfectivos.Size = new System.Drawing.Size(740, 265);
            this.lvPermisosEfectivos.TabIndex = 0;
            this.lvPermisosEfectivos.UseCompatibleStateImageBehavior = false;
            this.lvPermisosEfectivos.View = System.Windows.Forms.View.Details;
            // 
            // colTipo
            // 
            this.colTipo.Text = "Tipo";
            this.colTipo.Width = 100;
            // 
            // colNombre
            // 
            this.colNombre.Text = "Nombre";
            this.colNombre.Width = 300;
            // 
            // colOrigen
            // 
            this.colOrigen.Text = "Origen";
            this.colOrigen.Width = 300;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnGuardar.Location = new System.Drawing.Point(616, 584);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(75, 23);
            this.btnGuardar.TabIndex = 2;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(697, 584);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 23);
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 619);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(784, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 17);
            this.lblStatus.Text = "Listo";
            // 
            // btnActualizarPermisos
            // 
            this.btnActualizarPermisos.Location = new System.Drawing.Point(6, 21);
            this.btnActualizarPermisos.Name = "btnActualizarPermisos";
            this.btnActualizarPermisos.Size = new System.Drawing.Size(150, 23);
            this.btnActualizarPermisos.TabIndex = 0;
            this.btnActualizarPermisos.Text = "Actualizar Permisos";
            this.btnActualizarPermisos.UseVisualStyleBackColor = true;
            this.btnActualizarPermisos.Click += new System.EventHandler(this.btnActualizarPermisos_Click);
            // 
            // frmUsuarioEdit
            // 
            this.AcceptButton = this.btnGuardar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(784, 641);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.grpPermisos);
            this.Controls.Add(this.grpDatosBasicos);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "frmUsuarioEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Editar Usuario";
            this.Load += new System.EventHandler(this.frmUsuarioEdit_Load);
            this.grpDatosBasicos.ResumeLayout(false);
            this.grpDatosBasicos.PerformLayout();
            this.grpPermisos.ResumeLayout(false);
            this.tabPermisos.ResumeLayout(false);
            this.tabFamilias.ResumeLayout(false);
            this.tabPatentes.ResumeLayout(false);
            this.tabPermisosEfectivos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label lblClave;
        private System.Windows.Forms.TextBox txtClave;
        private System.Windows.Forms.Label lblConfirmarClave;
        private System.Windows.Forms.TextBox txtConfirmarClave;
        private System.Windows.Forms.CheckBox chkCambiarClave;
        private System.Windows.Forms.GroupBox grpDatosBasicos;
        private System.Windows.Forms.GroupBox grpPermisos;
        private System.Windows.Forms.TabControl tabPermisos;
        private System.Windows.Forms.TabPage tabFamilias;
        private System.Windows.Forms.TreeView tvFamilias;
        private System.Windows.Forms.TabPage tabPatentes;
        private System.Windows.Forms.CheckedListBox clbPatentes;
        private System.Windows.Forms.TabPage tabPermisosEfectivos;
        private System.Windows.Forms.ListView lvPermisosEfectivos;
        private System.Windows.Forms.ColumnHeader colTipo;
        private System.Windows.Forms.ColumnHeader colNombre;
        private System.Windows.Forms.ColumnHeader colOrigen;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Button btnActualizarPermisos;
    }
}