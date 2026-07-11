using System.Collections.Generic;
using AiEng.Platform.App.Components.Pages;
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
        Services.AddSingleton<IProjectService>(new StaticService(new List<Project>
        {
            new(Guid.NewGuid(), "alpha", "/tmp/alpha", new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero)),
        }));
        var cut = Render<ProjectsPage>();

        cut.WaitForState(() => cut.FindAll(".app-project-card").Count == 1);

        Assert.Contains("data-state=\"populated\"", cut.Markup);
        Assert.Contains("alpha", cut.Markup);
    }

    [Fact]
    public void Register_Button_Is_Disabled_In_M3_1()
    {
        var cut = Render<ProjectsPage>();

        cut.WaitForState(() => cut.FindAll("header.app-page-header").Count > 0);

        var button = cut.Find(".app-page-header-actions .app-button-primary");
        Assert.True(button.HasAttribute("disabled"));
    }

    private sealed class StaticService : IProjectService
    {
        private readonly IReadOnlyList<Project> _projects;

        public StaticService(IReadOnlyList<Project> projects) => _projects = projects;

        public Task<Result<Project>> RegisterAsync(string name, string path, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException("M3.1 ships the list surface; register lands in M3.2.");

        public Task<IReadOnlyList<Project>> ListAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(_projects);

        public Task<Project?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_projects.FirstOrDefault(p => p.Id == id));

        public Task<Result<Project>> RenameAsync(Guid id, string newName, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Result<Project>> UnregisterAsync(Guid id, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
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
}
