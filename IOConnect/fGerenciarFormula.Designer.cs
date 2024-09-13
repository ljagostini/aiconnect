namespace Percolore.IOConnect
{
    partial class fGerenciarFormula
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
            this.listView = new System.Windows.Forms.ListView();
            this.label_8 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cmbFormula = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnNovaFormula = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnExcluir = new System.Windows.Forms.Button();
            this.btnDispensar = new System.Windows.Forms.Button();
            this.btnTeclado = new System.Windows.Forms.Button();
            this.btnSair = new System.Windows.Forms.Button();
            this.label_7 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.BackColor = System.Drawing.Color.White;
            this.listView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel1.SetColumnSpan(this.listView, 5);
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.listView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView.Location = new System.Drawing.Point(15, 100);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(962, 421);
            this.listView.TabIndex = 1;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // label_8
            // 
            this.label_8.AutoSize = true;
            this.label_8.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.label_8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.label_8.Location = new System.Drawing.Point(12, 12);
            this.label_8.Margin = new System.Windows.Forms.Padding(0);
            this.label_8.Name = "label_8";
            this.label_8.Size = new System.Drawing.Size(68, 23);
            this.label_8.TabIndex = 308;
            this.label_8.Text = "Fórmula";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 7;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanel1.Controls.Add(this.cmbFormula, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.listView, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label_8, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 64);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 4F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 4F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(992, 536);
            this.tableLayoutPanel1.TabIndex = 310;
            // 
            // cmbFormula
            // 
            this.cmbFormula.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tableLayoutPanel1.SetColumnSpan(this.cmbFormula, 5);
            this.cmbFormula.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbFormula.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFormula.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbFormula.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.cmbFormula.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.cmbFormula.FormattingEnabled = true;
            this.cmbFormula.IntegralHeight = false;
            this.cmbFormula.ItemHeight = 40;
            this.cmbFormula.Location = new System.Drawing.Point(15, 42);
            this.cmbFormula.MaxDropDownItems = 12;
            this.cmbFormula.Name = "cmbFormula";
            this.cmbFormula.Size = new System.Drawing.Size(962, 48);
            this.cmbFormula.TabIndex = 0;
            this.cmbFormula.SelectedIndexChanged += new System.EventHandler(this.cmbFormula_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(81)))), ((int)(((byte)(76)))));
            this.panel2.Controls.Add(this.btnNovaFormula);
            this.panel2.Controls.Add(this.btnEditar);
            this.panel2.Controls.Add(this.btnExcluir);
            this.panel2.Controls.Add(this.btnDispensar);
            this.panel2.Controls.Add(this.btnTeclado);
            this.panel2.Controls.Add(this.btnSair);
            this.panel2.Controls.Add(this.label_7);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(992, 64);
            this.panel2.TabIndex = 372;
            // 
            // btnNovaFormula
            // 
            this.btnNovaFormula.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNovaFormula.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(160)))), ((int)(((byte)(15)))));
            this.btnNovaFormula.FlatAppearance.BorderSize = 0;
            this.btnNovaFormula.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNovaFormula.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnNovaFormula.Location = new System.Drawing.Point(593, 0);
            this.btnNovaFormula.Name = "btnNovaFormula";
            this.btnNovaFormula.Size = new System.Drawing.Size(64, 64);
            this.btnNovaFormula.TabIndex = 313;
            this.btnNovaFormula.Tag = "0";
            this.btnNovaFormula.Text = "Adicionar";
            this.btnNovaFormula.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnNovaFormula.UseVisualStyleBackColor = false;
            this.btnNovaFormula.Click += new System.EventHandler(this.btnNovaFormula_Click);
            // 
            // btnEditar
            // 
            this.btnEditar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(160)))), ((int)(((byte)(15)))));
            this.btnEditar.FlatAppearance.BorderSize = 0;
            this.btnEditar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditar.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnEditar.Location = new System.Drawing.Point(660, 0);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(64, 64);
            this.btnEditar.TabIndex = 312;
            this.btnEditar.Tag = "0";
            this.btnEditar.Text = "Editar";
            this.btnEditar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnEditar.UseVisualStyleBackColor = false;
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            // 
            // btnExcluir
            // 
            this.btnExcluir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExcluir.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(160)))), ((int)(((byte)(15)))));
            this.btnExcluir.FlatAppearance.BorderSize = 0;
            this.btnExcluir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExcluir.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnExcluir.Location = new System.Drawing.Point(727, 0);
            this.btnExcluir.Name = "btnExcluir";
            this.btnExcluir.Size = new System.Drawing.Size(64, 64);
            this.btnExcluir.TabIndex = 311;
            this.btnExcluir.Tag = "0";
            this.btnExcluir.Text = "Excluir";
            this.btnExcluir.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExcluir.UseVisualStyleBackColor = false;
            this.btnExcluir.Click += new System.EventHandler(this.btnExcluir_Click);
            // 
            // btnDispensar
            // 
            this.btnDispensar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDispensar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(160)))), ((int)(((byte)(15)))));
            this.btnDispensar.FlatAppearance.BorderSize = 0;
            this.btnDispensar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDispensar.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnDispensar.Location = new System.Drawing.Point(794, 0);
            this.btnDispensar.Name = "btnDispensar";
            this.btnDispensar.Size = new System.Drawing.Size(64, 64);
            this.btnDispensar.TabIndex = 310;
            this.btnDispensar.Tag = "0";
            this.btnDispensar.Text = "Dispensar";
            this.btnDispensar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDispensar.UseVisualStyleBackColor = false;
            this.btnDispensar.Click += new System.EventHandler(this.btnDispensar_Click);
            // 
            // btnTeclado
            // 
            this.btnTeclado.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTeclado.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(160)))), ((int)(((byte)(15)))));
            this.btnTeclado.FlatAppearance.BorderSize = 0;
            this.btnTeclado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTeclado.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnTeclado.Location = new System.Drawing.Point(861, 0);
            this.btnTeclado.Name = "btnTeclado";
            this.btnTeclado.Size = new System.Drawing.Size(64, 64);
            this.btnTeclado.TabIndex = 1;
            this.btnTeclado.Tag = "0";
            this.btnTeclado.Text = "Teclado";
            this.btnTeclado.UseVisualStyleBackColor = false;
            this.btnTeclado.Click += new System.EventHandler(this.btnTeclado_Click);
            // 
            // btnSair
            // 
            this.btnSair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSair.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(160)))), ((int)(((byte)(15)))));
            this.btnSair.FlatAppearance.BorderSize = 0;
            this.btnSair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSair.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnSair.Location = new System.Drawing.Point(928, 0);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(64, 64);
            this.btnSair.TabIndex = 2;
            this.btnSair.Tag = "0";
            this.btnSair.Text = "Sair";
            this.btnSair.UseVisualStyleBackColor = false;
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click);
            // 
            // label_7
            // 
            this.label_7.AutoSize = true;
            this.label_7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label_7.Font = new System.Drawing.Font("Segoe UI", 12.75F);
            this.label_7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.label_7.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_7.Location = new System.Drawing.Point(12, 21);
            this.label_7.Margin = new System.Windows.Forms.Padding(0);
            this.label_7.Name = "label_7";
            this.label_7.Size = new System.Drawing.Size(271, 23);
            this.label_7.TabIndex = 309;
            this.label_7.Text = "Gerenciar fórmulas personalizadas";
            this.label_7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fGerenciarFormula
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(992, 600);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "fGerenciarFormula";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gerenciar fórmulas personalizadas";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.CriarFormula_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.Label label_8;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox cmbFormula;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnTeclado;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.Label label_7;
        private System.Windows.Forms.Button btnDispensar;
        private System.Windows.Forms.Button btnNovaFormula;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnExcluir;
    }
}