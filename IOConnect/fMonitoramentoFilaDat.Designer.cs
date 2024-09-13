namespace Percolore.IOConnect
{
    partial class fMonitoramentoFilaDat
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btn_Consulta_Dat = new System.Windows.Forms.Button();
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.dataGridViewImageColumn2 = new System.Windows.Forms.DataGridViewImageColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Terminal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CodForumula = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CorFormula = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataGerada = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IdBaseDados = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColDispensar = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColExcluir = new System.Windows.Forms.DataGridViewImageColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(176)))), ((int)(((byte)(37)))));
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.lblTitulo.Location = new System.Drawing.Point(12, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(1197, 64);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Gerenciador Fila Dat";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(176)))), ((int)(((byte)(37)))));
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(557, 436);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(125, 50);
            this.btnCancelar.TabIndex = 25;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btn_Consulta_Dat
            // 
            this.btn_Consulta_Dat.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btn_Consulta_Dat.BackColor = System.Drawing.SystemColors.Control;
            this.btn_Consulta_Dat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Consulta_Dat.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btn_Consulta_Dat.ForeColor = System.Drawing.Color.White;
            this.btn_Consulta_Dat.Location = new System.Drawing.Point(1148, 21);
            this.btn_Consulta_Dat.Name = "btn_Consulta_Dat";
            this.btn_Consulta_Dat.Size = new System.Drawing.Size(44, 46);
            this.btn_Consulta_Dat.TabIndex = 26;
            this.btn_Consulta_Dat.UseVisualStyleBackColor = false;
            this.btn_Consulta_Dat.Click += new System.EventHandler(this.btn_Consulta_Dat_Click);
            // 
            // dataGrid
            // 
            this.dataGrid.AllowUserToAddRows = false;
            this.dataGrid.AllowUserToDeleteRows = false;
            this.dataGrid.AllowUserToResizeRows = false;
            this.dataGrid.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.dataGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dataGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(175)))), ((int)(((byte)(40)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 12F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGrid.ColumnHeadersHeight = 50;
            this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.Terminal,
            this.CodForumula,
            this.CorFormula,
            this.DataGerada,
            this.IdBaseDados,
            this.ColDispensar,
            this.ColExcluir});
            this.dataGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGrid.EnableHeadersVisualStyles = false;
            this.dataGrid.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.dataGrid.Location = new System.Drawing.Point(9, 83);
            this.dataGrid.Margin = new System.Windows.Forms.Padding(0);
            this.dataGrid.MultiSelect = false;
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.ReadOnly = true;
            this.dataGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.dataGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGrid.RowHeadersVisible = false;
            this.dataGrid.RowHeadersWidth = 50;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 15F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.dataGrid.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGrid.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dataGrid.RowTemplate.DefaultCellStyle.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.dataGrid.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dataGrid.RowTemplate.Height = 50;
            this.dataGrid.RowTemplate.ReadOnly = true;
            this.dataGrid.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGrid.Size = new System.Drawing.Size(1200, 332);
            this.dataGrid.TabIndex = 403;
            this.dataGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellContentClick);
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.HeaderText = "";
            this.dataGridViewImageColumn1.MinimumWidth = 64;
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.Width = 64;
            // 
            // dataGridViewImageColumn2
            // 
            this.dataGridViewImageColumn2.HeaderText = "";
            this.dataGridViewImageColumn2.Name = "dataGridViewImageColumn2";
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewTextBoxColumn1.FillWeight = 105.2631F;
            this.dataGridViewTextBoxColumn1.HeaderText = "Arquivo Dat";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Visible = false;
            this.dataGridViewTextBoxColumn1.Width = 436;
            // 
            // Terminal
            // 
            this.Terminal.FillWeight = 200F;
            this.Terminal.HeaderText = "Terminal";
            this.Terminal.Name = "Terminal";
            this.Terminal.ReadOnly = true;
            this.Terminal.Width = 200;
            // 
            // CodForumula
            // 
            this.CodForumula.FillWeight = 200F;
            this.CodForumula.HeaderText = "Codigo Formula";
            this.CodForumula.Name = "CodForumula";
            this.CodForumula.ReadOnly = true;
            this.CodForumula.Width = 200;
            // 
            // CorFormula
            // 
            this.CorFormula.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.CorFormula.HeaderText = "Cor Formula";
            this.CorFormula.Name = "CorFormula";
            this.CorFormula.ReadOnly = true;
            // 
            // DataGerada
            // 
            this.DataGerada.FillWeight = 200F;
            this.DataGerada.HeaderText = "Data";
            this.DataGerada.Name = "DataGerada";
            this.DataGerada.ReadOnly = true;
            this.DataGerada.Width = 200;
            // 
            // IdBaseDados
            // 
            this.IdBaseDados.HeaderText = "IdBaseDados";
            this.IdBaseDados.Name = "IdBaseDados";
            this.IdBaseDados.ReadOnly = true;
            this.IdBaseDados.Visible = false;
            // 
            // ColDispensar
            // 
            this.ColDispensar.HeaderText = "";
            this.ColDispensar.MinimumWidth = 64;
            this.ColDispensar.Name = "ColDispensar";
            this.ColDispensar.ReadOnly = true;
            this.ColDispensar.Width = 64;
            // 
            // ColExcluir
            // 
            this.ColExcluir.HeaderText = "";
            this.ColExcluir.MinimumWidth = 64;
            this.ColExcluir.Name = "ColExcluir";
            this.ColExcluir.ReadOnly = true;
            this.ColExcluir.Width = 64;
            // 
            // fMonitoramentoFilaDat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1221, 493);
            this.Controls.Add(this.dataGrid);
            this.Controls.Add(this.btn_Consulta_Dat);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fMonitoramentoFilaDat";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "fMonitoramentoFilaDat";
            this.Load += new System.EventHandler(this.fMonitoramentoFilaDat_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btn_Consulta_Dat;
        private System.Windows.Forms.DataGridView dataGrid;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Terminal;
        private System.Windows.Forms.DataGridViewTextBoxColumn CodForumula;
        private System.Windows.Forms.DataGridViewTextBoxColumn CorFormula;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataGerada;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdBaseDados;
        private System.Windows.Forms.DataGridViewImageColumn ColDispensar;
        private System.Windows.Forms.DataGridViewImageColumn ColExcluir;
    }
}