namespace Percolore.Core.UserControl
{
    partial class Mensagem
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
            this.lblText = new System.Windows.Forms.Label();
            this.btSave = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.pct01 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pct01)).BeginInit();
            this.SuspendLayout();
            // 
            // lblText
            // 
            this.lblText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblText.BackColor = System.Drawing.Color.Transparent;
            this.lblText.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.lblText.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblText.Location = new System.Drawing.Point(246, 29);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(1011, 218);
            this.lblText.TabIndex = 0;
            this.lblText.Text = "Xxxxxxxxxxxxxxx Xxxxxxxxxxxxx Xxxxxxxxxxxx\r\n\r\nXxxxxxxxxxxxxxxxxxxxx\r\nXxxxxxxxxxxx" +
    "xxxxxxxxx";
            this.lblText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btSave
            // 
            this.btSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btSave.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btSave.FlatAppearance.BorderSize = 0;
            this.btSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btSave.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btSave.ForeColor = System.Drawing.SystemColors.WindowText;
            this.btSave.Location = new System.Drawing.Point(448, 266);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(190, 55);
            this.btSave.TabIndex = 1;
            this.btSave.Text = "Salvar";
            this.btSave.UseVisualStyleBackColor = false;
            this.btSave.Click += new System.EventHandler(this.Salvar_Click);
            // 
            // btCancel
            // 
            this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btCancel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btCancel.FlatAppearance.BorderSize = 0;
            this.btCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btCancel.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btCancel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.btCancel.Location = new System.Drawing.Point(643, 266);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(190, 55);
            this.btCancel.TabIndex = 2;
            this.btCancel.Text = "Cancelar";
            this.btCancel.UseVisualStyleBackColor = false;
            this.btCancel.Click += new System.EventHandler(this.Cancelar_Click);
            // 
            // pct01
            // 
            this.pct01.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pct01.Location = new System.Drawing.Point(42, 48);
            this.pct01.Name = "pct01";
            this.pct01.Size = new System.Drawing.Size(180, 180);
            this.pct01.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pct01.TabIndex = 8;
            this.pct01.TabStop = false;
            // 
            // Mensagem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1280, 350);
            this.Controls.Add(this.pct01);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.lblText);
            this.ForeColor = System.Drawing.SystemColors.Control;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Mensagem";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FullscreenConfirm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Mensagens_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pct01)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.PictureBox pct01;
    }
}