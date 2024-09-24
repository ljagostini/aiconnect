using Microsoft.Extensions.Configuration;
using Serilog;

namespace Percolore.Core.Logging
{
	/// <summary>
	/// Classe degerenciamento de logs.
	/// </summary>
	public static class LogManager
	{
		private static ILogger _logger;

		/// <summary>
		/// Inicializa o logger com uma configuração default, escrevendo apenas em console.
 		/// </summary>
		public static void InitializeLogger()
		{
			_logger = new LoggerConfiguration()
				.WriteTo.Console()
				.CreateLogger();
		}

		/// <summary>
		/// Inicializa o logger com uma configuração customizada.
		/// </summary>
		/// <param name="configuration">Parâmetros de configuração da interface <see cref="IConfiguration"/>.</param>
		public static void InitializeLogger(IConfiguration configuration)
		{
			_logger = new LoggerConfiguration()
				.ReadFrom.Configuration(configuration)
				.CreateLogger();
		}

		/// <summary>
		/// Acessa o objeto para escrita de logs.
		/// </summary>
		/// <returns>Uma instância de objeto <see cref="ILogger"/>.</returns>
		/// <exception cref="InvalidOperationException">Instância do objeto <see cref="ILogger"/> não inicializada.</exception>
		public static ILogger GetLogger()
		{
			return _logger ?? throw new InvalidOperationException("Logger is not initialized. Call InitializeLogger method first.");
		}

		/// <summary>
		/// Escreve uma mensagem informativa no log.
		/// </summary>
		/// <param name="message">A mensagem a ser escrita no log.</param>
		public static void LogInformation(string message)
		{
			GetLogger().Information(message);
		}

		/// <summary>
		/// Escreve uma mensagem de erro no log.
		/// </summary>
		/// <param name="message">A mensagem a ser escrita no log.</param>
		/// <param name="ex">Objeto <see cref="Exception"/> contendo os detalhes do erro ocorrido.</param>
		public static void LogError(string message, Exception ex)
		{
			GetLogger().Error(ex, message);
		}

		/// <summary>
		/// Escreve uma mensagem de aviso no log.
		/// </summary>
		/// <param name="message"> mensagem a ser escrita no log.</param>
		public static void LogWarning(string message)
		{
			GetLogger().Warning(message);
		}

		/// <summary>
		/// Escreve uma mensagem de debug no log.
		/// </summary>
		/// <param name="message"> mensagem a ser escrita no log.</param>
		public static void LogDebug(string message)
		{
			GetLogger().Debug(message);
		}
	}
}