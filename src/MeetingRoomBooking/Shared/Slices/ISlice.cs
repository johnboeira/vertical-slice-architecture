using Microsoft.AspNetCore.Routing;

namespace MeetingRoomBooking.Shared.Slices;

public interface ISlice
{
    void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder);
}
