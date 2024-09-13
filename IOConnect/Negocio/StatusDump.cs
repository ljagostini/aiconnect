using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Negocio
{
    public class StatusDump
    {
        public string VersionIOConnect { get; set; }
        public string NumeroSerie { get; set; }
        public List<Util.ObjectCalibragem> listCalibragem { get; set; }
        public List<Util.ObjectColorante> listProdutos { get; set; }

        public StatusDump()
        {
            listCalibragem = new List<Util.ObjectCalibragem>();
            listProdutos = new List<Util.ObjectColorante>();

        }

    }
}
