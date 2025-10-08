partial class frmAlta
{
    private System.ComponentModel.IContainer components = null;

    // CONTROLES EXISTENTES
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txtNombre;
    private System.Windows.Forms.TextBox txtUsuario;
    private System.Windows.Forms.TextBox txtClave;
    private System.Windows.Forms.Button btnAceptar;
    private System.Windows.Forms.Button btnCancelar;

    // NUEVOS CONTROLES
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabDatosBasicos;
    private System.Windows.Forms.TabPage tabFamilias;
    private System.Windows.Forms.TabPage tabPatentes;
    private System.Windows.Forms.TreeView treeViewFamilias;
    private System.Windows.Forms.CheckedListBox checkedListBoxPatentes;

    private void InitializeComponent()
    {
        this.label1 = new System.Windows.Forms.Label();
        this.label2 = new System.Windows.Forms.Label();
        this.label3 = new System.Windows.Forms.Label();
        this.txtNombre = new System.Windows.Forms.TextBox();
        this.txtUsuario = new System.Windows.Forms.TextBox();
        this.txtClave = new System.Windows.Forms.TextBox();
        this.btnAceptar = new System.Windows.Forms.Button();
        this.btnCancelar = new System.Windows.Forms.Button();

        // Nuevos controles
        this.tabControl1 = new System.Windows.Forms.TabControl();
        this.tabDatosBasicos = new System.Windows.Forms.TabPage();
        this.tabFamilias = new System.Windows.Forms.TabPage();
        this.tabPatentes = new System.Windows.Forms.TabPage();
        this.treeViewFamilias = new System.Windows.Forms.TreeView();
        this.checkedListBoxPatentes = new System.Windows.Forms.CheckedListBox();

        this.tabControl1.SuspendLayout();
        this.tabDatosBasicos.SuspendLayout();
        this.tabFamilias.SuspendLayout();
        this.tabPatentes.SuspendLayout();
        this.SuspendLayout();

        // 
        // tabControl1
        // 
        this.tabControl1.Controls.Add(this.tabDatosBasicos);
        this.tabControl1.Controls.Add(this.tabFamilias);
        this.tabControl1.Controls.Add(this.tabPatentes);
        this.tabControl1.Location = new System.Drawing.Point(12, 12);
        this.tabControl1.Name = "tabControl1";
        this.tabControl1.SelectedIndex = 0;
        this.tabControl1.Size = new System.Drawing.Size(560, 380);
        this.tabControl1.TabIndex = 0;

        // 
        // tabDatosBasicos
        // 
        this.tabDatosBasicos.Controls.Add(this.label1);
        this.tabDatosBasicos.Controls.Add(this.label2);
        this.tabDatosBasicos.Controls.Add(this.label3);
        this.tabDatosBasicos.Controls.Add(this.txtNombre);
        this.tabDatosBasicos.Controls.Add(this.txtUsuario);
        this.tabDatosBasicos.Controls.Add(this.txtClave);
        this.tabDatosBasicos.Location = new System.Drawing.Point(4, 25);
        this.tabDatosBasicos.Name = "tabDatosBasicos";
        this.tabDatosBasicos.Padding = new System.Windows.Forms.Padding(3);
        this.tabDatosBasicos.Size = new System.Drawing.Size(552, 351);
        this.tabDatosBasicos.TabIndex = 0;
        this.tabDatosBasicos.Text = "Datos Básicos";
        this.tabDatosBasicos.UseVisualStyleBackColor = true;

        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(20, 30);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(56, 16);
        this.label1.TabIndex = 0;
        this.label1.Text = "Nombre";

        // 
        // txtNombre
        // 
        this.txtNombre.Location = new System.Drawing.Point(120, 27);
        this.txtNombre.Name = "txtNombre";
        this.txtNombre.Size = new System.Drawing.Size(400, 22);
        this.txtNombre.TabIndex = 1;

        // 
        // label2
        // 
        this.label2.AutoSize = true;
        this.label2.Location = new System.Drawing.Point(20, 80);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(54, 16);
        this.label2.TabIndex = 2;
        this.label2.Text = "Usuario";

        // 
        // txtUsuario
        // 
        this.txtUsuario.Location = new System.Drawing.Point(120, 77);
        this.txtUsuario.Name = "txtUsuario";
        this.txtUsuario.Size = new System.Drawing.Size(400, 22);
        this.txtUsuario.TabIndex = 3;

        // 
        // label3
        // 
        this.label3.AutoSize = true;
        this.label3.Location = new System.Drawing.Point(20, 130);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(70, 16);
        this.label3.TabIndex = 4;
        this.label3.Text = "Contraseña";

        // 
        // txtClave
        // 
        this.txtClave.Location = new System.Drawing.Point(120, 127);
        this.txtClave.Name = "txtClave";
        this.txtClave.Size = new System.Drawing.Size(400, 22);
        this.txtClave.TabIndex = 5;
        this.txtClave.UseSystemPasswordChar = true;

        // 
        // tabFamilias
        // 
        this.tabFamilias.Controls.Add(this.treeViewFamilias);
        this.tabFamilias.Location = new System.Drawing.Point(4, 25);
        this.tabFamilias.Name = "tabFamilias";
        this.tabFamilias.Padding = new System.Windows.Forms.Padding(3);
        this.tabFamilias.Size = new System.Drawing.Size(552, 351);
        this.tabFamilias.TabIndex = 1;
        this.tabFamilias.Text = "Familias";
        this.tabFamilias.UseVisualStyleBackColor = true;

        // 
        // treeViewFamilias
        // 
        this.treeViewFamilias.CheckBoxes = true;
        this.treeViewFamilias.Dock = System.Windows.Forms.DockStyle.Fill;
        this.treeViewFamilias.Location = new System.Drawing.Point(3, 3);
        this.treeViewFamilias.Name = "treeViewFamilias";
        this.treeViewFamilias.Size = new System.Drawing.Size(546, 345);
        this.treeViewFamilias.TabIndex = 0;

        // 
        // tabPatentes
        // 
        this.tabPatentes.Controls.Add(this.checkedListBoxPatentes);
        this.tabPatentes.Location = new System.Drawing.Point(4, 25);
        this.tabPatentes.Name = "tabPatentes";
        this.tabPatentes.Size = new System.Drawing.Size(552, 351);
        this.tabPatentes.TabIndex = 2;
        this.tabPatentes.Text = "Patentes";
        this.tabPatentes.UseVisualStyleBackColor = true;

        // 
        // checkedListBoxPatentes
        // 
        this.checkedListBoxPatentes.Dock = System.Windows.Forms.DockStyle.Fill;
        this.checkedListBoxPatentes.FormattingEnabled = true;
        this.checkedListBoxPatentes.Location = new System.Drawing.Point(0, 0);
        this.checkedListBoxPatentes.Name = "checkedListBoxPatentes";
        this.checkedListBoxPatentes.Size = new System.Drawing.Size(552, 351);
        this.checkedListBoxPatentes.TabIndex = 0;

        // 
        // btnAceptar
        // 
        this.btnAceptar.Location = new System.Drawing.Point(416, 405);
        this.btnAceptar.Name = "btnAceptar";
        this.btnAceptar.Size = new System.Drawing.Size(75, 30);
        this.btnAceptar.TabIndex = 1;
        this.btnAceptar.Text = "Aceptar";
        this.btnAceptar.UseVisualStyleBackColor = true;
        this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);

        // 
        // btnCancelar
        // 
        this.btnCancelar.Location = new System.Drawing.Point(497, 405);
        this.btnCancelar.Name = "btnCancelar";
        this.btnCancelar.Size = new System.Drawing.Size(75, 30);
        this.btnCancelar.TabIndex = 2;
        this.btnCancelar.Text = "Cancelar";
        this.btnCancelar.UseVisualStyleBackColor = true;
        this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

        // 
        // frmAlta
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(584, 450);
        this.Controls.Add(this.btnCancelar);
        this.Controls.Add(this.btnAceptar);
        this.Controls.Add(this.tabControl1);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "frmAlta";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "Gestión de Usuario";
        this.Load += new System.EventHandler(this.frmAlta_Load);

        this.tabControl1.ResumeLayout(false);
        this.tabDatosBasicos.ResumeLayout(false);
        this.tabDatosBasicos.PerformLayout();
        this.tabFamilias.ResumeLayout(false);
        this.tabPatentes.ResumeLayout(false);
        this.ResumeLayout(false);
    }
}