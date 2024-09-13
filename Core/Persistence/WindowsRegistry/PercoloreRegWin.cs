using Microsoft.Win32;
using Percolore.Core.Security.Cryptography;

namespace Percolore.Core.Persistence.WindowsRegistry
{
	public class PercoloreRegistry : AWindowsRegistry
    {
        const string SERIAL_GUID = @"91769e7b-6c4b-4a5f-96bf-7ae1fd8108f4";
        const string VALIDADE_MANUTENCAO_GUID = "f8182244-f1a3-46c9-9d71-6004cc7aa010";
        const string DATA_HORA_MAQUINA_GUID = "6eae9613-7685-4a43-ba15-de06016e138d";

        public PercoloreRegistry() : base(@"SOFTWARE\Percolore") { }

        /// <summary>
        /// Recupera número serial deifnido pela Percolore
        /// </summary>
        public string GetSerialNumber()
        {
            object obj =
                _registryKey.GetValue(SERIAL_GUID, null);

            string valor = string.Empty;
            if (obj != null)
                valor = AES.Decrypt(obj.ToString());

            return valor;
        }

        /// <summary>
        /// Persiste número serial definido pela Percolore
        /// </summary>
        public void SetSerialNumber(string serial)
        {
            string valor = AES.Encrypt(serial);
            _registryKey.SetValue(
                SERIAL_GUID, valor, RegistryValueKind.String);
        }

        /// <summary>
        /// Recupera UnixTimestamp de validade da manutenção
        /// </summary>
        public long GetValidadeManutencao()
        {
            object obj =
                _registryKey.GetValue(VALIDADE_MANUTENCAO_GUID, null);

            long timestamp = 0;
            if (obj != null)
            {
                string valor = AES.Decrypt(obj.ToString());
                timestamp = long.Parse(valor);
            }

            return timestamp;
        }

        /// <summary>
        /// Persiste UnixTimestamp de validade da manutenção
        /// </summary>
        public void SetValidadeManutencao(long timestamp)
        {
            string valor = AES.Encrypt(timestamp.ToString());
            _registryKey.SetValue(
                VALIDADE_MANUTENCAO_GUID, valor, RegistryValueKind.String);
        }

        /// <summary>
        /// Recupera UnixTimestamp referente à data e hora da máquina em UTC
        /// </summary>
        public long GetDataHoraMaquina()
        {
            object obj =
                _registryKey.GetValue(DATA_HORA_MAQUINA_GUID, null);

            long timestamp = 0;
            if (obj != null)
            {
                string valor = AES.Decrypt(obj.ToString());
                timestamp = long.Parse(valor);
            }

            return timestamp;
        }

        /// <summary>
        /// Persiste UnixTimestamp referente à data e hora da máquina em UTC
        /// </summary>
        public void SetDataHoraMaquina()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string valor = AES.Encrypt(timestamp.ToString());
            _registryKey.SetValue(
                DATA_HORA_MAQUINA_GUID, valor, RegistryValueKind.String);
        }
    }
}