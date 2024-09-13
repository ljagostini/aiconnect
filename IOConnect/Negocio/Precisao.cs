using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Negocio
{
    public class Precisao
    {
        public double volume { get; set; }
        public int tentativas { get; set; }
        public double volumeDos { get; set; }
        public string volumeDos_str { get; set; }
        public int executado { get; set; }
    }
}
