namespace ApiProvaMundialSession3.DTO;

public record StationDto(
    string Id,
    string Type,
    string Name,
    Coordinate Location,
    int AvailableUnitCount
);
