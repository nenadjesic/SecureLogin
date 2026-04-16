using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SecureLogin.Tests;

[Collection("mssql")]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    private readonly MsSqlContainerFixture _dbFixture;

    private SecureLoginWebApplicationFactory _factory = null!;

    protected IntegrationTestBase(MsSqlContainerFixture dbFixture)
    {
        _dbFixture = dbFixture;
    }

    protected HttpClient CreateClient()
    {
        return _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        _factory = new SecureLoginWebApplicationFactory(_dbFixture.ConnectionString);

        await InitializeDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
    }

    protected virtual async Task InitializeDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        await db.Database.MigrateAsync();
    }
}
