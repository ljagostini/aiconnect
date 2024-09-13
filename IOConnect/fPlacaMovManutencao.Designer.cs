namespace Percolore.IOConnect
{
    partial class fPlacaMovManutencao
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
            this.pbImageBck = new System.Windows.Forms.PictureBox();
            this.lblSubStatus = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.gbSensores = new System.Windows.Forms.GroupBox();
            this.txtMaquinaLigada = new Percolore.Core.UserControl.UTextBox();
            this.lblSensorMaqLigada = new System.Windows.Forms.Label();
            this.txtCodAlerta = new Percolore.Core.UserControl.UTextBox();
            this.lblSensorCodAlerta = new System.Windows.Forms.Label();
            this.txtCodErro = new Percolore.Core.UserControl.UTextBox();
            this.lblSensorCodErro = new System.Windows.Forms.Label();
            this.txtSensorEmergencia = new Percolore.Core.UserControl.UTextBox();
            this.lblSensorEmergencia = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.txtSensorValvulaFechada = new Percolore.Core.UserControl.UTextBox();
            this.lblSensorVavlulaRecirculacao = new System.Windows.Forms.Label();
            this.txtSensorValvulaAberta = new Percolore.Core.UserControl.UTextBox();
            this.lblSensorValvulaDosagem = new System.Windows.Forms.Label();
            this.txtSensorGavetaFechada = new Percolore.Core.UserControl.UTextBox();
            this.lblSensorGavetaFechada = new System.Windows.Forms.Label();
            this.txtSensorGavetaAberta = new Percolore.Core.UserControl.UTextBox();
            this.lblSensorGavetaAberta = new System.Windows.Forms.Label();
            this.txtSensorBaixoBico = new Percolore.Core.UserControl.UTextBox();
            this.lblSensorBaixoBico = new System.Windows.Forms.Label();
            this.txtSensorAltoBico = new Percolore.Core.UserControl.UTextBox();
            this.lblSensorAltoBico = new System.Windows.Forms.Label();
            this.txtSensorEsponja = new Percolore.Core.UserControl.UTextBox();
            this.lblSensorEsponja = new System.Windows.Forms.Label();
            this.txtSensorCopo = new Percolore.Core.UserControl.UTextBox();
            this.lblSensorCopo = new System.Windows.Forms.Label();
            this.gbAction = new System.Windows.Forms.GroupBox();
            this.gbActionComand = new System.Windows.Forms.GroupBox();
            this.btnSubirBico = new System.Windows.Forms.Button();
            this.btnDescerBico = new System.Windows.Forms.Button();
            this.btnValvulaRecirculacao = new System.Windows.Forms.Button();
            this.btnAbrirGaveta = new System.Windows.Forms.Button();
            this.btnFecharGaveta = new System.Windows.Forms.Button();
            this.btnValvulaDosagem = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.pbImageBck)).BeginInit();
            this.gbSensores.SuspendLayout();
            this.gbAction.SuspendLayout();
            this.gbActionComand.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Fechar
            // 
            this.btn_Fechar.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btn_Fechar.BackColor = System.Drawing.SystemColors.Control;
            this.btn_Fechar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Fechar.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btn_Fechar.ForeColor = System.Drawing.Color.Black;
            this.btn_Fechar.Location = new System.Drawing.Point(663, 30);
            this.btn_Fechar.Name = "btn_Fechar";
            this.btn_Fechar.Size = new System.Drawing.Size(125, 49);
            this.btn_Fechar.TabIndex = 33;
            this.btn_Fechar.Text = "Fechar";
            this.btn_Fechar.UseVisualStyleBackColor = false;
            this.btn_Fechar.Click += new System.EventHandler(this.btn_Fechar_Click);
            // 
            // pbImageBck
            // 
            this.pbImageBck.Location = new System.Drawing.Point(28, 12);
            this.pbImageBck.Name = "pbImageBck";
            this.pbImageBck.Size = new System.Drawing.Size(291, 184);
            this.pbImageBck.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbImageBck.TabIndex = 34;
            this.pbImageBck.TabStop = false;
            // 
            // lblSubStatus
            // 
            this.lblSubStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSubStatus.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.lblSubStatus.ForeColor = System.Drawing.Color.Black;
            this.lblSubStatus.Location = new System.Drawing.Point(333, 130);
            this.lblSubStatus.Name = "lblSubStatus";
            this.lblSubStatus.Size = new System.Drawing.Size(454, 28);
            this.lblSubStatus.TabIndex = 36;
            this.lblSubStatus.Text = "Xxxxxxxxxxx xxxxxxx xxxxxx";
            this.lblSubStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI Light", 30F);
            this.lblStatus.ForeColor = System.Drawing.Color.Black;
            this.lblStatus.Location = new System.Drawing.Point(333, 78);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(454, 54);
            this.lblStatus.TabIndex = 35;
            this.lblStatus.Text = "Xxxxxxxxxxx xxxxxxx xxxxxx";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gbSensores
            // 
            this.gbSensores.Controls.Add(this.txtMaquinaLigada);
            this.gbSensores.Controls.Add(this.lblSensorMaqLigada);
            this.gbSensores.Controls.Add(this.txtCodAlerta);
            this.gbSensores.Controls.Add(this.lblSensorCodAlerta);
            this.gbSensores.Controls.Add(this.txtCodErro);
            this.gbSensores.Controls.Add(this.lblSensorCodErro);
            this.gbSensores.Controls.Add(this.txtSensorEmergencia);
            this.gbSensores.Controls.Add(this.lblSensorEmergencia);
            this.gbSensores.Controls.Add(this.btnRefresh);
            this.gbSensores.Controls.Add(this.txtSensorValvulaFechada);
            this.gbSensores.Controls.Add(this.lblSensorVavlulaRecirculacao);
            this.gbSensores.Controls.Add(this.txtSensorValvulaAberta);
            this.gbSensores.Controls.Add(this.lblSensorValvulaDosagem);
            this.gbSensores.Controls.Add(this.txtSensorGavetaFechada);
            this.gbSensores.Controls.Add(this.lblSensorGavetaFechada);
            this.gbSensores.Controls.Add(this.txtSensorGavetaAberta);
            this.gbSensores.Controls.Add(this.lblSensorGavetaAberta);
            this.gbSensores.Controls.Add(this.txtSensorBaixoBico);
            this.gbSensores.Controls.Add(this.lblSensorBaixoBico);
            this.gbSensores.Controls.Add(this.txtSensorAltoBico);
            this.gbSensores.Controls.Add(this.lblSensorAltoBico);
            this.gbSensores.Controls.Add(this.txtSensorEsponja);
            this.gbSensores.Controls.Add(this.lblSensorEsponja);
            this.gbSensores.Controls.Add(this.txtSensorCopo);
            this.gbSensores.Controls.Add(this.lblSensorCopo);
            this.gbSensores.Location = new System.Drawing.Point(376, 203);
            this.gbSensores.Name = "gbSensores";
            this.gbSensores.Size = new System.Drawing.Size(407, 241);
            this.gbSensores.TabIndex = 37;
            this.gbSensores.TabStop = false;
            this.gbSensores.Text = "Sensores";
            // 
            // txtMaquinaLigada
            // 
            this.txtMaquinaLigada.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtMaquinaLigada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMaquinaLigada.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtMaquinaLigada.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtMaquinaLigada.Location = new System.Drawing.Point(325, 161);
            this.txtMaquinaLigada.Name = "txtMaquinaLigada";
            this.txtMaquinaLigada.ReadOnly = true;
            this.txtMaquinaLigada.Size = new System.Drawing.Size(37, 20);
            this.txtMaquinaLigada.TabIndex = 43;
            // 
            // lblSensorMaqLigada
            // 
            this.lblSensorMaqLigada.AutoSize = true;
            this.lblSensorMaqLigada.Location = new System.Drawing.Point(210, 163);
            this.lblSensorMaqLigada.Name = "lblSensorMaqLigada";
            this.lblSensorMaqLigada.Size = new System.Drawing.Size(86, 13);
            this.lblSensorMaqLigada.TabIndex = 42;
            this.lblSensorMaqLigada.Text = "Máquina Ligada:";
            // 
            // txtCodAlerta
            // 
            this.txtCodAlerta.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtCodAlerta.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCodAlerta.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtCodAlerta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtCodAlerta.Location = new System.Drawing.Point(130, 162);
            this.txtCodAlerta.Name = "txtCodAlerta";
            this.txtCodAlerta.ReadOnly = true;
            this.txtCodAlerta.Size = new System.Drawing.Size(37, 20);
            this.txtCodAlerta.TabIndex = 41;
            // 
            // lblSensorCodAlerta
            // 
            this.lblSensorCodAlerta.AutoSize = true;
            this.lblSensorCodAlerta.Location = new System.Drawing.Point(53, 164);
            this.lblSensorCodAlerta.Name = "lblSensorCodAlerta";
            this.lblSensorCodAlerta.Size = new System.Drawing.Size(37, 13);
            this.lblSensorCodAlerta.TabIndex = 40;
            this.lblSensorCodAlerta.Text = "Alerta:";
            // 
            // txtCodErro
            // 
            this.txtCodErro.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtCodErro.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCodErro.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtCodErro.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtCodErro.Location = new System.Drawing.Point(325, 136);
            this.txtCodErro.Name = "txtCodErro";
            this.txtCodErro.ReadOnly = true;
            this.txtCodErro.Size = new System.Drawing.Size(37, 20);
            this.txtCodErro.TabIndex = 39;
            // 
            // lblSensorCodErro
            // 
            this.lblSensorCodErro.AutoSize = true;
            this.lblSensorCodErro.Location = new System.Drawing.Point(210, 136);
            this.lblSensorCodErro.Name = "lblSensorCodErro";
            this.lblSensorCodErro.Size = new System.Drawing.Size(54, 13);
            this.lblSensorCodErro.TabIndex = 38;
            this.lblSensorCodErro.Text = "Cod. Erro:";
            // 
            // txtSensorEmergencia
            // 
            this.txtSensorEmergencia.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtSensorEmergencia.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSensorEmergencia.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtSensorEmergencia.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtSensorEmergencia.Location = new System.Drawing.Point(130, 136);
            this.txtSensorEmergencia.Name = "txtSensorEmergencia";
            this.txtSensorEmergencia.ReadOnly = true;
            this.txtSensorEmergencia.Size = new System.Drawing.Size(37, 20);
            this.txtSensorEmergencia.TabIndex = 37;
            // 
            // lblSensorEmergencia
            // 
            this.lblSensorEmergencia.AutoSize = true;
            this.lblSensorEmergencia.Location = new System.Drawing.Point(53, 138);
            this.lblSensorEmergencia.Name = "lblSensorEmergencia";
            this.lblSensorEmergencia.Size = new System.Drawing.Size(66, 13);
            this.lblSensorEmergencia.TabIndex = 36;
            this.lblSensorEmergencia.Text = "Emergência:";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnRefresh.BackColor = System.Drawing.SystemColors.Control;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btnRefresh.ForeColor = System.Drawing.Color.Black;
            this.btnRefresh.Location = new System.Drawing.Point(56, 189);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(306, 39);
            this.btnRefresh.TabIndex = 35;
            this.btnRefresh.Text = "Leitura dos Sensores";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // txtSensorValvulaFechada
            // 
            this.txtSensorValvulaFechada.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtSensorValvulaFechada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSensorValvulaFechada.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtSensorValvulaFechada.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtSensorValvulaFechada.Location = new System.Drawing.Point(325, 110);
            this.txtSensorValvulaFechada.Name = "txtSensorValvulaFechada";
            this.txtSensorValvulaFechada.ReadOnly = true;
            this.txtSensorValvulaFechada.Size = new System.Drawing.Size(37, 20);
            this.txtSensorValvulaFechada.TabIndex = 15;
            // 
            // lblSensorVavlulaRecirculacao
            // 
            this.lblSensorVavlulaRecirculacao.AutoSize = true;
            this.lblSensorVavlulaRecirculacao.Location = new System.Drawing.Point(210, 112);
            this.lblSensorVavlulaRecirculacao.Name = "lblSensorVavlulaRecirculacao";
            this.lblSensorVavlulaRecirculacao.Size = new System.Drawing.Size(111, 13);
            this.lblSensorVavlulaRecirculacao.TabIndex = 14;
            this.lblSensorVavlulaRecirculacao.Text = "Válvula Recirculação:";
            // 
            // txtSensorValvulaAberta
            // 
            this.txtSensorValvulaAberta.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtSensorValvulaAberta.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSensorValvulaAberta.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtSensorValvulaAberta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtSensorValvulaAberta.Location = new System.Drawing.Point(325, 84);
            this.txtSensorValvulaAberta.Name = "txtSensorValvulaAberta";
            this.txtSensorValvulaAberta.ReadOnly = true;
            this.txtSensorValvulaAberta.Size = new System.Drawing.Size(37, 20);
            this.txtSensorValvulaAberta.TabIndex = 13;
            // 
            // lblSensorValvulaDosagem
            // 
            this.lblSensorValvulaDosagem.AutoSize = true;
            this.lblSensorValvulaDosagem.Location = new System.Drawing.Point(210, 86);
            this.lblSensorValvulaDosagem.Name = "lblSensorValvulaDosagem";
            this.lblSensorValvulaDosagem.Size = new System.Drawing.Size(93, 13);
            this.lblSensorValvulaDosagem.TabIndex = 12;
            this.lblSensorValvulaDosagem.Text = "Válvula Dosagem:";
            // 
            // txtSensorGavetaFechada
            // 
            this.txtSensorGavetaFechada.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtSensorGavetaFechada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSensorGavetaFechada.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtSensorGavetaFechada.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtSensorGavetaFechada.Location = new System.Drawing.Point(325, 58);
            this.txtSensorGavetaFechada.Name = "txtSensorGavetaFechada";
            this.txtSensorGavetaFechada.ReadOnly = true;
            this.txtSensorGavetaFechada.Size = new System.Drawing.Size(37, 20);
            this.txtSensorGavetaFechada.TabIndex = 11;
            // 
            // lblSensorGavetaFechada
            // 
            this.lblSensorGavetaFechada.AutoSize = true;
            this.lblSensorGavetaFechada.Location = new System.Drawing.Point(210, 60);
            this.lblSensorGavetaFechada.Name = "lblSensorGavetaFechada";
            this.lblSensorGavetaFechada.Size = new System.Drawing.Size(90, 13);
            this.lblSensorGavetaFechada.TabIndex = 10;
            this.lblSensorGavetaFechada.Text = "Gaveta Fechada:";
            // 
            // txtSensorGavetaAberta
            // 
            this.txtSensorGavetaAberta.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtSensorGavetaAberta.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSensorGavetaAberta.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtSensorGavetaAberta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtSensorGavetaAberta.Location = new System.Drawing.Point(325, 32);
            this.txtSensorGavetaAberta.Name = "txtSensorGavetaAberta";
            this.txtSensorGavetaAberta.ReadOnly = true;
            this.txtSensorGavetaAberta.Size = new System.Drawing.Size(37, 20);
            this.txtSensorGavetaAberta.TabIndex = 9;
            // 
            // lblSensorGavetaAberta
            // 
            this.lblSensorGavetaAberta.AutoSize = true;
            this.lblSensorGavetaAberta.Location = new System.Drawing.Point(210, 34);
            this.lblSensorGavetaAberta.Name = "lblSensorGavetaAberta";
            this.lblSensorGavetaAberta.Size = new System.Drawing.Size(79, 13);
            this.lblSensorGavetaAberta.TabIndex = 8;
            this.lblSensorGavetaAberta.Text = "Gaveta Aberta:";
            // 
            // txtSensorBaixoBico
            // 
            this.txtSensorBaixoBico.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtSensorBaixoBico.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSensorBaixoBico.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtSensorBaixoBico.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtSensorBaixoBico.Location = new System.Drawing.Point(130, 110);
            this.txtSensorBaixoBico.Name = "txtSensorBaixoBico";
            this.txtSensorBaixoBico.ReadOnly = true;
            this.txtSensorBaixoBico.Size = new System.Drawing.Size(37, 20);
            this.txtSensorBaixoBico.TabIndex = 7;
            // 
            // lblSensorBaixoBico
            // 
            this.lblSensorBaixoBico.AutoSize = true;
            this.lblSensorBaixoBico.Location = new System.Drawing.Point(53, 112);
            this.lblSensorBaixoBico.Name = "lblSensorBaixoBico";
            this.lblSensorBaixoBico.Size = new System.Drawing.Size(60, 13);
            this.lblSensorBaixoBico.TabIndex = 6;
            this.lblSensorBaixoBico.Text = "Baixo Bico:";
            // 
            // txtSensorAltoBico
            // 
            this.txtSensorAltoBico.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtSensorAltoBico.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSensorAltoBico.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtSensorAltoBico.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtSensorAltoBico.Location = new System.Drawing.Point(130, 84);
            this.txtSensorAltoBico.Name = "txtSensorAltoBico";
            this.txtSensorAltoBico.ReadOnly = true;
            this.txtSensorAltoBico.Size = new System.Drawing.Size(37, 20);
            this.txtSensorAltoBico.TabIndex = 5;
            // 
            // lblSensorAltoBico
            // 
            this.lblSensorAltoBico.AutoSize = true;
            this.lblSensorAltoBico.Location = new System.Drawing.Point(53, 86);
            this.lblSensorAltoBico.Name = "lblSensorAltoBico";
            this.lblSensorAltoBico.Size = new System.Drawing.Size(52, 13);
            this.lblSensorAltoBico.TabIndex = 4;
            this.lblSensorAltoBico.Text = "Alto Bico:";
            // 
            // txtSensorEsponja
            // 
            this.txtSensorEsponja.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtSensorEsponja.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSensorEsponja.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtSensorEsponja.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtSensorEsponja.Location = new System.Drawing.Point(130, 58);
            this.txtSensorEsponja.Name = "txtSensorEsponja";
            this.txtSensorEsponja.ReadOnly = true;
            this.txtSensorEsponja.Size = new System.Drawing.Size(37, 20);
            this.txtSensorEsponja.TabIndex = 3;
            // 
            // lblSensorEsponja
            // 
            this.lblSensorEsponja.AutoSize = true;
            this.lblSensorEsponja.Location = new System.Drawing.Point(53, 60);
            this.lblSensorEsponja.Name = "lblSensorEsponja";
            this.lblSensorEsponja.Size = new System.Drawing.Size(48, 13);
            this.lblSensorEsponja.TabIndex = 2;
            this.lblSensorEsponja.Text = "Esponja:";
            // 
            // txtSensorCopo
            // 
            this.txtSensorCopo.BorderColor = System.Drawing.Color.Gainsboro;
            this.txtSensorCopo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSensorCopo.Conteudo = Percolore.Core.UserControl.TipoConteudo.Inteiro;
            this.txtSensorCopo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.txtSensorCopo.Location = new System.Drawing.Point(130, 32);
            this.txtSensorCopo.Name = "txtSensorCopo";
            this.txtSensorCopo.ReadOnly = true;
            this.txtSensorCopo.Size = new System.Drawing.Size(37, 20);
            this.txtSensorCopo.TabIndex = 1;
            // 
            // lblSensorCopo
            // 
            this.lblSensorCopo.AutoSize = true;
            this.lblSensorCopo.Location = new System.Drawing.Point(53, 34);
            this.lblSensorCopo.Name = "lblSensorCopo";
            this.lblSensorCopo.Size = new System.Drawing.Size(35, 13);
            this.lblSensorCopo.TabIndex = 0;
            this.lblSensorCopo.Text = "Copo:";
            // 
            // gbAction
            // 
            this.gbAction.Controls.Add(this.gbActionComand);
            this.gbAction.Controls.Add(this.btnCancelar);
            this.gbAction.Location = new System.Drawing.Point(28, 203);
            this.gbAction.Name = "gbAction";
            this.gbAction.Size = new System.Drawing.Size(342, 390);
            this.gbAction.TabIndex = 38;
            this.gbAction.TabStop = false;
            this.gbAction.Text = "Processos";
            // 
            // gbActionComand
            // 
            this.gbActionComand.Controls.Add(this.btnSubirBico);
            this.gbActionComand.Controls.Add(this.btnDescerBico);
            this.gbActionComand.Controls.Add(this.btnValvulaRecirculacao);
            this.gbActionComand.Controls.Add(this.btnAbrirGaveta);
            this.gbActionComand.Controls.Add(this.btnFecharGaveta);
            this.gbActionComand.Controls.Add(this.btnValvulaDosagem);
            this.gbActionComand.Location = new System.Drawing.Point(11, 19);
            this.gbActionComand.Name = "gbActionComand";
            this.gbActionComand.Size = new System.Drawing.Size(221, 365);
            this.gbActionComand.TabIndex = 38;
            this.gbActionComand.TabStop = false;
            this.gbActionComand.Text = "Comandos";
            // 
            // btnSubirBico
            // 
            this.btnSubirBico.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSubirBico.BackColor = System.Drawing.SystemColors.Control;
            this.btnSubirBico.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubirBico.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btnSubirBico.ForeColor = System.Drawing.Color.Black;
            this.btnSubirBico.Location = new System.Drawing.Point(7, 247);
            this.btnSubirBico.Name = "btnSubirBico";
            this.btnSubirBico.Size = new System.Drawing.Size(196, 49);
            this.btnSubirBico.TabIndex = 38;
            this.btnSubirBico.Text = "Subir Bico";
            this.btnSubirBico.UseVisualStyleBackColor = false;
            this.btnSubirBico.Click += new System.EventHandler(this.btnSubirBico_Click);
            // 
            // btnDescerBico
            // 
            this.btnDescerBico.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnDescerBico.BackColor = System.Drawing.SystemColors.Control;
            this.btnDescerBico.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDescerBico.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btnDescerBico.ForeColor = System.Drawing.Color.Black;
            this.btnDescerBico.Location = new System.Drawing.Point(7, 305);
            this.btnDescerBico.Name = "btnDescerBico";
            this.btnDescerBico.Size = new System.Drawing.Size(196, 49);
            this.btnDescerBico.TabIndex = 39;
            this.btnDescerBico.Text = "Descer Bico";
            this.btnDescerBico.UseVisualStyleBackColor = false;
            this.btnDescerBico.Click += new System.EventHandler(this.btnDescerBico_Click);
            // 
            // btnValvulaRecirculacao
            // 
            this.btnValvulaRecirculacao.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnValvulaRecirculacao.BackColor = System.Drawing.SystemColors.Control;
            this.btnValvulaRecirculacao.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnValvulaRecirculacao.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btnValvulaRecirculacao.ForeColor = System.Drawing.Color.Black;
            this.btnValvulaRecirculacao.Location = new System.Drawing.Point(6, 194);
            this.btnValvulaRecirculacao.Name = "btnValvulaRecirculacao";
            this.btnValvulaRecirculacao.Size = new System.Drawing.Size(196, 49);
            this.btnValvulaRecirculacao.TabIndex = 37;
            this.btnValvulaRecirculacao.Text = "Válvula Recirculação";
            this.btnValvulaRecirculacao.UseVisualStyleBackColor = false;
            this.btnValvulaRecirculacao.Click += new System.EventHandler(this.btnValvulaRecirculacao_Click);
            // 
            // btnAbrirGaveta
            // 
            this.btnAbrirGaveta.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnAbrirGaveta.BackColor = System.Drawing.SystemColors.Control;
            this.btnAbrirGaveta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbrirGaveta.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btnAbrirGaveta.ForeColor = System.Drawing.Color.Black;
            this.btnAbrirGaveta.Location = new System.Drawing.Point(6, 26);
            this.btnAbrirGaveta.Name = "btnAbrirGaveta";
            this.btnAbrirGaveta.Size = new System.Drawing.Size(196, 49);
            this.btnAbrirGaveta.TabIndex = 34;
            this.btnAbrirGaveta.Text = "Abrir Gaveta";
            this.btnAbrirGaveta.UseVisualStyleBackColor = false;
            this.btnAbrirGaveta.Click += new System.EventHandler(this.btnAbrirGaveta_Click);
            // 
            // btnFecharGaveta
            // 
            this.btnFecharGaveta.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnFecharGaveta.BackColor = System.Drawing.SystemColors.Control;
            this.btnFecharGaveta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFecharGaveta.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btnFecharGaveta.ForeColor = System.Drawing.Color.Black;
            this.btnFecharGaveta.Location = new System.Drawing.Point(6, 84);
            this.btnFecharGaveta.Name = "btnFecharGaveta";
            this.btnFecharGaveta.Size = new System.Drawing.Size(196, 49);
            this.btnFecharGaveta.TabIndex = 35;
            this.btnFecharGaveta.Text = "Fechar Gaveta";
            this.btnFecharGaveta.UseVisualStyleBackColor = false;
            this.btnFecharGaveta.Click += new System.EventHandler(this.btnFecharGaveta_Click);
            // 
            // btnValvulaDosagem
            // 
            this.btnValvulaDosagem.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnValvulaDosagem.BackColor = System.Drawing.SystemColors.Control;
            this.btnValvulaDosagem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnValvulaDosagem.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btnValvulaDosagem.ForeColor = System.Drawing.Color.Black;
            this.btnValvulaDosagem.Location = new System.Drawing.Point(6, 139);
            this.btnValvulaDosagem.Name = "btnValvulaDosagem";
            this.btnValvulaDosagem.Size = new System.Drawing.Size(196, 49);
            this.btnValvulaDosagem.TabIndex = 36;
            this.btnValvulaDosagem.Text = "Válvula Dosagem";
            this.btnValvulaDosagem.UseVisualStyleBackColor = false;
            this.btnValvulaDosagem.Click += new System.EventHandler(this.btnValvulaDosagem_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancelar.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancelar.Enabled = false;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI Light", 15F);
            this.btnCancelar.ForeColor = System.Drawing.Color.Black;
            this.btnCancelar.Location = new System.Drawing.Point(252, 324);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(87, 39);
            this.btnCancelar.TabIndex = 37;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Enabled = false;
            this.progressBar.ForeColor = System.Drawing.SystemColors.ControlText;
            this.progressBar.Location = new System.Drawing.Point(487, 166);
            this.progressBar.MarqueeAnimationSpeed = 15;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(146, 30);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 39;
            this.progressBar.Visible = false;
            // 
            // fPlacaMovManutencao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 605);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.gbSensores);
            this.Controls.Add(this.gbAction);
            this.Controls.Add(this.lblSubStatus);
            this.Controls.Add(this.pbImageBck);
            this.Controls.Add(this.btn_Fechar);
            this.Controls.Add(this.lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "fPlacaMovManutencao";
            this.ShowIcon = false;
            this.Text = "fPlacaMovManutencao";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fPlacaMovManutencao_FormClosing);
            this.Load += new System.EventHandler(this.fPlacaMovManutencao_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbImageBck)).EndInit();
            this.gbSensores.ResumeLayout(false);
            this.gbSensores.PerformLayout();
            this.gbAction.ResumeLayout(false);
            this.gbActionComand.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Fechar;
        private System.Windows.Forms.PictureBox pbImageBck;
        private System.Windows.Forms.Label lblSubStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox gbSensores;
        private System.Windows.Forms.Label lblSensorCopo;
        private System.Windows.Forms.GroupBox gbAction;
        private Percolore.Core.UserControl.UTextBox txtSensorCopo;
        private Percolore.Core.UserControl.UTextBox txtSensorValvulaFechada;
        private System.Windows.Forms.Label lblSensorVavlulaRecirculacao;
        private Percolore.Core.UserControl.UTextBox txtSensorValvulaAberta;
        private System.Windows.Forms.Label lblSensorValvulaDosagem;
        private Percolore.Core.UserControl.UTextBox txtSensorGavetaFechada;
        private System.Windows.Forms.Label lblSensorGavetaFechada;
        private Percolore.Core.UserControl.UTextBox txtSensorGavetaAberta;
        private System.Windows.Forms.Label lblSensorGavetaAberta;
        private Percolore.Core.UserControl.UTextBox txtSensorBaixoBico;
        private System.Windows.Forms.Label lblSensorBaixoBico;
        private Percolore.Core.UserControl.UTextBox txtSensorAltoBico;
        private System.Windows.Forms.Label lblSensorAltoBico;
        private Percolore.Core.UserControl.UTextBox txtSensorEsponja;
        private System.Windows.Forms.Label lblSensorEsponja;
        private System.Windows.Forms.Button btnValvulaDosagem;
        private System.Windows.Forms.Button btnFecharGaveta;
        private System.Windows.Forms.Button btnAbrirGaveta;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.GroupBox gbActionComand;
        private System.Windows.Forms.Button btnValvulaRecirculacao;
        private System.Windows.Forms.Button btnSubirBico;
        private System.Windows.Forms.Button btnDescerBico;
        private Percolore.Core.UserControl.UTextBox txtSensorEmergencia;
        private System.Windows.Forms.Label lblSensorEmergencia;
        private Percolore.Core.UserControl.UTextBox txtCodErro;
        private System.Windows.Forms.Label lblSensorCodErro;
        private Percolore.Core.UserControl.UTextBox txtCodAlerta;
        private System.Windows.Forms.Label lblSensorCodAlerta;
        private Percolore.Core.UserControl.UTextBox txtMaquinaLigada;
        private System.Windows.Forms.Label lblSensorMaqLigada;
    }
}