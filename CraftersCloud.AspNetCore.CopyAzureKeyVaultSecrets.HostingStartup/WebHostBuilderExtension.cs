using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;

namespace CraftersCloud.AspNetCore.CopyAzureKeyVaultSecrets.HostingStartup
{
    [PublicAPI]
    public static class WebHostBuilderExtension
    {
        public static IWebHostBuilder AddCopyKeyVaultSecretsHostingStartup(this IWebHostBuilder builder)
        {
            var startup = new CopyKeyVaultSecretsHostingStartup();
            startup.Configure(builder);
            return builder;
        }
    }
}
