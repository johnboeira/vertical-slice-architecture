namespace MeetingRoomBooking.Shared;

public static class ApiEndpoints
{
    public static class Rooms
    {
        public const string Base = "/api/rooms";
        public const string ById = $"{Base}/{{roomId:guid}}";
        public const string Activate = $"{ById}/activate";
        public const string Deactivate = $"{ById}/deactivate";

        public static string GetById(Guid roomId) => $"{Base}/{roomId}";

        public static string GetActivate(Guid roomId) => $"{GetById(roomId)}/activate";

        public static string GetDeactivate(Guid roomId) => $"{GetById(roomId)}/deactivate";
    }
}
