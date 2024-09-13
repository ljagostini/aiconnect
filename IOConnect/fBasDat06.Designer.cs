namespace Percolore.IOConnect
{
    partial class fBasDat06
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
            this.btnExcluir = new System.Windows.Forms.Button();
            this.btnConfirmar = new System.Windows.Forms.Button();
            this.btnSair = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblVolume = new System.Windows.Forms.Label();
            this.lblProduct = new System.Windows.Forms.Label();
            this.lblCircuito = new System.Windows.Forms.Label();
            this.txt_Circuito = new Percolore.Core.UserControl.UTextBox();
            this.txt_Nome = new Percolore.Core.UserControl.UTextBox();
            this.txtVolume = new Percolore.Core.UserControl.UTextBox();
            this.pnlBarraTitulo.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBarraTitulo
            // 
            this.pnlBarraTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(145)))), ((int)(((byte)(204)))));
            this.pnlBarraTitulo.Controls.Add(this.btnExcluir);
            this.pnlBarraTitulo.Controls.Add(this.btnConfirmar);
            this.pnlBarraTitulo.Controls.Add(this.btnSair);
            this.pnlBarraTitulo.Controls.Add(this.lblTitulo);
            this.pnlBarraTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBarraTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.pnlBarraTitulo.Location = new System.Drawing.Point(0, 0);
            this.pnlBarraTitulo.Name = "pnlBarraTitulo";
            this.pnlBarraTitulo.Size = new System.Drawing.Size(489, 110);
            this.pnlBarraTitulo.TabIndex = 376;
            // 
            // btnExcluir
            // 
            this.btnExcluir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExcluir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(145)))), ((int)(((byte)(204)))));
            this.btnExcluir.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(138)))), ((int)(((byte)(228)))));
            this.btnExcluir.FlatAppearance.BorderSize = 0;
            this.btnExcluir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExcluir.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnExcluir.Location = new System.Drawing.Point(222, 14);
            this.btnExcluir.Name = "btnExcluir";
            this.btnExcluir.Size = new System.Drawing.Size(95, 37);
            this.btnExcluir.TabIndex = 310;
            this.btnExcluir.Tag = "0";
            this.btnExcluir.Text = "Excluir";
            this.btnExcluir.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExcluir.UseVisualStyleBackColor = false;
            this.btnExcluir.Click += new System.EventHandler(this.btnExcluir_Click);
            // 
            // btnConfirmar
            // 
            this.btnConfirmar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirmar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(145)))), ((int)(((byte)(204)))));
            this.btnConfirmar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(138)))), ((int)(((byte)(228)))));
            this.btnConfirmar.FlatAppearance.BorderSize = 0;
            this.btnConfirmar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirmar.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnConfirmar.Location = new System.Drawing.Point(326, 11);
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
            this.btnSair.Location = new System.Drawing.Point(425, 0);
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
            this.lblTitulo.Size = new System.Drawing.Size(100, 23);
            this.lblTitulo.TabIndex = 309;
            this.lblTitulo.Text = "Base Dat 06";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblVolume
            // 
            this.lblVolume.AutoSize = true;
            this.lblVolume.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblVolume.Font = new System.Drawing.Font("Segoe UI", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVolume.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblVolume.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblVolume.Location = new System.Drawing.Point(23, 217);
            this.lblVolume.Margin = new System.Windows.Forms.Padding(0);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(75, 23);
            this.lblVolume.TabIndex = 378;
            this.lblVolume.Text = "Volume:";
            this.lblVolume.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblProduct
            // 
            this.lblProduct.AutoSize = true;
            this.lblProduct.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblProduct.Font = new System.Drawing.Font("Segoe UI", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProduct.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblProduct.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblProduct.Location = new System.Drawing.Point(23, 160);
            this.lblProduct.Margin = new System.Windows.Forms.Padding(0);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(63, 23);
            this.lblProduct.TabIndex = 380;
            this.lblProduct.Text = "Nome:";
            this.lblProduct.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCircuito
            // 
            this.lblCircuito.AutoSize = true;
            this.lblCircuito.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblCircuito.Font = new System.Drawing.Font("Segoe UI", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCircuito.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblCircuito.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCircuito.Location = new System.Drawing.Point(23, 275);
            this.lblCircuito.Margin = new System.Windows.Forms.Padding(0);
            this.lblCircuito.Name = "lblCircuito";
            this.lblCircuito.Size = new System.Drawing.Size(78, 23);
            this.lblCircuito.TabIndex = 382;
            this.lblCircuito.Text = "Circuito:";
            this.lblCircuito.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txt_Circuito
            // 
            this.txt_Circuito.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.txt_Circuito.BorderColor = System.Drawing.Color.Gainsboro;
            this.txt_Circuito.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_Circuito.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txt_Circuito.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txt_Circuito.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txt_Circuito.Location = new System.Drawing.Point(123, 259);
            this.txt_Circuito.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.txt_Circuito.MaxLength = 100;
            this.txt_Circuito.Name = "txt_Circuito";
            this.txt_Circuito.Size = new System.Drawing.Size(166, 47);
            this.txt_Circuito.TabIndex = 383;
            this.txt_Circuito.TabStop = false;
            this.txt_Circuito.Tag = "0";
            this.txt_Circuito.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_Nome
            // 
            this.txt_Nome.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.txt_Nome.BorderColor = System.Drawing.Color.Gainsboro;
            this.txt_Nome.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_Nome.Conteudo = Percolore.Core.UserControl.TipoConteudo.Padrao;
            this.txt_Nome.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txt_Nome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txt_Nome.Location = new System.Drawing.Point(123, 144);
            this.txt_Nome.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.txt_Nome.MaxLength = 100;
            this.txt_Nome.Name = "txt_Nome";
            this.txt_Nome.Size = new System.Drawing.Size(166, 47);
            this.txt_Nome.TabIndex = 381;
            this.txt_Nome.TabStop = false;
            this.txt_Nome.Tag = "0";
            this.txt_Nome.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtVolume
            // 
            this.txtVolume.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.txtVolume.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtVolume.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtVolume.Conteudo = Percolore.Core.UserControl.TipoConteudo.Decimal;
            this.txtVolume.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txtVolume.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtVolume.Location = new System.Drawing.Point(123, 201);
            this.txtVolume.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.txtVolume.MaxLength = 100;
            this.txtVolume.Name = "txtVolume";
            this.txtVolume.Size = new System.Drawing.Size(166, 47);
            this.txtVolume.TabIndex = 379;
            this.txtVolume.TabStop = false;
            this.txtVolume.Tag = "0";
            this.txtVolume.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // fBasDat06
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(489, 349);
            this.Controls.Add(this.txt_Circuito);
            this.Controls.Add(this.lblCircuito);
            this.Controls.Add(this.txt_Nome);
            this.Controls.Add(this.lblProduct);
            this.Controls.Add(this.txtVolume);
            this.Controls.Add(this.lblVolume);
            this.Controls.Add(this.pnlBarraTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fBasDat06";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "fBasDat06";
            this.Load += new System.EventHandler(this.fBasDat06_Load);
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
        private System.Windows.Forms.Button btnExcluir;
        private Percolore.Core.UserControl.UTextBox txtVolume;
        private System.Windows.Forms.Label lblVolume;
        private Percolore.Core.UserControl.UTextBox txt_Nome;
        private System.Windows.Forms.Label lblProduct;
        private Percolore.Core.UserControl.UTextBox txt_Circuito;
        private System.Windows.Forms.Label lblCircuito;
    }
}