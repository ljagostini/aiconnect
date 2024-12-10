namespace Percolore.IOConnect.Util
{
	internal class ErrorMessageHandler
	{
		public static string GetFriendlyErrorMessage(Exception exception)
		{
			if (exception is null)
				return string.Empty;

			// Connection lost with device (emergency button pressed, backward switch open, energy interrupted or energy plug disconected)
			if (exception.Message.Contains("Error timeout reader", StringComparison.InvariantCultureIgnoreCase) ||
				exception.Message.Contains("Serial port not open", StringComparison.InvariantCultureIgnoreCase))
				return Negocio.IdiomaResxExtensao.Global_Falha_PerdaConexaoDispositivo;

			// No connectivity with device (device turned of, cable disconnect or damaged, bluetooth turned of on device, emergency button pressed)
			if (exception.Message.Contains(Negocio.IdiomaResxExtensao.Global_DisensaNaoConectado, StringComparison.InvariantCultureIgnoreCase))
				return Negocio.IdiomaResxExtensao.Global_Falha_DispositivoSemConectividade
					 + Environment.NewLine
					 + Negocio.IdiomaResxExtensao.Global_Confirmar_DesejaTentarNovamente;

			// Bluetooth communication failed (Bluetooth issues on device or bluetooth turned of)
			if (exception.Message.Contains("The write timed out", StringComparison.InvariantCultureIgnoreCase) ||
				(exception is NullReferenceException && (exception.StackTrace != null) && exception.StackTrace.Contains("Percolore.IOConnect.ModBusRtu.SendFc3")))
				return Negocio.IdiomaResxExtensao.Global_Falha_PerdaConexaoBluetooh;

			// None of previous cases
			return string.Empty;
		}
	}
}