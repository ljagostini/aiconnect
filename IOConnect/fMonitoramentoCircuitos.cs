using Percolore.Core;
using Percolore.Core.Logging;
using Percolore.IOConnect.Util;
using System.ComponentModel;

namespace Percolore.IOConnect
{
	public partial class fMonitoramentoCircuitos : Form
    {
        Util.ObjectParametros _parametros = null;
        MonitoramentoVO _prmMoni;
        int INDEX_CONTADOR;
        int INDEX_ULTIMO_COLORANTE;

        int INDEX_CONTADOR2;
        int INDEX_ULTIMO_COLORANTE2;

        bool desativarUI;
        bool isReverse = true;
        System.ComponentModel.BackgroundWorker backgroundWorker1 = null;
        private bool isThread = false;
        private bool isRunning = false;

        private int i_Step = 0;

        public fMonitoramentoCircuitos()
        {
            InitializeComponent();
        }

        public fMonitoramentoCircuitos(MonitoramentoVO parametrosMonitCirc, bool desativarUI = false)
        {
            InitializeComponent();
            this._prmMoni = parametrosMonitCirc;
            this._parametros = Util.ObjectParametros.Load();

            /* Se a exibição da interface for desabilitada
             * não é necessário configurar propriedades dos controles */
            this.desativarUI = desativarUI;
            
            if (this.desativarUI)
            {
                //this.Visible = false;
                return;
            }
            

            //Redimensiona e posiciona form
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Location = new Point(0, 30);

            //Globalização
            lblStatus.Text = Negocio.IdiomaResxExtensao.PurgarMonit_lblStatus_Msg01;
            lblSubStatus.Text = Negocio.IdiomaResxExtensao.PurgarMonit_lblSubStatus_Msg01;
            btnIniciar.Text = Negocio.IdiomaResxExtensao.Global_Iniciar;
            btnCancelar.Text = Negocio.IdiomaResxExtensao.Global_Cancelar;
            btnAbortar.Text = Negocio.IdiomaResxExtensao.Global_Abortar;

            //Habilitar controles
            lblStatus.Visible = true;
            progressBar.Visible = false;
            btnIniciar.Enabled = true;
            btnCancelar.Enabled = true;
            btnAbortar.Visible = false;
        }       

        private void fMonitoramentoCircuitos_Load(object sender, EventArgs e)
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
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			/* É necessário indicar topMost aqui para que o form seja 
            * redesenhando em primeiro plano sobre qualquer processo em execução */
			TopMost = !desativarUI;

            /* Se a exibição da interface for desabilitada,
             * reduz form a tamanho zero e incia automaticamente a dispensa */
            if (desativarUI)
            {
                this.Width = 0;
                this.Height = 0;
               
            }
            btnIniciar_Click(sender, e);
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            try
            {
                progressBar.Visible = true;
                btnIniciar.Enabled = false;
                btnCancelar.Visible = false;
                btnAbortar.Visible = true;

                //Inicializa variáveis de monitoramento
                INDEX_CONTADOR = 0;
                //INDEX_ULTIMO_COLORANTE = _prmMoni.Colorantes.Count - 1;
                INDEX_ULTIMO_COLORANTE = _prmMoni.LMDispositivos[0].Colorantes.Count -1;

                INDEX_CONTADOR2 = 0;
                INDEX_ULTIMO_COLORANTE2 = 0;
                if (_prmMoni.LMDispositivos.Count > 1)
                {
                    INDEX_ULTIMO_COLORANTE2 = _prmMoni.LMDispositivos[1].Colorantes.Count - 1;
                }
               
                ExecutarMonitoramento();

                Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Global_MonitIniciada);
            }
            catch (Exception ex)
            {
                Falha(ex);
            }
        }

        //void Monitoramento_Event(object sender, EventArgs e)
        private void MonitoramentoEvent()
        {
            try
            {
                //Verifica se dispenser está pronto
                for (int i = 0; i < _prmMoni.Dispenser.Count; i++)
                {
                    if (INDEX_CONTADOR > 0 && !_prmMoni.Dispenser[i].IsReady)
                    {
                        this.isRunning = false;
                        return;
                    }
                }
                if (!_parametros.MonitMovimentoReverso)
                {
                    int[] pulso = new int[16];
                    int[] vel = new int[16];
                    int[] acc = new int[16];
                    int[] delay = new int[16];
                    int[] pulsoRev = new int[16];

                    int[] pulso2 = new int[16];
                    int[] vel2 = new int[16];
                    int[] acc2 = new int[16];
                    int[] delay2 = new int[16];
                    int[] pulsoRev2 = new int[16];

                    if((Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador /*|| (Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_4*/)
                    {
                        pulso = new int[16];
                        vel = new int[16];
                        acc = new int[16];
                        delay = new int[16];
                        pulsoRev = new int[16];
                    }

                    if ((Dispositivo2)_parametros.IdDispositivo2 == Dispositivo2.Simulador /*|| (Dispositivo2)_parametros.IdDispositivo2 == Dispositivo2.Placa_4*/)
                    {
                        pulso2 = new int[16];
                        vel2 = new int[16];
                        acc2 = new int[16];
                        delay2 = new int[16];
                        pulsoRev2 = new int[16];
                    }

                    //Verifica se já executou todos os circuitos de colorantes]
                    if (INDEX_CONTADOR >= INDEX_ULTIMO_COLORANTE && INDEX_CONTADOR2 >= INDEX_ULTIMO_COLORANTE2)
                    {
                        PausarMonitoramento();

                        /*
                        //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                        for (INDEX_CONTADOR = 0; INDEX_CONTADOR < INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                        {
                            int nCIRCUITO = _prmMoni.Colorantes[INDEX_CONTADOR].Circuito;
                            Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmMoni.PulsoReverso[nCIRCUITO - 1]);
                        }
                        */
                        DialogResult = DialogResult.OK;
                        Close();
                        return;
                    }

                    //Recupera posição do circuito do colorante
                    if (INDEX_CONTADOR <_prmMoni.LMDispositivos[0].Colorantes.Count)
                    {
                        int CIRCUITO = _prmMoni.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;

                        //Atualiza interface
                        lblStatus.Text =
                            Negocio.IdiomaResxExtensao.PurgarMonit_lblStatus_Msg02 + " " + _prmMoni.Colorantes[INDEX_CONTADOR].Nome;
                        lblSubStatus.Text =
                            Negocio.IdiomaResxExtensao.PurgarMonit_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                        int qtd_inc = 0;
                        for (int i = INDEX_CONTADOR, j = 0; i < _prmMoni.LMDispositivos[0].Colorantes.Count && j < _parametros.QtdeMonitCircuitoGrupo; i++, j++)
                        //for (int i = INDEX_CONTADOR; i < _prmMoni.Colorantes.Count - 1; i++)
                        {
                            pulso[i] = _prmMoni.LMDispositivos[0].PulsoReverso[INDEX_CONTADOR];
                            vel[i] = _prmMoni.LMDispositivos[0].PulsoHorario[INDEX_CONTADOR];
                            acc[i] = _prmMoni.LMDispositivos[0].Aceleracao[INDEX_CONTADOR];
                            vel[i] = _prmMoni.LMDispositivos[0].Velocidade[INDEX_CONTADOR];
                            delay[i] = _prmMoni.LMDispositivos[0].Delay[INDEX_CONTADOR];
                            pulsoRev[i] = _prmMoni.LMDispositivos[0].PulsoReverso[INDEX_CONTADOR];
                            qtd_inc++;
                        }
                        // _prmMoni.Dispenser.Disconnect();
                        // _prmMoni.Dispenser.Connect();

                        Task task = Task.Factory.StartNew(() => _prmMoni.Dispenser[0].Dispensar(pulso, vel, acc, delay, pulsoRev, i_Step));
                        INDEX_CONTADOR += qtd_inc;
                    }
                    
                    if (_prmMoni.LMDispositivos.Count > 1)
                    {
                        int qtd_inc2 = 0;
                        for (int i = INDEX_CONTADOR2, j = 0; i < _prmMoni.LMDispositivos[1].Colorantes.Count && j < _parametros.QtdeMonitCircuitoGrupo; i++, j++)
                        //for (int i = INDEX_CONTADOR; i < _prmMoni.Colorantes.Count - 1; i++)
                        {
                            pulso2[i] = _prmMoni.LMDispositivos[1].PulsoReverso[INDEX_CONTADOR2];
                            vel2[i] = _prmMoni.LMDispositivos[1].PulsoHorario[INDEX_CONTADOR2];
                            acc2[i] = _prmMoni.LMDispositivos[1].Aceleracao[INDEX_CONTADOR2];
                            vel2[i] = _prmMoni.LMDispositivos[1].Velocidade[INDEX_CONTADOR2];
                            delay2[i] = _prmMoni.LMDispositivos[1].Delay[INDEX_CONTADOR2];
                            pulsoRev2[i] = _prmMoni.LMDispositivos[1].PulsoReverso[INDEX_CONTADOR2];
                            qtd_inc2++;
                        }
                        Task task2 = Task.Factory.StartNew(() => _prmMoni.Dispenser[1].Dispensar(pulso2, vel2, acc2, delay2, pulsoRev2, i_Step));
                        INDEX_CONTADOR2 += qtd_inc2;
                    }



                    //Removido o abater 
                    /*
                    if (_parametros.ControlarNivel)
                    {
                        Operar.AbaterColorante(CIRCUITO, _prmMoni.Volume);
                    }*/

                    //Incrementa contador
                    this.isRunning = false;
                   
                   
                }
                else
                {
                    int[] pulso = new int[16];
                    int[] vel = new int[16];
                    int[] acc = new int[16];
                    int[] delay = new int[16];
                    int[] pulsoRev = new int[16];

                    int[] pulso2 = new int[16];
                    int[] vel2 = new int[16];
                    int[] acc2 = new int[16];
                    int[] delay2 = new int[16];
                    int[] pulsoRev2 = new int[16];

                    if ((Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador /*|| (Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_4*/)
                    {
                        pulso = new int[24];
                        vel = new int[24];
                        acc = new int[24];
                        delay = new int[24];
                        pulsoRev = new int[24];
                    }

                    if ((Dispositivo2)_parametros.IdDispositivo2 == Dispositivo2.Simulador/* || (Dispositivo2)_parametros.IdDispositivo2 == Dispositivo2.Placa_4*/)
                    {
                        pulso2 = new int[16];
                        vel2 = new int[16];
                        acc2 = new int[16];
                        delay2 = new int[16];
                        pulsoRev2 = new int[16];
                    }

                    if (this.isReverse)
                    {
                        if (INDEX_CONTADOR >= INDEX_ULTIMO_COLORANTE && INDEX_CONTADOR2 >= INDEX_ULTIMO_COLORANTE2)
                        {
                            PausarMonitoramento();

                            /*
                            //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR < INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = _prmMoni.Colorantes[INDEX_CONTADOR].Circuito;
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmMoni.PulsoReverso[nCIRCUITO - 1]);
                            }
                            */
                            DialogResult = DialogResult.OK;
                            Close();
                            return;
                        }

                        //Recupera posição do circuito do colorante
                        if (INDEX_CONTADOR < _prmMoni.LMDispositivos[0].Colorantes.Count)
                        {
                            int CIRCUITO = _prmMoni.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                            for (int i = INDEX_CONTADOR, j = 0; i < _prmMoni.LMDispositivos[0].Colorantes.Count && j < _parametros.QtdeMonitCircuitoGrupo; i++, j++)
                            //for (int i = INDEX_CONTADOR; i < _prmMoni.Colorantes.Count - 1; i++)
                            {
                                pulso[i] = 20;
                                vel[i] = _prmMoni.LMDispositivos[0].PulsoHorario[INDEX_CONTADOR];
                                acc[i] = _prmMoni.LMDispositivos[0].Aceleracao[INDEX_CONTADOR];
                                vel[i] = _prmMoni.LMDispositivos[0].Velocidade[INDEX_CONTADOR];
                                delay[i] = _prmMoni.LMDispositivos[0].Delay[INDEX_CONTADOR];
                                pulsoRev[i] = _prmMoni.LMDispositivos[0].PulsoReverso[INDEX_CONTADOR] + 20;

                            }

                            //Atualiza interface
                            lblStatus.Text =
                                Negocio.IdiomaResxExtensao.PurgarMonit_lblStatus_Msg02 + " " + _prmMoni.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Nome;
                            lblSubStatus.Text =
                                Negocio.IdiomaResxExtensao.PurgarMonit_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                            //_prmMoni.Dispenser.Disconnect();
                            //_prmMoni.Dispenser.Connect();
                            //Dispara thread para enviar dados ao dispositivo
                            Task task = Task.Factory.StartNew(() => _prmMoni.Dispenser[0].Dispensar(pulso, vel, acc, delay, pulsoRev, i_Step));
                        }

                        if (_prmMoni.LMDispositivos.Count > 1)
                        {
                            if (INDEX_CONTADOR2 < _prmMoni.LMDispositivos[1].Colorantes.Count)
                            {
                                int CIRCUITO2 = _prmMoni.LMDispositivos[1].Colorantes[INDEX_CONTADOR2].Circuito;
                                for (int i = INDEX_CONTADOR2, j = 0; i < _prmMoni.LMDispositivos[1].Colorantes.Count && j < _parametros.QtdeMonitCircuitoGrupo; i++, j++)
                                //for (int i = INDEX_CONTADOR; i < _prmMoni.Colorantes.Count - 1; i++)
                                {
                                    pulso2[i] = 20;
                                    vel2[i] = _prmMoni.LMDispositivos[1].PulsoHorario[INDEX_CONTADOR2];
                                    acc2[i] = _prmMoni.LMDispositivos[1].Aceleracao[INDEX_CONTADOR2];
                                    vel2[i] = _prmMoni.LMDispositivos[1].Velocidade[INDEX_CONTADOR2];
                                    delay2[i] = _prmMoni.LMDispositivos[1].Delay[INDEX_CONTADOR2];
                                    pulsoRev2[i] = _prmMoni.LMDispositivos[1].PulsoReverso[INDEX_CONTADOR2] + 20;

                                }
                                Task task2 = Task.Factory.StartNew(() => _prmMoni.Dispenser[1].Dispensar(pulso2, vel2, acc2, delay2, pulsoRev2, i_Step));

                                lblStatus.Text += " - " + _prmMoni.LMDispositivos[1].Colorantes[INDEX_CONTADOR2].Nome;
                                lblSubStatus.Text += " - " + CIRCUITO2.ToString("00");
                            }
                        }


                        //Removido o abater 
                        /*
                        if (_parametros.ControlarNivel)
                        {
                            Operar.AbaterColorante(CIRCUITO, _prmMoni.Volume);
                        }
                        */
                        


                        //Incrementa contador
                        //INDEX_CONTADOR++;
                        this.isReverse = false;
                        this.isRunning = false;


                    }
                    else
                    {
                        //Verifica se já executou todos os circuitos de colorantes]
                        if(INDEX_CONTADOR < _prmMoni.LMDispositivos[0].Colorantes.Count)
                        { 
                            int CIRCUITO = _prmMoni.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;

                            //Atualiza interface
                            lblStatus.Text =
                                Negocio.IdiomaResxExtensao.PurgarMonit_lblStatus_Msg02 + " " + _prmMoni.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Nome;
                            lblSubStatus.Text =
                                Negocio.IdiomaResxExtensao.PurgarMonit_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                            int qtd_inc = 0;
                            for (int i = INDEX_CONTADOR, j = 0; i < _prmMoni.LMDispositivos[0].Colorantes.Count && j < _parametros.QtdeMonitCircuitoGrupo; i++, j++)
                            //for (int i = INDEX_CONTADOR; i < _prmMoni.Colorantes.Count - 1 ; i++)
                            {
                                pulso[i] = _prmMoni.LMDispositivos[0].PulsoReverso[INDEX_CONTADOR]+20;
                                vel[i] = _prmMoni.LMDispositivos[0].PulsoHorario[INDEX_CONTADOR];
                                acc[i] = _prmMoni.LMDispositivos[0].Aceleracao[INDEX_CONTADOR];
                                vel[i] = _prmMoni.LMDispositivos[0].Velocidade[INDEX_CONTADOR];
                                delay[i] = _prmMoni.LMDispositivos[0].Delay[INDEX_CONTADOR];
                                pulsoRev[i] = 20;
                                qtd_inc++;

                            }
                            //_prmMoni.Dispenser.Disconnect();
                            //_prmMoni.Dispenser.Connect();
                            Task task = Task.Factory.StartNew(() => _prmMoni.Dispenser[0].Dispensar(pulso, vel, acc, delay, pulsoRev, i_Step));
                            INDEX_CONTADOR += qtd_inc;
                        }
                       
                        if(_prmMoni.LMDispositivos.Count > 1)
                        {
                            int qtd_inc2 = 0;
                            if (INDEX_CONTADOR2 < _prmMoni.LMDispositivos[1].Colorantes.Count)
                            {
                                int CIRCUITO2 = _prmMoni.LMDispositivos[1].Colorantes[INDEX_CONTADOR2].Circuito;
                                for (int i = INDEX_CONTADOR2, j = 0; i < _prmMoni.LMDispositivos[1].Colorantes.Count && j < _parametros.QtdeMonitCircuitoGrupo; i++, j++)
                                //for (int i = INDEX_CONTADOR; i < _prmMoni.Colorantes.Count - 1; i++)
                                {
                                    pulso2[i] = 20;
                                    vel2[i] = _prmMoni.LMDispositivos[1].PulsoHorario[INDEX_CONTADOR2];
                                    acc2[i] = _prmMoni.LMDispositivos[1].Aceleracao[INDEX_CONTADOR2];
                                    vel2[i] = _prmMoni.LMDispositivos[1].Velocidade[INDEX_CONTADOR2];
                                    delay2[i] = _prmMoni.LMDispositivos[1].Delay[INDEX_CONTADOR2];
                                    pulsoRev2[i] = _prmMoni.LMDispositivos[1].PulsoReverso[INDEX_CONTADOR2] + 20;
                                    qtd_inc2++;
                                }
                                Task task2 = Task.Factory.StartNew(() => _prmMoni.Dispenser[1].Dispensar(pulso2, vel2, acc2, delay2, pulsoRev2, i_Step));

                                lblStatus.Text += " - " + _prmMoni.LMDispositivos[1].Colorantes[INDEX_CONTADOR2].Nome;
                                lblSubStatus.Text += " - " + CIRCUITO2.ToString("00");
                            }
                            INDEX_CONTADOR2 += qtd_inc2;
                        }
                        //Removido o abater 
                        /*
                        if (_parametros.ControlarNivel)
                        {
                            Operar.AbaterColorante(CIRCUITO, _prmMoni.Volume);
                        }
                        */
                        this.isReverse = true;
                        this.isRunning = false;
                        
                    }

                }
            }
            catch (Exception ex)
            {
				string customMessage = ErrorMessageHandler.GetFriendlyErrorMessage(ex);
				Falha(ex, customMessage);
            }
        }

        void Abortar_Click(object sender, EventArgs e)
        {
            PausarMonitoramento();

            try
            {
                //Interrompe e retoma operação da placa
                for (int i = 0; i < _prmMoni.Dispenser.Count; i++)
                {
                    _prmMoni.Dispenser[i].Halt();
                    _prmMoni.Dispenser[i].UnHalt();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			//Define status da operação
			DialogResult = DialogResult.Abort;
            Close();
        }

        private void Cancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void Monitoramento_Closed(object sender, FormClosedEventArgs e)
        {
            PausarMonitoramento();

            switch (this.DialogResult)
            {
                #region Log de processo

                case DialogResult.OK:
                    {
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_36 + Negocio.IdiomaResxExtensao.Global_MonitConcluida);
                        Constants.numeroRetentativasBluetooth = 0;
                        //Persiste data e hora de execução da purga
                        //Util.ObjectParametros.SetExecucaoPurga(DateTime.Now);
                        break;
                    }
                case DialogResult.Cancel:
                    {
                        Constants.numeroRetentativasBluetooth = 0;
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_25 + Negocio.IdiomaResxExtensao.Global_MonitCancelada);
                        break;
                    }
                case DialogResult.Abort:
                    {
                        Constants.numeroRetentativasBluetooth = 0;
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_26 + Negocio.IdiomaResxExtensao.Global_MonitAbortada);
                        break;
                    }
                case DialogResult.No:
                    {                        
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_27 + Negocio.IdiomaResxExtensao.Global_FalhaMonit);
                        break;
                    }

                    #endregion
            }
        }

        #region Métodos privados

        void Falha(Exception ex, string customMessage = null)
		{
            LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			PausarMonitoramento();

            if (_parametros.NomeDispositivo != "" && Constants.numeroRetentativasBluetooth < 2)
            {
                Constants.numeroRetentativasBluetooth++;
            }
            else
            {              
               
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
						string.IsNullOrWhiteSpace(customMessage) ? Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + ex.Message
                                                                 : customMessage);
                }
                
            }

            DialogResult = DialogResult.No;
        }

        void PausarMonitoramento()
        {
            try
            {
                this.isThread = false;
                if (backgroundWorker1.IsBusy && backgroundWorker1.CancellationPending == false)
                {
                    backgroundWorker1.CancelAsync();
                }
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

                    while (backgroundWorker1.IsBusy)
                    {
                        this.isThread = false;
                        this.isRunning = true;
                    }
                    backgroundWorker1.RunWorkerAsync();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
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

        #endregion
    }
}