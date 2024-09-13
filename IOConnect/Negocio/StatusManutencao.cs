namespace Percolore.IOConnect
{
    public enum StatusManutencao
    {
        /// <summary>
        /// Acima de 60 dias até a data de vencimento
        /// </summary>
        Seguro = 0,

        /// <summary>
        /// De 60 a 30 dias até a data de vencimento
        /// </summary>
        Atencao = 1,

        /// <summary>
        /// De 30 a 0 dias até a data de vencimento
        /// </summary>
        Critico = 2,

        /// <summary>
        /// Manutenção vencida.
        /// </summary>
        Vencido = 3
    }
}
