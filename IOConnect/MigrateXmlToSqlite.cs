using System;
using Microsoft.Extensions.Configuration;
using Percolore.Core;
using Percolore.Core.Logging;
using Percolore.Core.Persistence.WindowsRegistry;
using Percolore.Core.Persistence.Xml;

namespace Percolore.IOConnect
{
    class MigrateXmlToSqlite
    {
        public static void Main()
        {
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

            // Conversão do arquivo de parâmetros
            if (!File.Exists(Util.ObjectParametros.PathFile))
            {
                /*Atualiza estrutura do arquivo xml*/
                Parametros.UpdateStructure();

                /*Persiste valores padrões*/
                Parametros tmp = Parametros.Load();
                Parametros.Persist(tmp);

                Parametros parametrosXML = Parametros.Load();

                Util.ObjectParametros.CreateBD();
                Util.ObjectParametros parametrosDB = new Util.ObjectParametros();

                #region Geral

                parametrosDB.ResponseTimeout = parametrosXML.ResponseTimeout;
                parametrosDB.Velocidade = parametrosXML.Velocidade;

                parametrosDB.Aceleracao = parametrosXML.Aceleracao;
                parametrosDB.DelayReverso = parametrosXML.DelayReverso;
                parametrosDB.PulsoReverso = parametrosXML.PulsoReverso;
                parametrosDB.SomarPulsoReverso = parametrosXML.SomarPulsoReverso;
                parametrosDB.HabilitarTecladoVirtual = parametrosXML.HabilitarTecladoVirtual;
                parametrosDB.HabilitarDispensaSequencial = parametrosXML.HabilitarDispensaSequencial;
                parametrosDB.HabilitarFormulaPersonalizada = parametrosXML.HabilitarFormulaPersonalizada;
                parametrosDB.HabilitarTesteRecipiente = parametrosXML.HabilitarTesteRecipiente;
                parametrosDB.IdIdioma = parametrosXML.IdIdioma;
                parametrosDB.IdDispositivo = parametrosXML.IdDispositivo;
                parametrosDB.HabilitarPurgaIndividual = parametrosXML.HabilitarPurgaIndividual;
                parametrosDB.HabilitarTouchScrenn = parametrosXML.HabilitarTouchScrenn;
                parametrosDB.IdDispositivo2 = parametrosXML.IdDispositivo2;
                parametrosDB.NomeDispositivo = parametrosXML.NomeDispositivo;
                parametrosDB.NomeDispositivo2 = parametrosXML.NomeDispositivo2;
                parametrosDB.VersaoIoconnect = parametrosXML.VersaoIoconnect;
                parametrosDB.HabilitarDispensaSequencialP1 = parametrosXML.HabilitarDispensaSequencialP1;
                parametrosDB.HabilitarDispensaSequencialP2 = parametrosXML.HabilitarDispensaSequencialP2;

                #endregion

                #region DAT

                parametrosDB.PathMonitoramentoDAT = parametrosXML.PathMonitoramentoDAT;
                parametrosDB.PathRepositorioDAT = parametrosXML.PathRepositorioDAT;
                parametrosDB.PadraoConteudoDAT = parametrosXML.PadraoConteudoDAT;
                parametrosDB.BasePosicaoCircuitoDAT = parametrosXML.BasePosicaoCircuitoDAT;
                parametrosDB.UtilizarCorrespondenciaDAT = parametrosXML.UtilizarCorrespondenciaDAT;
                parametrosDB.DesabilitarInterfaceDispensaSequencial = parametrosXML.DesabilitarInterfaceDispensaSequencial;
                parametrosDB.DesabilitarInterfaceDispensaSimultanea = parametrosXML.DesabilitarInterfaceDispensaSimultanea;
                parametrosDB.DesabilitarInterfaceInicializacaoCircuito = parametrosXML.DesabilitarInterfaceInicializacaoCircuito;
                parametrosDB.DesabilitarInterfacePurga = parametrosXML.DesabilitarInterfacePurga;

                #endregion

                #region Purga

                parametrosDB.PrazoExecucaoPurga = parametrosXML.PrazoExecucaoPurga;
                parametrosDB.DataExecucaoPurga = parametrosXML.DataExecucaoPurga;
                parametrosDB.VolumePurga = parametrosXML.VolumePurga;
                parametrosDB.VelocidadePurga = parametrosXML.VelocidadePurga;
                parametrosDB.AceleracaoPurga = parametrosXML.AceleracaoPurga;
                parametrosDB.DelayPurga = parametrosXML.DelayPurga;
                parametrosDB.ControlarExecucaoPurga = parametrosXML.ControlarExecucaoPurga;
                parametrosDB.ExigirExecucaoPurga = parametrosXML.ExigirExecucaoPurga;
                parametrosDB.PurgaSequencial = parametrosXML.PurgaSequencial;

                #endregion

                #region Controle de volume

                parametrosDB.VolumeMinimo = parametrosXML.VolumeMinimo;
                parametrosDB.VolumeMaximo = parametrosXML.VolumeMaximo;
                parametrosDB.ControlarNivel = parametrosXML.ControlarNivel;

                #endregion

                #region Inicialização dos circuitos

                parametrosDB.IniPulsoInicial = parametrosXML.IniPulsoInicial;
                parametrosDB.IniPulsoLimite = parametrosXML.IniPulsoLimite;
                parametrosDB.IniVariacaoPulso = parametrosXML.IniVariacaoPulso;
                parametrosDB.IniStepVariacao = parametrosXML.IniStepVariacao;
                parametrosDB.IniVelocidade = parametrosXML.IniVelocidade;
                parametrosDB.IniAceleracao = parametrosXML.IniAceleracao;
                parametrosDB.IniMovimentoReverso = parametrosXML.IniMovimentoReverso;
                parametrosDB.InicializarCircuitosPurga = parametrosXML.InicializarCircuitosPurga;
                parametrosDB.InicializarCircuitosPurgaIndividual = parametrosXML.InicializarCircuitosPurgaIndividual;
                parametrosDB.QtdeCircuitoGrupo = parametrosXML.QtdeCircuitoGrupo;

                #endregion

                #region Unidade de medida

                parametrosDB.ValorShot = parametrosXML.ValorShot;
                parametrosDB.HabilitarShot = parametrosXML.HabilitarShot;
                parametrosDB.HabilitarOnca = parametrosXML.HabilitarOnca;
                parametrosDB.HabilitarMililitro = parametrosXML.HabilitarMililitro;
                parametrosDB.HabilitarGrama = parametrosXML.HabilitarGrama;
                parametrosDB.UnidadeMedidaNivelColorante = parametrosXML.UnidadeMedidaNivelColorante;

                #endregion

                #region Log

                parametrosDB.PathLogProcessoDispensa = parametrosXML.PathLogProcessoDispensa;
                parametrosDB.PathLogControleDispensa = parametrosXML.PathLogControleDispensa;
                parametrosDB.HabilitarLogComunicacao = parametrosXML.HabilitarLogComunicacao;
                parametrosDB.PathLogComunicacao = parametrosXML.PathLogComunicacao;

                #endregion

                #region Monitoramento dos circuitos

                parametrosDB.QtdeMonitCircuitoGrupo = parametrosXML.QtdeMonitCircuitoGrupo;
                parametrosDB.MonitVelocidade = parametrosXML.MonitVelocidade;
                parametrosDB.MonitAceleracao = parametrosXML.MonitAceleracao;
                parametrosDB.MonitDelay = parametrosXML.MonitDelay;
                parametrosDB.MonitTimerDelay = parametrosXML.MonitTimerDelay;
                parametrosDB.MonitTimerDelayIni = parametrosXML.MonitTimerDelayIni;
                parametrosDB.DesabilitarInterfaceMonitCircuito = parametrosXML.DesabilitarInterfaceMonitCircuito;
                parametrosDB.DesabilitarProcessoMonitCircuito = parametrosXML.DesabilitarProcessoMonitCircuito;
                parametrosDB.MonitMovimentoReverso = parametrosXML.MonitMovimentoReverso;
                parametrosDB.MonitPulsos = parametrosXML.MonitPulsos;

                #endregion

                #region Producao

                parametrosDB.TipoProducao = parametrosXML.TipoProducao;
                parametrosDB.IpProducao = parametrosXML.IpProducao;
                parametrosDB.PortaProducao = parametrosXML.PortaProducao;
                parametrosDB.DesabilitaMonitProcessoProducao = parametrosXML.DesabilitaMonitProcessoProducao;

                #endregion

                #region Sinc Formula
                try
                {
                    parametrosDB.DesabilitaMonitSincFormula = parametrosXML.DesabilitaMonitSincFormula;
                    parametrosDB.PortaSincFormula = parametrosXML.PortaSincFormula;
                    parametrosDB.IpSincFormula = parametrosXML.IpSincFormula;
                }
                catch (Exception e)
                {
                    LogManager.LogError($"Erro no módulo {typeof(Program).Name}: ", e);
                }

                #endregion

                Util.ObjectParametros.PersistInsert(parametrosDB);

                Util.ObjectParametros.InitLoad();

            }

            // Conversão do arquivo de colorantes
            if (!File.Exists(Util.ObjectColorante.PathFile))
            {
                List<Colorante> colorantesXML = Colorante.List();

                Util.ObjectColorante.CreateBD();
                List<Util.ObjectColorante> colorantesDB = new List<Util.ObjectColorante>();

                foreach (Colorante unitColoranteXML in colorantesXML)
                {
                    Util.ObjectColorante unitColoranteDB = new Util.ObjectColorante();
                    unitColoranteDB.Circuito = unitColoranteXML.Circuito;
                    unitColoranteDB.Correspondencia = unitColoranteXML.Correspondencia;
                    unitColoranteDB.Dispositivo = unitColoranteXML.Dispositivo;
                    unitColoranteDB.Habilitado = unitColoranteXML.Habilitado;
                    unitColoranteDB.MassaEspecifica = unitColoranteXML.MassaEspecifica;
                    unitColoranteDB.NivelMaximo = unitColoranteXML.NivelMaximo;
                    unitColoranteDB.NivelMinimo = unitColoranteXML.NivelMinimo;
                    unitColoranteDB.Nome = unitColoranteXML.Nome;
                    unitColoranteDB.Volume = unitColoranteXML.Volume;
                    unitColoranteDB.IsBase = false;
                    unitColoranteDB.Seguidor = -1;
                    unitColoranteDB.IsBicoIndividual = false;
                    unitColoranteDB.VolumePurga = 5;
                    unitColoranteDB.VolumeBicoIndividual = 0.0;

                    colorantesDB.Add(unitColoranteDB);
                }

                Util.ObjectParametros parametrosDB = new Util.ObjectParametros();
                Util.ObjectParametros.InitLoad();
                parametrosDB = Util.ObjectParametros.Load();

                // completar com dados padrão, caso não informados no XML
                if (parametrosDB != null)
                {
                    for (int i = colorantesXML.Count + 1; i <= 32; i++)
                    {
                        Util.ObjectColorante unitColoranteDB = new Util.ObjectColorante();
                        unitColoranteDB.Circuito = i;
                        unitColoranteDB.Correspondencia = i;
                        unitColoranteDB.Dispositivo = 2;
                        unitColoranteDB.Habilitado = false;
                        unitColoranteDB.MassaEspecifica = 0;
                        unitColoranteDB.NivelMaximo = parametrosDB.VolumeMaximo;
                        unitColoranteDB.NivelMinimo = parametrosDB.VolumeMinimo;
                        unitColoranteDB.Nome = "";
                        unitColoranteDB.Volume = 0;
                        unitColoranteDB.IsBase = false;
                        unitColoranteDB.Seguidor = -1;
                        unitColoranteDB.IsBicoIndividual = false;
                        unitColoranteDB.VolumeBicoIndividual = 0.0;
                        colorantesDB.Add(unitColoranteDB);
                    }

                    Util.ObjectColorante.Persist(colorantesDB);
                }
                else
                    LogManager.LogInformation("Não foi possível carregar os parâmetros do sistema.");
            }

            // Conversão do arquivo de fórmulas
            if (!File.Exists(Util.ObjectFormula.PathFile))
            {                
                List<Formula> formulasXML = Formula.List();

                Util.ObjectFormula.CreateBD();

                foreach (Formula unitFormulaXML in formulasXML)
                {
                    Util.ObjectFormula formulasDB = new Util.ObjectFormula();
                    formulasDB.Itens = new List<Util.ObjectFormulaItem>();
                    formulasDB.Nome = unitFormulaXML.Nome;
                    foreach (FormulaItem formulaItemXML in unitFormulaXML.Itens)
                    {
                        Util.ObjectFormulaItem formulaItemDB = new Util.ObjectFormulaItem();
                        formulaItemDB.IdColorante = formulaItemXML.IdColorante;
                        formulaItemDB.Mililitros = formulaItemXML.Mililitros;
                        formulasDB.Itens.Add(formulaItemDB);
                    }
                    Util.ObjectFormula.Persist(formulasDB);
                }

            }

            // Conversão do arquivo de calibrações
            if (!File.Exists(Util.ObjectCalibragem.PathFile))
            {
                Util.ObjectCalibragem.CreateBD();
                Util.ObjectCalibracaoAutomatica.CreateBD();

                Util.ObjectCalibracaoAutomatica calibragemAutoDB = new Util.ObjectCalibracaoAutomatica();

                calibragemAutoDB.CapacideMaxBalanca = 420;
                calibragemAutoDB.MaxMassaAdmRecipiente = 100;
                calibragemAutoDB.NumeroMaxTentativaRec = 3;
                calibragemAutoDB.VolumeMaxRecipiente = 200;
                calibragemAutoDB._calibragem = new Util.ObjectCalibragem();
                calibragemAutoDB._calibragem.Motor = 1;
                calibragemAutoDB.listOperacaoAutomatica = new List<Negocio.OperacaoAutomatica>();
                Util.ObjectCalibracaoAutomatica.Add(calibragemAutoDB, true);

                // lendo e reparando arquivo XML
                List<Calibragem> calibragemXML = new List<Calibragem>();
                for (int i = 1; i <= 32; i++)
                {
                    try
                    {
                        Calibragem unitCalibragemXML = Calibragem.Load(i);
                        calibragemXML.Add(unitCalibragemXML);
                    }
                    catch
                    {
                        LogManager.LogInformation($"Calibração não encontrada para o motor '{i}'. Carregando última calibração válida.");

                        if (i > 16)
                        {
                            try
                            {
                                Calibragem unitCalibragemXML = Calibragem.Load(i - 16);
                                unitCalibragemXML.Motor = i;
                                calibragemXML.Add(unitCalibragemXML);
                            }
                            catch (Exception ex)
                            {
                                LogManager.LogError($"Não foi possível carregar calibração válida para o motor {i}.", ex);
                            }
                        }
                    }
                }

                // convertendo para o formato do banco de dados
                foreach (Calibragem unitCalibragemXML in calibragemXML)
                {
                    Util.ObjectCalibragem unitCalibragemDB = new Util.ObjectCalibragem();
                    unitCalibragemDB.Motor = unitCalibragemXML.Motor;
                    unitCalibragemDB.UltimoPulsoReverso = unitCalibragemXML.UltimoPulsoReverso;
                    unitCalibragemDB.Valores = new List<ValoresVO>();
                    foreach (ValoresVO calibragemValoresXML in unitCalibragemXML.Valores)
                    {
                        ValoresVO calibragemValoresDB = new ValoresVO();
                        calibragemValoresDB.Aceleracao = calibragemValoresXML.Aceleracao;
                        calibragemValoresDB.Delay = calibragemValoresXML.Delay;
                        calibragemValoresDB.DesvioMedio = calibragemValoresXML.DesvioMedio;
                        calibragemValoresDB.MassaMedia = calibragemValoresXML.MassaMedia;
                        calibragemValoresDB.PulsoHorario = calibragemValoresXML.PulsoHorario;
                        calibragemValoresDB.PulsoReverso = calibragemValoresXML.PulsoReverso;
                        calibragemValoresDB.Velocidade = calibragemValoresXML.Velocidade;
                        calibragemValoresDB.Volume = calibragemValoresXML.Volume;
                        unitCalibragemDB.Valores.Add(calibragemValoresDB);
                    }
                    Util.ObjectCalibragem.Add(unitCalibragemDB);
                }

            }

            // Criação do arquivo de configurações de recirculação
            if (!File.Exists(Util.ObjectRecircular.PathFile))
            {
                Util.ObjectRecircular.CreateBD();
                if (File.Exists(Util.ObjectRecircular.PathFile))
                {
                    List<Util.ObjectColorante> colorantesDB = Util.ObjectColorante.List();
                    List<Util.ObjectRecircular> recircularDB = new List<Util.ObjectRecircular>();
                    DateTime dtAgora = DateTime.Now;
                    foreach (Util.ObjectColorante unitColorantesDB in colorantesDB)
                    {
                        Util.ObjectRecircular unitRecircularDB = new Util.ObjectRecircular();
                        unitRecircularDB.Circuito = unitColorantesDB.Circuito;
                        unitRecircularDB.Dias = 10;
                        unitRecircularDB.Habilitado = false;
                        unitRecircularDB.VolumeDin = 0.03;
                        unitRecircularDB.VolumeRecircular = 0.5;
                        unitRecircularDB.VolumeDosado = 0;
                        unitRecircularDB.DtInicio = dtAgora;
                        recircularDB.Add(unitRecircularDB);
                    }
                    Util.ObjectRecircular.Persist(recircularDB);
                }
            }

            // Criação do arquivo de configurações de usuário
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

            // Criação do arquivo BasDat06.db
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

            // Criação do arquivo PlMov.db
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

            // Criação do arquivo BasDat05.db
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

            // Criação do arquivo Abastecimento.db
            if (!File.Exists(Util.ObjectAbastecimento.PathFile))
            {
                Util.ObjectAbastecimento.CreateBD();
            }

            // Criação do arquivo Eventos.db
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

        }

    }
}