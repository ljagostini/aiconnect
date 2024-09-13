using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Negocio
{
    public class MyModel
    {
        public string Output { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public Object ObjetoEntrada { get; set; }
        public List<Object> ListObjetoSaida;
        public List<string> listValuesEntrada = new List<string>();

    }
}
