namespace RPP.Core.DTOs
{
    public class ShiftEnriched
    {
        public string? ShiftId { get; init; }
        public string? DepotId { get; init; }
        public string? RoutingArea { get; init; }

        public DateTimeOffset StartTime { get; init; }
        public DateTimeOffset EndTime { get; init; }

        public string? VehicleId { get; init; }
        public IReadOnlyList<string>? Orders { get; init; }

        public int TestNumber { get; init; }
    }
}
