using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace CraftersCloud.AspNetCore.CopyAzureKeyVaultSecrets.HostingStartup
{
    internal static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddKeyVaultSecrets(this IConfigurationBuilder configurationBuilder,
            string keyVault)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var authenticationCallback =
                new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback);
            var keyVaultClient = new KeyVaultClient(authenticationCallback);

            configurationBuilder.AddAzureKeyVault(keyVault, keyVaultClient, new DefaultKeyVaultSecretManager());

            return configurationBuilder;
        }
    }
}
