namespace JWTAuthServer.Common.Constants
{
    public class AuthServerConstants
    {
        private static AuthServerConstants _instance;

        public static AuthServerConstants getInstance()
        {
            if (_instance == null)
            {
                _instance = new AuthServerConstants();
            }

            return _instance;
        }

        public string UsernameTokenKey { get { return "userName"; } }
        public string RoleTokenKey { get { return "role"; } }
        public string AccessTokenKey { get { return "access_token"; } }
        public string AccessTokenTypeKey { get { return "token_type"; } }
        public string ExpireTokenKey { get { return "expires_in"; } }
        public string IssuedTokenKey { get { return ".issued"; } }
        public string ExpireInTokenKey { get { return ".expires"; } }
        public string InActive { get { return "InActive"; } }
        public string Active { get { return "Active"; } }
        public string ExternalAccessTokenKey { get { return "ExternalAccessToken"; } }
    }
}

