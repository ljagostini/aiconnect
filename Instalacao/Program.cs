using Percolore.Core.Persistence.Xml;

namespace Percolore.Instalacao
{
	static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();

			//Adiciona delay para garantir que todos os arquivos foram copiados pelo instalador
			Thread.Sleep(2000);

            string[] arg = Environment.GetCommandLineArgs();
            string defaultDirName = Path.GetDirectoryName(arg[0]);

            try
            {
                Instalador instalador = new Instalador(defaultDirName);
                
                try
                {
                    instalador.AccessToXml();
                    instalador.AtualizarRegistro();
                    instalador.DeletarArquivos();
                }
                catch (Exception ex)
                {
                    string exception =
                   "Source: "
                   + Environment.NewLine + ex.Source
                   + Environment.NewLine + Environment.NewLine
                   + "Message: "
                   + Environment.NewLine + ex.Message
                   + Environment.NewLine + Environment.NewLine
                   + "StackTrace:"
                   + Environment.NewLine + ex.StackTrace
                   + Environment.NewLine + Environment.NewLine
                   + "InnerException: "
                   + Environment.NewLine + ex.InnerException;

                    File.AppendAllText(
                        Path.Combine(Environment.CurrentDirectory, "log.txt"), exception);
                }
                Parametros paramtros = Parametros.Load();
                //if(!instalador.createXmlFromAccess)
                int versaoIO = Convert.ToInt32(paramtros.VersaoIoconnect);
                if(versaoIO <= 18)
                {
                    instalador.UpdatePar();
                    instalador.UpgradeXML();

                    paramtros.VersaoIoconnect = "19";

                    Parametros.Persist(paramtros);
                }
            }
            catch (Exception ex)
            {
                string exception =
                    "Source: "
                    + Environment.NewLine + ex.Source
                    + Environment.NewLine + Environment.NewLine
                    + "Message: "
                    + Environment.NewLine + ex.Message
                    + Environment.NewLine + Environment.NewLine
                    + "StackTrace:"
                    + Environment.NewLine + ex.StackTrace
                    + Environment.NewLine + Environment.NewLine
                    + "InnerException: "
                    + Environment.NewLine + ex.InnerException;

                File.AppendAllText(
                    Path.Combine(Environment.CurrentDirectory, "log.txt"), exception);

                throw;
            }
        }
    }
}