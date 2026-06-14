namespace MeetingRoomBooking.Shared.Domain;

public sealed class DomainRuleViolationException(string message) : Exception(message);
