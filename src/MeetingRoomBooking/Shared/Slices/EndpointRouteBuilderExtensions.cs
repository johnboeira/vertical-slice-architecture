using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MeetingRoomBooking.Shared.Slices;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapSliceEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var slices = endpointRouteBuilder.ServiceProvider.GetRequiredService<IEnumerable<ISlice>>();

        foreach (var slice in slices)
        {
            slice.AddEndpoint(endpointRouteBuilder);
        }

        return endpointRouteBuilder;
    }
}
