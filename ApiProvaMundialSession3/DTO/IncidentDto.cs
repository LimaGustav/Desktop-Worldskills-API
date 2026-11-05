namespace ApiProvaMundialSession3.DTO;

public record IncidentDto(
    string Id,
    string Type,
    string Subtype,
    string Severity,
    string Status,
    DateTimeOffset ReportedAt,
    Coordinate Location,
    string? Description,
    List<string> AssignedUnitIds,
    List<string> AttendingUnitIds,
    string? CallId,
    DateTimeOffset? ResolvedAt
);
