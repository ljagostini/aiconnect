using PaintMixer;
using Percolore.Core;
using Percolore.IOConnect.Modbus;

namespace Percolore.IOConnect
{
	public class ModBusDispenserMover_P3 : IDispenser, IMover, IDisposable
    {
        private PaintMixerInterface_P2 mixer;
        private PaintMoverInterface_P3 mover;
        private Util.ObjectParametros parametros = null;
        PaintMoverInterface_P3.StatusValue stSensores = new PaintMoverInterface_P3.StatusValue();
        private bool isBusy = false;
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

        public ModBusDispenserMover_P3(string NomeDispositivo = "", string NomeDispositivoMov = "")
        {
            parametros = Util.ObjectParametros.Load();
            mixer = new PaintMixerInterface_P2(parametros.Slave, NomeDispositivo);
            mover = new PaintMoverInterface_P3(parametros.Address_PlacaMov, NomeDispositivoMov);
        }


        #region Propriedades

        public bool isOpenMixer()
        {
            if (mixer != null && mixer.mb != null)
            {
                return mixer.mb.isOpen();
            }
            else
            {
                return false;
            }
        }

        public bool isOpenMover()
        {
            if (mover != null && mover.mb != null)
            {
                return mover.mb.isOpen();
            }
            else
            {
                return false;
            }
        }

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
                //return !mixer.Status.Busy;
            }
        }
      
        public bool Connected
        {
            get
            {
                return mixer.Connected;
            }
        }

        public bool IsHalt
        {
            get
            {
                if (!this.mixer.isDosed)
                {
                    return false;
                }
                else
                {
                    return mixer.Status.Halted;
                }
            }
        }

        public bool IsBusy
        {
            get
            {
                return isBusy;
                
            }
        }

        public bool FirstInput
        {
            get
            {
                ushort inputs = mixer.Inputs;
                //bool firstInput = (inputs & (1 << 0)) != 0;
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

        public bool TerminouProcessoDuplo
        {
            get
            {
                return mover.TerminouProcessoDuplo;
            }
        }

        public int IsNativo { get { return this.stSensores.Nativo; } }

        public int CodError { get { return this.stSensores.CodError; } }

        public bool SensorCopo { get { return this.stSensores.SensorCopo; } }

        public bool SensorEsponja { get { return this.stSensores.SensorEsponja; } }

        public bool SensorAltoBicos { get { return this.stSensores.SensorAltoBicos; } }

        public bool SensorBaixoBicos { get { return this.stSensores.SensorBaixoBicos; } }

        public bool SensorGavetaAberta { get { return this.stSensores.SensorGavetaAberta; } }

        public bool SensorGavetaFechada { get { return this.stSensores.SensorGavetaFechada; } }

        public bool SensorValvulaDosagem { get { return this.stSensores.SensorValvulaDosagem; } }

        public bool SensorValvulaRecirculacao { get { return this.stSensores.SensorValvulaRecirculacao; } }

        public bool SensorEmergencia  { get { return this.stSensores.SensorEmergencia; } }

        public int CodAlerta { get { return this.stSensores.CodAlerta; } }

        public bool MaquinaLigada { get { return this.stSensores.MaquinaLigada; } }

        public bool IsEmergencia
        {
            get
            {
                ReadSensores_Mover();
                return this.stSensores.SensorEmergencia;
            }
        }

        #endregion

        #region Métodos públicos

        public void Connect()
        {
            mixer.Connect(
                this.parametros.ResponseTimeout);
        }

        public void Connect_Mover()
        {
            mover.Connect(this.parametros.ResponseTimeout);
        }

        public void Disconnect()
        {
            mixer.Disconnect();
        }

        public void Disconnect_Mover()
        {
            mover.Disconnect();
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

            mixer.RunReverse(dist, vel, ace, revDelay, revPulsos);
        }

        /// <summary>
        /// Envia para a placa todas as posições do array passado como parâmetro
        /// </summary>
        public void Dispensar(
            int[] pulsoHorario, int[] velocidade, int[] aceleracao, int[] delay, int[] pulsoReverso, int i_Step = 0)
        {
            mixer.RunReverse(
                pulsoHorario, velocidade, aceleracao, delay, pulsoReverso);
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
                vPulso, vVelocidade, vAceleracao, vDelay, vPulsoReverso);
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

            for (int i = 0; i < motor.Length; i++)
            {
                //Informa apenas para motor passado como parâmetro
                vPulso[motor[i]] = pulso;
                vVelocidade[motor[i]] = velocidade;
                vAceleracao[motor[i]] = aceleracao;
                vDelay[motor[i]] = delay;
                vPulsoReverso[motor[i]] = pulso_reverso;
            }

            mixer.RunReverse(
                vPulso, vVelocidade, vAceleracao, vDelay, vPulsoReverso);
        }

        /// <summary>
        /// Envia para a placa apenas as posições do array passado como parâmetro em grupos de 5.
        /// A utilidade desse métodos é executar uma dispensa sequencial de grupos.
        /// </summary>
        /// <param name="iniciar_reverso">Determina se o primeiro movimento é reverso</param>
        /// <param name="qtdeElementosGrupo">Quantidade de elementos mapeados para cada grupo de dispensa</param>
        public void Dispensar(
            int[] pulso, int velocidade, int aceleracao, bool iniciar_reverso, int qtdeElementosGrupo, int i_Step = 0)
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
                    mixer.RunReverse(pulso_minimo, vel, ace, new int[16], g1);

                    while (!IsReady)
                        Thread.Sleep(1);

                    mixer.Run(g1, vel, ace);

                    //Execução do grupo 02
                    if (g2.Count(c => c != 0) > 0)
                    {
                        while (!IsReady)
                            Thread.Sleep(10);

                        mixer.RunReverse(pulso_minimo, vel, ace, new int[16], g2);

                        while (!IsReady)
                            Thread.Sleep(10);

                        mixer.Run(g2, vel, ace);
                    }

                    //Execução do grupo 03
                    if (g3.Count(c => c != 0) > 0)
                    {
                        while (!IsReady)
                            Thread.Sleep(10);

                        mixer.RunReverse(pulso_minimo, vel, ace, new int[16], g3);

                        while (!IsReady)
                            Thread.Sleep(10);

                        mixer.Run(g3, vel, ace);
                    }

                    //Execução do grupo 04
                    if (g4.Count(c => c != 0) > 0)
                    {
                        while (!IsReady)
                            Thread.Sleep(10);

                        mixer.RunReverse(pulso_minimo, vel, ace, new int[16], g4);

                        while (!IsReady)
                            Thread.Sleep(10);

                        mixer.Run(g4, vel, ace);
                    }

                    //Execução do grupo 05
                    if (g5.Count(c => c != 0) > 0)
                    {
                        while (!IsReady)
                            Thread.Sleep(1);

                        mixer.RunReverse(pulso_minimo, vel, ace, new int[16], g5);

                        while (!IsReady)
                            Thread.Sleep(10);

                        mixer.Run(g5, vel, ace);
                    }

                    //Execução do grupo 06
                    if (g6.Count(c => c != 0) > 0)
                    {
                        while (!IsReady)
                            Thread.Sleep(10);

                        mixer.RunReverse(pulso_minimo, vel, ace, new int[16], g6);

                        while (!IsReady)
                            Thread.Sleep(10);

                        mixer.Run(g6, vel, ace);
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
                        mixer.RunReverse(pulso, vel, ace, new int[16], pulso);

                        #endregion
                    }

                    else
                    {

                        #region Iniciar para frente [Entre 3 e 4 circuitos pro grupo]

                        /* Quando a quantidade de circuitos é diferente de 5, para que haja controle,
                         * é necessário definir grupos de dispensa e controlar manualmente as chamadas 
                         * e os status de retorno do PaintMixer. */

                        //Execução do grupo 01
                        mixer.RunReverse(g1, vel, ace, new int[16], g1);

                        //Execução do grupo 02
                        if (g2.Count(c => c != 0) > 0)
                        {
                            while (!IsReady)
                                Thread.Sleep(10);

                            mixer.RunReverse(g2, vel, ace, new int[16], g2);
                        }

                        //Execução do grupo 03
                        if (g3.Count(c => c != 0) > 0)
                        {
                            while (!IsReady)
                                Thread.Sleep(10);

                            mixer.RunReverse(g3, vel, ace, new int[16], g3);
                        }

                        //Execução do grupo 04
                        if (g4.Count(c => c != 0) > 0)
                        {
                            while (!IsReady)
                                Thread.Sleep(10);

                            mixer.RunReverse(g4, vel, ace, new int[16], g4);
                        }

                        //Execução do grupo 05
                        if (g5.Count(c => c != 0) > 0)
                        {
                            while (!IsReady)
                                Thread.Sleep(10);

                            mixer.RunReverse(g5, vel, ace, new int[16], g5);
                        }

                        //Execução do grupo 06
                        if (g6.Count(c => c != 0) > 0)
                        {
                            while (!IsReady)
                                Thread.Sleep(10);

                            mixer.RunReverse(g6, vel, ace, new int[16], g6);
                        }

                        #endregion
                    }

                }
            }
            catch
            {
                // Disconnect();
                throw;
            }
            finally
            {
                //Status do dispensador
                isBusy = false;
            }
        }

        public void RessetDosed()
        {
            this.mixer.isDosed = true;
        }

        public void ReadSensores_Mover()
        {
            this.stSensores = this.mover.Status;
        }

        public void AbrirGaveta(bool? _subirBico = null)
        {
            this.mover.TerminouProcessoDuplo = false;
            
            if (_subirBico != null && _subirBico.HasValue && _subirBico.Value)
            {
                this.ReadSensores_Mover();
                if (!this.stSensores.SensorAltoBicos)
                {
                    Thread.Sleep(500);
                    SubirBico();
                    bool isSubiu = false;
                    for (int i = 0; !isSubiu && i < 20; i++)
                    {
                        Thread.Sleep(500);
                        this.ReadSensores_Mover();
                        if ((this.stSensores.Nativo == 0 || this.stSensores.Nativo == 2) && this.stSensores.SensorAltoBicos)
                        {
                            isSubiu = true;
                        }
                        else
                        {
                            Thread.Sleep(500);
                        }
                            
                    }
                    if (isSubiu)
                    {
                        Thread.Sleep(100);
                        this.MovimentarManual(1, true);
                    }

                }
                else
                {
                    Thread.Sleep(100);
                    this.MovimentarManual(1, true);
                }
            }
            else
            {
                this.MovimentarManual(1, true);
            }

            this.mover.TerminouProcessoDuplo = true;
        }

        public void FecharGaveta(bool? _descerBico = null)
        {
            this.mover.TerminouProcessoDuplo = false;
            
            bool isBicoAlto = false;
            this.ReadSensores_Mover();
            if(!this.stSensores.SensorAltoBicos)
            {
                Thread.Sleep(100);
                SubirBico();
                for (int i = 0; !isBicoAlto && i < 20; i++)
                {
                    Thread.Sleep(500);
                    this.ReadSensores_Mover();
                    if ((this.stSensores.Nativo == 0 || this.stSensores.Nativo == 2) && this.stSensores.SensorAltoBicos)
                    {
                        isBicoAlto = true;
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }                    
            }
            else
            {
                isBicoAlto = true;
            }
            if (isBicoAlto)
            {
                Thread.Sleep(100);
                this.MovimentarManual(1, false);
                if (_descerBico != null && _descerBico.HasValue && _descerBico.Value)
                {
                    bool isFechouGaveta = false;
                    for (int i = 0; !isFechouGaveta && i < 20; i++)
                    {
                        Thread.Sleep(500);
                        this.ReadSensores_Mover();
                        if ((this.stSensores.Nativo == 0 || this.stSensores.Nativo == 2) && this.stSensores.SensorGavetaFechada)
                        {
                            isFechouGaveta = true;
                        }
                        else
                        {
                            Thread.Sleep(500);
                        }
                    }
                        
                    //Thread.Sleep(1000);
                    if (isFechouGaveta && !this.stSensores.SensorBaixoBicos)
                    {
                        Thread.Sleep(100);
                        DescerBico();
                        bool isDesceu = false;
                        for (int i = 0; !isDesceu && i < 20; i++)
                        {
                            Thread.Sleep(500);
                            this.ReadSensores_Mover();
                            if ((this.stSensores.Nativo == 0 || this.stSensores.Nativo == 2) && this.stSensores.SensorBaixoBicos)
                            {
                                isDesceu = true;
                            }
                            else
                            {
                                Thread.Sleep(500);
                            }

                        }

                    }
                }
            }

            this.mover.TerminouProcessoDuplo = true;
        }

        public void ValvulaPosicaoDosagem()
        {
            this.MovimentarManual(2, true);
        }

        public void ValvulaPosicaoRecirculacao()
        {
            this.MovimentarManual(2, false);
        }

        public void SubirBico()
        {
            this.MovimentarManual(3, true);
        }

        public void DescerBico()
        {
            this.MovimentarManual(3, false);
        }

        /// <summary>
        /// Envia para a placa apenas mmovimento Manual.
        /// A utilidade desse métodos é executar o movimento Manual dos motores.
        /// </summary>
        /// <param name="motor">Determina o motor do movimento</param>
        /// <param name="isForward">Determina se é pra frente ou para recuar</param>
        public void MovimentarManual(int motor, bool isForward)
        {
            this.mover.MovimentarManual(motor, isForward);
        }

        /// <summary>
        /// Envia para a placa apenas o movimento Automático.
        /// A utilidade desse métodos é executar o Movimento de abertira e fachamento do copo.
        /// </summary>
        public void MovimentarAutomatico()
        {
            this.mover.MovimentarAutomatico();
        }

        public string GetDescCodError()
        {
            string retorno = "";
            switch (this.stSensores.CodError)
            {
                case 0:
                    {
                        //retorno = "Processo concluído";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Erro_00;
                        break;
                    }
                case 1:
                    {
                        //retorno = "Gaveta não abriu";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Erro_01;
                        break;
                    }
                case 2:
                    {
                        //retorno = "Gaveta não fechou";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Erro_02;
                        break;
                    }
                case 3:
                    {
                        //retorno = "Válvula não está em recirculação";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Erro_03;
                        break;
                    }
                case 4:
                    {
                        //retorno = "Válvula não está em dosagem";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Erro_04;
                        break;
                    }
                case 5:
                    {
                        //retorno = "Bico não subiu";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Erro_05;
                        break;
                    }
                case 6:
                    {
                        //retorno = "Bico não desceu";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Erro_06;
                        break;
                    }
                case 7:
                    {
                        //retorno = "Máquina desligada";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Erro_01;
                        break;
                    }
                default:
                    {
                        //retorno = "Cód error " + this.stSensores.CodError.ToString();
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Erro_Desconhecido + this.stSensores.CodError.ToString();
                        break;
                    }
            }
            return retorno;
        }

        public string GetDescCodAlerta()
        {
            string retorno = "";
            switch (this.stSensores.CodAlerta)
            {
                case 0:
                    {
                        retorno = "Ok";
                        break;
                    }
                case 1:
                    {
                        //retorno = "Usuário, por favor, acesse a tela de manutenção, retire o recipiente de dentro da dosadora, posicione o copo da esponja, feche a gaveta e baixe os bicos.";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_01;
                        break;
                    }
                case 2:
                    {
                        //retorno = "Usuário, por favor, acesse a tela de manutenção, retire o recipiente de dentro da dosadora, posicione o copo da esponja, feche a gaveta e baixe os bicos.";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_02;
                        break;
                    }
                case 3:
                    {
                        //retorno = "Usuário, por favor, acesse a tela de manutenção, retire o recipiente de dentro da dosadora, posicione o copo da esponja, feche a gaveta e baixe os bicos.";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_03;
                        break;
                    }
                case 4:
                    {
                        //retorno = "Usuário, por favor, acesse a tela de manutenção, retire o recipiente de dentro da dosadora, posicione o copo da esponja, feche a gaveta e baixe os bicos.";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_04;
                        break;
                    }
                case 5:
                    {
                        //retorno = "Código alerta 5";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_05;
                        break;
                    }
                case 6:
                    {
                        //retorno = "Código alerta 6";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_06;
                        break;
                    }
                case 7:
                    {
                        //retorno = "Usuário, por favor, acesse a tela de manutenção, abra a gaveta, posicione o copo da esponja, feche a gaveta e baixe os bicos.";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_07;
                        break;
                    }
                case 8:
                    {
                        //retorno = "Usuário, por favor, acesse a tela de manutenção, abra a gaveta, posicione o copo da esponja, feche a gaveta e baixe os bicos.";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_08;
                        break;
                    }
                case 9:
                    {
                        //retorno = "Usuário, por favor, acesse a tela de manutenção, posicione o copo da esponja, feche a gaveta e baixe os bicos.";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_09;
                        break;
                    }
                case 10:
                    {
                        //retorno = "Usuário, por favor, acesse a tela de manutenção, posicione o copo da esponja, feche a gaveta e baixe os bicos.";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_10;
                        break;
                    }
                case 11:
                    {
                        //retorno = "Usuário, por favor, acesse a tela de manutenção e posicione a dosadora em modo de repouso.";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_11;
                        break;
                    }
                case 12:
                    {
                        //retorno = "Usuário, por favor, acesse a tela de manutenção e posicione a dosadora em modo de repouso.";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_12;
                        break;
                    }
                case 13:
                    {
                        //retorno = "Usuário, por favor, acesse a tela de manutenção e posicione a gaveta.";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_13;
                        break;
                    }
                case 14:
                    {
                        //retorno = "Código alerta 14";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_14;
                        break;
                    }
                case 15:
                    {
                        //retorno = "Código alerta 15";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_15;
                        break;
                    }
                case 16:
                    {
                        //retorno = "Código alerta 16";
                        retorno = Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_16;
                        break;
                    }

                default:
                    {
                        retorno = this.stSensores.CodAlerta.ToString();
                        break;
                    }
            }
            return retorno;
        }

      
        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                mixer.Dispose();
                mover.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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