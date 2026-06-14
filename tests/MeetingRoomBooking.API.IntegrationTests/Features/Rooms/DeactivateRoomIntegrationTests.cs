using System.Net;
using System.Net.Http.Json;
using MeetingRoomBooking.API.IntegrationTests.Infrastructure;
using MeetingRoomBooking.Features.Rooms;

namespace MeetingRoomBooking.API.IntegrationTests.Features.Rooms;

public sealed class DeactivateRoomIntegrationTests : IClassFixture<MeetingRoomBookingWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly RoomRequestGenerator _requestGenerator = new();

    public DeactivateRoomIntegrationTests(MeetingRoomBookingWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task DeactivateRoom_ShouldReturnInactiveRoom_WhenRoomIsActive()
    {
        // Arrange
        var createResponse = await _client.PostAsJsonAsync("/api/rooms", _requestGenerator.GenerateCreateRoomRequest());

        var createdRoom = await createResponse.Content.ReadFromJsonAsync<CreateRoom.RoomDto>();
        createdRoom.Should().NotBeNull();

        // Act
        var deactivateResponse = await _client.PostAsync($"/api/rooms/{createdRoom.Id}/deactivate", content: null);
        var roomStatus = await deactivateResponse.Content.ReadFromJsonAsync<DeactivateRoom.RoomStatusDto>();

        // Assert
        deactivateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        roomStatus.Should().NotBeNull();
        roomStatus!.IsActive.Should().BeFalse();
    }
}
