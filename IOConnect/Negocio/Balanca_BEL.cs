using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Negocio
{
    public class Balanca_BEL: InterfaceBalanca
    {
        #region Serial Port
        public double CargaMaximaBalanca_Gramas { get; set; } = 400;
        private SerialPort sp = null;

        private bool isDataReceived = false;

        private byte[] readBytes = new byte[2048];
        private int inicio_reader = 0;
        private int tamanho_read = 30;
        public bool isTerminouRead { get; set; } = false;

        public int b_tara_Balanca { get; set; } = 84;
        public int b_peso_balanca { get; set; } = 69;
        public string _str_Serial { get; set; }
        private int count_TimerRec = 150;
        #endregion

        public string strUnit = "";
        public double valorPeso { get; set; } = 0;
        public double valorTara { get; set; } = 0;
        public int valorEstabilizado = 0;
        public int tamanhoLeituraBalanca { get; set; } = 15;

        public bool IsOpen { get; set; }

        public void Start_Serial()
        {
            try
            {
                CloseSerial();
                this.IsOpen = OpenSerial(_str_Serial, 9600, 8, Parity.None, StopBits.One);
            }
            catch
            {
                if (sp != null)
                {
                    this.IsOpen = sp.IsOpen;
                }
                else
                {
                    this.IsOpen = false;
                }
            }
        }

        public bool OpenSerial(string portName, int baudRate, int databits, Parity parity, StopBits stopBits)
        {
            if (sp == null)
            {
                sp = new SerialPort();
            }
            //Ensure port isn't already opened:
            if (!sp.IsOpen)
            {
                //Assign desired settings to the serial port:
                sp.PortName = portName;
                sp.BaudRate = baudRate;
                sp.DataBits = databits;
                sp.Parity = parity;
                sp.StopBits = stopBits;
                //These timeouts are default and cannot be editted through the class at this point:
                sp.ReadTimeout = 2000;
                sp.WriteTimeout = 2000;

                try
                {
                    sp.Open();
                    sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                    this.isDataReceived = true;
                }
                catch
                {
                    return false;
                }

                return true;
            }
            else
            {
                return true;
            }
        }

        public bool CloseSerial()
        {
            //Ensure port is opened before attempting to close:
            if (sp != null)
            {
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
                    catch
                    { }
                    
                    GC.Collect();
                    sp = null;

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
                    catch
                    { }
                    
                    GC.Collect();
                    string portname = sp.PortName;
                    sp = null;

                    this.isDataReceived = false;
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void WriteSerialPort(byte[] arrWS)
        {
            try
            {
                this.valorPeso = 0;
                this.isTerminouRead = false;
                this.inicio_reader = 0;
                this.sp.Write(arrWS, 0, arrWS.Length);
            }
            catch
            {
            }
        }

        public void WriteSerialPortTara(byte[] arrWS)
        {
            try
            {
                this.valorPeso = 0;
                this.isTerminouRead = false;
                this.inicio_reader = 0;
                this.sp.Write(arrWS, 0, arrWS.Length);
                Thread.Sleep(10000);
            }
            catch
            {
            }
        }

        private void DataReceivedHandler(
                      object sender,
                      SerialDataReceivedEventArgs e)
        {
            //string indata = sp.ReadExisting();
            Console.WriteLine("Data Received:");
            //Console.Write(indata);
            try
            {
                SerialPort sp2 = (SerialPort)sender;
                int tamanho = sp2.BytesToRead;
                sp2.Read(readBytes, inicio_reader, tamanho);
                inicio_reader += tamanho;
                if ((inicio_reader >= tamanho_read) || ((int)readBytes[inicio_reader - 2] == 13 && (int)readBytes[inicio_reader - 1] == 10))
                {
                    isTerminouRead = true;
                }
            }
            catch
            {

            }
        }

        public bool IsOpenSerial()
        {
            bool retorno = false;
            try
            {
                if (this.sp.IsOpen)
                {
                    retorno = true;
                }
            }
            catch
            {

            }
            return retorno;
        }

        #region Get Response
        public void GetResponse(ref byte[] response, bool isThrow = true)
        {
            //There is a bug in .Net 2.0 DataReceived Event that prevents people from using this
            //event as an interrupt to handle data (it doesn't fire all of the time).  Therefore
            //we have to use the ReadByte command for a fixed length as it's been shown to be reliable.
            for (int i = 0; i < count_TimerRec; i++)
            {
                if (!this.isTerminouRead)
                {
                    Thread.Sleep(100);
                }
            }
            
            if (!this.isTerminouRead && isThrow)
            {
                throw new Exception("Error timeout reader Balança Bel!");
            }
            if (this.isTerminouRead)
            {
                for (int i = 0; i < this.inicio_reader; i++)
                {
                    if (i < response.Length)
                    {
                        response[i] = readBytes[i];
                    }
                }
            }

        }
        #endregion

        public void SetValues(byte[] resp, bool isTara = false, bool isThrow = true)
        {
            try
            {
                bool temSinal = false;
                string caracteres_valor = "";
                if (resp != null && resp.Length >= 15 && (int)resp[0] > 0)
                {
                    if((int)resp[0] == 45)
                    {
                        temSinal = true;
                    }
                    for (int i = 1;  i < 9; i++)
                    {
                        int _car = (int)resp[i];
                        if((_car >= 48 && _car <=57) || _car==46 || _car == 44)
                        {
                            caracteres_valor += (char)_car;
                        }
                    }
                    double _vl;
                    if (double.TryParse(caracteres_valor, out _vl))
                    {
                        if (temSinal)
                        {
                            this.valorPeso = _vl * -1;
                        }
                        else
                        {
                            this.valorPeso = _vl;
                        }
                        if(isTara)
                        {
                            this.valorTara = this.valorPeso;
                        }
                    }
                    this.strUnit = "";
                    for (int i = 9; i < 12; i++)
                    {
                        int _car = (int)resp[i];
                        this.strUnit += (char)_car;
                    }
                    this.valorEstabilizado = (int)resp[12];
                }
                else
                {
                    if (isThrow)
                    {
                        throw new Exception("Balança Bel Error :" + " Erro Valores incorretos!");
                    }
                }
            }
            catch
            {
                throw new Exception("Balança Bel Error :" + " Erro de Comunicação!");
            }
        }
    }
}
