namespace Percolore.Core.Security.Token
{
	/// <summary>
	/// Mapa dos campos ue devem compor conteúdo de um token.
	/// </summary>
	public class TokenMap
    {
        List<TokenMapItem> _mapItems;

        /// <summary>
        /// Itens do token mapeados.
        /// </summary>
        public List<TokenMapItem> Items
        {
            get { return _mapItems; }
        }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public TokenMap()
        {
            _mapItems = new List<TokenMapItem>();
        }

        /// <summary>
        /// Soma de todos os caracteres de todos os itens mapeados.
        /// </summary>
        public int GetLenght()
        {
            int lenght = 0;
            foreach (TokenMapItem item in _mapItems)
            {
                lenght += item.Lenght;
            }

            return lenght;
        }
    }
}