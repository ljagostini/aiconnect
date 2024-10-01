using Percolore.Core.Logging;
using Percolore.Core.Persistence.WindowsRegistry;
using Percolore.Core.Security.License;
using Percolore.IOConnect.Util;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Percolore.IOConnect
{
	public partial class fLicenca : Form
    {
        private ALicenseKey _chave;
        private string _licença = "";
        private Util.ObjectParametros _parametros = null;

        fAguarde _fAguarde = null;
        TcpClient tcpcliente = new TcpClient();
        string _serial;

        public string Licença
        {
            get { return _licença; }
        }

        public fLicenca(ALicenseKey lk)
        {
            InitializeComponent();

            //Tamanho e posicionamento
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Location = new Point(0, 30);

            _parametros = Util.ObjectParametros.Load();

            //Globalização
            this.lblTitulo.Text = Negocio.IdiomaResxExtensao.Licenca_lblTitulo;
            this.lblChave.Text = Negocio.IdiomaResxExtensao.Licenca_lblChave;
            this.lblLicenca.Text = Negocio.IdiomaResxExtensao.Licenca_lblLicenca;

            btnCancelar.Text = string.Empty;
            btnCancelar.Image = Imagem.GetCancelar_48x48();
            btnAtivar.Text = string.Empty;
            btnAtivar.Image = Imagem.GetConfirmar_48x48();          

            _licença = "";
            _chave = lk;
            txtSerial.Text = _chave.KeyCode;
            //if (!_parametros.DesabilitaMonitSyncToken)
            //{
            //    btnOnLine.Enabled = true;
            //    btnOnLine.Visible = true;
            //}

            using (PercoloreRegistry percRegistry = new PercoloreRegistry())
            {
                _serial = percRegistry.GetSerialNumber();
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
            txtChave.isTecladoShow = ischek;
            txtSerial.isTecladoShow = ischek;

            if (_parametros != null)
            {
                ischek = _parametros.HabilitarTouchScrenn;
            }
            txtChave.isTouchScrenn = ischek;
            txtSerial.isTouchScrenn = ischek;
        }

        void btnAtivar_Click(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(btnAtivarClick));
        }

        private void btnAtivarClick()
        {
            try
            {
                if (!_chave.CheckLicense(txtChave.Text))
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(Negocio.IdiomaResxExtensao.Licenca_Informacao_ChaveIncorreta);
                    }

                    return;
                }

                _licença = txtChave.Text;
                this.DialogResult = DialogResult.OK;
                // KeyboardHelper.Kill();
                this.Close();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            //KeyboardHelper.Kill();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnTeclado_Click(object sender, EventArgs e)
        {
            string pathTeclado = Path.Combine(@"C:\Program Files\Common Files\Microsoft Shared\ink", @"TabTip.exe");

            //Valida se teclado existe
            if (!File.Exists(pathTeclado))
            {
                return;
            }

            //Executa processo
            Process.Start(pathTeclado);
            txtChave.Focus();
        }

        private void btnOnLine_Click(object sender, EventArgs e)
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

                }
                else
                {
                    this._fAguarde._TimerDelay = 330;
                    this._fAguarde.Focus();
                }
                
                Thread thrd = new Thread(new ThreadStart(RequestOnLineValidadeManutencao));
                thrd.Start();
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
                this._fAguarde.PausarMonitoramento();
                this._fAguarde.Close();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
        private void RequestOnLineValidadeManutencao()
        {
            try
            {
                while (!this._fAguarde.IsRunning)
                {
                    Thread.Sleep(1000);
                }
                this.Invoke(new MethodInvoker(GetOnLineValidadeManutencao));
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void GetOnLineValidadeManutencao()
        {
            try
            {   
                string imei = _serial;
                string msgC = txtSerial.Text + "|" + _serial;
                byte[] ptW = montaPacoteValManutencao(0x32, imei, msgC);
                string retornoMsg = "";
                if (isWriteTCP(ptW, imei, ref retornoMsg))
                {
                    //txtChave.Text = retornoMsg;
                    string[] _array = retornoMsg.Split(';');
                    if (_array[0] == "0")
                    {
                        txtChave.Text = _array[1];
                    }
                    else
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            string msg = _array[1];
                            if (msg == "Cod 1")
                            {
                                msg = Negocio.IdiomaResxExtensao.ValidadeManutencao_OnLine_Cod1;
                            }
                            else if (msg == "Cod 2")
                            {
                                msg = Negocio.IdiomaResxExtensao.ValidadeManutencao_OnLine_Cod2;
                            }
                            else if (msg == "Cod 3")
                            {
                                msg = Negocio.IdiomaResxExtensao.ValidadeManutencao_OnLine_Cod3;
                            }
                            else if (msg == "Cod 4")
                            {
                                msg = Negocio.IdiomaResxExtensao.ValidadeManutencao_OnLine_Cod4;
                            }
                            m.ShowDialog(msg);
                        }
                    }
                }
                else
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        string msg = Negocio.IdiomaResxExtensao.ValidadeManutencao_OnLine_Fail_Connect;
                        m.ShowDialog(msg);
                    }
                }
            
                if (this.tcpcliente.Connected)
                {
                    this.tcpcliente.Close();
                    this.tcpcliente.Dispose();
                }
            
                this.Invoke(new MethodInvoker(ClosePrg));
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        #region Metodos Online

        private bool isWriteTCP(byte[] pacoteMsg, string simCard, ref string _MsgRet)
        {
            bool retorno = false;
            _MsgRet = "";
            
            try
            {
                if (TestInternet())
                {
                    this.tcpcliente = new TcpClient();

                    this.tcpcliente.Connect(_parametros.IpSincToken, Convert.ToInt32(_parametros.PortaSincToken));
                    NetworkStream netWriteread = this.tcpcliente.GetStream();
                    netWriteread.Write(pacoteMsg, 0, pacoteMsg.Length);
                    Thread.Sleep(1000);

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
                            tamanho = this.tcpcliente.Available;
                            Byte[] bytes = new Byte[tamanho];
                            int i = netWriteread.Read(bytes, 0, tamanho);

                            StringBuilder sb = new StringBuilder();
                            for (int k = 0; k < i; k++)
                            {
                                byte b = bytes[k];
                                sb.Append(b.ToString("X2"));
                                if ((k - 1) < i)
                                {
                                    sb.Append("-");
                                }
                            }
                            string _token = "";
                            if (DesmontarPacoteTCP(sb.ToString(), simCard, ref _token))
                            {
                                _MsgRet = _token;
                                retorno = true;
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

        private bool TestInternet()
        {
            bool retorno = false;
            
            try
            {
                Ping myPing = new Ping();
                //String host = "google.com";
                String host = _parametros.IpSincToken;
                byte[] buffer = new byte[32];
                int timeout = 5000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                retorno = (reply.Status == IPStatus.Success);
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			return retorno;
        }

        public bool DesmontarPacoteTCP(string msgHex, string codSim, ref string retornoLicense)
        {
            bool retorno = false;
            if (msgHex != null && msgHex.Length > 0)
            {
                string[] arrMsg = msgHex.Split('-');
                //Verificar se possui o header
                if (arrMsg.Length > 25)
                {
                    string header = string.Empty;
                    int numeroBytes = 0;
                    int crc = 0;
                    string simCardTcp = string.Empty;
                    int id_evt = 0;
                    header = arrMsg[0];

                    numeroBytes = (Convert.ToInt32(arrMsg[1], 16) << 8) + Convert.ToInt32(arrMsg[2], 16);
                    crc = Convert.ToInt32(arrMsg[3], 16);
                    for (int i = 0; i < 20; i++)
                    {
                        if (arrMsg[4 + i] != "00")
                        {
                            byte[] bt = new byte[1];
                            bt[0] = Convert.ToByte(arrMsg[4 + i], 16);
                            simCardTcp += System.Text.Encoding.UTF8.GetString(bt);
                        }
                    }
                    byte[] bt_ret = new byte[numeroBytes];
                    for (int i = 0; i < numeroBytes; i++)
                    {
                        bt_ret[i] = Convert.ToByte(arrMsg[24 + i], 16);                      
                    }
                    retornoLicense = Encoding.UTF8.GetString(bt_ret);
                    id_evt = Convert.ToInt32(arrMsg[arrMsg.Length - 3], 16);



                    #region Recebeu Periódico Setar novo tempo de frame 
                    if (id_evt == 1 && codSim == simCardTcp)
                    {
                        retorno = true;
                    }
                    #endregion
                }
            }
            return retorno;
        }
        private byte[] montaPacoteValManutencao(byte codeMessage, string codSimcard, string message)
        {
            byte[] retorno = new byte[26 + message.Length];
            int tamP = 26 + message.Length;
            retorno[0] = codeMessage;
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
            for (int i = 0; i < fim; i++)
            {
                retorno[25 + i] = (byte)message[i];
            }

            retorno[25 + fim] = 0xFF;
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

    
        #endregion
    }
}
