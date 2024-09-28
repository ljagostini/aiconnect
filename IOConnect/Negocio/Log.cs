using Percolore.Core.Logging;
using Percolore.Core.Persistence.WindowsRegistry;
using Percolore.Core.Util;

namespace Percolore.IOConnect
{
	public enum TipoLog
    {
        Processo = 1,
        ControleDispensa = 2,
        Erro = 3,
        Comunicacao = 4
    }

    public static class Log
    {
        public static void Logar(TipoLog tipo, string path, string[] mensagens)
        {            
            if (File.Exists(path))
            {
                /* Retira permissão somente leitura do arquivo.
                 * Isso deve após certificar-se que arquivo existe.
                 * Referente à tarefa 190. */
                FileHelper.SetNotReadOnly(path);
            }
            Util.ObjectParametros p = Util.ObjectParametros.Load();

            switch (tipo)
            {
                case TipoLog.Processo:
                    {
                        if (!p.LogBD)
                        {
                            LogarProcesso(path, mensagens);
                        }
                        else
                        {
                            Util.ObjectLogBD logBD = new Util.ObjectLogBD(path, "AD-D8.db");
                            logBD.DATAHORA = DateTime.Now;
                            foreach (string detalhes in mensagens)
                            {
                                logBD.DETALHES = detalhes;
                                Util.ObjectLogBD.InsertLog(logBD);
                            }
                        }
                        break;
                    }
                case TipoLog.ControleDispensa:
                    {
                        if (!p.LogBD)
                        {
                            LogarControleDispensa(path, mensagens);
                        }
                        else
                        {
                            Util.ObjectLogBD logBD = new Util.ObjectLogBD(path, "ctrldsp.db");
                            logBD.DATAHORA = DateTime.Now;
                            foreach (string detalhes in mensagens)
                            {
                                logBD.DETALHES = detalhes;
                                Util.ObjectLogBD.InsertLog(logBD);
                            }
                        }
                        break;
                    }
                case TipoLog.Erro:
                    {
                        if (!p.LogBD)
                        {
                            path = Path.Combine(path, "erros.log");
                            LogarErro(path, mensagens);
                        }
                        else
                        {
                            Util.ObjectLogBD logBD = new Util.ObjectLogBD(path, "erros.db");
                            logBD.DATAHORA = DateTime.Now;
                            foreach (string detalhes in mensagens)
                            {
                                logBD.DETALHES = detalhes;
                                Util.ObjectLogBD.InsertLog(logBD);
                            }
                        }
                        break;
                    }
                case TipoLog.Comunicacao:
                    {
                        if (!p.LogBD)
                        {
                            path = Path.Combine(path, "comunicacao.log");
                            LogarComunicacao(path, mensagens);
                        }
                        else
                        {
                            Util.ObjectLogBD logBD = new Util.ObjectLogBD(path, "comunicacao.db");
                            logBD.DATAHORA = DateTime.Now;
                            foreach (string detalhes in mensagens)
                            {
                                logBD.DETALHES = detalhes;
                                Util.ObjectLogBD.InsertLog(logBD);
                            }
                        }
                        break;
                    }
            }
        }

        public static void Logar(TipoLog tipo, string path, string mensagem)
        {
            Logar(tipo, path, new string[] { mensagem });
        }

        /* Cada método corresponde a um tipo de log */
        #region Métodos privados 

        /// <summary>
        /// Registra etapas do processo de dispensa.
        /// Este log não pode ter seu fomrmato alterado pois foi definido para o cliente.
        /// </summary>
        private static void LogarProcesso(string path, string[] mensagens)
        {
            try
            {
                bool existe = File.Exists(path);
                if (existe)
                {
                    Util.ObjectParametros _par = Util.ObjectParametros.Load();
                    if (_par.LogAutomateBackup)
                    {
                        FileInfo info = new FileInfo(path);
                        double length = (info.Length / 1048576.0);
                        if (length > 5.0)
                        {
                            string[] arrayPath = path.Split(Path.DirectorySeparatorChar);
                            if (arrayPath.Length > 0)
                            {
                                string nameFile = string.Format("{0:dd-MM-yyyy}", DateTime.Now) + "_" + arrayPath[arrayPath.Length - 1];
                                string nPathFile = "";
                                for (int i = 0; i < arrayPath.Length - 1; i++)
                                {
                                    if (arrayPath[i].Length > 0)
                                    {
                                        nPathFile += arrayPath[i] + Path.DirectorySeparatorChar;
                                    }
                                }
                                if (nPathFile.Length > 0)
                                {
                                    nPathFile += nameFile;
                                }
                                File.Move(path, nPathFile);
                                existe = false;
                            }
                        }
                    }
                }
                string log = string.Empty;

                foreach (string msg in mensagens)
                {
                    if (existe)
                        log += Environment.NewLine;

                    log += string.Concat(DateTime.Now.ToString(), " | ", msg);
                    Modbus.Constantes.listLog_TXT.Add(string.Concat(DateTime.Now.ToString(), " | ", msg));

                }
                while(Modbus.Constantes.listLog_TXT.Count > Modbus.Constantes.qtdLog_TXT)
                {
                    Modbus.Constantes.listLog_TXT.RemoveAt(0);
                }

                File.AppendAllText(path, log);
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Log).Name}: ", e);
			}
		}

        private static void LogarControleDispensa(string path, string[] mensagens)
        {
            try
            {    
                bool existe = File.Exists(path);
                if(existe)
                {
                    Util.ObjectParametros _par = Util.ObjectParametros.Load();
                    if (_par.LogAutomateBackup)
                    {
                        FileInfo info = new FileInfo(path);
                        double length = (info.Length / 1048576.0);
                        if (length > 5.0)
                        {
                            string[] arrayPath = path.Split(Path.DirectorySeparatorChar);
                            if (arrayPath.Length > 0)
                            {
                                string nameFile = string.Format("{0:dd-MM-yyyy}", DateTime.Now) + "_" + arrayPath[arrayPath.Length - 1];
                                string nPathFile = "";
                                for (int i = 0; i < arrayPath.Length - 1; i++)
                                {
                                    if (arrayPath[i].Length > 0)
                                    {
                                        nPathFile += arrayPath[i] + Path.DirectorySeparatorChar;
                                    }
                                }
                                if (nPathFile.Length > 0)
                                {
                                    nPathFile += nameFile;
                                }
                                File.Move(path, nPathFile);
                                existe = false;
                            }
                        }
                    }
                }
                string log = string.Empty;

                string serial = string.Empty;
                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    serial = percRegistry.GetSerialNumber();
                }

                foreach (string mensagem in mensagens)
                {
                    if (existe)
                        log += Environment.NewLine;

                    long timestamp =
                        DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    log += $"{serial},{timestamp},{mensagem}";
                }

                File.AppendAllText(path, log);
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Log).Name}: ", e);
			}
		}

        private static void LogarErro(string path, string[] mensagens)
        {
            try
            {
                bool fileExist = File.Exists(path);
                if (fileExist)
                {
                    Util.ObjectParametros _par = Util.ObjectParametros.Load();
                    if (_par.LogAutomateBackup)
                    {
                        FileInfo info = new FileInfo(path);
                        double length = (info.Length / 1048576.0);
                        if (length > 5.0)
                        {
                            string[] arrayPath = path.Split(Path.DirectorySeparatorChar);
                            if (arrayPath.Length > 0)
                            {
                                string nameFile = string.Format("{0:dd-MM-yyyy}", DateTime.Now) + "_" + arrayPath[arrayPath.Length - 1];
                                string nPathFile = "";
                                for (int i = 0; i < arrayPath.Length - 1; i++)
                                {
                                    if (arrayPath[i].Length > 0)
                                    {
                                        nPathFile += arrayPath[i] + Path.DirectorySeparatorChar;
                                    }
                                }
                                if (nPathFile.Length > 0)
                                {
                                    nPathFile += nameFile;
                                }
                                File.Move(path, nPathFile);
                                fileExist = false;
                            }
                        }
                    }
                }
                string log = string.Empty;

                foreach (string mensagem in mensagens)
                {
                    if (fileExist)
                        log += Environment.NewLine;

                    log += $"{DateTimeOffset.Now} | {mensagem}";
                }

                File.AppendAllText(path, log);
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Log).Name}: ", e);
			}
		}

        private static void LogarComunicacao(string path, string[] mensagens)
        {
            try
            {
                bool fileExist = File.Exists(path);
                if (fileExist)
                {
                    Util.ObjectParametros _par = Util.ObjectParametros.Load();
                    if (_par.LogAutomateBackup)
                    {
                        FileInfo info = new FileInfo(path);
                        double length = (info.Length / 1048576.0);
                        if (length > 5.0)
                        {
                            string[] arrayPath = path.Split(Path.DirectorySeparatorChar);
                            if (arrayPath.Length > 0)
                            {
                                string nameFile = string.Format("{0:dd-MM-yyyy}", DateTime.Now) + "_" + arrayPath[arrayPath.Length - 1];
                                string nPathFile = "";
                                for (int i = 0; i < arrayPath.Length - 1; i++)
                                {
                                    if (arrayPath[i].Length > 0)
                                    {
                                        nPathFile += arrayPath[i] + Path.DirectorySeparatorChar;
                                    }
                                }
                                if (nPathFile.Length > 0)
                                {
                                    nPathFile += nameFile;
                                }
                                File.Move(path, nPathFile);
                                fileExist = false;
                            }
                        }
                    }
                }
                string log = string.Empty;

                foreach (string mensagem in mensagens)
                {
                    log += $"{DateTimeOffset.Now} | {mensagem}";
                    log += Environment.NewLine;
                }

                File.AppendAllText(path, log);
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(Log).Name}: ", e);
			}
		}

        #endregion
    }
}