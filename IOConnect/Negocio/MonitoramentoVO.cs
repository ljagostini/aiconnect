using Percolore.Core.Persistence.Xml;
using System.Collections.Generic;

namespace Percolore.IOConnect
{
    public class MonitoramentoVO
    {
        public List<Util.ObjectColorante> Colorantes { get; set; }
        public List<MDispositivos> LMDispositivos = new List<MDispositivos>();
        public double Volume { get; set; }
        public List<IDispenser> Dispenser { get; set; }       
        public int[] Dispositivo { get; set; }

        public class MDispositivos
        {
            public List<Util.ObjectColorante> Colorantes { get; set; }
            public int[] PulsoHorario { get; set; } = new int[16];
            public int[] Velocidade { get; set; } = new int[16];
            public int[] Aceleracao { get; set; } = new int[16];
            public int[] Delay { get; set; } = new int[16];
            public int[] PulsoReverso { get; set; } = new int[16];

            public MDispositivos(bool isMotor24 = false)
            {
                if(isMotor24)
                {
                    PulsoHorario = new int[24];
                    Velocidade = new int[24];
                    Aceleracao = new int[24];
                    Delay = new int[24];
                    PulsoReverso = new int[24];
                }

            }


        }

    }
    class Constants
    {
        public static int countTimerDelayMonit = 0;
        public static int numeroRetentativasBluetooth = 0;
    }
}
