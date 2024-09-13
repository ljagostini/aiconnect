namespace Percolore.IOConnect
{
    partial class fCalibracao
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
            this.lblNomeColorante = new System.Windows.Forms.Label();
            this.btnConfirmar = new System.Windows.Forms.Button();
            this.btnSair = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblVolume = new System.Windows.Forms.Label();
            this.txtVolume = new Percolore.Core.UserControl.UTextBox();
            this.txtPulsos = new Percolore.Core.UserControl.UTextBox();
            this.lblPulsos = new System.Windows.Forms.Label();
            this.txtVelocidade = new Percolore.Core.UserControl.UTextBox();
            this.lblVelocidade = new System.Windows.Forms.Label();
            this.txtAceleracao = new Percolore.Core.UserControl.UTextBox();
            this.lblAceleracao = new System.Windows.Forms.Label();
            this.txtDelay = new Percolore.Core.UserControl.UTextBox();
            this.lblDelay = new System.Windows.Forms.Label();
            this.txtPulsoReverso = new Percolore.Core.UserControl.UTextBox();
            this.lblPusloRev = new System.Windows.Forms.Label();
            this.pnlBarraTitulo.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBarraTitulo
            // 
            this.pnlBarraTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(145)))), ((int)(((byte)(204)))));
            this.pnlBarraTitulo.Controls.Add(this.lblNomeColorante);
            this.pnlBarraTitulo.Controls.Add(this.btnConfirmar);
            this.pnlBarraTitulo.Controls.Add(this.btnSair);
            this.pnlBarraTitulo.Controls.Add(this.lblTitulo);
            this.pnlBarraTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBarraTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.pnlBarraTitulo.Location = new System.Drawing.Point(0, 0);
            this.pnlBarraTitulo.Name = "pnlBarraTitulo";
            this.pnlBarraTitulo.Size = new System.Drawing.Size(800, 110);
            this.pnlBarraTitulo.TabIndex = 375;
            // 
            // lblNomeColorante
            // 
            this.lblNomeColorante.AutoSize = true;
            this.lblNomeColorante.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblNomeColorante.Font = new System.Drawing.Font("Segoe UI", 12.75F);
            this.lblNomeColorante.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.lblNomeColorante.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblNomeColorante.Location = new System.Drawing.Point(12, 65);
            this.lblNomeColorante.Margin = new System.Windows.Forms.Padding(0);
            this.lblNomeColorante.Name = "lblNomeColorante";
            this.lblNomeColorante.Size = new System.Drawing.Size(89, 23);
            this.lblNomeColorante.TabIndex = 310;
            this.lblNomeColorante.Text = "Colorante:";
            this.lblNomeColorante.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnConfirmar
            // 
            this.btnConfirmar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirmar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(145)))), ((int)(((byte)(204)))));
            this.btnConfirmar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(138)))), ((int)(((byte)(228)))));
            this.btnConfirmar.FlatAppearance.BorderSize = 0;
            this.btnConfirmar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirmar.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnConfirmar.Location = new System.Drawing.Point(638, 0);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(95, 64);
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
            this.btnSair.Location = new System.Drawing.Point(736, 0);
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
            this.lblTitulo.Size = new System.Drawing.Size(209, 23);
            this.lblTitulo.TabIndex = 309;
            this.lblTitulo.Text = "Adicionar Faixa Calibracão";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblVolume
            // 
            this.lblVolume.AutoSize = true;
            this.lblVolume.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblVolume.Font = new System.Drawing.Font("Segoe UI", 12.75F);
            this.lblVolume.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblVolume.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblVolume.Location = new System.Drawing.Point(12, 130);
            this.lblVolume.Margin = new System.Windows.Forms.Padding(0);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(72, 23);
            this.lblVolume.TabIndex = 376;
            this.lblVolume.Text = "Volume:";
            this.lblVolume.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtVolume
            // 
            this.txtVolume.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.txtVolume.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtVolume.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtVolume.Conteudo = Percolore.Core.UserControl.TipoConteudo.Decimal;
            this.txtVolume.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txtVolume.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtVolume.Location = new System.Drawing.Point(112, 114);
            this.txtVolume.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.txtVolume.MaxLength = 100;
            this.txtVolume.Name = "txtVolume";
            this.txtVolume.Size = new System.Drawing.Size(166, 47);
            this.txtVolume.TabIndex = 377;
            this.txtVolume.TabStop = false;
            this.txtVolume.Tag = "0";
            this.txtVolume.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtVolume.TextChanged += new System.EventHandler(this.txtVolume_TextChanged);
            // 
            // txtPulsos
            // 
            this.txtPulsos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.txtPulsos.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtPulsos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPulsos.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtPulsos.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txtPulsos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtPulsos.Location = new System.Drawing.Point(112, 171);
            this.txtPulsos.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.txtPulsos.MaxLength = 100;
            this.txtPulsos.Name = "txtPulsos";
            this.txtPulsos.Size = new System.Drawing.Size(166, 47);
            this.txtPulsos.TabIndex = 379;
            this.txtPulsos.TabStop = false;
            this.txtPulsos.Tag = "0";
            this.txtPulsos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblPulsos
            // 
            this.lblPulsos.AutoSize = true;
            this.lblPulsos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblPulsos.Font = new System.Drawing.Font("Segoe UI", 12.75F);
            this.lblPulsos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblPulsos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPulsos.Location = new System.Drawing.Point(12, 187);
            this.lblPulsos.Margin = new System.Windows.Forms.Padding(0);
            this.lblPulsos.Name = "lblPulsos";
            this.lblPulsos.Size = new System.Drawing.Size(62, 23);
            this.lblPulsos.TabIndex = 378;
            this.lblPulsos.Text = "Pulsos:";
            this.lblPulsos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtVelocidade
            // 
            this.txtVelocidade.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.txtVelocidade.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtVelocidade.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtVelocidade.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtVelocidade.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txtVelocidade.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtVelocidade.Location = new System.Drawing.Point(112, 229);
            this.txtVelocidade.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.txtVelocidade.MaxLength = 100;
            this.txtVelocidade.Name = "txtVelocidade";
            this.txtVelocidade.Size = new System.Drawing.Size(166, 47);
            this.txtVelocidade.TabIndex = 381;
            this.txtVelocidade.TabStop = false;
            this.txtVelocidade.Tag = "0";
            this.txtVelocidade.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtVelocidade.TextChanged += new System.EventHandler(this.txtVelocidade_TextChanged);
            // 
            // lblVelocidade
            // 
            this.lblVelocidade.AutoSize = true;
            this.lblVelocidade.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblVelocidade.Font = new System.Drawing.Font("Segoe UI", 12.75F);
            this.lblVelocidade.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblVelocidade.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblVelocidade.Location = new System.Drawing.Point(12, 245);
            this.lblVelocidade.Margin = new System.Windows.Forms.Padding(0);
            this.lblVelocidade.Name = "lblVelocidade";
            this.lblVelocidade.Size = new System.Drawing.Size(97, 23);
            this.lblVelocidade.TabIndex = 380;
            this.lblVelocidade.Text = "Velocidade:";
            this.lblVelocidade.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAceleracao
            // 
            this.txtAceleracao.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.txtAceleracao.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtAceleracao.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAceleracao.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtAceleracao.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txtAceleracao.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtAceleracao.Location = new System.Drawing.Point(446, 114);
            this.txtAceleracao.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.txtAceleracao.MaxLength = 100;
            this.txtAceleracao.Name = "txtAceleracao";
            this.txtAceleracao.Size = new System.Drawing.Size(166, 47);
            this.txtAceleracao.TabIndex = 383;
            this.txtAceleracao.TabStop = false;
            this.txtAceleracao.Tag = "0";
            this.txtAceleracao.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblAceleracao
            // 
            this.lblAceleracao.AutoSize = true;
            this.lblAceleracao.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblAceleracao.Font = new System.Drawing.Font("Segoe UI", 12.75F);
            this.lblAceleracao.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblAceleracao.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAceleracao.Location = new System.Drawing.Point(326, 130);
            this.lblAceleracao.Margin = new System.Windows.Forms.Padding(0);
            this.lblAceleracao.Name = "lblAceleracao";
            this.lblAceleracao.Size = new System.Drawing.Size(97, 23);
            this.lblAceleracao.TabIndex = 382;
            this.lblAceleracao.Text = "Aceleração:";
            this.lblAceleracao.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtDelay
            // 
            this.txtDelay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.txtDelay.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtDelay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDelay.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtDelay.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txtDelay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtDelay.Location = new System.Drawing.Point(446, 171);
            this.txtDelay.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.txtDelay.MaxLength = 100;
            this.txtDelay.Name = "txtDelay";
            this.txtDelay.Size = new System.Drawing.Size(166, 47);
            this.txtDelay.TabIndex = 385;
            this.txtDelay.TabStop = false;
            this.txtDelay.Tag = "0";
            this.txtDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblDelay
            // 
            this.lblDelay.AutoSize = true;
            this.lblDelay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblDelay.Font = new System.Drawing.Font("Segoe UI", 12.75F);
            this.lblDelay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblDelay.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDelay.Location = new System.Drawing.Point(326, 187);
            this.lblDelay.Margin = new System.Windows.Forms.Padding(0);
            this.lblDelay.Name = "lblDelay";
            this.lblDelay.Size = new System.Drawing.Size(56, 23);
            this.lblDelay.TabIndex = 384;
            this.lblDelay.Text = "Delay:";
            this.lblDelay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPulsoReverso
            // 
            this.txtPulsoReverso.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.txtPulsoReverso.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtPulsoReverso.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPulsoReverso.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtPulsoReverso.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txtPulsoReverso.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtPulsoReverso.Location = new System.Drawing.Point(446, 229);
            this.txtPulsoReverso.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.txtPulsoReverso.MaxLength = 100;
            this.txtPulsoReverso.Name = "txtPulsoReverso";
            this.txtPulsoReverso.Size = new System.Drawing.Size(166, 47);
            this.txtPulsoReverso.TabIndex = 387;
            this.txtPulsoReverso.TabStop = false;
            this.txtPulsoReverso.Tag = "0";
            this.txtPulsoReverso.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblPusloRev
            // 
            this.lblPusloRev.AutoSize = true;
            this.lblPusloRev.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblPusloRev.Font = new System.Drawing.Font("Segoe UI", 12.75F);
            this.lblPusloRev.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblPusloRev.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPusloRev.Location = new System.Drawing.Point(326, 245);
            this.lblPusloRev.Margin = new System.Windows.Forms.Padding(0);
            this.lblPusloRev.Name = "lblPusloRev";
            this.lblPusloRev.Size = new System.Drawing.Size(119, 23);
            this.lblPusloRev.TabIndex = 386;
            this.lblPusloRev.Text = "Pulso Reverso:";
            this.lblPusloRev.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fCalibracao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 304);
            this.Controls.Add(this.txtPulsoReverso);
            this.Controls.Add(this.lblPusloRev);
            this.Controls.Add(this.txtDelay);
            this.Controls.Add(this.lblDelay);
            this.Controls.Add(this.txtAceleracao);
            this.Controls.Add(this.lblAceleracao);
            this.Controls.Add(this.txtVelocidade);
            this.Controls.Add(this.lblVelocidade);
            this.Controls.Add(this.txtPulsos);
            this.Controls.Add(this.lblPulsos);
            this.Controls.Add(this.txtVolume);
            this.Controls.Add(this.lblVolume);
            this.Controls.Add(this.pnlBarraTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fCalibracao";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "fCalibracao";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.fCalibracao_Load);
            this.pnlBarraTitulo.ResumeLayout(false);
            this.pnlBarraTitulo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlBarraTitulo;
        private System.Windows.Forms.Button btnConfirmar;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblNomeColorante;
        private System.Windows.Forms.Label lblVolume;
        private Percolore.Core.UserControl.UTextBox txtVolume;
        private Percolore.Core.UserControl.UTextBox txtPulsos;
        private System.Windows.Forms.Label lblPulsos;
        private Percolore.Core.UserControl.UTextBox txtVelocidade;
        private System.Windows.Forms.Label lblVelocidade;
        private Percolore.Core.UserControl.UTextBox txtAceleracao;
        private System.Windows.Forms.Label lblAceleracao;
        private Percolore.Core.UserControl.UTextBox txtDelay;
        private System.Windows.Forms.Label lblDelay;
        private Percolore.Core.UserControl.UTextBox txtPulsoReverso;
        private System.Windows.Forms.Label lblPusloRev;
    }
}