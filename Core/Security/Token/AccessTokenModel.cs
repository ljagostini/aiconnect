namespace Percolore.Core.Security.Token
{
	/// <summary>
	/// Implementação de de um token de acesso
	/// </summary>
	public class AccessTokenModel : ATokenModel
    {
        int _profile;

        /// <summary>
        /// Código do perfil de usuário.
        /// </summary>
        public int Profile
        {
            get { return _profile; }
            set { _profile = value; }
        }

        public AccessTokenModel() : base((int)TokenType.AccessToken, 0)
        {
            _profile = 0;
            _map.Items.Add(
                new TokenMapItem(nameof(this.Profile), 3));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expiration">Data e hora em UTC</param>
        /// <param name="profile">Perfil de usuário.</param>
        public AccessTokenModel(DateTimeOffset expiration, int profile)
         : base(
               (int)TokenType.AccessToken, expiration.ToUnixTimeSeconds())
        {
            _profile = profile;
            _map.Items.Add(
                new TokenMapItem(nameof(this.Profile), 3));
        }
    }
}