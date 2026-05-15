using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TransactionSystemApi.Infrastructure;

namespace TransactionSystemApi.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    // Keep one connection open so the in-memory DB survives across DbContext instances
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public Mock<ITreasuryApiClient> TreasuryApiClient { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();

        builder.ConfigureServices(services =>
        {
            // Replace Postgres with the persistent SQLite connection
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(_connection));

            // Replace the real Treasury API client with a mock
            var clientDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ITreasuryApiClient));
            if (clientDescriptor != null)
                services.Remove(clientDescriptor);

            services.AddSingleton<ITreasuryApiClient>(TreasuryApiClient.Object);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _connection.Dispose();
    }
}
