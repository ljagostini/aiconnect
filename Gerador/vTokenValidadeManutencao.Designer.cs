namespace Percolore.Gerador
{
    partial class vTokenValidadeManutencao
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
            this.btnSair = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnGerar = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.mskValidadeManutencao = new Percolore.Core.UserControl.UMaskedTextBox();
            this.cboAplicacao = new Percolore.Core.UserControl.UComboBox();
            this.txtSerial = new Percolore.Core.UserControl.UTextBox();
            this.txtPrazoExpiracao = new Percolore.Core.UserControl.UTextBox();
            this.txtToken = new Percolore.Core.UserControl.UTextBox();
            this.pnlBarraTitulo.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBarraTitulo
            // 
            this.pnlBarraTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(34)))), ((int)(((byte)(127)))));
            this.pnlBarraTitulo.Controls.Add(this.btnSair);
            this.pnlBarraTitulo.Controls.Add(this.lblTitulo);
            this.pnlBarraTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBarraTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.pnlBarraTitulo.Location = new System.Drawing.Point(0, 0);
            this.pnlBarraTitulo.Name = "pnlBarraTitulo";
            this.pnlBarraTitulo.Size = new System.Drawing.Size(701, 64);
            this.pnlBarraTitulo.TabIndex = 3;
            // 
            // btnSair
            // 
            this.btnSair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSair.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(138)))), ((int)(((byte)(228)))));
            this.btnSair.FlatAppearance.BorderSize = 0;
            this.btnSair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSair.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnSair.Location = new System.Drawing.Point(637, 0);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(64, 64);
            this.btnSair.TabIndex = 0;
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
            this.lblTitulo.Size = new System.Drawing.Size(271, 23);
            this.lblTitulo.TabIndex = 309;
            this.lblTitulo.Text = "Token de validade da manutenção";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnGerar
            // 
            this.btnGerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(34)))), ((int)(((byte)(127)))));
            this.btnGerar.FlatAppearance.BorderSize = 0;
            this.btnGerar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGerar.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnGerar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnGerar.Location = new System.Drawing.Point(482, 243);
            this.btnGerar.Name = "btnGerar";
            this.btnGerar.Size = new System.Drawing.Size(203, 53);
            this.btnGerar.TabIndex = 5;
            this.btnGerar.Tag = "0";
            this.btnGerar.Text = "Gerar token";
            this.btnGerar.UseVisualStyleBackColor = false;
            this.btnGerar.Click += new System.EventHandler(this.btnGerar_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 12.75F);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.label5.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label5.Location = new System.Drawing.Point(43, 137);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(155, 23);
            this.label5.TabIndex = 381;
            this.label5.Text = "Prazo de expiração";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12.75F);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label3.Location = new System.Drawing.Point(85, 202);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 23);
            this.label3.TabIndex = 383;
            this.label3.Text = "Token gerado";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 12.75F);
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.label6.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label6.Location = new System.Drawing.Point(300, 137);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(200, 23);
            this.label6.TabIndex = 385;
            this.label6.Text = "Validade da manutenção";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 12.75F);
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.label8.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label8.Location = new System.Drawing.Point(51, 82);
            this.label8.Margin = new System.Windows.Forms.Padding(0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(147, 23);
            this.label8.TabIndex = 388;
            this.label8.Text = "Serial da máquina";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 12.75F);
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.label9.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label9.Location = new System.Drawing.Point(417, 82);
            this.label9.Margin = new System.Windows.Forms.Padding(0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 23);
            this.label9.TabIndex = 391;
            this.label9.Text = "Aplicação";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 10.25F);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.label4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label4.Location = new System.Drawing.Point(356, 160);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(144, 19);
            this.label4.TabIndex = 386;
            this.label4.Text = "Válido até as 23:59:59";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10.25F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Location = new System.Drawing.Point(12, 160);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(186, 19);
            this.label1.TabIndex = 382;
            this.label1.Text = "Expiração do token em horas";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 10.25F);
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.label10.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label10.Location = new System.Drawing.Point(322, 105);
            this.label10.Margin = new System.Windows.Forms.Padding(0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(178, 19);
            this.label10.TabIndex = 392;
            this.label10.Text = "Onde o token será utilizado";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 10.25F);
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.label7.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label7.Location = new System.Drawing.Point(20, 105);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(178, 19);
            this.label7.TabIndex = 389;
            this.label7.Text = "Onde o token será utilizado";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mskValidadeManutencao
            // 
            this.mskValidadeManutencao.BackColor = System.Drawing.SystemColors.Window;
            this.mskValidadeManutencao.BorderColor = System.Drawing.Color.Gainsboro;
            this.mskValidadeManutencao.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mskValidadeManutencao.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.mskValidadeManutencao.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.mskValidadeManutencao.Location = new System.Drawing.Point(503, 137);
            this.mskValidadeManutencao.Mask = "00/00/0000";
            this.mskValidadeManutencao.Name = "mskValidadeManutencao";
            this.mskValidadeManutencao.PromptChar = ' ';
            this.mskValidadeManutencao.Size = new System.Drawing.Size(182, 47);
            this.mskValidadeManutencao.TabIndex = 3;
            this.mskValidadeManutencao.ValidatingType = typeof(System.DateTime);
            // 
            // cboAplicacao
            // 
            this.cboAplicacao.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cboAplicacao.BorderColor = System.Drawing.Color.Gainsboro;
            this.cboAplicacao.BorderSize = 1;
            this.cboAplicacao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAplicacao.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboAplicacao.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.cboAplicacao.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.cboAplicacao.FormattingEnabled = true;
            this.cboAplicacao.Location = new System.Drawing.Point(503, 81);
            this.cboAplicacao.Name = "cboAplicacao";
            this.cboAplicacao.Size = new System.Drawing.Size(182, 48);
            this.cboAplicacao.TabIndex = 1;
            // 
            // txtSerial
            // 
            this.txtSerial.BackColor = System.Drawing.Color.White;
            this.txtSerial.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtSerial.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSerial.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtSerial.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txtSerial.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtSerial.Location = new System.Drawing.Point(201, 82);
            this.txtSerial.MaxLength = 4;
            this.txtSerial.Name = "txtSerial";
            this.txtSerial.Size = new System.Drawing.Size(83, 47);
            this.txtSerial.TabIndex = 0;
            // 
            // txtPrazoExpiracao
            // 
            this.txtPrazoExpiracao.BackColor = System.Drawing.Color.White;
            this.txtPrazoExpiracao.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtPrazoExpiracao.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPrazoExpiracao.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtPrazoExpiracao.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txtPrazoExpiracao.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtPrazoExpiracao.Location = new System.Drawing.Point(201, 137);
            this.txtPrazoExpiracao.MaxLength = 4;
            this.txtPrazoExpiracao.Name = "txtPrazoExpiracao";
            this.txtPrazoExpiracao.Size = new System.Drawing.Size(83, 47);
            this.txtPrazoExpiracao.TabIndex = 2;
            // 
            // txtToken
            // 
            this.txtToken.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtToken.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtToken.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtToken.Conteudo = Percolore.Core.UserControl.TipoConteudo.Padrao;
            this.txtToken.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txtToken.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtToken.Location = new System.Drawing.Point(201, 190);
            this.txtToken.MaxLength = 12;
            this.txtToken.Name = "txtToken";
            this.txtToken.ReadOnly = true;
            this.txtToken.Size = new System.Drawing.Size(484, 47);
            this.txtToken.TabIndex = 4;
            // 
            // vTokenValidadeManutencao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(701, 311);
            this.Controls.Add(this.mskValidadeManutencao);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.cboAplicacao);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtSerial);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtPrazoExpiracao);
            this.Controls.Add(this.btnGerar);
            this.Controls.Add(this.txtToken);
            this.Controls.Add(this.pnlBarraTitulo);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "vTokenValidadeManutencao";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.vPainelControle_Paint);
            this.pnlBarraTitulo.ResumeLayout(false);
            this.pnlBarraTitulo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlBarraTitulo;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.Label lblTitulo;
        private Core.UserControl.UTextBox txtToken;
        private System.Windows.Forms.Button btnGerar;
        private Core.UserControl.UTextBox txtPrazoExpiracao;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private Core.UserControl.UTextBox txtSerial;
        private Core.UserControl.UComboBox cboAplicacao;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label7;
        private Core.UserControl.UMaskedTextBox mskValidadeManutencao;
    }
}

