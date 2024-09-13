namespace DataHoraPercolore
{
    partial class Form1
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
            this.btnAjustaDataHoraPerc = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnAjustaDataHoraPerc
            // 
            this.btnAjustaDataHoraPerc.Location = new System.Drawing.Point(79, 44);
            this.btnAjustaDataHoraPerc.Name = "btnAjustaDataHoraPerc";
            this.btnAjustaDataHoraPerc.Size = new System.Drawing.Size(138, 65);
            this.btnAjustaDataHoraPerc.TabIndex = 0;
            this.btnAjustaDataHoraPerc.Text = "Habilitar";
            this.btnAjustaDataHoraPerc.UseVisualStyleBackColor = true;
            this.btnAjustaDataHoraPerc.Click += new System.EventHandler(this.btnAjustaDataHoraPerc_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(313, 157);
            this.Controls.Add(this.btnAjustaDataHoraPerc);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Ajustar Data Hora Percolore";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAjustaDataHoraPerc;
    }
}

