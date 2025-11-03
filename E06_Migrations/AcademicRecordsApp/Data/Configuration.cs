namespace AcademicRecordsApp.Data
{
    using Microsoft.Extensions.Configuration;

    internal static class Configuration
    {
        private const string DefaultConnectionString =
            "Server=.;Database=AcademicRecordsDb;Trusted_Connection=True;Encrypt=False;";

        internal static string GetConnectionString()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddUserSecrets<StartUp>()
                .Build();

            IConfigurationProvider secretsProvider = config.Providers.First();
            bool success = secretsProvider
                .TryGet("DefaultConnection_NoEncrypt", out string? connectionString);

            if (success && !string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }

            return DefaultConnectionString;
        }
    }
}
