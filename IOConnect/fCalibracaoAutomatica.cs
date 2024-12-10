using Percolore.Core;
using Percolore.Core.Logging;
using Percolore.IOConnect.Util;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO.Ports;

namespace Percolore.IOConnect
{
	public partial class fCalibracaoAutomatica : Form
    {
        public event CloseWindows OnClosedEvent = null;
        private List<Util.ObjectCalibracaoHistorico> lCalibracaoAutoHistorico = new List<Util.ObjectCalibracaoHistorico>();
        private List<Util.ObjectColorante> colorantes = new List<Util.ObjectColorante>();

        private Util.ObjectParametros _parametro;

        private DataTable dtCalAuto = null;
        int posInicio = -1;
        int posInicioHistorico = 0;

        System.ComponentModel.BackgroundWorker backgroundWorker1 = null;
        private bool isThread = false;
        private bool isRunning = false;
        bool execCal = false;
        bool stopExecution = false;

        private List<IDispenser> ldisp = new List<IDispenser>();
        private ModBusDispenserMover_P3 modBusDispenserMover_P3 = null;
        
        private Negocio.InterfaceBalanca interfaceBal = null;
        private int modeloBalanca = 1;

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
        private int numeroMedias = 0;
        private string mensagemStrip = "";

        private int stepPosicaoRec = 0;
        private DateTime dtInioLeituraBalanca = DateTime.Now;      

        private int stepFaixaCalibracao = 0;
        private int stepPrimeiraCalibracao = 0;
        private int stepTrocarRec = 0;
        private bool isTrocarRec = false;
        private int _tipoCalibacao = 1;
        private int _processoCalibracao = 1;

        private bool ModoDebug = false;

        List<Util.ObjectColorante> lColSeguidor = null;
        private int i_Step = 0;

        public fCalibracaoAutomatica(List<Util.ObjectCalibracaoHistorico> lcal)
        {
            InitializeComponent();
            int X = (Screen.PrimaryScreen.Bounds.Width) - (this.Width);
            int Y = 30;
            this.Location = new Point(X, Y);
            lCalibracaoAutoHistorico = lcal;
            statusStrip1.Visible = false;
            try
            {
                foreach (Util.ObjectCalibracaoHistorico hist in lCalibracaoAutoHistorico)
                {
                    colorantes.Add(hist._calibracaoAuto._colorante);
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void fCalibracaoAutomatica_Load(object sender, EventArgs e)
        {
            try
            {
                updateTeclado();
                
                DataTable dtBal = new DataTable();
                dtBal.Columns.Add("valor", typeof(int));
                dtBal.Columns.Add("descricao", typeof(string));
                DataRow dr = dtBal.NewRow();
                dr["valor"] = 1;
                dr["descricao"] = "Bell";
                dtBal.Rows.Add(dr);

                DataRow dr1 = dtBal.NewRow();
                dr1["valor"] = 2;
                dr1["descricao"] = "Toledo MS403-S";
                dtBal.Rows.Add(dr1);

                DataRow dr2 = dtBal.NewRow();
                dr2["valor"] = 3;
                dr2["descricao"] = "MT PG503-S";
                dtBal.Rows.Add(dr2);

                cmbTipoBalanca.DataSource = dtBal.DefaultView;
                cmbTipoBalanca.DisplayMember = "descricao";
                cmbTipoBalanca.ValueMember = "valor";
                cmbTipoBalanca.SelectedIndex = 0;
                this.interfaceBal = new Negocio.Balanca_BEL();
                
                DataTable dtTipoCal = new DataTable();
                dtTipoCal.Columns.Add("valor", typeof(int));
                dtTipoCal.Columns.Add("descricao", typeof(string));
                DataRow dr3 = dtTipoCal.NewRow();
                dr3["valor"] = 1;
                dr3["descricao"] = Negocio.IdiomaResxExtensao.CalibracaoAuto_TipoAll;
                dtTipoCal.Rows.Add(dr3);

                DataRow dr4 = dtTipoCal.NewRow();
                dr4["valor"] = 2;
                dr4["descricao"] = Negocio.IdiomaResxExtensao.CalibracaoAuto_TipoIndi;
                dtTipoCal.Rows.Add(dr4);

                cmb_TipoCalibracao.DataSource = dtTipoCal.DefaultView;
                cmb_TipoCalibracao.DisplayMember = "descricao";
                cmb_TipoCalibracao.ValueMember = "valor";
                cmb_TipoCalibracao.SelectedIndex = 0;
                this._tipoCalibacao = 1;
                
                DataTable dtProcCal = new DataTable();
                dtProcCal.Columns.Add("valor", typeof(int));
                dtProcCal.Columns.Add("descricao", typeof(string));

                Util.ObjectParametros parametros = Util.ObjectParametros.Load();
                bool forceProcAuto = true;
                switch ((Dispositivo)parametros.IdDispositivo)
                {
                    case Dispositivo.Placa_3:
                        {
                            forceProcAuto = false;
                            break;
                        }
                }

                if (forceProcAuto)
                {
                    DataRow dr5 = dtProcCal.NewRow();
                    dr5["valor"] = 1;
                    dr5["descricao"] = Negocio.IdiomaResxExtensao.CalibracaoAuto_ProcAut;
                    dtProcCal.Rows.Add(dr5);
                }
                DataRow dr6 = dtProcCal.NewRow();
                dr6["valor"] = 2;
                dr6["descricao"] = Negocio.IdiomaResxExtensao.CalibracaoAuto_ProcSemiAut;
                dtProcCal.Rows.Add(dr6);

                cmb_ProcessoCalibracao.DataSource = dtProcCal.DefaultView;
                cmb_ProcessoCalibracao.DisplayMember = "descricao";
                cmb_ProcessoCalibracao.ValueMember = "valor";
                cmb_ProcessoCalibracao.SelectedIndex = 1;
                this._processoCalibracao = 2;
                
                for (int t = 0; t < lCalibracaoAutoHistorico.Count; t++)
                {
                    lCalibracaoAutoHistorico[t]._calibracaoAuto.CapacideMaxBalanca = this.interfaceBal.CargaMaximaBalanca_Gramas;
                }
                txtCapacidadeMaxBalanca.Text = lCalibracaoAutoHistorico[0]._calibracaoAuto.CapacideMaxBalanca.ToString();
                txtMassaAdmRecipiente.Text = lCalibracaoAutoHistorico[0]._calibracaoAuto.MaxMassaAdmRecipiente.ToString();
                txtMinMassaAdmRecipiente.Text = lCalibracaoAutoHistorico[0]._calibracaoAuto.MinMassaAdmRecipiente.ToString();
                txtVolumeMaxRecipiente.Text = lCalibracaoAutoHistorico[0]._calibracaoAuto.VolumeMaxRecipiente.ToString();
                txtTentativasRecipiente.Text = lCalibracaoAutoHistorico[0]._calibracaoAuto.NumeroMaxTentativaRec.ToString();
                txtCapacidadeMaxBalanca.Text = "0";
                txtMassaAdmRecipiente.Text = "0";
                txtMinMassaAdmRecipiente.Text = "0";
                txtVolumeMaxRecipiente.Text = "0";
                txtTentativasRecipiente.Text = "0";

                this.btnSair.Text = string.Empty;
                this.btnSair.Image = Imagem.GetSair_32x32();
                this.btnStart.Text = Negocio.IdiomaResxExtensao.Global_Start;
                this.btnParar.Text = Negocio.IdiomaResxExtensao.Global_Stop;

                this.lblTitulo.Text = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Titulo;
                this.lblModelScale.Text = Negocio.IdiomaResxExtensao.CalibracaoAutomate_lblModelScale;
                this.lblVolumeMaxRecipiente.Text = Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoVolumeMaxRecipiente;
                this.lblTentativasRecipiente.Text = Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoTentativaPosicionamento;
                this.lblMinMassaAdmRecipiente.Text = Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoPesoRecipiente;

                this.lblCalibracaoAutoColorante.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemColorante;
                this.lblCalibracaoAutoLegendaMotor.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemLegendaMotor;
                this.lblCalibracaoAutoLegendaMassaEspec.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemLegendaMassaEspec;
                this.lblCapacidadeMaxBalanca.Text = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_lblCapacidadeMaxBalanca;
                this.lblTotalizador.Text = Negocio.IdiomaResxExtensao.CalibracaoAuto_lblTotalizador;
                this.lblTipo.Text = Negocio.IdiomaResxExtensao.CalibracaoAuto_Tipo;
                this.lblProcesso.Text = Negocio.IdiomaResxExtensao.CalibracaoAuto_Processo;

                this.gbComunicacaoBalanca.Text = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_ComBalanca;
                this.lblDelayBalanca.Text = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_lblDelayBalanca;
                this.lblTituloMassaBal.Text = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_lblTituloMassaBal;

                dgvCalibracaoAuto.Columns[0].HeaderText = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Etapa;
                dgvCalibracaoAuto.Columns[1].HeaderText = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Motor;
                dgvCalibracaoAuto.Columns[2].HeaderText = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Tentativa;
                dgvCalibracaoAuto.Columns[3].HeaderText = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Volume;
                dgvCalibracaoAuto.Columns[4].HeaderText = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_VolumeDos;
                dgvCalibracaoAuto.Columns[5].HeaderText = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_MassaIdeal;
                dgvCalibracaoAuto.Columns[6].HeaderText = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_MassaMedBalanca;
                dgvCalibracaoAuto.Columns[7].HeaderText = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Desvio;
                dgvCalibracaoAuto.Columns[8].HeaderText = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_DesvioAdmissel;
                dgvCalibracaoAuto.Columns[9].HeaderText = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Aprovado;
                dgvCalibracaoAuto.Columns[10].HeaderText = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Executado;             

                cb_CalibracaoAuto.DisplayMember = "Nome";
                cb_CalibracaoAuto.ValueMember = "Motor";
                cb_CalibracaoAuto.DataSource = colorantes.ToList();
                cb_CalibracaoAuto.SelectedIndex = -1;
                atualizaPortasComunicacaoBalanca();

                this._parametro = Util.ObjectParametros.Load();
                switch ((Dispositivo)this._parametro.IdDispositivo)
                {
                    case Dispositivo.Simulador:
                        {
                            this.ModoDebug = true;
                            break;
                        }
                    default:
                        {
                            this.ModoDebug = false;
                            break;
                        }
                }
            
                List<Util.ObjectColorante> lcol = Util.ObjectColorante.List();
                
                foreach (Util.ObjectColorante  _col in lcol)
                {
                    if(_col.Step > 0)
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

        private void fCalibracaoAutomatica_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (this.interfaceBal != null)
                {
                    this.interfaceBal.CloseSerial();
                }
            
                ClosedSerialDispensa();
            
                if (OnClosedEvent != null)
                {
                    OnClosedEvent();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnSair_Click(object sender, EventArgs e)
        {
            isThread = false;
            this.Close();
        }

        private void atualizaPortasComunicacaoBalanca()
        {
            try
            {
                string[] portas = SerialPort.GetPortNames();
                List<string> lPortas = new List<string>();
                foreach (string _str in portas)
                {
                    string np = ValidadeNameSerialPorta(_str);
                    if (np != null && np.Length > 1)
                    {
                        lPortas.Add(np);
                    }
                }
                cmbPortaSerial.Items.Clear();
                foreach (string _str in lPortas)
                {
                    cmbPortaSerial.Items.Add(_str);
                }
                if (cmbPortaSerial.Items.Count > 0)
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
                if ((_ic >= 48 && _ic <= 57) || (_ic >= 65 && _ic <= 90) || (_ic >= 97 && _ic <= 122))
                {
                    retorno += c.ToString();
                }
            }
            return retorno;
        }

        private void cmbTipoBalanca_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.modeloBalanca = Convert.ToInt32(cmbTipoBalanca.SelectedValue.ToString());
                if (this.interfaceBal != null)
                {
                    this.interfaceBal.CloseSerial();
                }
                if (this.modeloBalanca == 1)
                {
                    this.interfaceBal = new Negocio.Balanca_BEL();
                }
                else if (this.modeloBalanca == 2)
                {
                    this.interfaceBal = new Negocio.Balanca_ToledoMS403S();
                }
                else /*if (this.modeloBalanca == 3)*/
                {
                    this.interfaceBal = new Negocio.Balanca_ToledoPG503S();
                }
                txtCapacidadeMaxBalanca.Text = this.interfaceBal.CargaMaximaBalanca_Gramas.ToString();
                for (int t = 0; t < lCalibracaoAutoHistorico.Count; t++)
                {
                    lCalibracaoAutoHistorico[t]._calibracaoAuto.CapacideMaxBalanca = this.interfaceBal.CargaMaximaBalanca_Gramas;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void cmb_TipoCalibracao_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this._tipoCalibacao = Convert.ToInt32(cmb_TipoCalibracao.SelectedValue.ToString());
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void cmb_ProcessoCalibracao_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this._processoCalibracao = Convert.ToInt32(cmb_ProcessoCalibracao.SelectedValue.ToString());
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void cmbPortaSerial_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbPortaSerial.SelectedIndex >= 0)
                {
                    this.interfaceBal._str_Serial = cmbPortaSerial.Text;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void cb_CalibracaoAuto_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                Util.ObjectColorante colorante = (Util.ObjectColorante)cb_CalibracaoAuto.SelectedItem;

                lblCalibracaoAutoMotor.Text = colorante.Circuito.ToString();
                lblCalibracaoAutoMassaEspecifica.Text = colorante.MassaEspecifica.ToString();

                int index_posCal = 0;
                for(int i=0; i < this.lCalibracaoAutoHistorico.Count;i++)
                {
                    if(colorante.Circuito == this.lCalibracaoAutoHistorico[i]._calibracaoAuto._colorante.Circuito)
                    {
                        index_posCal = i;
                        break;
                    }
                }
               
                atualizaDGView(this.lCalibracaoAutoHistorico[index_posCal]);
                gbComunicacaoBalanca.Visible = true;
                gbComunicacaoBalanca.Enabled = true;
                
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void atualizaDGView(Util.ObjectCalibracaoHistorico calHist)
        {
            try
            {
                double _totalMassaMedBal = 0;
                double _totalVolumeDos = 0;
                if (dgvCalibracaoAuto.Rows != null)
                {
                    dgvCalibracaoAuto.Rows.Clear();
                }

                this.dtCalAuto = new DataTable();
                this.dtCalAuto.Columns.Add("Etapa", typeof(int));
                this.dtCalAuto.Columns.Add("Motor", typeof(string));
                this.dtCalAuto.Columns.Add("Etapa_Tentativa", typeof(string));
                this.dtCalAuto.Columns.Add("Volume", typeof(string));
                this.dtCalAuto.Columns.Add("VolumeDosado", typeof(string));
                this.dtCalAuto.Columns.Add("MassaIdeal", typeof(string));
                this.dtCalAuto.Columns.Add("MassaMedBalanca", typeof(string));               
                this.dtCalAuto.Columns.Add("Desvio_Med", typeof(string));
                this.dtCalAuto.Columns.Add("Desvio", typeof(string));
                this.dtCalAuto.Columns.Add("Aprovado", typeof(string));
                this.dtCalAuto.Columns.Add("Executado", typeof(string));

                if (calHist != null)
                {

                    foreach (Negocio.OperacaoAutoHist _op in calHist.listOperacaoAutoHist)
                    {
                        DataRow dr = this.dtCalAuto.NewRow();
                        dr["Etapa"] = _op.Etapa;
                        dr["Motor"] = _op.Motor.ToString();
                        dr["Etapa_Tentativa"] = _op.Etapa_Tentativa.ToString();
                        dr["Volume"] = _op.Volume.ToString();
                        dr["MassaIdeal"] = _op.MassaIdeal_str;
                       
                        dr["VolumeDosado"] = _op.VolumeDosadoMedia_str;
                        if (_op.Etapa > 0)
                        {                       
                            dr["MassaMedBalanca"] = _op.MassaMedBalancaMedia_str;
                            if (_op.MassaMedBalanca > 0 && _op.MassaIdeal > 0)
                            {
                                _op.Desvio_Med =((_op.MassaMedBalanca -  _op.MassaIdeal) / _op.MassaIdeal) * 100;
                                _op.Desvio_Med_str = string.Format("{0:N5}", _op.Desvio_Med);
                            }
                          
                        }
                        else
                        {
                            _op.Desvio_Med_str = "n/a";
                            if(_op.MassaMedBalanca_str == null || _op.MassaMedBalanca_str.Length == 0)
                            {
                                _op.MassaMedBalanca_str = "n/a";
                            }
                            dr["MassaMedBalanca"] = _op.MassaMedBalanca_str;
                        }
                        dr["Desvio_Med"] = _op.Desvio_Med_str;
                        dr["Desvio"] = _op.Desvio_str;
                        if(_op.Executado == 0)
                        {
                            dr["Aprovado"] = "";
                            dr["Executado"] = Negocio.IdiomaResxExtensao.Global_Nao;
                        }
                        else
                        {
                            if(_op.Aprovado == 0)
                            {
                                dr["Aprovado"] = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Reprovado;// "Reprovado";
                            }
                            else if (_op.Aprovado == 1)
                            {
                                dr["Aprovado"] = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Refazer;//"Refazer";
                            }
                            else
                            {
                                dr["Aprovado"] = "Ok";
                            }
                            
                            dr["Executado"] = Negocio.IdiomaResxExtensao.Global_Sim;
                        }

                        if (_op.MassaMedBalanca_str.Contains("#"))
                        {
                            string[] _str = _op.MassaMedBalanca_str.Split('#');
                            double totalM = 0;
                           
                            foreach (string _st in _str)
                            {
                                if (_st != null && _st.Length > 0)
                                {
                                    totalM += double.Parse(_st, CultureInfo.InvariantCulture);
                                   
                                }
                            }
                            _totalMassaMedBal += totalM;
                            
                        }
                        else
                        {
                            _totalMassaMedBal += _op.MassaMedBalanca;                           
                        }

                        if (_op.VolumeDosado_str.Contains("#"))
                        {
                            string[]  _str = _op.VolumeDosado_str.Split('#');
                            double totalV = 0;

                            foreach (string _st in _str)
                            {
                                if (_st != null && _st.Length > 0)
                                {
                                    totalV += double.Parse(_st, CultureInfo.InvariantCulture);
                                }
                            }

                            _totalVolumeDos += totalV;
                        }
                        else
                        {
                            _totalVolumeDos += _op.VolumeDosado;
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
                    dgvCalibracaoAuto.Rows.Add(this.dtCalAuto.Rows[i][0], this.dtCalAuto.Rows[i][1], this.dtCalAuto.Rows[i][2], this.dtCalAuto.Rows[i][3], this.dtCalAuto.Rows[i][4],
                        this.dtCalAuto.Rows[i][5], this.dtCalAuto.Rows[i][6], this.dtCalAuto.Rows[i][7], this.dtCalAuto.Rows[i][8], this.dtCalAuto.Rows[i][9], this.dtCalAuto.Rows[i][10]);
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("Etapa", typeof(int));
                dt.Columns.Add("Descricao", typeof(string));

                dt.Rows.Add(0, Negocio.IdiomaResxExtensao.CalibracaoAutomatica_PosicionamentoReciepiente);
                dt.Rows.Add(1, Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoPrimeiraCal);
                dt.Rows.Add(2, Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoFaixasCal);

                DataGridViewComboBoxColumn combOp = dgvCalibracaoAuto.Columns[0] as DataGridViewComboBoxColumn;
                combOp.DataSource = dt;
                combOp.DisplayMember = "Descricao";
                combOp.ValueMember = "Etapa";

                txtTotalMassaMedBal.Text = _totalMassaMedBal.ToString("N3");
                txtTotalVolumeDos.Text = _totalVolumeDos.ToString("N3");
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
                if (row.Cells["Executado"].Value.ToString() == Negocio.IdiomaResxExtensao.Global_Nao)// "Não")
                {
                    //228; 81; 76  E4514C
                    row.Cells["Executado"].Style.BackColor = Color.FromArgb(255, 228, 81, 76);
                }
                else
                {
                    row.Cells["Executado"].Style.BackColor = Color.FromArgb(255, 0, 128, 0);
                    if (row.Cells["Aprovado"].Value.ToString() == Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Reprovado)//"Reprovado")
                    {
                        //228; 81; 76  E4514C
                        row.Cells["Aprovado"].Style.BackColor = Color.FromArgb(255, 228, 81, 76);
                    }
                    else if(row.Cells["Aprovado"].Value.ToString() == Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Refazer)//"Refazer")
                    {
                        row.Cells["Aprovado"].Style.BackColor = Color.FromArgb(255, 225, 255, 0);
                    }
                    else
                    {
                        Color.FromArgb(255, 0, 128, 0); Color.FromArgb(255, 225, 255, 0);
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {  
                this.stopExecution = false; 
                this.numeroMedias = 0;
              
                this.DelayEntreBalanca = txt_delay_seg_bal.ToInt() * 1000;
                
                if(this.DelayEntreBalanca < 5000)
                {
                    this.DelayEntreBalanca = 5000;
                }

                this.posInicio = -1;
                if (cmbPortaSerial.SelectedIndex >= 0 || this.ModoDebug)
                {
                    if (this._tipoCalibacao == 1)
                    {
                        for (int i = 0; i < lCalibracaoAutoHistorico.Count; i++)
                        {
                            double volume = -1;
                            int etapa = -1;
                            for (int j = 0; j < this.lCalibracaoAutoHistorico[i].listOperacaoAutoHist.Count; j++)
                            {
                                if (this.lCalibracaoAutoHistorico[i].listOperacaoAutoHist[j].Etapa > 0)
                                {
                                    if (volume != this.lCalibracaoAutoHistorico[i].listOperacaoAutoHist[j].Volume)
                                    {
                                        bool existeExecutadoAprovado = false;
                                        int index = 0;
                                        volume = this.lCalibracaoAutoHistorico[i].listOperacaoAutoHist[j].Volume;
                                        etapa = this.lCalibracaoAutoHistorico[i].listOperacaoAutoHist[j].Etapa;

                                        for (index = j; index < this.lCalibracaoAutoHistorico[i].listOperacaoAutoHist.Count; index++)
                                        {
                                            if (etapa == 1 && this.lCalibracaoAutoHistorico[i].listOperacaoAutoHist[j].Etapa > 1)
                                            {
                                                break;
                                            }
                                            else if (volume != this.lCalibracaoAutoHistorico[i].listOperacaoAutoHist[index].Volume)
                                            {
                                                break;
                                            }
                                            else if (this.lCalibracaoAutoHistorico[i].listOperacaoAutoHist[index].Executado == 1 && this.lCalibracaoAutoHistorico[i].listOperacaoAutoHist[index].Aprovado == 2)
                                            {
                                                existeExecutadoAprovado = true;
                                                break;
                                            }
                                        }
                                        j = index - 1;
                                        if (!existeExecutadoAprovado)
                                        {
                                            posInicio = i;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (this.posInicio >= 0)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        if(cb_CalibracaoAuto.SelectedIndex >= 0)
                        {
                            this.posInicio = cb_CalibracaoAuto.SelectedIndex;
                        }
                    }

                    if (this.posInicio >= 0)
                    {
                        //Iciando desde o Posicionamento do Recipiente
                        for (int j = 0; j < this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist.Count; j++)
                        {
                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Executado = 0;
                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Aprovado = 0;

                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].VolumeDosado = 0;
                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].MassaMedBalanca = 0;

                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].MassaMedBalancaMedia_str = "";
                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].MassaMedBalanca_str = "";

                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].VolumeDosadoMedia_str = "";
                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].VolumeDosado_str = "";

                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Desvio_Med = 0;
                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Desvio_Med_str = "";
                        }
                        if (!this.ModoDebug)
                        {
                            this.interfaceBal.Start_Serial();
                        }
                        Util.ObjectCalibragem.gerarBkpCalibragem();
                        this.Invoke(new MethodInvoker(AttDGVBack));
                        this.Invoke(new MethodInvoker(iniciarCalibracaoAutomatica));
                        execCal = true;
                        btnStart.Enabled = false;
                    }
                }
                else
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAuto_SelecionarPortaSerialBalanca);
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void iniciarCalibracaoAutomatica()
        {
            bool isInicializarDisp = true;
            try
            {
                btnParar.Enabled = true;
                this.numeroMedias = 0;
                cmb_TipoCalibracao.Enabled = false;
                cmb_ProcessoCalibracao.Enabled = false;

                statusStrip1.Visible = true;
                this.mensagemStrip = string.Format(Negocio.IdiomaResxExtensao.CalibracaoAuto_PosicaoRecipiente, this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.Nome);
                AttStripMsg();

                this.stepPosicaoRec = 0;
                this.stepFaixaCalibracao = 0;
                this.stepPrimeiraCalibracao = 0;
                this.stepTrocarRec = 0;

                Util.ObjectParametros parametros = Util.ObjectParametros.Load();
                
                foreach(IDispenser _disp in this.ldisp)
                {
                    _disp.Disconnect();
                }

                this.ldisp.Clear();
                IDispenser dispenser = null;
                IDispenser dispenser2 = null;
                if(modBusDispenserMover_P3 != null)
                {
                    this.modBusDispenserMover_P3.Disconnect();
                    this.modBusDispenserMover_P3.Disconnect_Mover();
                }
                this.modBusDispenserMover_P3 = null;

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
                    case Dispositivo.Placa_3:
                        {
                            this.modBusDispenserMover_P3 = new ModBusDispenserMover_P3(parametros.NomeDispositivo, parametros.NomeDispositivo_PlacaMov);
                            break;
                        }
                }
                if (this.modBusDispenserMover_P3 == null)
                {
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
                }
            
                if (this.modBusDispenserMover_P3 != null)
                {
                    isInicializarDisp = false;

                    this.modBusDispenserMover_P3.Connect();
                    this.modBusDispenserMover_P3.Connect_Mover();
                    
                    this.modBusDispenserMover_P3.ReadSensores_Mover();

                    if (!this.modBusDispenserMover_P3.SensorEmergencia && this.modBusDispenserMover_P3.CodAlerta == 0 && this.modBusDispenserMover_P3.CodError == 0)
                    {
                        if (!this.modBusDispenserMover_P3.SensorValvulaDosagem)
                        {
                            this.modBusDispenserMover_P3.ValvulaPosicaoDosagem();
                            for (int _t = 0; _t < 20; _t++)
                            {
                                Thread.Sleep(500);
                                this.modBusDispenserMover_P3.ReadSensores_Mover();
                                if (this.modBusDispenserMover_P3.SensorValvulaDosagem)
                                {
                                    isInicializarDisp = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            isInicializarDisp = true;
                        }
                    }
                }
            
                if (isInicializarDisp)
                {
                    this.MassaBalancaOnLine = 0;
                    this.MassaBalancaOnLineInc = 0;
                    this.posInicioHistorico = 0;
                    cb_CalibracaoAuto.SelectedIndex = posInicio;

                    Util.ObjectColorante colorante = (Util.ObjectColorante)cb_CalibracaoAuto.SelectedItem;

                    lblCalibracaoAutoMotor.Text = colorante.Circuito.ToString();
                    lblCalibracaoAutoMassaEspecifica.Text = colorante.MassaEspecifica.ToString();

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
                else
                {
                    EnableBtnStart();
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
                            Thread.Sleep(2000);
                        }
                        else if(this.stopExecution)
                        {
                            this.isRunning = false;
                        }
                    }
                    Thread.Sleep(200);
                }

                worker = null;
            
                if(!this.execCal)
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                    {
                        if(this.stopExecution)
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAuto_Cancelado);
                        }
                        else if (this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Etapa == 0)
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAuto_ErrorRecipienteInadequado);
                        }
                        else
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAuto_ErrorSuporte);
                        }
                    }
                }
                else if (this.execCal && this.posInicio < this.lCalibracaoAutoHistorico.Count)
                {
                    if (this._tipoCalibacao == 1)
                    {
                        bool confirma = true;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            confirma = m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAuto_OK_NovaCalibracao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        if (confirma)
                        {
                            this.Invoke(new MethodInvoker(AttDGVBack));
                            this.Invoke(new MethodInvoker(iniciarCalibracaoAutomatica));
                        }
                        else
                        {
                            this.Invoke(new MethodInvoker(EnableBtnStart));
                        }
                    }
                    else
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAuto_OK, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        this.Invoke(new MethodInvoker(EnableBtnStart));
                    }
                }
                else if (this.execCal && this.posInicio >= this.lCalibracaoAutoHistorico.Count)
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                    {
                        m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAuto_OK, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                    }
                    this.Invoke(new MethodInvoker(EnableBtnStart));
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

        private void EnableBtnStart()
        {
            btnStart.Enabled = true;
            btnParar.Enabled = false;
            cmb_TipoCalibracao.Enabled = true;
            cmb_ProcessoCalibracao.Enabled = true;

            try
            {
                this.mensagemStrip = "";
                toolMessage.Text = this.mensagemStrip;
                toolProgress.Value = 0;
            
                if(this.modBusDispenserMover_P3 != null)
                {
                    this.modBusDispenserMover_P3.ValvulaPosicaoRecirculacao();
                }
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
                Util.ObjectColorante colorante = (Util.ObjectColorante)cb_CalibracaoAuto.SelectedItem;

                lblCalibracaoAutoMotor.Text = colorante.Circuito.ToString();
                lblCalibracaoAutoMassaEspecifica.Text = colorante.MassaEspecifica.ToString();
               
                atualizaDGView(this.lCalibracaoAutoHistorico[this.posInicio]);
                if (this.posInicioHistorico < this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist.Count)
                {                   
                    dgvCalibracaoAuto.Rows[this.posInicioHistorico].Selected = true;
                    int posScroll = (this.posInicioHistorico - 3 > 0) ? this.posInicioHistorico - 3 : 0;

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
                if(this.posInicioHistorico < this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist.Count)
                {
                    if(this.stopExecution)
                    {
                        bool confirma = true;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            confirma = m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAuto_Nova_Calibracao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        this.stopExecution = confirma;
                        if(this.stopExecution)
                        {
                            this.execCal = false;
                        }
                    }
                    if (!this.execCal)
                    {
                        this.posInicioHistorico = 0;
                        PausarMonitoramento();
                        this.Invoke(new MethodInvoker(EnableBtnStart));
                        return;
                    }
                    //Posição Recipiente
                    if (this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Etapa == 0)
                    {
                        if (!CapacidadeMaxRecipiente())
                        {
                            this.mensagemStrip = Negocio.IdiomaResxExtensao.CalibracaoAuto_Pos_Recipiente;
                            this.Invoke(new MethodInvoker(AttStripMsg));
                            PosicionarRecipiente();
                            this.Invoke(new MethodInvoker(AttDGVBack));
                        }
                        else
                        {
                            TrocarRecipiente();
                        }
                    }
                    //Primeira Calibracao
                    else if(this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Etapa == 1)
                    {
                        if (!CapacidadeMaxRecipiente())
                        {
                            PrimeiraCaliracao();
                        }
                        else
                        {
                            TrocarRecipiente();
                        }
                        return;
                    }
                    //Faixa Calibracao
                    else if (this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Etapa == 2)
                    {
                        if (!CapacidadeMaxRecipiente())
                        {
                            FaixaCaliracao();
                        }
                        else
                        {
                            TrocarRecipiente();
                        }
                        return;
                    }
                }
                else
                {
                    this.posInicioHistorico = 0;
                    this.posInicio++;
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
                double massaB = this.MassaBalancaOnLine + this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaIdeal;
                double volumeB = (massaB - this.MassaBalancaRecipiente) / this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.MassaEspecifica;
                if ((massaB > this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto.CapacideMaxBalanca) ||
                   (volumeB > this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto.VolumeMaxRecipiente) 
                   )
                {
                    retorno = true;
                    if (!this.isTrocarRec)
                    {
                        this.stepTrocarRec = 0;
                    }
                }
                else
                {
                    this.isTrocarRec = false;
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
                if (this.stepTrocarRec == 0)
                {
                    this.isTrocarRec = true;
                    this.stepPosicaoRec = 0;
                    this.mensagemStrip = Negocio.IdiomaResxExtensao.CalibracaoAuto_Trocar_Recipiente;
                    this.Invoke(new MethodInvoker(AttStripMsg));
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {                        
                        m.ShowDialog(string.Format(Negocio.IdiomaResxExtensao.CalibracaoAuto_MaxBalancaOuVolumeRecipiente, this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.Nome));
                    }

                    ClosedSerialDispensa();
                    this.stepTrocarRec++;
                    this.isRunning = false;
                }
                if (this.stepTrocarRec > 0)
                {
                    TrocarRecipiente1();
                }
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
                if (this.stepTrocarRec == 1)
                {
                    if (AbrirGaveta())
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            string msg = "";
                            if (this._processoCalibracao == 1)
                            {
                                msg = string.Format(Negocio.IdiomaResxExtensao.CalibracaoAuto_PosicaoRecipiente, this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.Nome);
                            }
                            else
                            {
                                msg = string.Format(Negocio.IdiomaResxExtensao.CalibracaoAutoSemi_PosicaoRecipiente, this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.Nome);
                            }
                            m.ShowDialog(msg);
                        }
                        this.isRunning = false;
                        this.stepTrocarRec++;
                    }
                }
                else if (this.stepTrocarRec == 2)
                {
                    if (this.modBusDispenserMover_P3 == null)
                    {
                        IDispenser _disp = this.ldisp[0];
                        bool conectar = Operar.Conectar(ref _disp, false);
                        bool temRecipiente = Operar.TemRecipiente(_disp, false);
                        
                        if (conectar && ((this._processoCalibracao == 2) || (temRecipiente)))
                        {
                            this.stepTrocarRec++;
                            this.dtInioLeituraBalanca = DateTime.Now;
                            ClosedSerialDispensa();
                        }
                        else
                        {
                            ClosedSerialDispensa();
                            this.stepTrocarRec = 0;
                            this.isTrocarRec = false;
                        }
                    }
                    else
                    {
                        this.modBusDispenserMover_P3.ReadSensores_Mover();
                        if(!this.modBusDispenserMover_P3.SensorCopo && !this.modBusDispenserMover_P3.SensorEsponja)
                        {
                            this.stepTrocarRec++;
                            this.dtInioLeituraBalanca = DateTime.Now;
                        }
                    }

                    this.isRunning = false;
                }
                else if (this.stepTrocarRec == 3)
                {
                    this.mensagemStrip = Negocio.IdiomaResxExtensao.CalibracaoAuto_Lendo_Balanca;
                    AttStripMsg();

                    if (DateTime.Now.Subtract(this.dtInioLeituraBalanca).TotalMilliseconds > this.DelayEntreBalanca)
                    {
                        byte[] b_tara = new byte[1];
                        byte[] b_w = new byte[1];
                        byte[] response = new byte[this.interfaceBal.tamanhoLeituraBalanca];
                        b_w[0] = (byte)this.interfaceBal.b_peso_balanca;
                        if (!this.ModoDebug)
                        {
                            this.interfaceBal.WriteSerialPort(b_w);
                            this.interfaceBal.GetResponse(ref response, false);
                        }
                        if (this.ModoDebug || this.interfaceBal.isTerminouRead)
                        {
                            if (this.ModoDebug)
                            {
                                this.interfaceBal.valorPeso = 0;                                
                            }
                            else
                            {
                                this.interfaceBal.SetValues(response, false, false);
                            }
                        }
                        else
                        {
                            this.isRunning = false;
                            this.execCal = false;

                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                            {
                                m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAuto_ErrorReadBalanca);
                            }
                            return;
                        }

                        if (this.interfaceBal.valorPeso < this.lCalibracaoAutoHistorico[this.posInicio].MinMassaAdmRecipiente)
                        {
                            this.MassaBalancaOnLine = this.interfaceBal.valorPeso;
                            this.MassaBalancaRecipiente = this.interfaceBal.valorPeso;

                            if (this.ModoDebug || this.interfaceBal.isTerminouRead)
                            {
                                this.interfaceBal.SetValues(response, false, false);
                                this.MassaBalancaOnLine = this.interfaceBal.valorPeso;
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
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAuto_ErrorReadBalanca);
                                }
                                return;
                            }
                        }
                        else
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                            {
                                m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAuto_PesoRecipienteExcedido);
                            }
                            this.stepTrocarRec = 1;
                            if (!this.stopExecution)
                            {
                                Thread.Sleep(1000);
                                this.Invoke(new MethodInvoker(TrocarRecipiente1));
                            }
                            return;
                        }
                    }
                    else
                    {
                        if (this.interfaceBal.valorPeso > this.lCalibracaoAutoHistorico[this.posInicio].MinMassaAdmRecipiente)
                        {
                            if (!this.stopExecution)
                            {
                                Thread.Sleep(1000);
                                this.Invoke(new MethodInvoker(TrocarRecipiente1));
                            }
                            else
                            {
                                this.isRunning = false;
                            }
                        }
                        else
                        {
                            this.isRunning = false;
                        }
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    this.isRunning = false;
            }
        }

        private void FaixaCaliracao()
        {
            try
            {
                if (this.IsDispensou)
                {
                    if (this.stepFaixaCalibracao == 0)
                    {                        
                        bool confirma = true;
                        if (this._processoCalibracao == 2)
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                string msg = string.Format(Negocio.IdiomaResxExtensao.CalibracaoAutoSemi_PosicaoRecipiente, this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.Nome);
                                confirma = m.ShowDialog(msg, Negocio.IdiomaResxExtensao.Global_Confirma, Negocio.IdiomaResxExtensao.Global_Cancelar);
                            }
                        }

                        if (confirma)
                        {
                            this.mensagemStrip = Negocio.IdiomaResxExtensao.CalibracaoAuto_Lendo_Balanca;
                            AttStripMsg();
                            this.stepFaixaCalibracao++;
                            this.dtInioLeituraBalanca = DateTime.Now;
                            this.isRunning = false;
                        }

                    }
                    if (this.stepFaixaCalibracao == 1)
                    {
                        if (DateTime.Now.Subtract(this.dtInioLeituraBalanca).TotalMilliseconds > this.DelayEntreBalanca)
                        {
                            byte[] b_w = new byte[1];
                            byte[] response = new byte[this.interfaceBal.tamanhoLeituraBalanca];
                            b_w[0] = (byte)this.interfaceBal.b_peso_balanca;
                            if (!this.ModoDebug)
                            {
                                this.interfaceBal.WriteSerialPort(b_w);
                                this.interfaceBal.GetResponse(ref response, false);
                            }
                            double massaAterior = this.MassaBalancaOnLine;
                            double massaDosada = 0;
                            bool isRealizarMediaMedica = false;
                            int _nDTM = 0;
                            for (int i = 0; i < this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto.listOperacaoAutomatica.Count; i++)
                            {
                                Negocio.OperacaoAutomatica opA = this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto.listOperacaoAutomatica[i];
                                if (opA.IsPrimeiraCalibracao == 0 && opA.Volume == this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[posInicioHistorico].Volume)
                                {
                                    if (opA.IsRealizarMediaMedicao == 1)
                                    {
                                        isRealizarMediaMedica = true;
                                        _nDTM = opA.NumeroDosagemTomadaMedia;
                                    }
                                    break;
                                }
                            }
                            if (isRealizarMediaMedica)
                            {
                                this.numeroMedias++;
                            }
                            if (this.ModoDebug || this.interfaceBal.isTerminouRead)
                            {
                                if (!this.ModoDebug)
                                {
                                    this.interfaceBal.SetValues(response, false, false);
                                }
                                else
                                {
                                    this.interfaceBal.valorPeso = massaAterior + this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaIdeal;
                                }
                                this.MassaBalancaOnLine = this.interfaceBal.valorPeso;
                                massaDosada = this.MassaBalancaOnLine - massaAterior;
                                if (isRealizarMediaMedica)
                                {
                                    double volDosado = massaDosada / this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.MassaEspecifica;
                                    if (this.numeroMedias < 2)
                                    {
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca_str = string.Format("{0:N5}", massaDosada);
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalancaMedia_str = string.Format("{0:N5}", massaDosada);
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado_str = string.Format("{0:N5}", volDosado);
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosadoMedia_str = string.Format("{0:N5}", volDosado);
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca = massaDosada;
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado = volDosado;
                                    }
                                    else
                                    {
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca_str += "#" + string.Format("{0:N5}", massaDosada);
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado_str += "#" + string.Format("{0:N5}", volDosado);
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca = massaDosada;
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado = volDosado;

                                        if (this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca_str.Contains("#"))
                                        {
                                            string[] _str = this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca_str.Split('#');
                                            double media = 0;
                                            int count = 0;
                                            foreach (string _st in _str)
                                            {
                                                if (_st != null && _st.Length > 0)
                                                {
                                                    media += double.Parse(_st, CultureInfo.InvariantCulture);
                                                    count++;
                                                }
                                            }
                                            media = media / count;
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalancaMedia_str = string.Format("{0:N5}", media);
                                        }

                                        if (this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado_str.Contains("#"))
                                        {
                                            string[] _str = this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado_str.Split('#');
                                            double media = 0;
                                            int count = 0;
                                            foreach (string _st in _str)
                                            {
                                                if (_st != null && _st.Length > 0)
                                                {
                                                    media += double.Parse(_st, CultureInfo.InvariantCulture);
                                                    count++;
                                                }
                                            }
                                            media = media / count;
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosadoMedia_str = string.Format("{0:N5}", media);
                                        }
                                    }
                                }
                                else
                                {
                                    this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca = massaDosada;
                                    this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca_str = string.Format("{0:N5}", massaDosada);
                                    this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalancaMedia_str = string.Format("{0:N5}", massaDosada);
                                    this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado =
                                                                                                                massaDosada / this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.MassaEspecifica;
                                    this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado_str =
                                                                                     string.Format("{0:N5}", this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado);
                                    this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosadoMedia_str =
                                                                                     string.Format("{0:N5}", this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado);
                                }
                            }
                            else
                            {
                                this.execCal = false;
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                                {
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAuto_ErrorReadBalanca);
                                }
                                this.isRunning = false;
                                return;
                            }
                            if (isRealizarMediaMedica)
                            {
                                if (this.numeroMedias >= _nDTM)
                                {
                                    this.numeroMedias = 0;
                                    double massaMedia = double.Parse(this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[posInicioHistorico].MassaMedBalancaMedia_str, CultureInfo.InvariantCulture);
                                    double desvio = Operar.CalcularDesvio(massaMedia, this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaIdeal);
                                    if (Math.Abs(desvio * 100) > this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Desvio)
                                    {
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Aprovado = 1;
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca = massaMedia;
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado = massaMedia / this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.MassaEspecifica; ;

                                        double pulsoRecalculado = lastPulsoHorario - (desvio * lastPulsoHorario);
                                        int posicaoEditada = 0;
                                        double vl_dat = _valores.Volume;
                                        for (int i = 0; i < this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem.Valores.Count; i++)
                                        {
                                            if (string.Format("{0:N5}", vl_dat).Equals(string.Format("{0:N5}", this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem.Valores[i].Volume)))
                                            {
                                                posicaoEditada = i;
                                                break;
                                            }
                                        }
                                        double pulso_por_volume_100_ml = (pulsoRecalculado / _valores.Volume) * vl_dat;

                                        RecalcularPulsosFaixaM(posicaoEditada, (int)Math.Round(pulso_por_volume_100_ml));
                                        Util.ObjectCalibracaoHistorico.Update(this.lCalibracaoAutoHistorico[this.posInicio]);

                                        this.posInicioHistorico++;
                                        if (this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Etapa_Tentativa == 1)
                                        {
                                            this.execCal = false;
                                        }
                                    }
                                    else
                                    {
                                        Util.ObjectCalibragem.Update(this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem);
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Aprovado = 2;

                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca = massaMedia;
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado = massaMedia / this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.MassaEspecifica; ;

                                        this.posInicioHistorico++;
                                        bool achouProximaFaixa = false;
                                        for (int j = this.posInicioHistorico; j < this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist.Count; j++)
                                        {
                                            if (this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Etapa == 2 && this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Etapa_Tentativa > 1)
                                            {
                                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Executado = 1;
                                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Aprovado = 1;
                                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].MassaMedBalanca = 0;
                                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].MassaMedBalanca_str = "";
                                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].VolumeDosado = 0;
                                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].VolumeDosado_str = "";

                                                this.posInicioHistorico = j;
                                            }
                                            else
                                            {
                                                achouProximaFaixa = true;
                                                this.posInicioHistorico = j;
                                                break;
                                            }
                                        }
                                        if (!achouProximaFaixa)
                                        {
                                            this.posInicioHistorico++;
                                        }
                                        Util.ObjectCalibracaoHistorico.Update(this.lCalibracaoAutoHistorico[this.posInicio]);
                                    }
                                    this.MassaBalancaOnLineInc += massaDosada;
                                    this.Invoke(new MethodInvoker(AttDGVBack));
                                }
                                else
                                {
                                    this.MassaBalancaOnLineInc += massaDosada;
                                    this.Invoke(new MethodInvoker(AttMassaRecipiente));
                                }
                            }
                            else
                            {
                                double desvio = Operar.CalcularDesvio(massaDosada, this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaIdeal);
                                if (Math.Abs(desvio * 100) > this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Desvio)
                                {
                                    this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Aprovado = 1;

                                    double pulsoRecalculado = lastPulsoHorario - (desvio * lastPulsoHorario);
                                    int posicaoEditada = 0;
                                    double vl_dat = _valores.Volume;
                                    for (int i = 0; i < this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem.Valores.Count; i++)
                                    {
                                        if (string.Format("{0:N5}", vl_dat).Equals(string.Format("{0:N5}", this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem.Valores[i].Volume)))
                                        {
                                            posicaoEditada = i;
                                            break;
                                        }
                                    }
                                    double pulso_por_volume_100_ml = (pulsoRecalculado / _valores.Volume) * vl_dat;

                                    RecalcularPulsosFaixaM(posicaoEditada, (int)Math.Round(pulso_por_volume_100_ml));
                                    Util.ObjectCalibracaoHistorico.Update(this.lCalibracaoAutoHistorico[this.posInicio]);

                                    this.posInicioHistorico++;
                                    if (this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Etapa_Tentativa == 1)
                                    {
                                        this.execCal = false;
                                    }
                                }
                                else
                                {
                                    Util.ObjectCalibragem.Update(this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem);
                                    this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Aprovado = 2;
                                    this.posInicioHistorico++;
                                    bool achouProximaFaixa = false;
                                    for (int j = this.posInicioHistorico; j < this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist.Count; j++)
                                    {
                                        if (this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Etapa == 2 && this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Etapa_Tentativa > 1)
                                        {
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Executado = 1;
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Aprovado = 1;
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].MassaMedBalanca = 0;
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].MassaMedBalanca_str = "";
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].MassaMedBalancaMedia_str = "";
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].VolumeDosado = 0;
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].VolumeDosado_str = "";
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].VolumeDosadoMedia_str = "";
                                            this.posInicioHistorico = j;
                                        }
                                        else
                                        {
                                            achouProximaFaixa = true;
                                            this.posInicioHistorico = j;
                                            break;
                                        }
                                    }
                                    if (!achouProximaFaixa)
                                    {
                                        this.posInicioHistorico++;
                                    }
                                    Util.ObjectCalibracaoHistorico.Update(this.lCalibracaoAutoHistorico[this.posInicio]);
                                }
                                this.MassaBalancaOnLineInc += massaDosada;
                                this.Invoke(new MethodInvoker(AttDGVBack));
                            }
                            this.IsDispensou = false;
                            this.isRunning = false;
                        }
                        this.isRunning = false;
                    }
                }
                else
                {
                    bool confirma = true;
                    if (this._processoCalibracao == 2)
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            string msg = string.Format(Negocio.IdiomaResxExtensao.CalibracaoAutoSemi_PosicaoRecipienteMaquina, this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.Nome);
                            confirma = m.ShowDialog(msg, Negocio.IdiomaResxExtensao.Global_Confirma, Negocio.IdiomaResxExtensao.Global_Cancelar);
                        }
                    }

                    if (confirma)
                    {
                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Executado = 1;
                        this.mensagemStrip = string.Format(Negocio.IdiomaResxExtensao.CalibracaoAuto_DispensandoVolume, this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Volume_str);
                        this.Invoke(new MethodInvoker(AttStripMsg));
                        ExecutarMonitoramento2();
                        this.stepFaixaCalibracao = 0;
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    this.isRunning = false;
                this.execCal = false;
            }
        }

        private void PrimeiraCaliracao()
        {
            try
            {
                if(this.IsDispensou)
                {
                    if (this.stepPrimeiraCalibracao == 0)
                    {
                        bool confirma = true;
                        if (this._processoCalibracao == 2)
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                string msg = string.Format(Negocio.IdiomaResxExtensao.CalibracaoAutoSemi_PosicaoRecipiente, this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.Nome);
                                confirma = m.ShowDialog(msg, Negocio.IdiomaResxExtensao.Global_Confirma, Negocio.IdiomaResxExtensao.Global_Cancelar);
                            }
                        }

                        if (confirma)
                        {
                            this.mensagemStrip = Negocio.IdiomaResxExtensao.CalibracaoAuto_Lendo_Balanca;
                            AttStripMsg();
                            this.dtInioLeituraBalanca = DateTime.Now;
                            this.stepPrimeiraCalibracao++;
                            this.isRunning = false;
                        }
                    }
                    else if (this.stepPrimeiraCalibracao == 1)
                    {
                        if (DateTime.Now.Subtract(this.dtInioLeituraBalanca).TotalMilliseconds > this.DelayEntreBalanca)
                        {
                            byte[] b_w = new byte[1];
                            byte[] response = new byte[this.interfaceBal.tamanhoLeituraBalanca];
                            b_w[0] = (byte)this.interfaceBal.b_peso_balanca;
                            if (!this.ModoDebug)
                            {
                                this.interfaceBal.WriteSerialPort(b_w);
                                this.interfaceBal.GetResponse(ref response, false);
                            }
                            double massaAterior = this.MassaBalancaOnLine;
                            double massaDosada = 0;
                            bool isRealizarMediaMedica = false;
                            int _nDTM = 0;
                            for (int i = 0; i < this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto.listOperacaoAutomatica.Count; i++)
                            {
                                Negocio.OperacaoAutomatica opA = this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto.listOperacaoAutomatica[i];
                                if (opA.IsPrimeiraCalibracao == 1 && opA.Volume == this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[posInicioHistorico].Volume)
                                {
                                    if (opA.IsRealizarMediaMedicao == 1)
                                    {
                                        isRealizarMediaMedica = true;
                                        _nDTM = opA.NumeroDosagemTomadaMedia;
                                    }
                                    break;
                                }
                            }
                            if (isRealizarMediaMedica)
                            {
                                this.numeroMedias++;
                            }
                            if (this.ModoDebug || this.interfaceBal.isTerminouRead)
                            {
                                if (!this.ModoDebug)
                                {
                                    this.interfaceBal.SetValues(response, false, false);
                                }
                                else
                                {
                                    this.interfaceBal.valorPeso = massaAterior + this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaIdeal;
                                }

                                this.MassaBalancaOnLine = this.interfaceBal.valorPeso;
                                massaDosada = this.MassaBalancaOnLine - massaAterior;
                                if (isRealizarMediaMedica)
                                {
                                    double volDosado = massaDosada / this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.MassaEspecifica;
                                    this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca = massaDosada;
                                    this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado = volDosado;

                                    if (this.numeroMedias < 2)
                                    {
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca_str = string.Format("{0:N5}", massaDosada);
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalancaMedia_str = string.Format("{0:N5}", massaDosada);
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado_str = string.Format("{0:N5}", volDosado);
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosadoMedia_str = string.Format("{0:N5}", volDosado);
                                    }
                                    else
                                    {
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca_str += "#" + string.Format("{0:N5}", massaDosada);
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado_str += "#" + string.Format("{0:N5}", volDosado);

                                        if (this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca_str.Contains("#"))
                                        {
                                            string[] _str = this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca_str.Split('#');
                                            double media = 0;
                                            int count = 0;
                                            foreach (string _st in _str)
                                            {
                                                if (_st != null && _st.Length > 0)
                                                {
                                                    media += double.Parse(_st, CultureInfo.InvariantCulture);
                                                    count++;
                                                }
                                            }
                                            media = media / count;
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalancaMedia_str = string.Format("{0:N5}", media);
                                        }

                                        if (this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado_str.Contains("#"))
                                        {
                                            string[] _str = this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado_str.Split('#');
                                            double media = 0;
                                            int count = 0;
                                            foreach (string _st in _str)
                                            {
                                                if (_st != null && _st.Length > 0)
                                                {
                                                    media += double.Parse(_st, CultureInfo.InvariantCulture);
                                                    count++;
                                                }
                                            }
                                            media = media / count;
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosadoMedia_str = string.Format("{0:N5}", media);
                                        }
                                    }
                                }
                                else
                                {
                                    this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca = massaDosada;
                                    this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca_str = string.Format("{0:N5}", massaDosada);
                                    this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalancaMedia_str = string.Format("{0:N5}", massaDosada);
                                    this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado =
                                                                                                                massaDosada / this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.MassaEspecifica;
                                    this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado_str =
                                                                                     string.Format("{0:N5}", this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado);
                                    this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosadoMedia_str =
                                                                                    string.Format("{0:N5}", this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado);

                                }
                            }
                            else
                            {
                                this.execCal = false;
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                                {
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAuto_ErrorReadBalanca);
                                }
                                this.isRunning = false;
                                return;
                            }

                            if (isRealizarMediaMedica)
                            {
                                if (this.numeroMedias >= _nDTM)
                                {
                                    this.numeroMedias = 0;
                                    double massaMedia = double.Parse(this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[posInicioHistorico].MassaMedBalancaMedia_str, CultureInfo.InvariantCulture);
                                    double desvio = Operar.CalcularDesvio(massaMedia, this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaIdeal);
                                    if (Math.Abs(desvio * 100) > this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Desvio)
                                    {
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Aprovado = 1;
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca = massaMedia;
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado = massaMedia / this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.MassaEspecifica; ;

                                        double pulsoRecalculado = lastPulsoHorario - (desvio * lastPulsoHorario);
                                        int posicaoEditada = 0;
                                        double vl_dat = this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem.Valores[posicaoEditada].Volume;

                                        double pulso_por_volume_100_ml = (pulsoRecalculado / _valores.Volume) * vl_dat;

                                        RecalcularPulsosM(posicaoEditada, (int)Math.Round(pulso_por_volume_100_ml));
                                        Util.ObjectCalibracaoHistorico.Update(this.lCalibracaoAutoHistorico[this.posInicio]);

                                        this.posInicioHistorico++;
                                        if (this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Etapa_Tentativa == 1)
                                        {
                                            this.execCal = false;
                                        }
                                    }
                                    else
                                    {
                                        Util.ObjectCalibragem.Update(this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem);
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Aprovado = 2;
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca = massaMedia;
                                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado = massaMedia / this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.MassaEspecifica; ;

                                        this.posInicioHistorico++;
                                        bool achouProximaFaixa = false;
                                        for (int j = this.posInicioHistorico; j < this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist.Count; j++)
                                        {
                                            if (this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Etapa == 1)
                                            {
                                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Executado = 1;
                                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Aprovado = 1;
                                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].MassaMedBalanca = 0;
                                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].MassaMedBalanca_str = "";
                                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].VolumeDosado = 0;
                                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].VolumeDosado_str = "";

                                                this.posInicioHistorico = j;
                                            }
                                            else
                                            {
                                                achouProximaFaixa = true;
                                                this.posInicioHistorico = j;
                                                break;
                                            }
                                        }
                                        if (!achouProximaFaixa)
                                        {
                                            this.posInicioHistorico++;
                                        }
                                        Util.ObjectCalibracaoHistorico.Update(this.lCalibracaoAutoHistorico[this.posInicio]);
                                    }
                                    this.MassaBalancaOnLineInc += massaDosada;
                                    this.Invoke(new MethodInvoker(AttDGVBack));
                                }
                                else
                                {
                                    this.MassaBalancaOnLineInc += massaDosada;
                                    this.Invoke(new MethodInvoker(AttMassaRecipiente));
                                }
                            }
                            else
                            {
                                double desvio = Operar.CalcularDesvio(massaDosada, this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaIdeal);
                                if (Math.Abs(desvio * 100) > this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Desvio)
                                {
                                    this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Aprovado = 1;

                                    double pulsoRecalculado = lastPulsoHorario - (desvio * lastPulsoHorario);
                                    int posicaoEditada = 0;
                                    double vl_dat = this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem.Valores[posicaoEditada].Volume;

                                    double pulso_por_volume_100_ml = (pulsoRecalculado / _valores.Volume) * vl_dat;
                                    RecalcularPulsosM(posicaoEditada, (int)Math.Round(pulso_por_volume_100_ml));

                                    this.posInicioHistorico++;
                                    Util.ObjectCalibracaoHistorico.Update(this.lCalibracaoAutoHistorico[this.posInicio]);
                                    if (this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Etapa_Tentativa == 1)
                                    {
                                        this.execCal = false;
                                    }
                                }
                                else
                                {
                                    this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Aprovado = 2;
                                    Util.ObjectCalibracaoHistorico.Update(this.lCalibracaoAutoHistorico[this.posInicio]);
                                    this.posInicioHistorico++;
                                    for (int j = this.posInicioHistorico; j < this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist.Count; j++)
                                    {
                                        if (this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Etapa == 1)
                                        {
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Executado = 1;
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Aprovado = 1;
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].MassaMedBalanca = 0;
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].MassaMedBalanca_str = "";
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].MassaMedBalancaMedia_str = "";
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].VolumeDosado = 0;
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].VolumeDosado_str = "";
                                            this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].VolumeDosadoMedia_str = "";
                                        }
                                        else
                                        {
                                            this.posInicioHistorico = j;
                                            break;
                                        }
                                    }
                                    Util.ObjectCalibracaoHistorico.Update(this.lCalibracaoAutoHistorico[this.posInicio]);
                                }
                                this.MassaBalancaOnLineInc += massaDosada;
                                this.Invoke(new MethodInvoker(AttDGVBack));
                            }

                            this.IsDispensou = false;
                            this.isRunning = false;
                            ClosedSerialDispensa();
                        }

                        this.isRunning = false;
                    }
                }
                else
                {
                    bool confirma = true;
                    if (this._processoCalibracao == 2)
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            string msg = string.Format(Negocio.IdiomaResxExtensao.CalibracaoAutoSemi_PosicaoRecipienteMaquina, this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.Nome);
                            m.ShowDialog(msg, Negocio.IdiomaResxExtensao.Global_Confirma, Negocio.IdiomaResxExtensao.Global_Cancelar);
                        }
                    }

                    if (confirma)
                    {
                        this.stepPrimeiraCalibracao = 0;
                        this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Executado = 1;
                        this.mensagemStrip = string.Format(Negocio.IdiomaResxExtensao.CalibracaoAuto_DispensandoVolume, this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Volume_str);
                        this.Invoke(new MethodInvoker(AttStripMsg));
                        ExecutarMonitoramento2();
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    this.isRunning = false;
                this.execCal = false;
            }
        }

        private void PosicionarRecipiente()
        {
            try
            {
                if (this.stepPosicaoRec == 0)
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        string msg = "";
                        if (this._processoCalibracao == 1)
                        {
                            msg = string.Format(Negocio.IdiomaResxExtensao.CalibracaoAuto_PosicaoRecipiente, this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.Nome);
                        }
                        else
                        {
                            msg = string.Format(Negocio.IdiomaResxExtensao.CalibracaoAutoSemi_PosicaoRecipiente, this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.Nome);
                        }
                        m.ShowDialog(msg);
                    }
                    this.stepPosicaoRec++;
                }
                else if (this.stepPosicaoRec == 1)
                {
                    if (FecharGaveta())
                    {
                        if (this.modBusDispenserMover_P3 == null)
                        {
                            IDispenser _disp = this.ldisp[0];
                            bool conectar = Operar.Conectar(ref _disp, false);
                            bool temRecipiente = Operar.TemRecipiente(_disp, false);
                            if (conectar && ((this._processoCalibracao == 2) || (temRecipiente)))
                            {
                                this.dtInioLeituraBalanca = DateTime.Now;
                                this.stepPosicaoRec++;
                            }
                            else
                            {
                                this.stepPosicaoRec = 0;
                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Executado = 1;
                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Aprovado = 0;
                                Util.ObjectCalibracaoHistorico.Update(this.lCalibracaoAutoHistorico[this.posInicio]);
                                this.posInicioHistorico++;
                                if (this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Etapa > 0)
                                {
                                    this.execCal = false;
                                }
                                ClosedSerialDispensa();
                            }
                        }
                        else
                        {
                            this.dtInioLeituraBalanca = DateTime.Now;
                            this.stepPosicaoRec++;
                        }
                    }
                }
                else if (this.stepPosicaoRec == 2)
                {
                    this.mensagemStrip = Negocio.IdiomaResxExtensao.CalibracaoAuto_Lendo_Balanca;
                    AttStripMsg();
                    
                    if (DateTime.Now.Subtract(this.dtInioLeituraBalanca).TotalMilliseconds > this.DelayEntreBalanca)
                    {
                        byte[] b_tara = new byte[1];
                        byte[] b_w = new byte[1];
                        byte[] response = new byte[this.interfaceBal.tamanhoLeituraBalanca];

                        ClosedSerialDispensa();
                        b_w[0] = (byte)this.interfaceBal.b_peso_balanca;
                        if (!this.ModoDebug)
                        {
                            this.interfaceBal.WriteSerialPort(b_w);
                            this.interfaceBal.GetResponse(ref response, false);
                        }
                        if (this.ModoDebug || this.interfaceBal.isTerminouRead)
                        {
                            if (!this.ModoDebug)
                            {
                                this.interfaceBal.SetValues(response, false, false);
                            }
                            else
                            {
                                this.interfaceBal.valorPeso = 0;
                            }
                            this.MassaBalancaOnLine = this.interfaceBal.valorPeso;
                            if (this.MassaBalancaOnLine < this.lCalibracaoAutoHistorico[this.posInicio].MinMassaAdmRecipiente)
                            {
                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Executado = 1;
                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Aprovado = 2;
                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca = 0;
                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].MassaMedBalanca_str = "0";
                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado = 0;
                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].VolumeDosado_str = "0";
                                Util.ObjectCalibracaoHistorico.Update(this.lCalibracaoAutoHistorico[this.posInicio]);
                            }
                            else
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                                {
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAuto_PesoRecipienteExcedido);
                                }
                                this.stepPosicaoRec = 0;
                                return;
                            }
                        }
                        else
                        {
                            this.execCal = false;
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                            {
                                m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAuto_ErrorReadBalanca);
                            }
                            return;
                        }

                        this.MassaBalancaOnLineInc = this.MassaBalancaOnLine;
                        this.MassaBalancaRecipiente = this.MassaBalancaOnLine;

                        for (int j = this.posInicioHistorico + 1; j < this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist.Count; j++)
                        {
                            if (this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Etapa == 0)
                            {
                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Executado = 1;
                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].Aprovado = 2;
                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].MassaMedBalanca = 0;
                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].MassaMedBalanca_str = "0";
                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].VolumeDosado = 0;
                                this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[j].VolumeDosado_str = "0";
                                Util.ObjectCalibracaoHistorico.Update(this.lCalibracaoAutoHistorico[this.posInicio]);
                            }
                            else
                            {
                                this.posInicioHistorico = j;
                                break;
                            }
                        }
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
        
        private void ClosedSerialDispensa()
        {
            try
            {
                if (this.modBusDispenserMover_P3 == null)
                {
                    foreach (IDispenser _disp in this.ldisp)
                    {
                        _disp.Disconnect();
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private bool AbrirGaveta()
        {
            bool retorno = true;
            try
            {
                if(this.modBusDispenserMover_P3 != null)
                {
                    retorno = false;                    
                    this.modBusDispenserMover_P3.ReadSensores_Mover();
                    if (this.modBusDispenserMover_P3.SensorEmergencia)
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            bool isRec = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Botao_Emergencia_Passos, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true, 60);
                            if (isRec)
                            {
                                PausarMonitoramento();
                            }
                        }
                    }
                    else if (!this.modBusDispenserMover_P3.SensorGavetaAberta)
                    {
                        this.modBusDispenserMover_P3.AbrirGaveta(true);
                        for(int _t = 0; _t < 20; _t++)
                        {
                            Thread.Sleep(500);
                            if(this.modBusDispenserMover_P3.TerminouProcessoDuplo)
                            {
                                this.modBusDispenserMover_P3.ReadSensores_Mover();
                                if(this.modBusDispenserMover_P3.SensorGavetaAberta)
                                {
                                    retorno = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        retorno = true;
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			return retorno;
        }

        private bool FecharGaveta()
        {
            bool retorno = true;
            try
            {
                if (this.modBusDispenserMover_P3 != null)
                {
                    retorno = false;
                    this.modBusDispenserMover_P3.ReadSensores_Mover();
                    if (this.modBusDispenserMover_P3.SensorEmergencia)
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            bool isRec = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Botao_Emergencia_Passos, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true, 60);
                            if (isRec)
                            {
                                PausarMonitoramento();
                            }
                        }
                    }
                    else if (!this.modBusDispenserMover_P3.SensorGavetaFechada)
                    {
                        if (this.modBusDispenserMover_P3.SensorCopo)
                        {
                            this.modBusDispenserMover_P3.FecharGaveta(true);
                            for (int _t = 0; _t < 20; _t++)
                            {
                                Thread.Sleep(500);
                                if (this.modBusDispenserMover_P3.TerminouProcessoDuplo)
                                {
                                    this.modBusDispenserMover_P3.ReadSensores_Mover();
                                    if (this.modBusDispenserMover_P3.SensorGavetaFechada)
                                    {
                                        retorno = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        retorno = true;
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			return retorno;
        }

        #region Dosagem Placa

        void Monitoramento_Event2()
        {
            try
            {
                /* Verifica se dispenser está liberado.
                 * isso representa que o processo foi concluído. */
                
                int motor = this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem.Motor - 1;

                List<Util.ObjectColorante> ncol = null;
                if (this.lColSeguidor != null)
                {
                    ncol = this.lColSeguidor.FindAll(o => o.Seguidor == this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem.Motor).ToList();
                }
                if (this.modBusDispenserMover_P3 == null)
                {
                    if (this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.Dispositivo == 1)
                    {
                        if (!this.ldisp[0].IsReady)
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
                                Util.ObjectCalibragem.UpdatePulsosRev(this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem.Motor, _valores.PulsoReverso);
                                _valores.PulsoHorario = _valores.PulsoHorario - _valores.PulsoReverso;
                                lastPulsoHorario = lastPulsoHorario - _valores.PulsoReverso;
                            }
                            
                            return;
                        }
                        _valores = Operar.Parser(this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Volume,
                            this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem.Valores, this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem.UltimoPulsoReverso);
                        lastPulsoHorario = _valores.PulsoHorario;

                        if (ncol != null && ncol.Count > 0)
                        {
                            int[] n_motor = new int[ncol.Count + 1];

                            n_motor[0] = motor;
                            for (int i = 0; i < ncol.Count; i++)
                            {                                
                                n_motor[i + 1] = (ncol[i].Circuito - 1);                                
                            }
                            //Dispara thread para enviar dados ao dispositivo
                            Task task = Task.Factory.StartNew(() => this.ldisp[0].Dispensar(
                                              n_motor,
                                             _valores.PulsoHorario,
                                             _valores.Velocidade,
                                             _valores.Aceleracao,
                                             _valores.Delay,
                                             _valores.PulsoReverso,
                                             i_Step)
                                         );
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.ldisp[0].Dispensar(
                                             motor,
                                            _valores.PulsoHorario,
                                            _valores.Velocidade,
                                            _valores.Aceleracao,
                                            _valores.Delay,
                                            _valores.PulsoReverso,
                                            i_Step)
                                        );
                        }
                    }
                    else if (this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.Dispositivo == 2)
                    {
                        int indexV2 = 16;
                        if((Dispositivo)_parametro.IdDispositivo == Dispositivo.Simulador)
                        {
                            motor = motor - 16;
                            indexV2 = 16;
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
                                Util.ObjectCalibragem.UpdatePulsosRev(this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem.Motor, _valores.PulsoReverso);
                                lastPulsoHorario = lastPulsoHorario - _valores.PulsoReverso;
                            }
                            
                            return;
                        }
                        _valores = Operar.Parser(this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Volume,
                           this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem.Valores, this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem.UltimoPulsoReverso);
                        lastPulsoHorario = _valores.PulsoHorario;

                        if (ncol != null && ncol.Count > 0)
                        {
                            int[] n_motor = new int[ncol.Count + 1];

                            n_motor[0] = motor;
                            for (int i = 0; i < ncol.Count; i++)
                            {
                                n_motor[i + 1] = ((ncol[i].Circuito - indexV2) - 1);
                            }
                            //Dispara thread para enviar dados ao dispositivo
                            Task task = Task.Factory.StartNew(() => this.ldisp[1].Dispensar(
                                           n_motor,
                                          _valores.PulsoHorario,
                                          _valores.Velocidade,
                                          _valores.Aceleracao,
                                          _valores.Delay,
                                          _valores.PulsoReverso,
                                          i_Step)
                                      );
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.ldisp[1].Dispensar(
                                             motor,
                                            _valores.PulsoHorario,
                                            _valores.Velocidade,
                                            _valores.Aceleracao,
                                            _valores.Delay,
                                            _valores.PulsoReverso,
                                            i_Step)
                                        );
                        }
                    }
                }
                else
                {
                    if (this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._colorante.Dispositivo == 1)
                    {
                        if (!this.modBusDispenserMover_P3.IsReady)
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
                                Util.ObjectCalibragem.UpdatePulsosRev(this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem.Motor, _valores.PulsoReverso);
                                _valores.PulsoHorario = _valores.PulsoHorario - _valores.PulsoReverso;
                                lastPulsoHorario = lastPulsoHorario - _valores.PulsoReverso;
                            }
                            
                            return;
                        }
                        _valores = Operar.Parser(this.lCalibracaoAutoHistorico[this.posInicio].listOperacaoAutoHist[this.posInicioHistorico].Volume,
                            this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem.Valores, this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem.UltimoPulsoReverso);
                        lastPulsoHorario = _valores.PulsoHorario;

                        Task task = Task.Factory.StartNew(() => this.modBusDispenserMover_P3.Dispensar(
                                             motor,
                                            _valores.PulsoHorario,
                                            _valores.Velocidade,
                                            _valores.Aceleracao,
                                            _valores.Delay,
                                            _valores.PulsoReverso,
                                            i_Step)
                                        );
                    }
                }

                this.IsDispensou = true;
                this.isRunning2 = false;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    
                if (!this.IsDispensou)
                {
					string customMessage = string.Empty;
					if (ex.Message.Contains("Could not read status register:"))
						customMessage = Negocio.IdiomaResxExtensao.Global_Falha_PerdaConexaoDispositivo;

					Falha2(ex, customMessage);
                }
                this.isRunning2 = false;
            }
        }

        void Falha2(Exception ex, string customMessage = null)
        {           
            PausarMonitoramento2();

            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
            {
                m.ShowDialog(
					string.IsNullOrWhiteSpace(customMessage) ? Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + ex.Message
                                                             : customMessage);
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
            
                foreach(IDispenser _disp in this.ldisp)
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
                        if (!this.isRunning2)
                        {
                            this.isRunning2 = true;
                            this.Invoke(new MethodInvoker(Monitoramento_Event2));
                            Thread.Sleep(2500);
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

        void RecalcularPulsosM(int indexInicio, double pulsosFaixaInicial)
        {
            Util.ObjectCalibragem _calibragem = this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem;

            _calibragem.Valores[indexInicio].PulsoHorario = (int)Math.Round(pulsosFaixaInicial);

            /* Zera valores de massa e desvio da posição inicial */
            _calibragem.Valores[indexInicio].MassaMedia = 0;
            _calibragem.Valores[indexInicio].DesvioMedio = 0;

            for (int indexAtual = indexInicio + 1; indexAtual < this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem.Valores.Count; indexAtual++)
            {
                /* Recalcula pulsos das faixas abaixo da posição inicial */
                double pulsoRecalculado =
                    (_calibragem.Valores[indexAtual].Volume * pulsosFaixaInicial) / _calibragem.Valores[indexInicio].Volume;

                /* Atualiza valores recalculados na tela */
                _calibragem.Valores[indexAtual].PulsoHorario = (int)Math.Round(pulsoRecalculado);
                _calibragem.Valores[indexAtual].MassaMedia = 0;
                _calibragem.Valores[indexAtual].DesvioMedio = 0;
            }
        }

        void RecalcularPulsosFaixaM(int indexPosicao, double pulsosFaixaInicial)
        {
            Util.ObjectCalibragem _calibragem = this.lCalibracaoAutoHistorico[this.posInicio]._calibracaoAuto._calibragem;
            /* Pulsos da posição inicial a ser utilizado para recalcular demais posições */

            _calibragem.Valores[indexPosicao].PulsoHorario = (int)Math.Round(pulsosFaixaInicial);

            /* Zera valores de massa e desvio da posição inicial */
            _calibragem.Valores[indexPosicao].MassaMedia = 0;
            _calibragem.Valores[indexPosicao].DesvioMedio = 0;
            
            /* Recalcula pulsos das faixas abaixo da posição inicial */
            double pulsoRecalculado =
                (_calibragem.Valores[indexPosicao].Volume * pulsosFaixaInicial) / _calibragem.Valores[indexPosicao].Volume;

            /* Atualiza valores recalculados na tela */
            _calibragem.Valores[indexPosicao].PulsoHorario = (int)Math.Round(pulsoRecalculado);
            _calibragem.Valores[indexPosicao].MassaMedia = 0;
            _calibragem.Valores[indexPosicao].DesvioMedio = 0;
        }

        private void btnParar_Click(object sender, EventArgs e)
        {
            this.stopExecution = true;
        }        
    }
}