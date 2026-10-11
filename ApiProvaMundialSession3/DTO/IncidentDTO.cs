namespace ApiProvaMundialSession3.DTO
{
    public class IncidentDTO
    {
        public int TypeId { get; set; }

        public int SubtypeId { get; set; }

        public int SeverityId { get; set; }
        public int StatusId { get; set; }

        public DateTime ReportedAt { get; set; }

        public int LocationId { get; set; }

        public string? Descricao { get; set; }

        public int? CallId { get; set; }

        public DateTime? ResolvedAt { get; set; }
    }
}
