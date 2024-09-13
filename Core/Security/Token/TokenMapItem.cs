namespace Percolore.Core.Security.Token
{
    public class TokenMapItem
    {
        string _field;
        int _lenght;

        /// <summary>
        /// Nome do campo.
        /// </summary>
        public string Field
        {
            get { return _field; }
        }

        /// <summary>
        /// Número de caracteres da informação. 
        /// Deve ser formatado exatamente do tamanho definido.
        /// </summary>
        public int Lenght
        {
            get { return _lenght; }
        }

        /// <summary>
        /// Contrutor padrão
        /// </summary>
        /// <param name="field"></param>
        /// <param name="leght"></param>
        public TokenMapItem(string field, int lenght)
        {
            _field = field;
            _lenght = lenght;
        }
    }
}