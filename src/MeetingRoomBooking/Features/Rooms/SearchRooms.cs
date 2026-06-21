using MeetingRoomBooking.Shared.Domain.Features.Rooms;
using MeetingRoomBooking.Shared.Persistence;
using MeetingRoomBooking.Shared.Slices;
using MeetingRoomBooking.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Features.Rooms;

public sealed class SearchRooms : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet(
            ApiEndpoints.Rooms.Base,
            async Task<IResult> (
                string? location,
                int? capacity,
                string[]? amenities,
                bool? isActive,
                MeetingRoomBookingDbContext dbContext,
                CancellationToken cancellationToken) =>
            {
                var query = dbContext.Rooms.AsNoTracking().AsQueryable();

                if (!string.IsNullOrWhiteSpace(location))
                {
                    var trimmedLocation = location.Trim();
                    query = query.Where(room => room.Location.Contains(trimmedLocation));
                }

                if (capacity.HasValue)
                {
                    query = query.Where(room => room.Capacity >= capacity.Value);
                }

                if (isActive.HasValue)
                {
                    query = query.Where(room => room.IsActive == isActive.Value);
                }

                var rooms = await query
                    .OrderBy(room => room.Unit)
                    .ThenBy(room => room.Name)
                    .ToListAsync(cancellationToken);

                if (amenities is { Length: > 0 })
                {
                    var requestedAmenities = amenities
                        .Where(amenity => !string.IsNullOrWhiteSpace(amenity))
                        .Select(ParseAmenity)
                        .Distinct()
                        .ToArray();

                    rooms = rooms
                        .Where(room => requestedAmenities.All(requestedAmenity => room.Amenities.Contains(requestedAmenity)))
                        .ToList();
                }

                return Results.Ok(rooms.Select(RoomListItemDto.From));
            });
    }

    public sealed record RoomListItemDto(
        Guid Id,
        string Name,
        string Unit,
        string Location,
        int Capacity,
        bool IsActive,
        string[] Amenities)
    {
        public static RoomListItemDto From(Room room)
            => new(
                room.Id,
                room.Name,
                room.Unit,
                room.Location,
                room.Capacity,
                room.IsActive,
                room.Amenities.Select(amenity => amenity.ToString()).Order().ToArray());
    }

    private static RoomAmenity ParseAmenity(string amenity)
        => amenity.Trim().ToLowerInvariant() switch
        {
            "tv" or "television" => RoomAmenity.Television,
            "projector" => RoomAmenity.Projector,
            "whiteboard" or "white-board" => RoomAmenity.Whiteboard,
            "videoconference" or "video-conference" or "video_conference" => RoomAmenity.VideoConference,
            "coffee" => RoomAmenity.Coffee,
            _ => throw new ArgumentException($"Unsupported amenity '{amenity}'.", nameof(amenity))
        };
}
