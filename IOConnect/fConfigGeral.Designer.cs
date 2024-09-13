
namespace Percolore.IOConnect
{
    partial class fConfigGeral
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
            this.pnlBarraTitulo = new System.Windows.Forms.Panel();
            this.lblGeralFuncinamentoSoftware = new System.Windows.Forms.Label();
            this.btnConfirmar = new System.Windows.Forms.Button();
            this.btnSair = new System.Windows.Forms.Button();
            this.chkViewMessageProc = new System.Windows.Forms.CheckBox();
            this.txt_NameRemoteAccess = new Percolore.Core.UserControl.UTextBox();
            this.lbl_NameRemoteAccess = new System.Windows.Forms.Label();
            this.chkTecladoVirtual = new System.Windows.Forms.CheckBox();
            this.chkTesteRecipiente = new System.Windows.Forms.CheckBox();
            this.chkFormulasPersonalizadas = new System.Windows.Forms.CheckBox();
            this.chkHabilitarIndentificacaoCopo = new System.Windows.Forms.CheckBox();
            this.chkTouchScrenn = new System.Windows.Forms.CheckBox();
            this.chk_TreinamentoCal = new System.Windows.Forms.CheckBox();
            this.lblDelayEsponja = new System.Windows.Forms.Label();
            this.txtDelayEsponja = new Percolore.Core.UserControl.UTextBox();
            this.chkDispensaSequencial = new System.Windows.Forms.CheckBox();
            this.btnRessetData = new System.Windows.Forms.Button();
            this.pnlBarraTitulo.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBarraTitulo
            // 
            this.pnlBarraTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(145)))), ((int)(((byte)(204)))));
            this.pnlBarraTitulo.Controls.Add(this.lblGeralFuncinamentoSoftware);
            this.pnlBarraTitulo.Controls.Add(this.btnConfirmar);
            this.pnlBarraTitulo.Controls.Add(this.btnSair);
            this.pnlBarraTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBarraTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.pnlBarraTitulo.Location = new System.Drawing.Point(0, 0);
            this.pnlBarraTitulo.Name = "pnlBarraTitulo";
            this.pnlBarraTitulo.Size = new System.Drawing.Size(915, 110);
            this.pnlBarraTitulo.TabIndex = 385;
            // 
            // lblGeralFuncinamentoSoftware
            // 
            this.lblGeralFuncinamentoSoftware.AutoSize = true;
            this.lblGeralFuncinamentoSoftware.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblGeralFuncinamentoSoftware.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.lblGeralFuncinamentoSoftware.Location = new System.Drawing.Point(26, 21);
            this.lblGeralFuncinamentoSoftware.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblGeralFuncinamentoSoftware.Name = "lblGeralFuncinamentoSoftware";
            this.lblGeralFuncinamentoSoftware.Size = new System.Drawing.Size(213, 23);
            this.lblGeralFuncinamentoSoftware.TabIndex = 383;
            this.lblGeralFuncinamentoSoftware.Text = "Funcionamento do software";
            // 
            // btnConfirmar
            // 
            this.btnConfirmar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirmar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(145)))), ((int)(((byte)(204)))));
            this.btnConfirmar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(138)))), ((int)(((byte)(228)))));
            this.btnConfirmar.FlatAppearance.BorderSize = 0;
            this.btnConfirmar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirmar.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnConfirmar.Location = new System.Drawing.Point(752, 11);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(95, 43);
            this.btnConfirmar.TabIndex = 0;
            this.btnConfirmar.Tag = "0";
            this.btnConfirmar.Text = "Confirmar";
            this.btnConfirmar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnConfirmar.UseVisualStyleBackColor = false;
            this.btnConfirmar.Click += new System.EventHandler(this.btnConfirmar_Click);
            // 
            // btnSair
            // 
            this.btnSair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSair.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(145)))), ((int)(((byte)(204)))));
            this.btnSair.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(138)))), ((int)(((byte)(228)))));
            this.btnSair.FlatAppearance.BorderSize = 0;
            this.btnSair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSair.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnSair.Location = new System.Drawing.Point(851, 0);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(64, 64);
            this.btnSair.TabIndex = 1;
            this.btnSair.Tag = "0";
            this.btnSair.Text = "Sair";
            this.btnSair.UseVisualStyleBackColor = false;
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click);
            // 
            // chkViewMessageProc
            // 
            this.chkViewMessageProc.AutoSize = true;
            this.chkViewMessageProc.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.chkViewMessageProc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.chkViewMessageProc.Location = new System.Drawing.Point(610, 271);
            this.chkViewMessageProc.Name = "chkViewMessageProc";
            this.chkViewMessageProc.Size = new System.Drawing.Size(208, 27);
            this.chkViewMessageProc.TabIndex = 432;
            this.chkViewMessageProc.TabStop = false;
            this.chkViewMessageProc.Text = "Mensagem em Processo";
            this.chkViewMessageProc.UseVisualStyleBackColor = true;
            // 
            // txt_NameRemoteAccess
            // 
            this.txt_NameRemoteAccess.BorderColor = System.Drawing.Color.Gainsboro;
            this.txt_NameRemoteAccess.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_NameRemoteAccess.Conteudo = Percolore.Core.UserControl.TipoConteudo.Padrao;
            this.txt_NameRemoteAccess.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txt_NameRemoteAccess.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txt_NameRemoteAccess.Location = new System.Drawing.Point(19, 157);
            this.txt_NameRemoteAccess.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.txt_NameRemoteAccess.MaxLength = 100;
            this.txt_NameRemoteAccess.Name = "txt_NameRemoteAccess";
            this.txt_NameRemoteAccess.Size = new System.Drawing.Size(398, 47);
            this.txt_NameRemoteAccess.TabIndex = 434;
            this.txt_NameRemoteAccess.Tag = "0";
            // 
            // lbl_NameRemoteAccess
            // 
            this.lbl_NameRemoteAccess.AutoSize = true;
            this.lbl_NameRemoteAccess.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lbl_NameRemoteAccess.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lbl_NameRemoteAccess.Location = new System.Drawing.Point(15, 130);
            this.lbl_NameRemoteAccess.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbl_NameRemoteAccess.Name = "lbl_NameRemoteAccess";
            this.lbl_NameRemoteAccess.Size = new System.Drawing.Size(174, 23);
            this.lbl_NameRemoteAccess.TabIndex = 433;
            this.lbl_NameRemoteAccess.Text = "Nome Acesso Remoto";
            // 
            // chkTecladoVirtual
            // 
            this.chkTecladoVirtual.AutoSize = true;
            this.chkTecladoVirtual.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.chkTecladoVirtual.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.chkTecladoVirtual.Location = new System.Drawing.Point(19, 238);
            this.chkTecladoVirtual.Name = "chkTecladoVirtual";
            this.chkTecladoVirtual.Size = new System.Drawing.Size(131, 27);
            this.chkTecladoVirtual.TabIndex = 435;
            this.chkTecladoVirtual.TabStop = false;
            this.chkTecladoVirtual.Text = "Teclado virtual";
            this.chkTecladoVirtual.UseVisualStyleBackColor = true;
            // 
            // chkTesteRecipiente
            // 
            this.chkTesteRecipiente.AutoSize = true;
            this.chkTesteRecipiente.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.chkTesteRecipiente.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.chkTesteRecipiente.Location = new System.Drawing.Point(354, 238);
            this.chkTesteRecipiente.Name = "chkTesteRecipiente";
            this.chkTesteRecipiente.Size = new System.Drawing.Size(165, 27);
            this.chkTesteRecipiente.TabIndex = 437;
            this.chkTesteRecipiente.TabStop = false;
            this.chkTesteRecipiente.Text = "Teste de recipiente";
            this.chkTesteRecipiente.UseVisualStyleBackColor = true;
            // 
            // chkFormulasPersonalizadas
            // 
            this.chkFormulasPersonalizadas.AutoSize = true;
            this.chkFormulasPersonalizadas.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.chkFormulasPersonalizadas.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.chkFormulasPersonalizadas.Location = new System.Drawing.Point(19, 313);
            this.chkFormulasPersonalizadas.Name = "chkFormulasPersonalizadas";
            this.chkFormulasPersonalizadas.Size = new System.Drawing.Size(205, 27);
            this.chkFormulasPersonalizadas.TabIndex = 436;
            this.chkFormulasPersonalizadas.TabStop = false;
            this.chkFormulasPersonalizadas.Text = "Fórmulas personalizadas";
            this.chkFormulasPersonalizadas.UseVisualStyleBackColor = true;
            // 
            // chkHabilitarIndentificacaoCopo
            // 
            this.chkHabilitarIndentificacaoCopo.AutoSize = true;
            this.chkHabilitarIndentificacaoCopo.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.chkHabilitarIndentificacaoCopo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.chkHabilitarIndentificacaoCopo.Location = new System.Drawing.Point(610, 238);
            this.chkHabilitarIndentificacaoCopo.Name = "chkHabilitarIndentificacaoCopo";
            this.chkHabilitarIndentificacaoCopo.Size = new System.Drawing.Size(237, 27);
            this.chkHabilitarIndentificacaoCopo.TabIndex = 438;
            this.chkHabilitarIndentificacaoCopo.Text = "Habilitar Indentificação Copo";
            this.chkHabilitarIndentificacaoCopo.UseVisualStyleBackColor = true;
            // 
            // chkTouchScrenn
            // 
            this.chkTouchScrenn.AutoSize = true;
            this.chkTouchScrenn.Location = new System.Drawing.Point(-177, -64);
            this.chkTouchScrenn.Name = "chkTouchScrenn";
            this.chkTouchScrenn.Size = new System.Drawing.Size(91, 17);
            this.chkTouchScrenn.TabIndex = 439;
            this.chkTouchScrenn.Text = "TouchScreen";
            this.chkTouchScrenn.UseVisualStyleBackColor = true;
            // 
            // chk_TreinamentoCal
            // 
            this.chk_TreinamentoCal.AutoSize = true;
            this.chk_TreinamentoCal.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.chk_TreinamentoCal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.chk_TreinamentoCal.Location = new System.Drawing.Point(354, 277);
            this.chk_TreinamentoCal.Name = "chk_TreinamentoCal";
            this.chk_TreinamentoCal.Size = new System.Drawing.Size(198, 27);
            this.chk_TreinamentoCal.TabIndex = 443;
            this.chk_TreinamentoCal.TabStop = false;
            this.chk_TreinamentoCal.Text = "Treinamento Calibração";
            this.chk_TreinamentoCal.UseVisualStyleBackColor = true;
            // 
            // lblDelayEsponja
            // 
            this.lblDelayEsponja.AutoSize = true;
            this.lblDelayEsponja.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblDelayEsponja.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblDelayEsponja.Location = new System.Drawing.Point(486, 130);
            this.lblDelayEsponja.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblDelayEsponja.Name = "lblDelayEsponja";
            this.lblDelayEsponja.Size = new System.Drawing.Size(155, 23);
            this.lblDelayEsponja.TabIndex = 442;
            this.lblDelayEsponja.Text = "Delay Esponja (min):";
            // 
            // txtDelayEsponja
            // 
            this.txtDelayEsponja.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtDelayEsponja.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDelayEsponja.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtDelayEsponja.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txtDelayEsponja.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtDelayEsponja.Location = new System.Drawing.Point(490, 157);
            this.txtDelayEsponja.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.txtDelayEsponja.MaxLength = 3;
            this.txtDelayEsponja.Name = "txtDelayEsponja";
            this.txtDelayEsponja.Size = new System.Drawing.Size(151, 47);
            this.txtDelayEsponja.TabIndex = 441;
            this.txtDelayEsponja.Tag = "0";
            // 
            // chkDispensaSequencial
            // 
            this.chkDispensaSequencial.AutoSize = true;
            this.chkDispensaSequencial.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.chkDispensaSequencial.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.chkDispensaSequencial.Location = new System.Drawing.Point(19, 277);
            this.chkDispensaSequencial.Name = "chkDispensaSequencial";
            this.chkDispensaSequencial.Size = new System.Drawing.Size(264, 27);
            this.chkDispensaSequencial.TabIndex = 440;
            this.chkDispensaSequencial.TabStop = false;
            this.chkDispensaSequencial.Text = "Dispensa sequencial Entre Placas";
            this.chkDispensaSequencial.UseVisualStyleBackColor = true;
            // 
            // btnRessetData
            // 
            this.btnRessetData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(178)))), ((int)(((byte)(89)))));
            this.btnRessetData.Location = new System.Drawing.Point(19, 359);
            this.btnRessetData.Name = "btnRessetData";
            this.btnRessetData.Size = new System.Drawing.Size(157, 47);
            this.btnRessetData.TabIndex = 444;
            this.btnRessetData.Text = "Ressetar Datas";
            this.btnRessetData.UseVisualStyleBackColor = false;
            this.btnRessetData.Click += new System.EventHandler(this.btnRessetData_Click);
            // 
            // fConfigGeral
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(915, 450);
            this.Controls.Add(this.btnRessetData);
            this.Controls.Add(this.chkTouchScrenn);
            this.Controls.Add(this.chk_TreinamentoCal);
            this.Controls.Add(this.lblDelayEsponja);
            this.Controls.Add(this.txtDelayEsponja);
            this.Controls.Add(this.chkDispensaSequencial);
            this.Controls.Add(this.chkTecladoVirtual);
            this.Controls.Add(this.chkTesteRecipiente);
            this.Controls.Add(this.chkFormulasPersonalizadas);
            this.Controls.Add(this.chkHabilitarIndentificacaoCopo);
            this.Controls.Add(this.txt_NameRemoteAccess);
            this.Controls.Add(this.lbl_NameRemoteAccess);
            this.Controls.Add(this.chkViewMessageProc);
            this.Controls.Add(this.pnlBarraTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fConfigGeral";
            this.Text = "fConfigGeral";
            this.Load += new System.EventHandler(this.fConfigGeral_Load);
            this.pnlBarraTitulo.ResumeLayout(false);
            this.pnlBarraTitulo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlBarraTitulo;
        private System.Windows.Forms.Button btnConfirmar;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.CheckBox chkViewMessageProc;
        private Percolore.Core.UserControl.UTextBox txt_NameRemoteAccess;
        private System.Windows.Forms.Label lbl_NameRemoteAccess;
        private System.Windows.Forms.CheckBox chkTecladoVirtual;
        private System.Windows.Forms.CheckBox chkTesteRecipiente;
        private System.Windows.Forms.CheckBox chkFormulasPersonalizadas;
        private System.Windows.Forms.CheckBox chkHabilitarIndentificacaoCopo;
        private System.Windows.Forms.CheckBox chkTouchScrenn;
        private System.Windows.Forms.CheckBox chk_TreinamentoCal;
        private System.Windows.Forms.Label lblDelayEsponja;
        private Percolore.Core.UserControl.UTextBox txtDelayEsponja;
        private System.Windows.Forms.CheckBox chkDispensaSequencial;
        private System.Windows.Forms.Label lblGeralFuncinamentoSoftware;
        private System.Windows.Forms.Button btnRessetData;
    }
}