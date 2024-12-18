using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Percolore.Core.Persistence.Xml
{
    public class Parametros
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "Parametros.xml");
        public static readonly string FileName = Path.GetFileName(PathFile);
        public static readonly string PathDiretorioSistema = Path.Combine(Environment.CurrentDirectory, "sistema");
        public byte PortaModbus;

        #region Geral
        public int ResponseTimeout { get; set; } = 1000;
        public byte Slave { get; } = 1;
        public int Velocidade { get; set; } = 0;
        public int Aceleracao { get; set; } = 0;
        public int PulsoReverso { get; set; } = 0;
        public int DelayReverso { get; set; } = 0;
        public bool SomarPulsoReverso { get; set; } = true;
        public bool HabilitarTecladoVirtual { get; set; } = true;
        public bool HabilitarDispensaSequencial { get; set; } = false;
        public bool HabilitarFormulaPersonalizada { get; set; } = true;
        public bool HabilitarPurgaIndividual { get; set; } = true;
        public bool HabilitarTouchScrenn { get; set; } = false;
        public bool HabilitarTesteRecipiente { get; set; } = true;
        public int IdIdioma { get; set; } = (int)Idioma.Português;
        public int IdDispositivo { get; set; } = (int)Dispositivo.Placa_2;
        public int IdDispositivo2 { get; set; } = 0;

        public string NomeDispositivo { get; set; } = "";
        public string NomeDispositivo2 { get; set; } = "";
        public string VersaoIoconnect { get; set; } = "";

        public bool HabilitarDispensaSequencialP1 { get; set; } = false;
        public bool HabilitarDispensaSequencialP2 { get; set; } = false;
        public int QtdTentativasConexao { get; set; } = 1;
        public bool HabilitarIdentificacaoCopo { get; set; } = false;
        public bool TreinamentoCal { get; set; } = false;
        public bool HabLimpBicos { get; set; } = false;
        public int DelayLimpBicos { get; set; } = 6;
        public int TipoLimpBicos { get; set; } = 1;
        public bool ViewMessageProc { get; set; } = false;
        public int TipoDosagemExec { get; set; } = 1;

        #endregion

        #region Nível de colorante

        public double VolumeMaximo { get; set; } = 0;
        public double VolumeMinimo { get; set; } = 0;
        public bool ControlarNivel { get; set; } = true;

        #endregion

        #region DAT       

        public int PadraoConteudoDAT { get; set; } = (int)DatPattern.Padrao01;
        public int BasePosicaoCircuitoDAT { get; set; } = 0;
        public bool UtilizarCorrespondenciaDAT { get; set; } = false;
        public string PathMonitoramentoDAT { get; set; } = Path.Combine(Environment.CurrentDirectory, "formula.dat");
        public string PathRepositorioDAT { get; set; } = Path.Combine(PathDiretorioSistema, "dat_old");
        public bool DesabilitarInterfaceDispensaSequencial { get; set; } = false;
        public bool DesabilitarInterfaceDispensaSimultanea { get; set; } = false;
        public string PathMonitoramentoFilaDAT { get; set; } = Path.Combine(Environment.CurrentDirectory, "formulaFila*.dat");
        public bool DesabilitarMonitoramentoFilaDAT { get; set; } = true;

        public int DelayMonitoramentoFilaDAT { get; set; } = 2;

        public bool DesabilitarVolumeMinimoDat { get; set; } = false;
        public double VolumeMinimoDat { get; set; } = 0.1;

        public string Dat_06_UNT_Pref { get; set; } = "@UNT";
        public int Dat_06_UNT_1_IsPonto { get; set; } = 0;
        public int Dat_06_UNT_2_IsPonto { get; set; } = 0;

        public string Dat_06_CAN_Pref { get; set; } = "@CAN";
        public int Dat_06_CAN_1_IsPonto { get; set; } = 0;

        public string Dat_06_FRM_Pref { get; set; } = "@FRM";
        public string Dat_06_FRM_SEP { get; set; } = ";";
        public int Dat_06_FRM_1_IsPonto { get; set; } = 0;

        public string Dat_06_BAS_Pref { get; set; } = "@BAS";
        public int Dat_06_BAS_1_IsPonto { get; set; } = 0;

        public int Dat_06_BAS_Habilitado { get; set; } = 0;


        public string Dat_05_UNT_Pref { get; set; } = "@UNT";
        public int Dat_05_UNT_1_IsPonto { get; set; } = 0;
        public int Dat_05_UNT_2_IsPonto { get; set; } = 0;

        public string Dat_05_CAN_Pref { get; set; } = "@CAN";
        public int Dat_05_CAN_1_IsPonto { get; set; } = 0;

        public string Dat_05_FRM_Pref { get; set; } = "@CNT";
        public string Dat_05_FRM_SEP { get; set; } = ";";
        public int Dat_05_FRM_1_IsPonto { get; set; } = 0;

        public string Dat_05_BAS_Pref { get; set; } = "@BAS";
        public int Dat_05_BAS_1_IsPonto { get; set; } = 0;

        public int Dat_05_BAS_Habilitado { get; set; } = 0;


        public int DelayUDCP { get; set; } = 0;

        public int CreateFileTmpUDCP { get; set; } = 0;

        public string ExtFileTmpUDCP { get; set; } = "tmp";

        public bool ProcRemoveLataUDCP { get; set; } = false;

        public bool DisablePopUpDispDat { get; set; } = true;

        #endregion

        #region Purga

        public TimeSpan PrazoExecucaoPurga { get; set; } = TimeSpan.Zero;
        public DateTime DataExecucaoPurga { get; set; } = DateTime.MinValue;
        public double VolumePurga { get; set; } = 0;
        public int VelocidadePurga { get; set; } = 0;
        public int AceleracaoPurga { get; set; } = 0;
        public int DelayPurga { get; set; } = 0;
        public bool ControlarExecucaoPurga { get; set; } = true;
        public bool DesabilitarInterfacePurga { get; set; } = false;
        public bool ExigirExecucaoPurga { get; set; } = true;
        public bool PurgaSequencial { get; set; } = true;

        #endregion

        #region Inicialização de circuitos

        public int IniPulsoInicial { get; set; } = 0;
        public int IniPulsoLimite { get; set; } = 0;
        public int IniVariacaoPulso { get; set; } = 0;
        public double IniStepVariacao { get; set; } = 0;
        public int IniVelocidade { get; set; } = 0;
        public int IniAceleracao { get; set; } = 0;
        public bool IniMovimentoReverso { get; set; } = false;
        public bool InicializarCircuitosPurga { get; set; } = false;
        public bool InicializarCircuitosPurgaIndividual { get; set; } = false;
        public int QtdeCircuitoGrupo { get; set; } = 5;
        public bool DesabilitarInterfaceInicializacaoCircuito { get; set; } = false;

        #endregion

        #region Monitoramento de circuitos

        public int MonitVelocidade { get; set; } = 833;
        public int MonitAceleracao { get; set; } = 833;
        public int MonitDelay { get; set; } = 500;
        public int MonitTimerDelay { get; set; } = 10;
        public int MonitTimerDelayIni { get; set; } = 10;
        public bool MonitMovimentoReverso { get; set; } = false;
        public int QtdeMonitCircuitoGrupo { get; set; } = 5;
        public bool DesabilitarInterfaceMonitCircuito { get; set; } = false;
        public bool DesabilitarProcessoMonitCircuito { get; set; } = true;
        public int MonitPulsos { get; set; } = 100;

        #endregion

        #region Unidade de medida

        public double ValorShot { get; set; } = 0;
        public bool HabilitarShot { get; set; } = false;
        public bool HabilitarOnca { get; set; } = false;
        public bool HabilitarMililitro { get; set; } = true;
        public bool HabilitarGrama { get; set; } = false;
        public int UnidadeMedidaNivelColorante { get; set; } = (int)UnidadeMedida.Mililitro;
        public int ValorFraction { get; set; } = 800;

        #endregion

        #region Log

        public string PathLogProcessoDispensa { get; set; } = Path.Combine(PathDiretorioSistema, "AD-D8.log");
        public string PathLogControleDispensa { get; set; } = Path.Combine(PathDiretorioSistema, "ctrldsp.log");
        public bool HabilitarLogComunicacao { get; set; } = false;
        public string PathLogComunicacao { get; set; } = Path.Combine(PathDiretorioSistema, "comunicacao.log");
        public bool HabilitarLogAutomateTesterProt { get; set; } = false;
        public bool LogAutomateBackup { get; set; } = false;

        #endregion

        #region Producao
        public string TipoProducao { get; set; } = "OnLine";
        public string IpProducao { get; set; } = "192.168.0.10";
        public string PortaProducao { get; set; } = "3110";
        public bool DesabilitaMonitProcessoProducao { get; set; } = true;
        public string TipoBaseDados { get; set; } = "0";
        public string PathBasesDados { get; set; } = "http://localhost/WebApiDosadora/api/";
        #endregion

        #region Sincronismo Formula
        public bool DesabilitaMonitSincFormula { get; set; } = true;
        public string IpSincFormula { get; set; } = "192.168.0.10";
        public string PortaSincFormula { get; set; } = "3111";
        #endregion

        #region Sincronismo Token
        public bool DesabilitaMonitSyncToken { get; set; } = true;
        public string IpSincToken { get; set; } = "192.168.0.10";
        public string PortaSincToken { get; set; } = "3112";

        public string TipoEventos { get; set; } = "HD";

        #endregion

        #region Sincronismo bkp Calibragem
        public bool DesabilitaMonitSyncBkpCalibragem { get; set; } = true;
        public string UrlSincBkpCalibragem { get; set; } = "ftp://192.168.125.116/BdCalicracao";

        #endregion

        public int TimeoutPingTcp = 8000;

        #region recirculação
        public bool HabilitarRecirculacao { get; set; } = false;
        public int DelayMonitRecirculacao { get; set; } = 4;
        #endregion

        #region Placa Mov
        public int Address_PlacaMov { get; set; } = 3;
        public string NomeDispositivo_PlacaMov { get; set; } = "";
        public int DelayAlertaPlacaMov { get; set; } = 5;
        #endregion

        #region recirculaçãoAuto
        public bool HabilitarRecirculacaoAuto { get; set; } = false;
        public int DelayMonitRecirculacaoAuto { get; set; } = 4;
        public int DelayNotificacaotRecirculacaoAuto { get; set; } = 5;
        public int QtdNotificacaotRecirculacaoAuto { get; set; } = 3;
        #endregion

        public int DelayEsponja { get; set; } = 5;
        public bool LogBD { get; set; } = false;
        public string NameRemoteAccess { get; set; } = "BASupSrvcCnfg.exe";
        public int TempoReciAuto { get; set; }
        public bool LogStatusMaquina { get; set; } = false;

        #region Métodos

        public static Parametros Load()
        {
            Parametros p = null;
            XElement xml = null;
            XElement elemento = null;

            p = new Parametros();
            xml = XElement.Load(PathFile);

            #region Geral

            elemento = xml.Element("ResponseTimeout");
            if (elemento != null)
                p.ResponseTimeout = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("Velocidade");
            if (elemento != null)
                p.Velocidade = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("Aceleracao");
            if (elemento != null)
                p.Aceleracao = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("RevDelay");
            if (elemento != null)
                p.DelayReverso = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("DelayReverso");
            if (elemento != null)
                p.DelayReverso = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("RevPulsos");
            if (elemento != null)
                p.PulsoReverso = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("PulsoReverso");
            if (elemento != null)
                p.PulsoReverso = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("SomarRevPulsos");
            if (elemento != null)
                p.SomarPulsoReverso = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("SomarPulsoReverso");
            if (elemento != null)
                p.SomarPulsoReverso = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("HabilitarTecladoVirtual");
            if (elemento != null)
                p.HabilitarTecladoVirtual = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("HabilitarDispensaSequencial");
            if (elemento != null)
                p.HabilitarDispensaSequencial = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("HabilitarFormulaPersonalizada");
            if (elemento != null)
                p.HabilitarFormulaPersonalizada = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("HabilitarTesteRecipiente");
            if (elemento != null)
                p.HabilitarTesteRecipiente = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("IdIdioma");
            if (elemento != null)
                p.IdIdioma = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("IdDispositivo");
            if (elemento != null)
                p.IdDispositivo = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("HabilitarPurgaIndividual");
            if (elemento != null)
                p.HabilitarPurgaIndividual = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("HabilitarTouchScrenn");
            if (elemento != null)
                p.HabilitarTouchScrenn = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("IdDispositivo2");
            if (elemento != null)
                p.IdDispositivo2 = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("NomeDispositivo");
            if (elemento != null)
                p.NomeDispositivo = elemento.Value;

            elemento = xml.Element("NomeDispositivo2");
            if (elemento != null)
                p.NomeDispositivo2 = elemento.Value;

            elemento = xml.Element("VersaoIoconnect");
            if (elemento != null)
                p.VersaoIoconnect = elemento.Value;

            elemento = xml.Element("HabilitarDispensaSequencialP1");
            if (elemento != null)
                p.HabilitarDispensaSequencialP1 = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("HabilitarDispensaSequencialP2");
            if (elemento != null)
                p.HabilitarDispensaSequencialP2 = XmlConvert.ToBoolean(elemento.Value);

            #endregion

            #region DAT

            elemento = xml.Element("PathDAT");
            if (elemento != null)
                p.PathMonitoramentoDAT = elemento.Value;

            elemento = xml.Element("PathRepositorioDAT");
            if (elemento != null)
                p.PathRepositorioDAT = elemento.Value;

            elemento = xml.Element("PadraoConteudoDAT");
            if (elemento != null)
            {
                p.PadraoConteudoDAT = XmlConvert.ToInt32(elemento.Value);
            }

            elemento = xml.Element("BasePosicaoCircuitoDAT");
            if (elemento != null)
            {
                p.BasePosicaoCircuitoDAT = XmlConvert.ToInt32(elemento.Value);
            }

            elemento = xml.Element("UtilizarCorrespondenciaDAT");
            if (elemento != null)
            {
                p.UtilizarCorrespondenciaDAT = XmlConvert.ToBoolean(elemento.Value);
            }

            elemento = xml.Element("DesabilitarInterfaceDispensaSequencial");
            if (elemento != null)
            {
                p.DesabilitarInterfaceDispensaSequencial = XmlConvert.ToBoolean(elemento.Value);
            }

            elemento = xml.Element("DesabilitarInterfaceDispensaSimultanea");
            if (elemento != null)
            {
                p.DesabilitarInterfaceDispensaSimultanea = XmlConvert.ToBoolean(elemento.Value);
            }

            elemento = xml.Element("DesabilitarInterfaceInicializacaoCircuito");
            if (elemento != null)
            {
                p.DesabilitarInterfaceInicializacaoCircuito = XmlConvert.ToBoolean(elemento.Value);
            }

            elemento = xml.Element("DesabilitarInterfacePurga");
            if (elemento != null)
            {
                p.DesabilitarInterfacePurga = XmlConvert.ToBoolean(elemento.Value);
            }

            #endregion

            #region Purga

            elemento = xml.Element("PrazoExecucaoPurga");
            if (elemento != null)
            {
                p.PrazoExecucaoPurga = XmlConvert.ToTimeSpan(elemento.Value);
            }

            elemento = xml.Element("DataExecucaoPurga");
            if (elemento != null)
                p.DataExecucaoPurga = DateTime.Parse(elemento.Value);

            elemento = xml.Element("VolumePurga");
            if (elemento != null)
                p.VolumePurga = XmlConvert.ToDouble(elemento.Value);

            elemento = xml.Element("VelocidadePurga");
            if (elemento != null)
                p.VelocidadePurga = XmlConvert.ToInt32((elemento.Value));

            elemento = xml.Element("AceleracaoPurga");
            if (elemento != null)
                p.AceleracaoPurga = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("DelayPurga");
            if (elemento != null)
                p.DelayPurga = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("ControlarExecucaoPurga");
            if (elemento != null)
            {
                p.ControlarExecucaoPurga = XmlConvert.ToBoolean(elemento.Value);
            }

            elemento = xml.Element("ExigirExecucaoPurga");
            if (elemento != null)
            {
                p.ExigirExecucaoPurga = XmlConvert.ToBoolean(elemento.Value);
            }

            elemento = xml.Element("PurgaSequencial");
            if (elemento != null)
            {
                p.PurgaSequencial = XmlConvert.ToBoolean(elemento.Value);
            }


            #endregion

            #region Controle de volume

            elemento = xml.Element("VolumeMinimo");
            if (elemento != null)
                p.VolumeMinimo = XmlConvert.ToDouble(elemento.Value);

            elemento = xml.Element("VolumeMaximo");
            if (elemento != null)
                p.VolumeMaximo = XmlConvert.ToDouble(elemento.Value);

            elemento = xml.Element("ControlarVolume");
            if (elemento != null)
                p.ControlarNivel = XmlConvert.ToBoolean(elemento.Value);

            #endregion

            #region Inicialização dos circuitos

            elemento = xml.Element("IniPulsoInicial");
            if (elemento != null)
                p.IniPulsoInicial = XmlConvert.ToInt32((elemento.Value));

            elemento = xml.Element("IniPulsoLimite");
            if (elemento != null)
                p.IniPulsoLimite = XmlConvert.ToInt32((elemento.Value));

            elemento = xml.Element("IniVariacaoPulso");
            if (elemento != null)
                p.IniVariacaoPulso = XmlConvert.ToInt32((elemento.Value));

            elemento = xml.Element("IniStepVariacao");
            if (elemento != null)
                p.IniStepVariacao = XmlConvert.ToDouble((elemento.Value));

            elemento = xml.Element("IniVelocidade");
            if (elemento != null)
                p.IniVelocidade = XmlConvert.ToInt32((elemento.Value));

            elemento = xml.Element("IniAceleracao");
            if (elemento != null)
                p.IniAceleracao = XmlConvert.ToInt32((elemento.Value));

            elemento = xml.Element("IniMovimentoReverso");
            if (elemento != null)
                p.IniMovimentoReverso = XmlConvert.ToBoolean((elemento.Value));

            elemento = xml.Element("InicializarCircuitosPurga");
            if (elemento != null)
                p.InicializarCircuitosPurga = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("InicializarCircuitosPurgaIndividual");
            if (elemento != null)
                p.InicializarCircuitosPurgaIndividual = XmlConvert.ToBoolean(elemento.Value);


            elemento = xml.Element("QtdeCircuitoGrupo");
            if (elemento != null)
                p.QtdeCircuitoGrupo = XmlConvert.ToInt32(elemento.Value);

            #endregion

            #region Unidade de medida

            elemento = xml.Element("ValorShot");
            if (elemento != null)
                p.ValorShot = XmlConvert.ToDouble(elemento.Value);

            elemento = xml.Element("HabilitarShot");
            if (elemento != null)
                p.HabilitarShot = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("HabilitarOnca");
            if (elemento != null)
                p.HabilitarOnca = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("HabilitarMililitro");
            if (elemento != null)
                p.HabilitarMililitro = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("HabilitarGrama");
            if (elemento != null)
                p.HabilitarGrama = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("UnidadeMedidaNivelColorante");
            if (elemento != null)
                p.UnidadeMedidaNivelColorante = XmlConvert.ToInt32(elemento.Value);

            #endregion

            #region Log

            elemento = xml.Element("PathLogProcessoDispensa");
            if (elemento != null)
                p.PathLogProcessoDispensa = elemento.Value;

            elemento = xml.Element("PathLogControleDispensa");
            if (elemento != null)
                p.PathLogControleDispensa = elemento.Value;

            elemento = xml.Element("HabilitarLogComunicacao");
            if (elemento != null)
                p.HabilitarLogComunicacao = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("PathLogComunicacao");
            if (elemento != null)
                p.PathLogComunicacao = elemento.Value;

            #endregion

            #region Monitoramento dos circuitos

            elemento = xml.Element("QtdeMonitCircuitoGrupo");
            if (elemento != null)
                p.QtdeMonitCircuitoGrupo = XmlConvert.ToInt32((elemento.Value));

            elemento = xml.Element("MonitVelocidade");
            if (elemento != null)
                p.MonitVelocidade = XmlConvert.ToInt32((elemento.Value));

            elemento = xml.Element("MonitAceleracao");
            if (elemento != null)
                p.MonitAceleracao = XmlConvert.ToInt32((elemento.Value));

            elemento = xml.Element("MonitDelay");
            if (elemento != null)
                p.MonitDelay = XmlConvert.ToInt32((elemento.Value));

            elemento = xml.Element("MonitTimerDelay");
            if (elemento != null)
                p.MonitTimerDelay = XmlConvert.ToInt32((elemento.Value));

            elemento = xml.Element("MonitTimerDelayIni");
            if (elemento != null)
                p.MonitTimerDelayIni = XmlConvert.ToInt32((elemento.Value));

            elemento = xml.Element("DesabilitarInterfaceMonitCircuito");
            if (elemento != null)
                p.DesabilitarInterfaceMonitCircuito = XmlConvert.ToBoolean((elemento.Value));

            elemento = xml.Element("DesabilitarProcessoMonitCircuito");
            if (elemento != null)
                p.DesabilitarProcessoMonitCircuito = XmlConvert.ToBoolean((elemento.Value));


            elemento = xml.Element("MonitMovimentoReverso");
            if (elemento != null)
                p.MonitMovimentoReverso = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("MonitPulsos");
            if (elemento != null)
                p.MonitPulsos = XmlConvert.ToInt32(elemento.Value);

            #endregion

            #region Producao

            elemento = xml.Element("TipoProducao");
            if (elemento != null)
                p.TipoProducao = elemento.Value;

            elemento = xml.Element("IpProducao");
            if (elemento != null)
                p.IpProducao = elemento.Value;


            elemento = xml.Element("PortaProducao");
            if (elemento != null)
                p.PortaProducao = elemento.Value;


            elemento = xml.Element("DesabilitaMonitProcessoProducao");
            if (elemento != null)
                p.DesabilitaMonitProcessoProducao = XmlConvert.ToBoolean((elemento.Value));

            #endregion

            #region Sinc Formula

            elemento = xml.Element("DesabilitaMonitSincFormula");
            if (elemento != null)
                p.DesabilitaMonitSincFormula = XmlConvert.ToBoolean((elemento.Value));

            elemento = xml.Element("PortaSincFormula");
            if (elemento != null)
                p.PortaSincFormula = elemento.Value;

            elemento = xml.Element("IpSincFormula");
            if (elemento != null)
                p.IpSincFormula = elemento.Value;

            #endregion

            #region Adição posterior

            elemento = xml.Element("QtdTentativasConexao");
            if (elemento != null)
                p.QtdTentativasConexao = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("HabilitarIdentificacaoCopo");
            if (elemento != null)
                p.HabilitarIdentificacaoCopo = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("TreinamentoCal");
            if (elemento != null)
                p.TreinamentoCal = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("HabLimpBicos");
            if (elemento != null)
                p.HabLimpBicos = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("DelayLimpBicos");
            if (elemento != null)
                p.DelayLimpBicos = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("TipoLimpBicos");
            if (elemento != null)
                p.TipoLimpBicos = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("ViewMessageProc");
            if (elemento != null)
                p.ViewMessageProc = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("TipoDosagemExec");
            if (elemento != null)
                p.TipoDosagemExec = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("PathMonitoramentoFilaDAT");
            if (elemento != null)
                p.PathMonitoramentoFilaDAT = elemento.Value;

            elemento = xml.Element("DesabilitarMonitoramentoFilaDAT");
            if (elemento != null)
                p.DesabilitarMonitoramentoFilaDAT = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("DelayMonitoramentoFilaDAT");
            if (elemento != null)
                p.DelayMonitoramentoFilaDAT = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("DesabilitarVolumeMinimoDat");
            if (elemento != null)
                p.DesabilitarVolumeMinimoDat = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("VolumeMinimoDat");
            if (elemento != null)
                p.VolumeMinimoDat = XmlConvert.ToDouble(elemento.Value);

            elemento = xml.Element("Dat_06_UNT_Pref");
            if (elemento != null)
                p.Dat_06_UNT_Pref = elemento.Value;

            elemento = xml.Element("Dat_06_UNT_1_IsPonto");
            if (elemento != null)
                p.Dat_06_UNT_1_IsPonto = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("Dat_06_UNT_2_IsPonto");
            if (elemento != null)
                p.Dat_06_UNT_2_IsPonto = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("Dat_06_CAN_Pref");
            if (elemento != null)
                p.Dat_06_CAN_Pref = elemento.Value;

            elemento = xml.Element("Dat_06_CAN_1_IsPonto");
            if (elemento != null)
                p.Dat_06_CAN_1_IsPonto = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("Dat_06_FRM_Pref");
            if (elemento != null)
                p.Dat_06_FRM_Pref = elemento.Value;

            elemento = xml.Element("Dat_06_FRM_SEP");
            if (elemento != null)
                p.Dat_06_FRM_SEP = elemento.Value;

            elemento = xml.Element("Dat_06_FRM_1_IsPonto");
            if (elemento != null)
                p.Dat_06_FRM_1_IsPonto = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("Dat_06_BAS_Pref");
            if (elemento != null)
                p.Dat_06_BAS_Pref = elemento.Value;

            elemento = xml.Element("Dat_06_BAS_1_IsPonto");
            if (elemento != null)
                p.Dat_06_BAS_1_IsPonto = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("Dat_06_BAS_Habilitado");
            if (elemento != null)
                p.Dat_06_BAS_Habilitado = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("Dat_05_UNT_Pref");
            if (elemento != null)
                p.Dat_05_UNT_Pref = elemento.Value;

            elemento = xml.Element("Dat_05_UNT_1_IsPonto");
            if (elemento != null)
                p.Dat_05_UNT_1_IsPonto = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("Dat_05_UNT_2_IsPonto");
            if (elemento != null)
                p.Dat_05_UNT_2_IsPonto = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("Dat_05_CAN_Pref");
            if (elemento != null)
                p.Dat_05_CAN_Pref = elemento.Value;

            elemento = xml.Element("Dat_05_CAN_1_IsPonto");
            if (elemento != null)
                p.Dat_05_CAN_1_IsPonto = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("Dat_05_FRM_Pref");
            if (elemento != null)
                p.Dat_05_FRM_Pref = elemento.Value;

            elemento = xml.Element("Dat_05_FRM_SEP");
            if (elemento != null)
                p.Dat_05_FRM_SEP = elemento.Value;

            elemento = xml.Element("Dat_05_FRM_1_IsPonto");
            if (elemento != null)
                p.Dat_05_FRM_1_IsPonto = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("Dat_05_BAS_Pref");
            if (elemento != null)
                p.Dat_05_BAS_Pref = elemento.Value;

            elemento = xml.Element("Dat_05_BAS_1_IsPonto");
            if (elemento != null)
                p.Dat_05_BAS_1_IsPonto = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("Dat_05_BAS_Habilitado");
            if (elemento != null)
                p.Dat_05_BAS_Habilitado = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("DelayUDCP");
            if (elemento != null)
                p.DelayUDCP = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("CreateFileTmpUDCP");
            if (elemento != null)
                p.CreateFileTmpUDCP = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("ExtFileTmpUDCP");
            if (elemento != null)
                p.ExtFileTmpUDCP = elemento.Value;

            elemento = xml.Element("ProcRemoveLataUDCP");
            if (elemento != null)
                p.ProcRemoveLataUDCP = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("DisablePopUpDispDat");
            if (elemento != null)
                p.DisablePopUpDispDat = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("ValorFraction");
            if (elemento != null)
                p.ValorFraction = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("HabilitarLogAutomateTesterProt");
            if (elemento != null)
                p.HabilitarLogAutomateTesterProt = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("LogAutomateBackup");
            if (elemento != null)
                p.LogAutomateBackup = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("TipoBaseDados");
            if (elemento != null)
                p.TipoBaseDados = elemento.Value;

            elemento = xml.Element("PathBasesDados");
            if (elemento != null)
                p.PathBasesDados = elemento.Value;

            elemento = xml.Element("DesabilitaMonitSyncToken");
            if (elemento != null)
                p.DesabilitaMonitSyncToken = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("IpSincToken");
            if (elemento != null)
                p.IpSincToken = elemento.Value;

            elemento = xml.Element("PortaSincToken");
            if (elemento != null)
                p.PortaSincToken = elemento.Value;

            elemento = xml.Element("TipoEventos");
            if (elemento != null)
                p.TipoEventos = elemento.Value;

            elemento = xml.Element("DesabilitaMonitSyncBkpCalibragem");
            if (elemento != null)
                p.DesabilitaMonitSyncBkpCalibragem = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("UrlSincBkpCalibragem");
            if (elemento != null)
                p.UrlSincBkpCalibragem = elemento.Value;

            elemento = xml.Element("TimeoutPingTcp");
            if (elemento != null)
                p.TimeoutPingTcp = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("HabilitarRecirculacao");
            if (elemento != null)
                p.HabilitarRecirculacao = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("DelayMonitRecirculacao");
            if (elemento != null)
                p.DelayMonitRecirculacao = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("Address_PlacaMov");
            if (elemento != null)
                p.Address_PlacaMov = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("NomeDispositivo_PlacaMov");
            if (elemento != null)
                p.NomeDispositivo_PlacaMov = elemento.Value;

            elemento = xml.Element("DelayAlertaPlacaMov");
            if (elemento != null)
                p.DelayAlertaPlacaMov = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("HabilitarRecirculacaoAuto");
            if (elemento != null)
                p.HabilitarRecirculacaoAuto = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("DelayMonitRecirculacaoAuto");
            if (elemento != null)
                p.DelayMonitRecirculacaoAuto = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("DelayNotificacaotRecirculacaoAuto");
            if (elemento != null)
                p.DelayNotificacaotRecirculacaoAuto = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("QtdNotificacaotRecirculacaoAuto");
            if (elemento != null)
                p.QtdNotificacaotRecirculacaoAuto = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("DelayEsponja");
            if (elemento != null)
                p.DelayEsponja = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("LogBD");
            if (elemento != null)
                p.LogBD = XmlConvert.ToBoolean(elemento.Value);

            elemento = xml.Element("NameRemoteAccess");
            if (elemento != null)
                p.NameRemoteAccess = elemento.Value;

            elemento = xml.Element("TempoReciAuto");
            if (elemento != null)
                p.TempoReciAuto = XmlConvert.ToInt32(elemento.Value);

            elemento = xml.Element("LogStatusMaquina");
            if (elemento != null)
                p.LogStatusMaquina = XmlConvert.ToBoolean(elemento.Value);

            #endregion

            return p;
        }

        /// <summary>
        /// Valida propriedades do modelo
        /// </summary>
        public bool Validate(Parametros p, out string outMsg)
        {
            if (p == null)
                throw new ArgumentNullException();

            StringBuilder validacoes = new StringBuilder();

            #region Geral

            if (p.ResponseTimeout > ushort.MaxValue)
            {
                validacoes.AppendLine(Properties.UI.Parametors_Comunicacao_TimeoutFaixa);
            }

            if (p.Velocidade == 0)
            {
                validacoes.AppendLine(Properties.UI.Parametros_Global_VelocidadeMaiorZero);
            }

            if (p.Aceleracao == 0)
            {
                validacoes.AppendLine(Properties.UI.Parametros_Global_AceleracaoMaiorZero);
            }

            #region [Desabilitado em 12/09/2016 por solicitação de Marcelo]

            //if (p.PulsoReverso == 0)
            //    validacoes.AppendLine("O pulso reverso global deve ser maior que zero.");   

            #endregion

            #endregion

            #region Purga

            if (p.PrazoExecucaoPurga == TimeSpan.Zero)
                validacoes.AppendLine(Properties.UI.Parametros_Purga_PrazoExecucaoMaiorZero);

            if (p.VolumePurga == 0)
                validacoes.AppendLine(Properties.UI.Parametros_Purga_VolumeMaiorZero);

            if (p.VelocidadePurga == 0)
                validacoes.AppendLine(Properties.UI.Parametros_Purga_VelocidadeMaiorZero);

            if (p.AceleracaoPurga == 0)
                validacoes.AppendLine(Properties.UI.Parametros_Purga_AceleracaoMaiorZero);

            #endregion

            #region DAT


            string diretorio = Path.GetDirectoryName(p.PathMonitoramentoDAT);
            if (!Directory.Exists(diretorio))
                validacoes.AppendLine(Properties.UI.Parametros_Dat_DiretorioInvalido);

            if (!Directory.Exists(p.PathRepositorioDAT))
                validacoes.AppendLine(Properties.UI.Parametros_Dat_RepositorioInvalido);

            if (p.PadraoConteudoDAT == 0)
                validacoes.AppendLine(Properties.UI.Parametros_Dat_PadraoConteudoObrigatorio);

            #endregion

            #region Nível de colorante opcional

            if (p.ControlarNivel)
            {
                if (p.VolumeMinimo == 0)
                    validacoes.AppendLine(Properties.UI.Parametros_Nivel_MinimoMaiorZero);

                if (p.VolumeMaximo == 0)
                    validacoes.AppendLine(Properties.UI.Parametros_Nivel_MaximoMaiorZero);

                if (p.VolumeMaximo < 0)
                    validacoes.AppendLine(Properties.UI.Parametros_Nivel_MaximoMaiorQueMInimo);
            }

            #endregion

            #region Inicialização de colorantes

            if (p.IniPulsoInicial == 0)
                validacoes.AppendLine(Properties.UI.Parametros_Inicializacao_PulsoInicialMaiorZero);

            if (p.IniPulsoLimite == 0)
                validacoes.AppendLine(Properties.UI.Parametros_Inicializacao_PulsoLimiteMaiorZero);

            if (p.IniVariacaoPulso == 0)
                validacoes.AppendLine(Properties.UI.Parametros_Inicializacao_VariacaoPulsoMaiorZero);

            if (p.IniStepVariacao == 0)
                validacoes.AppendLine(Properties.UI.Parametros_Inicializacao_StepVariacaoMaiorZero);

            if (p.IniAceleracao == 0)
                validacoes.AppendLine(Properties.UI.Parametros_Inicializacao_AceleracaoMaiorZero);

            if (p.IniVelocidade == 0)
                validacoes.AppendLine(Properties.UI.Parametros_Inicializacao_VelocidadeMaiorZero);

            if (p.QtdeCircuitoGrupo < 3 || p.QtdeCircuitoGrupo > 5)
                validacoes.AppendLine(Properties.UI.Parametros_Inicializacao_QtdeCircuitosGrupoFaixa);

            #endregion

            #region Monitoramento dos colorantes

            if (p.MonitVelocidade == 0)
                validacoes.AppendLine(Properties.UI.Parametros_Monit_VelocidadeMaiorZero);

            if (p.MonitAceleracao == 0)
                validacoes.AppendLine(Properties.UI.Parametros_Monit_AceleracaoMaiorZero);

            if (p.MonitDelay == 0)
                validacoes.AppendLine(Properties.UI.Parametros_Monit_DelayMaiorZero);

            if (p.MonitTimerDelay < 10)
                validacoes.AppendLine(Properties.UI.Parametros_Monit_TimerDelayMaiorDez);

            if (p.MonitTimerDelayIni < 10)
                validacoes.AppendLine(Properties.UI.Parametros_Monit_TimerDelayIniMaiorDez);

            if (p.QtdeMonitCircuitoGrupo < 3 || p.QtdeMonitCircuitoGrupo > 5)
                validacoes.AppendLine(Properties.UI.Parametros_Monit_QtdeCircuitosGrupoFaixa);

            if (p.MonitPulsos == 0)
                validacoes.AppendLine(Properties.UI.Parametros_Monit_PulsoMaiorZero);



            #endregion

            #region Unidade de medida

            /* Ao menos uma unidade de medida deve estar habilitada */
            if (!p.HabilitarShot && !p.HabilitarMililitro && !p.HabilitarOnca && !p.HabilitarGrama)
            {
                validacoes.AppendLine(Properties.UI.Parametros_UnidadeMedida_UnidadeEntradaObrigatoria);
            }
            else
            {
                if (p.HabilitarShot && p.ValorShot == 0)
                    validacoes.AppendLine(Properties.UI.Parametros_UnidadeMedida_ShotMaiorZero);
            }

            if (p.UnidadeMedidaNivelColorante == 0)
            {
                validacoes.AppendLine(Properties.UI.Parametros_UnidadeMedida_UnidadeExibicaoObrigatoria);
            }

            #endregion

            #region Log

            if (!Directory.Exists(Path.GetDirectoryName(p.PathLogProcessoDispensa)))
            {
                validacoes.AppendLine(Properties.UI.Parametros_Log_DiretorioLogProcessoInvalido);
            }

            if (!Directory.Exists(Path.GetDirectoryName(p.PathLogControleDispensa)))
            {
                validacoes.AppendLine(Properties.UI.Parametros_Log_DiretorioLogQuantidadeDispensadaInvalido);
            }

            #endregion

            outMsg = validacoes.ToString();
            return
                (validacoes.Length == 0);
        }

        public static void Persist(Parametros p)
        {
            XElement xml = XElement.Load(PathFile);

            // loop por todas as propriedades do objeto para atualizar o xml
            foreach (var prop in p.GetType().GetProperties())
            {
                if (xml.Element(prop.Name) == null)
                    xml.Add(new XElement(prop.Name, prop.GetValue(p)));
                else
                    xml.Element(prop.Name).SetValue(prop.GetValue(p));
            }

            xml.Save(PathFile);
        }

        public static void UpdateStructure()
        {
            Parametros p = new Parametros();

            XElement xml = XElement.Load(PathFile);
            XElement e = null;

            #region Adicionar

            Dictionary<string, object> dicionario = new Dictionary<string, object>();
            dicionario.Add("ResponseTimeout", p.ResponseTimeout);
            dicionario.Add("Velocidade", p.Velocidade);
            dicionario.Add("Aceleracao", p.Aceleracao);
            dicionario.Add("RevDelay", p.DelayReverso);
            dicionario.Add("RevPulsos", p.PulsoReverso);
            dicionario.Add("SomarRevPulsos", p.SomarPulsoReverso);
            dicionario.Add("HabilitarTecladoVirtual", p.HabilitarTecladoVirtual);
            dicionario.Add("HabilitarDispensaSequencial", p.HabilitarDispensaSequencial);
            dicionario.Add("HabilitarFormulaPersonalizada", p.HabilitarFormulaPersonalizada);
            dicionario.Add("HabilitarTesteRecipiente", p.HabilitarTesteRecipiente);
            dicionario.Add("IdIdioma", p.IdIdioma);
            dicionario.Add("IdDispositivo", p.IdDispositivo);
            dicionario.Add("PathLogComunicacao", p.PathLogComunicacao);
            dicionario.Add("HabilitarLogComunicacao", p.HabilitarLogComunicacao);
            dicionario.Add("QtdeCircuitoGrupo", p.QtdeCircuitoGrupo);
            dicionario.Add("DesabilitarInterfaceDispensaSequencial", p.DesabilitarInterfaceDispensaSequencial);
            dicionario.Add("DesabilitarInterfaceDispensaSimultanea", p.DesabilitarInterfaceDispensaSimultanea);
            dicionario.Add("DesabilitarInterfaceInicializacaoCircuito", p.DesabilitarInterfaceInicializacaoCircuito);
            dicionario.Add("DesabilitarInterfacePurga", p.DesabilitarInterfacePurga);

            dicionario.Add("QtdeMonitCircuitoGrupo", p.QtdeMonitCircuitoGrupo);
            dicionario.Add("DesabilitarInterfaceMonitCircuito", p.DesabilitarInterfaceMonitCircuito);
            dicionario.Add("DesabilitarProcessoMonitCircuito", p.DesabilitarProcessoMonitCircuito);
            dicionario.Add("MonitMovimentoReverso", p.MonitMovimentoReverso);
            dicionario.Add("MonitVelocidade", p.MonitVelocidade);
            dicionario.Add("MonitAceleracao", p.MonitAceleracao);
            dicionario.Add("MonitDelay", p.MonitDelay);
            dicionario.Add("MonitTimerDelay", p.MonitTimerDelay);
            dicionario.Add("MonitTimerDelayIni", p.MonitTimerDelayIni);
            dicionario.Add("MonitPulsos", p.MonitPulsos);

            dicionario.Add("TipoProducao", p.TipoProducao);
            dicionario.Add("IpProducao", p.IpProducao);
            dicionario.Add("PortaProducao", p.PortaProducao);
            dicionario.Add("DesabilitaMonitProcessoProducao", p.DesabilitaMonitProcessoProducao);

            #region Compatibilidade

            /* Recupera antigo caminho de repositório de log e utiliza 
            * como diretório dos arquivos de log*/
            e = xml.Element("PathRepositorioLog");
            if (e != null)
            {
                dicionario.Add("PathLogProcessoDispensa",
                    Path.Combine(e.Value, Path.GetFileName(p.PathLogProcessoDispensa)));
                dicionario.Add("PathLogControleDispensa",
                    Path.Combine(e.Value, Path.GetFileName(p.PathLogControleDispensa)));
            }
            else
            {
                dicionario.Add("PathLogProcessoDispensa", p.PathLogProcessoDispensa);

                e = xml.Element("PathLogQuantidadeDispensada");
                if (e != null)
                    dicionario.Add("PathLogControleDispensa", e.Value);
                else
                    dicionario.Add("PathLogControleDispensa", p.PathLogControleDispensa);
            }

            #endregion

            foreach (KeyValuePair<string, object> kv in dicionario)
            {
                e = xml.Element(kv.Key);
                if (e == null)
                    xml.Add(new XElement(kv.Key, kv.Value));
            }

            dicionario = null;

            #endregion

            #region Excluir

            string[] vetor = new string[]{
                "RevAceleracao",
                "RevVelocidade",
                "PortaBalanca",
                "PathRepositorioLog",
                "PathLogQuantidadeDispensada",
                "HabilitarModoSimulacao",
                "PortaModbus" };

            for (int i = 0; i < vetor.Length; i++)
            {
                e = xml.Element(vetor[i]);
                if (e != null)
                    xml.Element(e.Name).Remove();
            }

            #endregion

            xml.Save(PathFile);
        }

        public static void UpdateStructureInstall()
        {
            Parametros p = new Parametros();

            XElement xml = XElement.Load(PathFile);

            #region Adicionar

            #region Monitoramento dos circuitos

            if (xml.Element("QtdeMonitCircuitoGrupo") == null)
                xml.Add(new XElement("QtdeMonitCircuitoGrupo", p.QtdeMonitCircuitoGrupo));

            if (xml.Element("DesabilitarInterfaceMonitCircuito") == null)
                xml.Add(new XElement("DesabilitarInterfaceMonitCircuito", p.DesabilitarInterfaceMonitCircuito));

            if (xml.Element("DesabilitarProcessoMonitCircuito") == null)
                xml.Add(new XElement("DesabilitarProcessoMonitCircuito", p.DesabilitarProcessoMonitCircuito));

            if (xml.Element("MonitMovimentoReverso") == null)
                xml.Add(new XElement("MonitMovimentoReverso", p.MonitMovimentoReverso));

            if (xml.Element("MonitVelocidade") == null)
                xml.Add(new XElement("MonitVelocidade", p.MonitVelocidade));

            if (xml.Element("MonitAceleracao") == null)
                xml.Add(new XElement("MonitAceleracao", p.MonitAceleracao));

            if (xml.Element("MonitDelay") == null)
                xml.Add(new XElement("MonitDelay", p.MonitDelay));

            if (xml.Element("MonitTimerDelay") == null)
                xml.Add(new XElement("MonitTimerDelay", p.MonitTimerDelay));

            if (xml.Element("MonitTimerDelayIni") == null)
                xml.Add(new XElement("MonitTimerDelayIni", p.MonitTimerDelayIni));

            if (xml.Element("MonitPulsos") == null)
                xml.Add(new XElement("MonitPulsos", p.MonitPulsos));

            #endregion

            #region Producao

            if (xml.Element("TipoProducao") == null)
                xml.Add(new XElement("TipoProducao", p.TipoProducao));

            if (xml.Element("IpProducao") == null)
                xml.Add(new XElement("IpProducao", p.IpProducao));

            if (xml.Element("PortaProducao") == null)
                xml.Add(new XElement("PortaProducao", p.PortaProducao));

            if (xml.Element("DesabilitaMonitProcessoProducao") == null)
                xml.Add(new XElement("DesabilitaMonitProcessoProducao", p.DesabilitaMonitProcessoProducao));

            #endregion

            #region Sinc Formula

            if (xml.Element("DesabilitaMonitSincFormula") == null)
                xml.Add(new XElement("DesabilitaMonitSincFormula", p.DesabilitaMonitSincFormula));

            if (xml.Element("PortaSincFormula") == null)
                xml.Add(new XElement("PortaSincFormula", p.PortaSincFormula));

            if (xml.Element("IpSincFormula") == null)
                xml.Add(new XElement("IpSincFormula", p.IpSincFormula));

            #endregion

            if (xml.Element("IdDispositivo2") == null)
                xml.Add(new XElement("IdDispositivo2", p.IdDispositivo2));
            if (xml.Element("NomeDispositivo") == null)
                xml.Add(new XElement("NomeDispositivo", p.NomeDispositivo));
            if (xml.Element("NomeDispositivo2") == null)
                xml.Add(new XElement("NomeDispositivo2", p.NomeDispositivo2));
            /*
            elemento = xml.Element("NomeDispositivo");
            if (elemento != null)
                p.NomeDispositivo = xml.Element("NomeDispositivo").Value;
            */

            #endregion

            xml.Save(PathFile);
        }

        public static void SetExecucaoPurga(DateTime data_hora)
        {
            XElement xml = XElement.Load(PathFile);

            if (xml.Element("DataExecucaoPurga") == null)
                xml.Add(new XElement("DataExecucaoPurga", data_hora));
            else
                xml.Element("DataExecucaoPurga").SetValue(data_hora);

            xml.Save(PathFile);
        }

        public static void SetIdIdioma(int id)
        {
            XElement xml = XElement.Load(PathFile);

            if (xml.Element("IdIdioma") == null)
                xml.Add(new XElement("IdIdioma", id));
            else
                xml.Element("IdIdioma").SetValue(id);

            xml.Save(PathFile);
        }

        #endregion
    }
}