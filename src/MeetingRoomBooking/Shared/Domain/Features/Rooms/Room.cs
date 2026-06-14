namespace MeetingRoomBooking.Shared.Domain.Features.Rooms;

public sealed class Room : Entity
{
    private Room(string name, string unit, string location, int capacity, RoomAmenity[] amenities)
    {
        Name = name;
        Unit = unit;
        Location = location;
        Capacity = capacity;
        IsActive = true;
        Amenities = amenities;
    }

    public string Name { get; private set; }

    public string Unit { get; private set; }

    public string Location { get; private set; }

    public int Capacity { get; private set; }

    public bool IsActive { get; private set; }

    public RoomAmenity[] Amenities { get; private set; }

    public static Room Create(
        string name,
        string unit,
        string location,
        int capacity,
        IEnumerable<string>? amenities)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainRuleViolationException("Room name is required.");
        }

        if (string.IsNullOrWhiteSpace(unit))
        {
            throw new DomainRuleViolationException("Room unit is required.");
        }

        if (string.IsNullOrWhiteSpace(location))
        {
            throw new DomainRuleViolationException("Room location is required.");
        }

        if (capacity <= 0)
        {
            throw new DomainRuleViolationException("Room capacity must be greater than zero.");
        }

        return new Room(
            name.Trim(),
            unit.Trim(),
            location.Trim(),
            capacity,
            ParseAmenities(amenities));
    }

    public void Update(
        string name,
        string unit,
        string location,
        int capacity,
        IEnumerable<string>? amenities)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainRuleViolationException("Room name is required.");
        }

        if (string.IsNullOrWhiteSpace(unit))
        {
            throw new DomainRuleViolationException("Room unit is required.");
        }

        if (string.IsNullOrWhiteSpace(location))
        {
            throw new DomainRuleViolationException("Room location is required.");
        }

        if (capacity <= 0)
        {
            throw new DomainRuleViolationException("Room capacity must be greater than zero.");
        }

        Name = name.Trim();
        Unit = unit.Trim();
        Location = location.Trim();
        Capacity = capacity;
        Amenities = ParseAmenities(amenities);
    }

    public void Activate()
    {
        if (IsActive)
        {
            throw new DomainRuleViolationException("Only inactive rooms can be activated.");
        }

        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            throw new DomainRuleViolationException("Only active rooms can be deactivated.");
        }

        IsActive = false;
    }

    private static RoomAmenity[] ParseAmenities(IEnumerable<string>? amenities)
        => (amenities ?? [])
            .Where(amenity => !string.IsNullOrWhiteSpace(amenity))
            .Select(ParseAmenity)
            .Distinct()
            .ToArray();

    private static RoomAmenity ParseAmenity(string amenity)
        => amenity.Trim().ToLowerInvariant() switch
        {
            "tv" or "television" => RoomAmenity.Television,
            "projector" => RoomAmenity.Projector,
            "whiteboard" or "white-board" => RoomAmenity.Whiteboard,
            "videoconference" or "video-conference" or "video_conference" => RoomAmenity.VideoConference,
            "coffee" => RoomAmenity.Coffee,
            _ => throw new DomainRuleViolationException($"Unsupported amenity '{amenity}'.")
        };
}

public enum RoomAmenity
{
    Television,
    Projector,
    Whiteboard,
    VideoConference,
    Coffee
}
