namespace Percolore.IOConnect.Util
{
	internal class ErrorMessageHandler
	{
		public static string GetFriendlyErrorMessage(Exception exception)
		{
			if (exception is null)
				return string.Empty;

			const string MODBUS_RTU_SEND_FC3_METHOD = "Percolore.IOConnect.ModBusRtu.SendFc3";
			const string PURGE_FORM = "fPurga";
			const string SERIAL_PORT_NOT_OPEN = "Serial port not open";
			const string TIMEOUT_READING_ERROR = "Error timeout reader";
			const string TIMEOUT_WRITING_ERROR = "The write timed out";

			bool isPurge = (exception.StackTrace is null) ? false : exception.StackTrace.Contains(PURGE_FORM, StringComparison.InvariantCultureIgnoreCase);
			bool isSendFc3Method = (exception.StackTrace is null) ? false : exception.StackTrace.Contains(MODBUS_RTU_SEND_FC3_METHOD, StringComparison.InvariantCultureIgnoreCase);

			// Connection lost with device (emergency button pressed, backward switch open, energy interrupted or energy plug disconected)
			if (exception.Message.Contains(TIMEOUT_READING_ERROR, StringComparison.InvariantCultureIgnoreCase) ||
				(!isPurge && exception.Message.Contains(SERIAL_PORT_NOT_OPEN, StringComparison.InvariantCultureIgnoreCase)))
				return Negocio.IdiomaResxExtensao.Global_Falha_PerdaConexaoDispositivo;

			// No connectivity with device (device turned of, cable disconnect or damaged, bluetooth turned of on device, emergency button pressed)
			if (exception.Message.Contains(Negocio.IdiomaResxExtensao.Global_DisensaNaoConectado, StringComparison.InvariantCultureIgnoreCase))
				return Negocio.IdiomaResxExtensao.Global_Falha_DispositivoSemConectividade
					 + Environment.NewLine
					 + Negocio.IdiomaResxExtensao.Global_Confirmar_DesejaTentarNovamente;

			// Bluetooth communication failed (Bluetooth issues)
			if (exception.Message.Contains(TIMEOUT_WRITING_ERROR, StringComparison.InvariantCultureIgnoreCase) ||
				(exception is NullReferenceException && isSendFc3Method) ||
				(isPurge && exception.Message.Contains(SERIAL_PORT_NOT_OPEN, StringComparison.InvariantCultureIgnoreCase)))
				return Negocio.IdiomaResxExtensao.Global_Falha_PerdaConexaoBluetooh;

			// None of previous cases
			return string.Empty;
		}
	}
}