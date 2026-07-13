using AiEng.Platform.Application.Capabilities;
using AiEng.Platform.Application.Providers;
using AiEng.Platform.Infrastructure.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AiEng.Platform.UnitTests.Providers;

public class SystemProviderRegistryTests
{
    [Fact]
    public void Constructor_throws_ArgumentNullException_when_gitFamily_is_null()
    {
        Assert.Throws<ArgumentNullException>(() => new SystemProviderRegistry(
            null!,
            new FakeAgentRuntimeProviderFamily(),
            new FakeReviewProviderFamily(),
            new FakeQualityGateProviderFamily(),
            new FakeAutonomousLoopProviderFamily(),
            new FakeOrchestrationProviderFamily(),
            new FakeHostCapabilitiesService(),
            NullLogger<SystemProviderRegistry>.Instance));
    }

    [Fact]
    public void Constructor_throws_ArgumentNullException_when_agentRuntimeFamily_is_null()
    {
        Assert.Throws<ArgumentNullException>(() => new SystemProviderRegistry(
            new FakeGitProviderFamily(),
            null!,
            new FakeReviewProviderFamily(),
            new FakeQualityGateProviderFamily(),
            new FakeAutonomousLoopProviderFamily(),
            new FakeOrchestrationProviderFamily(),
            new FakeHostCapabilitiesService(),
            NullLogger<SystemProviderRegistry>.Instance));
    }

    [Fact]
    public void Constructor_throws_ArgumentNullException_when_reviewFamily_is_null()
    {
        Assert.Throws<ArgumentNullException>(() => new SystemProviderRegistry(
            new FakeGitProviderFamily(),
            new FakeAgentRuntimeProviderFamily(),
            null!,
            new FakeQualityGateProviderFamily(),
            new FakeAutonomousLoopProviderFamily(),
            new FakeOrchestrationProviderFamily(),
            new FakeHostCapabilitiesService(),
            NullLogger<SystemProviderRegistry>.Instance));
    }

    [Fact]
    public void Constructor_throws_ArgumentNullException_when_qualityGateFamily_is_null()
    {
        Assert.Throws<ArgumentNullException>(() => new SystemProviderRegistry(
            new FakeGitProviderFamily(),
            new FakeAgentRuntimeProviderFamily(),
            new FakeReviewProviderFamily(),
            null!,
            new FakeAutonomousLoopProviderFamily(),
            new FakeOrchestrationProviderFamily(),
            new FakeHostCapabilitiesService(),
            NullLogger<SystemProviderRegistry>.Instance));
    }

    [Fact]
    public void Constructor_throws_ArgumentNullException_when_autonomousLoopFamily_is_null()
    {
        Assert.Throws<ArgumentNullException>(() => new SystemProviderRegistry(
            new FakeGitProviderFamily(),
            new FakeAgentRuntimeProviderFamily(),
            new FakeReviewProviderFamily(),
            new FakeQualityGateProviderFamily(),
            null!,
            new FakeOrchestrationProviderFamily(),
            new FakeHostCapabilitiesService(),
            NullLogger<SystemProviderRegistry>.Instance));
    }

    [Fact]
    public void Constructor_throws_ArgumentNullException_when_orchestrationFamily_is_null()
    {
        Assert.Throws<ArgumentNullException>(() => new SystemProviderRegistry(
            new FakeGitProviderFamily(),
            new FakeAgentRuntimeProviderFamily(),
            new FakeReviewProviderFamily(),
            new FakeQualityGateProviderFamily(),
            new FakeAutonomousLoopProviderFamily(),
            null!,
            new FakeHostCapabilitiesService(),
            NullLogger<SystemProviderRegistry>.Instance));
    }

    [Fact]
    public void Constructor_throws_ArgumentNullException_when_capabilities_is_null()
    {
        Assert.Throws<ArgumentNullException>(() => new SystemProviderRegistry(
            new FakeGitProviderFamily(),
            new FakeAgentRuntimeProviderFamily(),
            new FakeReviewProviderFamily(),
            new FakeQualityGateProviderFamily(),
            new FakeAutonomousLoopProviderFamily(),
            new FakeOrchestrationProviderFamily(),
            null!,
            NullLogger<SystemProviderRegistry>.Instance));
    }

    [Fact]
    public void Constructor_throws_ArgumentNullException_when_logger_is_null()
    {
        Assert.Throws<ArgumentNullException>(() => new SystemProviderRegistry(
            new FakeGitProviderFamily(),
            new FakeAgentRuntimeProviderFamily(),
            new FakeReviewProviderFamily(),
            new FakeQualityGateProviderFamily(),
            new FakeAutonomousLoopProviderFamily(),
            new FakeOrchestrationProviderFamily(),
            new FakeHostCapabilitiesService(),
            null!));
    }

    [Fact]
    public async Task ListProvidersAsync_returns_descriptors_unchanged_when_family_capability_is_available()
    {
        var git = new FakeGitProviderFamily();
        git.QueueResult(new[]
        {
            new ProviderDescriptor(
                Id: "git-cli",
                DisplayName: "Git CLI",
                Family: ProviderFamily.Git,
                Status: ProviderStatus.Available,
                Version: "2.47.1",
                Metadata: new Dictionary<string, string>()),
        });
        var capabilities = new FakeHostCapabilitiesService();
        capabilities.SetCapabilities(new[]
        {
            new HostCapability(Key: "git", Available: true, Version: "2.47.1", CredentialAvailable: false, CredentialName: null),
        });
        var sut = BuildSut(git, capabilities, NullLogger<SystemProviderRegistry>.Instance);

        var result = await sut.ListProvidersAsync(ProviderFamily.Git);

        Assert.Single(result);
        Assert.Equal("git-cli", result[0].Id);
        Assert.Equal(ProviderStatus.Available, result[0].Status);
    }

    [Fact]
    public async Task ListProvidersAsync_downgrades_descriptors_to_Unavailable_when_family_capability_is_not_available()
    {
        var git = new FakeGitProviderFamily();
        git.QueueResult(new[]
        {
            new ProviderDescriptor(
                Id: "git-cli",
                DisplayName: "Git CLI",
                Family: ProviderFamily.Git,
                Status: ProviderStatus.Available,
                Version: "2.47.1",
                Metadata: new Dictionary<string, string>()),
        });
        var capabilities = new FakeHostCapabilitiesService();
        capabilities.SetCapabilities(Array.Empty<HostCapability>());
        var sut = BuildSut(git, capabilities, NullLogger<SystemProviderRegistry>.Instance);

        var result = await sut.ListProvidersAsync(ProviderFamily.Git);

        Assert.Single(result);
        Assert.Equal(ProviderStatus.Unavailable, result[0].Status);
    }

    [Fact]
    public async Task ListProvidersAsync_preserves_Disabled_descriptors_regardless_of_capability()
    {
        var git = new FakeGitProviderFamily();
        git.QueueResult(new[]
        {
            new ProviderDescriptor(
                Id: "git-cli",
                DisplayName: "Git CLI",
                Family: ProviderFamily.Git,
                Status: ProviderStatus.Disabled,
                Version: null,
                Metadata: new Dictionary<string, string>()),
        });
        var capabilities = new FakeHostCapabilitiesService();
        capabilities.SetCapabilities(new[]
        {
            new HostCapability(Key: "git", Available: true, Version: "2.47.1", CredentialAvailable: false, CredentialName: null),
        });
        var sut = BuildSut(git, capabilities, NullLogger<SystemProviderRegistry>.Instance);

        var result = await sut.ListProvidersAsync(ProviderFamily.Git);

        Assert.Single(result);
        Assert.Equal(ProviderStatus.Disabled, result[0].Status);
    }

    [Fact]
    public async Task ListProvidersAsync_returns_empty_for_family_with_no_descriptors()
    {
        var git = new FakeGitProviderFamily();
        var capabilities = new FakeHostCapabilitiesService();
        capabilities.SetCapabilities(new[]
        {
            new HostCapability(Key: "git", Available: true, Version: "2.47.1", CredentialAvailable: false, CredentialName: null),
        });
        var sut = BuildSut(git, capabilities, NullLogger<SystemProviderRegistry>.Instance);

        var result = await sut.ListProvidersAsync(ProviderFamily.Git);

        Assert.Empty(result);
    }

    [Fact]
    public async Task ListProvidersAsync_propagates_cancellation_when_token_is_cancelled_before_call()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var sut = BuildSut(
            new FakeGitProviderFamily(),
            new FakeHostCapabilitiesService(),
            NullLogger<SystemProviderRegistry>.Instance);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => sut.ListProvidersAsync(ProviderFamily.Git, cts.Token));
    }

    [Fact]
    public async Task ListProvidersAsync_logs_at_Information_level_on_success()
    {
        var logger = new ListLogger<SystemProviderRegistry>();
        var capabilities = new FakeHostCapabilitiesService();
        capabilities.SetCapabilities(new[]
        {
            new HostCapability(Key: "git", Available: true, Version: "2.47.1", CredentialAvailable: false, CredentialName: null),
        });
        var sut = BuildSut(new FakeGitProviderFamily(), capabilities, logger);

        await sut.ListProvidersAsync(ProviderFamily.Git);

        Assert.NotEmpty(logger.Records);
        Assert.Equal(LogLevel.Information, logger.Records[0].Level);
    }

    [Fact]
    public async Task ListProvidersAsync_calls_IGitProviderFamily_when_family_is_Git()
    {
        var git = new FakeGitProviderFamily();
        var sut = BuildSut(git, new FakeHostCapabilitiesService(), NullLogger<SystemProviderRegistry>.Instance);

        await sut.ListProvidersAsync(ProviderFamily.Git);

        Assert.Equal(1, git.CallCount);
    }

    [Fact]
    public async Task ListProvidersAsync_calls_IReviewProviderFamily_when_family_is_Review()
    {
        var review = new FakeReviewProviderFamily();
        var sut = BuildSut(
            git: new FakeGitProviderFamily(),
            agentRuntime: new FakeAgentRuntimeProviderFamily(),
            review: review,
            qualityGate: new FakeQualityGateProviderFamily(),
            autonomousLoop: new FakeAutonomousLoopProviderFamily(),
            orchestration: new FakeOrchestrationProviderFamily(),
            capabilities: new FakeHostCapabilitiesService(),
            logger: NullLogger<SystemProviderRegistry>.Instance);

        await sut.ListProvidersAsync(ProviderFamily.Review);

        Assert.Equal(1, review.CallCount);
    }

    [Fact]
    public async Task ListProvidersAsync_calls_IHostCapabilitiesService_once_per_invocation()
    {
        var capabilities = new FakeHostCapabilitiesService();
        capabilities.SetCapabilities(new[]
        {
            new HostCapability(Key: "git", Available: true, Version: "2.47.1", CredentialAvailable: false, CredentialName: null),
        });
        var sut = BuildSut(new FakeGitProviderFamily(), capabilities, NullLogger<SystemProviderRegistry>.Instance);

        await sut.ListProvidersAsync(ProviderFamily.Git);

        Assert.Equal(1, capabilities.CallCount);
    }

    [Fact]
    public async Task ListProvidersAsync_throws_ArgumentOutOfRangeException_for_unknown_family()
    {
        var sut = BuildSut(new FakeGitProviderFamily(), new FakeHostCapabilitiesService(), NullLogger<SystemProviderRegistry>.Instance);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => sut.ListProvidersAsync((ProviderFamily)999));
    }

    [Fact]
    public void FakeGitProviderFamily_records_CallCount()
    {
        var git = new FakeGitProviderFamily();

        git.QueueResult(Array.Empty<ProviderDescriptor>());
        _ = git.ListProvidersAsync();
        _ = git.ListProvidersAsync();

        Assert.Equal(2, git.CallCount);
    }

    private static SystemProviderRegistry BuildSut(
        FakeGitProviderFamily git,
        FakeHostCapabilitiesService capabilities,
        ILogger<SystemProviderRegistry> logger)
        => BuildSut(git, new FakeAgentRuntimeProviderFamily(), new FakeReviewProviderFamily(),
            new FakeQualityGateProviderFamily(), new FakeAutonomousLoopProviderFamily(),
            new FakeOrchestrationProviderFamily(), capabilities, logger);

    private static SystemProviderRegistry BuildSut(
        FakeGitProviderFamily git,
        FakeAgentRuntimeProviderFamily agentRuntime,
        FakeReviewProviderFamily review,
        FakeQualityGateProviderFamily qualityGate,
        FakeAutonomousLoopProviderFamily autonomousLoop,
        FakeOrchestrationProviderFamily orchestration,
        FakeHostCapabilitiesService capabilities,
        ILogger<SystemProviderRegistry> logger)
        => new SystemProviderRegistry(git, agentRuntime, review, qualityGate, autonomousLoop, orchestration, capabilities, logger);

    private sealed class FakeHostCapabilitiesService : IHostCapabilitiesService
    {
        private IReadOnlyList<HostCapability> _capabilities = Array.Empty<HostCapability>();

        public int CallCount { get; private set; }

        public void SetCapabilities(IReadOnlyList<HostCapability> capabilities)
            => _capabilities = capabilities;

        public Task<HostCapabilities> DetectAsync(CancellationToken cancellationToken = default)
        {
            CallCount++;
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(new HostCapabilities(_capabilities, DateTimeOffset.UtcNow));
        }
    }

    private sealed class ListLogger<T> : ILogger<T>
    {
        public List<(LogLevel Level, string Message, Exception? Exception)> Records { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Records.Add((logLevel, formatter(state, exception), exception));
        }
    }
}
