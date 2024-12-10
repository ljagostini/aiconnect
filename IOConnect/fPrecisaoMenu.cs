using Percolore.Core;
using Percolore.Core.Logging;
using Percolore.Core.Persistence.WindowsRegistry;
using Percolore.IOConnect.Util;
using System.ComponentModel;
using System.Data;
using System.IO.Ports;

namespace Percolore.IOConnect
{
	public partial class fPrecisaoMenu : Form
    {
        private int inicializacaoPG = -1;       
        public Brush _Brush = Brushes.ForestGreen;

        System.ComponentModel.BackgroundWorker bgWork = null;
        private bool isThreadWork = false;
        private bool isRunningWork = false;

        private List<Util.ObjectColorante> listColorantes = new List<Util.ObjectColorante>();
        private Negocio.Accurracy acurracy = new Negocio.Accurracy();

        private Util.ObjectColorante colorante = null;
        public Util.ObjectCalibragem calibracao = null;
        private Util.ObjectParametros _parametro;

        private DataTable dtPrecisaoAuto = null;
        private string mensagemStrip = "";
        int posInicio = -1;

        System.ComponentModel.BackgroundWorker backgroundWorker1 = null;
        private bool isThread = false;
        private bool isRunning = false;
        bool execCal = false;

        private List<IDispenser> ldisp = new List<IDispenser>();
        //private IDispenser disp;
        //private Negocio.Balanca_BEL balanca_bel = new Negocio.Balanca_BEL();

        private Negocio.InterfaceBalanca interfaceBal = null;

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

        private int stepPosicaoRec = 0;
        private DateTime dtInioLeituraBalanca = DateTime.Now;
        private bool isPosicaoRecipiente = false;
        private bool istrocarRecipiente = false;

        private int stepFaixaPrecisao = 0;

        private bool stopExecution = false;
        private bool ClosingWindows = false;

        private bool ModoDebug = false;

        List<Util.ObjectColorante> lColSeguidor = null;

        private int i_Step = 0;

        public fPrecisaoMenu()
        {
            InitializeComponent();
            try
            {
                int perc = (Screen.PrimaryScreen.Bounds.Width * 5) / 100;
                toolProgress.Size = new Size((Screen.PrimaryScreen.Bounds.Width - perc) - toolProgress.Margin.Left, 35);

                panel1.BackColor = System.Drawing.Color.Transparent;
                panel2.BackColor = System.Drawing.Color.Transparent;
                panel3.BackColor = System.Drawing.Color.Transparent;
                panel4.BackColor = System.Drawing.Color.Transparent;
                panel5.BackColor = System.Drawing.Color.Transparent;
                panel6.BackColor = System.Drawing.Color.Transparent;
                panel7.BackColor = System.Drawing.Color.Transparent;
                panel8.BackColor = System.Drawing.Color.Transparent;

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void fPrecisaoMenu_Load(object sender, EventArgs e)
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
            
                this.lblTitulo.Text = Negocio.IdiomaResxExtensao.CalibracaoAutomate_lblTitulo;
                this.lblConfigSetup.Text = Negocio.IdiomaResxExtensao.CalibracaoAutomate_lblConfigSetup;
                this.lblModelScale.Text = Negocio.IdiomaResxExtensao.CalibracaoAutomate_lblModelScale;
                this.btnEditarDiretorio.Text = Negocio.IdiomaResxExtensao.CalibracaoAutomate_btnEditarDiretorio;
                this.lblExecCircuito.Text = Negocio.IdiomaResxExtensao.CalibracaoAutomate_lblExecCircuito;
                this.lblFileImport.Text = Negocio.IdiomaResxExtensao.CalibracaoAutomate_lblFileImport;
                this.lblDelayScale.Text = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_lblDelayBalanca;
                this.lblTituloMassaBal.Text = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_lblTituloMassaBal;
                this.lblVolumeMaxRecipiente.Text = Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoVolumeMaxRecipiente;
                this.lblMinMassaAdmRecipiente.Text = Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoPesoRecipiente;
                this.lblMassaEspecifica.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemLegendaMassaEspec;
                this.lblMaxCapRecipiente.Text = Negocio.IdiomaResxExtensao.CalibracaoAutomatica_lblCapacidadeMaxBalanca;
                this.btnStart.Text = Negocio.IdiomaResxExtensao.Global_Start;
                this.btnCancel.Text = Negocio.IdiomaResxExtensao.Global_Stop;
                this.btnHelp.Text = Negocio.IdiomaResxExtensao.Global_BtnAjuda;
                this.btnClose.Text = Negocio.IdiomaResxExtensao.Global_BtnClose;
            
                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    this.acurracy.NumeroSerie = percRegistry.GetSerialNumber();
                }
                this.acurracy.listPrecisao = new List<Negocio.Precisao>();
                AttStripMsg();
                
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

                listColorantes = Util.ObjectColorante.List().Where(c => c.Habilitado == true && c.Seguidor == -1).ToList();

                DataTable dtGridCircuitos = new DataTable();
                dtGridCircuitos.Columns.Add("Circuito1", typeof(string));
                dtGridCircuitos.Columns.Add("Circuito2", typeof(string));
                dtGridCircuitos.Columns.Add("Circuito3", typeof(string));
                dtGridCircuitos.Columns.Add("Circuito4", typeof(string));
                dtGridCircuitos.Columns.Add("Circuito5", typeof(string));
                for(int i = 0; i < listColorantes.Count; i++)
                {
                    DataRow drC = dtGridCircuitos.NewRow();
                    drC["Circuito1"] = Negocio.IdiomaResxExtensao.CalibracaoAutomate_Circuit + " " + String.Format("{0:00}", listColorantes[i].Circuito) + "  " + listColorantes[i].Nome;
                    i++;
                    if(i < listColorantes.Count)
                    {
                        drC["Circuito2"] = Negocio.IdiomaResxExtensao.CalibracaoAutomate_Circuit + "  " + String.Format("{0:00}", listColorantes[i].Circuito) + "  " + listColorantes[i].Nome;
                        i++;
                        if (i < listColorantes.Count)
                        {
                            drC["Circuito3"] = Negocio.IdiomaResxExtensao.CalibracaoAutomate_Circuit + "  " + String.Format("{0:00}", listColorantes[i].Circuito) + "  " + listColorantes[i].Nome;
                            i++;
                            if (i < listColorantes.Count)
                            {
                                drC["Circuito4"] = Negocio.IdiomaResxExtensao.CalibracaoAutomate_Circuit + "  " + String.Format("{0:00}", listColorantes[i].Circuito) + "  " + listColorantes[i].Nome;
                                i++;
                                if (i < listColorantes.Count)
                                {
                                    drC["Circuito5"] = Negocio.IdiomaResxExtensao.CalibracaoAutomate_Circuit + "  " + String.Format("{0:00}", listColorantes[i].Circuito) + "  " + listColorantes[i].Nome;                                       
                                }
                            }
                        }
                    }

                    dtGridCircuitos.Rows.Add(drC);
                }

                if (dgvCircuit.Rows != null)
                {
                    dgvCircuit.Rows.Clear();
                }
                for (int i = 0; i < dtGridCircuitos.Rows.Count; i++)
                {
                    dgvCircuit.Rows.Add(dtGridCircuitos.Rows[i][0], dtGridCircuitos.Rows[i][1], dtGridCircuitos.Rows[i][2], dtGridCircuitos.Rows[i][3], dtGridCircuitos.Rows[i][4]);
                }

                this.colorante = this.listColorantes[0];
                this.calibracao = Util.ObjectCalibragem.Load(this.colorante.Circuito);
                atualizaDetailColorante();
                AttListPrecisao();

                atualizaPortasComunicacaoBalanca();
                
                iniciarbgWork();
                enableStatus(false);
            
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
            
                this.lColSeguidor = Util.ObjectColorante.List().FindAll(o => o.Seguidor > 0).ToList();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void fPrecisaoMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                ClosedSerialDispensa();
                if (this.interfaceBal != null)
                {
                    this.interfaceBal.CloseSerial();
                }
                this.ClosingWindows = true;
                this.isThreadWork = false;
                this.isThread = false;
                this.isThread2 = false;
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
                    cmbPortaSerial.SelectedIndex = 0;
                    cmbTipoBalanca_SelectedIndexChanged(null, null);
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

        private void dgvCircuit_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int c_index = e.ColumnIndex;
                if (c_index >= 0)
                {
                    int r_index = e.RowIndex ;
                    
                    int index = (r_index * 5) + c_index;
                    if (index < this.listColorantes.Count)
                    {

                        if (this.colorante == null || this.colorante.Circuito != this.listColorantes[index].Circuito)
                        {
                            this.colorante = this.listColorantes[index];
                            this.calibracao = Util.ObjectCalibragem.Load(this.colorante.Circuito);
                            atualizaDetailColorante();
                            AttListPrecisao();
                        }

                        dgvCircuit.Refresh();
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void AttListPrecisao()
        {
            try
            {
                if (this.acurracy.listPrecisao == null)
                {
                    this.acurracy.listPrecisao = new List<Negocio.Precisao>();
                }
                if (txt_PathFileImport.Text.Length > 0)
                {
                    this.acurracy.listPrecisao.Clear();
                    List<String> lReadFile = new List<string>();
                    using (StreamReader sr = new StreamReader(txt_PathFileImport.Text))
                    {
                        while (sr.Peek() >= 0)
                        {
                            string str = sr.ReadLine();
                            if (str.Contains("V;"))
                            {
                                lReadFile.Add(str);
                            }
                        }
                        sr.Close();
                    }

                    foreach (string _str in lReadFile)
                    {
                        string[] arrStr = _str.Split(';');
                        if (arrStr != null && arrStr.Length > 2)
                        {
                            int _tent = Convert.ToInt32(arrStr[2]);
                            for (int i = 0; i < _tent; i++)
                            {
                                Negocio.Precisao _p = new Negocio.Precisao();
                                _p.volume = double.Parse(arrStr[1].Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                                _p.tentativas = i + 1;
                                _p.volumeDos = 0;
                                _p.volumeDos_str = "0";
                                _p.executado = 0;
                                this.acurracy.listPrecisao.Add(_p);
                            }
                        }
                    }

                    atualizaDGView(this.acurracy);
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void atualizaDetailColorante()
        {
            try
            {
                txt_CircuitExecuting.Text = "Circuito " + String.Format("{0:00}", this.colorante.Circuito);
                txtMassaEspecifica.Text = this.colorante.MassaEspecifica.ToString();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        #region BackWork Datahora e progressbar
        private void iniciarbgWork()
        {
            try
            {
                if (bgWork == null)
                {
                    this.bgWork = new BackgroundWorker();
                    this.bgWork.DoWork += new System.ComponentModel.DoWorkEventHandler(bgWork_DoWork);
                    this.bgWork.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(bgWork_RunWorkerCompleted);
                    this.bgWork.WorkerSupportsCancellation = true;
                    this.isThreadWork = true;
                    this.isRunningWork = false;
                  
                    this.bgWork.RunWorkerAsync();
                }
                else
                {

                    this.bgWork = new BackgroundWorker();
                    this.bgWork.DoWork += new System.ComponentModel.DoWorkEventHandler(bgWork_DoWork);
                    this.bgWork.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(bgWork_RunWorkerCompleted);
                    this.bgWork.WorkerSupportsCancellation = true;
                    this.isThreadWork = true;
                    this.isRunningWork = false;
                    bgWork.RunWorkerAsync();

                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void bgWork_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                BackgroundWorker worker = sender as BackgroundWorker;
                while (this.isThreadWork)
                {
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        this.isThreadWork = false;
                    }
                    else
                    {
                        try
                        {
                            if (!this.isRunningWork)
                            {
                                this.isRunningWork = true;
                                this.Invoke(new MethodInvoker(MonitoramentoWork));
                                Thread.Sleep(800);
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
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void bgWork_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    this.isThreadWork = false;
                }
                else if (!(e.Error == null))
                {
                    this.isThreadWork = false;
                }
                else
                {
                    this.isThreadWork = true;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void MonitoramentoWork()
        {
            try
            {
                txtDataHora.Text = string.Format("{0:HH:mm:ss}", DateTime.Now);
                if (this.isThread)
                {
                    Invalidate();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			this.isRunningWork = false;
        }
        #endregion

        #region Eventos teclado

        private void txtVolRecipiente_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                char chrPoint = '.';
                char currentKey = e.KeyChar;
                char chrBackSpace = (char)Keys.Back;
                char chrEnterReturn = (char)Keys.Enter;
                if (currentKey == chrPoint)
                {
                    if (txtVolumeMaxRecipiente.Text.Contains("."))
                    {
                        string[] arrStr = txtVolumeMaxRecipiente.Text.Split('.');
                        if (arrStr.Length > 1)
                        {
                            e.Handled = true;
                        }
                        else
                        {
                            e.Handled = false;
                        }
                    }
                    else
                    {
                        e.Handled = false;
                    }
                }
                else if (Char.IsNumber(currentKey) || currentKey == chrBackSpace || currentKey == chrEnterReturn)
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }

                if (e.Handled) return;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void txtDelayBalanca_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                char currentKey = e.KeyChar;
                char chrBackSpace = (char)Keys.Back;
                char chrEnterReturn = (char)Keys.Enter;
                if (Char.IsNumber(currentKey) || currentKey == chrBackSpace || currentKey == chrEnterReturn)
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }

                if (e.Handled) return;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void txtVolumeMaxRecipiente_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                char currentKey = e.KeyChar;
                char chrBackSpace = (char)Keys.Back;
                char chrEnterReturn = (char)Keys.Enter;
                if (Char.IsNumber(currentKey) || currentKey == chrBackSpace || currentKey == chrEnterReturn)
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }

                if (e.Handled) return;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void txtCapacidadeMaxBalanca_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                char currentKey = e.KeyChar;
                char chrBackSpace = (char)Keys.Back;
                char chrEnterReturn = (char)Keys.Enter;
                if (Char.IsNumber(currentKey) || currentKey == chrBackSpace || currentKey == chrEnterReturn)
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }

                if (e.Handled) return;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void txtMinMassaAdmRecipiente_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                char currentKey = e.KeyChar;
                char chrBackSpace = (char)Keys.Back;
                char chrEnterReturn = (char)Keys.Enter;
                if (Char.IsNumber(currentKey) || currentKey == chrBackSpace || currentKey == chrEnterReturn)
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }

                if (e.Handled) return;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        #endregion

        #region Eventos Paint Progress
        private void fPrecisaoMenu_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                panel1.Invalidate();
                panel2.Invalidate();
                panel3.Invalidate();
                panel4.Invalidate();
                panel5.Invalidate();
                panel6.Invalidate();
                panel7.Invalidate();
                panel8.Invalidate();

                inicializacaoPG++;
                statusStrip1.Refresh();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			if (inicializacaoPG >= 8)
            {
                inicializacaoPG = 0;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (inicializacaoPG == 0)
            {
                e.Graphics.FillEllipse(_Brush, 0, 0, panel1.Width, panel1.Height);

            }
            else
            {
                e.Graphics.FillEllipse(Brushes.Black, 0, 0, panel1.Width, panel1.Height);

            }

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            if (inicializacaoPG == 1)
            {
                e.Graphics.FillEllipse(_Brush, 0, 0, panel2.Width, panel2.Height);

            }
            else
            {
                e.Graphics.FillEllipse(Brushes.Black, 0, 0, panel2.Width, panel2.Height);

            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            if (inicializacaoPG == 2)
            {
                e.Graphics.FillEllipse(_Brush, 0, 0, panel3.Width, panel3.Height);

            }
            else
            {
                e.Graphics.FillEllipse(Brushes.Black, 0, 0, panel3.Width, panel3.Height);

            }

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            if (inicializacaoPG == 3)
            {
                e.Graphics.FillEllipse(_Brush, 0, 0, panel4.Width, panel4.Height);

            }
            else
            {
                e.Graphics.FillEllipse(Brushes.Black, 0, 0, panel4.Width, panel4.Height);

            }
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {
            if (inicializacaoPG == 4)
            {
                e.Graphics.FillEllipse(_Brush, 0, 0, panel5.Width, panel5.Height);

            }
            else
            {
                e.Graphics.FillEllipse(Brushes.Black, 0, 0, panel5.Width, panel5.Height);

            }
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {
            if (inicializacaoPG == 5)
            {
                e.Graphics.FillEllipse(_Brush, 0, 0, panel6.Width, panel6.Height);

            }
            else
            {
                e.Graphics.FillEllipse(Brushes.Black, 0, 0, panel6.Width, panel6.Height);

            }
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {
            if (inicializacaoPG == 6)
            {
                e.Graphics.FillEllipse(_Brush, 0, 0, panel7.Width, panel7.Height);

            }
            else
            {
                e.Graphics.FillEllipse(Brushes.Black, 0, 0, panel7.Width, panel7.Height);

            }
        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {
            if (inicializacaoPG == 7)
            {
                e.Graphics.FillEllipse(_Brush, 0, 0, panel8.Width, panel8.Height);

            }
            else
            {
                e.Graphics.FillEllipse(Brushes.Black, 0, 0, panel8.Width, panel8.Height);

            }
        }
        #endregion


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEditarDiretorio_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog opFd = new OpenFileDialog();
                opFd.Multiselect = false;
                opFd.Filter = "Text|*.txt";
                DialogResult dr = opFd.ShowDialog();
                if (dr == DialogResult.OK && opFd.FileName != null && opFd.FileName.Length > 0 && File.Exists(opFd.FileName))
                {
                    if (this.acurracy.listPrecisao == null)
                    {
                        this.acurracy.listPrecisao = new List<Negocio.Precisao>();
                    }
                   
                    this.acurracy.listPrecisao.Clear();
                    txt_PathFileImport.Text = opFd.FileName;
                    List<String> lReadFile = new List<string>();
                    using (StreamReader sr = new StreamReader(opFd.FileName))
                    {
                        while (sr.Peek() >= 0)
                        {
                            string str = sr.ReadLine();
                            if (str.Contains("V;"))
                            {
                                lReadFile.Add(str);
                            }
                        }
                        sr.Close();
                    }

                    foreach (string _str in lReadFile)
                    {
                        string[] arrStr = _str.Split(';');
                        if (arrStr != null && arrStr.Length > 2)
                        {
                            int _tent = Convert.ToInt32(arrStr[2]);
                            for (int i = 0; i < _tent; i++)
                            {
                                Negocio.Precisao _p = new Negocio.Precisao();
                                _p.volume = double.Parse(arrStr[1].Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                                _p.tentativas = i + 1;
                                _p.volumeDos = 0;
                                _p.volumeDos_str = "0";                                   
                                _p.executado = 0;
                                this.acurracy.listPrecisao.Add(_p);
                            }
                        }
                    }
                }
                else
                {
                    this.acurracy.listPrecisao.Clear();
                }

                atualizaDGView(this.acurracy);
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

                this.dtPrecisaoAuto = new DataTable();

                this.dtPrecisaoAuto.Columns.Add("Volume", typeof(string));
                this.dtPrecisaoAuto.Columns.Add("Tentativas", typeof(string));
                this.dtPrecisaoAuto.Columns.Add("VolumeDos", typeof(string));
                this.dtPrecisaoAuto.Columns.Add("Executado", typeof(string));

                if (_accurracy != null)
                {
                    foreach (Negocio.Precisao _op in _accurracy.listPrecisao)
                    {
                        DataRow dr = this.dtPrecisaoAuto.NewRow();
                        dr["Volume"] = _op.volume.ToString();
                        dr["Tentativas"] = _op.tentativas.ToString();
                        dr["VolumeDos"] = _op.volumeDos_str;

                        if (_op.executado == 0)
                        {
                            dr["Executado"] = Negocio.IdiomaResxExtensao.Global_Nao;
                        }
                        else
                        {
                            dr["Executado"] = Negocio.IdiomaResxExtensao.Global_Sim ;
                        }
                        this.dtPrecisaoAuto.Rows.Add(dr);
                    }
                }
                if (dgvCalibracaoAuto.Rows != null)
                {
                    dgvCalibracaoAuto.Rows.Clear();
                }
                for (int i = 0; i < this.dtPrecisaoAuto.Rows.Count; i++)
                {
                    dgvCalibracaoAuto.Rows.Add(this.dtPrecisaoAuto.Rows[i][0], this.dtPrecisaoAuto.Rows[i][1], this.dtPrecisaoAuto.Rows[i][2], this.dtPrecisaoAuto.Rows[i][3]);
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
                //Green
                //Color.FromArgb(255, 0, 128, 0);
                //Red
                //Color.FromArgb(255, 228, 81, 76);
                //Yellow
                //Color.FromArgb(255, 225, 255, 0);

                DataGridViewRow row = dgvCalibracaoAuto.Rows[e.RowIndex];
                if (row.Cells["Executado"].Value.ToString() == Negocio.IdiomaResxExtensao.Global_Nao)
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

        private void cmbTipoBalanca_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.acurracy.ModeloBalanca = Convert.ToInt32(cmbTipoBalanca.SelectedValue.ToString());
                if (this.interfaceBal != null)
                {
                    this.interfaceBal.CloseSerial();
                }
                if (this.acurracy.ModeloBalanca == 1)
                {
                    this.interfaceBal = new Negocio.Balanca_BEL();                   
                }
                else if (this.acurracy.ModeloBalanca == 2)
                {
                    this.interfaceBal = new Negocio.Balanca_ToledoMS403S();
                }
                else/* if (this.acurracy.ModeloBalanca == 3)*/
                {
                    this.interfaceBal = new Negocio.Balanca_ToledoPG503S();
                }
                txtCapacidadeMaxBalanca.Text = this.interfaceBal.CargaMaximaBalanca_Gramas.ToString(); ;

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void enableStatus(bool _status)
        {
            try
            {
                this.pnlStatus.Visible = _status;
                this.pnlStatus2.Visible = _status;
                this.statusStrip1.Visible = _status;
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
                lblStatusMessage.Text = this.mensagemStrip;
                int valor_toolP = 5;
                if (this.posInicio >= 0)
                {
                    valor_toolP += (((this.posInicio + 1)  * 100) / this.acurracy.listPrecisao.Count);
                }
                
                if(valor_toolP > 100)
                {
                    if (toolProgress.Value < valor_toolP)
                    {
                        valor_toolP = 99;
                    }
                    else
                    {
                        valor_toolP = toolProgress.Value;
                    }
                }
                toolProgress.Value = valor_toolP;               
                statusStrip1.Refresh();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.stopExecution = false;
            if(this.colorante != null && this.acurracy != null && this.acurracy.listPrecisao != null && this.acurracy.listPrecisao.Count > 0)
            {
                if (!this.ModoDebug)
                {
                    this.interfaceBal._str_Serial = cmbPortaSerial.Text;
                    this.interfaceBal.Start_Serial();
                }
                this.acurracy.MinMassaAdmRecipiente = double.Parse( txtMinMassaAdmRecipiente.Text.Replace(",", ".") , System.Globalization.CultureInfo.InvariantCulture);
                this.acurracy.VolumeRecipiente = double.Parse(txtVolumeMaxRecipiente.Text.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                for(int i = 0; i < this.acurracy.listPrecisao.Count; i++)
                {
                    this.acurracy.listPrecisao[i].executado = 0;
                    this.acurracy.listPrecisao[i].volumeDos = 0;
                    this.acurracy.listPrecisao[i].volumeDos_str = "0";
                }
                toolProgress.Value = 5;

                btnStart.Enabled = false;
                btnEditarDiretorio.Enabled = false;
                startF();
            }
            else
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    string msg = string.Format(Negocio.IdiomaResxExtensao.CalibracaoAutomate_SemArquivoOuCircuito);
                    m.ShowDialog(msg);
                }

            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.stopExecution = true;
        }

        private void startF()
        {
            try
            {
                this.mensagemStrip = Negocio.IdiomaResxExtensao.CalibracaoAuto_Pos_Recipiente;
                enableStatus(true);                
                AttStripMsg();

                try
                {
                    this.DelayEntreBalanca = Convert.ToInt32(txtDelayBalanca.Text) * 1000;
                }
				catch (Exception ex)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
				}

				if (this.DelayEntreBalanca < 5000)
                {
                    this.DelayEntreBalanca = 5000;
                }
                this.acurracy.DelaySegBalanca = this.DelayEntreBalanca;
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
                    try
                    {
                        _disp.Disconnect();
                    }
					catch (Exception ex)
					{
						LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
					}
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
                this.isPosicaoRecipiente = false;
                
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
                            if (this.stopExecution)
                            {
                                this.isThread = false;
                            }
                            else if (!this.isRunning)
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

                if (!this.ClosingWindows)
                {
                    if (this.stopExecution)
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAutomate_Cancelado);
                        }
                    }
                    else
                    {
                        if (!this.execCal)
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                            {
                                m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAutomate_Error);
                            }                            
                        }
                        else if (this.execCal && this.posInicio >= this.acurracy.listPrecisao.Count)
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAutomate_OK);
                            }
                            this.acurracy.MessageRetorno = "OK";
                            this.Invoke(new MethodInvoker(ExportFile));
                        }
                    }
                    this.Invoke(new MethodInvoker(disabeStatus));
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void ExportFile()
        {
            try
            {
                string pathFile = Path.GetDirectoryName(txt_PathFileImport.Text) + @"\" + this.acurracy.NumeroSerie + "_" + this.colorante.Nome + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                using (StreamWriter sw = new StreamWriter(pathFile))
                {
                    foreach (Negocio.Precisao _p in this.acurracy.listPrecisao)
                    {                           
                        string WL = _p.tentativas + ";" + string.Format("{0:N5}", _p.volume).Replace(".", ",") + ";" + _p.volumeDos_str.Replace(".", ",");
                        sw.WriteLine(WL);                           
                    }
                    sw.Close();
                }
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    string msg = string.Format(Negocio.IdiomaResxExtensao.CalibracaoAutomate_ExportArquivo, pathFile);
                    m.ShowDialog(msg);
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void disabeStatus()
        {
            try
            {
                btnStart.Enabled = true;
                btnEditarDiretorio.Enabled = true;
                enableStatus(false);
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
                    if (!this.isPosicaoRecipiente)
                    {
                        this.isPosicaoRecipiente = PosicionarRecipiente();
                        this.isRunning = false;
                    }
                    else
                    {
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
                if ((massaB > this.interfaceBal.CargaMaximaBalanca_Gramas) || (volumeB > this.acurracy.VolumeRecipiente))
                {
                    retorno = true;                    
                }
                else
                {
                    this.istrocarRecipiente = false;
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
                if (!this.istrocarRecipiente)
                {
                    this.istrocarRecipiente = true;
                    this.stepPosicaoRec = 0;
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        string msg = string.Format(Negocio.IdiomaResxExtensao.CalibracaoAuto_MaxBalancaOuVolumeRecipiente, this.colorante.Nome);
                        m.ShowDialog(msg);
                    }
                    ClosedSerialDispensa();
                    if (this.ModoDebug)
                    {
                        this.MassaBalancaOnLine = 0;
                        this.MassaBalancaRecipiente = 0;
                        this.MassaBalancaOnLineInc = 0;
                    }
                }
                this.stepPosicaoRec = 0;
                this.isPosicaoRecipiente = false;
                this.isRunning = false;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void FaixaPrecisao()
        {
            try
            {
                if (this.IsDispensou)
                {
                   
                    if(this.stepFaixaPrecisao == 0)
                    {
                        this.stepFaixaPrecisao++;
                        this.dtInioLeituraBalanca = DateTime.Now;
                        this.mensagemStrip = Negocio.IdiomaResxExtensao.CalibracaoAuto_Lendo_Balanca;
                        AttStripMsg();
                    }
                    //Thread.Sleep(this.DelayEntreBalanca);
                    //int count_Delay = this.DelayEntreBalanca / 1000;
                    //for (int i = 0; i < count_Delay; i++)
                    //{
                    //    Thread.Sleep(1000);
                    //}
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

                        if (this.ModoDebug || this.interfaceBal.isTerminouRead)
                        {
                            if (!this.ModoDebug)
                            {
                                this.interfaceBal.SetValues(response, false, false);
                            }
                            else
                            {
                                this.interfaceBal.valorPeso = massaAterior + (this.acurracy.listPrecisao[this.posInicio].volume * this.colorante.MassaEspecifica);
                            }

                            this.MassaBalancaOnLine = this.interfaceBal.valorPeso;
                            massaDosada = this.MassaBalancaOnLine - massaAterior;

                            this.acurracy.listPrecisao[this.posInicio].volumeDos = massaDosada / this.colorante.MassaEspecifica;
                            this.acurracy.listPrecisao[this.posInicio].volumeDos_str = string.Format("{0:N5}", this.acurracy.listPrecisao[this.posInicio].volumeDos);

                        }
                        else
                        {
                            this.execCal = false;
                            this.acurracy.MessageRetorno = Negocio.IdiomaResxExtensao.CalibracaoAuto_ErrorReadBalanca;
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                            {
                                m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAuto_ErrorReadBalanca);
                            }
                            this.isRunning = false;
                            return;
                        }

                        this.posInicio++;
                        this.IsDispensou = false;
                        this.isRunning = false;
                        this.MassaBalancaOnLineInc += massaDosada;
                        this.Invoke(new MethodInvoker(AttDGVBack));
                       
                    }
                    this.isRunning = false;

                }
                else
                {
                    this.acurracy.listPrecisao[this.posInicio].executado = 1;
                    this.mensagemStrip = string.Format(Negocio.IdiomaResxExtensao.CalibracaoAuto_DispensandoVolume, this.acurracy.listPrecisao[this.posInicio].volume.ToString());
                    this.Invoke(new MethodInvoker(AttStripMsg));
                    ExecutarMonitoramento2();
                    this.stepFaixaPrecisao = 0;

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
                if (this.stepPosicaoRec == 0)
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        string msg = string.Format(Negocio.IdiomaResxExtensao.CalibracaoAuto_PosicaoRecipiente, this.colorante.Nome);
                        m.ShowDialog(msg);
                    }
                    this.stepPosicaoRec++;
                }
                else if (this.stepPosicaoRec == 1)
                {
                    IDispenser _disp = this.ldisp[0];
                    bool conectar = Operar.Conectar(ref _disp, false);
                    if (conectar && Operar.TemRecipiente(_disp, false))
                    {
                        this.dtInioLeituraBalanca = DateTime.Now;
                        this.stepPosicaoRec++;
                    }
                    else
                    {
                        this.stepPosicaoRec = 0;
                        ClosedSerialDispensa();
                        PosicionarRecipiente();
                    }
                }
                else if (this.stepPosicaoRec == 2)
                {
                    this.mensagemStrip = Negocio.IdiomaResxExtensao.CalibracaoAuto_Lendo_Balanca;
                    AttStripMsg();
                    //Thread.Sleep(this.DelayEntreBalanca);
                    //int count_Delay = this.DelayEntreBalanca / 1000;
                    //for (int i = 0; i < count_Delay; i++)
                    //{
                    //    Thread.Sleep(1000);

                    //}
                    if (DateTime.Now.Subtract(this.dtInioLeituraBalanca).TotalMilliseconds > this.DelayEntreBalanca)
                    {
                        this.dtInioLeituraBalanca = DateTime.Now;
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
                            this.MassaBalancaRecipiente = this.interfaceBal.valorPeso;
                            this.MassaBalancaOnLine = this.interfaceBal.valorPeso;
                            this.MassaBalancaOnLineInc = this.MassaBalancaOnLine;
                            if (this.MassaBalancaOnLine < this.acurracy.MinMassaAdmRecipiente)
                            {
                                retorno = true;
                                this.Invoke(new MethodInvoker(AttDGVBack));
                            }
                            else
                            {                               
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                                {
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAuto_PesoRecipienteExcedido);
                                }
                                this.stepPosicaoRec = 0;
                                retorno = false;
                            }

                        }
                        else
                        {
                            this.execCal = false;
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                            {
                                m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAuto_ErrorReadBalanca);
                                this.acurracy.MessageRetorno = Negocio.IdiomaResxExtensao.CalibracaoAuto_ErrorReadBalanca;
                            }
                            return false;
                        }
                    }
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
        #endregion

        #region Dosagem Placa

        void Monitoramento_Event2()
        {
            try
            {
                /* Verifica se dispenser está liberado.
                 * isso representa que o processo foi concluído. */

                int motor = this.calibracao.Motor - 1;

                List<Util.ObjectColorante> ncol = this.lColSeguidor.FindAll(o => o.Seguidor == this.calibracao.Motor).ToList();

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
                                         _valores.PulsoReverso, i_Step)
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
                                        _valores.PulsoReverso, i_Step)
                                    );
                    }
                }
                else if (this.colorante.Dispositivo == 2)
                {
                    int indexV2 = 16;
                    if(/*(Dispositivo)_parametro.IdDispositivo == Dispositivo.Placa_4 || */(Dispositivo)_parametro.IdDispositivo == Dispositivo.Simulador)
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
                            Util.ObjectCalibragem.UpdatePulsosRev(this.calibracao.Motor, _valores.PulsoReverso);
                            lastPulsoHorario = lastPulsoHorario - _valores.PulsoReverso;
                        }
                        //Util.ObjectCalibragem.UpdatePulsosRev(_motor + 1, _valores.PulsoReverso);
                        return;
                    }
                    _valores = Operar.Parser(this.acurracy.listPrecisao[this.posInicio].volume, this.calibracao.Valores, this.calibracao.UltimoPulsoReverso);
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
                                      _valores.PulsoReverso, i_Step)
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
                                        _valores.PulsoReverso, i_Step)
                                    );
                    }
                }

                this.IsDispensou = true;
                //Thread.Sleep(1000);
                this.isRunning2 = false;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                if (!this.IsDispensou)
                {
					string customMessage = ErrorMessageHandler.GetFriendlyErrorMessage(ex);
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
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			this.isThread2 = false;
            
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