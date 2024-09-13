namespace Percolore.IOConnect
{
    partial class fDispensaSimultanea
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
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnIniciar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnAbortar = new System.Windows.Forms.Button();
            this.lblSubStatus = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI Light", 30F);
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(14, 51);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(1036, 54);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Xxxxxxxxxxx xxxxxxx xxxxxx";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnIniciar
            // 
            this.btnIniciar.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnIniciar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(176)))), ((int)(((byte)(37)))));
            this.btnIniciar.FlatAppearance.BorderSize = 0;
            this.btnIniciar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIniciar.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btnIniciar.ForeColor = System.Drawing.Color.White;
            this.btnIniciar.Location = new System.Drawing.Point(316, 235);
            this.btnIniciar.Margin = new System.Windows.Forms.Padding(5);
            this.btnIniciar.Name = "btnIniciar";
            this.btnIniciar.Size = new System.Drawing.Size(212, 65);
            this.btnIniciar.TabIndex = 0;
            this.btnIniciar.Text = "Iniciar";
            this.btnIniciar.UseVisualStyleBackColor = false;
            this.btnIniciar.Click += new System.EventHandler(this.Iniciar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(176)))), ((int)(((byte)(37)))));
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(536, 235);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(5);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(212, 65);
            this.btnCancelar.TabIndex = 1;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.Cancelar_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(206)))), ((int)(((byte)(37)))));
            this.progressBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(206)))), ((int)(((byte)(37)))));
            this.progressBar.Location = new System.Drawing.Point(367, 165);
            this.progressBar.Margin = new System.Windows.Forms.Padding(5);
            this.progressBar.MarqueeAnimationSpeed = 15;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(331, 30);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 12;
            // 
            // btnAbortar
            // 
            this.btnAbortar.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnAbortar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(176)))), ((int)(((byte)(37)))));
            this.btnAbortar.FlatAppearance.BorderSize = 0;
            this.btnAbortar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbortar.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btnAbortar.ForeColor = System.Drawing.Color.White;
            this.btnAbortar.Location = new System.Drawing.Point(536, 235);
            this.btnAbortar.Margin = new System.Windows.Forms.Padding(5);
            this.btnAbortar.Name = "btnAbortar";
            this.btnAbortar.Size = new System.Drawing.Size(212, 65);
            this.btnAbortar.TabIndex = 13;
            this.btnAbortar.Text = "Abortar";
            this.btnAbortar.UseVisualStyleBackColor = false;
            this.btnAbortar.Visible = false;
            this.btnAbortar.Click += new System.EventHandler(this.Abortar_Click);
            // 
            // lblSubStatus
            // 
            this.lblSubStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSubStatus.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.lblSubStatus.ForeColor = System.Drawing.Color.White;
            this.lblSubStatus.Location = new System.Drawing.Point(14, 105);
            this.lblSubStatus.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblSubStatus.Name = "lblSubStatus";
            this.lblSubStatus.Size = new System.Drawing.Size(1036, 28);
            this.lblSubStatus.TabIndex = 14;
            this.lblSubStatus.Text = "Xxxxxxxxxxx xxxxxxx xxxxxx";
            this.lblSubStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(23, 165);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(172, 124);
            this.richTextBox1.TabIndex = 15;
            this.richTextBox1.Text = "";
            this.richTextBox1.Visible = false;
            // 
            // fDispensaSimultanea
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(206)))), ((int)(((byte)(37)))));
            this.ClientSize = new System.Drawing.Size(1064, 350);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.lblSubStatus);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnIniciar);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAbortar);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "fDispensaSimultanea";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Status da dispensa de colorantes";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ProcessoDispensaSimultanea_FormClosed);
            this.Load += new System.EventHandler(this.FormLoad);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnIniciar;
        private System.Windows.Forms.Button btnAbortar;
        private System.Windows.Forms.Label lblSubStatus;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}
