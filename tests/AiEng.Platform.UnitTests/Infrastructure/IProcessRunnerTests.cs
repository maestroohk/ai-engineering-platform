using AiEng.Platform.Application.Infrastructure;
using AiEng.Platform.Infrastructure.ProcessRunner;
using Xunit;

namespace AiEng.Platform.UnitTests.Infrastructure;

public class IProcessRunnerTests
{
    private readonly IProcessRunner _runner = new SystemProcessRunner();

    [Fact]
    public async Task RunToCompletionAsync_returns_zero_exit_code_for_successful_command()
    {
        var (executable, args) = SuccessfulCommand("echo", "hello world");

        var result = await _runner.RunToCompletionAsync(executable, args);

        Assert.True(result.Succeeded);
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("hello world", result.StandardOutput, StringComparison.Ordinal);
    }

    [Fact]
    public async Task RunToCompletionAsync_returns_nonzero_exit_code_for_failing_command()
    {
        var (executable, args) = FailingCommand();

        var result = await _runner.RunToCompletionAsync(executable, args);

        Assert.False(result.Succeeded);
        Assert.NotEqual(0, result.ExitCode);
    }

    [Fact]
    public async Task RunToCompletionAsync_captures_standard_error_for_failing_command()
    {
        var (executable, args) = FailingCommandWithStderr();

        var result = await _runner.RunToCompletionAsync(executable, args);

        Assert.False(result.Succeeded);
        Assert.Contains("err-line", result.StandardError, StringComparison.Ordinal);
    }

    [Fact]
    public async Task RunToCompletionAsync_captures_empty_arguments_without_throwing()
    {
        var (executable, args) = SuccessfulCommand("cmd", "/c", "exit", "0");

        var result = await _runner.RunToCompletionAsync(executable, args);

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public async Task RunToCompletionAsync_throws_when_executable_path_is_null()
    {
        await Assert.ThrowsAnyAsync<ArgumentException>(
            async () => await _runner.RunToCompletionAsync(null!, Array.Empty<string>()));
    }

    [Fact]
    public async Task RunToCompletionAsync_throws_when_executable_path_is_whitespace()
    {
        await Assert.ThrowsAnyAsync<ArgumentException>(
            async () => await _runner.RunToCompletionAsync("   ", Array.Empty<string>()));
    }

    [Fact]
    public async Task RunToCompletionAsync_throws_when_arguments_is_null()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _runner.RunToCompletionAsync("cmd", null!));
    }

    [Fact]
    public async Task RunAsync_streams_lines_from_successful_command()
    {
        var (executable, args) = MultiLineCommand("alpha", "bravo", "charlie");

        var lines = new List<string>();
        await foreach (var line in _runner.RunAsync(executable, args))
        {
            lines.Add(line);
        }

        Assert.Contains("alpha", lines);
        Assert.Contains("bravo", lines);
        Assert.Contains("charlie", lines);
    }

    [Fact]
    public async Task RunToCompletionAsync_propagates_specific_exit_code()
    {
        var (executable, args) = SuccessfulCommand("cmd", "/c", "exit", "42");

        var result = await _runner.RunToCompletionAsync(executable, args);

        Assert.Equal(42, result.ExitCode);
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task RunToCompletionAsync_throws_when_executable_does_not_exist()
    {
        var missing = OperatingSystem.IsWindows()
            ? @"C:\does-not-exist\__aieng_no_such_program__.exe"
            : "/does-not-exist/__aieng_no_such_program__";

        await Assert.ThrowsAnyAsync<Exception>(
            async () => await _runner.RunToCompletionAsync(missing, Array.Empty<string>()));
    }

    [Fact]
    public async Task RunToCompletionAsync_cancellation_propagates_when_token_cancelled()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await _runner.RunToCompletionAsync("cmd", new[] { "/c", "ping", "127.0.0.1", "-n", "30" }, cts.Token));
    }

    private static (string Executable, IReadOnlyList<string> Args) SuccessfulCommand(params string[] pieces)
    {
        if (OperatingSystem.IsWindows())
        {
            var args = new List<string> { "/c" };
            args.AddRange(pieces);
            return ("cmd", args);
        }
        return ("/bin/sh", new[] { "-c", string.Join(" ", pieces) });
    }

    private static (string Executable, IReadOnlyList<string> Args) FailingCommand()
    {
        if (OperatingSystem.IsWindows())
        {
            return ("cmd", new[] { "/c", "exit", "1" });
        }
        return ("/bin/sh", new[] { "-c", "exit 1" });
    }

    private static (string Executable, IReadOnlyList<string> Args) FailingCommandWithStderr()
    {
        if (OperatingSystem.IsWindows())
        {
            return ("cmd", new[] { "/c", "echo err-line 1>&2 & exit 2" });
        }
        return ("/bin/sh", new[] { "-c", "echo err-line 1>&2; exit 2" });
    }

    private static (string Executable, IReadOnlyList<string> Args) MultiLineCommand(params string[] lines)
    {
        if (OperatingSystem.IsWindows())
        {
            var script = string.Join("&", lines.Select(static l => $"echo {l}"));
            return ("cmd", new[] { "/c", script });
        }
        var shScript = string.Join("; ", lines.Select(static l => $"echo {l}"));
        return ("/bin/sh", new[] { "-c", shScript });
    }
}
