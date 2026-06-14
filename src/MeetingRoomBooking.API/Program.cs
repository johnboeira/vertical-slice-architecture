using MeetingRoomBooking;
using MeetingRoomBooking.Shared.Exceptions;
using MeetingRoomBooking.Shared.Persistence;
using MeetingRoomBooking.Shared.Slices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddMeetingRoomBooking(builder.Environment);

var app = builder.Build();

await app.Services.InitializeDatabaseAsync();

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapSliceEndpoints();

app.Run();

public partial class Program;
