using Bogus;
using MeetingRoomBooking.Features.Rooms;

namespace MeetingRoomBooking.API.IntegrationTests.Infrastructure;

public sealed class RoomRequestGenerator
{
    private readonly Faker<CreateRoom.CreateRoomRequest> _createRoomRequestFaker = new Faker<CreateRoom.CreateRoomRequest>()
        .CustomInstantiator(faker => new CreateRoom.CreateRoomRequest(
            Name: $"Room {faker.Random.AlphaNumeric(8)}",
            Unit: $"Unit {faker.Random.AlphaNumeric(5)}",
            Location: faker.Address.City(),
            Capacity: faker.Random.Int(2, 30),
            Amenities: ["Television", "Coffee"]));

    public CreateRoom.CreateRoomRequest GenerateCreateRoomRequest()
        => _createRoomRequestFaker.Generate();
}
