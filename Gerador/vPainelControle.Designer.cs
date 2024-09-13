namespace Percolore.Gerador
{
    partial class vPainelControle
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
            this.btnLicense = new System.Windows.Forms.Button();
            this.btnAccessToken = new System.Windows.Forms.Button();
            this.btnMaintenanceValidity = new System.Windows.Forms.Button();
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
            this.pnlBarraTitulo.Size = new System.Drawing.Size(368, 64);
            this.pnlBarraTitulo.TabIndex = 3;
            // 
            // btnSair
            // 
            this.btnSair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSair.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(138)))), ((int)(((byte)(228)))));
            this.btnSair.FlatAppearance.BorderSize = 0;
            this.btnSair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSair.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnSair.Location = new System.Drawing.Point(304, 0);
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
            this.lblTitulo.Size = new System.Drawing.Size(152, 23);
            this.lblTitulo.TabIndex = 309;
            this.lblTitulo.Text = "Gerador de chaves";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnLicense
            // 
            this.btnLicense.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(34)))), ((int)(((byte)(127)))));
            this.btnLicense.FlatAppearance.BorderSize = 0;
            this.btnLicense.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLicense.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnLicense.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnLicense.Location = new System.Drawing.Point(16, 79);
            this.btnLicense.Name = "btnLicense";
            this.btnLicense.Size = new System.Drawing.Size(337, 62);
            this.btnLicense.TabIndex = 0;
            this.btnLicense.Tag = "0";
            this.btnLicense.Text = "Licença de ativação de software";
            this.btnLicense.UseVisualStyleBackColor = false;
            this.btnLicense.Click += new System.EventHandler(this.btnLicense_Click);
            // 
            // btnAccessToken
            // 
            this.btnAccessToken.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(34)))), ((int)(((byte)(127)))));
            this.btnAccessToken.FlatAppearance.BorderSize = 0;
            this.btnAccessToken.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAccessToken.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnAccessToken.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnAccessToken.Location = new System.Drawing.Point(16, 147);
            this.btnAccessToken.Name = "btnAccessToken";
            this.btnAccessToken.Size = new System.Drawing.Size(337, 62);
            this.btnAccessToken.TabIndex = 1;
            this.btnAccessToken.Tag = "0";
            this.btnAccessToken.Text = "Token de acesso do usuário";
            this.btnAccessToken.UseVisualStyleBackColor = false;
            this.btnAccessToken.Click += new System.EventHandler(this.btnAccessToken_Click);
            // 
            // btnMaintenanceValidity
            // 
            this.btnMaintenanceValidity.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(34)))), ((int)(((byte)(127)))));
            this.btnMaintenanceValidity.FlatAppearance.BorderSize = 0;
            this.btnMaintenanceValidity.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMaintenanceValidity.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnMaintenanceValidity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnMaintenanceValidity.Location = new System.Drawing.Point(16, 215);
            this.btnMaintenanceValidity.Name = "btnMaintenanceValidity";
            this.btnMaintenanceValidity.Size = new System.Drawing.Size(337, 62);
            this.btnMaintenanceValidity.TabIndex = 2;
            this.btnMaintenanceValidity.Tag = "0";
            this.btnMaintenanceValidity.Text = "Token de validade da manutenção";
            this.btnMaintenanceValidity.UseVisualStyleBackColor = false;
            this.btnMaintenanceValidity.Click += new System.EventHandler(this.btnMaintenanceValidity_Click);
            // 
            // vPainelControle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(368, 292);
            this.Controls.Add(this.btnMaintenanceValidity);
            this.Controls.Add(this.btnAccessToken);
            this.Controls.Add(this.btnLicense);
            this.Controls.Add(this.pnlBarraTitulo);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "vPainelControle";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.vPainelControle_Paint);
            this.pnlBarraTitulo.ResumeLayout(false);
            this.pnlBarraTitulo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlBarraTitulo;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnLicense;
        private System.Windows.Forms.Button btnAccessToken;
        private System.Windows.Forms.Button btnMaintenanceValidity;
    }
}

