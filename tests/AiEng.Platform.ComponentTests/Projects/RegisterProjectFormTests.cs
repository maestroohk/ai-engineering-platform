using AiEng.Platform.App.Components.Projects;
using AiEng.Platform.Application.Projects;
using AiEng.Platform.Domain.Projects;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Projects;

public class RegisterProjectFormTests : BunitContext
{
    [Fact]
    public void Hidden_When_Visible_Is_False()
    {
        Services.AddSingleton<IProjectService>(new StaticService());
        var cut = Render<RegisterProjectForm>(parameters => parameters
            .Add(p => p.Visible, false));

        Assert.DoesNotContain("data-testid=\"register-project-modal\"", cut.Markup);
    }

    [Fact]
    public void Renders_Modal_With_Name_And_Path_Fields()
    {
        Services.AddSingleton<IProjectService>(new StaticService());
        var cut = Render<RegisterProjectForm>(parameters => parameters
            .Add(p => p.Visible, true));

        Assert.Contains("data-testid=\"register-project-modal\"", cut.Markup);
        Assert.Contains("data-testid=\"register-project-name\"", cut.Markup);
        Assert.Contains("data-testid=\"register-project-path\"", cut.Markup);
        Assert.Contains("data-testid=\"register-project-submit\"", cut.Markup);
        Assert.Contains("data-testid=\"register-project-cancel\"", cut.Markup);
    }

    [Fact]
    public void Submit_Is_Disabled_When_Name_Or_Path_Is_Empty()
    {
        Services.AddSingleton<IProjectService>(new StaticService());
        var cut = Render<RegisterProjectForm>(parameters => parameters
            .Add(p => p.Visible, true));

        var submit = cut.Find("[data-testid='register-project-submit']");
        Assert.True(submit.HasAttribute("disabled"));
    }

    [Fact]
    public void Submit_Is_Enabled_When_Name_And_Path_Are_NonEmpty()
    {
        Services.AddSingleton<IProjectService>(new StaticService());
        var cut = Render<RegisterProjectForm>(parameters => parameters
            .Add(p => p.Visible, true));

        cut.Find("[data-testid='register-project-name']").Input("alpha");
        cut.Find("[data-testid='register-project-path']").Input("/tmp/alpha");

        var submit = cut.Find("[data-testid='register-project-submit']");
        Assert.False(submit.HasAttribute("disabled"));
    }

    [Fact]
    public async Task Submit_Calls_RegisterAsync_And_Invokes_OnRegistered_On_Success()
    {
        var dir = CreateTempDirectory();
        try
        {
            var service = new StaticService();
            Services.AddSingleton<IProjectService>(service);

            Project? registered = null;
            var cut = Render<RegisterProjectForm>(parameters => parameters
                .Add(p => p.Visible, true)
                .Add(p => p.OnRegistered, (Project p) => { registered = p; return Task.CompletedTask; }));

            cut.Find("[data-testid='register-project-name']").Input("alpha");
            cut.Find("[data-testid='register-project-path']").Input(dir);
            cut.Find("[data-testid='register-project-submit']").Click();

            cut.WaitForState(() => registered is not null);

            Assert.NotNull(registered);
            Assert.Equal("alpha", registered!.Name);
            Assert.Equal(dir, registered.Path);
            Assert.Equal("alpha", service.LastRegisterName);
            Assert.Equal(dir, service.LastRegisterPath);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public async Task Submit_Shows_Validation_Error_When_Service_Returns_Failure()
    {
        var service = new StaticService
        {
            RegisterResult = Result<Project>.Failure(ValidationError.Required("name")),
        };
        Services.AddSingleton<IProjectService>(service);

        var dir = CreateTempDirectory();
        try
        {
            var cut = Render<RegisterProjectForm>(parameters => parameters
                .Add(p => p.Visible, true));

            cut.Find("[data-testid='register-project-name']").Input("alpha");
            cut.Find("[data-testid='register-project-path']").Input(dir);
            cut.Find("[data-testid='register-project-submit']").Click();

            cut.WaitForState(() => cut.Markup.Contains("data-testid=\"register-project-error\""));

            Assert.Contains("data-testid=\"register-project-error\"", cut.Markup);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public void Cancel_Button_Invokes_OnCancel()
    {
        Services.AddSingleton<IProjectService>(new StaticService());
        var cancelCount = 0;
        var cut = Render<RegisterProjectForm>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.OnCancel, () => { cancelCount++; return Task.CompletedTask; }));

        cut.Find("[data-testid='register-project-cancel']").Click();

        Assert.Equal(1, cancelCount);
    }

    private static string CreateTempDirectory()
    {
        var dir = Path.Combine(Path.GetTempPath(), "aieng-register-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    private sealed class StaticService : IProjectService
    {
        public string? LastRegisterName { get; private set; }

        public string? LastRegisterPath { get; private set; }

        public Result<Project>? RegisterResult { get; set; }

        public Task<Result<Project>> RegisterAsync(string name, string path, CancellationToken cancellationToken = default)
        {
            LastRegisterName = name;
            LastRegisterPath = path;
            if (RegisterResult is not null)
            {
                return Task.FromResult(RegisterResult);
            }
            if (!Directory.Exists(path))
            {
                return Task.FromResult(Result<Project>.Failure(ValidationError.InvalidPath("path", path)));
            }
            var project = new Project(Guid.NewGuid(), name.Trim(), path, DateTimeOffset.UtcNow);
            return Task.FromResult(Result<Project>.Success(project));
        }

        public Task<IReadOnlyList<Project>> ListAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Project>>(Array.Empty<Project>());

        public Task<Project?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<Project?>(null);

        public Task<Result<Project>> RenameAsync(Guid id, string newName, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Result<Project>> UnregisterAsync(Guid id, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }
}
