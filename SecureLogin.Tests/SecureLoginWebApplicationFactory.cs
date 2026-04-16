using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace SecureLogin.Tests;

public class SecureLoginWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public SecureLoginWebApplicationFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var dict = new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = _connectionString,
                ["RunMigrations"] = "false"
            };

            config.AddInMemoryCollection(dict!);
        });
    }
}
