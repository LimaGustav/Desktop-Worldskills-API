namespace ApiProvaMundialSession3.DTO;

public record CallDto(
    string Id,
    string Status,
    string? CallerName,
    string? CallerNumber,
    string ReportedType,
    string? Description,
    Coordinate? Location,
    DateTimeOffset StartedAt,
    DateTimeOffset? AnsweredAt,
    DateTimeOffset? EndedAt,
    string? LinkedIncidentId
);
