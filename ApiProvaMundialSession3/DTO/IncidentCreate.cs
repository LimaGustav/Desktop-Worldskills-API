namespace ApiProvaMundialSession3.DTO;

public record IncidentCreate(
    string? Type,
    Coordinate? Location,
    string? Subtype,
    string? Severity,
    string? Description,
    string? CallId
);
