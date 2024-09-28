using Percolore.Core.Logging;
using System.Net.Sockets;
using System.Text;

namespace Percolore.IOConnect.Negocio
{
	public class TCPEventos
    {
        private Util.ObjectParametros _parametros = null;
        TcpClient tcpcliente = new TcpClient();
        private Util.ObjectEventos objEvt = null;
        private bool testeConnect = false;
      

        public TCPEventos(Util.ObjectParametros _par)
        {
            this._parametros = _par;
            
        }

        public void SetObjEvt(Util.ObjectEventos _evt)
        {
            this.objEvt = _evt;
        }

        public bool SendEventToTCP(ref string msgRetrun)
        {
            bool retorno = false;
            try
            {
                msgRetrun = "";
                string imei = this.objEvt.NUMERO_SERIE;                
                string msgC = this.objEvt.NUMERO_SERIE + "|" + this.objEvt.COD_EVENTO.ToString() + "|" + this.objEvt.DETALHES + "|" + string.Format("{0:yyyy-MM-dd HH:mm:ss}",this.objEvt.DATAHORA);
                byte[] ptW = montaPacoteEvento(0x34, imei, msgC);
                string retornoMsg = "";
                if (isWriteTCP(ptW, imei, ref retornoMsg))
                {
                    //txtInput.Text = retornoMsg;
                    string[] _array = retornoMsg.Split(';');
                    if (_array[0] == "1")
                    {
                        retorno = true;
                    }
                    else
                    {
                        msgRetrun = _array[1];
                    }
                }
                else
                {
                    msgRetrun = Negocio.IdiomaResxExtensao.ValidadeManutencao_OnLine_Fail_Connect + " :" + retornoMsg;
                }
            }
            catch(Exception exc)
            {
                msgRetrun = Negocio.IdiomaResxExtensao.ValidadeManutencao_OnLine_Fail_Connect + " :" + exc.Message;
				LogManager.LogError(msgRetrun, exc);
			}

            return retorno;
        }

        public void CloseEventTCP()
        {
            try
            {
                if(this.tcpcliente.Connected)
                {
                    try
                    {
                        this.tcpcliente.Close();
                        this.tcpcliente.Dispose();
                    }
                    catch
                    {

                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}
		}

        #region Metodos Online

        private bool isWriteTCP(byte[] pacoteMsg, string simCard, ref string _MsgRet)
        {
            bool retorno = false;
            _MsgRet = "";
            
            try
            {
                if (this.testeConnect || CheckInternet.TestInternet(_parametros.IpSincToken, _parametros.TimeoutPingTcp))
                {
                    this.tcpcliente = new TcpClient();
                    this.tcpcliente.SendTimeout = _parametros.TimeoutPingTcp;
                    this.tcpcliente.NoDelay = true;
                    this.tcpcliente.Connect(_parametros.IpSincToken, Convert.ToInt32(_parametros.PortaSincToken));
                    
                    this.testeConnect = true;
                    NetworkStream netWriteread = this.tcpcliente.GetStream();
                    netWriteread.Write(pacoteMsg, 0, pacoteMsg.Length);
                    Thread.Sleep(1000);
                    int tempo_resposta = 5;
                    if (_parametros.TimeoutPingTcp > 5000)
                    { 
                        tempo_resposta = _parametros.TimeoutPingTcp / 1000;
                    }


                    if (this.tcpcliente.Connected)
                    {                       
                        for (int i = 0; i < tempo_resposta; i++)
                        {
                            if (this.tcpcliente.Available <= 0)
                            {
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                Thread.Sleep(1000);
                                break;
                            }
                        }
                        try
                        {
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
                            else
                            {
                                _MsgRet = "No Receive bytes";
                            }
                        }
                        catch (Exception exc1)
                        {
                            _MsgRet = exc1.Message;
							LogManager.LogError(_MsgRet, exc1);
						}
                    }
                }
            }
            catch (Exception exc)
            {
                _MsgRet = exc.Message;
				LogManager.LogError(_MsgRet, exc);
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

                    #region Recebeu confirmacao de frame 
                    if (id_evt == 1 && codSim == simCardTcp)
                    {
                        retorno = true;
                    }
                    #endregion
                }
            }
            return retorno;
        }
        private byte[] montaPacoteEvento(byte codeMessage, string codSimcard, string message)
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