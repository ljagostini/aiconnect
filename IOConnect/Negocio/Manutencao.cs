using Percolore.Core.Persistence.WindowsRegistry;
using System;

namespace Percolore.IOConnect
{
    public class Manutencao
    {
        private long timestampValidade = 0;
        private DateTimeOffset validade;

        public long TimestampValidade
        {
            get { return timestampValidade; }
        }
        public DateTimeOffset Validade
        {
            get { return this.validade; }
        }

        public Manutencao()
        {
            using (PercoloreRegistry percRegistry = new PercoloreRegistry())
            {
                this.timestampValidade = percRegistry.GetValidadeManutencao();
            }

            this.validade =
                DateTimeOffset.FromUnixTimeSeconds(timestampValidade);
        }

        /// <summary>
        /// Retorna string formatada com tempo restante calculado em dias, horas e minutos
        /// </summary>
        public string GetLabelTempoRestante()
        {
            TimeSpan ts = validade.Subtract(DateTimeOffset.UtcNow);
            string label = ts.ToString($"dd' {Properties.IOConnect.Global_Label_Dias}'");
            return label;
        }

        /// <summary>
        /// Calcula tempo restante até a expiração da validade e retorna timespan
        /// </summary>
        public TimeSpan GetTempoRestante()
        {
            return CalculaTempoRestante();
        }

        /// <summary>
        /// Define status da manutenção baseado no tempo restante até o vencimento.
        /// </summary>
        /// <returns></returns>
        public StatusManutencao GetStatus()
        {
            TimeSpan restante = CalculaTempoRestante();
            StatusManutencao status;
            double totalDays = restante.TotalDays;

            if (restante.Days == 60)
            {
                status = StatusManutencao.Atencao;
            }
            else if (restante.Days >= 0 && restante.Days <= 30)
            {
                status = StatusManutencao.Critico;
            }
            else if (totalDays < 0)
            {
                status = StatusManutencao.Vencido;
            }
            else
            {
                status = StatusManutencao.Seguro;
            }

            return status;
        }

        private TimeSpan CalculaTempoRestante()
        {
            TimeSpan restante = validade.Subtract(DateTimeOffset.UtcNow);
            return restante;
        }
    }
}
