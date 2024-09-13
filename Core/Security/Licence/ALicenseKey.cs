namespace Percolore.Core.Security.License
{
    /// <summary>
    /// Classe abstrata para validação de uma chave contra 
    /// uma chave de código único
    /// </summary>
    public abstract class ALicenseKey
    {
        public abstract string KeyCode { get; }
        public abstract string License { get; }
        public abstract bool CheckLicense(string license);
    }
}