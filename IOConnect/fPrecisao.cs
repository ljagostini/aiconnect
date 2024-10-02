using Percolore.Core;
using Percolore.Core.Logging;
using Percolore.Core.Persistence.WindowsRegistry;
using Percolore.IOConnect.Util;
using System.ComponentModel;
using System.Data;

namespace Percolore.IOConnect
{
	public partial class fPrecisao : Form
    {
        private Util.ObjectColorante colorante = new Util.ObjectColorante();
        public Util.ObjectCalibragem calibracao = new ObjectCalibragem();
        public Negocio.Accurracy acurracy = new Negocio.Accurracy();

        private DataTable dtCalAuto = null;
        int posInicio = -1;

        System.ComponentModel.BackgroundWorker backgroundWorker1 = null;
        private bool isThread = false;
        private bool isRunning = false;
        bool execCal = false;

        private List<IDispenser> ldisp = new List<IDispenser>();
        //private IDispenser disp;
        private Negocio.Balanca_BEL balanca_bel = new Negocio.Balanca_BEL();

        private double MassaBalancaOnLine = 0;
        private double MassaBalancaOnLineInc = 0;

        private double MassaBalancaRecipiente = 0;

        private bool IsDispensou = false;

        System.ComponentModel.BackgroundWorker backgroundWorker2 = null;
        private bool isThread2 = false;
        private bool isRunning2 = false;
        ValoresVO _valores = null;
        private int DelayEntreBalanca = 20 * 1000;

        private double lastPulsoHorario = 0;
        private string mensagemStrip = "";

        private int i_Step = 0;

        private Util.ObjectParametros _parametros = null;

        public fPrecisao(Negocio.Accurracy _accurracy)
        {
            InitializeComponent();
            try
            {
                UpdateStructAccurracy(_accurracy);
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void fPrecisao_Load(object sender, EventArgs e)
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
            
                btnSair.Text = string.Empty;
                btnSair.Image = Imagem.GetSair_32x32();

                this.balanca_bel._str_Serial = this.acurracy.SerialPortBal;
                this.balanca_bel.Start_Serial();

                this.colorante = Util.ObjectColorante.Load(this.acurracy.Circuito);
                this.calibracao = Util.ObjectCalibragem.Load(this.colorante.Circuito);
                txtCapacidadeMaxBalanca.Text = balanca_bel.CargaMaximaBalanca_Gramas.ToString();
                txtVolumeMaxRecipiente.Text = this.acurracy.VolumeRecipiente.ToString();
                txt_delay_seg_bal.Text = this.acurracy.DelaySegBalanca.ToString();
                txtMinMassaAdmRecipiente.Text = this.acurracy.MinMassaAdmRecipiente.ToString();

                lblAccurracyNameColorante.Text = this.colorante.Nome;
                lblCalibracaoAutoMotor.Text = this.colorante.Circuito.ToString();
                lblCalibracaoAutoMassaEspecifica.Text = this.colorante.MassaEspecifica.ToString();

                atualizaDGView(this.acurracy);

                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    this.acurracy.NumeroSerie = percRegistry.GetSerialNumber();
                }
                
                Thread thrd = new Thread(new ThreadStart(btnStart_Click));
                thrd.Start();
                _parametros = Util.ObjectParametros.Load();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void fPrecisao_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                balanca_bel.CloseSerial();
                ClosedSerialDispensa();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void updateTeclado()
        {
            try
            {
                Util.ObjectParametros _parametros = Util.ObjectParametros.Load();
                bool chb_tec = _parametros.HabilitarTecladoVirtual;
                bool chb_touch = _parametros.HabilitarTouchScrenn;
                txt_delay_seg_bal.isTecladoShow = chb_tec;
                txt_delay_seg_bal.isTouchScrenn = chb_touch;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void UpdateStructAccurracy(Negocio.Accurracy _accurracy)
        {
            try
            {
                this.acurracy = new Negocio.Accurracy();
                this.acurracy.Circuito = _accurracy.Circuito;
                this.acurracy.VolumeRecipiente = _accurracy.VolumeRecipiente;
                this.acurracy.ModeloBalanca = _accurracy.ModeloBalanca;
                this.acurracy.SerialPortBal = _accurracy.SerialPortBal;
                this.acurracy.DelaySegBalanca = _accurracy.DelaySegBalanca;
                this.acurracy.MinMassaAdmRecipiente = _accurracy.MinMassaAdmRecipiente;
                this.acurracy.MessageRetorno = "";
                if (this.acurracy.listPrecisao == null)
                {
                    this.acurracy.listPrecisao = new List<Negocio.Precisao>();
                }
                this.acurracy.listPrecisao.Clear();
                foreach(Negocio.Precisao _p in _accurracy.listPrecisao)
                {
                    for(int i = 1; i <= _p.tentativas; i++)
                    {
                        Negocio.Precisao precL = new Negocio.Precisao();
                        precL.executado = _p.executado;
                        precL.volume = _p.volume;
                        precL.tentativas = i;
                        precL.volumeDos = _p.volumeDos;
                        precL.volumeDos_str = "0";

                        this.acurracy.listPrecisao.Add(precL);

                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnSair_Click(object sender, EventArgs e)
        {
            try
            {
                this.isThread = false;
                this.isThread2 = false;
                this.Close();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void atualizaDGView(Negocio.Accurracy _accurracy)
        {
            try
            {
             
                if (dgvCalibracaoAuto.Rows != null)
                {
                    dgvCalibracaoAuto.Rows.Clear();
                }

                this.dtCalAuto = new DataTable();
               
                this.dtCalAuto.Columns.Add("Volume", typeof(string));
                this.dtCalAuto.Columns.Add("Tentativas", typeof(string));
                this.dtCalAuto.Columns.Add("VolumeDos", typeof(string));
                this.dtCalAuto.Columns.Add("Executado", typeof(string));

                if (_accurracy != null)
                {
                    foreach (Negocio.Precisao _op in _accurracy.listPrecisao)
                    {
                        DataRow dr = this.dtCalAuto.NewRow();
                        dr["Volume"] = _op.volume.ToString();
                        dr["Tentativas"] = _op.tentativas.ToString();
                        dr["VolumeDos"] = _op.volumeDos_str;
                       
                        if (_op.executado == 0)
                        {
                            dr["Executado"] = "Não";
                        }
                        else
                        {                            
                            dr["Executado"] = "Sim";
                        }
                        this.dtCalAuto.Rows.Add(dr);
                    }
                }
                if (dgvCalibracaoAuto.Rows != null)
                {
                    dgvCalibracaoAuto.Rows.Clear();
                }
                for (int i = 0; i < this.dtCalAuto.Rows.Count; i++)
                {
                    dgvCalibracaoAuto.Rows.Add(this.dtCalAuto.Rows[i][0], this.dtCalAuto.Rows[i][1], this.dtCalAuto.Rows[i][2], this.dtCalAuto.Rows[i][3]);
                }

                this.Refresh();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void dgvCalibracaoAuto_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                DataGridViewRow row = dgvCalibracaoAuto.Rows[e.RowIndex];
                if (row.Cells["Executado"].Value.ToString() == "Não")
                {
                    //228; 81; 76  E4514C
                    row.Cells["Executado"].Style.BackColor = Color.FromArgb(255, 228, 81, 76);
                }
                else
                {
                    row.Cells["Executado"].Style.BackColor = Color.FromArgb(255, 0, 128, 0);
                   
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnStart_Click()
        {
            try
            {
                Thread.Sleep(2000);
                this.Invoke(new MethodInvoker(startF));
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void startF()
        {
            try
            {
                statusStrip1.Visible = true;
                this.mensagemStrip = Negocio.IdiomaResxExtensao.CalibracaoAuto_Pos_Recipiente;
                AttStripMsg();

                this.DelayEntreBalanca = txt_delay_seg_bal.ToInt() * 1000;
                
                if (this.DelayEntreBalanca < 5000)
                {
                    this.DelayEntreBalanca = 5000;
                }

                if (this.acurracy.listPrecisao.Count > 0)
                {
                    this.posInicio = 0;

                    this.Invoke(new MethodInvoker(AttDGVBack));
                    this.Invoke(new MethodInvoker(iniciarCalibracaoAutomatica));
                    execCal = true;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void iniciarCalibracaoAutomatica()
        {
            try
            {               
                Util.ObjectParametros parametros = Util.ObjectParametros.Load();
               
                foreach (IDispenser _disp in this.ldisp)
                {
                    _disp.Disconnect();
                }

                this.ldisp.Clear();
                IDispenser dispenser = null;
                IDispenser dispenser2 = null;

                switch ((Dispositivo)parametros.IdDispositivo)
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
                            dispenser = new ModBusDispenser_P2(parametros.NomeDispositivo);
                            break;
                        }
                    //case Dispositivo.Placa_4:
                    //    {
                    //        dispenser = new ModBusDispenser_P4(parametros.NomeDispositivo);
                    //        break;
                    //    }
                }
                this.ldisp.Add(dispenser);
                switch ((Dispositivo2)parametros.IdDispositivo2)
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
                            dispenser2 = new ModBusDispenser_P2(parametros.NomeDispositivo2);
                            break;
                        }
                    //case Dispositivo2.Placa_4:
                    //    {
                    //        dispenser2 = new ModBusDispenser_P4(parametros.NomeDispositivo2);
                    //        break;
                    //    }
                    default:
                        {
                            dispenser2 = null;
                            break;
                        }
                }


                if (parametros.IdDispositivo2 != 0)
                {
                    this.ldisp.Add(dispenser2);
                }
            
                this.MassaBalancaOnLine = 0;
                this.MassaBalancaOnLineInc = 0;

                bool posRec = PosicionarRecipiente();
                if (posRec)
                {
                    //AttDGVBack();
                    if (backgroundWorker1 == null)
                    {
                        this.backgroundWorker1 = new BackgroundWorker();
                        this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker1_DoWork);
                        this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
                        this.backgroundWorker1.WorkerSupportsCancellation = true;
                        this.isThread = true;
                        this.isRunning = false;
                        this.execCal = true;
                        this.backgroundWorker1.RunWorkerAsync();
                    }
                    else
                    {

                        this.backgroundWorker1 = new BackgroundWorker();
                        this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker1_DoWork);
                        this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
                        this.backgroundWorker1.WorkerSupportsCancellation = true;
                        this.isThread = true;
                        this.isRunning = false;
                        this.execCal = true;
                        backgroundWorker1.RunWorkerAsync();

                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        #region Processo

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

                worker = null;
            
                if (!this.execCal)
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                    {
                        m.ShowDialog("O Teste de Precisão não foi realizado com sucesso! Por favor, contate o suporte técnico pelos canais disponiveis.");
                       
                    }
                    this.Invoke(new MethodInvoker(SairBGW));
                }

                else if (this.execCal && this.posInicio >= this.acurracy.listPrecisao.Count)
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                    {
                        m.ShowDialog("Usuário Teste de Precisão realizado com sucesso!", Negocio.IdiomaResxExtensao.Global_Sim);
                    }
                    this.acurracy.MessageRetorno = "OK";
                    this.Invoke(new MethodInvoker(SairBGW));
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void SairBGW()
        {
            btnSair_Click(null, null);
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

        void PausarMonitoramento()
        {
            try
            {
                this.isThread = false;
                this.isRunning = true;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void AttDGVBack()
        {
            try
            {
                atualizaDGView(this.acurracy);
                if (this.posInicio < this.acurracy.listPrecisao.Count)
                {
                    dgvCalibracaoAuto.Rows[this.posInicio].Selected = true;
                    int posScroll = (this.posInicio - 3 > 0) ? this.posInicio - 3 : 0;

                    dgvCalibracaoAuto.FirstDisplayedScrollingRowIndex = posScroll;
                    dgvCalibracaoAuto.Update();
                }

                AttMassaRecipiente();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void AttMassaRecipiente()
        {
            try
            {
                lblMassaRecipiente.Text = this.MassaBalancaOnLine.ToString();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void AttStripMsg()
        {
            try
            {
                int valor_toolP = toolProgress.Value + 5;
                if (valor_toolP > 100)
                {
                    valor_toolP = 0;
                }
                toolProgress.Value = valor_toolP;
                toolMessage.Text = this.mensagemStrip;
                statusStrip1.Refresh();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void MonitoramentoEvent()
        {
            try
            {
                if (this.posInicio < this.acurracy.listPrecisao.Count)
                {
                    if (!this.execCal)
                    {
                        PausarMonitoramento();                        
                        return;
                    }
                    
                    //Faixa Precisao
                    if (!CapacidadeMaxRecipiente())
                    {
                        FaixaPrecisao();
                    }
                    else
                    {
                        this.mensagemStrip = Negocio.IdiomaResxExtensao.CalibracaoAuto_Trocar_Recipiente;
                        this.Invoke(new MethodInvoker(AttStripMsg));
                        TrocarRecipiente();
                    }
                    return;
                }
                else
                { 
                    PausarMonitoramento();
                    return;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			this.isRunning = false;
        }

        private bool CapacidadeMaxRecipiente()
        {
            bool retorno = false;
            try
            {
                double MassaIdeal = this.acurracy.listPrecisao[this.posInicio].volume * this.colorante.MassaEspecifica;
                double massaB = this.MassaBalancaOnLine + MassaIdeal;
                double volumeB = (massaB - this.MassaBalancaRecipiente) / this.colorante.MassaEspecifica;
                if ((massaB > balanca_bel.CargaMaximaBalanca_Gramas) || (volumeB > this.acurracy.VolumeRecipiente))
                {
                    retorno = true;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			return retorno;
        }

        private void TrocarRecipiente()
        {
            try
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog("Capacidade Máxima da Balança ou Volume Máximo do Recipiente Excedido, esvaziar o Recipiente no respectivo canister.");
                }

                ClosedSerialDispensa();
                TrocarRecipiente1();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void TrocarRecipiente1()
        {
            try
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(string.Format(Negocio.IdiomaResxExtensao.CalibracaoAuto_PosicaoRecipiente, this.acurracy.listPrecisao[this.posInicio].volume.ToString()));
                }
                IDispenser _disp = this.ldisp[0];
                bool conectar = Operar.Conectar(ref _disp, false);
                if (conectar && Operar.TemRecipiente(_disp, false))
                {
                    this.mensagemStrip = Negocio.IdiomaResxExtensao.CalibracaoAuto_Lendo_Balanca;
                    AttStripMsg();

                    //Thread.Sleep(this.DelayEntreBalanca);
                    int count_Delay = this.DelayEntreBalanca / 1000;
                    for (int i = 0; i < count_Delay; i++)
                    {
                        Thread.Sleep(1000);
                       
                    }
                    
                    byte[] b_tara = new byte[1];
                    byte[] b_w = new byte[1];
                    byte[] response = new byte[15];
                    b_w[0] = (byte)this.balanca_bel.b_peso_balanca;
                    this.balanca_bel.WriteSerialPort(b_w);
                    this.balanca_bel.GetResponse(ref response, false);
                    if (this.balanca_bel.isTerminouRead)
                    {
                        this.balanca_bel.SetValues(response, false, false);
                    }
                    else
                    {
                        this.isRunning = false;
                        this.execCal = false;

                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                        {
                            m.ShowDialog("Erro na Leitura da Balança!");
                        }
                        return;
                    }

                    if (this.balanca_bel.valorPeso < this.acurracy.MinMassaAdmRecipiente)
                    {
                        this.MassaBalancaOnLine = this.balanca_bel.valorPeso;
                        this.MassaBalancaRecipiente = this.balanca_bel.valorPeso;

                        if (this.balanca_bel.isTerminouRead)
                        {
                            this.balanca_bel.SetValues(response, false, false);
                            this.MassaBalancaOnLine = this.balanca_bel.valorPeso;
                            this.MassaBalancaOnLineInc = this.MassaBalancaOnLine;
                            ClosedSerialDispensa();
                            this.isRunning = false;
                            this.Invoke(new MethodInvoker(AttMassaRecipiente));
                        }
                        else
                        {
                            this.execCal = false;
                            this.isRunning = false;
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                            {
                                m.ShowDialog("Erro na Leitura da Balança!");
                            }

                            return;
                        }
                    }
                    else
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                        {
                            m.ShowDialog("Peso do Recipiente Excedida!");
                        }
                        TrocarRecipiente1();
                    }
                }
                else
                {
                    ClosedSerialDispensa();
                    TrocarRecipiente1();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    this.isRunning = false;
            }
        }
       
        private void FaixaPrecisao()
        {
            try
            {
                if (this.IsDispensou)
                {
                    this.mensagemStrip = Negocio.IdiomaResxExtensao.CalibracaoAuto_Lendo_Balanca;
                    AttStripMsg();
                    //Thread.Sleep(this.DelayEntreBalanca);
                    int count_Delay = this.DelayEntreBalanca / 1000;
                    for (int i = 0; i < count_Delay; i++)
                    {
                        Thread.Sleep(1000);                        
                    }
                    
                    byte[] b_w = new byte[1];
                    byte[] response = new byte[15];
                    b_w[0] = (byte)this.balanca_bel.b_peso_balanca;
                    this.balanca_bel.WriteSerialPort(b_w);
                    this.balanca_bel.GetResponse(ref response, false);
                    double massaAterior = this.MassaBalancaOnLine;
                    double massaDosada = 0;
                    
                    if (this.balanca_bel.isTerminouRead)
                    {
                        this.balanca_bel.SetValues(response, false, false);

                        this.MassaBalancaOnLine = this.balanca_bel.valorPeso;
                        massaDosada = this.MassaBalancaOnLine - massaAterior;

                        this.acurracy.listPrecisao[this.posInicio].volumeDos = massaDosada / this.colorante.MassaEspecifica;
                        this.acurracy.listPrecisao[this.posInicio].volumeDos_str = string.Format("{0:N5}", this.acurracy.listPrecisao[this.posInicio].volumeDos);
                       
                    }
                    else
                    {
                        this.execCal = false;
                        this.acurracy.MessageRetorno = "Erro na Leitura da Balança!";
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                        {
                            m.ShowDialog("Erro na Leitura da Balança!");
                        }
                        this.isRunning = false;
                       
                        return;
                    }

                    this.posInicio++;
                    this.IsDispensou = false;
                    this.isRunning = false;
                    this.MassaBalancaOnLineInc += massaDosada;
                    this.Invoke(new MethodInvoker(AttDGVBack));
                    ClosedSerialDispensa();
                }
                else
                {
                    this.acurracy.listPrecisao[this.posInicio].executado = 1;

                    this.mensagemStrip = string.Format(Negocio.IdiomaResxExtensao.CalibracaoAuto_DispensandoVolume, this.acurracy.listPrecisao[this.posInicio].volume.ToString()); 
                    this.Invoke(new MethodInvoker(AttStripMsg));
                    ExecutarMonitoramento2();
                   
                }
                
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    this.isRunning = false;
                this.execCal = false;
            }
        }

        private bool PosicionarRecipiente()
        {
            bool retorno = false;
            try
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog("Posicionar Recipiente");
                }
                IDispenser _disp = this.ldisp[0];
                bool conectar = Operar.Conectar(ref _disp, false);
                if (conectar && Operar.TemRecipiente(_disp, false))
                {
                    this.mensagemStrip = Negocio.IdiomaResxExtensao.CalibracaoAuto_Lendo_Balanca;
                    AttStripMsg();
                    //Thread.Sleep(this.DelayEntreBalanca);
                    int count_Delay = this.DelayEntreBalanca / 1000;
                    for (int i = 0; i < count_Delay; i++)
                    {
                        Thread.Sleep(1000);
                        
                    }
                    
                    byte[] b_tara = new byte[1];
                    byte[] b_w = new byte[1];
                    byte[] response = new byte[15];

                    ClosedSerialDispensa();
                    b_w[0] = (byte)this.balanca_bel.b_peso_balanca;
                    this.balanca_bel.WriteSerialPort(b_w);
                    this.balanca_bel.GetResponse(ref response, false);
                    if (this.balanca_bel.isTerminouRead)
                    {
                        this.balanca_bel.SetValues(response, false, false);
                        this.MassaBalancaRecipiente = this.balanca_bel.valorPeso; 
                        this.MassaBalancaOnLine = this.balanca_bel.valorPeso;
                        this.MassaBalancaOnLineInc = this.MassaBalancaOnLine;
                        if (this.MassaBalancaOnLine < this.acurracy.MinMassaAdmRecipiente)
                        {
                            retorno = true;
                            
                        }
                        else
                        {
                            this.execCal = false;
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                            {
                                m.ShowDialog("Peso do Recipiente Excedida!");
                            }
                            this.acurracy.MessageRetorno = "Peso do Recipiente Excedida!";
                            retorno = false;
                        }

                    }
                    else
                    {
                        this.execCal = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                        {
                            m.ShowDialog("Erro na Leitura da Balança!");
                            this.acurracy.MessageRetorno = "Erro na Leitura da Balança!";
                        }
                        return false;
                    }
                    
                }
                else
                {
                    ClosedSerialDispensa();
                    PosicionarRecipiente();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			return retorno;
        }
        
        private void ClosedSerialDispensa()
        {
            try
            {                
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
        #endregion

        #region Dosagem Placa

        void Monitoramento_Event2()
        {
            try
            {
                /* Verifica se dispenser está liberado.
                 * isso representa que o processo foi concluído. */

                int motor = this.calibracao.Motor - 1;

                if (this.colorante.Dispositivo == 1)
                {
                    if (!this.ldisp[0].IsReady)
                    {
                        this.isRunning2 = false;
                        return;
                    }
                    if (this.IsDispensou)
                    {
                        this.isRunning2 = false;
                        this.isRunning = false;

                        PausarMonitoramento2();
                        if (_valores != null)
                        {
                            Util.ObjectCalibragem.UpdatePulsosRev(this.calibracao.Motor, _valores.PulsoReverso);
                            _valores.PulsoHorario = _valores.PulsoHorario - _valores.PulsoReverso;
                            lastPulsoHorario = lastPulsoHorario - _valores.PulsoReverso;
                        }
                        //Util.ObjectCalibragem.UpdatePulsosRev(_motor + 1, _valores.PulsoReverso);
                        return;
                    }
                    _valores = Operar.Parser(this.acurracy.listPrecisao[this.posInicio].volume, this.calibracao.Valores, this.calibracao.UltimoPulsoReverso);
                    lastPulsoHorario = _valores.PulsoHorario;

                    Task task = Task.Factory.StartNew(() => this.ldisp[0].Dispensar(
                                         motor,
                                        _valores.PulsoHorario,
                                        _valores.Velocidade,
                                        _valores.Aceleracao,
                                        _valores.Delay,
                                        _valores.PulsoReverso, i_Step)
                                    );
                }
                else if (this.colorante.Dispositivo == 2)
                {
                    if ((Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador)
                    {
                        motor = motor - 16;
                    }
                    else
                    {
                        motor = motor - 16;
                    }
                    
                    if (!this.ldisp[1].IsReady)
                    {
                        this.isRunning2 = false;
                        return;
                    }
                    if (this.IsDispensou)
                    {
                        this.isRunning2 = false;

                        PausarMonitoramento2();
                        if (_valores != null)
                        {
                            Util.ObjectCalibragem.UpdatePulsosRev(this.calibracao.Motor, _valores.PulsoReverso);
                            lastPulsoHorario = lastPulsoHorario - _valores.PulsoReverso;
                        }
                        
                        return;
                    }

                    _valores = Operar.Parser(this.acurracy.listPrecisao[this.posInicio].volume, this.calibracao.Valores, this.calibracao.UltimoPulsoReverso);
                    lastPulsoHorario = _valores.PulsoHorario;
                    Task task = Task.Factory.StartNew(() => this.ldisp[1].Dispensar(
                                         motor,
                                        _valores.PulsoHorario,
                                        _valores.Velocidade,
                                        _valores.Aceleracao,
                                        _valores.Delay,
                                        _valores.PulsoReverso, i_Step)
                                    );
                }

                this.IsDispensou = true;
                this.isRunning2 = false;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                if (!this.IsDispensou)
                {
                    Falha2(ex);
                }
                this.isRunning2 = false;
            }
        }

        void Falha2(Exception ex)
        {
            PausarMonitoramento2();

            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
            {
                m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + ex.Message);
            }
        }

        void PausarMonitoramento2()
        {
            try
            {
                if (backgroundWorker2.IsBusy && backgroundWorker2.CancellationPending == false)
                {
                    backgroundWorker2.CancelAsync();
                }
            
                this.isThread2 = false;
            
                foreach (IDispenser _disp in this.ldisp)
                {
                    _disp.Disconnect();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			if (!this.IsDispensou)
            {
                this.execCal = false;
                this.acurracy.MessageRetorno = "Erro ao Dispensar Produto!";
            }

            this.isRunning = false;
        }

        void ExecutarMonitoramento2()
        {
            try
            {
                IDispenser _disp = this.ldisp[0];
                bool conectar = Operar.Conectar(ref _disp, false);
                if (conectar)
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
                        this.IsDispensou = false;
                        while (backgroundWorker2.IsBusy)
                        {
                            this.isThread2 = false;
                            this.isRunning2 = true;
                        }
                        backgroundWorker2.RunWorkerAsync();
                    }
                }
                else
                {
                    PausarMonitoramento2();
                }
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
                                Thread.Sleep(2500);
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
    }
}