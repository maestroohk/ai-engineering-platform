using AiEng.Platform.App.Components.Projects;
using AiEng.Platform.Application.Projects;
using AiEng.Platform.Domain.Projects;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Projects;

public class RenameProjectFormTests : BunitContext
{
    [Fact]
    public void Hidden_When_Visible_Is_False()
    {
        Services.AddSingleton<IProjectService>(new StaticService());
        var project = NewProject("alpha", "/tmp/alpha");
        var cut = Render<RenameProjectForm>(parameters => parameters
            .Add(p => p.Visible, false)
            .Add(p => p.Project, project));

        Assert.DoesNotContain("data-testid=\"rename-project-modal\"", cut.Markup);
    }

    [Fact]
    public void Renders_Modal_With_Path_And_Name_Field()
    {
        Services.AddSingleton<IProjectService>(new StaticService());
        var project = NewProject("alpha", "/tmp/alpha");
        var cut = Render<RenameProjectForm>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.Project, project));

        Assert.Contains("data-testid=\"rename-project-modal\"", cut.Markup);
        Assert.Contains("data-testid=\"rename-project-name\"", cut.Markup);
        Assert.Contains("data-testid=\"rename-project-submit\"", cut.Markup);
        Assert.Contains("data-testid=\"rename-project-cancel\"", cut.Markup);
        Assert.Contains("/tmp/alpha", cut.Markup);
    }

    [Fact]
    public void Name_Field_Is_Prefilled_With_Project_Name()
    {
        Services.AddSingleton<IProjectService>(new StaticService());
        var project = NewProject("alpha", "/tmp/alpha");
        var cut = Render<RenameProjectForm>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.Project, project));

        var input = cut.Find("[data-testid='rename-project-name']");
        Assert.Equal("alpha", input.GetAttribute("value"));
    }

    [Fact]
    public void Submit_Is_Disabled_When_NewName_Equals_Current()
    {
        Services.AddSingleton<IProjectService>(new StaticService());
        var project = NewProject("alpha", "/tmp/alpha");
        var cut = Render<RenameProjectForm>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.Project, project));

        var submit = cut.Find("[data-testid='rename-project-submit']");
        Assert.True(submit.HasAttribute("disabled"));
    }

    [Fact]
    public void Submit_Is_Enabled_When_NewName_Differs_From_Current()
    {
        Services.AddSingleton<IProjectService>(new StaticService());
        var project = NewProject("alpha", "/tmp/alpha");
        var cut = Render<RenameProjectForm>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.Project, project));

        cut.Find("[data-testid='rename-project-name']").Input("alpha-renamed");

        var submit = cut.Find("[data-testid='rename-project-submit']");
        Assert.False(submit.HasAttribute("disabled"));
    }

    [Fact]
    public async Task Submit_Calls_RenameAsync_And_Invokes_OnRenamed_On_Success()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        var service = new StaticService();
        service.RenameResultFor = (id, newName) =>
        {
            var renamed = NewProject(newName, project.Path);
            return Result<Project>.Success(renamed);
        };
        Services.AddSingleton<IProjectService>(service);

        Project? observed = null;
        var cut = Render<RenameProjectForm>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.Project, project)
            .Add(p => p.OnRenamed, (Project p) => { observed = p; return Task.CompletedTask; }));

        cut.Find("[data-testid='rename-project-name']").Input("alpha-renamed");
        cut.Find("[data-testid='rename-project-submit']").Click();

        cut.WaitForState(() => observed is not null);

        Assert.NotNull(observed);
        Assert.Equal("alpha-renamed", observed!.Name);
        Assert.Equal(project.Id, service.LastRenameId);
        Assert.Equal("alpha-renamed", service.LastRenameName);
    }

    [Fact]
    public async Task Submit_Shows_Validation_Error_When_Service_Returns_Failure()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        var service = new StaticService
        {
            RenameResult = Result<Project>.Failure(ValidationError.NotFound("Project", project.Id)),
        };
        Services.AddSingleton<IProjectService>(service);

        var cut = Render<RenameProjectForm>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.Project, project));

        cut.Find("[data-testid='rename-project-name']").Input("alpha-renamed");
        cut.Find("[data-testid='rename-project-submit']").Click();

        cut.WaitForState(() => cut.Markup.Contains("data-testid=\"rename-project-error\""));

        Assert.Contains("data-testid=\"rename-project-error\"", cut.Markup);
    }

    [Fact]
    public void Cancel_Button_Invokes_OnCancel()
    {
        Services.AddSingleton<IProjectService>(new StaticService());
        var project = NewProject("alpha", "/tmp/alpha");
        var cancelCount = 0;
        var cut = Render<RenameProjectForm>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.Project, project)
            .Add(p => p.OnCancel, () => { cancelCount++; return Task.CompletedTask; }));

        cut.Find("[data-testid='rename-project-cancel']").Click();

        Assert.Equal(1, cancelCount);
    }

    private static Project NewProject(string name, string path) =>
        new(Guid.NewGuid(), name, path, new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero));

    private sealed class StaticService : IProjectService
    {
        public Guid LastRenameId { get; private set; }

        public string? LastRenameName { get; private set; }

        public Result<Project>? RenameResult { get; set; }

        public Func<Guid, string, Result<Project>>? RenameResultFor { get; set; }

        public Task<Result<Project>> RegisterAsync(string name, string path, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<Project>> ListAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Project>>(Array.Empty<Project>());

        public Task<Project?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<Project?>(null);

        public Task<Result<Project>> RenameAsync(Guid id, string newName, CancellationToken cancellationToken = default)
        {
            LastRenameId = id;
            LastRenameName = newName;
            if (RenameResultFor is not null)
            {
                return Task.FromResult(RenameResultFor(id, newName));
            }
            if (RenameResult is not null)
            {
                return Task.FromResult(RenameResult);
            }
            return Task.FromResult(Result<Project>.Failure(ValidationError.NotFound("Project", id)));
        }

        public Task<Result<Project>> UnregisterAsync(Guid id, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }
}
