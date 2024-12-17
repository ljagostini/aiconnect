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

            // Migrar XML para SQLite
            MigrateXmlToSqlite.Main();

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
                    log_nivel_colorante += $"{c.Circuito},{Math.Round(c.Volume, 3)},";

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
                for (int i = 33; i <= l_Col_exc.Count; i++)
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