using Percolore.Core;
using System.Diagnostics;

namespace Percolore.IOConnect
{
	public class TreinamentoHelper
    {
        public static bool RunTreinamento()
        {
            string processname = "Treinamento";
            string path = Path.Combine(Environment.CurrentDirectory, processname);

            try
            {
                /*Recupera processo*/
                Process treinamento =
                    Process.GetProcesses().FirstOrDefault(p => p.ProcessName == processname);

                if (treinamento != null)
                {
                    return true;
                }

                Util.ObjectParametros parametros = Util.ObjectParametros.Load();
                string prmIdioma = string.Empty;

                switch ((Idioma)parametros.IdIdioma)
                {
                    case Idioma.Português:
                        {
                            prmIdioma = "1";
                            break;
                        }
                    case Idioma.Español:
                        {
                            prmIdioma = "2";
                            break;
                        }
                    case Idioma.English:
                        {
                            prmIdioma = "3";
                            break;
                        }
                    default:
                        {
                            prmIdioma = "1";
                            break;
                        }
                }

                /*Se processo não for encontrado, inicia o mesmo*/
                Process.Start(path, prmIdioma);

                /*Verifica se processo foi iniciado com sucesso*/
                treinamento =
                    Process.GetProcesses().FirstOrDefault(p => p.ProcessName == processname);

                if (treinamento == null)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}