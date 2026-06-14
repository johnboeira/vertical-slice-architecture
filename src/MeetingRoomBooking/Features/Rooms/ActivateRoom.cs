using MeetingRoomBooking.Shared.Domain.Features.Rooms;
using MeetingRoomBooking.Shared.Persistence;
using MeetingRoomBooking.Shared.Slices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Features.Rooms;

public sealed class ActivateRoom : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/api/rooms/{roomId:guid}/activate", async Task<IResult> (Guid roomId, MeetingRoomBookingDbContext dbContext, CancellationToken cancellationToken) =>
            {
                var room = await dbContext.Rooms.SingleOrDefaultAsync(existingRoom => existingRoom.Id == roomId, cancellationToken);

                if (room is null)
                {
                    return Results.NotFound();
                }

                room.Activate();
                await dbContext.SaveChangesAsync(cancellationToken);

                return Results.Ok(RoomStatusDto.From(room));
            });
    }

    public sealed record RoomStatusDto(Guid Id, bool IsActive)
    {
        public static RoomStatusDto From(Room room) => new(room.Id, room.IsActive);
    }
}
