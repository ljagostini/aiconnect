using Microsoft.Win32;

namespace Percolore.Core.Persistence.WindowsRegistry
{
	/// <summary>
	/// Representa uma estrutura de registro com chave e valores
	/// </summary>
	public abstract class AWindowsRegistry : IDisposable
    {
        protected RegistryKey _registryKey = null;

        protected AWindowsRegistry(string subkey)
        {
            _registryKey =
                Registry.CurrentUser.OpenSubKey(subkey, true);

            if (_registryKey == null)
            {
                _registryKey =
                        Registry.CurrentUser.CreateSubKey(subkey);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_registryKey != null)
                    _registryKey.Dispose();
            }
        }
    }
}