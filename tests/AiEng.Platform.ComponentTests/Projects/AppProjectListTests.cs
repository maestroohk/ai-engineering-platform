using System.Collections.Generic;
using AiEng.Platform.App.Components.Projects;
using AiEng.Platform.Application.Projects;
using AiEng.Platform.Domain.Projects;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Projects;

public class AppProjectListTests : BunitContext
{
    public AppProjectListTests()
    {
        Services.AddSingleton<IProjectService>(new StaticService(new List<Project>(), afterLoadDelay: TimeSpan.Zero));
    }

    [Fact]
    public void Loading_State_Renders_AppLoading_When_List_Is_Async()
    {
        var projects = new List<Project>();
        Services.AddSingleton<IProjectService>(new StaticService(projects, afterLoadDelay: TimeSpan.FromMilliseconds(150)));
        var cut = Render<AppProjectList>();

        Assert.Contains("app-project-list-slot", cut.Markup);
        Assert.Contains("Loading projects", cut.Markup);
    }

    [Fact]
    public void Empty_State_Renders_When_No_Projects_Are_Registered()
    {
        var cut = Render<AppProjectList>();

        cut.WaitForState(() => cut.FindAll(".app-empty-state").Count > 0);

        Assert.Contains("No projects yet", cut.Markup);
        Assert.Contains("data-state=\"empty\"", cut.Markup);
    }

    [Fact]
    public void Populated_State_Renders_AppProjectCard_Per_Project()
    {
        var projects = new List<Project>
        {
            new(Guid.NewGuid(), "alpha", "/tmp/alpha", new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero)),
            new(Guid.NewGuid(), "bravo", "/tmp/bravo", new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero)),
        };
        Services.AddSingleton<IProjectService>(new StaticService(projects, afterLoadDelay: TimeSpan.Zero));
        var cut = Render<AppProjectList>();

        cut.WaitForState(() => cut.FindAll(".app-project-card").Count == 2);

        var cards = cut.FindAll(".app-project-card");
        Assert.Equal(2, cards.Count);
        Assert.Contains("alpha", cut.Markup);
        Assert.Contains("bravo", cut.Markup);
        Assert.Contains("data-state=\"populated\"", cut.Markup);
    }

    [Fact]
    public void Error_State_Renders_When_Service_Throws()
    {
        Services.AddSingleton<IProjectService>(new ThrowingService("boom"));
        var cut = Render<AppProjectList>();

        cut.WaitForState(() => cut.Markup.Contains("Cannot load projects"));

        Assert.Contains("Cannot load projects", cut.Markup);
        Assert.Contains("data-state=\"error\"", cut.Markup);
        Assert.Contains("m3.load_failed", cut.Markup);
    }

    private sealed class StaticService : IProjectService
    {
        private readonly IReadOnlyList<Project> _projects;
        private readonly TimeSpan _delay;

        public StaticService(IReadOnlyList<Project> projects, TimeSpan afterLoadDelay)
        {
            _projects = projects;
            _delay = afterLoadDelay;
        }

        public Task<Result<Project>> RegisterAsync(string name, string path, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException("M3.1 ships the list surface; register lands in M3.2.");

        public Task<IReadOnlyList<Project>> ListAsync(CancellationToken cancellationToken = default)
        {
            if (_delay > TimeSpan.Zero)
            {
                return Task.Run(async () =>
                {
                    await Task.Delay(_delay, cancellationToken);
                    return _projects;
                }, cancellationToken);
            }
            return Task.FromResult(_projects);
        }

        public Task<Project?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_projects.FirstOrDefault(p => p.Id == id));

        public Task<Result<Project>> RenameAsync(Guid id, string newName, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException("Rename lands in M3.2.");

        public Task<Result<Project>> UnregisterAsync(Guid id, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException("Unregister lands in M3.2.");
    }

    private sealed class ThrowingService : IProjectService
    {
        private readonly string _message;

        public ThrowingService(string message) => _message = message;

        public Task<Result<Project>> RegisterAsync(string name, string path, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<Project>> ListAsync(CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException(_message);

        public Task<Project?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Result<Project>> RenameAsync(Guid id, string newName, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Result<Project>> UnregisterAsync(Guid id, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }
}
