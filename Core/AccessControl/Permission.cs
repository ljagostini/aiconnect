namespace Percolore.Core.AccessControl
{
    public class Permissions
    {
        public bool HabilitarDispensaSequencial { get; internal set; }
        public bool HabilitarFormulaPersonalizada { get; internal set; }
        public bool HabilitarModoSimulacao { get; internal set; }
        public bool HabilitarTesteRecipiente { get; internal set; }
        public bool HabilitarControleNivelColorante { get; internal set; }
        public bool HabilitarCircuitoColorante { get; internal set; }
        public bool EditarInformacoesAbaUnidadeMedida { get; internal set; }
        public bool EditarInformacoesAbaInicializacaoCircuitos { get; internal set; }
        public bool EditarInformacoesAbaPurga { get; internal set; }
        public bool EditarInformacoesAbaSenha { get; internal set; }
        
    }
}