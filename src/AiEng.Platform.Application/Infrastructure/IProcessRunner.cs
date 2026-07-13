namespace AiEng.Platform.Application.Infrastructure;

public interface IProcessRunner
{
    IAsyncEnumerable<string> RunAsync(
        string executable,
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken = default);

    Task<ProcessResult> RunToCompletionAsync(
        string executable,
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken = default);
}
