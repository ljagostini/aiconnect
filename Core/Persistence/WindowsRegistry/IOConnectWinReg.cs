using Microsoft.Win32;
using Percolore.Core.Security.Cryptography;

namespace Percolore.Core.Persistence.WindowsRegistry
{
	public class IOConnectRegistry : AWindowsRegistry
    {
        const string LICENSE_GUID = @"91c93c47-e261-4812-b904-6c996933520c";
        const string SENHA_ADMIN_GUID = @"69a84728-4a13-4e80-b9e1-d7d8cec0da8b";
        const string DATA_INSTALACAO_GUID = "8e6b214a-c9d2-464a-9460-0860eda5f875";

        public IOConnectRegistry() :
            base(@"SOFTWARE\Percolore\85b88a51-63da-4851-b32c-bcf57ec9e5d4")
        { }

        /// <summary>
        /// Recupera licença do software.
        /// </summary>
        /// <returns></returns>
        public string GetLicense()
        {
            object obj =
                _registryKey.GetValue(LICENSE_GUID, null);

            string valor = string.Empty;
            if (obj != null)
                valor = AES.Decrypt(obj.ToString());

            return valor;
        }

        /// <summary>
        /// Persiste licença do software.
        /// </summary>
        /// <param name="valor"></param>
        public void SetLicenca(string valor)
        {
            valor = AES.Encrypt(valor);
            _registryKey.SetValue(LICENSE_GUID, valor, RegistryValueKind.String);
        }

        /// <summary>
        /// Recupera senha de administrador
        /// </summary>
        /// <returns></returns>
        public string GetSenhaAdmnistrador()
        {
            object obj =
                _registryKey.GetValue(SENHA_ADMIN_GUID, null);

            string valor = string.Empty;
            if (obj != null)
                valor = AES.Decrypt(obj.ToString());

            return valor;
        }

        /// <summary>
        /// Persiste senha do administrador
        /// </summary>
        /// <param name="valor"></param>
        public void SetSenhaAdministrador(string valor)
        {
            valor = AES.Encrypt(valor);
            _registryKey.SetValue(
                SENHA_ADMIN_GUID, valor, RegistryValueKind.String);
        }

        /// <summary>
        /// UnixTimestamp em UTC da data intalação do software
        /// </summary>
        public long GetDatainstalacao()
        {
            object obj =
                _registryKey.GetValue(DATA_INSTALACAO_GUID, null);

            long unixTimestampUTC = 0;
            if (obj != null)
            {
                string valor = AES.Decrypt(obj.ToString());
                unixTimestampUTC = long.Parse(valor);
            }

            return unixTimestampUTC;
        }

        /// <summary>
        /// Persiste UnixTimestamp UTC da data de instlação do software.
        /// </summary>
        public void SetDataInstalacao(long unixTimestampUTC)
        {
            string valor = AES.Encrypt(unixTimestampUTC.ToString());
            _registryKey.SetValue(
                DATA_INSTALACAO_GUID, valor, RegistryValueKind.String);
        }
    }
}