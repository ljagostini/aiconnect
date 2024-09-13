namespace Percolore.IOConnect
{
    partial class fLicenca
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Disposes resources used by the form.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtChave = new Percolore.Core.UserControl.UTextBox();
            this.lblLicenca = new System.Windows.Forms.Label();
            this.txtSerial = new Percolore.Core.UserControl.UTextBox();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.lblChave = new System.Windows.Forms.Label();
            this.btnAtivar = new System.Windows.Forms.Button();
            this.btnOnLine = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1024, 350);
            this.tableLayoutPanel1.TabIndex = 317;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.btnOnLine);
            this.panel1.Controls.Add(this.lblTitulo);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtChave);
            this.panel1.Controls.Add(this.lblLicenca);
            this.panel1.Controls.Add(this.txtSerial);
            this.panel1.Controls.Add(this.btnCancelar);
            this.panel1.Controls.Add(this.lblChave);
            this.panel1.Controls.Add(this.btnAtivar);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(237, 44);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(549, 261);
            this.panel1.TabIndex = 318;
            // 
            // lblTitulo
            // 
            this.lblTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitulo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.lblTitulo.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblTitulo.Location = new System.Drawing.Point(99, 0);
            this.lblTitulo.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(447, 41);
            this.lblTitulo.TabIndex = 318;
            this.lblTitulo.Text = "Ativação de software";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.label2.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label2.Location = new System.Drawing.Point(1, 61);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 23);
            this.label2.TabIndex = 316;
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtChave
            // 
            this.txtChave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.txtChave.BorderColor = System.Drawing.Color.Gray;
            this.txtChave.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtChave.Conteudo = Percolore.Core.UserControl.TipoConteudo.Padrao;
            this.txtChave.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txtChave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.txtChave.Location = new System.Drawing.Point(99, 151);
            this.txtChave.Name = "txtChave";
            this.txtChave.Size = new System.Drawing.Size(447, 47);
            this.txtChave.TabIndex = 1;
            // 
            // lblLicenca
            // 
            this.lblLicenca.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLicenca.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblLicenca.Font = new System.Drawing.Font("Segoe UI", 12.75F);
            this.lblLicenca.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.lblLicenca.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblLicenca.Location = new System.Drawing.Point(7, 163);
            this.lblLicenca.Margin = new System.Windows.Forms.Padding(0);
            this.lblLicenca.Name = "lblLicenca";
            this.lblLicenca.Size = new System.Drawing.Size(84, 23);
            this.lblLicenca.TabIndex = 314;
            this.lblLicenca.Text = "Licença";
            this.lblLicenca.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSerial
            // 
            this.txtSerial.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.txtSerial.BorderColor = System.Drawing.Color.Gray;
            this.txtSerial.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSerial.Conteudo = Percolore.Core.UserControl.TipoConteudo.Padrao;
            this.txtSerial.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.txtSerial.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.txtSerial.Location = new System.Drawing.Point(99, 88);
            this.txtSerial.MaxLength = 12;
            this.txtSerial.Name = "txtSerial";
            this.txtSerial.ReadOnly = true;
            this.txtSerial.Size = new System.Drawing.Size(447, 47);
            this.txtSerial.TabIndex = 0;
            // 
            // btnCancelar
            // 
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnCancelar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnCancelar.Location = new System.Drawing.Point(434, 206);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(52, 52);
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Tag = "0";
            this.btnCancelar.Text = "CAN";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // lblChave
            // 
            this.lblChave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblChave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblChave.Font = new System.Drawing.Font("Segoe UI", 12.75F);
            this.lblChave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.lblChave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblChave.Location = new System.Drawing.Point(3, 100);
            this.lblChave.Margin = new System.Windows.Forms.Padding(0);
            this.lblChave.Name = "lblChave";
            this.lblChave.Size = new System.Drawing.Size(88, 23);
            this.lblChave.TabIndex = 310;
            this.lblChave.Text = "Chave";
            this.lblChave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnAtivar
            // 
            this.btnAtivar.FlatAppearance.BorderSize = 0;
            this.btnAtivar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAtivar.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnAtivar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnAtivar.Location = new System.Drawing.Point(494, 206);
            this.btnAtivar.Name = "btnAtivar";
            this.btnAtivar.Size = new System.Drawing.Size(52, 52);
            this.btnAtivar.TabIndex = 4;
            this.btnAtivar.Tag = "0";
            this.btnAtivar.Text = "OK";
            this.btnAtivar.UseVisualStyleBackColor = false;
            this.btnAtivar.Click += new System.EventHandler(this.btnAtivar_Click);
            // 
            // btnOnLine
            // 
            this.btnOnLine.Enabled = false;
            this.btnOnLine.FlatAppearance.BorderSize = 0;
            this.btnOnLine.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnOnLine.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnOnLine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOnLine.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnOnLine.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnOnLine.Location = new System.Drawing.Point(354, 204);
            this.btnOnLine.Name = "btnOnLine";
            this.btnOnLine.Size = new System.Drawing.Size(74, 52);
            this.btnOnLine.TabIndex = 424;
            this.btnOnLine.Tag = "0";
            this.btnOnLine.Text = "OnLine";
            this.btnOnLine.UseVisualStyleBackColor = false;
            this.btnOnLine.Visible = false;
            this.btnOnLine.Click += new System.EventHandler(this.btnOnLine_Click);
            // 
            // fLicenca
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(1024, 350);
            this.Controls.Add(this.tableLayoutPanel1);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "fLicenca";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label label2;
        private Percolore.Core.UserControl.UTextBox txtChave;
        private System.Windows.Forms.Label lblLicenca;
        private Percolore.Core.UserControl.UTextBox txtSerial;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Label lblChave;
        private System.Windows.Forms.Button btnAtivar;
        private System.Windows.Forms.Button btnOnLine;
    }
}
