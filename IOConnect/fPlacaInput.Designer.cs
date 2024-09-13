namespace Percolore.IOConnect
{
    partial class fPlacaInput
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
            this.btn_Fechar = new System.Windows.Forms.Button();
            this.gbSensores = new System.Windows.Forms.GroupBox();
            this.btnVersion = new System.Windows.Forms.Button();
            this.txtVersionHard = new Percolore.Core.UserControl.UTextBox();
            this.lblVersionHard = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.txtInput4 = new Percolore.Core.UserControl.UTextBox();
            this.lblSensorBaixoBico = new System.Windows.Forms.Label();
            this.txtInput3 = new Percolore.Core.UserControl.UTextBox();
            this.lblSensorAltoBico = new System.Windows.Forms.Label();
            this.txtInput2 = new Percolore.Core.UserControl.UTextBox();
            this.lblSensorEsponja = new System.Windows.Forms.Label();
            this.txtInput1 = new Percolore.Core.UserControl.UTextBox();
            this.lblSensorCopo = new System.Windows.Forms.Label();
            this.gbSensores.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Fechar
            // 
            this.btn_Fechar.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btn_Fechar.BackColor = System.Drawing.SystemColors.Control;
            this.btn_Fechar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Fechar.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btn_Fechar.ForeColor = System.Drawing.Color.Black;
            this.btn_Fechar.Location = new System.Drawing.Point(294, 12);
            this.btn_Fechar.Name = "btn_Fechar";
            this.btn_Fechar.Size = new System.Drawing.Size(125, 49);
            this.btn_Fechar.TabIndex = 34;
            this.btn_Fechar.Text = "Close";
            this.btn_Fechar.UseVisualStyleBackColor = false;
            this.btn_Fechar.Click += new System.EventHandler(this.btn_Fechar_Click);
            // 
            // gbSensores
            // 
            this.gbSensores.Controls.Add(this.btnVersion);
            this.gbSensores.Controls.Add(this.txtVersionHard);
            this.gbSensores.Controls.Add(this.lblVersionHard);
            this.gbSensores.Controls.Add(this.btnRefresh);
            this.gbSensores.Controls.Add(this.txtInput4);
            this.gbSensores.Controls.Add(this.lblSensorBaixoBico);
            this.gbSensores.Controls.Add(this.txtInput3);
            this.gbSensores.Controls.Add(this.lblSensorAltoBico);
            this.gbSensores.Controls.Add(this.txtInput2);
            this.gbSensores.Controls.Add(this.lblSensorEsponja);
            this.gbSensores.Controls.Add(this.txtInput1);
            this.gbSensores.Controls.Add(this.lblSensorCopo);
            this.gbSensores.Location = new System.Drawing.Point(12, 77);
            this.gbSensores.Name = "gbSensores";
            this.gbSensores.Size = new System.Drawing.Size(407, 206);
            this.gbSensores.TabIndex = 38;
            this.gbSensores.TabStop = false;
            this.gbSensores.Text = "Input\'s";
            this.gbSensores.Enter += new System.EventHandler(this.gbSensores_Enter);
            // 
            // btnVersion
            // 
            this.btnVersion.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnVersion.BackColor = System.Drawing.SystemColors.Control;
            this.btnVersion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVersion.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btnVersion.ForeColor = System.Drawing.Color.Black;
            this.btnVersion.Location = new System.Drawing.Point(193, 150);
            this.btnVersion.Name = "btnVersion";
            this.btnVersion.Size = new System.Drawing.Size(195, 39);
            this.btnVersion.TabIndex = 38;
            this.btnVersion.Text = "Version Hardware";
            this.btnVersion.UseVisualStyleBackColor = false;
            this.btnVersion.Click += new System.EventHandler(this.btnVersion_Click);
            // 
            // txtVersionHard
            // 
            this.txtVersionHard.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtVersionHard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtVersionHard.Conteudo = Percolore.Core.UserControl.TipoConteudo.Padrao;
            this.txtVersionHard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtVersionHard.Location = new System.Drawing.Point(238, 32);
            this.txtVersionHard.Name = "txtVersionHard";
            this.txtVersionHard.ReadOnly = true;
            this.txtVersionHard.Size = new System.Drawing.Size(150, 20);
            this.txtVersionHard.TabIndex = 37;
            // 
            // lblVersionHard
            // 
            this.lblVersionHard.AutoSize = true;
            this.lblVersionHard.Location = new System.Drawing.Point(190, 34);
            this.lblVersionHard.Name = "lblVersionHard";
            this.lblVersionHard.Size = new System.Drawing.Size(42, 13);
            this.lblVersionHard.TabIndex = 36;
            this.lblVersionHard.Text = "Version";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnRefresh.BackColor = System.Drawing.SystemColors.Control;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btnRefresh.ForeColor = System.Drawing.Color.Black;
            this.btnRefresh.Location = new System.Drawing.Point(19, 150);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(140, 39);
            this.btnRefresh.TabIndex = 35;
            this.btnRefresh.Text = "Input";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // txtInput4
            // 
            this.txtInput4.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtInput4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInput4.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtInput4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtInput4.Location = new System.Drawing.Point(110, 110);
            this.txtInput4.Name = "txtInput4";
            this.txtInput4.ReadOnly = true;
            this.txtInput4.Size = new System.Drawing.Size(37, 20);
            this.txtInput4.TabIndex = 7;
            // 
            // lblSensorBaixoBico
            // 
            this.lblSensorBaixoBico.AutoSize = true;
            this.lblSensorBaixoBico.Location = new System.Drawing.Point(33, 112);
            this.lblSensorBaixoBico.Name = "lblSensorBaixoBico";
            this.lblSensorBaixoBico.Size = new System.Drawing.Size(43, 13);
            this.lblSensorBaixoBico.TabIndex = 6;
            this.lblSensorBaixoBico.Text = "Input 4:";
            // 
            // txtInput3
            // 
            this.txtInput3.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtInput3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInput3.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtInput3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtInput3.Location = new System.Drawing.Point(110, 84);
            this.txtInput3.Name = "txtInput3";
            this.txtInput3.ReadOnly = true;
            this.txtInput3.Size = new System.Drawing.Size(37, 20);
            this.txtInput3.TabIndex = 5;
            // 
            // lblSensorAltoBico
            // 
            this.lblSensorAltoBico.AutoSize = true;
            this.lblSensorAltoBico.Location = new System.Drawing.Point(33, 86);
            this.lblSensorAltoBico.Name = "lblSensorAltoBico";
            this.lblSensorAltoBico.Size = new System.Drawing.Size(43, 13);
            this.lblSensorAltoBico.TabIndex = 4;
            this.lblSensorAltoBico.Text = "Input 3:";
            // 
            // txtInput2
            // 
            this.txtInput2.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtInput2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInput2.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtInput2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtInput2.Location = new System.Drawing.Point(110, 58);
            this.txtInput2.Name = "txtInput2";
            this.txtInput2.ReadOnly = true;
            this.txtInput2.Size = new System.Drawing.Size(37, 20);
            this.txtInput2.TabIndex = 3;
            // 
            // lblSensorEsponja
            // 
            this.lblSensorEsponja.AutoSize = true;
            this.lblSensorEsponja.Location = new System.Drawing.Point(33, 60);
            this.lblSensorEsponja.Name = "lblSensorEsponja";
            this.lblSensorEsponja.Size = new System.Drawing.Size(43, 13);
            this.lblSensorEsponja.TabIndex = 2;
            this.lblSensorEsponja.Text = "Input 2:";
            // 
            // txtInput1
            // 
            this.txtInput1.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtInput1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInput1.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtInput1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtInput1.Location = new System.Drawing.Point(110, 32);
            this.txtInput1.Name = "txtInput1";
            this.txtInput1.ReadOnly = true;
            this.txtInput1.Size = new System.Drawing.Size(37, 20);
            this.txtInput1.TabIndex = 1;
            // 
            // lblSensorCopo
            // 
            this.lblSensorCopo.AutoSize = true;
            this.lblSensorCopo.Location = new System.Drawing.Point(33, 34);
            this.lblSensorCopo.Name = "lblSensorCopo";
            this.lblSensorCopo.Size = new System.Drawing.Size(43, 13);
            this.lblSensorCopo.TabIndex = 0;
            this.lblSensorCopo.Text = "Input 1:";
            // 
            // fPlacaInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 310);
            this.Controls.Add(this.gbSensores);
            this.Controls.Add(this.btn_Fechar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "fPlacaInput";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "fPlacaInput";
            this.Load += new System.EventHandler(this.fPlacaInput_Load);
            this.gbSensores.ResumeLayout(false);
            this.gbSensores.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Fechar;
        private System.Windows.Forms.GroupBox gbSensores;
        private System.Windows.Forms.Button btnRefresh;
        private Percolore.Core.UserControl.UTextBox txtInput4;
        private System.Windows.Forms.Label lblSensorBaixoBico;
        private Percolore.Core.UserControl.UTextBox txtInput3;
        private System.Windows.Forms.Label lblSensorAltoBico;
        private Percolore.Core.UserControl.UTextBox txtInput2;
        private System.Windows.Forms.Label lblSensorEsponja;
        private Percolore.Core.UserControl.UTextBox txtInput1;
        private System.Windows.Forms.Label lblSensorCopo;
        private System.Windows.Forms.Button btnVersion;
        private Percolore.Core.UserControl.UTextBox txtVersionHard;
        private System.Windows.Forms.Label lblVersionHard;
    }
}