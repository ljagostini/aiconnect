﻿using Percolore.Core;
using Percolore.Core.Logging;
using Percolore.IOConnect.Util;
using System.ComponentModel;
using System.Globalization;
using System.IO.Ports;

namespace Percolore.IOConnect
{
	public partial class fPrimeiraPesagem : Form
    {
        private int _motor;
        private int _circuito;
        public ValoresVO _valores = null;
        private Util.ObjectCalibragem _calibracao;
        private bool _redefinirpulsos;
        private double _massaMedia = 0;
        private double _desvioMedio;

        private Util.ObjectParametros _parametros = null;

        #region Propriedades
        
        public bool RedefinirPulsos
        {
            get { return _redefinirpulsos; }
        }

        public double MassaMedia
        {
            get { return _massaMedia; }
        }

        public double DesvioMedio
        {
            get { return _desvioMedio; }
        }



        #endregion

        private bool isCheckedBalanca = false;

        System.ComponentModel.BackgroundWorker backgroundWorker1 = null;
        private bool isThread = false;
        private bool isRunning = false;

        private Negocio.Balanca_BEL balanca_bel = new Negocio.Balanca_BEL();
        fAguarde _fAguarde = null;

        private IDispenser disp = null;
        private bool IsDispensou = false;
        private IDispenser _disp = null;

        System.ComponentModel.BackgroundWorker backgroundWorker2 = null;
        private bool isThread2 = false;
        private bool isRunning2 = false;


        private ModBusDispenserMover_P3 modBusDispenser_P3 = null;
        System.ComponentModel.BackgroundWorker backgroundWorkerP3 = null;
        private bool isThreadP3 = false;
        private bool isRunningP3 = false;

        private int tipoActionP3 = -1;
        private int passosActionP3 = 0;
        private int qtdTrocaRecipiente = 0;
        private int counterFalha = 0;
        private bool isNewOperacao = false;

        private bool isSeguidor = false;
        private List<Util.ObjectColorante> colorantes_Seg = null;

        private int i_Step = 0;

        public fPrimeiraPesagem(int motor, int circuito)
        {
            InitializeComponent();
            _motor = motor -1;
            _circuito = circuito;
        }
        #region Eventos
        private void fPrimeiraPesagem_Load(object sender, EventArgs e)
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

			this.Text = Negocio.IdiomaResxExtensao.Pesagem_Titulo;
            this.lblPesagemLegendaVolume.Text = Negocio.IdiomaResxExtensao.Pesagem_lblPesagemLegendaVolume;
            this.lblPesagemLegendaMassaIdeal.Text = Negocio.IdiomaResxExtensao.Pesagem_lblPesagemLegendaMassaIdeal;
            this.lblPesagemMassa.Text = Negocio.IdiomaResxExtensao.Pesagem_lblPesagemMassa;
            this.lblPesagemDesvio.Text = Negocio.IdiomaResxExtensao.Pesagem_lblPesagemDesvio;
            this.btnPesagemDispensar.Text = Negocio.IdiomaResxExtensao.Pesagem_btnPesagemDispensar;
            this.btnPesagemAdicionar.Text = Negocio.IdiomaResxExtensao.Pesagem_btnPesagemAdicionar;
            //this.chkPesagemRedefinirPulsos.Text = Negocio.IdiomaResxExtensao.Pesagem_chkPesagemRedefinirPulsos;
            //if (Negocio.IdiomaResxExtensao.Pesagem_chkPesagemRedefinirPulsos)
            this.btnPesagemConfirmar.Text = Negocio.IdiomaResxExtensao.Pesagem_btnPesagemConfirmar;

            //txtPesagemVolume.Text = _valores.Volume.ToString();

            rd_pulsos.Text = Negocio.IdiomaResxExtensao.Pesagem_rd_pulsos;
            chb_balanca_Pesagem.Text = Negocio.IdiomaResxExtensao.Pesagem_chb_balanca_Pesagem;
            gbComunicacaoBalanca.Text = Negocio.IdiomaResxExtensao.Pesagem_gbComunicacaoBalanca;
            


            #region Configura ListView

            listViewPesagem.FullRowSelect = true;
            listViewPesagem.GridLines = true;
            listViewPesagem.View = View.Details;
            listViewPesagem.Font = new Font("Segoe UI Light", 24);
            listViewPesagem.HeaderStyle = ColumnHeaderStyle.None;

            listViewPesagem.Columns.Add("", 0, HorizontalAlignment.Center);
            listViewPesagem.Columns.Add("", listViewPesagem.Width / 2, HorizontalAlignment.Right);
            listViewPesagem.Columns.Add("", listViewPesagem.Width / 2, HorizontalAlignment.Right);

            lblPesagemMassaIdeal.Text = "0.0 g";

            #endregion

            _parametros = Util.ObjectParametros.Load();
            try
            {
                switch ((Dispositivo)this._parametros.IdDispositivo)
                {
                    case Dispositivo.Placa_3:
                        {
                            this.isCheckedBalanca = false;
                            chb_balanca_Pesagem.Checked = false;
                            chb_balanca_Pesagem.Visible = false;
                            chb_balanca_Pesagem.Enabled = false;
                            break;
                        }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			updateTeclado();
        }

        private void updateTeclado()
        {
            bool ischek = false;
            if (_parametros != null)
            {
                ischek = _parametros.HabilitarTecladoVirtual;
            }
            txtPesagemDesvio.isTecladoShow = ischek;
            txtPesagemMassaVerificada.isTecladoShow = ischek;
            txtPesagemVolume.isTecladoShow = ischek;

            if (_parametros != null)
            {
                ischek = _parametros.HabilitarTouchScrenn;
            }
            txtPesagemDesvio.isTouchScrenn = ischek;
            txtPesagemMassaVerificada.isTouchScrenn = ischek;
            txtPesagemVolume.isTouchScrenn = ischek;
        }

        private void txtMassaVerificada_TextChanged(object sender, EventArgs e)
        {
            if (_valores != null)
            {
                double massaVerificada = 0;
                double.TryParse(txtPesagemMassaVerificada.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out massaVerificada);
                double desvio = Operar.CalcularDesvio(massaVerificada, _valores.MassaIdeal);
                txtPesagemDesvio.Text = string.Format("{0:P2}", desvio);
            }
        }

        private void ClosedProgressBar()
        {
            this._fAguarde = null;
        }

        private void btnDispensar_Click(object sender, EventArgs e)
        {
            try
            {

                if (this._valores != null)
                {
                    if (this._parametros.ViewMessageProc)
                    {
                        if (this._fAguarde == null)
                        {
                            this._fAguarde = new fAguarde(Negocio.IdiomaResxExtensao.Global_Aguarde_ProgressBar);
                            this._fAguarde.OnClosedEvent += new CloseWindows(ClosedProgressBar);
                            this._fAguarde.Show();
                            this._fAguarde.ExecutarMonitoramento();
                            this._fAguarde._TimerDelay = 330;

                        }
                        else
                        {
                            this._fAguarde._TimerDelay = 330;
                            this._fAguarde.Focus();
                        }
                        try
                        {
                            this.isCheckedBalanca = chb_balanca_Pesagem.Checked;

                            Thread thrd = new Thread(new ThreadStart(dispensarAguarde));
                            thrd.Start();
                        }
						catch (Exception ex)
						{
							LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
						}
					}
                    else
                    {
                        this.Invoke(new MethodInvoker(dispensarEvent));
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + ex.Message);
                }
            }
        }


        private void dispensarAguarde()
        {
            try
            {
                btnPesagemDispensar.Enabled = false;

                while (!this._fAguarde.IsRunning)
                {
                    Thread.Sleep(1000);
                }
                if (this.isCheckedBalanca)
                {
                    byte[] response = new byte[15];
                    byte[] b_tara = new byte[1];
                    byte[] b_peso = new byte[1];
                    b_tara[0] = (byte)this.balanca_bel.b_tara_Balanca;
                    this.balanca_bel.WriteSerialPort(b_tara);
                    
                    Thread.Sleep(10000);
                    b_peso[0] = (byte)this.balanca_bel.b_peso_balanca;
                    this.balanca_bel.WriteSerialPort(b_peso);
                    
                    //for (int i = 0; i < 5; i++)
                    //{
                    //    b_peso[0] = (byte)this.balanca_bel.b_peso_balanca;
                    //    this.balanca_bel.WriteSerialPort(b_peso);
                    //    Thread.Sleep(4000);
                    //    if(this.balanca_bel.isTerminouRead)
                    //    {
                    //        break;
                    //    }
                    //}
                    response[0] = (byte)0;
                    this.balanca_bel.GetResponse(ref response);
                    this.balanca_bel.SetValues(response, true);
                }
                this.Invoke(new MethodInvoker(dispensarEvent));

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + ex.Message);
                }
                this.Invoke(new MethodInvoker(ClosePrg));
                this.Invoke(new MethodInvoker(atualizaBtnDipensa));
            }
        }

        private void dispensarEvent()
        {
            try
            {
                //bool dispensou = true;
                this._parametros = Util.ObjectParametros.Load();
                this._calibracao = Util.ObjectCalibragem.Load(_motor + 1);

                if (this._parametros.ControlarExecucaoPurga)
                {
                    #region [Controle de execução de purga]

                    if (Operar.TemPurgaPendente())
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.Global_PurgaPendente);
                        }
                        this.Invoke(new MethodInvoker(dispensarBL));

                        return;

                    }

                    #endregion
                }
                switch ((Dispositivo)this._parametros.IdDispositivo)
                {
                    case Dispositivo.Simulador:
                        {
                            int milissegundosPorCircuito = 3000;

                            //Simula tempo de execução. 
                            Thread.Sleep(milissegundosPorCircuito);
                            this.Invoke(new MethodInvoker(dispensarBL));

                            break;
                        }
                    case Dispositivo.Placa_1:
                        {
                            if (this.disp == null)
                            {
                                this.disp = new ModBusDispenser_P1();
                            }

                            if (!Operar.Conectar(ref this.disp))
                            {
                                #region Não conectou e usuário cancelou
                                string descricaoDAT = Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao;
                                string mensagemUsuario = Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao;

                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    bool confirma = m.ShowDialog(mensagemUsuario);
                                }
                                disp.Disconnect();
                                //dispensou = false;
                                this.Invoke(new MethodInvoker(dispensarBL));
                                break;
                                #endregion
                            }
                            //Thread.Sleep(500);
                            if (!Operar.TemRecipiente(this.disp))
                            {
                                #region Não há recipiente e usuário cancelou

                                string mensagemUsuario = Negocio.IdiomaResxExtensao.Global_Informacao_RecipienteNaoPosicionado_2;

                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    bool confirma = m.ShowDialog(mensagemUsuario);
                                }
                                this.Invoke(new MethodInvoker(dispensarBL));
                                //dispensou = false;
                                break;
                                #endregion
                            }
                            //Thread.Sleep(500);
                            //this.disp.Disconnect();
                            //Operar.Calibrar_P1(_motor, _valores);
                            ExecutarMonitoramento2();
                            break;
                        }
                    case Dispositivo.Placa_2:
                        {
                            if (this._motor <= 15)
                            {
                                if (this.disp == null)
                                {
                                    this.disp = new ModBusDispenser_P2(_parametros.NomeDispositivo);
                                }
                            }
                            else
                            {
                                if (this.disp == null)
                                {
                                    this.disp = new ModBusDispenser_P2(_parametros.NomeDispositivo2);
                                }
                            }
                            //bool _conectou = false;
                            //for(int _iC = 0; _iC< 3; _iC++)
                            //{
                            //    _conectou = Operar.Conectar(ref disp, false);

                            //}
                            if (!Operar.Conectar(ref this.disp, false))
                            {
                                #region Não conectou e usuário cancelou
                                string descricaoDAT = Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao;
                                string mensagemUsuario = Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao;

                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    bool confirma = m.ShowDialog(mensagemUsuario);
                                }
                                this.disp.Disconnect();
                                //dispensou = false;
                                this.Invoke(new MethodInvoker(dispensarBL));
                                break;
                                #endregion
                            }
                            Thread.Sleep(500);
                            if (this._motor <= 15)
                            {
                                if (!Operar.TemRecipiente(this.disp))
                                {
                                    #region Não há recipiente e usuário cancelou

                                    string mensagemUsuario = Negocio.IdiomaResxExtensao.Global_Informacao_RecipienteNaoPosicionado_2;

                                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                    {
                                        bool confirma = m.ShowDialog(mensagemUsuario);
                                    }
                                    this.disp.Disconnect();
                                    this.Invoke(new MethodInvoker(dispensarBL));
                                    //dispensou = false;
                                    break;
                                    #endregion
                                }
                            }
                            else
                            {
                                if (this._disp == null)
                                {
                                    this._disp = new ModBusDispenser_P2(_parametros.NomeDispositivo);
                                }
                                if (!Operar.IsConnected(ref this._disp) && !Operar.Conectar(ref this._disp))
                                {
                                    #region Não conectou e usuário cancelou
                                    string descricaoDAT = Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao;
                                    string mensagemUsuario = Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao;

                                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                    {
                                        bool confirma = m.ShowDialog(mensagemUsuario);
                                    }
                                    this._disp.Disconnect();
                                    this.disp.Disconnect();
                                    if (_parametros.ViewMessageProc)
                                    {
                                        this.Invoke(new MethodInvoker(ClosePrg));
                                    }
                                    //dispensou = false;
                                    break;
                                    #endregion
                                }

                                if (!Operar.TemRecipiente(this._disp))
                                {
                                    #region Não há recipiente e usuário cancelou

                                    string mensagemUsuario = Negocio.IdiomaResxExtensao.Global_Informacao_RecipienteNaoPosicionado_2;

                                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                    {
                                        bool confirma = m.ShowDialog(mensagemUsuario);
                                    }
                                    this._disp.Disconnect();
                                    this.disp.Disconnect();
                                    if (_parametros.ViewMessageProc)
                                    {
                                        this.Invoke(new MethodInvoker(ClosePrg));
                                    }
                                    //dispensou = false;
                                    break;
                                    #endregion
                                }
                            }
                            Thread.Sleep(500);
                            //this.disp.Disconnect();
                            ExecutarMonitoramento2();
                            //Operar.Calibrar_P2(_motor, _valores);
                            break;
                        }
                    case Dispositivo.Placa_3:
                        {
                            this.modBusDispenser_P3 = new ModBusDispenserMover_P3(this._parametros.NomeDispositivo, this._parametros.NomeDispositivo_PlacaMov);
                            if (!Operar.ConectarP3(ref modBusDispenser_P3, false))
                            {
                                #region Não conectou e usuário cancelou
                                string descricaoDAT = Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao;
                                string mensagemUsuario = Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao;

                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    bool confirma = m.ShowDialog(mensagemUsuario);
                                }
                                this.modBusDispenser_P3.Disconnect();
                                this.modBusDispenser_P3.Disconnect_Mover();
                                if (_parametros.ViewMessageProc)
                                {
                                    this.Invoke(new MethodInvoker(ClosePrg));
                                }
                                //dispensou = false;
                                break;
                                #endregion
                            }
                            this.tipoActionP3 = -1;
                            this.passosActionP3 = 0;
                            if (this.modBusDispenser_P3 != null)
                            {
                                bool isPosicao = false;
                                this.modBusDispenser_P3.ReadSensores_Mover();
                                if (!(this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2))
                                {
                                    //lblStatus.Text = "Condição de placa movimentação incorreta!";
                                    //lblSubStatus.Text = "";
                                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                    {
                                        //m.ShowDialog("Condição de placa movimentação incorreta!");
                                        m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Condicao_Incorreta);
                                    }


                                    this.modBusDispenser_P3.Disconnect();
                                    this.modBusDispenser_P3.Disconnect_Mover();
                                    if (_parametros.ViewMessageProc)
                                    {
                                        this.Invoke(new MethodInvoker(ClosePrg));
                                    }
                                    break;
                                }

                                if (!this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada)
                                {
                                    this.modBusDispenser_P3.AbrirGaveta(true);
                                    for (int _i = 0; _i < 20; _i++)
                                    {
                                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                                        {
                                            _i = 20;
                                        }
                                        Thread.Sleep(500);
                                    }
                                    isPosicao = true;
                                }
                                if (!this.modBusDispenser_P3.SensorValvulaDosagem && !this.modBusDispenser_P3.SensorValvulaRecirculacao)
                                {
                                    this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                    isPosicao = true;
                                }
                                if (isPosicao)
                                {
                                    this.modBusDispenser_P3.ReadSensores_Mover();
                                    if ((!this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada) || (!this.modBusDispenser_P3.SensorValvulaDosagem && !this.modBusDispenser_P3.SensorValvulaRecirculacao))
                                    {
                                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                        {
                                            //m.ShowDialog("Condição de placa movimentação incorreta!");
                                            m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Condicao_Incorreta);
                                        }
                                        this.modBusDispenser_P3.Disconnect_Mover();
                                        this.modBusDispenser_P3.Disconnect();
                                        if (_parametros.ViewMessageProc)
                                        {
                                            this.Invoke(new MethodInvoker(ClosePrg));
                                        }
                                        break;
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
                                Thread.Sleep(500);
                                ExecutarMonitoramentoP3();
                            }
                            break;
                        }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    this.Invoke(new MethodInvoker(dispensarBL));
            }
        }

        private void dispensarBL()
        {
            try
            {
                ExecutarMonitoramento();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                if (_parametros.ViewMessageProc)
                {
                    this.Invoke(new MethodInvoker(ClosePrg));
                }
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + ex.Message);
                }
            }
        }

        private void btnAdicionar_Click(object sender, EventArgs e)
        {
            if (this._valores != null)
            {
                //Recupera massa informada pelo usuário
                double massaVerificada = 0;
                double.TryParse(txtPesagemMassaVerificada.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out massaVerificada);

                //Valida massa
                if (massaVerificada == 0)
                    return;

                //Insere na lista
                ListViewItem item;
                item = new ListViewItem(
                    new string[] { "", massaVerificada.ToString(), txtPesagemDesvio.Text });
                listViewPesagem.Items.Add(item);

                CalculaMedia();
                btnPesagemConfirmar.Enabled = true;

                txtPesagemMassaVerificada.Text = string.Empty;
                txtPesagemDesvio.Text = string.Empty;
            }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (this._valores != null)
            {
                //Redefini os pulsos da tela de calibragem para faixa de 100 mL;
                //this._redefinirpulsos = chkPesagemRedefinirPulsos.Checked;
                this._redefinirpulsos = rd_pulsos.Checked;


                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                {
                    bool confirma =
                        m.ShowDialog(Negocio.IdiomaResxExtensao.Pesagem_Confirma_MassaMedia,
                        Negocio.IdiomaResxExtensao.Global_Sim,
                        Negocio.IdiomaResxExtensao.Global_Nao);

                    if (confirma)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
        }

        private void btnDescartar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fPrimeiraPesagem_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                this.balanca_bel.CloseSerial();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			try
            {
                PausarMonitoramento();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			try
            {
                if (_parametros.ViewMessageProc)
                {
                    this.Invoke(new MethodInvoker(ClosePrg));
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			if (listViewPesagem.Items.Count == 0)
                return;
            if (this.DialogResult == DialogResult.OK)
                return;
            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
            {
                bool confirma =
                    m.ShowDialog(Negocio.IdiomaResxExtensao.Pesagem_Confirma_Descarte,
                    Negocio.IdiomaResxExtensao.Global_Sim,
                    Negocio.IdiomaResxExtensao.Global_Nao);

                if (!confirma)
                    e.Cancel = true;
            }
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            listViewPesagem.SelectedItems[0].Remove();
            CalculaMedia();
        }

        #endregion

        #region mdétodos

        private void CalculaMedia()
        {
            if (listViewPesagem.Items.Count == 0)
            {
                _massaMedia = 0;
                _desvioMedio = 0;
                btnPesagemConfirmar.Enabled = false;
            }
            else
            {
                //Soma massa e desvio
                double massaTotal = 0;
                double desvioTotal = 0;
                foreach (ListViewItem row in listViewPesagem.Items)
                {
                    massaTotal += double.Parse(row.SubItems[1].Text);

                    string entrada = row.SubItems[2].Text.Replace("%", string.Empty);
                    desvioTotal += double.Parse(entrada) / 100;
                }

                //Calcula médias
                _massaMedia = massaTotal / listViewPesagem.Items.Count;
                _desvioMedio = desvioTotal / listViewPesagem.Items.Count;
            }

            //Exibe ,édias
            lblMassaMedia.Text = _massaMedia.ToString();
            lblDesvioMedio.Text = string.Format("{0:P2}", _desvioMedio);
        }

        #endregion

        private void txtPesagemVolume_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double VOLUME = txtPesagemVolume.ToDouble();                    
                Util.ObjectCalibragem c = Util.ObjectCalibragem.Load(_motor+1);
                Util.ObjectColorante col = Util.ObjectColorante.Load(_circuito);
                List<ValoresVO> calibragem = c.Valores;
                //[Define valores para calibragem]
                _valores = Operar.Parser(VOLUME, calibragem, c.UltimoPulsoReverso);
                if (_valores != null)
                {
                    _valores.MassaIdeal = col.MassaEspecifica * _valores.Volume;
                    double mId = col.MassaEspecifica * _valores.Volume;
                    lblPesagemMassaIdeal.Text = mId.ToString("N5") + " g";
                }
                else
                {
                    lblPesagemMassaIdeal.Text = "0.00000 g";
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    _valores = null;
            }

            try
            {
                if (listViewPesagem.Items.Count > 0)
                {
                    listViewPesagem.Items.Clear();
                    CalculaMedia();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void chb_balanca_Pesagem_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox chk = (CheckBox)sender;
                if(chk.Checked)
                {
                    gbComunicacaoBalanca.Visible = true;
                    atualizaPortasComunicacaoBalanca();
                }
                else
                {
                    gbComunicacaoBalanca.Visible = false;
                    cmbPortaSerial.SelectedIndex = -1;
                    this.balanca_bel.CloseSerial();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void atualizaPortasComunicacaoBalanca()
        {
            try
            {
                string[] portas = SerialPort.GetPortNames();
                List<string> lPortas = new List<string>();
                foreach(string _str in portas)
                {
                    string np = ValidadeNameSerialPorta(_str);
                    if(np != null && np.Length > 1)
                    {
                        lPortas.Add(np);
                    }
                }
                cmbPortaSerial.Items.Clear();
                foreach (string _str in lPortas)
                {
                    cmbPortaSerial.Items.Add(_str);
                }
                if(cmbPortaSerial.Items.Count >0 )
                {
                    cmbPortaSerial.SelectedIndex = -1;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private string ValidadeNameSerialPorta(string _str)
        {
            string retorno = "";
            foreach (char c in _str)
            {
                int _ic = (int)c;
                if((_ic >= 48 && _ic <= 57) || (_ic >= 65 && _ic <= 90) || (_ic >= 97 && _ic <= 122))
                {
                    retorno += c.ToString();
                }
            }
            return retorno;
        }

        private void cmbPortaSerial_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbPortaSerial.SelectedIndex >= 0)
                {
                    this.balanca_bel._str_Serial = cmbPortaSerial.Text;
                    this.balanca_bel.Start_Serial();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        void PausarMonitoramento()
        {
            try
            {
                this.isThread = false;
                if (backgroundWorker1!= null && backgroundWorker1.IsBusy && backgroundWorker1.CancellationPending == false)
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
                        try
                        {
                            if (!this.isRunning)
                            {
                                this.isRunning = true;
                                this.Invoke(new MethodInvoker(Monitoramento_Event));
                                Thread.Sleep(1500);
                            }
                        }
						catch (Exception ex)
						{
							LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
						}
					}

                    Thread.Sleep(500);
                }

                worker = null;
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

        private void Monitoramento_Event()
        {
            try
            {
                
                this.isRunning = false;                
                PausarMonitoramento();
                if (this.isCheckedBalanca)
                {
                    Thread.Sleep(6000);
                    byte[] b_w = new byte[1];
                    byte[] response = new byte[15];
                    
                    b_w[0] = (byte)this.balanca_bel.b_peso_balanca;
                    this.balanca_bel.WriteSerialPort(b_w);

                   
                    this.balanca_bel.GetResponse(ref response);
                    if (this.balanca_bel.isTerminouRead)
                    {
                        this.balanca_bel.SetValues(response);
                        this.Invoke(new MethodInvoker(AtualizarPesoTextBox));
                    }
                    else
                    {
                        if (_parametros.ViewMessageProc)
                        {
                            this.Invoke(new MethodInvoker(ClosePrg));
                        }
                    }
                }
                else
                {
                    if (_parametros.ViewMessageProc)
                    {
                        this.Invoke(new MethodInvoker(ClosePrg));
                    }
                }

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                if (_parametros.ViewMessageProc)
                {
                    this.Invoke(new MethodInvoker(ClosePrg));
                }
            }
            this.isRunning = false;
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

        private void AtualizarPesoTextBox()
        {
            try
            {
                double valorPesoFinal = this.balanca_bel.valorPeso - this.balanca_bel.valorTara;
                txtPesagemMassaVerificada.Text = valorPesoFinal.ToString();
                btnAdicionar_Click(null, null);
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


        #region Pesagem Placa

        void Monitoramento_Event2()
        {
            try
            {
                /* Verifica se dispenser está liberado.
                 * isso representa que o processo foi concluído. */
                if (!this.disp.IsReady)
                {
                    this.isRunning2 = false;
                    return;
                }
                if (this.IsDispensou)
                {
                    this.isRunning2 = false;

                    PausarMonitoramento2();
                    Util.ObjectCalibragem.UpdatePulsosRev(_motor + 1, _valores.PulsoReverso);
                    return;
                }
                int pulso_horario = _valores.PulsoHorario;
                if (this._parametros.SomarPulsoReverso)
                {
                    pulso_horario += _calibracao.UltimoPulsoReverso;
                }
                if (!this.isSeguidor)
                {
                    //Thread.Sleep(500);
                    int n_motor = _motor > 15 ? (_motor - 16) : _motor;
                    Task task = Task.Factory.StartNew(() => this.disp.Dispensar(
                                        n_motor,
                                        pulso_horario,
                                        _valores.Velocidade,
                                        _valores.Aceleracao,
                                        _valores.Delay,
                                        _valores.PulsoReverso, i_Step)
                                    );
                }

                else
                {
                    int[] n_motor = new int[this.colorantes_Seg.Count + 1];
                    if (_motor > 15)
                    {
                        n_motor[0] = _motor - 16;
                        for (int i = 0; i < this.colorantes_Seg.Count; i++)
                        {
                            n_motor[i + 1] = (this.colorantes_Seg[i].Circuito - 16) - 1;
                        }
                    }
                    else
                    {
                        n_motor[0] = _motor;
                        for (int i = 0; i < this.colorantes_Seg.Count; i++)
                        {
                            n_motor[i + 1] = (this.colorantes_Seg[i].Circuito - 1);
                        }
                    }

                    Task task = Task.Factory.StartNew(() => this.disp.Dispensar(
                                      n_motor,
                                      pulso_horario,
                                      _valores.Velocidade,
                                      _valores.Aceleracao,
                                      _valores.Delay,
                                      _valores.PulsoReverso, i_Step)
                                  );
                }

                this.IsDispensou = true;
                Thread.Sleep(1000);
                this.isRunning2 = false;
            }
            catch (Exception ex)
            {
                Falha2(ex);
            }
        }

        void Falha2(Exception ex)
		{
            LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			PausarMonitoramento2();

            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
            {
                m.ShowDialog(
                    Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + ex.Message);
            }
        }

        void PausarMonitoramento2()
        {
            this.isThread2 = false;           
            
            try
            {
                if (!this.IsDispensou)
                {
                    if (_parametros.ViewMessageProc)
                    {
                        this.Invoke(new MethodInvoker(ClosePrg));
                    }
                }
                else
                {
                    this.Invoke(new MethodInvoker(dispensarBL));
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			try
            {
                this.disp.Disconnect();
                if (this._disp != null)
                {
                    this._disp.Disconnect();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        void ExecutarMonitoramento2()
        {
            try
            {
                if (backgroundWorker2 == null)
                {
                    this.backgroundWorker2 = new BackgroundWorker();
                    this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker2_DoWork);
                    this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundWorker2_RunWorkerCompleted);
                    this.backgroundWorker2.WorkerSupportsCancellation = true;
                    this.isThread2 = true;
                    this.isRunning2 = false;
                    this.IsDispensou = false;
                    this.backgroundWorker2.RunWorkerAsync();
                }
                else
                {
                    this.backgroundWorker2 = new BackgroundWorker();
                    this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker2_DoWork);
                    this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundWorker2_RunWorkerCompleted);
                    this.backgroundWorker2.WorkerSupportsCancellation = true;
                    this.IsDispensou = false;
                    //while (backgroundWorker2.IsBusy)
                    if(!this.backgroundWorker2.IsBusy)
                    {
                        this.isThread2 = true;
                        this.isRunning2 = false;                        
                        backgroundWorker2.RunWorkerAsync();
                    }
                    else
                    {
                        PausarMonitoramento2();
                        this.Invoke(new MethodInvoker(atualizaBtnDipensa));
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        void atualizaBtnDipensa()
        {
            try
            {
                Thread.Sleep(2000);
                btnPesagemDispensar.Enabled = true;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                BackgroundWorker worker = sender as BackgroundWorker;
                while (this.isThread2)
                {
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        this.isThread2 = false;
                    }
                    else
                    {
                        try
                        {
                            if (!this.isRunning2)
                            {
                                this.isRunning2 = true;
                                this.Invoke(new MethodInvoker(Monitoramento_Event2));
                                Thread.Sleep(500);
                            }
                        }
						catch (Exception ex)
						{
							LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
						    this.isRunning2 = false;
                        }
                    }

                    Thread.Sleep(500);
                }

                worker = null;
                this.Invoke(new MethodInvoker(atualizaBtnDipensa));
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}


        private void backgroundWorker2_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    this.isThread2 = false;
                }
                else if (!(e.Error == null))
                {
                    this.isThread2 = false;
                }
                else
                {
                    this.isThread2 = true;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        #endregion


        #region Pesagem Placa 3

        void Emergencia()
        {
            PausarMonitoramentoP3();
            
            try
            {
                this.modBusDispenser_P3.Halt();
                this.modBusDispenser_P3.UnHalt();
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
                this.modBusDispenser_P3.Halt();
                this.modBusDispenser_P3.UnHalt();
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
        }


        void Monitoramento_EventP3()
        {
            try
            {
                if (modBusDispenser_P3 != null)
                {
                    trataActionP3();
                }

                Thread.Sleep(1000);
                this.isRunningP3 = false;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private int getActionP3()
        {
            int retorno = -1;
            if (this.modBusDispenser_P3.SensorEsponja && this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
               && !this.modBusDispenser_P3.SensorGavetaAberta && this.modBusDispenser_P3.SensorGavetaFechada && this.modBusDispenser_P3.SensorValvulaDosagem
               && !this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 1;
            }
            else if (this.modBusDispenser_P3.SensorEsponja && this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
                && !this.modBusDispenser_P3.SensorGavetaAberta && this.modBusDispenser_P3.SensorGavetaFechada && !this.modBusDispenser_P3.SensorValvulaDosagem
                && this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 2;
            }
            else if (this.modBusDispenser_P3.SensorEsponja && this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
                && this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada && this.modBusDispenser_P3.SensorValvulaDosagem
                && !this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 3;
            }
            else if (this.modBusDispenser_P3.SensorEsponja && this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
                && this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada && !this.modBusDispenser_P3.SensorValvulaDosagem
                && this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 4;
            }
            else if (this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && this.modBusDispenser_P3.SensorBaixoBicos && !this.modBusDispenser_P3.SensorAltoBicos
                && !this.modBusDispenser_P3.SensorGavetaAberta && this.modBusDispenser_P3.SensorGavetaFechada && !this.modBusDispenser_P3.SensorValvulaDosagem
                && this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 5;
            }
            else if (this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
                && !this.modBusDispenser_P3.SensorGavetaAberta && this.modBusDispenser_P3.SensorGavetaFechada && !this.modBusDispenser_P3.SensorValvulaDosagem
                && this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 6;
            }
            else if (this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
                && this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada && !this.modBusDispenser_P3.SensorValvulaDosagem
                && this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 7;
            }
            else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
               && !this.modBusDispenser_P3.SensorGavetaAberta && this.modBusDispenser_P3.SensorGavetaFechada && this.modBusDispenser_P3.SensorValvulaDosagem
               && !this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 8;
            }
            else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
               && !this.modBusDispenser_P3.SensorGavetaAberta && this.modBusDispenser_P3.SensorGavetaFechada && !this.modBusDispenser_P3.SensorValvulaDosagem
               && this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 9;
            }
            else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
               && this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada && this.modBusDispenser_P3.SensorValvulaDosagem
               && !this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 10;
            }
            else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
               && this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada && !this.modBusDispenser_P3.SensorValvulaDosagem
               && this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 11;
            }
            else if (this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && this.modBusDispenser_P3.SensorBaixoBicos &&
             !this.modBusDispenser_P3.SensorAltoBicos && !this.modBusDispenser_P3.SensorGavetaAberta && this.modBusDispenser_P3.SensorGavetaFechada &&
             this.modBusDispenser_P3.SensorValvulaDosagem && !this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 14;
            }
            else if (this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos &&
              this.modBusDispenser_P3.SensorAltoBicos && !this.modBusDispenser_P3.SensorGavetaAberta && this.modBusDispenser_P3.SensorGavetaFechada &&
              this.modBusDispenser_P3.SensorValvulaDosagem && !this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 15;
            }
            else if (this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos &&
              this.modBusDispenser_P3.SensorAltoBicos && this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada &&
              this.modBusDispenser_P3.SensorValvulaDosagem && !this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 16;
            }
            else if (this.modBusDispenser_P3.SensorBaixoBicos && !this.modBusDispenser_P3.SensorAltoBicos
               && this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada && this.modBusDispenser_P3.SensorValvulaDosagem
               && !this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 12;
            }
            else if (this.modBusDispenser_P3.SensorBaixoBicos && !this.modBusDispenser_P3.SensorAltoBicos
              && this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada && !this.modBusDispenser_P3.SensorValvulaDosagem
              && this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 13;
            }

            return retorno;
        }

        void PausarMonitoramentoP3()
        {
           
            this.isThreadP3 = false;
            this.tipoActionP3 = -1;

            try
            {
                if (!this.IsDispensou)
                {
                    if (_parametros.ViewMessageProc)
                    {
                        this.Invoke(new MethodInvoker(ClosePrg));
                    }
                }
                else
                {
                    this.Invoke(new MethodInvoker(dispensarBL));
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			try
            {
                this.modBusDispenser_P3.Disconnect();
                this.modBusDispenser_P3.Disconnect_Mover();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        void ExecutarMonitoramentoP3()
        {
            try
            {
                if (backgroundWorkerP3 == null)
                {
                    this.backgroundWorkerP3 = new BackgroundWorker();
                    this.backgroundWorkerP3.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorkerP3_DoWork);
                    this.backgroundWorkerP3.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundWorkerP3_RunWorkerCompleted);
                    this.backgroundWorkerP3.WorkerSupportsCancellation = true;
                    this.isThreadP3 = true;
                    this.isRunningP3 = false;
                    this.IsDispensou = false;
                    this.backgroundWorkerP3.RunWorkerAsync();
                }
                else
                {
                    this.backgroundWorkerP3 = new BackgroundWorker();
                    this.backgroundWorkerP3.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorkerP3_DoWork);
                    this.backgroundWorkerP3.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundWorkerP3_RunWorkerCompleted);
                    this.backgroundWorkerP3.WorkerSupportsCancellation = true;
                    this.IsDispensou = false;
                    if(!backgroundWorkerP3.IsBusy)
                    {
                        this.isRunningP3 = false;
                        this.isThreadP3 = true;
                        backgroundWorkerP3.RunWorkerAsync();
                    }
                    else
                    {
                        PausarMonitoramentoP3();
                        this.Invoke(new MethodInvoker(atualizaBtnDipensa));
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void backgroundWorkerP3_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                BackgroundWorker worker = sender as BackgroundWorker;
                while (this.isThreadP3)
                {
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        this.isThreadP3 = false;
                    }
                    else
                    {
                        try
                        {
                            if (!this.isRunningP3)
                            {
                                this.isRunningP3 = true;
                                this.Invoke(new MethodInvoker(Monitoramento_EventP3));
                                Thread.Sleep(1500);
                            }
                        }
						catch (Exception ex)
						{
							LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
						    this.isRunningP3 = false;
                        }
                    }

                    Thread.Sleep(500);
                }

                worker = null;
                this.Invoke(new MethodInvoker(atualizaBtnDipensa));
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}


        private void backgroundWorkerP3_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    this.isThreadP3 = false;
                }
                else if (!(e.Error == null))
                {
                    this.isThreadP3 = false;
                }
                else
                {
                    this.isThreadP3 = true;
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
                            PausarMonitoramentoP3();
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
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
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
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this.isNewOperacao = false;

                        int pulso_horario = _valores.PulsoHorario;
                        if (this._parametros.SomarPulsoReverso)
                        {
                            pulso_horario += _calibracao.UltimoPulsoReverso;
                        }
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.Dispensar(
                                            _motor,
                                            pulso_horario,
                                            _valores.Velocidade,
                                            _valores.Aceleracao,
                                            _valores.Delay,
                                            _valores.PulsoReverso, i_Step)
                                        );

                        this.IsDispensou = true;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (this.IsDispensou)
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            Util.ObjectCalibragem.UpdatePulsosRev(_motor + 1, _valores.PulsoReverso);
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 10;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
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
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramentoP3();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramentoP3();
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
                            ////bool isRec = m.ShowDialog("Recipiente está na Dosador", "Yes", "No");
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
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
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
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this.isNewOperacao = false;

                        int pulso_horario = _valores.PulsoHorario;
                        if (this._parametros.SomarPulsoReverso)
                        {
                            pulso_horario += _calibracao.UltimoPulsoReverso;
                        }
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.Dispensar(
                                            _motor,
                                            pulso_horario,
                                            _valores.Velocidade,
                                            _valores.Aceleracao,
                                            _valores.Delay,
                                            _valores.PulsoReverso, i_Step)
                                        );

                        this.IsDispensou = true;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (this.IsDispensou)
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            Util.ObjectCalibragem.UpdatePulsosRev(_motor + 1, _valores.PulsoReverso);
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 10;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
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
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;                                
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramentoP3();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramentoP3();

                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            //Dosar
                            this.passosActionP3 = 6;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
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
                                this.modBusDispenser_P3.ReadSensores_Mover();
                                if (this.modBusDispenser_P3.SensorCopo && this.modBusDispenser_P3.SensorEsponja)
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
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
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
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this.isNewOperacao = false;

                        int pulso_horario = _valores.PulsoHorario;
                        if (this._parametros.SomarPulsoReverso)
                        {
                            pulso_horario += _calibracao.UltimoPulsoReverso;
                        }
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.Dispensar(
                                            _motor,
                                            pulso_horario,
                                            _valores.Velocidade,
                                            _valores.Aceleracao,
                                            _valores.Delay,
                                            _valores.PulsoReverso, i_Step)
                                        );

                        this.IsDispensou = true;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (this.IsDispensou)
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            Util.ObjectCalibragem.UpdatePulsosRev(_motor + 1, _valores.PulsoReverso);
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 10;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
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
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramentoP3();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramentoP3();

                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            //Dosar
                            this.passosActionP3 = 6;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                                this.modBusDispenser_P3.ReadSensores_Mover();
                                if (this.modBusDispenser_P3.SensorCopo && this.modBusDispenser_P3.SensorEsponja)
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
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
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
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this.isNewOperacao = false;

                        int pulso_horario = _valores.PulsoHorario;
                        if (this._parametros.SomarPulsoReverso)
                        {
                            pulso_horario += _calibracao.UltimoPulsoReverso;
                        }
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.Dispensar(
                                            _motor,
                                            pulso_horario,
                                            _valores.Velocidade,
                                            _valores.Aceleracao,
                                            _valores.Delay,
                                            _valores.PulsoReverso, i_Step)
                                        );

                        this.IsDispensou = true;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (this.IsDispensou)
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            Util.ObjectCalibragem.UpdatePulsosRev(_motor + 1, _valores.PulsoReverso);
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 10;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
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
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramentoP3();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramentoP3();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            //Dosar
                            this.passosActionP3 = 6;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
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
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this.isNewOperacao = false;

                        int pulso_horario = _valores.PulsoHorario;
                        if (this._parametros.SomarPulsoReverso)
                        {
                            pulso_horario += _calibracao.UltimoPulsoReverso;
                        }
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.Dispensar(
                                            _motor,
                                            pulso_horario,
                                            _valores.Velocidade,
                                            _valores.Aceleracao,
                                            _valores.Delay,
                                            _valores.PulsoReverso, i_Step)
                                        );

                        this.IsDispensou = true;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (this.IsDispensou)
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            Util.ObjectCalibragem.UpdatePulsosRev(_motor + 1, _valores.PulsoReverso);
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 10;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
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
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramentoP3();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramentoP3();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        this.passosActionP3 = 1;
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
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
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this.isNewOperacao = false;

                        int pulso_horario = _valores.PulsoHorario;
                        if (this._parametros.SomarPulsoReverso)
                        {
                            pulso_horario += _calibracao.UltimoPulsoReverso;
                        }
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.Dispensar(
                                            _motor,
                                            pulso_horario,
                                            _valores.Velocidade,
                                            _valores.Aceleracao,
                                            _valores.Delay,
                                            _valores.PulsoReverso, i_Step)
                                        );

                        this.IsDispensou = true;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (this.IsDispensou)
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            Util.ObjectCalibragem.UpdatePulsosRev(_motor + 1, _valores.PulsoReverso);
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 10;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
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
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramentoP3();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramentoP3();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {

                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //this.modBusDispenser_P3.AbrirGaveta();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {

                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
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
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this.isNewOperacao = false;

                        int pulso_horario = _valores.PulsoHorario;
                        if (this._parametros.SomarPulsoReverso)
                        {
                            pulso_horario += _calibracao.UltimoPulsoReverso;
                        }
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.Dispensar(
                                            _motor,
                                            pulso_horario,
                                            _valores.Velocidade,
                                            _valores.Aceleracao,
                                            _valores.Delay,
                                            _valores.PulsoReverso, i_Step)
                                        );

                        this.IsDispensou = true;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (this.IsDispensou)
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            Util.ObjectCalibragem.UpdatePulsosRev(_motor + 1, _valores.PulsoReverso);
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 10;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
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
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramentoP3();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramentoP3();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {

                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada && this.modBusDispenser_P3.SensorBaixoBicos)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || !this.modBusDispenser_P3.SensorBaixoBicos))
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //this.modBusDispenser_P3.AbrirGaveta();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
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
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this.isNewOperacao = false;

                        int pulso_horario = _valores.PulsoHorario;
                        if (this._parametros.SomarPulsoReverso)
                        {
                            pulso_horario += _calibracao.UltimoPulsoReverso;
                        }
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.Dispensar(
                                            _motor,
                                            pulso_horario,
                                            _valores.Velocidade,
                                            _valores.Aceleracao,
                                            _valores.Delay,
                                            _valores.PulsoReverso, i_Step)
                                        );

                        this.IsDispensou = true;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (this.IsDispensou)
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            Util.ObjectCalibragem.UpdatePulsosRev(_motor + 1, _valores.PulsoReverso);
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 10;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
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
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramentoP3();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramentoP3();

                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
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
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this.isNewOperacao = false;

                        int pulso_horario = _valores.PulsoHorario;
                        if (this._parametros.SomarPulsoReverso)
                        {
                            pulso_horario += _calibracao.UltimoPulsoReverso;
                        }
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.Dispensar(
                                            _motor,
                                            pulso_horario,
                                            _valores.Velocidade,
                                            _valores.Aceleracao,
                                            _valores.Delay,
                                            _valores.PulsoReverso, i_Step)
                                        );

                        this.IsDispensou = true;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (this.IsDispensou)
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            Util.ObjectCalibragem.UpdatePulsosRev(_motor + 1, _valores.PulsoReverso);
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 10;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
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
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramentoP3();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramentoP3();

                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {

                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 3;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
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
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this.isNewOperacao = false;

                        int pulso_horario = _valores.PulsoHorario;
                        if (this._parametros.SomarPulsoReverso)
                        {
                            pulso_horario += _calibracao.UltimoPulsoReverso;
                        }
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.Dispensar(
                                            _motor,
                                            pulso_horario,
                                            _valores.Velocidade,
                                            _valores.Aceleracao,
                                            _valores.Delay,
                                            _valores.PulsoReverso, i_Step)
                                        );

                        this.IsDispensou = true;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (this.IsDispensou)
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            Util.ObjectCalibragem.UpdatePulsosRev(_motor + 1, _valores.PulsoReverso);
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //this.modBusDispenser_P3.AbrirGaveta();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 10;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
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
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramentoP3();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramentoP3();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));

                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                //m.ShowDialog("Trocar recipiente!");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                this.passosActionP3 = 3;
                            }
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                        {
                            this.passosActionP3 = 1;
                        }

                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
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
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this.isNewOperacao = false;

                        int pulso_horario = _valores.PulsoHorario;
                        if (this._parametros.SomarPulsoReverso)
                        {
                            pulso_horario += _calibracao.UltimoPulsoReverso;
                        }
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.Dispensar(
                                            _motor,
                                            pulso_horario,
                                            _valores.Velocidade,
                                            _valores.Aceleracao,
                                            _valores.Delay,
                                            _valores.PulsoReverso, i_Step)
                                        );

                        this.IsDispensou = true;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (this.IsDispensou)
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            Util.ObjectCalibragem.UpdatePulsosRev(_motor + 1, _valores.PulsoReverso);
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 10;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
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
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramentoP3();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramentoP3();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 2;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        this.counterFalha = 0;
                        this.passosActionP3 = 18;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
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
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this.isNewOperacao = false;

                        int pulso_horario = _valores.PulsoHorario;
                        if (this._parametros.SomarPulsoReverso)
                        {
                            pulso_horario += _calibracao.UltimoPulsoReverso;
                        }
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.Dispensar(
                                            _motor,
                                            pulso_horario,
                                            _valores.Velocidade,
                                            _valores.Aceleracao,
                                            _valores.Delay,
                                            _valores.PulsoReverso, i_Step)
                                        );

                        this.IsDispensou = true;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (this.IsDispensou)
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            Util.ObjectCalibragem.UpdatePulsosRev(_motor + 1, _valores.PulsoReverso);
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 10;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
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
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramentoP3();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramentoP3();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 3;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        this.counterFalha = 0;
                        this.passosActionP3 = 18;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
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
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this.isNewOperacao = false;

                        int pulso_horario = _valores.PulsoHorario;
                        if (this._parametros.SomarPulsoReverso)
                        {
                            pulso_horario += _calibracao.UltimoPulsoReverso;
                        }
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.Dispensar(
                                            _motor,
                                            pulso_horario,
                                            _valores.Velocidade,
                                            _valores.Aceleracao,
                                            _valores.Delay,
                                            _valores.PulsoReverso, i_Step)
                                        );

                        this.IsDispensou = true;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (this.IsDispensou)
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            Util.ObjectCalibragem.UpdatePulsosRev(_motor + 1, _valores.PulsoReverso);
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this.modBusDispenser_P3.AbrirGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 10;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
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
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramentoP3();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramentoP3();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 6;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this.modBusDispenser_P3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        //this.modBusDispenser_P3.AbrirGaveta(true);
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
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
                        //this.modBusDispenser_P3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this.isNewOperacao = false;

                        int pulso_horario = _valores.PulsoHorario;
                        if (this._parametros.SomarPulsoReverso)
                        {
                            pulso_horario += _calibracao.UltimoPulsoReverso;
                        }
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.Dispensar(
                                            _motor,
                                            pulso_horario,
                                            _valores.Velocidade,
                                            _valores.Aceleracao,
                                            _valores.Delay,
                                            _valores.PulsoReverso, i_Step)
                                        );

                        this.IsDispensou = true;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (this.IsDispensou)
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            Util.ObjectCalibragem.UpdatePulsosRev(_motor + 1, _valores.PulsoReverso);
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this.modBusDispenser_P3.AbrirGaveta(true);
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 10;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
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
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada && this.modBusDispenser_P3.SensorBaixoBicos)
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                               
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || !this.modBusDispenser_P3.SensorBaixoBicos))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramentoP3();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramentoP3();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this.modBusDispenser_P3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada))
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
                        //this.modBusDispenser_P3.AbrirGaveta(true);
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
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
                        //this.modBusDispenser_P3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this.isNewOperacao = false;
                        int pulso_horario = _valores.PulsoHorario;
                        if (this._parametros.SomarPulsoReverso)
                        {
                            pulso_horario += _calibracao.UltimoPulsoReverso;
                        }
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.Dispensar(
                                            _motor,
                                            pulso_horario,
                                            _valores.Velocidade,
                                            _valores.Aceleracao,
                                            _valores.Delay,
                                            _valores.PulsoReverso, i_Step)
                                        );

                        this.IsDispensou = true;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (this.IsDispensou)
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            Util.ObjectCalibragem.UpdatePulsosRev(_motor + 1, _valores.PulsoReverso);
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this.modBusDispenser_P3.AbrirGaveta(true);
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 10;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
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
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada && this.modBusDispenser_P3.SensorBaixoBicos)
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;                               
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || !this.modBusDispenser_P3.SensorBaixoBicos))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                         this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramentoP3();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramentoP3();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this.modBusDispenser_P3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada))
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
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
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
                        //this.modBusDispenser_P3.AbrirGaveta(true);
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
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
                        //this.modBusDispenser_P3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.counterFalha = 0;
                        this.isNewOperacao = false;
                        int pulso_horario = _valores.PulsoHorario;
                        if (this._parametros.SomarPulsoReverso)
                        {
                            pulso_horario += _calibracao.UltimoPulsoReverso;
                        }
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.Dispensar(
                                            _motor,
                                            pulso_horario,
                                            _valores.Velocidade,
                                            _valores.Aceleracao,
                                            _valores.Delay,
                                            _valores.PulsoReverso, i_Step)
                                        );

                        this.IsDispensou = true;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (this.IsDispensou)
                        {
                            this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            Util.ObjectCalibragem.UpdatePulsosRev(_motor + 1, _valores.PulsoReverso);
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this.modBusDispenser_P3.AbrirGaveta(true);
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 10;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
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
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada && this.modBusDispenser_P3.SensorBaixoBicos)
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                                if (!this.isNewOperacao)
                                {
                                    this.passosActionP3 = 16;
                                    this.counterFalha = 0;
                                }
                                else
                                {
                                    PausarMonitoramentoP3();
                                }

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || !this.modBusDispenser_P3.SensorBaixoBicos))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramentoP3();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramentoP3();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this.modBusDispenser_P3.FecharGaveta(false);
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada))
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
            if (this.modBusDispenser_P3.SensorEmergencia)
            {
                retorno = true;
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                {
                    //bool isRec = m.ShowDialog("Usuário Botão de Emergência Pressionado. Deseja finalizar este processo?", "Yes", "No", true, 60);
                    bool isRec = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Botao_Emergencia_Passos, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true, 60);
                    if (isRec)
                    {
                        PausarMonitoramentoP3();
                        Thread.Sleep(1000);

                    }
                    else
                    {
                        this.passosActionP3 = passosActionP3Emergencia;
                    }
                }

            }
            else if (this.modBusDispenser_P3.CodError > 0)
            {
                retorno = true;
                if (this.counterFalha >= 2)
                {
                    PausarMonitoramentoP3();
                    Thread.Sleep(1000);
                }

                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                {
                    //string msg = "Usuário ocorreu algum erro na movimentação da placa. Deseja continuar este processo?" + Environment.NewLine + "Descrição: " + this.modBusDispenser_P3.GetDescCodError();
                    string msg = Negocio.IdiomaResxExtensao.PlacaMov_Erro_Movimentacao_Passos + this.modBusDispenser_P3.GetDescCodError();
                    bool isRec = m.ShowDialog(msg, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true, 30);
                    if (isRec)
                    {
                        this.counterFalha++;
                        this.passosActionP3 = passosActionP3CodError;
                    }
                    else
                    {
                        PausarMonitoramentoP3();
                        Thread.Sleep(1000);
                    }
                }
            }
            return retorno;
        }
        #endregion

    }
}
