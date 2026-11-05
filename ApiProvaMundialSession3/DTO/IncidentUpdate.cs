namespace ApiProvaMundialSession3.DTO;

public record IncidentUpdate(
    string? Type,
    string? Subtype,
    string? Severity,
    Coordinate? Location,
    string? Description,
    List<string>? AssignedUnitIds
);
