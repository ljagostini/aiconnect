using Percolore.Core;
using Percolore.Core.Logging;
using Percolore.IOConnect.Modbus;
using System.ComponentModel;

namespace Percolore.IOConnect
{
	public partial class fRecircularAuto : Form
    {
        Util.ObjectParametros _parametros = null;
        RecircularAutoVO _prmRecircular;
        int INDEX_CONTADOR;
        int INDEX_ULTIMO_COLORANTE = 0;        
        bool desativarUI;

        System.ComponentModel.BackgroundWorker backgroundWorker1 = null;
        private bool isThread = false;
        private bool isRunning = false;
        private int counterFalha = 0;
        // 
        private int tipoActionP3 = -1;
        private int passosActionP3 = 0;
        private List<Util.ObjectRecircular> listaRecircular = null;

        private int i_Step = 0;

        private int passosActionP2 = 0;

        public fRecircularAuto(RecircularAutoVO parametrosRecircular, List<Util.ObjectRecircular> lRecAuto, bool desativarUI = false )
        {
            InitializeComponent();
            this.listaRecircular = lRecAuto;
            this._prmRecircular = parametrosRecircular;
            this._parametros = Util.ObjectParametros.Load();

            /* Se a exibição da interface for desabilitada
             * não é necessário configurar propriedades dos controles */
            this.desativarUI = desativarUI;
            if (this.desativarUI)
            {
                return;
            }

            //Redimensiona e posiciona form
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Location = new Point(0, 30);

            //Globalização
            lblStatus.Text = Negocio.IdiomaResxExtensao.RecirculacaoAuto_Titulo /*"Recirculação Automática"*/;
            lblSubStatus.Text = "";
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
        
        #region Eventos

        private void RecircularAuto_Load(object sender, EventArgs e)
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
            Iniciar_Click(sender, e);
        }

        void Iniciar_Click(object sender, EventArgs e)
        {
            try
            {
                progressBar.Visible = true;
                btnIniciar.Enabled = false;
                btnCancelar.Visible = false;
                btnAbortar.Visible = true;

                //Inicializa variáveis de monitoramento
                INDEX_CONTADOR = 0;
                INDEX_ULTIMO_COLORANTE = _prmRecircular.LMDispositivos[0].Colorantes.Count - 1;

                this.counterFalha = 0;              
                this.tipoActionP3 = -1;
                this.passosActionP3 = 0;
               
                bool isPosicao = false;
                bool isEmergencia = false;
                if (_prmRecircular.DispenserP3 == null)
                {
                    this.tipoActionP3 = 1;
                }
                else
                {
                    _prmRecircular.DispenserP3.ReadSensores_Mover();
                    isEmergencia = _prmRecircular.DispenserP3.SensorEmergencia;
                    if (!isEmergencia)
                    {
                        if (!(_prmRecircular.DispenserP3.IsNativo == 0 || _prmRecircular.DispenserP3.IsNativo == 2))
                        {
                            //lblStatus.Text = "Condição de placa movimentação incorreta!";
                            lblStatus.Text = Negocio.IdiomaResxExtensao.PlacaMov_Condicao_Incorreta;
                            lblSubStatus.Text = "";
                            bool confirma = false;
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                            {
                                confirma = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Condicao_Incorreta, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true, 30);
                            }
                            if (confirma)
                            {
                                this.DialogResult = DialogResult.Cancel;
                                Close();
                            }
                            return;
                        }

                        if (!_prmRecircular.DispenserP3.SensorValvulaDosagem && !_prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            _prmRecircular.DispenserP3.ValvulaPosicaoRecirculacao();
                            Thread.Sleep(1000);
                            isPosicao = true;
                        }
                    }
                    else
                    {
                        isPosicao = true;
                    }

                    if (isPosicao)
                    {
                        _prmRecircular.DispenserP3.ReadSensores_Mover();
                        if ((!_prmRecircular.DispenserP3.SensorValvulaDosagem && !_prmRecircular.DispenserP3.SensorValvulaRecirculacao) || _prmRecircular.DispenserP3.SensorEmergencia)
                        {
                            //lblStatus.Text = "Condição de placa movimentação incorreta!";
                            lblStatus.Text = Negocio.IdiomaResxExtensao.PlacaMov_Condicao_Incorreta;
                            lblSubStatus.Text = "";
                            if (_prmRecircular.DispenserP3.SensorEmergencia)
                            {
                                bool confirma = false;
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    confirma = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Condicao_Incorreta, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true, 30);
                                }
                                if (confirma)
                                {
                                    this.DialogResult = DialogResult.Cancel;
                                    Close();
                                }
                            }
                            else
                            {
                                bool confirma = false;
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //confirma = m.ShowDialog("Condição de placa movimentação incorreta", Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true, 30);
                                    confirma = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Condicao_Incorreta, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true, 30);

                                }
                                if (confirma)
                                {
                                    this.DialogResult = DialogResult.Cancel;
                                    Close();
                                }
                            }
                            return;
                        }

                    }

                    this.tipoActionP3 = getActionP3();
                }                
                

                if (this.tipoActionP3 >= 0)
                {
                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.RecirculacaoAuto_SendoRealizada/*"Recirculação Automática Simultânea sendo realizada!"*/;
                    ExecutarMonitoramento();
                    Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.RecirculacaoAuto_Iniciada/*"Recirculação Automática Simultânea iniciada"*/);
                }
                else
                {
                    //lblStatus.Text = "Condição de placa movimentação incorreta!";
                    lblStatus.Text = Negocio.IdiomaResxExtensao.PlacaMov_Condicao_Incorreta;
                    lblSubStatus.Text = "";
                    progressBar.Visible = false;
                    btnIniciar.Enabled = true;
                    btnCancelar.Visible = true;
                    btnAbortar.Visible = false;
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
            if (_prmRecircular.DispenserP3.SensorEsponja && _prmRecircular.DispenserP3.SensorCopo && !_prmRecircular.DispenserP3.SensorBaixoBicos && _prmRecircular.DispenserP3.SensorAltoBicos
                && !_prmRecircular.DispenserP3.SensorGavetaAberta && _prmRecircular.DispenserP3.SensorGavetaFechada && _prmRecircular.DispenserP3.SensorValvulaDosagem
                && !_prmRecircular.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 1;
            }
            else if (_prmRecircular.DispenserP3.SensorEsponja && _prmRecircular.DispenserP3.SensorCopo && !_prmRecircular.DispenserP3.SensorBaixoBicos && _prmRecircular.DispenserP3.SensorAltoBicos
                && !_prmRecircular.DispenserP3.SensorGavetaAberta && _prmRecircular.DispenserP3.SensorGavetaFechada && !_prmRecircular.DispenserP3.SensorValvulaDosagem
                && _prmRecircular.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 2;
            }
            else if (_prmRecircular.DispenserP3.SensorEsponja && _prmRecircular.DispenserP3.SensorCopo && !_prmRecircular.DispenserP3.SensorBaixoBicos && _prmRecircular.DispenserP3.SensorAltoBicos
                && _prmRecircular.DispenserP3.SensorGavetaAberta && !_prmRecircular.DispenserP3.SensorGavetaFechada && _prmRecircular.DispenserP3.SensorValvulaDosagem
                && !_prmRecircular.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 3;
            }
            else if (_prmRecircular.DispenserP3.SensorEsponja && _prmRecircular.DispenserP3.SensorCopo && !_prmRecircular.DispenserP3.SensorBaixoBicos && _prmRecircular.DispenserP3.SensorAltoBicos
                && _prmRecircular.DispenserP3.SensorGavetaAberta && !_prmRecircular.DispenserP3.SensorGavetaFechada && !_prmRecircular.DispenserP3.SensorValvulaDosagem
                && _prmRecircular.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 4;
            }
            else if (_prmRecircular.DispenserP3.SensorEsponja && !_prmRecircular.DispenserP3.SensorCopo && _prmRecircular.DispenserP3.SensorBaixoBicos && !_prmRecircular.DispenserP3.SensorAltoBicos
                && !_prmRecircular.DispenserP3.SensorGavetaAberta && _prmRecircular.DispenserP3.SensorGavetaFechada && !_prmRecircular.DispenserP3.SensorValvulaDosagem
                && _prmRecircular.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 5;
            }
            else if (_prmRecircular.DispenserP3.SensorEsponja && !_prmRecircular.DispenserP3.SensorCopo && !_prmRecircular.DispenserP3.SensorBaixoBicos && _prmRecircular.DispenserP3.SensorAltoBicos
                && !_prmRecircular.DispenserP3.SensorGavetaAberta && _prmRecircular.DispenserP3.SensorGavetaFechada && !_prmRecircular.DispenserP3.SensorValvulaDosagem
                && _prmRecircular.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 6;
            }
            else if (_prmRecircular.DispenserP3.SensorEsponja && !_prmRecircular.DispenserP3.SensorCopo && !_prmRecircular.DispenserP3.SensorBaixoBicos && _prmRecircular.DispenserP3.SensorAltoBicos
                && _prmRecircular.DispenserP3.SensorGavetaAberta && !_prmRecircular.DispenserP3.SensorGavetaFechada && !_prmRecircular.DispenserP3.SensorValvulaDosagem
                && _prmRecircular.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 7;
            }
            else if (!_prmRecircular.DispenserP3.SensorEsponja && !_prmRecircular.DispenserP3.SensorCopo && !_prmRecircular.DispenserP3.SensorBaixoBicos && _prmRecircular.DispenserP3.SensorAltoBicos
               && !_prmRecircular.DispenserP3.SensorGavetaAberta && _prmRecircular.DispenserP3.SensorGavetaFechada && _prmRecircular.DispenserP3.SensorValvulaDosagem
               && !_prmRecircular.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 8;
            }
            else if (!_prmRecircular.DispenserP3.SensorEsponja && !_prmRecircular.DispenserP3.SensorCopo && !_prmRecircular.DispenserP3.SensorBaixoBicos && _prmRecircular.DispenserP3.SensorAltoBicos
               && !_prmRecircular.DispenserP3.SensorGavetaAberta && _prmRecircular.DispenserP3.SensorGavetaFechada && !_prmRecircular.DispenserP3.SensorValvulaDosagem
               && _prmRecircular.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 9;
            }
            else if (!_prmRecircular.DispenserP3.SensorEsponja && !_prmRecircular.DispenserP3.SensorCopo && !_prmRecircular.DispenserP3.SensorBaixoBicos && _prmRecircular.DispenserP3.SensorAltoBicos
               && _prmRecircular.DispenserP3.SensorGavetaAberta && !_prmRecircular.DispenserP3.SensorGavetaFechada && _prmRecircular.DispenserP3.SensorValvulaDosagem
               && !_prmRecircular.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 10;
            }
            else if (!_prmRecircular.DispenserP3.SensorEsponja && !_prmRecircular.DispenserP3.SensorCopo && !_prmRecircular.DispenserP3.SensorBaixoBicos && _prmRecircular.DispenserP3.SensorAltoBicos
               && _prmRecircular.DispenserP3.SensorGavetaAberta && !_prmRecircular.DispenserP3.SensorGavetaFechada && !_prmRecircular.DispenserP3.SensorValvulaDosagem
               && _prmRecircular.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 11;
            }
            else if (_prmRecircular.DispenserP3.SensorEsponja && !_prmRecircular.DispenserP3.SensorCopo && _prmRecircular.DispenserP3.SensorBaixoBicos &&
             !_prmRecircular.DispenserP3.SensorAltoBicos && !_prmRecircular.DispenserP3.SensorGavetaAberta && _prmRecircular.DispenserP3.SensorGavetaFechada &&
             _prmRecircular.DispenserP3.SensorValvulaDosagem && !_prmRecircular.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 14;
            }
            else if (_prmRecircular.DispenserP3.SensorEsponja && !_prmRecircular.DispenserP3.SensorCopo && !_prmRecircular.DispenserP3.SensorBaixoBicos &&
              _prmRecircular.DispenserP3.SensorAltoBicos && !_prmRecircular.DispenserP3.SensorGavetaAberta && _prmRecircular.DispenserP3.SensorGavetaFechada &&
              _prmRecircular.DispenserP3.SensorValvulaDosagem && !_prmRecircular.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 15;
            }
            else if (_prmRecircular.DispenserP3.SensorEsponja && !_prmRecircular.DispenserP3.SensorCopo && !_prmRecircular.DispenserP3.SensorBaixoBicos &&
              _prmRecircular.DispenserP3.SensorAltoBicos && _prmRecircular.DispenserP3.SensorGavetaAberta && !_prmRecircular.DispenserP3.SensorGavetaFechada &&
              _prmRecircular.DispenserP3.SensorValvulaDosagem && !_prmRecircular.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 16;
            }
            else if (_prmRecircular.DispenserP3.SensorBaixoBicos && !_prmRecircular.DispenserP3.SensorAltoBicos
              && _prmRecircular.DispenserP3.SensorGavetaAberta && !_prmRecircular.DispenserP3.SensorGavetaFechada && _prmRecircular.DispenserP3.SensorValvulaDosagem
              && !_prmRecircular.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 12;
            }
            else if (_prmRecircular.DispenserP3.SensorBaixoBicos && !_prmRecircular.DispenserP3.SensorAltoBicos
              && _prmRecircular.DispenserP3.SensorGavetaAberta && !_prmRecircular.DispenserP3.SensorGavetaFechada && !_prmRecircular.DispenserP3.SensorValvulaDosagem
              && _prmRecircular.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 13;
            }
            

            return retorno;
        }
        
        void Abortar_Click(object sender, EventArgs e)
        {
            PausarMonitoramento();

            //Interrompe e retoma operação da placa
            try
            {
                if (this._prmRecircular.DispenserP3 == null)
                {
                    for (int i = 0; i < this._prmRecircular.Dispenser.Count; i++)
                    {
                        this._prmRecircular.Dispenser[i].Halt();
                        this._prmRecircular.Dispenser[i].UnHalt();
                        this._prmRecircular.Dispenser[i].AcionaValvulas(false);
                    }
                }
                else
                {
                    this._prmRecircular.DispenserP3.Halt();
                    this._prmRecircular.DispenserP3.UnHalt();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			//Define status da operação
			DialogResult = DialogResult.Abort;
        }

        private void Cancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void RecircularAuto_Closed(object sender, FormClosedEventArgs e)
        {
            PausarMonitoramento();

            switch (this.DialogResult)
            {
                #region Log de processo

                case DialogResult.OK:
                    {
                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, new string[] { Negocio.IdiomaResxExtensao.RecirculacaoAuto_Concluido /* "Recirculação Automática concluida"*/ });

                        //Log.Logar(
                        //    TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_15 + Negocio.IdiomaResxExtensao.Global_PurgaConcluida);

                        ////Persiste data e hora de execução da purga
                        //Util.ObjectParametros.SetExecucaoPurga(DateTime.Now);
                        break;
                    }
                case DialogResult.Cancel:
                    {
                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, new string[] { Negocio.IdiomaResxExtensao.RecirculacaoAuto_Cancelado  /* "Recirculação Automática cancelada"*/ });

                        //DateTime dtAgora = DateTime.Now.AddMinutes(-1 * ((_parametros.DelayNotificacaotRecirculacaoAuto*80) / 100 ));
                        ////Aqui vamos setar o novo valor do ultimosPulsposRevereso
                        //for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                        //{
                        //    int nCIRCUITO = _prmRecircular.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                        //    //Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmRecircular.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                        //    Util.ObjectRecircular objRecAuto = this.listaRecircular.Find(o => o.Circuito == nCIRCUITO - 1);
                        //    if (objRecAuto != null)
                        //    {
                        //        objRecAuto.DtInicio = dtAgora;
                        //        objRecAuto.VolumeDosado = objRecAuto.VolumeRecircular;
                        //        Util.ObjectRecircular.Persist(objRecAuto);
                        //    }
                        //}
                        //Log.Logar(
                        //    TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_16 + Negocio.IdiomaResxExtensao.Global_PurgaCancelada);
                        break;
                    }
                case DialogResult.Abort:
                    {
                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, new string[] { Negocio.IdiomaResxExtensao.RecirculacaoAuto_Abortada /*"Recirculação Automática abortada" */});
                        //Log.Logar(
                        //    TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_17 + Negocio.IdiomaResxExtensao.Global_PurgaAbortada);
                        break;
                    }
                case DialogResult.No:
                    {
                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, new string[] { Negocio.IdiomaResxExtensao.RecirculacaoAuto_Falha /*"Recirculação Automática falha"*/ });
                        //Log.Logar(
                        //    TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_18 + Negocio.IdiomaResxExtensao.Global_FalhaPurgar);
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
            
            try
            {
                if (this._prmRecircular.DispenserP3 == null)
                {
                    for (int i = 0; i < this._prmRecircular.Dispenser.Count; i++)
                    {
                        this._prmRecircular.Dispenser[i].Halt();
                        this._prmRecircular.Dispenser[i].UnHalt();
                        this._prmRecircular.Dispenser[i].AcionaValvulas(false);
                    }
                }
                else
                {
                    this._prmRecircular.DispenserP3.Halt();
                    this._prmRecircular.DispenserP3.UnHalt();
                }
            }
			catch (Exception exc)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", exc);
			}

			using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
            {
                m.ShowDialog(
                    string.IsNullOrWhiteSpace(customMessage) ? Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace
                                                             : customMessage);
            }

            DialogResult = DialogResult.No;
        }

        void Emergencia()
        {
            PausarMonitoramento();
            try
            {
                this._prmRecircular.DispenserP3.Halt();
                this._prmRecircular.DispenserP3.UnHalt();
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
                this._prmRecircular.DispenserP3.Halt();
                this._prmRecircular.DispenserP3.UnHalt();
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

        void MonitoramentoEvent()
        {
            try
            {
                if (_prmRecircular.DispenserP3 != null)
                { 
                    trataActionP3();
                    this.counterFalha = 0;
                }
                else
                {
                    if (passosActionP2 == 0)
                    {
                        if (counterFalha < 3)
                        {
                            if (_prmRecircular.Dispenser.Count > 0)
                            {
                                if (_prmRecircular.LMDispositivos[0].NrCircuitosValve != null && _prmRecircular.LMDispositivos[0].NrCircuitosValve.Count > 0)
                                {
                                    _prmRecircular.Dispenser[0].AcionaValvulasRecirculacao(_prmRecircular.LMDispositivos[0].NrCircuitosValve);
                                }
                                if (_prmRecircular.Dispenser.Count > 1)
                                {
                                    if (_prmRecircular.LMDispositivos[1].NrCircuitosValve != null && _prmRecircular.LMDispositivos[1].NrCircuitosValve.Count > 0)
                                    {
                                        _prmRecircular.Dispenser[1].AcionaValvulasRecirculacao(_prmRecircular.LMDispositivos[1].NrCircuitosValve);
                                    }
                                }
                            }
                            passosActionP2 = 1;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.Cancel;
                            Close();
                            return;
                        }
                    }
                    else if (passosActionP2 == 1)
                    {
                        Thread.Sleep(1000);
                        bool allCircuitosPosRecirc = isValvulaRecP2();
                        if (allCircuitosPosRecirc)
                        {
                            passosActionP2 = 2;
                        }
                        else
                        {
                            passosActionP2 = 0;
                            counterFalha++;
                        }
                    }
                    else if (passosActionP2 == 2)
                    {
                        this.counterFalha = 0;
                        ValoresVO[] vValores = new ValoresVO[16];
                        ValoresVO[] vValores_2 = new ValoresVO[16];

                        if (/*(Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_4 ||*/ (Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador)
                        {
                            vValores = new ValoresVO[24];
                            for (int i = 0; i < 24; i++)
                            {
                                ValoresVO vo = new ValoresVO();
                                vo.Aceleracao = _prmRecircular.LMDispositivos[0].Aceleracao[i];
                                vo.Delay = _prmRecircular.LMDispositivos[0].Delay[i];
                                vo.PulsoReverso = _prmRecircular.LMDispositivos[0].PulsoReverso[i];
                                vo.PulsoHorario = _prmRecircular.LMDispositivos[0].PulsoHorario[i];
                                vo.Velocidade = _prmRecircular.LMDispositivos[0].Velocidade[i];

                                vValores[i] = vo;

                            }
                        }
                        else
                        {
                            for (int i = 0; i < 16; i++)
                            {
                                ValoresVO vo = new ValoresVO();
                                vo.Aceleracao = _prmRecircular.LMDispositivos[0].Aceleracao[i];
                                vo.Delay = _prmRecircular.LMDispositivos[0].Delay[i];
                                vo.PulsoReverso = _prmRecircular.LMDispositivos[0].PulsoReverso[i];
                                vo.PulsoHorario = _prmRecircular.LMDispositivos[0].PulsoHorario[i];
                                vo.Velocidade = _prmRecircular.LMDispositivos[0].Velocidade[i];

                                vValores[i] = vo;

                            }
                        }

                        if (_prmRecircular.Dispenser.Count > 1)
                        {
                            

                            if (/*(Dispositivo)_parametros.IdDispositivo2 == Dispositivo.Placa_4 || */(Dispositivo)_parametros.IdDispositivo2 == Dispositivo.Simulador)
                            {
                                vValores_2 = new ValoresVO[24];
                                for (int i = 0; i < 24; i++)
                                {
                                    ValoresVO vo = new ValoresVO();
                                    vo.Aceleracao = _prmRecircular.LMDispositivos[1].Aceleracao[i];
                                    vo.Delay = _prmRecircular.LMDispositivos[1].Delay[i];
                                    vo.PulsoReverso = _prmRecircular.LMDispositivos[1].PulsoReverso[i];
                                    vo.PulsoHorario = _prmRecircular.LMDispositivos[1].PulsoHorario[i];
                                    vo.Velocidade = _prmRecircular.LMDispositivos[1].Velocidade[i];

                                    vValores_2[i] = vo;

                                }
                            }
                            else
                            {
                                for (int i = 0; i < 16; i++)
                                {
                                    ValoresVO vo = new ValoresVO();
                                    vo.Aceleracao = _prmRecircular.LMDispositivos[1].Aceleracao[i];
                                    vo.Delay = _prmRecircular.LMDispositivos[1].Delay[i];
                                    vo.PulsoReverso = _prmRecircular.LMDispositivos[1].PulsoReverso[i];
                                    vo.PulsoHorario = _prmRecircular.LMDispositivos[1].PulsoHorario[i];
                                    vo.Velocidade = _prmRecircular.LMDispositivos[1].Velocidade[i];

                                    vValores_2[i] = vo;

                                }
                            }
                        }

                         
                        if (_prmRecircular.Dispenser.Count > 0)
                        {
                            _prmRecircular.Dispenser[0].Dispensar(vValores, i_Step);
                        }
                        else if (_prmRecircular.Dispenser.Count > 1)
                        {
                            _prmRecircular.Dispenser[1].Dispensar(vValores_2, i_Step);
                        }

                        passosActionP2 = 3;


                    }
                    else if (passosActionP2 == 3)
                    {
                        bool isReady = false;
                        foreach(IDispenser pD in _prmRecircular.Dispenser)
                        {
                            isReady = pD.IsReady;
                        }
                        //if ((_prmRecircular.Dispenser.Count > 0 && _prmRecircular.Dispenser[0].IsReady) || 
                        //    (_prmRecircular.Dispenser.Count > 1 && _prmRecircular.Dispenser[1].IsReady))
                            
                        if(isReady)
                        {
                            Thread.Sleep(2000);
                            foreach (IDispenser pD in _prmRecircular.Dispenser)
                            {
                                pD.AcionaValvulas(false);
                            }
                            //if (_prmRecircular.Dispenser.Count > 0)
                            //{
                            //    _prmRecircular.Dispenser[0].AcionaValvulas(false);
                            //}
                            //else if (_prmRecircular.Dispenser.Count > 1)
                            //{
                            //    _prmRecircular.Dispenser[0].AcionaValvulas(false);
                            //}
                            //DateTime dtAgora = DateTime.Now;
                            ////Aqui vamos setar o novo valor do ultimosPulsposRevereso
                            //for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                            //{
                            //    int nCIRCUITO = _prmRecircular.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                            //    //Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmRecircular.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                            //    Util.ObjectRecircular objRecAuto = this.listaRecircular.Find(o => o.Circuito == nCIRCUITO - 1);
                            //    if (objRecAuto != null)
                            //    {
                            //        objRecAuto.DtInicio = dtAgora;
                            //        objRecAuto.VolumeDosado = objRecAuto.VolumeRecircular;
                            //        Util.ObjectRecircular.Persist(objRecAuto);
                            //    }
                            //}
                            DateTime dtAgora = DateTime.Now;
                            if (_prmRecircular.Dispenser.Count > 0)
                            {
                                if (_prmRecircular.LMDispositivos[0].NrCircuitosValve != null && _prmRecircular.LMDispositivos[0].NrCircuitosValve.Count > 0)
                                {
                                    //_prmRecircular.Dispenser[0].AcionaValvulasRecirculacao(_prmRecircular.LMDispositivos[0].NrCircuitosValve);
                                    foreach(int nCIRCUITO in _prmRecircular.LMDispositivos[0].NrCircuitosValve)
                                    {
                                        Util.ObjectRecircular objRecAuto = this.listaRecircular.Find(o => o.Circuito == nCIRCUITO);
                                        if (objRecAuto != null)                                        
                                        {
                                            objRecAuto.DtInicio = dtAgora;
                                            objRecAuto.VolumeDosado = 0;
                                            Util.ObjectRecircular.Persist(objRecAuto);
                                            
                                        }
                                    }
                                }
                                if (_prmRecircular.Dispenser.Count > 1)
                                {
                                    if (_prmRecircular.LMDispositivos[1].NrCircuitosValve != null && _prmRecircular.LMDispositivos[1].NrCircuitosValve.Count > 0)
                                    {                                        
                                        foreach (int nCIRCUITO in _prmRecircular.LMDispositivos[1].NrCircuitosValve)
                                        {
                                            Util.ObjectRecircular objRecAuto = this.listaRecircular.Find(o => o.Circuito == nCIRCUITO);
                                            if (objRecAuto != null)
                                            {
                                                objRecAuto.DtInicio = dtAgora;
                                                objRecAuto.VolumeDosado = 0;
                                                Util.ObjectRecircular.Persist(objRecAuto);

                                            }
                                        }
                                    }
                                }
                            }

                            PausarMonitoramento();
                            Thread.Sleep(500);
                            DialogResult = DialogResult.OK;
                            Close();
                            return;
                        }
                        Thread.Sleep(1000);
                    }
                }
                this.isRunning = false;
                //this.counterFalha = 0;
                ////Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                if (this.counterFalha > 0)
                {
					string customMessage = string.Empty;
					if (ex.Message.Contains("Could not read status register:"))
						customMessage = Negocio.IdiomaResxExtensao.Global_Falha_PerdaConexaoDispositivo;

					Falha(ex, customMessage);
                }
                this.isRunning = false;
                this.counterFalha++;
            }
        }

        bool isValvulaRecP2()
        {
            bool allCircuitosPosRecirc = true;
            if (_prmRecircular.Dispenser.Count > 0)
            {
                if (_prmRecircular.LMDispositivos[0].NrCircuitosValve != null && _prmRecircular.LMDispositivos[0].NrCircuitosValve.Count > 0)
                {
                    StatusValvulas stVal = _prmRecircular.Dispenser[0].getStatusValvulas();

                    foreach (int _circuito in _prmRecircular.LMDispositivos[0].NrCircuitosValve)
                    {
                        switch (_circuito)
                        {
                            case 1:
                                {
                                    if (!stVal.Circuito_1)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    if (!stVal.Circuito_2)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    if (!stVal.Circuito_3)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 4:
                                {
                                    if (!stVal.Circuito_4)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 5:
                                {
                                    if (!stVal.Circuito_5)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 6:
                                {
                                    if (!stVal.Circuito_6)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 7:
                                {
                                    if (!stVal.Circuito_7)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 8:
                                {
                                    if (!stVal.Circuito_8)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 9:
                                {
                                    if (!stVal.Circuito_9)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 10:
                                {
                                    if (!stVal.Circuito_10)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 11:
                                {
                                    if (!stVal.Circuito_11)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 12:
                                {
                                    if (!stVal.Circuito_12)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 13:
                                {
                                    if (!stVal.Circuito_13)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 14:
                                {
                                    if (!stVal.Circuito_14)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 15:
                                {
                                    if (!stVal.Circuito_15)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 16:
                                {
                                    if (!stVal.Circuito_16)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            default:
                                {
                                    allCircuitosPosRecirc = false;
                                    break;
                                }
                        }
                        if (!allCircuitosPosRecirc)
                        {
                            break;
                        }

                    }
                }
                
            }

            if (_prmRecircular.Dispenser.Count > 1)
            {
                if (_prmRecircular.LMDispositivos[1].NrCircuitosValve != null && _prmRecircular.LMDispositivos[1].NrCircuitosValve.Count > 0)
                {
                    StatusValvulas stVal = _prmRecircular.Dispenser[1].getStatusValvulas();

                    foreach (int _circuito in _prmRecircular.LMDispositivos[1].NrCircuitosValve)
                    {
                        switch (_circuito)
                        {
                            case 1:
                                {
                                    if (!stVal.Circuito_1)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    if (!stVal.Circuito_2)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    if (!stVal.Circuito_3)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 4:
                                {
                                    if (!stVal.Circuito_4)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 5:
                                {
                                    if (!stVal.Circuito_5)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 6:
                                {
                                    if (!stVal.Circuito_6)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 7:
                                {
                                    if (!stVal.Circuito_7)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 8:
                                {
                                    if (!stVal.Circuito_8)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 9:
                                {
                                    if (!stVal.Circuito_9)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 10:
                                {
                                    if (!stVal.Circuito_10)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 11:
                                {
                                    if (!stVal.Circuito_11)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 12:
                                {
                                    if (!stVal.Circuito_12)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 13:
                                {
                                    if (!stVal.Circuito_13)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 14:
                                {
                                    if (!stVal.Circuito_14)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 15:
                                {
                                    if (!stVal.Circuito_15)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 16:
                                {
                                    if (!stVal.Circuito_16)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 17:
                                {
                                    if (!stVal.Circuito_17)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 18:
                                {
                                    if (!stVal.Circuito_18)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 19:
                                {
                                    if (!stVal.Circuito_19)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 20:
                                {
                                    if (!stVal.Circuito_20)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 21:
                                {
                                    if (!stVal.Circuito_21)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 22:
                                {
                                    if (!stVal.Circuito_22)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 23:
                                {
                                    if (!stVal.Circuito_23)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            case 24:
                                {
                                    if (!stVal.Circuito_24)
                                    {
                                        allCircuitosPosRecirc = false;
                                    }
                                    break;
                                }
                            default:
                                {
                                    allCircuitosPosRecirc = false;
                                    break;
                                }
                        }
                        if (!allCircuitosPosRecirc)
                        {
                            break;
                        }

                    }
                }
            }
            return allCircuitosPosRecirc;
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

				string customMessage = string.Empty;
				if (ex.Message.Contains("Could not read status register:"))
					customMessage = Negocio.IdiomaResxExtensao.Global_Falha_PerdaConexaoDispositivo;

                Falha(ex, customMessage);
			}
		}

        void trataPassosAction_01()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {   
                        this.passosActionP3 = 1;
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoRecirculacao();
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 3;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) && !this._prmRecircular.DispenserP3.SensorValvulaRecirculacao )
                        {
                            this.passosActionP3 = 1;
                        }
                        
                        break;
                    }
                case 3:
                    {                        
                        ValoresVO[] vValores = new ValoresVO[16];

                        for (int i = 0; i < 16; i++)
                        {
                            ValoresVO vo = new ValoresVO();
                            vo.Aceleracao = _prmRecircular.LMDispositivos[0].Aceleracao[i];
                            vo.Delay = _prmRecircular.LMDispositivos[0].Delay[i];
                            vo.PulsoReverso = 0;
                            vo.PulsoHorario = _prmRecircular.LMDispositivos[0].PulsoHorario[i];
                            vo.Velocidade = _prmRecircular.LMDispositivos[0].Velocidade[i];

                            vValores[i] = vo;

                        }
                        this._prmRecircular.DispenserP3.Dispensar(vValores, i_Step);

                        this.passosActionP3 = 4;
                        break;
                    }
                //tratar o termino da dispesna 
                case 4:
                    {
                        if (_prmRecircular.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmRecircular.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmRecircular.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        
                       
                        DateTime dtAgora = DateTime.Now;
                        //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                        for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                        {
                            int nCIRCUITO = _prmRecircular.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                            //Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmRecircular.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                            Util.ObjectRecircular objRecAuto =  this.listaRecircular.Find(o => o.Circuito == nCIRCUITO - 1);
                            if(objRecAuto != null)
                            {                                
                                objRecAuto.DtInicio = dtAgora;
                                objRecAuto.VolumeDosado = objRecAuto.VolumeRecircular;
                                Util.ObjectRecircular.Persist(objRecAuto);
                            }
                        }
                        this.passosActionP3 = 5;
                        this.counterFalha = 0;
                        break;
                    }
                //Dosado agora é movimentar Valvula para Dosagem
                case 5:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoDosagem();                        
                        this.passosActionP3 = 6;
                        break;
                    }
                case 6:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(6, 5))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 7;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            !this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 5;
                        }
                      
                        break;
                    }
                case 7:
                    {                        
                        PausarMonitoramento();
                        Thread.Sleep(1000);
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
                        this.passosActionP3 = 3;
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoRecirculacao();                        
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 3;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) && !this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 1;
                        }
                        
                        break;
                    }
                case 3:
                    {
                        ValoresVO[] vValores = new ValoresVO[16];

                        for (int i = 0; i < 16; i++)
                        {
                            ValoresVO vo = new ValoresVO();
                            vo.Aceleracao = _prmRecircular.LMDispositivos[0].Aceleracao[i];
                            vo.Delay = _prmRecircular.LMDispositivos[0].Delay[i];
                            vo.PulsoReverso = 0;
                            vo.PulsoHorario = _prmRecircular.LMDispositivos[0].PulsoHorario[i];
                            vo.Velocidade = _prmRecircular.LMDispositivos[0].Velocidade[i];

                            vValores[i] = vo;

                        }
                        this._prmRecircular.DispenserP3.Dispensar(vValores, i_Step);

                        this.passosActionP3 = 4;
                        break;
                    }
                //tratar o termino da dispesna 
                case 4:
                    {
                        if (_prmRecircular.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmRecircular.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmRecircular.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }

                        DateTime dtAgora = DateTime.Now;
                        //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                        for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                        {
                            int nCIRCUITO = _prmRecircular.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                            //Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmRecircular.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                            Util.ObjectRecircular objRecAuto = this.listaRecircular.Find(o => o.Circuito == nCIRCUITO);
                            if (objRecAuto != null)
                            {
                                objRecAuto.DtInicio = dtAgora;
                                objRecAuto.VolumeDosado = objRecAuto.VolumeRecircular;
                                Util.ObjectRecircular.Persist(objRecAuto);
                            }
                        }
                        this.counterFalha = 0;
                        this.passosActionP3 = 7;
                        break;
                    }
                //Dosado agora é movimentar Valvula para Dosagem
                case 5:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoDosagem();
                        this.passosActionP3 = 6;
                        break;
                    }
                case 6:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(6, 5))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 7;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            !this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 5;
                        }
                     
                        break;
                    }
                case 7:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.OK;
                        Close();
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
                        this.counterFalha = 0;
                        this.passosActionP3 = 1;
                        break;
                    }
                case 1:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoRecirculacao();
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 3;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) && !this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 1;
                        }
                       
                        break;
                    }
                case 3:
                    {
                        ValoresVO[] vValores = new ValoresVO[16];

                        for (int i = 0; i < 16; i++)
                        {
                            ValoresVO vo = new ValoresVO();
                            vo.Aceleracao = _prmRecircular.LMDispositivos[0].Aceleracao[i];
                            vo.Delay = _prmRecircular.LMDispositivos[0].Delay[i];
                            vo.PulsoReverso = 0;
                            vo.PulsoHorario = _prmRecircular.LMDispositivos[0].PulsoHorario[i];
                            vo.Velocidade = _prmRecircular.LMDispositivos[0].Velocidade[i];

                            vValores[i] = vo;

                        }
                        this._prmRecircular.DispenserP3.Dispensar(vValores, i_Step);

                        this.passosActionP3 = 4;
                        break;
                    }
                //tratar o termino da dispesna 
                case 4:
                    {
                        if (_prmRecircular.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmRecircular.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmRecircular.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }

                        DateTime dtAgora = DateTime.Now;
                        //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                        for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                        {
                            int nCIRCUITO = _prmRecircular.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                            //Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmRecircular.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                            Util.ObjectRecircular objRecAuto = this.listaRecircular.Find(o => o.Circuito == nCIRCUITO);
                            if (objRecAuto != null)
                            {
                                objRecAuto.DtInicio = dtAgora;
                                objRecAuto.VolumeDosado = objRecAuto.VolumeRecircular;
                                Util.ObjectRecircular.Persist(objRecAuto);
                            }
                        }
                        this.passosActionP3 = 5;
                        this.counterFalha = 0;
                        break;
                    }
                //Dosado agora é movimentar Valvula para Dosagem
                case 5:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoDosagem();
                        this.passosActionP3 = 6;
                        break;
                    }
                case 6:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(6, 5))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 7;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            !this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 5;
                        }
                       
                        break;
                    }
                case 7:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.OK;
                        Close();
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
                        this.counterFalha = 0;
                        this.passosActionP3 = 3;
                        break;
                    }
                case 1:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoRecirculacao();
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 3;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) && !this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 1;
                        }
                        
                        break;
                    }
                case 3:
                    {
                        ValoresVO[] vValores = new ValoresVO[16];

                        for (int i = 0; i < 16; i++)
                        {
                            ValoresVO vo = new ValoresVO();
                            vo.Aceleracao = _prmRecircular.LMDispositivos[0].Aceleracao[i];
                            vo.Delay = _prmRecircular.LMDispositivos[0].Delay[i];
                            vo.PulsoReverso = 0;
                            vo.PulsoHorario = _prmRecircular.LMDispositivos[0].PulsoHorario[i];
                            vo.Velocidade = _prmRecircular.LMDispositivos[0].Velocidade[i];

                            vValores[i] = vo;

                        }
                        this._prmRecircular.DispenserP3.Dispensar(vValores, i_Step);

                        this.passosActionP3 = 4;
                        break;
                    }
                //tratar o termino da dispesna 
                case 4:
                    {
                        if (_prmRecircular.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmRecircular.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmRecircular.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }

                        DateTime dtAgora = DateTime.Now;
                        //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                        for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                        {
                            int nCIRCUITO = _prmRecircular.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                            //Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmRecircular.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                            Util.ObjectRecircular objRecAuto = this.listaRecircular.Find(o => o.Circuito == nCIRCUITO);
                            if (objRecAuto != null)
                            {
                                objRecAuto.DtInicio = dtAgora;
                                objRecAuto.VolumeDosado = objRecAuto.VolumeRecircular;
                                Util.ObjectRecircular.Persist(objRecAuto);
                            }
                        }
                        this.counterFalha = 0;
                        this.passosActionP3 = 7;
                        break;
                    }
                //Dosado agora é movimentar Valvula para Dosagem
                case 5:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoDosagem();
                        this.passosActionP3 = 6;
                        break;
                    }
                case 6:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(6, 5))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 7;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            !this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 5;
                        }
                      
                        break;
                    }
                case 7:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.OK;
                        Close();
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
                        this.counterFalha = 0;
                        //Dosar
                        this.passosActionP3 = 3;                       
                        break;
                    }
                case 1:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoRecirculacao();
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 3;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) && !this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 1;
                        }
                        
                        break;
                    }
                case 3:
                    {
                        ValoresVO[] vValores = new ValoresVO[16];

                        for (int i = 0; i < 16; i++)
                        {
                            ValoresVO vo = new ValoresVO();
                            vo.Aceleracao = _prmRecircular.LMDispositivos[0].Aceleracao[i];
                            vo.Delay = _prmRecircular.LMDispositivos[0].Delay[i];
                            vo.PulsoReverso = 0;
                            vo.PulsoHorario = _prmRecircular.LMDispositivos[0].PulsoHorario[i];
                            vo.Velocidade = _prmRecircular.LMDispositivos[0].Velocidade[i];

                            vValores[i] = vo;

                        }
                        this._prmRecircular.DispenserP3.Dispensar(vValores, i_Step);
                        
                        this.passosActionP3 = 4;
                        break;
                    }
                //tratar o termino da dispesna 
                case 4:
                    {
                        if (_prmRecircular.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmRecircular.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmRecircular.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }

                        DateTime dtAgora = DateTime.Now;
                        //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                        for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                        {
                            int nCIRCUITO = _prmRecircular.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                            //Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmRecircular.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                            Util.ObjectRecircular objRecAuto = this.listaRecircular.Find(o => o.Circuito == nCIRCUITO);
                            if (objRecAuto != null)
                            {
                                objRecAuto.DtInicio = dtAgora;
                                objRecAuto.VolumeDosado = objRecAuto.VolumeRecircular;
                                Util.ObjectRecircular.Persist(objRecAuto);
                            }
                        }
                        this.counterFalha = 0;
                        this.passosActionP3 = 7;
                        break;
                    }
                //Dosado agora é movimentar Valvula para Dosagem
                case 5:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoDosagem();
                        this.passosActionP3 = 6;
                        break;
                    }
                case 6:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(6, 5))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 7;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            !this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 5;
                        }
                        
                        break;
                    }
                case 7:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.OK;
                        Close();
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
                        this.counterFalha = 0;
                        this.passosActionP3 = 3;
                        break;
                    }
                case 1:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoRecirculacao();
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 3;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) && !this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 1;
                        }
                     
                        break;
                    }
                case 3:
                    {
                        ValoresVO[] vValores = new ValoresVO[16];

                        for (int i = 0; i < 16; i++)
                        {
                            ValoresVO vo = new ValoresVO();
                            vo.Aceleracao = _prmRecircular.LMDispositivos[0].Aceleracao[i];
                            vo.Delay = _prmRecircular.LMDispositivos[0].Delay[i];
                            vo.PulsoReverso = 0;
                            vo.PulsoHorario = _prmRecircular.LMDispositivos[0].PulsoHorario[i];
                            vo.Velocidade = _prmRecircular.LMDispositivos[0].Velocidade[i];

                            vValores[i] = vo;

                        }
                        this._prmRecircular.DispenserP3.Dispensar(vValores, i_Step);

                        this.passosActionP3 = 4;
                        break;
                    }
                //tratar o termino da dispesna 
                case 4:
                    {
                        if (_prmRecircular.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmRecircular.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmRecircular.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }

                        DateTime dtAgora = DateTime.Now;
                        //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                        for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                        {
                            int nCIRCUITO = _prmRecircular.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                            //Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmRecircular.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                            Util.ObjectRecircular objRecAuto = this.listaRecircular.Find(o => o.Circuito == nCIRCUITO);
                            if (objRecAuto != null)
                            {
                                objRecAuto.DtInicio = dtAgora;
                                objRecAuto.VolumeDosado = objRecAuto.VolumeRecircular;
                                Util.ObjectRecircular.Persist(objRecAuto);
                            }
                        }
                        this.counterFalha = 0;
                        this.passosActionP3 = 7;
                        break;
                    }
                //Dosado agora é movimentar Valvula para Dosagem
                case 5:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoDosagem();
                        this.passosActionP3 = 6;
                        break;
                    }
                case 6:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(6, 5))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 7;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            !this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 5;
                        }
                      
                        break;
                    }
                case 7:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.OK;
                        Close();
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
                        this.counterFalha = 0;
                        this.passosActionP3 = 1;
                        break;
                    }
                case 1:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoRecirculacao();
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 3;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) && !this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 1;
                        }
                      
                        break;
                    }
                case 3:
                    {
                        ValoresVO[] vValores = new ValoresVO[16];

                        for (int i = 0; i < 16; i++)
                        {
                            ValoresVO vo = new ValoresVO();
                            vo.Aceleracao = _prmRecircular.LMDispositivos[0].Aceleracao[i];
                            vo.Delay = _prmRecircular.LMDispositivos[0].Delay[i];
                            vo.PulsoReverso = 0;
                            vo.PulsoHorario = _prmRecircular.LMDispositivos[0].PulsoHorario[i];
                            vo.Velocidade = _prmRecircular.LMDispositivos[0].Velocidade[i];

                            vValores[i] = vo;

                        }
                        this._prmRecircular.DispenserP3.Dispensar(vValores, i_Step);

                        this.passosActionP3 = 4;
                        break;
                    }
                //tratar o termino da dispesna 
                case 4:
                    {
                        if (_prmRecircular.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmRecircular.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmRecircular.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }

                        DateTime dtAgora = DateTime.Now;
                        //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                        for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                        {
                            int nCIRCUITO = _prmRecircular.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                            //Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmRecircular.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                            Util.ObjectRecircular objRecAuto = this.listaRecircular.Find(o => o.Circuito == nCIRCUITO);
                            if (objRecAuto != null)
                            {
                                objRecAuto.DtInicio = dtAgora;
                                objRecAuto.VolumeDosado = objRecAuto.VolumeRecircular;
                                Util.ObjectRecircular.Persist(objRecAuto);
                            }
                        }
                        this.counterFalha = 0;
                        this.passosActionP3 = 7;
                        break;
                    }
                //Dosado agora é movimentar Valvula para Dosagem
                case 5:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoDosagem();
                        this.passosActionP3 = 6;
                        break;
                    }
                case 6:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(6, 5))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 7;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            !this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 5;
                        }
                      
                        break;
                    }
                case 7:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.OK;
                        Close();
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
                        this.counterFalha = 0;
                        this.passosActionP3 = 1;
                        
                        break;
                    }
                case 1:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoRecirculacao();
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 3;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) && !this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 1;
                        }
                     
                        break;
                    }
                case 3:
                    {
                        ValoresVO[] vValores = new ValoresVO[16];

                        for (int i = 0; i < 16; i++)
                        {
                            ValoresVO vo = new ValoresVO();
                            vo.Aceleracao = _prmRecircular.LMDispositivos[0].Aceleracao[i];
                            vo.Delay = _prmRecircular.LMDispositivos[0].Delay[i];
                            vo.PulsoReverso = 0;
                            vo.PulsoHorario = _prmRecircular.LMDispositivos[0].PulsoHorario[i];
                            vo.Velocidade = _prmRecircular.LMDispositivos[0].Velocidade[i];

                            vValores[i] = vo;

                        }
                        this._prmRecircular.DispenserP3.Dispensar(vValores, i_Step);

                        this.passosActionP3 = 4;
                        break;
                    }
                //tratar o termino da dispesna 
                case 4:
                    {
                        if (_prmRecircular.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmRecircular.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmRecircular.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }

                        DateTime dtAgora = DateTime.Now;
                        //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                        for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                        {
                            int nCIRCUITO = _prmRecircular.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                            //Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmRecircular.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                            Util.ObjectRecircular objRecAuto = this.listaRecircular.Find(o => o.Circuito == nCIRCUITO);
                            if (objRecAuto != null)
                            {
                                objRecAuto.DtInicio = dtAgora;
                                objRecAuto.VolumeDosado = objRecAuto.VolumeRecircular;
                                Util.ObjectRecircular.Persist(objRecAuto);
                            }
                        }
                        this.counterFalha = 0;
                        this.passosActionP3 = 5;
                        break;
                    }
                //Dosado agora é movimentar Valvula para Dosagem
                case 5:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoDosagem();
                        this.passosActionP3 = 6;
                        break;
                    }
                case 6:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(6, 5))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 7;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            !this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 5;
                        }
                       

                        break;
                    }
                case 7:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.OK;
                        Close();
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
                        this.counterFalha = 0;
                        //Dosar
                        this.passosActionP3 = 3;                       
                        break;
                    }
                case 1:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoRecirculacao();
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 3;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) && !this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 1;
                        }
                      
                        break;
                    }
                case 3:
                    {
                        ValoresVO[] vValores = new ValoresVO[16];

                        for (int i = 0; i < 16; i++)
                        {
                            ValoresVO vo = new ValoresVO();
                            vo.Aceleracao = _prmRecircular.LMDispositivos[0].Aceleracao[i];
                            vo.Delay = _prmRecircular.LMDispositivos[0].Delay[i];
                            vo.PulsoReverso = 0;
                            vo.PulsoHorario = _prmRecircular.LMDispositivos[0].PulsoHorario[i];
                            vo.Velocidade = _prmRecircular.LMDispositivos[0].Velocidade[i];

                            vValores[i] = vo;

                        }
                        this._prmRecircular.DispenserP3.Dispensar(vValores, i_Step);

                        this.passosActionP3 = 4;
                        break;
                    }
                //tratar o termino da dispesna 
                case 4:
                    {
                        if (_prmRecircular.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmRecircular.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmRecircular.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }


                        DateTime dtAgora = DateTime.Now;
                        //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                        for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                        {
                            int nCIRCUITO = _prmRecircular.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                            //Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmRecircular.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                            Util.ObjectRecircular objRecAuto = this.listaRecircular.Find(o => o.Circuito == nCIRCUITO);
                            if (objRecAuto != null)
                            {
                                objRecAuto.DtInicio = dtAgora;
                                objRecAuto.VolumeDosado = objRecAuto.VolumeRecircular;
                                Util.ObjectRecircular.Persist(objRecAuto);
                            }
                        }
                        this.passosActionP3 = 7;
                        this.counterFalha = 0;
                        break;
                    }
                //Dosado agora é movimentar Valvula para Dosagem
                case 5:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoDosagem();
                        this.passosActionP3 = 6;
                        break;
                    }
                case 6:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(6, 5))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 7;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            !this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 5;
                        }
                      
                        break;
                    }
                case 7:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.OK;
                        Close();
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
                        this.counterFalha = 0;
                        this.passosActionP3 = 1;
                        break;
                    }
                case 1:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoRecirculacao();
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {

                            this.passosActionP3 = 3;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) && !this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 1;
                        }
                    
                        break;
                    }
                case 3:
                    {
                        ValoresVO[] vValores = new ValoresVO[16];

                        for (int i = 0; i < 16; i++)
                        {
                            ValoresVO vo = new ValoresVO();
                            vo.Aceleracao = _prmRecircular.LMDispositivos[0].Aceleracao[i];
                            vo.Delay = _prmRecircular.LMDispositivos[0].Delay[i];
                            vo.PulsoReverso = 0;
                            vo.PulsoHorario = _prmRecircular.LMDispositivos[0].PulsoHorario[i];
                            vo.Velocidade = _prmRecircular.LMDispositivos[0].Velocidade[i];

                            vValores[i] = vo;

                        }
                        this._prmRecircular.DispenserP3.Dispensar(vValores, i_Step);

                        this.passosActionP3 = 4;
                        break;
                    }
                //tratar o termino da dispesna 
                case 4:
                    {
                        if (_prmRecircular.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmRecircular.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmRecircular.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }

                        DateTime dtAgora = DateTime.Now;
                        //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                        for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                        {
                            int nCIRCUITO = _prmRecircular.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                            //Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmRecircular.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                            Util.ObjectRecircular objRecAuto = this.listaRecircular.Find(o => o.Circuito == nCIRCUITO);
                            if (objRecAuto != null)
                            {
                                objRecAuto.DtInicio = dtAgora;
                                objRecAuto.VolumeDosado = objRecAuto.VolumeRecircular;
                                Util.ObjectRecircular.Persist(objRecAuto);
                            }
                        }
                        this.passosActionP3 = 5;
                        this.counterFalha = 0;
                        break;
                    }
                //Dosado agora é movimentar Valvula para Dosagem
                case 5:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoDosagem();
                        this.passosActionP3 = 6;
                        break;
                    }
                case 6:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(6, 5))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 7;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            !this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 5;
                        }
                      
                        break;
                    }
                case 7:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.OK;
                        Close();
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
                        this.counterFalha = 0;
                        this.passosActionP3 = 3;
                        break;
                    }
                case 1:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoRecirculacao();
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 3;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) && !this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 1;
                        }
                       
                        break;
                    }
                case 3:
                    {
                        ValoresVO[] vValores = new ValoresVO[16];

                        for (int i = 0; i < 16; i++)
                        {
                            ValoresVO vo = new ValoresVO();
                            vo.Aceleracao = _prmRecircular.LMDispositivos[0].Aceleracao[i];
                            vo.Delay = _prmRecircular.LMDispositivos[0].Delay[i];
                            vo.PulsoReverso = 0;
                            vo.PulsoHorario = _prmRecircular.LMDispositivos[0].PulsoHorario[i];
                            vo.Velocidade = _prmRecircular.LMDispositivos[0].Velocidade[i];

                            vValores[i] = vo;

                        }
                        this._prmRecircular.DispenserP3.Dispensar(vValores, i_Step);

                        this.passosActionP3 = 4;
                        break;
                    }
                //tratar o termino da dispesna 
                case 4:
                    {
                        if (_prmRecircular.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmRecircular.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmRecircular.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }

                        DateTime dtAgora = DateTime.Now;
                        //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                        for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                        {
                            int nCIRCUITO = _prmRecircular.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                            //Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmRecircular.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                            Util.ObjectRecircular objRecAuto = this.listaRecircular.Find(o => o.Circuito == nCIRCUITO);
                            if (objRecAuto != null)
                            {
                                objRecAuto.DtInicio = dtAgora;
                                objRecAuto.VolumeDosado = objRecAuto.VolumeRecircular;
                                Util.ObjectRecircular.Persist(objRecAuto);
                            }
                        }
                        this.counterFalha = 0;
                        this.passosActionP3 = 7;
                        break;
                    }
                //Dosado agora é movimentar Valvula para Dosagem
                case 5:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoDosagem();
                        this.passosActionP3 = 6;
                        break;
                    }
                case 6:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(6, 5))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 7;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            !this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 5;
                        }
                       
                        break;
                    }
                case 7:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.OK;
                        Close();
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
                        this.counterFalha = 0;                        
                        this.passosActionP3 = 1;
                        break;
                    }
                case 1:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoRecirculacao();
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 3;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) && !this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 1;
                        }
                        
                        break;
                    }
                case 3:
                    {
                        ValoresVO[] vValores = new ValoresVO[16];

                        for (int i = 0; i < 16; i++)
                        {
                            ValoresVO vo = new ValoresVO();
                            vo.Aceleracao = _prmRecircular.LMDispositivos[0].Aceleracao[i];
                            vo.Delay = _prmRecircular.LMDispositivos[0].Delay[i];
                            vo.PulsoReverso = 0;
                            vo.PulsoHorario = _prmRecircular.LMDispositivos[0].PulsoHorario[i];
                            vo.Velocidade = _prmRecircular.LMDispositivos[0].Velocidade[i];

                            vValores[i] = vo;

                        }
                        this._prmRecircular.DispenserP3.Dispensar(vValores, i_Step);

                        this.passosActionP3 = 4;
                        break;
                    }
                //tratar o termino da dispesna 
                case 4:
                    {
                        if (_prmRecircular.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmRecircular.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmRecircular.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }

                        DateTime dtAgora = DateTime.Now;
                        //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                        for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                        {
                            int nCIRCUITO = _prmRecircular.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                            //Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmRecircular.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                            Util.ObjectRecircular objRecAuto = this.listaRecircular.Find(o => o.Circuito == nCIRCUITO);
                            if (objRecAuto != null)
                            {
                                objRecAuto.DtInicio = dtAgora;
                                objRecAuto.VolumeDosado = objRecAuto.VolumeRecircular;
                                Util.ObjectRecircular.Persist(objRecAuto);
                            }
                        }
                        this.counterFalha = 0;
                        this.passosActionP3 = 7;
                        break;
                    }
                //Dosado agora é movimentar Valvula para Dosagem
                case 5:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoDosagem();
                        this.passosActionP3 = 6;
                        break;
                    }
                case 6:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(6, 5))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 7;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            !this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 5;
                        }
                      
                        break;
                    }
                case 7:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.OK;
                        Close();
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
                        this.counterFalha = 0;
                        this.passosActionP3 = 3;
                        break;
                    }
                case 1:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoRecirculacao();
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {

                            this.passosActionP3 = 3;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) && !this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 1;
                        }
                       
                        break;
                    }
                case 3:
                    {
                        ValoresVO[] vValores = new ValoresVO[16];

                        for (int i = 0; i < 16; i++)
                        {
                            ValoresVO vo = new ValoresVO();
                            vo.Aceleracao = _prmRecircular.LMDispositivos[0].Aceleracao[i];
                            vo.Delay = _prmRecircular.LMDispositivos[0].Delay[i];
                            vo.PulsoReverso = 0;
                            vo.PulsoHorario = _prmRecircular.LMDispositivos[0].PulsoHorario[i];
                            vo.Velocidade = _prmRecircular.LMDispositivos[0].Velocidade[i];

                            vValores[i] = vo;

                        }
                        this._prmRecircular.DispenserP3.Dispensar(vValores, i_Step);

                        this.passosActionP3 = 4;
                        break;
                    }
                //tratar o termino da dispesna 
                case 4:
                    {
                        if (_prmRecircular.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmRecircular.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmRecircular.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }

                        DateTime dtAgora = DateTime.Now;
                        //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                        for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                        {
                            int nCIRCUITO = _prmRecircular.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                            //Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmRecircular.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                            Util.ObjectRecircular objRecAuto = this.listaRecircular.Find(o => o.Circuito == nCIRCUITO);
                            if (objRecAuto != null)
                            {
                                objRecAuto.DtInicio = dtAgora;
                                objRecAuto.VolumeDosado = objRecAuto.VolumeRecircular;
                                Util.ObjectRecircular.Persist(objRecAuto);
                            }
                        }
                        this.counterFalha = 0;
                        this.passosActionP3 = 7;
                        break;
                    }
                //Dosado agora é movimentar Valvula para Dosagem
                case 5:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoDosagem();
                        this.passosActionP3 = 6;
                        break;
                    }
                case 6:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(6, 5))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 7;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            !this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 5;
                        }
                       
                        break;
                    }
                case 7:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.OK;
                        Close();
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
                        this.counterFalha = 0;
                        //Dosar
                        this.passosActionP3 = 1;
                        break;
                    }
                case 1:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoRecirculacao();
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 3;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) && !this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 1;
                        }

                        break;
                    }
                case 3:
                    {
                        ValoresVO[] vValores = new ValoresVO[16];

                        for (int i = 0; i < 16; i++)
                        {
                            ValoresVO vo = new ValoresVO();
                            vo.Aceleracao = _prmRecircular.LMDispositivos[0].Aceleracao[i];
                            vo.Delay = _prmRecircular.LMDispositivos[0].Delay[i];
                            vo.PulsoReverso = 0;
                            vo.PulsoHorario = _prmRecircular.LMDispositivos[0].PulsoHorario[i];
                            vo.Velocidade = _prmRecircular.LMDispositivos[0].Velocidade[i];

                            vValores[i] = vo;

                        }
                        this._prmRecircular.DispenserP3.Dispensar(vValores, i_Step);

                        this.passosActionP3 = 4;
                        break;
                    }
                //tratar o termino da dispesna 
                case 4:
                    {
                        if (_prmRecircular.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmRecircular.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmRecircular.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }

                        DateTime dtAgora = DateTime.Now;
                        //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                        for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                        {
                            int nCIRCUITO = _prmRecircular.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                            //Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmRecircular.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                            Util.ObjectRecircular objRecAuto = this.listaRecircular.Find(o => o.Circuito == nCIRCUITO);
                            if (objRecAuto != null)
                            {
                                objRecAuto.DtInicio = dtAgora;
                                objRecAuto.VolumeDosado = objRecAuto.VolumeRecircular;
                                Util.ObjectRecircular.Persist(objRecAuto);
                            }
                        }
                        this.counterFalha = 0;
                        this.passosActionP3 = 7;
                        break;
                    }
                //Dosado agora é movimentar Valvula para Dosagem
                case 5:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoDosagem();
                        this.passosActionP3 = 6;
                        break;
                    }
                case 6:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(6, 5))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 7;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            !this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 5;
                        }

                        break;
                    }
                case 7:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.OK;
                        Close();
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
                        this.counterFalha = 0;
                        this.passosActionP3 = 1;
                        break;
                    }
                case 1:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoRecirculacao();
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 3;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) && !this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 1;
                        }

                        break;
                    }
                case 3:
                    {
                        ValoresVO[] vValores = new ValoresVO[16];

                        for (int i = 0; i < 16; i++)
                        {
                            ValoresVO vo = new ValoresVO();
                            vo.Aceleracao = _prmRecircular.LMDispositivos[0].Aceleracao[i];
                            vo.Delay = _prmRecircular.LMDispositivos[0].Delay[i];
                            vo.PulsoReverso = 0;
                            vo.PulsoHorario = _prmRecircular.LMDispositivos[0].PulsoHorario[i];
                            vo.Velocidade = _prmRecircular.LMDispositivos[0].Velocidade[i];

                            vValores[i] = vo;

                        }
                        this._prmRecircular.DispenserP3.Dispensar(vValores, i_Step);

                        this.passosActionP3 = 4;
                        break;
                    }
                //tratar o termino da dispesna 
                case 4:
                    {
                        if (_prmRecircular.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmRecircular.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmRecircular.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }

                        DateTime dtAgora = DateTime.Now;
                        //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                        for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                        {
                            int nCIRCUITO = _prmRecircular.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                            //Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmRecircular.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                            Util.ObjectRecircular objRecAuto = this.listaRecircular.Find(o => o.Circuito == nCIRCUITO);
                            if (objRecAuto != null)
                            {
                                objRecAuto.DtInicio = dtAgora;
                                objRecAuto.VolumeDosado = objRecAuto.VolumeRecircular;
                                Util.ObjectRecircular.Persist(objRecAuto);
                            }
                        }
                        this.counterFalha = 0;
                        this.passosActionP3 = 7;
                        break;
                    }
                //Dosado agora é movimentar Valvula para Dosagem
                case 5:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoDosagem();
                        this.passosActionP3 = 6;
                        break;
                    }
                case 6:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(6, 5))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 7;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            !this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 5;
                        }

                        break;
                    }
                case 7:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.OK;
                        Close();
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
                        this.counterFalha = 0;
                        this.passosActionP3 = 1;
                        break;
                    }
                case 1:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoRecirculacao();
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 3;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) && !this._prmRecircular.DispenserP3.SensorValvulaRecirculacao)
                        {
                            this.passosActionP3 = 1;
                        }

                        break;
                    }
                case 3:
                    {
                        ValoresVO[] vValores = new ValoresVO[16];

                        for (int i = 0; i < 16; i++)
                        {
                            ValoresVO vo = new ValoresVO();
                            vo.Aceleracao = _prmRecircular.LMDispositivos[0].Aceleracao[i];
                            vo.Delay = _prmRecircular.LMDispositivos[0].Delay[i];
                            vo.PulsoReverso = 0;
                            vo.PulsoHorario = _prmRecircular.LMDispositivos[0].PulsoHorario[i];
                            vo.Velocidade = _prmRecircular.LMDispositivos[0].Velocidade[i];

                            vValores[i] = vo;

                        }
                        this._prmRecircular.DispenserP3.Dispensar(vValores, i_Step);

                        this.passosActionP3 = 4;
                        break;
                    }
                //tratar o termino da dispesna 
                case 4:
                    {
                        if (_prmRecircular.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmRecircular.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmRecircular.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }

                        DateTime dtAgora = DateTime.Now;
                        //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                        for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                        {
                            int nCIRCUITO = _prmRecircular.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                            //Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmRecircular.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                            Util.ObjectRecircular objRecAuto = this.listaRecircular.Find(o => o.Circuito == nCIRCUITO);
                            if (objRecAuto != null)
                            {
                                objRecAuto.DtInicio = dtAgora;
                                objRecAuto.VolumeDosado = objRecAuto.VolumeRecircular;
                                Util.ObjectRecircular.Persist(objRecAuto);
                            }
                        }
                        this.counterFalha = 0;
                        this.passosActionP3 = 7;
                        break;
                    }
                //Dosado agora é movimentar Valvula para Dosagem
                case 5:
                    {
                        this._prmRecircular.DispenserP3.ValvulaPosicaoDosagem();
                        this.passosActionP3 = 6;
                        break;
                    }
                case 6:
                    {
                        this._prmRecircular.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(6, 5))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmRecircular.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 7;
                        }
                        else if ((this._prmRecircular.DispenserP3.IsNativo == 0 || this._prmRecircular.DispenserP3.IsNativo == 2) &&
                            !this._prmRecircular.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 5;
                        }

                        break;
                    }
                case 7:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.OK;
                        Close();
                        break;
                    }
            }
        }
               
        private bool btEmergenciaCodErro(int passosActionP3Emergencia, int passosActionP3CodError)
        {
            bool retorno = false;
            if (this._prmRecircular.DispenserP3.SensorEmergencia)
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
            else if (this._prmRecircular.DispenserP3.CodError > 0)
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
                    ////string msg = "Usuário ocorreu algum erro na movimentação da placa. Deseja continuar este processo?" + Environment.NewLine + "Descrição: " + this._prmRecircular.DispenserP3.GetDescCodError();
                    string msg = Negocio.IdiomaResxExtensao.PlacaMov_Erro_Movimentacao_Passos + this._prmRecircular.DispenserP3.GetDescCodError();
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