using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using AiEng.Platform.Application.Infrastructure;

namespace AiEng.Platform.Infrastructure.ProcessRunner;

public sealed class SystemProcessRunner : IProcessRunner
{
    public async IAsyncEnumerable<string> RunAsync(
        string executable,
        IReadOnlyList<string> arguments,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(executable))
        {
            throw new ArgumentException("Executable path is required.", nameof(executable));
        }
        if (arguments is null)
        {
            throw new ArgumentNullException(nameof(arguments));
        }

        var (fileName, args) = ResolveExecutableAndArgs(executable, arguments);

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        foreach (var arg in args)
        {
            process.StartInfo.ArgumentList.Add(arg);
        }

        if (!process.Start())
        {
            throw new InvalidOperationException(
                $"Failed to start process '{fileName}'.");
        }

        var outputTask = ReadLinesAsync(process.StandardOutput, cancellationToken);
        var errorTask = ReadLinesAsync(process.StandardError, cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        var lines = new List<string>();
        lines.AddRange(await outputTask);
        lines.AddRange(await errorTask);

        foreach (var line in lines)
        {
            yield return line;
        }
    }

    public async Task<ProcessResult> RunToCompletionAsync(
        string executable,
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(executable))
        {
            throw new ArgumentException("Executable path is required.", nameof(executable));
        }
        if (arguments is null)
        {
            throw new ArgumentNullException(nameof(arguments));
        }

        var (fileName, args) = ResolveExecutableAndArgs(executable, arguments);

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        foreach (var arg in args)
        {
            process.StartInfo.ArgumentList.Add(arg);
        }

        if (!process.Start())
        {
            throw new InvalidOperationException(
                $"Failed to start process '{fileName}'.");
        }

        var stdOutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stdErrTask = process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        var stdOut = await stdOutTask;
        var stdErr = await stdErrTask;
        return ProcessResult.From(process.ExitCode, stdOut, stdErr);
    }

    private static async Task<List<string>> ReadLinesAsync(TextReader reader, CancellationToken cancellationToken)
    {
        var lines = new List<string>();
        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            lines.Add(line);
        }
        return lines;
    }

    private static (string FileName, IReadOnlyList<string> Args) ResolveExecutableAndArgs(
        string executable,
        IReadOnlyList<string> arguments)
    {
        if (executable.Contains('"') || executable.Contains(' '))
        {
            var embedded = new StringBuilder();
            embedded.Append('"').Append(executable).Append('"');
            return (embedded.ToString(), arguments);
        }
        return (executable, arguments);
    }
}
