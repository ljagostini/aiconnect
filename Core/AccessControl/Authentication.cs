namespace Percolore.Core.AccessControl
{	public class Authentication
    {
        private Profile _profile;
        private Permissions _permissions;

        public Profile Profile
        {
            get { return _profile; }
        }
        public Permissions Permissions
        {
            get { return _permissions; }
        }

        public Authentication(Profile profile)
        {
            _profile = profile;

            PermissionBuilder permissionBuilder =
                new PermissionBuilder(Profile);
            _permissions = permissionBuilder.Generate();
        }
    }
}