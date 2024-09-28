using PaintMixer;
using Percolore.Core;
using Percolore.Core.Logging;
using Percolore.IOConnect.Modbus;

namespace Percolore.IOConnect
{
	public class ModBusDispenser_P2 : IDispenser, IDisposable
    {
        private PaintMixerInterface_P2 mixer;
        private Util.ObjectParametros parametros = null;
        private bool isBusy = false;
        private bool mayValve = true;

        public bool MayValve
        {
            get { return mayValve; }
            set { mayValve = value; }
        }

        public class StatusSensores
        {
            public bool Input_1 { get; set; }
            public bool Input_2 { get; set; }
            public bool Input_3 { get; set; }
            public bool Input_4 { get; set; }
        }

   


        private int idDispositivo;

        public int IdDispositivo
        {
            get { return idDispositivo; }
            set { idDispositivo = value; }
        }

        public ModBusDispenser_P2(string NomeDispositivo = "")
        {
            parametros = Util.ObjectParametros.Load();
            mixer = new PaintMixerInterface_P2(parametros.Slave, NomeDispositivo);
        }


        #region Propriedades

        public bool IsReady
        {
            get
            {
                if (!this.mixer.isDosed)
                {
                    return false;

                }
                else
                {
                    return !mixer.Status.Busy;
                }
               
            }
        }

        public bool IsBusy
        {
            get { return isBusy; }
        }

        public bool FirstInput
        {
            get
            {
                ushort inputs = mixer.Inputs;
                bool firstInput = (inputs & (0x01)) != 0;
                return firstInput;
            }
        }

        public bool IdentityPot
        {
            get
            {
                PaintMixerInterface_P2.StatusInput sti = mixer.Status_Inputs;
                if (sti.Input_2 && sti.Input_3)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        
        public bool IdentitySponge
        {
            get
            {
                PaintMixerInterface_P2.StatusInput sti = mixer.Status_Inputs;
                if (sti.Input_2 && !sti.Input_3)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
               
        public bool Connected
        {
            get
            {
                if(mixer== null)
                {
                    return false;
                }
                else
                {
                    return mixer.Connected;
                }
            }
        }

        #endregion

        #region Métodos públicos

        public StatusSensores getStatusSensores()
        {
            StatusSensores retorno = new StatusSensores();
            try
            {
                PaintMixerInterface_P2.StatusInput stI = mixer.Status_Inputs;
                retorno.Input_1 = stI.Input_1;
                retorno.Input_2 = stI.Input_2;
                retorno.Input_3 = stI.Input_3;
                retorno.Input_4 = stI.Input_4;
            }
            catch
            { }
            return retorno;
        }

        public void Connect()
        {
            mixer.Connect(
                this.parametros.ResponseTimeout);
        }

        public void Disconnect()
        {
            mixer.Disconnect();
        }

        public void Halt()
        {
            mixer.Halt();
        }

        public void UnHalt()
        {
            mixer.Unhalt();
        }

        /// <summary>
        /// Mapeia objeto com valores de calibragem para array e envia para a placa
        /// </summary>
        public void Dispensar(ValoresVO[] valores, int i_Step = 0)
        {

            int[] dist = new int[16];
            int[] vel = new int[16];
            int[] ace = new int[16];
            int[] revDelay = new int[16];
            int[] revPulsos = new int[16];

            /*Percorre posições do vetor com valores de calibragem que não sejam null 
            e preenche array de parâmetros do mixer*/
            for (int i = 0; i <= valores.GetUpperBound(0); i++)
            {
                ValoresVO v = valores[i];
                if (v != null)
                {
                    dist[i] = v.PulsoHorario;
                    vel[i] = v.Velocidade;
                    ace[i] = v.Aceleracao;
                    revDelay[i] = v.Delay;
                    revPulsos[i] = v.PulsoReverso;
                }
            }

            mixer.RunReverse(dist, vel, ace, revDelay, revPulsos, i_Step);
        }

        /// <summary>
        /// Envia para a placa todas as posições do array passado como parâmetro
        /// </summary>
        public void Dispensar(int[] pulsoHorario, int[] velocidade, int[] aceleracao, int[] delay, int[] pulsoReverso, int i_Step = 0)
        {
            mixer.RunReverse(
                pulsoHorario, velocidade, aceleracao, delay, pulsoReverso, i_Step);
        }

        /// <summary>
        /// Envia para a placa apenas os valores da posição informada em motor
        /// </summary>
        public void Dispensar(int motor, int pulso, int velocidade, int aceleracao, int delay, int pulso_reverso, int i_Step = 0)
        {
            int[] vPulso = new int[16];
            int[] vVelocidade = new int[16];
            int[] vAceleracao = new int[16];
            int[] vDelay = new int[16];
            int[] vPulsoReverso = new int[16];

            //Informa apenas para motor passado como parâmetro
            vPulso[motor] = pulso;
            vVelocidade[motor] = velocidade;
            vAceleracao[motor] = aceleracao;
            vDelay[motor] = delay;
            vPulsoReverso[motor] = pulso_reverso;

            mixer.RunReverse(
                vPulso, vVelocidade, vAceleracao, vDelay, vPulsoReverso, i_Step);
        }

        /// <summary>
        /// Envia para a placa apenas os valores da posição informada em motor
        /// </summary>
        public void Dispensar(int[] motor, int pulso, int velocidade, int aceleracao, int delay, int pulso_reverso, int i_Step = 0)
        {
            int[] vPulso = new int[16];
            int[] vVelocidade = new int[16];
            int[] vAceleracao = new int[16];
            int[] vDelay = new int[16];
            int[] vPulsoReverso = new int[16];

            //Informa apenas para motor passado como parâmetro
            for (int i = 0; i < motor.Length; i++)
            {
                vPulso[motor[i]] = pulso;
                vVelocidade[motor[i]] = velocidade;
                vAceleracao[motor[i]] = aceleracao;
                vDelay[motor[i]] = delay;
                vPulsoReverso[motor[i]] = pulso_reverso;
            }
            mixer.RunReverse(
                vPulso, vVelocidade, vAceleracao, vDelay, vPulsoReverso, i_Step);
        }

        /// <summary>
        /// Envia para a placa apenas as posições do array passado como parâmetro em grupos de 5.
        /// A utilidade desse métodos é executar uma dispensa sequencial de grupos.
        /// </summary>
        /// <param name="iniciar_reverso">Determina se o primeiro movimento é reverso</param>
        /// <param name="qtdeElementosGrupo">Quantidade de elementos mapeados para cada grupo de dispensa</param>
        public void Dispensar(int[] pulso, int velocidade, int aceleracao, bool iniciar_reverso, int qtdeElementosGrupo, int i_Step = 0)
        {
            //[Status do dispensador]
            isBusy = true;

            int[] vel = new int[16];
            int[] ace = new int[16];
            int[] pulso_minimo = new int[16];
            int[] g1 = new int[16];
            int[] g2 = new int[16];
            int[] g3 = new int[16];
            int[] g4 = new int[16];
            int[] g5 = new int[16];
            int[] g6 = new int[16];

            /* Preenche arrays de parâmetro de dispensa apenas nas posições onde 
             * foi informado pulso maior que zero*/
            for (int i = 0; i <= 15; i++)
            {
                if (pulso[i] == 0)
                {
                    continue;
                }

                vel[i] = velocidade;
                ace[i] = aceleracao;

                if (iniciar_reverso || qtdeElementosGrupo < 5)
                {
                    #region Mapeia o array de dispensa para grupos

                    /* O pulso mínimo é utilizado como distância na chamada do método RunReverse do WSMBS
                     * Ele é utilizado para que seja possível dispensar apenas reverso pois, para isso,
                     * é necessário informar ao menos 1(um) pulso na direção inicial */
                    pulso_minimo[i] = 1;

                    /* Grupos de dispensa utilizados para gerenciar o envio de dados para a placa
                     * São arrays de 16 posições onde apenas 5 posições terão valor maior que zero */
                    if (g1.Count(c => c != 0) < qtdeElementosGrupo)
                    {
                        g1[i] = pulso[i];
                    }
                    else if (g2.Count(c => c != 0) < qtdeElementosGrupo)
                    {
                        g2[i] = pulso[i];
                    }
                    else if (g3.Count(c => c != 0) < qtdeElementosGrupo)
                    {
                        g3[i] = pulso[i];
                    }
                    else if (g4.Count(c => c != 0) < qtdeElementosGrupo)
                    {
                        g4[i] = pulso[i];
                    }
                    else if (g5.Count(c => c != 0) < qtdeElementosGrupo)
                    {
                        g5[i] = pulso[i];
                    }
                    else if (g6.Count(c => c != 0) < qtdeElementosGrupo)
                    {
                        g6[i] = pulso[i];
                    }

                    #endregion
                }
            }

            try
            {
                if (iniciar_reverso)
                {
                    #region Iniciar com movimento reverso

                    //Execução do grupo 01
                    mixer.RunReverse(pulso_minimo, vel, ace, new int[16], g1, i_Step);

                    while (!IsReady)
                        Thread.Sleep(1);

                    mixer.Run(g1, vel, ace, i_Step);

                    //Execução do grupo 02
                    if (g2.Count(c => c != 0) > 0)
                    {
                        while (!IsReady)
                            Thread.Sleep(10);

                        mixer.RunReverse(pulso_minimo, vel, ace, new int[16], g2, i_Step);

                        while (!IsReady)
                            Thread.Sleep(10);

                        mixer.Run(g2, vel, ace, i_Step);
                    }

                    //Execução do grupo 03
                    if (g3.Count(c => c != 0) > 0)
                    {
                        while (!IsReady)
                            Thread.Sleep(10);

                        mixer.RunReverse(pulso_minimo, vel, ace, new int[16], g3, i_Step);

                        while (!IsReady)
                            Thread.Sleep(10);

                        mixer.Run(g3, vel, ace, i_Step);
                    }

                    //Execução do grupo 04
                    if (g4.Count(c => c != 0) > 0)
                    {
                        while (!IsReady)
                            Thread.Sleep(10);

                        mixer.RunReverse(pulso_minimo, vel, ace, new int[16], g4, i_Step);

                        while (!IsReady)
                            Thread.Sleep(10);

                        mixer.Run(g4, vel, ace, i_Step);
                    }

                    //Execução do grupo 05
                    if (g5.Count(c => c != 0) > 0)
                    {
                        while (!IsReady)
                            Thread.Sleep(1);

                        mixer.RunReverse(pulso_minimo, vel, ace, new int[16], g5, i_Step);

                        while (!IsReady)
                            Thread.Sleep(10);

                        mixer.Run(g5, vel, ace, i_Step);
                    }

                    //Execução do grupo 06
                    if (g6.Count(c => c != 0) > 0)
                    {
                        while (!IsReady)
                            Thread.Sleep(10);

                        mixer.RunReverse(pulso_minimo, vel, ace, new int[16], g6, i_Step);

                        while (!IsReady)
                            Thread.Sleep(10);

                        mixer.Run(g6, vel, ace, i_Step);
                    }

                    #endregion
                }
                else
                {

                    if (qtdeElementosGrupo == 5)
                    {
                        #region Iniciar para frente [Com 5 circuitos pro grupo]

                        /* A partir de 5 circuitos por grupo o firmware da placa é quem controlará o processo.
                         * É feito dessa forma pois quando o firmware controla os circuitos, ao terminar a dispensa 
                         * em um circuito, automaticamente uma nova dispensa é enviada para o circuito ocioso 
                         * reduzindo o tempo de dispensa */
                        mixer.RunReverse(pulso, vel, ace, new int[16], pulso, i_Step);

                        #endregion
                    }

                    else
                    {

                        #region Iniciar para frente [Entre 3 e 4 circuitos pro grupo]

                        /* Quando a quantidade de circuitos é diferente de 5, para que haja controle,
                         * é necessário definir grupos de dispensa e controlar manualmente as chamadas 
                         * e os status de retorno do PaintMixer. */

                        //Execução do grupo 01
                        mixer.RunReverse(g1, vel, ace, new int[16], g1, i_Step);

                        //Execução do grupo 02
                        if (g2.Count(c => c != 0) > 0)
                        {
                            while (!IsReady)
                                Thread.Sleep(10);

                            mixer.RunReverse(g2, vel, ace, new int[16], g2, i_Step);
                        }

                        //Execução do grupo 03
                        if (g3.Count(c => c != 0) > 0)
                        {
                            while (!IsReady)
                                Thread.Sleep(10);

                            mixer.RunReverse(g3, vel, ace, new int[16], g3, i_Step);
                        }

                        //Execução do grupo 04
                        if (g4.Count(c => c != 0) > 0)
                        {
                            while (!IsReady)
                                Thread.Sleep(10);

                            mixer.RunReverse(g4, vel, ace, new int[16], g4, i_Step);
                        }

                        //Execução do grupo 05
                        if (g5.Count(c => c != 0) > 0)
                        {
                            while (!IsReady)
                                Thread.Sleep(10);

                            mixer.RunReverse(g5, vel, ace, new int[16], g5, i_Step);
                        }

                        //Execução do grupo 06
                        if (g6.Count(c => c != 0) > 0)
                        {
                            while (!IsReady)
                                Thread.Sleep(10);

                            mixer.RunReverse(g6, vel, ace, new int[16], g6, i_Step);
                        }

                        #endregion
                    }

                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}
			finally
            {
                //Status do dispensador
                isBusy = false;
            }
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                mixer.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void RessetDosed()
        {
            this.mixer.isDosed = true;

        }

        public bool RessetHard()
        {
            bool retorno = false;
            try
            {
                mixer.RessetHard();
                retorno = true;
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}

			return retorno;
        }

        public string GetVersion()
        {
            string retorno = "";
            try
            {
                retorno = mixer.GetVersion();
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}

			return retorno;
        }


        public StatusValvulas getStatusValvulas()
        {
            StatusValvulas retorno = new StatusValvulas();
            try
            {
                PaintMixerInterface_P2.StatusValvulas stV = mixer.Status_Valvulas;
                retorno.Circuito_1 = stV.Input_1;
                retorno.Circuito_2 = stV.Input_2;
                retorno.Circuito_3 = stV.Input_3;
                retorno.Circuito_4 = stV.Input_4;
                retorno.Circuito_5 = stV.Input_5;
                retorno.Circuito_6 = stV.Input_6;
                retorno.Circuito_7 = stV.Input_7;
                retorno.Circuito_8 = stV.Input_8;
                retorno.Circuito_9 = stV.Input_9;
                retorno.Circuito_10 = stV.Input_10;
                retorno.Circuito_11 = stV.Input_11;
                retorno.Circuito_12 = stV.Input_12;
                retorno.Circuito_13 = stV.Input_13;
                retorno.Circuito_14 = stV.Input_14;
                retorno.Circuito_15 = stV.Input_15;
                retorno.Circuito_16 = stV.Input_16;
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}

			return retorno;
        }

        public void AcionaValvula(bool aVal, int circuito)
        {
            StatusValvulas status = getStatusValvulas();
            int nval = 0;
            if((status.Circuito_1 && circuito != 1) || (circuito == 1 && aVal))
            {
                nval += 1;
            }
            if ((status.Circuito_2 && circuito != 2) || (circuito == 2 && aVal))
            {
                nval += 2;
            }
            if ((status.Circuito_3 && circuito != 3) || (circuito == 3 && aVal))
            {
                nval += 4;
            }
            if ((status.Circuito_4 && circuito != 4) || (circuito == 4 && aVal))
            {
                nval += 8;
            }
            if ((status.Circuito_5 && circuito != 5) || (circuito == 5 && aVal))
            {
                nval += 16;
            }
            if ((status.Circuito_6 && circuito != 6) || (circuito == 6 && aVal))
            {
                nval += 32;
            }
            if ((status.Circuito_7 && circuito != 7) || (circuito == 7 && aVal))
            {
                nval += 64;
            }
            if ((status.Circuito_8 && circuito != 8) || (circuito == 8 && aVal))
            {
                nval += 128;
            }
            if ((status.Circuito_9 && circuito != 9) || (circuito == 9 && aVal))
            {
                nval += 256;
            }
            if ((status.Circuito_10 && circuito != 10) || (circuito == 10 && aVal))
            {
                nval += 512;
            }
            if ((status.Circuito_11 && circuito != 11) || (circuito == 11 && aVal))
            {
                nval += 1024;
            }
            if ((status.Circuito_12 && circuito != 12) || (circuito == 12 && aVal))
            {
                nval += 2048;
            }
            if ((status.Circuito_13 && circuito != 13) || (circuito == 13 && aVal))
            {
                nval += 4096;
            }
            if ((status.Circuito_14 && circuito != 14) || (circuito == 14 && aVal))
            {
                nval += 8192;
            }
            if ((status.Circuito_15 && circuito != 15) || (circuito == 15 && aVal))
            {                
                nval += 16384;
            }
            if ((status.Circuito_16 && circuito != 16) || (circuito == 16 && aVal))
            {
                nval += 32768;
            }

            //1111 1111 1111 1111
            AcionaValvulas(nval);
        }

        public void AcionaValvulasRecirculacao(List<int> lcircuito)
        {
            StatusValvulas status = getStatusValvulas();
            int nval = 0;
            foreach (int circuito in lcircuito)
            {
                if (circuito == 1)
                {
                    nval += 1;
                }
                if (circuito == 2)
                {
                    nval += 2;
                }
                if (circuito == 3)
                {
                    nval += 4;
                }
                if (circuito == 4 )
                {
                    nval += 8;
                }
                if (circuito == 5 )
                {
                    nval += 16;
                }
                if (circuito == 6 )
                {
                    nval += 32;
                }
                if (circuito == 7)
                {
                    nval += 64;
                }
                if (circuito == 8)
                {
                    nval += 128;
                }
                if (circuito == 9)
                {
                    nval += 256;
                }
                if (circuito == 10)
                {
                    nval += 512;
                }
                if (circuito == 11)
                {
                    nval += 1024;
                }
                if (circuito == 12)
                {
                    nval += 2048;
                }
                if (circuito == 13)
                {
                    nval += 4096;
                }
                if (circuito == 14)
                {
                    nval += 8192;
                }
                if (circuito == 15)
                {
                    nval += 16384;
                }
                if (circuito == 16)
                {
                    nval += 32768;
                }
            }

            //1111 1111 1111 1111
            AcionaValvulas(nval);
        }

        public void AcionaValvulas(bool allVal)
        {
            if(allVal)
            {
                //1111 1111 1111 1111
                AcionaValvulas(65535);
            }
            else
            {
                AcionaValvulas(0);
            }
        }

        public void AcionaValvulas(int nval)
        {
            mixer.AcionaValvulas(nval);
        }
    }
}