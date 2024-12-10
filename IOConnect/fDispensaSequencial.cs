using Percolore.Core;
using Percolore.Core.Logging;
using System.ComponentModel;

namespace Percolore.IOConnect
{
	public partial class fDispensaSequencial : Form
    {
        Util.ObjectParametros _parametros = Util.ObjectParametros.Load();

        Dictionary<int, double> _demanda;
        DispensaSequencialVO prmDispensa;
        int INDEX_CONTADOR;
        int INDEX_ULTIMO_CIRCUITO;

        //int INDEX_CONTADOR2;
        //int INDEX_ULTIMO_CIRCUITO2;

        readonly bool desativarUI;
        private int counterFalha = 0;

        System.ComponentModel.BackgroundWorker backgroundWorker1 = null;
        private bool isThread = false;
        private bool isRunning = false;
        private bool placa1Seq = true;
        private bool placa2Seq = true;


        private int tipoActionP3 = -1;
        private int passosActionP3 = 0;
        private int qtdTrocaRecipiente = 0;
        
        private bool isNewOperacao = false;

        private List<Util.ObjectColorante> colorantes_Seg = null;

        private int i_Step = 0;

        public fDispensaSequencial(
            DispensaSequencialVO dispensaSequencialVO, bool placa1Sequencial, bool placa2Sequencial, Dictionary<int, double> demanda, bool desativarUI = false)
        {
            InitializeComponent();
            this.prmDispensa = dispensaSequencialVO;

            /* Se a exibição da interface for desabilitada
             * não é necessário configurar propriedades dos controles */
            this.desativarUI = desativarUI;
            this.placa1Seq = placa1Sequencial;
            this.placa2Seq = placa2Sequencial;
            this._demanda = demanda;

            if (this.desativarUI)
            {
                return;
            }

            //Dimensiona e posiciona
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Location = new Point(0, 30);

            //Globalização
            this.btnIniciar.Text = Negocio.IdiomaResxExtensao.Global_Iniciar;
            this.btnCancelar.Text = Negocio.IdiomaResxExtensao.Global_Cancelar;
            this.btnAbortar.Text = Negocio.IdiomaResxExtensao.Global_Abortar;
            this.lblStatus.Text =
                Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
            this.lblSubStatus.Text =
                Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg01;

            //Habilitar controles
            lblStatus.Visible = true;
            progressBar.Visible = false;
            btnIniciar.Enabled = true;
            btnCancelar.Enabled = true;
            btnAbortar.Visible = false;
        }

        private void ProcessoDispensaSequencial_Load(object sender, EventArgs e)
        {
            /* É necessário indicar topMost aqui para que o form seja 
             * redesenhando em primeiro plano sobre qualquer processo em execução */
            TopMost = !desativarUI;

            try
            {
                this.colorantes_Seg = Util.ObjectColorante.List().FindAll(o => o.Seguidor > 0).ToList();
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

			/* Se a exibição da interface for desabilitada,
             * reduz form a tamanho zero e incia automaticamente a dispensa */
			if (desativarUI)
            {
                this.Width = 0;
                this.Height = 0;
                Iniciar_Click(sender, e);
            }
        }

        #region Eventos

        void Iniciar_Click(object sender, EventArgs e)
        {
            progressBar.Visible = true;
            btnIniciar.Enabled = false;
            btnCancelar.Visible = false;
            btnAbortar.Visible = true;

            //Inicializa variáveis de monitoramento
            INDEX_CONTADOR = 0;
            INDEX_ULTIMO_CIRCUITO = this.prmDispensa.Colorantes.Count - 1;
            this.counterFalha = 0;            
            RessetarTempoMonitoramentoCircuitos();

            try
            {
                this.tipoActionP3 = -1;
                this.passosActionP3 = 0;
                if (this.prmDispensa.modBusDispenser_P3 != null)
                {
                    bool isPosicao = false;
                    this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                    if (!(this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2))
                    {
                        //lblStatus.Text = "Condição de placa movimentação incorreta!";
                        lblStatus.Text = Negocio.IdiomaResxExtensao.PlacaMov_Condicao_Incorreta;
                        lblSubStatus.Text = "";
                        return;
                    }

                    if (!this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                    {
                        this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true);
                        for (int _i = 0; _i < 20; _i++)
                        {
                            if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                            {
                                _i = 20;
                            }
                            Thread.Sleep(500);
                        }
                        isPosicao = true;
                    }
                    if (!this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem && !this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
                    {
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                        isPosicao = true;
                    }
                    if (isPosicao)
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if ((!this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada) || (!this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem && !this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao))
                        {
                            //lblStatus.Text = "Condição de placa movimentação incorreta!";
                            lblStatus.Text = Negocio.IdiomaResxExtensao.PlacaMov_Condicao_Incorreta;                            
                            lblSubStatus.Text = "";
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
                    ExecutarMonitoramento();

                    Log.Logar(
                        TipoLog.Processo,
                        _parametros.PathLogProcessoDispensa,
                         Negocio.IdiomaResxExtensao.Log_Cod_01 + Negocio.IdiomaResxExtensao.Global_DosagemIniciada);
                }
               
            }
            catch (Exception ex)
            {
                Falha(ex);
            }
        }

        private int getActionP3()
        {
            int retorno = -1;
            if (this.prmDispensa.modBusDispenser_P3.SensorEsponja && this.prmDispensa.modBusDispenser_P3.SensorCopo && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos && this.prmDispensa.modBusDispenser_P3.SensorAltoBicos
                && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem
                && !this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 1;
            }
            else if (this.prmDispensa.modBusDispenser_P3.SensorEsponja && this.prmDispensa.modBusDispenser_P3.SensorCopo && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos && this.prmDispensa.modBusDispenser_P3.SensorAltoBicos
                && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem
                && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 2;
            }
            else if (this.prmDispensa.modBusDispenser_P3.SensorEsponja && this.prmDispensa.modBusDispenser_P3.SensorCopo && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos && this.prmDispensa.modBusDispenser_P3.SensorAltoBicos
                && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem
                && !this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 3;
            }
            else if (this.prmDispensa.modBusDispenser_P3.SensorEsponja && this.prmDispensa.modBusDispenser_P3.SensorCopo && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos && this.prmDispensa.modBusDispenser_P3.SensorAltoBicos
                && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem
                && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 4;
            }
            else if (this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo && this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos && !this.prmDispensa.modBusDispenser_P3.SensorAltoBicos
                && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem
                && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 5;
            }
            else if (this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos && this.prmDispensa.modBusDispenser_P3.SensorAltoBicos
                && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem
                && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 6;
            }
            else if (this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos && this.prmDispensa.modBusDispenser_P3.SensorAltoBicos
                && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem
                && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 7;
            }
            else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos && this.prmDispensa.modBusDispenser_P3.SensorAltoBicos
               && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem
               && !this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 8;
            }
            else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos && this.prmDispensa.modBusDispenser_P3.SensorAltoBicos
               && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem
               && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 9;
            }
            else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos && this.prmDispensa.modBusDispenser_P3.SensorAltoBicos
               && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem
               && !this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 10;
            }
            else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos && this.prmDispensa.modBusDispenser_P3.SensorAltoBicos
               && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem
               && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 11;
            }
            else if (this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos && !this.prmDispensa.modBusDispenser_P3.SensorAltoBicos
               && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem
               && !this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 12;
            }
            else if (this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos && !this.prmDispensa.modBusDispenser_P3.SensorAltoBicos
              && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem
              && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 13;
            }
            else if (this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo && this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos &&
                !this.prmDispensa.modBusDispenser_P3.SensorAltoBicos && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada &&
                this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem && !this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 14;
            }
            else if (this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos &&
                this.prmDispensa.modBusDispenser_P3.SensorAltoBicos && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada &&
                this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem && !this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 15;
            }
            else if (this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos &&
                this.prmDispensa.modBusDispenser_P3.SensorAltoBicos && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada &&
                this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem && !this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 16;
            }
            return retorno;
        }

       

        void Abortar_Click(object sender, EventArgs e)
        {           
            PausarMonitoramento();

            try
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; i < prmDispensa.Dispenser.Count; i++)
                    {
                        //Interrompe e tetoma operação da placa
                        prmDispensa.Dispenser[i].Halt();
                        prmDispensa.Dispenser[i].UnHalt();
                    }
                    Thread.Sleep(1000);
                    if (prmDispensa.Dispenser.Count > 1)
                    {
                        if (prmDispensa.Dispenser[0].IsReady && prmDispensa.Dispenser[1].IsReady)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (prmDispensa.Dispenser[0].IsReady)
                        {
                            break;
                        }
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
			finally
            {
                //Status da operação
                DialogResult = DialogResult.Abort;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void PurgaDispenseForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            PausarMonitoramento();

            switch (this.DialogResult)
            {
                #region Status da do processo

                case DialogResult.OK:
                    {
                        #region Sucesso

                        string desc =
                           string.IsNullOrEmpty(this.prmDispensa.DescricaoCor) ? string.Empty : " - " + this.prmDispensa.DescricaoCor;

                        Log.Logar(
                         TipoLog.Processo,
                         _parametros.PathLogProcessoDispensa,
                          Negocio.IdiomaResxExtensao.Log_Cod_02 + Negocio.IdiomaResxExtensao.Global_DosagemDescricaoCorantes + desc);

                      

                        if (_parametros.ControlarNivel)
                        {
                            Operar.AbaterColorante(_demanda);
                            Log.Logar(
                                TipoLog.Processo,
                                 Util.ObjectParametros.Load().PathLogProcessoDispensa,
                                Negocio.IdiomaResxExtensao.Log_Cod_03 + Negocio.IdiomaResxExtensao.Global_BaixaColoranteEfetuada);
                        }

                        string log = "";
                        foreach (KeyValuePair<int, double> d in prmDispensa.Demanda)
                            log += $"{d.Key},{Math.Round(d.Value, 3)},";

                        log = log.Remove(log.Length - 1);
                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, log);
                        Log.Logar(TipoLog.ControleDispensa, _parametros.PathLogControleDispensa, log);

                        desc =
                          string.IsNullOrEmpty(this.prmDispensa.CodigoCor) ? string.Empty : " - " + this.prmDispensa.CodigoCor;

                        Log.Logar(
                           TipoLog.Processo,
                           _parametros.PathLogProcessoDispensa,
                           Negocio.IdiomaResxExtensao.Log_Cod_04 + Negocio.IdiomaResxExtensao.Global_DosagemConcluida + desc);

                        break;

                        #endregion
                    }
                case DialogResult.Cancel:
                    {
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_07 + Negocio.IdiomaResxExtensao.Global_DosagemCancelada);
                        break;
                    }
                case DialogResult.Abort:
                    {
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_05 + Negocio.IdiomaResxExtensao.Global_DosagemAbortada);
                        break;
                    }
                case DialogResult.No:
                    {
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_06 + Negocio.IdiomaResxExtensao.Global_FalhaDosagem);
                        break;
                    }

                    #endregion
            }
        }

        #endregion

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
                this.prmDispensa.modBusDispenser_P3.Halt();
                this.prmDispensa.modBusDispenser_P3.UnHalt();
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
                this.prmDispensa.modBusDispenser_P3.Halt();
                this.prmDispensa.modBusDispenser_P3.UnHalt();
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
                    this.isThread = true;
                    this.isRunning = false;
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
                            this.Invoke(new MethodInvoker(Monitoramento_Event));
                            if (this.prmDispensa.modBusDispenser_P3 == null)
                            {
                                Thread.Sleep(500);
                            }
                            else
                            {
                                Thread.Sleep(500);
                            }
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

        //Função para identificar o reinicio do Monitoramento dos circuitos
        void RessetarTempoMonitoramentoCircuitos()
        {
            Constants.countTimerDelayMonit = 0;
        }

        //void Monitoramento_Event(object sender, EventArgs e)
        void Monitoramento_Event()
        {
            try
            {
                if (prmDispensa.modBusDispenser_P3 != null)
                {
                    trataActionP3();
                }
                else
                {
                    //[Verifica se dispositivo está pronto]
                    for (int i = 0; i < prmDispensa.Dispenser.Count; i++)
                    {
                        if (!prmDispensa.Dispenser[i].IsReady)
                        {
                            this.isRunning = false;
                            Thread.Sleep(1000);
                            return;
                        }
                    }

                    
                    //[Verifica se já executou todos os circuitos de colorantes]
                    if (INDEX_CONTADOR > INDEX_ULTIMO_CIRCUITO)
                    {
                        this.isRunning = false;
                        PausarMonitoramento();
                        int index_2 = 16;
                        if(/*(Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_4 ||*/ (Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador )
                        {
                            index_2 = 16;
                        }
                        //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                        for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_CIRCUITO; INDEX_CONTADOR++)
                        {
                            int nCIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                            if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 1)
                            {
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso[nCIRCUITO - 1]);
                                Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume[nCIRCUITO - 1]);
                            }
                            else if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 2)
                            {
                                
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso2[(nCIRCUITO - index_2) - 1]);
                                Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume2[(nCIRCUITO - index_2) - 1]);
                            }


                        }

                        DialogResult = DialogResult.OK;
                        Close();
                        return;
                    }


                    //[Recupera posição do circuito do colorante]
                    int CIRCUITO = prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                    int dispositivo = prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                    if (dispositivo == 1)
                    {
                        if (prmDispensa.PulsoHorario[CIRCUITO - 1] > 0)
                        {
                            if (this.placa1Seq)
                            {
                                //[Atualiza interface]
                                lblStatus.Text =
                                Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02 + " " + prmDispensa.Colorantes[INDEX_CONTADOR].Nome;
                                lblSubStatus.Text =
                                Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");
                                List<Util.ObjectColorante> lcol = this.colorantes_Seg.FindAll(o => o.Seguidor == CIRCUITO).ToList();

                                if (lcol == null || lcol.Count == 0)
                                {
                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => prmDispensa.Dispenser[0].Dispensar(
                                            CIRCUITO - 1,
                                            prmDispensa.PulsoHorario[CIRCUITO - 1],
                                            prmDispensa.Velocidade[CIRCUITO - 1],
                                            prmDispensa.Aceleracao[CIRCUITO - 1],
                                            prmDispensa.Delay[CIRCUITO - 1],
                                            prmDispensa.PulsoReverso[CIRCUITO - 1],
                                            i_Step)
                                        );
                                }
                                else
                                {
                                    int[] n_motor = new int[lcol.Count + 1];
                                    n_motor[0] = CIRCUITO-1;
                                    for (int i = 0; i < lcol.Count; i++)
                                    {
                                        n_motor[i + 1] = (lcol[i].Circuito - 1);
                                    }
                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => prmDispensa.Dispenser[0].Dispensar(
                                            n_motor,
                                            prmDispensa.PulsoHorario[CIRCUITO - 1],
                                            prmDispensa.Velocidade[CIRCUITO - 1],
                                            prmDispensa.Aceleracao[CIRCUITO - 1],
                                            prmDispensa.Delay[CIRCUITO - 1],
                                            prmDispensa.PulsoReverso[CIRCUITO - 1],
                                            i_Step)
                                        );
                                }
                                /*
                                if (_parametros.ControlarNivel)
                                {
                                    Operar.AbaterColorante(
                                        CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                }
                                */
                                INDEX_CONTADOR++;
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " 1 Simultânea";
                                ValoresVO[] _valores = new ValoresVO[16];
                                int i = 0;
                                List<Util.ObjectColorante> lCol = Util.ObjectColorante.List().FindAll(o => o.Seguidor > 0).ToList();
                                for (i = INDEX_CONTADOR; i < prmDispensa.Colorantes.Count; i++)
                                {
                                    CIRCUITO = prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                    dispositivo = prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                                    if (dispositivo == 1)
                                    {
                                        ValoresVO val = new ValoresVO();
                                        val.PulsoHorario = prmDispensa.PulsoHorario[CIRCUITO - 1];
                                        val.PulsoReverso = prmDispensa.PulsoReverso[CIRCUITO - 1];
                                        val.Aceleracao = prmDispensa.Aceleracao[CIRCUITO - 1];
                                        val.Delay = prmDispensa.Delay[CIRCUITO - 1];
                                        val.Velocidade = prmDispensa.Velocidade[CIRCUITO - 1];

                                        _valores[CIRCUITO - 1] = val;
                                        List<Util.ObjectColorante> ncol = lCol.FindAll(o => o.Seguidor == CIRCUITO).ToList();
                                        if (ncol != null && ncol.Count > 0)
                                        {
                                            foreach (Util.ObjectColorante _col in ncol)
                                            {
                                                _valores[_col.Circuito - 1] = val;
                                            }
                                        }

                                        /*
                                        if (_parametros.ControlarNivel)
                                        {
                                            Operar.AbaterColorante(
                                                CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                        }
                                        */
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    INDEX_CONTADOR++;
                                }
                                Task task = Task.Factory.StartNew(() => prmDispensa.Dispenser[0].Dispensar(_valores,
                                            i_Step));

                                Thread.Sleep(1000);
                            }
                        }

                    }
                    else if (dispositivo == 2)
                    {
                        int CIRCUITO2 = CIRCUITO - 16;
                        int index_2 = 16;
                        if (/*(Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_4 ||*/ (Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador)
                        {
                            CIRCUITO2 = CIRCUITO - 16;
                            index_2 = 16;
                        }

                        if (prmDispensa.PulsoHorario2[CIRCUITO2 - 1] > 0)
                        {
                            if (this.placa2Seq)
                            {
                                //[Atualiza interface]
                                lblStatus.Text =
                                Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02 + " " + prmDispensa.Colorantes[INDEX_CONTADOR].Nome;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                                List<Util.ObjectColorante> lcol = this.colorantes_Seg.FindAll(o => o.Seguidor == CIRCUITO).ToList();

                                if (lcol == null || lcol.Count == 0)
                                {
                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => prmDispensa.Dispenser[1].Dispensar(
                                        CIRCUITO2 - 1,
                                        prmDispensa.PulsoHorario2[CIRCUITO2 - 1],
                                        prmDispensa.Velocidade2[CIRCUITO2 - 1],
                                        prmDispensa.Aceleracao2[CIRCUITO2 - 1],
                                        prmDispensa.Delay2[CIRCUITO2 - 1],
                                        prmDispensa.PulsoReverso2[CIRCUITO2 - 1],
                                            i_Step)
                                    );
                                }
                                else
                                {
                                    int[] n_motor = new int[lcol.Count + 1];
                                    n_motor[0] = CIRCUITO2 - 1;
                                    for (int i = 0; i < lcol.Count; i++)
                                    {
                                        n_motor[i + 1] = ((lcol[i].Circuito - index_2) - 1);
                                    }

                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => prmDispensa.Dispenser[1].Dispensar(
                                        n_motor,
                                        prmDispensa.PulsoHorario2[CIRCUITO2 - 1],
                                        prmDispensa.Velocidade2[CIRCUITO2 - 1],
                                        prmDispensa.Aceleracao2[CIRCUITO2 - 1],
                                        prmDispensa.Delay2[CIRCUITO2 - 1],
                                        prmDispensa.PulsoReverso2[CIRCUITO2 - 1],
                                            i_Step)
                                    );
                                }

                                /*
                                if (_parametros.ControlarNivel)
                                {
                                    Operar.AbaterColorante(
                                        CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                }
                                */
                                Thread.Sleep(1000);
                                INDEX_CONTADOR++;
                            }
                            else
                            {
                                lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " 2 Simultânea";
                                ValoresVO[] _valores = new ValoresVO[16];

                                List<Util.ObjectColorante> lCol = Util.ObjectColorante.List().FindAll(o => o.Seguidor > 0).ToList();

                                int i = 0;
                                for (i = INDEX_CONTADOR; i < prmDispensa.Colorantes.Count; i++)
                                {
                                    CIRCUITO = prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                    CIRCUITO2 = CIRCUITO - 16;
                                    index_2 = 16;
                                    if (/*(Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_4 ||*/ (Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador)
                                    {
                                        CIRCUITO2 = CIRCUITO - 16;
                                        index_2 = 16;
                                    }
                                    dispositivo = prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                                    if (dispositivo == 2)
                                    {
                                        ValoresVO val = new ValoresVO();
                                        val.PulsoHorario = prmDispensa.PulsoHorario2[CIRCUITO2 - 1];
                                        val.PulsoReverso = prmDispensa.PulsoReverso2[CIRCUITO2 - 1];
                                        val.Aceleracao = prmDispensa.Aceleracao2[CIRCUITO2 - 1];
                                        val.Delay = prmDispensa.Delay2[CIRCUITO2 - 1];
                                        val.Velocidade = prmDispensa.Velocidade2[CIRCUITO2 - 1];


                                        _valores[CIRCUITO2 - 1] = val;

                                        List<Util.ObjectColorante> ncol = lCol.FindAll(o => o.Seguidor == CIRCUITO).ToList();
                                        if (ncol != null && ncol.Count > 0)
                                        {
                                            foreach (Util.ObjectColorante _col in ncol)
                                            {
                                                _valores[(_col.Circuito- index_2) - 1] = val;
                                            }
                                        }
                                        /*
                                        if (_parametros.ControlarNivel)
                                        {
                                            Operar.AbaterColorante(
                                                CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                        }
                                        */
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    INDEX_CONTADOR++;
                                }
                                Task task = Task.Factory.StartNew(() => prmDispensa.Dispenser[1].Dispensar(_valores,
                                            i_Step));

                                Thread.Sleep(1000);
                            }
                        }

                    }                    
                }

                this.isRunning = false;
                this.counterFalha = 0;
            }
            catch (Exception ex)
            {
                if (this.counterFalha > _parametros.QtdTentativasConexao)
                {
					string customMessage = string.Empty;
					if (ex.Message.Contains("Could not read status register:"))
						customMessage = Negocio.IdiomaResxExtensao.Global_Falha_PerdaConexaoDispositivo;

					Falha(ex, customMessage);
                }
                else
                {
                    ForceOpenConnection();
                }
               
                this.isRunning = false;
                this.counterFalha++;
            }
        }

        void ForceOpenConnection()
        {
            try
            {
                if (this.prmDispensa.modBusDispenser_P3 != null)
                {
                    this.prmDispensa.modBusDispenser_P3.Disconnect();
                    Thread.Sleep(1000);
                    this.prmDispensa.modBusDispenser_P3.Disconnect_Mover();
                    Thread.Sleep(1000);
                    this.prmDispensa.modBusDispenser_P3.Connect();
                    Thread.Sleep(1000);
                    this.prmDispensa.modBusDispenser_P3.Connect_Mover();
                }
                else
                {
                    for (int i = 0; i < this.prmDispensa.Dispenser.Count; i++)
                    {
                        this.prmDispensa.Dispenser[i].Disconnect();
                        Thread.Sleep(1000);
                        this.prmDispensa.Dispenser[i].Connect();
                        Thread.Sleep(1000);
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

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
                            //bool isRec = m.ShowDialog("Recipiente está na Dosador", "Yes", "No");
                            bool isRec = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Recipiente_Dosadora, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                            if (!isRec)
                            {
                                //this.passosActionP3 = 1;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 2;
                            }
                            else
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.prmDispensa.modBusDispenser_P3.SensorCopo || !this.prmDispensa.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";

                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        lblSubStatus.Text = "";
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        //[Verifica se dispositivo está pronto]
                        if (this.prmDispensa.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //[Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CONTADOR > INDEX_ULTIMO_CIRCUITO)
                        {                        
                            //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_CIRCUITO; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 1)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso[nCIRCUITO - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume[nCIRCUITO - 1]);
                                }
                                else if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 2)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso2[(nCIRCUITO - 16) - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume2[(nCIRCUITO - 16) - 1]);
                                }
                            }
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            break;
                        }


                        //[Recupera posição do circuito do colorante]
                        int CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                        int dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                        if (dispositivo == 1)
                        {
                            if (this.prmDispensa.PulsoHorario[CIRCUITO - 1] > 0)
                            {
                                if (this.placa1Seq)
                                {
                                    //[Atualiza interface]
                                    lblStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02 + " " + this.prmDispensa.Colorantes[INDEX_CONTADOR].Nome;
                                    lblSubStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => this.prmDispensa.Dispenser[0].Dispensar(
                                            CIRCUITO - 1,
                                            this.prmDispensa.PulsoHorario[CIRCUITO - 1],
                                            this.prmDispensa.Velocidade[CIRCUITO - 1],
                                            this.prmDispensa.Aceleracao[CIRCUITO - 1],
                                            this.prmDispensa.Delay[CIRCUITO - 1],
                                            this.prmDispensa.PulsoReverso[CIRCUITO - 1],
                                            i_Step)
                                        );

                                    /*
                                    if (_parametros.ControlarNivel)
                                    {
                                        Operar.AbaterColorante(
                                            CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                    }
                                    */
                                    INDEX_CONTADOR++;
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " 1 Simultânea";
                                    ValoresVO[] _valores = new ValoresVO[16];
                                    int i = 0;
                                    for (i = INDEX_CONTADOR; i < this.prmDispensa.Colorantes.Count; i++)
                                    {
                                        CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                        dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                                        if (dispositivo == 1)
                                        {
                                            ValoresVO val = new ValoresVO();
                                            val.PulsoHorario = this.prmDispensa.PulsoHorario[CIRCUITO - 1];
                                            val.PulsoReverso = this.prmDispensa.PulsoReverso[CIRCUITO - 1];
                                            val.Aceleracao = this.prmDispensa.Aceleracao[CIRCUITO - 1];
                                            val.Delay = this.prmDispensa.Delay[CIRCUITO - 1];
                                            val.Velocidade = this.prmDispensa.Velocidade[CIRCUITO - 1];

                                            _valores[CIRCUITO - 1] = val;


                                            /*
                                            if (_parametros.ControlarNivel)
                                            {
                                                Operar.AbaterColorante(
                                                    CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                            }
                                            */
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        INDEX_CONTADOR++;
                                    }
                                    this.prmDispensa.Dispenser[0].Dispensar(_valores,
                                            i_Step);

                                    Thread.Sleep(1000);
                                }
                            }

                        }
                        else
                        {
                            INDEX_CONTADOR++;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                this.counterFalha = 0;
                                if (!this.isNewOperacao)
                                {
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                    this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 17;
                                }
                                else
                                {
                                    PausarMonitoramento();
                                    DialogResult = DialogResult.OK;
                                    Close();
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
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
                            //bool isRec = m.ShowDialog("Recipiente está na Dosador", "Yes", "No");
                            bool isRec = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Recipiente_Dosadora, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                            if (!isRec)
                            {
                                //this.passosActionP3 = 1;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 2;
                            }
                            else
                            {
                                //Movimentar Válvula Recuar Bicos
                                //this.passosActionP3 = 18;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                                this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 19;
                            }
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.prmDispensa.modBusDispenser_P3.SensorCopo || !this.prmDispensa.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 18;
                                this.counterFalha = 0;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                                this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 19;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        lblSubStatus.Text = "";
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        //[Verifica se dispositivo está pronto]
                        if (this.prmDispensa.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //[Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CONTADOR > INDEX_ULTIMO_CIRCUITO)
                        {
                            //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_CIRCUITO; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 1)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso[nCIRCUITO - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume[nCIRCUITO - 1]);
                                }
                                else if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 2)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso2[(nCIRCUITO - 16) - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume2[(nCIRCUITO - 16) - 1]);
                                }
                            }
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            break;
                        }


                        //[Recupera posição do circuito do colorante]
                        int CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                        int dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                        if (dispositivo == 1)
                        {
                            if (this.prmDispensa.PulsoHorario[CIRCUITO - 1] > 0)
                            {
                                if (this.placa1Seq)
                                {
                                    //[Atualiza interface]
                                    lblStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02 + " " + this.prmDispensa.Colorantes[INDEX_CONTADOR].Nome;
                                    lblSubStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => this.prmDispensa.Dispenser[0].Dispensar(
                                            CIRCUITO - 1,
                                            this.prmDispensa.PulsoHorario[CIRCUITO - 1],
                                            this.prmDispensa.Velocidade[CIRCUITO - 1],
                                            this.prmDispensa.Aceleracao[CIRCUITO - 1],
                                            this.prmDispensa.Delay[CIRCUITO - 1],
                                            this.prmDispensa.PulsoReverso[CIRCUITO - 1],
                                            i_Step)
                                        );

                                    /*
                                    if (_parametros.ControlarNivel)
                                    {
                                        Operar.AbaterColorante(
                                            CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                    }
                                    */
                                    INDEX_CONTADOR++;
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " 1 Simultânea";
                                    ValoresVO[] _valores = new ValoresVO[16];
                                    int i = 0;
                                    for (i = INDEX_CONTADOR; i < this.prmDispensa.Colorantes.Count; i++)
                                    {
                                        CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                        dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                                        if (dispositivo == 1)
                                        {
                                            ValoresVO val = new ValoresVO();
                                            val.PulsoHorario = this.prmDispensa.PulsoHorario[CIRCUITO - 1];
                                            val.PulsoReverso = this.prmDispensa.PulsoReverso[CIRCUITO - 1];
                                            val.Aceleracao = this.prmDispensa.Aceleracao[CIRCUITO - 1];
                                            val.Delay = this.prmDispensa.Delay[CIRCUITO - 1];
                                            val.Velocidade = this.prmDispensa.Velocidade[CIRCUITO - 1];

                                            _valores[CIRCUITO - 1] = val;


                                            /*
                                            if (_parametros.ControlarNivel)
                                            {
                                                Operar.AbaterColorante(
                                                    CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                            }
                                            */
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        INDEX_CONTADOR++;
                                    }
                                    this.prmDispensa.Dispenser[0].Dispensar(_valores,
                                            i_Step);

                                    Thread.Sleep(1000);
                                }
                            }

                        }
                        else
                        {
                            INDEX_CONTADOR++;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                this.counterFalha = 0;
                                if (!this.isNewOperacao)
                                {
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                    this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 17;
                                }
                                else
                                {
                                    PausarMonitoramento();
                                    DialogResult = DialogResult.OK;
                                    Close();
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            //Dosar
                            //this.passosActionP3 = 6;
                            this.isNewOperacao = false;
                            this.passosActionP3 = 7;
                            lblSubStatus.Text = "";
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
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
                                //this.passosActionP3 = 1;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 2;
                            }
                            else
                            {
                                this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                                if (this.prmDispensa.modBusDispenser_P3.SensorCopo && this.prmDispensa.modBusDispenser_P3.SensorEsponja)
                                {
                                    //Movimentar fechar gaveta
                                    //this.passosActionP3 = 20;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                                    Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 21;
                                }
                            }
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.prmDispensa.modBusDispenser_P3.SensorCopo || !this.prmDispensa.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav; 
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada && this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada || !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        lblSubStatus.Text = "";
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        //[Verifica se dispositivo está pronto]
                        if (this.prmDispensa.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //[Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CONTADOR > INDEX_ULTIMO_CIRCUITO)
                        {
                            //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_CIRCUITO; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 1)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso[nCIRCUITO - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume[nCIRCUITO - 1]);
                                }
                                else if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 2)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso2[(nCIRCUITO - 16) - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume2[(nCIRCUITO - 16) - 1]);
                                }
                            }
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            break;
                        }


                        //[Recupera posição do circuito do colorante]
                        int CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                        int dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                        if (dispositivo == 1)
                        {
                            if (this.prmDispensa.PulsoHorario[CIRCUITO - 1] > 0)
                            {
                                if (this.placa1Seq)
                                {
                                    //[Atualiza interface]
                                    lblStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02 + " " + this.prmDispensa.Colorantes[INDEX_CONTADOR].Nome;
                                    lblSubStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => this.prmDispensa.Dispenser[0].Dispensar(
                                            CIRCUITO - 1,
                                            this.prmDispensa.PulsoHorario[CIRCUITO - 1],
                                            this.prmDispensa.Velocidade[CIRCUITO - 1],
                                            this.prmDispensa.Aceleracao[CIRCUITO - 1],
                                            this.prmDispensa.Delay[CIRCUITO - 1],
                                            this.prmDispensa.PulsoReverso[CIRCUITO - 1],
                                            i_Step)
                                        );

                                    /*
                                    if (_parametros.ControlarNivel)
                                    {
                                        Operar.AbaterColorante(
                                            CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                    }
                                    */
                                    INDEX_CONTADOR++;
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " 1 Simultânea";
                                    ValoresVO[] _valores = new ValoresVO[16];
                                    int i = 0;
                                    for (i = INDEX_CONTADOR; i < this.prmDispensa.Colorantes.Count; i++)
                                    {
                                        CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                        dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                                        if (dispositivo == 1)
                                        {
                                            ValoresVO val = new ValoresVO();
                                            val.PulsoHorario = this.prmDispensa.PulsoHorario[CIRCUITO - 1];
                                            val.PulsoReverso = this.prmDispensa.PulsoReverso[CIRCUITO - 1];
                                            val.Aceleracao = this.prmDispensa.Aceleracao[CIRCUITO - 1];
                                            val.Delay = this.prmDispensa.Delay[CIRCUITO - 1];
                                            val.Velocidade = this.prmDispensa.Velocidade[CIRCUITO - 1];

                                            _valores[CIRCUITO - 1] = val;


                                            /*
                                            if (_parametros.ControlarNivel)
                                            {
                                                Operar.AbaterColorante(
                                                    CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                            }
                                            */
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        INDEX_CONTADOR++;
                                    }
                                    this.prmDispensa.Dispenser[0].Dispensar(_valores,
                                            i_Step);

                                    Thread.Sleep(1000);
                                }
                            }

                        }
                        else
                        {
                            INDEX_CONTADOR++;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                this.counterFalha = 0;
                                if (!this.isNewOperacao)
                                {
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                    this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 17;
                                }
                                else
                                {
                                    PausarMonitoramento();
                                    DialogResult = DialogResult.OK;
                                    Close();
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            //Dosar
                            //this.passosActionP3 = 6;
                            this.isNewOperacao = false;
                            this.passosActionP3 = 7;
                            lblSubStatus.Text = "";
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
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
                                //this.passosActionP3 = 1;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 2;
                            }
                            else
                            {
                                this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                                if (this.prmDispensa.modBusDispenser_P3.SensorCopo && this.prmDispensa.modBusDispenser_P3.SensorEsponja)
                                {
                                    //Movimentar fechar gaveta
                                    //this.passosActionP3 = 20;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                                    Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 21;
                                }
                            }
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.prmDispensa.modBusDispenser_P3.SensorCopo || !this.prmDispensa.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 18;
                                this.counterFalha = 0;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                                this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 19;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && 
                                !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        lblSubStatus.Text = "";
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        //[Verifica se dispositivo está pronto]
                        if (this.prmDispensa.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //[Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CONTADOR > INDEX_ULTIMO_CIRCUITO)
                        {
                            //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_CIRCUITO; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 1)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso[nCIRCUITO - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume[nCIRCUITO - 1]);
                                }
                                else if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 2)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso2[(nCIRCUITO - 16) - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume2[(nCIRCUITO - 16) - 1]);
                                }
                            }
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            break;
                        }


                        //[Recupera posição do circuito do colorante]
                        int CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                        int dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                        if (dispositivo == 1)
                        {
                            if (this.prmDispensa.PulsoHorario[CIRCUITO - 1] > 0)
                            {
                                if (this.placa1Seq)
                                {
                                    //[Atualiza interface]
                                    lblStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02 + " " + this.prmDispensa.Colorantes[INDEX_CONTADOR].Nome;
                                    lblSubStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => this.prmDispensa.Dispenser[0].Dispensar(
                                            CIRCUITO - 1,
                                            this.prmDispensa.PulsoHorario[CIRCUITO - 1],
                                            this.prmDispensa.Velocidade[CIRCUITO - 1],
                                            this.prmDispensa.Aceleracao[CIRCUITO - 1],
                                            this.prmDispensa.Delay[CIRCUITO - 1],
                                            this.prmDispensa.PulsoReverso[CIRCUITO - 1],
                                            i_Step)
                                        );

                                    /*
                                    if (_parametros.ControlarNivel)
                                    {
                                        Operar.AbaterColorante(
                                            CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                    }
                                    */
                                    INDEX_CONTADOR++;
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " 1 Simultânea";
                                    ValoresVO[] _valores = new ValoresVO[16];
                                    int i = 0;
                                    for (i = INDEX_CONTADOR; i < this.prmDispensa.Colorantes.Count; i++)
                                    {
                                        CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                        dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                                        if (dispositivo == 1)
                                        {
                                            ValoresVO val = new ValoresVO();
                                            val.PulsoHorario = this.prmDispensa.PulsoHorario[CIRCUITO - 1];
                                            val.PulsoReverso = this.prmDispensa.PulsoReverso[CIRCUITO - 1];
                                            val.Aceleracao = this.prmDispensa.Aceleracao[CIRCUITO - 1];
                                            val.Delay = this.prmDispensa.Delay[CIRCUITO - 1];
                                            val.Velocidade = this.prmDispensa.Velocidade[CIRCUITO - 1];

                                            _valores[CIRCUITO - 1] = val;


                                            /*
                                            if (_parametros.ControlarNivel)
                                            {
                                                Operar.AbaterColorante(
                                                    CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                            }
                                            */
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        INDEX_CONTADOR++;
                                    }
                                    this.prmDispensa.Dispenser[0].Dispensar(_valores,
                                            i_Step);

                                    Thread.Sleep(1000);
                                }
                            }

                        }
                        else
                        {
                            INDEX_CONTADOR++;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                this.counterFalha = 0;
                                if (!this.isNewOperacao)
                                {
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                    this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 17;
                                }
                                else
                                {
                                    PausarMonitoramento();
                                    DialogResult = DialogResult.OK;
                                    Close();
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            //Dosar
                            //this.passosActionP3 = 6;
                            this.isNewOperacao = false;
                            this.passosActionP3 = 7;
                            lblSubStatus.Text = "";
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
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
                        //this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.prmDispensa.modBusDispenser_P3.SensorCopo || !this.prmDispensa.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        lblSubStatus.Text = "";
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        //[Verifica se dispositivo está pronto]
                        if (this.prmDispensa.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //[Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CONTADOR > INDEX_ULTIMO_CIRCUITO)
                        {
                            //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_CIRCUITO; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 1)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso[nCIRCUITO - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume[nCIRCUITO - 1]);
                                }
                                else if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 2)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso2[(nCIRCUITO - 16) - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume2[(nCIRCUITO - 16) - 1]);
                                }
                            }
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            break;
                        }


                        //[Recupera posição do circuito do colorante]
                        int CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                        int dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                        if (dispositivo == 1)
                        {
                            if (this.prmDispensa.PulsoHorario[CIRCUITO - 1] > 0)
                            {
                                if (this.placa1Seq)
                                {
                                    //[Atualiza interface]
                                    lblStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02 + " " + this.prmDispensa.Colorantes[INDEX_CONTADOR].Nome;
                                    lblSubStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => this.prmDispensa.Dispenser[0].Dispensar(
                                            CIRCUITO - 1,
                                            this.prmDispensa.PulsoHorario[CIRCUITO - 1],
                                            this.prmDispensa.Velocidade[CIRCUITO - 1],
                                            this.prmDispensa.Aceleracao[CIRCUITO - 1],
                                            this.prmDispensa.Delay[CIRCUITO - 1],
                                            this.prmDispensa.PulsoReverso[CIRCUITO - 1],
                                            i_Step)
                                        );

                                    /*
                                    if (_parametros.ControlarNivel)
                                    {
                                        Operar.AbaterColorante(
                                            CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                    }
                                    */
                                    INDEX_CONTADOR++;
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " 1 Simultânea";
                                    ValoresVO[] _valores = new ValoresVO[16];
                                    int i = 0;
                                    for (i = INDEX_CONTADOR; i < this.prmDispensa.Colorantes.Count; i++)
                                    {
                                        CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                        dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                                        if (dispositivo == 1)
                                        {
                                            ValoresVO val = new ValoresVO();
                                            val.PulsoHorario = this.prmDispensa.PulsoHorario[CIRCUITO - 1];
                                            val.PulsoReverso = this.prmDispensa.PulsoReverso[CIRCUITO - 1];
                                            val.Aceleracao = this.prmDispensa.Aceleracao[CIRCUITO - 1];
                                            val.Delay = this.prmDispensa.Delay[CIRCUITO - 1];
                                            val.Velocidade = this.prmDispensa.Velocidade[CIRCUITO - 1];

                                            _valores[CIRCUITO - 1] = val;


                                            /*
                                            if (_parametros.ControlarNivel)
                                            {
                                                Operar.AbaterColorante(
                                                    CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                            }
                                            */
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        INDEX_CONTADOR++;
                                    }
                                    this.prmDispensa.Dispenser[0].Dispensar(_valores,
                                            i_Step);

                                    Thread.Sleep(1000);
                                }
                            }

                        }
                        else
                        {
                            INDEX_CONTADOR++;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                this.counterFalha = 0;
                                if (!this.isNewOperacao)
                                {
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                    this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 17;
                                }
                                else
                                {
                                    PausarMonitoramento();
                                    DialogResult = DialogResult.OK;
                                    Close();
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
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
                        //this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.prmDispensa.modBusDispenser_P3.SensorCopo || !this.prmDispensa.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        lblSubStatus.Text = "";
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        //[Verifica se dispositivo está pronto]
                        if (this.prmDispensa.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //[Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CONTADOR > INDEX_ULTIMO_CIRCUITO)
                        {
                            //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_CIRCUITO; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 1)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso[nCIRCUITO - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume[nCIRCUITO - 1]);
                                }
                                else if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 2)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso2[(nCIRCUITO - 16) - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume2[(nCIRCUITO - 16) - 1]);
                                }
                            }
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            break;
                        }


                        //[Recupera posição do circuito do colorante]
                        int CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                        int dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                        if (dispositivo == 1)
                        {
                            if (this.prmDispensa.PulsoHorario[CIRCUITO - 1] > 0)
                            {
                                if (this.placa1Seq)
                                {
                                    //[Atualiza interface]
                                    lblStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02 + " " + this.prmDispensa.Colorantes[INDEX_CONTADOR].Nome;
                                    lblSubStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => this.prmDispensa.Dispenser[0].Dispensar(
                                            CIRCUITO - 1,
                                            this.prmDispensa.PulsoHorario[CIRCUITO - 1],
                                            this.prmDispensa.Velocidade[CIRCUITO - 1],
                                            this.prmDispensa.Aceleracao[CIRCUITO - 1],
                                            this.prmDispensa.Delay[CIRCUITO - 1],
                                            this.prmDispensa.PulsoReverso[CIRCUITO - 1],
                                            i_Step)
                                        );

                                    /*
                                    if (_parametros.ControlarNivel)
                                    {
                                        Operar.AbaterColorante(
                                            CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                    }
                                    */
                                    INDEX_CONTADOR++;
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " 1 Simultânea";
                                    ValoresVO[] _valores = new ValoresVO[16];
                                    int i = 0;
                                    for (i = INDEX_CONTADOR; i < this.prmDispensa.Colorantes.Count; i++)
                                    {
                                        CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                        dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                                        if (dispositivo == 1)
                                        {
                                            ValoresVO val = new ValoresVO();
                                            val.PulsoHorario = this.prmDispensa.PulsoHorario[CIRCUITO - 1];
                                            val.PulsoReverso = this.prmDispensa.PulsoReverso[CIRCUITO - 1];
                                            val.Aceleracao = this.prmDispensa.Aceleracao[CIRCUITO - 1];
                                            val.Delay = this.prmDispensa.Delay[CIRCUITO - 1];
                                            val.Velocidade = this.prmDispensa.Velocidade[CIRCUITO - 1];

                                            _valores[CIRCUITO - 1] = val;


                                            /*
                                            if (_parametros.ControlarNivel)
                                            {
                                                Operar.AbaterColorante(
                                                    CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                            }
                                            */
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        INDEX_CONTADOR++;
                                    }
                                    this.prmDispensa.Dispenser[0].Dispensar(_valores,
                                            i_Step);

                                    Thread.Sleep(1000);
                                }
                            }

                        }
                        else
                        {
                            INDEX_CONTADOR++;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                this.counterFalha = 0;
                                if (!this.isNewOperacao)
                                {
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                    this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 17;
                                }
                                else
                                {
                                    PausarMonitoramento();
                                    DialogResult = DialogResult.OK;
                                    Close();
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
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
                        //this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.prmDispensa.modBusDispenser_P3.SensorCopo || !this.prmDispensa.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        lblSubStatus.Text = "";
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        //[Verifica se dispositivo está pronto]
                        if (this.prmDispensa.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //[Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CONTADOR > INDEX_ULTIMO_CIRCUITO)
                        {
                            //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_CIRCUITO; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 1)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso[nCIRCUITO - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume[nCIRCUITO - 1]);
                                }
                                else if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 2)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso2[(nCIRCUITO - 16) - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume2[(nCIRCUITO - 16) - 1]);
                                }
                            }
                            //this.passosActionP3 = 8;
                            this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            this.counterFalha = 0;
                            break;
                        }


                        //[Recupera posição do circuito do colorante]
                        int CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                        int dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                        if (dispositivo == 1)
                        {
                            if (this.prmDispensa.PulsoHorario[CIRCUITO - 1] > 0)
                            {
                                if (this.placa1Seq)
                                {
                                    //[Atualiza interface]
                                    lblStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02 + " " + this.prmDispensa.Colorantes[INDEX_CONTADOR].Nome;
                                    lblSubStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => this.prmDispensa.Dispenser[0].Dispensar(
                                            CIRCUITO - 1,
                                            this.prmDispensa.PulsoHorario[CIRCUITO - 1],
                                            this.prmDispensa.Velocidade[CIRCUITO - 1],
                                            this.prmDispensa.Aceleracao[CIRCUITO - 1],
                                            this.prmDispensa.Delay[CIRCUITO - 1],
                                            this.prmDispensa.PulsoReverso[CIRCUITO - 1],
                                            i_Step)
                                        );

                                    /*
                                    if (_parametros.ControlarNivel)
                                    {
                                        Operar.AbaterColorante(
                                            CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                    }
                                    */
                                    INDEX_CONTADOR++;
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " 1 Simultânea";
                                    ValoresVO[] _valores = new ValoresVO[16];
                                    int i = 0;
                                    for (i = INDEX_CONTADOR; i < this.prmDispensa.Colorantes.Count; i++)
                                    {
                                        CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                        dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                                        if (dispositivo == 1)
                                        {
                                            ValoresVO val = new ValoresVO();
                                            val.PulsoHorario = this.prmDispensa.PulsoHorario[CIRCUITO - 1];
                                            val.PulsoReverso = this.prmDispensa.PulsoReverso[CIRCUITO - 1];
                                            val.Aceleracao = this.prmDispensa.Aceleracao[CIRCUITO - 1];
                                            val.Delay = this.prmDispensa.Delay[CIRCUITO - 1];
                                            val.Velocidade = this.prmDispensa.Velocidade[CIRCUITO - 1];

                                            _valores[CIRCUITO - 1] = val;


                                            /*
                                            if (_parametros.ControlarNivel)
                                            {
                                                Operar.AbaterColorante(
                                                    CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                            }
                                            */
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        INDEX_CONTADOR++;
                                    }
                                    this.prmDispensa.Dispenser[0].Dispensar(_valores,
                                            i_Step);

                                    Thread.Sleep(1000);
                                }
                            }

                        }
                        else
                        {
                            INDEX_CONTADOR++;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                this.counterFalha = 0;
                                if (!this.isNewOperacao)
                                {
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                    this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 17;
                                }
                                else
                                {
                                    PausarMonitoramento();
                                    DialogResult = DialogResult.OK;
                                    Close();
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
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
                        //this.passosActionP3 = 1;
                        this.counterFalha = 0;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.prmDispensa.modBusDispenser_P3.SensorCopo || !this.prmDispensa.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        lblSubStatus.Text = "";
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        //[Verifica se dispositivo está pronto]
                        if (this.prmDispensa.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //[Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CONTADOR > INDEX_ULTIMO_CIRCUITO)
                        {
                            //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_CIRCUITO; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 1)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso[nCIRCUITO - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume[nCIRCUITO - 1]);
                                }
                                else if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 2)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso2[(nCIRCUITO - 16) - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume2[(nCIRCUITO - 16) - 1]);
                                }
                            }
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            break;
                        }


                        //[Recupera posição do circuito do colorante]
                        int CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                        int dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                        if (dispositivo == 1)
                        {
                            if (this.prmDispensa.PulsoHorario[CIRCUITO - 1] > 0)
                            {
                                if (this.placa1Seq)
                                {
                                    //[Atualiza interface]
                                    lblStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02 + " " + this.prmDispensa.Colorantes[INDEX_CONTADOR].Nome;
                                    lblSubStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => this.prmDispensa.Dispenser[0].Dispensar(
                                            CIRCUITO - 1,
                                            this.prmDispensa.PulsoHorario[CIRCUITO - 1],
                                            this.prmDispensa.Velocidade[CIRCUITO - 1],
                                            this.prmDispensa.Aceleracao[CIRCUITO - 1],
                                            this.prmDispensa.Delay[CIRCUITO - 1],
                                            this.prmDispensa.PulsoReverso[CIRCUITO - 1],
                                            i_Step)
                                        );

                                    /*
                                    if (_parametros.ControlarNivel)
                                    {
                                        Operar.AbaterColorante(
                                            CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                    }
                                    */
                                    INDEX_CONTADOR++;
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " 1 Simultânea";
                                    ValoresVO[] _valores = new ValoresVO[16];
                                    int i = 0;
                                    for (i = INDEX_CONTADOR; i < this.prmDispensa.Colorantes.Count; i++)
                                    {
                                        CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                        dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                                        if (dispositivo == 1)
                                        {
                                            ValoresVO val = new ValoresVO();
                                            val.PulsoHorario = this.prmDispensa.PulsoHorario[CIRCUITO - 1];
                                            val.PulsoReverso = this.prmDispensa.PulsoReverso[CIRCUITO - 1];
                                            val.Aceleracao = this.prmDispensa.Aceleracao[CIRCUITO - 1];
                                            val.Delay = this.prmDispensa.Delay[CIRCUITO - 1];
                                            val.Velocidade = this.prmDispensa.Velocidade[CIRCUITO - 1];

                                            _valores[CIRCUITO - 1] = val;


                                            /*
                                            if (_parametros.ControlarNivel)
                                            {
                                                Operar.AbaterColorante(
                                                    CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                            }
                                            */
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        INDEX_CONTADOR++;
                                    }
                                    this.prmDispensa.Dispenser[0].Dispensar(_valores,
                                            i_Step);

                                    Thread.Sleep(1000);
                                }
                            }

                        }
                        else
                        {
                            INDEX_CONTADOR++;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                this.counterFalha = 0;
                                if (!this.isNewOperacao)
                                {
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                    this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 17;
                                }
                                else
                                {
                                    PausarMonitoramento();
                                    DialogResult = DialogResult.OK;
                                    Close();
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
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
                        //this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.prmDispensa.modBusDispenser_P3.SensorCopo || !this.prmDispensa.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";

                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        lblSubStatus.Text = "";
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        //[Verifica se dispositivo está pronto]
                        if (this.prmDispensa.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //[Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CONTADOR > INDEX_ULTIMO_CIRCUITO)
                        {
                            //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_CIRCUITO; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 1)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso[nCIRCUITO - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume[nCIRCUITO - 1]);
                                }
                                else if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 2)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso2[(nCIRCUITO - 16) - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume2[(nCIRCUITO - 16) - 1]);
                                }
                            }
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            break;
                        }


                        //[Recupera posição do circuito do colorante]
                        int CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                        int dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                        if (dispositivo == 1)
                        {
                            if (this.prmDispensa.PulsoHorario[CIRCUITO - 1] > 0)
                            {
                                if (this.placa1Seq)
                                {
                                    //[Atualiza interface]
                                    lblStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02 + " " + this.prmDispensa.Colorantes[INDEX_CONTADOR].Nome;
                                    lblSubStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => this.prmDispensa.Dispenser[0].Dispensar(
                                            CIRCUITO - 1,
                                            this.prmDispensa.PulsoHorario[CIRCUITO - 1],
                                            this.prmDispensa.Velocidade[CIRCUITO - 1],
                                            this.prmDispensa.Aceleracao[CIRCUITO - 1],
                                            this.prmDispensa.Delay[CIRCUITO - 1],
                                            this.prmDispensa.PulsoReverso[CIRCUITO - 1],
                                            i_Step)
                                        );

                                    /*
                                    if (_parametros.ControlarNivel)
                                    {
                                        Operar.AbaterColorante(
                                            CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                    }
                                    */
                                    INDEX_CONTADOR++;
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " 1 Simultânea";
                                    ValoresVO[] _valores = new ValoresVO[16];
                                    int i = 0;
                                    for (i = INDEX_CONTADOR; i < this.prmDispensa.Colorantes.Count; i++)
                                    {
                                        CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                        dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                                        if (dispositivo == 1)
                                        {
                                            ValoresVO val = new ValoresVO();
                                            val.PulsoHorario = this.prmDispensa.PulsoHorario[CIRCUITO - 1];
                                            val.PulsoReverso = this.prmDispensa.PulsoReverso[CIRCUITO - 1];
                                            val.Aceleracao = this.prmDispensa.Aceleracao[CIRCUITO - 1];
                                            val.Delay = this.prmDispensa.Delay[CIRCUITO - 1];
                                            val.Velocidade = this.prmDispensa.Velocidade[CIRCUITO - 1];

                                            _valores[CIRCUITO - 1] = val;


                                            /*
                                            if (_parametros.ControlarNivel)
                                            {
                                                Operar.AbaterColorante(
                                                    CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                            }
                                            */
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        INDEX_CONTADOR++;
                                    }
                                    this.prmDispensa.Dispenser[0].Dispensar(_valores,
                                            i_Step);

                                    Thread.Sleep(1000);
                                }
                            }

                        }
                        else
                        {
                            INDEX_CONTADOR++;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                this.counterFalha = 0;
                                if (!this.isNewOperacao)
                                {
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                    this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 17;
                                }
                                else
                                {
                                    PausarMonitoramento();
                                    DialogResult = DialogResult.OK;
                                    Close();
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
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
                        //this.passosActionP3 = 1;
                        this.counterFalha = 0;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.prmDispensa.modBusDispenser_P3.SensorCopo || !this.prmDispensa.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        lblSubStatus.Text = "";
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        //[Verifica se dispositivo está pronto]
                        if (this.prmDispensa.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //[Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CONTADOR > INDEX_ULTIMO_CIRCUITO)
                        {
                            //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_CIRCUITO; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 1)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso[nCIRCUITO - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume[nCIRCUITO - 1]);
                                }
                                else if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 2)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso2[(nCIRCUITO - 16) - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume2[(nCIRCUITO - 16) - 1]);
                                }
                            }
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            break;
                        }


                        //[Recupera posição do circuito do colorante]
                        int CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                        int dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                        if (dispositivo == 1)
                        {
                            if (this.prmDispensa.PulsoHorario[CIRCUITO - 1] > 0)
                            {
                                if (this.placa1Seq)
                                {
                                    //[Atualiza interface]
                                    lblStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02 + " " + this.prmDispensa.Colorantes[INDEX_CONTADOR].Nome;
                                    lblSubStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => this.prmDispensa.Dispenser[0].Dispensar(
                                            CIRCUITO - 1,
                                            this.prmDispensa.PulsoHorario[CIRCUITO - 1],
                                            this.prmDispensa.Velocidade[CIRCUITO - 1],
                                            this.prmDispensa.Aceleracao[CIRCUITO - 1],
                                            this.prmDispensa.Delay[CIRCUITO - 1],
                                            this.prmDispensa.PulsoReverso[CIRCUITO - 1],
                                            i_Step)
                                        );

                                    /*
                                    if (_parametros.ControlarNivel)
                                    {
                                        Operar.AbaterColorante(
                                            CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                    }
                                    */
                                    INDEX_CONTADOR++;
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " 1 Simultânea";
                                    ValoresVO[] _valores = new ValoresVO[16];
                                    int i = 0;
                                    for (i = INDEX_CONTADOR; i < this.prmDispensa.Colorantes.Count; i++)
                                    {
                                        CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                        dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                                        if (dispositivo == 1)
                                        {
                                            ValoresVO val = new ValoresVO();
                                            val.PulsoHorario = this.prmDispensa.PulsoHorario[CIRCUITO - 1];
                                            val.PulsoReverso = this.prmDispensa.PulsoReverso[CIRCUITO - 1];
                                            val.Aceleracao = this.prmDispensa.Aceleracao[CIRCUITO - 1];
                                            val.Delay = this.prmDispensa.Delay[CIRCUITO - 1];
                                            val.Velocidade = this.prmDispensa.Velocidade[CIRCUITO - 1];

                                            _valores[CIRCUITO - 1] = val;


                                            /*
                                            if (_parametros.ControlarNivel)
                                            {
                                                Operar.AbaterColorante(
                                                    CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                            }
                                            */
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        INDEX_CONTADOR++;
                                    }
                                    this.prmDispensa.Dispenser[0].Dispensar(_valores,
                                            i_Step);

                                    Thread.Sleep(1000);
                                }
                            }

                        }
                        else
                        {
                            INDEX_CONTADOR++;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                this.counterFalha = 0;
                                if (!this.isNewOperacao)
                                {
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                    this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 17;
                                }
                                else
                                {
                                    PausarMonitoramento();
                                    DialogResult = DialogResult.OK;
                                    Close();
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
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
                        //this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                //m.ShowDialog("Trocar recipiente!");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                this.passosActionP3 = 3;
                            }
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                        {
                            this.passosActionP3 = 1;
                        }

                        break;
                    }
                case 3:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.prmDispensa.modBusDispenser_P3.SensorCopo || !this.prmDispensa.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        lblSubStatus.Text = "";
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        //[Verifica se dispositivo está pronto]
                        if (this.prmDispensa.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //[Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CONTADOR > INDEX_ULTIMO_CIRCUITO)
                        {
                            //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_CIRCUITO; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 1)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso[nCIRCUITO - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume[nCIRCUITO - 1]);
                                }
                                else if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 2)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso2[(nCIRCUITO - 16) - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume2[(nCIRCUITO - 16) - 1]);
                                }
                            }
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            break;
                        }


                        //[Recupera posição do circuito do colorante]
                        int CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                        int dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                        if (dispositivo == 1)
                        {
                            if (this.prmDispensa.PulsoHorario[CIRCUITO - 1] > 0)
                            {
                                if (this.placa1Seq)
                                {
                                    //[Atualiza interface]
                                    lblStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02 + " " + this.prmDispensa.Colorantes[INDEX_CONTADOR].Nome;
                                    lblSubStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => this.prmDispensa.Dispenser[0].Dispensar(
                                            CIRCUITO - 1,
                                            this.prmDispensa.PulsoHorario[CIRCUITO - 1],
                                            this.prmDispensa.Velocidade[CIRCUITO - 1],
                                            this.prmDispensa.Aceleracao[CIRCUITO - 1],
                                            this.prmDispensa.Delay[CIRCUITO - 1],
                                            this.prmDispensa.PulsoReverso[CIRCUITO - 1],
                                            i_Step)
                                        );

                                    /*
                                    if (_parametros.ControlarNivel)
                                    {
                                        Operar.AbaterColorante(
                                            CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                    }
                                    */
                                    INDEX_CONTADOR++;
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " 1 Simultânea";
                                    ValoresVO[] _valores = new ValoresVO[16];
                                    int i = 0;
                                    for (i = INDEX_CONTADOR; i < this.prmDispensa.Colorantes.Count; i++)
                                    {
                                        CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                        dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                                        if (dispositivo == 1)
                                        {
                                            ValoresVO val = new ValoresVO();
                                            val.PulsoHorario = this.prmDispensa.PulsoHorario[CIRCUITO - 1];
                                            val.PulsoReverso = this.prmDispensa.PulsoReverso[CIRCUITO - 1];
                                            val.Aceleracao = this.prmDispensa.Aceleracao[CIRCUITO - 1];
                                            val.Delay = this.prmDispensa.Delay[CIRCUITO - 1];
                                            val.Velocidade = this.prmDispensa.Velocidade[CIRCUITO - 1];

                                            _valores[CIRCUITO - 1] = val;


                                            /*
                                            if (_parametros.ControlarNivel)
                                            {
                                                Operar.AbaterColorante(
                                                    CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                            }
                                            */
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        INDEX_CONTADOR++;
                                    }
                                    this.prmDispensa.Dispenser[0].Dispensar(_valores,
                                            i_Step);

                                    Thread.Sleep(1000);
                                }
                            }

                        }
                        else
                        {
                            INDEX_CONTADOR++;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 =9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                this.counterFalha = 0;
                                if (!this.isNewOperacao)
                                {
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                    this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 17;
                                }
                                else
                                {
                                    PausarMonitoramento();
                                    DialogResult = DialogResult.OK;
                                    Close();
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 2;
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
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
                        //this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
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
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
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
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.prmDispensa.modBusDispenser_P3.SensorCopo || !this.prmDispensa.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        lblSubStatus.Text = "";
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        //[Verifica se dispositivo está pronto]
                        if (this.prmDispensa.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //[Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CONTADOR > INDEX_ULTIMO_CIRCUITO)
                        {
                            //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_CIRCUITO; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 1)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso[nCIRCUITO - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume[nCIRCUITO - 1]);
                                }
                                else if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 2)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso2[(nCIRCUITO - 16) - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume2[(nCIRCUITO - 16) - 1]);
                                }
                            }
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            break;
                        }


                        //[Recupera posição do circuito do colorante]
                        int CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                        int dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                        if (dispositivo == 1)
                        {
                            if (this.prmDispensa.PulsoHorario[CIRCUITO - 1] > 0)
                            {
                                if (this.placa1Seq)
                                {
                                    //[Atualiza interface]
                                    lblStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02 + " " + this.prmDispensa.Colorantes[INDEX_CONTADOR].Nome;
                                    lblSubStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => this.prmDispensa.Dispenser[0].Dispensar(
                                            CIRCUITO - 1,
                                            this.prmDispensa.PulsoHorario[CIRCUITO - 1],
                                            this.prmDispensa.Velocidade[CIRCUITO - 1],
                                            this.prmDispensa.Aceleracao[CIRCUITO - 1],
                                            this.prmDispensa.Delay[CIRCUITO - 1],
                                            this.prmDispensa.PulsoReverso[CIRCUITO - 1],
                                            i_Step)
                                        );

                                    /*
                                    if (_parametros.ControlarNivel)
                                    {
                                        Operar.AbaterColorante(
                                            CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                    }
                                    */
                                    INDEX_CONTADOR++;
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " 1 Simultânea";
                                    ValoresVO[] _valores = new ValoresVO[16];
                                    int i = 0;
                                    for (i = INDEX_CONTADOR; i < this.prmDispensa.Colorantes.Count; i++)
                                    {
                                        CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                        dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                                        if (dispositivo == 1)
                                        {
                                            ValoresVO val = new ValoresVO();
                                            val.PulsoHorario = this.prmDispensa.PulsoHorario[CIRCUITO - 1];
                                            val.PulsoReverso = this.prmDispensa.PulsoReverso[CIRCUITO - 1];
                                            val.Aceleracao = this.prmDispensa.Aceleracao[CIRCUITO - 1];
                                            val.Delay = this.prmDispensa.Delay[CIRCUITO - 1];
                                            val.Velocidade = this.prmDispensa.Velocidade[CIRCUITO - 1];

                                            _valores[CIRCUITO - 1] = val;


                                            /*
                                            if (_parametros.ControlarNivel)
                                            {
                                                Operar.AbaterColorante(
                                                    CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                            }
                                            */
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        INDEX_CONTADOR++;
                                    }
                                    this.prmDispensa.Dispenser[0].Dispensar(_valores,
                                            i_Step);

                                    Thread.Sleep(1000);
                                }
                            }

                        }
                        else
                        {
                            INDEX_CONTADOR++;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                this.counterFalha = 0;
                                if (!this.isNewOperacao)
                                {
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                    this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 17;
                                }
                                else
                                {
                                    PausarMonitoramento();
                                    DialogResult = DialogResult.OK;
                                    Close();
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 3;
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
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
                        //this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
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
                                //this.passosActionP3 = 18;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                                this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 19;
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
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.prmDispensa.modBusDispenser_P3.SensorCopo || !this.prmDispensa.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        lblSubStatus.Text = "";
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        //[Verifica se dispositivo está pronto]
                        if (this.prmDispensa.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //[Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CONTADOR > INDEX_ULTIMO_CIRCUITO)
                        {
                            //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_CIRCUITO; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 1)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso[nCIRCUITO - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume[nCIRCUITO - 1]);
                                }
                                else if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 2)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso2[(nCIRCUITO - 16) - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume2[(nCIRCUITO - 16) - 1]);
                                }
                            }
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            break;
                        }


                        //[Recupera posição do circuito do colorante]
                        int CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                        int dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                        if (dispositivo == 1)
                        {
                            if (this.prmDispensa.PulsoHorario[CIRCUITO - 1] > 0)
                            {
                                if (this.placa1Seq)
                                {
                                    //[Atualiza interface]
                                    lblStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02 + " " + this.prmDispensa.Colorantes[INDEX_CONTADOR].Nome;
                                    lblSubStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => this.prmDispensa.Dispenser[0].Dispensar(
                                            CIRCUITO - 1,
                                            this.prmDispensa.PulsoHorario[CIRCUITO - 1],
                                            this.prmDispensa.Velocidade[CIRCUITO - 1],
                                            this.prmDispensa.Aceleracao[CIRCUITO - 1],
                                            this.prmDispensa.Delay[CIRCUITO - 1],
                                            this.prmDispensa.PulsoReverso[CIRCUITO - 1],
                                            i_Step)
                                        );

                                    /*
                                    if (_parametros.ControlarNivel)
                                    {
                                        Operar.AbaterColorante(
                                            CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                    }
                                    */
                                    INDEX_CONTADOR++;
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " 1 Simultânea";
                                    ValoresVO[] _valores = new ValoresVO[16];
                                    int i = 0;
                                    for (i = INDEX_CONTADOR; i < this.prmDispensa.Colorantes.Count; i++)
                                    {
                                        CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                        dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                                        if (dispositivo == 1)
                                        {
                                            ValoresVO val = new ValoresVO();
                                            val.PulsoHorario = this.prmDispensa.PulsoHorario[CIRCUITO - 1];
                                            val.PulsoReverso = this.prmDispensa.PulsoReverso[CIRCUITO - 1];
                                            val.Aceleracao = this.prmDispensa.Aceleracao[CIRCUITO - 1];
                                            val.Delay = this.prmDispensa.Delay[CIRCUITO - 1];
                                            val.Velocidade = this.prmDispensa.Velocidade[CIRCUITO - 1];

                                            _valores[CIRCUITO - 1] = val;


                                            /*
                                            if (_parametros.ControlarNivel)
                                            {
                                                Operar.AbaterColorante(
                                                    CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                            }
                                            */
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        INDEX_CONTADOR++;
                                    }
                                    this.prmDispensa.Dispenser[0].Dispensar(_valores,
                                            i_Step);

                                    Thread.Sleep(1000);
                                }
                            }

                        }
                        else
                        {
                            INDEX_CONTADOR++;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;

                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                this.counterFalha = 0;
                                if (!this.isNewOperacao)
                                {
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                    this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 17;
                                }
                                else
                                {
                                    PausarMonitoramento();
                                    DialogResult = DialogResult.OK;
                                    Close();
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 2;
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
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
                        //this.passosActionP3 = 1;
                        this.counterFalha = 0;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;

                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.prmDispensa.modBusDispenser_P3.SensorCopo || !this.prmDispensa.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        lblSubStatus.Text = "";
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        //[Verifica se dispositivo está pronto]
                        if (this.prmDispensa.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //[Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CONTADOR > INDEX_ULTIMO_CIRCUITO)
                        {
                            //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_CIRCUITO; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 1)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso[nCIRCUITO - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume[nCIRCUITO - 1]);
                                }
                                else if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 2)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso2[(nCIRCUITO - 16) - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume2[(nCIRCUITO - 16) - 1]);
                                }
                            }
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            break;
                        }


                        //[Recupera posição do circuito do colorante]
                        int CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                        int dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                        if (dispositivo == 1)
                        {
                            if (this.prmDispensa.PulsoHorario[CIRCUITO - 1] > 0)
                            {
                                if (this.placa1Seq)
                                {
                                    //[Atualiza interface]
                                    lblStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02 + " " + this.prmDispensa.Colorantes[INDEX_CONTADOR].Nome;
                                    lblSubStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => this.prmDispensa.Dispenser[0].Dispensar(
                                            CIRCUITO - 1,
                                            this.prmDispensa.PulsoHorario[CIRCUITO - 1],
                                            this.prmDispensa.Velocidade[CIRCUITO - 1],
                                            this.prmDispensa.Aceleracao[CIRCUITO - 1],
                                            this.prmDispensa.Delay[CIRCUITO - 1],
                                            this.prmDispensa.PulsoReverso[CIRCUITO - 1],
                                            i_Step)
                                        );

                                    /*
                                    if (_parametros.ControlarNivel)
                                    {
                                        Operar.AbaterColorante(
                                            CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                    }
                                    */
                                    INDEX_CONTADOR++;
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " 1 Simultânea";
                                    ValoresVO[] _valores = new ValoresVO[16];
                                    int i = 0;
                                    for (i = INDEX_CONTADOR; i < this.prmDispensa.Colorantes.Count; i++)
                                    {
                                        CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                        dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                                        if (dispositivo == 1)
                                        {
                                            ValoresVO val = new ValoresVO();
                                            val.PulsoHorario = this.prmDispensa.PulsoHorario[CIRCUITO - 1];
                                            val.PulsoReverso = this.prmDispensa.PulsoReverso[CIRCUITO - 1];
                                            val.Aceleracao = this.prmDispensa.Aceleracao[CIRCUITO - 1];
                                            val.Delay = this.prmDispensa.Delay[CIRCUITO - 1];
                                            val.Velocidade = this.prmDispensa.Velocidade[CIRCUITO - 1];

                                            _valores[CIRCUITO - 1] = val;


                                            /*
                                            if (_parametros.ControlarNivel)
                                            {
                                                Operar.AbaterColorante(
                                                    CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                            }
                                            */
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        INDEX_CONTADOR++;
                                    }
                                    this.prmDispensa.Dispenser[0].Dispensar(_valores,
                                            i_Step);

                                    Thread.Sleep(1000);
                                }
                            }

                        }
                        else
                        {
                            INDEX_CONTADOR++;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada && this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)
                            {
                                //this.passosActionP3 = 16;
                                this.counterFalha = 0;
                                if (!this.isNewOperacao)
                                {
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                    this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 17;
                                }
                                else
                                {
                                    PausarMonitoramento();
                                    DialogResult = DialogResult.OK;
                                    Close();
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada || !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                        }
                        break;
                    }
                case 17:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada))
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
                        //this.passosActionP3 = 1;
                        this.counterFalha = 0;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;

                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.prmDispensa.modBusDispenser_P3.SensorCopo || !this.prmDispensa.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        lblSubStatus.Text = "";
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        //[Verifica se dispositivo está pronto]
                        if (this.prmDispensa.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //[Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CONTADOR > INDEX_ULTIMO_CIRCUITO)
                        {
                            //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_CIRCUITO; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 1)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso[nCIRCUITO - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume[nCIRCUITO - 1]);
                                }
                                else if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 2)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso2[(nCIRCUITO - 16) - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume2[(nCIRCUITO - 16) - 1]);
                                }
                            }
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            break;
                        }


                        //[Recupera posição do circuito do colorante]
                        int CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                        int dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                        if (dispositivo == 1)
                        {
                            if (this.prmDispensa.PulsoHorario[CIRCUITO - 1] > 0)
                            {
                                if (this.placa1Seq)
                                {
                                    //[Atualiza interface]
                                    lblStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02 + " " + this.prmDispensa.Colorantes[INDEX_CONTADOR].Nome;
                                    lblSubStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => this.prmDispensa.Dispenser[0].Dispensar(
                                            CIRCUITO - 1,
                                            this.prmDispensa.PulsoHorario[CIRCUITO - 1],
                                            this.prmDispensa.Velocidade[CIRCUITO - 1],
                                            this.prmDispensa.Aceleracao[CIRCUITO - 1],
                                            this.prmDispensa.Delay[CIRCUITO - 1],
                                            this.prmDispensa.PulsoReverso[CIRCUITO - 1],
                                            i_Step)
                                        );

                                    /*
                                    if (_parametros.ControlarNivel)
                                    {
                                        Operar.AbaterColorante(
                                            CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                    }
                                    */
                                    INDEX_CONTADOR++;
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " 1 Simultânea";
                                    ValoresVO[] _valores = new ValoresVO[16];
                                    int i = 0;
                                    for (i = INDEX_CONTADOR; i < this.prmDispensa.Colorantes.Count; i++)
                                    {
                                        CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                        dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                                        if (dispositivo == 1)
                                        {
                                            ValoresVO val = new ValoresVO();
                                            val.PulsoHorario = this.prmDispensa.PulsoHorario[CIRCUITO - 1];
                                            val.PulsoReverso = this.prmDispensa.PulsoReverso[CIRCUITO - 1];
                                            val.Aceleracao = this.prmDispensa.Aceleracao[CIRCUITO - 1];
                                            val.Delay = this.prmDispensa.Delay[CIRCUITO - 1];
                                            val.Velocidade = this.prmDispensa.Velocidade[CIRCUITO - 1];

                                            _valores[CIRCUITO - 1] = val;


                                            /*
                                            if (_parametros.ControlarNivel)
                                            {
                                                Operar.AbaterColorante(
                                                    CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                            }
                                            */
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        INDEX_CONTADOR++;
                                    }
                                    this.prmDispensa.Dispenser[0].Dispensar(_valores,
                                            i_Step);

                                    Thread.Sleep(1000);
                                }
                            }

                        }
                        else
                        {
                            INDEX_CONTADOR++;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada && this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)
                            {
                                //this.passosActionP3 = 16;
                                this.counterFalha = 0;
                                if (!this.isNewOperacao)
                                {
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                    this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 17;
                                }
                                else
                                {
                                    PausarMonitoramento();
                                    DialogResult = DialogResult.OK;
                                    Close();
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada || !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                        }
                        break;
                    }
                case 17:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada))
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
                        //if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        //{
                        //    this.passosActionP3 = 2;
                        //}
                        //else
                        //{
                        //    //this.passosActionP3 = 1;
                        //}
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        this.counterFalha = 0;

                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.prmDispensa.modBusDispenser_P3.SensorCopo || !this.prmDispensa.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        lblSubStatus.Text = "";
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        //[Verifica se dispositivo está pronto]
                        if (this.prmDispensa.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //[Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CONTADOR > INDEX_ULTIMO_CIRCUITO)
                        {
                            //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_CIRCUITO; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 1)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso[nCIRCUITO - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume[nCIRCUITO - 1]);
                                }
                                else if (this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo == 2)
                                {
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, this.prmDispensa.PulsoReverso2[(nCIRCUITO - 16) - 1]);
                                    Util.ObjectRecircular.UpdateVolumeDosado(nCIRCUITO, this.prmDispensa.Volume2[(nCIRCUITO - 16) - 1]);
                                }
                            }
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            break;
                        }


                        //[Recupera posição do circuito do colorante]
                        int CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                        int dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                        if (dispositivo == 1)
                        {
                            if (this.prmDispensa.PulsoHorario[CIRCUITO - 1] > 0)
                            {
                                if (this.placa1Seq)
                                {
                                    //[Atualiza interface]
                                    lblStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02 + " " + this.prmDispensa.Colorantes[INDEX_CONTADOR].Nome;
                                    lblSubStatus.Text =
                                    Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                                    //[Dispara thread para enviar dados ao dispositivo]
                                    Task task = Task.Factory.StartNew(() => this.prmDispensa.Dispenser[0].Dispensar(
                                            CIRCUITO - 1,
                                            this.prmDispensa.PulsoHorario[CIRCUITO - 1],
                                            this.prmDispensa.Velocidade[CIRCUITO - 1],
                                            this.prmDispensa.Aceleracao[CIRCUITO - 1],
                                            this.prmDispensa.Delay[CIRCUITO - 1],
                                            this.prmDispensa.PulsoReverso[CIRCUITO - 1],
                                            i_Step)
                                        );

                                    /*
                                    if (_parametros.ControlarNivel)
                                    {
                                        Operar.AbaterColorante(
                                            CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                    }
                                    */
                                    INDEX_CONTADOR++;
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 + " 1 Simultânea";
                                    ValoresVO[] _valores = new ValoresVO[16];
                                    int i = 0;
                                    for (i = INDEX_CONTADOR; i < this.prmDispensa.Colorantes.Count; i++)
                                    {
                                        CIRCUITO = this.prmDispensa.Colorantes[INDEX_CONTADOR].Circuito;
                                        dispositivo = this.prmDispensa.Colorantes[INDEX_CONTADOR].Dispositivo;
                                        if (dispositivo == 1)
                                        {
                                            ValoresVO val = new ValoresVO();
                                            val.PulsoHorario = this.prmDispensa.PulsoHorario[CIRCUITO - 1];
                                            val.PulsoReverso = this.prmDispensa.PulsoReverso[CIRCUITO - 1];
                                            val.Aceleracao = this.prmDispensa.Aceleracao[CIRCUITO - 1];
                                            val.Delay = this.prmDispensa.Delay[CIRCUITO - 1];
                                            val.Velocidade = this.prmDispensa.Velocidade[CIRCUITO - 1];

                                            _valores[CIRCUITO - 1] = val;


                                            /*
                                            if (_parametros.ControlarNivel)
                                            {
                                                Operar.AbaterColorante(
                                                    CIRCUITO, prmDispensa.Demanda[CIRCUITO]);
                                            }
                                            */
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        INDEX_CONTADOR++;
                                    }
                                    this.prmDispensa.Dispenser[0].Dispensar(_valores,
                                            i_Step);

                                    Thread.Sleep(1000);
                                }
                            }

                        }
                        else
                        {
                            INDEX_CONTADOR++;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        this.lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                !this.prmDispensa.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorEsponja && !this.prmDispensa.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.prmDispensa.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada && this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos)
                            {
                                //this.passosActionP3 = 16;
                                this.counterFalha = 0;
                                if (!this.isNewOperacao)
                                {
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                    this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 17;
                                }
                                else
                                {
                                    PausarMonitoramento();
                                    DialogResult = DialogResult.OK;
                                    Close();
                                }
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada || !this.prmDispensa.modBusDispenser_P3.SensorBaixoBicos))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                        }
                        break;
                    }
                case 17:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.prmDispensa.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) && !this.prmDispensa.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.prmDispensa.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.prmDispensa.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.prmDispensa.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.prmDispensa.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.prmDispensa.modBusDispenser_P3.IsNativo == 0 || this.prmDispensa.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.prmDispensa.modBusDispenser_P3.SensorGavetaFechada))
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
            if (this.prmDispensa.modBusDispenser_P3.SensorEmergencia)
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
            else if (this.prmDispensa.modBusDispenser_P3.CodError > 0)
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
                    //string msg = "Usuário ocorreu algum erro na movimentação da placa. Deseja continuar este processo?" + Environment.NewLine + "Descrição: " + this.prmDispensa.modBusDispenser_P3.GetDescCodError();
                    string msg = Negocio.IdiomaResxExtensao.PlacaMov_Erro_Movimentacao_Passos + this.prmDispensa.modBusDispenser_P3.GetDescCodError();
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
    }
}
