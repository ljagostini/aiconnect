namespace Percolore.IOConnect
{
    partial class fAutenticacao
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rdbSenha = new System.Windows.Forms.RadioButton();
            this.rdbToken = new System.Windows.Forms.RadioButton();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.gbToken = new System.Windows.Forms.GroupBox();
            this.lblToken = new System.Windows.Forms.Label();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.txtInput = new Percolore.Core.UserControl.UTextBox();
            this.gbUser = new System.Windows.Forms.GroupBox();
            this.btnCancelUser = new System.Windows.Forms.Button();
            this.btnConfirmaUser = new System.Windows.Forms.Button();
            this.lblSenha = new System.Windows.Forms.Label();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.txtSenha = new Percolore.Core.UserControl.UTextBox();
            this.txtUsuario = new Percolore.Core.UserControl.UTextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.gbToken.SuspendLayout();
            this.gbUser.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1024, 350);
            this.tableLayoutPanel1.TabIndex = 317;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.rdbSenha);
            this.panel1.Controls.Add(this.rdbToken);
            this.panel1.Controls.Add(this.lblTitulo);
            this.panel1.Controls.Add(this.gbToken);
            this.panel1.Controls.Add(this.gbUser);
            this.panel1.Location = new System.Drawing.Point(181, 36);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(661, 278);
            this.panel1.TabIndex = 318;
            // 
            // rdbSenha
            // 
            this.rdbSenha.AutoSize = true;
            this.rdbSenha.FlatAppearance.BorderSize = 0;
            this.rdbSenha.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.rdbSenha.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.rdbSenha.Location = new System.Drawing.Point(85, 55);
            this.rdbSenha.Name = "rdbSenha";
            this.rdbSenha.Size = new System.Drawing.Size(71, 27);
            this.rdbSenha.TabIndex = 4;
            this.rdbSenha.Text = "Senha";
            this.rdbSenha.UseVisualStyleBackColor = false;
            this.rdbSenha.CheckedChanged += new System.EventHandler(this.rdbSenha_CheckedChanged);
            // 
            // rdbToken
            // 
            this.rdbToken.AutoSize = true;
            this.rdbToken.Checked = true;
            this.rdbToken.FlatAppearance.BorderSize = 0;
            this.rdbToken.FlatAppearance.CheckedBackColor = System.Drawing.Color.Red;
            this.rdbToken.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.rdbToken.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.rdbToken.Location = new System.Drawing.Point(7, 55);
            this.rdbToken.Name = "rdbToken";
            this.rdbToken.Size = new System.Drawing.Size(70, 27);
            this.rdbToken.TabIndex = 3;
            this.rdbToken.TabStop = true;
            this.rdbToken.Text = "Token";
            this.rdbToken.UseVisualStyleBackColor = false;
            this.rdbToken.CheckedChanged += new System.EventHandler(this.rdbToken_CheckedChanged);
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.lblTitulo.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblTitulo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTitulo.Location = new System.Drawing.Point(476, 0);
            this.lblTitulo.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(181, 41);
            this.lblTitulo.TabIndex = 314;
            this.lblTitulo.Text = "Autenticação";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gbToken
            // 
            this.gbToken.Controls.Add(this.lblToken);
            this.gbToken.Controls.Add(this.btnCancelar);
            this.gbToken.Controls.Add(this.btnOk);
            this.gbToken.Controls.Add(this.txtInput);
            this.gbToken.Location = new System.Drawing.Point(1, 88);
            this.gbToken.Name = "gbToken";
            this.gbToken.Size = new System.Drawing.Size(657, 187);
            this.gbToken.TabIndex = 315;
            this.gbToken.TabStop = false;
            // 
            // lblToken
            // 
            this.lblToken.AutoSize = true;
            this.lblToken.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.lblToken.Location = new System.Drawing.Point(18, 19);
            this.lblToken.Name = "lblToken";
            this.lblToken.Size = new System.Drawing.Size(89, 41);
            this.lblToken.TabIndex = 4;
            this.lblToken.Text = "Token";
            // 
            // btnCancelar
            // 
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnCancelar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnCancelar.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnCancelar.Location = new System.Drawing.Point(521, 128);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(52, 52);
            this.btnCancelar.TabIndex = 2;
            this.btnCancelar.Tag = "0";
            this.btnCancelar.Text = "CAN";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnOk
            // 
            this.btnOk.FlatAppearance.BorderSize = 0;
            this.btnOk.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnOk.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnOk.Location = new System.Drawing.Point(579, 128);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(52, 52);
            this.btnOk.TabIndex = 1;
            this.btnOk.Tag = "0";
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // txtInput
            // 
            this.txtInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.txtInput.BorderColor = System.Drawing.Color.Gray;
            this.txtInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInput.Conteudo = Percolore.Core.UserControl.TipoConteudo.Padrao;
            this.txtInput.Font = new System.Drawing.Font("Segoe UI Light", 25F);
            this.txtInput.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.txtInput.Location = new System.Drawing.Point(161, 13);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(492, 52);
            this.txtInput.TabIndex = 0;
            // 
            // gbUser
            // 
            this.gbUser.Controls.Add(this.btnCancelUser);
            this.gbUser.Controls.Add(this.btnConfirmaUser);
            this.gbUser.Controls.Add(this.lblSenha);
            this.gbUser.Controls.Add(this.lblUsuario);
            this.gbUser.Controls.Add(this.txtSenha);
            this.gbUser.Controls.Add(this.txtUsuario);
            this.gbUser.Location = new System.Drawing.Point(1, 88);
            this.gbUser.Name = "gbUser";
            this.gbUser.Size = new System.Drawing.Size(657, 187);
            this.gbUser.TabIndex = 319;
            this.gbUser.TabStop = false;
            this.gbUser.Visible = false;
            // 
            // btnCancelUser
            // 
            this.btnCancelUser.FlatAppearance.BorderSize = 0;
            this.btnCancelUser.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnCancelUser.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnCancelUser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelUser.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnCancelUser.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnCancelUser.Location = new System.Drawing.Point(521, 128);
            this.btnCancelUser.Name = "btnCancelUser";
            this.btnCancelUser.Size = new System.Drawing.Size(52, 52);
            this.btnCancelUser.TabIndex = 6;
            this.btnCancelUser.Tag = "0";
            this.btnCancelUser.Text = "CAN";
            this.btnCancelUser.UseVisualStyleBackColor = false;
            this.btnCancelUser.Click += new System.EventHandler(this.btnCancelUser_Click);
            // 
            // btnConfirmaUser
            // 
            this.btnConfirmaUser.FlatAppearance.BorderSize = 0;
            this.btnConfirmaUser.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnConfirmaUser.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnConfirmaUser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirmaUser.Font = new System.Drawing.Font("Segoe UI Light", 12.75F);
            this.btnConfirmaUser.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnConfirmaUser.Location = new System.Drawing.Point(579, 128);
            this.btnConfirmaUser.Name = "btnConfirmaUser";
            this.btnConfirmaUser.Size = new System.Drawing.Size(52, 52);
            this.btnConfirmaUser.TabIndex = 5;
            this.btnConfirmaUser.Tag = "0";
            this.btnConfirmaUser.Text = "Ok";
            this.btnConfirmaUser.UseVisualStyleBackColor = false;
            this.btnConfirmaUser.Click += new System.EventHandler(this.btnConfirmaUser_Click);
            // 
            // lblSenha
            // 
            this.lblSenha.AutoSize = true;
            this.lblSenha.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.lblSenha.Location = new System.Drawing.Point(18, 77);
            this.lblSenha.Name = "lblSenha";
            this.lblSenha.Size = new System.Drawing.Size(135, 41);
            this.lblSenha.TabIndex = 4;
            this.lblSenha.Text = "Password";
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Segoe UI Light", 22F);
            this.lblUsuario.Location = new System.Drawing.Point(18, 19);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(74, 41);
            this.lblUsuario.TabIndex = 3;
            this.lblUsuario.Text = "User";
            // 
            // txtSenha
            // 
            this.txtSenha.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.txtSenha.BorderColor = System.Drawing.Color.Gray;
            this.txtSenha.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSenha.Conteudo = Percolore.Core.UserControl.TipoConteudo.Padrao;
            this.txtSenha.Font = new System.Drawing.Font("Segoe UI Light", 25F);
            this.txtSenha.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.txtSenha.Location = new System.Drawing.Point(224, 71);
            this.txtSenha.Name = "txtSenha";
            this.txtSenha.PasswordChar = '*';
            this.txtSenha.Size = new System.Drawing.Size(429, 52);
            this.txtSenha.TabIndex = 2;
            this.txtSenha.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSenha_KeyPress);
            // 
            // txtUsuario
            // 
            this.txtUsuario.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.txtUsuario.BorderColor = System.Drawing.Color.Gray;
            this.txtUsuario.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsuario.Conteudo = Percolore.Core.UserControl.TipoConteudo.Padrao;
            this.txtUsuario.Font = new System.Drawing.Font("Segoe UI Light", 25F);
            this.txtUsuario.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.txtUsuario.Location = new System.Drawing.Point(224, 13);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(429, 52);
            this.txtUsuario.TabIndex = 1;
            this.txtUsuario.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtUsuario_KeyPress);
            // 
            // fAutenticacao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(1024, 350);
            this.Controls.Add(this.tableLayoutPanel1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "fAutenticacao";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.gbToken.ResumeLayout(false);
            this.gbToken.PerformLayout();
            this.gbUser.ResumeLayout(false);
            this.gbUser.PerformLayout();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private Percolore.Core.UserControl.UTextBox txtInput;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.RadioButton rdbSenha;
        private System.Windows.Forms.RadioButton rdbToken;
        private System.Windows.Forms.GroupBox gbToken;
        private System.Windows.Forms.GroupBox gbUser;
        private Percolore.Core.UserControl.UTextBox txtUsuario;
        private System.Windows.Forms.Button btnCancelUser;
        private System.Windows.Forms.Button btnConfirmaUser;
        private System.Windows.Forms.Label lblSenha;
        private System.Windows.Forms.Label lblUsuario;
        private Percolore.Core.UserControl.UTextBox txtSenha;
        private System.Windows.Forms.Label lblToken;
    }
}
