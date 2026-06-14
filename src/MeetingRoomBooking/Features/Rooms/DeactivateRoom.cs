using MeetingRoomBooking.Shared.Domain;
using MeetingRoomBooking.Shared.Domain.Features.Rooms;
using MeetingRoomBooking.Shared.Persistence;
using MeetingRoomBooking.Shared.Slices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Features.Rooms;

public sealed class DeactivateRoom : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost(
            "/api/rooms/{roomId:guid}/deactivate",
            async Task<IResult> (Guid roomId, MeetingRoomBookingDbContext dbContext, CancellationToken cancellationToken) =>
            {
                var room = await dbContext.Rooms
                    .SingleOrDefaultAsync(existingRoom => existingRoom.Id == roomId, cancellationToken);

                if (room is null)
                {
                    return Results.NotFound();
                }

                try
                {
                    room.Deactivate();
                    await dbContext.SaveChangesAsync(cancellationToken);

                    return Results.Ok(RoomStatusDto.From(room));
                }
                catch (DomainRuleViolationException exception)
                {
                    return Results.Problem(detail: exception.Message, statusCode: StatusCodes.Status400BadRequest);
                }
            });
    }

    public sealed record RoomStatusDto(Guid Id, bool IsActive)
    {
        public static RoomStatusDto From(Room room)
            => new(room.Id, room.IsActive);
    }
}
