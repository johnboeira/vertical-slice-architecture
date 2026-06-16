using MeetingRoomBooking.Shared.Persistence;
using MeetingRoomBooking.Shared.Slices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MeetingRoomBooking;

public static class DependencyInjection
{
    public static IServiceCollection AddMeetingRoomBooking(this IServiceCollection services, IWebHostEnvironment environment)
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

    public static IServiceCollection RegisterSlices(this IServiceCollection services, Assembly assembly)
    {
        var slices = assembly.GetTypes()
            .Where(type => typeof(ISlice).IsAssignableFrom(type) && type is { IsInterface: false, IsAbstract: false, IsPublic: true });

        foreach (var slice in slices)
        {
            services.AddSingleton(typeof(ISlice), slice);
        }

        return services;
    }
}