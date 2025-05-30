﻿using Percolore.Core.Logging;
using System.Diagnostics;
using System.IO.Ports;
using System.Reflection;

namespace Percolore.IOConnect
{
	class ModBusRtu
    {
        private SerialPort sp = null;
        public string modbusStatusStr;
        public bool modbusStatus;

        private byte[] readBytes = new byte[2048];
        private int inicio_reader = 0;
        private int tamanho_read = 0;
        private bool isTerminouRead = false;
        public static ModBusRtu mdb = null;
        private bool isDataReceived = false;


        #region Constructor / Deconstructor
        
        public ModBusRtu()
        {
            mdb = this;
        }

        ~ModBusRtu()
        {
            
        }
       
        #endregion
        public static ModBusRtu getModBusRtu()
        {
            return mdb;
        }

        #region Open / Close Procedures

        public bool Open(string portName, int baudRate, int databits, Parity parity, StopBits stopBits, int timeoutConexao = 2000)
        {
            // O timeout para conexão não deve ser menor do que meio segundo
            if (timeoutConexao < 500)
            {
                timeoutConexao = 500;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            LogManager.LogInformation("[ModBusRtu.Open] Started.");

            // Se não há conexão configurada, cria uma nova conexão
            if (sp == null)
            {
                sp = new SerialPort();
            }

            // Garante que a porta serial não está aberta
            if (!sp.IsOpen)
            {
                // Atribuição de parâmetros da porta serial
                sp.PortName = portName;
                sp.BaudRate = baudRate;
                sp.DataBits = databits;
                sp.Parity = parity;
                sp.StopBits = stopBits;

                // Timeouts de leitura e escrita, não usados na abertura da porta serial
                sp.ReadTimeout = 2000;
                sp.WriteTimeout = 2000;

                // Tenta abrir a conexão com a porta serial
                try
                {
                    sp.Open();
                    sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                    this.isDataReceived = true;

                    modbusStatusStr = portName + " opened successfully";
                    modbusStatus = true;
                }
                // Em caso de erro, exibe mensagem de erro e retorna falso
                catch (Exception err)
                {
                    modbusStatusStr = "Error opening " + portName + ": " + err.Message;
                    LogManager.LogError($"[ModBusRtu.Open] {modbusStatusStr}", err);
                    modbusStatus = false;
                }
            }
            // Se a porta serial já está aberta, atualiza status e retorna verdadeiro
            else
            {
                modbusStatusStr = portName + " already opened";
                modbusStatus = true;
            }

            sw.Stop();
            LogManager.LogInformation($"[ModBusRtu.Open] Finished. Elapsed time: {sw.ElapsedMilliseconds} ms.");
            return modbusStatus;
        }

        public bool CloseM()
        {
            // Ensure port is opened before attempting to close:
            if (sp != null)
            {
                SafeClose(sp);
                if (sp.IsOpen)
                {
                    string portname = sp.PortName;

                    try
                    {
                        if (this.isDataReceived)
                        {
                            sp.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
                        }

						sp.Close();
						sp.Dispose();
					}
					catch (Exception e)
					{
						LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
					}

					GC.Collect();
                    sp = null;
                    modbusStatusStr = portname + " closed successfully";
                    modbusStatus = true;
                    this.isDataReceived = false;
                    return true;
                }
                else
                {
                    try
                    {
                        if (this.isDataReceived)
                        {
                            sp.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
                        }

						sp.Close();
						sp.Dispose();
					}
					catch (Exception e)
					{
						LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
					}

					GC.Collect();
                    string portname = sp.PortName;
                    sp = null;
                    modbusStatusStr = portname + " is not open";
                    modbusStatus = false;
                    this.isDataReceived = false;
                    return false;
                }
            }
            else
            {
                modbusStatusStr = " is not open";
                modbusStatus = false;
                return false;
            }
        }

        public void SafeClose(SerialPort port)
        {
            if (port is null)
                return;

			Stopwatch sw = new Stopwatch();
			sw.Start();
			LogManager.LogInformation("[ModBusRtu.SafeClose] Started.");

			try
            {
                Stream internalSerialStream = (Stream)port.GetType()
                    .GetField("internalSerialStream", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(port);

                GC.SuppressFinalize(port);

                if (internalSerialStream != null)
                {
                    GC.SuppressFinalize(internalSerialStream);
                    internalSerialStream.Close();
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}

			sw.Stop();
			LogManager.LogInformation($"[ModBusRtu.SafeClose] Finished. Elapsed time: {sw.ElapsedMilliseconds} ms.");
		}

        static void ShutdownEventLoopHandler(Stream internalSerialStream)
        {
            try
            {
                FieldInfo eventRunnerField = internalSerialStream.GetType()
                    .GetField("eventRunner", BindingFlags.NonPublic | BindingFlags.Instance);

                if (eventRunnerField != null)
                {
                    object eventRunner = eventRunnerField.GetValue(internalSerialStream);
                    Type eventRunnerType = eventRunner.GetType();

                    FieldInfo endEventLoopFieldInfo = eventRunnerType.GetField(
                        "endEventLoop", BindingFlags.Instance | BindingFlags.NonPublic);

                    FieldInfo eventLoopEndedSignalFieldInfo = eventRunnerType.GetField(
                        "eventLoopEndedSignal", BindingFlags.Instance | BindingFlags.NonPublic);

                    FieldInfo waitCommEventWaitHandleFieldInfo = eventRunnerType.GetField(
                        "waitCommEventWaitHandle", BindingFlags.Instance | BindingFlags.NonPublic);

                    if (endEventLoopFieldInfo == null
                       || eventLoopEndedSignalFieldInfo == null
                       || waitCommEventWaitHandleFieldInfo == null)
                    {
                        /*
                        logger.Warning("serial-port-debug",
                            "Unable to find the EventLoopRunner internal wait handle or loop signal fields. "
                            + "SerialPort workaround failure. Application may crash after "
                            + "disposing SerialPort unless .NET 1.1 unhandled exception "
                            + "policy is enabled from the application's config file.");
                            */
                    }
                    else
                    {
                        var eventLoopEndedWaitHandle =
                            (WaitHandle)eventLoopEndedSignalFieldInfo.GetValue(eventRunner);
                        var waitCommEventWaitHandle =
                            (ManualResetEvent)waitCommEventWaitHandleFieldInfo.GetValue(eventRunner);

                        endEventLoopFieldInfo.SetValue(eventRunner, true);

                        // Sometimes the event loop handler resets the wait handle
                        // before exiting the loop and hangs (in case of USB disconnect)
                        // In case it takes too long, brute-force it out of its wait by
                        // setting the handle again.
                        int i = 0;
                        do
                        {
                            waitCommEventWaitHandle.Set();
                            if(i>5)
                            {
                                break;
                            }
                            i++;
                        } while (!eventLoopEndedWaitHandle.WaitOne(2000));
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ModBusRtu).Name}: ", e);
			}
		}

        #endregion

        public bool isOpen()
        {
            bool retorno = false;
            try
            {
                if (this.sp != null)
                {
                    retorno = this.sp.IsOpen;
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}

			return retorno;
        }

        private void LogSerialEvt(byte[] message, string tipoM, int tamanho)
        {
            try
            {
                Util.LogSerial lSer = new Util.LogSerial();
                lSer.dtHora = DateTime.Now;
                lSer.tipoMessage = tipoM;
                lSer.message = "";
                for(int i = 0; message != null && i < message.Length && i < tamanho; i++)
                {
                    if(lSer.message.Length == 0)
                    {
                        lSer.message = "" + (int)message[i];
                    }
                    else
                    {
                        lSer.message += "-" + (int)message[i];
                    }
                    
                }
                Percolore.IOConnect.Modbus.Constantes.listLogSerial.Add(lSer);
                if(Percolore.IOConnect.Modbus.Constantes.listLogSerial.Count > Percolore.IOConnect.Modbus.Constantes.qtdLogSerial)
                {
                    Percolore.IOConnect.Modbus.Constantes.listLogSerial.RemoveAt(0);
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}
		}

        private void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("Data Received:");
            
            try
            {
                SerialPort sp2 = (SerialPort)sender;
                int tamanho = sp2.BytesToRead;
                sp2.Read(readBytes, inicio_reader, tamanho);
                inicio_reader += tamanho;
                if (inicio_reader >= tamanho_read)
                {
                    isTerminouRead = true;
                    LogSerialEvt(readBytes, "Read", tamanho_read);
                }
            }
			catch (Exception err)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", err);
			}
		}

        #region CRC Computation
        private void GetCRC(byte[] message, ref byte[] CRC)
        {
            //Function expects a modbus message of any length as well as a 2 byte CRC array in which to 
            //return the CRC values:

            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }
        #endregion

        #region Build Message
        private void BuildMessage(byte address, byte type, ushort start, ushort registers, ref byte[] message)
        {
            //Array to receive CRC bytes:
            byte[] CRC = new byte[2];

            message[0] = address;
            message[1] = type;
            message[2] = (byte)(start >> 8);
            message[3] = (byte)start;
            message[4] = (byte)(registers >> 8);
            message[5] = (byte)registers;

            GetCRC(message, ref CRC);
            message[message.Length - 2] = CRC[0];
            message[message.Length - 1] = CRC[1];
        }
        #endregion

        #region Check Response
        private bool CheckResponse(byte[] response)
        {
            //Perform a basic CRC check:
            byte[] CRC = new byte[2];
            GetCRC(response, ref CRC);
            if (CRC[0] == response[response.Length - 2] && CRC[1] == response[response.Length - 1])
                return true;
            else
                return false;
        }
        #endregion

        #region Get Response
        private void GetResponse(ref byte[] response)
        {
            //There is a bug in .Net 2.0 DataReceived Event that prevents people from using this
            //event as an interrupt to handle data (it doesn't fire all of the time).  Therefore
            //we have to use the ReadByte command for a fixed length as it's been shown to be reliable.
            for (int i = 0; Modbus.USBConstant.connectUsb && i < 300; i++)
            {
                if (!this.isTerminouRead)
                {
                    Thread.Sleep(10);
                }
            }
            if (!this.isTerminouRead)
            {
                throw new Exception("Error timeout reader");
            }

            for (int i = 0; i < this.inicio_reader; i++)
            {
                response[i] = readBytes[i];
            }

        }
        #endregion

        #region Function 16 - Write Multiple Registers
        public bool SendFc16(byte address, ushort start, ushort registers, int[] values)
        {
            //Ensure port is open:
            if (sp.IsOpen && Modbus.USBConstant.connectUsb)
            {
                //Clear in/out buffers:
                try
                {
                    sp.DiscardOutBuffer();
                    sp.DiscardInBuffer();
                    Thread.Sleep(10);
                }
				catch (Exception e)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
				}

				readBytes = new byte[2048];
                //Message is 1 addr + 1 fcn + 2 start + 2 reg + 1 count + 2 * reg vals + 2 CRC
                byte[] message = new byte[9 + (2 * registers)];
                //Function 16 response is fixed at 8 bytes
                byte[] response = new byte[8];

                //Add bytecount to message:
                message[6] = (byte)(registers * 2);
                //Put write values into message prior to sending:
                for (int i = 0; i < registers; i++)
                {                   
                    message[7 + (2 * i)] = (byte)(values[i] >> 8);
                    message[8 + (2 * i)] = (byte)(values[i]);
                }
                //Build outgoing message:
                BuildMessage(address, (byte)16, start, registers, ref message);

                //Send Modbus message to Serial Port:
                try
                {
                    this.isTerminouRead = false;
                    this.inicio_reader = 0;
                    this.tamanho_read = 8;
                    
                    sp.Write(message, 0, message.Length);
                    LogSerialEvt(message, "Write", message.Length);
                    Thread.Sleep(20);
                    sp.BaseStream.Flush();
                    GetResponse(ref response);
                }
                catch (Exception err)
                {
                    modbusStatusStr = "Error in write event: " + err.Message;
                    LogManager.LogError(modbusStatusStr, err);
                    modbusStatus = false;
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    modbusStatusStr = "Write successful";
                    modbusStatus = true;
                    return true;
                }
                else
                {
                    modbusStatusStr = "CRC error";
                    modbusStatus = false;
                    return false;
                }
            }
            else
            {
                modbusStatusStr = "Write multiple registers failed. Serial port not open";
                modbusStatus = false;
                return false;
            }
        }
        #endregion

        #region Function 6 - Write Presset Registers
        public bool SendFc6(byte address, ushort start, int values)
        {
            //Ensure port is open:
            if (sp.IsOpen && Modbus.USBConstant.connectUsb)
            {
                //Clear in/out buffers:
                try
                {
                    sp.DiscardOutBuffer();
                    sp.DiscardInBuffer();
                    Thread.Sleep(10);
                }
				catch (Exception ex)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
				}

				readBytes = new byte[2048];
                //Message is 1 addr + 1 fcn + 2 start + 2 reg + 1 count + 2 * reg vals + 2 CRC
                byte[] message = new byte[8];
                //Function 16 response is fixed at 8 bytes
                byte[] response = new byte[8];
                message[0] = address;
                message[1] = (byte)6;

                message[2] = (byte)(start >> 8); 
                message[3] = (byte)start;

                message[4] = (byte)(values >> 8);
                message[5] = (byte)values;


                byte[] CRC = new byte[2];
                GetCRC(message, ref CRC);
                message[message.Length - 2] = CRC[0];
                message[message.Length - 1] = CRC[1];

                //Send Modbus message to Serial Port:
                try
                {
                    this.isTerminouRead = false;
                    this.inicio_reader = 0;
                    this.tamanho_read = 8;
                    
                    sp.Write(message, 0, message.Length);
                    LogSerialEvt(message, "Write", message.Length);
                    Thread.Sleep(20);
                    sp.BaseStream.Flush();
                    GetResponse(ref response);
                }
                catch (Exception err)
                {
                    modbusStatusStr = "Error in write event: " + err.Message;
                    LogManager.LogError(modbusStatusStr, err);
                    modbusStatus = false;
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    modbusStatusStr = "Write successful";
                    modbusStatus = true;
                    return true;
                }
                else
                {
                    modbusStatusStr = "CRC error";
                    modbusStatus = false;
                    return false;
                }
            }
            else
            {
                modbusStatusStr = "Write preset registers failed. Serial port not open";
                modbusStatus = false;
                return false;
            }
        }
        #endregion
        
        #region Function 3 - Read Registers
        public bool SendFc3(byte address, ushort start, ushort registers, ref int[] values)
        {
			Stopwatch sw = new Stopwatch();
			sw.Start();
			LogManager.LogInformation("[moduleName] Started.");

			//Ensure port is open:
			if (sp.IsOpen && Modbus.USBConstant.connectUsb)
            {
                //Clear in/out buffers:
                try
                {
                    sp.DiscardOutBuffer();
                    sp.DiscardInBuffer();
                    Thread.Sleep(10);
                }
				catch (Exception e)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
				}

				readBytes = new byte[2048];
                //Function 3 request is always 8 bytes:
                byte[] message = new byte[8];
                //Function 3 response buffer:
                byte[] response = new byte[5 + 2 * registers];
                //Build outgoing modbus message:
                BuildMessage(address, (byte)3, start, registers, ref message);
                //Send modbus message to Serial Port:
                try
                {
                    this.isTerminouRead = false;
                    this.inicio_reader = 0;
                    this.tamanho_read = response.Length;
                    
                    sp.Write(message, 0, message.Length);
                    LogSerialEvt(message, "Write", message.Length);
                    Thread.Sleep(20);
                    sp.BaseStream.Flush();                    
                    GetResponse(ref response);
                }
                catch (Exception err)
                {
                    modbusStatusStr = "Error in read event: " + err.Message;
                    LogManager.LogError(modbusStatusStr, err);
                    modbusStatus = false;

					sw.Stop();
					LogManager.LogInformation($"[ModBusRtu.SendFc3] Finished. Elapsed time: {sw.ElapsedMilliseconds} ms.");
					return false;
                }

                //Evaluate message:
                if (CheckResponse(response))
                {
                    //Return requested register values:
                    for (int i = 0; i < (response.Length - 5) / 2; i++)
                    {
                        values[i] = response[(2 * i) + 3];
                        values[i] <<= 8;
                        values[i] += response[(2 * i) + 4];
                    }
                    modbusStatusStr = "Read successful";
                    modbusStatus = true;

					sw.Stop();
					LogManager.LogInformation($"[ModBusRtu.SendFc3] Finished. Elapsed time: {sw.ElapsedMilliseconds} ms.");
					return true;
                }
                else
                {
                    modbusStatusStr = "CRC error";
                    modbusStatus = false;

					sw.Stop();
					LogManager.LogInformation($"[ModBusRtu.SendFc3] Finished. Elapsed time: {sw.ElapsedMilliseconds} ms.");
					return false;
                }
            }
            else
            {
                modbusStatusStr = "Read registers failed. Serial port not open";
                modbusStatus = true;

				sw.Stop();
				LogManager.LogInformation($"[ModBusRtu.SendFc3] Finished. Elapsed time: {sw.ElapsedMilliseconds} ms.");
				return false;
            }
        }
        #endregion
    }
}