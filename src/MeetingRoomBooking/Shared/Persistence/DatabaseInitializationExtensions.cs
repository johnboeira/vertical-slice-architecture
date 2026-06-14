using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MeetingRoomBooking.Shared.Persistence;

public static class DatabaseInitializationExtensions
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<MeetingRoomBookingDbContext>();
        var databaseFileOptions = scope.ServiceProvider.GetRequiredService<DatabaseFileOptions>();

        var databaseDirectory = Path.GetDirectoryName(databaseFileOptions.DatabaseFilePath);
        if (!string.IsNullOrWhiteSpace(databaseDirectory))
        {
            Directory.CreateDirectory(databaseDirectory);
            var directoryInfo = new DirectoryInfo(databaseDirectory);
            if ((directoryInfo.Attributes & FileAttributes.Hidden) == 0)
            {
                directoryInfo.Attributes |= FileAttributes.Hidden;
            }
        }

        await dbContext.Database.EnsureCreatedAsync();

        if (string.IsNullOrWhiteSpace(databaseDirectory))
        {
            return;
        }

        var databaseFileName = Path.GetFileName(databaseFileOptions.DatabaseFilePath);
        foreach (var filePath in Directory.GetFiles(databaseDirectory, $"{databaseFileName}*"))
        {
            var currentAttributes = File.GetAttributes(filePath);
            if ((currentAttributes & FileAttributes.Hidden) == 0)
            {
                File.SetAttributes(
                    filePath,
                    currentAttributes | FileAttributes.Hidden);
            }
        }
    }
}
