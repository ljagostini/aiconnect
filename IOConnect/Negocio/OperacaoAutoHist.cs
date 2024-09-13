using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Negocio
{
    public class OperacaoAutoHist
    {
        public int Motor { get; set; }
        public int Etapa { get; set; }
        public int Etapa_Tentativa { get; set; }
        public double Volume { get; set; }
        public string Volume_str { get; set; }
        public double MassaIdeal { get; set; }
        public string MassaIdeal_str { get; set; }
        public double MassaMedBalanca { get; set; }
        public string MassaMedBalanca_str { get; set; }
        public string MassaMedBalancaMedia_str { get; set; }
        public double VolumeDosado { get; set; }
        public string VolumeDosado_str { get; set; }
        public string VolumeDosadoMedia_str { get; set; }
        public double Desvio { get; set; }
        public string Desvio_str { get; set; }
        public double Desvio_Med { get; set; }
        public string Desvio_Med_str { get; set; }
        public int Aprovado { get; set; }
        public int Executado { get; set; }
    }
}
