using AiEng.Platform.Application.Capabilities;
using AiEng.Platform.Application.Infrastructure;
using AiEng.Platform.Infrastructure.Capabilities;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AiEng.Platform.UnitTests.Capabilities;

public class SystemHostCapabilitiesServiceTests
{
    [Fact]
    public void Constructor_throws_ArgumentNullException_when_processRunner_is_null()
    {
        Assert.Throws<ArgumentNullException>(() => new SystemHostCapabilitiesService(
            null!,
            new FakeCredentialVault(),
            new FakePlatformInfo(),
            NullLogger<SystemHostCapabilitiesService>.Instance));
    }

    [Fact]
    public void Constructor_throws_ArgumentNullException_when_credentialVault_is_null()
    {
        Assert.Throws<ArgumentNullException>(() => new SystemHostCapabilitiesService(
            new FakeProcessRunner(),
            null!,
            new FakePlatformInfo(),
            NullLogger<SystemHostCapabilitiesService>.Instance));
    }

    [Fact]
    public void Constructor_throws_ArgumentNullException_when_platformInfo_is_null()
    {
        Assert.Throws<ArgumentNullException>(() => new SystemHostCapabilitiesService(
            new FakeProcessRunner(),
            new FakeCredentialVault(),
            null!,
            NullLogger<SystemHostCapabilitiesService>.Instance));
    }

    [Fact]
    public void Constructor_throws_ArgumentNullException_when_logger_is_null()
    {
        Assert.Throws<ArgumentNullException>(() => new SystemHostCapabilitiesService(
            new FakeProcessRunner(),
            new FakeCredentialVault(),
            new FakePlatformInfo(),
            null!));
    }

    [Fact]
    public async Task DetectAsync_returns_all_six_host_tools_with_Available_true_when_each_returns_exit_zero()
    {
        var runner = new FakeProcessRunner();
        runner.QueueByExecutable("git", new ProcessResult(0, "git version 2.47.1.windows.1\n", ""));
        runner.QueueByExecutable("ollama", new ProcessResult(0, "ollama version 0.5.7\n", ""));
        runner.QueueByExecutable("powershell.exe", new ProcessResult(0, "5.1.22621.4391\n", ""));
        runner.QueueByExecutable("wsl.exe", new ProcessResult(0, "WSL version: 2.3.24.0\n", ""));
        runner.QueueByExecutable("wt.exe", new ProcessResult(0, "Windows Terminal v1.22.10351.0\n", ""));
        runner.QueueByExecutable("bash.exe", new ProcessResult(0, "GNU bash, version 5.2.37(1)-release (x86_64-pc-linux-gnu)\n", ""));
        var vault = new FakeCredentialVault();
        var platform = new FakePlatformInfo { IsWindows = true };
        var sut = new SystemHostCapabilitiesService(runner, vault, platform, NullLogger<SystemHostCapabilitiesService>.Instance);

        var result = await sut.DetectAsync();

        Assert.Equal(12, result.Capabilities.Count);
        foreach (var tool in result.Capabilities.Take(6))
        {
            Assert.True(tool.Available, $"{tool.Key} should be available");
            Assert.False(tool.CredentialAvailable);
        }
    }

    [Fact]
    public async Task DetectAsync_returns_Available_false_for_host_tool_with_nonzero_exit_code()
    {
        var runner = new FakeProcessRunner();
        runner.QueueByExecutable("git", new ProcessResult(0, "git version 2.47.1\n", ""));
        runner.QueueByExecutable("ollama", new ProcessResult(127, "", "command not found"));
        runner.QueueByExecutable("powershell.exe", new ProcessResult(0, "5.1.22621.4391\n", ""));
        runner.QueueByExecutable("wsl.exe", new ProcessResult(0, "WSL version: 2.3.24.0\n", ""));
        runner.QueueByExecutable("wt.exe", new ProcessResult(0, "Windows Terminal v1.22.10351.0\n", ""));
        runner.QueueByExecutable("bash.exe", new ProcessResult(0, "GNU bash, version 5.2.37(1)-release\n", ""));
        var sut = new SystemHostCapabilitiesService(runner, new FakeCredentialVault(), new FakePlatformInfo { IsWindows = true }, NullLogger<SystemHostCapabilitiesService>.Instance);

        var result = await sut.DetectAsync();

        var ollama = result.Capabilities.First(c => c.Key == "ollama");
        Assert.False(ollama.Available);
        Assert.Null(ollama.Version);
    }

    [Fact]
    public async Task DetectAsync_returns_Available_false_for_host_tool_that_throws_exception()
    {
        var runner = new FakeProcessRunner();
        runner.QueueByExecutable("git", new ProcessResult(0, "git version 2.47.1\n", ""));
        runner.QueueByExecutable("ollama", new ProcessResult(0, "ollama version 0.5.7\n", ""));
        runner.QueueByExecutable("powershell.exe", new ProcessResult(0, "5.1.22621.4391\n", ""));
        runner.QueueByExecutable("wsl.exe", new ProcessResult(0, "WSL version: 2.3.24.0\n", ""));
        runner.QueueByExecutable("wt.exe", new ProcessResult(0, "Windows Terminal v1.22.10351.0\n", ""));
        runner.QueueByExecutable("bash.exe", new ProcessResult(0, "GNU bash, version 5.2.37(1)-release\n", ""));
        runner.QueueExceptionForExecutable("wt.exe", new InvalidOperationException("simulated failure"));
        var sut = new SystemHostCapabilitiesService(runner, new FakeCredentialVault(), new FakePlatformInfo { IsWindows = true }, NullLogger<SystemHostCapabilitiesService>.Instance);

        var result = await sut.DetectAsync();

        var wt = result.Capabilities.First(c => c.Key == "wt");
        Assert.False(wt.Available);
    }

    [Fact]
    public async Task DetectAsync_returns_Available_false_for_host_tool_that_times_out()
    {
        var runner = new FakeProcessRunner();
        runner.QueueDelayForExecutable("wsl.exe", TimeSpan.FromSeconds(10));
        runner.QueueByExecutable("git", new ProcessResult(0, "git version 2.47.1\n", ""));
        runner.QueueByExecutable("ollama", new ProcessResult(0, "ollama version 0.5.7\n", ""));
        runner.QueueByExecutable("powershell.exe", new ProcessResult(0, "5.1.22621.4391\n", ""));
        runner.QueueByExecutable("wt.exe", new ProcessResult(0, "Windows Terminal v1.22.10351.0\n", ""));
        runner.QueueByExecutable("bash.exe", new ProcessResult(0, "GNU bash, version 5.2.37(1)-release\n", ""));
        var sut = new SystemHostCapabilitiesService(runner, new FakeCredentialVault(), new FakePlatformInfo { IsWindows = true }, NullLogger<SystemHostCapabilitiesService>.Instance);

        var result = await sut.DetectAsync();

        var wsl = result.Capabilities.First(c => c.Key == "wsl");
        Assert.False(wsl.Available);
        Assert.Null(wsl.Version);
    }

    [Fact]
    public async Task DetectAsync_on_non_Windows_host_returns_Available_false_for_windows_only_tools()
    {
        var runner = new FakeProcessRunner();
        var sut = new SystemHostCapabilitiesService(runner, new FakeCredentialVault(), new FakePlatformInfo { IsWindows = false }, NullLogger<SystemHostCapabilitiesService>.Instance);

        var result = await sut.DetectAsync();

        foreach (var key in new[] { "powershell", "wsl", "wt" })
        {
            var tool = result.Capabilities.First(c => c.Key == key);
            Assert.False(tool.Available, $"{key} should be unavailable on non-Windows");
            Assert.Null(tool.Version);
        }
    }

    [Fact]
    public async Task DetectAsync_on_non_Windows_host_still_probes_cross_platform_tools()
    {
        var runner = new FakeProcessRunner();
        runner.QueueByExecutable("git", new ProcessResult(0, "git version 2.47.1\n", ""));
        runner.QueueByExecutable("ollama", new ProcessResult(0, "ollama version 0.5.7\n", ""));
        runner.QueueByExecutable("bash.exe", new ProcessResult(0, "GNU bash, version 5.2.37(1)-release\n", ""));
        var sut = new SystemHostCapabilitiesService(runner, new FakeCredentialVault(), new FakePlatformInfo { IsWindows = false }, NullLogger<SystemHostCapabilitiesService>.Instance);

        var result = await sut.DetectAsync();

        Assert.Equal(3, runner.CallCount);
        foreach (var key in new[] { "git", "ollama", "bash" })
        {
            var tool = result.Capabilities.First(c => c.Key == key);
            Assert.True(tool.Available, $"{key} should be available on non-Windows");
        }
    }

    [Fact]
    public async Task DetectAsync_returns_CredentialAvailable_true_when_credential_vault_returns_a_value()
    {
        var vault = new FakeCredentialVault();
        vault.Set("provider:git:token", "secret-token");
        var sut = new SystemHostCapabilitiesService(new FakeProcessRunner(), vault, new FakePlatformInfo(), NullLogger<SystemHostCapabilitiesService>.Instance);

        var result = await sut.DetectAsync();

        var credential = result.Capabilities.First(c => c.Key == "provider:git");
        Assert.True(credential.CredentialAvailable);
        Assert.Equal("provider:git:token", credential.CredentialName);
        Assert.False(credential.Available);
    }

    [Fact]
    public async Task DetectAsync_returns_CredentialAvailable_false_when_credential_vault_returns_null()
    {
        var vault = new FakeCredentialVault();
        vault.Set("provider:git:token", "secret");
        var sut = new SystemHostCapabilitiesService(new FakeProcessRunner(), vault, new FakePlatformInfo(), NullLogger<SystemHostCapabilitiesService>.Instance);

        var result = await sut.DetectAsync();

        var credential = result.Capabilities.First(c => c.Key == "provider:ollama");
        Assert.False(credential.CredentialAvailable);
        Assert.Equal("provider:ollama:token", credential.CredentialName);
    }

    [Fact]
    public async Task DetectAsync_parses_git_version_from_standard_output()
    {
        var runner = new FakeProcessRunner();
        runner.QueueByExecutable("git", new ProcessResult(0, "git version 2.47.1.windows.1\n", ""));
        var sut = new SystemHostCapabilitiesService(runner, new FakeCredentialVault(), new FakePlatformInfo(), NullLogger<SystemHostCapabilitiesService>.Instance);

        var result = await sut.DetectAsync();

        var git = result.Capabilities.First(c => c.Key == "git");
        Assert.True(git.Available);
        Assert.Equal("2.47.1.windows.1", git.Version);
    }

    [Fact]
    public async Task DetectAsync_returns_Version_null_when_version_pattern_does_not_match()
    {
        var runner = new FakeProcessRunner();
        runner.QueueByExecutable("git", new ProcessResult(0, "completely unrelated output\n", ""));
        var sut = new SystemHostCapabilitiesService(runner, new FakeCredentialVault(), new FakePlatformInfo(), NullLogger<SystemHostCapabilitiesService>.Instance);

        var result = await sut.DetectAsync();

        var git = result.Capabilities.First(c => c.Key == "git");
        Assert.True(git.Available);
        Assert.Null(git.Version);
    }

    [Fact]
    public async Task DetectAsync_sets_DetectedAt_to_the_TimeProvider_value()
    {
        var fixedNow = new DateTimeOffset(2026, 7, 13, 12, 0, 0, TimeSpan.Zero);
        var clock = new FixedTimeProvider(fixedNow);
        var sut = new SystemHostCapabilitiesService(
            new FakeProcessRunner(),
            new FakeCredentialVault(),
            new FakePlatformInfo(),
            NullLogger<SystemHostCapabilitiesService>.Instance,
            clock);

        var result = await sut.DetectAsync();

        Assert.Equal(fixedNow, result.DetectedAt);
    }

    [Fact]
    public async Task DetectAsync_sets_DetectedAt_to_a_timestamp_within_the_call_window_when_using_default_clock()
    {
        var sut = new SystemHostCapabilitiesService(
            new FakeProcessRunner(),
            new FakeCredentialVault(),
            new FakePlatformInfo(),
            NullLogger<SystemHostCapabilitiesService>.Instance);

        var before = DateTimeOffset.UtcNow;
        var result = await sut.DetectAsync();
        var after = DateTimeOffset.UtcNow;

        Assert.InRange(result.DetectedAt.UtcDateTime, before.UtcDateTime.AddSeconds(-1), after.UtcDateTime.AddSeconds(1));
    }

    [Fact]
    public async Task DetectAsync_returns_capabilities_in_deterministic_order()
    {
        var vault = new FakeCredentialVault();
        vault.Set("provider:bash:token", "x");
        var sut = new SystemHostCapabilitiesService(
            new FakeProcessRunner(),
            vault,
            new FakePlatformInfo(),
            NullLogger<SystemHostCapabilitiesService>.Instance);

        var result = await sut.DetectAsync();

        var keys = result.Capabilities.Select(c => c.Key).ToArray();
        Assert.Equal(new[] { "git", "ollama", "powershell", "wsl", "wt", "bash", "provider:git", "provider:ollama", "provider:powershell", "provider:wsl", "provider:wt", "provider:bash" }, keys);
    }

    [Fact]
    public async Task DetectAsync_throws_OperationCanceledException_when_cancellation_token_is_cancelled_before_call()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var sut = new SystemHostCapabilitiesService(new FakeProcessRunner(), new FakeCredentialVault(), new FakePlatformInfo(), NullLogger<SystemHostCapabilitiesService>.Instance);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => sut.DetectAsync(cts.Token));
    }

    [Fact]
    public async Task DetectAsync_propagates_cancellation_when_cancellation_token_is_cancelled_during_call()
    {
        var runner = new FakeProcessRunner();
        runner.QueueDelayForExecutable("git", TimeSpan.FromSeconds(30));
        using var cts = new CancellationTokenSource();
        var sut = new SystemHostCapabilitiesService(runner, new FakeCredentialVault(), new FakePlatformInfo(), NullLogger<SystemHostCapabilitiesService>.Instance);

        var detectTask = sut.DetectAsync(cts.Token);
        cts.CancelAfter(TimeSpan.FromMilliseconds(100));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => detectTask);
    }

    [Fact]
    public async Task DetectAsync_swallows_timeout_without_propagating_when_cancellation_token_is_not_cancelled()
    {
        var runner = new FakeProcessRunner();
        runner.QueueByExecutable("wsl.exe", new ProcessResult(0, "WSL version: 2.3.24.0\n", ""));
        runner.QueueByExecutable("git", new ProcessResult(0, "git version 2.47.1\n", ""));
        runner.QueueByExecutable("ollama", new ProcessResult(0, "ollama version 0.5.7\n", ""));
        runner.QueueByExecutable("powershell.exe", new ProcessResult(0, "5.1.22621.4391\n", ""));
        runner.QueueByExecutable("wt.exe", new ProcessResult(0, "Windows Terminal v1.22.10351.0\n", ""));
        runner.QueueByExecutable("bash.exe", new ProcessResult(0, "GNU bash, version 5.2.37(1)-release\n", ""));
        runner.QueueDelayForExecutable("wsl.exe", TimeSpan.FromSeconds(10));
        var sut = new SystemHostCapabilitiesService(runner, new FakeCredentialVault(), new FakePlatformInfo { IsWindows = true }, NullLogger<SystemHostCapabilitiesService>.Instance);

        var result = await sut.DetectAsync();

        Assert.Equal(12, result.Capabilities.Count);
        var wsl = result.Capabilities.First(c => c.Key == "wsl");
        Assert.False(wsl.Available);
    }

    private sealed class FakeProcessRunner : IProcessRunner
    {
        private readonly Dictionary<string, Queue<ProcessResult>> _queue = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Exception> _exceptions = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, TimeSpan> _delays = new(StringComparer.OrdinalIgnoreCase);

        public int CallCount { get; private set; }

        public List<(string Executable, IReadOnlyList<string> Arguments)> Calls { get; } = new();

        public void QueueByExecutable(string executable, ProcessResult result)
        {
            if (!_queue.TryGetValue(executable, out var q))
            {
                q = new Queue<ProcessResult>();
                _queue[executable] = q;
            }
            q.Enqueue(result);
        }

        public void QueueExceptionForExecutable(string executable, Exception ex)
            => _exceptions[executable] = ex;

        public void QueueDelayForExecutable(string executable, TimeSpan delay)
            => _delays[executable] = delay;

        public Task<ProcessResult> RunToCompletionAsync(
            string executable,
            IReadOnlyList<string> arguments,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            Calls.Add((executable, arguments));

            if (_exceptions.TryGetValue(executable, out var ex))
            {
                throw ex;
            }

            if (_delays.TryGetValue(executable, out var delay))
            {
                return DelayedResultAsync(delay, cancellationToken);
            }

            if (_queue.TryGetValue(executable, out var q) && q.Count > 0)
            {
                return Task.FromResult(q.Dequeue());
            }

            return Task.FromResult(new ProcessResult(0, string.Empty, string.Empty));
        }

        public async IAsyncEnumerable<string> RunAsync(
            string executable,
            IReadOnlyList<string> arguments,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            yield break;
        }

        private static async Task<ProcessResult> DelayedResultAsync(TimeSpan delay, CancellationToken cancellationToken)
        {
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            return new ProcessResult(0, string.Empty, string.Empty);
        }
    }

    private sealed class FakeCredentialVault : ICredentialVault
    {
        private readonly Dictionary<string, string> _store = new(StringComparer.Ordinal);

        public void Set(string name, string secret) => _store[name] = secret;

        public Task<string?> GetAsync(string name, CancellationToken cancellationToken = default)
            => Task.FromResult(_store.TryGetValue(name, out var v) ? v : null);

        public Task SetAsync(string name, string secret, CancellationToken cancellationToken = default)
        {
            _store[name] = secret;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string name, CancellationToken cancellationToken = default)
        {
            _store.Remove(name);
            return Task.CompletedTask;
        }
    }

    private sealed class FakePlatformInfo : IPlatformInfo
    {
        public bool IsWindows { get; set; } = true;

        public string GetDataDirectory() => Path.Combine(Path.GetTempPath(), "aieng", "data");

        public string GetConfigDirectory() => Path.Combine(Path.GetTempPath(), "aieng", "config");
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _now;

        public FixedTimeProvider(DateTimeOffset now) => _now = now;

        public override DateTimeOffset GetUtcNow() => _now;
    }
}
