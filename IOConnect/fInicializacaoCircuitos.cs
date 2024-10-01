using Percolore.Core;
using Percolore.Core.Logging;
using Percolore.Core.UserControl;
using System.ComponentModel;

namespace Percolore.IOConnect
{
	public partial class fInicializacaoCircuitos : Form
    {
        Util.ObjectParametros _parametros = null;
        InicializacaoCircuitosVO _paramIni;
        List<int[]> _listaDispensa;
        List<int[]> _listaDispensa2;
        int INDEX_DISPENSA = 0;
        int INDEX_DISPENSA2 = 0;
        bool desativarUI;
        private int counterFalha = 0;

        System.ComponentModel.BackgroundWorker backgroundWorker1 = null;
        private bool isThread = false;
        private bool isRunning = false;

        private int tipoActionP3 = -1;
        private int passosActionP3 = 0;
        private int qtdTrocaRecipiente = 0;

        private int i_Step = 0;

        public fInicializacaoCircuitos(InicializacaoCircuitosVO parametroInicializacaoCircuitos, bool desativarUI = false)
        {
            InitializeComponent();
            _paramIni = parametroInicializacaoCircuitos;
            _parametros = Util.ObjectParametros.Load();

            //Redimensionar e posicionar
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Location = new Point(0, 30);

            /* Se a exibição da interface for desabilitada
             * não é necessário configurar propriedades dos controles */
            this.desativarUI = desativarUI;
            if (this.desativarUI)
            {
                return;
            }

            //Progress bar
            progressBar.DisplayStyle = ProgressBarDisplayStyle.Text;
            progressBar.Text = Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg01;

            //Globalização
            btnIniciar.Text = Negocio.IdiomaResxExtensao.Global_Iniciar;
            btnCancelar.Text = Negocio.IdiomaResxExtensao.Global_Cancelar;
            btnAbortar.Text = Negocio.IdiomaResxExtensao.Global_Abortar;

            //Habiltar controles
            btnIniciar.Enabled = true;
            btnCancelar.Enabled = true;
            btnAbortar.Visible = false;
        }

        #region Eventos

        void FormLoad(object sender, EventArgs e)
        {
            /* É necessário indicar topMost aqui para que o form seja 
             * redesenhando em primeiro plano sobre qualquer processo em execução */
            TopMost = !desativarUI;
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
#if DEBUG
			TopMost = false;
#endif

            double PULSO = _paramIni.PulsoInicial;
            double MULTIPLICADOR_VARIACAO = 0;

            _listaDispensa = new List<int[]>();
            _listaDispensa2 = new List<int[]>();

            do
            {
                //[Posiciona pulso calculado nos circuitos habilitados]
                int[] pulso = new int[16];
                int indexV2 = 16;
                if((Dispositivo) _parametros.IdDispositivo == Dispositivo.Simulador /*|| (Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_4*/)
                {
                    pulso = new int[16];
                    indexV2 = 16;
                }

                int imax1 = 0;
                //foreach (int circuito in _paramIni.Circuitos)
                for (int i = 0; i < _paramIni.Circuitos.Length; i++)
                {
                    if (imax1 < indexV2 && _paramIni.Dispositivo[i] == 1)
                    {
                        imax1++;
                        int circuito = _paramIni.Circuitos[i];
                        pulso[circuito - 1] = (int)PULSO;
                    }
                }
                //[Adiciona pulso na lista]
                _listaDispensa.Add(pulso);                
                
                //Aqui temos que garantir no máximo 16 circuitos para placa 2
                int[] pulso2 = new int[16];
                if ((Dispositivo2)_parametros.IdDispositivo2 == Dispositivo2.Simulador /*|| (Dispositivo2)_parametros.IdDispositivo == Dispositivo2.Placa_4*/)
                {
                    pulso2 = new int[16];
                }
                bool addDisp2 = false;
                int imax2 = 0;
                for (int i = 0; i < _paramIni.Circuitos.Length; i++)
                {
                    if (imax2 < indexV2 && _paramIni.Dispositivo[i] == 2)
                    {
                        addDisp2 = true;
                        int circuito = _paramIni.Circuitos[i];
                        pulso2[(circuito - indexV2) - 1] = (int)PULSO;
                        imax2++;
                    }
                }
                //Adiciona Segundo Pulso
                if (addDisp2)
                {
                    _listaDispensa2.Add(pulso2);
                }
                

                //[Define multiplicador da variação de pulsos]
                MULTIPLICADOR_VARIACAO += _paramIni.StepVariacao;

                //[Calcula variação e adiciona aos pulsos]
                PULSO += _paramIni.VariacaoPulso * MULTIPLICADOR_VARIACAO;

            } while (PULSO < _paramIni.PulsoLimite);

            
            /* Se a exibição da interface for desabilitada,
             * reduz form a tamanho zero e incia automaticamente a dispensa */
            if (desativarUI)
            {
                this.Width = 0;
                this.Height = 0;
                btnIniciar_Click(sender, e);
            }
        }

        void btnIniciar_Click(object sender, EventArgs e)
        {
            btnIniciar.Enabled = false;
            btnCancelar.Visible = false;
            btnAbortar.Visible = true;

            progressBar.Minimum = 0;
            progressBar.Maximum = _listaDispensa.Count;
            progressBar.Value = 0;
            progressBar.Step = 1;
            progressBar.DisplayStyle = ProgressBarDisplayStyle.TextAndPercentage;
            progressBar.Text = Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg02;

            
            this.counterFalha = 0;
            this.tipoActionP3 = -1;
            this.passosActionP3 = 0;
            if (_paramIni.DispenserP3 != null)
            {
                bool isPosicao = false;
                _paramIni.DispenserP3.ReadSensores_Mover();
                if (!(_paramIni.DispenserP3.IsNativo == 0 || _paramIni.DispenserP3.IsNativo == 2))
                {                    
                    return;
                }
                if (!_paramIni.DispenserP3.SensorGavetaAberta && !_paramIni.DispenserP3.SensorGavetaFechada)
                {
                    _paramIni.DispenserP3.AbrirGaveta(true);
                    for (int _i = 0; _i < 20; _i++)
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            _i = 20;
                        }
                        Thread.Sleep(500);
                    }
                    
                    isPosicao = true;
                }
                if (!_paramIni.DispenserP3.SensorValvulaDosagem && !_paramIni.DispenserP3.SensorValvulaRecirculacao)
                {
                    _paramIni.DispenserP3.ValvulaPosicaoRecirculacao();
                    isPosicao = true;
                }
                if (isPosicao)
                {
                    _paramIni.DispenserP3.ReadSensores_Mover();
                    if ((!_paramIni.DispenserP3.SensorGavetaAberta && !_paramIni.DispenserP3.SensorGavetaFechada) || (!_paramIni.DispenserP3.SensorValvulaDosagem && !_paramIni.DispenserP3.SensorValvulaRecirculacao))
                    {
                      return;
                    }
                }
                this.tipoActionP3 = getActionP3();
            }
            else
            {
                this.tipoActionP3 = 0;
            }
            if (this.tipoActionP3 >= 0)
            {  
                this.Invoke(new MethodInvoker(ExecutarMonitoramento));
            }
            //ExecutarMonitoramento();
        }

        private int getActionP3()
        {
            int retorno = -1;
          
            if (this._paramIni.DispenserP3.SensorEsponja && this._paramIni.DispenserP3.SensorCopo && !this._paramIni.DispenserP3.SensorBaixoBicos && this._paramIni.DispenserP3.SensorAltoBicos
               && !this._paramIni.DispenserP3.SensorGavetaAberta && this._paramIni.DispenserP3.SensorGavetaFechada && this._paramIni.DispenserP3.SensorValvulaDosagem
               && !this._paramIni.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 1;
            }
            else if (this._paramIni.DispenserP3.SensorEsponja && this._paramIni.DispenserP3.SensorCopo && !this._paramIni.DispenserP3.SensorBaixoBicos && this._paramIni.DispenserP3.SensorAltoBicos
                && !this._paramIni.DispenserP3.SensorGavetaAberta && this._paramIni.DispenserP3.SensorGavetaFechada && !this._paramIni.DispenserP3.SensorValvulaDosagem
                && this._paramIni.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 2;
            }
            else if (this._paramIni.DispenserP3.SensorEsponja && this._paramIni.DispenserP3.SensorCopo && !this._paramIni.DispenserP3.SensorBaixoBicos && this._paramIni.DispenserP3.SensorAltoBicos
                && this._paramIni.DispenserP3.SensorGavetaAberta && !this._paramIni.DispenserP3.SensorGavetaFechada && this._paramIni.DispenserP3.SensorValvulaDosagem
                && !this._paramIni.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 3;
            }
            else if (this._paramIni.DispenserP3.SensorEsponja && this._paramIni.DispenserP3.SensorCopo && !this._paramIni.DispenserP3.SensorBaixoBicos && this._paramIni.DispenserP3.SensorAltoBicos
                && this._paramIni.DispenserP3.SensorGavetaAberta && !this._paramIni.DispenserP3.SensorGavetaFechada && !this._paramIni.DispenserP3.SensorValvulaDosagem
                && this._paramIni.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 4;
            }
            else if (this._paramIni.DispenserP3.SensorEsponja && !this._paramIni.DispenserP3.SensorCopo && this._paramIni.DispenserP3.SensorBaixoBicos && !this._paramIni.DispenserP3.SensorAltoBicos
                && !this._paramIni.DispenserP3.SensorGavetaAberta && this._paramIni.DispenserP3.SensorGavetaFechada && !this._paramIni.DispenserP3.SensorValvulaDosagem
                && this._paramIni.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 5;
            }
            else if (this._paramIni.DispenserP3.SensorEsponja && !this._paramIni.DispenserP3.SensorCopo && !this._paramIni.DispenserP3.SensorBaixoBicos && this._paramIni.DispenserP3.SensorAltoBicos
                && !this._paramIni.DispenserP3.SensorGavetaAberta && this._paramIni.DispenserP3.SensorGavetaFechada && !this._paramIni.DispenserP3.SensorValvulaDosagem
                && this._paramIni.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 6;
            }
            else if (this._paramIni.DispenserP3.SensorEsponja && !this._paramIni.DispenserP3.SensorCopo && !this._paramIni.DispenserP3.SensorBaixoBicos && this._paramIni.DispenserP3.SensorAltoBicos
                && this._paramIni.DispenserP3.SensorGavetaAberta && !this._paramIni.DispenserP3.SensorGavetaFechada && !this._paramIni.DispenserP3.SensorValvulaDosagem
                && this._paramIni.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 7;
            }
            else if (!this._paramIni.DispenserP3.SensorEsponja && !this._paramIni.DispenserP3.SensorCopo && !this._paramIni.DispenserP3.SensorBaixoBicos && this._paramIni.DispenserP3.SensorAltoBicos
               && !this._paramIni.DispenserP3.SensorGavetaAberta && this._paramIni.DispenserP3.SensorGavetaFechada && this._paramIni.DispenserP3.SensorValvulaDosagem
               && !this._paramIni.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 8;
            }
            else if (!this._paramIni.DispenserP3.SensorEsponja && !this._paramIni.DispenserP3.SensorCopo && !this._paramIni.DispenserP3.SensorBaixoBicos && this._paramIni.DispenserP3.SensorAltoBicos
               && !this._paramIni.DispenserP3.SensorGavetaAberta && this._paramIni.DispenserP3.SensorGavetaFechada && !this._paramIni.DispenserP3.SensorValvulaDosagem
               && this._paramIni.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 9;
            }
            else if (!this._paramIni.DispenserP3.SensorEsponja && !this._paramIni.DispenserP3.SensorCopo && !this._paramIni.DispenserP3.SensorBaixoBicos && this._paramIni.DispenserP3.SensorAltoBicos
               && this._paramIni.DispenserP3.SensorGavetaAberta && !this._paramIni.DispenserP3.SensorGavetaFechada && this._paramIni.DispenserP3.SensorValvulaDosagem
               && !this._paramIni.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 10;
            }
            else if (!this._paramIni.DispenserP3.SensorEsponja && !this._paramIni.DispenserP3.SensorCopo && !this._paramIni.DispenserP3.SensorBaixoBicos && this._paramIni.DispenserP3.SensorAltoBicos
               && this._paramIni.DispenserP3.SensorGavetaAberta && !this._paramIni.DispenserP3.SensorGavetaFechada && !this._paramIni.DispenserP3.SensorValvulaDosagem
               && this._paramIni.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 11;
            }
            else if (this._paramIni.DispenserP3.SensorBaixoBicos && !this._paramIni.DispenserP3.SensorAltoBicos
               && this._paramIni.DispenserP3.SensorGavetaAberta && !this._paramIni.DispenserP3.SensorGavetaFechada && this._paramIni.DispenserP3.SensorValvulaDosagem
               && !this._paramIni.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 12;
            }
            else if (this._paramIni.DispenserP3.SensorBaixoBicos && !this._paramIni.DispenserP3.SensorAltoBicos
              && this._paramIni.DispenserP3.SensorGavetaAberta && !this._paramIni.DispenserP3.SensorGavetaFechada && !this._paramIni.DispenserP3.SensorValvulaDosagem
              && this._paramIni.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 13;
            }
            else if (this._paramIni.DispenserP3.SensorEsponja && !this._paramIni.DispenserP3.SensorCopo && this._paramIni.DispenserP3.SensorBaixoBicos &&
                !this._paramIni.DispenserP3.SensorAltoBicos && !this._paramIni.DispenserP3.SensorGavetaAberta && this._paramIni.DispenserP3.SensorGavetaFechada &&
                this._paramIni.DispenserP3.SensorValvulaDosagem && !this._paramIni.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 14;
            }
            else if (this._paramIni.DispenserP3.SensorEsponja && !this._paramIni.DispenserP3.SensorCopo && !this._paramIni.DispenserP3.SensorBaixoBicos &&
                this._paramIni.DispenserP3.SensorAltoBicos && !this._paramIni.DispenserP3.SensorGavetaAberta && this._paramIni.DispenserP3.SensorGavetaFechada &&
                this._paramIni.DispenserP3.SensorValvulaDosagem && !this._paramIni.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 15;
            }
            else if (this._paramIni.DispenserP3.SensorEsponja && !this._paramIni.DispenserP3.SensorCopo && !this._paramIni.DispenserP3.SensorBaixoBicos &&
                this._paramIni.DispenserP3.SensorAltoBicos && this._paramIni.DispenserP3.SensorGavetaAberta && !this._paramIni.DispenserP3.SensorGavetaFechada &&
                this._paramIni.DispenserP3.SensorValvulaDosagem && !this._paramIni.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 16;
            }

            return retorno;
        }

        // void MonitorarDispensa(object sender, EventArgs e)
        void MonitorarDispensa()
        {
            try
            {
                if (_paramIni.DispenserP3 != null)
                {
                    trataActionP3();
                }
                else
                {
                    for (int i = 0; i < _paramIni.Dispenser.Count; i++)
                    {
                        if (_paramIni.Dispenser[i].IsBusy)
                        {
                            this.isRunning = false;
                            Thread.Sleep(500);
                            return;
                        }
                    }

                    //Verifica se dispositivo está pronto
                    for (int i = 0; i < _paramIni.Dispenser.Count; i++)
                    {
                        if (!_paramIni.Dispenser[i].IsReady)
                        {
                            this.isRunning = false;
                            Thread.Sleep(500);
                            return;
                        }
                    }
                    Thread.Sleep(500);
                    //Progresso
                    if (INDEX_DISPENSA > 0)
                    {
                        progressBar.Increment(1);
                    }

                    if (progressBar.Value == progressBar.Maximum)
                    {
                        progressBar.Text = Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg03;
                        btnAbortar.Enabled = false;
                    }

                    /* Espera 1 segundo para que não ocorra eero no índice de 
                     * dispensa e a interface seja atualizada corretamente.*/
                    System.Threading.Thread.Sleep(2000);

                    //Verifica se já executou todas as dispensas
                    if ((INDEX_DISPENSA >= (_listaDispensa.Count - 1)) && (INDEX_DISPENSA2 >= (_listaDispensa2.Count - 1)))
                    {
                        this.isRunning = false;
                        PausarMonitoramento();
                        DialogResult = DialogResult.OK;
                        Close();

                        return;
                    }

                    if (INDEX_DISPENSA < (_listaDispensa.Count - 1))
                    {
                        //Dispara thread para enviar dados para a placa
                        Task task = Task.Factory.StartNew(() =>
                            _paramIni.Dispenser[0].Dispensar(
                            _listaDispensa[INDEX_DISPENSA],
                            _paramIni.Velocidade,
                            _paramIni.Aceleracao,
                            _paramIni.MovimentoInicialReverso,
                            _paramIni.QtdeCircuitoGrupo, 
                            i_Step));
                    }

                    if (_paramIni.Dispenser.Count > 1 && INDEX_DISPENSA2 < (_listaDispensa2.Count - 1))
                    {
                        //Dispara thread para enviar dados para a placa
                        Task task = Task.Factory.StartNew(() =>
                            _paramIni.Dispenser[1].Dispensar(
                            _listaDispensa2[INDEX_DISPENSA2],
                            _paramIni.Velocidade,
                            _paramIni.Aceleracao,
                            _paramIni.MovimentoInicialReverso,
                            _paramIni.QtdeCircuitoGrupo,
                            i_Step));
                    }

                    
                    this.counterFalha = 0;
                    //Incrementa contador
                    INDEX_DISPENSA++;
                    INDEX_DISPENSA2++;
                }
                this.isRunning = false;
                
            }
            catch (Exception ex)
            {
                if (this.counterFalha > 0)
                {
                    Falha(ex);
                }
                this.isRunning = false;
                Thread.Sleep(1000);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            PausarMonitoramento();

            //Define status da operação
            DialogResult = DialogResult.Cancel;
            Close();
        }

        void Abortar_Click(object sender, EventArgs e)
        {
            PausarMonitoramento();

            try
            {
                //Interrompe e retoma operação da placa
                for (int i = 0; i < _paramIni.Dispenser.Count; i++)
                {
                    _paramIni.Dispenser[i].Halt();
                    _paramIni.Dispenser[i].UnHalt();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			DialogResult = DialogResult.Abort;
            Close();
        }

        private void ProcessoDispensaSimultanea_FormClosed(object sender, FormClosedEventArgs e)
        {
            _paramIni = null;
            PausarMonitoramento();
        }

        #endregion      

        #region Métodos privados

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

        void Emergencia()
        {
            PausarMonitoramento();
            try
            {
                this._paramIni.DispenserP3.Halt();
                this._paramIni.DispenserP3.UnHalt();
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

            DialogResult = DialogResult.No;
        }

        void DispensaForceHalt()
        {
            PausarMonitoramento();
            try
            {
                this._paramIni.DispenserP3.Halt();
                this._paramIni.DispenserP3.UnHalt();
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
                            this.Invoke(new MethodInvoker(MonitorarDispensa));
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

        #region trata P3
      
        void trataActionP3()
        {
            try
            {
                switch (this.tipoActionP3)
                {
                    case 1:
                        {
                            trataPassosAction_01();
                            break;
                        }
                    case 2:
                        {
                            trataPassosAction_02();
                            break;
                        }
                    case 3:
                        {
                            trataPassosAction_03();
                            break;
                        }
                    case 4:
                        {
                            trataPassosAction_04();
                            break;
                        }
                    case 5:
                        {
                            trataPassosAction_05();
                            break;
                        }
                    case 6:
                        {
                            trataPassosAction_06();
                            break;
                        }
                    case 7:
                        {
                            trataPassosAction_07();
                            break;
                        }
                    case 8:
                        {
                            trataPassosAction_08();
                            break;
                        }
                    case 9:
                        {
                            trataPassosAction_09();
                            break;
                        }
                    case 10:
                        {
                            trataPassosAction_10();
                            break;
                        }
                    case 11:
                        {
                            trataPassosAction_11();
                            break;
                        }
                    case 12:
                        {
                            trataPassosAction_12();
                            break;
                        }
                    case 13:
                        {
                            trataPassosAction_13();
                            break;
                        }
                    case 14:
                        {
                            trataPassosAction_14();
                            break;
                        }
                    case 15:
                        {
                            trataPassosAction_15();
                            break;
                        }
                    case 16:
                        {
                            trataPassosAction_16();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        void trataPassosAction_01()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //bool isRec = m.ShowDialog("Recipiente está na Gaveta", "Yes", "No");
                            bool isRec = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Recipiente_Dosadora, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                            if (!isRec)
                            {
                                this.passosActionP3 = 1;
                            }
                            else
                            {
                                //Dosar
                                this.passosActionP3 = 6;
                            }
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        //this._paramIni.DispenserP3.AbrirGaveta(true);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaAberta && this._paramIni.DispenserP3.SensorAltoBicos)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaAberta || !this._paramIni.DispenserP3.SensorAltoBicos))
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (!this._paramIni.DispenserP3.SensorCopo)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }                        
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 2;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);                        
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        if (INDEX_DISPENSA > 0)
                        {
                            progressBar.Increment(1);
                        }

                        if (progressBar.Value == progressBar.Maximum)
                        {
                            progressBar.Text = Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg03;
                            btnAbortar.Enabled = false;
                        }

                        /* Espera 1 segundo para que não ocorra eero no índice de 
                         * dispensa e a interface seja atualizada corretamente.*/
                        System.Threading.Thread.Sleep(2000);

                        if (INDEX_DISPENSA < (_listaDispensa.Count - 1))
                        {
                            //Dispara thread para enviar dados para a placa
                            Task task = Task.Factory.StartNew(() =>
                                _paramIni.DispenserP3.Dispensar(
                                _listaDispensa[INDEX_DISPENSA],
                                _paramIni.Velocidade,
                                _paramIni.Aceleracao,
                                _paramIni.MovimentoInicialReverso,
                                _paramIni.QtdeCircuitoGrupo,
                            i_Step));
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_paramIni.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_paramIni.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_paramIni.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        this.counterFalha = 0;
                        //Incrementa contador
                        INDEX_DISPENSA++;
                        if (INDEX_DISPENSA >= (_listaDispensa.Count - 1))
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        else
                        {
                            this.passosActionP3 = 6;
                        }
                        
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {                                                
                        this.isRunning = false;
                        PausarMonitoramento();
                        DialogResult = DialogResult.OK;
                        Close();
                        break;
                    }
            }
        }

        void trataPassosAction_02()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //bool isRec = m.ShowDialog("Recipiente está na Gaveta", "Yes", "No");
                            bool isRec = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Recipiente_Dosadora, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                            if (!isRec)
                            {
                                this.passosActionP3 = 1;
                            }
                            else
                            {
                                //Movimentar Válvula Recuar Bicos
                                this.passosActionP3 = 18;
                            }
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        //this._paramIni.DispenserP3.AbrirGaveta(true);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                !this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (!this._paramIni.DispenserP3.SensorCopo)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }                        
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 2;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);                    
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;

                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        if (INDEX_DISPENSA > 0)
                        {
                            progressBar.Increment(1);
                        }

                        if (progressBar.Value == progressBar.Maximum)
                        {
                            progressBar.Text = Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg03;
                            btnAbortar.Enabled = false;
                        }

                        /* Espera 1 segundo para que não ocorra eero no índice de 
                         * dispensa e a interface seja atualizada corretamente.*/
                        System.Threading.Thread.Sleep(2000);

                        if (INDEX_DISPENSA < (_listaDispensa.Count - 1))
                        {
                            //Dispara thread para enviar dados para a placa
                            Task task = Task.Factory.StartNew(() =>
                                _paramIni.DispenserP3.Dispensar(
                                _listaDispensa[INDEX_DISPENSA],
                                _paramIni.Velocidade,
                                _paramIni.Aceleracao,
                                _paramIni.MovimentoInicialReverso,
                                _paramIni.QtdeCircuitoGrupo,
                            i_Step));
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_paramIni.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_paramIni.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_paramIni.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        this.counterFalha = 0;
                        //Incrementa contador
                        INDEX_DISPENSA++;
                        if (INDEX_DISPENSA >= (_listaDispensa.Count - 1))
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        else
                        {
                            this.passosActionP3 = 6;
                        }

                        break;
                    }                
                case 8:
                    {
                        this.isRunning = false;
                        PausarMonitoramento();
                        DialogResult = DialogResult.OK;
                        Close();
                        break;
                    }                
                case 18:
                    {
                        this._paramIni.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._paramIni.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            //Dosar
                            this.passosActionP3 = 6;
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && !this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
            }
        }

        void trataPassosAction_03()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //bool isRec = m.ShowDialog("Recipiente está na Gaveta", "Yes", "No");
                            bool isRec = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Recipiente_Dosadora, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                            if (!isRec)
                            {
                                this.passosActionP3 = 1;
                            }
                            else
                            {
                                this._paramIni.DispenserP3.ReadSensores_Mover();
                                if (this._paramIni.DispenserP3.SensorCopo && this._paramIni.DispenserP3.SensorEsponja)
                                {
                                    //Movimentar fechar gaveta
                                    this.passosActionP3 = 20;
                                }
                            }
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        //this._paramIni.DispenserP3.AbrirGaveta(true);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                !this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (!this._paramIni.DispenserP3.SensorCopo)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 2;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));                        
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        if (INDEX_DISPENSA > 0)
                        {
                            progressBar.Increment(1);
                        }

                        if (progressBar.Value == progressBar.Maximum)
                        {
                            progressBar.Text = Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg03;
                            btnAbortar.Enabled = false;
                        }

                        /* Espera 1 segundo para que não ocorra eero no índice de 
                         * dispensa e a interface seja atualizada corretamente.*/
                        System.Threading.Thread.Sleep(2000);

                        if (INDEX_DISPENSA < (_listaDispensa.Count - 1))
                        {
                            //Dispara thread para enviar dados para a placa
                            Task task = Task.Factory.StartNew(() =>
                                _paramIni.DispenserP3.Dispensar(
                                _listaDispensa[INDEX_DISPENSA],
                                _paramIni.Velocidade,
                                _paramIni.Aceleracao,
                                _paramIni.MovimentoInicialReverso,
                                _paramIni.QtdeCircuitoGrupo,
                            i_Step));
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_paramIni.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_paramIni.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_paramIni.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        this.counterFalha = 0;
                        //Incrementa contador
                        INDEX_DISPENSA++;
                        if (INDEX_DISPENSA >= (_listaDispensa.Count - 1))
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        else
                        {
                            this.passosActionP3 = 6;
                        }

                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.isRunning = false;                        
                        PausarMonitoramento();
                        DialogResult = DialogResult.OK;
                        Close();
                        break;
                    }
                case 18:
                    {
                        this._paramIni.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._paramIni.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            //Dosar
                            this.passosActionP3 = 6;
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && !this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }

        }

        void trataPassosAction_04()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //bool isRec = m.ShowDialog("Recipiente está na Gaveta", "Yes", "No");
                            bool isRec = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Recipiente_Dosadora, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                            if (!isRec)
                            {
                                this.passosActionP3 = 1;
                            }
                            else
                            {
                                this._paramIni.DispenserP3.ReadSensores_Mover();
                                if (this._paramIni.DispenserP3.SensorCopo && this._paramIni.DispenserP3.SensorEsponja)
                                {
                                    //Movimentar fechar gaveta
                                    this.passosActionP3 = 20;
                                }
                            }
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        //this._paramIni.DispenserP3.AbrirGaveta(true);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                !this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (!this._paramIni.DispenserP3.SensorCopo)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 2;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;

                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        if (INDEX_DISPENSA > 0)
                        {
                            progressBar.Increment(1);
                        }

                        if (progressBar.Value == progressBar.Maximum)
                        {
                            progressBar.Text = Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg03;
                            btnAbortar.Enabled = false;
                        }

                        /* Espera 1 segundo para que não ocorra eero no índice de 
                         * dispensa e a interface seja atualizada corretamente.*/
                        System.Threading.Thread.Sleep(2000);

                        if (INDEX_DISPENSA < (_listaDispensa.Count - 1))
                        {
                            //Dispara thread para enviar dados para a placa
                            Task task = Task.Factory.StartNew(() =>
                                _paramIni.DispenserP3.Dispensar(
                                _listaDispensa[INDEX_DISPENSA],
                                _paramIni.Velocidade,
                                _paramIni.Aceleracao,
                                _paramIni.MovimentoInicialReverso,
                                _paramIni.QtdeCircuitoGrupo,
                            i_Step));
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_paramIni.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_paramIni.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_paramIni.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        this.counterFalha = 0;
                        //Incrementa contador
                        INDEX_DISPENSA++;
                        if (INDEX_DISPENSA >= (_listaDispensa.Count - 1))
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        else
                        {
                            this.passosActionP3 = 6;
                        }

                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.isRunning = false;
                        PausarMonitoramento();
                        DialogResult = DialogResult.OK;
                        Close();
                        break;
                    }                
                case 18:
                    {
                        this._paramIni.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._paramIni.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            //Dosar
                            this.passosActionP3 = 6;
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && !this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }
        }

        void trataPassosAction_05()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        //this._paramIni.DispenserP3.AbrirGaveta(true);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                !this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (!this._paramIni.DispenserP3.SensorCopo)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 2;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        if (INDEX_DISPENSA > 0)
                        {
                            progressBar.Increment(1);
                        }

                        if (progressBar.Value == progressBar.Maximum)
                        {
                            progressBar.Text = Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg03;
                            btnAbortar.Enabled = false;
                        }

                        /* Espera 1 segundo para que não ocorra eero no índice de 
                         * dispensa e a interface seja atualizada corretamente.*/
                        System.Threading.Thread.Sleep(2000);

                        if (INDEX_DISPENSA < (_listaDispensa.Count - 1))
                        {
                            //Dispara thread para enviar dados para a placa
                            Task task = Task.Factory.StartNew(() =>
                                _paramIni.DispenserP3.Dispensar(
                                _listaDispensa[INDEX_DISPENSA],
                                _paramIni.Velocidade,
                                _paramIni.Aceleracao,
                                _paramIni.MovimentoInicialReverso,
                                _paramIni.QtdeCircuitoGrupo,
                            i_Step));
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_paramIni.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_paramIni.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_paramIni.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        this.counterFalha = 0;
                        //Incrementa contador
                        INDEX_DISPENSA++;
                        if (INDEX_DISPENSA >= (_listaDispensa.Count - 1))
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        else
                        {
                            this.passosActionP3 = 6;
                        }

                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.isRunning = false;
                        PausarMonitoramento();
                        DialogResult = DialogResult.OK;
                        Close();
                        break;
                    }                
                case 18:
                    {
                        this._paramIni.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._paramIni.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && !this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);                       
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }

        }

        void trataPassosAction_06()
        {

            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        //this._paramIni.DispenserP3.AbrirGaveta(true);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                !this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (!this._paramIni.DispenserP3.SensorCopo)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 2;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        if (INDEX_DISPENSA > 0)
                        {
                            progressBar.Increment(1);
                        }

                        if (progressBar.Value == progressBar.Maximum)
                        {
                            progressBar.Text = Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg03;
                            btnAbortar.Enabled = false;
                        }

                        /* Espera 1 segundo para que não ocorra eero no índice de 
                         * dispensa e a interface seja atualizada corretamente.*/
                        System.Threading.Thread.Sleep(2000);

                        if (INDEX_DISPENSA < (_listaDispensa.Count - 1))
                        {
                            //Dispara thread para enviar dados para a placa
                            Task task = Task.Factory.StartNew(() =>
                                _paramIni.DispenserP3.Dispensar(
                                _listaDispensa[INDEX_DISPENSA],
                                _paramIni.Velocidade,
                                _paramIni.Aceleracao,
                                _paramIni.MovimentoInicialReverso,
                                _paramIni.QtdeCircuitoGrupo,
                            i_Step));
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_paramIni.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_paramIni.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_paramIni.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        this.counterFalha = 0;
                        //Incrementa contador
                        INDEX_DISPENSA++;
                        if (INDEX_DISPENSA >= (_listaDispensa.Count - 1))
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        else
                        {
                            this.passosActionP3 = 6;
                        }

                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.isRunning = false;
                        PausarMonitoramento();
                        DialogResult = DialogResult.OK;
                        Close();
                        break;
                    }                
                case 18:
                    {
                        this._paramIni.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._paramIni.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && !this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }

        }

        void trataPassosAction_07()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        //this._paramIni.DispenserP3.AbrirGaveta(true);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                !this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (!this._paramIni.DispenserP3.SensorCopo)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 2;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        if (INDEX_DISPENSA > 0)
                        {
                            progressBar.Increment(1);
                        }

                        if (progressBar.Value == progressBar.Maximum)
                        {
                            progressBar.Text = Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg03;
                            btnAbortar.Enabled = false;
                        }

                        /* Espera 1 segundo para que não ocorra eero no índice de 
                         * dispensa e a interface seja atualizada corretamente.*/
                        System.Threading.Thread.Sleep(2000);

                        if (INDEX_DISPENSA < (_listaDispensa.Count - 1))
                        {
                            //Dispara thread para enviar dados para a placa
                            Task task = Task.Factory.StartNew(() =>
                                _paramIni.DispenserP3.Dispensar(
                                _listaDispensa[INDEX_DISPENSA],
                                _paramIni.Velocidade,
                                _paramIni.Aceleracao,
                                _paramIni.MovimentoInicialReverso,
                                _paramIni.QtdeCircuitoGrupo,
                            i_Step));
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_paramIni.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_paramIni.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_paramIni.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        this.counterFalha = 0;
                        //Incrementa contador
                        INDEX_DISPENSA++;
                        if (INDEX_DISPENSA >= (_listaDispensa.Count - 1))
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        else
                        {
                            this.passosActionP3 = 6;
                        }

                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.isRunning = false;
                        PausarMonitoramento();
                        DialogResult = DialogResult.OK;
                        Close();                        
                        break;
                    }                
                case 18:
                    {
                        this._paramIni.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._paramIni.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 1;
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && !this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }
        }

        void trataPassosAction_08()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        this.passosActionP3 = 1;
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        //this._paramIni.DispenserP3.AbrirGaveta(true);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                !this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (!this._paramIni.DispenserP3.SensorCopo)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 2;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        if (INDEX_DISPENSA > 0)
                        {
                            progressBar.Increment(1);
                        }

                        if (progressBar.Value == progressBar.Maximum)
                        {
                            progressBar.Text = Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg03;
                            btnAbortar.Enabled = false;
                        }

                        /* Espera 1 segundo para que não ocorra eero no índice de 
                         * dispensa e a interface seja atualizada corretamente.*/
                        System.Threading.Thread.Sleep(2000);

                        if (INDEX_DISPENSA < (_listaDispensa.Count - 1))
                        {
                            //Dispara thread para enviar dados para a placa
                            Task task = Task.Factory.StartNew(() =>
                                _paramIni.DispenserP3.Dispensar(
                                _listaDispensa[INDEX_DISPENSA],
                                _paramIni.Velocidade,
                                _paramIni.Aceleracao,
                                _paramIni.MovimentoInicialReverso,
                                _paramIni.QtdeCircuitoGrupo,
                            i_Step));
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_paramIni.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_paramIni.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_paramIni.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        this.counterFalha = 0;
                        //Incrementa contador
                        INDEX_DISPENSA++;
                        if (INDEX_DISPENSA >= (_listaDispensa.Count - 1))
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        else
                        {
                            this.passosActionP3 = 6;
                        }

                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.isRunning = false;
                        PausarMonitoramento();
                        DialogResult = DialogResult.OK;
                        Close();
                        break;
                    }                
                case 18:
                    {
                        this._paramIni.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._paramIni.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && !this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }

        }

        void trataPassosAction_09()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        //this._paramIni.DispenserP3.AbrirGaveta(true);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                !this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (!this._paramIni.DispenserP3.SensorCopo)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 2;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        if (INDEX_DISPENSA > 0)
                        {
                            progressBar.Increment(1);
                        }

                        if (progressBar.Value == progressBar.Maximum)
                        {
                            progressBar.Text = Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg03;
                            btnAbortar.Enabled = false;
                        }

                        /* Espera 1 segundo para que não ocorra eero no índice de 
                         * dispensa e a interface seja atualizada corretamente.*/
                        System.Threading.Thread.Sleep(2000);

                        if (INDEX_DISPENSA < (_listaDispensa.Count - 1))
                        {
                            //Dispara thread para enviar dados para a placa
                            Task task = Task.Factory.StartNew(() =>
                                _paramIni.DispenserP3.Dispensar(
                                _listaDispensa[INDEX_DISPENSA],
                                _paramIni.Velocidade,
                                _paramIni.Aceleracao,
                                _paramIni.MovimentoInicialReverso,
                                _paramIni.QtdeCircuitoGrupo,
                            i_Step));
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_paramIni.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_paramIni.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_paramIni.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        this.counterFalha = 0;
                        //Incrementa contador
                        INDEX_DISPENSA++;
                        if (INDEX_DISPENSA >= (_listaDispensa.Count - 1))
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        else
                        {
                            this.passosActionP3 = 6;
                        }

                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.isRunning = false;
                        PausarMonitoramento();
                        DialogResult = DialogResult.OK;
                        Close();
                        break;
                    }                
                case 18:
                    {
                        this._paramIni.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._paramIni.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && !this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }

        }

        void trataPassosAction_10()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        this.passosActionP3 = 1;
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        //this._paramIni.DispenserP3.AbrirGaveta(true);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                !this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (!this._paramIni.DispenserP3.SensorCopo)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 2;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        if (INDEX_DISPENSA > 0)
                        {
                            progressBar.Increment(1);
                        }

                        if (progressBar.Value == progressBar.Maximum)
                        {
                            progressBar.Text = Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg03;
                            btnAbortar.Enabled = false;
                        }

                        /* Espera 1 segundo para que não ocorra eero no índice de 
                         * dispensa e a interface seja atualizada corretamente.*/
                        System.Threading.Thread.Sleep(2000);

                        if (INDEX_DISPENSA < (_listaDispensa.Count - 1))
                        {
                            //Dispara thread para enviar dados para a placa
                            Task task = Task.Factory.StartNew(() =>
                                _paramIni.DispenserP3.Dispensar(
                                _listaDispensa[INDEX_DISPENSA],
                                _paramIni.Velocidade,
                                _paramIni.Aceleracao,
                                _paramIni.MovimentoInicialReverso,
                                _paramIni.QtdeCircuitoGrupo,
                            i_Step));
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_paramIni.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_paramIni.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_paramIni.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        this.counterFalha = 0;
                        //Incrementa contador
                        INDEX_DISPENSA++;
                        if (INDEX_DISPENSA >= (_listaDispensa.Count - 1))
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        else
                        {
                            this.passosActionP3 = 6;
                        }

                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.isRunning = false;
                        PausarMonitoramento();
                        DialogResult = DialogResult.OK;
                        Close();
                        break;
                    }                
                case 18:
                    {
                        this._paramIni.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._paramIni.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && !this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }
        }

        void trataPassosAction_11()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._paramIni.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && this._paramIni.DispenserP3.SensorGavetaAberta)
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                //m.ShowDialog("Trocar recipiente!");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                this.passosActionP3 = 3;
                            }
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && !this._paramIni.DispenserP3.SensorGavetaAberta)
                        {
                            this.passosActionP3 = 1;
                        }

                        break;
                    }
                case 3:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (!this._paramIni.DispenserP3.SensorCopo)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 2;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        if (INDEX_DISPENSA > 0)
                        {
                            progressBar.Increment(1);
                        }

                        if (progressBar.Value == progressBar.Maximum)
                        {
                            progressBar.Text = Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg03;
                            btnAbortar.Enabled = false;
                        }

                        /* Espera 1 segundo para que não ocorra eero no índice de 
                         * dispensa e a interface seja atualizada corretamente.*/
                        System.Threading.Thread.Sleep(2000);

                        if (INDEX_DISPENSA < (_listaDispensa.Count - 1))
                        {
                            //Dispara thread para enviar dados para a placa
                            Task task = Task.Factory.StartNew(() =>
                                _paramIni.DispenserP3.Dispensar(
                                _listaDispensa[INDEX_DISPENSA],
                                _paramIni.Velocidade,
                                _paramIni.Aceleracao,
                                _paramIni.MovimentoInicialReverso,
                                _paramIni.QtdeCircuitoGrupo,
                            i_Step));
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_paramIni.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_paramIni.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_paramIni.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        this.counterFalha = 0;
                        //Incrementa contador
                        INDEX_DISPENSA++;
                        if (INDEX_DISPENSA >= (_listaDispensa.Count - 1))
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        else
                        {
                            this.passosActionP3 = 6;
                        }

                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.isRunning = false;
                        PausarMonitoramento();
                        DialogResult = DialogResult.OK;
                        Close();
                        break;
                    }                
                case 18:
                    {
                        this._paramIni.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._paramIni.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 2;
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && !this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }
        }

        void trataPassosAction_12()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 2;
                        this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //bool isAct = m.ShowDialog("Posicionar recipiente nos bicos!");
                            bool isAct = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Posicionar_Recipiente_Bico);
                            if (isAct)
                            {
                                this.passosActionP3 = 6;
                            }
                            else
                            {
                                this.passosActionP3 = 12;
                            }

                        }

                        break;
                    }
                case 3:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (!this._paramIni.DispenserP3.SensorCopo)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 2;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        if (INDEX_DISPENSA > 0)
                        {
                            progressBar.Increment(1);
                        }

                        if (progressBar.Value == progressBar.Maximum)
                        {
                            progressBar.Text = Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg03;
                            btnAbortar.Enabled = false;
                        }

                        /* Espera 1 segundo para que não ocorra eero no índice de 
                         * dispensa e a interface seja atualizada corretamente.*/
                        System.Threading.Thread.Sleep(2000);

                        if (INDEX_DISPENSA < (_listaDispensa.Count - 1))
                        {
                            //Dispara thread para enviar dados para a placa
                            Task task = Task.Factory.StartNew(() =>
                                _paramIni.DispenserP3.Dispensar(
                                _listaDispensa[INDEX_DISPENSA],
                                _paramIni.Velocidade,
                                _paramIni.Aceleracao,
                                _paramIni.MovimentoInicialReverso,
                                _paramIni.QtdeCircuitoGrupo,
                            i_Step));
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_paramIni.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_paramIni.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_paramIni.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        this.counterFalha = 0;
                        //Incrementa contador
                        INDEX_DISPENSA++;
                        if (INDEX_DISPENSA >= (_listaDispensa.Count - 1))
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        else
                        {
                            this.passosActionP3 = 6;
                        }

                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.isRunning = false;
                        PausarMonitoramento();
                        DialogResult = DialogResult.OK;
                        Close();
                        break;
                    }                
                case 18:
                    {
                        this._paramIni.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._paramIni.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 3;
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && !this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }
        }

        void trataPassosAction_13()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 2;
                        this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //bool isAct = m.ShowDialog("Posicionar recipiente nos bicos!");
                            bool isAct = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Posicionar_Recipiente_Bico);
                            if (isAct)
                            {
                                this.passosActionP3 = 18;
                            }
                            else
                            {
                                this.passosActionP3 = 12;
                            }

                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 3:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (!this._paramIni.DispenserP3.SensorCopo)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 2;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        if (INDEX_DISPENSA > 0)
                        {
                            progressBar.Increment(1);
                        }

                        if (progressBar.Value == progressBar.Maximum)
                        {
                            progressBar.Text = Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg03;
                            btnAbortar.Enabled = false;
                        }

                        /* Espera 1 segundo para que não ocorra eero no índice de 
                         * dispensa e a interface seja atualizada corretamente.*/
                        System.Threading.Thread.Sleep(2000);

                        if (INDEX_DISPENSA < (_listaDispensa.Count - 1))
                        {
                            //Dispara thread para enviar dados para a placa
                            Task task = Task.Factory.StartNew(() =>
                                _paramIni.DispenserP3.Dispensar(
                                _listaDispensa[INDEX_DISPENSA],
                                _paramIni.Velocidade,
                                _paramIni.Aceleracao,
                                _paramIni.MovimentoInicialReverso,
                                _paramIni.QtdeCircuitoGrupo,
                            i_Step));
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_paramIni.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_paramIni.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_paramIni.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        this.counterFalha = 0;
                        //Incrementa contador
                        INDEX_DISPENSA++;
                        if (INDEX_DISPENSA >= (_listaDispensa.Count - 1))
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        else
                        {
                            this.passosActionP3 = 6;
                        }

                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.isRunning = false;
                        PausarMonitoramento();
                        DialogResult = DialogResult.OK;
                        Close();
                        break;
                    }                
                case 18:
                    {
                        this._paramIni.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._paramIni.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 6;
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && !this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }
        }

        void trataPassosAction_14()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        this.passosActionP3 = 1;
                        this.counterFalha = 0;

                        break;
                    }
                case 1:
                    {
                        //this._paramIni.DispenserP3.AbrirGaveta(true);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                !this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (!this._paramIni.DispenserP3.SensorCopo)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 2;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        if (INDEX_DISPENSA > 0)
                        {
                            progressBar.Increment(1);
                        }

                        if (progressBar.Value == progressBar.Maximum)
                        {
                            progressBar.Text = Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg03;
                            btnAbortar.Enabled = false;
                        }

                        /* Espera 1 segundo para que não ocorra eero no índice de 
                         * dispensa e a interface seja atualizada corretamente.*/
                        System.Threading.Thread.Sleep(2000);

                        if (INDEX_DISPENSA < (_listaDispensa.Count - 1))
                        {
                            //Dispara thread para enviar dados para a placa
                            Task task = Task.Factory.StartNew(() =>
                                _paramIni.DispenserP3.Dispensar(
                                _listaDispensa[INDEX_DISPENSA],
                                _paramIni.Velocidade,
                                _paramIni.Aceleracao,
                                _paramIni.MovimentoInicialReverso,
                                _paramIni.QtdeCircuitoGrupo,
                            i_Step));
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_paramIni.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_paramIni.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_paramIni.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        this.counterFalha = 0;
                        //Incrementa contador
                        INDEX_DISPENSA++;
                        if (INDEX_DISPENSA >= (_listaDispensa.Count - 1))
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        else
                        {
                            this.passosActionP3 = 6;
                        }

                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.isRunning = false;
                        PausarMonitoramento();
                        DialogResult = DialogResult.OK;
                        Close();
                        break;
                    }                
                case 18:
                    {
                        this._paramIni.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._paramIni.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && !this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }
        }

        void trataPassosAction_15()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        this.passosActionP3 = 1;
                        this.counterFalha = 0;

                        break;
                    }
                case 1:
                    {
                        //this._paramIni.DispenserP3.AbrirGaveta(true);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                !this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (!this._paramIni.DispenserP3.SensorCopo)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 2;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        if (INDEX_DISPENSA > 0)
                        {
                            progressBar.Increment(1);
                        }

                        if (progressBar.Value == progressBar.Maximum)
                        {
                            progressBar.Text = Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg03;
                            btnAbortar.Enabled = false;
                        }

                        /* Espera 1 segundo para que não ocorra eero no índice de 
                         * dispensa e a interface seja atualizada corretamente.*/
                        System.Threading.Thread.Sleep(2000);

                        if (INDEX_DISPENSA < (_listaDispensa.Count - 1))
                        {
                            //Dispara thread para enviar dados para a placa
                            Task task = Task.Factory.StartNew(() =>
                                _paramIni.DispenserP3.Dispensar(
                                _listaDispensa[INDEX_DISPENSA],
                                _paramIni.Velocidade,
                                _paramIni.Aceleracao,
                                _paramIni.MovimentoInicialReverso,
                                _paramIni.QtdeCircuitoGrupo,
                            i_Step));
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_paramIni.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_paramIni.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_paramIni.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        this.counterFalha = 0;
                        //Incrementa contador
                        INDEX_DISPENSA++;
                        if (INDEX_DISPENSA >= (_listaDispensa.Count - 1))
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        else
                        {
                            this.passosActionP3 = 6;
                        }

                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.isRunning = false;
                        PausarMonitoramento();
                        DialogResult = DialogResult.OK;
                        Close();
                        break;
                    }
                
                case 18:
                    {
                        this._paramIni.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._paramIni.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && !this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }
        }

        void trataPassosAction_16()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.passosActionP3 = 1;
                        }
                        this.counterFalha = 0;

                        break;
                    }
                case 1:
                    {
                        //this._paramIni.DispenserP3.AbrirGaveta(true);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                !this._paramIni.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (!this._paramIni.DispenserP3.SensorCopo)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 2;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        if (INDEX_DISPENSA > 0)
                        {
                            progressBar.Increment(1);
                        }

                        if (progressBar.Value == progressBar.Maximum)
                        {
                            progressBar.Text = Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg03;
                            btnAbortar.Enabled = false;
                        }

                        /* Espera 1 segundo para que não ocorra eero no índice de 
                         * dispensa e a interface seja atualizada corretamente.*/
                        System.Threading.Thread.Sleep(2000);

                        if (INDEX_DISPENSA < (_listaDispensa.Count - 1))
                        {
                            //Dispara thread para enviar dados para a placa
                            Task task = Task.Factory.StartNew(() =>
                                _paramIni.DispenserP3.Dispensar(
                                _listaDispensa[INDEX_DISPENSA],
                                _paramIni.Velocidade,
                                _paramIni.Aceleracao,
                                _paramIni.MovimentoInicialReverso,
                                _paramIni.QtdeCircuitoGrupo,
                            i_Step));
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_paramIni.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_paramIni.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_paramIni.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        this.counterFalha = 0;
                        //Incrementa contador
                        INDEX_DISPENSA++;
                        if (INDEX_DISPENSA >= (_listaDispensa.Count - 1))
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        else
                        {
                            this.passosActionP3 = 6;
                        }

                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.isRunning = false;
                        PausarMonitoramento();
                        DialogResult = DialogResult.OK;
                        Close();
                        break;
                    }                
                case 18:
                    {
                        this._paramIni.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._paramIni.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._paramIni.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) && !this._paramIni.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._paramIni.DispenserP3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this._paramIni.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._paramIni.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._paramIni.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._paramIni.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                this._paramIni.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._paramIni.DispenserP3.IsNativo == 0 || this._paramIni.DispenserP3.IsNativo == 2) &&
                                (!this._paramIni.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }
        }

        private bool btEmergenciaCodErro(int passosActionP3Emergencia, int passosActionP3CodError)
        {
            bool retorno = false;
            if (this._paramIni.DispenserP3.SensorEmergencia)
            {
                retorno = true;
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                {
                    //bool isRec = m.ShowDialog("Usuário Botão de Emergência Pressionado. Deseja finalizar este processo?", "Yes", "No", true, 60);
                    bool isRec = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Botao_Emergencia_Passos, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true, 60);
                    if (isRec)
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.No;
                        Close();
                    }
                    else
                    {
                        this.passosActionP3 = passosActionP3Emergencia;
                    }
                }

            }
            else if (this._paramIni.DispenserP3.CodError > 0)
            {
                retorno = true;
                if (this.counterFalha >= 2)
                {
                    PausarMonitoramento();
                    Thread.Sleep(1000);
                    DialogResult = DialogResult.No;
                    Close();
                }

                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                {
                    //string msg = "Usuário ocorreu algum erro na movimentação da placa. Deseja continuar este processo?" + Environment.NewLine + "Descrição: " + this._paramIni.DispenserP3.GetDescCodError();
                    string msg = Negocio.IdiomaResxExtensao.PlacaMov_Erro_Movimentacao_Passos + this._paramIni.DispenserP3.GetDescCodError();
                    bool isRec = m.ShowDialog(msg, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true, 30);
                    if (isRec)
                    {
                        this.counterFalha++;
                        this.passosActionP3 = passosActionP3CodError;
                    }
                    else
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.No;
                        Close();
                    }
                }
            }
            return retorno;
        }
        #endregion
    }
}
