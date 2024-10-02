using Percolore.Core;
using Percolore.Core.Logging;
using Percolore.Core.Persistence.WindowsRegistry;
using Percolore.IOConnect.Util;
using System.ComponentModel;

namespace Percolore.IOConnect
{
	public partial class fRecircular : Form
    {
        private List<Util.ObjectRecircular> _listRecircular;       
        Util.ObjectParametros _parametros = null;
        private Button[] _circuitos = null;
        int INDEX_CKT;
        int INDEX_ULTIMO_CKT;
        int INDEX_DISP;

        private List<IDispenser> ldisp = new List<IDispenser>();

        System.ComponentModel.BackgroundWorker backgroundWorker1 = null;
        private bool isThread = false;
        private bool isRunning = false;
        bool execPurga = false;
        int index_PurgaIndividual = -1;
        private Util.ObjectCalibragem calibracao = null;
        private bool isDispensou = false;
        private ToolTip _toolTip = new ToolTip();

        private int i_Step = 0;

        fAguarde _fAguarde = null;


        public fRecircular(List<Util.ObjectRecircular> listRecircular)
        {
            InitializeComponent();

            this._toolTip.ToolTipIcon = ToolTipIcon.None;
            this._toolTip.IsBalloon = true;
            this._toolTip.ShowAlways = true;

            this._listRecircular = listRecircular;
            this._parametros = Util.ObjectParametros.Load();
                

            this._circuitos = new Button[] { btn_ckt_01, btn_ckt_02, btn_ckt_03, btn_ckt_04, btn_ckt_05, btn_ckt_06, btn_ckt_07, btn_ckt_08, btn_ckt_09, btn_ckt_10, btn_ckt_11, btn_ckt_12, btn_ckt_13, btn_ckt_14, btn_ckt_15, btn_ckt_16,
            btn_ckt_17, btn_ckt_18, btn_ckt_19, btn_ckt_20, btn_ckt_21, btn_ckt_22, btn_ckt_23, btn_ckt_24, btn_ckt_25, btn_ckt_26, btn_ckt_27, btn_ckt_28, btn_ckt_29, btn_ckt_30, btn_ckt_31, btn_ckt_32};


            //Redimensiona e posiciona form
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Location = new Point(0, 30);

            //Globalização
            lblStatus.Text = Negocio.IdiomaResxExtensao.RecircularIndividualMonit_lblStatus_Msg01;
            lblSubStatus.Text = Negocio.IdiomaResxExtensao.RecircularIndividual_lblSubStatus_Msg01;


            btnCancelar.Text = Negocio.IdiomaResxExtensao.Global_Cancelar;
            btn_Fechar.Text = Negocio.IdiomaResxExtensao.Global_Fechar;
            gbIndividual.Text = Negocio.IdiomaResxExtensao.RecircularIndividual_gbIndividual;

            //Habilitar controles
            lblStatus.Visible = true;
            progressBar.Visible = false;
            this.INDEX_CKT = -1;
            this.INDEX_ULTIMO_CKT = -1;
            gbIndividual.Enabled = true;
        }

        private int ConvertDoubletoInt(double valor)
        {
            int retorno = 0;
            string _str = valor.ToString().Replace(",", "");

            if (_str.Contains("."))
            {
                string[] strArr = _str.Split('.');
                _str = strArr[0];
            }
            int.TryParse(_str, out retorno);
            return retorno;
        }

        private void fRecircular_Load(object sender, EventArgs e)
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

			for (int i = 0; i < 16; i++)
            {
                this._circuitos[i].Enabled = (this._listRecircular[i].Habilitado);
                this._circuitos[i].Text = this._listRecircular[i]._colorante.Nome;
                if (this._listRecircular[i].Habilitado )
                {
                    if (this._listRecircular[i]._colorante.Seguidor < 0)
                    {
                        TimeSpan? ts = DateTime.Now.Subtract(this._listRecircular[i].DtInicio);
                        if (ts != null && ts.HasValue && ConvertDoubletoInt(ts.Value.TotalDays) >= this._listRecircular[i].Dias)
                        {
                            if (this._listRecircular[i].VolumeDosado > this._listRecircular[i].VolumeDin)
                            {
                                this._circuitos[i].BackColor = Color.FromArgb(6, 176, 37);
                            }
                            else
                            {
                                this._circuitos[i].BackColor = Color.Red;
                            }

                        }
                        else
                        {
                            this._circuitos[i].BackColor = Color.Orange;
                        }

                        ts = null;
                        this._toolTip.SetToolTip(this._circuitos[i], string.Format("Volume Dosado: {0} ml apartir do dia {1:dd/MM/yyyy HH:mm:ss}", this._listRecircular[i].VolumeDosado.ToString(), this._listRecircular[i].DtInicio));
                    }
                    else
                    {
                        this._circuitos[i].BackColor = Cores.Seguidor_Tom_01;
                        this._toolTip.SetToolTip(this._circuitos[i], "Seguidor do Circuito: " + this._listRecircular[i]._colorante.Seguidor.ToString());
                    }
                }
                else
                {
                    if (this._listRecircular[i]._colorante.Seguidor < 0 )
                    {
                        this._circuitos[i].BackColor = Color.Gray;
                       
                    }
                    else
                    {
                        this._circuitos[i].Enabled = true;
                        this._circuitos[i].BackColor = Cores.Seguidor_Tom_01;
                        this._toolTip.SetToolTip(this._circuitos[i], "Seguidor do Circuito: " + this._listRecircular[i]._colorante.Seguidor.ToString());

                    }
                }
                
            }
            //if (this._parametros.IdDispositivo2 == 0)
            //{
            //    for (int i = 16; i < 32; i++)
            //    {
            //        this._circuitos[i].Visible = false;
            //        this._circuitos[i].Enabled = false;
            //        this._circuitos[i].Text = this._listRecircular[i]._colorante.Nome;
            //    }
            //}
            //else
            {
                for (int i = 16; i < 32; i++)
                {
                    this._circuitos[i].Enabled = this._listRecircular[i].Habilitado;
                    this._circuitos[i].Text = this._listRecircular[i]._colorante.Nome;
                    if (this._listRecircular[i].Habilitado )
                    {
                        if (this._listRecircular[i]._colorante.Seguidor < 0)
                        {
                            TimeSpan? ts = DateTime.Now.Subtract(this._listRecircular[i].DtInicio);
                            if (ts != null && ts.HasValue && ConvertDoubletoInt(ts.Value.TotalDays) >= this._listRecircular[i].Dias)
                            {
                                if (this._listRecircular[i].VolumeDosado > this._listRecircular[i].VolumeDin)
                                {
                                    this._circuitos[i].BackColor = Color.FromArgb(6, 176, 37);
                                }
                                else
                                {
                                    this._circuitos[i].BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                this._circuitos[i].BackColor = Color.Orange;
                            }
                            ts = null;
                            this._toolTip.SetToolTip(this._circuitos[i], string.Format("Volume Dosado: {0} ml apartir do dia {1:dd/MM/yyyy HH:mm:ss}", this._listRecircular[i].VolumeDosado.ToString(), this._listRecircular[i].DtInicio));
                        }
                        else
                        {
                            this._circuitos[i].BackColor = Cores.Seguidor_Tom_01;
                            this._toolTip.SetToolTip(this._circuitos[i], "Seguidor do Circuito: " + this._listRecircular[i]._colorante.Seguidor.ToString());
                        }
                    }
                    else
                    {
                        if (this._listRecircular[i]._colorante.Seguidor < 0 )
                        {
                            this._circuitos[i].BackColor = Color.Gray;
                            
                        }
                        else
                        {
                            this._circuitos[i].Enabled = true;
                            this._circuitos[i].BackColor = Cores.Seguidor_Tom_01;
                            this._toolTip.SetToolTip(this._circuitos[i], "Seguidor do Circuito: " + this._listRecircular[i]._colorante.Seguidor.ToString());

                        }
                    }
                }
            }
        }

        private void btn_Fechar_Click(object sender, EventArgs e)
        {
            try
            {
                gerarEventoRecircular(1);
                DialogResult = DialogResult.OK;
                this.Close();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void fRecircular_FormClosed(object sender, FormClosedEventArgs e)
        {
            PausarMonitoramento();
            switch (this.DialogResult)
            {
                #region Log de processo

                case DialogResult.OK:
                    {
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Global_RecircularIndividualConcluida);

                        //Persiste data e hora de execução da purga
                        //Parametros.SetExecucaoPurga(DateTime.Now);
                        break;
                    }
                case DialogResult.Cancel:
                    {
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_33 + Negocio.IdiomaResxExtensao.Global_RecircularIndividualCancelada);
                        break;
                    }

                case DialogResult.No:
                    {
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_35 + Negocio.IdiomaResxExtensao.Global_FalhaRecircularIndividual);
                        break;
                    }

                    #endregion
            }
            if (_parametros.ViewMessageProc)
            {
                this.Invoke(new MethodInvoker(ClosePrg));
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (this.execPurga)
            {
                gerarEventoRecircular(2);
                string msg = string.Format(Negocio.IdiomaResxExtensao.Log_Cod_31 + Negocio.IdiomaResxExtensao.Global_RecircularIndividualAbortada, this._listRecircular[this.index_PurgaIndividual]._colorante.Nome, this._listRecircular[this.index_PurgaIndividual]._colorante.Circuito);
                Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
            }
            DialogResult = DialogResult.Cancel;
        }

        private void ClosedSerialDispensa()
        {
            try
            {
                //this.disp.Disconnect();
                foreach (IDispenser _disp in this.ldisp)
                {
                    _disp.Disconnect();
                }
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
                if (_parametros.ViewMessageProc)
                {
                    this.Invoke(new MethodInvoker(Wait_Message));
                }
                this.isDispensou = false;
                this.execPurga = true;
                int circuito = int.Parse(((Button)sender).Tag.ToString()) + 1;

                bool isSeguidor = false;
                for (int i = 0; i < this._listRecircular.Count; i++)
                {
                    if (circuito == this._listRecircular[i]._colorante.Circuito)
                    {
                        if(this._listRecircular[i]._colorante.Seguidor >0)
                        {
                            isSeguidor = true;
                        }
                        this.INDEX_CKT = i;
                        this.INDEX_DISP = this._listRecircular[i]._colorante.Dispositivo;
                        this.calibracao = Util.ObjectCalibragem.Load(this._listRecircular[i]._colorante.Circuito);
                    }
                }
                if (!isSeguidor)
                {
                    this.Visible = true;
                    if (execPurga)
                    {
                        this.INDEX_ULTIMO_CKT = this.INDEX_CKT;
                        this.index_PurgaIndividual = this.INDEX_CKT;

                        this.ldisp.Clear();
                        IDispenser dispenser = null;
                        IDispenser dispenser2 = null;
                        switch ((Dispositivo)_parametros.IdDispositivo)
                        {
                            case Dispositivo.Simulador:
                                {
                                    dispenser = new ModBusSimulador();
                                    break;
                                }
                            case Dispositivo.Placa_1:
                                {
                                    dispenser = new ModBusDispenser_P1();
                                    break;
                                }
                            case Dispositivo.Placa_2:
                                {
                                    dispenser = new ModBusDispenser_P2(_parametros.NomeDispositivo);
                                    break;
                                }
                            //case Dispositivo.Placa_4:
                            //    {
                            //        dispenser = new ModBusDispenser_P4(_parametros.NomeDispositivo);
                            //        break;
                            //    }
                        }

                        this.ldisp.Add(dispenser);

                        switch ((Dispositivo2)_parametros.IdDispositivo2)
                        {
                            case Dispositivo2.Nenhum:
                                {
                                    dispenser2 = null;
                                    break;
                                }
                            case Dispositivo2.Simulador:
                                {
                                    dispenser2 = new ModBusSimulador();
                                    break;
                                }
                            case Dispositivo2.Placa_2:
                                {
                                    dispenser2 = new ModBusDispenser_P2(_parametros.NomeDispositivo2);
                                    break;
                                }
                            //case Dispositivo2.Placa_4:
                            //    {
                            //        dispenser2 = new ModBusDispenser_P4(_parametros.NomeDispositivo2);
                            //        break;
                            //    }
                            default:
                                {
                                    dispenser2 = null;
                                    break;
                                }
                        }
                        if (_parametros.IdDispositivo2 != 0)
                        {
                            this.ldisp.Add(dispenser2);
                        }
                        IDispenser _disp = this.ldisp[0];
                        bool conectar = Operar.Conectar(ref _disp);
                        bool conectar2 = true;
                        if (_parametros.IdDispositivo2 != 0)
                        {
                            conectar2 = Operar.Conectar(ref dispenser2);
                        }
                        if (conectar && conectar2 && Operar.TemRecipiente(_disp))
                        {
                            execPurga = true;
                            gbIndividual.Enabled = false;
                        }
                        else
                        {
                            execPurga = false;
                            ClosedSerialDispensa();
                        }

                        if (execPurga)
                        {
                            this.Invoke(new MethodInvoker(ExecutarMonitoramento));
                        }
                        else
                        {
                            if (_parametros.ViewMessageProc)
                            {
                                this.Invoke(new MethodInvoker(atualizaGridEnable));
                            }
                        }
                        //ExecutarMonitoramento();
                    }
                    else
                    {
                        if (_parametros.ViewMessageProc)
                        {
                            this.Invoke(new MethodInvoker(atualizaGridEnable));
                        }
                    }
                }
                else
                {
                    if (_parametros.ViewMessageProc)
                    {
                        this.Invoke(new MethodInvoker(atualizaGridEnable));
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        int gerarEventoRecircular(int result, string detalhes = "")
        {
            int retorno = 0;
            try
            {
                #region gravar Evento Recircular
                Util.ObjectEventos objEvt = new Util.ObjectEventos();
                objEvt.DATAHORA = DateTime.Now;
                objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.Recircular;
                objEvt.DETALHES = result.ToString() + ";" + detalhes;
                objEvt.INTEGRADO = false;
                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    objEvt.NUMERO_SERIE = percRegistry.GetSerialNumber();
                }
                retorno = Util.ObjectEventos.InsertEvento(objEvt);
                #endregion
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			return retorno;
        }
        void MonitoramentoEvent()
        {
            try
            {
                if (this._listRecircular[this.index_PurgaIndividual]._colorante.Dispositivo == 1)
                {
                    //Verifica se dispenser está pronto
                    if (!this.ldisp[0].IsReady)
                    {
                        this.isRunning = false;
                        return;
                    }
                    //Verifica se já executou todos os circuitos de colorantes]
                    if (INDEX_CKT > INDEX_ULTIMO_CKT)
                    {
                       
                        bool mayValve = true;
                        if (ldisp[0].MayValve)
                        {
                            if (this._listRecircular[this.index_PurgaIndividual].isValve)
                            {
                                bool actionValve = false;
                                for (int iV = 0; !actionValve && iV < 3; iV++)
                                {
                                    ldisp[0].AcionaValvula(false, this._listRecircular[this.index_PurgaIndividual].Circuito);
                                    Thread.Sleep(1000);

                                    actionValve = isValvulaDos(ldisp[0], this._listRecircular[this.index_PurgaIndividual].Circuito);
                                }
                                mayValve = actionValve;
                            }
                        }

                        if (mayValve)
                        {
                            /*
                            if (this.isThread)
                            {
                                Log.Logar(
                                        TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Global_PurgaIndividualConcluida);
                            }
                            */
                            this.isDispensou = true;
                            this.isRunning = true;
                            this.execPurga = false;
                            string msg = string.Format(Negocio.IdiomaResxExtensao.Global_RecircularIndividualRealizada, this._listRecircular[this.index_PurgaIndividual]._colorante.Nome, this._listRecircular[this.index_PurgaIndividual]._colorante.Circuito);
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);

                            #region Gerar Evento Recircular

                            string detalhes_recircular = this._listRecircular[this.index_PurgaIndividual]._colorante.Circuito.ToString() + "," + this._listRecircular[this.index_PurgaIndividual]._colorante.Nome +
                                "," + Math.Round(this._listRecircular[this.index_PurgaIndividual].VolumeRecircular, 3).ToString();
                            gerarEventoRecircular(0, detalhes_recircular);

                            #endregion

                            this._listRecircular[this.index_PurgaIndividual].VolumeDosado = 0;
                            this._listRecircular[this.index_PurgaIndividual].DtInicio = DateTime.Now;
                            Util.ObjectRecircular.Persist(this._listRecircular[this.index_PurgaIndividual]);
                            PausarMonitoramento();
                        }
                        else
                        {
                            FalhaValve(false);
                        }

                       
                        //DialogResult = DialogResult.OK;
                        return;
                    }
                    //Recupera posição do circuito do colorante
                    int CIRCUITO = this._listRecircular[this.INDEX_CKT]._colorante.Circuito;

                    List<Util.ObjectColorante> lCol = Util.ObjectColorante.List().FindAll(o => o.Seguidor > 0).ToList();
                    List<Util.ObjectColorante> ncol = lCol.FindAll(o => o.Seguidor == CIRCUITO).ToList();

                    if (this._listRecircular[this.INDEX_CKT]._colorante.Dispositivo == 2)
                    {
                        if (/*(Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_4 ||*/ (Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador)
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                        else
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }                        
                    }
                    //int CIRCUITO = this.INDEX_CKT;
                    //Atualiza interface
                    lblStatus.Text =
                        Negocio.IdiomaResxExtensao.RecircularIndividual_lblStatus_Msg02 + " " + this._listRecircular[this.INDEX_CKT]._colorante.Nome;
                    lblSubStatus.Text =
                        Negocio.IdiomaResxExtensao.RecircularIndividual_lblSubStatus_Msg02 + " " + this._listRecircular[this.INDEX_CKT]._colorante.Circuito.ToString("00");

                    ValoresVO _valores = Operar.Parser(this._listRecircular[this.INDEX_CKT].VolumeRecircular, this.calibracao.Valores, this.calibracao.UltimoPulsoReverso);
                    if (ncol != null && ncol.Count > 0)
                    {
                        int[] n_motor = new int[ncol.Count + 1];

                        n_motor[0] = CIRCUITO - 1;
                        for (int i = 0; i < ncol.Count; i++)
                        {
                            n_motor[i + 1] = (ncol[i].Circuito - 1);
                            
                        }
                        bool mayValve = true;
                        if(ldisp[0].MayValve)
                        {
                            if(this._listRecircular[this.INDEX_CKT].isValve)
                            {
                                bool actionValve = false;
                                for (int iV = 0; !actionValve && iV < 3; iV++)
                                {
                                    ldisp[0].AcionaValvula(true, this._listRecircular[this.INDEX_CKT].Circuito);
                                    Thread.Sleep(1000);

                                    actionValve = isValvulaRec(ldisp[0], this._listRecircular[this.INDEX_CKT].Circuito);
                                }
                                mayValve = actionValve;
                            }
                        }

                        if (mayValve)
                        {
                            //Dispara thread para enviar dados ao dispositivo
                            Task task = Task.Factory.StartNew(() => ldisp[0].Dispensar(
                              n_motor,
                              _valores.PulsoHorario,
                              _valores.Velocidade,
                              _valores.Aceleracao,
                              _valores.Delay,
                              _valores.PulsoReverso, i_Step)
                            );
                        }
                        else
                        {
                            FalhaValve(true);
                        }
                    }
                    else
                    {
                        bool mayValve = true;
                        if (ldisp[0].MayValve)
                        {
                            if (this._listRecircular[this.INDEX_CKT].isValve)
                            {
                                bool actionValve = false;
                                for (int iV = 0; !actionValve && iV < 3; iV++)
                                {
                                    ldisp[0].AcionaValvula(true, this._listRecircular[this.INDEX_CKT].Circuito);
                                    Thread.Sleep(1000);

                                    actionValve = isValvulaRec(ldisp[0], this._listRecircular[this.INDEX_CKT].Circuito);
                                }
                                mayValve = actionValve;
                            }
                        }

                        if (mayValve)
                        {
                            //Dispara thread para enviar dados ao dispositivo
                            Task task = Task.Factory.StartNew(() => ldisp[0].Dispensar(
                                CIRCUITO - 1,
                                _valores.PulsoHorario,
                                _valores.Velocidade,
                                _valores.Aceleracao,
                                _valores.Delay,
                                _valores.PulsoReverso, i_Step)
                            );
                        }
                        else
                        {
                            FalhaValve(true);
                        }

                        //if (_parametros.ControlarNivel)
                        //{
                        //    Operar.AbaterColorante(this._listRecircular[this.INDEX_CKT]._colorante.Circuito, this._listRecircular[this.INDEX_CKT].VolumeRecircular);
                        //}


                    }
                }
                else if(this._listRecircular[this.index_PurgaIndividual]._colorante.Dispositivo == 2)
                {
                    //Verifica se dispenser está pronto
                    if (!this.ldisp[1].IsReady)
                    {
                        this.isRunning = false;
                        return;
                    }

                    //Verifica se já executou todos os circuitos de colorantes]
                    if (INDEX_CKT > INDEX_ULTIMO_CKT)
                    {
                        /*
                        if (this.isThread)
                        {
                            Log.Logar(
                                    TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Global_PurgaIndividualConcluida);
                        }
                        */
                        bool mayValve = true;
                        if (ldisp[1].MayValve)
                        {
                            if (this._listRecircular[this.index_PurgaIndividual].isValve)
                            {                               
                                bool actionValve = false;
                                for (int iV = 0; !actionValve && iV < 3; iV++)
                                {
                                    ldisp[1].AcionaValvula(false, this._listRecircular[this.index_PurgaIndividual].Circuito);
                                    Thread.Sleep(1000);

                                    actionValve = isValvulaDos(ldisp[1], this._listRecircular[this.index_PurgaIndividual].Circuito);
                                }
                                mayValve = actionValve;
                            }
                        }

                        if (mayValve)
                        {
                            this.isDispensou = true;
                            this.isRunning = true;
                            this.execPurga = false;
                            string msg = string.Format(Negocio.IdiomaResxExtensao.Global_RecircularIndividualRealizada, this._listRecircular[this.index_PurgaIndividual]._colorante.Nome, this._listRecircular[this.index_PurgaIndividual]._colorante.Circuito);
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);

                            string detalhes_recircular = this._listRecircular[this.index_PurgaIndividual]._colorante.Circuito.ToString() + "," + this._listRecircular[this.index_PurgaIndividual]._colorante.Nome +
                                "," + Math.Round(this._listRecircular[this.index_PurgaIndividual].VolumeRecircular, 3).ToString();
                            gerarEventoRecircular(0, detalhes_recircular);

                            this._listRecircular[this.index_PurgaIndividual].VolumeDosado = 0;
                            this._listRecircular[this.index_PurgaIndividual].DtInicio = DateTime.Now;
                            Util.ObjectRecircular.Persist(this._listRecircular[this.index_PurgaIndividual]);
                            PausarMonitoramento();
                        }
                        else
                        {
                            FalhaValve(false);
                        }


                        //DialogResult = DialogResult.OK;
                        return;
                    }
                    //Recupera posição do circuito do colorante
                    int CIRCUITO = this._listRecircular[this.INDEX_CKT]._colorante.Circuito;

                    List<Util.ObjectColorante> lCol = Util.ObjectColorante.List().FindAll(o => o.Seguidor > 0).ToList();
                    List<Util.ObjectColorante> ncol = lCol.FindAll(o => o.Seguidor == CIRCUITO).ToList();


                    if (this._listRecircular[this.INDEX_CKT]._colorante.Dispositivo == 2)
                    {
                        if (/*(Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_4 || */(Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador)
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                        else
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                    }
                    //int CIRCUITO = this.INDEX_CKT;
                    //Atualiza interface
                    lblStatus.Text =
                        Negocio.IdiomaResxExtensao.RecircularIndividual_lblStatus_Msg02 + " " + this._listRecircular[this.INDEX_CKT]._colorante.Nome;
                    lblSubStatus.Text =
                        Negocio.IdiomaResxExtensao.RecircularIndividual_lblSubStatus_Msg02 + " " + this._listRecircular[this.INDEX_CKT]._colorante.Circuito.ToString("00");

                    ValoresVO _valores = Operar.Parser(this._listRecircular[this.INDEX_CKT].VolumeRecircular, this.calibracao.Valores, this.calibracao.UltimoPulsoReverso);
                    //Dispara thread para enviar dados ao dispositivo
                    if (ncol != null && ncol.Count > 0)
                    {
                        int[] n_motor = new int[ncol.Count + 1];

                        n_motor[0] = CIRCUITO -1;
                        int index_p2 = 16;
                        if (/*(Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_4 ||*/ (Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador)
                        {
                            index_p2 =  16;
                        }
                        
                        for (int i = 0; i < ncol.Count; i++)
                        {
                            n_motor[i + 1] = ((ncol[i].Circuito - index_p2) - 1);
                        }
                        bool mayValve = true;
                        if (ldisp[1].MayValve)
                        {
                            if (this._listRecircular[this.INDEX_CKT].isValve)
                            {                               
                                if (this._listRecircular[this.INDEX_CKT].isValve)
                                {
                                    bool actionValve = false;
                                    for (int iV = 0; !actionValve && iV < 3; iV++)
                                    {
                                        ldisp[1].AcionaValvula(true, this._listRecircular[this.INDEX_CKT].Circuito);
                                        Thread.Sleep(1000);

                                        actionValve = isValvulaRec(ldisp[1], this._listRecircular[this.INDEX_CKT].Circuito);
                                    }
                                    mayValve = actionValve;
                                }
                            }
                        }
                        if (mayValve)
                        {
                            //Dispara thread para enviar dados ao dispositivo
                            Task task = Task.Factory.StartNew(() => ldisp[1].Dispensar(
                                n_motor,
                                _valores.PulsoHorario,
                                _valores.Velocidade,
                                _valores.Aceleracao,
                                _valores.Delay,
                                _valores.PulsoReverso, i_Step)
                            );
                        }
                        else
                        {
                            FalhaValve(true);
                        }
                    }
                    else
                    {
                        bool mayValve = true;
                        if (ldisp[1].MayValve)
                        {
                            if (this._listRecircular[this.INDEX_CKT].isValve)
                            {
                                if (this._listRecircular[this.INDEX_CKT].isValve)
                                {
                                    bool actionValve = false;
                                    for (int iV = 0; !actionValve && iV < 3; iV++)
                                    {
                                        ldisp[1].AcionaValvula(true, this._listRecircular[this.INDEX_CKT].Circuito);
                                        Thread.Sleep(1000);

                                        actionValve = isValvulaRec(ldisp[1], this._listRecircular[this.INDEX_CKT].Circuito);
                                    }
                                    mayValve = actionValve;
                                }
                            }
                        }
                        if (mayValve)
                        {
                            Task task = Task.Factory.StartNew(() => ldisp[1].Dispensar(
                                CIRCUITO - 1,
                                _valores.PulsoHorario,
                                _valores.Velocidade,
                                _valores.Aceleracao,
                                _valores.Delay,
                                _valores.PulsoReverso, i_Step)
                            );
                        }
                        else
                        {
                            FalhaValve(true);
                        }

                        //if (_parametros.ControlarNivel)
                        //{
                        //    Operar.AbaterColorante(this._listRecircular[this.INDEX_CKT]._colorante.Circuito, this._listRecircular[this.INDEX_CKT].VolumeRecircular);

                        //}


                    }
                }

                //Incrementa contador
                this.INDEX_CKT++;
                this.isRunning = false;

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                if (!this.isDispensou)
                {
                    Falha(ex);
                }
            }
        }

        bool isValvulaRec(IDispenser disp, int circuito)
        {
            bool retorno = true;
            Task<Modbus.StatusValvulas> task = Task.Factory.StartNew(() => disp.getStatusValvulas());
            Modbus.StatusValvulas stVal = task.Result;
            switch (circuito)
            {
                case 1:
                    {
                        if (!stVal.Circuito_1)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 2:
                    {
                        if (!stVal.Circuito_2)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 3:
                    {
                        if (!stVal.Circuito_3)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 4:
                    {
                        if (!stVal.Circuito_4)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 5:
                    {
                        if (!stVal.Circuito_5)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 6:
                    {
                        if (!stVal.Circuito_6)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 7:
                    {
                        if (!stVal.Circuito_7)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 8:
                    {
                        if (!stVal.Circuito_8)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 9:
                    {
                        if (!stVal.Circuito_9)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 10:
                    {
                        if (!stVal.Circuito_10)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 11:
                    {
                        if (!stVal.Circuito_11)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 12:
                    {
                        if (!stVal.Circuito_12)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 13:
                    {
                        if (!stVal.Circuito_13)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 14:
                    {
                        if (!stVal.Circuito_14)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 15:
                    {
                        if (!stVal.Circuito_15)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 16:
                    {
                        if (!stVal.Circuito_16)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 17:
                    {
                        if (!stVal.Circuito_17)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 18:
                    {
                        if (!stVal.Circuito_18)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 19:
                    {
                        if (!stVal.Circuito_19)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 20:
                    {
                        if (!stVal.Circuito_20)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 21:
                    {
                        if (!stVal.Circuito_21)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 22:
                    {
                        if (!stVal.Circuito_22)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 23:
                    {
                        if (!stVal.Circuito_23)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 24:
                    {
                        if (!stVal.Circuito_24)
                        {
                            retorno = false;
                        }
                        break;
                    }
                default:
                    {
                        retorno = false;
                        break;
                    }
            }
            return retorno;

        }

        bool isValvulaDos(IDispenser disp, int circuito)
        {
            bool retorno = true;
            Task<Modbus.StatusValvulas> task = Task.Factory.StartNew(() => disp.getStatusValvulas());
            Modbus.StatusValvulas stVal = task.Result;
            switch (circuito)
            {
                case 1:
                    {
                        if (stVal.Circuito_1)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 2:
                    {
                        if (stVal.Circuito_2)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 3:
                    {
                        if (stVal.Circuito_3)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 4:
                    {
                        if (!stVal.Circuito_4)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 5:
                    {
                        if (stVal.Circuito_5)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 6:
                    {
                        if (stVal.Circuito_6)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 7:
                    {
                        if (stVal.Circuito_7)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 8:
                    {
                        if (stVal.Circuito_8)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 9:
                    {
                        if (stVal.Circuito_9)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 10:
                    {
                        if (stVal.Circuito_10)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 11:
                    {
                        if (stVal.Circuito_11)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 12:
                    {
                        if (stVal.Circuito_12)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 13:
                    {
                        if (stVal.Circuito_13)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 14:
                    {
                        if (stVal.Circuito_14)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 15:
                    {
                        if (stVal.Circuito_15)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 16:
                    {
                        if (stVal.Circuito_16)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 17:
                    {
                        if (stVal.Circuito_17)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 18:
                    {
                        if (stVal.Circuito_18)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 19:
                    {
                        if (stVal.Circuito_19)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 20:
                    {
                        if (stVal.Circuito_20)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 21:
                    {
                        if (stVal.Circuito_21)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 22:
                    {
                        if (stVal.Circuito_22)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 23:
                    {
                        if (stVal.Circuito_23)
                        {
                            retorno = false;
                        }
                        break;
                    }
                case 24:
                    {
                        if (stVal.Circuito_24)
                        {
                            retorno = false;
                        }
                        break;
                    }
                default:
                    {
                        retorno = false;
                        break;
                    }
            }
            return retorno;

        }
        #region Métodos privados

        void Falha(Exception ex)
        {
            PausarMonitoramento();
            gerarEventoRecircular(3);
            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
            {
                m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + ex.Message);
            }

            DialogResult = DialogResult.No;
        }

        void FalhaValve(bool action)
        {
            PausarMonitoramento();
          
            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
            {
                if (action)
                {
                    m.ShowDialog("Falha Válvula não acionada para posição de Recirculação");
                }
                else
                {
                    m.ShowDialog("Falha Válvula não acionada para posição de Dosagem");
                }
            }
           
        }

        void PausarMonitoramento()
        {
            try
            {
                this.isThread = false;
                this.isRunning = true;
                lblStatus.Text = Negocio.IdiomaResxExtensao.RecircularIndividualMonit_lblStatus_Msg01;
                lblSubStatus.Text = Negocio.IdiomaResxExtensao.RecircularIndividual_lblSubStatus_Msg01;

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
                        lblStatus.Text = Negocio.IdiomaResxExtensao.RecircularIndividualMonit_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.RecircularIndividual_lblSubStatus_Msg01;
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
                        try
                        {
                            if (!this.isRunning)
                            {
                                this.isRunning = true;
                                this.Invoke(new MethodInvoker(MonitoramentoEvent));
                                Thread.Sleep(500);
                            }
                        }
						catch (Exception ex)
						{
							LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
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
                if (this.isDispensou)
                {
                    this.isDispensou = false;
                    this._circuitos[this.index_PurgaIndividual].BackColor = Color.FromArgb(6, 176, 37);
                    this._toolTip.SetToolTip(this._circuitos[this.index_PurgaIndividual], string.Format("Volume Dosado: {0} ml apartir do dia {1:dd/MM/yyyy HH:mm:ss}", this._listRecircular[this.index_PurgaIndividual].VolumeDosado.ToString(), this._listRecircular[this.index_PurgaIndividual].DtInicio));
                }
                
                if (_parametros.ViewMessageProc)
                {
                    this.Invoke(new MethodInvoker(ClosePrg));
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        #endregion

        private void Wait_Message()
        {
            try
            {
                if (this._fAguarde == null)
                {
                    this._fAguarde = new fAguarde(Negocio.IdiomaResxExtensao.Global_Aguarde_ProgressBar);
                    this._fAguarde.OnClosedEvent += new CloseWindows(ClosedProgressBar);
                    this._fAguarde.Show();
                    this._fAguarde.ExecutarMonitoramento();
                    this._fAguarde._TimerDelay = 330;
                    Application.DoEvents();
                }
                else
                {
                    this._fAguarde._TimerDelay = 330;
                    this._fAguarde.Focus();
                }
                Thread.Sleep(1500);
                //this.Invoke(new MethodInvoker(WaitIsrunning));
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void ClosedProgressBar()
        {
            this._fAguarde = null;
        }

        private void ClosePrg()
        {
            try
            {
                if (this._fAguarde != null)
                {
                    this._fAguarde.PausarMonitoramento();
                    this._fAguarde.Close();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
    }
}