namespace ApiProvaMundialSession3.DTO;

public record StateDto(
    DateTimeOffset ServerTime,
    List<IncidentDto> Incidents,
    List<StationDto> Stations,
    List<UnitDto> Units
);
