using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MeetingRoomBooking.Shared.Slices;

public static class ServiceCollectionExtensions
{
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