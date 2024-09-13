using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Negocio
{
    public class OperacaoAutomatica
    {
        public int Motor { get; set; }
        public int IsPrimeiraCalibracao { get; set; }
        public double Volume { get; set; } 
        public double DesvioAdmissivel { get; set; }
        public int IsCalibracaoAutomatica { get; set; }
        public int NumeroMaxTentativa { get; set; }
        public int IsRealizarMediaMedicao { get; set; }
        public int NumeroDosagemTomadaMedia { get; set; }


    }
}
