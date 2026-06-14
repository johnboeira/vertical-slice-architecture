using System.Net;
using System.Net.Http.Json;
using MeetingRoomBooking.API.IntegrationTests.Infrastructure;
using MeetingRoomBooking.Features.Rooms;

namespace MeetingRoomBooking.API.IntegrationTests.Features.Rooms;

public sealed class SearchRoomsIntegrationTests : IClassFixture<MeetingRoomBookingWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SearchRoomsIntegrationTests(MeetingRoomBookingWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task SearchRooms_ShouldReturnMatchingRooms_WhenFiltersAreProvided()
    {
        // Arrange
        var createResponse = await _client.PostAsJsonAsync(
            "/api/rooms",
            new CreateRoom.CreateRoomRequest("Ocean Room", "Sao Paulo HQ", "Second Floor", 10, ["Television", "Coffee"]));

        var createdRoom = await createResponse.Content.ReadFromJsonAsync<CreateRoom.RoomDto>();
        createdRoom.Should().NotBeNull();

        // Act
        var searchResponse = await _client.GetAsync("/api/rooms?location=Second%20Floor&capacity=8&amenities=Television&isActive=true");
        var rooms = await searchResponse.Content.ReadFromJsonAsync<SearchRooms.RoomListItemDto[]>();

        // Assert
        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        rooms.Should().NotBeNull();
        rooms.Should().ContainSingle();
        rooms![0].Id.Should().Be(createdRoom!.Id);
    }
}
