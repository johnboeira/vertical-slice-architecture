using System.Net;
using System.Net.Http.Json;
using MeetingRoomBooking.API.IntegrationTests.Infrastructure;
using MeetingRoomBooking.Features.Rooms;

namespace MeetingRoomBooking.API.IntegrationTests.Features.Rooms;

public sealed class GetRoomIntegrationTests : IClassFixture<MeetingRoomBookingWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly RoomRequestGenerator _requestGenerator = new();

    public GetRoomIntegrationTests(MeetingRoomBookingWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetRoom_ShouldReturnRoomDetails_WhenRoomExists()
    {
        // Arrange
        var request = _requestGenerator.GenerateCreateRoomRequest();
        var createResponse = await _client.PostAsJsonAsync("/api/rooms", request);

        var createdRoom = await createResponse.Content.ReadFromJsonAsync<CreateRoom.RoomDto>();
        createdRoom.Should().NotBeNull();

        // Act
        var getResponse = await _client.GetAsync($"/api/rooms/{createdRoom.Id}");
        var room = await getResponse.Content.ReadFromJsonAsync<GetRoom.RoomDetailsDto>();

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        room.Should().NotBeNull();
        room!.Id.Should().Be(createdRoom!.Id);
        room.Name.Should().Be(request.Name);
        room.Location.Should().Be(request.Location);
    }
}
