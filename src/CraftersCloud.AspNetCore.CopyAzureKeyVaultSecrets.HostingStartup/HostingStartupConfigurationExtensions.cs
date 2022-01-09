using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace CraftersCloud.AspNetCore.CopyAzureKeyVaultSecrets.HostingStartup
{
    internal static class HostingStartupConfigurationExtensions
    {
        public static IConfiguration GetBaseConfiguration()
        {
            return new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .AddEnvironmentVariables("DOTNET_")// needed for resolving applicationName
                .Build();
        }
        public static bool IsEnabled(this IConfiguration configuration, string hostingStartupName, string featureName)
        {
            if (configuration.TryGetOption(hostingStartupName, featureName, out var value))
            {
                value = value.ToLowerInvariant();
                return value != "false" && value != "0";
            }

            return false;
        }

        public static bool TryGetOption(this IConfiguration configuration, string hostingStartupName, string featureName, out string value)
        {
            value = configuration[$"HostingStartup:{hostingStartupName}:{featureName}"];
            return !string.IsNullOrEmpty(value);
        }

        public static string ResolveUserSecretsId(this IConfiguration configuration,string hostingStartupName, string featureName)
        {
            string applicationName = configuration[HostDefaults.ApplicationKey];

            configuration.TryGetOption(hostingStartupName, featureName, out string userSecretsIdOverride);
            if (!string.IsNullOrEmpty(userSecretsIdOverride))
            {
                return userSecretsIdOverride;
            }

            Assembly assembly = !string.IsNullOrEmpty(applicationName)
                ? Assembly.Load(applicationName)
                : Assembly.GetEntryAssembly();

            return assembly.ResolveUserSecretsId();
        }
    }
}