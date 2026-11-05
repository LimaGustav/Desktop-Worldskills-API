using System.Globalization;
using System.Security.Cryptography;

namespace ApiProvaMundialSession3.Services;

public sealed class CallProbabilityService(IHostEnvironment environment)
{
    private readonly SemaphoreSlim gate = new(1, 1);
    private readonly string statePath = Path.Combine(
        environment.ContentRootPath,
        "temp",
        "call-probability.txt"
    );

    public static int ChanceFor(int failedAttempts) =>
        (int)Math.Min(100L, 20L + 10L * Math.Max(0, failedAttempts));

    public async Task<bool> AttemptAsync(
        Func<CancellationToken, Task> createCall,
        CancellationToken ct = default
    )
    {
        await gate.WaitAsync(ct);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(statePath)!);
            var failedAttempts = await ReadFailedAttemptsAsync(ct);
            var generated = RandomNumberGenerator.GetInt32(100) < ChanceFor(failedAttempts);

            if (generated)
                await createCall(ct);

            await SaveFailedAttemptsAsync(generated ? 0 : Math.Min(failedAttempts, 7) + 1, ct);
            return generated;
        }
        finally
        {
            gate.Release();
        }
    }

    private async Task<int> ReadFailedAttemptsAsync(CancellationToken ct)
    {
        if (!File.Exists(statePath))
            return 0;

        var contents = await File.ReadAllTextAsync(statePath, ct);
        if (!int.TryParse(contents, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            || value < 0)
            throw new InvalidDataException("O contador de probabilidade de chamadas é inválido.");

        return value;
    }

    private async Task SaveFailedAttemptsAsync(int value, CancellationToken ct)
    {
        var temporaryPath = statePath + ".tmp";
        await File.WriteAllTextAsync(
            temporaryPath,
            value.ToString(CultureInfo.InvariantCulture),
            ct
        );
        File.Move(temporaryPath, statePath, overwrite: true);
    }
}
