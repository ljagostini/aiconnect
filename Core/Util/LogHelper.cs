namespace Percolore.Core.Util
{
	public class LogHelper
    {
        /// <summary>
        /// Recebe uma exception, extrai informações e retorna mensagem formatada
        /// </summary>
        public static string ExceptionToLog(Exception exception)
        {
            string log =
                "message:"
                + Environment.NewLine + exception.Message
                + Environment.NewLine + "stacktrace:"
                + Environment.NewLine + exception.StackTrace;

            return log;
        }
    }
}