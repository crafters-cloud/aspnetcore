# Copy Azure KeyVault secrets hosting startup

CraftersCloud.AspNetCore.CopyAzureKeyVaultSecrets.HostingStartup is a .NET library for copying secrets from [Azure Key Vault](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#secret-manager) into local user secrets storage. See: [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#secret-manager).

Allows development teams to store secrets on shared development Key Vault instance and keeps them locally in sync in local User Secrets store without having to perform manual (and error prone) updates.

Also, avoids paying the performance penalty of fetching secrets from KeyVault on every start of the application when in development. See: [Reading a secret from Azure Key Vault takes a long time](https://stackoverflow.com/questions/52399018/reading-a-secret-from-azure-key-vault-takes-a-long-time).

## Installation

You will need to add a reference to *CraftersCloud.AspNetCore.CopyAzureKeyVaultSecrets.HostingStartup.dll* in your project. The simplest way to do this is to use either the NuGet package manager, or the dotnet CLI.

```shell
Install-Package CraftersCloud.AspNetCore.CopyAzureKeyVaultSecrets.HostingStartup
```

Or using the .net core CLI from a terminal window:

```shell
dotnet add package CraftersCloud.AspNetCore.CopyAzureKeyVaultSecrets.HostingStartup
```

## Configuring the Asp Net core web project

### Enable user secrets in your Asp Net core web project

Using the .net core CLi from a terminal window run:

```shell
dotnet user-secrets init
```

This will add the UserSecretsId element within a PropertyGroup of the project file. For more information see: [Enable secret storage](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#enable-secret-storage).

### Edit launchSettings.json

Set the environment variables for the profiles for which you want this tool to run.

#### Environment Variables

##### ASPNETCORE_HOSTINGSTARTUP__KEYVAULT__CONFIGURATIONVAULT

This variable is used to set Url to the Azure Key Vault from which you want to read the secrets.

This library uses same Visual Studio's Connected Service authentication mechanism as the Microsoft.AspNetCore.AzureKeyVault.HostingStartup NuGet. (see [Add Key Vault to your web application by using Visual Studio Connected Services](https://docs.microsoft.com/en-us/azure/key-vault/general/vs-key-vault-add-connected-service])

The Microsoft account under which you are logged in to Visual Studio must have granted **GET** and **LIST** Secret Management Operations in the corresponding Key Vault Access Policies for the secrets to be read locally. More info [Key Vault security features](https://docs.microsoft.com/en-us/azure/key-vault/general/security-features)

```json
"ASPNETCORE_HOSTINGSTARTUP__KEYVAULT__CONFIGURATIONVAULT": "https://put-your-dev-vault-here.vault.azure.net"
```

##### ASPNETCORE_HOSTINGSTARTUP__COPYKEYVAULTSECRETS__CONFIGURATIONENABLED

This variable enables or disables the copy functionality. In development environment this should be set to **true**. In production the variable should be removed or set to **false**

```json
"ASPNETCORE_HOSTINGSTARTUP__COPYKEYVAULTSECRETS__CONFIGURATIONENABLED": "true"
```

##### ASPNETCORE_HOSTINGSTARTUP__COPYKEYVAULTSECRETS__COPYINTERVAL

This variable defines the time span after which secrets will be refreshed from the key vault. The format of the value should be a valid TimeSpan string that can be parsed (see [TimeSpan.Parse](https://docs.microsoft.com/en-us/dotnet/api/system.timespan.parse?view=netcore-2.0)).

```json
"ASPNETCORE_HOSTINGSTARTUP__COPYKEYVAULTSECRETS__COPYINTERVAL": "7.00:00:00"
```

##### ASPNETCORE_HOSTINGSTARTUP__COPYKEYVAULTSECRETS__FORCECOPYENABLED

This variable when set to **true** overrides the copy interval variable, and forces copying of the values from the key vault. Useful when a new secrets is available in the key vault and you do not want to wait for the previous fetch interval to expire.

```json
"ASPNETCORE_HOSTINGSTARTUP__COPYKEYVAULTSECRETS__FORCECOPYENABLED": "false"
```

### Update Program.cs and add call to the AddCopyKeyVaultSecretsHostingStartup() extension method

If you are using .NET 6 version of Asp Net Core project add call to the extension method *AddCopyKeyVaultSecretsHostingStartup()* while configuring the *WebApplicationBuilder*.

```csharp
using CraftersCloud.AspNetCore.CopyAzureKeyVaultSecrets.HostingStartup;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add copy KeyVault secrets hosting startup
builder.WebHost.AddCopyKeyVaultSecretsHostingStartup();

// Add services to the container.
builder.Services.AddControllers();

```

If you are using .NET 3.1 or .NET 5 version of Asp Net Core project add call to the extension method *AddCopyKeyVaultSecretsHostingStartup()* while configuring the *IWebHostBuilder*.

```csharp
using CraftersCloud.AspNetCore.CopyAzureKeyVaultSecrets.HostingStartup;

IHostBuilder builder = Host.CreateDefaultBuilder();
            builder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.AddCopyKeyVaultSecretsHostingStartup();
            });
```

#### How to setup application when in production

In production you should either remove the environment variable **ASPNETCORE_HOSTINGSTARTUP__COPYKEYVAULTSECRETS__CONFIGURATIONENABLED** or set it to **false**. The call to the method *AddCopyKeyVaultSecretsHostingStartup()* can remain since the actual copying is performed only if the value of this environment variable is set to **true**.

This library can work without any problems along with the NuGet *Microsoft.AspNetCore.AzureKeyVault.HostingStartup*.

The same environment variable for the KeyVault Url is used in both librarires (i.e. **ASPNETCORE_HOSTINGSTARTUP__KEYVAULT__CONFIGURATIONVAULT**). The environment variable **ASPNETCORE_HOSTINGSTARTUP__KEYVAULT__CONFIGURATIONENABLED** is not being used by the copy Key Vault secrets library, so the two libraries can be independently turned on/off depending on the environment.

In development:
```json
"ASPNETCORE_HOSTINGSTARTUP__COPYKEYVAULTSECRETS__CONFIGURATIONENABLED": "true",
"ASPNETCORE_HOSTINGSTARTUP__KEYVAULT__CONFIGURATIONVAULT": "https://put-your-dev-vault-here.vault.azure.net",
"ASPNETCORE_HOSTINGSTARTUP__KEYVAULT__CONFIGURATIONENABLED": "false",

```

In production
```json
"ASPNETCORE_HOSTINGSTARTUP__COPYKEYVAULTSECRETS__CONFIGURATIONENABLED": "false",
"ASPNETCORE_HOSTINGSTARTUP__KEYVAULT__CONFIGURATIONVAULT": "https://put-your-prod-vault-here.vault.azure.net",
"ASPNETCORE_HOSTINGSTARTUP__KEYVAULT__CONFIGURATIONENABLED": "true",
```
