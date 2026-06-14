using MeetingRoomBooking.Shared.Domain.Features.Rooms;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Shared.Persistence;

public sealed class MeetingRoomBookingDbContext(DbContextOptions<MeetingRoomBookingDbContext> options)
    : DbContext(options)
{
    public DbSet<Room> Rooms => Set<Room>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeetingRoomBookingDbContext).Assembly);
    }
}
