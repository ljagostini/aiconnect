using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Negocio
{
    public interface InterfaceBalanca
    {
        double CargaMaximaBalanca_Gramas { get; set; }
        string _str_Serial { get; set; }
        int b_peso_balanca { get; set; }
        bool isTerminouRead { get; set; }
        double valorPeso { get; set; }
        int b_tara_Balanca { get; set; }
        double valorTara { get; set; }
        int tamanhoLeituraBalanca { get; set; }

        bool CloseSerial();
        void Start_Serial();
        void WriteSerialPort(byte[] arrWS);
        void GetResponse(ref byte[] response, bool isThrow = true);
        void SetValues(byte[] resp, bool isTara = false, bool isThrow = true);
    }
}
