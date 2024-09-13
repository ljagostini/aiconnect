namespace Percolore.IOConnect
{
    partial class fPrimeiraPesagem
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
            this.rd_pulsos = new System.Windows.Forms.RadioButton();
            this.btnPesagemConfirmar = new System.Windows.Forms.Button();
            this.lblDesvioMedio = new System.Windows.Forms.Label();
            this.txtPesagemDesvio = new Percolore.Core.UserControl.UTextBox();
            this.lblPesagemDesvio = new System.Windows.Forms.Label();
            this.listViewPesagem = new System.Windows.Forms.ListView();
            this.lblPesagemLegendaMassaIdeal = new System.Windows.Forms.Label();
            this.btnPesagemAdicionar = new System.Windows.Forms.Button();
            this.btnPesagemDispensar = new System.Windows.Forms.Button();
            this.lblPesagemMassa = new System.Windows.Forms.Label();
            this.lblMassaMedia = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblPesagemLegendaVolume = new System.Windows.Forms.Label();
            this.txtPesagemMassaVerificada = new Percolore.Core.UserControl.UTextBox();
            this.lblPesagemMassaIdeal = new System.Windows.Forms.Label();
            this.txtPesagemVolume = new Percolore.Core.UserControl.UTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chb_balanca_Pesagem = new System.Windows.Forms.CheckBox();
            this.gbComunicacaoBalanca = new System.Windows.Forms.GroupBox();
            this.cmbPortaSerial = new System.Windows.Forms.ComboBox();
            this.gbComunicacaoBalanca.SuspendLayout();
            this.SuspendLayout();
            // 
            // rd_pulsos
            // 
            this.rd_pulsos.AutoSize = true;
            this.rd_pulsos.Checked = true;
            this.rd_pulsos.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.rd_pulsos.Location = new System.Drawing.Point(39, 324);
            this.rd_pulsos.Name = "rd_pulsos";
            this.rd_pulsos.Size = new System.Drawing.Size(142, 27);
            this.rd_pulsos.TabIndex = 382;
            this.rd_pulsos.TabStop = true;
            this.rd_pulsos.Text = "Redefinir pulsos";
            this.rd_pulsos.UseVisualStyleBackColor = true;
            // 
            // btnPesagemConfirmar
            // 
            this.btnPesagemConfirmar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.btnPesagemConfirmar.Enabled = false;
            this.btnPesagemConfirmar.FlatAppearance.BorderSize = 0;
            this.btnPesagemConfirmar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPesagemConfirmar.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnPesagemConfirmar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnPesagemConfirmar.Location = new System.Drawing.Point(39, 249);
            this.btnPesagemConfirmar.Name = "btnPesagemConfirmar";
            this.btnPesagemConfirmar.Size = new System.Drawing.Size(146, 52);
            this.btnPesagemConfirmar.TabIndex = 371;
            this.btnPesagemConfirmar.Text = "Confirmar";
            this.btnPesagemConfirmar.UseVisualStyleBackColor = false;
            this.btnPesagemConfirmar.Click += new System.EventHandler(this.btnConfirmar_Click);
            // 
            // lblDesvioMedio
            // 
            this.lblDesvioMedio.Font = new System.Drawing.Font("Segoe UI Light", 25F);
            this.lblDesvioMedio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblDesvioMedio.Location = new System.Drawing.Point(421, 440);
            this.lblDesvioMedio.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblDesvioMedio.Name = "lblDesvioMedio";
            this.lblDesvioMedio.Size = new System.Drawing.Size(173, 46);
            this.lblDesvioMedio.TabIndex = 381;
            this.lblDesvioMedio.Text = "0";
            this.lblDesvioMedio.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPesagemDesvio
            // 
            this.txtPesagemDesvio.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtPesagemDesvio.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPesagemDesvio.Conteudo = Percolore.Core.UserControl.TipoConteudo.Decimal;
            this.txtPesagemDesvio.Enabled = false;
            this.txtPesagemDesvio.Font = new System.Drawing.Font("Segoe UI Light", 25F);
            this.txtPesagemDesvio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtPesagemDesvio.Location = new System.Drawing.Point(421, 131);
            this.txtPesagemDesvio.Name = "txtPesagemDesvio";
            this.txtPesagemDesvio.Size = new System.Drawing.Size(164, 52);
            this.txtPesagemDesvio.TabIndex = 369;
            this.txtPesagemDesvio.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblPesagemDesvio
            // 
            this.lblPesagemDesvio.AutoSize = true;
            this.lblPesagemDesvio.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblPesagemDesvio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblPesagemDesvio.Location = new System.Drawing.Point(417, 102);
            this.lblPesagemDesvio.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblPesagemDesvio.Name = "lblPesagemDesvio";
            this.lblPesagemDesvio.Size = new System.Drawing.Size(58, 23);
            this.lblPesagemDesvio.TabIndex = 380;
            this.lblPesagemDesvio.Text = "Desvio";
            // 
            // listViewPesagem
            // 
            this.listViewPesagem.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewPesagem.Font = new System.Drawing.Font("Segoe UI Light", 24F);
            this.listViewPesagem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.listViewPesagem.FullRowSelect = true;
            this.listViewPesagem.GridLines = true;
            this.listViewPesagem.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewPesagem.LabelWrap = false;
            this.listViewPesagem.Location = new System.Drawing.Point(249, 189);
            this.listViewPesagem.MultiSelect = false;
            this.listViewPesagem.Name = "listViewPesagem";
            this.listViewPesagem.Size = new System.Drawing.Size(336, 250);
            this.listViewPesagem.TabIndex = 372;
            this.listViewPesagem.UseCompatibleStateImageBehavior = false;
            this.listViewPesagem.View = System.Windows.Forms.View.Details;
            this.listViewPesagem.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            // 
            // lblPesagemLegendaMassaIdeal
            // 
            this.lblPesagemLegendaMassaIdeal.AutoSize = true;
            this.lblPesagemLegendaMassaIdeal.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblPesagemLegendaMassaIdeal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblPesagemLegendaMassaIdeal.Location = new System.Drawing.Point(344, 12);
            this.lblPesagemLegendaMassaIdeal.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblPesagemLegendaMassaIdeal.Name = "lblPesagemLegendaMassaIdeal";
            this.lblPesagemLegendaMassaIdeal.Size = new System.Drawing.Size(92, 23);
            this.lblPesagemLegendaMassaIdeal.TabIndex = 378;
            this.lblPesagemLegendaMassaIdeal.Text = "Massa ideal";
            // 
            // btnPesagemAdicionar
            // 
            this.btnPesagemAdicionar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.btnPesagemAdicionar.FlatAppearance.BorderSize = 0;
            this.btnPesagemAdicionar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPesagemAdicionar.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnPesagemAdicionar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnPesagemAdicionar.Location = new System.Drawing.Point(39, 189);
            this.btnPesagemAdicionar.Name = "btnPesagemAdicionar";
            this.btnPesagemAdicionar.Size = new System.Drawing.Size(146, 52);
            this.btnPesagemAdicionar.TabIndex = 370;
            this.btnPesagemAdicionar.Text = "Adicionar";
            this.btnPesagemAdicionar.UseVisualStyleBackColor = false;
            this.btnPesagemAdicionar.Click += new System.EventHandler(this.btnAdicionar_Click);
            // 
            // btnPesagemDispensar
            // 
            this.btnPesagemDispensar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.btnPesagemDispensar.FlatAppearance.BorderSize = 0;
            this.btnPesagemDispensar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPesagemDispensar.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnPesagemDispensar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnPesagemDispensar.Location = new System.Drawing.Point(39, 129);
            this.btnPesagemDispensar.Name = "btnPesagemDispensar";
            this.btnPesagemDispensar.Size = new System.Drawing.Size(146, 52);
            this.btnPesagemDispensar.TabIndex = 367;
            this.btnPesagemDispensar.Text = "Dispensar";
            this.btnPesagemDispensar.UseVisualStyleBackColor = false;
            this.btnPesagemDispensar.Click += new System.EventHandler(this.btnDispensar_Click);
            // 
            // lblPesagemMassa
            // 
            this.lblPesagemMassa.AutoSize = true;
            this.lblPesagemMassa.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblPesagemMassa.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblPesagemMassa.Location = new System.Drawing.Point(253, 102);
            this.lblPesagemMassa.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblPesagemMassa.Name = "lblPesagemMassa";
            this.lblPesagemMassa.Size = new System.Drawing.Size(79, 23);
            this.lblPesagemMassa.TabIndex = 376;
            this.lblPesagemMassa.Text = "Massa (g)";
            // 
            // lblMassaMedia
            // 
            this.lblMassaMedia.Font = new System.Drawing.Font("Segoe UI Light", 25F);
            this.lblMassaMedia.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblMassaMedia.Location = new System.Drawing.Point(249, 442);
            this.lblMassaMedia.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblMassaMedia.Name = "lblMassaMedia";
            this.lblMassaMedia.Size = new System.Drawing.Size(173, 46);
            this.lblMassaMedia.TabIndex = 375;
            this.lblMassaMedia.Text = "0";
            this.lblMassaMedia.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Gainsboro;
            this.panel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.panel1.Location = new System.Drawing.Point(24, 92);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(560, 2);
            this.panel1.TabIndex = 374;
            // 
            // lblPesagemLegendaVolume
            // 
            this.lblPesagemLegendaVolume.AutoSize = true;
            this.lblPesagemLegendaVolume.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblPesagemLegendaVolume.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblPesagemLegendaVolume.Location = new System.Drawing.Point(20, 12);
            this.lblPesagemLegendaVolume.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblPesagemLegendaVolume.Name = "lblPesagemLegendaVolume";
            this.lblPesagemLegendaVolume.Size = new System.Drawing.Size(63, 23);
            this.lblPesagemLegendaVolume.TabIndex = 373;
            this.lblPesagemLegendaVolume.Text = "Volume";
            // 
            // txtPesagemMassaVerificada
            // 
            this.txtPesagemMassaVerificada.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtPesagemMassaVerificada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPesagemMassaVerificada.Conteudo = Percolore.Core.UserControl.TipoConteudo.Decimal;
            this.txtPesagemMassaVerificada.Font = new System.Drawing.Font("Segoe UI Light", 25F);
            this.txtPesagemMassaVerificada.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtPesagemMassaVerificada.Location = new System.Drawing.Point(249, 129);
            this.txtPesagemMassaVerificada.Name = "txtPesagemMassaVerificada";
            this.txtPesagemMassaVerificada.Size = new System.Drawing.Size(164, 52);
            this.txtPesagemMassaVerificada.TabIndex = 368;
            this.txtPesagemMassaVerificada.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPesagemMassaVerificada.TextChanged += new System.EventHandler(this.txtMassaVerificada_TextChanged);
            // 
            // lblPesagemMassaIdeal
            // 
            this.lblPesagemMassaIdeal.AutoSize = true;
            this.lblPesagemMassaIdeal.Font = new System.Drawing.Font("Segoe UI Light", 25F);
            this.lblPesagemMassaIdeal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblPesagemMassaIdeal.Location = new System.Drawing.Point(340, 38);
            this.lblPesagemMassaIdeal.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblPesagemMassaIdeal.Name = "lblPesagemMassaIdeal";
            this.lblPesagemMassaIdeal.Size = new System.Drawing.Size(176, 46);
            this.lblPesagemMassaIdeal.TabIndex = 379;
            this.lblPesagemMassaIdeal.Text = "139.0000 g";
            // 
            // txtPesagemVolume
            // 
            this.txtPesagemVolume.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtPesagemVolume.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPesagemVolume.Conteudo = Percolore.Core.UserControl.TipoConteudo.Decimal;
            this.txtPesagemVolume.Font = new System.Drawing.Font("Segoe UI Light", 25F);
            this.txtPesagemVolume.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtPesagemVolume.Location = new System.Drawing.Point(24, 38);
            this.txtPesagemVolume.Name = "txtPesagemVolume";
            this.txtPesagemVolume.Size = new System.Drawing.Size(172, 52);
            this.txtPesagemVolume.TabIndex = 385;
            this.txtPesagemVolume.TextChanged += new System.EventHandler(this.txtPesagemVolume_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.label1.Location = new System.Drawing.Point(205, 56);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 23);
            this.label1.TabIndex = 386;
            this.label1.Text = "mL";
            // 
            // chb_balanca_Pesagem
            // 
            this.chb_balanca_Pesagem.AutoSize = true;
            this.chb_balanca_Pesagem.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.chb_balanca_Pesagem.Location = new System.Drawing.Point(39, 366);
            this.chb_balanca_Pesagem.Name = "chb_balanca_Pesagem";
            this.chb_balanca_Pesagem.Size = new System.Drawing.Size(82, 27);
            this.chb_balanca_Pesagem.TabIndex = 387;
            this.chb_balanca_Pesagem.Text = "Balança";
            this.chb_balanca_Pesagem.UseVisualStyleBackColor = true;
            this.chb_balanca_Pesagem.CheckedChanged += new System.EventHandler(this.chb_balanca_Pesagem_CheckedChanged);
            // 
            // gbComunicacaoBalanca
            // 
            this.gbComunicacaoBalanca.Controls.Add(this.cmbPortaSerial);
            this.gbComunicacaoBalanca.Location = new System.Drawing.Point(24, 399);
            this.gbComunicacaoBalanca.Name = "gbComunicacaoBalanca";
            this.gbComunicacaoBalanca.Size = new System.Drawing.Size(200, 74);
            this.gbComunicacaoBalanca.TabIndex = 388;
            this.gbComunicacaoBalanca.TabStop = false;
            this.gbComunicacaoBalanca.Text = "Comunicação Balança";
            this.gbComunicacaoBalanca.Visible = false;
            // 
            // cmbPortaSerial
            // 
            this.cmbPortaSerial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPortaSerial.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.cmbPortaSerial.FormattingEnabled = true;
            this.cmbPortaSerial.Location = new System.Drawing.Point(15, 28);
            this.cmbPortaSerial.Name = "cmbPortaSerial";
            this.cmbPortaSerial.Size = new System.Drawing.Size(169, 31);
            this.cmbPortaSerial.TabIndex = 0;
            this.cmbPortaSerial.SelectedIndexChanged += new System.EventHandler(this.cmbPortaSerial_SelectedIndexChanged);
            // 
            // fPrimeiraPesagem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 500);
            this.Controls.Add(this.gbComunicacaoBalanca);
            this.Controls.Add(this.chb_balanca_Pesagem);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPesagemVolume);
            this.Controls.Add(this.rd_pulsos);
            this.Controls.Add(this.btnPesagemConfirmar);
            this.Controls.Add(this.lblDesvioMedio);
            this.Controls.Add(this.txtPesagemDesvio);
            this.Controls.Add(this.lblPesagemDesvio);
            this.Controls.Add(this.listViewPesagem);
            this.Controls.Add(this.lblPesagemLegendaMassaIdeal);
            this.Controls.Add(this.btnPesagemAdicionar);
            this.Controls.Add(this.btnPesagemDispensar);
            this.Controls.Add(this.lblPesagemMassa);
            this.Controls.Add(this.lblMassaMedia);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblPesagemLegendaVolume);
            this.Controls.Add(this.txtPesagemMassaVerificada);
            this.Controls.Add(this.lblPesagemMassaIdeal);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fPrimeiraPesagem";
            this.Text = "Pesagem";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fPrimeiraPesagem_FormClosing);
            this.Load += new System.EventHandler(this.fPrimeiraPesagem_Load);
            this.gbComunicacaoBalanca.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RadioButton rd_pulsos;
        private System.Windows.Forms.Button btnPesagemConfirmar;
        private System.Windows.Forms.Label lblDesvioMedio;
        private Percolore.Core.UserControl.UTextBox txtPesagemDesvio;
        private System.Windows.Forms.Label lblPesagemDesvio;
        private System.Windows.Forms.ListView listViewPesagem;
        private System.Windows.Forms.Label lblPesagemLegendaMassaIdeal;
        private System.Windows.Forms.Button btnPesagemAdicionar;
        private System.Windows.Forms.Button btnPesagemDispensar;
        private System.Windows.Forms.Label lblPesagemMassa;
        private System.Windows.Forms.Label lblMassaMedia;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblPesagemLegendaVolume;
        private Percolore.Core.UserControl.UTextBox txtPesagemMassaVerificada;
        private System.Windows.Forms.Label lblPesagemMassaIdeal;
        private Percolore.Core.UserControl.UTextBox txtPesagemVolume;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chb_balanca_Pesagem;
        private System.Windows.Forms.GroupBox gbComunicacaoBalanca;
        private System.Windows.Forms.ComboBox cmbPortaSerial;
    }
}