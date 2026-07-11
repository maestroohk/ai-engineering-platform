using AiEng.Platform.App.Components.Projects;
using AiEng.Platform.Application.Projects;
using AiEng.Platform.Domain.Projects;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Projects;

public class ConfirmUnregisterProjectTests : BunitContext
{
    [Fact]
    public void Hidden_When_Visible_Is_False()
    {
        Services.AddSingleton<IProjectService>(new StaticService());
        var project = NewProject("alpha", "/tmp/alpha");
        var cut = Render<ConfirmUnregisterProject>(parameters => parameters
            .Add(p => p.Visible, false)
            .Add(p => p.Project, project));

        Assert.DoesNotContain("data-testid=\"unregister-project-modal\"", cut.Markup);
    }

    [Fact]
    public void Renders_Project_Name_And_Path()
    {
        Services.AddSingleton<IProjectService>(new StaticService());
        var project = NewProject("alpha", "/tmp/alpha");
        var cut = Render<ConfirmUnregisterProject>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.Project, project));

        Assert.Contains("data-testid=\"unregister-project-modal\"", cut.Markup);
        Assert.Contains("alpha", cut.Markup);
        Assert.Contains("/tmp/alpha", cut.Markup);
        Assert.Contains("data-testid=\"unregister-project-cancel\"", cut.Markup);
        Assert.Contains("data-testid=\"unregister-project-confirm\"", cut.Markup);
    }

    [Fact]
    public void Cancel_Button_Invokes_OnCancel()
    {
        Services.AddSingleton<IProjectService>(new StaticService());
        var project = NewProject("alpha", "/tmp/alpha");
        var cancelCount = 0;
        var cut = Render<ConfirmUnregisterProject>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.Project, project)
            .Add(p => p.OnCancel, () => { cancelCount++; return Task.CompletedTask; }));

        cut.Find("[data-testid='unregister-project-cancel']").Click();

        Assert.Equal(1, cancelCount);
    }

    [Fact]
    public async Task Confirm_Calls_UnregisterAsync_And_Invokes_OnUnregistered_On_Success()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        var service = new StaticService { UnregisterResult = Result<Project>.Success(project) };
        Services.AddSingleton<IProjectService>(service);

        Project? observed = null;
        var cut = Render<ConfirmUnregisterProject>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.Project, project)
            .Add(p => p.OnUnregistered, (Project p) => { observed = p; return Task.CompletedTask; }));

        cut.Find("[data-testid='unregister-project-confirm']").Click();

        cut.WaitForState(() => observed is not null);

        Assert.NotNull(observed);
        Assert.Equal(project.Id, service.LastUnregisterId);
    }

    [Fact]
    public async Task Confirm_Shows_Not_Found_Error_When_Service_Returns_Failure()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        var service = new StaticService
        {
            UnregisterResult = Result<Project>.Failure(ValidationError.NotFound("Project", project.Id)),
        };
        Services.AddSingleton<IProjectService>(service);

        var cut = Render<ConfirmUnregisterProject>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.Project, project));

        cut.Find("[data-testid='unregister-project-confirm']").Click();

        cut.WaitForState(() => cut.Markup.Contains("data-testid=\"unregister-project-error\""));

        Assert.Contains("data-testid=\"unregister-project-error\"", cut.Markup);
    }

    private static Project NewProject(string name, string path) =>
        new(Guid.NewGuid(), name, path, new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero));

    private sealed class StaticService : IProjectService
    {
        public Guid LastUnregisterId { get; private set; }

        public Result<Project>? UnregisterResult { get; set; }

        public Task<Result<Project>> RegisterAsync(string name, string path, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<Project>> ListAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Project>>(Array.Empty<Project>());

        public Task<Project?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<Project?>(null);

        public Task<Result<Project>> RenameAsync(Guid id, string newName, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Result<Project>> UnregisterAsync(Guid id, CancellationToken cancellationToken = default)
        {
            LastUnregisterId = id;
            if (UnregisterResult is not null)
            {
                return Task.FromResult(UnregisterResult);
            }
            return Task.FromResult(Result<Project>.Failure(ValidationError.NotFound("Project", id)));
        }
    }
}
