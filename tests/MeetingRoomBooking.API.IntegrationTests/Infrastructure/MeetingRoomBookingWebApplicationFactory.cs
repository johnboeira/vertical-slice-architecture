using MeetingRoomBooking.Shared.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MeetingRoomBooking.API.IntegrationTests.Infrastructure;

public sealed class MeetingRoomBookingWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly string _databaseDirectory;
    private readonly string _databaseFilePath;

    public MeetingRoomBookingWebApplicationFactory()
    {
        _databaseDirectory = Path.Combine(Path.GetTempPath(), "MeetingRoomBooking.Tests", Guid.NewGuid().ToString("N"));
        _databaseFilePath = Path.Combine(_databaseDirectory, "integration-tests.db");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<MeetingRoomBookingDbContext>>();
            services.RemoveAll<MeetingRoomBookingDbContext>();
            services.RemoveAll<DatabaseFileOptions>();
            services.RemoveAll<IHostedService>();

            Directory.CreateDirectory(_databaseDirectory);

            services.AddDbContext<MeetingRoomBookingDbContext>(options =>
                options.UseSqlite($"Data Source={_databaseFilePath}"));

            services.AddSingleton(new DatabaseFileOptions(_databaseFilePath));
        });
    }

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MeetingRoomBookingDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public new Task DisposeAsync()
    {
        Dispose();
        return Task.CompletedTask;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
        {
            return;
        }

        if (Directory.Exists(_databaseDirectory))
        {
            try
            {
                var directoryInfo = new DirectoryInfo(_databaseDirectory)
                {
                    Attributes = FileAttributes.Normal
                };

                foreach (var fileInfo in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
                {
                    fileInfo.Attributes = FileAttributes.Normal;
                }

                directoryInfo.Delete(recursive: true);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}
