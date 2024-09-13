/*
 * Created by SharpDevelop.
 * User: Eric
 * Date: 7/27/2010
 * Time: 3:36 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Percolore.IOConnect
{
    partial class fPurga
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
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnIniciar = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblSubStatus = new System.Windows.Forms.Label();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.pnlCor = new System.Windows.Forms.Panel();
            this.SuspendLayout();
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
            this.btnAbortar.Name = "btnAbortar";
            this.btnAbortar.Size = new System.Drawing.Size(212, 65);
            this.btnAbortar.TabIndex = 3;
            this.btnAbortar.Text = "Abortar";
            this.btnAbortar.UseVisualStyleBackColor = false;
            this.btnAbortar.Click += new System.EventHandler(this.Abortar_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI Light", 30F);
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(14, 51);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(1036, 54);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Xxxxxxxxxxx xxxxxxx xxxxxx";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnIniciar
            // 
            this.btnIniciar.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnIniciar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(176)))), ((int)(((byte)(37)))));
            this.btnIniciar.Enabled = false;
            this.btnIniciar.FlatAppearance.BorderSize = 0;
            this.btnIniciar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIniciar.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btnIniciar.ForeColor = System.Drawing.Color.White;
            this.btnIniciar.Location = new System.Drawing.Point(316, 235);
            this.btnIniciar.Name = "btnIniciar";
            this.btnIniciar.Size = new System.Drawing.Size(212, 65);
            this.btnIniciar.TabIndex = 0;
            this.btnIniciar.Text = "Iniciar";
            this.btnIniciar.UseVisualStyleBackColor = false;
            this.btnIniciar.Click += new System.EventHandler(this.Iniciar_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Enabled = false;
            this.progressBar.ForeColor = System.Drawing.SystemColors.ControlText;
            this.progressBar.Location = new System.Drawing.Point(367, 165);
            this.progressBar.MarqueeAnimationSpeed = 15;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(331, 30);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 4;
            // 
            // lblSubStatus
            // 
            this.lblSubStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSubStatus.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.lblSubStatus.ForeColor = System.Drawing.Color.White;
            this.lblSubStatus.Location = new System.Drawing.Point(14, 105);
            this.lblSubStatus.Name = "lblSubStatus";
            this.lblSubStatus.Size = new System.Drawing.Size(1036, 28);
            this.lblSubStatus.TabIndex = 2;
            this.lblSubStatus.Text = "Xxxxxxxxxxx xxxxxxx xxxxxx";
            this.lblSubStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(212, 65);
            this.btnCancelar.TabIndex = 1;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.Cancelar_Click);
            // 
            // pnlCor
            // 
            this.pnlCor.Location = new System.Drawing.Point(12, 12);
            this.pnlCor.Name = "pnlCor";
            this.pnlCor.Size = new System.Drawing.Size(130, 52);
            this.pnlCor.TabIndex = 5;
            this.pnlCor.Visible = false;
            // 
            // fPurga
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(206)))), ((int)(((byte)(37)))));
            this.ClientSize = new System.Drawing.Size(1064, 350);
            this.Controls.Add(this.pnlCor);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblSubStatus);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnIniciar);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAbortar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "fPurga";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Purga_Closed);
            this.Load += new System.EventHandler(this.ProcessoPurga_Load);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnIniciar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnAbortar;
        private System.Windows.Forms.Label lblSubStatus;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Panel pnlCor;
    }
}
