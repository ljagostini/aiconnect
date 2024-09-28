using Microsoft.Win32;
using Percolore.Core;
using Percolore.Core.Logging;
using Percolore.Core.Persistence.WindowsRegistry;
using Percolore.Core.Security.License;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Percolore.IOConnect
{
	public class Init
    {

        public static int versaoReleaseBD = 66;

        /// <summary>
        /// Valida pré-requisitos
        /// </summary>
        public static bool PreRequisitosOk(out string report)
        {
            string path = string.Empty;
            string[] pathList = null;
            StringBuilder reportList = new StringBuilder();

            #region Data e hora da máquina

            PercoloreRegistry percRegistry = new PercoloreRegistry();

            try
            {
                // Data e hora da máquina gravada no registro
                long tsDataHoraMaquina = 0;
                tsDataHoraMaquina = percRegistry.GetDataHoraMaquina();
                DateTimeOffset dataRegistro =
                    DateTimeOffset.FromUnixTimeSeconds(tsDataHoraMaquina);

                //Data e hora atuais
                DateTimeOffset dataCorrente = DateTimeOffset.UtcNow;

                /* A data e hora da máquina deve ser maior que a gravada no registro. */
                if (dataCorrente < dataRegistro)
                {
                    reportList.AppendLine(Negocio.IdiomaResxExtensao.Init_RelogioAlterado);
                }
                else
                {
                    //Se hora da máquina não foi atrasada, grava data e hora locais no registro
                    percRegistry.SetDataHoraMaquina();
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Init).Name}: ", e);
			    reportList.AppendLine(Negocio.IdiomaResxExtensao.Init_ValidarDataHora);
            }
            finally
            {
                percRegistry.Dispose();
            }

            #endregion
                       
            #region Diretórios configurados no arquivo xml

            Util.ObjectParametros parametros = Util.ObjectParametros.Load();
            if (parametros != null)
            {
                pathList = new string[]{
                Path.GetDirectoryName(parametros.PathLogProcessoDispensa),
                Path.GetDirectoryName(parametros.PathLogControleDispensa),
                Path.GetDirectoryName(parametros.PathRepositorioDAT),
                parametros.PathRepositorioDAT};

                for (int i = 0; i < pathList.Length; i++)
                {
                    try
                    {
                        path = pathList[i];
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                    }
					catch (Exception e)
					{
						LogManager.LogError($"Erro no módulo {typeof(Init).Name}: ", e);
					    reportList.AppendLine(
                            $"{Negocio.IdiomaResxExtensao.Init_Falha_Diretorio} { path}.");
                    }
                }
            }

            #endregion
          
            if (reportList.Length == 0)
            {
                report = string.Empty;
                return true;
            }
            else
            {
                report = reportList.ToString();
                return false;
            }
        }

        /// <summary>
        /// Executa atualizações e correções cumulativas
        /// </summary>
        /// 
        public static bool RegistroAtualizadoOk()
        {
            PercoloreRegistry percRegistry = new PercoloreRegistry();
            IOConnectRegistry icntRegistry = new IOConnectRegistry();

            try
            {
                #region Licença

                string DEPRECEATED_SUBKEY = @"206dff3e-f775-4cb3-9251-b933070e7e4a";
                RegistryKey rkDepreceatedLicence = Registry.CurrentUser.OpenSubKey(DEPRECEATED_SUBKEY);
                if (rkDepreceatedLicence != null)
                {
                    object CA = rkDepreceatedLicence.GetValue("CA", null);
                    if (CA != null)
                        icntRegistry.SetLicenca(CA.ToString());

                    Registry.CurrentUser.DeleteSubKey(DEPRECEATED_SUBKEY);
                }

                if (rkDepreceatedLicence != null)
                    rkDepreceatedLicence.Dispose();

                #endregion

                /*Persiste senha de administrador caso não exista*/
                string senha = "avsb";
                if (icntRegistry.GetSenhaAdmnistrador() == string.Empty)
                {
                    icntRegistry.SetSenhaAdministrador(senha);
                }

                /* Corrige número serial para 4 dígitos.
                 * Em algumas máquinas foi permitido que se digitasse serial com 5 dígitos.*/
                string serial = percRegistry.GetSerialNumber();
                if (serial.Length == 5)
                {
                    percRegistry.SetSerialNumber(serial.Substring(1));
                }

                #region Chamada ao teclado virtual

                const string TABTIP_SUBKEY = @"SOFTWARE\Microsoft\TabletTip\1.7";
                RegistryKey rkSubKeyTabTip =
                    Registry.CurrentUser.OpenSubKey(TABTIP_SUBKEY, true);
                if (rkSubKeyTabTip != null)
                {
                    rkSubKeyTabTip.SetValue("EnableDesktopModeAutoInvoke", 1);
                }

                if (rkSubKeyTabTip != null)
                    rkSubKeyTabTip.Dispose();

                #endregion

                #region Barra de tarefas

                const string ADVANCED_SUBKEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
                RegistryKey rkSubKeyAdvanced =
                    Registry.CurrentUser.OpenSubKey(ADVANCED_SUBKEY, true);
                if (rkSubKeyAdvanced != null)
                {
                    //Habilita exibição de ícones na barra de tarefa
                    rkSubKeyAdvanced.SetValue("TaskbarAppsVisibleInTabletMode", 1);

                    //Habilita notificações na barra de tarefas
                    rkSubKeyAdvanced.SetValue("UseTabletModeNotificationIcons", 0);
                }

                if (rkSubKeyAdvanced != null)
                    rkSubKeyAdvanced.Dispose();

                #endregion

                return true;
            }
            catch (Exception ex)
            {
                string log = LogManager.ExceptionToLog(ex);
                Log.Logar(TipoLog.Erro, Util.ObjectParametros.PathDiretorioSistema, log);

                return false;
            }
            finally
            {
                icntRegistry.Dispose();
                percRegistry.Dispose();
            }
        }

        /// <summary>
        /// Verifica se já existe instância do processo em execução
        /// </summary>
        /// <returns></returns>
        public static bool ProcessoEmExecucao()
        {
            Process[] localProcesses = Process.GetProcesses();
            Process currentProcess = Process.GetCurrentProcess();
            int currentProcessId = currentProcess.Id;
            string currentProcessName = currentProcess.ProcessName;

            try
            {
                /* Procura nos processos locais outro processo com nome igual 
                 * mas id diferente do processo corrente */
                Process foundProcess = localProcesses.FirstOrDefault(
                    lp => lp.ProcessName.Equals(currentProcessName) && !lp.Id.Equals(currentProcessId));

                return (foundProcess != null);
            }
            catch (Exception ex)
            {
                string log = LogManager.ExceptionToLog(ex);
                Log.Logar(TipoLog.Erro, Util.ObjectParametros.PathDiretorioSistema, log);
                return false;
            }
            finally
            {
                currentProcess.Dispose();
            }
        }

        /// <summary>
        /// Define cultura da aplicação
        /// </summary>
        /// <returns></returns>
        public static void DefineCultura()
        {
            //Cria cultura padrão
            CultureInfo Culture = new CultureInfo("pt-BR");

            //Personaliza itens da cultura padrão
            Culture.NumberFormat.NumberDecimalSeparator = ".";
            Culture.NumberFormat.NumberGroupSeparator = "";
            Culture.NumberFormat.PercentDecimalSeparator = ".";
            Culture.NumberFormat.PercentGroupSeparator = "";
            Culture.NumberFormat.CurrencySymbol = "R$";

            //Recupera idioma selecionado pelo usuário
            Idioma idioma = (Idioma)Util.ObjectParametros.Load().IdIdioma;
            switch (idioma)
            {
                case Idioma.Español:
                    {
                        Culture.NumberFormat.CurrencySymbol = "$";
                        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es");
                        break;
                    }
                default:
                    {
                        CultureInfo.DefaultThreadCurrentUICulture = Culture;
                        break;
                    }
            }

            //Define culture utilizada pela aplicação
            CultureInfo.DefaultThreadCurrentCulture = Culture;
        }

        /// <summary>
        /// Valida se número serial foi informado e caso não exibe interface
        /// para entrada do usuário.
        /// </summary>
        /// <returns></returns>
        public static bool NumeroSerialOk()
        {
            PercoloreRegistry percRegistry = new PercoloreRegistry();

            try
            {
                string serial = percRegistry.GetSerialNumber();
                if (!string.IsNullOrWhiteSpace(serial))
                {
                    return true;
                }

                using (fNumeroSerie f = new fNumeroSerie())
                {
                    f.ShowDialog();
                    serial = f.Serial;
                }

                if (serial == string.Empty)
                    return false;

                percRegistry.SetSerialNumber(serial);
                return true;
            }
            catch (Exception ex)
            {
                string log = LogManager.ExceptionToLog(ex);
                Log.Logar(TipoLog.Erro, Util.ObjectParametros.PathDiretorioSistema, log);
                return false;
            }
            finally
            {
                if (percRegistry != null)
                    percRegistry.Dispose();
            }
        }

        public static bool RegUDCP_OK( Util.ObjectParametros _par )
        {
            bool retorno = true;
            string msgLog = "RegUDCP_OK";
            switch ((DatPattern)_par.PadraoConteudoDAT)
            {
                case DatPattern.PadraoUDCP:
                    {
                        msgLog += "|DatPattern.PadraoUDCP";
                        string _udcp_reg = GetUDCP();
                        if(string.IsNullOrEmpty(_udcp_reg))
                        {
                            msgLog += "|setRegister";
                            //Setar os registros tanto no wwin 32 ou win 64
                            string[] arrayStr = _par.PathMonitoramentoDAT.Split(Path.DirectorySeparatorChar);
                            string strPath = "";
                            for (int i = 0; i < arrayStr.Length - 1; i++)
                            {
                                strPath += arrayStr[i] + Path.DirectorySeparatorChar;
                            }
                            SetUDCPCommandPath(strPath);
                            SetUDCPDriver(strPath + "IOConnect.exe");
                            SetUDCPActiveVersion("-1");
                            SetUDCPDispenserFamily("9000");
                            SetUDCPClienteUDCPVersion("1");
                        }
                        else
                        {
                            msgLog += "|existRegister";
                            retorno = true;
                        }
                        break;
                    }
                default:
                    {
                        msgLog += "|No UDCP";
                        retorno = true;
                        break;
                    }
            }
            Log.Logar(TipoLog.Processo, _par.PathLogProcessoDispensa, msgLog);
            return retorno;
        }

        public static void SetUDCPDriver(string valor_App)
        {
            try
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\udcp", true);
                    if (rk == null)
                    {
                        rk = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\udcp", true);
                    }
                    
                    rk.SetValue("Driver", valor_App, RegistryValueKind.String);
                }
                else
                {
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\udcp", true);
                    if (rk == null)
                    {
                        rk = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\udcp", true);
                    }
                    rk.SetValue("Driver", valor_App, RegistryValueKind.String);

                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Init).Name}: ", e);
			}
		}
        public static void SetUDCPCommandPath(string valor_Path)
        {
            try
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\udcp", true);
                    if (rk == null)
                    {
                        rk = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\udcp", true);
                    }
                    rk.SetValue("CommandPath", valor_Path, RegistryValueKind.String);
                }
                else
                {
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\udcp", true);
                    if (rk == null)
                    {
                        rk = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\udcp", true);
                    }
                    rk.SetValue("CommandPath", valor_Path, RegistryValueKind.String);

                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Init).Name}: ", e);
			}
		}
        public static void SetUDCPDispenserFamily(string valor_App)
        {
            try
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\udcp", true);
                    if (rk == null)
                    {
                        rk = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\udcp", true);
                    }
                    rk.SetValue("DispenserFamily", valor_App, RegistryValueKind.String);
                }
                else
                {
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\udcp", true);
                    if (rk == null)
                    {
                        rk = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\udcp", true);
                    }
                    rk.SetValue("DispenserFamily", valor_App, RegistryValueKind.String);

                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Init).Name}: ", e);
			}
		}
        public static void SetUDCPClienteUDCPVersion(string valor_App)
        {
            try
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\udcp", true);
                    if (rk == null)
                    {
                        rk = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\udcp", true);
                    }
                    rk.SetValue("ClientUDCPVersion", valor_App, RegistryValueKind.String);
                }
                else
                {
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\udcp", true);
                    if (rk == null)
                    {
                        rk = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\udcp", true);
                    }
                    rk.SetValue("ClientUDCPVersion", valor_App, RegistryValueKind.String);

                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Init).Name}: ", e);
			}
		}
        public static void SetUDCPActiveVersion(string valor_App)
        {
            try
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\udcp", true);
                    if (rk == null)
                    {
                        rk = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\udcp", true);
                    }
                    rk.SetValue("ActiveVersion", valor_App, RegistryValueKind.String);
                }
                else
                {
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\udcp", true);
                    if (rk == null)
                    {
                        rk = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\udcp", true);
                    }
                    rk.SetValue("ActiveVersion", valor_App, RegistryValueKind.String);

                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Init).Name}: ", e);
			}
		}
        private static string GetUDCP()
        {
            string retorno = string.Empty;
            try
            {   
                if (Environment.Is64BitOperatingSystem)
                {
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\udcp", true);
                    if(rk == null)
                    {
                        rk = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\udcp", true);
                    }
                    object obj = rk.GetValue("Driver");
                    if (obj != null)
                    {
                        retorno = obj.ToString();
                    }
                }
                else
                {
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\udcp", true);
                    if (rk == null)
                    {
                        rk = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\udcp", true);
                    }
                    object obj = rk.GetValue("Driver");
                    if (obj != null)
                    {
                        retorno = obj.ToString();
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Init).Name}: ", e);
			}

			return retorno;
        }
       
        /// <summary>
        /// Valida se licença de software foi informada e caso não
        /// exibe interface para entrada do usuário.
        /// </summary>
        /// <returns></returns>
        public static bool LicencaOk()
        {
            IOConnectRegistry icntRegistry = new IOConnectRegistry();
            string licenca = string.Empty;

            try
            {
                licenca = icntRegistry.GetLicense();
                if (!string.IsNullOrWhiteSpace(licenca))
                {
                    return true;
                }

                MACLicenseKey macLicenseKey = new MACLicenseKey();
                if (macLicenseKey.CheckLicense(licenca))
                {
                    return true;
                }

                using (fLicenca f = new fLicenca(macLicenseKey))
                {
                    f.ShowDialog();
                    licenca = f.Licença;
                }

                if (licenca == string.Empty)
                    return false;

                //Persiste licença no registro
                icntRegistry.SetLicenca(licenca);

                return true;
            }
            catch (Exception ex)
            {
                string log = LogManager.ExceptionToLog(ex);
                Log.Logar(
                    TipoLog.Erro, Util.ObjectParametros.PathDiretorioSistema, log);

                return false;
            }
            finally
            {
                if (icntRegistry != null)
                    icntRegistry.Dispose();
            }
        }

        /// <summary>
        /// Executa controle da validade da manutenção
        /// </summary>
        /// <returns></returns>
        public static bool ManutencaoOk()
        {
            Manutencao manutencao = new Manutencao();
            bool encerrarAplicacao = false;
            string mensagem = string.Empty;
            bool informarToken = false;

            switch (manutencao.GetStatus())
            {
                case StatusManutencao.Atencao:
                case StatusManutencao.Critico:
                    {
                        #region Atenção, Crítico

                        encerrarAplicacao = false;
                        mensagem =
                             $"{Negocio.IdiomaResxExtensao.Global_Informacao_ExpiracaoManutencao} {manutencao.GetLabelTempoRestante()}."
                             + Environment.NewLine
                             + Negocio.IdiomaResxExtensao.Global_Confirma_RedefinirValidade;

                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            informarToken = m.ShowDialog(
                                mensagem,
                                Negocio.IdiomaResxExtensao.Global_Sim,
                                Negocio.IdiomaResxExtensao.Global_Nao);
                        }

                        break;

                        #endregion
                    }
                case StatusManutencao.Vencido:
                    {
                        #region Vencido

                        encerrarAplicacao = true;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            informarToken = m.ShowDialog(
                                Negocio.IdiomaResxExtensao.Init_Confirmacao_ManutencaoExpirada,
                                Negocio.IdiomaResxExtensao.Global_Sim,
                                Negocio.IdiomaResxExtensao.Global_Nao);
                        }

                        break;

                        #endregion
                    }
                default:
                    break;
            }

            bool validadeRedefinida = false;
            if (informarToken)
                validadeRedefinida = TokenOk();

            if (!validadeRedefinida && encerrarAplicacao)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool TokenOk()
        {
            try
            {
                string serial = string.Empty;
                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    serial = percRegistry.GetSerialNumber();
                }

                AssemblyInfo info = new AssemblyInfo(Assembly.GetExecutingAssembly());
                string guid = info.Guid;

                bool tokenOk = false;
                using (fValidadeLicensa vToken = new fValidadeLicensa(serial, guid))
                {
                    tokenOk = (vToken.ShowDialog() == DialogResult.OK);
                }

                return tokenOk;
            }
            catch (Exception ex)
            {
                string log = LogManager.ExceptionToLog(ex);
                Log.Logar(TipoLog.Erro, Util.ObjectParametros.PathDiretorioSistema, log);
                return false;
            }
        }

        public static bool AtualizacaoBD()
        {
            bool retorno = true;
            #region Atualizacao Base de Dados
            Util.ObjectParametros parametros = Util.ObjectParametros.Load();

            DataSet dsTables = Util.ObjectParametros.getTables();

            DataSet dsTablesCol = Util.ObjectColorante.getTables();

            DataSet dsTablesRec = Util.ObjectRecircular.getTables();

            int versao_BD = 0;
            if (int.TryParse(parametros.VersaoIoconnect, out versao_BD))
            {
                if (versao_BD < versaoReleaseBD)
                {
                    try
                    {
                        if (versao_BD == 18)
                        {
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                string pathF = Path.Combine(Environment.CurrentDirectory, "*.dat");
                                StringBuilder sb = new StringBuilder();

                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD PathFilaDAT TEXT NULL; ");
                                        sb.Append("ALTER TABLE Parametros ADD DesabilitarMonitoramentoFilaDAT TEXT NULL; ");
                                        sb.Append("ALTER TABLE Parametros ADD DelayMonitoramentoFilaDAT TEXT NULL; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET ");
                                        sb.Append(" PathFilaDAT = '" + pathF + "', ");
                                        sb.Append(" DesabilitarMonitoramentoFilaDAT = 'True', ");
                                        sb.Append(" DelayMonitoramentoFilaDAT = '2', ");
                                        sb.Append(" VersaoIoconnect = '19'; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 19;

                                    }
                                }
                            }
                        }

                        if (versao_BD == 19)
                        {
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                string pathF = Path.Combine(Environment.CurrentDirectory, "*.dat");
                                StringBuilder sb = new StringBuilder();

                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD TipoBaseDados TEXT NULL; ");
                                        sb.Append("ALTER TABLE Parametros ADD PathBasesDados TEXT NULL; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET ");
                                        sb.Append(" TipoBaseDados = '0', ");
                                        sb.Append(" PathBasesDados = 'http://localhost/WebApiDosadora/api/', ");
                                        sb.Append(" VersaoIoconnect = '20'; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 20;

                                    }
                                }
                            }
                        }

                        if (versao_BD == 20)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (!File.Exists(Util.ObjectCalibracaoAutomatica.PathFile))
                            {
                                Util.ObjectCalibracaoAutomatica.CreateBD();

                                if (File.Exists(Util.ObjectCalibracaoAutomatica.PathFile))
                                {
                                    sb = new StringBuilder();
                                    List<Util.ObjectColorante> _colorantes = Util.ObjectColorante.List();
                                    bool att = true;
                                    foreach (Util.ObjectColorante _col in _colorantes)
                                    {
                                        Util.ObjectCalibragem _calib = Util.ObjectCalibragem.Load(_col.Circuito);
                                        if (_calib != null)
                                        {
                                            Util.ObjectCalibracaoAutomatica _calibracao = new Util.ObjectCalibracaoAutomatica();

                                            _calibracao.CapacideMaxBalanca = 420;
                                            _calibracao.MaxMassaAdmRecipiente = 100;
                                            _calibracao.NumeroMaxTentativaRec = 3;
                                            _calibracao.VolumeMaxRecipiente = 200;

                                            _calibracao._colorante = _col;
                                            _calibracao._calibragem = _calib;
                                            int motor = _calib.Motor;

                                            _calibracao.listOperacaoAutomatica = new List<Negocio.OperacaoAutomatica>();
                                            Negocio.OperacaoAutomatica _firstCal = new Negocio.OperacaoAutomatica();
                                            _firstCal.Motor = motor;
                                            _firstCal.IsCalibracaoAutomatica = 1;
                                            _firstCal.Volume = 20;
                                            _firstCal.IsPrimeiraCalibracao = 1;
                                            _firstCal.DesvioAdmissivel = 0.5;
                                            _firstCal.NumeroMaxTentativa = 3;
                                            _firstCal.IsRealizarMediaMedicao = 0;
                                            _firstCal.NumeroDosagemTomadaMedia = 0;
                                            _calibracao.listOperacaoAutomatica.Add(_firstCal);
                                            foreach (ValoresVO _vVO in _calibracao._calibragem.Valores)
                                            {
                                                Negocio.OperacaoAutomatica _opreacao = new Negocio.OperacaoAutomatica();
                                                _opreacao.Motor = motor;
                                                _opreacao.IsCalibracaoAutomatica = 0;
                                                _opreacao.Volume = _vVO.Volume;
                                                _opreacao.IsPrimeiraCalibracao = 0;
                                                _opreacao.DesvioAdmissivel = 1;
                                                _opreacao.NumeroMaxTentativa = 3;
                                                _opreacao.IsRealizarMediaMedicao = 0;
                                                _opreacao.NumeroDosagemTomadaMedia = 0;
                                                _calibracao.listOperacaoAutomatica.Add(_opreacao);
                                            }
                                            Util.ObjectCalibracaoAutomatica.Add(_calibracao, att);
                                            att = false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                sb = new StringBuilder();
                                List<Util.ObjectColorante> _colorantes = Util.ObjectColorante.List();
                                bool att = true;
                                foreach (Util.ObjectColorante _col in _colorantes)
                                {
                                    Util.ObjectCalibragem _calib = Util.ObjectCalibragem.Load(_col.Circuito);
                                    if (_calib != null)
                                    {
                                        Util.ObjectCalibracaoAutomatica _calibracao = new Util.ObjectCalibracaoAutomatica();

                                        _calibracao.CapacideMaxBalanca = 420;
                                        _calibracao.MaxMassaAdmRecipiente = 100;
                                        _calibracao.NumeroMaxTentativaRec = 3;
                                        _calibracao.VolumeMaxRecipiente = 200;

                                        _calibracao._colorante = _col;
                                        _calibracao._calibragem = _calib;
                                        int motor = _calib.Motor;

                                        _calibracao.listOperacaoAutomatica = new List<Negocio.OperacaoAutomatica>();
                                        Negocio.OperacaoAutomatica _firstCal = new Negocio.OperacaoAutomatica();
                                        _firstCal.Motor = motor;
                                        _firstCal.IsCalibracaoAutomatica = 1;
                                        _firstCal.Volume = 20;
                                        _firstCal.IsPrimeiraCalibracao = 1;
                                        _firstCal.DesvioAdmissivel = 0.5;
                                        _firstCal.NumeroMaxTentativa = 3;
                                        _firstCal.IsRealizarMediaMedicao = 0;
                                        _firstCal.NumeroDosagemTomadaMedia = 0;
                                        _calibracao.listOperacaoAutomatica.Add(_firstCal);
                                        foreach (ValoresVO _vVO in _calibracao._calibragem.Valores)
                                        {
                                            Negocio.OperacaoAutomatica _opreacao = new Negocio.OperacaoAutomatica();
                                            _opreacao.Motor = motor;
                                            _opreacao.IsCalibracaoAutomatica = 0;
                                            _opreacao.Volume = _vVO.Volume;
                                            _opreacao.IsPrimeiraCalibracao = 0;
                                            _opreacao.DesvioAdmissivel = 1;
                                            _opreacao.NumeroMaxTentativa = 3;
                                            _opreacao.IsRealizarMediaMedicao = 0;
                                            _opreacao.NumeroDosagemTomadaMedia = 0;
                                            _calibracao.listOperacaoAutomatica.Add(_opreacao);
                                        }

                                        Util.ObjectCalibracaoAutomatica.Add(_calibracao, att);
                                        att = false;
                                    }
                                }
                            }

                            #region SetVersao 21
                            sb = new StringBuilder();
                            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                            {
                                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                {
                                    conn.Open();
                                    sb.Append("UPDATE Parametros SET ");
                                    sb.Append(" VersaoIoconnect = '21'; ");
                                    cmd.CommandText = sb.ToString();
                                    cmd.ExecuteNonQuery();
                                    conn.Close();
                                    versao_BD = 21;
                                }
                            }
                            #endregion
                        }

                        if (versao_BD == 21)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectCalibragem.PathFile))
                            {
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectCalibragem.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Calibragem ADD MinimoFaixas TEXT NULL; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectCalibragem.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Calibragem SET MinimoFaixas = '7'; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }
                            }

                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET VersaoIoconnect = '22'; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 22;

                                    }
                                }
                            }
                        }

                        if (versao_BD == 22)
                        {
                            Util.ObjectCalibracaoHistorico.CreateBD();
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET VersaoIoconnect = '23'; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 23;

                                    }
                                }
                            }
                        }

                        if (versao_BD == 23)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectCalibracaoAutomatica.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectCalibracaoAutomatica.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Recipiente ADD MinMassaAdmRecipiente TEXT NULL; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectCalibracaoAutomatica.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Recipiente SET MinMassaAdmRecipiente = '50'; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }
                            }

                            if (File.Exists(Util.ObjectCalibracaoHistorico.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectCalibracaoHistorico.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Recipiente ADD MinMassaAdmRecipiente TEXT NULL; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectCalibracaoHistorico.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Recipiente SET MinMassaAdmRecipiente = '50'; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }
                            }

                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET VersaoIoconnect = '24'; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 24;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 24)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD HabilitarLogAutomateTesterProt TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET VersaoIoconnect = '25', HabilitarLogAutomateTesterProt = 'False'; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 25;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 25)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD DesabilitarVolumeMinimoDat TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD VolumeMinimoDat TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET VersaoIoconnect = '26', DesabilitarVolumeMinimoDat = 'True', VolumeMinimoDat = '0.1'; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 26;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 26)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD HabilitarRecirculacao TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();


                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET VersaoIoconnect = '27', HabilitarRecirculacao = 'False'; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 27;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 27)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD DelayMonitRecirculacao TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET VersaoIoconnect = '28', DelayMonitRecirculacao = '4'; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 28;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 28)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_06_UNT_Pref TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_06_UNT_1_IsPonto TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_06_UNT_2_IsPonto TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_06_CAN_Pref TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_06_CAN_1_IsPonto TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_06_FRM_Pref TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_06_FRM_SEP TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_06_FRM_1_IsPonto TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_06_BAS_Pref TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_06_BAS_1_IsPonto TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        string str = "UPDATE Parametros SET VersaoIoconnect = '29', Dat_06_UNT_Pref = '@UNT', Dat_06_UNT_1_IsPonto = '0', Dat_06_UNT_2_IsPonto = '0', ";
                                        str += "Dat_06_CAN_Pref = '@CAN', Dat_06_CAN_1_IsPonto = '0',";
                                        str += "Dat_06_FRM_Pref = '@FRM', Dat_06_FRM_SEP = ',', Dat_06_FRM_1_IsPonto = '0',";
                                        str += "Dat_06_BAS_Pref = '@BAS', Dat_06_BAS_1_IsPonto = '0';";
                                        sb.Append(str);
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 29;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 29)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_06_BAS_Habilitado TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        string str = "UPDATE Parametros SET VersaoIoconnect = '30', Dat_06_BAS_Habilitado = '1';";
                                        sb.Append(str);
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 30;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 30)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Address_PlacaMov TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        string str = "UPDATE Parametros SET VersaoIoconnect = '31', Address_PlacaMov = '3';";
                                        sb.Append(str);
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 31;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 31)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_05_UNT_Pref TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_05_UNT_1_IsPonto TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_05_UNT_2_IsPonto TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_05_CAN_Pref TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_05_CAN_1_IsPonto TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_05_FRM_Pref TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_05_FRM_SEP TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_05_FRM_1_IsPonto TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_05_BAS_Pref TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_05_BAS_1_IsPonto TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD Dat_05_BAS_Habilitado TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        string str = "UPDATE Parametros SET VersaoIoconnect = '32', ";
                                        str += "Dat_05_UNT_Pref = '@UNT', Dat_05_UNT_1_IsPonto = '0', Dat_05_UNT_2_IsPonto = '0', ";
                                        str += "Dat_05_CAN_Pref = '@CAN', Dat_05_CAN_1_IsPonto = '0',";
                                        str += "Dat_05_FRM_Pref = '@CNT', Dat_05_FRM_SEP = '" + '"' + "', Dat_05_FRM_1_IsPonto = '0',"; ;

                                        str += "Dat_05_BAS_Pref = '@BAS', Dat_05_BAS_1_IsPonto = '0', ";
                                        str += "Dat_05_BAS_Habilitado = '1';";
                                        sb.Append(str);
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 32;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 32)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD NomeDispositivo_PlacaMov TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        string str = "UPDATE Parametros SET VersaoIoconnect = '33', NomeDispositivo_PlacaMov = '';";
                                        sb.Append(str);
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 33;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 33)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectMotorPlacaMovimentacao.PathFile))
                            {
                                Util.ObjectMotorPlacaMovimentacao plMov = new Util.ObjectMotorPlacaMovimentacao();
                                plMov.Circuito = 3;
                                plMov.NameTag = "Bico";
                                plMov.TipoMotor = 0;
                                plMov.Pulsos = 100;
                                plMov.Aceleracao = 100;
                                plMov.Delay = 100;
                                plMov.Habilitado = true;
                                plMov.Velocidade = 100;
                                Util.ObjectMotorPlacaMovimentacao.Persist(plMov);
                            }

                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        string str = "UPDATE Parametros SET VersaoIoconnect = '34';";
                                        sb.Append(str);
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 34;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 34)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD HabilitarRecirculacaoAuto TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD DelayMonitRecirculacaoAuto TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET VersaoIoconnect = '35', HabilitarRecirculacaoAuto = 'False',  DelayMonitRecirculacaoAuto = '60'; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 35;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 35)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD DelayNotificacaotRecirculacaoAuto TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET VersaoIoconnect = '36',  DelayNotificacaotRecirculacaoAuto = '5'; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 36;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 36)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD QtdNotificacaotRecirculacaoAuto TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET VersaoIoconnect = '37',  QtdNotificacaotRecirculacaoAuto = '3'; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 37;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 37)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD DelayAlertaPlacaMov TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();


                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET VersaoIoconnect = '38', DelayAlertaPlacaMov = '5'; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 38;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 38)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD LogAutomateBackup TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET VersaoIoconnect = '39', LogAutomateBackup = 'False'; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 39;
                                    }
                                }
                            }

                            versao_BD = 39;
                        }

                        if (versao_BD == 39)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD QtdTentativasConexao TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET VersaoIoconnect = '40', QtdTentativasConexao = '1'; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 40;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 40)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectColorante.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Corantes ADD IsBase TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectColorante.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Corantes SET IsBase = 'False';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET VersaoIoconnect = '41'; ");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 41;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 41)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD HabilitarIdentificacaoCopo TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET HabilitarIdentificacaoCopo = 'False', VersaoIoconnect = '42';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 42;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 42)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD DelayEsponja TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET DelayEsponja = '5', VersaoIoconnect = '43';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 43;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 43)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD TreinamentoCal TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET TreinamentoCal = 'True', VersaoIoconnect = '44';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 44;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 44)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectColorante.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Corantes ADD Seguidor TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectColorante.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Corantes SET Seguidor = '-1';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET VersaoIoconnect = '45';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 45;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 45)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectColorante.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Corantes ADD Step TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectColorante.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Corantes SET Step = '0';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET VersaoIoconnect = '46';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 46;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 46)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD DelayUDCP TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD CreateFileTmpUDCP TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD ExtFileTmpUDCP TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET ExtFileTmpUDCP = 'tmp', CreateFileTmpUDCP = '0', DelayUDCP = '0', VersaoIoconnect = '47';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 47;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 47)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD DelayLimpBicos TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD HabLimpBicos TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET HabLimpBicos = 'False', DelayLimpBicos = '6', VersaoIoconnect = '48';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 48;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 48)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD DesabilitaMonitSyncToken TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD IpSincToken TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD PortaSincToken TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET DesabilitaMonitSyncToken = 'True', IpSincToken = '192.168.125.116', PortaSincToken = '3112', VersaoIoconnect = '49';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 49;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 49)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD TipoEventos TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET TipoEventos = 'HD', VersaoIoconnect = '50';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 50;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 50)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD DesabilitaMonitSyncBkpCalibragem TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD UrlSincBkpCalibragem TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET DesabilitaMonitSyncBkpCalibragem = 'True', UrlSincBkpCalibragem = 'ftp://192.168.125.116/BdCalicracao', VersaoIoconnect = '51';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 51;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 51)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD LogBD TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET LogBD = 'False', VersaoIoconnect = '52';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 52;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 52)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD NameRemoteAccess TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET NameRemoteAccess = 'BASupSrvcCnfg.exe', VersaoIoconnect = '53';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 53;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 53)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD ProcRemoveLataUDCP TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET ProcRemoveLataUDCP = 'False', VersaoIoconnect = '54';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 54;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 54)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD TipoLimpBicos TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET TipoLimpBicos = '1', VersaoIoconnect = '55';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 55;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 55)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectUser.PathFile))
                            {
                                List<Util.ObjectUser> lUser = Util.ObjectUser.List();
                                bool existe_manager = false;
                                for (int i = 0; i < lUser.Count; i++)
                                {
                                    if (lUser[i].Nome == "master" && lUser[i].Tipo == 1)
                                    {
                                        lUser[i].Senha = "1500k";
                                        Util.ObjectUser.Persist(lUser[i]);
                                    }
                                    else if (lUser[i].Nome == "manager")
                                    {
                                        existe_manager = true;
                                    }
                                }

                                if (!existe_manager)
                                {
                                    Util.ObjectUser userMan = new Util.ObjectUser();

                                    userMan.Nome = "manager";
                                    userMan.Senha = "avsb";
                                    userMan.Tecnico = false;
                                    userMan.Tipo = 3;

                                    Util.ObjectUser.Persist(userMan);
                                }
                            }

                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET VersaoIoconnect = '56';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 56;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 56)
                        {
                            StringBuilder sb = new StringBuilder();

                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD DisablePopUpDispDat TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET DisablePopUpDispDat = 'True', VersaoIoconnect = '57';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 57;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 57)
                        {
                            StringBuilder sb = new StringBuilder();

                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD TimeoutPingTcp TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET TimeoutPingTcp = '5000', VersaoIoconnect = '58';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 58;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 58)
                        {
                            StringBuilder sb = new StringBuilder();

                            if (File.Exists(Util.ObjectParametros.PathFile) && File.Exists(Util.ObjectColorante.PathFile))
                            {
                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectColorante.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Corantes ADD VolumePurga TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectColorante.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Corantes SET VolumePurga = '" + parametros.VolumePurga.ToString().Replace(",", ".") + "';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("ALTER TABLE Parametros ADD ViewMessageProc TEXT NULL;");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET ViewMessageProc = 'True',  VersaoIoconnect = '59';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 59;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 59)
                        {
                            StringBuilder sb = new StringBuilder();

                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                bool existeColum = false;
                                if (dsTables != null)
                                {
                                    foreach (DataTable dt in dsTables.Tables)
                                    {
                                        foreach (DataColumn dc in dt.Columns)
                                        {
                                            if (dc.ColumnName == "TipoDosagemExec")
                                            {
                                                existeColum = true;
                                            }
                                        }
                                    }
                                }

                                if (!existeColum)
                                {
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("ALTER TABLE Parametros ADD TipoDosagemExec TEXT NULL;");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                        }
                                    }

                                    sb = new StringBuilder();
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("UPDATE Parametros SET TipoDosagemExec = '1',  VersaoIoconnect = '60';");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                            versao_BD = 60;
                                        }
                                    }
                                }
                                else
                                {
                                    sb = new StringBuilder();
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("UPDATE Parametros SET VersaoIoconnect = '60';");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                            versao_BD = 60;
                                        }
                                    }
                                }
                            }
                        }

                        if (versao_BD == 60)
                        {
                            StringBuilder sb = new StringBuilder();

                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                bool existeColum = false;
                                if (dsTables != null)
                                {
                                    foreach (DataTable dt in dsTables.Tables)
                                    {
                                        foreach (DataColumn dc in dt.Columns)
                                        {
                                            if (dc.ColumnName == "ValorFraction")
                                            {
                                                existeColum = true;
                                            }
                                        }
                                    }
                                }
                                if (!existeColum)
                                {
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("ALTER TABLE Parametros ADD ValorFraction TEXT NULL;");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                        }
                                    }

                                    sb = new StringBuilder();
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("UPDATE Parametros SET ValorFraction = '800',  VersaoIoconnect = '61';");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                            versao_BD = 61;
                                        }
                                    }
                                }
                                else
                                {
                                    sb = new StringBuilder();
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("UPDATE Parametros SET VersaoIoconnect = '61';");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                            versao_BD = 61;
                                        }
                                    }
                                }
                            }
                        }

                        if (versao_BD == 61)
                        {
                            StringBuilder sb = new StringBuilder();

                            if (File.Exists(Util.ObjectColorante.PathFile))
                            {
                                bool existeColum = false;
                                if (dsTablesCol != null)
                                {
                                    foreach (DataTable dt in dsTablesCol.Tables)
                                    {
                                        foreach (DataColumn dc in dt.Columns)
                                        {
                                            if (dc.ColumnName == "ColorRGB")
                                            {
                                                existeColum = true;
                                            }
                                        }
                                    }
                                }

                                if (!existeColum)
                                {
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectColorante.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("ALTER TABLE Corantes ADD ColorRGB TEXT NULL;");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                        }
                                    }

                                    sb = new StringBuilder();
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectColorante.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("UPDATE Corantes SET  ColorRGB = '255;255;255;';");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                            versao_BD = 62;
                                        }
                                    }

                                    sb = new StringBuilder();
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("UPDATE Parametros SET  VersaoIoconnect = '62';");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                            versao_BD = 62;
                                        }
                                    }
                                }
                                else
                                {
                                    sb = new StringBuilder();
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("UPDATE Parametros SET VersaoIoconnect = '62';");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                            versao_BD = 62;
                                        }
                                    }
                                }
                            }
                        }

                        if (versao_BD == 62)
                        {
                            if (File.Exists(Util.ObjectRecircular.PathFile))
                            {
                                bool existeColum = false;
                                bool existeColumAuto = false;
                                if (dsTablesRec != null)
                                {
                                    foreach (DataTable dt in dsTablesRec.Tables)
                                    {
                                        foreach (DataColumn dc in dt.Columns)
                                        {
                                            if (dc.ColumnName == "isValve")
                                            {
                                                existeColum = true;
                                            }
                                            if (dc.ColumnName == "isAuto")
                                            {
                                                existeColumAuto = true;
                                            }
                                        }
                                    }
                                }

                                StringBuilder sb = new StringBuilder();
                                if (!existeColum)
                                {
                                    sb = new StringBuilder();
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectRecircular.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("ALTER TABLE Recircular ADD isValve TEXT NULL;");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                        }
                                    }

                                    sb = new StringBuilder();
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectRecircular.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("UPDATE Recircular SET isValve = 'False';");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();

                                        }
                                    }
                                }

                                if (!existeColumAuto)
                                {
                                    sb = new StringBuilder();
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectRecircular.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("ALTER TABLE Recircular ADD isAuto TEXT NULL;");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                        }
                                    }

                                    sb = new StringBuilder();
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectRecircular.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("UPDATE Recircular SET isAuto = 'False';");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();

                                        }
                                    }
                                }

                                sb = new StringBuilder();
                                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                {
                                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                    {
                                        conn.Open();
                                        sb.Append("UPDATE Parametros SET VersaoIoconnect = '63';");
                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        versao_BD = 63;
                                    }
                                }
                            }
                        }

                        if (versao_BD == 63)
                        {
                            StringBuilder sb = new StringBuilder();

                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                bool existeColum = false;
                                if (dsTables != null)
                                {
                                    foreach (DataTable dt in dsTables.Tables)
                                    {
                                        foreach (DataColumn dc in dt.Columns)
                                        {
                                            if (dc.ColumnName == "TempoReciAuto")
                                            {
                                                existeColum = true;
                                            }
                                        }
                                    }
                                }

                                if (!existeColum)
                                {
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("ALTER TABLE Parametros ADD TempoReciAuto TEXT NULL;");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                        }
                                    }

                                    sb = new StringBuilder();
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("UPDATE Parametros SET TempoReciAuto = '1',  VersaoIoconnect = '64';");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                            versao_BD = 64;
                                        }
                                    }
                                }
                                else
                                {
                                    sb = new StringBuilder();
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("UPDATE Parametros SET VersaoIoconnect = '64';");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                            versao_BD = 64;
                                        }
                                    }
                                }
                            }
                        }

                        if (versao_BD == 64)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectColorante.PathFile))
                            {
                                bool existeColum = false;
                                if (dsTablesCol != null)
                                {
                                    foreach (DataTable dt in dsTablesCol.Tables)
                                    {
                                        foreach (DataColumn dc in dt.Columns)
                                        {
                                            if (dc.ColumnName == "IsBicoIndividual")
                                            {
                                                existeColum = true;
                                            }
                                        }
                                    }
                                }

                                if (!existeColum)
                                {
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectColorante.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("ALTER TABLE Corantes ADD IsBicoIndividual TEXT NULL;");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                        }
                                    }

                                    sb = new StringBuilder();
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectColorante.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("ALTER TABLE Corantes ADD VolumeBicoIndividual TEXT NULL;");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                        }
                                    }

                                    sb = new StringBuilder();
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectColorante.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("UPDATE Corantes SET VolumeBicoIndividual = '5', IsBicoIndividual = 'False';");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                        }
                                    }

                                    sb = new StringBuilder();
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("UPDATE Parametros SET  VersaoIoconnect = '65';");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                            versao_BD = 65;
                                        }
                                    }
                                }
                            }
                        }

                        if (versao_BD == 65)
                        {
                            StringBuilder sb = new StringBuilder();
                            if (File.Exists(Util.ObjectParametros.PathFile))
                            {
                                bool existeColum = false;
                                if (dsTablesCol != null)
                                {
                                    foreach (DataTable dt in dsTables.Tables)
                                    {
                                        foreach (DataColumn dc in dt.Columns)
                                        {
                                            if (dc.ColumnName == "LogStatusMaquina")
                                            {
                                                existeColum = true;
                                            }
                                        }
                                    }
                                }

                                if (!existeColum)
                                {
                                    sb = new StringBuilder();
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("ALTER TABLE Parametros ADD LogStatusMaquina TEXT NULL;");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                        }
                                    }

                                    sb = new StringBuilder();
                                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false))
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                        {
                                            conn.Open();
                                            sb.Append("UPDATE Parametros SET LogStatusMaquina = 'False',  VersaoIoconnect = '66';");
                                            cmd.CommandText = sb.ToString();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                            versao_BD = 66;
                                        }
                                    }
                                }
                            }
                        }
                    }
					catch (Exception e)
					{
						LogManager.LogError($"Erro no módulo {typeof(Init).Name}: ", e);
                        retorno = false;
					}
				}
			}

            #endregion

            return retorno;
        }

        public static bool CreateFileVersion()
        {
            bool retorno = false;
            try
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                string texto = $"{version.Major}.{version.Minor}.{version.Build}";
                version = null;
                string PathFile = Path.Combine(Environment.CurrentDirectory, "IoconnectVersion.txt");
                using (StreamWriter outputFile = new StreamWriter(PathFile))
                {
                    outputFile.Write(texto);
                    outputFile.Close();
                }
                retorno = true;
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Init).Name}: ", e);
			}
			return retorno;
        }
    }
}