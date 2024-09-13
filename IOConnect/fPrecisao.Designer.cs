namespace Percolore.IOConnect
{
    partial class fPrecisao
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fPrecisao));
            this.toolMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTituloMassaBal = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.lblDelayScale = new System.Windows.Forms.Label();
            this.lblMassaRecipiente = new System.Windows.Forms.Label();
            this.gbComunicacaoBalanca = new System.Windows.Forms.GroupBox();
            this.btnSair = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblCalibracaoAutoColorante = new System.Windows.Forms.Label();
            this.lblCalibracaoAutoMotor = new System.Windows.Forms.Label();
            this.lblCalibracaoAutoMassaEspecifica = new System.Windows.Forms.Label();
            this.lblCalibracaoAutoLegendaMotor = new System.Windows.Forms.Label();
            this.lblCalibracaoAutoLegendaMassaEspec = new System.Windows.Forms.Label();
            this.tableLayoutPanel12 = new System.Windows.Forms.TableLayoutPanel();
            this.lblMinMassaAdmRecipiente = new System.Windows.Forms.Label();
            this.txtVolumeMaxRecipiente = new Percolore.Core.UserControl.UTextBox();
            this.lblVolumeMaxRecipiente = new System.Windows.Forms.Label();
            this.txtMassaAdmRecipiente = new Percolore.Core.UserControl.UTextBox();
            this.lblMassaAdmRecipiente = new System.Windows.Forms.Label();
            this.lblCapacidadeMaxBalanca = new System.Windows.Forms.Label();
            this.txtCapacidadeMaxBalanca = new Percolore.Core.UserControl.UTextBox();
            this.txtMinMassaAdmRecipiente = new Percolore.Core.UserControl.UTextBox();
            this.tableLayoutPanel11 = new System.Windows.Forms.TableLayoutPanel();
            this.lblAccurracyNameColorante = new System.Windows.Forms.Label();
            this.pnlBarraTitulo = new System.Windows.Forms.Panel();
            this.dgvCalibracaoAuto = new System.Windows.Forms.DataGridView();
            this.Volume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tentativas = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VolumeDos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Executado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txt_delay_seg_bal = new Percolore.Core.UserControl.UTextBox();
            this.statusStrip1.SuspendLayout();
            this.gbComunicacaoBalanca.SuspendLayout();
            this.tableLayoutPanel12.SuspendLayout();
            this.tableLayoutPanel11.SuspendLayout();
            this.pnlBarraTitulo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCalibracaoAuto)).BeginInit();
            this.SuspendLayout();
            // 
            // toolMessage
            // 
            this.toolMessage.Name = "toolMessage";
            this.toolMessage.Size = new System.Drawing.Size(95, 17);
            this.toolMessage.Text = "Status Accurracy";
            // 
            // lblTituloMassaBal
            // 
            this.lblTituloMassaBal.AutoSize = true;
            this.lblTituloMassaBal.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblTituloMassaBal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblTituloMassaBal.Location = new System.Drawing.Point(17, 18);
            this.lblTituloMassaBal.Name = "lblTituloMassaBal";
            this.lblTituloMassaBal.Size = new System.Drawing.Size(136, 23);
            this.lblTituloMassaBal.TabIndex = 1;
            this.lblTituloMassaBal.Text = "Massa Balança(g):";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolProgress,
            this.toolMessage});
            this.statusStrip1.Location = new System.Drawing.Point(0, 628);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1218, 22);
            this.statusStrip1.TabIndex = 408;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolProgress
            // 
            this.toolProgress.Name = "toolProgress";
            this.toolProgress.Size = new System.Drawing.Size(400, 16);
            this.toolProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // lblDelayScale
            // 
            this.lblDelayScale.AutoSize = true;
            this.lblDelayScale.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblDelayScale.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblDelayScale.Location = new System.Drawing.Point(563, 147);
            this.lblDelayScale.Name = "lblDelayScale";
            this.lblDelayScale.Size = new System.Drawing.Size(156, 23);
            this.lblDelayScale.TabIndex = 406;
            this.lblDelayScale.Text = "Delay Balança (seg.):";
            // 
            // lblMassaRecipiente
            // 
            this.lblMassaRecipiente.AutoSize = true;
            this.lblMassaRecipiente.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblMassaRecipiente.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblMassaRecipiente.Location = new System.Drawing.Point(155, 18);
            this.lblMassaRecipiente.Name = "lblMassaRecipiente";
            this.lblMassaRecipiente.Size = new System.Drawing.Size(19, 23);
            this.lblMassaRecipiente.TabIndex = 2;
            this.lblMassaRecipiente.Text = "0";
            // 
            // gbComunicacaoBalanca
            // 
            this.gbComunicacaoBalanca.Controls.Add(this.lblMassaRecipiente);
            this.gbComunicacaoBalanca.Controls.Add(this.lblTituloMassaBal);
            this.gbComunicacaoBalanca.Location = new System.Drawing.Point(945, 129);
            this.gbComunicacaoBalanca.Name = "gbComunicacaoBalanca";
            this.gbComunicacaoBalanca.Size = new System.Drawing.Size(245, 48);
            this.gbComunicacaoBalanca.TabIndex = 402;
            this.gbComunicacaoBalanca.TabStop = false;
            this.gbComunicacaoBalanca.Text = "Comunicação Balança";
            // 
            // btnSair
            // 
            this.btnSair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSair.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(145)))), ((int)(((byte)(204)))));
            this.btnSair.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(138)))), ((int)(((byte)(228)))));
            this.btnSair.FlatAppearance.BorderSize = 0;
            this.btnSair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSair.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnSair.Location = new System.Drawing.Point(1154, 0);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(64, 64);
            this.btnSair.TabIndex = 1;
            this.btnSair.Tag = "0";
            this.btnSair.Text = "Close";
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
            this.lblTitulo.Size = new System.Drawing.Size(78, 23);
            this.lblTitulo.TabIndex = 309;
            this.lblTitulo.Text = "Accuracy";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCalibracaoAutoColorante
            // 
            this.lblCalibracaoAutoColorante.AutoSize = true;
            this.lblCalibracaoAutoColorante.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCalibracaoAutoColorante.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblCalibracaoAutoColorante.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblCalibracaoAutoColorante.Location = new System.Drawing.Point(3, 3);
            this.lblCalibracaoAutoColorante.Margin = new System.Windows.Forms.Padding(3);
            this.lblCalibracaoAutoColorante.Name = "lblCalibracaoAutoColorante";
            this.lblCalibracaoAutoColorante.Size = new System.Drawing.Size(85, 49);
            this.lblCalibracaoAutoColorante.TabIndex = 352;
            this.lblCalibracaoAutoColorante.Text = "Colorante:";
            this.lblCalibracaoAutoColorante.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCalibracaoAutoMotor
            // 
            this.lblCalibracaoAutoMotor.AutoSize = true;
            this.lblCalibracaoAutoMotor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCalibracaoAutoMotor.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.lblCalibracaoAutoMotor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblCalibracaoAutoMotor.Location = new System.Drawing.Point(307, 3);
            this.lblCalibracaoAutoMotor.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.lblCalibracaoAutoMotor.Name = "lblCalibracaoAutoMotor";
            this.lblCalibracaoAutoMotor.Size = new System.Drawing.Size(48, 49);
            this.lblCalibracaoAutoMotor.TabIndex = 365;
            this.lblCalibracaoAutoMotor.Text = "00";
            this.lblCalibracaoAutoMotor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCalibracaoAutoMassaEspecifica
            // 
            this.lblCalibracaoAutoMassaEspecifica.AutoSize = true;
            this.lblCalibracaoAutoMassaEspecifica.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCalibracaoAutoMassaEspecifica.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.lblCalibracaoAutoMassaEspecifica.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblCalibracaoAutoMassaEspecifica.Location = new System.Drawing.Point(500, 3);
            this.lblCalibracaoAutoMassaEspecifica.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.lblCalibracaoAutoMassaEspecifica.Name = "lblCalibracaoAutoMassaEspecifica";
            this.lblCalibracaoAutoMassaEspecifica.Size = new System.Drawing.Size(130, 49);
            this.lblCalibracaoAutoMassaEspecifica.TabIndex = 361;
            this.lblCalibracaoAutoMassaEspecifica.Text = "0000.000";
            this.lblCalibracaoAutoMassaEspecifica.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCalibracaoAutoLegendaMotor
            // 
            this.lblCalibracaoAutoLegendaMotor.AutoSize = true;
            this.lblCalibracaoAutoLegendaMotor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCalibracaoAutoLegendaMotor.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblCalibracaoAutoLegendaMotor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblCalibracaoAutoLegendaMotor.Location = new System.Drawing.Point(247, 3);
            this.lblCalibracaoAutoLegendaMotor.Margin = new System.Windows.Forms.Padding(6, 3, 0, 3);
            this.lblCalibracaoAutoLegendaMotor.Name = "lblCalibracaoAutoLegendaMotor";
            this.lblCalibracaoAutoLegendaMotor.Size = new System.Drawing.Size(60, 49);
            this.lblCalibracaoAutoLegendaMotor.TabIndex = 364;
            this.lblCalibracaoAutoLegendaMotor.Text = "Motor:";
            this.lblCalibracaoAutoLegendaMotor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCalibracaoAutoLegendaMassaEspec
            // 
            this.lblCalibracaoAutoLegendaMassaEspec.AutoSize = true;
            this.lblCalibracaoAutoLegendaMassaEspec.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCalibracaoAutoLegendaMassaEspec.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblCalibracaoAutoLegendaMassaEspec.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblCalibracaoAutoLegendaMassaEspec.Location = new System.Drawing.Point(364, 3);
            this.lblCalibracaoAutoLegendaMassaEspec.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.lblCalibracaoAutoLegendaMassaEspec.Name = "lblCalibracaoAutoLegendaMassaEspec";
            this.lblCalibracaoAutoLegendaMassaEspec.Size = new System.Drawing.Size(133, 49);
            this.lblCalibracaoAutoLegendaMassaEspec.TabIndex = 359;
            this.lblCalibracaoAutoLegendaMassaEspec.Text = "Massa específica:";
            this.lblCalibracaoAutoLegendaMassaEspec.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel12
            // 
            this.tableLayoutPanel12.ColumnCount = 2;
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel12.Controls.Add(this.lblMinMassaAdmRecipiente, 0, 4);
            this.tableLayoutPanel12.Controls.Add(this.txtVolumeMaxRecipiente, 1, 2);
            this.tableLayoutPanel12.Controls.Add(this.lblVolumeMaxRecipiente, 0, 2);
            this.tableLayoutPanel12.Controls.Add(this.txtMassaAdmRecipiente, 1, 1);
            this.tableLayoutPanel12.Controls.Add(this.lblMassaAdmRecipiente, 0, 1);
            this.tableLayoutPanel12.Controls.Add(this.lblCapacidadeMaxBalanca, 0, 0);
            this.tableLayoutPanel12.Controls.Add(this.txtCapacidadeMaxBalanca, 1, 0);
            this.tableLayoutPanel12.Controls.Add(this.txtMinMassaAdmRecipiente, 1, 4);
            this.tableLayoutPanel12.Location = new System.Drawing.Point(12, 67);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            this.tableLayoutPanel12.RowCount = 5;
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel12.Size = new System.Drawing.Size(539, 110);
            this.tableLayoutPanel12.TabIndex = 399;
            // 
            // lblMinMassaAdmRecipiente
            // 
            this.lblMinMassaAdmRecipiente.AutoSize = true;
            this.lblMinMassaAdmRecipiente.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMinMassaAdmRecipiente.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblMinMassaAdmRecipiente.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblMinMassaAdmRecipiente.Location = new System.Drawing.Point(3, 72);
            this.lblMinMassaAdmRecipiente.Name = "lblMinMassaAdmRecipiente";
            this.lblMinMassaAdmRecipiente.Size = new System.Drawing.Size(371, 38);
            this.lblMinMassaAdmRecipiente.TabIndex = 8;
            this.lblMinMassaAdmRecipiente.Text = "Peso do recipiente (g):";
            // 
            // txtVolumeMaxRecipiente
            // 
            this.txtVolumeMaxRecipiente.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtVolumeMaxRecipiente.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtVolumeMaxRecipiente.Conteudo = Percolore.Core.UserControl.TipoConteudo.Decimal;
            this.txtVolumeMaxRecipiente.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtVolumeMaxRecipiente.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.txtVolumeMaxRecipiente.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtVolumeMaxRecipiente.Location = new System.Drawing.Point(380, 39);
            this.txtVolumeMaxRecipiente.Name = "txtVolumeMaxRecipiente";
            this.txtVolumeMaxRecipiente.ReadOnly = true;
            this.txtVolumeMaxRecipiente.Size = new System.Drawing.Size(156, 30);
            this.txtVolumeMaxRecipiente.TabIndex = 5;
            // 
            // lblVolumeMaxRecipiente
            // 
            this.lblVolumeMaxRecipiente.AutoSize = true;
            this.lblVolumeMaxRecipiente.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblVolumeMaxRecipiente.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblVolumeMaxRecipiente.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblVolumeMaxRecipiente.Location = new System.Drawing.Point(3, 36);
            this.lblVolumeMaxRecipiente.Name = "lblVolumeMaxRecipiente";
            this.lblVolumeMaxRecipiente.Size = new System.Drawing.Size(371, 36);
            this.lblVolumeMaxRecipiente.TabIndex = 4;
            this.lblVolumeMaxRecipiente.Text = "Volume máximo recipiente (ml):";
            // 
            // txtMassaAdmRecipiente
            // 
            this.txtMassaAdmRecipiente.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtMassaAdmRecipiente.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMassaAdmRecipiente.Conteudo = Percolore.Core.UserControl.TipoConteudo.Decimal;
            this.txtMassaAdmRecipiente.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMassaAdmRecipiente.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.txtMassaAdmRecipiente.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtMassaAdmRecipiente.Location = new System.Drawing.Point(380, 39);
            this.txtMassaAdmRecipiente.Name = "txtMassaAdmRecipiente";
            this.txtMassaAdmRecipiente.ReadOnly = true;
            this.txtMassaAdmRecipiente.Size = new System.Drawing.Size(156, 30);
            this.txtMassaAdmRecipiente.TabIndex = 3;
            // 
            // lblMassaAdmRecipiente
            // 
            this.lblMassaAdmRecipiente.AutoSize = true;
            this.lblMassaAdmRecipiente.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMassaAdmRecipiente.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblMassaAdmRecipiente.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblMassaAdmRecipiente.Location = new System.Drawing.Point(3, 36);
            this.lblMassaAdmRecipiente.Name = "lblMassaAdmRecipiente";
            this.lblMassaAdmRecipiente.Size = new System.Drawing.Size(371, 1);
            this.lblMassaAdmRecipiente.TabIndex = 2;
            this.lblMassaAdmRecipiente.Text = "Máxima massa admissivel recipiente (g):";
            // 
            // lblCapacidadeMaxBalanca
            // 
            this.lblCapacidadeMaxBalanca.AutoSize = true;
            this.lblCapacidadeMaxBalanca.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCapacidadeMaxBalanca.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblCapacidadeMaxBalanca.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblCapacidadeMaxBalanca.Location = new System.Drawing.Point(3, 0);
            this.lblCapacidadeMaxBalanca.Name = "lblCapacidadeMaxBalanca";
            this.lblCapacidadeMaxBalanca.Size = new System.Drawing.Size(371, 36);
            this.lblCapacidadeMaxBalanca.TabIndex = 0;
            this.lblCapacidadeMaxBalanca.Text = "Capacidade Máxima Balança (g):";
            // 
            // txtCapacidadeMaxBalanca
            // 
            this.txtCapacidadeMaxBalanca.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtCapacidadeMaxBalanca.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCapacidadeMaxBalanca.Conteudo = Percolore.Core.UserControl.TipoConteudo.Decimal;
            this.txtCapacidadeMaxBalanca.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCapacidadeMaxBalanca.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.txtCapacidadeMaxBalanca.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtCapacidadeMaxBalanca.Location = new System.Drawing.Point(380, 3);
            this.txtCapacidadeMaxBalanca.Name = "txtCapacidadeMaxBalanca";
            this.txtCapacidadeMaxBalanca.ReadOnly = true;
            this.txtCapacidadeMaxBalanca.Size = new System.Drawing.Size(156, 30);
            this.txtCapacidadeMaxBalanca.TabIndex = 1;
            // 
            // txtMinMassaAdmRecipiente
            // 
            this.txtMinMassaAdmRecipiente.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtMinMassaAdmRecipiente.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMinMassaAdmRecipiente.Conteudo = Percolore.Core.UserControl.TipoConteudo.Decimal;
            this.txtMinMassaAdmRecipiente.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.txtMinMassaAdmRecipiente.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtMinMassaAdmRecipiente.Location = new System.Drawing.Point(380, 75);
            this.txtMinMassaAdmRecipiente.Name = "txtMinMassaAdmRecipiente";
            this.txtMinMassaAdmRecipiente.ReadOnly = true;
            this.txtMinMassaAdmRecipiente.Size = new System.Drawing.Size(156, 30);
            this.txtMinMassaAdmRecipiente.TabIndex = 9;
            // 
            // tableLayoutPanel11
            // 
            this.tableLayoutPanel11.AutoSize = true;
            this.tableLayoutPanel11.ColumnCount = 6;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel11.Controls.Add(this.lblAccurracyNameColorante, 1, 0);
            this.tableLayoutPanel11.Controls.Add(this.lblCalibracaoAutoColorante, 0, 0);
            this.tableLayoutPanel11.Controls.Add(this.lblCalibracaoAutoMotor, 3, 0);
            this.tableLayoutPanel11.Controls.Add(this.lblCalibracaoAutoMassaEspecifica, 5, 0);
            this.tableLayoutPanel11.Controls.Add(this.lblCalibracaoAutoLegendaMotor, 2, 0);
            this.tableLayoutPanel11.Controls.Add(this.lblCalibracaoAutoLegendaMassaEspec, 4, 0);
            this.tableLayoutPanel11.Location = new System.Drawing.Point(560, 68);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 1;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel11.Size = new System.Drawing.Size(630, 55);
            this.tableLayoutPanel11.TabIndex = 398;
            // 
            // lblAccurracyNameColorante
            // 
            this.lblAccurracyNameColorante.AutoSize = true;
            this.lblAccurracyNameColorante.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAccurracyNameColorante.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblAccurracyNameColorante.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblAccurracyNameColorante.Location = new System.Drawing.Point(94, 0);
            this.lblAccurracyNameColorante.Name = "lblAccurracyNameColorante";
            this.lblAccurracyNameColorante.Size = new System.Drawing.Size(144, 55);
            this.lblAccurracyNameColorante.TabIndex = 410;
            this.lblAccurracyNameColorante.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlBarraTitulo
            // 
            this.pnlBarraTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(145)))), ((int)(((byte)(204)))));
            this.pnlBarraTitulo.Controls.Add(this.btnSair);
            this.pnlBarraTitulo.Controls.Add(this.lblTitulo);
            this.pnlBarraTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBarraTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.pnlBarraTitulo.Location = new System.Drawing.Point(0, 0);
            this.pnlBarraTitulo.Name = "pnlBarraTitulo";
            this.pnlBarraTitulo.Size = new System.Drawing.Size(1218, 64);
            this.pnlBarraTitulo.TabIndex = 397;
            // 
            // dgvCalibracaoAuto
            // 
            this.dgvCalibracaoAuto.AllowUserToAddRows = false;
            this.dgvCalibracaoAuto.AllowUserToDeleteRows = false;
            this.dgvCalibracaoAuto.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dgvCalibracaoAuto.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.dgvCalibracaoAuto.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCalibracaoAuto.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Volume,
            this.Tentativas,
            this.VolumeDos,
            this.Executado});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCalibracaoAuto.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCalibracaoAuto.Location = new System.Drawing.Point(262, 183);
            this.dgvCalibracaoAuto.MinimumSize = new System.Drawing.Size(400, 300);
            this.dgvCalibracaoAuto.MultiSelect = false;
            this.dgvCalibracaoAuto.Name = "dgvCalibracaoAuto";
            this.dgvCalibracaoAuto.ReadOnly = true;
            this.dgvCalibracaoAuto.RowHeadersVisible = false;
            this.dgvCalibracaoAuto.RowTemplate.Height = 40;
            this.dgvCalibracaoAuto.Size = new System.Drawing.Size(681, 442);
            this.dgvCalibracaoAuto.TabIndex = 422;
            this.dgvCalibracaoAuto.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvCalibracaoAuto_CellFormatting);
            // 
            // Volume
            // 
            this.Volume.DataPropertyName = "Volume";
            this.Volume.FillWeight = 200F;
            this.Volume.HeaderText = "Volume (ml)";
            this.Volume.Name = "Volume";
            this.Volume.ReadOnly = true;
            this.Volume.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Volume.Width = 200;
            // 
            // Tentativas
            // 
            this.Tentativas.DataPropertyName = "Tentativas";
            this.Tentativas.HeaderText = "Attempts";
            this.Tentativas.Name = "Tentativas";
            this.Tentativas.ReadOnly = true;
            // 
            // VolumeDos
            // 
            this.VolumeDos.DataPropertyName = "VolumeDos";
            this.VolumeDos.FillWeight = 200F;
            this.VolumeDos.HeaderText = "Volume Dispensed (ml)";
            this.VolumeDos.Name = "VolumeDos";
            this.VolumeDos.ReadOnly = true;
            this.VolumeDos.Width = 200;
            // 
            // Executado
            // 
            this.Executado.DataPropertyName = "Executado";
            this.Executado.FillWeight = 150F;
            this.Executado.HeaderText = "Executed";
            this.Executado.Name = "Executado";
            this.Executado.ReadOnly = true;
            this.Executado.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Executado.Width = 150;
            // 
            // txt_delay_seg_bal
            // 
            this.txt_delay_seg_bal.BorderColor = System.Drawing.Color.Gainsboro;
            this.txt_delay_seg_bal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_delay_seg_bal.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txt_delay_seg_bal.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.txt_delay_seg_bal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txt_delay_seg_bal.Location = new System.Drawing.Point(725, 143);
            this.txt_delay_seg_bal.MaxLength = 3;
            this.txt_delay_seg_bal.Name = "txt_delay_seg_bal";
            this.txt_delay_seg_bal.Size = new System.Drawing.Size(43, 30);
            this.txt_delay_seg_bal.TabIndex = 407;
            this.txt_delay_seg_bal.Text = "20";
            // 
            // fPrecisao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1218, 650);
            this.Controls.Add(this.dgvCalibracaoAuto);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.txt_delay_seg_bal);
            this.Controls.Add(this.lblDelayScale);
            this.Controls.Add(this.gbComunicacaoBalanca);
            this.Controls.Add(this.tableLayoutPanel12);
            this.Controls.Add(this.tableLayoutPanel11);
            this.Controls.Add(this.pnlBarraTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1024, 650);
            this.Name = "fPrecisao";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "fPrecisao";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fPrecisao_FormClosing);
            this.Load += new System.EventHandler(this.fPrecisao_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.gbComunicacaoBalanca.ResumeLayout(false);
            this.gbComunicacaoBalanca.PerformLayout();
            this.tableLayoutPanel12.ResumeLayout(false);
            this.tableLayoutPanel12.PerformLayout();
            this.tableLayoutPanel11.ResumeLayout(false);
            this.tableLayoutPanel11.PerformLayout();
            this.pnlBarraTitulo.ResumeLayout(false);
            this.pnlBarraTitulo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCalibracaoAuto)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripStatusLabel toolMessage;
        private System.Windows.Forms.Label lblTituloMassaBal;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolProgress;
        private Percolore.Core.UserControl.UTextBox txt_delay_seg_bal;
        private System.Windows.Forms.Label lblDelayScale;
        private System.Windows.Forms.Label lblMassaRecipiente;
        private System.Windows.Forms.GroupBox gbComunicacaoBalanca;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblCalibracaoAutoColorante;
        private System.Windows.Forms.Label lblCalibracaoAutoMotor;
        private System.Windows.Forms.Label lblCalibracaoAutoMassaEspecifica;
        private System.Windows.Forms.Label lblCalibracaoAutoLegendaMotor;
        private System.Windows.Forms.Label lblCalibracaoAutoLegendaMassaEspec;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel12;
        private System.Windows.Forms.Label lblMinMassaAdmRecipiente;
        private Percolore.Core.UserControl.UTextBox txtVolumeMaxRecipiente;
        private System.Windows.Forms.Label lblVolumeMaxRecipiente;
        private Percolore.Core.UserControl.UTextBox txtMassaAdmRecipiente;
        private System.Windows.Forms.Label lblMassaAdmRecipiente;
        private System.Windows.Forms.Label lblCapacidadeMaxBalanca;
        private Percolore.Core.UserControl.UTextBox txtCapacidadeMaxBalanca;
        private Percolore.Core.UserControl.UTextBox txtMinMassaAdmRecipiente;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel11;
        private System.Windows.Forms.Label lblAccurracyNameColorante;
        private System.Windows.Forms.Panel pnlBarraTitulo;
        private System.Windows.Forms.DataGridView dgvCalibracaoAuto;
        private System.Windows.Forms.DataGridViewTextBoxColumn Volume;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tentativas;
        private System.Windows.Forms.DataGridViewTextBoxColumn VolumeDos;
        private System.Windows.Forms.DataGridViewTextBoxColumn Executado;
    }
}