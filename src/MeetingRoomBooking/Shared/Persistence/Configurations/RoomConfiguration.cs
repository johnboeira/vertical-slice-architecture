using MeetingRoomBooking.Shared.Domain.Features.Rooms;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingRoomBooking.Shared.Persistence.Configurations;

public sealed class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        var amenitiesComparer = new ValueComparer<RoomAmenity[]>(
            (left, right) => (left ?? Array.Empty<RoomAmenity>()).SequenceEqual(right ?? Array.Empty<RoomAmenity>()),
            amenities => (amenities ?? Array.Empty<RoomAmenity>())
                .Aggregate(0, (current, amenity) => HashCode.Combine(current, amenity.GetHashCode())),
            amenities => (amenities ?? Array.Empty<RoomAmenity>()).ToArray());

        builder.HasKey(room => room.Id);
        builder.Property(room => room.Name).IsRequired().HasMaxLength(200);
        builder.Property(room => room.Unit).IsRequired().HasMaxLength(200);
        builder.Property(room => room.Location).IsRequired().HasMaxLength(200);
        builder.Property(room => room.IsActive).IsRequired();
        builder.Property(room => room.Capacity).IsRequired();
        builder.Property(room => room.Amenities)
            .HasConversion(
                amenities => string.Join(',', amenities.Select(amenity => amenity.ToString())),
                value => string.IsNullOrWhiteSpace(value)
                    ? Array.Empty<RoomAmenity>()
                    : value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(static amenity => Enum.Parse<RoomAmenity>(amenity))
                        .ToArray())
            .Metadata.SetValueComparer(amenitiesComparer);

        builder.HasIndex(room => new { room.Unit, room.Name });
    }
}
