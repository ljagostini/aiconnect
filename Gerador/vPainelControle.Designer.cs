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
			pnlBarraTitulo = new Panel();
			btnSair = new Button();
			lblTitulo = new Label();
			btnLicense = new Button();
			btnAccessToken = new Button();
			btnMaintenanceValidity = new Button();
			chkHabilitarTecladoVirtual = new CheckBox();
			pnlBarraTitulo.SuspendLayout();
			SuspendLayout();
			// 
			// pnlBarraTitulo
			// 
			pnlBarraTitulo.BackColor = Color.FromArgb(105, 34, 127);
			pnlBarraTitulo.Controls.Add(btnSair);
			pnlBarraTitulo.Controls.Add(lblTitulo);
			pnlBarraTitulo.Dock = DockStyle.Top;
			pnlBarraTitulo.ForeColor = Color.FromArgb(250, 250, 250);
			pnlBarraTitulo.Location = new Point(0, 0);
			pnlBarraTitulo.Margin = new Padding(4, 5, 4, 5);
			pnlBarraTitulo.Name = "pnlBarraTitulo";
			pnlBarraTitulo.Size = new Size(491, 98);
			pnlBarraTitulo.TabIndex = 3;
			// 
			// btnSair
			// 
			btnSair.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			btnSair.FlatAppearance.BorderColor = Color.FromArgb(39, 138, 228);
			btnSair.FlatAppearance.BorderSize = 0;
			btnSair.FlatStyle = FlatStyle.Flat;
			btnSair.Font = new Font("Segoe UI Light", 12.75F);
			btnSair.Location = new Point(405, 0);
			btnSair.Margin = new Padding(4, 5, 4, 5);
			btnSair.Name = "btnSair";
			btnSair.Size = new Size(85, 98);
			btnSair.TabIndex = 0;
			btnSair.Tag = "0";
			btnSair.Text = "Sair";
			btnSair.UseVisualStyleBackColor = false;
			btnSair.Click += btnSair_Click;
			// 
			// lblTitulo
			// 
			lblTitulo.AutoSize = true;
			lblTitulo.FlatStyle = FlatStyle.Flat;
			lblTitulo.Font = new Font("Segoe UI", 12.75F);
			lblTitulo.ForeColor = Color.FromArgb(250, 250, 250);
			lblTitulo.ImageAlign = ContentAlignment.MiddleLeft;
			lblTitulo.Location = new Point(16, 32);
			lblTitulo.Margin = new Padding(0);
			lblTitulo.Name = "lblTitulo";
			lblTitulo.Size = new Size(195, 30);
			lblTitulo.TabIndex = 309;
			lblTitulo.Text = "Gerador de chaves";
			lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// btnLicense
			// 
			btnLicense.BackColor = Color.FromArgb(105, 34, 127);
			btnLicense.FlatAppearance.BorderSize = 0;
			btnLicense.FlatStyle = FlatStyle.Flat;
			btnLicense.Font = new Font("Segoe UI Light", 12.75F);
			btnLicense.ForeColor = Color.FromArgb(250, 250, 250);
			btnLicense.Location = new Point(21, 134);
			btnLicense.Margin = new Padding(4, 5, 4, 5);
			btnLicense.Name = "btnLicense";
			btnLicense.Size = new Size(449, 95);
			btnLicense.TabIndex = 0;
			btnLicense.Tag = "0";
			btnLicense.Text = "Licença de ativação de software";
			btnLicense.UseVisualStyleBackColor = false;
			btnLicense.Click += btnLicense_Click;
			// 
			// btnAccessToken
			// 
			btnAccessToken.BackColor = Color.FromArgb(105, 34, 127);
			btnAccessToken.FlatAppearance.BorderSize = 0;
			btnAccessToken.FlatStyle = FlatStyle.Flat;
			btnAccessToken.Font = new Font("Segoe UI Light", 12.75F);
			btnAccessToken.ForeColor = Color.FromArgb(250, 250, 250);
			btnAccessToken.Location = new Point(21, 238);
			btnAccessToken.Margin = new Padding(4, 5, 4, 5);
			btnAccessToken.Name = "btnAccessToken";
			btnAccessToken.Size = new Size(449, 95);
			btnAccessToken.TabIndex = 1;
			btnAccessToken.Tag = "0";
			btnAccessToken.Text = "Token de acesso do usuário";
			btnAccessToken.UseVisualStyleBackColor = false;
			btnAccessToken.Click += btnAccessToken_Click;
			// 
			// btnMaintenanceValidity
			// 
			btnMaintenanceValidity.BackColor = Color.FromArgb(105, 34, 127);
			btnMaintenanceValidity.FlatAppearance.BorderSize = 0;
			btnMaintenanceValidity.FlatStyle = FlatStyle.Flat;
			btnMaintenanceValidity.Font = new Font("Segoe UI Light", 12.75F);
			btnMaintenanceValidity.ForeColor = Color.FromArgb(250, 250, 250);
			btnMaintenanceValidity.Location = new Point(21, 343);
			btnMaintenanceValidity.Margin = new Padding(4, 5, 4, 5);
			btnMaintenanceValidity.Name = "btnMaintenanceValidity";
			btnMaintenanceValidity.Size = new Size(449, 95);
			btnMaintenanceValidity.TabIndex = 2;
			btnMaintenanceValidity.Tag = "0";
			btnMaintenanceValidity.Text = "Token de validade da manutenção";
			btnMaintenanceValidity.UseVisualStyleBackColor = false;
			btnMaintenanceValidity.Click += btnMaintenanceValidity_Click;
			// 
			// chkHabilitarTecladoVirtual
			// 
			chkHabilitarTecladoVirtual.AutoSize = true;
			chkHabilitarTecladoVirtual.BackColor = Color.Transparent;
			chkHabilitarTecladoVirtual.ForeColor = Color.FromArgb(105, 34, 127);
			chkHabilitarTecladoVirtual.Location = new Point(21, 106);
			chkHabilitarTecladoVirtual.Name = "chkHabilitarTecladoVirtual";
			chkHabilitarTecladoVirtual.Size = new Size(188, 24);
			chkHabilitarTecladoVirtual.TabIndex = 4;
			chkHabilitarTecladoVirtual.Text = "Habilitar teclado virtual";
			chkHabilitarTecladoVirtual.UseVisualStyleBackColor = false;
			chkHabilitarTecladoVirtual.CheckedChanged += chkHabilitarTecladoVirtual_CheckedChanged;
			// 
			// vPainelControle
			// 
			AutoScaleDimensions = new SizeF(8F, 20F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.FromArgb(250, 250, 250);
			ClientSize = new Size(491, 449);
			Controls.Add(chkHabilitarTecladoVirtual);
			Controls.Add(btnMaintenanceValidity);
			Controls.Add(btnAccessToken);
			Controls.Add(btnLicense);
			Controls.Add(pnlBarraTitulo);
			ForeColor = Color.FromArgb(120, 120, 120);
			FormBorderStyle = FormBorderStyle.None;
			Margin = new Padding(4, 5, 4, 5);
			Name = "vPainelControle";
			SizeGripStyle = SizeGripStyle.Hide;
			StartPosition = FormStartPosition.Manual;
			Paint += vPainelControle_Paint;
			pnlBarraTitulo.ResumeLayout(false);
			pnlBarraTitulo.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Panel pnlBarraTitulo;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnLicense;
        private System.Windows.Forms.Button btnAccessToken;
        private System.Windows.Forms.Button btnMaintenanceValidity;
		private CheckBox chkHabilitarTecladoVirtual;
	}
}

