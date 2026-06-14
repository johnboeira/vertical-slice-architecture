using System.Net;
using System.Net.Http.Json;
using MeetingRoomBooking.API.IntegrationTests.Infrastructure;
using MeetingRoomBooking.Features.Rooms;

namespace MeetingRoomBooking.API.IntegrationTests.Features.Rooms;

public sealed class ActivateRoomIntegrationTests : IClassFixture<MeetingRoomBookingWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly RoomRequestGenerator _requestGenerator = new();

    public ActivateRoomIntegrationTests(MeetingRoomBookingWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ActivateRoom_ShouldReturnActiveRoom_WhenRoomIsInactive()
    {
        // Arrange
        var createResponse = await _client.PostAsJsonAsync("/api/rooms", _requestGenerator.GenerateCreateRoomRequest());

        var createdRoom = await createResponse.Content.ReadFromJsonAsync<CreateRoom.RoomDto>();
        createdRoom.Should().NotBeNull();

        await _client.PostAsync($"/api/rooms/{createdRoom.Id}/deactivate", content: null);

        // Act
        var activateResponse = await _client.PostAsync($"/api/rooms/{createdRoom.Id}/activate", content: null);
        var roomStatus = await activateResponse.Content.ReadFromJsonAsync<ActivateRoom.RoomStatusDto>();

        // Assert
        activateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        roomStatus.Should().NotBeNull();
        roomStatus!.IsActive.Should().BeTrue();
    }
}
