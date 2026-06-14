using System.Net;
using System.Net.Http.Json;
using MeetingRoomBooking.API.IntegrationTests.Infrastructure;
using MeetingRoomBooking.Features.Rooms;

namespace MeetingRoomBooking.API.IntegrationTests.Features.Rooms;

public sealed class UpdateRoomIntegrationTests : IClassFixture<MeetingRoomBookingWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly RoomRequestGenerator _requestGenerator = new();

    public UpdateRoomIntegrationTests(MeetingRoomBookingWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UpdateRoom_ShouldReturnUpdatedRoom_WhenRequestIsValid()
    {
        // Arrange
        var createResponse = await _client.PostAsJsonAsync(
            "/api/rooms",
            _requestGenerator.GenerateCreateRoomRequest());

        var createdRoom = await createResponse.Content.ReadFromJsonAsync<CreateRoom.RoomDto>();
        createdRoom.Should().NotBeNull();

        // Act
        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/rooms/{createdRoom.Id}",
            new UpdateRoom.UpdateRoomRequest("Ocean Room Updated", "Sao Paulo HQ", "Third Floor", 12, ["Projector"]));
        var updatedRoom = await updateResponse.Content.ReadFromJsonAsync<UpdateRoom.RoomDetailsDto>();

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedRoom.Should().NotBeNull();
        updatedRoom!.Name.Should().Be("Ocean Room Updated");
        updatedRoom.Location.Should().Be("Third Floor");
        updatedRoom.Capacity.Should().Be(12);
    }
}
