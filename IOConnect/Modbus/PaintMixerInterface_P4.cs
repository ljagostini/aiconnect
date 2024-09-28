using InTheHand.Net.Sockets;
using Percolore.Core.Logging;
using Percolore.IOConnect;
using Percolore.IOConnect.Util;
using System.IO.Ports;

namespace PaintMixer
{
	/// <summary>
	/// Basic wrapper around the S3MBPC2 Modbus interface for the new firmware</summary>
	/// <remarks>
	/// This is a basic wrapper around the Modbus interface to the new board firmware for 
	/// the S3MBPC2 (16 motor paint mixer control board). Like the interface for the 
	/// last firmware, it uses the WSMBS.dll library from http://www.modbustools.com/.
	/// 
	/// Usage is fairly straight forward:
	/// 
	/// The constructor receives the Modbus slave address and COM port number. 
	/// To open the COM port, call Connect, which will fail if the COM port is
	/// open in another application, doesn't exist, or doesn't support 9600 baud,
	/// one stop bit, even parity, eight data bits as a config.
	/// 
	/// To close the COM port, call disconnect. While not connected, all other public
	/// methods will fail.
	/// 
	/// Once connected, there are two public methods to perform the motion routine:
	/// Run and RunReverse. Both functions take three int[16] arrays containing the
	/// distance each motor should move in steps, the velocity each motor should 
	/// reach (in steps/sec), and the rate of acceleration/deceleration (in
	/// steps/sec^2)
	/// 
	/// The Run method just moves each motor, whereas RunReverse includes an additional
	/// reverse move (to suck the last drop of paint back into the pump) specified by two
	/// parameters: the delay between the normal move and the reverse move, and an int[16] array
	/// providing the length of the reverse move in steps for each motor.
	/// 
	/// When one of these methods is called, the move will begin, and the Busy flag in the
	/// Status property will become true. The move is complete once the Busy flag becomes
	/// false again.
	/// 
	/// While the move takes place, some information on the current state can be obtained:
	/// <list type="bullet">
	///     <item><description>
	///     The Status flags Moving, Reving, and Delaying each indicate which portion of the move
	///     is currently occuring (only one of these will be enabled at a time).
	///     </item></description>
	///     <item><description>
	///     The current motor property returns the index (0 indexed) of the motor that is
	///     currently moving.
	///     </item></description>
	/// </list>
	/// 
	/// To abrubtly abort motion, call the Halt method, and then clear the halt with Unhalt or
	/// by simply performing a move.
	/// 
	/// Note that all of the methods and properties here perform blocking I/O operations on the
	/// serial port. The Run and ReverseRun also sit in an idle loop for about 1 ms or 2. As such, 
	/// consider operating this interface in a separate thread from GUI event loop if you find it
	/// adds undesirable delays/jitters to the UI.
	/// 
	/// When polling the Status and CurrentMotor properties while a move takes place, it is
	/// reccomended that you do not poll more than once every 100 milliseconds, to avoid 
	/// "Denial of Servicing" the board and slowing down the motion control code. It is likely that
	/// poll rates of less than this are fine, but they are probably useless and put an uneeded load on 
	/// the board to process a bunch of redundant requests.
	/// 
	/// </remarks>
	class PaintMixerInterface_P4 : IDisposable
    {
        private const int MOTOR_COUNT = 24;
        private const int REG_STATUS = 0;
        private const int REG_CONTROL = 1;
        private const int REG_MOTOR_NUM = 2;
        private const int REG_MOTORS = 3;
        private const int REG_VEL = 35;
        private const int REG_ACC = 67;
        private const int REG_REVSTEPS = 99;
        private const int REG_REVDELAY = 131;
        private const int REG_MOTOR_ORDER = 147;
        private const int REG_INPUTS = 163;
        private const int REG_OUTPUTS = 164;
        private const int REG_PULSEWIDTH = 165;
        private const int REG_POLARITY = 166;
        private const int REG_COUNT = 167;
        private const int REG_STEP = 168;

        private const int REG_RESSET_HARD = 169;
        private const int REG_VERSION = 170;

        private const int REG_STATUSVALVULA = 171;

        private const int REG_ACIONAVALVULA = 173;



        private const int STATUS_BUSY = 1;
        private const int STATUS_MOVING = 2;
        private const int STATUS_REVING = 4;
        private const int STATUS_DELAYED = 8;
        private const int STATUS_HALTED = 16;
        private const int CONTROL_IDLE = 0;
        private const int CONTROL_RUN = 1;
        private const int CONTROL_RUNREV = 2;
        private const int CONTROL_HALT = 3;

        public ModBusRtu mb = null;
        private byte slaveAddr;
        Percolore.IOConnect.Util.ObjectParametros parametros;
        private int timeoutResp = 1000;
        private string nomeDispositivo = "";

        public bool isDosed = true;

        private void writeControl(ushort val)
        {
            string control;
            switch (val)
            {
                #region Control

                case CONTROL_IDLE:
                    {
                        control = "IDLE";
                        break;
                    }
                case CONTROL_RUN:
                    {
                        control = "RUN";
                        break;
                    }
                case CONTROL_RUNREV:
                    {
                        control = "RUNREV";
                        break;
                    }
                case CONTROL_HALT:
                    {
                        control = "HALT";
                        break;
                    }
                default:
                    {
                        control = string.Empty;
                        break;
                    }

                    #endregion
            }

            int nval = val;

            mb.SendFc6(slaveAddr, REG_CONTROL, nval);

            if (mb.modbusStatus)
            {
                Log.Logar(
                    TipoLog.Comunicacao, Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema, "Registro de controle: " + control);
            }
            else
            {
                Log.Logar(
                    TipoLog.Comunicacao,
                    Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema,
                    "Não foi possível gravar o registro de controle."
                    + Environment.NewLine
                     + mb.modbusStatusStr);

                throw new Exception(
                    //"Could not write control register: " + wsmbs.GetLastErrorString());
                    "Could not write control register: " + mb.modbusStatusStr + Environment.NewLine + Percolore.IOConnect.Negocio.IdiomaResxExtensao.FalhaPortaSerial);
            }
        }

        private void writeData(int[] dist, int[] vel, int[] acc, int[] dly, int[] revs)
        {
            if (parametros.HabilitarLogComunicacao)
            {
                Log.Logar(
                    TipoLog.Comunicacao,
                    Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema,
                    "Iniciando escrita de dados para o dispositivo.");
            }

            if (!mb.isOpen())
            {
                Connect(this.timeoutResp);
            }

            int[] pacoteWr = new int[96];
            ushort start = 3;

            for (int i = 0; i < dist.Length; i++)
            {
                int ant = dist[i];

                int nshift = (int)((ant >> 16) & 0x0000FFFF);

                int i16 = (int)(ant & 0x0000FFFF);

                pacoteWr[(i * 2)] = nshift;
                pacoteWr[(i * 2) + 1] = i16;
            }
            
            for (int i = 0; i < vel.Length; i++)
            {
                int ant = vel[i];

                int nshift = (int)((ant >> 16) & 0x0000FFFF);

                int i16 = (int)(ant & 0x0000FFFF);

                pacoteWr[(48) + (i * 2)] = nshift;
                pacoteWr[(48) + (i * 2) + 1] = i16;
            }

            mb.SendFc16(slaveAddr, start, (ushort)pacoteWr.Length, pacoteWr);

            if (mb.modbusStatus)
            {
                pacoteWr = new int[120];
                start += 96;

                for (int i = 0; i < acc.Length; i++)
                {
                    int ant = acc[i];
                    int nshift = (int)((ant >> 16) & 0x0000FFFF);
                    int i16 = (int)(ant & 0x0000FFFF);

                    pacoteWr[(i * 2)] = nshift;
                    pacoteWr[(i * 2) + 1] = i16;
                }

                for (int i = 0; i < revs.Length; i++)
                {

                    int ant = revs[i];
                    int nshift = (int)((ant >> 16) & 0x0000FFFF);
                    int i16 = (int)(ant & 0x0000FFFF);

                    pacoteWr[(48) + (i * 2)] = nshift;
                    pacoteWr[(48) + (i * 2) + 1] = i16;
                }
                for (int i = 0; i < dly.Length; i++)
                {
                    pacoteWr[(96) + i] = dly[i];
                }

                mb.SendFc16(slaveAddr, start, (ushort)pacoteWr.Length, pacoteWr);

            }
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

                throw new Exception("Could not write movement data: " + mb.modbusStatusStr + Environment.NewLine + Percolore.IOConnect.Negocio.IdiomaResxExtensao.FalhaPortaSerial);
            }
            

            if (parametros.HabilitarLogComunicacao)
            {

                string p = string.Join(", ", dist);
                string v = string.Join(", ", vel);
                string a = string.Join(", ", acc);
                string d = string.Join(", ", dly);
                string r = string.Join(", ", revs);

                Log.Logar(
                    TipoLog.Comunicacao,
                    Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema,
                    "Dados enviados ao dispositivo:"
                    + $"{Environment.NewLine}Pulso.....: {p}"
                    + $"{Environment.NewLine}Velocidade: {v}"
                    + $"{Environment.NewLine}Aceleração: {a}"
                    + $"{Environment.NewLine}Delay.....: {d}"
                    + $"{Environment.NewLine}Reverse...: {r}"
                    );
            }
        }


        private int idDispositivo;
        private int counterConnect = 0;

        public int IdDispositivo
        {
            get { return idDispositivo; }
            set { idDispositivo = value; }
        }

        /// <summary>
        /// Constructs a new paint mixer interface, opening the COM port "COMp" where p is the
        /// value of port, and communicating over Modbus to a device with the given slave address
        /// </summary>
        /// <param name="slaveAddr">The slave address of the board</param>
        /// <param name="port">The COM port number</param>
        public PaintMixerInterface_P4(int slaveAddr, string NomeDispositivo = "")
        {
            if (slaveAddr <= 0 || slaveAddr > 255)
            {
                throw new Exception("Bad slave address; must be > 0 and < 255");
            }

            this.slaveAddr = (byte)slaveAddr;
            this.parametros = Percolore.IOConnect.Util.ObjectParametros.Load();
            this.nomeDispositivo = NomeDispositivo;
        }


        public bool Connected
        {
            get
            {
                if (mb == null)
                {
                    return false;
                }
                else
                {
                    return mb.isOpen();
                }
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
            //mb = ModBusRtu.getModBusRtu();
            bool bConnect = false;
            if (mb == null)
            {
                mb = new ModBusRtu();
            }

            if (!mb.isOpen())
            {
                string[] portas = null;
                try
                {
                    portas = SerialPort.GetPortNames();
                    //if(parametros.NomeDispositivo != "")
                    if (this.nomeDispositivo != "")
                    {
                        bool achouPorta = false;
                        string mPortaConfig = "";
                        foreach (string np in portas)
                        {
                            //if(np.ToUpper() == parametros.NomeDispositivo.ToUpper())
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
                    mb.Open(porta, 9600, 8, System.IO.Ports.Parity.Even, StopBits.One);
                    if (mb.isOpen())
                    {
                        bConnect = true;
                        int[] v = new int[1];
                        ushort pollStart = (ushort)REG_STATUS;
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
                            break;
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
            this.isDosed = true;
            
            if (mb != null)
            {
                mb.CloseM();
            }
        }

        /// <summary>
        /// The set of flags obtained from the board Status register.
        /// </summary>
        /// <remarks>
        /// This struct contains 5 flags which are read from the Status register
        /// of the board in when the Status property is accesed. The flags are as
        /// follows:
        /// 
        /// <list type="table">
        ///     <listheader>
        ///         <term>Flag</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term>Busy</term>
        ///         <description>
        ///         This flag indicates that the board is currently performing motion and
        ///         will not respond to any motion commands until the flag is cleared, indicating that
        ///         the current motion is complete.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>Moving</term>
        ///         <description>
        ///         Indicates that the board is currently performing the dispensing motion of the
        ///         current motor.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>Reving</term>
        ///         <description>
        ///         Indicates that the board is currently performing the reverse drip motion of the
        ///         current motor.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>Delaying</term>
        ///         <description>
        ///         Indicates that the board is currently performing the delay between the dispense and
        ///         reverse moves for the current motor.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>Halted</term>
        ///         <description>
        ///         Indicates that the board is currently halted such that no motion is occuring and
        ///         all motion commands are ignored.
        ///         </description>
        ///     </item>
        /// </list>
        /// </remarks>
        public struct StatusValue
        {
            public StatusValue(bool b, bool m, bool r, bool d, bool h)
            {
                Busy = b; Moving = m; Reving = r; Delaying = d; Halted = h;
            }

            public bool Busy;
            public bool Moving;
            public bool Reving;
            public bool Delaying;
            public bool Halted;
        }

        /// <summary>
        /// Polls the board for the Status register and returns the current status flags.
        /// </summary>
        /// <see cref="StatusValue"/>
        public StatusValue Status
        {
            get
            {
                int[] v = new int[2];

                ushort pollStart = (ushort)REG_STATUS;
                if (!mb.SendFc3(slaveAddr, pollStart, 1, ref v))
                {
                    throw new Exception("Could not read status register: " + mb.modbusStatusStr + Environment.NewLine + Percolore.IOConnect.Negocio.IdiomaResxExtensao.FalhaPortaSerial);
                }

                return new StatusValue
                    (
                        (v[0] & STATUS_BUSY) != 0,
                        (v[0] & STATUS_MOVING) != 0,
                        (v[0] & STATUS_REVING) != 0,
                        (v[0] & STATUS_DELAYED) != 0,
                        (v[0] & STATUS_HALTED) != 0
                    );
            }
        }


        public struct StatusInput
        {
            public StatusInput(bool in1, bool in2, bool in3, bool in4)
            {
                Input_1 = in1; Input_2 = in2; Input_3 = in3; Input_4 = in4;
            }

            public bool Input_1;
            public bool Input_2;
            public bool Input_3;
            public bool Input_4;
        }

        public StatusInput Status_Inputs
        {
            get
            {
                if (parametros.HabilitarLogComunicacao)
                {
                    Log.Logar(
                        TipoLog.Comunicacao, Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema, "Inicia a leitura de Inputs.");
                }

                if (!mb.isOpen())
                {
                    Connect(this.timeoutResp);
                }
                int[] v = new int[2];
                ushort pollStart = (ushort)REG_INPUTS;
                if (mb.isOpen())
                {
                    mb.SendFc3(slaveAddr, pollStart, 1, ref v);
                }
                bool input01 = (v[0] & (0x01)) != 0;
                bool input02 = (v[0] & (0x02)) != 0;
                bool input03 = (v[0] & (0x04)) != 0;
                bool input04 = (v[0] & (0x08)) != 0;
                if (parametros.HabilitarLogComunicacao)
                {
                    Log.Logar(
                        TipoLog.Comunicacao,
                        Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema,
                        string.Format(
                            "Input 01 = {0}, Input 02 = {1}, Input 03 = {2}, Input 04 = {3}",
                            input01.ToString(), input02.ToString(), input03.ToString(), input04.ToString())
                    );
                }
                return new StatusInput
                   (
                       input01,
                       input02,
                       input03,
                       input04
                   );
            }
        }

        public bool IndentifyPot
        {
            get
            {
                StatusInput sti = Status_Inputs;
                if (sti.Input_2 && sti.Input_3)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Polls the board for the Inputs register, whose first four bits represent the state of the
        /// digital inputs
        /// </summary>
        public ushort Inputs
        {
            get
            {
                if (parametros.HabilitarLogComunicacao)
                {
                    Log.Logar(
                        TipoLog.Comunicacao, Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema, "Inicia a leitura de Inputs.");
                }
                
                if (!mb.isOpen())
                {
                    Connect(this.timeoutResp);
                }
                int[] v = new int[2];
                ushort pollStart = (ushort)REG_INPUTS;
                if (mb.isOpen())
                {
                    mb.SendFc3(slaveAddr, pollStart, 1, ref v);
                }

                bool input01 = (v[0] & (0x01)) != 0;
                bool input02 = (v[0] & (0x02)) != 0;
                bool input03 = (v[0] & (0x04)) != 0;
                bool input04 = (v[0] & (0x08)) != 0;

                if (parametros.HabilitarLogComunicacao)
                {
                    Log.Logar(
                        TipoLog.Comunicacao,
                        Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema,
                        string.Format(
                            "Input 01 = {0}, Input 02 = {1}, Input 03 = {2}, Input 04 = {3}",
                            input01.ToString(), input02.ToString(), input03.ToString(), input04.ToString())
                    );
                }

                return (ushort)v[0];
            }
        }

        /// <summary>
        /// Polls the board for the outputs register or sets the output register, whose first four bits
        /// set the state of the digital outputs.
        /// </summary>
        public ushort Outputs
        {
            get
            {
                if (!mb.isOpen())
                {
                    Connect(this.timeoutResp);
                }
                int[] v = new int[2];
                if (mb.isOpen())
                {
                    ushort pollStart = (ushort)REG_OUTPUTS;
                    if (!mb.SendFc3(slaveAddr, pollStart, 1, ref v))
                    {
                        throw new Exception("Could not read outputs register: " + mb.modbusStatusStr + Environment.NewLine + Percolore.IOConnect.Negocio.IdiomaResxExtensao.FalhaPortaSerial);
                    }
                }
                return (ushort)v[0];
            }
            set
            {
                if (!mb.isOpen())
                {
                    Connect(this.timeoutResp);
                }
                //int[] v = new int[1];
                if (mb.isOpen())
                {
                    ushort pollStart = (ushort)REG_OUTPUTS;
                    int v = (int)value;
                    
                    if (!mb.SendFc6(slaveAddr, pollStart, v))
                    {
                        throw new Exception("Could not write outputs register: " + mb.modbusStatusStr + Environment.NewLine + Percolore.IOConnect.Negocio.IdiomaResxExtensao.FalhaPortaSerial);
                    }
                }
            }
        }


        /// <summary>
        /// Performs a motion command, moving all 16 motors the amounts specified in the distances array,
        /// using the velocities and accelerations from the following two arrays.
        /// </summary>
        /// <remarks>
        /// This command performs the dispensing cycle without any reverse drip moves.
        /// In order to perform this command, the Busy flag must currently be clear, indicating that there
        /// is no command/motion currently occuring. Also, the halted flag must be clear, indicating that
        /// the board has not been halted.
        /// 
        /// The command requires three arrays of 16 ints, specifying the move distance (steps), velocity,
        /// and acceleration for each of the 16 motors.
        /// </remarks>
        /// <param name="distances">16 ints, each denoting move distance in steps. Can be positive or negative</param>
        /// <param name="velocities">16 ints, each denoting velocity in steps/sec. Must be positive.</param>
        /// <param name="accelerations">16 ints, each denoting acceleration in steps/sec^2.</param>
        public void Run(int[] distances, int[] velocities, int[] accelerations, int i_Step_Circuito = 0)
        {
            this.isDosed = false;
            try
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

                if (distances.Length < MOTOR_COUNT)
                {
                    if (parametros.HabilitarLogComunicacao)
                    {
                        Log.Logar(
                            TipoLog.Comunicacao, Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema, "Foram informados menos de 16 motores.");
                    }

                    throw new Exception("Less than 16 distances provided. Cannot run motors");
                }

                if (velocities.Length < MOTOR_COUNT)
                {
                    if (parametros.HabilitarLogComunicacao)
                    {
                        Log.Logar(
                            TipoLog.Comunicacao, Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema, "Foram informados menos de 16 velocidades.");
                    }

                    throw new Exception("Less than 16 velocities provided. Cannot run motors");
                }

                if (accelerations.Length < MOTOR_COUNT)
                {
                    if (parametros.HabilitarLogComunicacao)
                    {
                        Log.Logar(
                            TipoLog.Comunicacao, Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema, "Foram informados menos de 16 acelerações.");
                    }

                    throw new Exception("Less than 16 accelerations provided. Cannot run motors");
                }
                int[] v = new int[2];

                ushort pollStart = (ushort)REG_STATUS;
                if (!mb.SendFc3(slaveAddr, pollStart, 1, ref v))
                {
                    throw new Exception("Could not read status register: " + mb.modbusStatusStr + Environment.NewLine + Percolore.IOConnect.Negocio.IdiomaResxExtensao.FalhaPortaSerial);
                }

                if (i_Step_Circuito > 0)
                {
                    int pacoteWr = i_Step_Circuito;
                    ushort m_start = REG_STEP;
                    if (!mb.SendFc6(slaveAddr, m_start, pacoteWr))
                    {
                        throw new Exception("Could not read status register: " + mb.modbusStatusStr + Environment.NewLine + Percolore.IOConnect.Negocio.IdiomaResxExtensao.FalhaPortaSerial);
                    }
                }

                int[] revs = new int[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                writeData(distances, velocities, accelerations, revs, revs);
                writeControl(CONTROL_RUN);

                while (!Status.Busy)
                {
                    Thread.Sleep(1);
                }

                writeControl(CONTROL_IDLE);
                this.isDosed = true;
            }
            catch (Exception exc)
            {
                this.isDosed = true;
                LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", exc);
				throw;
            }
        }

        /// <summary>
        /// Performs a motion command, moving all 16 motors the amounts specified in the distances array,
        /// using the velocities and accelerations from the following two arrays. Each dispensing move on each
        /// motor will be followed by a reverse move to suck the last drop of paint back in. The length of this move
        /// is set per motor in the revSteps array (an int[16] array) and occurs revDelayMs milliseconds 
        /// after the dispense move.
        /// </summary>
        /// <remarks>
        /// This command performs the dispensing cycle with reverse drip moves.
        /// In order to perform this command, the Busy flag must currently be clear, indicating that there
        /// is no command/motion currently occuring. Also, the halted flag must be clear, indicating that
        /// the board has not been halted.
        /// 
        /// The command requires three arrays of 16 ints, specifying the move distance (steps), velocity,
        /// and acceleration for each of the 16 motors, and additionally, a
        /// third array of 16 ints, providing the reverse delay amount in milliseconds for each motor
        /// and a fourth array of 16 ints providing the reverse amounts in steps for the
        /// motors.
        /// </remarks>
        /// <param name="distances">16 ints, each denoting move distance in steps. Can be positive or negative</param>
        /// <param name="velocities">16 ints, each denoting velocity in steps/sec. Must be positive.</param>
        /// <param name="accelerations">16 ints, each denoting acceleration in steps/sec^2</param>
        /// <param name="revDelaysMs">16 ints, Delay between the dispense move and the reverse move in milliseconds for each motor.</param>
        /// <param name="revSteps">16 ints, each denoting the reverse move distance in steps. Should be positive.
        ///  (negative values make it a second forward move instead of a reverse one.</param>
        public void RunReverse(int[] distances, int[] velocities, int[] accelerations, int[] revDelaysMs, int[] revSteps, int i_Step_Circuito = 0)
        {
            this.isDosed = false;
            try
            {

                if (parametros.HabilitarLogComunicacao)
                {
                    Log.Logar(
                        TipoLog.Comunicacao, Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema, "Executa comando RunReverse");
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
                if (distances.Length < MOTOR_COUNT)
                {
                    if (parametros.HabilitarLogComunicacao)
                    {
                        Log.Logar(
                            TipoLog.Comunicacao, Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema, "Foram informadas menos de 16 distâncias.");
                    }

                    throw new Exception("Less than 16 distances provided. Cannot run motors");
                }

                if (velocities.Length < MOTOR_COUNT)
                {
                    if (parametros.HabilitarLogComunicacao)
                    {
                        Log.Logar(
                            TipoLog.Comunicacao, Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema, "Foram informadas menos de 16 velocidades.");
                    }

                    throw new Exception("Less than 16 velocities provided. Cannot run motors");
                }

                if (accelerations.Length < MOTOR_COUNT)
                {
                    if (parametros.HabilitarLogComunicacao)
                    {
                        Log.Logar(TipoLog.Comunicacao, Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema, "Foram informadas menos de 16 acelerações.");
                    }

                    throw new Exception("Less than 16 accelerations provided. Cannot run motors");
                }

                if (revDelaysMs.Length < MOTOR_COUNT)
                {
                    if (parametros.HabilitarLogComunicacao)
                    {
                        Log.Logar(TipoLog.Comunicacao, Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema, "Foram informadas menos de 16 delays.");
                    }

                    throw new Exception("Less than 16 reverse delays provided. Cannot run motors");
                }

                if (revSteps.Length < MOTOR_COUNT)
                {
                    if (parametros.HabilitarLogComunicacao)
                    {
                        Log.Logar(TipoLog.Comunicacao, Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema, "Foram informados menos de 16 pulsos reversos.");
                    }

                    throw new Exception("Less than 16 reverse distances provided. Cannot run motors.");
                }

                int[] v = new int[2];

                ushort pollStart = (ushort)REG_STATUS;
                if (!mb.SendFc3(slaveAddr, pollStart, 1, ref v))
                {
                    throw new Exception("Could not read status register: " + mb.modbusStatusStr + Environment.NewLine + Percolore.IOConnect.Negocio.IdiomaResxExtensao.FalhaPortaSerial);
                }

                if (i_Step_Circuito > 0)
                {
                    int pacoteWr = i_Step_Circuito;
                    ushort m_start = REG_STEP;
                    if (!mb.SendFc6(slaveAddr, m_start, pacoteWr))
                    {
                        throw new Exception("Could not read status register: " + mb.modbusStatusStr + Environment.NewLine + Percolore.IOConnect.Negocio.IdiomaResxExtensao.FalhaPortaSerial);
                    }
                }
                
                writeData(distances, velocities, accelerations, revDelaysMs, revSteps);
                
                writeControl(CONTROL_RUNREV);
                
                while (!Status.Busy)
                {
                    Thread.Sleep(100);
                }
                
                writeControl(CONTROL_IDLE);
                this.isDosed = true;
            }
            catch (Exception exc)
            {
                this.isDosed = true;
                LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", exc);
				throw;
            }
        }

        /// <summary>
        /// Halts the board, abruptly stopping all motion until the halt is cleared.
        /// </summary>
        public void Halt()
        {
            this.isDosed = true;
            if (parametros.HabilitarLogComunicacao)
            {
                Log.Logar(
                    TipoLog.Comunicacao, Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema, "Executa comando Halt.");
            }
            if (mb == null || !mb.isOpen())
            {
                throw new Exception("Not connected");
            }
            
            Thread.Sleep(1000);
            writeControl(CONTROL_HALT);
        }

        /// <summary>
        /// Clears a halt, returning the board to an idle state.
        /// </summary>
        public void Unhalt()
        {
            this.isDosed = true;
            if (parametros.HabilitarLogComunicacao)
            {
                Log.Logar(
                    TipoLog.Comunicacao, Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema, "Executa comando Unhalt.");
            }

            if (mb == null || !mb.isOpen())
            {
                throw new Exception("Not connected");
            }

            writeControl(CONTROL_IDLE);
        }

        /// <summary>
        /// Sets the pulse width of the the 'on' portion of the step signal, in microseconds.
        /// Must be >= 1. The default on the board 22 microseconds, which is probably fine.
        /// </summary>
        public void SetPulseWidth(ushort pw)
        {
            if (pw < 1)
                throw new Exception("PulseWidth must be >= 1");
            int[] v = new int[1];
            v[0] = (short)pw;
            if (!mb.SendFc16(slaveAddr, REG_PULSEWIDTH, 1, v))
            {
                throw new Exception("Could not write pulsewidth registers: " + mb.modbusStatusStr + Environment.NewLine + Percolore.IOConnect.Negocio.IdiomaResxExtensao.FalhaPortaSerial);
            }
        }

        /// <summary>
        /// This register can be used to invert the polarity of the step or
        /// enable signals of the drive signals. This may be necessary when
        /// using certain step motor drives. The default is that both step and
        /// enable are active high (as though you called 
        /// SetPolarity(false, false);).
        ///
        /// Direction can be inverted by simply using negative distance
        /// values or swapping the phases on the stepper motor.
        /// </summary>
        public void SetPolarity(bool stepActiveLow, bool enableActiveLow)
        {
            ushort val = 0;

            if (stepActiveLow)
                val |= 1;

            if (enableActiveLow)
                val |= 2;
            int[] v = new int[1];
            v[0] = (int)val;
            if (!mb.SendFc16(slaveAddr, REG_POLARITY, 1, v))
            {
                throw new Exception("Could not write polarity register: " + mb.modbusStatusStr + Environment.NewLine + Percolore.IOConnect.Negocio.IdiomaResxExtensao.FalhaPortaSerial);
            }
        }

        /// <summary>
        /// Sets the order that the motors run in. This array of 16 ushorts must contain each
        /// of the numbers from 0 to 15 inclusive exactly once. These are the motor numbers,
        /// such that the motors will move in the order they are specified in the given array.
        /// By default, the motor order is {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
        /// that is, the motors run in numerical order, starting with motor 1 and ending with
        /// motor 15.
        /// </summary>
        public void SetMotorOrder(ushort[] motorOrder)
        {
            byte[] test = new byte[24];

            for (int i = 0; i < 24; i++)
            {
                test[i] = 0;
            }

            foreach (ushort i in motorOrder)
            {
                if (i > 23)
                    throw new Exception("Invalid motor number in motor order array");

                test[i]++;
            }
            int[] v = new int[24];
            foreach (byte i in test)
            {
                if (i != 1)
                {
                    throw new Exception(
                        "Motor order array is missing a motor number or has a duplicate motor number");
                }
            }
            for (int i = 0; i < motorOrder.Length; i++)
            {
                v[i] = (int)motorOrder[i];
            }

            if (!mb.SendFc16(slaveAddr, (ushort)REG_MOTOR_ORDER, (ushort)motorOrder.Length, v))
            {
                throw new Exception(
                    "Could not write motor order: " + mb.modbusStatusStr + Environment.NewLine + Percolore.IOConnect.Negocio.IdiomaResxExtensao.FalhaPortaSerial);
            }
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

        public string GetVersion()
        {
            string retorno = string.Empty;

            if (!mb.isOpen())
            {
                Connect(this.timeoutResp);
            }
            int[] v = new int[3];
            ushort pollStart = (ushort)REG_VERSION;
            if (mb.isOpen())
            {
                if (mb.SendFc3(slaveAddr, pollStart, 3, ref v))
                {
                    retorno = v[0].ToString() + "." + v[1].ToString() + "." + v[2].ToString();
                }
            }

            return retorno;
        }

        public void RessetHard()
        {

            if (!mb.isOpen())
            {
                Connect(this.timeoutResp);
            }
            ushort m_start = (ushort)REG_RESSET_HARD;
            if (!mb.SendFc6(slaveAddr, m_start, 1))
            {
                throw new Exception("Could not read status register: " + mb.modbusStatusStr + Environment.NewLine + Percolore.IOConnect.Negocio.IdiomaResxExtensao.FalhaPortaSerial);
            }
        }

        public struct StatusValvulas
        {
            public StatusValvulas(bool in1, bool in2, bool in3, bool in4, bool in5, bool in6, bool in7, bool in8, bool in9, bool in10,
                bool in11, bool in12, bool in13, bool in14, bool in15, bool in16, bool in17, bool in18, bool in19, bool in20, bool in21,
                bool in22, bool in23, bool in24)
            {
                Input_1 = in1; Input_2 = in2; Input_3 = in3; Input_4 = in4; Input_5 = in5; Input_6 = in6; Input_7 = in7; Input_8 = in8; Input_9 = in9;
                Input_10 = in10; Input_11 = in11; Input_12 = in12; Input_13 = in13; Input_14 = in14; Input_15 = in15; Input_16 = in16;
                Input_17 = in17;
                Input_18 = in18;
                Input_19 = in19;
                Input_20 = in20;
                Input_21 = in21;
                Input_22 = in22;
                Input_23 = in23;
                Input_24 = in24;
            }

            public bool Input_1;
            public bool Input_2;
            public bool Input_3;
            public bool Input_4;
            public bool Input_5;
            public bool Input_6;
            public bool Input_7;
            public bool Input_8;
            public bool Input_9;
            public bool Input_10;
            public bool Input_11;
            public bool Input_12;
            public bool Input_13;
            public bool Input_14;
            public bool Input_15;
            public bool Input_16;
            public bool Input_17;
            public bool Input_18;
            public bool Input_19;
            public bool Input_20;
            public bool Input_21;
            public bool Input_22;
            public bool Input_23;
            public bool Input_24;
        }

        public StatusValvulas Status_Valvulas
        {
            get
            {
                if (parametros.HabilitarLogComunicacao)
                {
                    Log.Logar(
                        TipoLog.Comunicacao, Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema, "Inicia a leitura de V[alvulas.");
                }

                if (!mb.isOpen())
                {
                    Connect(this.timeoutResp);
                }
                int[] v = new int[2];
                ushort pollStart = (ushort)REG_STATUSVALVULA;
                if (mb.isOpen())
                {
                    mb.SendFc3(slaveAddr, pollStart, 2, ref v);
                    if (!mb.modbusStatus)
                    {
                        throw new Exception("Could not write satatus valve: " + mb.modbusStatusStr + Environment.NewLine + Percolore.IOConnect.Negocio.IdiomaResxExtensao.FalhaPortaSerial);
                    }
                }
                bool input01 = (v[0] & (0x0001)) != 0;
                bool input02 = (v[0] & (0x0002)) != 0;
                bool input03 = (v[0] & (0x0004)) != 0;
                bool input04 = (v[0] & (0x0008)) != 0;
                bool input05 = (v[0] & (0x0010)) != 0;
                bool input06 = (v[0] & (0x0020)) != 0;
                bool input07 = (v[0] & (0x0040)) != 0;
                bool input08 = (v[0] & (0x0080)) != 0;
                int nShift = v[0] >> 8;

                bool input09 = (nShift & (0x0001)) != 0;
                bool input10 = (nShift & (0x0002)) != 0;
                bool input11 = (nShift & (0x0004)) != 0;
                bool input12 = (nShift & (0x0008)) != 0;
                bool input13 = (nShift & (0x0010)) != 0;
                bool input14 = (nShift & (0x0020)) != 0;
                bool input15 = (nShift & (0x0040)) != 0;
                bool input16 = (nShift & (0x0080)) != 0;

                bool input17 = (v[1] & (0x0001)) != 0;
                bool input18 = (v[1] & (0x0002)) != 0;
                bool input19 = (v[1] & (0x0004)) != 0;
                bool input20 = (v[1] & (0x0008)) != 0;
                bool input21 = (v[1] & (0x0010)) != 0;
                bool input22 = (v[1] & (0x0020)) != 0;
                bool input23 = (v[1] & (0x0040)) != 0;
                bool input24 = (v[1] & (0x0080)) != 0;

                if (parametros.HabilitarLogComunicacao)
                {
                    Log.Logar(
                        TipoLog.Comunicacao,
                        Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema,
                        string.Format(
                            "Val 01 = {0}, Val 02 = {1}, Val 03 = {2}, Val 04 = {3}, Val 05 = {4}, Val 06 = {5}, Val 07 = {6}, Val 08 = {7}, " +
                            "Val 09 = {8}, Val 10 = {9}, Val 11 = {10}, Val 12 = {11}, Val 13 = {12}, Val 14 = {13}, Val 15 = {14}, Val 16 = {15}",
                            input01.ToString(), input02.ToString(), input03.ToString(), input04.ToString(),
                            input05.ToString(), input06.ToString(), input07.ToString(), input08.ToString(),
                            input09.ToString(), input10.ToString(), input11.ToString(), input12.ToString(),
                            input13.ToString(), input14.ToString(), input15.ToString(), input16.ToString())

                    );
                }

                return new StatusValvulas
                   (
                       input01,
                       input02,
                       input03,
                       input04,
                       input05,
                       input06,
                       input07,
                       input08,
                       input09,
                       input10,
                       input11,
                       input12,
                       input13,
                       input14,
                       input15,
                       input16,
                       input17,
                       input18,
                       input19,
                       input20,
                       input21,
                       input22,
                       input23,
                       input24
                   );
            }
        }

        public void AcionaValvulas(int nVal)
        {
            int[] pacoteWr = new int[2];
            ushort start = (ushort)REG_ACIONAVALVULA;

            pacoteWr[0] = (nVal & 0xFFFF);
            pacoteWr[1] = (nVal >> 16) & 0xFFFF;

            if (!mb.isOpen())
            {
                Connect(this.timeoutResp);
            }

            if (!mb.SendFc16(slaveAddr, start, 2, pacoteWr))
            {
                throw new Exception("Could not write action valve: " + mb.modbusStatusStr + Environment.NewLine + Percolore.IOConnect.Negocio.IdiomaResxExtensao.FalhaPortaSerial);
            }
        }
    }
}