using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AiEng.Platform.App.Components.Projects;
using AiEng.Platform.Application.Infrastructure;
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
        Services.AddSingleton<IPlatformInfo>(new FakePlatformInfo(isWindows: true));
        Services.AddSingleton<IProcessRunner>(new FakeProcessRunner());
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

    [Fact]
    public void ShowRegisterDialog_Opens_Register_Modal()
    {
        var cut = Render<AppProjectList>();

        cut.WaitForState(() => cut.FindAll(".app-empty-state").Count > 0);

        Assert.DoesNotContain("data-testid=\"register-project-modal\"", cut.Markup);

        cut.InvokeAsync(() => cut.Instance.ShowRegisterDialog());

        cut.WaitForState(() => cut.Markup.Contains("data-testid=\"register-project-modal\""));

        Assert.Contains("data-testid=\"register-project-modal\"", cut.Markup);
    }

    [Fact]
    public async Task RefreshAsync_Loads_Newly_Added_Project()
    {
        var service = (StaticService)Services.GetRequiredService<IProjectService>();
        var cut = Render<AppProjectList>();

        cut.WaitForState(() => cut.FindAll(".app-empty-state").Count > 0);

        var dir = Path.Combine(Path.GetTempPath(), "aieng-list-refresh-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            var newProject = new Project(Guid.NewGuid(), "delta", dir, DateTimeOffset.UtcNow);
            service.Projects.Add(newProject);

            await cut.InvokeAsync(() => cut.Instance.RefreshAsync());

            cut.WaitForState(() => cut.FindAll(".app-project-card").Count == 1);

            Assert.Contains("delta", cut.Markup);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public async Task RefreshAsync_Removes_Deleted_Project()
    {
        var dir = Path.Combine(Path.GetTempPath(), "aieng-list-remove-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            var removed = new Project(Guid.NewGuid(), "alpha", dir, DateTimeOffset.UtcNow);
            var kept = new Project(Guid.NewGuid(), "bravo", dir, DateTimeOffset.UtcNow);
            Services.AddSingleton<IProjectService>(new StaticService(new List<Project> { removed, kept }, afterLoadDelay: TimeSpan.Zero));
            var cut = Render<AppProjectList>();

            cut.WaitForState(() => cut.FindAll(".app-project-card").Count == 2);

            var refreshed = (StaticService)Services.GetRequiredService<IProjectService>();
            refreshed.Projects.Remove(removed);

            await cut.InvokeAsync(() => cut.Instance.RefreshAsync());

            cut.WaitForState(() => cut.FindAll(".app-project-card").Count == 1);

            Assert.DoesNotContain("alpha", cut.Markup);
            Assert.Contains("bravo", cut.Markup);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public async Task Clicking_Rename_On_A_Card_Opens_The_Rename_Modal()
    {
        var dir = Path.Combine(Path.GetTempPath(), "aieng-list-rename-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            var project = new Project(Guid.NewGuid(), "alpha", dir, DateTimeOffset.UtcNow);
            Services.AddSingleton<IProjectService>(new StaticService(new List<Project> { project }, afterLoadDelay: TimeSpan.Zero));
            var cut = Render<AppProjectList>();

            cut.WaitForState(() => cut.FindAll(".app-project-card").Count == 1);

            Assert.DoesNotContain("data-testid=\"rename-project-modal\"", cut.Markup);

            cut.Find("[data-testid='rename-project']").Click();

            cut.WaitForState(() => cut.Markup.Contains("data-testid=\"rename-project-modal\""));

            Assert.Contains("data-testid=\"rename-project-modal\"", cut.Markup);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public async Task Clicking_Unregister_On_A_Card_Opens_The_Unregister_Modal()
    {
        var dir = Path.Combine(Path.GetTempPath(), "aieng-list-unregister-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            var project = new Project(Guid.NewGuid(), "alpha", dir, DateTimeOffset.UtcNow);
            Services.AddSingleton<IProjectService>(new StaticService(new List<Project> { project }, afterLoadDelay: TimeSpan.Zero));
            var cut = Render<AppProjectList>();

            cut.WaitForState(() => cut.FindAll(".app-project-card").Count == 1);

            Assert.DoesNotContain("data-testid=\"unregister-project-modal\"", cut.Markup);

            cut.Find("[data-testid='unregister-project']").Click();

            cut.WaitForState(() => cut.Markup.Contains("data-testid=\"unregister-project-modal\""));

            Assert.Contains("data-testid=\"unregister-project-modal\"", cut.Markup);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    private sealed class StaticService : IProjectService
    {
        public StaticService(IReadOnlyList<Project> projects, TimeSpan afterLoadDelay)
        {
            Projects = new List<Project>(projects);
            _delay = afterLoadDelay;
        }

        public List<Project> Projects { get; }

        private readonly TimeSpan _delay;

        public Task<Result<Project>> RegisterAsync(string name, string path, CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(path))
            {
                return Task.FromResult(Result<Project>.Failure(ValidationError.InvalidPath("path", path)));
            }
            var project = new Project(Guid.NewGuid(), name.Trim(), path, DateTimeOffset.UtcNow);
            Projects.Add(project);
            return Task.FromResult(Result<Project>.Success(project));
        }

        public Task<IReadOnlyList<Project>> ListAsync(CancellationToken cancellationToken = default)
        {
            if (_delay > TimeSpan.Zero)
            {
                return Task.Run(async () =>
                {
                    await Task.Delay(_delay, cancellationToken);
                    return (IReadOnlyList<Project>)Projects.ToArray();
                }, cancellationToken);
            }
            return Task.FromResult((IReadOnlyList<Project>)Projects.ToArray());
        }

        public Task<Project?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Projects.FirstOrDefault(p => p.Id == id));

        public Task<Result<Project>> RenameAsync(Guid id, string newName, CancellationToken cancellationToken = default)
        {
            var project = Projects.FirstOrDefault(p => p.Id == id);
            if (project is null)
            {
                return Task.FromResult(Result<Project>.Failure(ValidationError.NotFound("Project", id)));
            }
            project.Rename(newName);
            return Task.FromResult(Result<Project>.Success(project));
        }

        public Task<Result<Project>> UnregisterAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var project = Projects.FirstOrDefault(p => p.Id == id);
            if (project is null)
            {
                return Task.FromResult(Result<Project>.Failure(ValidationError.NotFound("Project", id)));
            }
            Projects.Remove(project);
            return Task.FromResult(Result<Project>.Success(project));
        }
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

    private sealed class FakeProcessRunner : IProcessRunner
    {
        public async IAsyncEnumerable<string> RunAsync(
            string executable,
            IReadOnlyList<string> arguments,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            yield break;
        }

        public Task<ProcessResult> RunToCompletionAsync(
            string executable,
            IReadOnlyList<string> arguments,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(ProcessResult.From(0, string.Empty, string.Empty));
    }

    private sealed class FakePlatformInfo : IPlatformInfo
    {
        public FakePlatformInfo(bool isWindows) => IsWindows = isWindows;

        public bool IsWindows { get; }

        public string GetDataDirectory() => Path.Combine(Path.GetTempPath(), "aieng-fake-data");

        public string GetConfigDirectory() => Path.Combine(Path.GetTempPath(), "aieng-fake-config");
    }
}
