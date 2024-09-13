namespace Percolore.IOConnect
{
    partial class fPesagem
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
            this.lblPesagemLegendaVolume = new System.Windows.Forms.Label();
            this.lblPesagemMassa = new System.Windows.Forms.Label();
            this.lblMassaMedia = new System.Windows.Forms.Label();
            this.btnPesagemDispensar = new System.Windows.Forms.Button();
            this.btnPesagemAdicionar = new System.Windows.Forms.Button();
            this.lblPesagemVolume = new System.Windows.Forms.Label();
            this.lblPesagemMassaIdeal = new System.Windows.Forms.Label();
            this.lblPesagemLegendaMassaIdeal = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listViewPesagem = new System.Windows.Forms.ListView();
            this.btnPesagemConfirmar = new System.Windows.Forms.Button();
            this.lblPesagemDesvio = new System.Windows.Forms.Label();
            this.lblDesvioMedio = new System.Windows.Forms.Label();
            this.rd_pulsos = new System.Windows.Forms.RadioButton();
            this.rd_pulsos_faixa = new System.Windows.Forms.RadioButton();
            this.rd_sem_pulsos = new System.Windows.Forms.RadioButton();
            this.chbTesteRecipiente = new System.Windows.Forms.CheckBox();
            this.btnTutorial = new System.Windows.Forms.Button();
            this.txtPesagemDesvio = new Percolore.Core.UserControl.UTextBox();
            this.txtPesagemMassaVerificada = new Percolore.Core.UserControl.UTextBox();
            this.SuspendLayout();
            // 
            // lblPesagemLegendaVolume
            // 
            this.lblPesagemLegendaVolume.AutoSize = true;
            this.lblPesagemLegendaVolume.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblPesagemLegendaVolume.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblPesagemLegendaVolume.Location = new System.Drawing.Point(12, 12);
            this.lblPesagemLegendaVolume.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblPesagemLegendaVolume.Name = "lblPesagemLegendaVolume";
            this.lblPesagemLegendaVolume.Size = new System.Drawing.Size(63, 23);
            this.lblPesagemLegendaVolume.TabIndex = 283;
            this.lblPesagemLegendaVolume.Text = "Volume";
            // 
            // lblPesagemMassa
            // 
            this.lblPesagemMassa.AutoSize = true;
            this.lblPesagemMassa.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblPesagemMassa.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblPesagemMassa.Location = new System.Drawing.Point(245, 95);
            this.lblPesagemMassa.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblPesagemMassa.Name = "lblPesagemMassa";
            this.lblPesagemMassa.Size = new System.Drawing.Size(79, 23);
            this.lblPesagemMassa.TabIndex = 323;
            this.lblPesagemMassa.Text = "Massa (g)";
            // 
            // lblMassaMedia
            // 
            this.lblMassaMedia.Font = new System.Drawing.Font("Segoe UI Light", 25F);
            this.lblMassaMedia.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblMassaMedia.Location = new System.Drawing.Point(241, 435);
            this.lblMassaMedia.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblMassaMedia.Name = "lblMassaMedia";
            this.lblMassaMedia.Size = new System.Drawing.Size(173, 46);
            this.lblMassaMedia.TabIndex = 320;
            this.lblMassaMedia.Text = "0";
            this.lblMassaMedia.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnPesagemDispensar
            // 
            this.btnPesagemDispensar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.btnPesagemDispensar.FlatAppearance.BorderSize = 0;
            this.btnPesagemDispensar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPesagemDispensar.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnPesagemDispensar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnPesagemDispensar.Location = new System.Drawing.Point(31, 122);
            this.btnPesagemDispensar.Name = "btnPesagemDispensar";
            this.btnPesagemDispensar.Size = new System.Drawing.Size(146, 52);
            this.btnPesagemDispensar.TabIndex = 0;
            this.btnPesagemDispensar.Text = "Dispensar";
            this.btnPesagemDispensar.UseVisualStyleBackColor = false;
            this.btnPesagemDispensar.Click += new System.EventHandler(this.btnDispensar_Click);
            // 
            // btnPesagemAdicionar
            // 
            this.btnPesagemAdicionar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.btnPesagemAdicionar.FlatAppearance.BorderSize = 0;
            this.btnPesagemAdicionar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPesagemAdicionar.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnPesagemAdicionar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnPesagemAdicionar.Location = new System.Drawing.Point(31, 182);
            this.btnPesagemAdicionar.Name = "btnPesagemAdicionar";
            this.btnPesagemAdicionar.Size = new System.Drawing.Size(146, 52);
            this.btnPesagemAdicionar.TabIndex = 3;
            this.btnPesagemAdicionar.Text = "Adicionar";
            this.btnPesagemAdicionar.UseVisualStyleBackColor = false;
            this.btnPesagemAdicionar.Click += new System.EventHandler(this.btnAdicionar_Click);
            // 
            // lblPesagemVolume
            // 
            this.lblPesagemVolume.AutoSize = true;
            this.lblPesagemVolume.Font = new System.Drawing.Font("Segoe UI Light", 25F);
            this.lblPesagemVolume.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblPesagemVolume.Location = new System.Drawing.Point(9, 31);
            this.lblPesagemVolume.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblPesagemVolume.Name = "lblPesagemVolume";
            this.lblPesagemVolume.Size = new System.Drawing.Size(171, 46);
            this.lblPesagemVolume.TabIndex = 355;
            this.lblPesagemVolume.Text = "0,0385 mL";
            // 
            // lblPesagemMassaIdeal
            // 
            this.lblPesagemMassaIdeal.AutoSize = true;
            this.lblPesagemMassaIdeal.Font = new System.Drawing.Font("Segoe UI Light", 25F);
            this.lblPesagemMassaIdeal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblPesagemMassaIdeal.Location = new System.Drawing.Point(332, 31);
            this.lblPesagemMassaIdeal.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblPesagemMassaIdeal.Name = "lblPesagemMassaIdeal";
            this.lblPesagemMassaIdeal.Size = new System.Drawing.Size(176, 46);
            this.lblPesagemMassaIdeal.TabIndex = 357;
            this.lblPesagemMassaIdeal.Text = "139.0000 g";
            // 
            // lblPesagemLegendaMassaIdeal
            // 
            this.lblPesagemLegendaMassaIdeal.AutoSize = true;
            this.lblPesagemLegendaMassaIdeal.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblPesagemLegendaMassaIdeal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblPesagemLegendaMassaIdeal.Location = new System.Drawing.Point(336, 12);
            this.lblPesagemLegendaMassaIdeal.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblPesagemLegendaMassaIdeal.Name = "lblPesagemLegendaMassaIdeal";
            this.lblPesagemLegendaMassaIdeal.Size = new System.Drawing.Size(92, 23);
            this.lblPesagemLegendaMassaIdeal.TabIndex = 356;
            this.lblPesagemLegendaMassaIdeal.Text = "Massa ideal";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Gainsboro;
            this.panel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.panel1.Location = new System.Drawing.Point(16, 85);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(560, 2);
            this.panel1.TabIndex = 284;
            // 
            // listViewPesagem
            // 
            this.listViewPesagem.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewPesagem.Font = new System.Drawing.Font("Segoe UI Light", 24F);
            this.listViewPesagem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.listViewPesagem.FullRowSelect = true;
            this.listViewPesagem.GridLines = true;
            this.listViewPesagem.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewPesagem.HideSelection = false;
            this.listViewPesagem.LabelWrap = false;
            this.listViewPesagem.Location = new System.Drawing.Point(241, 182);
            this.listViewPesagem.MultiSelect = false;
            this.listViewPesagem.Name = "listViewPesagem";
            this.listViewPesagem.Size = new System.Drawing.Size(336, 250);
            this.listViewPesagem.TabIndex = 6;
            this.listViewPesagem.UseCompatibleStateImageBehavior = false;
            this.listViewPesagem.View = System.Windows.Forms.View.Details;
            this.listViewPesagem.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            // 
            // btnPesagemConfirmar
            // 
            this.btnPesagemConfirmar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.btnPesagemConfirmar.Enabled = false;
            this.btnPesagemConfirmar.FlatAppearance.BorderSize = 0;
            this.btnPesagemConfirmar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPesagemConfirmar.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnPesagemConfirmar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnPesagemConfirmar.Location = new System.Drawing.Point(31, 242);
            this.btnPesagemConfirmar.Name = "btnPesagemConfirmar";
            this.btnPesagemConfirmar.Size = new System.Drawing.Size(146, 52);
            this.btnPesagemConfirmar.TabIndex = 5;
            this.btnPesagemConfirmar.Text = "Confirmar";
            this.btnPesagemConfirmar.UseVisualStyleBackColor = false;
            this.btnPesagemConfirmar.Click += new System.EventHandler(this.btnConfirmar_Click);
            // 
            // lblPesagemDesvio
            // 
            this.lblPesagemDesvio.AutoSize = true;
            this.lblPesagemDesvio.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblPesagemDesvio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblPesagemDesvio.Location = new System.Drawing.Point(409, 95);
            this.lblPesagemDesvio.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblPesagemDesvio.Name = "lblPesagemDesvio";
            this.lblPesagemDesvio.Size = new System.Drawing.Size(58, 23);
            this.lblPesagemDesvio.TabIndex = 359;
            this.lblPesagemDesvio.Text = "Desvio";
            // 
            // lblDesvioMedio
            // 
            this.lblDesvioMedio.Font = new System.Drawing.Font("Segoe UI Light", 25F);
            this.lblDesvioMedio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblDesvioMedio.Location = new System.Drawing.Point(413, 433);
            this.lblDesvioMedio.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblDesvioMedio.Name = "lblDesvioMedio";
            this.lblDesvioMedio.Size = new System.Drawing.Size(173, 46);
            this.lblDesvioMedio.TabIndex = 363;
            this.lblDesvioMedio.Text = "0";
            this.lblDesvioMedio.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rd_pulsos
            // 
            this.rd_pulsos.AutoSize = true;
            this.rd_pulsos.Checked = true;
            this.rd_pulsos.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.rd_pulsos.Location = new System.Drawing.Point(31, 317);
            this.rd_pulsos.Name = "rd_pulsos";
            this.rd_pulsos.Size = new System.Drawing.Size(142, 27);
            this.rd_pulsos.TabIndex = 364;
            this.rd_pulsos.TabStop = true;
            this.rd_pulsos.Text = "Redefinir pulsos";
            this.rd_pulsos.UseVisualStyleBackColor = true;
            // 
            // rd_pulsos_faixa
            // 
            this.rd_pulsos_faixa.AutoSize = true;
            this.rd_pulsos_faixa.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.rd_pulsos_faixa.Location = new System.Drawing.Point(31, 350);
            this.rd_pulsos_faixa.Name = "rd_pulsos_faixa";
            this.rd_pulsos_faixa.Size = new System.Drawing.Size(180, 27);
            this.rd_pulsos_faixa.TabIndex = 365;
            this.rd_pulsos_faixa.Text = "Redefinir pulsos Faixa";
            this.rd_pulsos_faixa.UseVisualStyleBackColor = true;
            // 
            // rd_sem_pulsos
            // 
            this.rd_sem_pulsos.AutoSize = true;
            this.rd_sem_pulsos.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.rd_sem_pulsos.Location = new System.Drawing.Point(31, 383);
            this.rd_sem_pulsos.Name = "rd_sem_pulsos";
            this.rd_sem_pulsos.Size = new System.Drawing.Size(126, 27);
            this.rd_sem_pulsos.TabIndex = 366;
            this.rd_sem_pulsos.Text = "Não Redefinir";
            this.rd_sem_pulsos.UseVisualStyleBackColor = true;
            // 
            // chbTesteRecipiente
            // 
            this.chbTesteRecipiente.AutoSize = true;
            this.chbTesteRecipiente.Checked = true;
            this.chbTesteRecipiente.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbTesteRecipiente.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.chbTesteRecipiente.Location = new System.Drawing.Point(30, 416);
            this.chbTesteRecipiente.Name = "chbTesteRecipiente";
            this.chbTesteRecipiente.Size = new System.Drawing.Size(168, 27);
            this.chbTesteRecipiente.TabIndex = 367;
            this.chbTesteRecipiente.Text = "Teste de Recipiente";
            this.chbTesteRecipiente.UseVisualStyleBackColor = true;
            // 
            // btnTutorial
            // 
            this.btnTutorial.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.btnTutorial.FlatAppearance.BorderSize = 0;
            this.btnTutorial.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTutorial.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnTutorial.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnTutorial.Location = new System.Drawing.Point(43, 458);
            this.btnTutorial.Name = "btnTutorial";
            this.btnTutorial.Size = new System.Drawing.Size(114, 33);
            this.btnTutorial.TabIndex = 368;
            this.btnTutorial.Text = "Treinamento";
            this.btnTutorial.UseVisualStyleBackColor = false;
            this.btnTutorial.Click += new System.EventHandler(this.btnTutorial_Click);
            // 
            // txtPesagemDesvio
            // 
            this.txtPesagemDesvio.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtPesagemDesvio.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPesagemDesvio.Conteudo = Percolore.Core.UserControl.TipoConteudo.Decimal;
            this.txtPesagemDesvio.Enabled = false;
            this.txtPesagemDesvio.Font = new System.Drawing.Font("Segoe UI Light", 25F);
            this.txtPesagemDesvio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtPesagemDesvio.Location = new System.Drawing.Point(413, 122);
            this.txtPesagemDesvio.Name = "txtPesagemDesvio";
            this.txtPesagemDesvio.Size = new System.Drawing.Size(164, 52);
            this.txtPesagemDesvio.TabIndex = 2;
            this.txtPesagemDesvio.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtPesagemMassaVerificada
            // 
            this.txtPesagemMassaVerificada.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtPesagemMassaVerificada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPesagemMassaVerificada.Conteudo = Percolore.Core.UserControl.TipoConteudo.Decimal;
            this.txtPesagemMassaVerificada.Font = new System.Drawing.Font("Segoe UI Light", 25F);
            this.txtPesagemMassaVerificada.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtPesagemMassaVerificada.Location = new System.Drawing.Point(241, 122);
            this.txtPesagemMassaVerificada.Name = "txtPesagemMassaVerificada";
            this.txtPesagemMassaVerificada.Size = new System.Drawing.Size(164, 52);
            this.txtPesagemMassaVerificada.TabIndex = 1;
            this.txtPesagemMassaVerificada.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPesagemMassaVerificada.TextChanged += new System.EventHandler(this.txtMassaVerificada_TextChanged);
            // 
            // fPesagem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(610, 500);
            this.Controls.Add(this.btnTutorial);
            this.Controls.Add(this.chbTesteRecipiente);
            this.Controls.Add(this.rd_sem_pulsos);
            this.Controls.Add(this.rd_pulsos_faixa);
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
            this.Controls.Add(this.lblPesagemVolume);
            this.Controls.Add(this.lblPesagemMassaIdeal);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fPesagem";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pesagem";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Pesagem_FormClosing);
            this.Load += new System.EventHandler(this.Pesagem_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblPesagemLegendaVolume;
        private System.Windows.Forms.Label lblPesagemMassa;
        private System.Windows.Forms.Label lblMassaMedia;
        private System.Windows.Forms.Button btnPesagemDispensar;
        private System.Windows.Forms.Button btnPesagemAdicionar;
        private System.Windows.Forms.Label lblPesagemVolume;
        private System.Windows.Forms.Label lblPesagemLegendaMassaIdeal;
        private System.Windows.Forms.Label lblPesagemMassaIdeal;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListView listViewPesagem;
        private System.Windows.Forms.Button btnPesagemConfirmar;
        private Percolore.Core.UserControl.UTextBox txtPesagemMassaVerificada;
        private System.Windows.Forms.Label lblPesagemDesvio;
        private Percolore.Core.UserControl.UTextBox txtPesagemDesvio;
        private System.Windows.Forms.Label lblDesvioMedio;
        private System.Windows.Forms.RadioButton rd_pulsos;
        private System.Windows.Forms.RadioButton rd_pulsos_faixa;
        private System.Windows.Forms.RadioButton rd_sem_pulsos;
        private System.Windows.Forms.CheckBox chbTesteRecipiente;
        private System.Windows.Forms.Button btnTutorial;
    }
}