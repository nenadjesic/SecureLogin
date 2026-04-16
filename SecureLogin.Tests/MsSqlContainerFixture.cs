using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace SecureLogin.Tests;

public class MsSqlContainerFixture  : IAsyncLifetime
{
    public MsSqlContainer Container { get; private set; } = default!;

    public string ConnectionString => Container.GetConnectionString();

    public async Task InitializeAsync()
    {
        Container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04")
            .WithPassword("Test12345!")
            .Build();
        await Container.StartAsync();

        // Optional but recommended: wait until DB is actually ready
        await WaitUntilDatabaseReady();
    }

    public async Task DisposeAsync()
    {
        await Container.StopAsync();
        await Container.DisposeAsync();
    }

    private async Task WaitUntilDatabaseReady()
    {
        var retries = 10;

        while (retries-- > 0)
        {
            try
            {
                await using var conn = new SqlConnection(ConnectionString);
                await conn.OpenAsync();
                return;
            }
            catch
            {
                await Task.Delay(1000);
            }
        }

        throw new Exception("SQL Server did not become ready in time.");
    }
}
