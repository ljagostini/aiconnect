using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using Percolore.Core.Logging;

namespace Percolore.IOConnect.Util
{
	/// <summary>
	/// Utility class for Bluetooth.
	/// </summary>
	public class Bluetooth
	{
		/// <summary>
		/// Pair bluetooth devices from a collection of bluetooth device.
		/// </summary>
		/// <param name="result">A collection of <see cref="BluetoothDeviceInfo"/>.</param>
		public static void PairBluetoothDevices(IReadOnlyCollection<BluetoothDeviceInfo> result)
		{
			foreach (BluetoothDeviceInfo d in result)
			{
				try
				{
					if (d.DeviceName.Contains("HC-06-") || d.DeviceName.Contains("HC-05-"))
					{
						bool pairResult = BluetoothSecurity.PairRequest(d.DeviceAddress, "avsb");

						if (pairResult)
						{
							Log.Logar(
								TipoLog.Comunicacao,
								Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema,
								"Dispositivo Pareando Bluetooth: " + d.DeviceName
								);

							bool state = true;
							foreach (var service in d.InstalledServices)
								d.SetServiceState(service, state);
						}
						else if (d.DeviceName.Contains("HC-06-"))
						{
							pairResult = BluetoothSecurity.PairRequest(d.DeviceAddress, "1500");

							if (pairResult)
							{
								Log.Logar(
									TipoLog.Comunicacao,
									Percolore.IOConnect.Util.ObjectParametros.PathDiretorioSistema,
									"Dispositivo Pareando Bluetooth: " + d.DeviceName
									);

								bool state = true;
								foreach (var service in d.InstalledServices)
									d.SetServiceState(service, state);
							}
						}
					}
				}
				catch (Exception e)
				{
					LogManager.LogError($"Erro no módulo {typeof(Bluetooth).Name}: ", e);
				}
			}
		}
	}
}