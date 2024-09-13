namespace Percolore.IOConnect
{
    partial class fInicializacaoCircuitos
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
            this.btnAbortar = new System.Windows.Forms.Button();
            this.progressBar = new Percolore.Core.UserControl.UProgressBar();
            this.btnIniciar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnAbortar
            // 
            this.btnAbortar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbortar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(185)))), ((int)(((byte)(37)))));
            this.btnAbortar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(176)))), ((int)(((byte)(37)))));
            this.btnAbortar.FlatAppearance.BorderSize = 0;
            this.btnAbortar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbortar.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btnAbortar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnAbortar.Location = new System.Drawing.Point(570, 179);
            this.btnAbortar.Margin = new System.Windows.Forms.Padding(5);
            this.btnAbortar.Name = "btnAbortar";
            this.btnAbortar.Size = new System.Drawing.Size(194, 65);
            this.btnAbortar.TabIndex = 1;
            this.btnAbortar.Text = "Abortar";
            this.btnAbortar.UseVisualStyleBackColor = false;
            this.btnAbortar.Click += new System.EventHandler(this.Abortar_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(200)))), ((int)(((byte)(37)))));
            this.progressBar.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(170)))), ((int)(((byte)(37)))));
            this.progressBar.BorderSize = 1;
            this.progressBar.DisplayStyle = Percolore.Core.UserControl.ProgressBarDisplayStyle.TextAndPercentage;
            this.progressBar.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.progressBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.progressBar.Location = new System.Drawing.Point(300, 106);
            this.progressBar.Name = "progressBar";
            this.progressBar.Orientacao = System.Windows.Forms.Orientation.Horizontal;
            this.progressBar.ProgressBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(185)))), ((int)(((byte)(37)))));
            this.progressBar.Size = new System.Drawing.Size(464, 65);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 14;
            this.progressBar.Text = "uProgressBar1";
            // 
            // btnIniciar
            // 
            this.btnIniciar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIniciar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(185)))), ((int)(((byte)(37)))));
            this.btnIniciar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(176)))), ((int)(((byte)(37)))));
            this.btnIniciar.FlatAppearance.BorderSize = 0;
            this.btnIniciar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIniciar.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btnIniciar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnIniciar.Location = new System.Drawing.Point(366, 179);
            this.btnIniciar.Margin = new System.Windows.Forms.Padding(5);
            this.btnIniciar.Name = "btnIniciar";
            this.btnIniciar.Size = new System.Drawing.Size(194, 65);
            this.btnIniciar.TabIndex = 0;
            this.btnIniciar.Text = "Iniciar";
            this.btnIniciar.UseVisualStyleBackColor = false;
            this.btnIniciar.Click += new System.EventHandler(this.btnIniciar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(185)))), ((int)(((byte)(37)))));
            this.btnCancelar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(176)))), ((int)(((byte)(37)))));
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btnCancelar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnCancelar.Location = new System.Drawing.Point(570, 179);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(5);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(194, 65);
            this.btnCancelar.TabIndex = 15;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // fInicializacaoCircuitos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(206)))), ((int)(((byte)(37)))));
            this.ClientSize = new System.Drawing.Size(1064, 350);
            this.Controls.Add(this.btnIniciar);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAbortar);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "fInicializacaoCircuitos";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ProcessoDispensaSimultanea_FormClosed);
            this.Load += new System.EventHandler(this.FormLoad);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.Button btnAbortar;
        private Percolore.Core.UserControl.UProgressBar progressBar;
        private System.Windows.Forms.Button btnIniciar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
