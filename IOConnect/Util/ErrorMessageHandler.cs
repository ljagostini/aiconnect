using Percolore.Core.Exceptions;
using Percolore.IOConnect.Negocio;

namespace Percolore.IOConnect.Util
{
	internal class ErrorMessageHandler
	{
		/// <summary>
		/// Get a friendly error message from an exception
		/// </summary>
		/// <param name="exception">The exception</param>
		/// <param name="tipoConexao">The connection type of the serial port</param>
		/// <returns></returns>
		public static string GetFriendlyErrorMessage(Exception exception, SerialPortConnectionType tipoConexao = SerialPortConnectionType.Unknown)
		{
			if (exception is null)
				return string.Empty;

			if (exception is BluetoothException)
				return exception.Message;

			const string MODBUS_RTU_SEND_FC3_METHOD = "Percolore.IOConnect.ModBusRtu.SendFc3";
			// Forms
			const string PURGE_FORM = "fPurga";
			const string RECIRCULAR_FORM = "fRecircular";
			// Messages
			const string SERIAL_PORT_NOT_OPEN = "Serial port not open";
			const string TIMEOUT_READING_ERROR = "Error timeout reader";
			const string TIMEOUT_WRITING_ERROR = "The write timed out";

			bool isPurge = (exception.StackTrace is null) ? false : exception.StackTrace.Contains(PURGE_FORM, StringComparison.InvariantCultureIgnoreCase);
			bool isRecircular = (exception.StackTrace is null) ? false : exception.StackTrace.Contains(RECIRCULAR_FORM, StringComparison.InvariantCultureIgnoreCase);
			bool isSendFc3Method = (exception.StackTrace is null) ? false : exception.StackTrace.Contains(MODBUS_RTU_SEND_FC3_METHOD, StringComparison.InvariantCultureIgnoreCase);

			// No serial port found (Bluetooth disconnected on Windows ou USB/Serial cable disconnected on PC)
			if (exception.Message.Contains(Negocio.IdiomaResxExtensao.Global_Falha_NenhumaPortaSerial))
				return Negocio.IdiomaResxExtensao.Global_Falha_NenhumaPortaSerial
					 + Environment.NewLine
					 + Negocio.IdiomaResxExtensao.Global_Confirmar_DesejaTentarNovamente;

			// Connection lost with device (emergency button pressed, backward switch open, energy interrupted or energy plug disconected)
			if (exception.Message.Contains(TIMEOUT_READING_ERROR, StringComparison.InvariantCultureIgnoreCase) ||
				(!isPurge && !isRecircular && exception.Message.Contains(SERIAL_PORT_NOT_OPEN, StringComparison.InvariantCultureIgnoreCase)))
				return Negocio.IdiomaResxExtensao.Global_Falha_PerdaConexaoDispositivo
					 + Environment.NewLine
					 + Negocio.IdiomaResxExtensao.Global_Confirmar_DesejaTentarNovamente;

			// No connectivity with device (device turned of, cable disconnect or damaged, bluetooth turned of on device, emergency button pressed)
			if (exception.Message.Contains(Negocio.IdiomaResxExtensao.Global_DisensaNaoConectado, StringComparison.InvariantCultureIgnoreCase))
				return Negocio.IdiomaResxExtensao.Global_Falha_DispositivoSemConectividade
					 + Environment.NewLine
					 + Negocio.IdiomaResxExtensao.Global_Confirmar_DesejaTentarNovamente;

			// Bluetooth communication failed (Bluetooth issues)
			if (exception.Message.Contains(TIMEOUT_WRITING_ERROR, StringComparison.InvariantCultureIgnoreCase) ||
				(exception is NullReferenceException && isSendFc3Method) ||
				((isPurge || isRecircular) && exception.Message.Contains(SERIAL_PORT_NOT_OPEN, StringComparison.InvariantCultureIgnoreCase)))
				return Negocio.IdiomaResxExtensao.Global_Falha_PerdaConexaoBluetooh;

			// None of previous cases
			return string.Empty;
		}
	}
}