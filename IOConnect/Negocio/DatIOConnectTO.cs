using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Negocio
{
    public class DatIOConnectTO
    {
        public int Id { get; set; }
        public string NomeDat { get; set; }
        public string ValoresDat { get; set; }
        public string Executado { get; set; }
        public DateTime DATA_CADASTRO { get; set; }
    }
}
