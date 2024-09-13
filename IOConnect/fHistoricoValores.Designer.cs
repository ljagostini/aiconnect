namespace Percolore.IOConnect
{
    partial class fHistoricoValores
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
            this.lblHistoricoLegendaVolume = new System.Windows.Forms.Label();
            this.lblHistoricoVolume = new System.Windows.Forms.Label();
            this.listView = new System.Windows.Forms.ListView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblHistoricoLegendaVolume
            // 
            this.lblHistoricoLegendaVolume.AutoSize = true;
            this.lblHistoricoLegendaVolume.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHistoricoLegendaVolume.Font = new System.Drawing.Font("Segoe UI Light", 11F);
            this.lblHistoricoLegendaVolume.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblHistoricoLegendaVolume.Location = new System.Drawing.Point(0, 3);
            this.lblHistoricoLegendaVolume.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.lblHistoricoLegendaVolume.Name = "lblHistoricoLegendaVolume";
            this.lblHistoricoLegendaVolume.Size = new System.Drawing.Size(59, 24);
            this.lblHistoricoLegendaVolume.TabIndex = 283;
            this.lblHistoricoLegendaVolume.Text = "Volume:";
            this.lblHistoricoLegendaVolume.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHistoricoVolume
            // 
            this.lblHistoricoVolume.AutoSize = true;
            this.lblHistoricoVolume.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHistoricoVolume.Font = new System.Drawing.Font("Segoe UI Light", 16F);
            this.lblHistoricoVolume.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblHistoricoVolume.Location = new System.Drawing.Point(59, 0);
            this.lblHistoricoVolume.Margin = new System.Windows.Forms.Padding(0);
            this.lblHistoricoVolume.Name = "lblHistoricoVolume";
            this.lblHistoricoVolume.Size = new System.Drawing.Size(80, 30);
            this.lblHistoricoVolume.TabIndex = 355;
            this.lblHistoricoVolume.Text = "000 mL";
            this.lblHistoricoVolume.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // listView
            // 
            this.listView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView.Font = new System.Drawing.Font("Segoe UI Light", 16F);
            this.listView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView.Location = new System.Drawing.Point(16, 50);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(576, 242);
            this.listView.TabIndex = 6;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.lblHistoricoVolume, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblHistoricoLegendaVolume, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(16, 12);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(139, 30);
            this.tableLayoutPanel1.TabIndex = 356;
            // 
            // HistoricoValores
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(608, 307);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.listView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "HistoricoValores";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Histórico de valores confirmados";
            this.Load += new System.EventHandler(this.Historico_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblHistoricoLegendaVolume;
        private System.Windows.Forms.Label lblHistoricoVolume;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}