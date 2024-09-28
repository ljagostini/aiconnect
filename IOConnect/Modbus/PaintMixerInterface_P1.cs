//1.1
using Percolore.Core.Logging;
using System.IO.Ports;
using System.Text.RegularExpressions;
using WSMBS;

namespace PaintMixer
{
	/// <summary>
	/// Basic wrapper around the S3MBPC1 Modbus interface for the new firmware
	/// </summary>
	/// <remarks>
	/// This is a basic wrapper around the Modbus interface to the new board firmware for 
	/// the S3MBPC1 (12 motor paint mixer control board). Like the interface for the 
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
	/// Run and RunReverse. Both functions take three int[12] arrays containing the
	/// distance each motor should move in steps, the velocity each motor should 
	/// reach (in steps/sec), and the rate of acceleration/deceleration (in multiples
	/// of 256, such that ActualAcceleration = AccelValue * 256) The actual acceleration is
	/// in units of steps/sec^2.
	/// 
	/// The Run method just moves each motor, whereas RunReverse includes an additional
	/// reverse move (to suck the last drop of paint back into the pump) specified by two
	/// parameters: the delay between the normal move and the reverse move, and the length
	/// of the reverse move in steps.
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

	public class PaintMixerInterface_P1 : IDisposable
    {

        #region Constantes

        private const int REG_STATUS = 0;
        private const int REG_CONTROL = 1;
        private const int REG_MOTOR_NUM = 2;
        private const int REG_MOTORS = 3;
        private const int REG_VEL = 27;
        private const int REG_ACC = 51;
        private const int REG_REVSTEPS = 75;
        private const int REG_REVDELAY = 77;
        private const int REG_COUNT = 78;
        private const int STATUS_BUSY = 1;
        private const int STATUS_MOVING = 2;
        private const int STATUS_REVING = 4;
        private const int STATUS_DELAYED = 8;
        private const int STATUS_HALTED = 16;
        private const int CONTROL_IDLE = 0;
        private const int CONTROL_RUN = 1;
        private const int CONTROL_RUNREV = 2;
        private const int CONTROL_HALT = 3;

        #endregion

        private WSMBSControl wsmbs;
        private byte slaveAddr;

        /// <summary>
        /// Constructs a new paint mixer interface, opening the COM port "COMp" where p is the
        /// value of port, and communicating over Modbus to a device with the given slave address
        /// </summary>
        /// <param name="slaveAddr">The slave address of the board</param>
        /// <param name="port">The COM port number</param>
        public PaintMixerInterface_P1(int slaveAddr)
        {
            if (slaveAddr <= 0 || slaveAddr > 255)
            {
                throw new Exception("Bad slave address; must be > 0 and < 255");
            }

            this.slaveAddr = (byte)slaveAddr;
        }

        public bool Connected
        {
            get
            {
                return false;               
            }
        }

        /// <summary>
        /// Opens the COM port to allow communication to occur. 
        /// Must be called before any other methods or properties.
        /// Throws an exception if the COM port could not be opened.
        /// </summary>
        public void Connect(int responseTimeout)
        {
            wsmbs = new WSMBSControl();
            wsmbs.LicenseKey("61D6-9D69-7F93-020C-3DD5-9E8E");
            wsmbs.Mode = WSMBSControl.MODE_ENUM.RTU;
            wsmbs.BaudRate = 9600;
            wsmbs.StopBits = WSMBSControl.STOPBITS_ENUM.TWO_STOP_BITS;
            wsmbs.Parity = WSMBSControl.PARITY_ENUM.NONE;
            wsmbs.ResponseTimeout = (ushort)responseTimeout;
            wsmbs.CommPort = 0;

            //Recupera portas da máquina onde há dispositivos ativos
            string[] portas = null;
            try
            {
                portas = SerialPort.GetPortNames();
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}

			bool bConnect = false; 
            if (portas == null || portas.LongLength == 0)
            {
                throw new Exception(
                    Percolore.IOConnect.Negocio.IdiomaResxExtensao.Global_DisensaNaoConectado);
                    //"O dispositivo de dispensa não encontra-se conectado à porta de comunicação.");
            }

            WSMBSControl.RESULT result = WSMBSControl.RESULT.ERR_NO_CONNECTION;
            foreach (string porta in portas)
            {
                try
                {
                    //Extrai apenas os número da porta
                    Match match = Regex.Match(porta, @"[0-9]");
                    if (match.Success)
                    {
                        wsmbs.CommPort = byte.Parse(match.Value);
                    }

                    //Inicia comunicação com porta encontrada
                    result = wsmbs.Connect();

                    if (result == WSMBSControl.RESULT.SUCCESS)
                    {
                        bConnect = true;
                        ushort[] v = new ushort[1];

                        /* Efetua leitura de registrador para certificar-se que a porta com a qual
                         * foi estabeleciada a conexão, é realmente a porta onde o dispositivo 
                         * está conectado */
                        result = wsmbs.ReadHoldingRegisters(slaveAddr, REG_STATUS, 1, ref v);
                        if (result == WSMBSControl.RESULT.SUCCESS)
                        {
                            wsmbs.IsAccessible = true;
                            break;
                        }
                        else
                        {
                            //Se não for possível ler status, encerra comunicação com a porta
                            wsmbs.Disconnect();
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
				catch (Exception e)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
				}
			}
            
            if (!bConnect)
            {
                throw new Exception("Error failure communications with serial port!");
            }
            else if (result != WSMBSControl.RESULT.SUCCESS)
            {
                throw new Exception(wsmbs.GetLastErrorString());
            }
        }

        /// <summary>
        /// Close the COM port and consequently, disconnect from the paint mixer board.
        /// </summary>
        public void Disconnect()
        {
            if (wsmbs != null)
            {
                wsmbs.Disconnect();
            }

            wsmbs = null;
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
                ushort[] v = new ushort[1];

                if (wsmbs.ReadHoldingRegisters(slaveAddr, REG_STATUS, 1, ref v) != WSMBSControl.RESULT.SUCCESS)
                {
                    throw new Exception("Could not read status register: " + wsmbs.GetLastErrorString());
                }

                return new StatusValue(
                    (v[0] & STATUS_BUSY) != 0,
                    (v[0] & STATUS_MOVING) != 0,
                    (v[0] & STATUS_REVING) != 0,
                    (v[0] & STATUS_DELAYED) != 0,
                    (v[0] & STATUS_HALTED) != 0);
            }
        }

        /// <summary>
        /// Polls the board for the value of the CurrentMotor register and returns it. While a move
        /// is occuring, this indicates which motor is currently active.
        /// </summary>
        public int CurrentMotor
        {
            get
            {
                if (wsmbs == null)
                {
                    throw new Exception("Not connected");
                }

                ushort[] v = new ushort[1];

                if (wsmbs.ReadHoldingRegisters(slaveAddr, REG_MOTOR_NUM, 1, ref v) != WSMBSControl.RESULT.SUCCESS)
                {
                    throw new Exception("Could not read motor number register: " + wsmbs.GetLastErrorString());
                }

                return (int)v[0];
            }
        }

        /// <summary>
        /// Performs a motion command, moving all 12 motors the amounts specified in the distances array,
        /// using the velocities and accelerations from the following two arrays.
        /// </summary>
        /// <remarks>
        /// This command performs the dispensing cycle without any reverse drip moves.
        /// In order to perform this command, the Busy flag must currently be clear, indicating that there
        /// is no command/motion currently occuring. Also, the halted flag must be clear, indicating that
        /// the board has not been halted.
        /// 
        /// The command requires three arrays of 12 ints, specifying the move distance (steps), velocity,
        /// and acceleration for each of the 12 motors.
        /// </remarks>
        /// <param name="distances">12 ints, each denoting move distance in steps. Can be positive or negative</param>
        /// <param name="velocities">12 ints, each denoting velocity in steps/sec. Must be positive.</param>
        /// <param name="accelerations">12 ints, each denoting acceleration in multiples of 256 steps/sec^2.
        /// That is, the actual acceleration is equal to 256 times the value of this parameter.</param>
        public void Run(int[] distances, int[] velocities, int[] accelerations)
        {
            if (wsmbs == null || !wsmbs.IsAccessible)
            {
                throw new Exception("Not connected");
            }

            if (distances.Length < 12)
                throw new Exception("Less than 12 distances provided. Cannot run motors");

            if (velocities.Length < 12)
                throw new Exception("Less than 12 velocities provided. Cannot run motors");

            if (accelerations.Length < 12)
                throw new Exception("Less than 12 accelerations provided. Cannot run motors");

            writeData(distances, velocities, accelerations, 0, 0);
            writeControl(CONTROL_RUN);
            while (!Status.Busy)
            {
                Thread.Sleep(1);
            }

            writeControl(CONTROL_IDLE);
        }

        /// <summary>
        /// Performs a motion command, moving all 12 motors the amounts specified in the distances array,
        /// using the velocities and accelerations from the following two arrays. Each dispensing move on each
        /// motor will be followed by a reverse move to suck the last drop of paint back in. This move is
        /// revSteps in length and occurs revDelayMs milliseconds after the dispense move.
        /// </summary>
        /// <remarks>
        /// This command performs the dispensing cycle without any reverse drip moves.
        /// In order to perform this command, the Busy flag must currently be clear, indicating that there
        /// is no command/motion currently occuring. Also, the halted flag must be clear, indicating that
        /// the board has not been halted.
        /// 
        /// The command requires three arrays of 12 ints, specifying the move distance (steps), velocity,
        /// and acceleration for each of the 12 motors.
        /// </remarks>
        /// <param name="distances">12 ints, each denoting move distance in steps. Can be positive or negative</param>
        /// <param name="velocities">12 ints, each denoting velocity in steps/sec. Must be positive.</param>
        /// <param name="accelerations">12 ints, each denoting acceleration in multiples of 256 steps/sec^2.
        /// That is, the actual acceleration is equal to 256 times the value of this parameter.</param>
        /// <param name="revDelayMs">Delay between the dispense move and the reverse move in milliseconds</param>
        /// <param name="revSteps">Length of the reverse move in steps.</param>
        public void RunReverse(int[] distances, int[] velocities, int[] accelerations, int revDelayMs, int revSteps)
        {

            if (wsmbs == null || !wsmbs.IsAccessible)
                throw new Exception("Not connected");

            if (distances.Length < 12)
                throw new Exception("Less than 12 distances provided. Cannot run motors");

            if (velocities.Length < 12)
                throw new Exception("Less than 12 velocities provided. Cannot run motors");

            if (accelerations.Length < 12)
                throw new Exception("Less than 12 accelerations provided. Cannot run motors");

            writeData(
                distances, velocities, accelerations, (ushort)revDelayMs, revSteps);

            writeControl(CONTROL_RUNREV);

            while (!Status.Busy)
            {
                Thread.Sleep(1);
            }

            writeControl(CONTROL_IDLE);
        }

        private void writeControl(ushort val)
        {
            if (wsmbs.WriteSingleRegister(slaveAddr, REG_CONTROL, val) != WSMBSControl.RESULT.SUCCESS)
            {
                throw new Exception(
                    "Could not write control register: " + wsmbs.GetLastErrorString());
            }
        }

        private void writeData(int[] dist, int[] vel, int[] acc, ushort dly, int revdist)
        {
            ushort[] dreg = new ushort[24];
            ushort[] vreg = new ushort[24];
            ushort[] areg = new ushort[24];
            ushort[] revreg = new ushort[2];
            int[] r = new int[1] { revdist };

            wsmbs.Int32ToRegisters(12, ref dist, ref dreg, WSMBSControl.FORMAT.B4B3B2B1);
            wsmbs.Int32ToRegisters(12, ref vel, ref vreg, WSMBSControl.FORMAT.B4B3B2B1);
            wsmbs.Int32ToRegisters(12, ref acc, ref areg, WSMBSControl.FORMAT.B4B3B2B1);
            wsmbs.Int32ToRegisters(1, ref r, ref revreg, WSMBSControl.FORMAT.B4B3B2B1);

            List<ushort> allData = new List<ushort>();
            allData.AddRange(dreg); allData.AddRange(vreg);
            allData.AddRange(areg); allData.AddRange(revreg);
            allData.Add(dly);
            ushort[] all = allData.ToArray();

            if (wsmbs.WriteMultipleRegisters(slaveAddr, (ushort)REG_MOTORS, (ushort)all.Length, ref all) != WSMBSControl.RESULT.SUCCESS)
            {
                throw new Exception("Could not write movement data: " + wsmbs.GetLastErrorString());
            }
        }

        /// <summary>
        /// Halts the board, abruptly stopping all motion until the halt is cleared.
        /// </summary>
        public void Halt()
        {
            if (wsmbs == null)
                throw new Exception("Not connected");

            writeControl(CONTROL_HALT);
        }

        /// <summary>
        /// Clears a halt, returning the board to an idle state.
        /// </summary>
        public void Unhalt()
        {
            if (wsmbs == null)
                throw new Exception("Not connected");

            writeControl(CONTROL_IDLE);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                wsmbs.Disconnect();
                wsmbs.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}