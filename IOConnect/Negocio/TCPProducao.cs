using Percolore.Core.Logging;
using System.Net.Sockets;
using System.Text;

namespace Percolore.IOConnect.Negocio
{
	public class TCPProducao
    {
        private Util.ObjectParametros _parametros = null;
        TcpClient tcpcliente = new TcpClient();
        
        private bool testeConnect = false;

        public TCPProducao(Util.ObjectParametros _par)
        {
            this._parametros = _par;
        }
        public bool SendProducaoToTCP(string imei, string msgC)
        {
            bool retorno = false;

            byte[] ptW = montaPacoteProducao(imei, msgC);
            if (isWriteTCP(ptW, imei))
            {
                retorno = true;
            }
            return retorno;
        }

        public void CloseProducaoTCP()
        {
            try
            {
                if (this.tcpcliente.Connected)
                {
                    this.tcpcliente.Close();
                    this.tcpcliente.Dispose();
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}
		}

        #region Metodos Producao

        private bool isWriteTCP(byte[] pacoteMsg, string simCard)
        {
            bool retorno = false;
            try
            {
                if (this.testeConnect || CheckInternet.TestInternet(_parametros.IpProducao, _parametros.TimeoutPingTcp))
                {
                    this.tcpcliente = new TcpClient();

                    this.tcpcliente.Connect(_parametros.IpProducao, Convert.ToInt32(_parametros.PortaProducao));
                    this.testeConnect = true;
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
                                Thread.Sleep(1000);
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
                            if (DesmontarPacoteTCP(sb.ToString(), simCard))
                            {
                                retorno = true;
                            }
                        }
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}

			return retorno;
        }

        public bool DesmontarPacoteTCP(string msgHex, string codSim)
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
                    id_evt = Convert.ToInt32(arrMsg[26], 16);

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

        private byte[] montaPacoteProducao(string codSimcard, string message)
        {
            byte[] retorno = new byte[26 + message.Length];
            int tamP = 26 + message.Length;
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