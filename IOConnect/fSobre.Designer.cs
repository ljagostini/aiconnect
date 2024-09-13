namespace Percolore.IOConnect
{
    partial class fSobre
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
            this.label_0 = new System.Windows.Forms.Label();
            this.label_3 = new System.Windows.Forms.Label();
            this.lblVersao = new System.Windows.Forms.Label();
            this.lblLegendaSerial = new System.Windows.Forms.Label();
            this.lblSerial = new System.Windows.Forms.Label();
            this.label_5 = new System.Windows.Forms.Label();
            this.lblValidadeManutencao = new System.Windows.Forms.Label();
            this.lnkRedefinirManutencao = new System.Windows.Forms.LinkLabel();
            this.lblDataInstalação = new System.Windows.Forms.Label();
            this.label_4 = new System.Windows.Forms.Label();
            this.label_6 = new System.Windows.Forms.Label();
            this.label_7 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cboDispositivo = new Percolore.Core.UserControl.UComboBox();
            this.cboIdioma = new Percolore.Core.UserControl.UComboBox();
            this.btnStatusDump = new System.Windows.Forms.Button();
            this.pnlBarraTitulo.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBarraTitulo
            // 
            this.pnlBarraTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(81)))), ((int)(((byte)(76)))));
            this.pnlBarraTitulo.Controls.Add(this.btnSair);
            this.pnlBarraTitulo.Controls.Add(this.label_0);
            this.pnlBarraTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBarraTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.pnlBarraTitulo.Location = new System.Drawing.Point(0, 0);
            this.pnlBarraTitulo.Name = "pnlBarraTitulo";
            this.pnlBarraTitulo.Size = new System.Drawing.Size(607, 64);
            this.pnlBarraTitulo.TabIndex = 1;
            // 
            // btnSair
            // 
            this.btnSair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSair.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(138)))), ((int)(((byte)(228)))));
            this.btnSair.FlatAppearance.BorderSize = 0;
            this.btnSair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSair.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnSair.Location = new System.Drawing.Point(543, 0);
            this.btnSair.Margin = new System.Windows.Forms.Padding(0);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(64, 64);
            this.btnSair.TabIndex = 1;
            this.btnSair.Tag = "0";
            this.btnSair.Text = "Sair";
            this.btnSair.UseVisualStyleBackColor = false;
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click);
            // 
            // label_0
            // 
            this.label_0.AutoSize = true;
            this.label_0.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label_0.Font = new System.Drawing.Font("Segoe UI", 12.75F);
            this.label_0.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.label_0.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_0.Location = new System.Drawing.Point(12, 21);
            this.label_0.Margin = new System.Windows.Forms.Padding(0);
            this.label_0.Name = "label_0";
            this.label_0.Size = new System.Drawing.Size(156, 23);
            this.label_0.TabIndex = 309;
            this.label_0.Text = "Sobre o IOConnect";
            this.label_0.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_3
            // 
            this.label_3.AccessibleDescription = "lblSerial";
            this.label_3.AutoSize = true;
            this.label_3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_3.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.label_3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.label_3.Location = new System.Drawing.Point(15, 49);
            this.label_3.Margin = new System.Windows.Forms.Padding(0);
            this.label_3.Name = "label_3";
            this.label_3.Size = new System.Drawing.Size(190, 28);
            this.label_3.TabIndex = 417;
            this.label_3.Text = "Versão do software:";
            this.label_3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblVersao
            // 
            this.lblVersao.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblVersao.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.lblVersao.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblVersao.Location = new System.Drawing.Point(205, 49);
            this.lblVersao.Margin = new System.Windows.Forms.Padding(0);
            this.lblVersao.Name = "lblVersao";
            this.lblVersao.Size = new System.Drawing.Size(106, 28);
            this.lblVersao.TabIndex = 2;
            this.lblVersao.Text = "0.0.0.0000";
            this.lblVersao.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLegendaSerial
            // 
            this.lblLegendaSerial.AutoSize = true;
            this.lblLegendaSerial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLegendaSerial.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.lblLegendaSerial.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblLegendaSerial.Location = new System.Drawing.Point(15, 15);
            this.lblLegendaSerial.Margin = new System.Windows.Forms.Padding(0);
            this.lblLegendaSerial.Name = "lblLegendaSerial";
            this.lblLegendaSerial.Size = new System.Drawing.Size(190, 28);
            this.lblLegendaSerial.TabIndex = 419;
            this.lblLegendaSerial.Text = "Número de série:";
            this.lblLegendaSerial.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSerial
            // 
            this.lblSerial.AutoSize = true;
            this.lblSerial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSerial.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.lblSerial.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblSerial.Location = new System.Drawing.Point(205, 15);
            this.lblSerial.Margin = new System.Windows.Forms.Padding(0);
            this.lblSerial.Name = "lblSerial";
            this.lblSerial.Size = new System.Drawing.Size(106, 28);
            this.lblSerial.TabIndex = 0;
            this.lblSerial.Text = "0000";
            this.lblSerial.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_5
            // 
            this.label_5.AutoSize = true;
            this.label_5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_5.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.label_5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.label_5.Location = new System.Drawing.Point(15, 117);
            this.label_5.Margin = new System.Windows.Forms.Padding(0);
            this.label_5.Name = "label_5";
            this.label_5.Size = new System.Drawing.Size(190, 28);
            this.label_5.TabIndex = 426;
            this.label_5.Text = "Validade da manutenção:";
            this.label_5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblValidadeManutencao
            // 
            this.lblValidadeManutencao.AutoSize = true;
            this.lblValidadeManutencao.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblValidadeManutencao.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.lblValidadeManutencao.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblValidadeManutencao.Location = new System.Drawing.Point(205, 117);
            this.lblValidadeManutencao.Margin = new System.Windows.Forms.Padding(0);
            this.lblValidadeManutencao.Name = "lblValidadeManutencao";
            this.lblValidadeManutencao.Size = new System.Drawing.Size(106, 28);
            this.lblValidadeManutencao.TabIndex = 4;
            this.lblValidadeManutencao.Text = "00/00/0000";
            this.lblValidadeManutencao.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lnkRedefinirManutencao
            // 
            this.lnkRedefinirManutencao.AutoSize = true;
            this.lnkRedefinirManutencao.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lnkRedefinirManutencao.Font = new System.Drawing.Font("Segoe UI Light", 10.75F);
            this.lnkRedefinirManutencao.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkRedefinirManutencao.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(81)))), ((int)(((byte)(76)))));
            this.lnkRedefinirManutencao.Location = new System.Drawing.Point(311, 117);
            this.lnkRedefinirManutencao.Margin = new System.Windows.Forms.Padding(0);
            this.lnkRedefinirManutencao.Name = "lnkRedefinirManutencao";
            this.lnkRedefinirManutencao.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lnkRedefinirManutencao.Size = new System.Drawing.Size(279, 28);
            this.lnkRedefinirManutencao.TabIndex = 0;
            this.lnkRedefinirManutencao.TabStop = true;
            this.lnkRedefinirManutencao.Text = "[Redefinir]";
            this.lnkRedefinirManutencao.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lnkRedefinirManutencao.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkRedefineManutencao_LinkClicked);
            // 
            // lblDataInstalação
            // 
            this.lblDataInstalação.AutoSize = true;
            this.lblDataInstalação.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataInstalação.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.lblDataInstalação.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblDataInstalação.Location = new System.Drawing.Point(205, 83);
            this.lblDataInstalação.Margin = new System.Windows.Forms.Padding(0);
            this.lblDataInstalação.Name = "lblDataInstalação";
            this.lblDataInstalação.Size = new System.Drawing.Size(106, 28);
            this.lblDataInstalação.TabIndex = 3;
            this.lblDataInstalação.Text = "00/00/0000";
            this.lblDataInstalação.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_4
            // 
            this.label_4.AutoSize = true;
            this.label_4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_4.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.label_4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.label_4.Location = new System.Drawing.Point(15, 83);
            this.label_4.Margin = new System.Windows.Forms.Padding(0);
            this.label_4.Name = "label_4";
            this.label_4.Size = new System.Drawing.Size(190, 28);
            this.label_4.TabIndex = 435;
            this.label_4.Text = "Data de instalação:";
            this.label_4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_6
            // 
            this.label_6.AutoSize = true;
            this.label_6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_6.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.label_6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.label_6.Location = new System.Drawing.Point(15, 209);
            this.label_6.Margin = new System.Windows.Forms.Padding(0);
            this.label_6.Name = "label_6";
            this.label_6.Size = new System.Drawing.Size(190, 48);
            this.label_6.TabIndex = 437;
            this.label_6.Text = "Dispositivo:";
            this.label_6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_7
            // 
            this.label_7.AutoSize = true;
            this.label_7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_7.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.label_7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.label_7.Location = new System.Drawing.Point(15, 155);
            this.label_7.Margin = new System.Windows.Forms.Padding(0);
            this.label_7.Name = "label_7";
            this.label_7.Size = new System.Drawing.Size(190, 48);
            this.label_7.TabIndex = 439;
            this.label_7.Text = "Idioma:";
            this.label_7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lblLegendaSerial, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cboDispositivo, 1, 10);
            this.tableLayoutPanel1.Controls.Add(this.lnkRedefinirManutencao, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this.label_6, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.label_7, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.lblValidadeManutencao, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.lblDataInstalação, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label_5, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.lblSerial, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label_4, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.cboIdioma, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.label_3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblVersao, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnStatusDump, 2, 12);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1, 65);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(15);
            this.tableLayoutPanel1.RowCount = 13;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(605, 331);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // cboDispositivo
            // 
            this.cboDispositivo.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cboDispositivo.BorderColor = System.Drawing.Color.Gainsboro;
            this.cboDispositivo.BorderSize = 1;
            this.tableLayoutPanel1.SetColumnSpan(this.cboDispositivo, 2);
            this.cboDispositivo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboDispositivo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cboDispositivo.DropDownWidth = 100;
            this.cboDispositivo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboDispositivo.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.cboDispositivo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.cboDispositivo.FormattingEnabled = true;
            this.cboDispositivo.IntegralHeight = false;
            this.cboDispositivo.Location = new System.Drawing.Point(209, 209);
            this.cboDispositivo.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.cboDispositivo.Name = "cboDispositivo";
            this.cboDispositivo.Size = new System.Drawing.Size(381, 48);
            this.cboDispositivo.TabIndex = 2;
            // 
            // cboIdioma
            // 
            this.cboIdioma.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cboIdioma.BorderColor = System.Drawing.Color.Gainsboro;
            this.cboIdioma.BorderSize = 1;
            this.tableLayoutPanel1.SetColumnSpan(this.cboIdioma, 2);
            this.cboIdioma.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboIdioma.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboIdioma.DropDownWidth = 100;
            this.cboIdioma.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboIdioma.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.cboIdioma.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.cboIdioma.FormattingEnabled = true;
            this.cboIdioma.IntegralHeight = false;
            this.cboIdioma.Location = new System.Drawing.Point(209, 155);
            this.cboIdioma.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.cboIdioma.Name = "cboIdioma";
            this.cboIdioma.Size = new System.Drawing.Size(381, 48);
            this.cboIdioma.TabIndex = 1;
            this.cboIdioma.SelectionChangeCommitted += new System.EventHandler(this.cboIdioma_SelectionChangeCommitted);
            // 
            // btnStatusDump
            // 
            this.btnStatusDump.Location = new System.Drawing.Point(314, 266);
            this.btnStatusDump.Name = "btnStatusDump";
            this.btnStatusDump.Size = new System.Drawing.Size(273, 47);
            this.btnStatusDump.TabIndex = 440;
            this.btnStatusDump.Text = "Status";
            this.btnStatusDump.UseVisualStyleBackColor = true;
            this.btnStatusDump.Click += new System.EventHandler(this.btnStatusDump_Click);
            // 
            // fSobre
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(607, 397);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.pnlBarraTitulo);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(0, 100);
            this.Name = "fSobre";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Editar nível";
            this.TopMost = true;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.EditarVolumeColorante_Paint);
            this.pnlBarraTitulo.ResumeLayout(false);
            this.pnlBarraTitulo.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pnlBarraTitulo;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.Label label_0;
        private System.Windows.Forms.Label label_3;
        private System.Windows.Forms.Label lblVersao;
        private System.Windows.Forms.Label lblLegendaSerial;
        private System.Windows.Forms.Label lblSerial;
        private System.Windows.Forms.Label label_5;
        private System.Windows.Forms.Label lblValidadeManutencao;
        private System.Windows.Forms.LinkLabel lnkRedefinirManutencao;
        private System.Windows.Forms.Label lblDataInstalação;
        private System.Windows.Forms.Label label_4;
        private System.Windows.Forms.Label label_6;
        private Percolore.Core.UserControl.UComboBox cboIdioma;
        private Percolore.Core.UserControl.UComboBox cboDispositivo;
        private System.Windows.Forms.Label label_7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnStatusDump;
    }
}