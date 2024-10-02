using Percolore.Core;
using Percolore.Core.Logging;
using Percolore.Core.Persistence.WindowsRegistry;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace Percolore.IOConnect.Util
{
	public class ObjectParametros
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "Parametros.db");
        public static readonly string FileName = Path.GetFileName(PathFile);
        public static readonly string PathDiretorioSistema = Path.Combine(Environment.CurrentDirectory, "sistema");
        public byte PortaModbus;

        public static ObjectParametros objPar = null;

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
        public int UnidadeMedidaNivelColorante { get; set; } = (int)Percolore.IOConnect.Core.UnidadeMedida.Mililitro;

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

        public static void CreateBD()
        {
            try
            {
                if (!File.Exists(PathFile))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("CREATE TABLE IF NOT EXISTS [Parametros](");
                    #region Geral
                    sb.Append(" ResponseTimeout TEXT NULL,");
                    sb.Append(" Velocidade TEXT NULL,");
                    sb.Append(" Aceleracao TEXT NULL,");
                    sb.Append(" RevDelay TEXT NULL,");
                    sb.Append(" RevPulsos TEXT NULL,");
                    sb.Append(" SomarRevPulsos TEXT NULL,");
                    sb.Append(" HabilitarTecladoVirtual TEXT NULL,");
                    sb.Append(" HabilitarDispensaSequencial TEXT NULL,");
                    sb.Append(" HabilitarFormulaPersonalizada TEXT NULL,");
                    sb.Append(" HabilitarTesteRecipiente TEXT NULL,");
					sb.Append(" HabilitarIdentificacaoCopo TEXT NULL,");
					sb.Append(" IdIdioma TEXT NULL,");
                    sb.Append(" IdDispositivo TEXT NULL,");
                    sb.Append(" HabilitarPurgaIndividual TEXT NULL,");
                    sb.Append(" HabilitarTouchScrenn TEXT NULL,");
                    sb.Append(" IdDispositivo2 TEXT NULL,");
                    sb.Append(" NomeDispositivo TEXT NULL,");
                    sb.Append(" NomeDispositivo2 TEXT NULL,");
                    sb.Append(" VersaoIoconnect TEXT NULL,");
                    sb.Append(" HabilitarDispensaSequencialP1 TEXT NULL,");
                    sb.Append(" HabilitarDispensaSequencialP2 TEXT NULL,");
					sb.Append(" TreinamentoCal TEXT NULL,");
					sb.Append(" ViewMessageProc TEXT NULL,");
					sb.Append(" QtdTentativasConexao TEXT NULL,");
					sb.Append(" DelayEsponja TEXT NULL,");
					sb.Append(" HabLimpBicos TEXT NULL,");
					sb.Append(" DelayLimpBicos TEXT NULL,");
					sb.Append(" TipoLimpBicos TEXT NULL,");
					sb.Append(" TipoDosagemExec TEXT NULL,");

					#endregion
					#region DAT
					sb.Append(" PathDAT TEXT NULL,");
                    sb.Append(" PathRepositorioDAT TEXT NULL,");
                    sb.Append(" PadraoConteudoDAT TEXT NULL,");
                    sb.Append(" BasePosicaoCircuitoDAT TEXT NULL,");
                    sb.Append(" UtilizarCorrespondenciaDAT TEXT NULL,");
                    sb.Append(" DesabilitarInterfaceDispensaSequencial TEXT NULL,");
                    sb.Append(" DesabilitarInterfaceDispensaSimultanea TEXT NULL,");
                    sb.Append(" DesabilitarInterfaceInicializacaoCircuito TEXT NULL,");
                    sb.Append(" DesabilitarInterfacePurga TEXT NULL,");

                    sb.Append(" PathFilaDAT TEXT NULL,");
                    sb.Append(" DesabilitarMonitoramentoFilaDAT TEXT NULL,");
                    sb.Append(" DelayMonitoramentoFilaDAT TEXT NULL,");
                    sb.Append(" DesabilitarVolumeMinimoDat TEXT NULL,");
                    sb.Append(" VolumeMinimoDat TEXT NULL,");

					sb.Append(" Dat_06_UNT_Pref TEXT NULL,");
					sb.Append(" Dat_06_UNT_1_IsPonto  TEXT NULL,");
					sb.Append(" Dat_06_UNT_2_IsPonto TEXT NULL,");
					sb.Append(" Dat_06_CAN_Pref TEXT NULL,");
					sb.Append(" Dat_06_CAN_1_IsPonto TEXT NULL,");
					sb.Append(" Dat_06_FRM_Pref TEXT NULL,");
					sb.Append(" Dat_06_FRM_SEP TEXT NULL,");
					sb.Append(" Dat_06_FRM_1_IsPonto TEXT NULL,");
					sb.Append(" Dat_06_BAS_Pref TEXT NULL,");
					sb.Append(" Dat_06_BAS_1_IsPonto TEXT NULL,");
					sb.Append(" Dat_06_BAS_Habilitado TEXT NULL,");
					sb.Append(" Dat_05_UNT_Pref TEXT NULL,");
					sb.Append(" Dat_05_UNT_1_IsPonto TEXT NULL,");
					sb.Append(" Dat_05_UNT_2_IsPonto TEXT NULL,");
					sb.Append(" Dat_05_CAN_Pref TEXT NULL,");
					sb.Append(" Dat_05_CAN_1_IsPonto TEXT NULL,");
					sb.Append(" Dat_05_FRM_Pref TEXT NULL,");
					sb.Append(" Dat_05_FRM_SEP TEXT NULL,");
					sb.Append(" Dat_05_FRM_1_IsPonto TEXT NULL,");
					sb.Append(" Dat_05_BAS_Pref TEXT NULL,");
					sb.Append(" Dat_05_BAS_1_IsPonto TEXT NULL,");
					sb.Append(" Dat_05_BAS_Habilitado TEXT NULL,");
					sb.Append(" ExtFileTmpUDCP TEXT NULL,");
					sb.Append(" CreateFileTmpUDCP TEXT NULL,");
					sb.Append(" DelayUDCP TEXT NULL,");
					sb.Append(" ProcRemoveLataUDCP TEXT NULL,");
					sb.Append(" DisablePopUpDispDat TEXT NULL,");

					#endregion
					#region Purga
					sb.Append(" PrazoExecucaoPurga TEXT NULL,");
                    sb.Append(" DataExecucaoPurga TEXT NULL,");
                    sb.Append(" VolumePurga TEXT NULL,");
                    sb.Append(" VelocidadePurga TEXT NULL,");
                    sb.Append(" AceleracaoPurga TEXT NULL,");
                    sb.Append(" DelayPurga TEXT NULL,");
                    sb.Append(" ControlarExecucaoPurga TEXT NULL,");
                    sb.Append(" ExigirExecucaoPurga TEXT NULL,");
                    sb.Append(" PurgaSequencial TEXT NULL,");

                    #endregion
                    #region Controle de volume

                    sb.Append(" VolumeMinimo TEXT NULL,");
                    sb.Append(" VolumeMaximo TEXT NULL,");
                    sb.Append(" ControlarVolume TEXT NULL,");

                    #endregion
                    #region Inicilização dos circuitos
                    sb.Append(" IniPulsoInicial TEXT NULL,");
                    sb.Append(" IniPulsoLimite TEXT NULL,");
                    sb.Append(" IniVariacaoPulso TEXT NULL,");
                    sb.Append(" IniStepVariacao TEXT NULL,");
                    sb.Append(" IniVelocidade TEXT NULL,");
                    sb.Append(" IniAceleracao TEXT NULL,");
                    sb.Append(" IniMovimentoReverso TEXT NULL,");
                    sb.Append(" InicializarCircuitosPurga TEXT NULL,");
                    sb.Append(" InicializarCircuitosPurgaIndividual TEXT NULL,");
                    sb.Append(" QtdeCircuitoGrupo TEXT NULL,");

					#endregion
					#region Unidade de medida
					sb.Append(" ValorShot TEXT NULL,");
                    sb.Append(" HabilitarShot TEXT NULL,");
                    sb.Append(" HabilitarOnca TEXT NULL,");
                    sb.Append(" HabilitarMililitro TEXT NULL,");
                    sb.Append(" HabilitarGrama TEXT NULL,");
                    sb.Append(" UnidadeMedidaNivelColorante TEXT NULL,");
                    sb.Append(" ValorFraction TEXT NULL,");

					#endregion
					#region Log
					sb.Append(" PathLogProcessoDispensa TEXT NULL,");
                    sb.Append(" PathLogControleDispensa TEXT NULL,");
                    sb.Append(" HabilitarLogComunicacao TEXT NULL,");
                    sb.Append(" PathLogComunicacao TEXT NULL,");
					sb.Append(" HabilitarLogAutomateTesterProt TEXT NULL,");
					sb.Append(" LogAutomateBackup TEXT NULL,");

					#endregion
					#region Monitoramento dos circuitos
					sb.Append(" QtdeMonitCircuitoGrupo TEXT NULL,");
					sb.Append(" MonitVelocidade TEXT NULL,");
                    sb.Append(" MonitAceleracao TEXT NULL,");
                    sb.Append(" MonitDelay TEXT NULL,");
                    sb.Append(" MonitTimerDelay TEXT NULL,");
                    sb.Append(" MonitTimerDelayIni TEXT NULL,");
                    sb.Append(" DesabilitarInterfaceMonitCircuito TEXT NULL,");
                    sb.Append(" DesabilitarProcessoMonitCircuito TEXT NULL,");
                    sb.Append(" MonitMovimentoReverso TEXT NULL,");
                    sb.Append(" MonitPulsos TEXT NULL,");

                    #endregion
                    #region Producao
                    sb.Append(" TipoProducao TEXT NULL,");
                    sb.Append(" IpProducao TEXT NULL,");
                    sb.Append(" PortaProducao TEXT NULL,");
                    sb.Append(" DesabilitaMonitProcessoProducao TEXT NULL,");
                    #endregion
                    
                    #region Sinc Formula
                    sb.Append(" DesabilitaMonitSincFormula TEXT NULL,");
                    sb.Append(" PortaSincFormula TEXT NULL,");
                    sb.Append(" IpSincFormula TEXT NULL,");
					#endregion

					#region Miscelanea
					sb.Append(" TimeoutPingTcp TEXT NULL,");
					sb.Append(" DesabilitaMonitSyncToken TEXT NULL,");
					sb.Append(" IpSincToken TEXT NULL,");
					sb.Append(" PortaSincToken TEXT NULL,");
					sb.Append(" TipoEventos TEXT NULL,");
					sb.Append(" DesabilitaMonitSyncBkpCalibragem TEXT NULL,");
					sb.Append(" UrlSincBkpCalibragem TEXT NULL,");
					sb.Append(" HabilitarRecirculacao TEXT NULL,");
					sb.Append(" DelayMonitRecirculacao TEXT NULL,");
					sb.Append(" Address_PlacaMov TEXT NULL,");
					sb.Append(" NomeDispositivo_PlacaMov TEXT NULL,");
					sb.Append(" DelayAlertaPlacaMov TEXT NULL,");
					sb.Append(" HabilitarRecirculacaoAuto TEXT NULL,");
					sb.Append(" DelayMonitRecirculacaoAuto TEXT NULL,");
					sb.Append(" DelayNotificacaotRecirculacaoAuto TEXT NULL,");
					sb.Append(" QtdNotificacaotRecirculacaoAuto TEXT NULL,");
					sb.Append(" TempoReciAuto TEXT NULL,");
					sb.Append(" LogBD TEXT NULL,");
					sb.Append(" NameRemoteAccess TEXT NULL,");
					sb.Append(" TipoBaseDados TEXT NULL,");
					sb.Append(" PathBasesDados TEXT NULL,");
					sb.Append(" LogStatusMaquina TEXT NULL");

					#endregion

					sb.Append(");");

                    string createQuery = sb.ToString();

                    SQLiteConnection connectCreate = Util.SQLite.CreateSQLiteConnection(PathFile, false);
                    connectCreate.Open();
					// Open connection to create DB if not exists.
					connectCreate.Close();
                    Thread.Sleep(2000);
                    if (File.Exists(PathFile))
                    {
                        using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand(conn))
                            {
                                conn.Open();
                                cmd.CommandText = createQuery;
                                cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                        }
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectParametros).Name}: ", e);
			}
		}

        public static void InitLoad()
        {
            try
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM Parametros;";
                        TimeSpan ts = new TimeSpan();


                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                objPar = new ObjectParametros();

                                #region Geral

                                objPar.ResponseTimeout = Convert.ToInt32(reader["ResponseTimeout"].ToString());
                                objPar.Velocidade = Convert.ToInt32(reader["Velocidade"].ToString());
                                objPar.Aceleracao = Convert.ToInt32(reader["Aceleracao"].ToString());
                                objPar.DelayReverso = Convert.ToInt32(reader["RevDelay"].ToString());
                                objPar.PulsoReverso = Convert.ToInt32(reader["RevPulsos"].ToString());
                                //objPar.SomarPulsoReverso = Convert.ToBoolean(reader["SomarRevPulsos"].ToString());
                                objPar.SomarPulsoReverso = true;
                                objPar.HabilitarTecladoVirtual = Convert.ToBoolean(reader["HabilitarTecladoVirtual"].ToString());
                                objPar.HabilitarDispensaSequencial = Convert.ToBoolean(reader["HabilitarDispensaSequencial"].ToString());
                                objPar.HabilitarFormulaPersonalizada = Convert.ToBoolean(reader["HabilitarFormulaPersonalizada"].ToString());
                                objPar.HabilitarTesteRecipiente = Convert.ToBoolean(reader["HabilitarTesteRecipiente"].ToString());
                                objPar.HabilitarIdentificacaoCopo = Convert.ToBoolean(reader["HabilitarIdentificacaoCopo"].ToString());
                                objPar.TreinamentoCal = Convert.ToBoolean(reader["TreinamentoCal"].ToString());                                    
                                objPar.DelayEsponja = Convert.ToInt32(reader["DelayEsponja"].ToString());
                                
                                try
                                {
                                    objPar.IdIdioma = Convert.ToInt32(reader["IdIdioma"].ToString());
                                }
								catch (Exception e)
								{
									LogManager.LogError($"Erro no módulo {typeof(ObjectParametros).Name}: ", e);
								    objPar.IdIdioma = 1;
                                }

                                objPar.ViewMessageProc = Convert.ToBoolean(reader["ViewMessageProc"].ToString());
                                objPar.IdDispositivo = Convert.ToInt32(reader["IdDispositivo"].ToString());                                
                                objPar.HabilitarPurgaIndividual = Convert.ToBoolean(reader["HabilitarPurgaIndividual"].ToString());
                                objPar.HabilitarTouchScrenn = Convert.ToBoolean(reader["HabilitarTouchScrenn"].ToString());
                                objPar.IdDispositivo2 = Convert.ToInt32(reader["IdDispositivo2"].ToString());
                                objPar.NomeDispositivo = reader["NomeDispositivo"].ToString();
                                objPar.NomeDispositivo2 = reader["NomeDispositivo2"].ToString();
                                objPar.VersaoIoconnect = reader["VersaoIoconnect"].ToString();
                                objPar.HabilitarDispensaSequencialP1 = Convert.ToBoolean(reader["HabilitarDispensaSequencialP1"].ToString());
                                objPar.HabilitarDispensaSequencialP2 = Convert.ToBoolean(reader["HabilitarDispensaSequencialP2"].ToString());
                                
                                try
                                {
                                    objPar.QtdTentativasConexao = Convert.ToInt32(reader["QtdTentativasConexao"].ToString());
                                }
								catch (Exception e)
								{
									LogManager.LogError($"Erro no módulo {typeof(ObjectParametros).Name}: ", e);
								    objPar.QtdTentativasConexao = 1;
                                }

                                try
                                {
                                    objPar.HabLimpBicos = Convert.ToBoolean(reader["HabLimpBicos"].ToString());
                                }
								catch (Exception e)
								{
									LogManager.LogError($"Erro no módulo {typeof(ObjectParametros).Name}: ", e);
									objPar.HabLimpBicos = false;
                                }

                                try
                                {
                                    objPar.DelayLimpBicos = Convert.ToInt32(reader["DelayLimpBicos"].ToString());
                                }
								catch (Exception e)
								{
									LogManager.LogError($"Erro no módulo {typeof(ObjectParametros).Name}: ", e);
									objPar.DelayLimpBicos = 1;
                                }
                                try
                                {
                                    objPar.TipoLimpBicos = Convert.ToInt32(reader["TipoLimpBicos"].ToString());
                                }
								catch (Exception e)
								{
									LogManager.LogError($"Erro no módulo {typeof(ObjectParametros).Name}: ", e);
									objPar.TipoLimpBicos = 1;
                                }

                                objPar.TipoDosagemExec = Convert.ToInt32(reader["TipoDosagemExec"].ToString());

                                #endregion

                                #region DAT

                                objPar.PathMonitoramentoDAT = reader["PathDAT"].ToString();
                                objPar.PathRepositorioDAT = reader["PathRepositorioDAT"].ToString();
                                objPar.PadraoConteudoDAT = Convert.ToInt32(reader["PadraoConteudoDAT"].ToString());
                                objPar.BasePosicaoCircuitoDAT = Convert.ToInt32(reader["BasePosicaoCircuitoDAT"].ToString());
                                objPar.UtilizarCorrespondenciaDAT = Convert.ToBoolean(reader["UtilizarCorrespondenciaDAT"].ToString());
                                objPar.DesabilitarInterfaceDispensaSequencial = Convert.ToBoolean(reader["DesabilitarInterfaceDispensaSequencial"].ToString());
                                objPar.DesabilitarInterfaceDispensaSimultanea = Convert.ToBoolean(reader["DesabilitarInterfaceDispensaSimultanea"].ToString());
                                objPar.DesabilitarInterfaceInicializacaoCircuito = Convert.ToBoolean(reader["DesabilitarInterfaceInicializacaoCircuito"].ToString());
                                objPar.DesabilitarInterfacePurga = Convert.ToBoolean(reader["DesabilitarInterfacePurga"].ToString());

                                //Versão 19
                                objPar.PathMonitoramentoFilaDAT = reader["PathFilaDAT"].ToString();
                                objPar.DesabilitarMonitoramentoFilaDAT = Convert.ToBoolean(reader["DesabilitarMonitoramentoFilaDAT"].ToString());
                                objPar.DelayMonitoramentoFilaDAT = Convert.ToInt32(reader["DelayMonitoramentoFilaDAT"].ToString());

                                //Versão 25
                                objPar.DesabilitarVolumeMinimoDat = Convert.ToBoolean(reader["DesabilitarVolumeMinimoDat"].ToString());
                                objPar.VolumeMinimoDat = double.Parse(reader["VolumeMinimoDat"].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                                //Versão 29
                                objPar.Dat_06_BAS_Pref = reader["Dat_06_BAS_Pref"].ToString();
                                objPar.Dat_06_CAN_Pref = reader["Dat_06_CAN_Pref"].ToString();
                                objPar.Dat_06_FRM_Pref = reader["Dat_06_FRM_Pref"].ToString();
                                objPar.Dat_06_FRM_SEP = reader["Dat_06_FRM_SEP"].ToString();
                                objPar.Dat_06_UNT_Pref = reader["Dat_06_UNT_Pref"].ToString();

                                objPar.Dat_06_CAN_1_IsPonto = reader["Dat_06_CAN_1_IsPonto"].ToString() == "0" ? 0 : 1 ;
                                objPar.Dat_06_FRM_1_IsPonto= reader["Dat_06_FRM_1_IsPonto"].ToString() == "0" ? 0 : 1;
                                objPar.Dat_06_UNT_1_IsPonto = reader["Dat_06_UNT_1_IsPonto"].ToString() == "0" ? 0 : 1;
                                objPar.Dat_06_UNT_2_IsPonto= reader["Dat_06_UNT_2_IsPonto"].ToString() == "0" ? 0 : 1;
                                objPar.Dat_06_BAS_1_IsPonto = reader["Dat_06_BAS_1_IsPonto"].ToString() == "0" ? 0 : 1;

                                objPar.Dat_06_BAS_Habilitado = reader["Dat_06_BAS_Habilitado"].ToString() == "0" ? 0 : 1;

                                //Versão 32
                                objPar.Dat_05_BAS_Pref = reader["Dat_05_BAS_Pref"].ToString();
                                objPar.Dat_05_CAN_Pref = reader["Dat_05_CAN_Pref"].ToString();
                                objPar.Dat_05_FRM_Pref = reader["Dat_05_FRM_Pref"].ToString();
                                objPar.Dat_05_FRM_SEP = reader["Dat_05_FRM_SEP"].ToString();
                                objPar.Dat_05_UNT_Pref = reader["Dat_05_UNT_Pref"].ToString();

                                objPar.Dat_05_CAN_1_IsPonto = reader["Dat_05_CAN_1_IsPonto"].ToString() == "0" ? 0 : 1;
                                objPar.Dat_05_FRM_1_IsPonto = reader["Dat_05_FRM_1_IsPonto"].ToString() == "0" ? 0 : 1;
                                objPar.Dat_05_UNT_1_IsPonto = reader["Dat_05_UNT_1_IsPonto"].ToString() == "0" ? 0 : 1;
                                objPar.Dat_05_UNT_2_IsPonto = reader["Dat_05_UNT_2_IsPonto"].ToString() == "0" ? 0 : 1;
                                objPar.Dat_05_BAS_1_IsPonto = reader["Dat_05_BAS_1_IsPonto"].ToString() == "0" ? 0 : 1;

                                objPar.Dat_05_BAS_Habilitado = reader["Dat_05_BAS_Habilitado"].ToString() == "0" ? 0 : 1;

                                #region Version 56
                                try
                                {
                                    objPar.DisablePopUpDispDat = Convert.ToBoolean(reader["DisablePopUpDispDat"].ToString());
                                }
								catch (Exception e)
								{
									LogManager.LogError($"Erro no módulo {typeof(ObjectParametros).Name}: ", e);
                                    objPar.DisablePopUpDispDat = true;
                                }
                                #endregion
                                #endregion

                                #region Version 46
                                try
                                {
                                    objPar.CreateFileTmpUDCP = reader["CreateFileTmpUDCP"].ToString() == "0" ? 0 : 1;
                                    objPar.ExtFileTmpUDCP = reader["ExtFileTmpUDCP"].ToString();
                                    objPar.DelayUDCP = Convert.ToInt32(reader["DelayUDCP"].ToString());
                                    if(objPar.DelayUDCP < 0)
                                    {
                                        objPar.DelayUDCP = 0;
                                    }
                                    objPar.ProcRemoveLataUDCP = Convert.ToBoolean(reader["ProcRemoveLataUDCP"].ToString());
                                }
								catch (Exception e)
								{
									LogManager.LogError($"Erro no módulo {typeof(ObjectParametros).Name}: ", e);
									objPar.DelayUDCP = 0;
                                }
                                #endregion

                                #region Purga

                                if (TimeSpan.TryParse(reader["PrazoExecucaoPurga"].ToString(), out ts))
                                {
                                    objPar.PrazoExecucaoPurga = ts;
                                }
                                try
                                {
                                    objPar.DataExecucaoPurga = DateTime.Parse(reader["DataExecucaoPurga"].ToString());
                                }
								catch (Exception e)
								{
									LogManager.LogError($"Erro no módulo {typeof(ObjectParametros).Name}: ", e);
									objPar.DataExecucaoPurga = DateTime.MinValue;
                                }

                                objPar.VolumePurga = double.Parse(reader["VolumePurga"].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                                objPar.VelocidadePurga = Convert.ToInt32((reader["VelocidadePurga"].ToString()));

                                objPar.AceleracaoPurga = Convert.ToInt32(reader["AceleracaoPurga"].ToString());

                                objPar.DelayPurga = Convert.ToInt32(reader["DelayPurga"].ToString());

                                objPar.ControlarExecucaoPurga = Convert.ToBoolean(reader["ControlarExecucaoPurga"].ToString());
                                objPar.ExigirExecucaoPurga = Convert.ToBoolean(reader["ExigirExecucaoPurga"].ToString());
                                objPar.PurgaSequencial = Convert.ToBoolean(reader["PurgaSequencial"].ToString());

                                #endregion

                                #region Controle de volume

                                objPar.VolumeMinimo = double.Parse(reader["VolumeMinimo"].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                                objPar.VolumeMaximo = double.Parse(reader["VolumeMaximo"].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                                objPar.ControlarNivel = Convert.ToBoolean(reader["ControlarVolume"].ToString());

                                #endregion

                                #region Inicialização dos circuitos

                                objPar.IniPulsoInicial = Convert.ToInt32(reader["IniPulsoInicial"].ToString());

                                objPar.IniPulsoLimite = Convert.ToInt32(reader["IniPulsoLimite"].ToString());

                                objPar.IniVariacaoPulso = Convert.ToInt32(reader["IniVariacaoPulso"].ToString());

                                objPar.IniStepVariacao = double.Parse(reader["IniStepVariacao"].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                                objPar.IniVelocidade = Convert.ToInt32(reader["IniVelocidade"].ToString());

                                objPar.IniAceleracao = Convert.ToInt32(reader["IniAceleracao"].ToString());

                                objPar.IniMovimentoReverso = Convert.ToBoolean(reader["IniMovimentoReverso"].ToString());

                                objPar.InicializarCircuitosPurga = Convert.ToBoolean(reader["InicializarCircuitosPurga"].ToString());

                                objPar.InicializarCircuitosPurgaIndividual = Convert.ToBoolean(reader["InicializarCircuitosPurgaIndividual"].ToString());

                                objPar.QtdeCircuitoGrupo = Convert.ToInt32(reader["QtdeCircuitoGrupo"].ToString());

                                #endregion

                                #region Unidade de medida

                                objPar.ValorShot = double.Parse(reader["ValorShot"].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                                objPar.HabilitarShot = Convert.ToBoolean(reader["HabilitarShot"].ToString());

                                objPar.HabilitarOnca = Convert.ToBoolean(reader["HabilitarOnca"].ToString());

                                objPar.HabilitarMililitro = Convert.ToBoolean(reader["HabilitarMililitro"].ToString());

                                objPar.HabilitarGrama = Convert.ToBoolean(reader["HabilitarGrama"].ToString());

                                objPar.UnidadeMedidaNivelColorante = Convert.ToInt32(reader["UnidadeMedidaNivelColorante"].ToString());

                                try
                                {
                                    objPar.ValorFraction = Convert.ToInt32(reader["ValorFraction"].ToString());
                                }
								catch (Exception e)
								{
									LogManager.LogError($"Erro no módulo {typeof(ObjectParametros).Name}: ", e);
									objPar.ValorFraction = 800;
                                }

                                #endregion

                                #region Log

                                objPar.PathLogProcessoDispensa = reader["PathLogProcessoDispensa"].ToString();

                                objPar.PathLogControleDispensa = reader["PathLogControleDispensa"].ToString();

                                objPar.HabilitarLogComunicacao = Convert.ToBoolean(reader["HabilitarLogComunicacao"].ToString());

                                objPar.PathLogComunicacao = reader["PathLogComunicacao"].ToString();

                                objPar.HabilitarLogAutomateTesterProt = Convert.ToBoolean(reader["HabilitarLogAutomateTesterProt"].ToString());
                                
                                objPar.LogAutomateBackup = Convert.ToBoolean(reader["LogAutomateBackup"].ToString());
                                
                                #endregion

                                #region Monitoramento dos circuitos

                                objPar.QtdeMonitCircuitoGrupo = Convert.ToInt32(reader["QtdeMonitCircuitoGrupo"].ToString());

                                objPar.MonitVelocidade = Convert.ToInt32(reader["MonitVelocidade"].ToString());

                                objPar.MonitAceleracao = Convert.ToInt32(reader["MonitAceleracao"].ToString());

                                objPar.MonitDelay = Convert.ToInt32(reader["MonitDelay"].ToString());

                                objPar.MonitTimerDelay = Convert.ToInt32(reader["MonitTimerDelay"].ToString());

                                objPar.MonitTimerDelayIni = Convert.ToInt32(reader["MonitTimerDelayIni"].ToString());

                                objPar.DesabilitarInterfaceMonitCircuito = Convert.ToBoolean(reader["DesabilitarInterfaceMonitCircuito"].ToString());

                                objPar.DesabilitarProcessoMonitCircuito = Convert.ToBoolean(reader["DesabilitarProcessoMonitCircuito"].ToString());

                                objPar.MonitMovimentoReverso = Convert.ToBoolean(reader["MonitMovimentoReverso"].ToString());

                                objPar.MonitPulsos = Convert.ToInt32(reader["MonitPulsos"].ToString());

                                #endregion

                                #region Producao

                                objPar.TipoProducao = reader["TipoProducao"].ToString();

                                objPar.IpProducao = reader["IpProducao"].ToString();

                                objPar.PortaProducao = reader["PortaProducao"].ToString();

                                objPar.DesabilitaMonitProcessoProducao = Convert.ToBoolean(reader["DesabilitaMonitProcessoProducao"].ToString());

                                #endregion

                                #region Sinc Formula
                                
                                objPar.DesabilitaMonitSincFormula = Convert.ToBoolean(reader["DesabilitaMonitSincFormula"].ToString());

                                objPar.PortaSincFormula = reader["PortaSincFormula"].ToString();

                                objPar.IpSincFormula = reader["IpSincFormula"].ToString();

                                objPar.TipoBaseDados = reader["TipoBaseDados"].ToString();

                                objPar.PathBasesDados = reader["PathBasesDados"].ToString();

                                #endregion

                                #region sync Token
                                
                                objPar.DesabilitaMonitSyncToken = Convert.ToBoolean(reader["DesabilitaMonitSyncToken"].ToString());
                                objPar.IpSincToken = reader["IpSincToken"].ToString();
                                objPar.PortaSincToken = reader["PortaSincToken"].ToString();
                                objPar.TipoEventos = reader["TipoEventos"].ToString();
                                
                                #endregion

                                objPar.TimeoutPingTcp = Convert.ToInt32(reader["TimeoutPingTcp"].ToString());

                                #region sync BkpCalibragem
                                
                                objPar.DesabilitaMonitSyncBkpCalibragem = Convert.ToBoolean(reader["DesabilitaMonitSyncBkpCalibragem"].ToString());
                                objPar.UrlSincBkpCalibragem = reader["UrlSincBkpCalibragem"].ToString();
                                
                                #endregion

                                #region Recirculacao
                                
                                objPar.HabilitarRecirculacao = Convert.ToBoolean(reader["HabilitarRecirculacao"].ToString());
                                objPar.DelayMonitRecirculacao = Convert.ToInt32(reader["DelayMonitRecirculacao"].ToString());
                                
                                #endregion

                                #region Placa Movimentacao
                                
                                objPar.Address_PlacaMov = Convert.ToInt32(reader["Address_PlacaMov"].ToString());
                                objPar.NomeDispositivo_PlacaMov = reader["NomeDispositivo_PlacaMov"].ToString();
                                objPar.DelayAlertaPlacaMov = Convert.ToInt32(reader["DelayAlertaPlacaMov"].ToString());
                                
                                #endregion

                                #region RecirculacaoAuto
                                
                                objPar.HabilitarRecirculacaoAuto = Convert.ToBoolean(reader["HabilitarRecirculacaoAuto"].ToString());
                                objPar.DelayMonitRecirculacaoAuto = Convert.ToInt32(reader["DelayMonitRecirculacaoAuto"].ToString());
                                objPar.DelayNotificacaotRecirculacaoAuto = Convert.ToInt32(reader["DelayNotificacaotRecirculacaoAuto"].ToString());
                                objPar.QtdNotificacaotRecirculacaoAuto = Convert.ToInt32(reader["QtdNotificacaotRecirculacaoAuto"].ToString());

                                #endregion

                                #region LogBD
                                
                                objPar.LogBD = Convert.ToBoolean(reader["LogBD"].ToString());
                                
                                #endregion
                                
                                objPar.NameRemoteAccess = reader["NameRemoteAccess"].ToString();

                                try
                                {
                                    objPar.TempoReciAuto = Convert.ToInt32(reader["TempoReciAuto"].ToString());
                                }
								catch (Exception e)
								{
									LogManager.LogError($"Erro no módulo {typeof(ObjectParametros).Name}: ", e);
                                    objPar.TempoReciAuto = 1;
                                }

                                try
                                {
                                    objPar.LogStatusMaquina = Convert.ToBoolean(reader["LogStatusMaquina"].ToString());
                                }
								catch (Exception e)
								{
									LogManager.LogError($"Erro no módulo {typeof(ObjectParametros).Name}: ", e);
                                    objPar.LogStatusMaquina = false;
                                }
                                
                                break;
                            }
                            reader.Close();
                        }

                        conn.Close();
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectParametros).Name}: ", e);
			    objPar = null;
            }
        }

        public static ObjectParametros Load()
        {
            ObjectParametros retorno = new ObjectParametros();
            try
            {
                if(objPar == null)
                {
                    InitLoad();
                }
                if(objPar != null)
                {
                    #region Geral

                    retorno.ResponseTimeout = objPar.ResponseTimeout;
                    retorno.Velocidade  = objPar.Velocidade;

                    retorno.Aceleracao = objPar.Aceleracao;
                    retorno.DelayReverso = objPar.DelayReverso;
                    retorno.PulsoReverso = objPar.PulsoReverso;
                    retorno.SomarPulsoReverso = objPar.SomarPulsoReverso;
                    retorno.HabilitarTecladoVirtual = objPar.HabilitarTecladoVirtual;
                    retorno.HabilitarDispensaSequencial = objPar.HabilitarDispensaSequencial;
                    retorno.HabilitarFormulaPersonalizada = objPar.HabilitarFormulaPersonalizada;
                    retorno.HabilitarTesteRecipiente = objPar.HabilitarTesteRecipiente;
                    retorno.HabilitarIdentificacaoCopo = objPar.HabilitarIdentificacaoCopo;
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

                    retorno.QtdTentativasConexao = objPar.QtdTentativasConexao;

                    retorno.DelayEsponja = objPar.DelayEsponja;

                    retorno.TreinamentoCal = objPar.TreinamentoCal;
                    retorno.ViewMessageProc = objPar.ViewMessageProc;

                    retorno.DelayLimpBicos = objPar.DelayLimpBicos;
                    retorno.HabLimpBicos = objPar.HabLimpBicos;
                    retorno.TipoLimpBicos = objPar.TipoLimpBicos;

                    retorno.TipoDosagemExec = objPar.TipoDosagemExec;
                    
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

                    retorno.PathMonitoramentoFilaDAT = objPar.PathMonitoramentoFilaDAT;
                    retorno.DesabilitarMonitoramentoFilaDAT = objPar.DesabilitarMonitoramentoFilaDAT;
                    retorno.DelayMonitoramentoFilaDAT = objPar.DelayMonitoramentoFilaDAT;

                    retorno.DesabilitarVolumeMinimoDat = objPar.DesabilitarVolumeMinimoDat;
                    retorno.VolumeMinimoDat = objPar.VolumeMinimoDat;

                    retorno.Dat_06_BAS_Pref = objPar.Dat_06_BAS_Pref;
                    retorno.Dat_06_CAN_Pref = objPar.Dat_06_CAN_Pref;
                    retorno.Dat_06_FRM_Pref = objPar.Dat_06_FRM_Pref;
                    retorno.Dat_06_FRM_SEP = objPar.Dat_06_FRM_SEP;
                    retorno.Dat_06_UNT_Pref = objPar.Dat_06_UNT_Pref;

                    retorno.Dat_06_CAN_1_IsPonto = objPar.Dat_06_CAN_1_IsPonto;
                    retorno.Dat_06_FRM_1_IsPonto = objPar.Dat_06_FRM_1_IsPonto;
                    retorno.Dat_06_UNT_1_IsPonto = objPar.Dat_06_UNT_1_IsPonto;
                    retorno.Dat_06_UNT_2_IsPonto = objPar.Dat_06_UNT_2_IsPonto;
                    retorno.Dat_06_BAS_1_IsPonto = objPar.Dat_06_BAS_1_IsPonto;

                    retorno.Dat_06_BAS_Habilitado = objPar.Dat_06_BAS_Habilitado;

                    retorno.Dat_05_BAS_Pref = objPar.Dat_05_BAS_Pref;
                    retorno.Dat_05_CAN_Pref = objPar.Dat_05_CAN_Pref;
                    retorno.Dat_05_FRM_Pref = objPar.Dat_05_FRM_Pref;
                    retorno.Dat_05_FRM_SEP = objPar.Dat_05_FRM_SEP;
                    retorno.Dat_05_UNT_Pref = objPar.Dat_05_UNT_Pref;

                    retorno.Dat_05_CAN_1_IsPonto = objPar.Dat_05_CAN_1_IsPonto;
                    retorno.Dat_05_FRM_1_IsPonto = objPar.Dat_05_FRM_1_IsPonto;
                    retorno.Dat_05_UNT_1_IsPonto = objPar.Dat_05_UNT_1_IsPonto;
                    retorno.Dat_05_UNT_2_IsPonto = objPar.Dat_05_UNT_2_IsPonto;
                    retorno.Dat_05_BAS_1_IsPonto = objPar.Dat_05_BAS_1_IsPonto;

                    retorno.Dat_05_BAS_Habilitado = objPar.Dat_05_BAS_Habilitado;

                    retorno.ExtFileTmpUDCP = objPar.ExtFileTmpUDCP;
                    retorno.CreateFileTmpUDCP = objPar.CreateFileTmpUDCP;
                    retorno.DelayUDCP = objPar.DelayUDCP;
                    retorno.ProcRemoveLataUDCP = objPar.ProcRemoveLataUDCP;

                    retorno.DisablePopUpDispDat = objPar.DisablePopUpDispDat;

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

                    retorno.ValorFraction = objPar.ValorFraction;

                    #endregion

                    #region Log

                    retorno.PathLogProcessoDispensa = objPar.PathLogProcessoDispensa;
                    retorno.PathLogControleDispensa = objPar.PathLogControleDispensa;
                    retorno.HabilitarLogComunicacao = objPar.HabilitarLogComunicacao;
                    retorno.PathLogComunicacao = objPar.PathLogComunicacao;

                    retorno.HabilitarLogAutomateTesterProt = objPar.HabilitarLogAutomateTesterProt;
                    retorno.LogAutomateBackup = objPar.LogAutomateBackup;

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
                    
                    retorno.DesabilitaMonitSincFormula = objPar.DesabilitaMonitSincFormula;

                    retorno.PortaSincFormula = objPar.PortaSincFormula;

                    retorno.IpSincFormula = objPar.IpSincFormula;

                    #endregion

                    retorno.TimeoutPingTcp = objPar.TimeoutPingTcp;                    

                    #region Sync Token
                    retorno.DesabilitaMonitSyncToken = objPar.DesabilitaMonitSyncToken;

                    retorno.IpSincToken = objPar.IpSincToken;

                    retorno.PortaSincToken = objPar.PortaSincToken;
                    retorno.TipoEventos = objPar.TipoEventos;
                    #endregion

                    #region Sync BkpCalibragem
                    retorno.DesabilitaMonitSyncBkpCalibragem = objPar.DesabilitaMonitSyncBkpCalibragem;
                    retorno.UrlSincBkpCalibragem = objPar.UrlSincBkpCalibragem;
                    #endregion

                    #region Base de Dados
                    
                    retorno.TipoBaseDados = objPar.TipoBaseDados;

                    retorno.PathBasesDados = objPar.PathBasesDados;
                    
                    #endregion

                    #region Recirculacao
                    
                    retorno.HabilitarRecirculacao = objPar.HabilitarRecirculacao;
                    retorno.DelayMonitRecirculacao = objPar.DelayMonitRecirculacao;
                    
                    #endregion

                    #region Placa Movimentacao
                    
                    retorno.Address_PlacaMov = objPar.Address_PlacaMov;
                    retorno.NomeDispositivo_PlacaMov = objPar.NomeDispositivo_PlacaMov;
                    retorno.DelayAlertaPlacaMov = objPar.DelayAlertaPlacaMov;
                    
                    #endregion

                    #region RecirculacaoAuto
                    
                    retorno.HabilitarRecirculacaoAuto = objPar.HabilitarRecirculacaoAuto;
                    retorno.DelayMonitRecirculacaoAuto = objPar.DelayMonitRecirculacaoAuto;
                    retorno.DelayNotificacaotRecirculacaoAuto = objPar.DelayNotificacaotRecirculacaoAuto;
                    retorno.QtdNotificacaotRecirculacaoAuto = objPar.QtdNotificacaotRecirculacaoAuto;
                    
                    #endregion

                    #region LogBD
                    retorno.LogBD = objPar.LogBD;
                    #endregion
                    retorno.NameRemoteAccess = objPar.NameRemoteAccess;
                    retorno.TempoReciAuto = objPar.TempoReciAuto;

                    retorno.LogStatusMaquina = objPar.LogStatusMaquina;
                }
                else
                {
                    retorno = null;
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectParametros).Name}: ", e);
			}

			return retorno;
        }

        /// <summary>
        /// Valida propriedades do modelo
        /// </summary>
        public bool Validate(ObjectParametros p, out string outMsg)
        {
            if (p == null)
                throw new ArgumentNullException();

            StringBuilder validacoes = new StringBuilder();

            #region Geral

            if (p.ResponseTimeout > ushort.MaxValue)
            {
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametors_Comunicacao_TimeoutFaixa);
            }

            if (p.Velocidade == 0)
            {
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Global_VelocidadeMaiorZero);
            }

            if (p.Aceleracao == 0)
            {
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Global_AceleracaoMaiorZero);
            }

            #endregion

            #region Purga

            if (p.PrazoExecucaoPurga == TimeSpan.Zero)
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Purga_PrazoExecucaoMaiorZero);

            if (p.VolumePurga == 0)
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Purga_VolumeMaiorZero);

            if (p.VelocidadePurga == 0)
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Purga_VelocidadeMaiorZero);

            if (p.AceleracaoPurga == 0)
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Purga_AceleracaoMaiorZero);

            #endregion

            #region DAT

            string diretorio = Path.GetDirectoryName(p.PathMonitoramentoDAT);
            if (!Directory.Exists(diretorio))
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Dat_DiretorioInvalido);

            if (!Directory.Exists(p.PathRepositorioDAT))
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Dat_RepositorioInvalido);

            if (p.PadraoConteudoDAT == 0)
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Dat_PadraoConteudoObrigatorio);

            #endregion

            #region Nível de colorante opcional

            if (p.ControlarNivel)
            {
                if (p.VolumeMinimo == 0)
                    validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Nivel_MinimoMaiorZero);

                if (p.VolumeMaximo == 0)
                    validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Nivel_MaximoMaiorZero);

                if (p.VolumeMaximo < 0)
                    validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Nivel_MaximoMaiorQueMInimo);
            }

            #endregion

            #region Inicialização de colorantes

            if (p.IniPulsoInicial == 0)
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Inicializacao_PulsoInicialMaiorZero);

            if (p.IniPulsoLimite == 0)
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Inicializacao_PulsoLimiteMaiorZero);

            if (p.IniVariacaoPulso == 0)
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Inicializacao_VariacaoPulsoMaiorZero);

            if (p.IniStepVariacao == 0)
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Inicializacao_StepVariacaoMaiorZero);

            if (p.IniAceleracao == 0)
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Inicializacao_AceleracaoMaiorZero);

            if (p.IniVelocidade == 0)
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Inicializacao_VelocidadeMaiorZero);

            if (p.QtdeCircuitoGrupo < 2 || p.QtdeCircuitoGrupo > 5)
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Inicializacao_QtdeCircuitosGrupoFaixa);

            #endregion

            #region Monitoramento dos colorantes

            if (p.MonitVelocidade == 0)
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Monit_VelocidadeMaiorZero);

            if (p.MonitAceleracao == 0)
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Monit_AceleracaoMaiorZero);

            if (p.MonitDelay == 0)
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Monit_DelayMaiorZero);

            if (p.MonitTimerDelay < 10)
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Monit_TimerDelayMaiorDez);

            if (p.MonitTimerDelayIni < 10)
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Monit_TimerDelayIniMaiorDez);

            if (p.QtdeMonitCircuitoGrupo < 2 || p.QtdeMonitCircuitoGrupo > 5)
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Monit_QtdeCircuitosGrupoFaixa);

            if (p.MonitPulsos == 0)
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Monit_PulsoMaiorZero);

            #endregion

            #region Unidade de medida

            /* Ao menos uma unidade de medida deve estar habilitada */
            if (!p.HabilitarShot && !p.HabilitarMililitro && !p.HabilitarOnca && !p.HabilitarGrama)
            {
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_UnidadeMedida_UnidadeEntradaObrigatoria);
            }
            else
            {
                if (p.HabilitarShot && p.ValorShot == 0)
                    validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_UnidadeMedida_ShotMaiorZero);
            }

            if (p.UnidadeMedidaNivelColorante == 0)
            {
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_UnidadeMedida_UnidadeExibicaoObrigatoria);
            }

            #endregion

            #region Log

            if (!Directory.Exists(Path.GetDirectoryName(p.PathLogProcessoDispensa)))
            {
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Log_DiretorioLogProcessoInvalido);
            }

            if (!Directory.Exists(Path.GetDirectoryName(p.PathLogControleDispensa)))
            {
                validacoes.AppendLine(Negocio.IdiomaResxExtensao.Parametros_Log_DiretorioLogQuantidadeDispensadaInvalido);
            }

            #endregion

            outMsg = validacoes.ToString();
            return (validacoes.Length == 0);
        }

        public static bool SetExecucaoPurga(DateTime data_hora)
        {
            bool retorno = false;
            try
            {               
                if (File.Exists(PathFile))
                {
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();

                            cmd.CommandText = "UPDATE Parametros SET DataExecucaoPurga = '" + string.Format("{0:yyyy-MM-dd HH:mm:ss}", data_hora) + "';";
                            
                            cmd.ExecuteNonQuery();

                            conn.Close();
                            retorno = true;
                        }
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectParametros).Name}: ", e);
                throw;
			}

			return retorno;
        }

        public static void SetIdIdioma(int id)
        {
            try
            {
                if (File.Exists(PathFile))
                {
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();

                            cmd.CommandText = "UPDATE Parametros SET IdIdioma = '" + id.ToString() + "';";

                            cmd.ExecuteNonQuery();

                            conn.Close();
                        }
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectParametros).Name}: ", e);
                throw;
			}
		}

        private static string RetunStringFromTimespan(TimeSpan b)
        {
            string retorno = string.Format("{0:D2}:{1:D2}:{2:D2}", b.Hours, b.Minutes, b.Seconds);
            return retorno;
        }

        public static void Persist(ObjectParametros p)
        {
            try
            {
                if (File.Exists(PathFile))
                {
                    StringBuilder sb = new StringBuilder();
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();
                            sb.Append("UPDATE Parametros SET ");
                            #region Geral

                            sb.Append(" ResponseTimeout = '"+ p.ResponseTimeout.ToString() + "', " );
                            sb.Append(" Velocidade = '" + p.Velocidade.ToString() + "', ");
                            sb.Append(" Aceleracao = '" + p.Aceleracao.ToString() + "', ");
                            sb.Append(" RevDelay = '" + p.DelayReverso.ToString() + "', ");
                            sb.Append(" RevPulsos = '" + p.PulsoReverso.ToString() + "', ");
                            sb.Append(" SomarRevPulsos = 'True', ");
                            sb.Append(" HabilitarTecladoVirtual = '" + (p.HabilitarTecladoVirtual ? "True" : "False") + "', ");
                            sb.Append(" HabilitarDispensaSequencial = '" + (p.HabilitarDispensaSequencial ? "True" : "False")  + "', ");
                            sb.Append(" HabilitarFormulaPersonalizada = '" +(p.HabilitarFormulaPersonalizada ? "True" : "False")  + "', ");
                            sb.Append(" HabilitarTesteRecipiente = '" + (p.HabilitarTesteRecipiente ? "True" : "False") + "', ");
                            sb.Append(" HabilitarIdentificacaoCopo = '" + (p.HabilitarIdentificacaoCopo? "True" : "False") + "', ");
                            sb.Append(" IdDispositivo = '" + p.IdDispositivo.ToString() + "', ");
                            sb.Append(" HabilitarPurgaIndividual = '" + (p.HabilitarPurgaIndividual ? "True" : "False") + "', ");
                            sb.Append(" HabilitarTouchScrenn = '" + (p.HabilitarTouchScrenn ? "True" : "False") + "', ");
                            sb.Append(" IdDispositivo2 = '" + p.IdDispositivo2.ToString() + "', ");
                            sb.Append(" NomeDispositivo = '" + p.NomeDispositivo + "', ");
                            sb.Append(" NomeDispositivo2 = '" + p.NomeDispositivo2 + "', ");
                            sb.Append(" HabilitarDispensaSequencialP1 = '" + (p.HabilitarDispensaSequencialP1 ? "True" : "False") + "', ");
                            sb.Append(" HabilitarDispensaSequencialP2 = '" + (p.HabilitarDispensaSequencialP2 ? "True" : "False") + "', ");

                            sb.Append(" TreinamentoCal = '" + (p.TreinamentoCal ? "True" : "False") + "', ");
                            sb.Append(" ViewMessageProc = '" + (p.ViewMessageProc ? "True" : "False") + "', ");

                            sb.Append(" QtdTentativasConexao = '" + p.QtdTentativasConexao + "', ");
                            sb.Append(" DelayEsponja = '" + p.DelayEsponja + "', ");

                            sb.Append(" HabLimpBicos = '" + (p.HabLimpBicos ? "True" : "False") + "', ");
                            sb.Append(" DelayLimpBicos = '" + p.DelayLimpBicos.ToString() + "', ");
                            sb.Append(" TipoLimpBicos = '" + p.TipoLimpBicos.ToString() + "', ");

                            sb.Append(" TipoDosagemExec = '" + p.TipoDosagemExec.ToString() + "', ");
                            
                            #endregion

                            #region DAT

                            sb.Append(" PathDAT = '" + p.PathMonitoramentoDAT + "', ");
                            sb.Append(" PathRepositorioDAT = '" + p.PathRepositorioDAT + "', ");
                            sb.Append(" PadraoConteudoDAT = '" + p.PadraoConteudoDAT.ToString() + "', ");
                            sb.Append(" BasePosicaoCircuitoDAT = '" + p.BasePosicaoCircuitoDAT.ToString() + "', ");
                            sb.Append(" UtilizarCorrespondenciaDAT = '" + (p.UtilizarCorrespondenciaDAT ? "True" : "False") + "', ");
                            sb.Append(" DesabilitarInterfaceDispensaSequencial = '" + (p.DesabilitarInterfaceDispensaSequencial ? "True" : "False") + "', ");
                            sb.Append(" DesabilitarInterfaceDispensaSimultanea = '" + (p.DesabilitarInterfaceDispensaSimultanea ? "True" : "False") + "', ");
                            sb.Append(" DesabilitarInterfaceInicializacaoCircuito = '" + (p.DesabilitarInterfaceInicializacaoCircuito ? "True" : "False") + "', ");
                            sb.Append(" DesabilitarInterfacePurga = '" + (p.DesabilitarInterfacePurga ? "True" : "False") + "', ");

                            sb.Append(" PathFilaDAT = '" + p.PathMonitoramentoFilaDAT + "', ");
                            sb.Append(" DesabilitarMonitoramentoFilaDAT = '" + (p.DesabilitarMonitoramentoFilaDAT ? "True" : "False") + "', ");
                            sb.Append(" DelayMonitoramentoFilaDAT = '" + p.DelayMonitoramentoFilaDAT.ToString() + "', ");

                            sb.Append(" DesabilitarVolumeMinimoDat = '" + (p.DesabilitarVolumeMinimoDat ? "True" : "False") + "', ");
                            sb.Append(" VolumeMinimoDat = '" + p.VolumeMinimoDat.ToString().Replace(",", ".") + "', ");

                            sb.Append(" Dat_06_UNT_Pref = '" + p.Dat_06_UNT_Pref+ "', ");
                            sb.Append(" Dat_06_UNT_1_IsPonto = '" + p.Dat_06_UNT_1_IsPonto.ToString() + "', ");
                            sb.Append(" Dat_06_UNT_2_IsPonto = '" + p.Dat_06_UNT_2_IsPonto.ToString() + "', ");

                            sb.Append(" Dat_06_CAN_Pref = '" + p.Dat_06_CAN_Pref + "', ");
                            sb.Append(" Dat_06_CAN_1_IsPonto = '" + p.Dat_06_CAN_1_IsPonto.ToString() + "', ");

                            sb.Append(" Dat_06_FRM_Pref = '" + p.Dat_06_FRM_Pref + "', ");
                            sb.Append(" Dat_06_FRM_SEP = '" + p.Dat_06_FRM_SEP + "', ");
                            sb.Append(" Dat_06_FRM_1_IsPonto = '" + p.Dat_06_FRM_1_IsPonto.ToString() + "', ");

                            sb.Append(" Dat_06_BAS_Pref = '" + p.Dat_06_BAS_Pref + "', ");
                            sb.Append(" Dat_06_BAS_1_IsPonto = '" + p.Dat_06_BAS_1_IsPonto.ToString() + "', ");

                            sb.Append(" Dat_06_BAS_Habilitado = '" + p.Dat_06_BAS_Habilitado.ToString() + "', ");

                            sb.Append(" Dat_05_UNT_Pref = '" + p.Dat_05_UNT_Pref + "', ");
                            sb.Append(" Dat_05_UNT_1_IsPonto = '" + p.Dat_05_UNT_1_IsPonto.ToString() + "', ");
                            sb.Append(" Dat_05_UNT_2_IsPonto = '" + p.Dat_05_UNT_2_IsPonto.ToString() + "', ");

                            sb.Append(" Dat_05_CAN_Pref = '" + p.Dat_05_CAN_Pref + "', ");
                            sb.Append(" Dat_05_CAN_1_IsPonto  = '" + p.Dat_05_CAN_1_IsPonto.ToString() + "', ");

                            sb.Append(" Dat_05_FRM_Pref = '" + p.Dat_05_FRM_Pref + "', ");
                            sb.Append(" Dat_05_FRM_SEP = '" + p.Dat_05_FRM_SEP + "', ");
                            sb.Append(" Dat_05_FRM_1_IsPonto = '" + p.Dat_05_FRM_1_IsPonto.ToString() + "', ");

                            sb.Append(" Dat_05_BAS_Pref = '" + p.Dat_05_BAS_Pref + "', ");
                            sb.Append(" Dat_05_BAS_1_IsPonto = '" + p.Dat_05_BAS_1_IsPonto.ToString() + "', ");

                            sb.Append(" Dat_05_BAS_Habilitado = '" + p.Dat_05_BAS_Habilitado.ToString() + "', ");

                            sb.Append(" ExtFileTmpUDCP = '" + p.ExtFileTmpUDCP.ToString() + "', ");
                            sb.Append(" CreateFileTmpUDCP = '" + p.CreateFileTmpUDCP.ToString() + "', ");
                            sb.Append(" DelayUDCP = '" + p.DelayUDCP.ToString() + "', ");

                            sb.Append(" ProcRemoveLataUDCP = '" + (p.ProcRemoveLataUDCP ? "True" : "False") + "', ");

                            sb.Append(" DisablePopUpDispDat = '" + (p.DisablePopUpDispDat ? "True" : "False") + "', ");

                            #endregion

                            #region Purga

                            sb.Append(" PrazoExecucaoPurga = '" + RetunStringFromTimespan(p.PrazoExecucaoPurga) + "', ");
                            sb.Append(" VolumePurga = '" + p.VolumePurga.ToString().Replace(",", ".") + "', ");
                            sb.Append(" VelocidadePurga = '" + p.VelocidadePurga.ToString() + "', ");
                            sb.Append(" AceleracaoPurga = '" + p.AceleracaoPurga.ToString() + "', ");
                            sb.Append(" DelayPurga = '" + p.DelayPurga.ToString() + "', ");
                            sb.Append(" ControlarExecucaoPurga = '" + (p.ControlarExecucaoPurga ? "True" : "False") + "', ");
                            sb.Append(" ExigirExecucaoPurga = '" + (p.ExigirExecucaoPurga ? "True" : "False") + "', ");
                            sb.Append(" PurgaSequencial = '" + (p.PurgaSequencial ? "True" : "False") + "', ");

                            #endregion

                            #region Controle de volume

                            sb.Append(" VolumeMinimo = '" + p.VolumeMinimo.ToString().Replace(",", ".") + "', ");
                            sb.Append(" VolumeMaximo = '" + p.VolumeMaximo.ToString().Replace(",", ".") + "', ");
                            sb.Append(" ControlarVolume = '" + (p.ControlarNivel ? "True" : "False") + "', ");

                            #endregion

                            #region Inicialização de circuitos

                            sb.Append(" IniPulsoInicial = '" + p.IniPulsoInicial.ToString() + "', ");
                            sb.Append(" IniPulsoLimite = '" + p.IniPulsoLimite.ToString() + "', ");
                            sb.Append(" IniVariacaoPulso = '" + p.IniVariacaoPulso.ToString() + "', ");
                            sb.Append(" IniStepVariacao = '" + p.IniStepVariacao.ToString().Replace(",", ".") + "', ");
                            sb.Append(" IniVelocidade = '" + p.IniVelocidade.ToString() + "', ");
                            sb.Append(" IniAceleracao = '" + p.IniAceleracao.ToString() + "', ");
                            sb.Append(" IniMovimentoReverso = '" + (p.IniMovimentoReverso ? "True" : "False") + "', ");
                            sb.Append(" InicializarCircuitosPurga = '" + (p.InicializarCircuitosPurga ? "True" : "False") + "', ");
                            sb.Append(" InicializarCircuitosPurgaIndividual = '" + (p.InicializarCircuitosPurgaIndividual ? "True" : "False") + "', ");
                            sb.Append(" QtdeCircuitoGrupo = '" + p.QtdeCircuitoGrupo.ToString() + "', ");

                            #endregion

                            #region Monitoramento dos circuitos

                            sb.Append(" QtdeMonitCircuitoGrupo = '" + p.QtdeMonitCircuitoGrupo.ToString() + "', ");
                            sb.Append(" DesabilitarInterfaceMonitCircuito = '" + (p.DesabilitarInterfaceMonitCircuito ? "True" : "False") + "', ");
                            sb.Append(" DesabilitarProcessoMonitCircuito = '" + (p.DesabilitarProcessoMonitCircuito ? "True" : "False") + "', ");
                            sb.Append(" MonitMovimentoReverso = '" + (p.MonitMovimentoReverso ? "True" : "False") + "', ");
                            sb.Append(" MonitVelocidade = '" + p.MonitVelocidade.ToString() + "', ");
                            sb.Append(" MonitAceleracao = '" + p.MonitAceleracao.ToString() + "', ");
                            sb.Append(" MonitDelay = '" + p.MonitDelay.ToString() + "', ");
                            sb.Append(" MonitTimerDelay = '" + p.MonitTimerDelay.ToString() + "', ");
                            sb.Append(" MonitTimerDelayIni = '" + p.MonitTimerDelayIni.ToString() + "', ");
                            sb.Append(" MonitPulsos = '" + p.MonitPulsos.ToString() + "', ");

                            #endregion

                            #region Unidade de medida

                            sb.Append(" ValorShot = '" + p.ValorShot.ToString().Replace(",", ".") + "', ");
                            sb.Append(" HabilitarShot = '" + (p.HabilitarShot ? "True" : "False") + "', ");
                            sb.Append(" HabilitarOnca = '" + (p.HabilitarOnca ? "True" : "False") + "', ");
                            sb.Append(" HabilitarMililitro = '" + (p.HabilitarMililitro ? "True" : "False") + "', ");
                            sb.Append(" HabilitarGrama = '" + (p.HabilitarGrama ? "True" : "False") + "', ");
                            sb.Append(" UnidadeMedidaNivelColorante = '" + p.UnidadeMedidaNivelColorante.ToString() + "', ");

                            sb.Append(" ValorFraction = '" + p.ValorFraction.ToString() + "', ");
                            
                            #endregion

                            #region Log

                            sb.Append(" PathLogProcessoDispensa = '" + p.PathLogProcessoDispensa + "', ");
                            sb.Append(" PathLogControleDispensa = '" + p.PathLogControleDispensa + "', ");
                            sb.Append(" HabilitarLogComunicacao = '" + (p.HabilitarLogComunicacao ? "True" : "False") + "', ");
                            sb.Append(" PathLogComunicacao = '" + p.PathLogComunicacao + "', ");

                            sb.Append(" HabilitarLogAutomateTesterProt = '" + (p.HabilitarLogAutomateTesterProt ? "True" : "False") + "', ");
                            sb.Append(" LogAutomateBackup = '" + (p.LogAutomateBackup ? "True" : "False") + "', ");
                            
                            #endregion

                            #region Produção

                            sb.Append(" TipoProducao = '" + p.TipoProducao + "', ");
                            sb.Append(" IpProducao = '" + p.IpProducao + "', ");
                            sb.Append(" PortaProducao = '" + p.PortaProducao + "', ");
                            sb.Append(" DesabilitaMonitProcessoProducao = '" + (p.DesabilitaMonitProcessoProducao ? "True" : "False") + "', ");
                            #endregion

                            #region Sinc Formula
                            sb.Append(" DesabilitaMonitSincFormula = '" + ( p.DesabilitaMonitSincFormula ? "True" : "False") + "', ");
                            sb.Append(" PortaSincFormula = '" + p.PortaSincFormula + "', ");
                            sb.Append(" IpSincFormula = '" + p.IpSincFormula + "', ");
                            #endregion

                            sb.Append(" TimeoutPingTcp = '" + p.TimeoutPingTcp.ToString() + "', ");
                            
                            #region Sinc Token
                            sb.Append(" DesabilitaMonitSyncToken = '" + (p.DesabilitaMonitSyncToken ? "True" : "False") + "', ");
                            sb.Append(" IpSincToken  = '" + p.IpSincToken + "', ");
                            sb.Append(" PortaSincToken = '" + p.PortaSincToken + "', ");
                            sb.Append(" TipoEventos = '" + p.TipoEventos + "', ");
                            #endregion

                            #region Sinc BkpCalibragem
                            sb.Append(" DesabilitaMonitSyncBkpCalibragem = '" + (p.DesabilitaMonitSyncBkpCalibragem ? "True" : "False") + "', ");
                            sb.Append(" UrlSincBkpCalibragem  = '" + p.UrlSincBkpCalibragem + "', ");
                            #endregion

                            #region Recirculacao
                            sb.Append(" HabilitarRecirculacao = '" + (p.HabilitarRecirculacao ? "True" : "False") + "', ");
                            sb.Append(" DelayMonitRecirculacao = '" + p.DelayMonitRecirculacao.ToString() + "', ");
                            #endregion

                            #region Placa Movimentacao
                            sb.Append(" Address_PlacaMov = '" + p.Address_PlacaMov.ToString() + "', ");
                            sb.Append(" NomeDispositivo_PlacaMov = '" + p.NomeDispositivo_PlacaMov.ToString() + "', ");
                            sb.Append(" DelayAlertaPlacaMov = '" + p.DelayAlertaPlacaMov.ToString() + "', ");

                            #endregion

                            #region RecirculacaoAuto
                            sb.Append(" HabilitarRecirculacaoAuto = '" + (p.HabilitarRecirculacaoAuto ? "True" : "False") + "', ");
                            sb.Append(" DelayMonitRecirculacaoAuto = '" + p.DelayMonitRecirculacaoAuto.ToString() + "', ");
                            sb.Append(" DelayNotificacaotRecirculacaoAuto = '" + p.DelayNotificacaotRecirculacaoAuto.ToString() + "', ");
                            sb.Append(" QtdNotificacaotRecirculacaoAuto = '" + p.QtdNotificacaotRecirculacaoAuto.ToString() + "', ");
                            sb.Append(" TempoReciAuto = '" + p.TempoReciAuto.ToString() + "', ");
                            
                            #endregion

                            #region LogBD
                            sb.Append(" LogBD = '" + (p.LogBD ? "True" : "False") + "', ");
                            #endregion

                            sb.Append(" NameRemoteAccess = '" + p.NameRemoteAccess + "', ");
                            
                            #region  Base de Dados
                            sb.Append(" TipoBaseDados = '" + p.TipoBaseDados + "', ");
                            sb.Append(" PathBasesDados = '" + p.PathBasesDados + "', ");
                            #endregion
                            sb.Append(" LogStatusMaquina = '" + (p.LogStatusMaquina ? "True" : "False")  + "'; ");
                            
                            cmd.CommandText = sb.ToString();

                            cmd.ExecuteNonQuery();

                            conn.Close();
                        }
                    }

                    gerarEventoALteradaConfiguacao(0, "");
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectParametros).Name}: ", e);
                throw;
			}
		}

        public static void PersistInsert(ObjectParametros p)
        {
            try
            {
                if (File.Exists(PathFile))
                {
                    StringBuilder sb = new StringBuilder();
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
					{
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();
                            sb.Append("INSERT INTO Parametros ( ");

                            #region Geral

                            sb.Append(" ResponseTimeout, ");
                            sb.Append(" Velocidade, ");
                            sb.Append(" Aceleracao, ");
                            sb.Append(" RevDelay, ");
                            sb.Append(" RevPulsos, ");
                            sb.Append(" SomarRevPulsos, ");
                            sb.Append(" HabilitarTecladoVirtual, ");
                            sb.Append(" HabilitarDispensaSequencial, ");
                            sb.Append(" HabilitarFormulaPersonalizada, ");
                            sb.Append(" HabilitarTesteRecipiente, ");
                            sb.Append(" IdDispositivo, ");
                            sb.Append(" HabilitarPurgaIndividual, ");
                            sb.Append(" HabilitarTouchScrenn, ");
                            sb.Append(" VersaoIoconnect, ");
                            sb.Append(" IdDispositivo2, ");
                            sb.Append(" NomeDispositivo, ");
                            sb.Append(" NomeDispositivo2, ");
                            sb.Append(" HabilitarDispensaSequencialP1, ");
                            sb.Append(" HabilitarDispensaSequencialP2, ");

                            #endregion

                            #region DAT

                            sb.Append(" PathDAT, ");
                            sb.Append(" PathRepositorioDAT, ");
                            sb.Append(" PadraoConteudoDAT, ");
                            sb.Append(" BasePosicaoCircuitoDAT, ");
                            sb.Append(" UtilizarCorrespondenciaDAT, ");
                            sb.Append(" DesabilitarInterfaceDispensaSequencial, ");
                            sb.Append(" DesabilitarInterfaceDispensaSimultanea, ");
                            sb.Append(" DesabilitarInterfaceInicializacaoCircuito, ");
                            sb.Append(" DesabilitarInterfacePurga, ");

                            sb.Append(" DesabilitarVolumeMinimoDat, ");
                            sb.Append(" VolumeMinimoDat, ");

                            #endregion

                            #region Purga

                            sb.Append(" PrazoExecucaoPurga, ");
                            sb.Append(" VolumePurga, ");
                            sb.Append(" VelocidadePurga, ");
                            sb.Append(" AceleracaoPurga, ");
                            sb.Append(" DelayPurga, ");
                            sb.Append(" ControlarExecucaoPurga, ");
                            sb.Append(" ExigirExecucaoPurga, ");
                            sb.Append(" PurgaSequencial, ");

                            #endregion

                            #region Controle de volume

                            sb.Append(" VolumeMinimo, ");
                            sb.Append(" VolumeMaximo, ");
                            sb.Append(" ControlarVolume, ");

                            #endregion

                            #region Inicialização de circuitos

                            sb.Append(" IniPulsoInicial, ");
                            sb.Append(" IniPulsoLimite, ");
                            sb.Append(" IniVariacaoPulso, ");
                            sb.Append(" IniStepVariacao, ");
                            sb.Append(" IniVelocidade, ");
                            sb.Append(" IniAceleracao, ");
                            sb.Append(" IniMovimentoReverso, ");
                            sb.Append(" InicializarCircuitosPurga, ");
                            sb.Append(" InicializarCircuitosPurgaIndividual, ");
                            sb.Append(" QtdeCircuitoGrupo, ");

                            #endregion

                            #region Monitoramento dos circuitos

                            sb.Append(" QtdeMonitCircuitoGrupo, ");
                            sb.Append(" DesabilitarInterfaceMonitCircuito, ");
                            sb.Append(" DesabilitarProcessoMonitCircuito, ");
                            sb.Append(" MonitMovimentoReverso, ");
                            sb.Append(" MonitVelocidade, ");
                            sb.Append(" MonitAceleracao, ");
                            sb.Append(" MonitDelay, ");
                            sb.Append(" MonitTimerDelay, ");
                            sb.Append(" MonitTimerDelayIni, ");
                            sb.Append(" MonitPulsos, ");

                            #endregion

                            #region Unidade de medida

                            sb.Append(" ValorShot, ");
                            sb.Append(" HabilitarShot, ");
                            sb.Append(" HabilitarOnca, ");
                            sb.Append(" HabilitarMililitro, ");
                            sb.Append(" HabilitarGrama, ");
                            sb.Append(" UnidadeMedidaNivelColorante, ");

                            sb.Append(" ValorFraction, ");
                            
                            #endregion

                            #region Log

                            sb.Append(" PathLogProcessoDispensa, ");
                            sb.Append(" PathLogControleDispensa, ");
                            sb.Append(" HabilitarLogComunicacao, ");
                            sb.Append(" PathLogComunicacao, ");
                            
                            #endregion

                            #region Produção

                            sb.Append(" TipoProducao, ");
                            sb.Append(" IpProducao, ");
                            sb.Append(" PortaProducao, ");
                            sb.Append(" DesabilitaMonitProcessoProducao, ");

                            #endregion

                            #region Sinc Formula
                            sb.Append(" DesabilitaMonitSincFormula, ");
                            sb.Append(" PortaSincFormula, ");
                            sb.Append(" IpSincFormula ");

                            #endregion

                            //Values
                            sb.Append(") VALUES ( ");

                            #region Geral

                            sb.Append("'" + p.ResponseTimeout.ToString() + "', ");
                            sb.Append("'" + p.Velocidade.ToString() + "', ");
                            sb.Append("'" + p.Aceleracao.ToString() + "', ");
                            sb.Append("'" + p.DelayReverso.ToString() + "', ");
                            sb.Append("'" + p.PulsoReverso.ToString() + "', ");
                            sb.Append("'True', ");
                            sb.Append("'" + (p.HabilitarTecladoVirtual ? "True" : "False") + "', ");
                            sb.Append("'" + (p.HabilitarDispensaSequencial ? "True" : "False") + "', ");
                            sb.Append("'" + (p.HabilitarFormulaPersonalizada ? "True" : "False") + "', ");
                            sb.Append("'" + (p.HabilitarTesteRecipiente ? "True" : "False") + "', ");
                            sb.Append("'" + p.IdDispositivo.ToString() + "', ");
                            sb.Append("'" + (p.HabilitarPurgaIndividual ? "True" : "False") + "', ");
                            sb.Append("'" + (p.HabilitarTouchScrenn ? "True" : "False") + "', ");
                            sb.Append("'" + p.VersaoIoconnect + "', ");
                            sb.Append("'" + p.IdDispositivo2.ToString() + "', ");
                            sb.Append("'" + p.NomeDispositivo + "', ");
                            sb.Append("'" + p.NomeDispositivo2 + "', ");
                            sb.Append("'" + (p.HabilitarDispensaSequencialP1 ? "True" : "False") + "', ");
                            sb.Append("'" + (p.HabilitarDispensaSequencialP2 ? "True" : "False") + "', ");

                            #endregion
                            
                            #region DAT

                            sb.Append("'" + p.PathMonitoramentoDAT + "', ");
                            sb.Append("'" + p.PathRepositorioDAT + "', ");
                            sb.Append("'" + p.PadraoConteudoDAT.ToString() + "', ");
                            sb.Append("'" + p.BasePosicaoCircuitoDAT.ToString() + "', ");
                            sb.Append("'" + (p.UtilizarCorrespondenciaDAT ? "True" : "False") + "', ");
                            sb.Append("'" + (p.DesabilitarInterfaceDispensaSequencial ? "True" : "False") + "', ");
                            sb.Append("'" + (p.DesabilitarInterfaceDispensaSimultanea ? "True" : "False") + "', ");
                            sb.Append("'" + (p.DesabilitarInterfaceInicializacaoCircuito ? "True" : "False") + "', ");
                            sb.Append("'" + (p.DesabilitarInterfacePurga ? "True" : "False") + "', ");

                            sb.Append("'" + (p.DesabilitarVolumeMinimoDat ? "True" : "False") + "', ");
                            sb.Append("'" + p.VolumeMinimoDat.ToString().Replace(",", ".") + "', ");

                            #endregion

                            #region Purga

                            sb.Append("'" + RetunStringFromTimespan(p.PrazoExecucaoPurga) + "', ");
                            sb.Append("'" + p.VolumePurga.ToString().Replace(",", ".") + "', ");
                            sb.Append("'" + p.VelocidadePurga.ToString() + "', ");
                            sb.Append("'" + p.AceleracaoPurga.ToString() + "', ");
                            sb.Append("'" + p.DelayPurga.ToString() + "', ");
                            sb.Append("'" + (p.ControlarExecucaoPurga ? "True" : "False") + "', ");
                            sb.Append("'" + (p.ExigirExecucaoPurga ? "True" : "False") + "', ");
                            sb.Append("'" + (p.PurgaSequencial ? "True" : "False") + "', ");

                            #endregion

                            #region Controle de volume

                            sb.Append("'" + p.VolumeMinimo.ToString().Replace(",", ".") + "', ");
                            sb.Append("'" + p.VolumeMaximo.ToString().Replace(",", ".") + "', ");
                            sb.Append("'" + (p.ControlarNivel ? "True" : "False") + "', ");

                            #endregion

                            #region Inicialização de circuitos

                            sb.Append("'" + p.IniPulsoInicial.ToString() + "', ");
                            sb.Append("'" + p.IniPulsoLimite.ToString() + "', ");
                            sb.Append("'" + p.IniVariacaoPulso.ToString() + "', ");
                            sb.Append("'" + p.IniStepVariacao.ToString().Replace(",", ".") + "', ");
                            sb.Append("'" + p.IniVelocidade.ToString() + "', ");
                            sb.Append("'" + p.IniAceleracao.ToString() + "', ");
                            sb.Append("'" + (p.IniMovimentoReverso ? "True" : "False") + "', ");
                            sb.Append("'" + (p.InicializarCircuitosPurga ? "True" : "False") + "', ");
                            sb.Append("'" + (p.InicializarCircuitosPurgaIndividual ? "True" : "False") + "', ");
                            sb.Append("'" + p.QtdeCircuitoGrupo.ToString() + "', ");

                            #endregion

                            #region Monitoramento dos circuitos

                            sb.Append("'" + p.QtdeMonitCircuitoGrupo.ToString() + "', ");
                            sb.Append("'" + (p.DesabilitarInterfaceMonitCircuito ? "True" : "False") + "', ");
                            sb.Append("'" + (p.DesabilitarProcessoMonitCircuito ? "True" : "False") + "', ");
                            sb.Append("'" + (p.MonitMovimentoReverso ? "True" : "False") + "', ");
                            sb.Append("'" + p.MonitVelocidade.ToString() + "', ");
                            sb.Append("'" + p.MonitAceleracao.ToString() + "', ");
                            sb.Append("'" + p.MonitDelay.ToString() + "', ");
                            sb.Append("'" + p.MonitTimerDelay.ToString() + "', ");
                            sb.Append("'" + p.MonitTimerDelayIni.ToString() + "', ");
                            sb.Append("'" + p.MonitPulsos.ToString() + "', ");

                            #endregion

                            #region Unidade de medida

                            sb.Append("'" + p.ValorShot.ToString().Replace(",", ".") + "', ");
                            sb.Append("'" + (p.HabilitarShot ? "True" : "False") + "', ");
                            sb.Append("'" + (p.HabilitarOnca ? "True" : "False") + "', ");
                            sb.Append("'" + (p.HabilitarMililitro ? "True" : "False") + "', ");
                            sb.Append("'" + (p.HabilitarGrama ? "True" : "False") + "', ");
                            sb.Append("'" + p.UnidadeMedidaNivelColorante.ToString() + "', ");

                            sb.Append("'" + p.ValorFraction.ToString() + "', ");

							#endregion

							#region Log

							sb.Append("'" + p.PathLogProcessoDispensa + "', ");
                            sb.Append("'" + p.PathLogControleDispensa + "', ");
                            sb.Append("'" + (p.HabilitarLogComunicacao ? "True" : "False") + "', ");
                            sb.Append("'" + p.PathLogComunicacao + "', ");
                            
                            #endregion

                            #region Produção

                            sb.Append("'" + p.TipoProducao + "', ");
                            sb.Append("'" + p.IpProducao + "', ");
                            sb.Append("'" + p.PortaProducao + "', ");
                            sb.Append("'" + (p.DesabilitaMonitProcessoProducao ? "True" : "False") + "', ");

                            #endregion

                            #region Sinc Formula
                            sb.Append("'" + (p.DesabilitaMonitSincFormula ? "True" : "False") + "', ");
                            sb.Append("'" + p.PortaSincFormula + "', ");
                            sb.Append("'" + p.IpSincFormula + "'); ");

                            #endregion

                            cmd.CommandText = sb.ToString();

                            cmd.ExecuteNonQuery();

                            conn.Close();

                        }
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectParametros).Name}: ", e);
                throw;
			}
		}

        private static int gerarEventoALteradaConfiguacao(int result, string detalhes = "")
        {
            int retorno = 0;
            try
            {
                #region gravar Evento Alterada Configuração
                Util.ObjectEventos objEvt = new Util.ObjectEventos();
                objEvt.DATAHORA = DateTime.Now;
                objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.ALteradaConfiguacao;
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
				LogManager.LogError($"Erro no módulo {typeof(ObjectParametros).Name}: ", e);
			}

			return retorno;
        }

        public static DataSet getTables()
        {
            DataSet ds = new DataSet();
            if (File.Exists(PathFile))
            {
                using (SQLiteConnection con = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    con.Open();
                    DataTable dt = con.GetSchema("Tables");
                    foreach (DataRow row in dt.Rows)
                    {
                        string tablename = (string)row[2];
                        DataTable tbl = new DataTable(tablename);
                        DataTable columnsTable = con.GetSchema("Columns", new[] { null, null, tablename, null });

                        bool incluir = false;

                        foreach (DataRow rownm in columnsTable.Rows)
                        {
                            string colname = rownm["COLUMN_NAME"].ToString();
                            DataColumn col = new DataColumn(colname);
                            string data_type = rownm["DATA_TYPE"].ToString();
                            if (data_type != "")
                            {
                                if (data_type == "integer")
                                {
                                    col.DataType = typeof(System.Int32);
                                }
                                else if (data_type == "text")
                                {
                                    col.DataType = typeof(System.String);
                                }
                                incluir = true;
                                tbl.Columns.Add(col);
                            }
                        }
                        if (incluir)
                        {
                            ds.Tables.Add(tbl);
                        }
                    }
                    con.Close();
                }
            }

            return ds;
        }
        #endregion
    }
}