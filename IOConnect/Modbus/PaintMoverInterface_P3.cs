using InTheHand.Net.Sockets;
using Percolore.Core.Logging;
using Percolore.IOConnect;
using Percolore.IOConnect.Util;
using System.IO.Ports;

namespace PaintMixer
{
	class PaintMoverInterface_P3 : IDisposable
    {
        public ModBusRtu mb = null;
        private byte slaveAddr;
        Percolore.IOConnect.Util.ObjectParametros parametros;
        List<Percolore.IOConnect.Util.ObjectMotorPlacaMovimentacao> _placaMotMov;
        private int timeoutResp = 1000;
        private string nomeDispositivo = "";
        private int counterConnect = 0;

        public bool TerminouProcessoDuplo { get; set; } = true;

        public PaintMoverInterface_P3(int slaveAddr, string NomeDispositivo = "")
        {
            if (slaveAddr <= 0 || slaveAddr > 255)
            {
                throw new Exception("Bad slave address; must be > 0 and < 255");
            }

            this.slaveAddr = (byte)slaveAddr;
            this.parametros = Percolore.IOConnect.Util.ObjectParametros.Load();
            this._placaMotMov = Percolore.IOConnect.Util.ObjectMotorPlacaMovimentacao.List();
            this.nomeDispositivo = NomeDispositivo;
        }


        public struct StatusValue
        {
            public StatusValue(int nativo, int coderror, bool copo, bool esponja, bool altoBico, bool baixoBico, bool gavetaAberta, bool gavetaFechada, bool valvulaAberta, 
                bool valvulaFechada, bool sensorEmergencia, int codAlerta, bool maquinaLigada)
            {
                Nativo = nativo;
                CodError = coderror;
                SensorCopo = copo;
                SensorAltoBicos = altoBico;
                SensorBaixoBicos = baixoBico;
                SensorEsponja = esponja;
                SensorGavetaAberta = gavetaAberta;
                SensorGavetaFechada = gavetaFechada;
                SensorValvulaDosagem = valvulaAberta;
                SensorValvulaRecirculacao = valvulaFechada;
                SensorEmergencia = sensorEmergencia;
                CodAlerta = codAlerta;
                MaquinaLigada = maquinaLigada;
            }

            public int Nativo;
            public int CodError;
            public bool SensorCopo;
            public bool SensorEsponja;
            public bool SensorAltoBicos;
            public bool SensorBaixoBicos;
            public bool SensorGavetaAberta;
            public bool SensorGavetaFechada;
            public bool SensorValvulaDosagem;
            public bool SensorValvulaRecirculacao;
            public bool SensorEmergencia;
            public int CodAlerta;
            public bool MaquinaLigada;
        }

        /// <summary>
        /// Polls the board for the Status register and returns the current status flags.
        /// </summary>
        /// <see cref="StatusValue"/>
        public StatusValue Status
        {
            get
            {
                int[] v = new int[13];
              
                ushort pollStart = (ushort)100;
                if (!mb.SendFc3(slaveAddr, pollStart, 13, ref v))
                {
                    throw new Exception("Could not read status register: " + mb.modbusStatusStr);
                }

                int _nativo = 0;
                int _error = 0;
                bool _copo = false;
                bool _esponja = false;
                bool _altoBico = false;
                bool _baixoBico = false;
                bool _gavetaAberta = false;
                bool _gavetaFechada = false;
                bool _valvulaAberta = false;
                bool _valvulaFechada = false;
                bool _emergencia = false;
                int _aletra = 0;
                bool _maquinaLigada = false;
                if (v[0] <= 3)
                {
                    _nativo = v[0]; 
                }
                else
                {
                    _nativo = 4;
                }
                _error = v[1];
                if(v[2] == 1)
                {
                    _copo = true;
                }
                if(v[3] == 1)
                {
                    _esponja = true;
                }
                if(v[4] == 1)
                {
                    _altoBico = true;
                }
                if(v[5] == 1)
                {
                    _baixoBico = true;
                }
                if(v[6] == 1)
                {
                    _gavetaAberta = true;
                }
                if(v[7] == 1)
                {
                    _gavetaFechada = true;
                }
                if(v[8] == 1)
                {
                    _valvulaAberta = true;
                }
                if(v[9] == 1)
                {
                    _valvulaFechada = true;
                }
                if(v[10] == 1)
                {
                    _emergencia = true;
                }
                _aletra = v[11];
                if(v[12] == 1)
                {
                    _maquinaLigada = true;
                }
                
                return new StatusValue
                    (
                        _nativo,
                        _error,
                        _copo,
                        _esponja,
                        _altoBico,
                        _baixoBico,
                        _gavetaAberta,
                        _gavetaFechada,
                        _valvulaAberta,
                        _valvulaFechada,
                        _emergencia,
                        _aletra,
                        _maquinaLigada
                    );
            }
        }

        /// <summary>
        /// Opens the COM port to allow communication to occur. 
        /// Must be called before any other methods or properties.
        /// Throws an exception if the COM port could not be opened.
        /// </summary>
        public void Connect(int responseTimeout)
        {
            this.timeoutResp = responseTimeout;
            bool bConnect = false;
            if (mb == null)
            {
                mb = new ModBusRtu();
            }

            bConnect = mb.isOpen();
            if (!mb.isOpen())
            {
                string[] portas = null;
                try
                {
                    portas = SerialPort.GetPortNames();
                    
                    if (this.nomeDispositivo != "")
                    {
                        bool achouPorta = false;
                        string mPortaConfig = "";
                        foreach (string np in portas)
                        {
                            if (np.ToUpper() == this.nomeDispositivo.ToUpper())
                            {
                                mPortaConfig = np;
                                achouPorta = true;
                            }
                        }
                        if (achouPorta)
                        {
                            portas = new string[1];
                            portas[0] = mPortaConfig;
                        }
                    }
                }
				catch (Exception e)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
				}
				
                if (portas == null || portas.LongLength == 0)
                {
                    throw new Exception(
                        Percolore.IOConnect.Negocio.IdiomaResxExtensao.Global_DisensaNaoConectado);
                    //"O dispositivo de dispensa não encontra-se conectado à porta de comunicação.");
                }
                foreach (string porta in portas)
                {
                    mb.Open(porta, 9600, 8, Parity.Even, StopBits.One);
                    if (mb.isOpen())
                    {
                        bConnect = true;
                        int[] v = new int[1];
                        ushort pollStart = (ushort)100;
                        mb.SendFc3(slaveAddr, pollStart, 1, ref v);
                        if (!mb.modbusStatus)
                        {
                            this.counterConnect++;
                            //Se não for possível ler status, encerra comunicação com a porta                   
                            mb.CloseM();
                            try
                            {
                                if (counterConnect > 2)
                                {
                                    counterConnect = 0;
                                    BluetoothClient btClient = new BluetoothClient();
									var devices = btClient.DiscoverDevices();
									Bluetooth.PairBluetoothDevices(devices);
								}
                            }
							catch (Exception e)
							{
								LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
							}
						}
                        else
                        {
                            this.counterConnect = 0;
                        }
                    }
                }
            }

            if (!bConnect)
            {
                throw new Exception("Error failure communications with serial port!");
            }
            else if (!mb.isOpen())
            {
                throw new Exception(mb.modbusStatusStr);
            }
        }

        /// <summary>
        /// Close the COM port and consequently, disconnect from the paint mixer board.
        /// </summary>
        public void Disconnect()
        {
            if (mb != null)
            {
                mb.CloseM();
            }
        }


        public void MovimentarManual(int motor, bool isForward)
        {
            if (parametros.HabilitarLogComunicacao)
            {
                Log.Logar(
                    TipoLog.Comunicacao, Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema, "Iniciando comando Run.");
            }
            if (mb == null || !mb.isOpen())
            {
                if (parametros.HabilitarLogComunicacao)
                {
                    Log.Logar(
                        TipoLog.Comunicacao, Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema, "Dispositivo desconectado.");
                }
                throw new Exception("Not connected");
            }
            if(motor == 1)
            {
                Percolore.IOConnect.Util.ObjectMotorPlacaMovimentacao opM = this._placaMotMov.Find(o => o.Circuito == motor);
                #region Motor 1
                int[] v_write = new int[35];
                int _fw = 0;

                if(isForward)
                {
                    _fw = 1;
                }
                else
                {
                    _fw = 2;
                }
                // Home desabilitado
                v_write[0] = 0;
                // Modo Operacao em Manual                
                v_write[1] = 0;
                // Modo Operacao em Manual Motor 1                
                v_write[2] = 1;
                // Modo Direcao                
                v_write[3] = _fw;
                // Executar comando no motor 1                
                v_write[4] = 1;

                // Setar pulsos motor 1                
                int ant = opM.Pulsos;
                int nshift = (int)((ant >> 16) & 0x0000FFFF);
                int i16 = (int)(ant & 0x0000FFFF);
                v_write[5] = nshift;
                v_write[6] = i16;

                // Setar velocidade motor 1                
                ant = opM.Velocidade;
                nshift = (int)((ant >> 16) & 0x0000FFFF);
                i16 = (int)(ant & 0x0000FFFF);
                v_write[7] = nshift;
                v_write[8] = i16;

                // Setar aceleracao motor 1                
                ant = opM.Aceleracao;
                nshift = (int)((ant >> 16) & 0x0000FFFF);
                i16 = (int)(ant & 0x0000FFFF);
                v_write[9] = nshift;
                v_write[10] = i16;

                // Setar Delay motor 1                
                ant = opM.Delay;
                nshift = (int)((ant >> 16) & 0x0000FFFF);
                i16 = (int)(ant & 0x0000FFFF);
                v_write[11] = nshift;
                v_write[12] = i16;

                // Modo Operacao em Manual Motor 2                
                v_write[13] = 0;
                // Modo Direcao no motor 2
                v_write[14] = 0;
                // Executar comando no motor 2                
                v_write[15] = 0;


                // Setar pulsos motor 2              
                v_write[16] = 0;
                v_write[17] = 0;

                // Setar velocidade motor 2                
                v_write[18] = 0;
                v_write[19] = 0;

                // Setar aceleracao motor 2                
                v_write[20] = 0;
                v_write[21] = 0;

                // Setar Delay motor 2                
                v_write[22] = 0;
                v_write[23] = 0;

                // Modo Operacao em Manual Motor 3                
                v_write[24] = 0;
                // Modo Direcao no motor 3
                v_write[25] = 0;
                // Executar comando no motor 3                
                v_write[26] = 0;


                // Setar pulsos motor 3                                
                v_write[27] = 0;
                v_write[28] = 0;

                // Setar velocidade motor 3                       
                v_write[29] = 0;
                v_write[30] = 0;

                // Setar aceleracao motor 3   
                v_write[31] = 0;
                v_write[32] = 0;

                // Setar Delay motor 3                
                v_write[33] = 0;
                v_write[34] = 0;

                mb.SendFc16(this.slaveAddr, (ushort)1, 35, v_write);
                if (!mb.modbusStatus)
                {
                    if (parametros.HabilitarLogComunicacao)
                    {

                        Log.Logar(
                        TipoLog.Comunicacao,
                        Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema,
                        "Não foi possível escrever dados de movimentação."
                        + Environment.NewLine
                        + mb.modbusStatusStr);
                    }

                    throw new Exception("Could not write movement data: " + mb.modbusStatusStr);
                }
                #endregion

            }
            else if(motor == 2)
            {
                Percolore.IOConnect.Util.ObjectMotorPlacaMovimentacao opM = this._placaMotMov.Find(o => o.Circuito == motor);
                #region Motor 2
                int[] v_write = new int[35]; 
                int _fw = 0;

                if (isForward)
                {
                    _fw = 1;
                }
                else
                {
                    _fw = 2;
                }
                // Home desabilitado
                v_write[0] = 0;
                // Modo Operacao em Manual                
                v_write[1] = 0;
                // Modo Operacao em Manual Motor 1                
                v_write[2] = 0;
                // Modo Direcao                
                v_write[3] = 0;
                // Executar comando no motor 1                
                v_write[4] = 0;

                // Setar pulsos motor 1    
                v_write[5] = 0;
                v_write[6] = 0;

                // Setar velocidade motor 1                
                v_write[7] = 0;
                v_write[8] = 0;

                // Setar aceleracao motor 1                
                v_write[9] = 0;
                v_write[10] = 0;

                // Setar Delay motor 1                
                v_write[11] = 0;
                v_write[12] = 0;

                // Modo Operacao em Manual Motor 2                
                v_write[13] = 1;
                // Modo Direcao no motor 2
                v_write[14] = _fw;
                // Executar comando no motor 2                
                v_write[15] = 1;


                // Setar pulsos motor 2                
                int ant = opM.Pulsos;
                int nshift = (int)((ant >> 16) & 0x0000FFFF);
                int i16 = (int)(ant & 0x0000FFFF);
                v_write[16] = nshift;
                v_write[17] = i16;

                // Setar velocidade motor 2       
                ant = opM.Velocidade;
                nshift = (int)((ant >> 16) & 0x0000FFFF);
                i16 = (int)(ant & 0x0000FFFF);
                v_write[18] = nshift;
                v_write[19] = i16;

                // Setar aceleracao motor 2   
                ant = opM.Aceleracao;
                nshift = (int)((ant >> 16) & 0x0000FFFF);
                i16 = (int)(ant & 0x0000FFFF);
                v_write[20] = nshift;
                v_write[21] = i16;

                // Setar Delay motor 2                
                ant = opM.Delay;
                nshift = (int)((ant >> 16) & 0x0000FFFF);
                i16 = (int)(ant & 0x0000FFFF);
                v_write[22] = nshift;
                v_write[23] = i16;

                // Modo Operacao em Manual Motor 3                
                v_write[24] = 0;
                // Modo Direcao no motor 3
                v_write[25] = 0;
                // Executar comando no motor 3                
                v_write[26] = 0;


                // Setar pulsos motor 3                                
                v_write[27] = 0;
                v_write[28] = 0;

                // Setar velocidade motor 3                       
                v_write[29] = 0;
                v_write[30] = 0;

                // Setar aceleracao motor 3   
                v_write[31] = 0;
                v_write[32] = 0;

                // Setar Delay motor 3                
                v_write[33] = 0;
                v_write[34] = 0;

                mb.SendFc16(this.slaveAddr, (ushort)1, 35, v_write);
                if (!mb.modbusStatus)
                {
                    if (parametros.HabilitarLogComunicacao)
                    {
                        Log.Logar(
                        TipoLog.Comunicacao,
                        Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema,
                        "Não foi possível escrever dados de movimentação."
                        + Environment.NewLine
                        + mb.modbusStatusStr);
                    }

                    throw new Exception("Could not write movement data: " + mb.modbusStatusStr);
                }
                #endregion
            }
            else if (motor ==3)
            {
                Percolore.IOConnect.Util.ObjectMotorPlacaMovimentacao opM = this._placaMotMov.Find(o => o.Circuito == motor);
                #region Motor 3
                int[] v_write = new int[35];
                int _fw = 0;

                if (isForward)
                {
                    _fw = 1;
                }
                else
                {
                    _fw = 2;
                }
                // Home desabilitado
                v_write[0] = 0;
                // Modo Operacao em Manual                
                v_write[1] = 0;
                // Modo Operacao em Manual Motor 1                
                v_write[2] = 0;
                // Modo Direcao                
                v_write[3] = 0;
                // Executar comando no motor 1                
                v_write[4] = 0;

                // Setar pulsos motor 1    
                v_write[5] = 0;
                v_write[6] = 0;

                // Setar velocidade motor 1                
                v_write[7] = 0;
                v_write[8] = 0;

                // Setar aceleracao motor 1                
                v_write[9] = 0;
                v_write[10] = 0;

                // Setar Delay motor 1                
                v_write[11] = 0;
                v_write[12] = 0;

                // Modo Operacao em Manual Motor 2                
                v_write[13] = 0;
                // Modo Direcao no motor 2
                v_write[14] = 0;
                // Executar comando no motor 2                
                v_write[15] = 0;


                // Setar pulsos motor 2                                
                v_write[16] = 0;
                v_write[17] = 0;

                // Setar velocidade motor 2                       
                v_write[18] = 0;
                v_write[19] = 0;

                // Setar aceleracao motor 2   
                v_write[20] = 0;
                v_write[21] = 0;

                // Setar Delay motor 2                
                v_write[22] = 0;
                v_write[23] = 0;



                // Modo Operacao em Manual Motor 3                
                v_write[24] = 1;
                // Modo Direcao no motor 3
                v_write[25] = _fw;
                // Executar comando no motor 3                
                v_write[26] = 1;


                // Setar pulsos motor 3                
                int ant = opM.Pulsos;
                int nshift = (int)((ant >> 16) & 0x0000FFFF);
                int i16 = (int)(ant & 0x0000FFFF);                
                v_write[27] = nshift;
                v_write[28] = i16;

                // Setar velocidade motor 3       
                ant = opM.Velocidade;
                nshift = (int)((ant >> 16) & 0x0000FFFF);
                i16 = (int)(ant & 0x0000FFFF);
                v_write[29] = nshift;
                v_write[30] = i16;

                // Setar aceleracao motor 3   
                ant = opM.Aceleracao;
                nshift = (int)((ant >> 16) & 0x0000FFFF);
                i16 = (int)(ant & 0x0000FFFF);
                v_write[31] = nshift;
                v_write[32] = i16;

                // Setar Delay motor 3           
                ant = opM.Delay;
                nshift = (int)((ant >> 16) & 0x0000FFFF);
                i16 = (int)(ant & 0x0000FFFF);
                v_write[33] = nshift;
                v_write[34] = i16;

                mb.SendFc16(this.slaveAddr, (ushort)1, 35, v_write);
                if (!mb.modbusStatus)
                {
                    if (parametros.HabilitarLogComunicacao)
                    {
                        Log.Logar(
                        TipoLog.Comunicacao,
                        Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema,
                        "Não foi possível escrever dados de movimentação."
                        + Environment.NewLine
                        + mb.modbusStatusStr);
                    }

                    throw new Exception("Could not write movement data: " + mb.modbusStatusStr);
                }
                #endregion
            }

        }

        public void MovimentarAutomatico()
        {
            if (parametros.HabilitarLogComunicacao)
            {
                Log.Logar(
                    TipoLog.Comunicacao, Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema, "Iniciando comando Run.");
            }
            if (mb == null || !mb.isOpen())
            {
                if (parametros.HabilitarLogComunicacao)
                {
                    Log.Logar(
                        TipoLog.Comunicacao, Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema, "Dispositivo desconectado.");
                }
                throw new Exception("Not connected");
            }
            Percolore.IOConnect.Util.ObjectMotorPlacaMovimentacao opM = this._placaMotMov.Find(o => o.Circuito == 1);
            Percolore.IOConnect.Util.ObjectMotorPlacaMovimentacao opM2 = this._placaMotMov.Find(o => o.Circuito == 2);
            #region Motor 1
            int[] v_write = new int[24];
            int _fw = 0;

            
            // Home desabilitado
            v_write[0] = 0;
            // Modo Operacao em Automático                
            v_write[1] = 0;
            // Modo Operacao em Manual Motor 1                
            v_write[2] = 0;
            // Modo Direcao                
            v_write[3] = _fw;
            // Executar comando no motor 1                
            v_write[4] = 1;

            // Setar pulsos motor 1                
            int ant = opM.Pulsos;
            int nshift = (int)((ant >> 16) & 0x0000FFFF);
            int i16 = (int)(ant & 0x0000FFFF);
            v_write[5] = nshift;
            v_write[6] = i16;

            // Setar velocidade motor 1                
            ant = opM.Velocidade;
            nshift = (int)((ant >> 16) & 0x0000FFFF);
            i16 = (int)(ant & 0x0000FFFF);
            v_write[7] = nshift;
            v_write[8] = i16;

            // Setar aceleracao motor 1                
            ant = opM.Aceleracao;
            nshift = (int)((ant >> 16) & 0x0000FFFF);
            i16 = (int)(ant & 0x0000FFFF);
            v_write[9] = nshift;
            v_write[10] = i16;

            // Setar Delay motor 1                
            ant = opM.Delay;
            nshift = (int)((ant >> 16) & 0x0000FFFF);
            i16 = (int)(ant & 0x0000FFFF);
            v_write[11] = nshift;
            v_write[12] = i16;

            // Modo Operacao em Manual Motor 2                
            v_write[13] = 0;
            // Modo Direcao no motor 2
            v_write[14] = 0;
            // Executar comando no motor 2                
            v_write[15] = 0;


            // Setar pulsos motor 2  
            int ant2 = opM2.Pulsos;
            int nshift2 = (int)((ant2 >> 16) & 0x0000FFFF);
            int i162 = (int)(ant2 & 0x0000FFFF);
            v_write[16] = nshift2;
            v_write[17] = i162;

            // Setar velocidade motor 2    
            ant2 = opM2.Velocidade;
            nshift2 = (int)((ant2 >> 16) & 0x0000FFFF);
            i162 = (int)(ant2 & 0x0000FFFF);
            v_write[18] = nshift2;
            v_write[19] = i162;

            // Setar aceleracao motor 2     
            ant2 = opM2.Aceleracao;
            nshift2 = (int)((ant2 >> 16) & 0x0000FFFF);
            i162 = (int)(ant2 & 0x0000FFFF);
            v_write[20] = nshift2;
            v_write[21] = i162;

            // Setar Delay motor 2    
            ant2 = opM2.Delay;
            nshift2 = (int)((ant2 >> 16) & 0x0000FFFF);
            i162 = (int)(ant2 & 0x0000FFFF);
            v_write[22] = nshift2;
            v_write[23] = i162;

            mb.SendFc16(this.slaveAddr, (ushort)1, 24, v_write);
            if (!mb.modbusStatus)
            {
                if (parametros.HabilitarLogComunicacao)
                {

                    Log.Logar(
                    TipoLog.Comunicacao,
                    Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema,
                    "Não foi possível escrever dados de movimentação."
                    + Environment.NewLine
                    + mb.modbusStatusStr);
                }

                throw new Exception("Could not write movement data: " + mb.modbusStatusStr);
            }
            #endregion
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (mb.isOpen())
                {
                    mb.CloseM();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}