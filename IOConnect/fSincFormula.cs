using Percolore.Core.Logging;
using Percolore.Core.Persistence.WindowsRegistry;
using Percolore.IOConnect.Negocio;
using System.ComponentModel;
using System.Net.Sockets;

namespace Percolore.IOConnect
{
	public partial class fSincFormula : Form
    {
        TcpClient tcpcliente = new TcpClient();
        Util.ObjectParametros _parametros = null;

        string CLICK_UPLOAD_FORMULA =
          Path.Combine(Environment.CurrentDirectory, "UpFormula.json");
        string CLICK_DOWN_FORMULA =
         Path.Combine(Environment.CurrentDirectory, "DownFormula.json");

        System.ComponentModel.BackgroundWorker backgroundWorker1 = null;
        private bool isThread = false;
        private bool isRunning = false;


        private int _tipo = 0;
        public fSincFormula(int tipo)
        {
            InitializeComponent();
            _tipo = tipo;
            _parametros = Util.ObjectParametros.Load();
            progressBar.Visible = false;
            btnCancelar.Text = Negocio.IdiomaResxExtensao.Global_Cancelar;
            #region identify click
            if (this._parametros.PathMonitoramentoDAT.Contains("DosadoraPercolore_zhm69scv2n72e"))
            {
                string[] arrayStr = this._parametros.PathMonitoramentoDAT.Split(Path.DirectorySeparatorChar);
                string strPath = "";
                for (int i = 0; i < arrayStr.Length - 1; i++)
                {
                    strPath += arrayStr[i] + Path.DirectorySeparatorChar;
                }
                //CLICK_PURGA_INDIVIDUAL = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Local\\Packages\\DosadoraPercolore_zhm69scv2n72e\\LocalState\\click.purgarindividual");
                CLICK_UPLOAD_FORMULA = strPath + "UpFormula.json";
                CLICK_DOWN_FORMULA = strPath + "DownFormula.json";
               
            }
            #endregion
        }

        private void fSincFormula_Load(object sender, EventArgs e)
        {
            try
            {
                progressBar.Visible = true;
                if (_tipo == 1)
                {
                    //lblStatus.Text = "Realizando Sincronismo de Fórmulas com o sistema LAB!";
                    lblStatus.Text = Negocio.IdiomaResxExtensao.Configuracoes_SincFormula;
                }
                else if (_tipo == 2)
                {
                    //lblStatus.Text = "Realizando Upload de Fórmula!";
                    lblStatus.Text = Negocio.IdiomaResxExtensao.Configuracoes_UpFormula;
                }
                ExecutarMonitoramento();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void fSincFormula_FormClosed(object sender, FormClosedEventArgs e)
        {
            PausarMonitoramento();
        }

        private bool isWriteTCP(byte[] pacoteMsg, string simCard)
        {
            bool retorno = false;
            try
            {
                this.tcpcliente = new TcpClient();
                this.tcpcliente.Connect(_parametros.IpSincFormula, Convert.ToInt32(_parametros.PortaSincFormula));
                NetworkStream netWriteread = this.tcpcliente.GetStream();
                netWriteread.Write(pacoteMsg, 0, pacoteMsg.Length);
                Thread.Sleep(2000);

                if (this.tcpcliente.Connected)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (this.tcpcliente.Available <= 0)
                        {
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                    if (this.tcpcliente.Available > 0)
                    {
                        int tamanho = 0;
                        int countInt = 0;
                        Byte[] PacRec = null;
                        bool achouFim = false;
                        for (int z = 0; z < 10; z++)
                        {
                            tamanho = this.tcpcliente.Available;
                            Byte[] bytes = new Byte[tamanho];
                            int i = netWriteread.Read(bytes, 0, tamanho);
                            countInt += i;
                            PacRec = ConcatenaBytes(PacRec, bytes);
                            if (i > 0 && bytes != null && PacRec.Length > 0 && (PacRec[countInt - 1] == 255 && PacRec[0] == 0xA5))
                            {
                                achouFim = true;
                                break;
                            }
                            if(i == 0)
                            {
                                break;
                            }
                            Thread.Sleep(2000);
                        }
                        if (achouFim)
                        {
                            ReadMessageTcp rdTcp = new ReadMessageTcp();
                            if (rdTcp.desmontarMessage(PacRec, 1))
                            {
                                if(rdTcp.TipoEvento == 1)
                                {
                                    string jsp = rdTcp.message;
                                    using (StreamWriter sW = new StreamWriter(CLICK_DOWN_FORMULA))
                                    {
                                        sW.WriteLine(jsp);
                                        sW.Close();
                                    }
                                    retorno = true;
                                }
                            }
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
        
        private byte[] montaPacoteProducao(string codSimcard, string message, int tipo)
        {
            byte[] retorno = new byte[27 + message.Length];
            int tamP = 27 + message.Length;
            retorno[0] = 0xA5;
            //retorno[1] = 0x00;
            //retorno[2] = (byte)(26 + message.Length);

            retorno[1] = (byte)(tamP >> 8);
            retorno[2] = (byte)(tamP & 0xFF);
            retorno[3] = 0x00;
            int fim = codSimcard.Length;
            if (fim > 20)
            {
                fim = 20;
            }
            for (int i = 0; i < fim; i++)
            {
                retorno[4 + i] = (byte)codSimcard[i];
            }
            retorno[24] = 0x01;
            fim = message.Length;
            retorno[25] = (byte)tipo;
            for (int i = 0; i < fim; i++)
            {
                retorno[26 + i] = (byte)message[i];
            }

            
            retorno[26 + fim] = 0xFF;
            int n_crc = 0;
            for (int i = 4; i < retorno.Length; i++)
            {
                char b = (char)retorno[i];
                int intVal = ((int)b) & 0xff;
                n_crc += intVal;
                if (n_crc > 255)
                {
                    n_crc -= 255;
                }
            }
            retorno[3] = (byte)n_crc;
            return retorno;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
        
        public static byte[] ConcatenaBytes(byte[] pac1, byte[] pac2)
        {
            byte[] retorno = null;
            
            try
            {
                int tamanho_pac = 0;
                if (pac1 != null && pac1.Length > 0)
                {
                    tamanho_pac += pac1.Length;
                }
                if (pac2 != null && pac2.Length > 0)
                {
                    tamanho_pac += pac2.Length;
                }
                int inc = 0;
                if (tamanho_pac > 0)
                {
                    retorno = new byte[tamanho_pac];
                    for (int i = 0; pac1 != null && i < pac1.Length; i++)
                    {
                        retorno[inc++] = pac1[i];
                    }
                    for (int i = 0; pac2 != null && i < pac2.Length; i++)
                    {
                        retorno[inc++] = pac2[i];
                    }

                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {typeof(fSincFormula).Name}: ", ex);
			    retorno = null;
            }

            return retorno;
        }

        void Monitoramento_Event()
        {
            try
            {
                if (_tipo == 1)
                {
                    //lblStatus.Text = "Realizando Sincronismo de Fórmulas com o sistema LAB!";

                    string imei = "";
                    //Popula campos
                    using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                    {
                        imei = percRegistry.GetSerialNumber();
                    }
                    string msgC = string.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now) + "|";
                    byte[] ptW = montaPacoteProducao(imei, msgC, _tipo);
                    if (isWriteTCP(ptW, imei))
                    {
                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, "Transmitiu Sincronismo Formula :" + msgC);
                    }

                }
                else if (_tipo == 2)
                {
                    //lblStatus.Text = "Realizando Upload de Fórmula!";

                    string imei = "";
                    //Popula campos
                    using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                    {
                        imei = percRegistry.GetSerialNumber();
                    }
                    string msgC = string.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now) + "|";

                    using (StreamReader sr = new StreamReader(CLICK_UPLOAD_FORMULA))
                    {
                        msgC += sr.ReadToEnd();
                        sr.Close();
                    }


                    byte[] ptW = montaPacoteProducao(imei, msgC, _tipo);
                    if (isWriteTCP(ptW, imei))
                    {
                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, "Transmitiu Sincronismo Formula :" + msgC);
                    }
                    File.Delete(CLICK_UPLOAD_FORMULA);
                }

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			this.DialogResult = DialogResult.OK;
            this.Close();
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
                            Thread.Sleep(2500);
                            this.isRunning = true;
                            this.Invoke(new MethodInvoker(Monitoramento_Event));
                            Thread.Sleep(1000);
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
    }
}