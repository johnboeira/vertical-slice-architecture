using MeetingRoomBooking.Shared.Domain.Features.Rooms;
using MeetingRoomBooking.Shared.Persistence;
using MeetingRoomBooking.Shared.Slices;
using MeetingRoomBooking.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Features.Rooms;

public sealed class GetRoom : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet(
            ApiEndpoints.Rooms.ById,
            async Task<IResult> (Guid roomId, MeetingRoomBookingDbContext dbContext, CancellationToken cancellationToken) =>
            {
                var room = await dbContext.Rooms
                    .AsNoTracking()
                    .SingleOrDefaultAsync(existingRoom => existingRoom.Id == roomId, cancellationToken);

                return room is null ? Results.NotFound() : Results.Ok(RoomDetailsDto.From(room));
            });
    }

    public sealed record RoomDetailsDto(
        Guid Id,
        string Name,
        string Unit,
        string Location,
        int Capacity,
        bool IsActive,
        string[] Amenities)
    {
        public static RoomDetailsDto From(Room room)
            => new(
                room.Id,
                room.Name,
                room.Unit,
                room.Location,
                room.Capacity,
                room.IsActive,
                room.Amenities.Select(amenity => amenity.ToString()).Order().ToArray());
    }
}
