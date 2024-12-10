using Percolore.Core.Logging;
using System.ComponentModel;
using System.Data;

namespace Percolore.IOConnect
{
	public partial class fLimpezaBicos : Form
    {
        Util.ObjectParametros _parametros = null;
        System.ComponentModel.BackgroundWorker backgroundWorker1 = null;
        private bool isThread = false;
        private bool isRunning = false;

        PurgaVO _prmPurga;
        private List<Util.ObjectColorante> _colorantes = null;
        private Button[] _circuitos = null;

        int INDEX_CKT;
        int INDEX_ULTIMO_CKT;
        int INDEX_DISP;
        bool execPurga = false;
        int index_PurgaIndividual = 0;

        private int i_Step = 0;

        public fLimpezaBicos(PurgaVO parametrosPurga)
        {
            InitializeComponent();
            try
            {
                this._parametros = Util.ObjectParametros.Load();
                this._prmPurga = parametrosPurga;

                this._colorantes = Util.ObjectColorante.List().Where(o=>o.Dispositivo == 1).ToList();
                this._circuitos = new Button[] { btn_ckt_01, btn_ckt_02, btn_ckt_03, btn_ckt_04, btn_ckt_05, btn_ckt_06, btn_ckt_07, btn_ckt_08, btn_ckt_09, btn_ckt_10, btn_ckt_11, btn_ckt_12, btn_ckt_13, btn_ckt_14, btn_ckt_15, btn_ckt_16};

                this.INDEX_CKT = -1;
                this.INDEX_ULTIMO_CKT = -1;

                lblStatus.Text = Negocio.IdiomaResxExtensao.LimpezaBico_lblStatus_Msg01;
                lblSubStatus.Text = Negocio.IdiomaResxExtensao.LimpezaBico_lblSubStatus_Msg01;

                btn_Fechar.Text = Negocio.IdiomaResxExtensao.Manutencao_Fechar;
                btnAbrirGaveta.Text = Negocio.IdiomaResxExtensao.Manutencao_BT_Abrir_Gaveta;
                btnFecharGaveta.Text = Negocio.IdiomaResxExtensao.Manutencao_BT_Fechar_Gaveta;

                gbIndividual.Enabled = false;
                progressBar.Visible = false;

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void fLimpezaBicos_Load(object sender, EventArgs e)
        {
            try
            {
                List<Util.ObjectColorante> lcol = Util.ObjectColorante.List();
                foreach (Util.ObjectColorante _col in lcol)
                {
                    if (_col.Step > 0)
                    {
                        i_Step = _col.Step;
                        break;
                    }
                }
            
                for (int i = 0; i <= 15; i++)
                {
                    this._circuitos[i].Enabled = this._colorantes[i].Habilitado;
                    this._circuitos[i].Text = this._colorantes[i].Nome;
                    if (this._colorantes[i].Habilitado)
                    {
                        this._circuitos[i].BackColor = Color.FromArgb(6, 176, 37);
                    }
                    else
                    {
                        this._circuitos[i].BackColor = Color.Red;
                    }
                }

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void fLimpezaBicos_FormClosing(object sender, FormClosingEventArgs e)
        {
            //PausarMonitoramento();
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

        private void Circuito_Click(object sender, EventArgs e)
        {
            try
            {
                this.execPurga = true;
                int circuito = int.Parse(((Button)sender).Tag.ToString()) + 1;

                for (int i = 0; i < this._prmPurga.Colorantes.Count; i++)
                {
                    if (circuito == this._prmPurga.Colorantes[i].Circuito)
                    {
                        this.INDEX_CKT = i;
                        this.INDEX_DISP = this._prmPurga.Colorantes[i].Dispositivo - 1;
                    }
                }
            
                this.Visible = true;
                this.execPurga = true;
                if (execPurga)
                {
                    if (this._prmPurga.DispenserP3 != null)
                    {
                        bool isPosicao = false;
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!(this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2))
                        {
                            //lblStatus.Text = "Condição de placa movimentação incorreta!";
                            lblStatus.Text = Negocio.IdiomaResxExtensao.PlacaMov_Condicao_Incorreta;
                            lblSubStatus.Text = "";
                            return;
                        }
                        if (!this._prmPurga.DispenserP3.SensorGavetaAberta )
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            for (int _i = 0; _i < 20; _i++)
                            {
                                Thread.Sleep(500);
                                if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                                {
                                    _i = 20;
                                }
                                
                            }
                            //Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            isPosicao = true;
                        }
                        if (!this._prmPurga.DispenserP3.SensorValvulaDosagem )
                        {
                            this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                            for (int _i = 0; _i < 20; _i++)
                            {
                                Thread.Sleep(500);
                                this._prmPurga.DispenserP3.ReadSensores_Mover();
                                if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                                {
                                    _i = 20;
                                }                                
                            }
                            isPosicao = true;
                        }
                        if (isPosicao)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if ((!this._prmPurga.DispenserP3.SensorGavetaAberta) || (!this._prmPurga.DispenserP3.SensorValvulaDosagem))
                            {
                                //lblStatus.Text = "Condição de placa movimentação incorreta!";
                                lblStatus.Text = Negocio.IdiomaResxExtensao.PlacaMov_Condicao_Incorreta;
                                lblSubStatus.Text = "";
                                return;
                            }
                        }
                        if (!this._prmPurga.DispenserP3.SensorBaixoBicos)
                        {
                            this._prmPurga.DispenserP3.DescerBico();
                        }
                        this.INDEX_ULTIMO_CKT = this.INDEX_CKT;
                        this.index_PurgaIndividual = this.INDEX_CKT;
                        gbIndividual.Enabled = false;
                        progressBar.Visible = true;
                        this.Invoke(new MethodInvoker(ExecutarMonitoramento));
                    }                   
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        #region Métodos privados

        void Falha(Exception ex, string customMessage = null)
		{
            LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			PausarMonitoramento();

            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
            {
                m.ShowDialog(
					string.IsNullOrWhiteSpace(customMessage) ? Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + ex.Message
                                                             : customMessage);
            }

            DialogResult = DialogResult.No;
        }

        void Emergencia()
        {
            PausarMonitoramento();
            try
            {
                this._prmPurga.DispenserP3.Halt();
                this._prmPurga.DispenserP3.UnHalt();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
            {
                //m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + "Botão de Emergência pressionado!");
                m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + Negocio.IdiomaResxExtensao.PlacaMov_Botao_Emergencia);
            }
        }

        void DispensaForceHalt()
        {
            PausarMonitoramento();
            try
            {
                this._prmPurga.DispenserP3.Halt();
                this._prmPurga.DispenserP3.UnHalt();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
            {
                //m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + "Dispensa parou!");
                m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + Negocio.IdiomaResxExtensao.PlacaMov_Disp_Parou);
            }

            DialogResult = DialogResult.No;
        }

        void PausarMonitoramento()
        {
            try
            {
                this.isThread = false;
                this.isRunning = true;
                progressBar.Visible = false;
                lblStatus.Text = Negocio.IdiomaResxExtensao.LimpezaBico_lblStatus_Msg01;
                lblSubStatus.Text = Negocio.IdiomaResxExtensao.LimpezaBico_lblSubStatus_Msg01;
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
                    }
                    else
                    {
                        lblStatus.Text = Negocio.IdiomaResxExtensao.PurgarIndividualMonit_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.PurgarIndividual_lblSubStatus_Msg01;
                        gbIndividual.Enabled = true;
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    gbIndividual.Enabled = true;
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
                        if (!this.isRunning)
                        {
                            this.isRunning = true;
                            this.Invoke(new MethodInvoker(MonitoramentoEvent));
                            Thread.Sleep(500);
                        }
                    }
                    Thread.Sleep(500);
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
                if (!gbIndividual.Enabled)
                {
                    Thread.Sleep(2000);
                    gbIndividual.Enabled = true;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        void MonitoramentoEvent()
        {
            try
            {
                if (this._prmPurga.DispenserP3 != null)
                {
                    if (this._prmPurga.DispenserP3.IsEmergencia)
                    {
                        Emergencia();
                        return;
                    }
                    if (this._prmPurga.DispenserP3.IsHalt)
                    {
                        DispensaForceHalt();
                        return;
                    }
                    else if (!this._prmPurga.DispenserP3.IsReady)
                    {
                        this.isRunning = false;
                        Thread.Sleep(500);
                        return;
                    }

                    //Verifica se já executou todos os circuitos de colorantes]
                    if (INDEX_CKT > INDEX_ULTIMO_CKT)
                    {
                        this.isRunning = false;
                        this.execPurga = false;                      
                        PausarMonitoramento();
                        return;
                    }
                    
                    //Recupera posição do circuito do colorante
                    int CIRCUITO = this._prmPurga.Colorantes[this.INDEX_CKT].Circuito;
                    
                    //Atualiza interface
                    lblStatus.Text = Negocio.IdiomaResxExtensao.LimpezaBico_lblStatus_Msg02 + " " + this._prmPurga.Colorantes[this.INDEX_CKT].Nome;
                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.LimpezaBico_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                    //Dispara thread para enviar dados ao dispositivo
                    Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.Dispensar(
                            CIRCUITO - 1,
                            this._prmPurga.LMDispositivos[this.INDEX_DISP].PulsoHorario[CIRCUITO - 1],
                            this._prmPurga.LMDispositivos[this.INDEX_DISP].Velocidade[CIRCUITO - 1],
                            this._prmPurga.LMDispositivos[this.INDEX_DISP].Aceleracao[CIRCUITO - 1],
                            this._prmPurga.LMDispositivos[this.INDEX_DISP].Delay[CIRCUITO - 1],
                            this._prmPurga.LMDispositivos[this.INDEX_DISP].PulsoReverso[CIRCUITO - 1], 
                            i_Step)
                        );

                    if (_parametros.ControlarNivel)
                    {
                        Operar.AbaterColorante(this._prmPurga.Colorantes[this.INDEX_CKT].Circuito, this._prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);

                    }
                    
                    Util.ObjectRecircular.UpdateVolumeDosado(this._prmPurga.Colorantes[this.INDEX_CKT].Circuito, this._prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                    
                    //Incrementa contador
                    this.INDEX_CKT++;
                    this.isRunning = false;
                }
            }
            catch (Exception ex)
            {
				string customMessage = string.Empty;
				if (ex.Message.Contains("Could not read status register:"))
					customMessage = Negocio.IdiomaResxExtensao.Global_Falha_PerdaConexaoDispositivo;

				Falha(ex, customMessage);
            }
        }
        #endregion

        private void btnAbrirGaveta_Click(object sender, EventArgs e)
        {
            try
            {
                this._prmPurga.DispenserP3.ReadSensores_Mover();
                if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorEmergencia )
                {
                    if (!this._prmPurga.DispenserP3.SensorGavetaAberta && this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                    {
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_BT_Abrir_Gaveta);
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.Manutencao_BT_Abrir_Gaveta);
                        }
                    }
                    
                    gbIndividual.Enabled = true;
                    
                }
                else if (this._prmPurga.DispenserP3.SensorEmergencia)
                {
                    //MessageBox.Show("Emergência pressionado");
                    //MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Emergencia);
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                    {
                        m.ShowDialog(Negocio.IdiomaResxExtensao.Manutencao_Show_Emergencia);
                    }
                }
                else
                {
                    //MessageBox.Show("Error Nativo:" + this.modBusDispenser_P3.IsNativo.ToString());
                    //MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Erro_Nativo + this._prmPurga.DispenserP3.IsNativo.ToString());
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                    {
                        m.ShowDialog(Negocio.IdiomaResxExtensao.Manutencao_Show_Erro_Nativo + this._prmPurga.DispenserP3.IsNativo.ToString());
                    }
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
                this._prmPurga.DispenserP3.ReadSensores_Mover();

                if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorEmergencia)
                {                 
                    if (!this._prmPurga.DispenserP3.SensorGavetaFechada && this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                    {
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        //MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_BT_Fechar_Gaveta);
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.Manutencao_BT_Fechar_Gaveta);
                        }
                    }
                    
                    gbIndividual.Enabled = false;
                    
                }
                else if (this._prmPurga.DispenserP3.SensorEmergencia)
                {
                    //MessageBox.Show("Emergência pressionado");
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                    {
                        m.ShowDialog(Negocio.IdiomaResxExtensao.Manutencao_Show_Emergencia);
                    }
                }
                else
                {
                    //MessageBox.Show("Error Nativo:" + this.modBusDispenser_P3.IsNativo.ToString());
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                    {
                        m.ShowDialog(Negocio.IdiomaResxExtensao.Manutencao_Show_Erro_Nativo + this._prmPurga.DispenserP3.IsNativo.ToString());
                    }
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
                this._prmPurga.DispenserP3.ReadSensores_Mover();
                if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorEmergencia)
                {
                    if (!this._prmPurga.DispenserP3.SensorValvulaDosagem && this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                    {
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.ValvulaPosicaoDosagem());
                        //MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_BT_Valvula_Dosagem);
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.Manutencao_BT_Valvula_Dosagem);
                        }
                    }                    

                }
                else if (this._prmPurga.DispenserP3.SensorEmergencia)
                {
                    //MessageBox.Show("Emergência pressionado");
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                    {
                        m.ShowDialog(Negocio.IdiomaResxExtensao.Manutencao_Show_Emergencia);
                    }
                }
                else
                {
                    //MessageBox.Show("Error Nativo:" + this.modBusDispenser_P3.IsNativo.ToString());
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                    {
                        m.ShowDialog(Negocio.IdiomaResxExtensao.Manutencao_Show_Erro_Nativo + this._prmPurga.DispenserP3.IsNativo.ToString());
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    MessageBox.Show("Error:" + ex.Message);
            }
        }
    }
}