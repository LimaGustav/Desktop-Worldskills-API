namespace ApiProvaMundialSession3.DTO;

public record RecommendationDto(
    string Subtype,
    List<CompositionPart> Composition,
    List<string> SuggestedUnitIds
);
