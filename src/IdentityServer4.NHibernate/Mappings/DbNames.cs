namespace IdentityServer4.NHibernate.Mappings
{
    public static class DbNames
    {
        public static string ConfigurationSchema { get; set; }
        public static string OperationalSchema { get; set; }

        public static class Columns
        {
            public const string ApiResourceFk = "ApiResourceId";
            public const string ClientFk = "ClientId";
            public const string Value = nameof(Value);
        }

        public static class Tables
        {
            public const string ApiResources = nameof(ApiResources);
            public const string ApiResourceScopes = nameof(ApiResourceScopes);

            public const string Clients = nameof(Clients);
            public const string ClientAllowedCorsOrigins = nameof(ClientAllowedCorsOrigins);

            public const string IdentityResources = nameof(IdentityResources);
            public const string PersistedGrants = nameof(PersistedGrants);
        }
        
    }
}