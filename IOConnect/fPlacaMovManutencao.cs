using Percolore.Core.Logging;
using System.ComponentModel;

namespace Percolore.IOConnect
{
	public partial class fPlacaMovManutencao : Form
    {
        Util.ObjectParametros _parametros = null;
        System.ComponentModel.BackgroundWorker backgroundWorker1 = null;
        private bool isThread = false;
        private bool isRunning = false;

        // 0 -> Abrindo Gaveta
        // 1 -> Fechando Gaveta
        // 3 -> Avança Válvulas
        // 4 -> Recua Válvulas
        private int tipoAction = -1;
        private int passosAction = 0;

        private ModBusDispenserMover_P3 modBusDispenser_P3 = null;

        public fPlacaMovManutencao()
        {
            InitializeComponent();
            this._parametros = Util.ObjectParametros.Load();

            this.modBusDispenser_P3 = new ModBusDispenserMover_P3(this._parametros.NomeDispositivo, this._parametros.NomeDispositivo_PlacaMov);

        }

        private void fPlacaMovManutencao_Load(object sender, EventArgs e)
        {
            try
            {
                pbImageBck.Visible = false;
                lblSubStatus.Visible = false;
                //lblStatus.Text = "Executar Novo Processo";
                lblStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Novo_Proc;
                gbActionComand.Text = Negocio.IdiomaResxExtensao.Manutencao_GB_Comandos;
                gbAction.Text = Negocio.IdiomaResxExtensao.Manutencao_GB_Processos;
                gbSensores.Text = Negocio.IdiomaResxExtensao.Manutencao_GB_Sensores;
                btnAbrirGaveta.Text = Negocio.IdiomaResxExtensao.Manutencao_BT_Abrir_Gaveta;
                btnFecharGaveta.Text = Negocio.IdiomaResxExtensao.Manutencao_BT_Fechar_Gaveta;
                btnValvulaDosagem.Text = Negocio.IdiomaResxExtensao.Manutencao_BT_Valvula_Dosagem;
                btnValvulaRecirculacao.Text = Negocio.IdiomaResxExtensao.Manutencao_BT_Valvula_Recirculacao;
                btnSubirBico.Text = Negocio.IdiomaResxExtensao.Manutencao_BT_Subir_Bico;
                btnDescerBico.Text = Negocio.IdiomaResxExtensao.Manutencao_BT_Descer_Bico;
                btn_Fechar.Text = Negocio.IdiomaResxExtensao.Manutencao_Fechar;
                btnCancelar.Text = Negocio.IdiomaResxExtensao.Manutencao_BT_Cancel;

                lblSensorCopo.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Copo;
                lblSensorAltoBico.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Alto_Bico;
                lblSensorBaixoBico.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Baixo_Bico;
                lblSensorEsponja.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Esponja;
                lblSensorGavetaAberta.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Gaveta_Aberta;
                lblSensorGavetaFechada.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Gaveta_Fechada;
                lblSensorValvulaDosagem.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Valvula_Dosagem;
                lblSensorVavlulaRecirculacao.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Valvula_Recirculacao;
                lblSensorCodAlerta.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Alerta;
                lblSensorCodErro.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Cod_Erro;
                lblSensorEmergencia.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Emergencia;
                lblSensorMaqLigada.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Maquina_Ligada;


                bool chb_tec = this._parametros.HabilitarTecladoVirtual;
                bool chb_touch = this._parametros.HabilitarTouchScrenn;

                txtSensorCopo.isTecladoShow = chb_tec;
                txtSensorAltoBico.isTecladoShow = chb_tec;
                txtSensorBaixoBico.isTecladoShow = chb_tec;
                txtSensorEsponja.isTecladoShow = chb_tec;
                txtSensorGavetaAberta.isTecladoShow = chb_tec;
                txtSensorGavetaFechada.isTecladoShow = chb_tec;
                txtSensorValvulaAberta.isTecladoShow = chb_tec;
                txtSensorValvulaFechada.isTecladoShow = chb_tec;

                txtSensorAltoBico.isTouchScrenn = chb_touch;
                txtSensorBaixoBico.isTouchScrenn = chb_touch;
                txtSensorCopo.isTouchScrenn = chb_touch;
                txtSensorEsponja.isTouchScrenn = chb_touch;
                txtSensorGavetaAberta.isTouchScrenn = chb_touch;
                txtSensorGavetaFechada.isTouchScrenn = chb_touch;
                txtSensorValvulaAberta.isTouchScrenn = chb_touch;
                txtSensorValvulaFechada.isTouchScrenn = chb_touch;

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void fPlacaMovManutencao_FormClosing(object sender, FormClosingEventArgs e)
        {
            PausarMonitoramento();
            switch (this.DialogResult)
            {
                #region Log de processo

                case DialogResult.OK:
                    {
                        //Log.Logar(
                        //    TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Global_RecircularIndividualConcluida);

                        break;
                    }
                case DialogResult.Cancel:
                    {
                        //Log.Logar(
                        //    TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_33 + Negocio.IdiomaResxExtensao.Global_RecircularIndividualCancelada);
                        break;
                    }

                case DialogResult.No:
                    {
                        //Log.Logar(
                        //    TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_35 + Negocio.IdiomaResxExtensao.Global_FalhaRecircularIndividual);
                        break;
                    }

                    #endregion
            }

        }

        private void btn_Fechar_Click(object sender, EventArgs e)
        {
            try
            {
                PausarMonitoramento();
                DialogResult = DialogResult.OK;               
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnAbrirGaveta_Click(object sender, EventArgs e)
        {
            try
            {
                this.modBusDispenser_P3.Disconnect_Mover();
                Thread.Sleep(200);
                this.modBusDispenser_P3.Connect_Mover();
                Thread.Sleep(200);
                this.modBusDispenser_P3.ReadSensores_Mover();
                if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorEmergencia)
                {
                    //Image img = Percolore.IOConnect.Properties.IOConnect.openDrawer;
                    Image img = Percolore.IOConnect.Imagem.Get_PlacaMov_Open_Drawer();
                    pbImageBck.Image = (Bitmap)img.Clone();

                    //lblStatus.Text = "Abrindo Gaveta";
                    lblStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                    lblSubStatus.Text = "";
                    lblSubStatus.Visible = true;
                    pbImageBck.Visible = true;

                    this.tipoAction = 0;
                    this.passosAction = 0;
                    ExecutarMonitoramento();
                }
                else if (this.modBusDispenser_P3.SensorEmergencia)
                {
                    //MessageBox.Show("Emergência pressionado");
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Emergencia);
                }
                else
                {
                    //MessageBox.Show("Error Nativo:" + this.modBusDispenser_P3.IsNativo.ToString());
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Erro_Nativo + this.modBusDispenser_P3.IsNativo.ToString());
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    MessageBox.Show("Error:" + ex.Message);
            }
        }

        private void btnFecharGaveta_Click(object sender, EventArgs e)
        {

            try
            {
                this.modBusDispenser_P3.Disconnect_Mover();
                Thread.Sleep(200);
                this.modBusDispenser_P3.Connect_Mover();
                Thread.Sleep(200);
                this.modBusDispenser_P3.ReadSensores_Mover();
                if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorEmergencia)
                {
                    //Image img = Percolore.IOConnect.Properties.IOConnect.openDrawer;
                    Image img = Percolore.IOConnect.Imagem.Get_PlacaMov_Close_Drawer();
                    pbImageBck.Image = (Bitmap)img.Clone();

                    //lblStatus.Text = "Fechando Gaveta";
                    lblStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                    pbImageBck.Visible = true;
                    lblSubStatus.Text = "";
                    lblSubStatus.Visible = true;
                    gbActionComand.Enabled = false;
                    this.tipoAction = 1;
                    
                    this.passosAction = 0;
                    ExecutarMonitoramento();
                }
                else if (this.modBusDispenser_P3.SensorEmergencia)
                {
                    //MessageBox.Show("Emergência pressionado");
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Emergencia);                   
                }                
                else
                {
                    //MessageBox.Show("Error Nativo:" + this.modBusDispenser_P3.IsNativo.ToString());
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Erro_Nativo + this.modBusDispenser_P3.IsNativo.ToString());
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    MessageBox.Show("Error:" + ex.Message);
            }

        }

        private void btnValvulaDosagem_Click(object sender, EventArgs e)
        {
            try
            {
                //gbActionComand.Enabled = false;
                //this.tipoAction = 2;
                //this.passosAction = 0;

                this.modBusDispenser_P3.Disconnect_Mover();
                Thread.Sleep(200);
                this.modBusDispenser_P3.Connect_Mover();
                Thread.Sleep(200);
                this.modBusDispenser_P3.ReadSensores_Mover();
                if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorEmergencia)
                {

                    pbImageBck.Image = null;

                    //lblStatus.Text = "Posicionar Válvulas Dosagem";
                    lblStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                    lblSubStatus.Text = "";
                    lblSubStatus.Visible = true;
                    pbImageBck.Visible = true;
                    gbActionComand.Enabled = false;
                    this.tipoAction = 2;
                    this.passosAction = 0;
                    ExecutarMonitoramento();
                }
                else if (this.modBusDispenser_P3.SensorEmergencia)
                {
                    //MessageBox.Show("Emergência pressionado");
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Emergencia);
                }
                else
                {
                    //MessageBox.Show("Error Nativo:" + this.modBusDispenser_P3.IsNativo.ToString());
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Erro_Nativo + this.modBusDispenser_P3.IsNativo.ToString());
                }

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    MessageBox.Show("Error:" + ex.Message);
            }
        }

        private void btnValvulaRecirculacao_Click(object sender, EventArgs e)
        {
            try
            {
                //gbActionComand.Enabled = false;
                //this.tipoAction = 2;
                //this.passosAction = 0;

                this.modBusDispenser_P3.Disconnect_Mover();
                Thread.Sleep(200);
                this.modBusDispenser_P3.Connect_Mover();
                Thread.Sleep(200);
                this.modBusDispenser_P3.ReadSensores_Mover();
                if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorEmergencia)
                {
                    //Image img = Percolore.IOConnect.Properties.IOConnect.openDrawer;
                    //Image img = Percolore.IOConnect.Imagem.Get_PlacaMov_Close_Drawer();
                    //pbImageBck.Image = (Bitmap)img.Clone();
                    pbImageBck.Image = null;
                    //lblStatus.Text = "Posicionar Válvulas Recirculação";
                    lblStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                    lblSubStatus.Text = "";
                    lblSubStatus.Visible = true;
                    pbImageBck.Visible = true;
                    gbActionComand.Enabled = false;
                    this.tipoAction = 3;
                    this.passosAction = 0;
                    ExecutarMonitoramento();
                }
                else if (this.modBusDispenser_P3.SensorEmergencia)
                {
                    //MessageBox.Show("Emergência pressionado");
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Emergencia);
                }
                else
                {
                    //MessageBox.Show("Error Nativo:" + this.modBusDispenser_P3.IsNativo.ToString());
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Erro_Nativo + this.modBusDispenser_P3.IsNativo.ToString());
                }

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    MessageBox.Show("Error:" + ex.Message);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                this.modBusDispenser_P3.Disconnect_Mover();
                Thread.Sleep(200);
                this.modBusDispenser_P3.Connect_Mover();
                Thread.Sleep(200);
                this.modBusDispenser_P3.ReadSensores_Mover();
                this.Invoke(new MethodInvoker(atualizaGridEnable));
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    MessageBox.Show("Error:" + ex.Message);
            }
        }

        private void btnDescerBico_Click(object sender, EventArgs e)
        {
            try
            {
                //gbActionComand.Enabled = false;
                //this.tipoAction = 2;
                //this.passosAction = 0;

                this.modBusDispenser_P3.Disconnect_Mover();
                Thread.Sleep(200);
                this.modBusDispenser_P3.Connect_Mover();
                Thread.Sleep(200);
                this.modBusDispenser_P3.ReadSensores_Mover();
                if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2 ) && !this.modBusDispenser_P3.SensorEmergencia)
                {
                    //Image img = Percolore.IOConnect.Properties.IOConnect.openDrawer;
                    //Image img = Percolore.IOConnect.Imagem.Get_PlacaMov_Close_Drawer();
                    //pbImageBck.Image = (Bitmap)img.Clone();
                    Image img = Percolore.IOConnect.Imagem.Get_PlacaMov_Bicos();
                    pbImageBck.Image = (Bitmap)img.Clone();
                    //pbImageBck.Image = null;
                    //lblStatus.Text = "Descer Bicos";
                    lblStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Descer_Bico;
                    lblSubStatus.Text = "";
                    lblSubStatus.Visible = true;
                    pbImageBck.Visible = true;
                    gbActionComand.Enabled = false;
                    this.tipoAction = 5;
                    this.passosAction = 0;
                    ExecutarMonitoramento();
                }
                else if (this.modBusDispenser_P3.SensorEmergencia)
                {
                    //MessageBox.Show("Emergência pressionado");
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Emergencia);
                }
                else
                {
                    //MessageBox.Show("Error Nativo:" + this.modBusDispenser_P3.IsNativo.ToString());
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Erro_Nativo + this.modBusDispenser_P3.IsNativo.ToString());
                }

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    MessageBox.Show("Error:" + ex.Message);
            }
        }

        private void btnSubirBico_Click(object sender, EventArgs e)
        {
            try
            {
                //gbActionComand.Enabled = false;
                //this.tipoAction = 2;
                //this.passosAction = 0;

                this.modBusDispenser_P3.Disconnect_Mover();
                Thread.Sleep(200);
                this.modBusDispenser_P3.Connect_Mover();
                Thread.Sleep(200);
                this.modBusDispenser_P3.ReadSensores_Mover();
                if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2 ) && !this.modBusDispenser_P3.SensorEmergencia)
                {
                    //Image img = Percolore.IOConnect.Properties.IOConnect.openDrawer;
                    //Image img = Percolore.IOConnect.Imagem.Get_PlacaMov_Close_Drawer();
                    //pbImageBck.Image = (Bitmap)img.Clone();
                    Image img = Percolore.IOConnect.Imagem.Get_PlacaMov_Bicos();
                    pbImageBck.Image = (Bitmap)img.Clone();
                    //lblStatus.Text = "Subir Bicos";
                    lblStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Subir_Bico;
                    lblSubStatus.Text = "";
                    lblSubStatus.Visible = true;
                    pbImageBck.Visible = true;
                    gbActionComand.Enabled = false;
                    this.tipoAction = 4;
                    this.passosAction = 0;
                    ExecutarMonitoramento();
                }
                else if(this.modBusDispenser_P3.SensorEmergencia)
                {
                    //MessageBox.Show("Emergência pressionado");
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Emergencia);
                }
                else
                {
                    //MessageBox.Show("Error Nativo:" + this.modBusDispenser_P3.IsNativo.ToString());
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Erro_Nativo + this.modBusDispenser_P3.IsNativo.ToString());
                }

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    MessageBox.Show("Error:" + ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            PausarMonitoramento();
        }           

        void MonitoramentoEvent()
        {
            try
            {
                // 0 -> Abrindo Gaveta
                if (this.tipoAction== 0)
                {
                    if(this.passosAction == 0)
                    {
                        //this.modBusDispenser_P3.Disconnect_Mover();
                        //this.modBusDispenser_P3.Connect_Mover();
                        //Thread.Sleep(1000);
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));                        
                        //lblSubStatus.Text = "Executando Comando";
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Executando_Comando;
                        //this.Invoke(new MethodInvoker(AtualizarStatusComando));
                        this.passosAction++;
                    }
                    else
                    {
                        //lblSubStatus.Text = "Aguardando Comando terminar";
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Aguarde_Comando;
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            
                            //this.Invoke(new MethodInvoker(AtualizarAguardandoComando));
                            if (this.modBusDispenser_P3.IsNativo != 1 || this.modBusDispenser_P3.SensorEmergencia)
                            {
                                PausarMonitoramento();
                                return;

                            }
                        }
                        else if(this.modBusDispenser_P3.SensorEmergencia)
                        {
                            PausarMonitoramento();
                            return;
                        }
                      
                    }
                }
                // 1 -> Fechando Gaveta
                else if (this.tipoAction == 1)
                {
                    if (this.passosAction == 0)
                    {
                        //this.modBusDispenser_P3.Disconnect_Mover();
                        //this.modBusDispenser_P3.Connect_Mover();
                        //Thread.Sleep(1000);
                        //this.modBusDispenser_P3.FecharGaveta();
                        
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));

                        ////lblSubStatus.Text = "Executando Comando";
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Executando_Comando;                        
                        this.passosAction++;
                    }
                    else if (this.passosAction == 1)
                    {
                        //lblSubStatus.Text = "Aguardando Comando terminar";
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Aguarde_Comando;
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            
                            if (this.modBusDispenser_P3.IsNativo != 1 || this.modBusDispenser_P3.SensorEmergencia)
                            {
                                PausarMonitoramento();
                                return;
                            }
                        }
                        else if (this.modBusDispenser_P3.SensorEmergencia)
                        {
                            PausarMonitoramento();
                            return;
                        }
                    }
                }
                // 2 -> Avancar Válvula
                else if (this.tipoAction == 2)
                {
                    if (this.passosAction == 0)
                    {
                        //this.modBusDispenser_P3.Disconnect_Mover();
                        //this.modBusDispenser_P3.Connect_Mover();
                        //Thread.Sleep(1000);
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //lblSubStatus.Text = "Executando Comando";
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Executando_Comando;
                        this.passosAction++;
                    }
                    else
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        //lblSubStatus.Text = "Aguardando Comando terminar";
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Aguarde_Comando;
                        if (this.modBusDispenser_P3.IsNativo != 1 || this.modBusDispenser_P3.SensorEmergencia)
                        {
                            PausarMonitoramento();
                            return;
                        }
                        
                    }
                }
                // 3 -> Recuar Válvulas
                else if (this.tipoAction == 3)
                {
                    if (this.passosAction == 0)
                    {
                        //this.modBusDispenser_P3.Disconnect_Mover();
                        //this.modBusDispenser_P3.Connect_Mover();
                        //Thread.Sleep(1000);
                        this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                        //lblSubStatus.Text = "Executando Comando";
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Executando_Comando;
                        this.passosAction++;
                    }
                    else
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        //lblSubStatus.Text = "Aguardando Comando terminar";
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Aguarde_Comando;
                        if (this.modBusDispenser_P3.IsNativo != 1 || this.modBusDispenser_P3.SensorEmergencia)
                        {
                            PausarMonitoramento();
                            return;
                        }

                    }
                }

                // 4 -> Subir Bico
                else if (this.tipoAction == 4)
                {
                    if (this.passosAction == 0)
                    {
                        //this.modBusDispenser_P3.Disconnect_Mover();
                        //this.modBusDispenser_P3.Connect_Mover();
                        //Thread.Sleep(1000);
                        this.modBusDispenser_P3.SubirBico();
                        //lblSubStatus.Text = "Executando Comando";
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Executando_Comando;
                        this.passosAction++;
                    }
                    else
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        //lblSubStatus.Text = "Aguardando Comando terminar";
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Aguarde_Comando;
                        if (this.modBusDispenser_P3.IsNativo != 1 || this.modBusDispenser_P3.SensorEmergencia)
                        {
                            PausarMonitoramento();
                            return;
                        }

                    }
                }
                // 5 -> Descer Bicos
                else if (this.tipoAction == 5)
                {
                    if (this.passosAction == 0)
                    {
                        //this.modBusDispenser_P3.Disconnect_Mover();
                        //this.modBusDispenser_P3.Connect_Mover();
                        //Thread.Sleep(1000);
                        this.modBusDispenser_P3.DescerBico();
                        //lblSubStatus.Text = "Executando Comando";
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Executando_Comando;
                        this.passosAction++;
                    }
                    else
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        //lblSubStatus.Text = "Aguardando Comando terminar";
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Aguarde_Comando;
                        if (this.modBusDispenser_P3.IsNativo != 1 || this.modBusDispenser_P3.SensorEmergencia)
                        {
                            PausarMonitoramento();
                            return;
                        }

                    }
                }
                // Indefinido a Ação

                else
                {
                    PausarMonitoramento();
                    return;
                }
                this.isRunning = false;

            }
            catch (Exception ex)
            {
                Falha(ex);
            }
        }

        #region Métodos privados

        private void ClosedSerialDispensa()
        {
            try
            {
                this.modBusDispenser_P3.Disconnect_Mover();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        void Falha(Exception ex)
		{
            LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			PausarMonitoramento();

            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
            {
                m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + ex.Message);
            }

            DialogResult = DialogResult.No;
        }

        void PausarMonitoramento()
        {
            try
            {
                this.isThread = false;
                this.isRunning = true;
                lblStatus.Text = "";
                lblSubStatus.Text = "";
                gbActionComand.Enabled = true;
                ClosedSerialDispensa();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        void ExecutarMonitoramento()
        {
            try
            {

                if (backgroundWorker1 == null)
                {
                    this.backgroundWorker1 = new BackgroundWorker();
                    this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker1_DoWork);
                    this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
                    this.backgroundWorker1.WorkerSupportsCancellation = true;
                    this.isThread = true;
                    this.isRunning = false;
                    gbActionComand.Enabled = false;
                    this.backgroundWorker1.RunWorkerAsync();
                }
                else
                {

                    this.backgroundWorker1 = new BackgroundWorker();
                    this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker1_DoWork);
                    this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
                    this.backgroundWorker1.WorkerSupportsCancellation = true;

                    if (!backgroundWorker1.IsBusy)
                    {
                        this.isThread = true;
                        this.isRunning = false;
                        backgroundWorker1.RunWorkerAsync();
                        gbActionComand.Enabled = false;
                    }
                    else
                    {
                        lblStatus.Text = "";
                        lblSubStatus.Text = "";
                        gbActionComand.Enabled = true;
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    gbActionComand.Enabled = true;
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                BackgroundWorker worker = sender as BackgroundWorker;
                while (this.isThread)
                {
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        this.isThread = false;
                    }
                    else
                    {
                        try
                        {
                            if (!this.isRunning)
                            {
                                this.isRunning = true;
                                this.Invoke(new MethodInvoker(MonitoramentoEvent));
                                Thread.Sleep(2000);
                            }
                        }
						catch (Exception ex)
						{
							LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
						}
					}
                    Thread.Sleep(200);
                }

                this.Invoke(new MethodInvoker(atualizaGridEnable));
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    this.isThread = false;
                }
                else if (!(e.Error == null))
                {
                    this.isThread = false;
                }
                else
                {
                    this.isThread = true;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void atualizaGridEnable()
        {
            try
            {
                if(!gbActionComand.Enabled)
                {
                    Thread.Sleep(2000);
                    gbActionComand.Enabled = true;
                }

                txtSensorCopo.Text = this.modBusDispenser_P3.SensorCopo ? "1" : "0";
                txtSensorAltoBico.Text = this.modBusDispenser_P3.SensorAltoBicos ? "1" : "0";
                txtSensorBaixoBico.Text = this.modBusDispenser_P3.SensorBaixoBicos ? "1" : "0";
                txtSensorEsponja.Text = this.modBusDispenser_P3.SensorEsponja ? "1" : "0";
                txtSensorGavetaAberta.Text = this.modBusDispenser_P3.SensorGavetaAberta ? "1" : "0";
                txtSensorGavetaFechada.Text = this.modBusDispenser_P3.SensorGavetaFechada ? "1" : "0";
                txtSensorValvulaAberta.Text = this.modBusDispenser_P3.SensorValvulaDosagem ? "1" : "0";
                txtSensorValvulaFechada.Text = this.modBusDispenser_P3.SensorValvulaRecirculacao ? "1" : "0";
                txtSensorEmergencia.Text = this.modBusDispenser_P3.SensorEmergencia ? "1" : "0";
                txtCodErro.Text = this.modBusDispenser_P3.CodError.ToString();
                txtCodAlerta.Text = this.modBusDispenser_P3.CodAlerta.ToString();
                txtMaquinaLigada.Text = this.modBusDispenser_P3.MaquinaLigada ? "1" : "0";
                if (this.modBusDispenser_P3.CodError > 0)
                {
                    //MessageBox.Show("Error Código:" + this.modBusDispenser_P3.GetDescCodError());
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_LBL_Cod_Erro + this.modBusDispenser_P3.GetDescCodError());
                }
                if(this.modBusDispenser_P3.SensorEmergencia)
                {
                    //MessageBox.Show("Emergência Pressionado!");
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Emergencia);
                }
                
                if(this.modBusDispenser_P3.CodAlerta > 0)
                {
                    //MessageBox.Show("Alerta de fMensagem:" + this.modBusDispenser_P3.GetDescCodAlerta());
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Alerta_Mensagem + this.modBusDispenser_P3.GetDescCodAlerta());
                }
                if(!this.modBusDispenser_P3.MaquinaLigada)
                {
                //    //MessageBox.Show("Máquina Desligada!");
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Maquina_Desligada);
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        #endregion
       
    }
}