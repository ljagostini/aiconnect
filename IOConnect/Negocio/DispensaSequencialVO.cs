using Percolore.Core.Persistence.Xml;
using System.Collections.Generic;

namespace Percolore.IOConnect
{
    /// <summary>
    /// Value objet para transferência de informações de dispensa sequencial
    /// </summary>
    public class DispensaSequencialVO
    {
        public int[] PulsoHorario { get; set; } = new int[16];
        public int[] Velocidade { get; set; } = new int[16];
        public int[] Aceleracao { get; set; } = new int[16];
        public int[] Delay { get; set; } = new int[16];
        public int[] PulsoReverso { get; set; } = new int[16];
        public double[] Volume { get; set; } = new double[16];

        public int[] PulsoHorario2 { get; set; } = new int[16];
        public int[] Velocidade2 { get; set; } = new int[16];
        public int[] Aceleracao2 { get; set; } = new int[16];
        public int[] Delay2 { get; set; } = new int[16];
        public int[] PulsoReverso2 { get; set; } = new int[16];
        public double[] Volume2 { get; set; } = new double[16];

        public List<IDispenser> Dispenser { get; set; }
        public ModBusDispenserMover_P3 modBusDispenser_P3 { get; set; } = null;
        public List<Util.ObjectColorante> Colorantes { get; set; } = new List<Util.ObjectColorante>();
        public Dictionary<int, double> Demanda { get; set; } = new Dictionary<int, double>();
        public string DescricaoCor { get; set; }
        public string CodigoCor { get; set; }

        public DispensaSequencialVO(bool is24MotoresP1 = false, bool is24MotoresP2 = false)
        {
            if(is24MotoresP1)
            {
                PulsoHorario  = new int[24];
        
                Velocidade  = new int[24];
                Aceleracao  = new int[24];
                Delay  = new int[24];
                PulsoReverso  = new int[24];
                Volume = new double[24];
                
            }
            if(is24MotoresP2)
            {
                PulsoHorario2 = new int[24];
                Velocidade2 = new int[24];
                Aceleracao2 = new int[24];
                Delay2 = new int[24];
                PulsoReverso2 = new int[24];
                Volume2 = new double[24];
            }
        }

    }
}
