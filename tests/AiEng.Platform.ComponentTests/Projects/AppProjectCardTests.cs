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
        var project = NewProject("alpha", "/tmp/alpha");

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        Assert.Contains("alpha", cut.Find(".app-project-card-title").TextContent);
        Assert.Contains("/tmp/alpha", cut.Find(".app-project-card-path").TextContent);
    }

    [Fact]
    public void Renders_New_Badge_When_LastUsedAt_Is_Null()
    {
        var project = NewProject("alpha", "/tmp/alpha");

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        Assert.Contains("New", cut.Markup);
        Assert.Contains("app-badge-neutral", cut.Markup);
    }

    [Fact]
    public void Renders_Active_Badge_When_LastUsedAt_Is_Present()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        project.Touch(new DateTimeOffset(2026, 7, 11, 9, 0, 0, TimeSpan.Zero));

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        Assert.Contains("Active", cut.Markup);
        Assert.Contains("app-badge-success", cut.Markup);
    }

    [Fact]
    public void Renders_Open_Rename_And_Unregister_Buttons()
    {
        var project = NewProject("alpha", "/tmp/alpha");

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        Assert.Contains("Open", cut.Markup);
        Assert.Contains("Rename", cut.Markup);
        Assert.Contains("Unregister", cut.Markup);
        Assert.Contains("data-testid=\"open-project\"", cut.Markup);
        Assert.Contains("data-testid=\"rename-project\"", cut.Markup);
        Assert.Contains("data-testid=\"unregister-project\"", cut.Markup);
    }

    [Fact]
    public void Open_Button_Remains_Disabled_In_M3_2()
    {
        var project = NewProject("alpha", "/tmp/alpha");

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        var open = cut.Find("[data-testid='open-project']");
        Assert.True(open.HasAttribute("disabled"));
    }

    [Fact]
    public void Rename_Button_Is_Enabled_In_M3_2()
    {
        var project = NewProject("alpha", "/tmp/alpha");

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        var rename = cut.Find("[data-testid='rename-project']");
        Assert.False(rename.HasAttribute("disabled"));
    }

    [Fact]
    public void Unregister_Button_Is_Enabled_In_M3_2()
    {
        var project = NewProject("alpha", "/tmp/alpha");

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        var unregister = cut.Find("[data-testid='unregister-project']");
        Assert.False(unregister.HasAttribute("disabled"));
    }

    [Fact]
    public async Task Clicking_Rename_Invokes_OnRename()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        var renameCount = 0;
        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project)
            .Add(p => p.OnRename, () => { renameCount++; return Task.CompletedTask; }));

        cut.Find("[data-testid='rename-project']").Click();

        Assert.Equal(1, renameCount);
    }

    [Fact]
    public async Task Clicking_Unregister_Invokes_OnUnregister()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        var unregisterCount = 0;
        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project)
            .Add(p => p.OnUnregister, () => { unregisterCount++; return Task.CompletedTask; }));

        cut.Find("[data-testid='unregister-project']").Click();

        Assert.Equal(1, unregisterCount);
    }

    private static Project NewProject(string name, string path) =>
        new(Guid.NewGuid(), name, path, new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero));
}
