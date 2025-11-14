namespace SistemaPresupuestario.Venta.Factura
{
    partial class frmFacturar
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.dgvPresupuestos = new System.Windows.Forms.DataGridView();
            this.panelInfo = new System.Windows.Forms.Panel();
            this.lblPresupuestosSeleccionados = new System.Windows.Forms.Label();
            this.lblTotalSeleccionado = new System.Windows.Forms.Label();
            this.lblClienteSeleccionado = new System.Windows.Forms.Label();
            this.lblTotalPresupuestos = new System.Windows.Forms.Label();
            this.lblSubtotal = new System.Windows.Forms.Label();
            this.lblIVA = new System.Windows.Forms.Label();
            this.lblIIBBArba = new System.Windows.Forms.Label();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.btnObtenerCAE = new System.Windows.Forms.Button();
            this.panelHeader.SuspendLayout();
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPresupuestos)).BeginInit();
            this.panelInfo.SuspendLayout();
            this.panelFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.panelHeader.Controls.Add(this.lblTitulo);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1000, 70);
            this.panelHeader.TabIndex = 0;
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(12, 15);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(392, 37);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Facturación de Presupuestos";
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.dgvPresupuestos);
            this.panelMain.Controls.Add(this.panelInfo);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 70);
            this.panelMain.Name = "panelMain";
            this.panelMain.Padding = new System.Windows.Forms.Padding(10);
            this.panelMain.Size = new System.Drawing.Size(1000, 510);
            this.panelMain.TabIndex = 1;
            // 
            // dgvPresupuestos
            // 
            this.dgvPresupuestos.AllowUserToAddRows = false;
            this.dgvPresupuestos.AllowUserToDeleteRows = false;
            this.dgvPresupuestos.BackgroundColor = System.Drawing.Color.White;
            this.dgvPresupuestos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPresupuestos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPresupuestos.ColumnHeadersHeight = 30;
            this.dgvPresupuestos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPresupuestos.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPresupuestos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPresupuestos.EnableHeadersVisualStyles = false;
            this.dgvPresupuestos.Location = new System.Drawing.Point(10, 10);
            this.dgvPresupuestos.MultiSelect = false;
            this.dgvPresupuestos.Name = "dgvPresupuestos";
            this.dgvPresupuestos.RowHeadersVisible = false;
            this.dgvPresupuestos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPresupuestos.Size = new System.Drawing.Size(980, 390);
            this.dgvPresupuestos.TabIndex = 0;
            // 
            // panelInfo
            // 
            this.panelInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.panelInfo.Controls.Add(this.lblIIBBArba);
            this.panelInfo.Controls.Add(this.lblIVA);
            this.panelInfo.Controls.Add(this.lblSubtotal);
            this.panelInfo.Controls.Add(this.lblPresupuestosSeleccionados);
            this.panelInfo.Controls.Add(this.lblTotalSeleccionado);
            this.panelInfo.Controls.Add(this.lblClienteSeleccionado);
            this.panelInfo.Controls.Add(this.lblTotalPresupuestos);
            this.panelInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelInfo.Location = new System.Drawing.Point(10, 400);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Padding = new System.Windows.Forms.Padding(10);
            this.panelInfo.Size = new System.Drawing.Size(980, 100);
            this.panelInfo.TabIndex = 1;
            // 
            // lblPresupuestosSeleccionados
            // 
            this.lblPresupuestosSeleccionados.AutoSize = true;
            this.lblPresupuestosSeleccionados.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPresupuestosSeleccionados.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.lblPresupuestosSeleccionados.Location = new System.Drawing.Point(13, 40);
            this.lblPresupuestosSeleccionados.Name = "lblPresupuestosSeleccionados";
            this.lblPresupuestosSeleccionados.Size = new System.Drawing.Size(124, 19);
            this.lblPresupuestosSeleccionados.TabIndex = 1;
            this.lblPresupuestosSeleccionados.Text = "Seleccionados: 0";
            // 
            // lblTotalSeleccionado
            // 
            this.lblTotalSeleccionado.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalSeleccionado.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotalSeleccionado.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.lblTotalSeleccionado.Location = new System.Drawing.Point(700, 13);
            this.lblTotalSeleccionado.Name = "lblTotalSeleccionado";
            this.lblTotalSeleccionado.Size = new System.Drawing.Size(267, 25);
            this.lblTotalSeleccionado.TabIndex = 3;
            this.lblTotalSeleccionado.Text = "Total: $0.00";
            this.lblTotalSeleccionado.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblClienteSeleccionado
            // 
            this.lblClienteSeleccionado.AutoSize = true;
            this.lblClienteSeleccionado.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblClienteSeleccionado.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.lblClienteSeleccionado.Location = new System.Drawing.Point(13, 68);
            this.lblClienteSeleccionado.Name = "lblClienteSeleccionado";
            this.lblClienteSeleccionado.Size = new System.Drawing.Size(129, 19);
            this.lblClienteSeleccionado.TabIndex = 2;
            this.lblClienteSeleccionado.Text = "Cliente: (ninguno)";
            // 
            // lblTotalPresupuestos
            // 
            this.lblTotalPresupuestos.AutoSize = true;
            this.lblTotalPresupuestos.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotalPresupuestos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.lblTotalPresupuestos.Location = new System.Drawing.Point(13, 13);
            this.lblTotalPresupuestos.Name = "lblTotalPresupuestos";
            this.lblTotalPresupuestos.Size = new System.Drawing.Size(152, 19);
            this.lblTotalPresupuestos.TabIndex = 0;
            this.lblTotalPresupuestos.Text = "Total presupuestos: 0";
            // 
            // lblSubtotal
            // 
            this.lblSubtotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSubtotal.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSubtotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.lblSubtotal.Location = new System.Drawing.Point(700, 42);
            this.lblSubtotal.Name = "lblSubtotal";
            this.lblSubtotal.Size = new System.Drawing.Size(267, 19);
            this.lblSubtotal.TabIndex = 4;
            this.lblSubtotal.Text = "Subtotal (Neto): $0.00";
            this.lblSubtotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblIVA
            // 
            this.lblIVA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIVA.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblIVA.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.lblIVA.Location = new System.Drawing.Point(700, 61);
            this.lblIVA.Name = "lblIVA";
            this.lblIVA.Size = new System.Drawing.Size(267, 19);
            this.lblIVA.TabIndex = 5;
            this.lblIVA.Text = "IVA: $0.00";
            this.lblIVA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblIIBBArba
            // 
            this.lblIIBBArba.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIIBBArba.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblIIBBArba.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.lblIIBBArba.Location = new System.Drawing.Point(700, 80);
            this.lblIIBBArba.Name = "lblIIBBArba";
            this.lblIIBBArba.Size = new System.Drawing.Size(267, 19);
            this.lblIIBBArba.TabIndex = 6;
            this.lblIIBBArba.Text = "IIBB ARBA: $0.00";
            this.lblIIBBArba.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelFooter
            // 
            this.panelFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.panelFooter.Controls.Add(this.btnCerrar);
            this.panelFooter.Controls.Add(this.btnActualizar);
            this.panelFooter.Controls.Add(this.btnObtenerCAE);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(0, 580);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Padding = new System.Windows.Forms.Padding(10);
            this.panelFooter.Size = new System.Drawing.Size(1000, 70);
            this.panelFooter.TabIndex = 2;
            // 
            // btnCerrar
            // 
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.btnCerrar.FlatAppearance.BorderSize = 0;
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Location = new System.Drawing.Point(875, 13);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(113, 44);
            this.btnCerrar.TabIndex = 2;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = false;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // btnActualizar
            // 
            this.btnActualizar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnActualizar.FlatAppearance.BorderSize = 0;
            this.btnActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActualizar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnActualizar.ForeColor = System.Drawing.Color.White;
            this.btnActualizar.Location = new System.Drawing.Point(13, 13);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(120, 44);
            this.btnActualizar.TabIndex = 0;
            this.btnActualizar.Text = "Actualizar";
            this.btnActualizar.UseVisualStyleBackColor = false;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            // 
            // btnObtenerCAE
            // 
            this.btnObtenerCAE.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnObtenerCAE.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.btnObtenerCAE.Enabled = false;
            this.btnObtenerCAE.FlatAppearance.BorderSize = 0;
            this.btnObtenerCAE.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnObtenerCAE.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnObtenerCAE.ForeColor = System.Drawing.Color.White;
            this.btnObtenerCAE.Location = new System.Drawing.Point(405, 10);
            this.btnObtenerCAE.Name = "btnObtenerCAE";
            this.btnObtenerCAE.Size = new System.Drawing.Size(190, 50);
            this.btnObtenerCAE.TabIndex = 1;
            this.btnObtenerCAE.Text = "Obtener CAE";
            this.btnObtenerCAE.UseVisualStyleBackColor = false;
            this.btnObtenerCAE.Click += new System.EventHandler(this.btnObtenerCAE_Click);
            // 
            // frmFacturar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1000, 650);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelFooter);
            this.Controls.Add(this.panelHeader);
            this.MinimumSize = new System.Drawing.Size(1016, 689);
            this.Name = "frmFacturar";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Facturación de Presupuestos";
            this.Load += new System.EventHandler(this.frmFacturar_Load);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPresupuestos)).EndInit();
            this.panelInfo.ResumeLayout(false);
            this.panelInfo.PerformLayout();
            this.panelFooter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.DataGridView dgvPresupuestos;
        private System.Windows.Forms.Panel panelInfo;
        private System.Windows.Forms.Label lblTotalPresupuestos;
        private System.Windows.Forms.Label lblClienteSeleccionado;
        private System.Windows.Forms.Label lblTotalSeleccionado;
        private System.Windows.Forms.Panel panelFooter;
        private System.Windows.Forms.Button btnObtenerCAE;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Label lblPresupuestosSeleccionados;
        private System.Windows.Forms.Label lblSubtotal;
        private System.Windows.Forms.Label lblIVA;
        private System.Windows.Forms.Label lblIIBBArba;
    }
}
