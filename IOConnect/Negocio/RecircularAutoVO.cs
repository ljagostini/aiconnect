using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect
{
    public class RecircularAutoVO
    {
        public ModBusDispenserMover_P3 DispenserP3 { get; set; } = null;

        public List<IDispenser> Dispenser { get; set; } = new List<IDispenser>();
      
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

            public List<int> NrCircuitosValve = new List<int>();

            public MDispositivos(bool isMotor24_P1 = false)
            {
                if(isMotor24_P1)
                {
                    PulsoHorario = new int[24];
                    Velocidade = new int[24];
                    Aceleracao  = new int[24];
                    Delay = new int[24];
                    PulsoReverso  = new int[24];
                }
            }

        }
    }
}
