namespace Percolore.Instalacao
{
    public class DbPigmento
    {
        public string Nome { get; set; }
        public double PesoEspecificao { get; set; }
        public int Velocidade { get; set; }
        public int Aceleracao { get; set; }
        public int Posicao { get; set; }
        public int FatorEscalaCorrente { get; set; }
        public int ReverseDelay { get; set; }
        public int ReverseSteps { get; set; }

        public DbPigmento()
        {
            Nome = string.Empty;
            PesoEspecificao = 0;
            Velocidade = 0;
            Aceleracao = 0;
            Posicao = 1;
            FatorEscalaCorrente = 0;
            ReverseDelay = 0;
            ReverseSteps = 0;
        }
    }
}