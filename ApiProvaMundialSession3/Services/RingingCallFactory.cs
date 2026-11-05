using System.Security.Cryptography;
using ApiProvaMundialSession3.Models;

namespace ApiProvaMundialSession3.Services;

public static class RingingCallFactory
{
    private sealed record CallScenario(
        string Name,
        string Number,
        string ReportedType,
        string Description,
        decimal X,
        decimal Y
    );

    private static readonly CallScenario[] Scenarios =
    [
        new("Ana Costa", "11987654321", "fire", "Fumaca em um edificio comercial", 300, 400),
        new("Bruno Lima", "11991234567", "medical", "Pessoa desacordada na rua", 850, 620),
        new("Carla Souza", "11998887766", "crime", "Assalto em uma loja", 500, 350),
        new("Diego Alves", "11995554433", "fire", "Incendio em um veiculo", 800, 600),
        new("Elisa Martins", "11994443322", "medical", "Pessoa com dificuldade para respirar", 450, 300),
        new("Felipe Rocha", "11993332211", "crime", "Invasao de residencia", 150, 220),
        new("Gabriela Nunes", "11992221100", "fire", "Cheiro de gas em um predio", 110, 210),
        new("Henrique Dias", "11990001122", "medical", "Acidente de transito com feridos", 100, 200),
    ];

    public static Chamadum Create(int ringingStatusId)
    {
        var scenario = Scenarios[RandomNumberGenerator.GetInt32(Scenarios.Length)];
        return new Chamadum
        {
            StatusCallId = ringingStatusId,
            CallerName = scenario.Name,
            CallerNumber = scenario.Number,
            ReportedType = scenario.ReportedType,
            Descricao = scenario.Description,
            Location = new Cordenadum { X = scenario.X, Y = scenario.Y },
            StartedAt = DateTime.UtcNow,
            AnsweredAt = null,
            EndedAt = null,
            LinkedIncidentId = null,
        };
    }
}
