namespace Percolore.IOConnect
{
    partial class fFormulaPersonalizada
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
            this.cmbCorante = new System.Windows.Forms.ComboBox();
            this.label_11 = new System.Windows.Forms.Label();
            this.listView = new System.Windows.Forms.ListView();
            this.label_10 = new System.Windows.Forms.Label();
            this.label_12 = new System.Windows.Forms.Label();
            this.pnlBarraTitulo = new System.Windows.Forms.Panel();
            this.btnDispensar = new System.Windows.Forms.Button();
            this.btnGravar = new System.Windows.Forms.Button();
            this.btnTeclado = new System.Windows.Forms.Button();
            this.btnSair = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnAdicionar = new System.Windows.Forms.Button();
            this.pnlUnDecimal = new System.Windows.Forms.Panel();
            this.lblDecimal = new System.Windows.Forms.Label();
            this.txtUnDecimal = new Percolore.Core.UserControl.UTextBox();
            this.pnlUnOnca = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtUnOnca48 = new Percolore.Core.UserControl.UTextBox();
            this.txtUnOncaY = new Percolore.Core.UserControl.UTextBox();
            this.lblUnidadesConvertidas = new System.Windows.Forms.Label();
            this.pnlQuantidade = new System.Windows.Forms.Panel();
            this.cboUnidade = new Percolore.Core.UserControl.UComboBox();
            this.txtNomeFormula = new Percolore.Core.UserControl.UTextBox();
            this.pnlBarraTitulo.SuspendLayout();
            this.pnlUnDecimal.SuspendLayout();
            this.pnlUnOnca.SuspendLayout();
            this.pnlQuantidade.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbCorante
            // 
            this.cmbCorante.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cmbCorante.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCorante.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCorante.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.cmbCorante.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.cmbCorante.FormattingEnabled = true;
            this.cmbCorante.IntegralHeight = false;
            this.cmbCorante.ItemHeight = 40;
            this.cmbCorante.Location = new System.Drawing.Point(16, 193);
            this.cmbCorante.MaxDropDownItems = 12;
            this.cmbCorante.Name = "cmbCorante";
            this.cmbCorante.Size = new System.Drawing.Size(448, 48);
            this.cmbCorante.TabIndex = 1;
            this.cmbCorante.SelectionChangeCommitted += new System.EventHandler(this.cmbCorante_SelectionChangeCommitted);
            // 
            // label_11
            // 
            this.label_11.AutoSize = true;
            this.label_11.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.label_11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.label_11.Location = new System.Drawing.Point(12, 166);
            this.label_11.Margin = new System.Windows.Forms.Padding(0, 9, 0, 3);
            this.label_11.Name = "label_11";
            this.label_11.Size = new System.Drawing.Size(81, 23);
            this.label_11.TabIndex = 305;
            this.label_11.Text = "Colorante";
            // 
            // listView
            // 
            this.listView.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.BackColor = System.Drawing.Color.White;
            this.listView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView.Font = new System.Drawing.Font("Segoe UI Light", 18F);
            this.listView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView.LabelWrap = false;
            this.listView.Location = new System.Drawing.Point(16, 353);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.ShowGroups = false;
            this.listView.Size = new System.Drawing.Size(867, 143);
            this.listView.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.listView.TabIndex = 8;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            // 
            // label_10
            // 
            this.label_10.AutoSize = true;
            this.label_10.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.label_10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.label_10.Location = new System.Drawing.Point(12, 79);
            this.label_10.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.label_10.Name = "label_10";
            this.label_10.Size = new System.Drawing.Size(55, 23);
            this.label_10.TabIndex = 315;
            this.label_10.Text = "Nome";
            // 
            // label_12
            // 
            this.label_12.AutoSize = true;
            this.label_12.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.label_12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.label_12.Location = new System.Drawing.Point(12, 254);
            this.label_12.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label_12.Name = "label_12";
            this.label_12.Size = new System.Drawing.Size(94, 23);
            this.label_12.TabIndex = 327;
            this.label_12.Text = "Quantidade";
            // 
            // pnlBarraTitulo
            // 
            this.pnlBarraTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBarraTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(145)))), ((int)(((byte)(204)))));
            this.pnlBarraTitulo.Controls.Add(this.btnDispensar);
            this.pnlBarraTitulo.Controls.Add(this.btnGravar);
            this.pnlBarraTitulo.Controls.Add(this.btnTeclado);
            this.pnlBarraTitulo.Controls.Add(this.btnSair);
            this.pnlBarraTitulo.Controls.Add(this.lblTitulo);
            this.pnlBarraTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.pnlBarraTitulo.Location = new System.Drawing.Point(0, 0);
            this.pnlBarraTitulo.Name = "pnlBarraTitulo";
            this.pnlBarraTitulo.Size = new System.Drawing.Size(905, 64);
            this.pnlBarraTitulo.TabIndex = 373;
            // 
            // btnDispensar
            // 
            this.btnDispensar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDispensar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(138)))), ((int)(((byte)(228)))));
            this.btnDispensar.FlatAppearance.BorderSize = 0;
            this.btnDispensar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDispensar.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnDispensar.Location = new System.Drawing.Point(637, 0);
            this.btnDispensar.Name = "btnDispensar";
            this.btnDispensar.Size = new System.Drawing.Size(64, 64);
            this.btnDispensar.TabIndex = 20;
            this.btnDispensar.Tag = "0";
            this.btnDispensar.Text = "Dispensar";
            this.btnDispensar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDispensar.UseVisualStyleBackColor = false;
            this.btnDispensar.Click += new System.EventHandler(this.btnDispensar_Click);
            // 
            // btnGravar
            // 
            this.btnGravar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGravar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(138)))), ((int)(((byte)(228)))));
            this.btnGravar.FlatAppearance.BorderSize = 0;
            this.btnGravar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGravar.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnGravar.Location = new System.Drawing.Point(704, 0);
            this.btnGravar.Name = "btnGravar";
            this.btnGravar.Size = new System.Drawing.Size(64, 64);
            this.btnGravar.TabIndex = 21;
            this.btnGravar.Tag = "0";
            this.btnGravar.Text = "Gravar";
            this.btnGravar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnGravar.UseVisualStyleBackColor = false;
            this.btnGravar.Click += new System.EventHandler(this.btnGravar_Click);
            // 
            // btnTeclado
            // 
            this.btnTeclado.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTeclado.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(138)))), ((int)(((byte)(228)))));
            this.btnTeclado.FlatAppearance.BorderSize = 0;
            this.btnTeclado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTeclado.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnTeclado.Location = new System.Drawing.Point(771, 0);
            this.btnTeclado.Name = "btnTeclado";
            this.btnTeclado.Size = new System.Drawing.Size(64, 64);
            this.btnTeclado.TabIndex = 22;
            this.btnTeclado.Tag = "0";
            this.btnTeclado.Text = "Teclado";
            this.btnTeclado.UseVisualStyleBackColor = false;
            this.btnTeclado.Click += new System.EventHandler(this.btnTeclado_Click);
            // 
            // btnSair
            // 
            this.btnSair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSair.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(138)))), ((int)(((byte)(228)))));
            this.btnSair.FlatAppearance.BorderSize = 0;
            this.btnSair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSair.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnSair.Location = new System.Drawing.Point(838, 0);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(64, 64);
            this.btnSair.TabIndex = 23;
            this.btnSair.Tag = "0";
            this.btnSair.Text = "Sair";
            this.btnSair.UseVisualStyleBackColor = false;
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click);
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12.75F);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.lblTitulo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTitulo.Location = new System.Drawing.Point(12, 21);
            this.lblTitulo.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(228, 23);
            this.lblTitulo.TabIndex = 309;
            this.lblTitulo.Text = "Eidtar fórmula personalizada";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnAdicionar
            // 
            this.btnAdicionar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdicionar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(145)))), ((int)(((byte)(204)))));
            this.btnAdicionar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(160)))), ((int)(((byte)(15)))));
            this.btnAdicionar.FlatAppearance.BorderSize = 0;
            this.btnAdicionar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdicionar.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnAdicionar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnAdicionar.Location = new System.Drawing.Point(819, 281);
            this.btnAdicionar.Margin = new System.Windows.Forms.Padding(0);
            this.btnAdicionar.Name = "btnAdicionar";
            this.btnAdicionar.Size = new System.Drawing.Size(64, 64);
            this.btnAdicionar.TabIndex = 7;
            this.btnAdicionar.Tag = "0";
            this.btnAdicionar.UseVisualStyleBackColor = false;
            this.btnAdicionar.Click += new System.EventHandler(this.btnAdicionar_Click);
            // 
            // pnlUnDecimal
            // 
            this.pnlUnDecimal.AutoSize = true;
            this.pnlUnDecimal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pnlUnDecimal.Controls.Add(this.lblDecimal);
            this.pnlUnDecimal.Controls.Add(this.txtUnDecimal);
            this.pnlUnDecimal.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.pnlUnDecimal.Location = new System.Drawing.Point(232, 9);
            this.pnlUnDecimal.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.pnlUnDecimal.Name = "pnlUnDecimal";
            this.pnlUnDecimal.Size = new System.Drawing.Size(216, 47);
            this.pnlUnDecimal.TabIndex = 419;
            this.pnlUnDecimal.TabStop = true;
            this.pnlUnDecimal.Visible = false;
            // 
            // lblDecimal
            // 
            this.lblDecimal.AutoSize = true;
            this.lblDecimal.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblDecimal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblDecimal.Location = new System.Drawing.Point(175, 12);
            this.lblDecimal.Margin = new System.Windows.Forms.Padding(0);
            this.lblDecimal.Name = "lblDecimal";
            this.lblDecimal.Size = new System.Drawing.Size(41, 23);
            this.lblDecimal.TabIndex = 325;
            this.lblDecimal.Text = "shot";
            // 
            // txtUnDecimal
            // 
            this.txtUnDecimal.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtUnDecimal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUnDecimal.Conteudo = Percolore.Core.UserControl.TipoConteudo.Decimal;
            this.txtUnDecimal.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txtUnDecimal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtUnDecimal.Location = new System.Drawing.Point(0, 0);
            this.txtUnDecimal.Margin = new System.Windows.Forms.Padding(0);
            this.txtUnDecimal.MaxLength = 100;
            this.txtUnDecimal.Name = "txtUnDecimal";
            this.txtUnDecimal.Size = new System.Drawing.Size(175, 47);
            this.txtUnDecimal.TabIndex = 4;
            this.txtUnDecimal.Tag = "grama";
            this.txtUnDecimal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtUnDecimal.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Quantidade_KeyUp);
            // 
            // pnlUnOnca
            // 
            this.pnlUnOnca.AutoSize = true;
            this.pnlUnOnca.Controls.Add(this.label7);
            this.pnlUnOnca.Controls.Add(this.label9);
            this.pnlUnOnca.Controls.Add(this.txtUnOnca48);
            this.pnlUnOnca.Controls.Add(this.txtUnOncaY);
            this.pnlUnOnca.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.pnlUnOnca.Location = new System.Drawing.Point(232, 9);
            this.pnlUnOnca.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.pnlUnOnca.Name = "pnlUnOnca";
            this.pnlUnOnca.Size = new System.Drawing.Size(221, 47);
            this.pnlUnOnca.TabIndex = 1;
            this.pnlUnOnca.TabStop = true;
            this.pnlUnOnca.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.label7.Location = new System.Drawing.Point(183, 12);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 23);
            this.label7.TabIndex = 325;
            this.label7.Text = "1:48";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.label9.Location = new System.Drawing.Point(65, 12);
            this.label9.Margin = new System.Windows.Forms.Padding(0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(19, 23);
            this.label9.TabIndex = 322;
            this.label9.Text = "Y";
            // 
            // txtUnOnca48
            // 
            this.txtUnOnca48.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtUnOnca48.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUnOnca48.Conteudo = Percolore.Core.UserControl.TipoConteudo.Decimal;
            this.txtUnOnca48.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txtUnOnca48.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtUnOnca48.Location = new System.Drawing.Point(92, 0);
            this.txtUnOnca48.Margin = new System.Windows.Forms.Padding(0);
            this.txtUnOnca48.MaxLength = 100;
            this.txtUnOnca48.Name = "txtUnOnca48";
            this.txtUnOnca48.Size = new System.Drawing.Size(91, 47);
            this.txtUnOnca48.TabIndex = 6;
            this.txtUnOnca48.Tag = "onça48";
            this.txtUnOnca48.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtUnOnca48.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Quantidade_KeyUp);
            // 
            // txtUnOncaY
            // 
            this.txtUnOncaY.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtUnOncaY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUnOncaY.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtUnOncaY.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txtUnOncaY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtUnOncaY.Location = new System.Drawing.Point(0, 0);
            this.txtUnOncaY.Margin = new System.Windows.Forms.Padding(0);
            this.txtUnOncaY.MaxLength = 100;
            this.txtUnOncaY.Name = "txtUnOncaY";
            this.txtUnOncaY.Size = new System.Drawing.Size(65, 47);
            this.txtUnOncaY.TabIndex = 5;
            this.txtUnOncaY.Tag = "onçaY";
            this.txtUnOncaY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtUnOncaY.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Quantidade_KeyUp);
            // 
            // lblUnidadesConvertidas
            // 
            this.lblUnidadesConvertidas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUnidadesConvertidas.AutoSize = true;
            this.lblUnidadesConvertidas.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblUnidadesConvertidas.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblUnidadesConvertidas.Location = new System.Drawing.Point(477, 21);
            this.lblUnidadesConvertidas.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblUnidadesConvertidas.Name = "lblUnidadesConvertidas";
            this.lblUnidadesConvertidas.Size = new System.Drawing.Size(166, 23);
            this.lblUnidadesConvertidas.TabIndex = 2;
            this.lblUnidadesConvertidas.Text = "Unidades convertidas";
            this.lblUnidadesConvertidas.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlQuantidade
            // 
            this.pnlQuantidade.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlQuantidade.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlQuantidade.Controls.Add(this.pnlUnDecimal);
            this.pnlQuantidade.Controls.Add(this.lblUnidadesConvertidas);
            this.pnlQuantidade.Controls.Add(this.cboUnidade);
            this.pnlQuantidade.Controls.Add(this.pnlUnOnca);
            this.pnlQuantidade.Enabled = false;
            this.pnlQuantidade.Location = new System.Drawing.Point(16, 281);
            this.pnlQuantidade.Name = "pnlQuantidade";
            this.pnlQuantidade.Size = new System.Drawing.Size(798, 64);
            this.pnlQuantidade.TabIndex = 1;
            // 
            // cboUnidade
            // 
            this.cboUnidade.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cboUnidade.BorderColor = System.Drawing.Color.Gainsboro;
            this.cboUnidade.BorderSize = 1;
            this.cboUnidade.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboUnidade.DropDownWidth = 100;
            this.cboUnidade.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboUnidade.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.cboUnidade.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.cboUnidade.FormattingEnabled = true;
            this.cboUnidade.IntegralHeight = false;
            this.cboUnidade.Location = new System.Drawing.Point(9, 9);
            this.cboUnidade.Name = "cboUnidade";
            this.cboUnidade.Size = new System.Drawing.Size(215, 48);
            this.cboUnidade.TabIndex = 3;
            this.cboUnidade.SelectionChangeCommitted += new System.EventHandler(this.cboUnidade_SelectionChangeCommitted);
            // 
            // txtNomeFormula
            // 
            this.txtNomeFormula.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNomeFormula.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtNomeFormula.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNomeFormula.Conteudo = Percolore.Core.UserControl.TipoConteudo.Padrao;
            this.txtNomeFormula.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txtNomeFormula.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtNomeFormula.Location = new System.Drawing.Point(16, 106);
            this.txtNomeFormula.MaxLength = 100;
            this.txtNomeFormula.Name = "txtNomeFormula";
            this.txtNomeFormula.Size = new System.Drawing.Size(867, 47);
            this.txtNomeFormula.TabIndex = 0;
            this.txtNomeFormula.Tag = "";
            // 
            // fFormulaPersonalizada
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(900, 513);
            this.Controls.Add(this.pnlQuantidade);
            this.Controls.Add(this.pnlBarraTitulo);
            this.Controls.Add(this.label_12);
            this.Controls.Add(this.label_10);
            this.Controls.Add(this.txtNomeFormula);
            this.Controls.Add(this.btnAdicionar);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.label_11);
            this.Controls.Add(this.cmbCorante);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fFormulaPersonalizada";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Editar fórmula personalizada";
            this.TopMost = true;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormulaEditar_Paint);
            this.pnlBarraTitulo.ResumeLayout(false);
            this.pnlBarraTitulo.PerformLayout();
            this.pnlUnDecimal.ResumeLayout(false);
            this.pnlUnDecimal.PerformLayout();
            this.pnlUnOnca.ResumeLayout(false);
            this.pnlUnOnca.PerformLayout();
            this.pnlQuantidade.ResumeLayout(false);
            this.pnlQuantidade.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cmbCorante;
        private System.Windows.Forms.Label label_11;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.Label label_10;
        private Percolore.Core.UserControl.UTextBox txtNomeFormula;
        private System.Windows.Forms.Label label_12;
        private System.Windows.Forms.Panel pnlBarraTitulo;
        private System.Windows.Forms.Button btnDispensar;
        private System.Windows.Forms.Button btnGravar;
        private System.Windows.Forms.Button btnTeclado;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnAdicionar;
        private System.Windows.Forms.Panel pnlUnDecimal;
        private System.Windows.Forms.Label lblDecimal;
        private Percolore.Core.UserControl.UTextBox txtUnDecimal;
        private Percolore.Core.UserControl.UComboBox cboUnidade;
        private System.Windows.Forms.Panel pnlUnOnca;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private Percolore.Core.UserControl.UTextBox txtUnOnca48;
        private Percolore.Core.UserControl.UTextBox txtUnOncaY;
        private System.Windows.Forms.Label lblUnidadesConvertidas;
        private System.Windows.Forms.Panel pnlQuantidade;
    }
}