using Percolore.Core.Security.Cryptography;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Percolore.Core.Security.Token
{
	public class TokenHandler<T> : ITokenHandler<T> where T : ATokenModel, new()
    {
        public string Create(TokenTargetIdentity target, T model)
        {
            string value;
            string content = string.Empty;

            //Recupera propriedades do modelo fornecido
            PropertyInfo[] properties = typeof(T).GetProperties();

            //Percorre mapa de conteúdo do modelo
            foreach (TokenMapItem map in model.Map.Items)
            {
                //Inicia valor
                value = string.Empty;

                //Recupera propriedade do modelo a partir do mapa
                PropertyInfo property =
                    properties.SingleOrDefault(f => f.Name == map.Field);

                //Verifica se a propriedade foi encontrada e extrai seu valor
                if (property != null)
                    value = property.GetValue(model).ToString();

                /* Formata o valor extraído com zeros à esquerda 
                 * para que a informação tenha o mesmo tamanho definido no 
                 * mapa de conteúdo*/
                content += value.PadLeft(map.Lenght, '0');
            }

            //Criptografa conteúdo
            int key;
            string encryption = RailFence.Encrypt(content, out key);

            /* Target do token */
            string contentTarget = target.ToString();

            /* Gera hash sobre alvo, conteúdo criptografado e chave de criptografia */
            string hashContent =
                string.Concat(contentTarget, encryption, key);
            string checkHash = GetCheckHash(hashContent);

            //Gera token string
            string tokenString =
                string.Concat(encryption, key, checkHash);

            return tokenString;
        }

        public T Read(string token)
        {
            /* Instância do model fornecido em T */
            T model = new T();

            //Número de caractere dos campos checkHash e key
            const int VALIDATION_FIELDS_LENGHT = 5;
            int TOKEN_LENGHT = VALIDATION_FIELDS_LENGHT + model.Map.GetLenght();

            /* Verifica se token tem tamanho mínimo e se todos os dígitos são numéricos.
             * Se uma das condições falhar o token será reriado com zeros até atingir o númeró padrão 
             * de caracteres definido para o token.
             * Isso é feito para que não ocorra erros ao extrair caracteres de um token contendo 
             * caracteres não inválidos ou sinsuficientes. */
            bool notNumeric = Regex.IsMatch(token, @"[^0-9]+");
            bool notLenght = (token.Length != TOKEN_LENGHT);
            if (notNumeric || notLenght)
            {
                token = new string('0', TOKEN_LENGHT);
            }

            //CheckHash
            model.CheckHash = token.Substring(token.Length - 4);

            //Chave de criptogafia
            string keyString =
                token.Substring(token.Length - model.CheckHash.Length - 1, 1);
            int key = 0;
            int.TryParse(keyString, out key);
            model.EncryptionKey = key;

            //Conteúdo criptografado
            model.EncryptedContent = token.Remove(token.Length - 5);
            string contentString =
                RailFence.Decrypt(model.EncryptedContent, model.EncryptionKey);

            //Recupera propriedades do modelo fornecido
            PropertyInfo[] properties = typeof(T).GetProperties();

            //Percore mapa de conteúdo do token
            foreach (TokenMapItem item in model.Map.Items)
            {
                //Extrai item do mapa da tokenString
                string value = contentString.Substring(0, item.Lenght);

                /* Remove item do mapa da tokenString para que não seja 
                 * necessário efetuar cálculo considerando o tamanho 
                 * das posições anteriores. */
                contentString = contentString.Remove(0, item.Lenght);

                //Localiza propriedade do model referente ao item do mapa
                PropertyInfo property =
                    properties.SingleOrDefault(f => f.Name == item.Field);

                if (property != null)
                {
                    //Define propriedade do model com valor extraído da tokenString
                    property.SetValue(
                        model, Convert.ChangeType(value, property.PropertyType), null);
                }
            }

            return model;
        }

        public TokenStatus Validate(TokenTargetIdentity target, T model)
        {
            TokenStatus status = TokenStatus.Valid;

            string hashContent = string.Concat(
                target.ToString(),
                model.EncryptedContent,
                model.EncryptionKey);

            string CheckHashValidator = GetCheckHash(hashContent);

            /* Valida se hash recalculado é igual ao hash do model */
            if (model.CheckHash != CheckHashValidator)
            {
                return TokenStatus.InvalidFormat;
            }

            long expirationValidator =
                DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            /* Valida se token expirou */
            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > model.Expiration)
            {
                return TokenStatus.Expired;
            }

            return status;
        }

        string GetCheckHash(string text)
        {
            using SHA256 sha256 = SHA256.Create();
			byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
			string hashCode = BitConverter.ToUInt32(bytes, 0).ToString();
			string hashToken = hashCode.Substring(hashCode.Length - 4, 4);
			return hashToken;
		}
    }
}