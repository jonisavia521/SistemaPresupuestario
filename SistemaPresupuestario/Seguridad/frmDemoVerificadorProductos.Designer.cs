namespace SistemaPresupuestario.Seguridad
{
    partial class frmDemoVerificadorProductos
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
            this.dgvProductos = new System.Windows.Forms.DataGridView();
            this.colCodigo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPorcentajeIVA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDigitoH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblSumaControlVertical = new System.Windows.Forms.Label();
            this.lblEstadoGeneral = new System.Windows.Forms.Label();
            this.groupBoxControles = new System.Windows.Forms.GroupBox();
            this.btnCargarDatos = new System.Windows.Forms.Button();
            this.btnCalcularDigitos = new System.Windows.Forms.Button();
            this.btnSimularCorrupcion = new System.Windows.Forms.Button();
            this.btnVerificarIntegridad = new System.Windows.Forms.Button();
            this.panelSuperior = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.panelInferior = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).BeginInit();
            this.groupBoxControles.SuspendLayout();
            this.panelSuperior.SuspendLayout();
            this.panelInferior.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvProductos
            // 
            this.dgvProductos.AllowUserToAddRows = false;
            this.dgvProductos.AllowUserToDeleteRows = false;
            this.dgvProductos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvProductos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProductos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProductos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCodigo,
            this.colDescripcion,
            this.colPorcentajeIVA,
            this.colDigitoH});
            this.dgvProductos.Location = new System.Drawing.Point(12, 80);
            this.dgvProductos.Name = "dgvProductos";
            this.dgvProductos.RowHeadersWidth = 51;
            this.dgvProductos.RowTemplate.Height = 24;
            this.dgvProductos.Size = new System.Drawing.Size(776, 250);
            this.dgvProductos.TabIndex = 0;
            // 
            // colCodigo
            // 
            this.colCodigo.DataPropertyName = "Codigo";
            this.colCodigo.HeaderText = "Código";
            this.colCodigo.MinimumWidth = 6;
            this.colCodigo.Name = "colCodigo";
            // 
            // colDescripcion
            // 
            this.colDescripcion.DataPropertyName = "Descripcion";
            this.colDescripcion.HeaderText = "Descripción";
            this.colDescripcion.MinimumWidth = 6;
            this.colDescripcion.Name = "colDescripcion";
            // 
            // colPorcentajeIVA
            // 
            this.colPorcentajeIVA.DataPropertyName = "PorcentajeIVA";
            this.colPorcentajeIVA.HeaderText = "IVA %";
            this.colPorcentajeIVA.MinimumWidth = 6;
            this.colPorcentajeIVA.Name = "colPorcentajeIVA";
            // 
            // colDigitoH
            // 
            this.colDigitoH.HeaderText = "Dígito H";
            this.colDigitoH.MinimumWidth = 6;
            this.colDigitoH.Name = "colDigitoH";
            this.colDigitoH.ReadOnly = true;
            // 
            // lblSumaControlVertical
            // 
            this.lblSumaControlVertical.AutoSize = true;
            this.lblSumaControlVertical.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSumaControlVertical.Location = new System.Drawing.Point(12, 12);
            this.lblSumaControlVertical.Name = "lblSumaControlVertical";
            this.lblSumaControlVertical.Size = new System.Drawing.Size(235, 20);
            this.lblSumaControlVertical.TabIndex = 1;
            this.lblSumaControlVertical.Text = "Suma de Control (Lote): N/A";
            // 
            // lblEstadoGeneral
            // 
            this.lblEstadoGeneral.AutoSize = true;
            this.lblEstadoGeneral.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEstadoGeneral.Location = new System.Drawing.Point(12, 42);
            this.lblEstadoGeneral.Name = "lblEstadoGeneral";
            this.lblEstadoGeneral.Size = new System.Drawing.Size(232, 25);
            this.lblEstadoGeneral.TabIndex = 2;
            this.lblEstadoGeneral.Text = "Listo para cargar datos.";
            // 
            // groupBoxControles
            // 
            this.groupBoxControles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxControles.Controls.Add(this.btnCargarDatos);
            this.groupBoxControles.Controls.Add(this.btnCalcularDigitos);
            this.groupBoxControles.Controls.Add(this.btnSimularCorrupcion);
            this.groupBoxControles.Controls.Add(this.btnVerificarIntegridad);
            this.groupBoxControles.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxControles.Location = new System.Drawing.Point(12, 336);
            this.groupBoxControles.Name = "groupBoxControles";
            this.groupBoxControles.Size = new System.Drawing.Size(776, 102);
            this.groupBoxControles.TabIndex = 3;
            this.groupBoxControles.TabStop = false;
            this.groupBoxControles.Text = "Controles de la Demo";
            // 
            // btnCargarDatos
            // 
            this.btnCargarDatos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCargarDatos.Location = new System.Drawing.Point(20, 32);
            this.btnCargarDatos.Name = "btnCargarDatos";
            this.btnCargarDatos.Size = new System.Drawing.Size(160, 50);
            this.btnCargarDatos.TabIndex = 0;
            this.btnCargarDatos.Text = "1. Cargar Datos";
            this.btnCargarDatos.UseVisualStyleBackColor = true;
            this.btnCargarDatos.Click += new System.EventHandler(this.btnCargarDatos_Click);
            // 
            // btnCalcularDigitos
            // 
            this.btnCalcularDigitos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCalcularDigitos.Location = new System.Drawing.Point(200, 32);
            this.btnCalcularDigitos.Name = "btnCalcularDigitos";
            this.btnCalcularDigitos.Size = new System.Drawing.Size(160, 50);
            this.btnCalcularDigitos.TabIndex = 1;
            this.btnCalcularDigitos.Text = "2. Calcular Dígitos";
            this.btnCalcularDigitos.UseVisualStyleBackColor = true;
            this.btnCalcularDigitos.Click += new System.EventHandler(this.btnCalcularDigitos_Click);
            // 
            // btnSimularCorrupcion
            // 
            this.btnSimularCorrupcion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSimularCorrupcion.Location = new System.Drawing.Point(380, 32);
            this.btnSimularCorrupcion.Name = "btnSimularCorrupcion";
            this.btnSimularCorrupcion.Size = new System.Drawing.Size(160, 50);
            this.btnSimularCorrupcion.TabIndex = 2;
            this.btnSimularCorrupcion.Text = "3. Simular Corrupción";
            this.btnSimularCorrupcion.UseVisualStyleBackColor = true;
            this.btnSimularCorrupcion.Click += new System.EventHandler(this.btnSimularCorrupcion_Click);
            // 
            // btnVerificarIntegridad
            // 
            this.btnVerificarIntegridad.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerificarIntegridad.Location = new System.Drawing.Point(560, 32);
            this.btnVerificarIntegridad.Name = "btnVerificarIntegridad";
            this.btnVerificarIntegridad.Size = new System.Drawing.Size(160, 50);
            this.btnVerificarIntegridad.TabIndex = 3;
            this.btnVerificarIntegridad.Text = "4. Verificar Integridad";
            this.btnVerificarIntegridad.UseVisualStyleBackColor = true;
            this.btnVerificarIntegridad.Click += new System.EventHandler(this.btnVerificarIntegridad_Click);
            // 
            // panelSuperior
            // 
            this.panelSuperior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.panelSuperior.Controls.Add(this.lblTitulo);
            this.panelSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSuperior.Location = new System.Drawing.Point(0, 0);
            this.panelSuperior.Name = "panelSuperior";
            this.panelSuperior.Size = new System.Drawing.Size(800, 60);
            this.panelSuperior.TabIndex = 4;
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(12, 14);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(536, 32);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Demostración: Dígitos Verificadores";
            // 
            // panelInferior
            // 
            this.panelInferior.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelInferior.Controls.Add(this.lblSumaControlVertical);
            this.panelInferior.Controls.Add(this.lblEstadoGeneral);
            this.panelInferior.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelInferior.Location = new System.Drawing.Point(0, 450);
            this.panelInferior.Name = "panelInferior";
            this.panelInferior.Size = new System.Drawing.Size(800, 80);
            this.panelInferior.TabIndex = 5;
            // 
            // frmDemoVerificadorProductos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 530);
            this.Controls.Add(this.panelInferior);
            this.Controls.Add(this.panelSuperior);
            this.Controls.Add(this.groupBoxControles);
            this.Controls.Add(this.dgvProductos);
            this.MinimumSize = new System.Drawing.Size(818, 577);
            this.Name = "frmDemoVerificadorProductos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Demostración: Dígitos Verificadores de Productos";
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).EndInit();
            this.groupBoxControles.ResumeLayout(false);
            this.panelSuperior.ResumeLayout(false);
            this.panelSuperior.PerformLayout();
            this.panelInferior.ResumeLayout(false);
            this.panelInferior.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvProductos;
        private System.Windows.Forms.Label lblSumaControlVertical;
        private System.Windows.Forms.Label lblEstadoGeneral;
        private System.Windows.Forms.GroupBox groupBoxControles;
        private System.Windows.Forms.Button btnCargarDatos;
        private System.Windows.Forms.Button btnCalcularDigitos;
        private System.Windows.Forms.Button btnSimularCorrupcion;
        private System.Windows.Forms.Button btnVerificarIntegridad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCodigo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPorcentajeIVA;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDigitoH;
        private System.Windows.Forms.Panel panelSuperior;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel panelInferior;
    }
}
