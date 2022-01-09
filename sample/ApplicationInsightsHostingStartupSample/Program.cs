using CraftersCloud.AspNetCore.CopyAzureKeyVaultSecrets.HostingStartup;

var builder = WebApplication.CreateBuilder(args);

// Add copy KeyVault secrets hosting startup
builder.WebHost.AddCopyKeyVaultSecretsHostingStartup();

// Add services to the container.
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
