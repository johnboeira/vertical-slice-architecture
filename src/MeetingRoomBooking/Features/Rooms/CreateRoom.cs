using MeetingRoomBooking.Shared.Domain.Features.Rooms;
using MeetingRoomBooking.Shared.Persistence;
using MeetingRoomBooking.Shared.Slices;
using MeetingRoomBooking.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Features.Rooms;

public sealed class CreateRoom : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost(ApiEndpoints.Rooms.Base,
            async Task<IResult> (CreateRoomRequest request, MeetingRoomBookingDbContext dbContext, CancellationToken cancellationToken) =>
            {
                var validationErrors = Validate(request);
                if (validationErrors.Count > 0)
                {
                    return Results.ValidationProblem(validationErrors);
                }

                var duplicateExists = await dbContext.Rooms
                    .AnyAsync(
                        room => room.Unit == request.Unit.Trim() &&
                                room.Name.ToLower() == request.Name.Trim().ToLower(),
                        cancellationToken);

                if (duplicateExists)
                {
                    return Results.ValidationProblem(new Dictionary<string, string[]>
                    {
                        ["name"] = ["Room name cannot be duplicated within the same unit."]
                    });
                }

                var room = Room.Create(request.Name, request.Unit, request.Location, request.Capacity, request.Amenities);

                dbContext.Rooms.Add(room);
                await dbContext.SaveChangesAsync(cancellationToken);

                return Results.Created(ApiEndpoints.Rooms.GetById(room.Id), RoomDto.From(room));
            });
    }

    public sealed record CreateRoomRequest(
        string Name,
        string Unit,
        string Location,
        int Capacity,
        string[]? Amenities);

    public sealed record RoomDto(
        Guid Id,
        string Name,
        string Unit,
        string Location,
        int Capacity,
        bool IsActive,
        string[] Amenities)
    {
        public static RoomDto From(Room room)
            => new(
                room.Id,
                room.Name,
                room.Unit,
                room.Location,
                room.Capacity,
                room.IsActive,
                room.Amenities.Select(amenity => amenity.ToString()).Order().ToArray());
    }

    private static Dictionary<string, string[]> Validate(CreateRoomRequest request)
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
