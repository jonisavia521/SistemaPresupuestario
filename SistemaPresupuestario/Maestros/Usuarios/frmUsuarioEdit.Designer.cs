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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabDatosBasicos = new System.Windows.Forms.TabPage();
            this.chkForzarCambioClave = new System.Windows.Forms.CheckBox();
            this.txtConfirmarClave = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtClave = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPermisos = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tvFamilias = new System.Windows.Forms.TreeView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.label5 = new System.Windows.Forms.Label();
            this.lstPatentesDisponibles = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnAsignarPatente = new System.Windows.Forms.Button();
            this.btnQuitarPatente = new System.Windows.Forms.Button();
            this.lstPatentesAsignadas = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tabPermisosEfectivos = new System.Windows.Forms.TabPage();
            this.dgvPermisosEfectivos = new System.Windows.Forms.DataGridView();
            this.Tipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NombrePermiso = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Origen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DetalleOrigen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.lblRequisitos = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabDatosBasicos.SuspendLayout();
            this.tabPermisos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPermisosEfectivos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPermisosEfectivos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabDatosBasicos);
            this.tabControl1.Controls.Add(this.tabPermisos);
            this.tabControl1.Controls.Add(this.tabPermisosEfectivos);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(776, 397);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabDatosBasicos
            // 
            this.tabDatosBasicos.Controls.Add(this.lblRequisitos);
            this.tabDatosBasicos.Controls.Add(this.chkForzarCambioClave);
            this.tabDatosBasicos.Controls.Add(this.txtConfirmarClave);
            this.tabDatosBasicos.Controls.Add(this.label4);
            this.tabDatosBasicos.Controls.Add(this.txtClave);
            this.tabDatosBasicos.Controls.Add(this.label3);
            this.tabDatosBasicos.Controls.Add(this.txtNombre);
            this.tabDatosBasicos.Controls.Add(this.label2);
            this.tabDatosBasicos.Controls.Add(this.txtUsuario);
            this.tabDatosBasicos.Controls.Add(this.label1);
            this.tabDatosBasicos.Location = new System.Drawing.Point(4, 25);
            this.tabDatosBasicos.Name = "tabDatosBasicos";
            this.tabDatosBasicos.Padding = new System.Windows.Forms.Padding(3);
            this.tabDatosBasicos.Size = new System.Drawing.Size(768, 368);
            this.tabDatosBasicos.TabIndex = 0;
            this.tabDatosBasicos.Text = "Datos Básicos";
            this.tabDatosBasicos.UseVisualStyleBackColor = true;
            // 
            // chkForzarCambioClave
            // 
            this.chkForzarCambioClave.AutoSize = true;
            this.chkForzarCambioClave.Location = new System.Drawing.Point(150, 180);
            this.chkForzarCambioClave.Name = "chkForzarCambioClave";
            this.chkForzarCambioClave.Size = new System.Drawing.Size(206, 20);
            this.chkForzarCambioClave.TabIndex = 8;
            this.chkForzarCambioClave.Text = "Forzar cambio en próximo login";
            this.chkForzarCambioClave.UseVisualStyleBackColor = true;
            // 
            // txtConfirmarClave
            // 
            this.txtConfirmarClave.Location = new System.Drawing.Point(150, 152);
            this.txtConfirmarClave.Name = "txtConfirmarClave";
            this.txtConfirmarClave.PasswordChar = '*';
            this.txtConfirmarClave.Size = new System.Drawing.Size(250, 22);
            this.txtConfirmarClave.TabIndex = 7;
            this.txtConfirmarClave.Validating += new System.ComponentModel.CancelEventHandler(this.txtConfirmarClave_Validating);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 155);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(115, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Confirmar Clave:";
            // 
            // txtClave
            // 
            this.txtClave.Location = new System.Drawing.Point(150, 124);
            this.txtClave.Name = "txtClave";
            this.txtClave.PasswordChar = '*';
            this.txtClave.Size = new System.Drawing.Size(250, 22);
            this.txtClave.TabIndex = 5;
            this.txtClave.TextChanged += new System.EventHandler(this.txtClave_TextChanged);
            this.txtClave.Validating += new System.ComponentModel.CancelEventHandler(this.txtClave_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 127);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Nueva Clave:";
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(150, 68);
            this.txtNombre.MaxLength = 1000;
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(400, 22);
            this.txtNombre.TabIndex = 3;
            this.txtNombre.Validating += new System.ComponentModel.CancelEventHandler(this.txtNombre_Validating);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Nombre:";
            // 
            // txtUsuario
            // 
            this.txtUsuario.Location = new System.Drawing.Point(150, 40);
            this.txtUsuario.MaxLength = 20;
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(200, 22);
            this.txtUsuario.TabIndex = 1;
            this.txtUsuario.Validating += new System.ComponentModel.CancelEventHandler(this.txtUsuario_Validating);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Usuario:";
            // 
            // tabPermisos
            // 
            this.tabPermisos.Controls.Add(this.splitContainer1);
            this.tabPermisos.Location = new System.Drawing.Point(4, 25);
            this.tabPermisos.Name = "tabPermisos";
            this.tabPermisos.Padding = new System.Windows.Forms.Padding(3);
            this.tabPermisos.Size = new System.Drawing.Size(768, 368);
            this.tabPermisos.TabIndex = 1;
            this.tabPermisos.Text = "Asignación de Permisos";
            this.tabPermisos.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Size = new System.Drawing.Size(762, 362);
            this.splitContainer1.SplitterDistance = 181;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tvFamilias);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(762, 181);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Familias de Permisos (marque las que desea asignar)";
            // 
            // tvFamilias
            // 
            this.tvFamilias.CheckBoxes = true;
            this.tvFamilias.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFamilias.Location = new System.Drawing.Point(3, 18);
            this.tvFamilias.Name = "tvFamilias";
            this.tvFamilias.Size = new System.Drawing.Size(756, 160);
            this.tvFamilias.TabIndex = 0;
            this.tvFamilias.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvFamilias_AfterCheck);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.splitContainer2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(762, 177);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Patentes Individuales";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 18);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.label5);
            this.splitContainer2.Panel1.Controls.Add(this.lstPatentesDisponibles);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lstPatentesAsignadas);
            this.splitContainer2.Panel2.Controls.Add(this.label6);
            this.splitContainer2.Panel2.Controls.Add(this.panel1);
            this.splitContainer2.Size = new System.Drawing.Size(756, 156);
            this.splitContainer2.SplitterDistance = 350;
            this.splitContainer2.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(133, 16);
            this.label5.TabIndex = 1;
            this.label5.Text = "Patentes Disponibles";
            // 
            // lstPatentesDisponibles
            // 
            this.lstPatentesDisponibles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstPatentesDisponibles.DisplayMember = "Nombre";
            this.lstPatentesDisponibles.Location = new System.Drawing.Point(3, 24);
            this.lstPatentesDisponibles.Name = "lstPatentesDisponibles";
            this.lstPatentesDisponibles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstPatentesDisponibles.Size = new System.Drawing.Size(344, 121);
            this.lstPatentesDisponibles.TabIndex = 0;
            this.lstPatentesDisponibles.ValueMember = "Id";
            this.lstPatentesDisponibles.DoubleClick += new System.EventHandler(this.lstPatentesDisponibles_DoubleClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnAsignarPatente);
            this.panel1.Controls.Add(this.btnQuitarPatente);
            this.panel1.Location = new System.Drawing.Point(3, 60);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(75, 60);
            this.panel1.TabIndex = 0;
            // 
            // btnAsignarPatente
            // 
            this.btnAsignarPatente.Location = new System.Drawing.Point(0, 3);
            this.btnAsignarPatente.Name = "btnAsignarPatente";
            this.btnAsignarPatente.Size = new System.Drawing.Size(75, 23);
            this.btnAsignarPatente.TabIndex = 0;
            this.btnAsignarPatente.Text = "Asignar >";
            this.btnAsignarPatente.UseVisualStyleBackColor = true;
            this.btnAsignarPatente.Click += new System.EventHandler(this.btnAsignarPatente_Click);
            // 
            // btnQuitarPatente
            // 
            this.btnQuitarPatente.Location = new System.Drawing.Point(0, 32);
            this.btnQuitarPatente.Name = "btnQuitarPatente";
            this.btnQuitarPatente.Size = new System.Drawing.Size(75, 23);
            this.btnQuitarPatente.TabIndex = 1;
            this.btnQuitarPatente.Text = "< Quitar";
            this.btnQuitarPatente.UseVisualStyleBackColor = true;
            this.btnQuitarPatente.Click += new System.EventHandler(this.btnQuitarPatente_Click);
            // 
            // lstPatentesAsignadas
            // 
            this.lstPatentesAsignadas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstPatentesAsignadas.DisplayMember = "Nombre";
            this.lstPatentesAsignadas.Location = new System.Drawing.Point(84, 24);
            this.lstPatentesAsignadas.Name = "lstPatentesAsignadas";
            this.lstPatentesAsignadas.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstPatentesAsignadas.Size = new System.Drawing.Size(315, 121);
            this.lstPatentesAsignadas.TabIndex = 2;
            this.lstPatentesAsignadas.ValueMember = "Id";
            this.lstPatentesAsignadas.DoubleClick += new System.EventHandler(this.lstPatentesAsignadas_DoubleClick);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(84, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(127, 16);
            this.label6.TabIndex = 3;
            this.label6.Text = "Patentes Asignadas";
            // 
            // tabPermisosEfectivos
            // 
            this.tabPermisosEfectivos.Controls.Add(this.dgvPermisosEfectivos);
            this.tabPermisosEfectivos.Location = new System.Drawing.Point(4, 25);
            this.tabPermisosEfectivos.Name = "tabPermisosEfectivos";
            this.tabPermisosEfectivos.Size = new System.Drawing.Size(768, 368);
            this.tabPermisosEfectivos.TabIndex = 2;
            this.tabPermisosEfectivos.Text = "Permisos Efectivos";
            this.tabPermisosEfectivos.UseVisualStyleBackColor = true;
            // 
            // dgvPermisosEfectivos
            // 
            this.dgvPermisosEfectivos.AllowUserToAddRows = false;
            this.dgvPermisosEfectivos.AllowUserToDeleteRows = false;
            this.dgvPermisosEfectivos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPermisosEfectivos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Tipo,
            this.NombrePermiso,
            this.Origen,
            this.DetalleOrigen});
            this.dgvPermisosEfectivos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPermisosEfectivos.Location = new System.Drawing.Point(0, 0);
            this.dgvPermisosEfectivos.Name = "dgvPermisosEfectivos";
            this.dgvPermisosEfectivos.ReadOnly = true;
            this.dgvPermisosEfectivos.RowHeadersWidth = 51;
            this.dgvPermisosEfectivos.RowTemplate.Height = 24;
            this.dgvPermisosEfectivos.Size = new System.Drawing.Size(768, 368);
            this.dgvPermisosEfectivos.TabIndex = 0;
            // 
            // Tipo
            // 
            this.Tipo.DataPropertyName = "TipoTexto";
            this.Tipo.HeaderText = "Tipo";
            this.Tipo.MinimumWidth = 6;
            this.Tipo.Name = "Tipo";
            this.Tipo.ReadOnly = true;
            this.Tipo.Width = 80;
            // 
            // NombrePermiso
            // 
            this.NombrePermiso.DataPropertyName = "Nombre";
            this.NombrePermiso.HeaderText = "Permiso";
            this.NombrePermiso.MinimumWidth = 6;
            this.NombrePermiso.Name = "NombrePermiso";
            this.NombrePermiso.ReadOnly = true;
            this.NombrePermiso.Width = 200;
            // 
            // Origen
            // 
            this.Origen.DataPropertyName = "OrigenTexto";
            this.Origen.HeaderText = "Origen";
            this.Origen.MinimumWidth = 6;
            this.Origen.Name = "Origen";
            this.Origen.ReadOnly = true;
            this.Origen.Width = 100;
            // 
            // DetalleOrigen
            // 
            this.DetalleOrigen.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DetalleOrigen.DataPropertyName = "DetalleOrigen";
            this.DetalleOrigen.HeaderText = "Detalle";
            this.DetalleOrigen.MinimumWidth = 6;
            this.DetalleOrigen.Name = "DetalleOrigen";
            this.DetalleOrigen.ReadOnly = true;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.Location = new System.Drawing.Point(632, 415);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(75, 23);
            this.btnGuardar.TabIndex = 1;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(713, 415);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 23);
            this.btnCancelar.TabIndex = 2;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // lblRequisitos
            // 
            this.lblRequisitos.AutoSize = true;
            this.lblRequisitos.ForeColor = System.Drawing.Color.Gray;
            this.lblRequisitos.Location = new System.Drawing.Point(147, 102);
            this.lblRequisitos.Name = "lblRequisitos";
            this.lblRequisitos.Size = new System.Drawing.Size(180, 16);
            this.lblRequisitos.TabIndex = 9;
            this.lblRequisitos.Text = "Requisitos de contraseña...";
            // 
            // frmUsuarioEdit
            // 
            this.AcceptButton = this.btnGuardar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(816, 489);
            this.Name = "frmUsuarioEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Editar Usuario";
            this.Load += new System.EventHandler(this.frmUsuarioEdit_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabDatosBasicos.ResumeLayout(false);
            this.tabDatosBasicos.PerformLayout();
            this.tabPermisos.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabPermisosEfectivos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPermisosEfectivos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabDatosBasicos;
        private System.Windows.Forms.TabPage tabPermisos;
        private System.Windows.Forms.TabPage tabPermisosEfectivos;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtClave;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtConfirmarClave;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkForzarCambioClave;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TreeView tvFamilias;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListBox lstPatentesDisponibles;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnAsignarPatente;
        private System.Windows.Forms.Button btnQuitarPatente;
        private System.Windows.Forms.ListBox lstPatentesAsignadas;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridView dgvPermisosEfectivos;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn NombrePermiso;
        private System.Windows.Forms.DataGridViewTextBoxColumn Origen;
        private System.Windows.Forms.DataGridViewTextBoxColumn DetalleOrigen;
        private System.Windows.Forms.Label lblRequisitos;
    }
}