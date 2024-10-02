using Percolore.Core;
using Percolore.Core.Logging;
using System.ComponentModel;

namespace Percolore.IOConnect
{
	public partial class fPurga : Form
    {
        Util.ObjectParametros _parametros = null;
        PurgaVO _prmPurga;
        int INDEX_CONTADOR;
        int INDEX_ULTIMO_COLORANTE=0;

        int INDEX_CONTADOR2;
        int INDEX_ULTIMO_COLORANTE2 =0;
        bool desativarUI;

        System.ComponentModel.BackgroundWorker backgroundWorker1 = null;
        private bool isThread = false;
        private bool isRunning = false;
        private int counterFalha = 0;
        // 
        private int tipoActionP3 = -1;
        private int passosActionP3 = 0;
        private int qtdTrocaRecipiente = 0;
        private bool isNewOperacao = false;
        private bool isPurga_All = true;

        private int i_Step = 0;

        public fPurga(
            PurgaVO parametrosPurga, bool purga_all, bool desativarUI = false)
        {
            InitializeComponent();            

            this._prmPurga = parametrosPurga;
            this.isPurga_All = purga_all;
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
            lblStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg01;
            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg01;
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

        private void ProcessoPurga_Load(object sender, EventArgs e)
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
                Iniciar_Click(sender, e);
            }
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
                INDEX_ULTIMO_COLORANTE = _prmPurga.LMDispositivos[0].Colorantes.Count - 1;

                INDEX_CONTADOR2 = 0;
                if (_prmPurga.LMDispositivos.Count > 1)
                {
                    INDEX_ULTIMO_COLORANTE2 = _prmPurga.LMDispositivos[1].Colorantes.Count - 1;
                }
                else
                {
                    INDEX_CONTADOR2 = 1;
                    INDEX_ULTIMO_COLORANTE2 = 0;
                }


                this.counterFalha = 0;
                
                this.tipoActionP3 = -1;
                this.passosActionP3 = 0;
                if (_prmPurga.DispenserP3 == null)
                {
                    if (!_parametros.PurgaSequencial)
                    {
                        ValoresVO[] vValores = new ValoresVO[16];
                        ValoresVO[] vValores2 = new ValoresVO[16];
                        //switch((Dispositivo)_parametros.IdDispositivo)
                        //{
                        //    case Dispositivo.Placa_4:
                        //        {
                        //            vValores = new ValoresVO[24];
                        //            break;
                        //        }
                        //}

                        //switch ((Dispositivo2)_parametros.IdDispositivo2)
                        //{
                        //    case Dispositivo2.Placa_4:
                        //        {
                        //            vValores2 = new ValoresVO[24];
                        //            break;
                        //        }
                        //}
                        List<Util.ObjectColorante> lCol = Util.ObjectColorante.List().FindAll(o=>o.Seguidor > 0).ToList();

                        for (int i = 0; i < _prmPurga.LMDispositivos[0].Colorantes.Count; i++)
                        {
                            ValoresVO vo = new ValoresVO();
                            vo.Aceleracao = _prmPurga.LMDispositivos[0].Aceleracao[i];
                            vo.Delay = _prmPurga.LMDispositivos[0].Delay[i];
                            vo.PulsoReverso = _prmPurga.LMDispositivos[0].PulsoReverso[i];
                            vo.PulsoHorario = _prmPurga.LMDispositivos[0].PulsoHorario[i];
                            vo.Velocidade = _prmPurga.LMDispositivos[0].Velocidade[i];

                            List<Util.ObjectColorante> ncol = lCol.FindAll(o => o.Seguidor == _prmPurga.LMDispositivos[0].Colorantes[i].Circuito).ToList();
                            if(ncol != null && ncol.Count > 0)
                            {
                                foreach(Util.ObjectColorante _col in ncol)
                                {
                                    vValores[_col.Circuito - 1] = vo;
                                }
                            }
                            vValores[_prmPurga.LMDispositivos[0].Colorantes[i].Circuito-1] = vo;

                        }
                        


                        if (_prmPurga.LMDispositivos.Count > 1)
                        {
                            for (int i = 0; i < _prmPurga.LMDispositivos[1].Colorantes.Count; i++)
                            {
                                ValoresVO vo = new ValoresVO();
                                vo.Aceleracao = _prmPurga.LMDispositivos[1].Aceleracao[i];
                                vo.Delay = _prmPurga.LMDispositivos[1].Delay[i];
                                vo.PulsoReverso = _prmPurga.LMDispositivos[1].PulsoReverso[i];
                                vo.PulsoHorario = _prmPurga.LMDispositivos[1].PulsoHorario[i];
                                vo.Velocidade = _prmPurga.LMDispositivos[1].Velocidade[i];

                                List<Util.ObjectColorante> ncol = lCol.FindAll(o => o.Seguidor == _prmPurga.LMDispositivos[1].Colorantes[i].Circuito).ToList();
                                if (ncol != null && ncol.Count > 0)
                                {
                                    foreach (Util.ObjectColorante _col in ncol)
                                    {
                                        vValores2[_col.Circuito - 17] = vo;
                                    }
                                }
                                vValores2[_prmPurga.LMDispositivos[1].Colorantes[i].Circuito-17] = vo;
                            }
                        }

                        _prmPurga.Dispenser[0].Dispensar(vValores, i_Step);
                        if (_prmPurga.LMDispositivos.Count > 1)
                        {
                            _prmPurga.Dispenser[1].Dispensar(vValores2, i_Step);
                        }

                        lblStatus.Text = "Purga Simultânea sendo realizada!";                      
                        lblSubStatus.Text = "";
                    }
                    this.tipoActionP3 = 0;
                }
                else
                {
                    bool isPosicao = false;
                    _prmPurga.DispenserP3.ReadSensores_Mover();
                    if(!(_prmPurga.DispenserP3.IsNativo == 0 || _prmPurga.DispenserP3.IsNativo == 2))
                    {
                        //lblStatus.Text = "Condição de placa movimentação incorreta!";
                        lblStatus.Text = Negocio.IdiomaResxExtensao.PlacaMov_Condicao_Incorreta;
                        lblSubStatus.Text = "";
                        return;
                    }
                    if(!_prmPurga.DispenserP3.SensorGavetaAberta && !_prmPurga.DispenserP3.SensorGavetaFechada )
                    {
                        _prmPurga.DispenserP3.AbrirGaveta(true);
                        for (int _i = 0; _i < 20; _i++)
                        {
                            if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                            {
                                _i = 20;
                            }
                            Thread.Sleep(500);
                        }
                        //Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        isPosicao = true;
                    }
                    if (!_prmPurga.DispenserP3.SensorValvulaDosagem && !_prmPurga.DispenserP3.SensorValvulaRecirculacao)
                    {
                        _prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                        isPosicao = true;
                    }
                    if(isPosicao)
                    {
                        _prmPurga.DispenserP3.ReadSensores_Mover();
                        if ((!_prmPurga.DispenserP3.SensorGavetaAberta && !_prmPurga.DispenserP3.SensorGavetaFechada) || (!_prmPurga.DispenserP3.SensorValvulaDosagem && !_prmPurga.DispenserP3.SensorValvulaRecirculacao))
                        {
                            //lblStatus.Text = "Condição de placa movimentação incorreta!";
                            lblStatus.Text = Negocio.IdiomaResxExtensao.PlacaMov_Condicao_Incorreta;
                            lblSubStatus.Text = "";
                            return;
                        }
                    }


                    this.tipoActionP3 = getActionP3();
                }

                if (this.tipoActionP3 >= 0)
                {
                    ExecutarMonitoramento();
                    Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_14 + Negocio.IdiomaResxExtensao.Global_PurgaIniciada);
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
            if(_prmPurga.DispenserP3.SensorEsponja && _prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
                && !_prmPurga.DispenserP3.SensorGavetaAberta && _prmPurga.DispenserP3.SensorGavetaFechada && _prmPurga.DispenserP3.SensorValvulaDosagem 
                && !_prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 1;
            }
            else if (_prmPurga.DispenserP3.SensorEsponja && _prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
                && !_prmPurga.DispenserP3.SensorGavetaAberta && _prmPurga.DispenserP3.SensorGavetaFechada && !_prmPurga.DispenserP3.SensorValvulaDosagem
                && _prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 2;
            }
            else if (_prmPurga.DispenserP3.SensorEsponja && _prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
                && _prmPurga.DispenserP3.SensorGavetaAberta && !_prmPurga.DispenserP3.SensorGavetaFechada && _prmPurga.DispenserP3.SensorValvulaDosagem
                && !_prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 3;
            }
            else if (_prmPurga.DispenserP3.SensorEsponja && _prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
                && _prmPurga.DispenserP3.SensorGavetaAberta && !_prmPurga.DispenserP3.SensorGavetaFechada && !_prmPurga.DispenserP3.SensorValvulaDosagem
                && _prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 4;
            }
            else if (_prmPurga.DispenserP3.SensorEsponja && !_prmPurga.DispenserP3.SensorCopo && _prmPurga.DispenserP3.SensorBaixoBicos && !_prmPurga.DispenserP3.SensorAltoBicos
                && !_prmPurga.DispenserP3.SensorGavetaAberta && _prmPurga.DispenserP3.SensorGavetaFechada && !_prmPurga.DispenserP3.SensorValvulaDosagem
                && _prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 5;
            }
            else if (_prmPurga.DispenserP3.SensorEsponja && !_prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
                && !_prmPurga.DispenserP3.SensorGavetaAberta && _prmPurga.DispenserP3.SensorGavetaFechada && !_prmPurga.DispenserP3.SensorValvulaDosagem
                && _prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 6;
            }
            else if (_prmPurga.DispenserP3.SensorEsponja && !_prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
                && _prmPurga.DispenserP3.SensorGavetaAberta && !_prmPurga.DispenserP3.SensorGavetaFechada && !_prmPurga.DispenserP3.SensorValvulaDosagem
                && _prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 7;
            }
            else if (!_prmPurga.DispenserP3.SensorEsponja && !_prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
               && !_prmPurga.DispenserP3.SensorGavetaAberta && _prmPurga.DispenserP3.SensorGavetaFechada && _prmPurga.DispenserP3.SensorValvulaDosagem
               && !_prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 8;
            }
            else if (!_prmPurga.DispenserP3.SensorEsponja && !_prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
               && !_prmPurga.DispenserP3.SensorGavetaAberta && _prmPurga.DispenserP3.SensorGavetaFechada && !_prmPurga.DispenserP3.SensorValvulaDosagem
               && _prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 9;
            }
            else if (!_prmPurga.DispenserP3.SensorEsponja && !_prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
               && _prmPurga.DispenserP3.SensorGavetaAberta && !_prmPurga.DispenserP3.SensorGavetaFechada && _prmPurga.DispenserP3.SensorValvulaDosagem
               && !_prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 10;
            }
            else if (!_prmPurga.DispenserP3.SensorEsponja && !_prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
               && _prmPurga.DispenserP3.SensorGavetaAberta && !_prmPurga.DispenserP3.SensorGavetaFechada && !_prmPurga.DispenserP3.SensorValvulaDosagem
               && _prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 11;
            }
            else if (_prmPurga.DispenserP3.SensorEsponja && !_prmPurga.DispenserP3.SensorCopo && _prmPurga.DispenserP3.SensorBaixoBicos &&
             !_prmPurga.DispenserP3.SensorAltoBicos && !_prmPurga.DispenserP3.SensorGavetaAberta && _prmPurga.DispenserP3.SensorGavetaFechada &&
             _prmPurga.DispenserP3.SensorValvulaDosagem && !_prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 14;
            }
            else if (_prmPurga.DispenserP3.SensorEsponja && !_prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos &&
              _prmPurga.DispenserP3.SensorAltoBicos && !_prmPurga.DispenserP3.SensorGavetaAberta && _prmPurga.DispenserP3.SensorGavetaFechada &&
              _prmPurga.DispenserP3.SensorValvulaDosagem && !_prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 15;
            }
            else if (_prmPurga.DispenserP3.SensorEsponja && !_prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos &&
              _prmPurga.DispenserP3.SensorAltoBicos && _prmPurga.DispenserP3.SensorGavetaAberta && !_prmPurga.DispenserP3.SensorGavetaFechada &&
              _prmPurga.DispenserP3.SensorValvulaDosagem && !_prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 16;
            }
            else if (_prmPurga.DispenserP3.SensorBaixoBicos && !_prmPurga.DispenserP3.SensorAltoBicos
               && _prmPurga.DispenserP3.SensorGavetaAberta && !_prmPurga.DispenserP3.SensorGavetaFechada && _prmPurga.DispenserP3.SensorValvulaDosagem
               && !_prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 12;
            }
            else if (_prmPurga.DispenserP3.SensorBaixoBicos && !_prmPurga.DispenserP3.SensorAltoBicos
              && _prmPurga.DispenserP3.SensorGavetaAberta && !_prmPurga.DispenserP3.SensorGavetaFechada && !_prmPurga.DispenserP3.SensorValvulaDosagem
              && _prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 13;
            }


            return retorno;
        }
        
        void Abortar_Click(object sender, EventArgs e)
        {
            PausarMonitoramento();
            try
            {
                Thread.Sleep(1000);
                //Interrompe e retoma operação da placa
                if (_prmPurga.DispenserP3 == null)
                {
                    for (int i = 0; i < _prmPurga.Dispenser.Count; i++)
                    {
                        _prmPurga.Dispenser[i].Halt();
                        _prmPurga.Dispenser[i].UnHalt();
                    }
                }
                else
                {
                    _prmPurga.DispenserP3.Halt();
                    _prmPurga.DispenserP3.UnHalt();
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

        private void Purga_Closed(object sender, FormClosedEventArgs e)
        {
            PausarMonitoramento();

            switch (this.DialogResult)
            {
                #region Log de processo

                case DialogResult.OK:
                    {
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_15 + Negocio.IdiomaResxExtensao.Global_PurgaConcluida);

                        //Persiste data e hora de execução da purga
                        if (this.isPurga_All)
                        {
                            Util.ObjectParametros.SetExecucaoPurga(DateTime.Now);
                        }
                        break;
                    }
                case DialogResult.Cancel:
                    {
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_16 + Negocio.IdiomaResxExtensao.Global_PurgaCancelada);
                        break;
                    }
                case DialogResult.Abort:
                    {
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_17 + Negocio.IdiomaResxExtensao.Global_PurgaAbortada);
                        break;
                    }
                case DialogResult.No:
                    {
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_18 + Negocio.IdiomaResxExtensao.Global_FalhaPurgar);
                        break;
                    }

                    #endregion
            }
        }

       
        #endregion

        #region Métodos privados

        void Falha(Exception ex)
		{
            LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			PausarMonitoramento();

            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
            {
                m.ShowDialog(
                    Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace, true);
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
                m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + Negocio.IdiomaResxExtensao.PlacaMov_Botao_Emergencia, true);
            }

            DialogResult = DialogResult.No;
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
                m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + Negocio.IdiomaResxExtensao.PlacaMov_Disp_Parou, true);
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
                        try
                        {
                            if (!this.isRunning)
                            {
                                this.isRunning = true;
                                this.Invoke(new MethodInvoker(MonitoramentoEvent));
                                if (_prmPurga.DispenserP3 == null)
                                {
                                    Thread.Sleep(500);
                                }
                            }
                        }
						catch (Exception ex)
						{
							LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
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

        // void Monitoramento_Event(object sender, EventArgs e)
        void MonitoramentoEvent()
        {
            try
            {
                if (_prmPurga.DispenserP3 == null)
                {
                    //Verifica se dispenser está pronto
                    for (int i = 0; i < _prmPurga.Dispenser.Count; i++)
                    {
                        if (!_prmPurga.Dispenser[i].IsReady)
                        {
                            this.isRunning = false;
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            return;
                        }
                    }
                    
                    if (_parametros.PurgaSequencial)
                    {
                        //Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CONTADOR > INDEX_ULTIMO_COLORANTE && INDEX_CONTADOR2 > INDEX_ULTIMO_COLORANTE2)
                        {
                            this.isRunning = false;
                            PausarMonitoramento();

                            //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                            }
                            if (_prmPurga.LMDispositivos.Count > 1)
                            {
                                for (INDEX_CONTADOR2 = 0; INDEX_CONTADOR2 <= INDEX_ULTIMO_COLORANTE2; INDEX_CONTADOR2++)
                                {
                                    int nCIRCUITO = _prmPurga.LMDispositivos[1].Colorantes[INDEX_CONTADOR2].Circuito;
                                    int desconta_C2 = 16;
                                    if ((Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador /*|| (Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_4*/)
                                    {
                                        desconta_C2 = 16;
                                    }
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[1].PulsoReverso[(nCIRCUITO - desconta_C2) - 1]);

                                }
                            }

                            DialogResult = DialogResult.OK;
                            Close();
                            return;
                        }

                        //Recupera posição do circuito do colorante
                        string nomCol = "";
                        string statusCol = "";
                        Color corColorante = Color.Transparent;


                        if (INDEX_CONTADOR < _prmPurga.LMDispositivos[0].Colorantes.Count)
                        {
                            int CIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;

                            //Atualiza interface
                            nomCol =
                                Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Nome;
                            statusCol =
                                Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                            corColorante = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].corCorante;

                           List <Util.ObjectColorante> lcol = Util.ObjectColorante.List().FindAll(o => o.Seguidor == CIRCUITO).ToList();
                            if (lcol == null || lcol.Count == 0)
                            {

                                //Dispara thread para enviar dados ao dispositivo
                                Task task = Task.Factory.StartNew(() => _prmPurga.Dispenser[0].Dispensar(
                                        CIRCUITO - 1,
                                        _prmPurga.LMDispositivos[0].PulsoHorario[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Velocidade[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Aceleracao[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Delay[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].PulsoReverso[CIRCUITO - 1], i_Step)
                                    );
                            }
                            else
                            {
                                int[] n_motor = new int[lcol.Count + 1];
                                
                                n_motor[0] = CIRCUITO - 1;
                                for (int i = 0; i < lcol.Count; i++)
                                {
                                    n_motor[i + 1] = (lcol[i].Circuito - 1);
                                }
                                //Dispara thread para enviar dados ao dispositivo
                                Task task = Task.Factory.StartNew(() => _prmPurga.Dispenser[0].Dispensar(
                                        n_motor,
                                        _prmPurga.LMDispositivos[0].PulsoHorario[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Velocidade[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Aceleracao[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Delay[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].PulsoReverso[CIRCUITO - 1], i_Step)
                                    );

                            }

                            if (_parametros.ControlarNivel)
                            {
                                //Operar.AbaterColorante(CIRCUITO, _prmPurga.Volume);
                                Operar.AbaterColorante(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                            }
                            //Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.Volume);
                            Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                            //Incrementa contador
                            INDEX_CONTADOR++;
                        }

                        else if (_prmPurga.LMDispositivos.Count > 1 && INDEX_CONTADOR2 < _prmPurga.LMDispositivos[1].Colorantes.Count)
                        {
                            int CIRCUITO2 = _prmPurga.LMDispositivos[1].Colorantes[INDEX_CONTADOR2].Circuito;

                            //Atualiza interface
                            if (nomCol != "")
                            {
                                nomCol += " - " + _prmPurga.LMDispositivos[1].Colorantes[INDEX_CONTADOR2].Nome;
                                statusCol += " - " + CIRCUITO2.ToString("00");
                            }
                            else
                            {
                                nomCol = Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.LMDispositivos[1].Colorantes[INDEX_CONTADOR2].Nome;
                                statusCol = Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO2.ToString("00");
                            }
                            int desconta_C2 = 16;
                            //if ((Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador /*|| (Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_4*/)
                            //{
                            //    desconta_C2 = 24;
                            //}                           

                            List<Util.ObjectColorante> lcol = Util.ObjectColorante.List().FindAll(o => o.Seguidor == CIRCUITO2).ToList();
                            if (lcol == null)
                            {                                
                               
                                //Dispara thread para enviar dados ao dispositivo
                                Task task2 = Task.Factory.StartNew(() => _prmPurga.Dispenser[1].Dispensar(
                                     (CIRCUITO2 - desconta_C2) - 1,
                                     _prmPurga.LMDispositivos[1].PulsoHorario[(CIRCUITO2 - desconta_C2) - 1],
                                     _prmPurga.LMDispositivos[1].Velocidade[(CIRCUITO2 - desconta_C2) - 1],
                                     _prmPurga.LMDispositivos[1].Aceleracao[(CIRCUITO2 - desconta_C2) - 1],
                                     _prmPurga.LMDispositivos[1].Delay[(CIRCUITO2 - desconta_C2) - 1],
                                     _prmPurga.LMDispositivos[1].PulsoReverso[(CIRCUITO2 - desconta_C2) - 1], i_Step)
                                );
                            }
                            else
                            {
                                int[] n_motor = new int[lcol.Count + 1];
                               
                                n_motor[0] = (CIRCUITO2 - desconta_C2) - 1;
                                for (int i = 0; i < lcol.Count; i++)
                                {
                                    n_motor[i + 1] = ((lcol[i].Circuito - desconta_C2) - 1);
                                }
                                Task task2 = Task.Factory.StartNew(() => _prmPurga.Dispenser[1].Dispensar(
                                     n_motor,
                                     _prmPurga.LMDispositivos[1].PulsoHorario[(CIRCUITO2 - desconta_C2) - 1],
                                     _prmPurga.LMDispositivos[1].Velocidade[(CIRCUITO2 - desconta_C2) - 1],
                                     _prmPurga.LMDispositivos[1].Aceleracao[(CIRCUITO2 - desconta_C2) - 1],
                                     _prmPurga.LMDispositivos[1].Delay[(CIRCUITO2 - desconta_C2) - 1],
                                     _prmPurga.LMDispositivos[1].PulsoReverso[(CIRCUITO2 - desconta_C2) - 1], i_Step)
                                );
                            }

                            if (_parametros.ControlarNivel)
                            {
                                //Operar.AbaterColorante(CIRCUITO2, _prmPurga.Volume);
                                Operar.AbaterColorante(CIRCUITO2, _prmPurga.LMDispositivos[1].VolumeDosado[(CIRCUITO2 - desconta_C2) - 1]);
                            }
                            //Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO2, _prmPurga.Volume);
                            Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO2, _prmPurga.LMDispositivos[1].VolumeDosado[(CIRCUITO2 - desconta_C2) - 1]);
                            INDEX_CONTADOR2++;
                        }
                                               
                        if (nomCol != "")
                        {
                            lblStatus.Text = nomCol;
                            lblSubStatus.Text = statusCol;
                            if (corColorante != Color.Transparent)
                            {
                                pnlCor.BackColor = corColorante;
                                pnlCor.Visible = true;
                            }
                            else
                            {
                                pnlCor.Visible = false;
                            }

                        }
                    }
                    else
                    {
                        this.isRunning = false;
                        PausarMonitoramento();

                        for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                        {
                            int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                            Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                        }
                        if (_prmPurga.LMDispositivos.Count > 1)
                        {
                            int desconta_C2 = 16;
                            if ((Dispositivo2)_parametros.IdDispositivo2 == Dispositivo2.Simulador /*|| (Dispositivo2)_parametros.IdDispositivo2 == Dispositivo2.Placa_4*/)
                            {
                                desconta_C2 = 16;
                            }
                            for (INDEX_CONTADOR2 = 0; INDEX_CONTADOR2 <= INDEX_ULTIMO_COLORANTE2; INDEX_CONTADOR2++)
                            {
                                int nCIRCUITO = _prmPurga.LMDispositivos[1].Colorantes[INDEX_CONTADOR2].Circuito;
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[1].PulsoReverso[(nCIRCUITO - desconta_C2) - 1]);
                            }
                        }

                        DialogResult = DialogResult.OK;
                        Close();
                        return;

                    }
                }
                else
                {
                    trataActionP3();
                }
                this.isRunning = false;
                this.counterFalha = 0;
                ////Thread.Sleep(1000);
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                if (this.counterFalha > 0)
                {
                    Falha(ex);
                }

                this.isRunning = false;
                this.counterFalha++;
            }
        }
        
        void trataActionP3()
        {
            try
            {
                switch(this.tipoActionP3)
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
            switch(this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //bool isRec = m.ShowDialog("Recipiente está na Gaveta", "Yes", "No");
                            bool isRec = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Recipiente_Dosadora, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                            if (!isRec)
                            {
                                //this.passosActionP3 = 1;
                                //this._prmPurga.DispenserP3.AbrirGaveta(true);
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                                //Thread.Sleep(1000);
                                this.counterFalha = 0;
                                this.passosActionP3 = 2;
                            }
                            else
                            {
                                //Dosar
                                this.passosActionP3 = 6;
                            }
                        }

                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.counterFalha = 0;
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaAberta && this._prmPurga.DispenserP3.SensorAltoBicos)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaAberta || !this._prmPurga.DispenserP3.SensorAltoBicos))
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
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
                        if(this.qtdTrocaRecipiente  > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        break;
                    }
                case 4:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);     
                        this.counterFalha = 0;
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        lblSubStatus.Text = "";
                        this._prmPurga.DispenserP3.UnHalt();
                        if (!_parametros.PurgaSequencial)
                        {
                            ValoresVO[] vValores = new ValoresVO[16];                            

                            for (int i = 0; i < 16; i++)
                            {
                                ValoresVO vo = new ValoresVO();
                                vo.Aceleracao = _prmPurga.LMDispositivos[0].Aceleracao[i];
                                vo.Delay = _prmPurga.LMDispositivos[0].Delay[i];
                                vo.PulsoReverso = _prmPurga.LMDispositivos[0].PulsoReverso[i];
                                vo.PulsoHorario = _prmPurga.LMDispositivos[0].PulsoHorario[i];
                                vo.Velocidade = _prmPurga.LMDispositivos[0].Velocidade[i];

                                vValores[i] = vo;

                            }
                            this._prmPurga.DispenserP3.Dispensar(vValores, i_Step);
                            
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                    //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (_parametros.PurgaSequencial)
                        {
                            //Verifica se já executou todos os circuitos de colorantes]
                            if (INDEX_CONTADOR > INDEX_ULTIMO_COLORANTE)
                            {
                                //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                                for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                                {
                                    int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                                }
                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }

                            //Recupera posição do circuito do colorante
                            string nomCol = "";
                            string statusCol = "";
                            if (INDEX_CONTADOR < _prmPurga.LMDispositivos[0].Colorantes.Count)
                            {
                                int CIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;

                                //Atualiza interface
                                nomCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Nome;
                                statusCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");


                                //Dispara thread para enviar dados ao dispositivo
                                Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                        CIRCUITO - 1,
                                        _prmPurga.LMDispositivos[0].PulsoHorario[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Velocidade[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Aceleracao[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Delay[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].PulsoReverso[CIRCUITO - 1], i_Step)
                                    );

                                if (_parametros.ControlarNivel)
                                {
                                    //Operar.AbaterColorante(CIRCUITO, _prmPurga.Volume);
                                    Operar.AbaterColorante(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                }
                                //Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.Volume);
                                Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);

                                //Incrementa contador
                                INDEX_CONTADOR++;
                            }

                            if (nomCol != "")
                            {
                                lblStatus.Text = nomCol;
                                lblSubStatus.Text = statusCol;
                            }

                        }
                        else
                        {
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);
                                
                            }
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
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
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
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
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
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
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        //lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                        //this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                        ////Thread.Sleep(1000);                        
                        //this.passosActionP3 = 17;
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {                            
                            PausarMonitoramento();
                            Thread.Sleep(1000);
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
                        this.counterFalha = 0;
                        this.qtdTrocaRecipiente = 0;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //bool isRec = m.ShowDialog("Recipiente está na Gaveta", "Yes", "No");
                            bool isRec = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Recipiente_Dosadora, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                            if (!isRec)
                            {
                                //this.passosActionP3 = 1;
                                //this._prmPurga.DispenserP3.AbrirGaveta(true);
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                                //Thread.Sleep(1000);
                                this.counterFalha = 0;
                                this.passosActionP3 = 2;
                            }
                            else
                            {
                                //Movimentar Válvula Recuar Bicos
                                //this.passosActionP3 = 18;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                                this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 19;
                            }
                        }

                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
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
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        
                        
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && 
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        this.counterFalha = 0;
                        this._prmPurga.DispenserP3.UnHalt();
                        if (!_parametros.PurgaSequencial)
                        {
                            ValoresVO[] vValores = new ValoresVO[16];

                            for (int i = 0; i < 16; i++)
                            {
                                ValoresVO vo = new ValoresVO();
                                vo.Aceleracao = _prmPurga.LMDispositivos[0].Aceleracao[i];
                                vo.Delay = _prmPurga.LMDispositivos[0].Delay[i];
                                vo.PulsoReverso = _prmPurga.LMDispositivos[0].PulsoReverso[i];
                                vo.PulsoHorario = _prmPurga.LMDispositivos[0].PulsoHorario[i];
                                vo.Velocidade = _prmPurga.LMDispositivos[0].Velocidade[i];

                                vValores[i] = vo;

                            }
                            this._prmPurga.DispenserP3.Dispensar(vValores, i_Step);
                            
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (_parametros.PurgaSequencial)
                        {
                            //Verifica se já executou todos os circuitos de colorantes]
                            if (INDEX_CONTADOR > INDEX_ULTIMO_COLORANTE)
                            {
                                //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                                for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                                {
                                    int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                                }
                                this.counterFalha = 0;
                                this.passosActionP3 = 8;
                                break;
                            }

                            //Recupera posição do circuito do colorante
                            string nomCol = "";
                            string statusCol = "";
                            if (INDEX_CONTADOR < _prmPurga.LMDispositivos[0].Colorantes.Count)
                            {
                                int CIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;

                                //Atualiza interface
                                nomCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Nome;
                                statusCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");


                                //Dispara thread para enviar dados ao dispositivo
                                Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                        CIRCUITO - 1,
                                        _prmPurga.LMDispositivos[0].PulsoHorario[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Velocidade[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Aceleracao[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Delay[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].PulsoReverso[CIRCUITO - 1], i_Step)
                                    );

                                if (_parametros.ControlarNivel)
                                {
                                    //Operar.AbaterColorante(CIRCUITO, _prmPurga.Volume);
                                    Operar.AbaterColorante(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);

                                }
                                //Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.Volume);
                                Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                //Incrementa contador
                                INDEX_CONTADOR++;
                            }
                           
                            if (nomCol != "")
                            {
                                lblStatus.Text = nomCol;
                                lblSubStatus.Text = statusCol;
                            }

                        }
                        else
                        {
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                            }
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);                        
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
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
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
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
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
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
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        //lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                        //this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                        ////Thread.Sleep(1000);                        
                        //this.passosActionP3 = 17;
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
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
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);                        
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            //Dosar
                            this.passosActionP3 = 6;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
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
                            bool isRec = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Recipiente_Dosadora, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                            if (!isRec)
                            {
                                //this.passosActionP3 = 1;
                                //this._prmPurga.DispenserP3.AbrirGaveta(true);
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                                //Thread.Sleep(1000);
                                this.counterFalha = 0;
                                this.passosActionP3 = 2;
                            }
                            else
                            {
                                this._prmPurga.DispenserP3.ReadSensores_Mover();
                                if (this._prmPurga.DispenserP3.SensorCopo && this._prmPurga.DispenserP3.SensorEsponja)
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
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
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
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));                        
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this._prmPurga.DispenserP3.UnHalt();
                        if (!_parametros.PurgaSequencial)
                        {
                            ValoresVO[] vValores = new ValoresVO[16];

                            for (int i = 0; i < 16; i++)
                            {
                                ValoresVO vo = new ValoresVO();
                                vo.Aceleracao = _prmPurga.LMDispositivos[0].Aceleracao[i];
                                vo.Delay = _prmPurga.LMDispositivos[0].Delay[i];
                                vo.PulsoReverso = _prmPurga.LMDispositivos[0].PulsoReverso[i];
                                vo.PulsoHorario = _prmPurga.LMDispositivos[0].PulsoHorario[i];
                                vo.Velocidade = _prmPurga.LMDispositivos[0].Velocidade[i];

                                vValores[i] = vo;

                            }
                            this._prmPurga.DispenserP3.Dispensar(vValores, i_Step);
                           
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (_parametros.PurgaSequencial)
                        {
                            //Verifica se já executou todos os circuitos de colorantes]
                            if (INDEX_CONTADOR > INDEX_ULTIMO_COLORANTE)
                            {
                                //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                                for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                                {
                                    int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                                }
                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }

                            //Recupera posição do circuito do colorante
                            string nomCol = "";
                            string statusCol = "";
                            if (INDEX_CONTADOR < _prmPurga.LMDispositivos[0].Colorantes.Count)
                            {
                                int CIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;

                                //Atualiza interface
                                nomCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Nome;
                                statusCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");


                                //Dispara thread para enviar dados ao dispositivo
                                Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                        CIRCUITO - 1,
                                        _prmPurga.LMDispositivos[0].PulsoHorario[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Velocidade[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Aceleracao[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Delay[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].PulsoReverso[CIRCUITO - 1], i_Step)
                                    );

                                if (_parametros.ControlarNivel)
                                {
                                    //Operar.AbaterColorante(CIRCUITO, _prmPurga.Volume);
                                    Operar.AbaterColorante(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                }
                                //Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.Volume);
                                Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                //Incrementa contador
                                INDEX_CONTADOR++;
                            }
                            if (nomCol != "")
                            {
                                lblStatus.Text = nomCol;
                                lblSubStatus.Text = statusCol;
                            }

                        }
                        else
                        {
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                            }
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }

                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
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
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
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
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
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
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        //lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                        //this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                        ////Thread.Sleep(1000);
                        //this.passosActionP3 = 17;
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
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
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            //Dosar
                            this.passosActionP3 = 6;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;
                        
                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && 
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && 
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
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
                            bool isRec = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Recipiente_Dosadora, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                            if (!isRec)
                            {
                                //this.passosActionP3 = 1;
                                //this._prmPurga.DispenserP3.AbrirGaveta(true);
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                                //Thread.Sleep(1000);
                                this.counterFalha = 0;
                                this.passosActionP3 = 2;
                            }
                            else
                            {
                                this._prmPurga.DispenserP3.ReadSensores_Mover();
                                if (this._prmPurga.DispenserP3.SensorCopo && this._prmPurga.DispenserP3.SensorEsponja)
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
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
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
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 18;
                                this.counterFalha = 0;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                                this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 19;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && 
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this._prmPurga.DispenserP3.UnHalt();
                        if (!_parametros.PurgaSequencial)
                        {
                            ValoresVO[] vValores = new ValoresVO[16];

                            for (int i = 0; i < 16; i++)
                            {
                                ValoresVO vo = new ValoresVO();
                                vo.Aceleracao = _prmPurga.LMDispositivos[0].Aceleracao[i];
                                vo.Delay = _prmPurga.LMDispositivos[0].Delay[i];
                                vo.PulsoReverso = _prmPurga.LMDispositivos[0].PulsoReverso[i];
                                vo.PulsoHorario = _prmPurga.LMDispositivos[0].PulsoHorario[i];
                                vo.Velocidade = _prmPurga.LMDispositivos[0].Velocidade[i];

                                vValores[i] = vo;

                            }
                            this._prmPurga.DispenserP3.Dispensar(vValores, i_Step);
                           
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (_parametros.PurgaSequencial)
                        {
                            //Verifica se já executou todos os circuitos de colorantes]
                            if (INDEX_CONTADOR > INDEX_ULTIMO_COLORANTE)
                            {
                                //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                                for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                                {
                                    int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                                }
                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }

                            //Recupera posição do circuito do colorante
                            string nomCol = "";
                            string statusCol = "";
                            if (INDEX_CONTADOR < _prmPurga.LMDispositivos[0].Colorantes.Count)
                            {
                                int CIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;

                                //Atualiza interface
                                nomCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Nome;
                                statusCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");


                                //Dispara thread para enviar dados ao dispositivo
                                Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                        CIRCUITO - 1,
                                        _prmPurga.LMDispositivos[0].PulsoHorario[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Velocidade[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Aceleracao[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Delay[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].PulsoReverso[CIRCUITO - 1], i_Step)
                                    );

                                if (_parametros.ControlarNivel)
                                {
                                    //Operar.AbaterColorante(CIRCUITO, _prmPurga.Volume);
                                    Operar.AbaterColorante(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                }
                                //Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.Volume);
                                Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                //Incrementa contador
                                INDEX_CONTADOR++;
                            }

                            if (nomCol != "")
                            {
                                lblStatus.Text = nomCol;
                                lblSubStatus.Text = statusCol;
                            }

                        }
                        else
                        {
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                            }
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
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
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
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
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
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
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        //lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                        //this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                        ////Thread.Sleep(1000);
                        //this.passosActionP3 = 17;
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            //Thread.Sleep(1000);
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
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            //Dosar
                            this.passosActionP3 = 6;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 18;
                                this.counterFalha = 0;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                                this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 19;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
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
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
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
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this._prmPurga.DispenserP3.UnHalt();
                        if (!_parametros.PurgaSequencial)
                        {
                            ValoresVO[] vValores = new ValoresVO[16];

                            for (int i = 0; i < 16; i++)
                            {
                                ValoresVO vo = new ValoresVO();
                                vo.Aceleracao = _prmPurga.LMDispositivos[0].Aceleracao[i];
                                vo.Delay = _prmPurga.LMDispositivos[0].Delay[i];
                                vo.PulsoReverso = _prmPurga.LMDispositivos[0].PulsoReverso[i];
                                vo.PulsoHorario = _prmPurga.LMDispositivos[0].PulsoHorario[i];
                                vo.Velocidade = _prmPurga.LMDispositivos[0].Velocidade[i];

                                vValores[i] = vo;

                            }
                            this._prmPurga.DispenserP3.Dispensar(vValores, i_Step);
                            
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (_parametros.PurgaSequencial)
                        {
                            //Verifica se já executou todos os circuitos de colorantes]
                            if (INDEX_CONTADOR > INDEX_ULTIMO_COLORANTE)
                            {
                                //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                                for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                                {
                                    int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                                }
                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }

                            //Recupera posição do circuito do colorante
                            string nomCol = "";
                            string statusCol = "";
                            if (INDEX_CONTADOR < _prmPurga.LMDispositivos[0].Colorantes.Count)
                            {
                                int CIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;

                                //Atualiza interface
                                nomCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Nome;
                                statusCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");


                                //Dispara thread para enviar dados ao dispositivo
                                Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                        CIRCUITO - 1,
                                        _prmPurga.LMDispositivos[0].PulsoHorario[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Velocidade[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Aceleracao[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Delay[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].PulsoReverso[CIRCUITO - 1], i_Step)
                                    );

                                if (_parametros.ControlarNivel)
                                {
                                    //Operar.AbaterColorante(CIRCUITO, _prmPurga.Volume);
                                    Operar.AbaterColorante(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                }
                                //Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.Volume);
                                Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                //Incrementa contador
                                INDEX_CONTADOR++;
                            }
                          
                            if (nomCol != "")
                            {
                                lblStatus.Text = nomCol;
                                lblSubStatus.Text = statusCol;
                            }

                        }
                        else
                        {
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                            }
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
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
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
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
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
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
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        //lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                        //this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                        ////Thread.Sleep(1000);
                        //this.passosActionP3 = 17;
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
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
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {                           
                            this.passosActionP3 = 1;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);                       
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && 
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
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
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;

                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
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
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this._prmPurga.DispenserP3.UnHalt();
                        if (!_parametros.PurgaSequencial)
                        {
                            ValoresVO[] vValores = new ValoresVO[16];

                            for (int i = 0; i < 16; i++)
                            {
                                ValoresVO vo = new ValoresVO();
                                vo.Aceleracao = _prmPurga.LMDispositivos[0].Aceleracao[i];
                                vo.Delay = _prmPurga.LMDispositivos[0].Delay[i];
                                vo.PulsoReverso = _prmPurga.LMDispositivos[0].PulsoReverso[i];
                                vo.PulsoHorario = _prmPurga.LMDispositivos[0].PulsoHorario[i];
                                vo.Velocidade = _prmPurga.LMDispositivos[0].Velocidade[i];

                                vValores[i] = vo;

                            }
                            this._prmPurga.DispenserP3.Dispensar(vValores, i_Step);
                           
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (_parametros.PurgaSequencial)
                        {
                            //Verifica se já executou todos os circuitos de colorantes]
                            if (INDEX_CONTADOR > INDEX_ULTIMO_COLORANTE)
                            {
                                //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                                for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                                {
                                    int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                                }
                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }

                            //Recupera posição do circuito do colorante
                            string nomCol = "";
                            string statusCol = "";
                            if (INDEX_CONTADOR < _prmPurga.LMDispositivos[0].Colorantes.Count)
                            {
                                int CIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;

                                //Atualiza interface
                                nomCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Nome;
                                statusCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");


                                //Dispara thread para enviar dados ao dispositivo
                                Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                        CIRCUITO - 1,
                                        _prmPurga.LMDispositivos[0].PulsoHorario[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Velocidade[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Aceleracao[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Delay[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].PulsoReverso[CIRCUITO - 1], i_Step)
                                    );

                                if (_parametros.ControlarNivel)
                                {
                                    //Operar.AbaterColorante(CIRCUITO, _prmPurga.Volume);
                                    Operar.AbaterColorante(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                }
                                //Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.Volume);
                                Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                //Incrementa contador
                                INDEX_CONTADOR++;
                            }
                            if (nomCol != "")
                            {
                                lblStatus.Text = nomCol;
                                lblSubStatus.Text = statusCol;
                            }

                        }
                        else
                        {
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                            }
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
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
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
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
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
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
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                    //{
                    //    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                    //    this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                    //    //Thread.Sleep(1000);
                    //    this.passosActionP3 = 17;
                    if (!this.isNewOperacao)
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                        this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 17;
                    }
                    else
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
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
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
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
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
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
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && 
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && 
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this._prmPurga.DispenserP3.UnHalt();
                        if (!_parametros.PurgaSequencial)
                        {
                            ValoresVO[] vValores = new ValoresVO[16];

                            for (int i = 0; i < 16; i++)
                            {
                                ValoresVO vo = new ValoresVO();
                                vo.Aceleracao = _prmPurga.LMDispositivos[0].Aceleracao[i];
                                vo.Delay = _prmPurga.LMDispositivos[0].Delay[i];
                                vo.PulsoReverso = _prmPurga.LMDispositivos[0].PulsoReverso[i];
                                vo.PulsoHorario = _prmPurga.LMDispositivos[0].PulsoHorario[i];
                                vo.Velocidade = _prmPurga.LMDispositivos[0].Velocidade[i];

                                vValores[i] = vo;

                            }
                            this._prmPurga.DispenserP3.Dispensar(vValores, i_Step);
                            
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (_parametros.PurgaSequencial)
                        {
                            //Verifica se já executou todos os circuitos de colorantes]
                            if (INDEX_CONTADOR > INDEX_ULTIMO_COLORANTE)
                            {
                                //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                                for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                                {
                                    int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                                }
                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }

                            //Recupera posição do circuito do colorante
                            string nomCol = "";
                            string statusCol = "";
                            if (INDEX_CONTADOR < _prmPurga.LMDispositivos[0].Colorantes.Count)
                            {
                                int CIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;

                                //Atualiza interface
                                nomCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Nome;
                                statusCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");


                                //Dispara thread para enviar dados ao dispositivo
                                Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                        CIRCUITO - 1,
                                        _prmPurga.LMDispositivos[0].PulsoHorario[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Velocidade[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Aceleracao[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Delay[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].PulsoReverso[CIRCUITO - 1], i_Step)
                                    );

                                if (_parametros.ControlarNivel)
                                {
                                    //Operar.AbaterColorante(CIRCUITO, _prmPurga.Volume);
                                    Operar.AbaterColorante(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                }
                                //Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.Volume);
                                Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                //Incrementa contador
                                INDEX_CONTADOR++;
                            }
                           
                            if (nomCol != "")
                            {
                                lblStatus.Text = nomCol;
                                lblSubStatus.Text = statusCol;
                            }

                        }
                        else
                        {
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                            }
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
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
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
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
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
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
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        //lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                        //this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                        ////Thread.Sleep(1000);
                        //this.passosActionP3 = 17;
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
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
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && 
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && 
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 20;
                                this.counterFalha = 0;
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
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.counterFalha = 0;
                        this.passosActionP3 = 2;
                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
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
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this._prmPurga.DispenserP3.UnHalt();
                        if (!_parametros.PurgaSequencial)
                        {
                            ValoresVO[] vValores = new ValoresVO[16];

                            for (int i = 0; i < 16; i++)
                            {
                                ValoresVO vo = new ValoresVO();
                                vo.Aceleracao = _prmPurga.LMDispositivos[0].Aceleracao[i];
                                vo.Delay = _prmPurga.LMDispositivos[0].Delay[i];
                                vo.PulsoReverso = _prmPurga.LMDispositivos[0].PulsoReverso[i];
                                vo.PulsoHorario = _prmPurga.LMDispositivos[0].PulsoHorario[i];
                                vo.Velocidade = _prmPurga.LMDispositivos[0].Velocidade[i];

                                vValores[i] = vo;

                            }
                            this._prmPurga.DispenserP3.Dispensar(vValores, i_Step);
                            
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (_parametros.PurgaSequencial)
                        {
                            //Verifica se já executou todos os circuitos de colorantes]
                            if (INDEX_CONTADOR > INDEX_ULTIMO_COLORANTE)
                            {
                                //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                                for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                                {
                                    int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                                }
                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }

                            //Recupera posição do circuito do colorante
                            string nomCol = "";
                            string statusCol = "";
                            if (INDEX_CONTADOR < _prmPurga.LMDispositivos[0].Colorantes.Count)
                            {
                                int CIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;

                                //Atualiza interface
                                nomCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Nome;
                                statusCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");


                                //Dispara thread para enviar dados ao dispositivo
                                Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                        CIRCUITO - 1,
                                        _prmPurga.LMDispositivos[0].PulsoHorario[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Velocidade[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Aceleracao[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Delay[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].PulsoReverso[CIRCUITO - 1], i_Step)
                                    );

                                if (_parametros.ControlarNivel)
                                {
                                    //Operar.AbaterColorante(CIRCUITO, _prmPurga.Volume);
                                    Operar.AbaterColorante(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);

                                }
                                //Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.Volume);
                                Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                //Incrementa contador
                                INDEX_CONTADOR++;
                            }
                            if (nomCol != "")
                            {
                                lblStatus.Text = nomCol;
                                lblSubStatus.Text = statusCol;
                            }

                        }
                        else
                        {
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                            }
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
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
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
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
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
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
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        //lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                        //this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                        ////Thread.Sleep(1000);
                        //this.passosActionP3 = 17;
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();

                        }
                        else
                        {
                            this.passosActionP3 = 16;
                            this.counterFalha = 0;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
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
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
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
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && 
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this._prmPurga.DispenserP3.UnHalt();
                        if (!_parametros.PurgaSequencial)
                        {
                            ValoresVO[] vValores = new ValoresVO[16];

                            for (int i = 0; i < 16; i++)
                            {
                                ValoresVO vo = new ValoresVO();
                                vo.Aceleracao = _prmPurga.LMDispositivos[0].Aceleracao[i];
                                vo.Delay = _prmPurga.LMDispositivos[0].Delay[i];
                                vo.PulsoReverso = _prmPurga.LMDispositivos[0].PulsoReverso[i];
                                vo.PulsoHorario = _prmPurga.LMDispositivos[0].PulsoHorario[i];
                                vo.Velocidade = _prmPurga.LMDispositivos[0].Velocidade[i];

                                vValores[i] = vo;

                            }
                            this._prmPurga.DispenserP3.Dispensar(vValores, i_Step);
                            
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (_parametros.PurgaSequencial)
                        {
                            //Verifica se já executou todos os circuitos de colorantes]
                            if (INDEX_CONTADOR > INDEX_ULTIMO_COLORANTE)
                            {
                                //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                                for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                                {
                                    int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                                }
                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }

                            //Recupera posição do circuito do colorante
                            string nomCol = "";
                            string statusCol = "";
                            if (INDEX_CONTADOR < _prmPurga.LMDispositivos[0].Colorantes.Count)
                            {
                                int CIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;

                                //Atualiza interface
                                nomCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Nome;
                                statusCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");


                                //Dispara thread para enviar dados ao dispositivo
                                Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                        CIRCUITO - 1,
                                        _prmPurga.LMDispositivos[0].PulsoHorario[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Velocidade[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Aceleracao[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Delay[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].PulsoReverso[CIRCUITO - 1], i_Step)
                                    );

                                if (_parametros.ControlarNivel)
                                {
                                    //Operar.AbaterColorante(CIRCUITO, _prmPurga.Volume);
                                    Operar.AbaterColorante(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);

                                }
                                //Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.Volume);
                                Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                //Incrementa contador
                                INDEX_CONTADOR++;
                            }


                         
                            if (nomCol != "")
                            {
                                lblStatus.Text = nomCol;
                                lblSubStatus.Text = statusCol;
                            }

                        }
                        else
                        {
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                            }
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
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
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
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
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
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
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        //lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                        //this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                        ////Thread.Sleep(1000);
                        //this.passosActionP3 = 17;
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();

                        }
                        else
                        {
                            this.passosActionP3 = 16;
                            this.counterFalha = 0;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
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
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.counterFalha = 0;
                        this.passosActionP3 = 2;
                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
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
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this._prmPurga.DispenserP3.UnHalt();
                        if (!_parametros.PurgaSequencial)
                        {
                            ValoresVO[] vValores = new ValoresVO[16];

                            for (int i = 0; i < 16; i++)
                            {
                                ValoresVO vo = new ValoresVO();
                                vo.Aceleracao = _prmPurga.LMDispositivos[0].Aceleracao[i];
                                vo.Delay = _prmPurga.LMDispositivos[0].Delay[i];
                                vo.PulsoReverso = _prmPurga.LMDispositivos[0].PulsoReverso[i];
                                vo.PulsoHorario = _prmPurga.LMDispositivos[0].PulsoHorario[i];
                                vo.Velocidade = _prmPurga.LMDispositivos[0].Velocidade[i];

                                vValores[i] = vo;

                            }
                            this._prmPurga.DispenserP3.Dispensar(vValores, i_Step);
                            
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_prmPurga.DispenserP3.IsHalt)
                        {
                            Emergencia();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (_parametros.PurgaSequencial)
                        {
                            //Verifica se já executou todos os circuitos de colorantes]
                            if (INDEX_CONTADOR > INDEX_ULTIMO_COLORANTE)
                            {
                                //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                                for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                                {
                                    int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                                }
                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }

                            //Recupera posição do circuito do colorante
                            string nomCol = "";
                            string statusCol = "";
                            if (INDEX_CONTADOR < _prmPurga.LMDispositivos[0].Colorantes.Count)
                            {
                                int CIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;

                                //Atualiza interface
                                nomCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Nome;
                                statusCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");


                                //Dispara thread para enviar dados ao dispositivo
                                Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                        CIRCUITO - 1,
                                        _prmPurga.LMDispositivos[0].PulsoHorario[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Velocidade[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Aceleracao[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Delay[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].PulsoReverso[CIRCUITO - 1], i_Step)
                                    );

                                if (_parametros.ControlarNivel)
                                {
                                    //Operar.AbaterColorante(CIRCUITO, _prmPurga.Volume);
                                    Operar.AbaterColorante(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                }
                                //Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.Volume);
                                Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                //Incrementa contador
                                INDEX_CONTADOR++;
                            }
                                                                                 
                            if (nomCol != "")
                            {
                                lblStatus.Text = nomCol;
                                lblSubStatus.Text = statusCol;
                            }

                        }
                        else
                        {
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                            }
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
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
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
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
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
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
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        //lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                        //this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                        ////Thread.Sleep(1000);
                        //this.passosActionP3 = 17;
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
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
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && 
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
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
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
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
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                //m.ShowDialog("Trocar recipiente!");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                this.passosActionP3 = 3;
                            }
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                        {
                            this.passosActionP3 = 1;
                        }

                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
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
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&                   
                                this._prmPurga.DispenserP3.SensorGavetaFechada )
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this._prmPurga.DispenserP3.UnHalt();
                        if (!_parametros.PurgaSequencial)
                        {
                            ValoresVO[] vValores = new ValoresVO[16];

                            for (int i = 0; i < 16; i++)
                            {
                                ValoresVO vo = new ValoresVO();
                                vo.Aceleracao = _prmPurga.LMDispositivos[0].Aceleracao[i];
                                vo.Delay = _prmPurga.LMDispositivos[0].Delay[i];
                                vo.PulsoReverso = _prmPurga.LMDispositivos[0].PulsoReverso[i];
                                vo.PulsoHorario = _prmPurga.LMDispositivos[0].PulsoHorario[i];
                                vo.Velocidade = _prmPurga.LMDispositivos[0].Velocidade[i];

                                vValores[i] = vo;

                            }
                            this._prmPurga.DispenserP3.Dispensar(vValores, i_Step);                           
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (_parametros.PurgaSequencial)
                        {
                            //Verifica se já executou todos os circuitos de colorantes]
                            if (INDEX_CONTADOR > INDEX_ULTIMO_COLORANTE)
                            {
                                //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                                for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                                {
                                    int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                                }
                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }

                            //Recupera posição do circuito do colorante
                            string nomCol = "";
                            string statusCol = "";
                            if (INDEX_CONTADOR < _prmPurga.LMDispositivos[0].Colorantes.Count)
                            {
                                int CIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;

                                //Atualiza interface
                                nomCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Nome;
                                statusCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");


                                //Dispara thread para enviar dados ao dispositivo
                                Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                        CIRCUITO - 1,
                                        _prmPurga.LMDispositivos[0].PulsoHorario[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Velocidade[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Aceleracao[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Delay[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].PulsoReverso[CIRCUITO - 1], i_Step)
                                    );

                                if (_parametros.ControlarNivel)
                                {
                                    //Operar.AbaterColorante(CIRCUITO, _prmPurga.Volume);
                                    Operar.AbaterColorante(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                }
                                //Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.Volume);
                                Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                //Incrementa contador
                                INDEX_CONTADOR++;
                            }

                           
                            if (nomCol != "")
                            {
                                lblStatus.Text = nomCol;
                                lblSubStatus.Text = statusCol;
                            }

                        }
                        else
                        {
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                            }
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
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
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
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
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
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
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        //lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                        //this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                        ////Thread.Sleep(1000);
                        //this.passosActionP3 = 17;
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
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
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 2;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
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
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        this.counterFalha = 0;
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //bool isAct = m.ShowDialog("Posicionar recipiente nos bicos!");
                            bool isAct = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Posicionar_Recipiente_Bico, true);
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
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
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
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this._prmPurga.DispenserP3.UnHalt();
                        if (!_parametros.PurgaSequencial)
                        {
                            ValoresVO[] vValores = new ValoresVO[16];

                            for (int i = 0; i < 16; i++)
                            {
                                ValoresVO vo = new ValoresVO();
                                vo.Aceleracao = _prmPurga.LMDispositivos[0].Aceleracao[i];
                                vo.Delay = _prmPurga.LMDispositivos[0].Delay[i];
                                vo.PulsoReverso = _prmPurga.LMDispositivos[0].PulsoReverso[i];
                                vo.PulsoHorario = _prmPurga.LMDispositivos[0].PulsoHorario[i];
                                vo.Velocidade = _prmPurga.LMDispositivos[0].Velocidade[i];

                                vValores[i] = vo;

                            }
                            this._prmPurga.DispenserP3.Dispensar(vValores, i_Step);
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (_parametros.PurgaSequencial)
                        {
                            //Verifica se já executou todos os circuitos de colorantes]
                            if (INDEX_CONTADOR > INDEX_ULTIMO_COLORANTE)
                            {
                                //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                                for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                                {
                                    int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                                }
                                this.counterFalha = 0;
                                this.passosActionP3 = 12;
                                break;
                            }

                            //Recupera posição do circuito do colorante
                            string nomCol = "";
                            string statusCol = "";
                            if (INDEX_CONTADOR < _prmPurga.LMDispositivos[0].Colorantes.Count)
                            {
                                int CIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;

                                //Atualiza interface
                                nomCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Nome;
                                statusCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");


                                //Dispara thread para enviar dados ao dispositivo
                                Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                        CIRCUITO - 1,
                                        _prmPurga.LMDispositivos[0].PulsoHorario[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Velocidade[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Aceleracao[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Delay[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].PulsoReverso[CIRCUITO - 1], i_Step)
                                    );

                                if (_parametros.ControlarNivel)
                                {
                                    //Operar.AbaterColorante(CIRCUITO, _prmPurga.Volume);
                                    Operar.AbaterColorante(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                }
                                //Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.Volume);
                                Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                //Incrementa contador
                                INDEX_CONTADOR++;
                            }
                            if (nomCol != "")
                            {
                                lblStatus.Text = nomCol;
                                lblSubStatus.Text = statusCol;
                            }

                        }
                        else
                        {
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                            }
                            this.counterFalha = 0;
                            this.passosActionP3 = 12;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
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
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
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
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
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
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        //lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                        //this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                        ////Thread.Sleep(1000);
                        //this.passosActionP3 = 17;
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
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
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 3;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
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
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        this.counterFalha = 0;
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //bool isAct = m.ShowDialog("Posicionar recipiente nos bicos!");
                            bool isAct = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Posicionar_Recipiente_Bico, true);
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
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
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
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this._prmPurga.DispenserP3.UnHalt();
                        if (!_parametros.PurgaSequencial)
                        {
                            ValoresVO[] vValores = new ValoresVO[16];

                            for (int i = 0; i < 16; i++)
                            {
                                ValoresVO vo = new ValoresVO();
                                vo.Aceleracao = _prmPurga.LMDispositivos[0].Aceleracao[i];
                                vo.Delay = _prmPurga.LMDispositivos[0].Delay[i];
                                vo.PulsoReverso = _prmPurga.LMDispositivos[0].PulsoReverso[i];
                                vo.PulsoHorario = _prmPurga.LMDispositivos[0].PulsoHorario[i];
                                vo.Velocidade = _prmPurga.LMDispositivos[0].Velocidade[i];

                                vValores[i] = vo;

                            }
                            this._prmPurga.DispenserP3.Dispensar(vValores, i_Step);
                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (_parametros.PurgaSequencial)
                        {
                            //Verifica se já executou todos os circuitos de colorantes]
                            if (INDEX_CONTADOR > INDEX_ULTIMO_COLORANTE)
                            {
                                //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                                for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                                {
                                    int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                                }
                                this.passosActionP3 = 12;
                                this.counterFalha = 0;
                                break;
                            }

                            //Recupera posição do circuito do colorante
                            string nomCol = "";
                            string statusCol = "";
                            if (INDEX_CONTADOR < _prmPurga.LMDispositivos[0].Colorantes.Count)
                            {
                                int CIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;

                                //Atualiza interface
                                nomCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Nome;
                                statusCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");


                                //Dispara thread para enviar dados ao dispositivo
                                Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                        CIRCUITO - 1,
                                        _prmPurga.LMDispositivos[0].PulsoHorario[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Velocidade[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Aceleracao[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Delay[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].PulsoReverso[CIRCUITO - 1], i_Step)
                                    );

                                if (_parametros.ControlarNivel)
                                {
                                    //Operar.AbaterColorante(CIRCUITO, _prmPurga.Volume);
                                    Operar.AbaterColorante(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                }
                                //Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.Volume);
                                Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                //Incrementa contador
                                INDEX_CONTADOR++;
                            }
                            
                            if (nomCol != "")
                            {
                                lblStatus.Text = nomCol;
                                lblSubStatus.Text = statusCol;
                            }

                        }
                        else
                        {
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                            }
                            this.passosActionP3 = 12;
                            this.counterFalha = 0;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
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
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
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
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
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
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        //lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                        //this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                        ////Thread.Sleep(1000);
                        //this.passosActionP3 = 17;
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
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
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 6;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
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
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.counterFalha = 0;
                        this.passosActionP3 = 2;
                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
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
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this._prmPurga.DispenserP3.UnHalt();
                        if (!_parametros.PurgaSequencial)
                        {
                            ValoresVO[] vValores = new ValoresVO[16];

                            for (int i = 0; i < 16; i++)
                            {
                                ValoresVO vo = new ValoresVO();
                                vo.Aceleracao = _prmPurga.LMDispositivos[0].Aceleracao[i];
                                vo.Delay = _prmPurga.LMDispositivos[0].Delay[i];
                                vo.PulsoReverso = _prmPurga.LMDispositivos[0].PulsoReverso[i];
                                vo.PulsoHorario = _prmPurga.LMDispositivos[0].PulsoHorario[i];
                                vo.Velocidade = _prmPurga.LMDispositivos[0].Velocidade[i];

                                vValores[i] = vo;

                            }
                            this._prmPurga.DispenserP3.Dispensar(vValores, i_Step);

                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (_parametros.PurgaSequencial)
                        {
                            //Verifica se já executou todos os circuitos de colorantes]
                            if (INDEX_CONTADOR > INDEX_ULTIMO_COLORANTE)
                            {
                                //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                                for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                                {
                                    int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                                }
                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }

                            //Recupera posição do circuito do colorante
                            string nomCol = "";
                            string statusCol = "";
                            if (INDEX_CONTADOR < _prmPurga.LMDispositivos[0].Colorantes.Count)
                            {
                                int CIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;

                                //Atualiza interface
                                nomCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Nome;
                                statusCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");


                                //Dispara thread para enviar dados ao dispositivo
                                Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                        CIRCUITO - 1,
                                        _prmPurga.LMDispositivos[0].PulsoHorario[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Velocidade[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Aceleracao[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Delay[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].PulsoReverso[CIRCUITO - 1], i_Step)
                                    );

                                if (_parametros.ControlarNivel)
                                {
                                    //Operar.AbaterColorante(CIRCUITO, _prmPurga.Volume);
                                    Operar.AbaterColorante(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                }
                                //Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.Volume);
                                Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                //Incrementa contador
                                INDEX_CONTADOR++;
                            }

                            if (nomCol != "")
                            {
                                lblStatus.Text = nomCol;
                                lblSubStatus.Text = statusCol;
                            }

                        }
                        else
                        {
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                            }
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
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
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
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
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
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
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        //lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                        //this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                        ////Thread.Sleep(1000);
                        //this.passosActionP3 = 17;
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
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
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
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
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.counterFalha = 0;
                        this.passosActionP3 = 2;

                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
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
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this._prmPurga.DispenserP3.UnHalt();
                        if (!_parametros.PurgaSequencial)
                        {
                            ValoresVO[] vValores = new ValoresVO[16];

                            for (int i = 0; i < 16; i++)
                            {
                                ValoresVO vo = new ValoresVO();
                                vo.Aceleracao = _prmPurga.LMDispositivos[0].Aceleracao[i];
                                vo.Delay = _prmPurga.LMDispositivos[0].Delay[i];
                                vo.PulsoReverso = _prmPurga.LMDispositivos[0].PulsoReverso[i];
                                vo.PulsoHorario = _prmPurga.LMDispositivos[0].PulsoHorario[i];
                                vo.Velocidade = _prmPurga.LMDispositivos[0].Velocidade[i];

                                vValores[i] = vo;

                            }
                            this._prmPurga.DispenserP3.Dispensar(vValores, i_Step);

                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (_parametros.PurgaSequencial)
                        {
                            //Verifica se já executou todos os circuitos de colorantes]
                            if (INDEX_CONTADOR > INDEX_ULTIMO_COLORANTE)
                            {
                                //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                                for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                                {
                                    int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                                }
                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }

                            //Recupera posição do circuito do colorante
                            string nomCol = "";
                            string statusCol = "";
                            if (INDEX_CONTADOR < _prmPurga.LMDispositivos[0].Colorantes.Count)
                            {
                                int CIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;

                                //Atualiza interface
                                nomCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Nome;
                                statusCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");


                                //Dispara thread para enviar dados ao dispositivo
                                Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                        CIRCUITO - 1,
                                        _prmPurga.LMDispositivos[0].PulsoHorario[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Velocidade[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Aceleracao[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Delay[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].PulsoReverso[CIRCUITO - 1], i_Step)
                                    );

                                if (_parametros.ControlarNivel)
                                {
                                    //Operar.AbaterColorante(CIRCUITO, _prmPurga.Volume);
                                    Operar.AbaterColorante(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                }
                                //Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.Volume);
                                Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                //Incrementa contador
                                INDEX_CONTADOR++;
                            }
                                                                                   
                            if (nomCol != "")
                            {
                                lblStatus.Text = nomCol;
                                lblSubStatus.Text = statusCol;
                            }

                        }
                        else
                        {
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                            }
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
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
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
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
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
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
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        //lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                        //this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                        ////Thread.Sleep(1000);
                        //this.passosActionP3 = 17;
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
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
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
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
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            //this.passosActionP3 = 1;
                            //this._prmPurga.DispenserP3.AbrirGaveta(true);
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.counterFalha = 0;
                            this.passosActionP3 = 2;
                        }
                        this.counterFalha = 0;

                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
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
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this._prmPurga.DispenserP3.UnHalt();
                        if (!_parametros.PurgaSequencial)
                        {
                            ValoresVO[] vValores = new ValoresVO[16];

                            for (int i = 0; i < 16; i++)
                            {
                                ValoresVO vo = new ValoresVO();
                                vo.Aceleracao = _prmPurga.LMDispositivos[0].Aceleracao[i];
                                vo.Delay = _prmPurga.LMDispositivos[0].Delay[i];
                                vo.PulsoReverso = _prmPurga.LMDispositivos[0].PulsoReverso[i];
                                vo.PulsoHorario = _prmPurga.LMDispositivos[0].PulsoHorario[i];
                                vo.Velocidade = _prmPurga.LMDispositivos[0].Velocidade[i];

                                vValores[i] = vo;

                            }
                            this._prmPurga.DispenserP3.Dispensar(vValores, i_Step);

                        }
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (_prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }

                        if (_parametros.PurgaSequencial)
                        {
                            //Verifica se já executou todos os circuitos de colorantes]
                            if (INDEX_CONTADOR > INDEX_ULTIMO_COLORANTE)
                            {
                                //Aqui vamos setar o novo valor do ultimosPulsposRevereso
                                for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                                {
                                    int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                    Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                                }
                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }

                            //Recupera posição do circuito do colorante
                            string nomCol = "";
                            string statusCol = "";
                            if (INDEX_CONTADOR < _prmPurga.LMDispositivos[0].Colorantes.Count)
                            {
                                int CIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;

                                //Atualiza interface
                                nomCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Nome;
                                statusCol =
                                    Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");


                                //Dispara thread para enviar dados ao dispositivo
                                Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                        CIRCUITO - 1,
                                        _prmPurga.LMDispositivos[0].PulsoHorario[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Velocidade[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Aceleracao[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].Delay[CIRCUITO - 1],
                                        _prmPurga.LMDispositivos[0].PulsoReverso[CIRCUITO - 1], i_Step)
                                    );

                                if (_parametros.ControlarNivel)
                                {
                                    //Operar.AbaterColorante(CIRCUITO, _prmPurga.Volume);
                                    Operar.AbaterColorante(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                }
                                //Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.Volume);
                                Util.ObjectRecircular.UpdateVolumeDosado(CIRCUITO, _prmPurga.LMDispositivos[0].VolumeDosado[CIRCUITO - 1]);
                                //Incrementa contador
                                INDEX_CONTADOR++;
                            }
                           
                            if (nomCol != "")
                            {
                                lblStatus.Text = nomCol;
                                lblSubStatus.Text = statusCol;
                            }

                        }
                        else
                        {
                            for (INDEX_CONTADOR = 0; INDEX_CONTADOR <= INDEX_ULTIMO_COLORANTE; INDEX_CONTADOR++)
                            {
                                int nCIRCUITO = _prmPurga.LMDispositivos[0].Colorantes[INDEX_CONTADOR].Circuito;
                                Util.ObjectCalibragem.UpdatePulsosRev(nCIRCUITO, _prmPurga.LMDispositivos[0].PulsoReverso[nCIRCUITO - 1]);

                            }
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
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
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
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
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
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
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        //lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                        //this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                        ////Thread.Sleep(1000);
                        //this.passosActionP3 = 17;
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
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
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta(false);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
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
            if (this._prmPurga.DispenserP3.SensorEmergencia)
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
            else if (this._prmPurga.DispenserP3.CodError > 0)
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
                    //string msg = "Usuário ocorreu algum erro na movimentação da placa. Deseja continuar este processo?" + Environment.NewLine + "Descrição: " + this._prmPurga.DispenserP3.GetDescCodError();
                    string msg = Negocio.IdiomaResxExtensao.PlacaMov_Erro_Movimentacao_Passos + this._prmPurga.DispenserP3.GetDescCodError();
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
