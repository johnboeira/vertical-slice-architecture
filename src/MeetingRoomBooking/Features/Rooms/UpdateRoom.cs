using MeetingRoomBooking.Shared.Domain.Features.Rooms;
using MeetingRoomBooking.Shared.Persistence;
using MeetingRoomBooking.Shared.Slices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Features.Rooms;

public sealed class UpdateRoom : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPut(
            "/api/rooms/{roomId:guid}",
            async Task<IResult> (Guid roomId, UpdateRoomRequest request, MeetingRoomBookingDbContext dbContext, CancellationToken cancellationToken) =>
            {
                var validationErrors = Validate(request);
                if (validationErrors.Count > 0)
                {
                    return Results.ValidationProblem(validationErrors);
                }

                var room = await dbContext.Rooms
                    .SingleOrDefaultAsync(existingRoom => existingRoom.Id == roomId, cancellationToken);

                if (room is null)
                {
                    return Results.NotFound();
                }

                var duplicateExists = await dbContext.Rooms
                    .AnyAsync(
                        existingRoom => existingRoom.Id != roomId &&
                                        existingRoom.Unit == request.Unit.Trim() &&
                                        existingRoom.Name.ToLower() == request.Name.Trim().ToLower(),
                        cancellationToken);

                if (duplicateExists)
                {
                    return Results.ValidationProblem(new Dictionary<string, string[]>
                    {
                        ["name"] = ["Room name cannot be duplicated within the same unit."]
                    });
                }

                room.Update(
                    request.Name,
                    request.Unit,
                    request.Location,
                    request.Capacity,
                    request.Amenities);

                await dbContext.SaveChangesAsync(cancellationToken);
                return Results.Ok(RoomDetailsDto.From(room));
            });
    }

    public sealed record UpdateRoomRequest(
        string Name,
        string Unit,
        string Location,
        int Capacity,
        string[]? Amenities);

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

    private static Dictionary<string, string[]> Validate(UpdateRoomRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors["name"] = ["Name is required."];
        }

        if (string.IsNullOrWhiteSpace(request.Unit))
        {
            errors["unit"] = ["Unit is required."];
        }

        if (string.IsNullOrWhiteSpace(request.Location))
        {
            errors["location"] = ["Location is required."];
        }

        if (request.Capacity <= 0)
        {
            errors["capacity"] = ["Capacity must be greater than zero."];
        }

        return errors;
    }
}
