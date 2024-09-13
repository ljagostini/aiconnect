using Percolore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Negocio
{
    public class ListColoranteSeguidor
    {
        public int Circuito { get; set; }
        public int Qtd_Circuito { get; set; }
        public bool isbase { get; set; }
        public List<int> lCircuitoSeguidores { get; set; } = new List<int>();
        public ValoresVO myValoresPrincipal { get; set; } = new ValoresVO();
        public List<ListValoresVOCircuitos> myValores { get; set; } = new List<ListValoresVOCircuitos>();
    }

    public class ListValoresVOCircuitos
    {
        public int circuito { get; set; }
        public ValoresVO myValores { get; set; } = new ValoresVO();
    }
}
