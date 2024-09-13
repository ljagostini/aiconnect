namespace Percolore.IOConnect
{
    partial class fCaminhoArquivo
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
            this.btnConfirmar = new System.Windows.Forms.Button();
            this.btnSair = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblDiretorio = new System.Windows.Forms.Label();
            this.btnEditarDiretorio = new System.Windows.Forms.Button();
            this.txtDiretorio = new Percolore.Core.UserControl.UTextBox();
            this.txtArquivo = new Percolore.Core.UserControl.UTextBox();
            this.lblNome = new System.Windows.Forms.Label();
            this.pnlBarraTitulo.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBarraTitulo
            // 
            this.pnlBarraTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(145)))), ((int)(((byte)(204)))));
            this.pnlBarraTitulo.Controls.Add(this.btnConfirmar);
            this.pnlBarraTitulo.Controls.Add(this.btnSair);
            this.pnlBarraTitulo.Controls.Add(this.lblTitulo);
            this.pnlBarraTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBarraTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.pnlBarraTitulo.Location = new System.Drawing.Point(0, 0);
            this.pnlBarraTitulo.Name = "pnlBarraTitulo";
            this.pnlBarraTitulo.Size = new System.Drawing.Size(900, 64);
            this.pnlBarraTitulo.TabIndex = 374;
            // 
            // btnConfirmar
            // 
            this.btnConfirmar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirmar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(145)))), ((int)(((byte)(204)))));
            this.btnConfirmar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(138)))), ((int)(((byte)(228)))));
            this.btnConfirmar.FlatAppearance.BorderSize = 0;
            this.btnConfirmar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirmar.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnConfirmar.Location = new System.Drawing.Point(769, 0);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(64, 64);
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
            this.btnSair.Location = new System.Drawing.Point(836, 0);
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
            this.lblTitulo.Size = new System.Drawing.Size(138, 23);
            this.lblTitulo.TabIndex = 309;
            this.lblTitulo.Text = "Informar arquivo";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDiretorio
            // 
            this.lblDiretorio.AutoSize = true;
            this.lblDiretorio.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblDiretorio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblDiretorio.Location = new System.Drawing.Point(12, 80);
            this.lblDiretorio.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblDiretorio.Name = "lblDiretorio";
            this.lblDiretorio.Size = new System.Drawing.Size(73, 23);
            this.lblDiretorio.TabIndex = 416;
            this.lblDiretorio.Text = "Diretório";
            // 
            // btnEditarDiretorio
            // 
            this.btnEditarDiretorio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditarDiretorio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(145)))), ((int)(((byte)(204)))));
            this.btnEditarDiretorio.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditarDiretorio.Font = new System.Drawing.Font("Segoe UI Light", 20.75F);
            this.btnEditarDiretorio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnEditarDiretorio.Location = new System.Drawing.Point(839, 106);
            this.btnEditarDiretorio.Name = "btnEditarDiretorio";
            this.btnEditarDiretorio.Size = new System.Drawing.Size(47, 48);
            this.btnEditarDiretorio.TabIndex = 1;
            this.btnEditarDiretorio.Tag = "0";
            this.btnEditarDiretorio.UseVisualStyleBackColor = false;
            this.btnEditarDiretorio.Click += new System.EventHandler(this.btnEditarDiretorio_Click);
            // 
            // txtDiretorio
            // 
            this.txtDiretorio.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDiretorio.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtDiretorio.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtDiretorio.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDiretorio.Conteudo = Percolore.Core.UserControl.TipoConteudo.Padrao;
            this.txtDiretorio.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txtDiretorio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtDiretorio.Location = new System.Drawing.Point(16, 107);
            this.txtDiretorio.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.txtDiretorio.Name = "txtDiretorio";
            this.txtDiretorio.ReadOnly = true;
            this.txtDiretorio.Size = new System.Drawing.Size(820, 47);
            this.txtDiretorio.TabIndex = 0;
            this.txtDiretorio.Tag = "0";
            // 
            // txtArquivo
            // 
            this.txtArquivo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtArquivo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.txtArquivo.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtArquivo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtArquivo.Conteudo = Percolore.Core.UserControl.TipoConteudo.Padrao;
            this.txtArquivo.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txtArquivo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtArquivo.Location = new System.Drawing.Point(16, 189);
            this.txtArquivo.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.txtArquivo.MaxLength = 255;
            this.txtArquivo.Name = "txtArquivo";
            this.txtArquivo.Size = new System.Drawing.Size(446, 47);
            this.txtArquivo.TabIndex = 2;
            this.txtArquivo.Tag = "0";
            // 
            // lblNome
            // 
            this.lblNome.AutoSize = true;
            this.lblNome.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblNome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblNome.Location = new System.Drawing.Point(12, 162);
            this.lblNome.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblNome.Name = "lblNome";
            this.lblNome.Size = new System.Drawing.Size(55, 23);
            this.lblNome.TabIndex = 420;
            this.lblNome.Text = "Nome";
            // 
            // fCaminhoArquivo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(900, 252);
            this.Controls.Add(this.lblNome);
            this.Controls.Add(this.txtArquivo);
            this.Controls.Add(this.btnEditarDiretorio);
            this.Controls.Add(this.txtDiretorio);
            this.Controls.Add(this.lblDiretorio);
            this.Controls.Add(this.pnlBarraTitulo);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(0, 100);
            this.Name = "fCaminhoArquivo";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Editar nível";
            this.Load += new System.EventHandler(this.fCaminhoArquivo_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.EditarVolumeColorante_Paint);
            this.pnlBarraTitulo.ResumeLayout(false);
            this.pnlBarraTitulo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel pnlBarraTitulo;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnConfirmar;
        private System.Windows.Forms.Label lblDiretorio;
        private System.Windows.Forms.Button btnEditarDiretorio;
        private Percolore.Core.UserControl.UTextBox txtDiretorio;
        private Percolore.Core.UserControl.UTextBox txtArquivo;
        private System.Windows.Forms.Label lblNome;
    }
}