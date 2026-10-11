namespace ApiProvaMundialSession3.DTO
{
    public class ChamadaDTO
    {
        public int StatusCallId { get; set; }

        public string? CallerName { get; set; }

        public string? CallerNumber { get; set; }

        public string ReportedType { get; set; } = null!;

        public string? Descricao { get; set; }

        public int? LocationId { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime? AnsweredAt { get; set; }

        public DateTime? EndedAt { get; set; }

        public int? LinkedIncidentId { get; set; }
    }
}
