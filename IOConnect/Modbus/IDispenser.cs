using Percolore.Core;
using Percolore.IOConnect.Modbus;
using System.Collections.Generic;

namespace Percolore.IOConnect
{
    /// <summary>
    /// Description of PaintDispenser.
    /// </summary>
    public interface IDispenser
    {
        bool IsReady { get; }

        /// <summary>
        /// Sinaliza se dispensador está ocupado.
        /// Esta flag é utilizada na rotina de dispensa criada para 
        /// o processo de inicializaço de circuitos pois é necessário 
        /// obter status ao final da dispensa a cada grupo de 5 motores.
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// Lê a primeira entrada digital da placa e verifica se é 0 ou 1.
        /// </summary>
        bool FirstInput { get; }

        bool IdentityPot { get; }

        bool IdentitySponge { get; }

        bool Connected { get; }

        void Connect();

        void Disconnect();

        void Dispensar(ValoresVO[] valores, int i_Step = 0);

        void Dispensar(
          int[] pulsoHorario, int[] velocidade, int[] aceleracao, int[] delay, int[] pulso_reverso, int i_Step = 0);

        void Dispensar(
            int motor, int pulso, int velocidade, int aceleracao, int delay, int pulso_reverso, int i_Step = 0);

        void Dispensar(
           int[] motor, int pulso, int velocidade, int aceleracao, int delay, int pulso_reverso, int i_Step = 0);

        void Dispensar(
            int[] pulso, int velocidade, int aceleracao, bool iniciar_reverso, int qtdeElementosGrupo, int i_Step = 0);

        void Halt();

        void UnHalt();

        void RessetDosed();
        

        int IdDispositivo { get; set; }


        bool MayValve { get; set; }
        StatusValvulas getStatusValvulas();

        void AcionaValvula(bool aVal, int circuito);

        void AcionaValvulasRecirculacao(List<int> lcircuito);

        void AcionaValvulas(bool allVal);

        void AcionaValvulas(int nval);
    }
}
