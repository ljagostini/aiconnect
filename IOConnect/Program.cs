using Microsoft.Extensions.Configuration;
using Percolore.Core;
using Percolore.Core.Logging;
using Percolore.Core.Persistence.WindowsRegistry;
using Percolore.Core.Persistence.Xml;

namespace Percolore.IOConnect
{
	static class Program
    {
        [STAThread]
        static void Main()
        {
			// Lendo as configurações do appsettings.json
			var config = new ConfigurationBuilder()
				.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.Build();

			// Inicializando o logger com base no appsettings.json
			LogManager.InitializeLogger(config);

			// Configura os handlers de exceções não tratadas na aplicação.
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();
            string log_inicializacao = string.Empty;
            
            #region migrando xml to SQLITE 

            #region excluindo os arquivo db caso exista xml referente por default
            if (File.Exists(Parametros.PathFile))
            {
                if (File.Exists(Util.ObjectParametros.PathFile))
                {
                    File.Delete(Util.ObjectParametros.PathFile);
                    Thread.Sleep(2000);
                }
            }

            if (File.Exists(Colorante.PathFile))
            {
                if (File.Exists(Util.ObjectColorante.PathFile))
                {
                    File.Delete(Util.ObjectColorante.PathFile);
                    Thread.Sleep(2000);
                }
            }

            if (File.Exists(Formula.PathFile))
            {
                if (File.Exists(Util.ObjectFormula.PathFile))
                {
                    File.Delete(Util.ObjectFormula.PathFile);
                    Thread.Sleep(2000);
                }
            }

            if (File.Exists(Calibragem.PathFile))
            {
                if (File.Exists(Util.ObjectCalibragem.PathFile))
                {
                    File.Delete(Util.ObjectCalibragem.PathFile);
                    Thread.Sleep(2000);
                }
            }
            #endregion

            if (!File.Exists(Util.ObjectParametros.PathFile))
            {
                /*Atualiza estrutura do arquivo xml*/
                Parametros.UpdateStructure();

                /*Persiste valores padrões*/
                Parametros _parametros = Parametros.Load();
                Parametros.Persist(_parametros);


                Util.ObjectParametros.CreateBD();
                Parametros objPar = Parametros.Load();

                Util.ObjectParametros retorno = new Util.ObjectParametros();

                #region Geral

                retorno.ResponseTimeout = objPar.ResponseTimeout;
                retorno.Velocidade = objPar.Velocidade;

                retorno.Aceleracao = objPar.Aceleracao;
                retorno.DelayReverso = objPar.DelayReverso;
                retorno.PulsoReverso = objPar.PulsoReverso;
                retorno.SomarPulsoReverso = objPar.SomarPulsoReverso;
                retorno.HabilitarTecladoVirtual = objPar.HabilitarTecladoVirtual;
                retorno.HabilitarDispensaSequencial = objPar.HabilitarDispensaSequencial;
                retorno.HabilitarFormulaPersonalizada = objPar.HabilitarFormulaPersonalizada;
                retorno.HabilitarTesteRecipiente = objPar.HabilitarTesteRecipiente;
                retorno.IdIdioma = objPar.IdIdioma;
                retorno.IdDispositivo = objPar.IdDispositivo;
                retorno.HabilitarPurgaIndividual = objPar.HabilitarPurgaIndividual;
                retorno.HabilitarTouchScrenn = objPar.HabilitarTouchScrenn;
                retorno.IdDispositivo2 = objPar.IdDispositivo2;
                retorno.NomeDispositivo = objPar.NomeDispositivo;
                retorno.NomeDispositivo2 = objPar.NomeDispositivo2;
                retorno.VersaoIoconnect = objPar.VersaoIoconnect;
                retorno.HabilitarDispensaSequencialP1 = objPar.HabilitarDispensaSequencialP1;
                retorno.HabilitarDispensaSequencialP2 = objPar.HabilitarDispensaSequencialP2;

                #endregion

                #region DAT

                retorno.PathMonitoramentoDAT = objPar.PathMonitoramentoDAT;
                retorno.PathRepositorioDAT = objPar.PathRepositorioDAT;
                retorno.PadraoConteudoDAT = objPar.PadraoConteudoDAT;
                retorno.BasePosicaoCircuitoDAT = objPar.BasePosicaoCircuitoDAT;
                retorno.UtilizarCorrespondenciaDAT = objPar.UtilizarCorrespondenciaDAT;
                retorno.DesabilitarInterfaceDispensaSequencial = objPar.DesabilitarInterfaceDispensaSequencial;
                retorno.DesabilitarInterfaceDispensaSimultanea = objPar.DesabilitarInterfaceDispensaSimultanea;
                retorno.DesabilitarInterfaceInicializacaoCircuito = objPar.DesabilitarInterfaceInicializacaoCircuito;
                retorno.DesabilitarInterfacePurga = objPar.DesabilitarInterfacePurga;

                #endregion

                #region Purga

                retorno.PrazoExecucaoPurga = objPar.PrazoExecucaoPurga;
                retorno.DataExecucaoPurga = objPar.DataExecucaoPurga;
                retorno.VolumePurga = objPar.VolumePurga;
                retorno.VelocidadePurga = objPar.VelocidadePurga;
                retorno.AceleracaoPurga = objPar.AceleracaoPurga;
                retorno.DelayPurga = objPar.DelayPurga;
                retorno.ControlarExecucaoPurga = objPar.ControlarExecucaoPurga;
                retorno.ExigirExecucaoPurga = objPar.ExigirExecucaoPurga;
                retorno.PurgaSequencial = objPar.PurgaSequencial;

                #endregion

                #region Controle de volume

                retorno.VolumeMinimo = objPar.VolumeMinimo;
                retorno.VolumeMaximo = objPar.VolumeMaximo;
                retorno.ControlarNivel = objPar.ControlarNivel;

                #endregion

                #region Inicialização dos circuitos

                retorno.IniPulsoInicial = objPar.IniPulsoInicial;
                retorno.IniPulsoLimite = objPar.IniPulsoLimite;
                retorno.IniVariacaoPulso = objPar.IniVariacaoPulso;
                retorno.IniStepVariacao = objPar.IniStepVariacao;
                retorno.IniVelocidade = objPar.IniVelocidade;
                retorno.IniAceleracao = objPar.IniAceleracao;
                retorno.IniMovimentoReverso = objPar.IniMovimentoReverso;
                retorno.InicializarCircuitosPurga = objPar.InicializarCircuitosPurga;
                retorno.InicializarCircuitosPurgaIndividual = objPar.InicializarCircuitosPurgaIndividual;
                retorno.QtdeCircuitoGrupo = objPar.QtdeCircuitoGrupo;

                #endregion

                #region Unidade de medida

                retorno.ValorShot = objPar.ValorShot;
                retorno.HabilitarShot = objPar.HabilitarShot;
                retorno.HabilitarOnca = objPar.HabilitarOnca;
                retorno.HabilitarMililitro = objPar.HabilitarMililitro;
                retorno.HabilitarGrama = objPar.HabilitarGrama;
                retorno.UnidadeMedidaNivelColorante = objPar.UnidadeMedidaNivelColorante;

                #endregion

                #region Log

                retorno.PathLogProcessoDispensa = objPar.PathLogProcessoDispensa;
                retorno.PathLogControleDispensa = objPar.PathLogControleDispensa;
                retorno.HabilitarLogComunicacao = objPar.HabilitarLogComunicacao;
                retorno.PathLogComunicacao = objPar.PathLogComunicacao;

                #endregion

                #region Monitoramento dos circuitos

                retorno.QtdeMonitCircuitoGrupo = objPar.QtdeMonitCircuitoGrupo;
                retorno.MonitVelocidade = objPar.MonitVelocidade;
                retorno.MonitAceleracao = objPar.MonitAceleracao;
                retorno.MonitDelay = objPar.MonitDelay;
                retorno.MonitTimerDelay = objPar.MonitTimerDelay;
                retorno.MonitTimerDelayIni = objPar.MonitTimerDelayIni;
                retorno.DesabilitarInterfaceMonitCircuito = objPar.DesabilitarInterfaceMonitCircuito;
                retorno.DesabilitarProcessoMonitCircuito = objPar.DesabilitarProcessoMonitCircuito;
                retorno.MonitMovimentoReverso = objPar.MonitMovimentoReverso;
                retorno.MonitPulsos = objPar.MonitPulsos;

                #endregion

                #region Producao

                retorno.TipoProducao = objPar.TipoProducao;

                retorno.IpProducao = objPar.IpProducao;

                retorno.PortaProducao = objPar.PortaProducao;

                retorno.DesabilitaMonitProcessoProducao = objPar.DesabilitaMonitProcessoProducao;

                #endregion

                #region Sinc Formula
                try
                {
                    retorno.DesabilitaMonitSincFormula = objPar.DesabilitaMonitSincFormula;

                    retorno.PortaSincFormula = objPar.PortaSincFormula;

                    retorno.IpSincFormula = objPar.IpSincFormula;

                }
				catch (Exception e)
				{
					LogManager.LogError($"Erro no módulo {typeof(Program).Name}: ", e);
				}

				#endregion

				Util.ObjectParametros.PersistInsert(retorno);

                Util.ObjectParametros.InitLoad();

            }

            if (!File.Exists(Util.ObjectColorante.PathFile))
            {
                Util.ObjectColorante.CreateBD();

                List<Colorante> lCor = Colorante.List();
                List<Util.ObjectColorante> lObjCor = new List<Util.ObjectColorante>();
                foreach (Colorante cor in lCor)
                {
                    Util.ObjectColorante objCor = new Util.ObjectColorante();
                    objCor.Circuito = cor.Circuito;
                    objCor.Correspondencia = cor.Correspondencia;
                    objCor.Dispositivo = cor.Dispositivo;
                    objCor.Habilitado = cor.Habilitado;
                    objCor.MassaEspecifica = cor.MassaEspecifica;
                    objCor.NivelMaximo = cor.NivelMaximo;
                    objCor.NivelMinimo = cor.NivelMinimo;
                    objCor.Nome = cor.Nome;
                    objCor.Volume = cor.Volume;
                    objCor.IsBase = false;
                    objCor.Seguidor = -1;
                    objCor.IsBicoIndividual = false;
                    objCor.VolumePurga = 5;
                    objCor.VolumeBicoIndividual = 0.0;

                    lObjCor.Add(objCor);
                }

                Util.ObjectParametros retorno = new Util.ObjectParametros();
                Util.ObjectParametros.InitLoad();
                retorno = Util.ObjectParametros.Load();

                if (retorno != null)
                {
                    for (int i = lCor.Count + 1; i <= 32; i++)
                    {
                        Util.ObjectColorante objCor = new Util.ObjectColorante();
                        objCor.Circuito = i;
                        objCor.Correspondencia = i;
                        objCor.Dispositivo = 2;
                        objCor.Habilitado = false;
                        objCor.MassaEspecifica = 0;
                        objCor.NivelMaximo = retorno.VolumeMaximo;
                        objCor.NivelMinimo = retorno.VolumeMinimo;
                        objCor.Nome = "";
                        objCor.Volume = 0;
                        objCor.IsBase = false;
                        objCor.Seguidor = -1;
                        objCor.IsBicoIndividual = false;
                        objCor.VolumeBicoIndividual = 0.0;
                        lObjCor.Add(objCor);
                    }

                    Util.ObjectColorante.Persist(lObjCor);
                }
                else
                    LogManager.LogInformation("Parâmetros de colorante não encontrados.");
            }

            if (!File.Exists(Util.ObjectFormula.PathFile))
            {
                Util.ObjectFormula.CreateBD();

                List<Formula> lFor = Formula.List();

                foreach (Formula formula in lFor)
                {
                    Util.ObjectFormula objFor = new Util.ObjectFormula();
                    objFor.Itens = new List<Util.ObjectFormulaItem>();
                    objFor.Nome = formula.Nome;
                    foreach (FormulaItem fitem in formula.Itens)
                    {
                        Util.ObjectFormulaItem item = new Util.ObjectFormulaItem();
                        item.IdColorante = fitem.IdColorante;
                        item.Mililitros = fitem.Mililitros;
                        objFor.Itens.Add(item);
                    }
                    Util.ObjectFormula.Persist(objFor);
                }

            }

            if (!File.Exists(Util.ObjectCalibragem.PathFile))
            {
                Util.ObjectCalibragem.CreateBD();
                Util.ObjectCalibracaoAutomatica.CreateBD();

                Util.ObjectCalibracaoAutomatica _calibracao = new Util.ObjectCalibracaoAutomatica();

                _calibracao.CapacideMaxBalanca = 420;
                _calibracao.MaxMassaAdmRecipiente = 100;
                _calibracao.NumeroMaxTentativaRec = 3;
                _calibracao.VolumeMaxRecipiente = 200;
                _calibracao._calibragem = new Util.ObjectCalibragem();
                _calibracao._calibragem.Motor = 1;
                _calibracao.listOperacaoAutomatica = new List<Negocio.OperacaoAutomatica>();
                Util.ObjectCalibracaoAutomatica.Add(_calibracao, true);

                List<Calibragem> lCal = new List<Calibragem>();
                for (int i = 1; i <= 32; i++)
                {
                    try
                    {
                        Calibragem cal = Calibragem.Load(i);
                        lCal.Add(cal);
                    }
                    catch
					{
						LogManager.LogInformation($"Calibração não encontrada para o motor '{i}'. Carregando última calibração válida.");
					
                        if (i > 16)
                        {
                            try
                            {
                                Calibragem cal = Calibragem.Load(i - 16);
                                cal.Motor = i;
                                lCal.Add(cal);
                            }
							catch (Exception ex)
							{
								LogManager.LogError($"Não foi possível carregar calibração válida para o motor {i}.", ex);
							}
						}
                    }
                }
                foreach (Calibragem cal in lCal)
                {
                    Util.ObjectCalibragem objCal = new Util.ObjectCalibragem();
                    objCal.Motor = cal.Motor;
                    objCal.UltimoPulsoReverso = cal.UltimoPulsoReverso;
                    objCal.Valores = new List<ValoresVO>();
                    foreach (ValoresVO vO in cal.Valores)
                    {
                        ValoresVO _vO = new ValoresVO();
                        _vO.Aceleracao = vO.Aceleracao;
                        _vO.Delay = vO.Delay;
                        _vO.DesvioMedio = vO.DesvioMedio;
                        _vO.MassaMedia = vO.MassaMedia;
                        _vO.PulsoHorario = vO.PulsoHorario;
                        _vO.PulsoReverso = vO.PulsoReverso;
                        _vO.Velocidade = vO.Velocidade;
                        _vO.Volume = vO.Volume;
                        objCal.Valores.Add(_vO);
                    }
                    Util.ObjectCalibragem.Add(objCal);
                }

            }

            if (!File.Exists(Util.ObjectRecircular.PathFile))
            {
                Util.ObjectRecircular.CreateBD();
                if (File.Exists(Util.ObjectRecircular.PathFile))
                {
                    List<Util.ObjectColorante> lCol = Util.ObjectColorante.List();
                    List<Util.ObjectRecircular> lRecirc = new List<Util.ObjectRecircular>();
                    DateTime dtAgora = DateTime.Now;
                    foreach (Util.ObjectColorante _col in lCol)
                    {
                        Util.ObjectRecircular _rec = new Util.ObjectRecircular();
                        _rec.Circuito = _col.Circuito;
                        _rec.Dias = 10;
                        _rec.Habilitado = false;
                        _rec.VolumeDin = 0.03;
                        _rec.VolumeRecircular = 0.5;
                        _rec.VolumeDosado = 0;
                        _rec.DtInicio = dtAgora;
                        lRecirc.Add(_rec);
                    }
                    Util.ObjectRecircular.Persist(lRecirc);
                }
            }

            if (!File.Exists(Util.ObjectUser.PathFile))
            {
                Util.ObjectUser.CreateBD();

                Util.ObjectUser userInit = new Util.ObjectUser();

                userInit.Nome = "master";
                userInit.Senha = "1500k";
                userInit.Tecnico = false;
                userInit.Tipo = 1;

                Util.ObjectUser.Persist(userInit);

                Util.ObjectUser userTecnico = new Util.ObjectUser();

                userTecnico.Nome = "tec";
                userTecnico.Senha = "1234";
                userTecnico.Tecnico = true;
                userTecnico.Tipo = 2;
                Util.ObjectUser.Persist(userTecnico);

                Util.ObjectUser userMan = new Util.ObjectUser();

                userMan.Nome = "manager";
                userMan.Senha = "avsb";
                userMan.Tecnico = false;
                userMan.Tipo = 3;

                Util.ObjectUser.Persist(userMan);

            }

            if (!File.Exists(Util.ObjectBasDat06.PathFile))
            {
                Util.ObjectBasDat06.CreateBD();

                Util.ObjectBasDat06 Accent = new Util.ObjectBasDat06();
                Accent.Name = "ACCENT";
                Accent.Circuito = 14;
                Accent.Volume = 0.92;

                Util.ObjectBasDat06.Persist(Accent);

                Util.ObjectBasDat06 Neutral = new Util.ObjectBasDat06();
                Neutral.Name = "NEUTRAL";
                Neutral.Circuito = 15;
                Neutral.Volume = 0.82;

                Util.ObjectBasDat06.Persist(Neutral);

                Util.ObjectBasDat06 White_ = new Util.ObjectBasDat06();
                White_.Name = "WHITE";
                White_.Circuito = 16;
                White_.Volume = 0.935;

                Util.ObjectBasDat06.Persist(White_);

            }

            if (!File.Exists(Util.ObjectMotorPlacaMovimentacao.PathFile))
            {
                Util.ObjectMotorPlacaMovimentacao.CreateBD();
                if (File.Exists(Util.ObjectMotorPlacaMovimentacao.PathFile))
                {
                    List<Util.ObjectMotorPlacaMovimentacao> lPlaMov = new List<Util.ObjectMotorPlacaMovimentacao>();
                    Util.ObjectMotorPlacaMovimentacao plMov = new Util.ObjectMotorPlacaMovimentacao();
                    plMov.Circuito = 1;
                    plMov.NameTag = "Gaveta";
                    plMov.TipoMotor = 0;
                    plMov.Pulsos = 100;
                    plMov.Aceleracao = 100;
                    plMov.Delay = 100;
                    plMov.Habilitado = true;
                    plMov.Velocidade = 100;
                    lPlaMov.Add(plMov);

                    plMov = new Util.ObjectMotorPlacaMovimentacao();
                    plMov.Circuito = 2;
                    plMov.NameTag = "Válvula";
                    plMov.TipoMotor = 0;
                    plMov.Pulsos = 100;
                    plMov.Aceleracao = 100;
                    plMov.Delay = 100;
                    plMov.Habilitado = true;
                    plMov.Velocidade = 100;
                    lPlaMov.Add(plMov);

                    plMov = new Util.ObjectMotorPlacaMovimentacao();
                    plMov.Circuito = 3;
                    plMov.NameTag = "Bico";
                    plMov.TipoMotor = 0;
                    plMov.Pulsos = 100;
                    plMov.Aceleracao = 100;
                    plMov.Delay = 100;
                    plMov.Habilitado = true;
                    plMov.Velocidade = 100;
                    lPlaMov.Add(plMov);

                    Util.ObjectMotorPlacaMovimentacao.Persist(lPlaMov);

                }
            }

            if (!File.Exists(Util.ObjectBasDat05.PathFile))
            {
                Util.ObjectBasDat05.CreateBD();

                Util.ObjectBasDat05 Accent = new Util.ObjectBasDat05();
                Accent.Name = "ACCENT";
                Accent.Circuito = 14;
                Accent.Volume = 0.92;

                Util.ObjectBasDat05.Persist(Accent);

                Util.ObjectBasDat05 Neutral = new Util.ObjectBasDat05();
                Neutral.Name = "NEUTRAL";
                Neutral.Circuito = 15;
                Neutral.Volume = 0.82;

                Util.ObjectBasDat05.Persist(Neutral);

                Util.ObjectBasDat05 White_ = new Util.ObjectBasDat05();
                White_.Name = "WHITE";
                White_.Circuito = 16;
                White_.Volume = 0.935;

                Util.ObjectBasDat05.Persist(White_);
            }


            if (!File.Exists(Util.ObjectAbastecimento.PathFile))
            {
                Util.ObjectAbastecimento.CreateBD();
            }

            if (!File.Exists(Util.ObjectEventos.PathFile))
            {
                Util.ObjectEventos.CreateBD();
            }

            #region excluindo os Xml

            if (File.Exists(Parametros.PathFile))
            {
                File.Delete(Parametros.PathFile);
                Thread.Sleep(2000);
            }

            if (File.Exists(Colorante.PathFile))
            {
                File.Delete(Colorante.PathFile);
                Thread.Sleep(2000);
            }

            if (File.Exists(Formula.PathFile))
            {
                File.Delete(Formula.PathFile);
                Thread.Sleep(2000);
            }

            if (File.Exists(Calibragem.PathFile))
            {
                File.Delete(Calibragem.PathFile);
                Thread.Sleep(2000);
            }
            #endregion

            #endregion

            Util.ObjectParametros parametros = Util.ObjectParametros.Load();

            if (parametros != null)
            {
                Negocio.IdiomaResx.GetIDiomaREsx(parametros.IdIdioma);
                Init.DefineCultura();
            }
            else
                LogManager.LogInformation("Parâmetros de idioma não encontrados.");

            try
            {
                if (!File.Exists(Util.ObjectMensagem.PathFile))
                {
                    Util.ObjectMensagem.CreateBD();
                }

                Util.ObjectMensagem.LoadMessage();

                if (!File.Exists(Util.ObjectLimpBicos.PathFile))
                {
                    Util.ObjectLimpBicos.CreateBD();
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Program).Name}: ", e);
			}

			if (!Init.AtualizacaoBD())
            {
                #region Erro ao atualizar o processo de atualização

                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Informacao_AplicativoAtualizacaoBD);
                }

                return;

                #endregion
            }


            if (Init.ProcessoEmExecucao())
            {
                #region Verifica se processo está em execução

                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Informacao_AplicativoEmExecucao);
                }

                return;

                #endregion
            }

            string report = string.Empty;
            if (!Init.PreRequisitosOk(out report))
            {
                #region  Valida pré-requisitos

                string mensagem =
                    Negocio.IdiomaResxExtensao.Global_Informacao_RequisitosNaoAtendidos
                    + Environment.NewLine + Environment.NewLine
                    + report;

                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(mensagem);
                }

                return;

                #endregion
            }

            if (!Init.RegistroAtualizadoOk())
            {
                #region Executa atualizações e correções

                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_NaoFoiPossivelAtualizar);
                }

                return;

                #endregion
            }

            #region Logar inicialização

            if (!Init.NumeroSerialOk())
                return;

            if (!Init.LicencaOk())
                return;

            if (!Init.ManutencaoOk())
                return;

            if (!Init.CreateFileVersion())
                return;

            if ((Dispositivo)parametros.IdDispositivo == Dispositivo.Simulador)
            {
                log_inicializacao = Negocio.IdiomaResxExtensao.Log_Cod_52 + Negocio.IdiomaResxExtensao.Global_IOConnectInicializadoModoSimulacao;
            }
            else
            {
                log_inicializacao = Negocio.IdiomaResxExtensao.Log_Cod_51 + Negocio.IdiomaResxExtensao.Global_IOConnectInicializado;
            }

            Log.Logar(TipoLog.Processo, parametros.PathLogProcessoDispensa, log_inicializacao);

            if (parametros.ControlarNivel)
            {
                List<Util.ObjectColorante> colorantes =
                    Util.ObjectColorante.List().Where(s => s.Habilitado && s.Seguidor == -1).ToList();
                string log_nivel_colorante = "";
                foreach (Util.ObjectColorante c in colorantes)
                    log_nivel_colorante += $"{c.Circuito },{Math.Round(c.Volume, 3)},";

                log_nivel_colorante =
                    log_nivel_colorante.Remove(log_nivel_colorante.Length - 1);

                Log.Logar(
                    TipoLog.Processo, parametros.PathLogProcessoDispensa, log_nivel_colorante);
            }

            try
            {
                #region gravar Evento Inicializacao
                Util.ObjectEventos objEvt = new Util.ObjectEventos();
                objEvt.DATAHORA = DateTime.Now;
                objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.InicalizarSistema;
                objEvt.INTEGRADO = false;
                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    objEvt.NUMERO_SERIE = percRegistry.GetSerialNumber();
                }

                List<Util.ObjectColorante> colorantes = Util.ObjectColorante.List().Where(s => s.Habilitado && s.Seguidor == -1).ToList();
                string detalhes = "";
                foreach (Util.ObjectColorante objC in colorantes)
                {
                    if (detalhes == "")
                    {
                        detalhes = "0;" + objC.Circuito.ToString() + "," + objC.Nome + "," + Math.Round(objC.Volume, 3).ToString();
                    }
                    else
                    {
                        detalhes += "," + objC.Circuito.ToString() + "," + objC.Nome + "," + Math.Round(objC.Volume, 3).ToString();
                    }
                }
                objEvt.DETALHES = detalhes;
                Util.ObjectEventos.InsertEvento(objEvt);
                #endregion
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Program)}: ", e);
			}

			parametros = null;

            #endregion  
            try
            {
                if (!Directory.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "bkp"))
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "bkp");
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Program).Name}: ", e);
			}

			#region excluindo 48 Motores
			#region Colorantes
			List<Util.ObjectColorante> l_Col_exc = Util.ObjectColorante.List();
            if (l_Col_exc.Count > 32)
            {
                for(int i = 33; i<= l_Col_exc.Count; i++)
                {
                    Util.ObjectColorante.Delete(i);
                }
            }
            #endregion
            #region Calibragem
            List<Util.ObjectCalibragem> lCal_exc = Util.ObjectCalibragem.List();
            for (int i = 33; i <= lCal_exc.Count; i++)
            {
                Util.ObjectCalibragem.Delete(i);
            }
            #endregion
            #region Recircular
            List<Util.ObjectRecircular> lEc_Exc = Util.ObjectRecircular.List();
            for (int i = 33; i <= lEc_Exc.Count; i++)
            {
                Util.ObjectRecircular.Delete(i);
            }
            #endregion

            #region Recircular

            if (!File.Exists(Util.ObjectRecircularAuto.PathFile))
            {
                try
                {
                    Util.ObjectRecircularAuto.CreateBD();
                }
                catch (Exception e)
                {
                    LogManager.LogError($"Erro no módulo {typeof(Program).Name}: ", e);
                }
            }

			List<Util.ObjectRecircularAuto> lEc_Auto_Exc = Util.ObjectRecircularAuto.List();
            for (int i = 33; i <= lEc_Auto_Exc.Count; i++)
            {
                Util.ObjectRecircularAuto.Delete(i);
            }
            #endregion
            #endregion

            Application.Run(new fPainelControle());
        }

		/// <summary>
        /// Handler para exceções no thread da interface do usuário (UI)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			// Registra a exceção no arquivo de log
			LogManager.LogError("Exceção não tratada capturada no thread de UI.", e.Exception);

			// Opcional: Exibe uma mensagem amigável ao usuário
			MessageBox.Show("Ocorreu um erro inesperado. Por favor, entre em contato com o suporte.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		/// <summary>
        /// Handler para exceções fora do thread de UI (ex.: threads de background)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject is Exception ex)
			{
				// Registra a exceção no arquivo de log
				LogManager.LogError("Exceção fatal não tratada capturada em thread não-UI", ex);

				// Opcional: Exibe uma mensagem amigável ao usuário
				MessageBox.Show("Ocorreu um erro fatal. O aplicativo poderá ser encerrado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}