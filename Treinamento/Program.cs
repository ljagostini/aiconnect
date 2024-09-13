using System.Globalization;

namespace Percolore.Treinamento
{
	static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /* Recupera array com argumentos */
            string[] arg = Environment.GetCommandLineArgs();

            string idioma = "1";
            if (arg.GetLength(0) > 1)
            {
                idioma = arg[1];
            }

            switch (idioma)
            {
                case "2":
                    {
                        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-ES");
                        break;
                    }
                case "3":
                    {
                        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
                        break;
                    }
                default:
                    {
                        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");
                        break;
                    }
            }
            int _ididioma = 1;
            Int32.TryParse(idioma, out _ididioma);

            Negocio.IdiomaResx.GetIDiomaREsx(_ididioma);

			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();
            Application.Run(new frmRecipiente());
        }
    }
}