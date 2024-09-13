namespace Percolore.Core.Security.Token
{
	/// <summary>
	/// implementação de um token de validade da manutenção
	/// </summary>
	public class MaintenanceValidityTokenModel : ATokenModel
    {
        long _Validity;

        /// <summary>
        /// UTC Timestamp representando a data de vlaidade da manutenção. 
        /// </summary>
        public long Validity
        {
            get { return _Validity; }
            set { _Validity = value; }
        }

        public MaintenanceValidityTokenModel()
            : base((int)TokenType.MaintenanceValidityToken, 0)
        {
            _Validity = 0;
            _map.Items.Add(
                new TokenMapItem(nameof(Validity), 10));
        }

        public MaintenanceValidityTokenModel(DateTimeOffset expiration, DateTimeOffset validity)
            : base(
                  (int)TokenType.MaintenanceValidityToken, expiration.ToUnixTimeSeconds())
        {
            _Validity = validity.ToUnixTimeSeconds();
            _map.Items.Add(
                new TokenMapItem(nameof(Validity), 10));
        }

    }
}