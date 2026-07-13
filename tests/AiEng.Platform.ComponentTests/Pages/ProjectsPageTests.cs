using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AiEng.Platform.App.Components.Pages;
using AiEng.Platform.Application.Infrastructure;
using AiEng.Platform.Application.Navigation;
using AiEng.Platform.Application.Projects;
using AiEng.Platform.Domain.Projects;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ProjectsPage = AiEng.Platform.App.Components.Pages.Projects;

namespace AiEng.Platform.ComponentTests.Pages;

public class ProjectsPageTests : BunitContext
{
    public ProjectsPageTests()
    {
        Services.AddSingleton<INavigationRegistry>(new RegistryWithRoute());
        Services.AddSingleton<IProjectService>(new StaticService(new List<Project>()));
        Services.AddSingleton<IPlatformInfo>(new FakePlatformInfo(isWindows: true));
        Services.AddSingleton<IProcessRunner>(new FakeProcessRunner());
        JSInterop.Setup<string>("appTheme.current").SetResult("light");
        JSInterop.SetupVoid("appTheme.set", _ => true);
    }

    [Fact]
    public void Renders_Page_Header_With_Title_And_Description()
    {
        var cut = Render<ProjectsPage>();

        cut.WaitForState(() => cut.FindAll("h1.app-page-header-title").Count > 0);

        var heading = cut.Find("h1.app-page-header-title");
        Assert.Contains("Projects", heading.TextContent);
        Assert.Contains("Registered project folders", cut.Markup);
    }

    [Fact]
    public void Renders_Empty_State_When_No_Projects_Are_Registered()
    {
        var cut = Render<ProjectsPage>();

        cut.WaitForState(() => cut.FindAll(".app-empty-state").Count > 0);

        Assert.Contains("No projects yet", cut.Markup);
    }

    [Fact]
    public void Renders_App_Project_List_Slot_With_Populated_State_When_Projects_Are_Registered()
    {
        var dir = Path.Combine(Path.GetTempPath(), "aieng-page-pop-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            Services.AddSingleton<IProjectService>(new StaticService(new List<Project>
            {
                new(Guid.NewGuid(), "alpha", dir, new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero)),
            }));
            var cut = Render<ProjectsPage>();

            cut.WaitForState(() => cut.FindAll(".app-project-card").Count == 1);

            Assert.Contains("data-state=\"populated\"", cut.Markup);
            Assert.Contains("alpha", cut.Markup);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public void Register_Button_Is_Enabled_In_M3_2()
    {
        var cut = Render<ProjectsPage>();

        cut.WaitForState(() => cut.FindAll("header.app-page-header").Count > 0);

        var button = cut.Find("[data-testid='register-project']");
        Assert.False(button.HasAttribute("disabled"));
    }

    [Fact]
    public void Clicking_Register_Button_Opens_The_Registration_Modal()
    {
        var cut = Render<ProjectsPage>();

        cut.WaitForState(() => cut.FindAll("header.app-page-header").Count > 0);

        Assert.DoesNotContain("data-testid=\"register-project-modal\"", cut.Markup);

        cut.Find("[data-testid='register-project']").Click();

        cut.WaitForState(() => cut.Markup.Contains("data-testid=\"register-project-modal\""));

        Assert.Contains("data-testid=\"register-project-modal\"", cut.Markup);
    }

    private sealed class StaticService : IProjectService
    {
        private readonly List<Project> _projects;

        public StaticService(IReadOnlyList<Project> projects) => _projects = new List<Project>(projects);

        public Task<Result<Project>> RegisterAsync(string name, string path, CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(path))
            {
                return Task.FromResult(Result<Project>.Failure(ValidationError.InvalidPath("path", path)));
            }
            var project = new Project(Guid.NewGuid(), name.Trim(), path, DateTimeOffset.UtcNow);
            _projects.Add(project);
            return Task.FromResult(Result<Project>.Success(project));
        }

        public Task<IReadOnlyList<Project>> ListAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult((IReadOnlyList<Project>)_projects.ToArray());

        public Task<Project?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_projects.FirstOrDefault(p => p.Id == id));

        public Task<Result<Project>> RenameAsync(Guid id, string newName, CancellationToken cancellationToken = default)
        {
            var project = _projects.FirstOrDefault(p => p.Id == id);
            if (project is null)
            {
                return Task.FromResult(Result<Project>.Failure(ValidationError.NotFound("Project", id)));
            }
            project.Rename(newName);
            return Task.FromResult(Result<Project>.Success(project));
        }

        public Task<Result<Project>> UnregisterAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var project = _projects.FirstOrDefault(p => p.Id == id);
            if (project is null)
            {
                return Task.FromResult(Result<Project>.Failure(ValidationError.NotFound("Project", id)));
            }
            _projects.Remove(project);
            return Task.FromResult(Result<Project>.Success(project));
        }
    }

    private sealed class RegistryWithRoute : INavigationRegistry
    {
        public IReadOnlyList<RouteMetadata> Routes { get; } = new[]
        {
            new RouteMetadata(
                Href: "/projects",
                Title: "Projects",
                Order: 1,
                Description: "Registered projects.",
                Icon: "▢",
                Parent: null,
                BadgeText: null,
                ShowInSidebar: true,
                MatchPrefix: false),
        };

        public RouteMetadata? FindByHref(string href) => Routes.FirstOrDefault(r => string.Equals(r.Href, href, StringComparison.OrdinalIgnoreCase));

        public IReadOnlyList<RouteMetadata> ChildrenOf(string? parentHref) => Array.Empty<RouteMetadata>();
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
