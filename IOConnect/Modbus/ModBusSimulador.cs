using Percolore.Core;
using Percolore.IOConnect.Modbus;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Percolore.IOConnect
{
    public class ModBusSimulador : IDispenser
    {
        private bool isBusy = false;
        private bool isReady = true;

        private bool mayValve = false;

        public bool MayValve
        {
            get { return mayValve; }
            set { mayValve = value; }
        }

        private int idDispositivo;

        public int IdDispositivo
        {
            get { return idDispositivo; }
            set { idDispositivo = value; }
        }

        public ModBusSimulador() { }

        #region Propriedades

        public bool IsReady
        {
            get { return this.isReady; }
        }

        public bool IdentityPot
        {
            get
            {
                return false;        
            }
        }

        public bool IdentitySponge
        {
            get
            {
                return false;
            }
        }

        public bool IsBusy
        {
            get { return this.isBusy; }
        }

        public bool FirstInput
        {
            get { return true; }
        }

        public bool Connected
        {
            get { return false; }
        }
        #endregion

        #region Métodos públicos

        public void Connect()
        {
            //mixer.Connect(); 
            this.isReady = true;
        }

        public void Disconnect()
        {
            //mixer.Disconnect();
            this.isReady = false;
        }

        public void Halt()
        {
            this.isReady = false;
            //mixer.Halt();
        }

        public void UnHalt()
        {
            this.isReady = true;
            //mixer.Unhalt();
        }

        /// <summary>
        /// Dispensa objeto com valores de calibragem
        /// </summary>
        public void Dispensar(ValoresVO[] valores, int i_Step = 0)
        {
            #region Código original comentado

            //int[] dist = new int[16];
            //int[] vel = new int[16];
            //int[] ace = new int[16];
            //int[] revDelay = new int[16];
            //int[] revPulsos = new int[16];

            ///* Percorre posições do vetor com valores de calibragem.
            // * Quando a posição não for nula, será simulado tempo de execução. */
            //for (int i = 0; i <= this.valores.GetUpperBound(0); i++)
            //{
            //    ValoresVO v = this.valores[i];
            //    if (v != null)
            //    {
            //        dist[i] = v.PulsoHorario;
            //        vel[i] = v.Velocidade;
            //        ace[i] = v.Aceleracao;
            //        revDelay[i] = v.Delay;
            //        revPulsos[i] = v.PulsoReverso;
            //    }
            //}

            //mixer.RunReverse(dist, vel, ace, revDelay, revPulsos);

            #endregion

            //Recupera quantidade de circuitos
            int circuitos = valores.Where(v => v != null).ToArray().Count();

            //Executa simulação
            Simular(circuitos);
        }

        /// <summary>
        /// Dispensa todas as posições do array passado como parâmetro
        /// </summary>
        public void Dispensar(
          int[] pulsoHorario, int[] velocidade, int[] aceleracao, int[] delay, int[] pulsoReverso, int i_Step = 0)
        {
            //Chamada de método original
            //mixer.RunReverse(
            //    pulsoHorario, velocidade, aceleracao, delay, pulsoReverso);

            //Recupera quantidade de circuitos
            int circuitos = pulsoHorario.Count();

            //Executa simulação
            Simular(circuitos);
        }

        /// <summary>
        /// Dispensa parâmetros apenas o motor informado como parâmetro
        /// </summary>
        public void Dispensar(int motor, int pulso, int velocidade, int aceleracao, int delay, int pulso_reverso, int i_Step = 0)
        {
            #region Código original comentado

            //int[] vPulso = new int[16];
            //int[] vVelocidade = new int[16];
            //int[] vAceleracao = new int[16];
            //int[] vDelay = new int[16];
            //int[] vPulsoReverso = new int[16];

            ////Informa apenas para motor passado como parâmetro
            //vPulso[motor] = pulso;
            //vVelocidade[motor] = velocidade;
            //vAceleracao[motor] = aceleracao;
            //vDelay[motor] = delay;
            //vPulsoReverso[motor] = pulso_reverso;

            //mixer.RunReverse(
            //    vPulso, vVelocidade, vAceleracao, vDelay, vPulsoReverso);

            #endregion

            //Quantidade de circuitos
            int circuitos = 1;

            //Executa simulação
            Simular(circuitos);
           
        }

        /// <summary>
        /// Dispensa parâmetros apenas o motor informado como parâmetro
        /// </summary>
        public void Dispensar(int[] motor, int pulso, int velocidade, int aceleracao, int delay, int pulso_reverso, int i_Step = 0)
        {
            #region Código original comentado

            //int[] vPulso = new int[16];
            //int[] vVelocidade = new int[16];
            //int[] vAceleracao = new int[16];
            //int[] vDelay = new int[16];
            //int[] vPulsoReverso = new int[16];

            ////Informa apenas para motor passado como parâmetro
            //vPulso[motor] = pulso;
            //vVelocidade[motor] = velocidade;
            //vAceleracao[motor] = aceleracao;
            //vDelay[motor] = delay;
            //vPulsoReverso[motor] = pulso_reverso;

            //mixer.RunReverse(
            //    vPulso, vVelocidade, vAceleracao, vDelay, vPulsoReverso);

            #endregion

            //Quantidade de circuitos
            int circuitos = motor.Length;

            //Executa simulação
            Simular(circuitos);

        }

        /// <summary>
        /// Dispensa as posições do array passado como parâmetro de 5 em 5
        /// </summary>
        /// <param name="iniciar_reverso">Determina se o primeiro movimento é reverso</param>       
        public void Dispensar(
            int[] pulso, int velocidade, int aceleracao, bool iniciar_reverso, int qtdeElementosGrupo, int i_Step = 0)
        {
            //[Status do dispensador]
            this.isBusy = true;

            int[] vel = new int[16];
            int[] ace = new int[16];
            int[] pulso_minimo = new int[16];
            int[] g1 = new int[16];
            int[] g2 = new int[16];
            int[] g3 = new int[16];
            int[] g4 = new int[16];

            /* Preenche arrays de parâmetro de dispensa apenas nas posições onde 
             * foi informado pulso maior que zero*/
            for (int i = 0; i <= 15; i++)
            {
                if (pulso[i] == 0)
                    continue;

                vel[i] = velocidade;
                ace[i] = aceleracao;

                if (iniciar_reverso)
                {
                    #region Dados utilizados quando primeiro movimento reverso habilitado

                    /* O pulso mínimo é utilizado como distância na chamada do método RunReverse do WSMBS
                     * Ele é utilizado para que seja possível dispensar apenas reverso pois, para isso,
                     * é necessário informar ao menos 1(um) pulso na direção inicial */
                    pulso_minimo[i] = 1;

                    /* Grupos de dispensa utilizados para gerenciar o envio de dados para a placa
                     * São arrays de 16 posições onde apenas 5 posições terão valor maior que zero */
                    if (g1.Count(c => c != 0) < 5)
                    {
                        g1[i] = pulso[i];
                    }
                    else if (g2.Count(c => c != 0) < 5)
                    {
                        g2[i] = pulso[i];
                    }
                    else if (g3.Count(c => c != 0) < 5)
                    {
                        g3[i] = pulso[i];
                    }
                    else if (g4.Count(c => c != 0) < 5)
                    {
                        g4[i] = pulso[i];
                    }

                    #endregion
                }
            }


            if (iniciar_reverso)
            {
                #region Iniciar com movimento reverso

                //Execução do grupo 01
                //mixer.RunReverse(pulso_minimo, vel, ace, new int[16], g1);
                Simular(1);

                while (!isReady)
                    Thread.Sleep(1);

                //mixer.Run(g1, vel, ace);
                Simular(1);

                //Execução do grupo 02
                if (g2.Count(c => c != 0) > 0)
                {
                    while (!isReady)
                        Thread.Sleep(1);

                    //mixer.RunReverse(pulso_minimo, vel, ace, new int[16], g2);
                    Simular(1);

                    while (!isReady)
                        Thread.Sleep(1);

                    //mixer.Run(g2, vel, ace);
                    Simular(1);
                }

                //Execução do grupo 03
                if (g3.Count(c => c != 0) > 0)
                {
                    while (!isReady)
                        Thread.Sleep(1);

                    //mixer.RunReverse(pulso_minimo, vel, ace, new int[16], g3);
                    Simular(1);

                    while (!isReady)
                        Thread.Sleep(1);

                    //mixer.Run(g3, vel, ace);
                    Simular(1);
                }

                //Execução do grupo 04
                if (g4.Count(c => c != 0) > 0)
                {
                    while (!isReady)
                        Thread.Sleep(1);

                    //mixer.RunReverse(pulso_minimo, vel, ace, new int[16], g4);
                    Simular(1);

                    while (!isReady)
                        Thread.Sleep(1);

                    //mixer.Run(g4, vel, ace);
                    Simular(1);
                }

                #endregion
            }
            else
            {
                //mixer.RunReverse(
                //    pulso, vel, ace, new int[16], pulso);
                int circuitos = pulso.Count();
                Simular(circuitos);
            }

            //Status do dispensador
            this.isBusy = false;
        }

        #endregion

        #region Métodos privados

        private void Simular(int circuitos)
        {
            int milissegundosPorCircuito = 1000;

            //Calcula tempo de simulação em milissegundos
            int totalMilissegundos = circuitos * milissegundosPorCircuito;

            //Definie dispositivo como ocupado
            this.isReady = false;

            //Simula o processo executado pelo mixer de forma assíncrona
            Task task = Task.Factory.StartNew(() =>
            {
                //Simula tempo de execução. 
                Thread.Sleep(totalMilissegundos);

                //Define dispositivo como pronto
                this.isReady = true;
            });
        }

        public void RessetDosed()
        {          

        }
        #endregion


        //Metodos criados somente para sinalizar, não existe valvulas
        public StatusValvulas getStatusValvulas()
        {
            StatusValvulas retorno = new StatusValvulas();

            return retorno;
        }

        public void AcionaValvula(bool aVal, int circuito)
        {

        }

        public void AcionaValvulasRecirculacao(List<int> lcircuito)
        {
        }

        public void AcionaValvulas(bool allVal)
        {

        }

        public void AcionaValvulas(int nval)
        {

        }
    }
}
