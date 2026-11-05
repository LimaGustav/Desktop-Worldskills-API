namespace ApiProvaMundialSession3.DTO;

public record UnitDto(
    string Id,
    string StationId,
    string Type,
    string CallSign,
    string Status,
    Coordinate Position,
    string? AssignedIncidentId
);
