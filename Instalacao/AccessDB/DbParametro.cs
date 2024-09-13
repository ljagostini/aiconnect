namespace Percolore.Instalacao
{
    public class DbParametro
    {
        public byte ComPort { get; set; }
        public int ReverseDelay { get; set; }
        public int ReverseSteps { get; set; }

        public DbParametro()
        {
            ComPort = 1;
            ReverseDelay = 0;
            ReverseSteps = 0;
        }
    }
}