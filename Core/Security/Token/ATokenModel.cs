namespace Percolore.Core.Security.Token
{
	public abstract class ATokenModel
    {
        protected long _expiration;
        protected int _type;
        protected string _checkHash;
        protected int _encryptionKey;
        protected string _encryptedContent;
        protected TokenMap _map;

        /// <summary>
        /// Data de expiração do token. [Campo de conteúdo]        
        /// </summary>
        public long Expiration
        {
            get { return _expiration; }
            set { _expiration = value; }
        }

        /// <summary>
        /// Tipo do token. [Campo de conteúdo]
        /// </summary>
        public int Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Hash de verificação do token. [Campo de verificação]
        /// </summary>
        public string CheckHash
        {
            get { return _checkHash; }
            internal set { _checkHash = value; }
        }

        /// <summary>
        /// Chave de criptografia. [Campo de verificação]
        /// </summary>
        public int EncryptionKey
        {
            get { return _encryptionKey; }
            internal set { _encryptionKey = value; }
        }

        /// <summary>
        /// Campos de conteúdo criptografados.
        /// </summary>
        public string EncryptedContent
        {
            get { return _encryptedContent; }
            internal set { _encryptedContent = value; }
        }

        /// <summary>
        /// Mapeamento dos campos de conteúdo.
        /// </summary>
        public TokenMap Map
        {
            get { return _map; }
        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ATokenModel(int type, long expiration)
        {
            _type = type;
            _expiration = expiration;
            _checkHash = string.Empty;
            _encryptedContent = string.Empty;
            _encryptionKey = 0;

            _map = new TokenMap();
            _map.Items.Add(
                new TokenMapItem(nameof(this.Type), 3));
            _map.Items.Add(
                new TokenMapItem(nameof(this.Expiration), 10));
        }
    }
}