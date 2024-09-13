namespace Percolore.IOConnect
{
    partial class fCalibracaoAutomatica
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fCalibracaoAutomatica));
            this.pnlBarraTitulo = new System.Windows.Forms.Panel();
            this.btnSair = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.tableLayoutPanel11 = new System.Windows.Forms.TableLayoutPanel();
            this.lblCalibracaoAutoColorante = new System.Windows.Forms.Label();
            this.lblCalibracaoAutoMotor = new System.Windows.Forms.Label();
            this.cb_CalibracaoAuto = new System.Windows.Forms.ComboBox();
            this.lblCalibracaoAutoMassaEspecifica = new System.Windows.Forms.Label();
            this.lblCalibracaoAutoLegendaMotor = new System.Windows.Forms.Label();
            this.lblCalibracaoAutoLegendaMassaEspec = new System.Windows.Forms.Label();
            this.tableLayoutPanel12 = new System.Windows.Forms.TableLayoutPanel();
            this.lblMinMassaAdmRecipiente = new System.Windows.Forms.Label();
            this.lblTentativasRecipiente = new System.Windows.Forms.Label();
            this.txtVolumeMaxRecipiente = new Percolore.Core.UserControl.UTextBox();
            this.lblVolumeMaxRecipiente = new System.Windows.Forms.Label();
            this.txtMassaAdmRecipiente = new Percolore.Core.UserControl.UTextBox();
            this.lblMassaAdmRecipiente = new System.Windows.Forms.Label();
            this.lblCapacidadeMaxBalanca = new System.Windows.Forms.Label();
            this.txtCapacidadeMaxBalanca = new Percolore.Core.UserControl.UTextBox();
            this.txtTentativasRecipiente = new Percolore.Core.UserControl.UTextBox();
            this.txtMinMassaAdmRecipiente = new Percolore.Core.UserControl.UTextBox();
            this.dgvCalibracaoAuto = new System.Windows.Forms.DataGridView();
            this.Etapa = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Motor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Etapa_Tentativa = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Volume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VolumeDosado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MassaIdeal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MassaMedBalanca = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Desvio_Med = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Desvio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Aprovado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Executado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnStart = new System.Windows.Forms.Button();
            this.gbComunicacaoBalanca = new System.Windows.Forms.GroupBox();
            this.lblModelScale = new System.Windows.Forms.Label();
            this.cmbTipoBalanca = new System.Windows.Forms.ComboBox();
            this.lblSerialPort = new System.Windows.Forms.Label();
            this.cmbPortaSerial = new System.Windows.Forms.ComboBox();
            this.lblTituloMassaBal = new System.Windows.Forms.Label();
            this.lblMassaRecipiente = new System.Windows.Forms.Label();
            this.lblDelayBalanca = new System.Windows.Forms.Label();
            this.lblTotalizador = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.toolMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnParar = new System.Windows.Forms.Button();
            this.lblTipo = new System.Windows.Forms.Label();
            this.cmb_TipoCalibracao = new System.Windows.Forms.ComboBox();
            this.lblProcesso = new System.Windows.Forms.Label();
            this.cmb_ProcessoCalibracao = new System.Windows.Forms.ComboBox();
            this.txtTotalVolumeDos = new Percolore.Core.UserControl.UTextBox();
            this.txt_delay_seg_bal = new Percolore.Core.UserControl.UTextBox();
            this.txtTotalMassaMedBal = new Percolore.Core.UserControl.UTextBox();
            this.pnlBarraTitulo.SuspendLayout();
            this.tableLayoutPanel11.SuspendLayout();
            this.tableLayoutPanel12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCalibracaoAuto)).BeginInit();
            this.gbComunicacaoBalanca.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
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
            this.pnlBarraTitulo.Size = new System.Drawing.Size(1284, 64);
            this.pnlBarraTitulo.TabIndex = 375;
            // 
            // btnSair
            // 
            this.btnSair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSair.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(145)))), ((int)(((byte)(204)))));
            this.btnSair.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(138)))), ((int)(((byte)(228)))));
            this.btnSair.FlatAppearance.BorderSize = 0;
            this.btnSair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSair.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnSair.Location = new System.Drawing.Point(1220, 0);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(64, 64);
            this.btnSair.TabIndex = 1;
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
            this.lblTitulo.Size = new System.Drawing.Size(183, 23);
            this.lblTitulo.TabIndex = 309;
            this.lblTitulo.Text = "Calibração Automática";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel11
            // 
            this.tableLayoutPanel11.AutoSize = true;
            this.tableLayoutPanel11.ColumnCount = 6;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel11.Controls.Add(this.lblCalibracaoAutoColorante, 0, 0);
            this.tableLayoutPanel11.Controls.Add(this.lblCalibracaoAutoMotor, 3, 0);
            this.tableLayoutPanel11.Controls.Add(this.cb_CalibracaoAuto, 1, 0);
            this.tableLayoutPanel11.Controls.Add(this.lblCalibracaoAutoMassaEspecifica, 5, 0);
            this.tableLayoutPanel11.Controls.Add(this.lblCalibracaoAutoLegendaMotor, 2, 0);
            this.tableLayoutPanel11.Controls.Add(this.lblCalibracaoAutoLegendaMassaEspec, 4, 0);
            this.tableLayoutPanel11.Location = new System.Drawing.Point(560, 68);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 1;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel11.Size = new System.Drawing.Size(632, 55);
            this.tableLayoutPanel11.TabIndex = 377;
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
            this.lblCalibracaoAutoMotor.Location = new System.Drawing.Point(309, 3);
            this.lblCalibracaoAutoMotor.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.lblCalibracaoAutoMotor.Name = "lblCalibracaoAutoMotor";
            this.lblCalibracaoAutoMotor.Size = new System.Drawing.Size(48, 49);
            this.lblCalibracaoAutoMotor.TabIndex = 365;
            this.lblCalibracaoAutoMotor.Text = "00";
            this.lblCalibracaoAutoMotor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cb_CalibracaoAuto
            // 
            this.cb_CalibracaoAuto.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cb_CalibracaoAuto.DropDownHeight = 600;
            this.cb_CalibracaoAuto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_CalibracaoAuto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cb_CalibracaoAuto.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.cb_CalibracaoAuto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.cb_CalibracaoAuto.FormattingEnabled = true;
            this.cb_CalibracaoAuto.IntegralHeight = false;
            this.cb_CalibracaoAuto.ItemHeight = 40;
            this.cb_CalibracaoAuto.Location = new System.Drawing.Point(94, 3);
            this.cb_CalibracaoAuto.Name = "cb_CalibracaoAuto";
            this.cb_CalibracaoAuto.Size = new System.Drawing.Size(146, 48);
            this.cb_CalibracaoAuto.TabIndex = 0;
            this.cb_CalibracaoAuto.SelectionChangeCommitted += new System.EventHandler(this.cb_CalibracaoAuto_SelectionChangeCommitted);
            // 
            // lblCalibracaoAutoMassaEspecifica
            // 
            this.lblCalibracaoAutoMassaEspecifica.AutoSize = true;
            this.lblCalibracaoAutoMassaEspecifica.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCalibracaoAutoMassaEspecifica.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.lblCalibracaoAutoMassaEspecifica.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblCalibracaoAutoMassaEspecifica.Location = new System.Drawing.Point(502, 3);
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
            this.lblCalibracaoAutoLegendaMotor.Location = new System.Drawing.Point(249, 3);
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
            this.lblCalibracaoAutoLegendaMassaEspec.Location = new System.Drawing.Point(366, 3);
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
            this.tableLayoutPanel12.Controls.Add(this.lblTentativasRecipiente, 0, 3);
            this.tableLayoutPanel12.Controls.Add(this.txtVolumeMaxRecipiente, 1, 2);
            this.tableLayoutPanel12.Controls.Add(this.lblVolumeMaxRecipiente, 0, 2);
            this.tableLayoutPanel12.Controls.Add(this.txtMassaAdmRecipiente, 1, 1);
            this.tableLayoutPanel12.Controls.Add(this.lblMassaAdmRecipiente, 0, 1);
            this.tableLayoutPanel12.Controls.Add(this.lblCapacidadeMaxBalanca, 0, 0);
            this.tableLayoutPanel12.Controls.Add(this.txtCapacidadeMaxBalanca, 1, 0);
            this.tableLayoutPanel12.Controls.Add(this.txtTentativasRecipiente, 1, 3);
            this.tableLayoutPanel12.Controls.Add(this.txtMinMassaAdmRecipiente, 1, 4);
            this.tableLayoutPanel12.Location = new System.Drawing.Point(12, 67);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            this.tableLayoutPanel12.RowCount = 5;
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel12.Size = new System.Drawing.Size(539, 142);
            this.tableLayoutPanel12.TabIndex = 378;
            // 
            // lblMinMassaAdmRecipiente
            // 
            this.lblMinMassaAdmRecipiente.AutoSize = true;
            this.lblMinMassaAdmRecipiente.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMinMassaAdmRecipiente.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblMinMassaAdmRecipiente.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblMinMassaAdmRecipiente.Location = new System.Drawing.Point(3, 108);
            this.lblMinMassaAdmRecipiente.Name = "lblMinMassaAdmRecipiente";
            this.lblMinMassaAdmRecipiente.Size = new System.Drawing.Size(371, 36);
            this.lblMinMassaAdmRecipiente.TabIndex = 8;
            this.lblMinMassaAdmRecipiente.Text = "Peso do recipiente (g):";
            // 
            // lblTentativasRecipiente
            // 
            this.lblTentativasRecipiente.AutoSize = true;
            this.lblTentativasRecipiente.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTentativasRecipiente.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblTentativasRecipiente.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblTentativasRecipiente.Location = new System.Drawing.Point(3, 72);
            this.lblTentativasRecipiente.Name = "lblTentativasRecipiente";
            this.lblTentativasRecipiente.Size = new System.Drawing.Size(371, 36);
            this.lblTentativasRecipiente.TabIndex = 6;
            this.lblTentativasRecipiente.Text = "Numero Tentativas Posicionamento do Recipiente:";
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
            // txtTentativasRecipiente
            // 
            this.txtTentativasRecipiente.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtTentativasRecipiente.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTentativasRecipiente.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtTentativasRecipiente.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTentativasRecipiente.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.txtTentativasRecipiente.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtTentativasRecipiente.Location = new System.Drawing.Point(380, 75);
            this.txtTentativasRecipiente.Name = "txtTentativasRecipiente";
            this.txtTentativasRecipiente.ReadOnly = true;
            this.txtTentativasRecipiente.Size = new System.Drawing.Size(156, 30);
            this.txtTentativasRecipiente.TabIndex = 7;
            // 
            // txtMinMassaAdmRecipiente
            // 
            this.txtMinMassaAdmRecipiente.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtMinMassaAdmRecipiente.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMinMassaAdmRecipiente.Conteudo = Percolore.Core.UserControl.TipoConteudo.Decimal;
            this.txtMinMassaAdmRecipiente.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.txtMinMassaAdmRecipiente.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtMinMassaAdmRecipiente.Location = new System.Drawing.Point(380, 111);
            this.txtMinMassaAdmRecipiente.Name = "txtMinMassaAdmRecipiente";
            this.txtMinMassaAdmRecipiente.ReadOnly = true;
            this.txtMinMassaAdmRecipiente.Size = new System.Drawing.Size(156, 30);
            this.txtMinMassaAdmRecipiente.TabIndex = 9;
            // 
            // dgvCalibracaoAuto
            // 
            this.dgvCalibracaoAuto.AllowUserToAddRows = false;
            this.dgvCalibracaoAuto.AllowUserToDeleteRows = false;
            this.dgvCalibracaoAuto.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dgvCalibracaoAuto.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.dgvCalibracaoAuto.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCalibracaoAuto.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Etapa,
            this.Motor,
            this.Etapa_Tentativa,
            this.Volume,
            this.VolumeDosado,
            this.MassaIdeal,
            this.MassaMedBalanca,
            this.Desvio_Med,
            this.Desvio,
            this.Aprovado,
            this.Executado});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCalibracaoAuto.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCalibracaoAuto.Location = new System.Drawing.Point(4, 270);
            this.dgvCalibracaoAuto.MinimumSize = new System.Drawing.Size(1024, 300);
            this.dgvCalibracaoAuto.Name = "dgvCalibracaoAuto";
            this.dgvCalibracaoAuto.ReadOnly = true;
            this.dgvCalibracaoAuto.RowHeadersVisible = false;
            this.dgvCalibracaoAuto.RowTemplate.Height = 40;
            this.dgvCalibracaoAuto.Size = new System.Drawing.Size(1245, 333);
            this.dgvCalibracaoAuto.TabIndex = 379;
            this.dgvCalibracaoAuto.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvCalibracaoAuto_CellFormatting);
            // 
            // Etapa
            // 
            this.Etapa.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Etapa.DataPropertyName = "Etapa";
            this.Etapa.Frozen = true;
            this.Etapa.HeaderText = "Etapa";
            this.Etapa.Name = "Etapa";
            this.Etapa.ReadOnly = true;
            this.Etapa.Width = 300;
            // 
            // Motor
            // 
            this.Motor.DataPropertyName = "Motor";
            this.Motor.HeaderText = "Motor";
            this.Motor.Name = "Motor";
            this.Motor.ReadOnly = true;
            this.Motor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Motor.Visible = false;
            // 
            // Etapa_Tentativa
            // 
            this.Etapa_Tentativa.DataPropertyName = "Etapa_Tentativa";
            this.Etapa_Tentativa.FillWeight = 60F;
            this.Etapa_Tentativa.HeaderText = "Tentativa";
            this.Etapa_Tentativa.Name = "Etapa_Tentativa";
            this.Etapa_Tentativa.ReadOnly = true;
            this.Etapa_Tentativa.Width = 60;
            // 
            // Volume
            // 
            this.Volume.DataPropertyName = "Volume";
            this.Volume.HeaderText = "Volume (ml)";
            this.Volume.Name = "Volume";
            this.Volume.ReadOnly = true;
            this.Volume.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // VolumeDosado
            // 
            this.VolumeDosado.DataPropertyName = "VolumeDosado";
            this.VolumeDosado.HeaderText = "Volume Dosado(ml)";
            this.VolumeDosado.Name = "VolumeDosado";
            this.VolumeDosado.ReadOnly = true;
            // 
            // MassaIdeal
            // 
            this.MassaIdeal.DataPropertyName = "MassaIdeal";
            this.MassaIdeal.FillWeight = 120F;
            this.MassaIdeal.HeaderText = "Massa Ideal (g)";
            this.MassaIdeal.Name = "MassaIdeal";
            this.MassaIdeal.ReadOnly = true;
            this.MassaIdeal.Width = 120;
            // 
            // MassaMedBalanca
            // 
            this.MassaMedBalanca.DataPropertyName = "MassaMedBalanca";
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.MassaMedBalanca.DefaultCellStyle = dataGridViewCellStyle1;
            this.MassaMedBalanca.FillWeight = 120F;
            this.MassaMedBalanca.HeaderText = "Massa Medida Balanca (g)";
            this.MassaMedBalanca.Name = "MassaMedBalanca";
            this.MassaMedBalanca.ReadOnly = true;
            this.MassaMedBalanca.Width = 120;
            // 
            // Desvio_Med
            // 
            this.Desvio_Med.DataPropertyName = "Desvio_Med";
            this.Desvio_Med.FillWeight = 80F;
            this.Desvio_Med.HeaderText = "Desvio (%)";
            this.Desvio_Med.Name = "Desvio_Med";
            this.Desvio_Med.ReadOnly = true;
            this.Desvio_Med.Width = 80;
            // 
            // Desvio
            // 
            this.Desvio.DataPropertyName = "Desvio";
            this.Desvio.FillWeight = 80F;
            this.Desvio.HeaderText = "Desvio Admissivel (%)";
            this.Desvio.Name = "Desvio";
            this.Desvio.ReadOnly = true;
            this.Desvio.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Desvio.Width = 80;
            // 
            // Aprovado
            // 
            this.Aprovado.DataPropertyName = "Aprovado";
            this.Aprovado.FillWeight = 120F;
            this.Aprovado.HeaderText = "Aprovado";
            this.Aprovado.Name = "Aprovado";
            this.Aprovado.ReadOnly = true;
            this.Aprovado.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Aprovado.Width = 120;
            // 
            // Executado
            // 
            this.Executado.DataPropertyName = "Executado";
            this.Executado.FillWeight = 80F;
            this.Executado.HeaderText = "Executado";
            this.Executado.Name = "Executado";
            this.Executado.ReadOnly = true;
            this.Executado.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Executado.Width = 80;
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(145)))), ((int)(((byte)(204)))));
            this.btnStart.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.btnStart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnStart.Location = new System.Drawing.Point(1084, 134);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(165, 53);
            this.btnStart.TabIndex = 380;
            this.btnStart.Text = "Iniciar";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // gbComunicacaoBalanca
            // 
            this.gbComunicacaoBalanca.Controls.Add(this.lblModelScale);
            this.gbComunicacaoBalanca.Controls.Add(this.cmbTipoBalanca);
            this.gbComunicacaoBalanca.Controls.Add(this.lblSerialPort);
            this.gbComunicacaoBalanca.Controls.Add(this.cmbPortaSerial);
            this.gbComunicacaoBalanca.Controls.Add(this.lblTituloMassaBal);
            this.gbComunicacaoBalanca.Controls.Add(this.lblMassaRecipiente);
            this.gbComunicacaoBalanca.Location = new System.Drawing.Point(850, 126);
            this.gbComunicacaoBalanca.Name = "gbComunicacaoBalanca";
            this.gbComunicacaoBalanca.Size = new System.Drawing.Size(228, 118);
            this.gbComunicacaoBalanca.TabIndex = 389;
            this.gbComunicacaoBalanca.TabStop = false;
            this.gbComunicacaoBalanca.Text = "Comunicação Balança";
            this.gbComunicacaoBalanca.Visible = false;
            // 
            // lblModelScale
            // 
            this.lblModelScale.AutoSize = true;
            this.lblModelScale.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblModelScale.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblModelScale.Location = new System.Drawing.Point(9, 23);
            this.lblModelScale.Name = "lblModelScale";
            this.lblModelScale.Size = new System.Drawing.Size(70, 23);
            this.lblModelScale.TabIndex = 397;
            this.lblModelScale.Text = "Modelo:";
            // 
            // cmbTipoBalanca
            // 
            this.cmbTipoBalanca.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoBalanca.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.cmbTipoBalanca.FormattingEnabled = true;
            this.cmbTipoBalanca.Location = new System.Drawing.Point(101, 17);
            this.cmbTipoBalanca.Name = "cmbTipoBalanca";
            this.cmbTipoBalanca.Size = new System.Drawing.Size(121, 31);
            this.cmbTipoBalanca.TabIndex = 396;
            this.cmbTipoBalanca.SelectedIndexChanged += new System.EventHandler(this.cmbTipoBalanca_SelectedIndexChanged);
            // 
            // lblSerialPort
            // 
            this.lblSerialPort.AutoSize = true;
            this.lblSerialPort.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblSerialPort.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblSerialPort.Location = new System.Drawing.Point(9, 58);
            this.lblSerialPort.Name = "lblSerialPort";
            this.lblSerialPort.Size = new System.Drawing.Size(86, 23);
            this.lblSerialPort.TabIndex = 395;
            this.lblSerialPort.Text = "Serial Port:";
            // 
            // cmbPortaSerial
            // 
            this.cmbPortaSerial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPortaSerial.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.cmbPortaSerial.FormattingEnabled = true;
            this.cmbPortaSerial.Location = new System.Drawing.Point(101, 52);
            this.cmbPortaSerial.Name = "cmbPortaSerial";
            this.cmbPortaSerial.Size = new System.Drawing.Size(121, 31);
            this.cmbPortaSerial.TabIndex = 0;
            this.cmbPortaSerial.SelectedIndexChanged += new System.EventHandler(this.cmbPortaSerial_SelectedIndexChanged);
            // 
            // lblTituloMassaBal
            // 
            this.lblTituloMassaBal.AutoSize = true;
            this.lblTituloMassaBal.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblTituloMassaBal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblTituloMassaBal.Location = new System.Drawing.Point(9, 89);
            this.lblTituloMassaBal.Name = "lblTituloMassaBal";
            this.lblTituloMassaBal.Size = new System.Drawing.Size(136, 23);
            this.lblTituloMassaBal.TabIndex = 1;
            this.lblTituloMassaBal.Text = "Massa Balança(g):";
            // 
            // lblMassaRecipiente
            // 
            this.lblMassaRecipiente.AutoSize = true;
            this.lblMassaRecipiente.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblMassaRecipiente.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblMassaRecipiente.Location = new System.Drawing.Point(151, 89);
            this.lblMassaRecipiente.Name = "lblMassaRecipiente";
            this.lblMassaRecipiente.Size = new System.Drawing.Size(19, 23);
            this.lblMassaRecipiente.TabIndex = 2;
            this.lblMassaRecipiente.Text = "0";
            // 
            // lblDelayBalanca
            // 
            this.lblDelayBalanca.AutoSize = true;
            this.lblDelayBalanca.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblDelayBalanca.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblDelayBalanca.Location = new System.Drawing.Point(556, 214);
            this.lblDelayBalanca.Name = "lblDelayBalanca";
            this.lblDelayBalanca.Size = new System.Drawing.Size(152, 23);
            this.lblDelayBalanca.TabIndex = 393;
            this.lblDelayBalanca.Text = "Delay Balança (seg):";
            // 
            // lblTotalizador
            // 
            this.lblTotalizador.AutoSize = true;
            this.lblTotalizador.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblTotalizador.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblTotalizador.Location = new System.Drawing.Point(345, 606);
            this.lblTotalizador.Name = "lblTotalizador";
            this.lblTotalizador.Size = new System.Drawing.Size(92, 23);
            this.lblTotalizador.TabIndex = 390;
            this.lblTotalizador.Text = "Totalizador:";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolProgress,
            this.toolMessage});
            this.statusStrip1.Location = new System.Drawing.Point(0, 636);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1284, 22);
            this.statusStrip1.TabIndex = 395;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolProgress
            // 
            this.toolProgress.Name = "toolProgress";
            this.toolProgress.Size = new System.Drawing.Size(400, 16);
            this.toolProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // toolMessage
            // 
            this.toolMessage.Name = "toolMessage";
            this.toolMessage.Size = new System.Drawing.Size(163, 17);
            this.toolMessage.Text = "Status Calibração Automática";
            // 
            // btnParar
            // 
            this.btnParar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(145)))), ((int)(((byte)(204)))));
            this.btnParar.Enabled = false;
            this.btnParar.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.btnParar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnParar.Location = new System.Drawing.Point(1084, 191);
            this.btnParar.Name = "btnParar";
            this.btnParar.Size = new System.Drawing.Size(165, 53);
            this.btnParar.TabIndex = 396;
            this.btnParar.Text = "Parar";
            this.btnParar.UseVisualStyleBackColor = false;
            this.btnParar.Click += new System.EventHandler(this.btnParar_Click);
            // 
            // lblTipo
            // 
            this.lblTipo.AutoSize = true;
            this.lblTipo.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblTipo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblTipo.Location = new System.Drawing.Point(559, 175);
            this.lblTipo.Name = "lblTipo";
            this.lblTipo.Size = new System.Drawing.Size(89, 23);
            this.lblTipo.TabIndex = 397;
            this.lblTipo.Text = "Calibração:";
            // 
            // cmb_TipoCalibracao
            // 
            this.cmb_TipoCalibracao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_TipoCalibracao.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.cmb_TipoCalibracao.FormattingEnabled = true;
            this.cmb_TipoCalibracao.Location = new System.Drawing.Point(654, 172);
            this.cmb_TipoCalibracao.Name = "cmb_TipoCalibracao";
            this.cmb_TipoCalibracao.Size = new System.Drawing.Size(152, 31);
            this.cmb_TipoCalibracao.TabIndex = 398;
            this.cmb_TipoCalibracao.SelectedIndexChanged += new System.EventHandler(this.cmb_TipoCalibracao_SelectedIndexChanged);
            // 
            // lblProcesso
            // 
            this.lblProcesso.AutoSize = true;
            this.lblProcesso.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblProcesso.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblProcesso.Location = new System.Drawing.Point(563, 139);
            this.lblProcesso.Name = "lblProcesso";
            this.lblProcesso.Size = new System.Drawing.Size(80, 23);
            this.lblProcesso.TabIndex = 399;
            this.lblProcesso.Text = "Processo:";
            // 
            // cmb_ProcessoCalibracao
            // 
            this.cmb_ProcessoCalibracao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_ProcessoCalibracao.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.cmb_ProcessoCalibracao.FormattingEnabled = true;
            this.cmb_ProcessoCalibracao.Location = new System.Drawing.Point(654, 136);
            this.cmb_ProcessoCalibracao.Name = "cmb_ProcessoCalibracao";
            this.cmb_ProcessoCalibracao.Size = new System.Drawing.Size(152, 31);
            this.cmb_ProcessoCalibracao.TabIndex = 400;
            this.cmb_ProcessoCalibracao.SelectedIndexChanged += new System.EventHandler(this.cmb_ProcessoCalibracao_SelectedIndexChanged);
            // 
            // txtTotalVolumeDos
            // 
            this.txtTotalVolumeDos.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtTotalVolumeDos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTotalVolumeDos.Conteudo = Percolore.Core.UserControl.TipoConteudo.Decimal;
            this.txtTotalVolumeDos.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.txtTotalVolumeDos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtTotalVolumeDos.Location = new System.Drawing.Point(466, 604);
            this.txtTotalVolumeDos.Name = "txtTotalVolumeDos";
            this.txtTotalVolumeDos.Size = new System.Drawing.Size(100, 30);
            this.txtTotalVolumeDos.TabIndex = 392;
            // 
            // txt_delay_seg_bal
            // 
            this.txt_delay_seg_bal.BorderColor = System.Drawing.Color.Gainsboro;
            this.txt_delay_seg_bal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_delay_seg_bal.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txt_delay_seg_bal.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.txt_delay_seg_bal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txt_delay_seg_bal.Location = new System.Drawing.Point(726, 212);
            this.txt_delay_seg_bal.MaxLength = 3;
            this.txt_delay_seg_bal.Name = "txt_delay_seg_bal";
            this.txt_delay_seg_bal.Size = new System.Drawing.Size(43, 30);
            this.txt_delay_seg_bal.TabIndex = 394;
            this.txt_delay_seg_bal.Text = "20";
            // 
            // txtTotalMassaMedBal
            // 
            this.txtTotalMassaMedBal.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtTotalMassaMedBal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTotalMassaMedBal.Conteudo = Percolore.Core.UserControl.TipoConteudo.Decimal;
            this.txtTotalMassaMedBal.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.txtTotalMassaMedBal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtTotalMassaMedBal.Location = new System.Drawing.Point(690, 604);
            this.txtTotalMassaMedBal.Name = "txtTotalMassaMedBal";
            this.txtTotalMassaMedBal.Size = new System.Drawing.Size(110, 30);
            this.txtTotalMassaMedBal.TabIndex = 391;
            // 
            // fCalibracaoAutomatica
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 658);
            this.Controls.Add(this.cmb_ProcessoCalibracao);
            this.Controls.Add(this.lblProcesso);
            this.Controls.Add(this.cmb_TipoCalibracao);
            this.Controls.Add(this.lblTipo);
            this.Controls.Add(this.btnParar);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.txtTotalVolumeDos);
            this.Controls.Add(this.txt_delay_seg_bal);
            this.Controls.Add(this.lblDelayBalanca);
            this.Controls.Add(this.txtTotalMassaMedBal);
            this.Controls.Add(this.lblTotalizador);
            this.Controls.Add(this.gbComunicacaoBalanca);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.dgvCalibracaoAuto);
            this.Controls.Add(this.tableLayoutPanel12);
            this.Controls.Add(this.tableLayoutPanel11);
            this.Controls.Add(this.pnlBarraTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1024, 650);
            this.Name = "fCalibracaoAutomatica";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "fCalibracaoAuomatica";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fCalibracaoAutomatica_FormClosing);
            this.Load += new System.EventHandler(this.fCalibracaoAutomatica_Load);
            this.pnlBarraTitulo.ResumeLayout(false);
            this.pnlBarraTitulo.PerformLayout();
            this.tableLayoutPanel11.ResumeLayout(false);
            this.tableLayoutPanel11.PerformLayout();
            this.tableLayoutPanel12.ResumeLayout(false);
            this.tableLayoutPanel12.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCalibracaoAuto)).EndInit();
            this.gbComunicacaoBalanca.ResumeLayout(false);
            this.gbComunicacaoBalanca.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlBarraTitulo;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel11;
        private System.Windows.Forms.Label lblCalibracaoAutoColorante;
        private System.Windows.Forms.Label lblCalibracaoAutoMotor;
        private System.Windows.Forms.ComboBox cb_CalibracaoAuto;
        private System.Windows.Forms.Label lblCalibracaoAutoMassaEspecifica;
        private System.Windows.Forms.Label lblCalibracaoAutoLegendaMotor;
        private System.Windows.Forms.Label lblCalibracaoAutoLegendaMassaEspec;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel12;
        private System.Windows.Forms.Label lblTentativasRecipiente;
        private Percolore.Core.UserControl.UTextBox txtVolumeMaxRecipiente;
        private System.Windows.Forms.Label lblVolumeMaxRecipiente;
        private Percolore.Core.UserControl.UTextBox txtMassaAdmRecipiente;
        private System.Windows.Forms.Label lblMassaAdmRecipiente;
        private System.Windows.Forms.Label lblCapacidadeMaxBalanca;
        private Percolore.Core.UserControl.UTextBox txtCapacidadeMaxBalanca;
        private Percolore.Core.UserControl.UTextBox txtTentativasRecipiente;
        private System.Windows.Forms.DataGridView dgvCalibracaoAuto;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox gbComunicacaoBalanca;
        private System.Windows.Forms.ComboBox cmbPortaSerial;
        private System.Windows.Forms.Label lblMassaRecipiente;
        private System.Windows.Forms.Label lblTituloMassaBal;
        private System.Windows.Forms.Label lblTotalizador;
        private Percolore.Core.UserControl.UTextBox txtTotalMassaMedBal;
        private Percolore.Core.UserControl.UTextBox txtTotalVolumeDos;
        private System.Windows.Forms.Label lblDelayBalanca;
        private Percolore.Core.UserControl.UTextBox txt_delay_seg_bal;
        private System.Windows.Forms.Label lblMinMassaAdmRecipiente;
        private Percolore.Core.UserControl.UTextBox txtMinMassaAdmRecipiente;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolProgress;
        private System.Windows.Forms.ToolStripStatusLabel toolMessage;
        private System.Windows.Forms.DataGridViewComboBoxColumn Etapa;
        private System.Windows.Forms.DataGridViewTextBoxColumn Motor;
        private System.Windows.Forms.DataGridViewTextBoxColumn Etapa_Tentativa;
        private System.Windows.Forms.DataGridViewTextBoxColumn Volume;
        private System.Windows.Forms.DataGridViewTextBoxColumn VolumeDosado;
        private System.Windows.Forms.DataGridViewTextBoxColumn MassaIdeal;
        private System.Windows.Forms.DataGridViewTextBoxColumn MassaMedBalanca;
        private System.Windows.Forms.DataGridViewTextBoxColumn Desvio_Med;
        private System.Windows.Forms.DataGridViewTextBoxColumn Desvio;
        private System.Windows.Forms.DataGridViewTextBoxColumn Aprovado;
        private System.Windows.Forms.DataGridViewTextBoxColumn Executado;
        private System.Windows.Forms.Button btnParar;
        private System.Windows.Forms.Label lblSerialPort;
        private System.Windows.Forms.Label lblModelScale;
        private System.Windows.Forms.ComboBox cmbTipoBalanca;
        private System.Windows.Forms.Label lblTipo;
        private System.Windows.Forms.ComboBox cmb_TipoCalibracao;
        private System.Windows.Forms.Label lblProcesso;
        private System.Windows.Forms.ComboBox cmb_ProcessoCalibracao;
    }
}