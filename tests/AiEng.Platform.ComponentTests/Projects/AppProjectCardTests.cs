using AiEng.Platform.App.Components.Projects;
using AiEng.Platform.Domain.Projects;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Projects;

public class AppProjectCardTests : BunitContext
{
    [Fact]
    public void Renders_Project_Name_And_Path()
    {
        var project = new Project(
            Guid.NewGuid(),
            "alpha",
            "/tmp/alpha",
            new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero));

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        Assert.Contains("alpha", cut.Find(".app-project-card-title").TextContent);
        Assert.Contains("/tmp/alpha", cut.Find(".app-project-card-path").TextContent);
    }

    [Fact]
    public void Renders_New_Badge_When_LastUsedAt_Is_Null()
    {
        var project = new Project(
            Guid.NewGuid(),
            "alpha",
            "/tmp/alpha",
            new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero));

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        Assert.Contains("New", cut.Markup);
        Assert.Contains("app-badge-neutral", cut.Markup);
    }

    [Fact]
    public void Renders_Active_Badge_When_LastUsedAt_Is_Present()
    {
        var project = new Project(
            Guid.NewGuid(),
            "alpha",
            "/tmp/alpha",
            new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero));
        project.Touch(new DateTimeOffset(2026, 7, 11, 9, 0, 0, TimeSpan.Zero));

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        Assert.Contains("Active", cut.Markup);
        Assert.Contains("app-badge-success", cut.Markup);
    }

    [Fact]
    public void Renders_Open_Rename_And_Unregister_Buttons()
    {
        var project = new Project(
            Guid.NewGuid(),
            "alpha",
            "/tmp/alpha",
            new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero));

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        Assert.Contains("Open", cut.Markup);
        Assert.Contains("Rename", cut.Markup);
        Assert.Contains("Unregister", cut.Markup);
        Assert.Contains("app-button-primary", cut.Markup);
        Assert.Contains("app-button-outline", cut.Markup);
        Assert.Contains("app-button-ghost", cut.Markup);
    }

    [Fact]
    public void Actions_Are_Disabled_In_M3_1()
    {
        var project = new Project(
            Guid.NewGuid(),
            "alpha",
            "/tmp/alpha",
            new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero));

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        var buttons = cut.FindAll(".app-project-card-actions .app-button");
        Assert.NotEmpty(buttons);
        Assert.All(buttons, b => Assert.True(b.HasAttribute("disabled"), "All action buttons must be disabled in M3.1."));
    }
}
