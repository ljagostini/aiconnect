namespace Percolore.IOConnect
{
    partial class fMensagem
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
            this.pct01 = new System.Windows.Forms.PictureBox();
            this.btCancel = new System.Windows.Forms.Button();
            this.btSave = new System.Windows.Forms.Button();
            this.lblText = new System.Windows.Forms.Label();
            this.lblDelay = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pct01)).BeginInit();
            this.SuspendLayout();
            // 
            // pct01
            // 
            this.pct01.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pct01.Location = new System.Drawing.Point(24, 46);
            this.pct01.Name = "pct01";
            this.pct01.Size = new System.Drawing.Size(196, 180);
            this.pct01.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pct01.TabIndex = 12;
            this.pct01.TabStop = false;
            // 
            // btCancel
            // 
            this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btCancel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btCancel.FlatAppearance.BorderSize = 0;
            this.btCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btCancel.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btCancel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.btCancel.Location = new System.Drawing.Point(647, 262);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(190, 55);
            this.btCancel.TabIndex = 11;
            this.btCancel.Text = "Cancelar";
            this.btCancel.UseVisualStyleBackColor = false;
            this.btCancel.Click += new System.EventHandler(this.Cancelar_Click);
            // 
            // btSave
            // 
            this.btSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btSave.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btSave.FlatAppearance.BorderSize = 0;
            this.btSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btSave.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btSave.ForeColor = System.Drawing.SystemColors.WindowText;
            this.btSave.Location = new System.Drawing.Point(452, 262);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(190, 55);
            this.btSave.TabIndex = 10;
            this.btSave.Text = "Salvar";
            this.btSave.UseVisualStyleBackColor = false;
            this.btSave.Click += new System.EventHandler(this.Salvar_Click);
            // 
            // lblText
            // 
            this.lblText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblText.BackColor = System.Drawing.Color.Transparent;
            this.lblText.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.lblText.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblText.Location = new System.Drawing.Point(252, 27);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(992, 218);
            this.lblText.TabIndex = 9;
            this.lblText.Text = "Xxxxxxxxxxxxxxx Xxxxxxxxxxxxx Xxxxxxxxxxxx\r\n\r\nXxxxxxxxxxxxxxxxxxxxx\r\nXxxxxxxxxxxx" +
    "xxxxxxxxx";
            this.lblText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDelay
            // 
            this.lblDelay.AutoSize = true;
            this.lblDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDelay.Location = new System.Drawing.Point(1111, 270);
            this.lblDelay.Name = "lblDelay";
            this.lblDelay.Size = new System.Drawing.Size(44, 31);
            this.lblDelay.TabIndex = 13;
            this.lblDelay.Text = "30";
            this.lblDelay.Visible = false;
            // 
            // fMensagem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 350);
            this.Controls.Add(this.lblDelay);
            this.Controls.Add(this.pct01);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.lblText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "fMensagem";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FullscreenConfirm";
            this.Load += new System.EventHandler(this.Mensagens_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pct01)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pct01;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.Label lblDelay;
    }
}