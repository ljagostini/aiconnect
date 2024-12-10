using PaintMixer;
using Percolore.Core;
using Percolore.Core.Logging;
using Percolore.Core.Persistence.WindowsRegistry;
using Percolore.IOConnect.Util;
using System.CodeDom;

namespace Percolore.IOConnect
{
	public class Operar
    {
        private static int CounterComunication = 0;
        //Define o modo de execução do dispenser
        public static void Calibrar_P2(int motor, ValoresVO valores, bool aguardeFim = false)
        {
            Util.ObjectCalibragem c = Util.ObjectCalibragem.Load(motor + 1);
            Util.ObjectColorante cl = Util.ObjectColorante.Load(motor + 1);
            PaintMixerInterface_P2 mixer = null;           

            int[] dist = new int[16];
            int[] vel = new int[16];
            int[] ace = new int[16];
            int[] revDelay = new int[16];
            int[] revPulsos = new int[16];

            Util.ObjectParametros p = Util.ObjectParametros.Load();
            if (cl.Dispositivo == 1)
            {
                if (p.SomarPulsoReverso)
                {
                    dist[motor] = valores.PulsoHorario + c.UltimoPulsoReverso;
                }

                else
                {
                    dist[motor] = valores.PulsoHorario;
                }
                vel[motor] = valores.Velocidade;
                ace[motor] = valores.Aceleracao;
                revDelay[motor] = valores.Delay;
                revPulsos[motor] = valores.PulsoReverso;
            }
            else if (cl.Dispositivo == 2)
            {
                if(p.SomarPulsoReverso)
                {
                    dist[motor-16] = valores.PulsoHorario + c.UltimoPulsoReverso;
                }

                else
                {
                    dist[motor-16] = valores.PulsoHorario;
                }
                vel[motor-16] = valores.Velocidade;
                ace[motor-16] = valores.Aceleracao;
                revDelay[motor-16] = valores.Delay;
                revPulsos[motor-16] = valores.PulsoReverso;

            }

           

            try
            {
                //Conecta
                if (cl.Dispositivo == 1)
                {
                    mixer = new PaintMixerInterface_P2(p.Slave, p.NomeDispositivo);
                }
                else if(cl.Dispositivo == 2)
                {
                    mixer = new PaintMixerInterface_P2(p.Slave, p.NomeDispositivo2);
                }
                if (mixer != null)
                {
                    mixer.Connect(p.ResponseTimeout);

                    //Aguarda até ficar pronto
                    while (mixer.Status.Busy)
                        Thread.Sleep(1000);

                    //Executa
                    mixer.RunReverse(dist, vel, ace, revDelay, revPulsos);
                    if(aguardeFim)
                    {
                        while (mixer.Status.Busy)
                            Thread.Sleep(1000);
                    }
                }
               
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Operar).Name}: ", e);
			}
			finally
            {
                if (mixer != null)
                {
                    mixer.Disconnect();
                }
            }
        }

        public static void Calibrar_P1(int motor, ValoresVO valores)
        {
            Util.ObjectCalibragem c = Util.ObjectCalibragem.Load(motor);
            PaintMixerInterface_P1 mixer = null;

            int[] dist = new int[12];
            int[] vel = new int[12];
            int[] ace = new int[12];
            int[] revDelay = new int[12];
            int[] revPulsos = new int[12];

            Util.ObjectParametros p = Util.ObjectParametros.Load();

            if (p.SomarPulsoReverso)
            {
                dist[motor] = valores.PulsoHorario + c.UltimoPulsoReverso;
            }
            else
                dist[motor] = valores.PulsoHorario;

            vel[motor] = valores.Velocidade;
            ace[motor] = valores.Aceleracao;

            try
            {
                //Conecta
                mixer = new PaintMixerInterface_P1(p.Slave);
                mixer.Connect(p.ResponseTimeout);

                //Aguarda até ficar pronto
                while (mixer.Status.Busy)
                    Thread.Sleep(1000);

                //Executa
                mixer.RunReverse(dist, vel, ace, valores.Delay, valores.PulsoReverso);
                //Aqui salvando o ultimo Pulso Reverso
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Operar).Name}: ", e);
			}
			finally
            {
                mixer.Disconnect();
            }
        }

        public static bool IsConnected(ref IDispenser dispenser)
        {
            if(dispenser == null)
            {
                return false;
            }
            else
            {
                return dispenser.Connected;
            }
        }

        /// <summary>
        /// Efetua conexão em objeto dispenser passado como referência.
        /// Em caso de falha nna tentativa de conexão, exibi mensagem de confirmação
        /// recursivamente até que a conexão seja efetuada com sucesso ou o usuário
        /// cancele a operação.
        /// </summary>
        /// <param name="dispenser">Dispenser passado como referência para ser conectado.</param>
        /// 
        public static bool Conectar(ref IDispenser dispenser, bool forceMessage = true)
        {
            bool booFullScreenReturn = false;
            bool booRetorno = false;
            Util.ObjectParametros p = Util.ObjectParametros.Load();
            try
            {   
                dispenser.Disconnect();
                dispenser.Connect();
                booRetorno = true;
                CounterComunication = 0;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {typeof(Operar).Name}: ", ex);
			
                dispenser.Disconnect();
               
                if (forceMessage && CounterComunication >= p.QtdTentativasConexao)
                {

                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                    {
                        string mensagem = ErrorMessageHandler.GetFriendlyErrorMessage(ex);

						booFullScreenReturn = m.ShowDialog(
                            mensagem, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        geraEventoFalhaComunicacao(Negocio.IdiomaResxExtensao.Global_Falha_DispositivoSemConectividade);
                    };
                }
                else
                {
                    CounterComunication++;
                    if(CounterComunication <= p.QtdTentativasConexao)
                    {
                        booFullScreenReturn = true;
                    }
                    else
                    {
                        booFullScreenReturn = false;
                    }
                }
               

                if (booFullScreenReturn)
                {
                    Thread.Sleep(2000);
                    booRetorno = Conectar(ref dispenser, forceMessage);
                }
                else
                {
                    booRetorno = false;
                    CounterComunication = 0;
                    Log.Logar(
                        TipoLog.Processo,
                        Util.ObjectParametros.Load().PathLogProcessoDispensa,
                        Negocio.IdiomaResxExtensao.Log_Cod_98 + Negocio.IdiomaResxExtensao.Global_Falha_NaoFoiPossivelConectar + Environment.NewLine + Negocio.IdiomaResxExtensao.Global_ProcessoCanceladoUsuario);
                    geraEventoFalhaComunicacao(Negocio.IdiomaResxExtensao.Global_Falha_NaoFoiPossivelConectar);
                }
            }

            // retorno da function
            return booRetorno;
        }

       
        public static bool ConectarP3(ref ModBusDispenserMover_P3 dispenser, bool forceMessage = true)
        {
            bool booFullScreenReturn = false;
            bool booRetorno = false;
            string tipo_error = "Placa Dispensa";
            Util.ObjectParametros p = Util.ObjectParametros.Load();
            try
            {
                dispenser.Disconnect();                
                dispenser.Connect();
                tipo_error = "Placa Movimentação"; 
                Thread.Sleep(100);
                dispenser.Disconnect_Mover();
                Thread.Sleep(100);               
                dispenser.Connect_Mover(); 
                booRetorno = true;
                CounterComunication = 0;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {typeof(Operar).Name}: ", ex);
			
                Thread.Sleep(100);
                dispenser.Disconnect_Mover();
                Thread.Sleep(100);
                dispenser.Disconnect();

                if (forceMessage && CounterComunication >= p.QtdTentativasConexao)
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                    {
                        string mensagem = ErrorMessageHandler.GetFriendlyErrorMessage(ex);

                        booFullScreenReturn = m.ShowDialog(
                            mensagem, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        geraEventoFalhaComunicacao(Negocio.IdiomaResxExtensao.Global_Falha_DispositivoSemConectividade);
                    };
                }
                else
                {
                    CounterComunication++;
                    if (CounterComunication <= p.QtdTentativasConexao)
                    {
                        booFullScreenReturn = true;
                    }
                    else
                    {
                        booFullScreenReturn = false;
                    }
                }


                if (booFullScreenReturn)
                {
                    Thread.Sleep(2000);
                    booRetorno = ConectarP3(ref dispenser, forceMessage);
                }
                else
                {
                    booRetorno = false;
                    CounterComunication = 0;
                    Log.Logar(
                        TipoLog.Processo,
                        Util.ObjectParametros.Load().PathLogProcessoDispensa,
                        Negocio.IdiomaResxExtensao.Log_Cod_98 + Negocio.IdiomaResxExtensao.Global_Falha_NaoFoiPossivelConectar + Environment.NewLine + Negocio.IdiomaResxExtensao.Global_ProcessoCanceladoUsuario +
                        Environment.NewLine + "Placa:" + tipo_error);
                    geraEventoFalhaComunicacao(Negocio.IdiomaResxExtensao.Global_Falha_NaoFoiPossivelConectar);
                }
            }

            // retorno da function
            return booRetorno;
        }

        static void geraEventoFalhaComunicacao(string detalhes = "")
        {
            try
            {
                #region gravar Evento Inicializacao
                Util.ObjectEventos objEvt = new Util.ObjectEventos();
                objEvt.DATAHORA = DateTime.Now;
                objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.FalhaComunicacaoPlaca;
                objEvt.DETALHES = "0;" + detalhes;
                objEvt.INTEGRADO = false;
                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    objEvt.NUMERO_SERIE = percRegistry.GetSerialNumber();
                }
                Util.ObjectEventos.InsertEvento(objEvt);
                #endregion
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Operar).Name}: ", e);
			}
		}

        public static double CalcularDesvio(double valorMedio, double massaIdeal)
        {
            double desvio = (valorMedio - massaIdeal) / massaIdeal;
            return desvio;
        }

        public static ValoresVO Parser(double volume, List<ValoresVO> listaValores, int pulsosRev)
        {
            ValoresVO superior = null;
            ValoresVO inferior = null;
            ValoresVO valores = null;

            int indexLimite = listaValores.Count - 1;
            for (int index = 0; index <= indexLimite; index++)
            {
                #region Encontra a faixa de volumes

                if (volume == listaValores[index].Volume)
                {
                    valores = new ValoresVO();
                    valores.PulsoHorario = listaValores[index].PulsoHorario;
                    valores.PulsoReverso = listaValores[index].PulsoReverso;
                    valores.Velocidade = listaValores[index].Velocidade;
                    valores.Delay = listaValores[index].Delay;
                    valores.Aceleracao = listaValores[index].Aceleracao;
                    break;
                }

                if (volume > listaValores[index].Volume)
                {
                    superior = (index == 0) ? null : listaValores[index - 1];
                    inferior = listaValores[index];
                    break;
                }

                //Se não encontrou limite inferior da faixa então o volume 
                //é menor que a última posição da lista
                if (inferior == null)
                    superior = listaValores[index];

                #endregion
            }

            if (valores == null)
            {
                valores = new ValoresVO();

                if (superior == null)
                {
                    #region [Valores acima do maior volume]

                    double P = volume * (inferior.PulsoHorario / inferior.Volume);
                    double PR = volume * (inferior.PulsoReverso / inferior.Volume);
                    valores.PulsoHorario = int.Parse(Math.Round(P).ToString());
                    valores.PulsoReverso = inferior.PulsoReverso;
                    valores.Velocidade = inferior.Velocidade;
                    valores.Delay = inferior.Delay;
                    valores.Aceleracao = inferior.Aceleracao;
                   
                    #endregion
                }
                else if (inferior == null)
                {
                    #region [Valores abaixo do menor volume]

                    double P = volume * (superior.PulsoHorario / superior.Volume);
                    double PR = volume * (superior.PulsoReverso / superior.Volume);
                    valores.PulsoHorario = int.Parse(Math.Round(P).ToString());
                    
                    valores.PulsoReverso = superior.PulsoReverso;
                    valores.Velocidade = superior.Velocidade;
                    valores.Delay = superior.Delay;
                    valores.Aceleracao = superior.Aceleracao;
                    #endregion
                }
                else
                {
                    #region[Valores entre maior e menor volume]

                    double VOL = volume;
                    double VOLS = superior.Volume;
                    double VOLI = inferior.Volume;
                    double PULS = superior.PulsoHorario;
                    double PULI = inferior.PulsoHorario;

                    double PULRS = superior.PulsoReverso;
                    double PULRI = inferior.PulsoReverso;

                    double VELS = superior.Velocidade;
                    double VELI = inferior.Velocidade;

                    double DELS = superior.Delay;
                    double DELI = inferior.Delay;

                    double ACCS = superior.Aceleracao;
                    double ACCI = inferior.Aceleracao;

                    //Taxa auxiliar    
                    double TX = 0;
                    if (VOL > 1)
                        TX = (VOL + VOLI) / (VOLS + VOLI);
                    else
                        TX = (VOL - VOLI) / (VOLS - VOLI);

                    //Velocidade
                    double V = VELI + (TX * (VELS - VELI));

                    //Delay
                    double D = DELI + (TX * (DELS - DELI));

                    TX = (VOL - VOLI) / (VOLS - VOLI);
                    if (TX < 0)
                    {
                        TX *= -1;
                    }

                    //Pulso Reverso
                    double PR = PULRI + (TX * (PULRS - PULRI));
                    
                    //Pulsos
                    double P = PULI + (TX * (PULS - PULI));

                    double ac_ = ACCI + (TX * (ACCS - ACCI));

                    valores.PulsoHorario = int.Parse(Math.Round(P).ToString());
                    valores.PulsoReverso = int.Parse(Math.Round(PR).ToString());
                    valores.Velocidade = int.Parse(Math.Round(V).ToString());
                    valores.Delay = int.Parse(Math.Round(D).ToString());
                    valores.Aceleracao = int.Parse(Math.Round(ac_).ToString());

                    #endregion
                }
            }

            //[Soma os pulsos reversos à quantidade final de pulsos]
            if (Util.ObjectParametros.Load().SomarPulsoReverso)
            {
                valores.PulsoHorario += pulsosRev;
            }


            valores.Volume = volume;
            return valores;
        }

        public static bool TemPurgaPendente()
        {
            bool TEM_PURGA_PENDENTE = false;
            Util.ObjectParametros parametros = Util.ObjectParametros.Load();

            TimeSpan? ts = DateTime.Now.Subtract(parametros.DataExecucaoPurga);
            if(ts != null && ts.HasValue && ts.Value.TotalHours > parametros.PrazoExecucaoPurga.TotalHours)
            {
                TEM_PURGA_PENDENTE = true;
            }
            ts = null;
                       
            /* Se o intervalo de horas entra última execução e a hora atual for maior 
             * que o prazo de execução configurado, a purga é considerada pendente*/            
            
            if (TEM_PURGA_PENDENTE)
            {
                Log.Logar(
                    TipoLog.Processo, Util.ObjectParametros.Load().PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_10 + Negocio.IdiomaResxExtensao.Global_PurgaPendente3);
            }

            return TEM_PURGA_PENDENTE;
        }

        public static bool TemRecipiente(IDispenser dispenser, bool mostraMsg = true)
        {
            bool tentarNovamente = false;
            bool temRecipiente = false;

            /* Solicitado por Marcelo em 05/12/2016
             * Se verificação de recipiente estiver desabilitada, 
             * retorna sempre verdadeiro */
            Util.ObjectParametros parametros = Util.ObjectParametros.Load();
            if (!parametros.HabilitarTesteRecipiente)
            {
                return true;
            }

            try
            {
                if (!parametros.HabilitarIdentificacaoCopo)
                {
                    //Verifica entrada digital
                    temRecipiente = dispenser.FirstInput;
                }
                else
                {
                    temRecipiente = dispenser.IdentityPot;
                }

                if (!temRecipiente)
                {
                    if (mostraMsg)
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                        {

                            string mensagem =
                                Negocio.IdiomaResxExtensao.Global_Informacao_RecipienteNaoPosicionado
                                + Environment.NewLine
                                + Negocio.IdiomaResxExtensao.Global_Confirmar_DesejaTentarNovamente;

                            tentarNovamente = m.ShowDialog(
                                mensagem, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                   

                        if (tentarNovamente)
                        {
                            temRecipiente = TemRecipiente(dispenser, mostraMsg);
                        }
                        else
                        {
                            temRecipiente = false;
                            dispenser.Disconnect();

                            Log.Logar(
                                TipoLog.Processo,
                                Util.ObjectParametros.Load().PathLogProcessoDispensa,
                                $"{Negocio.IdiomaResxExtensao.Log_Cod_50 + Negocio.IdiomaResxExtensao.Global_Informacao_RecipienteNaoPosicionado_2} {Negocio.IdiomaResxExtensao.Global_ProcessoCanceladoUsuario}");
                        }
                    }
                }

                return temRecipiente;
            }
            catch (Exception ex)
            {
                string mensagem =
                    Negocio.IdiomaResxExtensao.Global_Falha_TestarRecipiente
                    + Environment.NewLine
                    + ex.Message;
                throw new Exception(mensagem, ex);
            }
        }

        public static bool TemEsponja(IDispenser dispenser, bool mostraMsg = true)
        {
            bool tentarNovamente = false;
            bool temEsponja = false;

            /* Solicitado por Marcelo em 05/12/2016
             * Se verificação de recipiente estiver desabilitada, 
             * retorna sempre verdadeiro */
            Util.ObjectParametros parametros = Util.ObjectParametros.Load();
            if (!parametros.HabilitarTesteRecipiente)
            {
                return true;
            }

            try
            {
                if (!parametros.HabilitarIdentificacaoCopo)
                { 
                    temEsponja = true;
                }
                else
                {
                    temEsponja = dispenser.IdentitySponge;
                }

                if (!temEsponja)
                {
                    if (mostraMsg)
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                        {

                            string mensagem =
                                Negocio.IdiomaResxExtensao.Global_Informacao_RecipienteEsponjaNaoPosicionado
                                + Environment.NewLine
                                + Negocio.IdiomaResxExtensao.Global_Confirmar_DesejaTentarNovamente;

                            tentarNovamente = m.ShowDialog(
                                mensagem, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }


                        if (tentarNovamente)
                        {
                            temEsponja = TemEsponja(dispenser, mostraMsg);
                        }
                        else
                        {
                            temEsponja = false;
                            dispenser.Disconnect();

                            Log.Logar(
                                TipoLog.Processo,
                                Util.ObjectParametros.Load().PathLogProcessoDispensa,
                                $"{Negocio.IdiomaResxExtensao.Log_Cod_50 + Negocio.IdiomaResxExtensao.Global_Informacao_RecipienteNaoPosicionado_2} {Negocio.IdiomaResxExtensao.Global_ProcessoCanceladoUsuario}");
                        }
                    }
                }

                return temEsponja;
            }
            catch (Exception ex)
            {
                string mensagem =
                    Negocio.IdiomaResxExtensao.Global_Falha_TestarRecipiente
                    + Environment.NewLine
                    + ex.Message;
                throw new Exception(mensagem, ex);
            }
        }

        #region TemColoranteSuficiente

        static void geraEventoNivelBaixo(string detalhes="")
        {
            try
            {
                #region gravar Evento Inicializacao
                Util.ObjectEventos objEvt = new Util.ObjectEventos();
                objEvt.DATAHORA = DateTime.Now;
                objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.NivelCanisterBaixo;
                objEvt.DETALHES = "0;" + detalhes;
                objEvt.INTEGRADO = false;
                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    objEvt.NUMERO_SERIE = percRegistry.GetSerialNumber();
                }
                Util.ObjectEventos.InsertEvento(objEvt);
                #endregion
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Operar).Name}: ", e);
			}
		}

        static bool TemColoranteSuficiente(int circuito, double volume, bool executaEvento = true)
        {
            Util.ObjectColorante c = Util.ObjectColorante.Load(circuito);

            bool Dat_usado_tcp_12 = false;
            Util.ObjectParametros _parametros = Util.ObjectParametros.Load();
            switch ((DatPattern)_parametros.PadraoConteudoDAT)
            {
                #region Padrão de dat configurado

                case DatPattern.Padrao12:
                    {
                        Dat_usado_tcp_12 = true;

                        break;
                    }
                    #endregion
            }
            double VOLUME_ATUAL = c.Volume;
            double VOLUME_A_SER_DISPENSADO = volume;
            if (Dat_usado_tcp_12)
            {
                VOLUME_A_SER_DISPENSADO = UnidadeMedidaHelper.GramaToMililitro(volume, c.MassaEspecifica);
            }
            
            double VOLUME_MINIMO = c.NivelMinimo;
            //Nível de colorante não pode ficar abaixo do mínimo configurado
            bool TEM_COLORANTE_SUFICIENTE =
                ((VOLUME_ATUAL - VOLUME_A_SER_DISPENSADO) >= VOLUME_MINIMO);

            if(!TEM_COLORANTE_SUFICIENTE && executaEvento)
            {
                string detalhes_evt = c.Nome + ", " + Math.Round(volume, 3).ToString();
                geraEventoNivelBaixo(detalhes_evt);
            }

            return TEM_COLORANTE_SUFICIENTE;
        }

        public static bool TemColoranteSuficiente(Util.ObjectFormula formula, out List<Util.ObjectColorante> excedentes)
        {
            excedentes = new List<Util.ObjectColorante>();
            string detalhes_evt = "";

            bool Dat_usado_tcp_12 = false;
            Util.ObjectParametros _parametros = Util.ObjectParametros.Load();
            switch ((DatPattern)_parametros.PadraoConteudoDAT)
            {
                #region Padrão de dat configurado

                case DatPattern.Padrao12:
                    {
                        Dat_usado_tcp_12 = true;

                        break;
                    }
                    #endregion
            }

            foreach (Util.ObjectFormulaItem item in formula.Itens)
            {
                double volume = item.Mililitros;
                if(Dat_usado_tcp_12)
                {
                    volume = UnidadeMedidaHelper.MililitroToGrama(volume, item.Colorante.MassaEspecifica);
                }

                if (!TemColoranteSuficiente(item.Colorante.Circuito, volume, false))
                {
                    excedentes.Add(item.Colorante);
                    if(detalhes_evt =="")
                    {
                        detalhes_evt += item.Colorante.Circuito.ToString() + "," + item.Colorante.Nome + "," + Math.Round(item.Mililitros, 3).ToString();
                    }
                    else
                    {
                        detalhes_evt += "," + item.Colorante.Circuito.ToString() + "," + item.Colorante.Nome + "," + Math.Round(item.Mililitros, 3).ToString();
                    }
                }
            }

            bool TEM_COLORANTE_SUFICIENTE = (excedentes.Count == 0);
            if (!TEM_COLORANTE_SUFICIENTE)
            {
                Log.Logar(
                    TipoLog.Processo,
                    Util.ObjectParametros.Load().PathLogProcessoDispensa,
                    Negocio.IdiomaResxExtensao.Global_Informacao_NivelColoranteInsuficiente);
                geraEventoNivelBaixo(detalhes_evt);
            }

            return TEM_COLORANTE_SUFICIENTE;
        }

        public static bool TemColoranteSuficiente(Dictionary<Util.ObjectColorante, double> lista_dispensa,
            out List<Util.ObjectColorante> excedentes)
        {
            string detalhes_evt = "";
            excedentes = new List<Util.ObjectColorante>();

            foreach (KeyValuePair<Util.ObjectColorante, double> item in lista_dispensa)
            {
                if (!TemColoranteSuficiente(item.Key.Circuito, item.Value, false))
                {
                    excedentes.Add(item.Key);
                    if (detalhes_evt == "")
                    {
                        detalhes_evt += item.Key.Circuito.ToString() + "," + item.Key.Nome + "," + Math.Round(item.Value, 3).ToString();
                    }
                    else
                    {
                        detalhes_evt += "," + item.Key.Circuito.ToString() + "," + item.Key.Nome + "," + Math.Round(item.Value, 3).ToString();
                    }
                }
                    
            }

            bool TEM_COLORANTE_SUFICIENTE = (excedentes.Count == 0);
            if (!TEM_COLORANTE_SUFICIENTE)
            {
                Log.Logar(
                    TipoLog.Processo,
                    Util.ObjectParametros.Load().PathLogProcessoDispensa,
                    Negocio.IdiomaResxExtensao.Global_Informacao_NivelColoranteInsuficiente);
                geraEventoNivelBaixo(detalhes_evt);
            }

            return TEM_COLORANTE_SUFICIENTE;
        }

        public static bool TemColoranteSuficiente(Dictionary<int, double> lista_dispensa,
          out List<Util.ObjectColorante> excedentes)
        {
            string detalhes_evt = "";
            excedentes = new List<Util.ObjectColorante>();

            foreach (KeyValuePair<int, double> item in lista_dispensa)
            {
               
                if (!TemColoranteSuficiente(item.Key, item.Value, false))
                {
                    Util.ObjectColorante objcol = Util.ObjectColorante.Load(item.Key);
                    excedentes.Add(objcol);
                    if (detalhes_evt == "")
                    {
                        detalhes_evt += objcol.Circuito.ToString() + "," + objcol.Nome + "," + Math.Round(item.Value, 3).ToString();
                    }
                    else
                    {
                        detalhes_evt += "," + objcol.Circuito.ToString() + "," + objcol.Nome + "," + Math.Round(item.Value, 3).ToString();
                    }
                }
            }

            bool TEM_COLORANTE_SUFICIENTE = (excedentes.Count == 0);
            if (!TEM_COLORANTE_SUFICIENTE)
            {
                Log.Logar(
                    TipoLog.Processo,
                    Util.ObjectParametros.Load().PathLogProcessoDispensa,
                    Negocio.IdiomaResxExtensao.Global_Informacao_NivelColoranteInsuficiente);
            }

            return TEM_COLORANTE_SUFICIENTE;
        }

        #endregion

        #region AbaterColorante

        public static void AbaterColorante(int circuito, double volume)
        {
            Util.ObjectColorante colorante = Util.ObjectColorante.Load(circuito);
            bool Dat_usado_tcp_12 = false;
            Util.ObjectParametros _parametros = Util.ObjectParametros.Load();
            switch ((DatPattern)_parametros.PadraoConteudoDAT)
            {
                #region Padrão de dat configurado

                case DatPattern.Padrao12:
                    {
                        Dat_usado_tcp_12 = true;

                        break;
                    }
                    #endregion
            }
            if (Dat_usado_tcp_12)
            {
                colorante.Volume -= UnidadeMedidaHelper.GramaToMililitro((volume / 1000), colorante.MassaEspecifica);
            }
            else
            {
                colorante.Volume -= volume;
            }
            Util.ObjectColorante.Persist(colorante);
        }

        public static void AbaterColorante(Dictionary<int, double> lista)
        {
            foreach (KeyValuePair<int, double> item in lista)
            {
                Util.ObjectColorante colorante = Util.ObjectColorante.Load(item.Key);
                bool Dat_usado_tcp_12 = false;
                Util.ObjectParametros _parametros = Util.ObjectParametros.Load();
                switch ((DatPattern)_parametros.PadraoConteudoDAT)
                {
                    #region Padrão de dat configurado

                    case DatPattern.Padrao12:
                        {
                            Dat_usado_tcp_12 = true;

                            break;
                        }
                        #endregion
                }
                if (Dat_usado_tcp_12)
                {
                    colorante.Volume -= UnidadeMedidaHelper.GramaToMililitro((item.Value), colorante.MassaEspecifica);
                }
                else
                {
                    colorante.Volume -= item.Value;
                }
                
                Util.ObjectColorante.Persist(colorante);
            }
        }

        #endregion

        #region getVersion Resset
        public static string GetVersion(ModBusDispenser_P2 mdp2)
        {
            string retorno = "";
            IDispenser disp = (IDispenser)mdp2;
            try
            {                
                if (Conectar(ref disp, false))
                {
                    Thread.Sleep(100);
                    retorno = mdp2.GetVersion();
                }                
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Operar).Name}: ", e);
			}
			finally
            {
                try
                {
                    disp.Disconnect();
                }
				catch (Exception e)
				{
					LogManager.LogError($"Erro no módulo {typeof(Operar).Name}: ", e);
				}
			}

            return retorno;
        }
        public static bool RessetHard(ModBusDispenser_P2 mdp2, int board)
        {
            bool retorno = false;
            IDispenser disp = (IDispenser)mdp2;
            try
            {                
                if (Conectar(ref disp, false))
                {
                    Thread.Sleep(100);
                    retorno = mdp2.RessetHard();
                    gerarEventoRessetplaca(0, board.ToString());
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Operar).Name}: ", e);
			}
			finally
            {
                try
                {
                    disp.Disconnect();
                }
				catch (Exception e)
				{
					LogManager.LogError($"Erro no módulo {typeof(Operar).Name}: ", e);
				}
			}
            return retorno;
        }

        private static int gerarEventoRessetplaca(int result, string detalhes = "")
        {
            int retorno = 0;
            try
            {
                #region gravar Evento Resset Placa
                Util.ObjectEventos objEvt = new Util.ObjectEventos();
                objEvt.DATAHORA = DateTime.Now;
                objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.RessetPlaca;
                objEvt.DETALHES = result.ToString() + ";" + detalhes;
                objEvt.INTEGRADO = false;
                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    objEvt.NUMERO_SERIE = percRegistry.GetSerialNumber();
                }
                retorno = Util.ObjectEventos.InsertEvento(objEvt);
                #endregion
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Operar).Name}: ", e);
			}

			return retorno;
        }
        #endregion
    }
}