using MeetingRoomBooking.Shared.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using MeetingRoomBooking.Shared.Slices;
using Microsoft.Extensions.DependencyInjection;

namespace MeetingRoomBooking;

public static class DependencyInjection
{
    public static IServiceCollection AddMeetingRoomBooking(
        this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        var databaseDirectory = Path.Combine(environment.ContentRootPath, "App_Data");
        Directory.CreateDirectory(databaseDirectory);

        var databaseFilePath = Path.Combine(databaseDirectory, "meeting-room-booking.db");

        services.AddDbContext<MeetingRoomBookingDbContext>(options =>
            options.UseSqlite($"Data Source={databaseFilePath}"));

        services.AddSingleton(new DatabaseFileOptions(databaseFilePath));
        services.RegisterSlices(typeof(DependencyInjection).Assembly);

        return services;
    }
}
