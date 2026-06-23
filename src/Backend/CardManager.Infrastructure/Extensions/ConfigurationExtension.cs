using Microsoft.Extensions.Configuration;

namespace CardManager.Infrastructure.Extensions
{
    public static class ConfigurationExtension
    {

        public static bool IsUniTestEnviromente(this IConfiguration configuration)
        {
            return configuration.GetValue<bool>("InMemoryTest");
        }

        public static string ConnectionString(this IConfiguration configuration)
        {
            return configuration.GetConnectionString("Connection")!;
        }
    }
}
