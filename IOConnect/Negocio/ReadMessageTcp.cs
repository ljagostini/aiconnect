using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Negocio
{
    public class ReadMessageTcp
    {
        public byte Header;
        public int NumeroBytes;
        public string IMEI;
        public int TipoEvento;
        public byte Crc;
        public string message;
        public int Footer;

        public bool desmontarMessage(byte[] bMessage, int protocolo)
        {
            bool retorno = false;
            try
            {
                this.Header = bMessage[0];
                this.NumeroBytes = ((int)bMessage[1] << 8) + (int)bMessage[2];
                this.Crc = bMessage[3];
                this.IMEI = "";
                this.message = "";
                for (int i = 4; i < 24; i++)
                {
                    if (bMessage[i] == 0x00)
                    {
                        break;
                    }
                    this.IMEI += (char)bMessage[i];
                }
                if (protocolo == 0)
                {
                    this.TipoEvento = (int)bMessage[25];

                    for (int i = 26; i < this.NumeroBytes - 1; i++)
                    {
                        if (bMessage[i] == 0x00)
                        {
                            break;
                        }
                        this.message += (char)bMessage[i];
                    }
                }
                else
                {
                    this.TipoEvento = (int)bMessage[26];

                    for (int i = 27; i < this.NumeroBytes - 1; i++)
                    {
                        if (bMessage[i] == 0x00)
                        {
                            break;
                        }
                        this.message += (char)bMessage[i];
                    }
                }

                this.Footer = (int)bMessage[this.NumeroBytes - 1];

                if ((int)this.Crc != calculoCheckSum(bMessage))
                {
                    this.Header = 0x00;
                    this.NumeroBytes = 0;
                    this.Crc = 0x00;
                    this.IMEI = "";
                    this.TipoEvento = -1;
                    this.message = "";

                }
                else
                {
                    retorno = true;
                }
            }
            catch
            {
                this.Header = 0x00;
            }
            return retorno;
        }

        public int calculoCheckSum(byte[] array)
        {
            int retorno = 0;
            try
            {
                for (int i = 4; i < array.Length; i++)
                {
                    char b = (char)array[i];
                    int intVal = ((int)b) & 0xff;
                    retorno += intVal;
                    if (retorno > 255)
                    {
                        retorno -= 255;
                    }
                }
            }
            catch
            {
                // TODO: handle exception
            }
            return retorno;
        }
    }
}
