namespace ApiProvaMundialSession3.DTO;

public record CallsDto(DateTimeOffset ServerTime, int RingingCount, List<CallDto> Calls);
