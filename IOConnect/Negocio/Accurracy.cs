using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Negocio
{
    public class Accurracy
    {
        public double VolumeRecipiente { get; set; }
        public int Circuito { get; set; }
        public int ModeloBalanca { get; set; }
        public string SerialPortBal { get; set; }
        public int DelaySegBalanca { get; set; }
        public double MinMassaAdmRecipiente { get; set; }
        public string MessageRetorno { get; set; }
        public string NumeroSerie { get; set; }
        public List<Negocio.Precisao> listPrecisao { get; set; }

    }
}
