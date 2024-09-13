using Percolore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Core
{
    public enum UnidadeMedida
    {
        [ResourceNameAtributte("Global_UnidadeMedida_Grama")]
        [ResourceNameComplementAtributte("Global_UnidadeMedida_Abreviacao_Grama")]
        Grama = 1,
        [ResourceNameAtributte("Global_UnidadeMedida_Mililitro")]
        [ResourceNameComplementAtributte("Global_UnidadeMedida_Abreviacao_Mililitro")]
        Mililitro = 2,
        [ResourceNameAtributte("Global_UnidadeMedida_Onca")]
        [ResourceNameComplementAtributte("Global_UnidadeMedida_Abreviacao_Onca")]
        Onca = 3,
        [ResourceNameAtributte("Global_UnidadeMedida_Shot")]
        Shot = 4,
        [ResourceNameAtributte("Global_UnidadeMedida_Fraction")]
        Fraction = 5
    }
}
