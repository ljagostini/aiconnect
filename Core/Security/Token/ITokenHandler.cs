namespace Percolore.Core.Security.Token
{
    public interface ITokenHandler<T> where T : ATokenModel
    {
        /// <summary>
        /// Cria um token válido a partir do modelo fornecido.
        /// </summary>
        /// <returns>Token string criptografado</returns>
        string Create(
            TokenTargetIdentity target, T model);

        /// <summary>
        /// Lê um token string e constrói um modelo do tipo fornecido.
        /// </summary>
        /// <returns>Modelo populado do mesmo tipo fornecido.</returns>
        T Read(string token);

        /// <summary>
        /// Valida informações contidas no token.
        /// </summary>
        /// <returns>Status do token.</returns>
        TokenStatus Validate(
            TokenTargetIdentity target, T model);
    }
}