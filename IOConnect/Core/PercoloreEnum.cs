using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Core
{
    public class PercoloreEnum
    {
        public enum Eventos
        {
            InicalizarSistema = 1,
            FecharSistema = 2,
            AlteradaCalibracao = 3,
            Purga = 4,
            PurgaIndividual = 5,
            Recircular = 6,
            InicializarCircuitos = 7,
            MonitoramentoCircuitos = 8,
            FormulaPersonalizada = 9,
            Abastecimento = 10,
            OnLine_MSP = 11,
            OffLine_MSP = 12,
            ALteradaConfiguacao = 13,
            FalhaComunicacaoPlaca = 14,
            NivelCanisterBaixo = 15,
            RessetPlaca = 16,
            MaquinaLigada = 17,
            MaquinaDesligada = 18,
            AlteradoProdutos = 19,
            DispensaProdutos = 20
        }

        public enum TipoDosagem
        {   
            Circuito = 1,
            Bases = 2,         
            Colorantes = 3
        }
    }
}
