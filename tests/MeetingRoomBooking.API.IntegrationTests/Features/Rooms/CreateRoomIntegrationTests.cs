using System.Net;
using System.Net.Http.Json;
using MeetingRoomBooking.API.IntegrationTests.Infrastructure;
using MeetingRoomBooking.Features.Rooms;

namespace MeetingRoomBooking.API.IntegrationTests.Features.Rooms;

public sealed class CreateRoomIntegrationTests : IClassFixture<MeetingRoomBookingWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly RoomRequestGenerator _requestGenerator = new();

    public CreateRoomIntegrationTests(MeetingRoomBookingWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateRoom_ShouldReturnCreatedAndStartActive_WhenRequestIsValid()
    {
        // Arrange
        var request = _requestGenerator.GenerateCreateRoomRequest();

        // Act
        var response = await _client.PostAsJsonAsync("/api/rooms", request);
        var room = await response.Content.ReadFromJsonAsync<CreateRoom.RoomDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        room.Should().NotBeNull();
        room.Should().BeEquivalentTo(request, options => options
            .Including(dto => dto.Name)
            .Including(dto => dto.Unit)
            .Including(dto => dto.Location)
            .Including(dto => dto.Capacity));
        room!.IsActive.Should().BeTrue();
        response.Headers.Location!.ToString().Should().Be($"/api/rooms/{room.Id}");
    }
}
