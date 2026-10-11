namespace ApiProvaMundialSession3.DTO
{
    public class IncidenteDTOPatch
    {

        public int StatusId { get; set; }

        public string? Descricao { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
