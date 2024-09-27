using System.Diagnostics;

namespace Percolore.IOConnect.Core
{
	public static class RunIOConnectHelper
    {
        const string PROCESS_NAME = "RunIOConnect";
        public static string PATH_DIRECTORY = @"C:\Percolore\IOConnect\RunIOConnect";
        /// <summary>
        /// Recupera processo referente ao IOConnect
        /// </summary>
        static Process GetProcess()
        {
            return
                Process.GetProcesses().FirstOrDefault(p => p.ProcessName == PROCESS_NAME);
        }

        public static bool isRunIOConnect()
        {
            return GetProcess() != null;
        }

        /// <summary>
        /// Executa IOConnect
        /// </summary>
        public static bool RunIOConnect()
        {
            string path = Path.Combine(PATH_DIRECTORY, PROCESS_NAME);

            try
            {
                if (GetProcess() != null)
                {
                    return true;
                }
                //Se não estiver em execução, inicia o processo
                else
                {
                    Process process = new Process()
                    {
                        StartInfo = new ProcessStartInfo(path, "{Arguments If Needed}")
                        {
                            WindowStyle = ProcessWindowStyle.Normal,
                            WorkingDirectory = Path.GetDirectoryName(path)
                        }
                    };

                    process.Start();
                }

                //Verifica se processo foi iniciado com sucesso
                return (GetProcess() != null);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Encerra execução do IOConnect
        /// </summary>
        public static bool ShutdownRunIOConnect()
        {
            bool retorno = false;
            Process ioconnect = GetProcess();

            if (ioconnect != null)
            {
                retorno = true;
                ioconnect.Kill();
                ioconnect.CloseMainWindow();
            }

            return retorno;
        }
    }
}