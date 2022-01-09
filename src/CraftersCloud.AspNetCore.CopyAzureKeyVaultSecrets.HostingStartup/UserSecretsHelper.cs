using System.Reflection;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace CraftersCloud.AspNetCore.CopyAzureKeyVaultSecrets.HostingStartup
{
    internal static class AssemblyExtensions
    {
        public static string ResolveUserSecretsId(this Assembly assembly)
        {
            var attribute = assembly
                .GetCustomAttribute<UserSecretsIdAttribute>();

            return attribute?.UserSecretsId;
        }
    }
}
