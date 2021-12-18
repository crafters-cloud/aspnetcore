using System;
using CraftersCloud.AspNetCore.CopyAzureKeyVaultSecrets.HostingStartup;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

[assembly: HostingStartup(typeof(CopyKeyVaultSecretsHostingStartup))]

namespace CraftersCloud.AspNetCore.CopyAzureKeyVaultSecrets.HostingStartup
{
    [UsedImplicitly]
    public class CopyKeyVaultSecretsHostingStartup : IHostingStartup
    {
        private const string HostingStartupName = "CopyKeyVaultSecrets";
        private const string ConfigurationFeatureName = "ConfigurationEnabled";
        private const string ConfigurationCopyInterval = "CopyInterval";
        private const string ConfigurationForceCopyEnabled = "ForceCopyEnabled";
        private const string ConfigurationUserSecretsId = "UserSecretsId";

        // sections from AzureKeyVaultHostingStartup
        private const string KeyVaultHostingStartupName = "KeyVault";
        private const string ConfigurationVaultName = "ConfigurationVault";

        public void Configure(IWebHostBuilder builder)
        {
            var baseConfiguration = HostingStartupConfigurationExtensions.GetBaseConfiguration();
            if (baseConfiguration.IsEnabled(HostingStartupName, ConfigurationFeatureName))
            {
                baseConfiguration.TryGetOption(KeyVaultHostingStartupName, ConfigurationVaultName,
                    out var keyVault);

                baseConfiguration.TryGetOption(HostingStartupName,
                    ConfigurationCopyInterval, out var copyIntervalStr);

                var forceCopy = baseConfiguration.IsEnabled(HostingStartupName, ConfigurationForceCopyEnabled);

                if (!string.IsNullOrEmpty(keyVault))
                {
                    var userSecretsId =
                        baseConfiguration.ResolveUserSecretsId(HostingStartupName, ConfigurationUserSecretsId);

                    if (!string.IsNullOrEmpty(userSecretsId))
                    {
                        var store = new SecretsStore(userSecretsId);
                        var copyInterval = ParseCopyInterval(copyIntervalStr);
                        var lastRefreshDateTime = store.GetLastRefreshDateTime();
                        var nextRefreshDateTime = lastRefreshDateTime.Add(copyInterval);
                        var now = DateTime.UtcNow;

                        if (now > nextRefreshDateTime || forceCopy)
                        {
                            var configuration = RedConfigurationFromKeyVault(keyVault);
                            store.CopyValuesFrom(configuration);
                            store.UpdateLastRefreshTime(now);
                            store.Save();
                        }
                    }
                }
            }
        }

        private static IConfiguration RedConfigurationFromKeyVault(string keyVault)
        {
            return new ConfigurationBuilder()
                .AddKeyVaultSecrets(keyVault)
                .Build();
        }

        private static TimeSpan ParseCopyInterval(string refreshIntervalStr)
        {
            if (!string.IsNullOrEmpty(refreshIntervalStr))
            {
                if (TimeSpan.TryParse(refreshIntervalStr, out var result))
                {
                    return result;
                }
            }

            return TimeSpan.FromDays(7);
        }
    }
}
