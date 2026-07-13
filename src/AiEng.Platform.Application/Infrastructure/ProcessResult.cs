namespace AiEng.Platform.Application.Infrastructure;

public readonly record struct ProcessResult(int ExitCode, string StandardOutput, string StandardError)
{
    public bool Succeeded => ExitCode == 0;

    public static ProcessResult From(int exitCode, string standardOutput, string standardError) =>
        new(exitCode, standardOutput ?? string.Empty, standardError ?? string.Empty);
}
