using Percolore.Core.Persistence.Xml;
using System.Collections.Generic;

namespace Percolore.IOConnect
{
    /// <summary>
    /// Value objet para transferência de informações da purga
    /// </summary>
    public class PurgaVO
    {
        /*
        public int[] PulsoHorario { get; set; } = new int[16];
        public int[] Velocidade { get; set; } = new int[16];
        public int[] Aceleracao { get; set; } = new int[16];
        public int[] Delay { get; set; } = new int[16];
        public int[] PulsoReverso { get; set; } = new int[16];
        public int[] Circuitos { get; set; } = new int[16];
        */

        //public double Volume { get; set; }
        public List<IDispenser> Dispenser { get; set; }
        public ModBusDispenserMover_P3 DispenserP3 { get; set; } = null;
        public List<Util.ObjectColorante> Colorantes { get; set; }
        public List<MDispositivos> LMDispositivos = new List<MDispositivos>();
        public int[] Dispositivo { get; set; }

        public class MDispositivos
        {
            public List<Util.ObjectColorante> Colorantes { get; set; }
            public int[] PulsoHorario { get; set; } = new int[16];
            public int[] Velocidade { get; set; } = new int[16];
            public int[] Aceleracao { get; set; } = new int[16];
            public int[] Delay { get; set; } = new int[16];
            public int[] PulsoReverso { get; set; } = new int[16];
            public double[] VolumeDosado { get; set; } = new double[16];

            public MDispositivos(bool isPlaca24Motor = false)
            {
                if(isPlaca24Motor)
                {
                    PulsoHorario = new int[24];
                    Velocidade = new int[24];
                    Aceleracao  = new int[24];
                    Delay  = new int[24];
                    PulsoReverso = new int[24];
                    VolumeDosado  = new double[24];
                }
            }

        }
    }
}
