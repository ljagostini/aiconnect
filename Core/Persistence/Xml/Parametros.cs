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
        public string VersaoIoconnect { get; set; } = "18";

        public bool HabilitarDispensaSequencialP1 { get; set; } = false;
        public bool HabilitarDispensaSequencialP2 { get; set; } = false;

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

        #endregion

        #region Log

        public string PathLogProcessoDispensa { get; set; } = Path.Combine(PathDiretorioSistema, "AD-D8.log");
        public string PathLogControleDispensa { get; set; } = Path.Combine(PathDiretorioSistema, "ctrldsp.log");
        public bool HabilitarLogComunicacao { get; set; } = false;
        public string PathLogComunicacao { get; set; } = Path.Combine(PathDiretorioSistema, "comunicacao.log");

        #endregion

        #region Producao
        public string TipoProducao { get; set; } = "OnLine";
        public string IpProducao { get; set; } = "192.168.0.10";
        public string PortaProducao { get; set; } = "3110";
        public bool DesabilitaMonitProcessoProducao { get; set; } = true;
        #endregion

        #region Sincronismo Formula
        public bool DesabilitaMonitSincFormula { get; set; } = true;
        public string IpSincFormula { get; set; } = "192.168.0.10";
        public string PortaSincFormula { get; set; } = "3111";
        #endregion

        #region Métodos

        public static Parametros Load()
        {
            Parametros p = null;
            XElement xml = null;
            XElement elemento = null;

            p = new Parametros();
            xml = XElement.Load(PathFile);

            try
            {
                #region Geral

                p.ResponseTimeout = XmlConvert.ToInt32(xml.Element("ResponseTimeout").Value);
                p.Velocidade = XmlConvert.ToInt32(xml.Element("Velocidade").Value);
                p.Aceleracao = XmlConvert.ToInt32(xml.Element("Aceleracao").Value);
                p.DelayReverso = XmlConvert.ToInt32(xml.Element("RevDelay").Value);
                p.PulsoReverso = XmlConvert.ToInt32(xml.Element("RevPulsos").Value);
                p.SomarPulsoReverso = XmlConvert.ToBoolean(xml.Element("SomarRevPulsos").Value);
                p.HabilitarTecladoVirtual =
                    XmlConvert.ToBoolean(xml.Element("HabilitarTecladoVirtual").Value);
                p.HabilitarDispensaSequencial =
                    XmlConvert.ToBoolean(xml.Element("HabilitarDispensaSequencial").Value);
                p.HabilitarFormulaPersonalizada =
                    XmlConvert.ToBoolean(xml.Element("HabilitarFormulaPersonalizada").Value);
                p.HabilitarTesteRecipiente =
                    XmlConvert.ToBoolean(xml.Element("HabilitarTesteRecipiente").Value);
                p.IdIdioma =
                    XmlConvert.ToInt32(xml.Element("IdIdioma").Value);
                p.IdDispositivo =
                   XmlConvert.ToInt32(xml.Element("IdDispositivo").Value);

                elemento = xml.Element("HabilitarPurgaIndividual");
                if (elemento != null)
                    p.HabilitarPurgaIndividual = XmlConvert.ToBoolean(xml.Element("HabilitarPurgaIndividual").Value);

                elemento = xml.Element("HabilitarTouchScrenn");
                if (elemento != null)
                    p.HabilitarTouchScrenn = XmlConvert.ToBoolean(xml.Element("HabilitarTouchScrenn").Value);


                elemento = xml.Element("IdDispositivo2");
                if (elemento != null)
                    p.IdDispositivo2 = XmlConvert.ToInt32(xml.Element("IdDispositivo2").Value);

                //
                elemento = xml.Element("NomeDispositivo");
                if (elemento != null)
                    p.NomeDispositivo = xml.Element("NomeDispositivo").Value;

                //
                elemento = xml.Element("NomeDispositivo2");
                if (elemento != null)
                    p.NomeDispositivo2 = xml.Element("NomeDispositivo2").Value;

                //
                elemento = xml.Element("VersaoIoconnect");
                if (elemento != null)
                    p.VersaoIoconnect = xml.Element("VersaoIoconnect").Value;

                //
                elemento = xml.Element("HabilitarDispensaSequencialP1");
                if (elemento != null)
                    p.HabilitarDispensaSequencialP1 = XmlConvert.ToBoolean(xml.Element("HabilitarDispensaSequencialP1").Value);

                elemento = xml.Element("HabilitarDispensaSequencialP2");
                if (elemento != null)
                    p.HabilitarDispensaSequencialP2 = XmlConvert.ToBoolean(xml.Element("HabilitarDispensaSequencialP2").Value);

                #endregion

                #region DAT

                elemento = xml.Element("PathDAT");
                if (elemento != null)
                    p.PathMonitoramentoDAT = xml.Element("PathDAT").Value;

                elemento = xml.Element("PathRepositorioDAT");
                if (elemento != null)
                    p.PathRepositorioDAT = xml.Element("PathRepositorioDAT").Value;

                elemento = xml.Element("PadraoConteudoDAT");
                if (elemento != null)
                {
                    p.PadraoConteudoDAT =
                        XmlConvert.ToInt32(xml.Element("PadraoConteudoDAT").Value);
                }

                elemento = xml.Element("BasePosicaoCircuitoDAT");
                if (elemento != null)
                {
                    p.BasePosicaoCircuitoDAT =
                        XmlConvert.ToInt32(xml.Element("BasePosicaoCircuitoDAT").Value);
                }

                elemento = xml.Element("UtilizarCorrespondenciaDAT");
                if (elemento != null)
                {
                    p.UtilizarCorrespondenciaDAT =
                        XmlConvert.ToBoolean(xml.Element("UtilizarCorrespondenciaDAT").Value);
                }

                elemento = xml.Element("DesabilitarInterfaceDispensaSequencial");
                if (elemento != null)
                {
                    p.DesabilitarInterfaceDispensaSequencial =
                        XmlConvert.ToBoolean(xml.Element("DesabilitarInterfaceDispensaSequencial").Value);
                }

                elemento = xml.Element("DesabilitarInterfaceDispensaSimultanea");
                if (elemento != null)
                {
                    p.DesabilitarInterfaceDispensaSimultanea =
                        XmlConvert.ToBoolean(xml.Element("DesabilitarInterfaceDispensaSimultanea").Value);
                }

                elemento = xml.Element("DesabilitarInterfaceInicializacaoCircuito");
                if (elemento != null)
                {
                    p.DesabilitarInterfaceInicializacaoCircuito =
                        XmlConvert.ToBoolean(xml.Element("DesabilitarInterfaceInicializacaoCircuito").Value);
                }

                elemento = xml.Element("DesabilitarInterfacePurga");
                if (elemento != null)
                {
                    p.DesabilitarInterfacePurga =
                        XmlConvert.ToBoolean(xml.Element("DesabilitarInterfacePurga").Value);
                }

                #endregion

                #region Purga

                elemento = xml.Element("PrazoExecucaoPurga");
                if (elemento != null)
                {
                    p.PrazoExecucaoPurga =
                        XmlConvert.ToTimeSpan(xml.Element("PrazoExecucaoPurga").Value);
                }

                elemento = xml.Element("DataExecucaoPurga");
                if (elemento != null)
                    p.DataExecucaoPurga = DateTime.Parse(xml.Element("DataExecucaoPurga").Value);

                elemento = xml.Element("VolumePurga");
                if (elemento != null)
                    p.VolumePurga = XmlConvert.ToDouble(xml.Element("VolumePurga").Value);

                elemento = xml.Element("VelocidadePurga");
                if (elemento != null)
                    p.VelocidadePurga = XmlConvert.ToInt32((xml.Element("VelocidadePurga").Value));

                elemento = xml.Element("AceleracaoPurga");
                if (elemento != null)
                    p.AceleracaoPurga = XmlConvert.ToInt32(xml.Element("AceleracaoPurga").Value);

                elemento = xml.Element("DelayPurga");
                if (elemento != null)
                    p.DelayPurga = XmlConvert.ToInt32(xml.Element("DelayPurga").Value);

                elemento = xml.Element("ControlarExecucaoPurga");
                if (elemento != null)
                {
                    p.ControlarExecucaoPurga =
                        XmlConvert.ToBoolean(xml.Element("ControlarExecucaoPurga").Value);
                }

                elemento = xml.Element("ExigirExecucaoPurga");
                if (elemento != null)
                {
                    p.ExigirExecucaoPurga =
                        XmlConvert.ToBoolean(xml.Element("ExigirExecucaoPurga").Value);
                }

                elemento = xml.Element("PurgaSequencial");
                if (elemento != null)
                {
                    p.PurgaSequencial =
                        XmlConvert.ToBoolean(xml.Element("PurgaSequencial").Value);
                }


                #endregion

                #region Controle de volume

                elemento = xml.Element("VolumeMinimo");
                if (elemento != null)
                    p.VolumeMinimo = XmlConvert.ToDouble(xml.Element("VolumeMinimo").Value);

                elemento = xml.Element("VolumeMaximo");
                if (elemento != null)
                    p.VolumeMaximo = XmlConvert.ToDouble(xml.Element("VolumeMaximo").Value);

                elemento = xml.Element("ControlarVolume");
                if (elemento != null)
                    p.ControlarNivel = XmlConvert.ToBoolean(xml.Element("ControlarVolume").Value);

                #endregion

                #region Inicialização dos circuitos

                elemento = xml.Element("IniPulsoInicial");
                if (elemento != null)
                    p.IniPulsoInicial = XmlConvert.ToInt32((xml.Element("IniPulsoInicial").Value));

                elemento = xml.Element("IniPulsoLimite");
                if (elemento != null)
                    p.IniPulsoLimite = XmlConvert.ToInt32((xml.Element("IniPulsoLimite").Value));

                elemento = xml.Element("IniVariacaoPulso");
                if (elemento != null)
                    p.IniVariacaoPulso = XmlConvert.ToInt32((xml.Element("IniVariacaoPulso").Value));

                elemento = xml.Element("IniStepVariacao");
                if (elemento != null)
                    p.IniStepVariacao = XmlConvert.ToDouble((xml.Element("IniStepVariacao").Value));

                elemento = xml.Element("IniVelocidade");
                if (elemento != null)
                    p.IniVelocidade = XmlConvert.ToInt32((xml.Element("IniVelocidade").Value));

                elemento = xml.Element("IniAceleracao");
                if (elemento != null)
                    p.IniAceleracao = XmlConvert.ToInt32((xml.Element("IniAceleracao").Value));

                elemento = xml.Element("IniMovimentoReverso");
                if (elemento != null)
                    p.IniMovimentoReverso = XmlConvert.ToBoolean((xml.Element("IniMovimentoReverso").Value));

                elemento = xml.Element("InicializarCircuitosPurga");
                if (elemento != null)
                    p.InicializarCircuitosPurga = XmlConvert.ToBoolean(xml.Element("InicializarCircuitosPurga").Value);

                //
                elemento = xml.Element("InicializarCircuitosPurgaIndividual");
                if (elemento != null)
                    p.InicializarCircuitosPurgaIndividual = XmlConvert.ToBoolean(xml.Element("InicializarCircuitosPurgaIndividual").Value);


                elemento = xml.Element("QtdeCircuitoGrupo");
                if (elemento != null)
                    p.QtdeCircuitoGrupo = XmlConvert.ToInt32(xml.Element("QtdeCircuitoGrupo").Value);

                #endregion

                #region Unidade de medida

                elemento = xml.Element("ValorShot");
                if (elemento != null)
                    p.ValorShot = XmlConvert.ToDouble(xml.Element("ValorShot").Value);

                elemento = xml.Element("HabilitarShot");
                if (elemento != null)
                    p.HabilitarShot = XmlConvert.ToBoolean(xml.Element("HabilitarShot").Value);

                elemento = xml.Element("HabilitarOnca");
                if (elemento != null)
                    p.HabilitarOnca = XmlConvert.ToBoolean(xml.Element("HabilitarOnca").Value);

                elemento = xml.Element("HabilitarMililitro");
                if (elemento != null)
                    p.HabilitarMililitro = XmlConvert.ToBoolean(xml.Element("HabilitarMililitro").Value);

                elemento = xml.Element("HabilitarGrama");
                if (elemento != null)
                    p.HabilitarGrama = XmlConvert.ToBoolean(xml.Element("HabilitarGrama").Value);

                elemento = xml.Element("UnidadeMedidaNivelColorante");
                if (elemento != null)
                    p.UnidadeMedidaNivelColorante = XmlConvert.ToInt32(xml.Element("UnidadeMedidaNivelColorante").Value);

                #endregion

                #region Log

                elemento = xml.Element("PathLogProcessoDispensa");
                if (elemento != null)
                    p.PathLogProcessoDispensa = xml.Element("PathLogProcessoDispensa").Value;

                elemento = xml.Element("PathLogControleDispensa");
                if (elemento != null)
                    p.PathLogControleDispensa = xml.Element("PathLogControleDispensa").Value;

                elemento = xml.Element("HabilitarLogComunicacao");
                if (elemento != null)
                    p.HabilitarLogComunicacao = XmlConvert.ToBoolean(xml.Element("HabilitarLogComunicacao").Value);

                elemento = xml.Element("PathLogComunicacao");
                if (elemento != null)
                    p.PathLogComunicacao = xml.Element("PathLogComunicacao").Value;

                #endregion
                
                #region Monitoramento dos circuitos

                elemento = xml.Element("QtdeMonitCircuitoGrupo");
                if (elemento != null)
                    p.QtdeMonitCircuitoGrupo = XmlConvert.ToInt32((xml.Element("QtdeMonitCircuitoGrupo").Value));

                elemento = xml.Element("MonitVelocidade");
                if (elemento != null)
                    p.MonitVelocidade = XmlConvert.ToInt32((xml.Element("MonitVelocidade").Value));

                elemento = xml.Element("MonitAceleracao");
                if (elemento != null)
                    p.MonitAceleracao = XmlConvert.ToInt32((xml.Element("MonitAceleracao").Value));
               
                elemento = xml.Element("MonitDelay");
                if (elemento != null)
                    p.MonitDelay = XmlConvert.ToInt32((xml.Element("MonitDelay").Value));

                elemento = xml.Element("MonitTimerDelay");
                if (elemento != null)
                    p.MonitTimerDelay = XmlConvert.ToInt32((xml.Element("MonitTimerDelay").Value));

                elemento = xml.Element("MonitTimerDelayIni");
                if (elemento != null)
                    p.MonitTimerDelayIni = XmlConvert.ToInt32((xml.Element("MonitTimerDelayIni").Value));

                elemento = xml.Element("DesabilitarInterfaceMonitCircuito");
                if (elemento != null)
                    p.DesabilitarInterfaceMonitCircuito = XmlConvert.ToBoolean((xml.Element("DesabilitarInterfaceMonitCircuito").Value));

                elemento = xml.Element("DesabilitarProcessoMonitCircuito");
                if (elemento != null)
                    p.DesabilitarProcessoMonitCircuito = XmlConvert.ToBoolean((xml.Element("DesabilitarProcessoMonitCircuito").Value));
                

                elemento = xml.Element("MonitMovimentoReverso");
                if (elemento != null)
                    p.MonitMovimentoReverso = XmlConvert.ToBoolean(xml.Element("MonitMovimentoReverso").Value);

                elemento = xml.Element("MonitPulsos");
                if (elemento != null)
                    p.MonitPulsos = XmlConvert.ToInt32(xml.Element("MonitPulsos").Value);




                #endregion

                #region Producao
            
                elemento = xml.Element("TipoProducao");
                if (elemento != null)
                    p.TipoProducao = xml.Element("TipoProducao").Value;

                elemento = xml.Element("IpProducao");
                if (elemento != null)
                    p.IpProducao = xml.Element("IpProducao").Value;


                elemento = xml.Element("PortaProducao");
                if (elemento != null)
                    p.PortaProducao = xml.Element("PortaProducao").Value;                


                elemento = xml.Element("DesabilitaMonitProcessoProducao");
                if (elemento != null)
                    p.DesabilitaMonitProcessoProducao = XmlConvert.ToBoolean((xml.Element("DesabilitaMonitProcessoProducao").Value));

                #endregion

                #region Sinc Formula
                try
                {
                    elemento = xml.Element("DesabilitaMonitSincFormula");
                    if (elemento != null)
                        p.DesabilitaMonitSincFormula = XmlConvert.ToBoolean((xml.Element("DesabilitaMonitSincFormula").Value));

                    elemento = xml.Element("PortaSincFormula");
                    if (elemento != null)
                        p.PortaSincFormula = xml.Element("PortaSincFormula").Value;

                    elemento = xml.Element("IpSincFormula");
                    if (elemento != null)
                        p.IpSincFormula = xml.Element("IpSincFormula").Value;

                }
                catch
                { }

                #endregion

                return p;
            }
            catch
            {
                return null;
            }
            finally
            {
                xml = null;
            }
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
            try
            {
                XElement xml = XElement.Load(PathFile);

                #region Geral

                xml.Element("ResponseTimeout").SetValue(p.ResponseTimeout);
                xml.Element("Velocidade").SetValue(p.Velocidade);
                xml.Element("Aceleracao").SetValue(p.Aceleracao);
                xml.Element("RevDelay").SetValue(p.DelayReverso);
                xml.Element("RevPulsos").SetValue(p.PulsoReverso);
                xml.Element("SomarRevPulsos").SetValue(p.SomarPulsoReverso);
                xml.Element("HabilitarTecladoVirtual").SetValue(p.HabilitarTecladoVirtual);
                xml.Element("HabilitarDispensaSequencial").SetValue(p.HabilitarDispensaSequencial);
                xml.Element("HabilitarFormulaPersonalizada").SetValue(p.HabilitarFormulaPersonalizada);
                xml.Element("HabilitarTesteRecipiente").SetValue(p.HabilitarTesteRecipiente);
                xml.Element("IdDispositivo").SetValue(p.IdDispositivo);

                if (xml.Element("HabilitarPurgaIndividual") == null)
                    xml.Add(new XElement("HabilitarPurgaIndividual", p.HabilitarPurgaIndividual));
                else
                    xml.Element("HabilitarPurgaIndividual").SetValue(p.HabilitarPurgaIndividual);

                if (xml.Element("HabilitarTouchScrenn") == null)
                    xml.Add(new XElement("HabilitarTouchScrenn", p.HabilitarTouchScrenn));
                else
                    xml.Element("HabilitarTouchScrenn").SetValue(p.HabilitarTouchScrenn);


                //
                if (xml.Element("VersaoIoconnect") == null)
                    xml.Add(new XElement("VersaoIoconnect", p.VersaoIoconnect));
                else
                    xml.Element("VersaoIoconnect").SetValue(p.VersaoIoconnect);

                if (xml.Element("IdDispositivo2") == null)
                    xml.Add(new XElement("IdDispositivo2", p.IdDispositivo2));
                else
                    xml.Element("IdDispositivo2").SetValue(p.IdDispositivo2);

                if (xml.Element("NomeDispositivo") == null)
                    xml.Add(new XElement("NomeDispositivo", p.NomeDispositivo));
                else
                    xml.Element("NomeDispositivo").SetValue(p.NomeDispositivo);


                if (xml.Element("NomeDispositivo2") == null)
                    xml.Add(new XElement("NomeDispositivo2", p.NomeDispositivo2));
                else
                    xml.Element("NomeDispositivo2").SetValue(p.NomeDispositivo2);

                if (xml.Element("HabilitarDispensaSequencialP1") == null)
                    xml.Add(new XElement("HabilitarDispensaSequencialP1", p.HabilitarDispensaSequencialP1));
                else
                    xml.Element("HabilitarDispensaSequencialP1").SetValue(p.HabilitarDispensaSequencialP1);

                if (xml.Element("HabilitarDispensaSequencialP2") == null)
                    xml.Add(new XElement("HabilitarDispensaSequencialP2", p.HabilitarDispensaSequencialP2));
                else
                    xml.Element("HabilitarDispensaSequencialP2").SetValue(p.HabilitarDispensaSequencialP2);


                #endregion

                #region DAT

                if (xml.Element("PathDAT") == null)
                    xml.Add(new XElement("PathDAT", p.PathMonitoramentoDAT));
                else
                    xml.Element("PathDAT").SetValue(p.PathMonitoramentoDAT);

                if (xml.Element("PathRepositorioDAT") == null)
                    xml.Add(new XElement("PathRepositorioDAT", p.PathRepositorioDAT));
                else
                    xml.Element("PathRepositorioDAT").SetValue(p.PathRepositorioDAT);

                if (xml.Element("PadraoConteudoDAT") == null)
                    xml.Add(new XElement("PadraoConteudoDAT", p.PadraoConteudoDAT));
                else
                    xml.Element("PadraoConteudoDAT").SetValue(p.PadraoConteudoDAT);

                if (xml.Element("BasePosicaoCircuitoDAT") == null)
                    xml.Add(new XElement("BasePosicaoCircuitoDAT", p.BasePosicaoCircuitoDAT));
                else
                    xml.Element("BasePosicaoCircuitoDAT").SetValue(p.BasePosicaoCircuitoDAT);

                if (xml.Element("UtilizarCorrespondenciaDAT") == null)
                    xml.Add(new XElement("UtilizarCorrespondenciaDAT", p.UtilizarCorrespondenciaDAT));
                else
                    xml.Element("UtilizarCorrespondenciaDAT").SetValue(p.UtilizarCorrespondenciaDAT);

                if (xml.Element("DesabilitarInterfaceDispensaSequencial") == null)
                    xml.Add(new XElement("DesabilitarInterfaceDispensaSequencial", p.DesabilitarInterfaceDispensaSequencial));
                else
                    xml.Element("DesabilitarInterfaceDispensaSequencial").SetValue(p.DesabilitarInterfaceDispensaSequencial);

                if (xml.Element("DesabilitarInterfaceDispensaSimultanea") == null)
                    xml.Add(new XElement("DesabilitarInterfaceDispensaSimultanea", p.DesabilitarInterfaceDispensaSimultanea));
                else
                    xml.Element("DesabilitarInterfaceDispensaSimultanea").SetValue(p.DesabilitarInterfaceDispensaSimultanea);

                if (xml.Element("DesabilitarInterfaceInicializacaoCircuito") == null)
                    xml.Add(new XElement("DesabilitarInterfaceInicializacaoCircuito", p.DesabilitarInterfaceInicializacaoCircuito));
                else
                    xml.Element("DesabilitarInterfaceInicializacaoCircuito").SetValue(p.DesabilitarInterfaceInicializacaoCircuito);

                if (xml.Element("DesabilitarInterfacePurga") == null)
                    xml.Add(new XElement("DesabilitarInterfacePurga", p.DesabilitarInterfacePurga));
                else
                    xml.Element("DesabilitarInterfacePurga").SetValue(p.DesabilitarInterfacePurga);

                #endregion

                #region Purga

                if (xml.Element("PrazoExecucaoPurga") == null)
                    xml.Add(new XElement("PrazoExecucaoPurga", p.PrazoExecucaoPurga));
                else
                    xml.Element("PrazoExecucaoPurga").SetValue(p.PrazoExecucaoPurga);

                if (xml.Element("VolumePurga") == null)
                    xml.Add(new XElement("VolumePurga", p.VolumePurga));
                else
                    xml.Element("VolumePurga").SetValue(p.VolumePurga);

                if (xml.Element("VelocidadePurga") == null)
                    xml.Add(new XElement("VelocidadePurga", p.VelocidadePurga));
                else
                    xml.Element("VelocidadePurga").SetValue(p.VelocidadePurga);

                if (xml.Element("AceleracaoPurga") == null)
                    xml.Add(new XElement("AceleracaoPurga", p.AceleracaoPurga));
                else
                    xml.Element("AceleracaoPurga").SetValue(p.AceleracaoPurga);

                if (xml.Element("DelayPurga") == null)
                    xml.Add(new XElement("DelayPurga", p.DelayPurga));
                else
                    xml.Element("DelayPurga").SetValue(p.DelayPurga);

                if (xml.Element("ControlarExecucaoPurga") == null)
                    xml.Add(new XElement("ControlarExecucaoPurga", p.ControlarExecucaoPurga));
                else
                    xml.Element("ControlarExecucaoPurga").SetValue(p.ControlarExecucaoPurga);

                if (xml.Element("ExigirExecucaoPurga") == null)
                    xml.Add(new XElement("ExigirExecucaoPurga", p.ExigirExecucaoPurga));
                else
                    xml.Element("ExigirExecucaoPurga").SetValue(p.ExigirExecucaoPurga);

                if (xml.Element("PurgaSequencial") == null)
                    xml.Add(new XElement("PurgaSequencial", p.PurgaSequencial));
                else
                    xml.Element("PurgaSequencial").SetValue(p.PurgaSequencial);


                

                #endregion

                #region Controle de volume

                if (xml.Element("VolumeMinimo") == null)
                    xml.Add(new XElement("VolumeMinimo", p.VolumeMinimo));
                else
                    xml.Element("VolumeMinimo").SetValue(p.VolumeMinimo);

                if (xml.Element("VolumeMaximo") == null)
                    xml.Add(new XElement("VolumeMaximo", p.VolumeMaximo));
                else
                    xml.Element("VolumeMaximo").SetValue(p.VolumeMaximo);

                if (xml.Element("ControlarVolume") == null)
                    xml.Add(new XElement("ControlarVolume", p.ControlarNivel));
                else
                    xml.Element("ControlarVolume").SetValue(p.ControlarNivel);

                #endregion

                #region Inicialização de circuitos

                if (xml.Element("IniPulsoInicial") == null)
                    xml.Add(new XElement("IniPulsoInicial", p.IniPulsoInicial));
                else
                    xml.Element("IniPulsoInicial").SetValue(p.IniPulsoInicial);

                if (xml.Element("IniPulsoLimite") == null)
                    xml.Add(new XElement("IniPulsoLimite", p.IniPulsoLimite));
                else
                    xml.Element("IniPulsoLimite").SetValue(p.IniPulsoLimite);

                if (xml.Element("IniVariacaoPulso") == null)
                    xml.Add(new XElement("IniVariacaoPulso", p.IniVariacaoPulso));
                else
                    xml.Element("IniVariacaoPulso").SetValue(p.IniVariacaoPulso);

                if (xml.Element("IniStepVariacao") == null)
                    xml.Add(new XElement("IniStepVariacao", p.IniStepVariacao));
                else
                    xml.Element("IniStepVariacao").SetValue(p.IniStepVariacao);

                if (xml.Element("IniVelocidade") == null)
                    xml.Add(new XElement("IniVelocidade", p.IniVelocidade));
                else
                    xml.Element("IniVelocidade").SetValue(p.IniVelocidade);

                if (xml.Element("IniAceleracao") == null)
                    xml.Add(new XElement("IniAceleracao", p.IniAceleracao));
                else
                    xml.Element("IniAceleracao").SetValue(p.IniAceleracao);

                if (xml.Element("IniMovimentoReverso") == null)
                    xml.Add(new XElement("IniMovimentoReverso", p.IniMovimentoReverso));
                else
                    xml.Element("IniMovimentoReverso").SetValue(p.IniMovimentoReverso);

                if (xml.Element("InicializarCircuitosPurga") == null)
                    xml.Add(new XElement("InicializarCircuitosPurga", p.InicializarCircuitosPurga));
                else
                    xml.Element("InicializarCircuitosPurga").SetValue(p.InicializarCircuitosPurga);

                //
                if (xml.Element("InicializarCircuitosPurgaIndividual") == null)
                    xml.Add(new XElement("InicializarCircuitosPurgaIndividual", p.InicializarCircuitosPurgaIndividual));
                else
                    xml.Element("InicializarCircuitosPurgaIndividual").SetValue(p.InicializarCircuitosPurgaIndividual);

                if (xml.Element("QtdeCircuitoGrupo") == null)
                    xml.Add(new XElement("QtdeCircuitoGrupo", p.QtdeCircuitoGrupo));
                else
                    xml.Element("QtdeCircuitoGrupo").SetValue(p.QtdeCircuitoGrupo);

                #endregion

                #region Monitoramento dos circuitos

                if (xml.Element("QtdeMonitCircuitoGrupo") == null)
                    xml.Add(new XElement("QtdeMonitCircuitoGrupo", p.QtdeMonitCircuitoGrupo));
                else
                    xml.Element("QtdeMonitCircuitoGrupo").SetValue(p.QtdeMonitCircuitoGrupo);

                if (xml.Element("DesabilitarInterfaceMonitCircuito") == null)
                    xml.Add(new XElement("DesabilitarInterfaceMonitCircuito", p.DesabilitarInterfaceMonitCircuito));
                else
                    xml.Element("DesabilitarInterfaceMonitCircuito").SetValue(p.DesabilitarInterfaceMonitCircuito);

                if (xml.Element("DesabilitarProcessoMonitCircuito") == null)
                    xml.Add(new XElement("DesabilitarProcessoMonitCircuito", p.DesabilitarProcessoMonitCircuito));
                else
                    xml.Element("DesabilitarProcessoMonitCircuito").SetValue(p.DesabilitarProcessoMonitCircuito);
                

                if (xml.Element("MonitMovimentoReverso") == null)
                    xml.Add(new XElement("MonitMovimentoReverso", p.MonitMovimentoReverso));
                else
                    xml.Element("MonitMovimentoReverso").SetValue(p.MonitMovimentoReverso);

                if (xml.Element("MonitVelocidade") == null)
                    xml.Add(new XElement("MonitVelocidade", p.MonitVelocidade));
                else
                    xml.Element("MonitVelocidade").SetValue(p.MonitVelocidade);

                if (xml.Element("MonitAceleracao") == null)
                    xml.Add(new XElement("MonitAceleracao", p.MonitAceleracao));
                else
                    xml.Element("MonitAceleracao").SetValue(p.MonitAceleracao);

                if (xml.Element("MonitDelay") == null)
                    xml.Add(new XElement("MonitDelay", p.MonitDelay));
                else
                    xml.Element("MonitDelay").SetValue(p.MonitDelay);

                if (xml.Element("MonitTimerDelay") == null)
                    xml.Add(new XElement("MonitTimerDelay", p.MonitTimerDelay));
                else
                    xml.Element("MonitTimerDelay").SetValue(p.MonitTimerDelay);

                if (xml.Element("MonitTimerDelayIni") == null)
                    xml.Add(new XElement("MonitTimerDelayIni", p.MonitTimerDelayIni));
                else
                    xml.Element("MonitTimerDelayIni").SetValue(p.MonitTimerDelayIni);

                if (xml.Element("MonitPulsos") == null)
                    xml.Add(new XElement("MonitPulsos", p.MonitPulsos));
                else
                    xml.Element("MonitPulsos").SetValue(p.MonitPulsos);


                

                #endregion

                #region Unidade de medida

                if (xml.Element("ValorShot") == null)
                    xml.Add(new XElement("ValorShot", p.ValorShot));
                else
                    xml.Element("ValorShot").SetValue(p.ValorShot);

                if (xml.Element("HabilitarShot") == null)
                    xml.Add(new XElement("HabilitarShot", p.HabilitarShot));
                else
                    xml.Element("HabilitarShot").SetValue(p.HabilitarShot);

                if (xml.Element("HabilitarOnca") == null)
                    xml.Add(new XElement("HabilitarOnca", p.HabilitarOnca));
                else
                    xml.Element("HabilitarOnca").SetValue(p.HabilitarOnca);

                if (xml.Element("HabilitarMililitro") == null)
                    xml.Add(new XElement("HabilitarMililitro", p.HabilitarMililitro));
                else
                    xml.Element("HabilitarMililitro").SetValue(p.HabilitarMililitro);

                if (xml.Element("HabilitarGrama") == null)
                    xml.Add(new XElement("HabilitarGrama", p.HabilitarGrama));
                else
                    xml.Element("HabilitarGrama").SetValue(p.HabilitarGrama);

                if (xml.Element("UnidadeMedidaNivelColorante") == null)
                    xml.Add(new XElement("UnidadeMedidaNivelColorante", p.UnidadeMedidaNivelColorante));
                else
                    xml.Element("UnidadeMedidaNivelColorante").SetValue(p.UnidadeMedidaNivelColorante);

                #endregion

                #region Log

                if (xml.Element("PathLogProcessoDispensa") == null)
                    xml.Add(new XElement("PathLogProcessoDispensa", p.PathLogProcessoDispensa));
                else
                    xml.Element("PathLogProcessoDispensa").SetValue(p.PathLogProcessoDispensa);

                if (xml.Element("PathLogControleDispensa") == null)
                    xml.Add(new XElement("PathLogControleDispensa", p.PathLogControleDispensa));
                else
                    xml.Element("PathLogControleDispensa").SetValue(p.PathLogControleDispensa);

                if (xml.Element("HabilitarLogComunicacao") == null)
                    xml.Add(new XElement("HabilitarLogComunicacao", p.HabilitarLogComunicacao));
                else
                    xml.Element("HabilitarLogComunicacao").SetValue(p.HabilitarLogComunicacao);

                if (xml.Element("PathLogComunicacao") == null)
                    xml.Add(new XElement("PathLogComunicacao", p.PathLogComunicacao));
                else
                    xml.Element("PathLogComunicacao").SetValue(p.PathLogComunicacao);

                #endregion


                #region Produção

                if (xml.Element("TipoProducao") == null)
                    xml.Add(new XElement("TipoProducao", p.TipoProducao));
                else
                    xml.Element("TipoProducao").SetValue(p.TipoProducao);

                if (xml.Element("IpProducao") == null)
                    xml.Add(new XElement("IpProducao", p.IpProducao));
                else
                    xml.Element("IpProducao").SetValue(p.IpProducao);

                if (xml.Element("PortaProducao") == null)
                    xml.Add(new XElement("PortaProducao", p.PortaProducao));
                else
                    xml.Element("PortaProducao").SetValue(p.PortaProducao);

                if (xml.Element("DesabilitaMonitProcessoProducao") == null)
                    xml.Add(new XElement("DesabilitaMonitProcessoProducao", p.DesabilitaMonitProcessoProducao));
                else
                    xml.Element("DesabilitaMonitProcessoProducao").SetValue(p.DesabilitaMonitProcessoProducao);
                #endregion

                #region Sinc Formula

                if (xml.Element("DesabilitaMonitSincFormula") == null)
                    xml.Add(new XElement("DesabilitaMonitSincFormula", p.DesabilitaMonitSincFormula));
                else
                    xml.Element("DesabilitaMonitSincFormula").SetValue(p.DesabilitaMonitSincFormula);


                if (xml.Element("PortaSincFormula") == null)
                    xml.Add(new XElement("PortaSincFormula", p.PortaSincFormula));
                else
                    xml.Element("PortaSincFormula").SetValue(p.PortaSincFormula);

                if (xml.Element("IpSincFormula") == null)
                    xml.Add(new XElement("IpSincFormula", p.IpSincFormula));
                else
                    xml.Element("IpSincFormula").SetValue(p.IpSincFormula);
                #endregion


                xml.Save(PathFile);
            }
            catch
            {
                throw;
            }
        }

        public static void UpdateStructure()
        {
            Parametros p = new Parametros();

            try
            {
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
            catch
            {
                throw;
            }
        }

        public static void UpdateStructureInstall()
        {
            Parametros p = new Parametros();

            try
            {
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

                //
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
            catch
            {
                throw;
            }
        }

        public static void SetExecucaoPurga(DateTime data_hora)
        {
            try
            {
                XElement xml = XElement.Load(PathFile);

                if (xml.Element("DataExecucaoPurga") == null)
                    xml.Add(new XElement("DataExecucaoPurga", data_hora));
                else
                    xml.Element("DataExecucaoPurga").SetValue(data_hora);

                xml.Save(PathFile);
            }
            catch
            {
                throw;
            }
        }

        public static void SetIdIdioma(int id)
        {
            try
            {
                XElement xml = XElement.Load(PathFile);

                if (xml.Element("IdIdioma") == null)
                    xml.Add(new XElement("IdIdioma", id));
                else
                    xml.Element("IdIdioma").SetValue(id);

                xml.Save(PathFile);
            }
            catch
            {
                throw;
            }
        }

        #endregion
    }
}