namespace Percolore.Core.Security.Token
{
    /// <summary>
    /// Identidade do alvo de execução de um token.
    /// </summary>
    public class TokenTargetIdentity
    {
        string _serial;
        string _appGuid;

        /// <summary>
        /// Número serial da máquina onde na qual o token será autenticado.
        /// </summary>
        public string Serial
        {
            get { return _serial; }
            set { _serial = value; }
        }

        /// <summary>
        /// Guid de identificação da aplicação na qual o token será autenticado.
        /// </summary>
        public string AppGuid
        {
            get { return _appGuid; }
            set { _appGuid = value; }
        }

        public TokenTargetIdentity()
        {
            _serial = string.Empty;
            _appGuid = string.Empty;
        }

        public TokenTargetIdentity(string serial, string appGuid)
        {
            _serial = serial;
            _appGuid = appGuid;
        }

        public override string ToString()
        {
            return
                string.Concat(Serial, AppGuid);
        }
    }
}