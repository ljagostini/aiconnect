namespace Percolore.Core
{
	[Serializable]
    public class ValoresVO
    {
        public double Volume { get; set; }
        public int PulsoHorario { get; set; }
        public int Velocidade { get; set; }

        private int aceleracao;
       
        public int Aceleracao
        {
            get
            {
                if (Velocidade == 0)
                {
                    this.aceleracao = 0;
                    return this.aceleracao;
                }
                /*
                Parametros prametros = Parametros.Load();
                double AG = prametros.Aceleracao;
                double VG = prametros.Velocidade;
                double V = Velocidade;

                double FATOR = (VG / V) * (VG / V);
                double AC = (1 / FATOR) * VG;
                this.aceleracao = int.Parse(Math.Round(AC).ToString());
                */
                return this.aceleracao;
            }
            set {
                this.aceleracao = value;
            }
        }

        public int Delay { get; set; }

        public int PulsoReverso
        {
            get; set;
            /*
            get
            {
                return Parametros.Load().PulsoReverso;
            }
            */
        }    

        public double MassaMedia { get; set; }

        public double MassaIdeal { get; set; }

        public double DesvioMedio { get; set; }
    }
}