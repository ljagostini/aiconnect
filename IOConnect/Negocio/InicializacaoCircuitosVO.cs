using System.Collections.Generic;

namespace Percolore.IOConnect
{
    /// <summary>
    /// Value objet para transferência de referentes à inicialização de circuitos.
    /// </summary>

    public class InicializacaoCircuitosVO
    {
        public int PulsoInicial { get; set; }
        public int PulsoLimite { get; set; }
        public int VariacaoPulso { get; set; }
        public double StepVariacao { get; set; }
        public int Velocidade { get; set; }
        public int Aceleracao { get; set; }
        public int Delay { get; set; }
        public bool MovimentoInicialReverso { get; set; }
        public int QtdeCircuitoGrupo { get; set; }

        public List<IDispenser> Dispenser { get; set; }
        public ModBusDispenserMover_P3 DispenserP3 { get; set; } = null;
        public int[] Circuitos { get; set; }
        public int[] Dispositivo { get; set; }
    }
}
